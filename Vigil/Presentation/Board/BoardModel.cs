using System.Collections.Immutable;
using Vigil.Domain;
using Vigil.Services;

namespace Vigil.Presentation;

public enum BoardFilter { Today, All }

/// <summary>A case row on the board. Inherits <see cref="CaseRef"/> so a tap navigates with the row as data.</summary>
public partial record CaseSummary(
    Guid Id,
    string Name,
    string Signalment,
    string Procedure,
    string Team,
    CaseStatus Status,
    string StatusText,
    string TimeText) : CaseRef(Id)
{
    public bool IsActive => Status is CaseStatus.Anesthetized or CaseStatus.Recovery;

    public static CaseSummary From(Case c, DateTimeOffset now)
    {
        var p = c.Patient;
        var time = c.Status switch
        {
            CaseStatus.Anesthetized => Loc.F("Board_UnderTime", "Under {0}", Schedule.HoursMinutes(Schedule.Elapsed(c, now))),
            CaseStatus.Recovery => c.Recovery.ExtubatedAt is DateTimeOffset x ? Loc.F("Board_ExtubatedTime", "Extubated {0:HH:mm}", x) : Loc.F("Board_EndedTime", "Ended {0:HH:mm}", c.EndedAt),
            CaseStatus.Signed => Loc.F("Board_SignedTime", "Signed {0:HH:mm}", c.SignedAt),
            _ => Loc.F("Board_AddedTime", "Added {0:HH:mm}", c.CreatedAt),
        };
        return new CaseSummary(
            c.Id,
            p.Name,
            $"{p.SpeciesAndBreed} · {p.WeightKg:0.0} kg",
            c.Procedure,
            $"{c.Veterinarian} · {c.Technician}",
            c.Status,
            StatusLabel(c.Status),
            time);
    }

    public static string StatusLabel(CaseStatus s) => s switch
    {
        CaseStatus.Anesthetized => Loc.T("Status_Anesthetized", "Under anesthesia"),
        CaseStatus.Recovery => Loc.T("Status_Recovery", "Recovery"),
        CaseStatus.Signed => Loc.T("Status_Signed", "Signed"),
        _ => Loc.T("Status_Scheduled", "Scheduled"),
    };
}

/// <summary>The case under anesthesia right now, ticking once per second.</summary>
public partial record LiveCase(
    Guid Id,
    string Name,
    string Band,
    string Procedure,
    string Elapsed,
    string NextLabel,
    string NextText,
    DueState Due,
    double Progress,
    string LastReading,
    string Others) : CaseRef(Id)
{
    public bool HasOthers => Others.Length > 0;

    /// <summary>Visual state name for <c>utu:VisualStateManagerExtensions.States</c>.</summary>
    public string DueStateName => Due.ToString();

    public double ProgressPercent => Progress * 100;

    public static LiveCase? From(IImmutableList<Case> cases, DateTimeOffset now, TimeSpan interval)
    {
        var live = cases.Where(x => x.Status == CaseStatus.Anesthetized).ToList();
        if (live.Count == 0)
        {
            return null;
        }
        // The band follows the most recent induction; anyone else asleep is named so no patient is out of sight.
        var c = live[0];
        var others = live.Count > 1
            ? Loc.F("Board_AlsoUnder", "Also under anesthesia: {0}", string.Join(", ", live.Skip(1).Select(x => x.Patient.Name)))
            : "";
        var p = c.Patient;
        var remaining = Schedule.Remaining(c, interval, now) ?? TimeSpan.Zero;
        var due = Schedule.State(c, interval, now);
        var last = c.Readings.Count > 0 ? c.Readings[^1] : null;
        return new LiveCase(
            c.Id,
            p.Name,
            $"{Loc.Species(p.Species)} · {p.WeightKg:0.0} kg · {c.Preop.AsaLabel}",
            c.Procedure,
            Schedule.HoursMinutes(Schedule.Elapsed(c, now)),
            due == DueState.Overdue ? Loc.T("Due_Overdue", "Reading overdue") : Loc.T("Due_Next", "Next reading"),
            Schedule.Clock(remaining.Duration()),
            due,
            Schedule.Progress(c, interval, now),
            last is null ? Loc.T("Readings_None", "No readings yet") : Loc.F("Board_LastReading", "Last {0:HH:mm} · HR {1} · SpO₂ {2} · MAP {3}", last.At, last.Hr?.ToString() ?? "—", last.SpO2?.ToString() ?? "—", last.EffectiveMap?.ToString() ?? "—"),
            others);
    }
}

public partial record BoardModel(ICaseStore Store, IClock Clock, IPreferences Preferences, INavigator Navigator)
{
    // One shared source, reloaded by the store's change signal: the list and the live band fail and
    // recover together. Retry raises the same signal (FeedView.Refresh does not recover an upstream failure).
    private IFeed<IImmutableList<Case>> Cases { get; } = Feed.Async(async ct => (IImmutableList<Case>)await Store.GetAllAsync(ct), Store.Changed);

    private IFeed<DateTimeOffset> Now { get; } = Feed.AsyncEnumerable(Clock.Ticks);

    public IState<BoardFilter> Filter => State.Value(this, () => BoardFilter.Today);

    public IListFeed<CaseSummary> Today => Feed
        .Combine(Cases, Filter)
        .Select(x => (IImmutableList<CaseSummary>)x.Item1
            .Where(c => x.Item2 == BoardFilter.All || c.CreatedAt.Date == Clock.Now.Date || c.Status != CaseStatus.Signed)
            .Select(c => CaseSummary.From(c, Clock.Now))
            .ToImmutableList())
        .AsListFeed();

    public IFeed<LiveCase> Live => Feed
        .Combine(Cases, Now)
        .Select(x => LiveCase.From(x.Item1, x.Item2, Preferences.ReadingInterval)!);

    public void Retry() => Store.Reload();

    /// <summary>Opens the New case sheet; on Create, opens the new record with the board behind it (Back returns here).</summary>
    public async ValueTask NewCase(CancellationToken ct)
    {
        var result = await Navigator.NavigateRouteForResultAsync<CaseRef>(this, "!NewCase", cancellation: ct).AsResult();
        if (result.SomeOrDefault() is CaseRef created)
        {
            await Navigator.NavigateRouteAsync(this, "Record", data: created, cancellation: ct);
        }
    }

    public async ValueTask ShowAll(CancellationToken ct) => await Filter.UpdateAsync(_ => BoardFilter.All, ct);

    public async ValueTask ShowToday(CancellationToken ct) => await Filter.UpdateAsync(_ => BoardFilter.Today, ct);
}
