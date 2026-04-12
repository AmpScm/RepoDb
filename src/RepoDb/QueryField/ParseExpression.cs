using System.Linq.Expressions;
using System.Reflection;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Extensions;

namespace RepoDb;

public partial class QueryField
{
    /// <summary>
    ///
    /// </summary>
    protected internal virtual bool NoParametersNeeded =>
        Operation is Operation.IsNotNull or Operation.IsNull
        || (Operation is Operation.Equal or Operation.NotEqual && Value is null);

    private static ClassProperty? GetTargetProperty<TEntity>(Field field)
        where TEntity : class
    {
        var properties = PropertyCache.Get<TEntity>();

        // Failing at some point - for base interfaces
        return
            properties.GetByFieldName(field.FieldName)
            ?? properties.GetByPropertyName(field.FieldName);
    }

    internal static Operation GetOperation(ExpressionType expressionType)
    {
        return expressionType switch
        {
            ExpressionType.Equal => Operation.Equal,
            ExpressionType.GreaterThan => Operation.GreaterThan,
            ExpressionType.LessThan => Operation.LessThan,
            ExpressionType.NotEqual => Operation.NotEqual,
            ExpressionType.GreaterThanOrEqual => Operation.GreaterThanOrEqual,
            ExpressionType.LessThanOrEqual => Operation.LessThanOrEqual,
            _ => throw new NotSupportedException($"Operation: Expression '{expressionType}' is currently not supported.")
        };
    }

    internal static QueryField ToIn(Field field, System.Collections.IEnumerable enumerable, bool isNot = false)
    {
        var operation = isNot ? Operation.NotIn : Operation.In;
        return new QueryField(field, operation, enumerable.AsTypedSet(), null);
    }

    internal static QueryField ToLike(Field field, object? value, bool isNot = false)
    {
        var operation = isNot ? Operation.NotLike : Operation.Like;
        return new QueryField(field, operation, value, null);
    }

    internal static string ConvertToLikeableValue(string methodName,
        string value)
    {
        if (methodName == nameof(string.Contains))
        {
            value = value.StartsWith('%') ? value : string.Concat("%", value);
            value = value.EndsWith('%') ? value : string.Concat(value, "%");
        }
        else if (methodName == nameof(string.StartsWith))
        {
            value = value.EndsWith('%') ? value : string.Concat(value, "%");
        }
        else if (methodName == nameof(string.EndsWith))
        {
            value = value.StartsWith('%') ? value : string.Concat("%", value);
        }
        return value;
    }

    /*
     * Binary
     */

