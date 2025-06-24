using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Resolvers;

namespace RepoDb;

/// <summary>
/// A class that is being used to cache the mappings between a class property and a <see cref="IPropertyHandler{TInput, TResult}"/> object.
/// </summary>
public static class PropertyHandlerCache
{
    #region Privates

    private static readonly ConcurrentDictionary<int, object?> cache = new();
    private static readonly PropertyHandlerPropertyLevelResolver propertyLevelResolver = new();
    private static readonly PropertyHandlerTypeLevelResolver typeLevelResolver = new();

    #endregion

    #region Methods

    #region Type Level

    /// <summary>
    /// Type Level: Gets the cached <see cref="IPropertyHandler{TInput, TResult}"/> object that is being mapped to a specific .NET CLR type.
    /// </summary>
    /// <typeparam name="TType">The .NET CLR type.</typeparam>
    /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
    /// <returns>The mapped <see cref="IPropertyHandler{TInput, TResult}"/> object of the .NET CLR type.</returns>
    public static TPropertyHandler? Get<TType, TPropertyHandler>() =>
        Get<TPropertyHandler>(typeof(TType));

    /// <summary>
    /// Type Level: Gets the cached <see cref="IPropertyHandler{TInput, TResult}"/> object that is being mapped to a specific .NET CLR type.
    /// </summary>
    /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
    /// <param name="type">The target .NET CLR type.</param>
    /// <returns>The mapped <see cref="IPropertyHandler{TInput, TResult}"/> object of the .NET CLR type.</returns>
    public static TPropertyHandler? Get<TPropertyHandler>(Type type)
    {
        // Validate
        ObjectExtension.ThrowIfNull(type, nameof(type));

        // Variables
        var key = GenerateHashCode(type);

        // Try get the value
        var value = cache.GetOrAdd(key, (_) => typeLevelResolver.Resolve(type));

        return Converter.ToType<TPropertyHandler>(value);
    }

    #endregion

    #region Property Level

    /// <summary>
    /// Property Level: Gets the cached <see cref="IPropertyHandler{TInput, TResult}"/> object that is being mapped on a specific class property (via expression).
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
    /// <param name="expression">The expression to be parsed.</param>
    /// <returns>The mapped <see cref="IPropertyHandler{TInput, TResult}"/> object of the property.</returns>
    public static TPropertyHandler? Get<TEntity, TPropertyHandler>(Expression<Func<TEntity, object?>> expression)
        where TEntity : class =>
        Get<TEntity, TPropertyHandler>(ExpressionExtension.GetProperty(expression));

    /// <summary>
    /// Property Level: Gets the cached <see cref="IPropertyHandler{TInput, TResult}"/> object that is being mapped on a specific class property (via property name).
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>The mapped <see cref="IPropertyHandler{TInput, TResult}"/> object of the property.</returns>
    public static TPropertyHandler? Get<TEntity, TPropertyHandler>(string propertyName)
        where TEntity : class =>
        Get<TEntity, TPropertyHandler>(TypeExtension.GetProperty<TEntity>(propertyName) ?? throw new PropertyNotFoundException(nameof(propertyName), "Property not found"));

    /// <summary>
    /// Property Level: Gets the cached <see cref="IPropertyHandler{TInput, TResult}"/> object that is being mapped on a specific class property (via <see cref="Field"/> object).
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
    /// <param name="field">The instance of <see cref="Field"/> object.</param>
    /// <returns>The mapped <see cref="IPropertyHandler{TInput, TResult}"/> object of the property.</returns>
    public static TPropertyHandler? Get<TEntity, TPropertyHandler>(Field field)
        where TEntity : class =>
        Get<TEntity, TPropertyHandler>(TypeExtension.GetProperty<TEntity>(field.FieldName) ?? throw new PropertyNotFoundException(nameof(field), "Property not found"));


    /// <summary>
    /// Property Level: Gets the cached <see cref="IPropertyHandler{TInput, TResult}"/> object that is being mapped on a specific <see cref="PropertyInfo"/> object.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
    /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
    /// <returns>The mapped <see cref="IPropertyHandler{TInput, TResult}"/> object of the property.</returns>
    internal static TPropertyHandler? Get<TEntity, TPropertyHandler>(PropertyInfo propertyInfo)
        where TEntity : class =>
        Get<TPropertyHandler>(typeof(TEntity), propertyInfo);

    /// <summary>
    /// Property Level: Gets the cached <see cref="IPropertyHandler{TInput, TResult}"/> object that is being mapped on a specific <see cref="PropertyInfo"/> object.
    /// </summary>
    /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
    /// <param name="entityType">The type of the data entity.</param>
    /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
    /// <returns>The mapped <see cref="IPropertyHandler{TInput, TResult}"/> object of the property.</returns>
    internal static TPropertyHandler? Get<TPropertyHandler>(Type entityType, PropertyInfo propertyInfo)
    {
        // Validate
        ObjectExtension.ThrowIfNull(propertyInfo, nameof(propertyInfo));

        // Variables
        var key = GenerateHashCode(entityType, propertyInfo);

        // Try get the value
        var value = cache.GetOrAdd(key, (_) => propertyLevelResolver.Resolve(entityType, propertyInfo));

        return Converter.ToType<TPropertyHandler>(value);
    }

    #endregion

    #endregion

    #region Helpers

    /// <summary>
    /// Flushes all the existing cached <see cref="IPropertyHandler{TInput, TResult}"/> objects.
    /// </summary>
    public static void Flush() =>
        cache.Clear();

    /// <summary>
    /// Generates a hashcode for caching.
    /// </summary>
    /// <param name="type">The type of the data entity.</param>
    /// <returns>The generated hashcode.</returns>
    private static int GenerateHashCode(Type type) =>
        TypeExtension.GenerateHashCode(type);

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
