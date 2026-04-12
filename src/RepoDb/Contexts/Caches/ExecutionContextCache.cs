using System.Collections.Concurrent;

namespace RepoDb.Contexts;

internal abstract class ExecutionContextCache<TKeyType, TContext>
    where TKeyType : struct, IEquatable<TKeyType>
    where TContext : class
{
    private static readonly ConcurrentDictionary<TKeyType, TContext> cache = new();

    /// <summary>
    /// Flushes all the cached execution context.
    /// </summary>
    public static void Flush() =>
        cache.Clear();

    internal static void Add(TKeyType key,
        TContext context) =>
        cache.TryAdd(key, context);

    internal static TContext? Get(TKeyType key)
    {
        return cache.TryGetValue(key, out var result) ? result : null;
    }

    internal static TContext GetOrAdd(TKeyType key, Func<TKeyType, TContext> creator)
        => cache.GetOrAdd(key, creator);
}
