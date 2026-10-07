using Windows.UI;

namespace Vigil.Controls;

/// <summary>
/// Resolves a theme colour for a specific element theme. Skia drawing has no {ThemeResource}, so the strip
/// takes a snapshot of its colours on load and on every ActualThemeChanged (never through a converter: a
/// converter snapshots the app theme once and is never re-asked).
/// </summary>
internal static class ThemePalette
{
    public static Color Resolve(string key, ElementTheme theme, Color fallback)
    {
        var themeKey = theme == ElementTheme.Dark ? "Dark" : "Light";
        return Find(Application.Current.Resources, key, themeKey) ?? fallback;
    }

    private static Color? Find(ResourceDictionary dictionary, string key, string themeKey)
    {
        foreach (var name in new[] { themeKey, themeKey == "Light" ? "Default" : "Dark" })
        {
            if (dictionary.ThemeDictionaries.TryGetValue(name, out var t) && t is ResourceDictionary td && Value(td, key) is Color c)
            {
                return c;
            }
        }
        if (Value(dictionary, key) is Color direct)
        {
            return direct;
        }
        foreach (var merged in dictionary.MergedDictionaries)
        {
            if (Find(merged, key, themeKey) is Color found)
            {
                return found;
            }
        }
        return null;
    }

    private static Color? Value(ResourceDictionary d, string key) =>
        d.TryGetValue(key, out var v) ? v switch
        {
            Color c => c,
            SolidColorBrush b => b.Color,
            _ => null,
        } : null;
}
