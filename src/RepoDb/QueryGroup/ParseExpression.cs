using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using RepoDb.Enumerations;
using RepoDb.Extensions;

namespace RepoDb;

public partial class QueryGroup
{
    /*
     * Others
     */

    /// <summary>
    ///
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    private static bool IsDirect(BinaryExpression expression) =>
        (
            expression.Left.NodeType == ExpressionType.Constant ||
            expression.Left.NodeType == ExpressionType.Convert ||
            expression.Left.NodeType == ExpressionType.MemberAccess
        )
        &&
        (
            expression.Right.NodeType == ExpressionType.Call ||
            expression.Right.NodeType == ExpressionType.Conditional ||
            expression.Right.NodeType == ExpressionType.Constant ||
            expression.Right.NodeType == ExpressionType.Convert ||
            expression.Right.NodeType == ExpressionType.MemberAccess ||
            expression.Right.NodeType == ExpressionType.NewArrayInit
        );

    /*
     * Expression
     */

    /// <summary>
    /// Parses a customized query expression.
    /// </summary>
    /// <typeparam name="TEntity">The target entity type</typeparam>
    /// <param name="expression">The expression to be converted to a <see cref="QueryGroup"/> object.</param>
    /// <returns>An instance of the <see cref="QueryGroup"/> object that contains the parsed query expression.</returns>
    public static QueryGroup Parse<TEntity>(Expression<Func<TEntity, bool>> expression, IDbConnection? connection = null, IDbTransaction? transaction = null, string? tableName = null)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(expression);

        // Parse the expression base on type
        var parsed = Parse<TEntity>(expression.Body) ?? throw new NotSupportedException($"Expression '{expression}' is currently not supported.");

        // Return the parsed values
        return parsed.Fix(connection, transaction, tableName ?? ClassMappedNameCache.Get<TEntity>());
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="expression"></param>
    /// <returns></returns>
    private static QueryGroup? Parse<TEntity>(Expression expression)
        where TEntity : class
    {
        return expression switch
        {
            LambdaExpression lambdaExpression => Parse<TEntity>(lambdaExpression.Body),
            BinaryExpression binaryExpression => Parse<TEntity>(binaryExpression),
            UnaryExpression unaryExpression => Parse<TEntity>(unaryExpression),
            MethodCallExpression methodCallExpression => Parse<TEntity>(methodCallExpression),
            MemberExpression memberExpression when (memberExpression.Type == StaticType.Boolean && memberExpression.Member is PropertyInfo) => ParseDirectBool<TEntity>(memberExpression),
            _ => null
        };
    }

    private static QueryGroup? ParseDirectBool<TEntity>(MemberExpression memberExpression)
        where TEntity : class
    {
        var qf = QueryField.Parse<TEntity>(memberExpression);

        if (qf is null)
            return null;

        return new QueryGroup(qf);
    }

    /*
     * Binary
     */

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="expression"></param>
    /// <returns></returns>
    private static QueryGroup Parse<TEntity>(BinaryExpression expression)
        where TEntity : class
    {
        // Check directness
        if (IsDirect(expression))
        {
            return QueryField.Parse<TEntity>(expression);
        }

        // Variables
        var leftQueryGroup = Parse<TEntity>(expression.Left) ?? throw new NotSupportedException($"Expression {expression.Left} is currently not supported");

        // IsNot
        if (expression.NodeType is ExpressionType.Equal or ExpressionType.NotEqual
            && expression.Right.Type == StaticType.Boolean && expression.IsExtractable() == true
            && expression.Right.GetValue() is bool rightValue)
        {
            var isNot = (expression.NodeType == ExpressionType.Equal && rightValue == false) ||
                (expression.NodeType == ExpressionType.NotEqual && rightValue == true);

            leftQueryGroup.SetIsNot(isNot);
        }
        else
        {
            var rightQueryGroup = Parse<TEntity>(expression.Right) ?? throw new NotSupportedException($"Expression {expression.Right} is currently not supported");
            return new QueryGroup([leftQueryGroup, rightQueryGroup], GetConjunction(expression));
        }

        // Return
        return leftQueryGroup;
    }

