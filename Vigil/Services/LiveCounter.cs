#if DEBUG
using System.Collections.Concurrent;

namespace Vigil.Diagnostics;

/// <summary>DEBUG-only: counts live instances per owner type (finalizer decrements) for the memory loop.</summary>
internal sealed class LiveCounter
{
    public static readonly ConcurrentDictionary<string, int> Alive = new();

    private readonly string _name;

    public LiveCounter(string name)
    {
        _name = name;
        Alive.AddOrUpdate(name, 1, (_, n) => n + 1);
    }

    ~LiveCounter() => Alive.AddOrUpdate(_name, 0, (_, n) => n - 1);

    public static string Report() => string.Join(" ", Alive.Select(kv => $"{kv.Key}={kv.Value}"));
}
#endif
