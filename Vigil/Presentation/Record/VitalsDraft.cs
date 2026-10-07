using System.Globalization;

namespace Vigil.Presentation;

/// <summary>
/// The Monitor pad. Values stay text while typing ("37." is fine); steppers and Record parse them.
/// Pre-filled from the last reading so an unchanged patient is one tap: Record.
/// </summary>
public partial record VitalsDraft(
    string Hr,
    string Rr,
    string SpO2,
    string EtCo2,
    string Sys,
    string Dia,
    string Map,
    string TempC,
    string Vaporizer,
    string O2,
    int PlaneIndex,
    string Note)
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    public static VitalsDraft Empty { get; } = new("", "", "", "", "", "", "", "", "", "", 1, "");

    public static VitalsDraft From(VitalsReading? r) => r is null ? Empty : new(
        I(r.Hr), I(r.Rr), I(r.SpO2), I(r.EtCo2), I(r.Sys), I(r.Dia), I(r.Map), D(r.TempC, "0.0"), D(r.VaporizerPct, "0.##"), D(r.O2LMin, "0.##"),
        (int)r.Plane, "");

    /// <summary>Applies a stepper tap. <paramref name="arg"/> is "Field|+" or "Field|-".</summary>
    public VitalsDraft Step(string arg)
    {
        var parts = arg.Split('|');
        if (parts.Length != 2)
        {
            return this;
        }
        var sign = parts[1] == "-" ? -1 : 1;
        return parts[0] switch
        {
            nameof(Hr) => this with { Hr = StepInt(Hr, 2 * sign, 0, 400, 80) },
            nameof(Rr) => this with { Rr = StepInt(Rr, sign, 0, 120, 12) },
            nameof(SpO2) => this with { SpO2 = StepInt(SpO2, sign, 0, 100, 98) },
            nameof(EtCo2) => this with { EtCo2 = StepInt(EtCo2, sign, 0, 150, 40) },
            nameof(Sys) => this with { Sys = StepInt(Sys, 5 * sign, 0, 350, 110) },
            nameof(Dia) => this with { Dia = StepInt(Dia, 5 * sign, 0, 300, 60) },
            nameof(Map) => this with { Map = StepInt(Map, 5 * sign, 0, 300, 80) },
            nameof(TempC) => this with { TempC = StepDec(TempC, 0.1m * sign, 25m, 45m, 37.5m, "0.0") },
            nameof(Vaporizer) => this with { Vaporizer = StepDec(Vaporizer, 0.25m * sign, 0m, 8m, 2m, "0.##") },
            nameof(O2) => this with { O2 = StepDec(O2, 0.5m * sign, 0m, 15m, 1m, "0.##") },
            _ => this,
        };
    }

    public IImmutableList<string> Validate()
    {
        var errors = ImmutableList.CreateBuilder<string>();
        void Int(string label, string text)
        {
            if (!string.IsNullOrWhiteSpace(text) && ParseInt(text) is null)
            {
                errors.Add(Loc.F("Validation_WholeNumber", "{0} must be a whole number.", label));
            }
        }
        void Dec(string label, string text)
        {
            if (!string.IsNullOrWhiteSpace(text) && ParseDec(text) is null)
            {
                errors.Add(Loc.F("Validation_Number", "{0} must be a number.", label));
            }
        }
        Int(Loc.T("Vital_HeartRate", "Heart rate"), Hr);
        Int(Loc.T("Vital_RespiratoryRate", "Respiratory rate"), Rr);
        Int("SpO₂", SpO2);
        Int("EtCO₂", EtCo2);
        Int(Loc.T("Vital_Systolic", "Systolic"), Sys);
        Int(Loc.T("Vital_Diastolic", "Diastolic"), Dia);
        Int("MAP", Map);
        Dec(Loc.T("Vital_Temperature", "Temperature"), TempC);
        Dec(Loc.T("Vital_Vaporizer", "Vaporizer"), Vaporizer);
        Dec(Loc.T("Vital_O2Flow", "O₂ flow"), O2);
        if (errors.Count == 0 && new[] { Hr, Rr, SpO2, EtCo2, Sys, Dia, Map, TempC }.All(string.IsNullOrWhiteSpace))
        {
            errors.Add(Loc.T("Validation_OneVital", "Enter at least one vital sign."));
        }
        return errors.ToImmutable();
    }

    public VitalsReading ToReading(DateTimeOffset at) => new(
        at, ParseInt(Hr), ParseInt(Rr), ParseInt(SpO2), ParseInt(EtCo2), ParseInt(Sys), ParseInt(Dia), ParseInt(Map),
        ParseDec(TempC), ParseDec(Vaporizer), ParseDec(O2), (Plane)Math.Clamp(PlaneIndex, 0, 2), Note.Trim());

    public static int? ParseInt(string text) =>
        int.TryParse(text.Trim(), NumberStyles.Integer, Inv, out var v) ? v : null;

    public static decimal? ParseDec(string text) =>
        decimal.TryParse(text.Trim().Replace(',', '.'), NumberStyles.Number, Inv, out var v) ? v : null;

    private static string I(int? v) => v?.ToString(Inv) ?? "";

    private static string D(decimal? v, string format) => v?.ToString(format, Inv) ?? "";

    /// <summary>A blank field starts at a typical value on its first tap; after that each tap moves one step.</summary>
    private static string StepInt(string text, int delta, int min, int max, int start) =>
        Math.Clamp(ParseInt(text) is int v ? v + delta : start, min, max).ToString(Inv);

    private static string StepDec(string text, decimal delta, decimal min, decimal max, decimal start, string format) =>
        Math.Clamp(ParseDec(text) is decimal v ? v + delta : start, min, max).ToString(format, Inv);
}

/// <summary>Out-of-range flags for the pad, as text ("▲ High" / "▼ Low"): status is never colour alone.</summary>
public partial record DraftFlags(string Hr, string Rr, string SpO2, string EtCo2, string Sys, string Dia, string Map, string TempC, string MapHint)
{
    public static DraftFlags None { get; } = new("", "", "", "", "", "", "", "", "");

    public static DraftFlags From(VitalsDraft d, Species species)
    {
        string F(VitalKind kind, decimal? v) => Vitals.Classify(species, kind, v) switch
        {
            RangeClass.High => Loc.T("Flag_High", "▲ High"),
            RangeClass.Low => Loc.T("Flag_Low", "▼ Low"),
            _ => "",
        };
        var sys = VitalsDraft.ParseInt(d.Sys);
        var dia = VitalsDraft.ParseInt(d.Dia);
        var map = VitalsDraft.ParseInt(d.Map) ?? Vitals.DeriveMap(sys, dia);
        var hint = VitalsDraft.ParseInt(d.Map) is null && map is int m ? Loc.F("Map_CalcHint", "calc {0}", m) : "";
        return new DraftFlags(
            F(VitalKind.Hr, VitalsDraft.ParseInt(d.Hr)),
            F(VitalKind.Rr, VitalsDraft.ParseInt(d.Rr)),
            F(VitalKind.SpO2, VitalsDraft.ParseInt(d.SpO2)),
            F(VitalKind.EtCo2, VitalsDraft.ParseInt(d.EtCo2)),
            F(VitalKind.Sys, sys),
            F(VitalKind.Dia, dia),
            F(VitalKind.Map, map),
            F(VitalKind.TempC, VitalsDraft.ParseDec(d.TempC)),
            hint);
    }
}