    /*
     * Unary
     */

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="expression"></param>
    /// <returns></returns>
    private static QueryGroup Parse<TEntity>(UnaryExpression expression)
        where TEntity : class
    {
        if (expression.NodeType == ExpressionType.Not || expression.NodeType == ExpressionType.Convert)
        {
            // These two handle
            if (expression.Operand is MemberExpression memberExpression && Parse<TEntity>(memberExpression, expression.NodeType) is { } r1)
                return r1;
            else if (expression.Operand is MethodCallExpression methodCallExpression && Parse<TEntity>(methodCallExpression, expression.NodeType) is { } r2)
                return r2;
        }

        if (Parse<TEntity>(expression.Operand) is { } r)
        {
            if (expression.NodeType == ExpressionType.Not)
            {
                // Wrap result in A NOT expression
                return new QueryGroup(r, true);
            }
            else
                throw new NotSupportedException($"Unary operation '{expression.NodeType}' is currently not supported.");
        }
        else
        {
            throw new NotSupportedException($"Unary operation '{expression.NodeType}' is currently not supported.");
        }
    }

    /*
     * Member
     */

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="expression"></param>
    /// <param name="unaryNodeType"></param>
    /// <returns></returns>
    private static QueryGroup? Parse<TEntity>(MemberExpression expression,
        ExpressionType? unaryNodeType = null)
        where TEntity : class
    {
        var queryFields = QueryField.Parse<TEntity>(expression, unaryNodeType);
        return queryFields != null ? new QueryGroup(queryFields) : null;
    }

    /*
     * MethodCall
     */

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="expression"></param>
    /// <returns></returns>
    private static QueryGroup? Parse<TEntity>(MethodCallExpression expression)
        where TEntity : class
    {
        var unaryNodeType = (expression.Object?.Type == StaticType.String) ? GetNodeType(expression.Object.ToMember()) :
            GetNodeType(expression.Arguments.LastOrDefault());
        return Parse<TEntity>(expression, unaryNodeType);
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="expression"></param>
    /// <param name="unaryNodeType"></param>
    /// <returns></returns>
    private static QueryGroup? Parse<TEntity>(MethodCallExpression expression,
        ExpressionType? unaryNodeType = null)
        where TEntity : class
    {
        var queryFields = QueryField.Parse<TEntity>(expression, unaryNodeType);
        return queryFields != null ? new QueryGroup(queryFields, GetConjunction(expression)) : null;
    }

    #region GetConjunction

    /// <summary>
    ///
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    private static Conjunction GetConjunction(BinaryExpression expression) => expression.NodeType switch
    {
        ExpressionType.Or or ExpressionType.OrElse => Conjunction.Or,
        ExpressionType.And or ExpressionType.AndAlso => Conjunction.And,
        _ => throw new NotSupportedException($"Unsupported expression for conjunction: {expression}")
    };

    /// <summary>
    ///
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    private static Conjunction GetConjunction(MethodCallExpression expression) =>
        expression.Method.Name == "Any" ? Conjunction.Or : Conjunction.And;

    #endregion

    #region GetNodeType

    /// <summary>
    ///
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    internal static ExpressionType? GetNodeType(Expression? expression)
    {
        return expression switch
        {
            null => null,
            LambdaExpression lambdaExpression => GetNodeType(lambdaExpression),
            BinaryExpression binaryExpression => GetNodeType(binaryExpression),
            MethodCallExpression methodCallExpression => GetNodeType(methodCallExpression),
            MemberExpression memberExpression => GetNodeType(memberExpression),
            _ => null
        };
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    internal static ExpressionType? GetNodeType(LambdaExpression expression) =>
        GetNodeType(expression.Body);

    /// <summary>
    ///
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    internal static ExpressionType? GetNodeType(BinaryExpression expression) =>
        expression.NodeType;

    /// <summary>
    ///
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    internal static ExpressionType? GetNodeType(MemberExpression expression) =>
        expression.NodeType;

    /// <summary>
    ///
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    internal static ExpressionType? GetNodeType(MethodCallExpression expression) =>
        expression.NodeType;

    #endregion
}
