namespace Vigil.Presentation;

public partial record SignOffSummary(
    string Patient,
    string Procedure,
    string Duration,
    int Readings,
    int Doses,
    int AlarmReadings,
    int OutOfRangeDoses,
    string Recovery,
    string Veterinarian,
    bool CanSign,
    string Blocker)
{
    public string Counts =>
        $"{Plural(Doses, "dose")} · {OutOfRangeDoses} out of range, confirmed · {Plural(AlarmReadings, "reading")} with an alarm";

    private static string Plural(int n, string noun) => n == 1 ? $"1 {noun}" : $"{n} {noun}s";

    public static SignOffSummary From(Case c)
    {
        var alarms = c.Readings.Count(r => Vitals.Alarms(c.Patient.Species, r).Count > 0);
        var blocker = c.Status switch
        {
            CaseStatus.Recovery => "",
            CaseStatus.Signed => $"Already signed by {c.SignedBy}.",
            _ => "End anesthesia before signing.",
        };
        var r = c.Recovery;
        return new SignOffSummary(
            $"{c.Patient.Name} · {c.Patient.SpeciesAndBreed} · {c.Patient.WeightKg:0.0} kg · {c.Preop.AsaLabel}",
            c.Procedure,
            Schedule.HoursMinutes(Schedule.Elapsed(c, c.EndedAt ?? c.CreatedAt)),
            c.Readings.Count,
            c.Doses.Count,
            alarms,
            c.Doses.Count(d => d.OutOfRangeConfirmed),
            $"Extubated {r.ExtubatedAt?.ToString("HH:mm") ?? "—"} · sternal {r.SternalAt?.ToString("HH:mm") ?? "—"} · pain {r.PainScore?.ToString() ?? "—"}/4",
            c.Veterinarian,
            c.Status == CaseStatus.Recovery,
            blocker);
    }
}

/// <summary>The veterinarian's sign-off: a typed name and an explicit confirmation on a shared device (no authentication).</summary>
public partial record SignOffModel(CaseRef Ref, ICaseStore Store, IClock Clock, INavigator Navigator)
{
    public IFeed<SignOffSummary> Summary => Feed<SignOffSummary>.Async(async ct =>
        await Store.GetAsync(Ref.Id, ct) is { } c ? Option.Some(SignOffSummary.From(c)) : Option.None<SignOffSummary>(), Store.Changed);

    public IState<string> Veterinarian => State.Async(this, async ct => (await Store.GetAsync(Ref.Id, ct))?.Veterinarian ?? "");

    public IState<bool> Confirmed => State.Value(this, () => false);

    public IState<string> Error => State.Value(this, () => "");

    public void Retry() => Store.Reload();

    public async ValueTask Sign(string veterinarian, bool confirmed, CancellationToken ct)
    {
        if (!confirmed || string.IsNullOrWhiteSpace(veterinarian))
        {
            await Error.UpdateAsync(_ => "Enter the veterinarian's name and confirm the record is complete.", ct);
            return;
        }
        try
        {
            await Store.UpdateAsync(Ref.Id, c => c.Status == CaseStatus.Recovery
                ? c with { Status = CaseStatus.Signed, SignedBy = veterinarian.Trim(), SignedAt = Clock.Now }
                : throw new InvalidOperationException("End anesthesia before signing."), ct);
        }
        catch (Exception ex) when (ex is IOException or InvalidOperationException or KeyNotFoundException)
        {
            await Error.UpdateAsync(_ => $"Not signed: {ex.Message}", ct);
            return;
        }
        await Navigator.NavigateBackAsync(this, cancellation: ct);
    }

    public async ValueTask Cancel(CancellationToken ct) => await Navigator.NavigateBackAsync(this, cancellation: ct);
}
