using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using RepoDb.Exceptions;

namespace RepoDb.Extensions;

/**
 * Though we know that throwing an exception in the extension is not advisable, but I tend to do it to ensure that the
 * parsing of the Linq expressions are properly handled. Please be guided about this extension that it somehow throws
 * and exception at some scenarios.
 */

/// <summary>
/// Contains the extension methods for <see cref="Expression"/> object.
/// </summary>
public static partial class ExpressionExtension
{
    private static readonly HashSet<ExpressionType> extractableExpressionTypes =
    [
        ExpressionType.Equal,
        ExpressionType.NotEqual,
        ExpressionType.GreaterThan,
        ExpressionType.GreaterThanOrEqual,
        ExpressionType.LessThan,
        ExpressionType.LessThanOrEqual
    ];

    /// <summary>
    /// Identify whether the instance of <see cref="Expression"/> can be extracted as <see cref="QueryField"/> object.
    /// </summary>
    /// <param name="expression">The instance of <see cref="Expression"/> object to be identified.</param>
    /// <returns>Returns true if the expression can be extracted as <see cref="QueryField"/> object.</returns>
    internal static bool IsExtractable(this Expression expression) =>
        extractableExpressionTypes.Contains(expression.NodeType);

    /// <summary>
    /// Identify whether the instance of <see cref="Expression"/> can be grouped as <see cref="QueryGroup"/> object.
    /// </summary>
    /// <param name="expression">The instance of <see cref="Expression"/> object to be identified.</param>
    /// <returns>Returns true if the expression can be grouped as <see cref="QueryGroup"/> object.</returns>
    internal static bool IsGroupable(this Expression expression) =>
        expression.NodeType is ExpressionType.AndAlso or ExpressionType.OrElse;

    #region GetField

    /// <summary>
    /// Gets the <see cref="Field"/> defined on the current instance of <see cref="UnaryExpression"/>
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public static Field GetField(this Expression expression)
        => expression.GetField(out _);

    /// <summary>
    /// Gets the <see cref="Field"/> defined on the current instance of <see cref="UnaryExpression"/>
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="coalesceValue"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public static Field GetField(this Expression expression, out object? coalesceValue)
    {
        return expression.TryGetField(out var field, out coalesceValue) ? field : throw new NotSupportedException($"Expression '{expression}' is currently not supported.");
    }

    /// <summary>
    /// Gets the <see cref="Field"/> defined on the current instance of <see cref="Expression"/>
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    public static bool TryGetField(this Expression expression, [NotNullWhen(true)] out Field? field)
    {
        return expression.TryGetField(out field, out _);
    }

    /// <summary>
    /// Gets the <see cref="Field"/> defined on the current instance of <see cref="Expression"/>
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="field"></param>
    /// <param name="coalesceValue"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public static bool TryGetField(this Expression expression, [NotNullWhen(true)] out Field? field, out object? coalesceValue)
    {
        ArgumentNullException.ThrowIfNull(expression);

        coalesceValue = null;
        field = expression switch
        {
            MethodCallExpression methodCallExpression => GetField(methodCallExpression),
            MemberExpression memberExpression => FieldFromMember(memberExpression),
            BinaryExpression { NodeType: ExpressionType.Coalesce } b when
                b.Left.TryGetField(out var leftField)
                && b.Right.TryGetValue(out var value) => (coalesceValue = value) == value ? leftField : throw new NotSupportedException($"Expression '{expression}' is currently not supported."),
            UnaryExpression { NodeType: ExpressionType.Convert } un when un.Operand.TryGetField(out var cv, out coalesceValue) => cv,
            _ => null
        };
        return field is not null;

        static Field? GetField(MethodCallExpression expression)
        {
            ArgumentNullException.ThrowIfNull(expression);
            if (expression.Object is MemberExpression objectMemberExpression)
            {
                return FieldFromMember(objectMemberExpression);
            }
            else if (expression.Method.Name == nameof(Enumerable.Contains)
                && (expression.Object is { } ? expression.Arguments[0] : expression.Arguments[1]) is MemberExpression memberExpression)
            {
                return FieldFromMember(memberExpression);
            }
            else if (expression.Method.Name == nameof(JsonQueryExtensions.ExtractValue) && expression.Method.DeclaringType == typeof(JsonQueryExtensions)
                && expression.Arguments[0].TryGetField(out var v))
            {
                return v;
            }

            return null;
        }

        static Field? FieldFromMember(MemberExpression expression)
        {
            return expression.Member switch
            {
                PropertyInfo propertyInfo => new Field(propertyInfo.GetMappedName(expression.Expression?.Type), expression.Type),
                _ => null
            };
        }
    }

    #endregion

    #region GetName

    /// <summary>
    /// Gets the name of the <see cref="MemberInfo"/> defines on the current instance of <see cref="BinaryExpression"/> object.
    /// </summary>
    /// <param name="expression">The instance of <see cref="BinaryExpression"/> to be checked.</param>
    /// <returns>The name of the <see cref="MemberInfo"/>.</returns>
    internal static string GetMappedName(this BinaryExpression expression)
    {
        ArgumentNullException.ThrowIfNull(expression);
        return expression.Left switch
        {
            MemberExpression memberExpression => memberExpression.Member.GetMappedName(),
            UnaryExpression unaryExpression when unaryExpression.Operand is MethodCallExpression methodCallExpression => GetNameFromMethodCall(methodCallExpression),
            _ => throw new NotSupportedException($"Expression '{expression}' is currently not supported.")
        };

        static string GetNameFromMethodCall(MethodCallExpression expression)
        {
            ArgumentNullException.ThrowIfNull(expression);
            if (expression.Object is MemberExpression objectMemberExpression)
            {
                return objectMemberExpression.Member.GetMappedName();
            }
            else
            {
                if (expression.Method.Name is (nameof(string.Contains)) or
                    (nameof(string.StartsWith)) or
                    (nameof(string.EndsWith)))
                {
                    var last = expression.Object is { } ? expression.Arguments[0] : expression.Arguments[1];
                    if (last is MemberExpression memberExpression)
                    {
                        return memberExpression.Member.GetMappedName();
                    }
                }
            }
            throw new NotSupportedException($"Expression '{expression}' is currently not supported.");
        }
    }

