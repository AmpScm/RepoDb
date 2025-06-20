using System.Collections.Concurrent;
using RepoDb.Extensions;

namespace RepoDb;

/// <summary>
/// A class that is being used to cache the list of <see cref="Field"/> objects of the data entity.
/// </summary>
public static class FieldCache
{
    private static readonly ConcurrentDictionary<Type, FieldSet> cache = new();

    #region Methods

    /// <summary>
    /// Gets the cached list of <see cref="Field"/> objects of the data entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <returns>The cached list <see cref="Field"/> objects.</returns>
    public static FieldSet Get<TEntity>()
        where TEntity : class =>
        Get(typeof(TEntity));

    /// <summary>
    /// Gets the cached list of <see cref="Field"/> objects of the data entity.
    /// </summary>
    /// <param name="entityType">The type of the data entity.</param>
    /// <returns>The cached list <see cref="Field"/> objects.</returns>
    public static FieldSet Get(Type entityType)
    {
        if (!TypeCache.Get(entityType).IsClassType())
            throw new ArgumentOutOfRangeException(nameof(entityType));

        // Try get the value
        return cache.GetOrAdd(entityType, TypeExtension.AsFields);
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Flushes all the existing cached enumerable of <see cref="Field"/> objects.
    /// </summary>
    public static void Flush() =>
        cache.Clear();

    #endregion
}
