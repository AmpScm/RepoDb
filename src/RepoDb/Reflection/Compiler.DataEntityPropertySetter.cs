using System.Linq.Expressions;
using System.Reflection;
using RepoDb.Extensions;

namespace RepoDb.Reflection;

internal partial class Compiler
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

        if (property is null)
        {
            // If the property is not found, then return a no-op function
            return (_, _) => { };
        }

        // Return the function
        return CompileDataEntityPropertySetter(entityType, property);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="property"></param>
    /// <param name="targetType"></param>
    /// <returns></returns>
    private static Action<object, object?> CompileDataEntityPropertySetter(Type entityType,
        PropertyInfo property)
    {
        // Make sure we can write
        if (!property.CanWrite)
        {
            return (_, _) => { };
        }

        // Variables for argument
        var valueParameter = Expression.Parameter(StaticType.Object, "value");
        Type targetType = property.PropertyType;

        // Get the converter
        var toTypeMethod = StaticType
            .Converter
            .GetMethod(nameof(Converter.ToType), [StaticType.Object])!
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
        var propExpr = Expression.Property(Expression.Convert(entityParameter, entityType), property);
        var propertyAssignment = Expression.Assign(propExpr, valueExpression);

        // Return function
        return Expression.Lambda<Action<object, object?>>(propertyAssignment,
            entityParameter, valueParameter).Compile();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    public static Func<object, object?> CompileDataEntityPropertyGetter(Type entityType,
        Field field)
    {
        // Get the entity property
        var property = PropertyCache.Get(entityType).GetByFieldName(field.FieldName)?.PropertyInfo;

        if (property is null)
        {
            // If the property is not found, then return a no-op function
            return (_) => null;
        }

        // Return the function
        return CompileDataEntityPropertyGetter(entityType, property);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="property"></param>
    /// <param name="targetType"></param>
    /// <returns></returns>
    private static Func<object, object?> CompileDataEntityPropertyGetter(Type entityType,
        PropertyInfo property)
    {
        // Make sure we can write
        if (!property.CanWrite)
        {
            return (_) => null;
        }

        var entityParameter = Expression.Parameter(StaticType.Object, "entity");
        var propertyValue = Expression.Property(Expression.Convert(entityParameter, entityType), property);
        Type targetType = property.PropertyType;

        // Get the converter
        var toTypeMethod = StaticType
            .Converter
            .GetMethod(nameof(Converter.ToType), [StaticType.Object])!
            .MakeGenericMethod(TypeCache.Get(targetType).UnderlyingType);

        // Conversion (if needed)
        var valueExpression = ConvertExpressionToTypeExpression(Expression.Call(toTypeMethod, propertyValue), targetType);

        // Return function
        return Expression.Lambda<Func<object, object?>>(valueExpression, entityParameter).Compile();
    }
}
