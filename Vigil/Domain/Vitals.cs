namespace Vigil.Domain;

public enum RangeClass { Low, Normal, High }

public enum VitalKind { Hr, Rr, SpO2, EtCo2, Sys, Dia, Map, TempC }

// A class, not a record struct: the MVUX bindable generator cannot proxy record structs (CS0019/CS0023).
public sealed record VitalRange(decimal Min, decimal Max)
{
    public RangeClass Classify(decimal value) => value < Min ? RangeClass.Low : value > Max ? RangeClass.High : RangeClass.Normal;
}

/// <summary>
/// Alarm thresholds under general anesthesia. Sample reference data, labelled as such in the app:
/// a practice would supply its own.
/// </summary>
public static class Vitals
{
    public static int? DeriveMap(int? sys, int? dia) =>
        sys is int s && dia is int d ? (int)Math.Round((s + 2m * d) / 3m, MidpointRounding.AwayFromZero) : null;

    public static VitalRange RangeFor(Species species, VitalKind kind) => (species, kind) switch
    {
        (Species.Feline, VitalKind.Hr) => new(100, 200),
        (_, VitalKind.Hr) => new(60, 140),
        (Species.Feline, VitalKind.Rr) => new(8, 30),
        (_, VitalKind.Rr) => new(8, 30),
        (_, VitalKind.SpO2) => new(95, 100),
        (_, VitalKind.EtCo2) => new(35, 45),
        (_, VitalKind.Sys) => new(90, 160),
        (_, VitalKind.Dia) => new(40, 100),
        (_, VitalKind.Map) => new(60, 110),
        (_, VitalKind.TempC) => new(36.5m, 39.5m),
        _ => new(decimal.MinValue, decimal.MaxValue),
    };

    public static RangeClass Classify(Species species, VitalKind kind, decimal? value) =>
        value is decimal v ? RangeFor(species, kind).Classify(v) : RangeClass.Normal;

    /// <summary>Every out-of-range value in a reading, in pad order.</summary>
    public static IReadOnlyList<(VitalKind Kind, RangeClass Class)> Alarms(Species species, VitalsReading r)
    {
        var values = new (VitalKind, decimal?)[]
        {
            (VitalKind.Hr, r.Hr), (VitalKind.Rr, r.Rr), (VitalKind.SpO2, r.SpO2), (VitalKind.EtCo2, r.EtCo2),
            (VitalKind.Sys, r.Sys), (VitalKind.Dia, r.Dia), (VitalKind.Map, r.EffectiveMap), (VitalKind.TempC, r.TempC),
        };
        return values
            .Select(v => (v.Item1, Classify(species, v.Item1, v.Item2)))
            .Where(v => v.Item2 != RangeClass.Normal)
            .ToList();
    }

    public static string Label(VitalKind kind) => kind switch
    {
        VitalKind.Hr => "HR",
        VitalKind.Rr => "RR",
        VitalKind.SpO2 => "SpO₂",
        VitalKind.EtCo2 => "EtCO₂",
        VitalKind.Sys => "SYS",
        VitalKind.Dia => "DIA",
        VitalKind.Map => "MAP",
        VitalKind.TempC => "Temp",
        _ => kind.ToString(),
    };
}
