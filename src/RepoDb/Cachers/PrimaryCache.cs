using System.Collections.Concurrent;
using RepoDb.Resolvers;

namespace RepoDb;

/// <summary>
/// A class that is being used to cache the primary property of the data entity.
/// </summary>
public static class PrimaryCache
{
    private static readonly ConcurrentDictionary<Type, IEnumerable<ClassProperty>?> cache = new();
    private static readonly PrimaryResolver resolver = new();

    /// <summary>
    /// Gets the cached primary property of the data entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <returns>The cached primary property.</returns>
    public static ClassProperty? Get<TEntity>() where TEntity : class
        => Get(typeof(TEntity));

    /// <summary>
    /// Gets the cached primary property of the data entity.
    /// </summary>
    /// <param name="entityType">The type of the data entity.</param>
    /// <returns>The cached primary property.</returns>
    public static ClassProperty? Get(Type entityType) => GetPrimaryKeys(entityType)?.FirstOrDefault();

    /// <summary>
    /// Gets the cached primary property of the data entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <returns>The cached primary property.</returns>
    public static IEnumerable<ClassProperty>? GetPrimaryKeys<TEntity>() where TEntity : class
        => GetPrimaryKeys(typeof(TEntity));

    /// <summary>
    /// Gets the cached primary property of the data entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <returns>The cached primary property.</returns>
    public static IEnumerable<ClassProperty>? GetPrimaryKeys(Type entityType)
        => cache.GetOrAdd(entityType, resolver.Resolve);

    /// <summary>
    /// Flushes all the existing cached primary <see cref="ClassProperty"/> objects.
    /// </summary>
    public static void Flush() =>
        cache.Clear();
}
