using System.Collections.Concurrent;
using RepoDb.Contexts.Execution;

namespace RepoDb.Contexts.Cachers;

/// <summary>
/// A class that is being used to cache the execution context of the MergeAll operation.
/// </summary>
internal static class MergeAllExecutionContextCache
{
    private static readonly ConcurrentDictionary<string, MergeAllExecutionContext> cache = new();

    /// <summary>
    /// Flushes all the cached execution context.
    /// </summary>
    public static void Flush() =>
        cache.Clear();

    /// <summary>
    ///
    /// </summary>
    /// <param name="key"></param>
    /// <param name="context"></param>
    internal static void Add(string key,
        MergeAllExecutionContext context) =>
        cache.TryAdd(key, context);

    /// <summary>
    ///
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    internal static MergeAllExecutionContext? Get(string key)
    {
        if (cache.TryGetValue(key, out var result))
        {
            return result;
        }
        return null;
    }
}
