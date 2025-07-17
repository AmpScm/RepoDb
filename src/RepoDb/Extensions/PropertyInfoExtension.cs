using System.Buffers;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;
using RepoDb.Attributes;
using RepoDb.Attributes.Parameter;
using RepoDb.Enumerations;
using RepoDb.Interfaces;

namespace RepoDb.Extensions;

/// <summary>
/// Contains the extension methods for <see cref="PropertyInfo"/> object.
/// </summary>
public static class PropertyInfoExtension
{
    /// <summary>
    /// Gets a custom attribute defined on the property.
    /// </summary>
    /// <typeparam name="T">The custom attribute that is defined into the property.</typeparam>
    /// <param name="property">The property of where the custom attribute is defined.</param>
    /// <returns>The custom attribute.</returns>
    internal static T? GetCustomAttribute<T>(this PropertyInfo property)
        where T : Attribute =>
        property.GetCustomAttribute<T>(false);

    /// <summary>
    /// Gets the mapped name of the property.
    /// </summary>
    /// <param name="property">The property where the mapped name will be retrieved.</param>
    /// <returns>A string containing the mapped name.</returns>
    public static string GetMappedName(this PropertyInfo property)
    {
        ArgumentNullException.ThrowIfNull(property);
        return GetMappedName(property, property.DeclaringType!);
    }

#if NET8_0_OR_GREATER
    static readonly SearchValues<char> unquote = SearchValues.Create(['[', ']', '`', '\"']);
#endif

    /// <summary>
    /// Gets the mapped name of the property.
    /// </summary>
    /// <param name="property">The property where the mapped name will be retrieved.</param>
    /// <param name="declaringType">The declaring type of the property.</param>
    /// <returns>A string containing the mapped name.</returns>
    internal static string GetMappedName(this PropertyInfo property,
        Type declaringType)
    {
        var mappedName = GetCustomAttribute<MapAttribute>(property)?.Name ??
            GetCustomAttribute<ColumnAttribute>(property)?.Name ??
            GetCustomAttribute<NameAttribute>(property)?.Name;

#if NET8_0_OR_GREATER
        if (mappedName is { } && mappedName.IndexOfAny(unquote) >= 0)
#else
        if (mappedName is { })
#endif
        {
            if (mappedName.StartsWith('[') && mappedName.EndsWith(']'))
            {
                var nn = mappedName.Substring(1, mappedName.Length - 2);

                if (nn.IndexOfAny(['[', ']']) < 0)
                    mappedName = nn;
            }
            else
            {
                if (mappedName.Contains('`', StringComparison.Ordinal))
                    mappedName = mappedName.Replace("`", "");
                if (mappedName.Contains('"', StringComparison.Ordinal))
                    mappedName = mappedName.Replace("\"", "");
            }
        }

        return mappedName ??
            PropertyMapper.Get(declaringType, property) ??
            GetPropertyValueAttribute<NameAttribute>(property, declaringType)?.Name ??
            property.Name;
    }

    /// <summary>
    /// Converts a <see cref="PropertyInfo"/> into a query field object.
    /// </summary>
    /// <param name="property">The instance of <see cref="PropertyInfo"/> to be converted.</param>
    /// <param name="entity">The entity object where the value of the property will be retrieved.</param>
    /// <returns>An instance of query field object that holds the converted name and values of the property.</returns>
    public static QueryField AsQueryField(this PropertyInfo property,
        object entity) =>
        AsQueryField(property, entity, false);

    /// <summary>
    /// Converts a <see cref="PropertyInfo"/> into a query field object.
    /// </summary>
    /// <param name="property">The instance of <see cref="PropertyInfo"/> to be converted.</param>
    /// <param name="entity">The entity object where the value of the property will be retrieved.</param>
    /// <returns>An instance of query field object that holds the converted name and values of the property.</returns>
    /// <param name="prependUnderscore">The value to identify whether the underscore prefix will be appended to the parameter name.</param>
    internal static QueryField AsQueryField(this PropertyInfo property,
        object entity,
        bool prependUnderscore) =>
        new(property.AsField(), Operation.Equal, property.GetHandledValue(entity), null, prependUnderscore);

    /// <summary>
    /// Converts a <see cref="PropertyInfo"/> into a mapped name.
    /// </summary>
    /// <param name="property">The instance of the property to be converted.</param>
    /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
    /// <returns>A instance of string containing the value of a mapped name.</returns>
    internal static string AsFieldAsString(this PropertyInfo property,
        IDbSetting? dbSetting) =>
        PropertyMappedNameCache.Get(property).AsQuoted(true, dbSetting);

