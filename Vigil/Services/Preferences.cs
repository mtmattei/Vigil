namespace Vigil.Services;

/// <summary>Practice preferences that change domain behaviour (the reading interval).</summary>
public interface IPreferences
{
    TimeSpan ReadingInterval { get; }
}

public sealed class Preferences : IPreferences
{
    public TimeSpan ReadingInterval { get; set; } = TimeSpan.FromMinutes(5);
}
