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



    private static bool IsDirect<TEntity>(BinaryExpression expression)
    {
        return
        expression.IsExtractable() &&
        (
            (DropCoalesce(expression.Left.UnwrapUnary(ExpressionType.Convert)) is MemberExpression meLeft && meLeft.NodeType == ExpressionType.MemberAccess && meLeft.Expression is ParameterExpression && meLeft.Expression?.Type == typeof(TEntity)
            && expression.Right.TryGetValue(out _))

        ||
            (expression.Right.UnwrapUnary(ExpressionType.Convert) is MemberExpression meRight && meRight.NodeType == ExpressionType.MemberAccess && meRight.Expression is ParameterExpression && meRight.Expression?.Type == typeof(TEntity)
            && expression.Left.TryGetValue(out _))
        );


        static Expression DropCoalesce(Expression x)
            => x is BinaryExpression { NodeType: ExpressionType.Coalesce } be && be.Right.TryGetValue(out _)
                ? be.Left.UnwrapUnary(ExpressionType.Convert)
                : x;
    }
    /*
     * Expression
     */

    /// <summary>
    /// Parses a customized query expression.
    /// </summary>
    /// <typeparam name="TEntity">The target entity type</typeparam>
    /// <param name="expression">The expression to be converted to a <see cref="QueryGroup"/> object.</param>
    /// <param name="connection"></param>
    /// <param name="transaction"></param>
    /// <param name="tableName"></param>
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
            var unaryNodeType = (expression.Object?.Type == StaticType.String) ? ((MemberExpression)expression.Object).NodeType :
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

    private static readonly Lazy<MemberInfo?> VBCompareString = new(() =>
        (Type.GetType("Microsoft.VisualBasic.CompilerServices.Operators, Microsoft.VisualBasic.Core", false)
            ?? Type.GetType("Microsoft.VisualBasic.CompilerServices.Operators, Microsoft.VisualBasic", false)
        )?.GetMethod("CompareString", BindingFlags.Static | BindingFlags.Public));

    private static QueryGroup Parse<TEntity>(BinaryExpression expression)
        where TEntity : class
    {
        // Check directness (column-to-value, value-to-column, etc.)

        bool isSimpleCheck = expression.NodeType is ExpressionType.Equal or ExpressionType.NotEqual or ExpressionType.LessThan or ExpressionType.LessThanOrEqual or ExpressionType.GreaterThan or ExpressionType.GreaterThanOrEqual;

        if (IsDirect<TEntity>(expression))
        {
            // normal column-to-value
            return QueryField.Parse<TEntity>(expression);
        }
        else if (expression.Left is MemberExpression leftMember && leftMember.Expression is ParameterExpression &&
                expression.Right is MemberExpression rightMember && rightMember.Expression is ParameterExpression)
        {
            var leftField = new Field(leftMember.Member.Name);
            var rightField = new Field(rightMember.Member.Name);
            var op = QueryField.GetOperation(expression.NodeType);
            var fieldComp = new Extensions.QueryFields.FieldComparisonQueryField(leftField, op, rightField);
            return new QueryGroup(fieldComp);
        }
        else if (expression.Left is MethodCallExpression m
            && expression.Right is ConstantExpression c && c.Value is int intVal && intVal == 0
            && isSimpleCheck
            && ((m.Method.Name is nameof(string.Compare) or nameof(string.CompareTo) && m.Method.DeclaringType == StaticType.String) || m.Method == VBCompareString.Value)
            && m.Arguments[m.Object is { } ? 0 : 1].TryGetValue(out var value))
        {
            var propExpr = m.Object is { } ob ? ob : m.Arguments[0];
            var property = QueryField.GetProperty<TEntity>(propExpr) ?? throw new NotSupportedException($"Expression {propExpr} in {expression} is currently not supported");

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
            && QueryField.GetProperty<TEntity>(m2.Arguments[0]) is { } propExpr
            && m2.Arguments[1].TryGetValue(out var pathArg))
        {
            var jsonPath = pathArg is Expression expr ? JsonExtractQueryField.ParsePath(expr) : (pathArg as string);
            ArgumentNullException.ThrowIfNull(jsonPath);
            var vv = QueryField.Parse<TEntity>(expression).GetFields(false)!.Single();

            var rightType = expression.Right.Type;
            return new QueryGroup([new JsonExtractQueryField(vv.Field!.FieldName, jsonPath, QueryField.GetOperation(expression.NodeType), vv.Value, dbType: TypeMapCache.Get(rightType) ?? ClientTypeToDbTypeResolver.Instance.Resolve(rightType))]);
        }
        else if (expression.Left is MethodCallExpression m3
            && m3.Object is { }
            && m3.Method.DeclaringType == StaticType.String
            && m3.Method.Name is nameof(string.Trim) or nameof(string.TrimStart) or nameof(string.TrimEnd) or nameof(string.ToUpper) or nameof(string.ToLower) or nameof(string.ToUpperInvariant) or nameof(string.ToLowerInvariant) or nameof(string.Substring)
            && isSimpleCheck
            && QueryField.GetProperty<TEntity>(m3.Object) is { } propExpr3
            && expression.Right.TryGetValue(out var rightValue2))
        {
            QueryField? qf = m3.Method.Name switch
            {
                nameof(string.Trim) => new TrimQueryField(propExpr3.AsField().FieldName, QueryField.GetOperation(expression.NodeType), rightValue2),
                nameof(string.TrimStart) => new LeftTrimQueryField(propExpr3.AsField().FieldName, QueryField.GetOperation(expression.NodeType), rightValue2),
                nameof(string.TrimEnd) => new RightTrimQueryField(propExpr3.AsField().FieldName, QueryField.GetOperation(expression.NodeType), rightValue2),
                nameof(string.ToUpper) or nameof(string.ToUpperInvariant) => new UpperQueryField(propExpr3.AsField().FieldName, QueryField.GetOperation(expression.NodeType), rightValue2),
                nameof(string.ToLower) or nameof(string.ToLowerInvariant) => new LowerQueryField(propExpr3.AsField().FieldName, QueryField.GetOperation(expression.NodeType), rightValue2),
                nameof(string.Substring) when m3.Arguments.Count == 2 && m3.Arguments[0].TryGetValue(out var a0) && m3.Arguments[1].TryGetValue(out var a1) && a0 is int v1 && v1 == 0 && a1 is int left => new LeftQueryField(propExpr3.AsField().FieldName, QueryField.GetOperation(expression.NodeType), rightValue2, dbType: null, left),
                _ => null
            };

            if (qf is { })
                return new QueryGroup(qf.AsEnumerable());
            // Fall through to cleaner error message
        }
        else if (expression.Left is MemberExpression m4
            && m4.Expression is { }
            && m4.Member.DeclaringType == StaticType.String
            && m4.Member.Name is nameof(string.Length)
            && isSimpleCheck
            && QueryField.GetProperty<TEntity>(m4.Expression) is { } propExpr4
            && expression.Right.TryGetValue(out var rightValue4))
        {
            return new QueryGroup(new LengthQueryField(propExpr4.AsField().FieldName, QueryField.GetOperation(expression.NodeType), rightValue4).AsEnumerable());
        }
        else if (expression.Left is MemberExpression m5
            && m5.Expression is { }
            && m5.Member.DeclaringType?.IsDateTimeType() == true
            && m5.Member.Name is nameof(DateTime.Date) or nameof(DateTime.Year) or nameof(DateTime.Month) or nameof(DateTime.Day) or nameof(DateTime.Hour) or nameof(DateTime.Minute) or nameof(DateTime.Second) or nameof(DateTime.Millisecond)
            && QueryField.GetProperty<TEntity>(m5.Expression) is { } propExpr5
            && expression.Right.TryGetValue(out var rightValue5))
        {
            return new QueryGroup(new DateTimePartQueryField(propExpr5.AsField().FieldName, QueryField.GetOperation(expression.NodeType), rightValue5, dateTimePart: m5.Member.Name switch
            {
                nameof(DateTime.Year) => DateTimePartType.Year,
                nameof(DateTime.Month) => DateTimePartType.Month,
                nameof(DateTime.Day) => DateTimePartType.Day,
                nameof(DateTime.Hour) => DateTimePartType.Hour,
                nameof(DateTime.Minute) => DateTimePartType.Minute,
                nameof(DateTime.Second) => DateTimePartType.Second,
                nameof(DateTime.Millisecond) => DateTimePartType.Millisecond,
                nameof(DateTime.Date) => DateTimePartType.Date,
                _ => throw new InvalidOperationException()
            }).AsEnumerable());
        }
        else if (isSimpleCheck
            && GetJsonExtractFromPath<TEntity>(expression.Left) is { FieldName: not null, Path: not null } jsonExtract
            && expression.Right.TryGetValue(out var rightValue6))
        {
            var rightType = expression.Right.Type;
            return new QueryGroup([new JsonExtractQueryField(jsonExtract.FieldName, jsonExtract.Path, QueryField.GetOperation(expression.NodeType), rightValue6, dbType: TypeMapCache.Get(rightType) ?? ClientTypeToDbTypeResolver.Instance.Resolve(rightType))]);
        }

        // Otherwise, recursively parse as before (for AndAlso, OrElse, etc.)
        var leftQueryGroup = Parse<TEntity>(expression.Left) ?? throw new NotSupportedException($"Expression {expression.Left} in {expression} is currently not supported");

        // IsNot
        if (expression.NodeType is ExpressionType.Equal or ExpressionType.NotEqual
            && expression.Right.Type == StaticType.Boolean && expression.IsExtractable()
            && expression.Right.TryGetValue(out var rightRaw) && rightRaw is bool rightValue)
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
                path = left.Replace(next, arg);

                return (p.Expression is { } ? QueryField.GetProperty<TEntity>(p.Expression)?.FieldName : null, JsonExtractQueryField.ParsePath(Expression.Lambda(path, arg)));
            }
            e = next;
        }

        return (null, null);
    }



    /*
     * Unary
     */

    private static QueryGroup Parse<TEntity>(UnaryExpression expression)
        where TEntity : class
    {
        if (expression.NodeType is ExpressionType.Not or ExpressionType.Convert)
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

    private static QueryGroup? Parse<TEntity>(MethodCallExpression expression,
        ExpressionType? unaryNodeType = null)
        where TEntity : class
    {
        var queryFields = QueryField.Parse<TEntity>(expression, unaryNodeType);
        return queryFields != null ? new QueryGroup(queryFields, GetConjunction(expression)) : null;
    }

    #region GetConjunction

    private static Conjunction GetConjunction(BinaryExpression expression) => expression.NodeType switch
    {
        ExpressionType.Or or ExpressionType.OrElse => Conjunction.Or,
        ExpressionType.And or ExpressionType.AndAlso => Conjunction.And,
        _ => throw new NotSupportedException($"Unsupported expression for conjunction: {expression}")
    };

    private static Conjunction GetConjunction(MethodCallExpression expression) =>
        expression.Method.Name == "Any" ? Conjunction.Or : Conjunction.And;

    #endregion
}