    /// <summary>
    /// Converts a <see cref="PropertyInfo"/> into a parameterized name.
    /// </summary>
    /// <param name="property">The instance of the property to be converted.</param>
    /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
    /// <returns>A instance of string containing the value of a parameterized name.</returns>
    internal static string AsParameterAsString(this PropertyInfo property,
        IDbSetting dbSetting) =>
        string.Concat(dbSetting.ParameterPrefix, PropertyMappedNameCache.Get(property));

    /// <summary>
    /// Converts a <see cref="PropertyInfo"/> into a parameterized (as field) name.
    /// </summary>
    /// <param name="property">The instance of the property to be converted.</param>
    /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
    /// <returns>A instance of string containing the value of a parameterized (as field) name.</returns>
    internal static string AsParameterAsFieldAsString(this PropertyInfo property,
        IDbSetting dbSetting) =>
        string.Concat(AsParameterAsString(property, dbSetting), " AS ", AsFieldAsString(property, dbSetting));

    /// <summary>
    /// Converts a <see cref="PropertyInfo"/> into a field and parameter name.
    /// </summary>
    /// <param name="property">The instance of the property to be converted.</param>
    /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
    /// <returns>A instance of string containing the value of a field and parameter name.</returns>
    internal static string AsFieldAndParameterAsString(this PropertyInfo property,
        IDbSetting dbSetting) =>
        string.Concat(AsFieldAsString(property, dbSetting), " = ", AsParameterAsString(property, dbSetting));

    /// <summary>
    /// Converts a <see cref="PropertyInfo"/> into a field (and its alias) name.
    /// </summary>
    /// <param name="property">The instance of the property to be converted.</param>
    /// <param name="alias">The alias to be used.</param>
    /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
    /// <returns>A instance of string containing the value of a field (and its alias) name.</returns>
    internal static string AsFieldAndAliasField(this PropertyInfo property,
        string alias,
        IDbSetting? dbSetting) =>
        string.Concat(AsFieldAsString(property, dbSetting), " = ", alias, ".", AsFieldAsString(property, dbSetting));

    /// <summary>
    /// Generates a hashcode of the <see cref="PropertyInfo"/> object based on the parent class name and its own name.
    /// </summary>
    /// <param name="property">The instance of the <see cref="PropertyInfo"/> object.</param>
    /// <param name="declaringType">The declaring type of the <see cref="PropertyInfo"/> object. This refers to the derived class if present.</param>
    /// <returns>The generated hashcode.</returns>
    internal static int GenerateCustomizedHashCode(this PropertyInfo property, Type? declaringType = null) =>
        HashCode.Combine((declaringType ?? property.DeclaringType), property.Name, property.PropertyType);

    /// <summary>
    /// Converts an instance of <see cref="PropertyInfo"/> object into <see cref="Field"/> object.
    /// </summary>
    /// <param name="property">The instance of <see cref="PropertyInfo"/> object to be converted.</param>
    /// <returns>The converted instance of <see cref="Field"/> object.</returns>
    public static Field AsField(this PropertyInfo property)
    {
        ArgumentNullException.ThrowIfNull(property);
        return new(PropertyMappedNameCache.Get(property), TypeCache.Get(property.PropertyType).UnderlyingType);
    }

    /// <summary>
    /// Converts an enumerable of <see cref="PropertyInfo"/> objects into an enumerable array of <see cref="Field"/>.
    /// </summary>
    /// <param name="properties">The enumerable array of properties to be converted.</param>
    /// <returns>An enumerable array of <see cref="Field"/>.</returns>
    public static FieldSet AsFields(this IEnumerable<PropertyInfo> properties)
        => new(properties.Select(x => x.AsField()));

