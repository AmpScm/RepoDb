using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using RepoDb.Attributes.Parameter;
using RepoDb.Exceptions;
using RepoDb.Extensions;

namespace RepoDb;

/// <summary>
/// A class that is being used to map a list of <see cref="PropertyValueAttribute"/> object into a class property.
/// </summary>
public static class PropertyValueAttributeMapper
{
    #region Privates

    private static readonly ConcurrentDictionary<(Type Type, PropertyInfo PropertyInfo), IEnumerable<PropertyValueAttribute>> maps = new();

    #endregion

    #region Methods

    #region Property Level

    /*
     * Add
     */

    /// <summary>
    /// Property Level: Adds a mapping between a class property and an instance of <see cref="PropertyValueAttribute"/> object (via expression).
    /// </summary>
    /// <typeparam name="TEntity">The target type.</typeparam>
    /// <param name="expression">The property expression.</param>
    /// <param name="attribute">The instance of <see cref="PropertyValueAttribute"/> object.</param>
    public static void Add<TEntity>(Expression<Func<TEntity, object?>> expression,
        PropertyValueAttribute attribute)
        where TEntity : class =>
        Add(expression, attribute, false);

    /// <summary>
    /// Property Level: Adds a mapping between a class property and an instance of <see cref="PropertyValueAttribute"/> object (via expression).
    /// </summary>
    /// <typeparam name="TEntity">The target type.</typeparam>
    /// <param name="expression">The property expression.</param>
    /// <param name="attribute">The instance of <see cref="PropertyValueAttribute"/> object.</param>
    /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
    public static void Add<TEntity>(Expression<Func<TEntity, object?>> expression,
        PropertyValueAttribute attribute,
        bool force)
        where TEntity : class =>
        Add(expression, [attribute], force);

    /// <summary>
    /// Property Level: Adds a mapping between a class property and a list of <see cref="PropertyValueAttribute"/> object (via expression).
    /// </summary>
    /// <typeparam name="TEntity">The target type.</typeparam>
    /// <param name="expression">The property expression.</param>
    /// <param name="attributes">The list of <see cref="PropertyValueAttribute"/> object.</param>
    public static void Add<TEntity>(Expression<Func<TEntity, object?>> expression,
        IEnumerable<PropertyValueAttribute> attributes)
        where TEntity : class =>
        Add(expression, attributes, false);

