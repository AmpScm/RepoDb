using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Resolvers;

namespace RepoDb;

/// <summary>
/// A class that is being used to cache the mapped-name of the property.
/// </summary>
public static class PropertyMappedNameCache
{
    #region Privates

    private static readonly ConcurrentDictionary<int, string> cache = new();
    private static readonly PropertyMappedNameResolver resolver = new();

    #endregion

    #region Methods

    /// <summary>
    /// Gets the cached column name mappings of the property (via expression).
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="expression">The expression to be parsed.</param>
    /// <returns>The cached column name mappings of the property.</returns>
    public static string Get<TEntity>(Expression<Func<TEntity, object?>> expression)
        where TEntity : class =>
        Get<TEntity>(ExpressionExtension.GetProperty(expression));

    /// <summary>
    /// Gets the cached column name mappings of the property (via property name).
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>The cached column name mappings of the property.</returns>
    public static string Get<TEntity>(string propertyName)
        where TEntity : class =>
        Get<TEntity>(TypeExtension.GetProperty<TEntity>(propertyName) ?? throw new PropertyNotFoundException(nameof(propertyName), "Property not found"));

    /// <summary>
    /// Gets the cached column name mappings of the property (via <see cref="Field"/> object).
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="field">The instance of <see cref="Field"/> object.</param>
    /// <returns>The cached column name mappings of the property.</returns>
    public static string Get<TEntity>(Field field)
        where TEntity : class =>
        Get<TEntity>((TypeExtension.GetProperty<TEntity>(field?.FieldName ?? throw new ArgumentNullException(nameof(field))) ?? throw new PropertyNotFoundException(nameof(field), "Property not found")));

    /// <summary>
    /// Gets the cached column name mappings of the property.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="propertyInfo">The target property.</param>
    /// <returns>The cached column name mappings of the property.</returns>
    internal static string Get<TEntity>(PropertyInfo propertyInfo)
        where TEntity : class =>
        Get(typeof(TEntity), propertyInfo);

    /// <summary>
    /// Gets the cached column name mappings of the property.
    /// </summary>
    /// <param name="propertyInfo">The target property.</param>
    /// <returns>The cached column name mappings of the property.</returns>
    internal static string Get(PropertyInfo propertyInfo) =>
        Get(propertyInfo.DeclaringType!, propertyInfo);

    /// <summary>
    /// Gets the cached column name mappings of the property.
    /// </summary>
    /// <param name="entityType">The type of the data entity.</param>
    /// <param name="propertyInfo">The target property.</param>
    /// <returns>The cached column name mappings of the property.</returns>
    internal static string Get(Type entityType,
        PropertyInfo propertyInfo)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo);

        // Variables
        var key = GenerateHashCode(entityType, propertyInfo);

        // Try get the value
        return cache.GetOrAdd(key, (_) => resolver.Resolve(propertyInfo, entityType));
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Flushes all the existing cached property mapped names.
    /// </summary>
    public static void Flush() =>
        cache.Clear();

    /// <summary>
    /// Generates a hashcode for caching.
    /// </summary>
    /// <param name="entityType">The type of the data entity.</param>
    /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
    /// <returns>The generated hashcode.</returns>
    private static int GenerateHashCode(Type entityType,
        PropertyInfo propertyInfo) =>
        TypeExtension.GenerateHashCode(entityType, propertyInfo);

    #endregion
}
