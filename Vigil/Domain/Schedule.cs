namespace Vigil.Domain;

public enum DueState { None, Waiting, Due, Overdue }

/// <summary>When the next vitals reading is due, from the last reading (or induction) and the interval.</summary>
public static class Schedule
{
    public static readonly TimeSpan DueWindow = TimeSpan.FromSeconds(30);

    public static DateTimeOffset? LastMark(Case c) =>
        c.Status != CaseStatus.Anesthetized ? null
        : c.Readings.Count > 0 ? c.Readings[^1].At
        : c.InducedAt;

    public static DateTimeOffset? NextDue(Case c, TimeSpan interval) => LastMark(c) is DateTimeOffset m ? m + interval : null;

    /// <summary>Positive while waiting, negative once overdue.</summary>
    public static TimeSpan? Remaining(Case c, TimeSpan interval, DateTimeOffset now) => NextDue(c, interval) is DateTimeOffset d ? d - now : null;

    public static DueState State(Case c, TimeSpan interval, DateTimeOffset now) => Remaining(c, interval, now) switch
    {
        null => DueState.None,
        TimeSpan r when r < TimeSpan.Zero => DueState.Overdue,
        TimeSpan r when r <= DueWindow => DueState.Due,
        _ => DueState.Waiting,
    };

    /// <summary>0 right after a reading, 1 when the next one is due (the due slot fills like a sight glass).</summary>
    public static double Progress(Case c, TimeSpan interval, DateTimeOffset now)
    {
        if (LastMark(c) is not DateTimeOffset m || interval <= TimeSpan.Zero)
        {
            return 0;
        }
        return Math.Clamp((now - m).TotalSeconds / interval.TotalSeconds, 0, 1);
    }

    public static TimeSpan Elapsed(Case c, DateTimeOffset now) =>
        c.InducedAt is DateTimeOffset i ? (c.EndedAt ?? now) - i : TimeSpan.Zero;

    public static string Clock(TimeSpan t)
    {
        var neg = t < TimeSpan.Zero;
        t = t.Duration();
        var s = t.TotalHours >= 1 ? $"{(int)t.TotalHours:00}:{t.Minutes:00}:{t.Seconds:00}" : $"{t.Minutes:0}:{t.Seconds:00}";
        return neg ? "-" + s : s;
    }

    /// <summary>Elapsed anesthesia time as hh:mm (the board's display value).</summary>
    public static string HoursMinutes(TimeSpan t) => $"{(int)t.TotalHours:00}:{t.Minutes:00}";
}
