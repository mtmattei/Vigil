using System.Runtime.CompilerServices;

namespace Vigil.Services;

public interface IClock
{
    DateTimeOffset Now { get; }

    /// <summary>The current time now, then once per second.</summary>
    IAsyncEnumerable<DateTimeOffset> Ticks(CancellationToken ct);
}

public sealed class SystemClock : IClock
{
    public DateTimeOffset Now => DateTimeOffset.Now;

    public async IAsyncEnumerable<DateTimeOffset> Ticks([EnumeratorCancellation] CancellationToken ct)
    {
        yield return Now;
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
        while (await timer.WaitForNextTickAsync(ct))
        {
            yield return Now;
        }
    }
}
