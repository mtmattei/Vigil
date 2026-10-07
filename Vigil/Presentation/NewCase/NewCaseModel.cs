using System.Globalization;

namespace Vigil.Presentation;

/// <summary>The new-case form. Numbers stay text while typing so partial input ("18.") is never rejected.</summary>
public partial record CaseDraft(
    string Name,
    int SpeciesIndex,
    string Breed,
    string WeightText,
    string AgeText,
    string Owner,
    string Procedure,
    string Veterinarian,
    string Technician)
{
    public static CaseDraft Empty { get; } = new("", 0, "", "", "", "", "", "", "");

    public Species Species => SpeciesIndex switch { 1 => Species.Feline, 2 => Species.Other, _ => Species.Canine };

    public decimal? WeightKg => ParseDecimal(WeightText);

    public decimal? AgeYears => ParseDecimal(AgeText);

    public IImmutableList<string> Validate()
    {
        var errors = ImmutableList.CreateBuilder<string>();
        if (string.IsNullOrWhiteSpace(Name))
        {
            errors.Add(Loc.T("NewCase_NameRequired", "Enter the patient's name."));
        }
        if (WeightKg is not decimal w || w is < 0.1m or > 150m)
        {
            errors.Add(Loc.T("NewCase_WeightRequired", "Enter a weight between 0.1 and 150 kg: every dose is calculated from it."));
        }
        if (!string.IsNullOrWhiteSpace(AgeText) && AgeYears is null)
        {
            errors.Add(Loc.T("NewCase_AgeNumber", "Age must be a number of years."));
        }
        if (string.IsNullOrWhiteSpace(Procedure))
        {
            errors.Add(Loc.T("NewCase_ProcedureRequired", "Enter the procedure."));
        }
        if (string.IsNullOrWhiteSpace(Veterinarian))
        {
            errors.Add(Loc.T("NewCase_VetRequired", "Enter the veterinarian."));
        }
        if (string.IsNullOrWhiteSpace(Technician))
        {
            errors.Add(Loc.T("NewCase_TechRequired", "Enter the technician running anesthesia."));
        }
        return errors.ToImmutable();
    }

    public Case ToCase(DateTimeOffset now) => Case.New(
        new Patient(Name.Trim(), Species, Breed.Trim(), Math.Round(WeightKg ?? 0, 1), AgeYears ?? 0, Owner.Trim()),
        Procedure.Trim(), Veterinarian.Trim(), Technician.Trim(), now);

    private static decimal? ParseDecimal(string text)
    {
        var t = text.Trim().Replace(',', '.');
        return decimal.TryParse(t, NumberStyles.Number, CultureInfo.InvariantCulture, out var v) ? v : null;
    }
}

public partial record NewCaseModel(ICaseStore Store, IClock Clock, INavigator Navigator)
{
    public IState<CaseDraft> Draft => State.Value(this, () => CaseDraft.Empty);

    /// <summary>Set by the first Create attempt so errors appear after the user tries, then update live.</summary>
    public IState<bool> Attempted => State.Value(this, () => false);

    public IFeed<string> ErrorText => Feed.Combine(Draft, Attempted)
        .Select(x => x.Item2 ? string.Join(Environment.NewLine, x.Item1.Validate()) : "");

    public IState<string> SaveError => State.Value(this, () => "");

    public async ValueTask Create(CaseDraft draft, CancellationToken ct)
    {
        await Attempted.UpdateAsync(_ => true, ct);
        if (draft.Validate().Count > 0)
        {
            return;
        }
        var created = draft.ToCase(Clock.Now);
        try
        {
            await Store.SaveAsync(created, ct);
        }
        catch (IOException ex)
        {
            await SaveError.UpdateAsync(_ => Loc.F("NewCase_SaveFailed", "The case could not be saved: {0} Your entries are kept; try again.", ex.Message), ct);
            return;
        }
        await Navigator.NavigateBackWithResultAsync(this, data: new CaseRef(created.Id), cancellation: ct);
    }

    public async ValueTask Cancel(CancellationToken ct) => await Navigator.NavigateBackAsync(this, cancellation: ct);
}
