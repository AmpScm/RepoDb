using System.Collections.Concurrent;
using RepoDb.Resolvers;

namespace RepoDb;

/// <summary>
/// A class that is being used to cache the identity property of the data entity.
/// </summary>
public static class IdentityCache
{
    private static readonly ConcurrentDictionary<Type, ClassProperty?> cache = new();
    private static readonly IdentityResolver resolver = new();

    /// <summary>
    /// Gets the cached identity property of the data entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <returns>The cached identity property.</returns>
    public static ClassProperty? Get<TEntity>()
        where TEntity : class =>
        Get(typeof(TEntity));

    /// <summary>
    /// Gets the cached identity property of the data entity.
    /// </summary>
    /// <param name="entityType">The type of the data entity.</param>
    /// <returns>The cached identity property.</returns>
    public static ClassProperty? Get(Type entityType)
        => cache.GetOrAdd(entityType, resolver.Resolve);

    /// <summary>
    /// Flushes all the existing cached identity <see cref="ClassProperty"/> objects.
    /// </summary>
    public static void Flush() =>
        cache.Clear();
}