    /// <summary>
    /// Gets the mapped <see cref="DbType"/> for the current <see cref="PropertyInfo"/>.
    /// </summary>
    /// <param name="propertyInfo">The target property.</param>
    /// <param name="declaringType">The declaring type.</param>
    /// <returns>The mapped <see cref="DbType"/> object.</returns>
    public static DbType? GetDbType(this PropertyInfo propertyInfo, Type? declaringType = null)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo);
        return TypeMapCache.Get(declaringType ??= propertyInfo.DeclaringType!, propertyInfo) ??
            GetPropertyValueAttribute<DbTypeAttribute>(propertyInfo, declaringType)?.DbType ??
            (DbType?)GetDbTypePropertyValueAttribute(propertyInfo, declaringType)?.Value ??
            TypeMapCache.Get(propertyInfo.PropertyType);
    }

    /// <summary>
    /// Returns the list of <see cref="PropertyValueAttribute"/> object that is currently mapped
    /// on the target <see cref="PropertyInfo"/> object.
    /// </summary>
    /// <param name="property">The target property.</param>
    /// <param name="declaringType">The declaring type of the property.</param>
    /// <returns>The list of mapped <see cref="PropertyHandlerAttribute"/> objects.</returns>
    public static IEnumerable<PropertyValueAttribute> GetPropertyValueAttributes(this PropertyInfo property, Type? declaringType = null)
    {
        var customAttributes = property
            .GetCustomAttributes<PropertyValueAttribute>()
            .ToHashSet();

        var mappedAttributes = PropertyValueAttributeMapper
            .Get((declaringType ?? property.DeclaringType!), property)
            .Where(e => customAttributes?.Contains(e) != true) ?? PropertyValueAttributeMapper.Get(property.PropertyType);

        return customAttributes == null ? mappedAttributes : mappedAttributes == null ? customAttributes :
            mappedAttributes.Union(customAttributes);
    }

    /// <summary>
    /// Gets the instance of <see cref="PropertyValueAttribute"/> object from the existing mapped
    /// list of <see cref="PropertyValueAttribute"/> objects.
    /// </summary>
    /// <typeparam name="TPropertyValueAttribute">The target type of the <see cref="PropertyValueAttribute"/>.</typeparam>
    /// <param name="property">The property where to extract the instance of <see cref="PropertyValueAttribute"/> object.</param>
    /// <param name="declaringType">The declaring type of the property.</param>
    /// <returns>The instance of target <see cref="PropertyValueAttribute"/> object.</returns>
    public static TPropertyValueAttribute? GetPropertyValueAttribute<TPropertyValueAttribute>(this PropertyInfo property,
        Type? declaringType = null)
        where TPropertyValueAttribute : PropertyValueAttribute
    {
        ArgumentNullException.ThrowIfNull(property);
        return GetPropertyValueAttributes(property, declaringType ?? property.DeclaringType!).OfType<TPropertyValueAttribute>().LastOrDefault();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="property"></param>
    /// <param name="declaringType"></param>
    /// <returns></returns>
    internal static PropertyValueAttribute? GetDbTypePropertyValueAttribute(this PropertyInfo property, Type? declaringType = null) =>
        GetPropertyValueAttributeByParameterName(property, declaringType ?? property.DeclaringType!, nameof(IDbDataParameter.DbType));

    /// <summary>
    ///
    /// </summary>
    /// <param name="property"></param>
    /// <param name="declaringType"></param>
    /// <param name="parameterName"></param>
    /// <returns></returns>
    internal static PropertyValueAttribute? GetPropertyValueAttributeByParameterName(this PropertyInfo property,
        Type? declaringType,
        string parameterName) =>
        PropertyValueAttributeMapper
            .Get(declaringType ?? property.DeclaringType!, property)
            ?.LastOrDefault(e => string.Equals(parameterName, e.PropertyName, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Returns the value of the data entity property. If the property handler is defined in the property, then the
    /// handled value will be returned.
    /// </summary>
    /// <param name="property">The target <see cref="PropertyInfo"/> object.</param>
    /// <param name="entity">The instance of the data entity object.</param>
    /// <returns>The handled value of the data entity property.</returns>
    public static object? GetHandledValue(this PropertyInfo property,
        object entity)
    {
        ArgumentNullException.ThrowIfNull(property);
        return GetHandledValue(property, entity, property.DeclaringType!);
    }

    /// <summary>
    /// Returns the value of the data entity property. If the property handler is defined in the property, then the
    /// handled value will be returned.
    /// </summary>
    /// <param name="property">The target <see cref="PropertyInfo"/> object.</param>
    /// <param name="entity">The instance of the data entity object.</param>
    /// <param name="declaringType">The customized declaring type of the <see cref="PropertyInfo"/> object.</param>
    /// <returns>The handled value of the data entity property.</returns>
    public static object? GetHandledValue(this PropertyInfo property,
        object entity,
        Type declaringType)
    {
        ArgumentNullException.ThrowIfNull(property);

        var classProperty = PropertyCache.Get((declaringType ?? property.DeclaringType), property, true);
        var propertyHandler = classProperty?.GetPropertyHandler();
        var value = property?.GetValue(entity);
        if (propertyHandler != null)
        {
            var setMethod = propertyHandler.GetType().GetMethod(nameof(IPropertyHandler<,>.Set))!;
            return setMethod.Invoke(propertyHandler, [value, classProperty]);
        }
        return value;
    }
}
