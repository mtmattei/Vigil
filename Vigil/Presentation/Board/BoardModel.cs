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
            CaseStatus.Anesthetized => $"Under {Schedule.HoursMinutes(Schedule.Elapsed(c, now))}",
            CaseStatus.Recovery => c.Recovery.ExtubatedAt is DateTimeOffset x ? $"Extubated {x:HH:mm}" : $"Ended {c.EndedAt:HH:mm}",
            CaseStatus.Signed => $"Signed {c.SignedAt:HH:mm}",
            _ => $"Added {c.CreatedAt:HH:mm}",
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
        CaseStatus.Anesthetized => "Under anesthesia",
        CaseStatus.Recovery => "Recovery",
        CaseStatus.Signed => "Signed",
        _ => "Scheduled",
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
    string LastReading) : CaseRef(Id)
{
    /// <summary>Visual state name for <c>utu:VisualStateManagerExtensions.States</c>.</summary>
    public string DueStateName => Due.ToString();

    public double ProgressPercent => Progress * 100;

    public static LiveCase? From(IImmutableList<Case> cases, DateTimeOffset now, TimeSpan interval)
    {
        if (cases.FirstOrDefault(c => c.Status == CaseStatus.Anesthetized) is not { } c)
        {
            return null;
        }
        var p = c.Patient;
        var remaining = Schedule.Remaining(c, interval, now) ?? TimeSpan.Zero;
        var due = Schedule.State(c, interval, now);
        var last = c.Readings.Count > 0 ? c.Readings[^1] : null;
        return new LiveCase(
            c.Id,
            p.Name,
            $"{p.Species} · {p.WeightKg:0.0} kg · {c.Preop.AsaLabel}",
            c.Procedure,
            Schedule.HoursMinutes(Schedule.Elapsed(c, now)),
            due == DueState.Overdue ? "Reading overdue" : "Next reading",
            Schedule.Clock(remaining.Duration()),
            due,
            Schedule.Progress(c, interval, now),
            last is null ? "No readings yet" : $"Last {last.At:HH:mm} · HR {last.Hr?.ToString() ?? "—"} · SpO₂ {last.SpO2?.ToString() ?? "—"} · MAP {last.EffectiveMap?.ToString() ?? "—"}");
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