    #endregion

    #region Helper

    /*
     * GetProperty
     */

    /// <summary>
    /// A helper method to return the instance of <see cref="PropertyInfo"/> object based on expression.
    /// </summary>
    /// <typeparam name="T">The target .NET CLR type.</typeparam>
    /// <param name="expression">The expression to be extracted.</param>
    /// <returns>An instance of <see cref="PropertyInfo"/> object.</returns>
    internal static PropertyInfo GetProperty<T>(Expression<Func<T, object?>> expression)
        where T : class
    {
        return expression.Body switch
        {
            UnaryExpression unaryExpression => GetProperty<T>(unaryExpression),
            MemberExpression memberExpression => GetProperty<T>(memberExpression),
            BinaryExpression binaryExpression => GetProperty<T>(binaryExpression),
            _ => throw new InvalidExpressionException($"Expression '{expression}' is not valid.")
        };
    }

    /// <summary>
    /// A helper method to return the instance of <see cref="PropertyInfo"/> object based on <see cref="BinaryExpression"/> object.
    /// </summary>
    /// <typeparam name="T">The target .NET CLR type.</typeparam>
    /// <param name="expression">The expression to be extracted.</param>
    /// <returns>An instance of <see cref="PropertyInfo"/> object.</returns>
    internal static PropertyInfo GetProperty<T>(BinaryExpression expression)
        where T : class
    {
        return expression.Left switch
        {
            MemberExpression memberExpression => GetProperty<T>(memberExpression),
            UnaryExpression unaryExpression => GetProperty<T>(unaryExpression),
            _ => throw new InvalidExpressionException($"Expression '{expression}' is not valid.")
        };
    }

    /// <summary>
    /// A helper method to return the instance of <see cref="PropertyInfo"/> object based on <see cref="UnaryExpression"/> object.
    /// </summary>
    /// <typeparam name="T">The target .NET CLR type.</typeparam>
    /// <param name="expression">The expression to be extracted.</param>
    /// <returns>An instance of <see cref="PropertyInfo"/> object.</returns>
    internal static PropertyInfo GetProperty<T>(UnaryExpression expression)
        where T : class
    {
        return expression.Operand switch
        {
            MemberExpression memberExpression => GetProperty<T>(memberExpression),
            BinaryExpression binaryExpression => GetProperty<T>(binaryExpression),
            _ => throw new InvalidExpressionException($"Expression '{expression}' is not valid.")
        };
    }

    /// <summary>
    /// A helper method to return the instance of <see cref="PropertyInfo"/> object based on <see cref="MemberExpression"/> object.
    /// </summary>
    /// <typeparam name="T">The target .NET CLR type.</typeparam>
    /// <param name="expression">The expression to be extracted.</param>
    /// <returns>An instance of <see cref="PropertyInfo"/> object.</returns>
    internal static PropertyInfo GetProperty<T>(MemberExpression expression)
        where T : class
    {
        if (expression.Member is PropertyInfo propertyInfo)
        {
            return propertyInfo;
        }
        throw new InvalidExpressionException($"Expression '{expression}' is not valid.");
    }


    #endregion


    private sealed class ReplaceVisitor : ExpressionVisitor
    {
        private readonly Expression _from;
        private readonly Expression _to;

        public ReplaceVisitor(Expression from, Expression to)
        {
            _from = from;
            _to = to;
        }

        [return: NotNullIfNotNull(nameof(node))]
        public override Expression? Visit(Expression? node)
        {
            return node == _from ? _to : base.Visit(node);
        }
    }

    /// <summary>
    /// Replaces all occurrences of a specified expression within an expression tree with another expression.
    /// </summary>
    /// <remarks>The replacement is performed recursively throughout the entire expression tree. The original
    /// expression tree is not modified.</remarks>
    /// <typeparam name="TExpression">The type of the expression tree. Must derive from Expression.</typeparam>
    /// <param name="expression">The expression tree in which to perform the replacement. Cannot be null.</param>
    /// <param name="from">The expression to search for within the expression tree. All matching instances will be replaced. Cannot be
    /// null.</param>
    /// <param name="to">The expression to substitute in place of each occurrence of the 'from' expression. Cannot be null.</param>
    /// <returns>A new expression tree of type TExpression with all occurrences of the specified expression replaced.</returns>
    internal static TExpression Replace<TExpression>(this TExpression expression, Expression from, Expression to)
        where TExpression : Expression
    {
        return (TExpression)new ReplaceVisitor(from, to).Visit(expression);
    }

    internal static Expression UnwrapUnary(this Expression expression, params ExpressionType[] types)
    {
        while (expression is UnaryExpression unaryExpression && types.Contains(unaryExpression.NodeType))
        {
            expression = unaryExpression.Operand;
        }
        return expression;
    }
}