    /// <summary>
    /// Property Level: Adds a mapping between a class property and a list of <see cref="PropertyValueAttribute"/> object (via expression).
    /// </summary>
    /// <typeparam name="TEntity">The target type.</typeparam>
    /// <param name="expression">The property expression.</param>
    /// <param name="attributes">The list of <see cref="PropertyValueAttribute"/> object.</param>
    /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
    public static void Add<TEntity>(Expression<Func<TEntity, object?>> expression,
        IEnumerable<PropertyValueAttribute> attributes,
        bool force)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(expression);
        Add(ExpressionExtension.GetProperty(expression), attributes, force);
    }

    /// <summary>
    /// Property Level: Adds a mapping between a class property and an instance of <see cref="PropertyValueAttribute"/> object (property name).
    /// </summary>
    /// <typeparam name="TEntity">The target type.</typeparam>
    /// <param name="propertyName">The name of the target class property.</param>
    /// <param name="attribute">The instance of <see cref="PropertyValueAttribute"/> object.</param>
    public static void Add<TEntity>(string propertyName,
        PropertyValueAttribute attribute)
        where TEntity : class =>
        Add<TEntity>(propertyName, attribute, false);

    /// <summary>
    /// Property Level: Adds a mapping between a class property and an instance of <see cref="PropertyValueAttribute"/> object (property name).
    /// </summary>
    /// <typeparam name="TEntity">The target type.</typeparam>
    /// <param name="propertyName">The name of the target class property.</param>
    /// <param name="attribute">The instance of <see cref="PropertyValueAttribute"/> object.</param>
    /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
    public static void Add<TEntity>(string propertyName,
        PropertyValueAttribute attribute,
        bool force)
        where TEntity : class =>
        Add<TEntity>(propertyName, [attribute], force);

    /// <summary>
    /// Property Level: Adds a mapping between a class property and a list of <see cref="PropertyValueAttribute"/> object (via property name).
    /// </summary>
    /// <typeparam name="TEntity">The target type.</typeparam>
    /// <param name="propertyName">The name of the target class property.</param>
    /// <param name="attributes">The list of <see cref="PropertyValueAttribute"/> object.</param>
    public static void Add<TEntity>(string propertyName,
        IEnumerable<PropertyValueAttribute> attributes)
        where TEntity : class =>
        Add<TEntity>(propertyName, attributes, false);

    /// <summary>
    /// Property Level: Adds a mapping between a class property and a list of <see cref="PropertyValueAttribute"/> object (via property name).
    /// </summary>
    /// <typeparam name="TEntity">The target type.</typeparam>
    /// <param name="propertyName">The name of the target class property.</param>
    /// <param name="attributes">The list of <see cref="PropertyValueAttribute"/> object.</param>
    /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
    public static void Add<TEntity>(string propertyName,
        IEnumerable<PropertyValueAttribute> attributes,
        bool force)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(propertyName);

        // Add to the mapping
        Add(DataEntityExtension.GetPropertyOrThrow<TEntity>(propertyName), attributes, force);
    }

    /// <summary>
    /// Property Level: Adds a mapping between a class property and an instance of <see cref="PropertyValueAttribute"/> object (via <see cref="Field"/> object).
    /// </summary>
    /// <typeparam name="TEntity">The target type.</typeparam>
    /// <param name="field">The instance of <see cref="Field"/> object to be mapped.</param>
    /// <param name="attribute">The instance of <see cref="PropertyValueAttribute"/> object.</param>
    public static void Add<TEntity>(Field field,
        PropertyValueAttribute attribute)
        where TEntity : class =>
        Add<TEntity>(field, attribute, false);

    /// <summary>
    /// Property Level: Adds a mapping between a class property and an instance of <see cref="PropertyValueAttribute"/> object (via <see cref="Field"/> object).
    /// </summary>
    /// <typeparam name="TEntity">The target type.</typeparam>
    /// <param name="field">The instance of <see cref="Field"/> object to be mapped.</param>
    /// <param name="attribute">The instance of <see cref="PropertyValueAttribute"/> object.</param>
    /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
    public static void Add<TEntity>(Field field,
        PropertyValueAttribute attribute,
        bool force)
        where TEntity : class =>
        Add<TEntity>(field, [attribute], force);

    /// <summary>
    /// Property Level: Adds a mapping between a class property and a list of <see cref="PropertyValueAttribute"/> object (via <see cref="Field"/> object).
    /// </summary>
    /// <typeparam name="TEntity">The target type.</typeparam>
    /// <param name="field">The instance of <see cref="Field"/> object to be mapped.</param>
    /// <param name="attributes">The list of <see cref="PropertyValueAttribute"/> object.</param>
    public static void Add<TEntity>(Field field,
        IEnumerable<PropertyValueAttribute> attributes)
        where TEntity : class =>
        Add<TEntity>(field, attributes, false);

    /// <summary>
    /// Property Level: Adds a mapping between a class property and a list of <see cref="PropertyValueAttribute"/> object (via <see cref="Field"/> object).
    /// </summary>
    /// <typeparam name="TEntity">The target type.</typeparam>
    /// <param name="field">The instance of <see cref="Field"/> object to be mapped.</param>
    /// <param name="attributes">The list of <see cref="PropertyValueAttribute"/> object.</param>
    /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
    public static void Add<TEntity>(Field field,
        IEnumerable<PropertyValueAttribute> attributes,
        bool force)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(field);

        // Add to the mapping
        Add(DataEntityExtension.GetPropertyOrThrow<TEntity>(field.FieldName), attributes, force);
    }

    /// <summary>
    /// Property Level: Adds a mapping between a <see cref="PropertyInfo"/> object and an instance of <see cref="PropertyValueAttribute"/> object.
    /// </summary>
    /// <param name="propertyInfo">The instance of the target <see cref="PropertyInfo"/> object.</param>
    /// <param name="attribute">The instance of <see cref="PropertyValueAttribute"/> object.</param>
    public static void Add(PropertyInfo propertyInfo,
        PropertyValueAttribute attribute) =>
        Add(propertyInfo, attribute, false);

    /// <summary>
    /// Property Level: Adds a mapping between a <see cref="PropertyInfo"/> object and an instance of <see cref="PropertyValueAttribute"/> object.
    /// </summary>
    /// <param name="propertyInfo">The instance of the target <see cref="PropertyInfo"/> object.</param>
    /// <param name="attribute">The instance of <see cref="PropertyValueAttribute"/> object.</param>
    /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
    public static void Add(PropertyInfo propertyInfo,
        PropertyValueAttribute attribute,
        bool force) =>
        Add(propertyInfo, [attribute], force);

    /// <summary>
    /// Property Level: Adds a mapping between a <see cref="PropertyInfo"/> object and a list of <see cref="PropertyValueAttribute"/> object.
    /// </summary>
    /// <param name="propertyInfo">The instance of the target <see cref="PropertyInfo"/> object.</param>
    /// <param name="attributes">The list of <see cref="PropertyValueAttribute"/> object.</param>
    public static void Add(PropertyInfo propertyInfo,
        IEnumerable<PropertyValueAttribute> attributes) =>
        Add(propertyInfo, attributes, false);

    /// <summary>
    /// Property Level: Adds a mapping between a <see cref="PropertyInfo"/> object and a list of <see cref="PropertyValueAttribute"/> object.
    /// </summary>
    /// <param name="propertyInfo">The instance of the target <see cref="PropertyInfo"/> object.</param>
    /// <param name="attributes">The list of <see cref="PropertyValueAttribute"/> object.</param>
    /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
    public static void Add(PropertyInfo propertyInfo,
        IEnumerable<PropertyValueAttribute> attributes,
        bool force)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo);
        Add(propertyInfo.DeclaringType!, propertyInfo, attributes, force);
    }

    /// <summary>
    /// Property Level: Adds a mapping between a <see cref="PropertyInfo"/> object and an instance of <see cref="PropertyValueAttribute"/> object.
    /// </summary>
    /// <param name="entityType">The target type.</param>
    /// <param name="propertyInfo">The instance of the target <see cref="PropertyInfo"/> object.</param>
    /// <param name="attribute">The instance of <see cref="PropertyValueAttribute"/> object.</param>
    public static void Add(Type entityType,
        PropertyInfo propertyInfo,
        PropertyValueAttribute attribute) =>
        Add(entityType, propertyInfo, attribute, false);

    /// <summary>
    /// Property Level: Adds a mapping between a <see cref="PropertyInfo"/> object and an instance of <see cref="PropertyValueAttribute"/> object.
    /// </summary>
    /// <param name="entityType">The target type.</param>
    /// <param name="propertyInfo">The instance of the target <see cref="PropertyInfo"/> object.</param>
    /// <param name="attribute">The instance of <see cref="PropertyValueAttribute"/> object.</param>
    /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
    public static void Add(Type entityType,
        PropertyInfo propertyInfo,
        PropertyValueAttribute attribute,
        bool force) =>
        Add(entityType, propertyInfo, [attribute], force);

    /// <summary>
    /// Property Level: Adds a mapping between a <see cref="PropertyInfo"/> object and a list of <see cref="PropertyValueAttribute"/> object.
    /// </summary>
    /// <param name="entityType">The target type.</param>
    /// <param name="propertyInfo">The instance of the target <see cref="PropertyInfo"/> object.</param>
    /// <param name="attributes">The list of <see cref="PropertyValueAttribute"/> object.</param>
    public static void Add(Type entityType,
        PropertyInfo propertyInfo,
        IEnumerable<PropertyValueAttribute> attributes) =>
        Add(entityType, propertyInfo, attributes, false);

    /// <summary>
    /// Property Level: Adds a mapping between a <see cref="PropertyInfo"/> object and a list of <see cref="PropertyValueAttribute"/> object.
    /// </summary>
    /// <param name="entityType">The target type.</param>
    /// <param name="propertyInfo">The instance of the target <see cref="PropertyInfo"/> object.</param>
    /// <param name="attributes">The list of <see cref="PropertyValueAttribute"/> object.</param>
    /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
    public static void Add(Type entityType,
        PropertyInfo propertyInfo,
        IEnumerable<PropertyValueAttribute> attributes,
        bool force)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo);
        ArgumentNullException.ThrowIfNull(attributes);

        // Variables
        var key = (entityType, propertyInfo);

        // Add to the cache
        if (maps.TryGetValue(key, out var value))
        {
            if (force)
            {
                maps.TryUpdate(key, attributes, value);
            }
            else
            {
                throw new MappingExistsException($"The mappings are already existing.");
            }
        }
        else
        {
            maps.TryAdd(key, attributes);
        }
    }

    /*
     * Get
     */

    /// <summary>
    /// Get the list of mapped <see cref="PropertyValueAttribute"/> objects of the class property (via expression).
    /// </summary>
    /// <typeparam name="TEntity">The target type.</typeparam>
    /// <param name="expression">The property expression.</param>
    /// <returns>The list of mapped <see cref="PropertyValueAttribute"/> objects.</returns>
    public static IEnumerable<PropertyValueAttribute>? Get<TEntity>(Expression<Func<TEntity, object?>> expression)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(expression);
        return Get(typeof(TEntity), ExpressionExtension.GetProperty(expression));
    }

    /// <summary>
    /// Get the list of mapped <see cref="PropertyValueAttribute"/> objects of the class property (via property name).
    /// </summary>
    /// <typeparam name="TEntity">The target type.</typeparam>
    /// <param name="propertyName">The name of the target class property.</param>
    /// <returns>The list of mapped <see cref="PropertyValueAttribute"/> objects.</returns>
    public static IEnumerable<PropertyValueAttribute>? Get<TEntity>(string propertyName)
        where TEntity : class =>
        Get(typeof(TEntity), TypeExtension.GetProperty<TEntity>(propertyName) ?? throw new PropertyNotFoundException(nameof(propertyName), "Property not found"));

    /// <summary>
    /// Get the list of mapped <see cref="PropertyValueAttribute"/> objects of the class property (via <see cref="Field"/> object).
    /// </summary>
    /// <typeparam name="TEntity">The target type.</typeparam>
    /// <param name="field">The instance of <see cref="Field"/> object.</param>
    /// <returns>The list of mapped <see cref="PropertyValueAttribute"/> objects.</returns>
    public static IEnumerable<PropertyValueAttribute>? Get<TEntity>(Field field)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(field);
        return Get(typeof(TEntity), TypeExtension.GetProperty<TEntity>(field.FieldName));
    }

    /// <summary>
    /// Get the list of mapped <see cref="PropertyValueAttribute"/> objects of the <see cref="PropertyInfo"/> object.
    /// </summary>
    /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
    /// <returns>The list of mapped <see cref="PropertyValueAttribute"/> objects.</returns>
    public static IEnumerable<PropertyValueAttribute>? Get(PropertyInfo propertyInfo)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo);
        return Get(propertyInfo.DeclaringType!, propertyInfo);
    }

    /// <summary>
    /// Get the list of mapped <see cref="PropertyValueAttribute"/> objects of the <see cref="PropertyInfo"/> object.
    /// </summary>
    /// <param name="entityType">The target type.</param>
    /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
    /// <returns>The list of mapped <see cref="PropertyValueAttribute"/> objects.</returns>
    public static IEnumerable<PropertyValueAttribute> Get(Type entityType,
        PropertyInfo propertyInfo)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo);

        // Variables
        var key = (entityType, propertyInfo);

        // Try get the value
        maps.TryGetValue(key, out var value);

        // Return the value
        return value ?? [];
    }

    /*
     * Remove
     */

    /// <summary>
    /// Removes the existing mapped <see cref="PropertyValueAttribute"/> objects of the class property (via expression).
    /// </summary>
    /// <typeparam name="TEntity">The target type.</typeparam>
    /// <param name="expression">The property expression.</param>
    public static bool Remove<TEntity>(Expression<Func<TEntity, object?>> expression)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(expression);
        return Remove(typeof(TEntity), ExpressionExtension.GetProperty(expression));
    }

    /// <summary>
    /// Removes the existing mapped <see cref="PropertyValueAttribute"/> objects of the class property (via property name).
    /// </summary>
    /// <typeparam name="TEntity">The target type.</typeparam>
    /// <param name="propertyName">The name of the target class property.</param>
    public static bool Remove<TEntity>(string propertyName)
        where TEntity : class =>
        Remove(typeof(TEntity), TypeExtension.GetProperty<TEntity>(propertyName));

    /// <summary>
    /// Removes the existing mapped <see cref="PropertyValueAttribute"/> objects of the class property (via <see cref="Field"/> object).
    /// </summary>
    /// <typeparam name="TEntity">The target type.</typeparam>
    /// <param name="field">The instance of <see cref="Field"/> object.</param>
    public static bool Remove<TEntity>(Field field)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(field);
        return Remove(typeof(TEntity), TypeExtension.GetProperty<TEntity>(field.FieldName));
    }

    /// <summary>
    /// Removes the existing mapped <see cref="PropertyValueAttribute"/> objects of the <see cref="PropertyInfo"/> object.
    /// </summary>
    /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
    public static bool Remove(PropertyInfo propertyInfo)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo);
        return Remove(propertyInfo.DeclaringType!, propertyInfo);
    }

    /// <summary>
    /// Removes the existing mapped <see cref="PropertyValueAttribute"/> objects of the <see cref="PropertyInfo"/> object.
    /// </summary>
    /// <param name="entityType">The target type.</param>
    /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
    public static bool Remove(Type entityType,
        PropertyInfo propertyInfo)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo);

        // Variables
        var key = (entityType, propertyInfo);

        // Try get the value
        return maps.TryRemove(key, out var _);
    }

    #endregion

    #region Type Level

    /*
     * Add
     */

    /// <summary>
    /// Type Level: Adds a mapping between a .NET CLR type and a list of <see cref="PropertyValueAttribute"/> object.
    /// </summary>
    /// <typeparam name="TType">The target type.</typeparam>
    /// <param name="attributes">The list of <see cref="PropertyValueAttribute"/> object.</param>
    /// <remarks>The default behavior will be affected if the settings are not handled properly by the user (i.e.: setting the type <see cref="string"/> name attribute to something would affect all the objects properties with type <see cref="string"/>).</remarks>
    public static void Add<TType>(IEnumerable<PropertyValueAttribute> attributes) =>
        Add<TType>(attributes, false);

    /// <summary>
    /// Type Level: Adds a mapping between a .NET CLR type and a list of <see cref="PropertyValueAttribute"/> object.
    /// </summary>
    /// <typeparam name="TType">The target type.</typeparam>
    /// <param name="attributes">The list of <see cref="PropertyValueAttribute"/> object.</param>
    /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
    /// <remarks>The default behavior will be affected if the settings are not handled properly by the user (i.e.: setting the type <see cref="string"/> name attribute to something would affect all the objects properties with type <see cref="string"/>).</remarks>
    public static void Add<TType>(IEnumerable<PropertyValueAttribute> attributes,
        bool force) =>
        Add(typeof(TType), attributes, force);

    /// <summary>
    /// Type Level: Adds a mapping between a .NET CLR type and a list of <see cref="PropertyValueAttribute"/> object.
    /// </summary>
    /// <param name="type">The target type.</param>
    /// <param name="attributes">The list of <see cref="PropertyValueAttribute"/> object.</param>
    /// <remarks>The default behavior will be affected if the settings are not handled properly by the user (i.e.: setting the type <see cref="string"/> name attribute to something would affect all the objects properties with type <see cref="string"/>).</remarks>
    public static void Add(Type type,
        IEnumerable<PropertyValueAttribute> attributes) =>
        Add(type, attributes, false);


    /// <summary>
    /// Type Level: Adds a mapping between a .NET CLR type and a list of <see cref="PropertyValueAttribute"/> object.
    /// </summary>
    /// <param name="type">The target type.</param>
    /// <param name="attributes">The list of <see cref="PropertyValueAttribute"/> object.</param>
    /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
    /// <remarks>The default behavior will be affected if the settings are not handled properly by the user (i.e.: setting the type <see cref="string"/> name attribute to something would affect all the objects properties with type <see cref="string"/>).</remarks>
    public static void Add(Type type,
        IEnumerable<PropertyValueAttribute> attributes,
        bool force)
    {
        ArgumentNullException.ThrowIfNull(attributes);

        // Variables
        var key = TypeExtension.GenerateHashCode(type);

        // Add to the cache
        if (maps.TryGetValue(key, out var value))
        {
            if (force)
            {
                maps.TryUpdate(key, attributes, value);
            }
            else
            {
                throw new MappingExistsException($"The mappings are already existing.");
            }
        }
        else
        {
            maps.TryAdd(key, attributes);
        }
    }

    /*
     * Get
     */

    /// <summary>
    /// Get the list of mapped <see cref="PropertyValueAttribute"/> objects of the .NET CLR type.
    /// </summary>
    /// <typeparam name="TType">The target type.</typeparam>
    /// <returns>The list of mapped <see cref="PropertyValueAttribute"/> objects.</returns>
    public static IEnumerable<PropertyValueAttribute>? Get<TType>() =>
        Get(typeof(TType));

    /// <summary>
    /// Get the list of mapped <see cref="PropertyValueAttribute"/> objects of the .NET CLR type.
    /// </summary>
    /// <param name="type">The target type.</param>
    /// <returns>The list of mapped <see cref="PropertyValueAttribute"/> objects.</returns>
    public static IEnumerable<PropertyValueAttribute> Get(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        // Variables
        var key = TypeExtension.GenerateHashCode(type);

        // Try get the value
        maps.TryGetValue(key, out var value);

        // Return the value
        return value ?? [];
    }

    #endregion

    #region Helpers

    /*
     * Clear
     */

    /// <summary>
    /// Clear all the existing cached mappings.
    /// </summary>
    public static void Clear() =>
        maps.Clear();

    #endregion

    #endregion
}
