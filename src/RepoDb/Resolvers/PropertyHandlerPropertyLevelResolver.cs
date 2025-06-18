using System.Reflection;
using RepoDb.Attributes;
using RepoDb.Interfaces;

namespace RepoDb.Resolvers;

/// <summary>
/// A class that is being used to resolve the equivalent <see cref="IPropertyHandler{TInput, TResult}"/> object of the property.
/// </summary>
public class PropertyHandlerPropertyLevelResolver : IResolver<Type, PropertyInfo, object>
{
    /// <summary>
    /// Resolves the equivalent <see cref="IPropertyHandler{TInput, TResult}"/> object of the property.
    /// </summary>
    /// <param name="entityType">The type of the data entity.</param>
    /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/> to be resolved.</param>
    /// <returns>The equivalent <see cref="IPropertyHandler{TInput, TResult}"/> object of the property.</returns>
    public object? Resolve(Type entityType,
        PropertyInfo propertyInfo)
    {
        object? propertyHandler = null;

        // Attribute
        var attribute = propertyInfo.GetCustomAttribute<PropertyHandlerAttribute>();
        if (attribute != null)
        {
            propertyHandler = Converter.ToType<object>(Activator.CreateInstance(attribute.HandlerType));
        }

        // Property Level
        propertyHandler ??= PropertyHandlerMapper.Get<object>(entityType, propertyInfo);

        // Type Level
        propertyHandler ??= PropertyHandlerMapper.Get<object>(propertyInfo.PropertyType);

        // Return the value
        return propertyHandler;
    }
}
