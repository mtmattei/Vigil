namespace Vigil.Presentation;

/// <summary>Editable part of the recovery log (times are stamped by commands, not typed).</summary>
public partial record RecoveryDraft(int PainIndex, string TempC, string Notes)
{
    public static RecoveryDraft From(RecoveryLog r) =>
        new(r.PainScore is int p ? p + 1 : 0, r.TempC?.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture) ?? "", r.Notes);

    /// <summary>Applies the draft onto the stored log. PainIndex 0 = not scored, 1..5 = score 0..4.</summary>
    public RecoveryLog ApplyTo(RecoveryLog r) => r with
    {
        PainScore = PainIndex is >= 1 and <= 5 ? PainIndex - 1 : null,
        TempC = VitalsDraft.ParseDec(TempC),
        Notes = Notes.Trim(),
    };
}