    internal static QueryGroup Parse<TEntity>(BinaryExpression expression)
        where TEntity : class
    {
        // Only support the following expression type
        if (!expression.IsExtractable())
        {
            throw new NotSupportedException($"Expression '{expression}' is currently not supported.");
        }

        // Field
        var operation = GetOperation(expression.NodeType);
        var fieldExpression = expression.Left;
        if (!expression.Left.TryGetField(out var field, out var coalesceValue) || !expression.Right.TryGetValue(out var value))
        {
            if (!expression.Left.TryGetValue(out value) || !expression.Right.TryGetField(out var field2y, out coalesceValue))
            {
                throw new InvalidExpressionException($"Invalid expression '{expression}'. The expression can't be converted to a <FIELD> = <VALUE> check.");
            }
            field = field2y;

            operation = operation.SwitchOperands();
            fieldExpression = expression.Right;
        }

        // Enum values are typically compared as their numeric value, but we really want to compare them as proper values
        if (TypeCache.Get(fieldExpression.UnwrapUnary(ExpressionType.Convert).Type).UnderlyingType is { IsEnum: true } ut)
        {
            value = ToEnumValue(ut, value);
        }

        var check = new QueryField(field, operation, value, null);

        if (coalesceValue is { })
        {
            if (operation is Operation.Equal && Equals(value, coalesceValue) && value is { })
            {
                // X = @Y OR X IS NULL

                return new QueryGroup([check, new QueryField(field, Operation.IsNull, value, null)], Conjunction.Or);
            }
            else if (operation is Operation.NotEqual && !Equals(value, coalesceValue))
            {
                // X <> @Y OR X IS NULL
                return new QueryGroup([check, new QueryField(field, Operation.IsNull, value, null)], Conjunction.Or);
            }
            else
                throw new InvalidExpressionException($"Invalid expression '??' can only be applied in an Equals or NotEquals .");
        }
        else if (operation == Operation.Equal)
        {
            if (value is null)
                check = new QueryField(field, Operation.IsNull, value, null);
            else if (GlobalConfiguration.Options.ExpressionNullSemantics == ExpressionNullSemantics.NullNotEqual)
                return new QueryGroup([check, new QueryField(field, Operation.IsNotNull, value, null) { CanSkip = true }], Conjunction.And);
        }
        else if (operation == Operation.NotEqual)
        {
            if (value is null)
                check = new QueryField(field, Operation.IsNotNull, value, null);
            else if (GlobalConfiguration.Options.ExpressionNullSemantics == ExpressionNullSemantics.NullNotEqual)
            {
                // X != @Y OR X is NULL
                return new QueryGroup([check, new QueryField(field, Operation.IsNull, value, null) { CanSkip = true }], Conjunction.Or);
            }
        }

        // Return the value
        return new QueryGroup(check.AsEnumerable());
    }

    private static object? ToEnumValue(Type enumType,
        object? value)
    {
        return (value != null ?
            ToEnumValue(enumType, Enum.GetName(enumType, value)) : null) ?? value;

        static object? ToEnumValue(Type enumType, string? name)
        {
            return !string.IsNullOrEmpty(name) && Enum.IsDefined(enumType, name) ?
                Enum.Parse(enumType, name) : null;
        }
    }

    /*
     * Member
     */


    #region GetProperty

    private static ClassProperty? GetProperty<TEntity>(Expression expression)
        where TEntity : class
    {
        return expression switch
        {
            LambdaExpression lambdaExpression => GetProperty<TEntity>(lambdaExpression.Body),
            BinaryExpression binaryExpression => GetProperty<TEntity>(binaryExpression.Left) ?? GetProperty<TEntity>(binaryExpression.Right),
            MethodCallExpression methodCallExpression when methodCallExpression.Method is { IsSpecialName: true, Name: "op_Implicit" } m && m.DeclaringType?.IsSpan() == true => GetProperty<TEntity>(methodCallExpression.Arguments[0]),
            MethodCallExpression mce => mce.Object?.Type == StaticType.String ?
                GetProperty<TEntity>(mce.Object) :
                GetProperty<TEntity>(mce.Arguments[1]),
            MemberExpression memberExpression => GetProperty<TEntity>(memberExpression),
            _ => null
        };
    }

    private static ClassProperty? GetProperty<TEntity>(MemberExpression memberExpression)
        where TEntity : class
    {
        return memberExpression.Member.DeclaringType is { } dt && dt.IsGenericType && dt.GetGenericTypeDefinition() == StaticType.Nullable && memberExpression.Expression is { }
            ? GetProperty<TEntity>(memberExpression.Expression)
            : memberExpression.Member is PropertyInfo pi ? GetProperty<TEntity>(pi) : null;
    }

    private static ClassProperty? GetProperty<TEntity>(PropertyInfo propertyInfo)
        where TEntity : class
    {
        if (propertyInfo is null)
        {
            return null;
        }

        // Variables
        var properties = PropertyCache.Get<TEntity>();
        var name = PropertyMappedNameCache.Get(propertyInfo);

        // Failing at some point - for base interfaces
        return properties.GetByFieldName(name)
            ?? properties.GetByPropertyName(name);
    }

    #endregion
}
