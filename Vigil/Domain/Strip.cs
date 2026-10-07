namespace Vigil.Domain;

/// <summary>One reading on the strip, already placed in time (minutes since the strip origin).</summary>
public sealed record StripReading(double Minute, int? Hr, int? Sys, int? Dia, int? Map, int? SpO2, int? EtCo2, decimal? TempC, bool HasAlarm);

/// <summary>A dose on the drug row.</summary>
public sealed record StripDose(double Minute, string Label);

/// <summary>
/// Everything the five-minute strip draws, in plain numbers so the renderer needs no domain logic.
/// Columns are five minutes wide from <see cref="Origin"/> (induction, or the first reading).
/// </summary>
public sealed record StripData(
    DateTimeOffset Origin,
    IReadOnlyList<StripReading> Readings,
    IReadOnlyList<StripDose> Doses,
    double NowMinute,
    double IntervalMinutes,
    double DueProgress,
    DueState Due,
    bool Live,
    double DueMinute)
{
    public static StripData Empty { get; } = new(DateTimeOffset.MinValue, [], [], 0, 5, 0, DueState.None, false, 0);

    public static StripData From(Case c, DateTimeOffset now, TimeSpan interval)
    {
        var origin = c.InducedAt ?? (c.Readings.Count > 0 ? c.Readings[0].At : now);
        double M(DateTimeOffset t) => (t - origin).TotalMinutes;
        var readings = c.Readings
            .Select(r => new StripReading(M(r.At), r.Hr, r.Sys, r.Dia, r.EffectiveMap, r.SpO2, r.EtCo2, r.TempC,
                Vitals.Alarms(c.Patient.Species, r).Count > 0))
            .ToList();
        var doses = c.Doses
            .Where(d => d.At >= origin.AddMinutes(-60))
            .Select(d => new StripDose(Math.Max(M(d.At), 0), Abbreviate(d.DrugName)))
            .ToList();
        var end = c.EndedAt ?? now;
        var due = Schedule.NextDue(c, interval) is DateTimeOffset next ? M(next) : M(end);
        return new StripData(origin, readings, doses, M(end), interval.TotalMinutes,
            Schedule.Progress(c, interval, now), Schedule.State(c, interval, now), c.Status == CaseStatus.Anesthetized,
            // The slot sits where the next reading lands (at least the current column when overdue).
            Math.Max(due, M(end)));
    }

    /// <summary>Screen-reader summary standing in for the drawing.</summary>
    public string Summary(Species species)
    {
        if (Readings.Count == 0)
        {
            return "Vitals strip: no readings yet.";
        }
        var last = Readings[^1];
        var at = Origin.AddMinutes(last.Minute);
        return $"Vitals strip: {Readings.Count} readings, last at {at:HH:mm}: heart rate {last.Hr?.ToString() ?? "not recorded"}, " +
            $"blood pressure {last.Sys?.ToString() ?? "-"} over {last.Dia?.ToString() ?? "-"}, SpO2 {last.SpO2?.ToString() ?? "not recorded"}. " +
            "Use the Table view for every value.";
    }

    private static string Abbreviate(string name) => name.Length <= 5 ? name : name[..4];
}
