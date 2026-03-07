using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Extensions.QueryFields;
using RepoDb.Resolvers;

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
    private static bool IsDirect<TEntity>(BinaryExpression expression) =>
        (
            expression.Left.NodeType == ExpressionType.Constant ||
            expression.Left.NodeType == ExpressionType.Convert ||
            (expression.Left is MemberExpression meLeft && meLeft.NodeType == ExpressionType.MemberAccess && meLeft.Expression is ParameterExpression && meLeft.Expression?.Type == typeof(TEntity))
        )
        &&
        (
            expression.Right.NodeType == ExpressionType.Call ||
            expression.Right.NodeType == ExpressionType.Conditional ||
            expression.Right.NodeType == ExpressionType.Constant ||
            expression.Right.NodeType == ExpressionType.Convert ||
            expression.Right.NodeType == ExpressionType.MemberAccess ||
            expression.Right.NodeType == ExpressionType.NewArrayInit ||
            (expression.Right.NodeType == ExpressionType.MemberInit && expression.Right.GetValue() is { })
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
        parsed.Fix(connection, transaction, tableName ?? ClassMappedNameCache.Get<TEntity>());
        return parsed;
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
            MethodCallExpression methodCallExpression => ParseMCE(methodCallExpression),
            MemberExpression memberExpression when memberExpression.Type == StaticType.Boolean && memberExpression.Member is PropertyInfo => ParseDirectBool<TEntity>(memberExpression),
            _ => null
        };


        static QueryGroup? ParseMCE(MethodCallExpression expression)
        {
            var unaryNodeType = (expression.Object?.Type == StaticType.String) ? expression.Object.ToMember().NodeType :
                GetNodeType(expression.Arguments.LastOrDefault());
            return Parse<TEntity>(expression, unaryNodeType);
        }

        static ExpressionType? GetNodeType(Expression? expression)
        {
            return expression switch
            {
                null => null,
                LambdaExpression lambdaExpression => lambdaExpression.Body.NodeType,
                BinaryExpression binaryExpression => binaryExpression.NodeType,
                MethodCallExpression methodCallExpression => methodCallExpression.NodeType,
                MemberExpression memberExpression => memberExpression.NodeType,
                _ => null
            };
        }
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

    static readonly Lazy<MemberInfo?> VBCompareString = new(() =>
        (Type.GetType("Microsoft.VisualBasic.CompilerServices.Operators, Microsoft.VisualBasic.Core", false)
            ?? Type.GetType("Microsoft.VisualBasic.CompilerServices.Operators, Microsoft.VisualBasic", false)
        )?.GetMethod("CompareString", BindingFlags.Static | BindingFlags.Public));

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="expression"></param>
    /// <returns></returns>
    private static QueryGroup Parse<TEntity>(BinaryExpression expression)
        where TEntity : class
    {
        // Check directness (column-to-value, value-to-column, etc.)

        bool isSimpleCheck = expression.NodeType is ExpressionType.Equal or ExpressionType.NotEqual or ExpressionType.LessThan or ExpressionType.LessThanOrEqual or ExpressionType.GreaterThan or ExpressionType.GreaterThanOrEqual;

        if (IsDirect<TEntity>(expression))
        {
            // Column-to-column comparison: both sides are member accesses on the parameter
            if (expression.Left is MemberExpression leftMember && leftMember.Expression is ParameterExpression &&
                expression.Right is MemberExpression rightMember && rightMember.Expression is ParameterExpression)
            {
                var leftField = new Field(leftMember.Member.Name);
                var rightField = new Field(rightMember.Member.Name);
                var op = QueryField.GetOperation(expression.NodeType);
                var fieldComp = new Extensions.QueryFields.FieldComparisonQueryField(leftField, op, rightField);
                return new QueryGroup(fieldComp);
            }
            // If only right is a member on the parameter, but left is not, throw (unsupported)
            if (expression.Right is MemberExpression mx && mx.Expression is ParameterExpression)
                throw new NotSupportedException($"Comparing an entity to values on itself is not currently supported in {expression}");
            // Otherwise, normal column-to-value
            return QueryField.Parse<TEntity>(expression);
        }
        else if (expression.Left is MethodCallExpression m
            && expression.Right is ConstantExpression c && c.Value is int intVal && intVal == 0
            && isSimpleCheck
            && ((m.Method.Name is nameof(string.Compare) or nameof(string.CompareTo) && m.Method.DeclaringType == StaticType.String) || m.Method == VBCompareString.Value))
        {
            var propExpr = m.Object is { } ob ? ob : m.Arguments[0];
            var property = QueryField.GetProperty<TEntity>(propExpr) ?? throw new NotSupportedException($"Expression {propExpr} in {expression} is currently not supported");
            var value = m.Object is { } ? m.Arguments[0].GetValue() : m.Arguments[1].GetValue();

            return new QueryGroup(new QueryField(property.AsField(),
                expression.NodeType switch
                {
                    ExpressionType.Equal => Operation.Equal,
                    ExpressionType.NotEqual => Operation.NotEqual,
                    ExpressionType.LessThan => Operation.LessThan,
                    ExpressionType.LessThanOrEqual => Operation.LessThanOrEqual,
                    ExpressionType.GreaterThan => Operation.GreaterThan,
                    ExpressionType.GreaterThanOrEqual => Operation.GreaterThanOrEqual,
                    _ => throw new InvalidOperationException()
                }, value, dbType: null).AsEnumerable());
        }
        else if (expression.Left is MethodCallExpression m2
            && m2.Method.Name is nameof(JsonQueryExtensions.ExtractValue) && m2.Method.DeclaringType == typeof(JsonQueryExtensions)
            && isSimpleCheck
            && QueryField.GetProperty<TEntity>(m2.Arguments[0]) is { } propExpr)
        {
            var pathArg = m2.Arguments[1];
            var jsonPath = pathArg is { NodeType: ExpressionType.Quote } ? JsonExtractQueryField.ParsePath(((UnaryExpression)pathArg).Operand) : pathArg.GetValue() as string;
            ArgumentNullException.ThrowIfNull(jsonPath);
            var vv = QueryField.Parse<TEntity>(expression).GetFields(false)!.Single();

            var rightType = expression.Right.Type;
            return new QueryGroup([new JsonExtractQueryField(vv.Field!.FieldName, jsonPath, QueryField.GetOperation(expression.NodeType), vv.GetValue(), dbType: TypeMapCache.Get(rightType) ?? ClientTypeToDbTypeResolver.Instance.Resolve(rightType))]);
        }
        else if (expression.Left is MethodCallExpression m3
            && m3.Object is { }
            && m3.Method.DeclaringType == StaticType.String
            && m3.Method.Name is nameof(string.Trim) or nameof(string.TrimStart) or nameof(string.TrimEnd) or nameof(string.ToUpper) or nameof(string.ToLower) or nameof(string.ToUpperInvariant) or nameof(string.ToLowerInvariant)
            && isSimpleCheck
            && QueryField.GetProperty<TEntity>(m3.Object) is { } propExpr3)
        {
            var value = expression.Right.GetValue();

            QueryField qf = m3.Method.Name switch
            {
                nameof(string.Trim) => new TrimQueryField(propExpr3.AsField().FieldName, QueryField.GetOperation(expression.NodeType), value),
                nameof(string.TrimStart) => new LeftTrimQueryField(propExpr3.AsField().FieldName, QueryField.GetOperation(expression.NodeType), value),
                nameof(string.TrimEnd) => new RightTrimQueryField(propExpr3.AsField().FieldName, QueryField.GetOperation(expression.NodeType), value),
                nameof(string.ToUpper) or nameof(string.ToUpperInvariant) => new UpperQueryField(propExpr3.AsField().FieldName, QueryField.GetOperation(expression.NodeType), value),
                nameof(string.ToLower) or nameof(string.ToLowerInvariant) => new LowerQueryField(propExpr3.AsField().FieldName, QueryField.GetOperation(expression.NodeType), value),
                _ => throw new NotImplementedException()
            };

            return new QueryGroup(qf.AsEnumerable());
        }
        else if (expression.Left is MemberExpression m4
            && m4.Expression is { }
            && m4.Member.DeclaringType == StaticType.String
            && m4.Member.Name is nameof(string.Length)
            && isSimpleCheck
            && QueryField.GetProperty<TEntity>(m4.Expression) is { } propExpr4)
        {
            return new QueryGroup(new LengthQueryField(propExpr4.AsField().FieldName, QueryField.GetOperation(expression.NodeType), expression.Right.GetValue()).AsEnumerable());
        }
        else if (isSimpleCheck
            && GetJsonExtractFromPath<TEntity>(expression.Left) is { FieldName: not null, Path: not null } jsonExtract)
        {
            var rightType = expression.Right.Type;
            return new QueryGroup([new JsonExtractQueryField(jsonExtract.FieldName, jsonExtract.Path, expression.Right.GetValue(), dbType: TypeMapCache.Get(rightType) ?? ClientTypeToDbTypeResolver.Instance.Resolve(rightType))]);
        }

        // Otherwise, recursively parse as before (for AndAlso, OrElse, etc.)
        var leftQueryGroup = Parse<TEntity>(expression.Left) ?? throw new NotSupportedException($"Expression {expression.Left} in {expression} is currently not supported");

        // IsNot
        if (expression.NodeType is ExpressionType.Equal or ExpressionType.NotEqual
            && expression.Right.Type == StaticType.Boolean && expression.IsExtractable() && expression.Right.GetValue() is bool rightValue)
        {
            var isNot = (expression.NodeType == ExpressionType.Equal && !rightValue) ||
                (expression.NodeType == ExpressionType.NotEqual && rightValue);

            leftQueryGroup.SetIsNot(isNot);
        }
        else
        {
            var rightQueryGroup = Parse<TEntity>(expression.Right) ?? throw new NotSupportedException($"Expression {expression.Right} in {expression} is currently not supported");
            return new QueryGroup([leftQueryGroup, rightQueryGroup], GetConjunction(expression));
        }

        // Return the left query group, which is now modified to include the right side
        return leftQueryGroup;
    }

    sealed class ReplaceVisitor : ExpressionVisitor
    {
        private readonly Expression _from;
        private readonly Expression _to;

        public ReplaceVisitor(Expression from, Expression to)
        {
            _from = from;
            _to = to;
        }

        public override Expression? Visit(Expression? node)
        {
            return node == _from ? _to : base.Visit(node);
        }
    }


    private static (string? FieldName, string? Path) GetJsonExtractFromPath<TEntity>(Expression left) where TEntity : class
    {
        var e = left;
        Expression path;

        while (e != null)
        {
            Expression? next;
            if (e is MemberExpression me)
                next = me.Expression;
            else if (e is BinaryExpression be && be.NodeType == ExpressionType.ArrayIndex)
                next = be.Left;
            else
                return (null, null);

            if (next is MemberExpression p && p.Member.DeclaringType?.IsGenericType == true && p.Member.DeclaringType.GetGenericTypeDefinition() == typeof(DbJsonValue<>))
            {
                var arg = Expression.Parameter(p.Member.DeclaringType.GetGenericArguments()[0], "e");
                path = new ReplaceVisitor(next, arg).Visit(left)!;

                return (p.Expression is { } ? QueryField.GetProperty<TEntity>(p.Expression)?.FieldName : null, JsonExtractQueryField.ParsePath(Expression.Lambda(path, arg)));
            }
            e = next;
        }

        return (null, null);
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
            if (expression.Operand is MemberExpression memberExpression && ParseME(memberExpression, expression.NodeType) is { } r1)
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

        static QueryGroup? ParseME(MemberExpression expression, ExpressionType unaryNodeType)
        {
            var queryFields = QueryField.Parse<TEntity>(expression, unaryNodeType);
            return queryFields != null ? new QueryGroup(queryFields) : null;
        }
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
}
