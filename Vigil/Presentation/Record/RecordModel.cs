namespace Vigil.Presentation;

public partial record ReadingRow(string Time, string Hr, string Rr, string SpO2, string EtCo2, string Bp, string Map, string Temp, string Plane, string Alarms, string Note);

public partial record DoseRow(string Time, string Drug, string Dose, string Volume, string Route, string Flag);

/// <summary>Read-only projection of a case for the record page, refreshed by store changes and the 1 Hz clock.</summary>
public partial record CaseView(
    Guid Id,
    string Name,
    string Band,
    string Procedure,
    string Team,
    CaseStatus Status,
    string StatusText,
    decimal WeightKg,
    string WeightText,
    string Signalment,
    PreopCheck Preop,
    string PreopProgress,
    string InducedText,
    string Elapsed,
    string NextLabel,
    string NextText,
    string DueStateName,
    double ProgressPercent,
    IImmutableList<ReadingRow> Readings,
    IImmutableList<DoseRow> Doses,
    int ReadingCount) : CaseRef(Id)
{
    public bool IsScheduled => Status == CaseStatus.Scheduled;

    public bool IsAnesthetized => Status == CaseStatus.Anesthetized;

    public bool IsSigned => Status == CaseStatus.Signed;

    public bool CanInduce => IsScheduled && Preop.IsComplete;

    public const int MonitorTab = 2;

    public const int RecoveryTab = 3;

    /// <summary>Tab the record opens on: Monitor while the patient is asleep, Recovery after, Pre-op before.</summary>
    public static int TabFor(CaseStatus status) => status switch
    {
        CaseStatus.Anesthetized => MonitorTab,
        CaseStatus.Recovery or CaseStatus.Signed => RecoveryTab,
        _ => 0,
    };

    public static CaseView From(Case c, DateTimeOffset now, TimeSpan interval)
    {
        var p = c.Patient;
        var due = Schedule.State(c, interval, now);
        var remaining = Schedule.Remaining(c, interval, now) ?? TimeSpan.Zero;
        return new CaseView(
            c.Id,
            p.Name,
            $"{p.Species} · {p.WeightKg:0.0} kg · {c.Preop.AsaLabel}",
            c.Procedure,
            $"{c.Veterinarian} · {c.Technician}",
            c.Status,
            CaseSummary.StatusLabel(c.Status),
            p.WeightKg,
            $"{p.WeightKg:0.0}",
            string.Join(" · ", new[] { p.SpeciesAndBreed, p.AgeYears > 0 ? $"{p.AgeYears:0.#} y" : "", string.IsNullOrWhiteSpace(p.Owner) ? "" : $"owner {p.Owner}" }.Where(x => x.Length > 0)),
            c.Preop,
            $"{c.Preop.DoneCount} of 5 checks",
            c.InducedAt is DateTimeOffset i ? $"Induced at {i:HH:mm}" : "Not induced",
            Schedule.HoursMinutes(Schedule.Elapsed(c, now)),
            due switch { DueState.Overdue => "Reading overdue", DueState.None => "No reading due", _ => "Next reading" },
            due == DueState.None ? "—" : Schedule.Clock(remaining.Duration()),
            due.ToString(),
            Schedule.Progress(c, interval, now) * 100,
            c.Readings.Reverse().Select(r => Row(p.Species, r)).ToImmutableList(),
            c.Doses.Reverse().Select(DoseRowOf).ToImmutableList(),
            c.Readings.Count);
    }

    private static ReadingRow Row(Species species, VitalsReading r)
    {
        static string N(int? v) => v?.ToString() ?? "—";
        var alarms = Vitals.Alarms(species, r)
            .Select(a => $"{Vitals.Label(a.Kind)} {(a.Class == RangeClass.High ? "▲ high" : "▼ low")}");
        return new ReadingRow(
            $"{r.At:HH:mm}", N(r.Hr), N(r.Rr), N(r.SpO2), N(r.EtCo2),
            r.Sys is null && r.Dia is null ? "—" : $"{N(r.Sys)}/{N(r.Dia)}",
            r.EffectiveMap is int m ? (r.MapIsCalculated ? $"{m} calc" : $"{m}") : "—",
            r.TempC is decimal t ? $"{t:0.0}" : "—",
            r.Plane.ToString(),
            string.Join(" · ", alarms),
            r.Note);
    }

    private static DoseRow DoseRowOf(DoseGiven d) => new(
        $"{d.At:HH:mm}", d.DrugName, $"{d.MgPerKg:0.###} mg/kg", $"{d.VolumeMl:0.00} mL", d.Route.ToString(),
        d.OutOfRangeConfirmed ? "Out of range, confirmed" : "");
}

public partial record RecordModel(CaseRef Ref, ICaseStore Store, IClock Clock, IPreferences Preferences, INavigator Navigator)
{
    // The stored case; null (deleted or never existed) renders the None template.
    private IFeed<Case> Source { get; } = Feed<Case>.Async(
        async ct => await Store.GetAsync(Ref.Id, ct) is { } c ? Option.Some(c) : Option.None<Case>(), Store.Changed);

    private IFeed<DateTimeOffset> Now { get; } = Feed.AsyncEnumerable(Clock.Ticks);

    public IFeed<CaseView> Case => Feed.Combine(Source, Now).Select(x => CaseView.From(x.Item1, x.Item2, Preferences.ReadingInterval));

    public IFeed<string> Title => Source.Select(c => $"{c.Patient.Name} · {c.Patient.WeightKg:0.0} kg · {c.Procedure}");

    /// <summary>The pre-op checklist, edited in place and saved on every change while the case is scheduled.</summary>
    public IState<PreopCheck> Preop => State
        .Async(this, async ct => (await Store.GetAsync(Ref.Id, ct))?.Preop ?? PreopCheck.Empty)
        .ForEach(SavePreopAsync);

    /// <summary>The selected tab. Opens where the case is (Monitor while asleep); the user owns it after that.</summary>
    public IState<int> Tab => State.Async(this, async ct =>
        (await Store.GetAsync(Ref.Id, ct)) is { } c ? CaseView.TabFor(c.Status) : 0);

    /// <summary>Store write failures surface here (InfoBar) with the action to retry.</summary>
    public IState<string> SaveError => State.Value(this, () => "");

    public IFeed<bool> HasSaveError => SaveError.Select(s => !string.IsNullOrEmpty(s));

    public void Retry() => Store.Reload();

    public async ValueTask Induce(CancellationToken ct)
    {
        await SaveAsync(c => c.Status == CaseStatus.Scheduled
            ? c with { Status = CaseStatus.Anesthetized, InducedAt = Clock.Now }
            : c, ct);
        await Tab.UpdateAsync(_ => CaseView.MonitorTab, ct);
    }

    public async ValueTask DismissError(CancellationToken ct) => await SaveError.UpdateAsync(_ => "", ct);

    private async ValueTask SavePreopAsync(PreopCheck? preop, CancellationToken ct)
    {
        if (preop is null)
        {
            return;
        }
        await SaveAsync(c => c.Status == CaseStatus.Scheduled ? c with { Preop = preop } : c, ct);
    }

    private async ValueTask SaveAsync(Func<Case, Case> update, CancellationToken ct)
    {
        try
        {
            await Store.UpdateAsync(Ref.Id, update, ct);
            await SaveError.UpdateAsync(_ => "", ct);
        }
        catch (Exception ex) when (ex is IOException or SignedRecordException or KeyNotFoundException)
        {
            await SaveError.UpdateAsync(_ => $"Not saved: {ex.Message}", ct);
        }
    }
}
