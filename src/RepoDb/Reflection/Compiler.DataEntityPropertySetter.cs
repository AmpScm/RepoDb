using System.Linq.Expressions;
using System.Reflection;
using RepoDb.Extensions;

namespace RepoDb.Reflection;

partial class Compiler
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    public static Action<object, object?> CompileDataEntityPropertySetter(Type entityType,
        Field field)
    {
        // Get the entity property
        var property = PropertyCache.Get(entityType).GetByFieldName(field.FieldName)?.PropertyInfo;

        if (property == null)
        {
            // If the property is not found, then return a no-op function
            return (_, _) => { };
        }

        // Return the function
        return CompileDataEntityPropertySetter(entityType,
            property,
            property.PropertyType);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="property"></param>
    /// <param name="targetType"></param>
    /// <returns></returns>
    private static Action<object, object?> CompileDataEntityPropertySetter(Type entityType,
        PropertyInfo property,
        Type targetType)
    {
        // Make sure we can write
        if (property.CanWrite == false)
        {
            return (_, _) => { };
        }

        // Variables for argument
        var valueParameter = Expression.Parameter(StaticType.Object, "value");

        // Get the converter
        var toTypeMethod = StaticType
            .Converter
            .GetMethod("ToType", [StaticType.Object])!
            .MakeGenericMethod(TypeCache.Get(targetType).UnderlyingType);

        // Conversion (if needed)
        var valueExpression = ConvertExpressionToTypeExpression(Expression.Call(toTypeMethod, valueParameter), targetType);

        // Property Handler
        if (TypeCache.Get(entityType).IsClassType)
        {
            var classProperty = PropertyCache.Get(entityType, property, true);
            valueExpression = ConvertExpressionToPropertyHandlerSetExpression(valueExpression,
                null, classProperty, targetType);
        }

        // Assign the value into DataEntity.Property
        var entityParameter = Expression.Parameter(StaticType.Object, "entity");
        var propertyAssignment = Expression.Call(Expression.Convert(entityParameter, entityType), property.SetMethod!,
            valueExpression);

        // Return function
        return Expression.Lambda<Action<object, object?>>(propertyAssignment,
            entityParameter, valueParameter).Compile();
    }
}
