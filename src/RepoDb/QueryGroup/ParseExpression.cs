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
            MethodCallExpression methodCallExpression => Parse<TEntity>(methodCallExpression, false),
            MemberExpression memberExpression when memberExpression.Type == StaticType.Boolean && memberExpression.Member is PropertyInfo && expression.TryGetField(out var fld) => new QueryField(fld, Operation.Equal, true, null),
            _ => null
        };
    }

    /*
     * MethodCall
     */

    internal static QueryGroup? Parse<TEntity>(MethodCallExpression expression, bool isNot = false)
    where TEntity : class
    {
        if (expression.Method.Name == nameof(string.Equals))
        {
            return ParseEquals<TEntity>(expression).Not(isNot);
        }
        else if (expression.Method.Name == "CompareString")
        {
            // Usual case for VB.Net (Microsoft.VisualBasic.CompilerServices.Operators.CompareString #767)
            return ParseCompareString<TEntity>(expression).Not(isNot);
        }
        else if (expression.Method.Name == nameof(string.Contains))
        {
            return ParseContains<TEntity>(expression, isNot);
        }
        else if (expression.Method.Name is nameof(string.StartsWith) or nameof(string.EndsWith))
        {
            return ParseStartEndsWith<TEntity>(expression, isNot);
        }
        else if (expression.Method.Name == nameof(Enumerable.All))
        {
            return ParseAll<TEntity>(expression)?.Not(isNot);
        }
        else if (expression.Method.Name == nameof(Enumerable.Any))
        {
            return ParseAny<TEntity>(expression)?.Not(isNot);
        }
        else
            return null;
    }

    internal static QueryGroup ParseEquals<TEntity>(MethodCallExpression expression)
        where TEntity : class
    {
        if (expression.Object is null // string.Equals(field, arg.prop)
            && expression.Method.DeclaringType == typeof(string)
            && expression.Arguments.Count == 2
            && expression.Arguments[0].TryGetField(out var field)
            && expression.Arguments[1].TryGetValue(out var pv))
        {
            return new QueryField(field, Converter.ToType<string>(pv));
        }
        else if (expression.Object?.Type == typeof(string) // field.Equals(arg.prop)
            && expression.Object.TryGetField(out var f2)
            && expression.Arguments.Count >= 1
            && expression.Arguments[0].TryGetValue(out var v2))
        {
            return new QueryField(f2, v2);
        }
        else if (expression.Object is null // extension method: Equals(field, arg.prop)
            && expression.Arguments.Count >= 2
            && expression.Arguments[0].TryGetField(out var f3)
            && expression.Arguments[1].TryGetValue(out var v3))
        {
            return new QueryField(f3, v3);
        }

        throw new InvalidOperationException($"Can't parse '{expression}' to query");
    }

    internal static QueryGroup ParseCompareString<TEntity>(MethodCallExpression expression)
        where TEntity : class
    {
        // Property
        var property = (expression.Arguments[0] as MemberExpression)?.Member ?? throw new InvalidOperationException($"Can't parse '{expression}' to entity property");

        // Value
        if (!expression.Arguments[1].TryGetValue(out var vv))
            throw new InvalidOperationException($"Can't parse {expression.Arguments[1]} to value");

        var value = Converter.ToType<string>(vv);

        // Return
        if (property is PropertyInfo pi
            && PropertyCache.Get(pi.DeclaringType, pi, true) is { } mappedProperty)
        {
            return new QueryField(mappedProperty.AsField(), value);
        }
        return new QueryField(property.GetMappedName(), value);
    }

    /// <summary>
    /// Parses variable.Contains(entity.Property) or entity.Propery.Contains(variable), directly on object or via extension method
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="expression"></param>
    /// <param name="isNot"></param>
    /// <returns></returns>
    internal static QueryGroup ParseContains<TEntity>(MethodCallExpression expression, bool isNot = false)
        where TEntity : class
    {
        // Value. The list to check in
        var listExpression = expression.Object ?? expression.Arguments[0];
        var memberExpression = expression.Object != null ? expression.Arguments[0] : expression.Arguments[1];

        if (listExpression.Type != StaticType.String)
        {
            // Handling variable.Contains(entity.Property)
            // Property. The argument of List.Contains(<what>, ...) or the second argument of Extension.Contains(list, <what>, ...)
            var valueExpression = listExpression;
            var propExpression = memberExpression;

            if (!propExpression.TryGetField(out var field))
                throw new InvalidOperationException($"Can't parse '{propExpression}' to entity property");

            if (!valueExpression.TryGetValue(out var listValue))
                throw new InvalidOperationException($"Can't parse {valueExpression} to list value");

            var enumerable = Converter.ToType<System.Collections.IEnumerable>(listValue);
            return QueryField.ToIn(field, enumerable!, isNot);
        }
        else
        {
            // Handling entity.Property.Contains(variable)

            var valueExpression = memberExpression;
            var propExpression = listExpression;

            if (!propExpression.TryGetField(out var field))
                throw new InvalidOperationException($"Can't parse '{propExpression}' to entity property");

            if (!valueExpression.TryGetValue(out var value))
                throw new InvalidOperationException($"Can't parse {valueExpression} to needle value");

            var likeable = QueryField.ConvertToLikeableValue("Contains", Converter.ToType<string>(value ?? ""));
            return QueryField.ToLike(field, likeable, isNot);
        }
    }

    /// <summary>
    /// Parses entity.Property.StartsWith(...)
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="expression"></param>
    /// <param name="isNot"></param>
    /// <returns></returns>
    internal static QueryGroup ParseStartEndsWith<TEntity>(MethodCallExpression expression, bool isNot = false)
        where TEntity : class
    {
        // Property
        var propertyExpression = expression.Object ?? expression.Arguments[0];
        var matchExpression = expression.Object != null ? expression.Arguments[0] : expression.Arguments[1];

        if (!propertyExpression.TryGetField(out var field))
            throw new InvalidOperationException($"Can't parse '{propertyExpression}' to entity property");

        // Values
        if (!matchExpression.TryGetValue(out var needleValue))
            throw new InvalidOperationException($"Can't parse {matchExpression} to needle value");
        var value = Converter.ToType<string>(needleValue);

        // Fields
        return QueryField.ToLike(field,
            QueryField.ConvertToLikeableValue(expression.Method.Name, value ?? ""), isNot);
    }

    internal static QueryGroup? ParseAll<TEntity>(MethodCallExpression expression)
        where TEntity : class
    {
        if (!expression.Arguments[0].TryGetValue(out var listValue))
            throw new InvalidOperationException($"Can't parse {expression.Arguments[0]} to list value");

        // Lets see if we have something like <list>.All(x => x != e.Field), which we can fold into x IN <list>
        if (expression.Method.DeclaringType == typeof(Enumerable)
            && expression.Arguments.Count >= 2
            && expression.Arguments[1] is LambdaExpression lmb
            && lmb.Body is BinaryExpression { NodeType: ExpressionType.NotEqual or ExpressionType.Equal } be
            && be.Left == lmb.Parameters.First() && be.Right.TryGetField(out var f1))
        {
            if (be.NodeType == ExpressionType.NotEqual)
                return new QueryField(f1, Operation.NotIn, Converter.ToType<System.Collections.IEnumerable>(listValue), dbType: null);
            else
                return And(ToQueryFields(f1, Converter.ToType<System.Collections.IEnumerable>(listValue)!, Operation.Equal));
        }

        return null;
    }

    internal static QueryGroup? ParseAny<TEntity>(MethodCallExpression expression)
        where TEntity : class
    {

        if (!expression.Arguments[0].TryGetValue(out var listValue))
            throw new InvalidOperationException($"Can't parse {expression.Arguments[0]} to list value");

        // Lets see if we have something like <list>.Any(x => x == e.Field), which we can fold into x IN <list>
        if (expression.Method.DeclaringType == typeof(Enumerable)
            && expression.Arguments.Count >= 2
            && expression.Arguments[1] is LambdaExpression lmb
            && lmb.Body is BinaryExpression { NodeType: ExpressionType.Equal or ExpressionType.NotEqual } be
            && be.Left == lmb.Parameters.First() && be.Right.TryGetField(out var f1))
        {
            if (be.NodeType == ExpressionType.Equal)
                return new QueryField(f1, Operation.In, Converter.ToType<System.Collections.IEnumerable>(listValue), dbType: null);
            else
                return Or(ToQueryFields(f1, Converter.ToType<System.Collections.IEnumerable>(listValue)!, Operation.NotEqual));
        }

        return null;
    }

    private static IEnumerable<QueryField> ToQueryFields(Field field,
        System.Collections.IEnumerable enumerable,
        Operation operation)
    {
        return enumerable.Cast<object>().Select(item => new QueryField(field, operation, item, null));
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
                expression.Right is MemberExpression rightMember && rightMember.Expression is ParameterExpression &&
                expression.Left.TryGetField(out var leftField) &&
                expression.Right.TryGetField(out var rightField))
        {
            var op = QueryField.GetOperation(expression.NodeType);
            return new FieldComparisonQueryField(leftField, op, rightField);
        }
        else if (expression.Left is MethodCallExpression m
            && expression.Right is ConstantExpression c && c.Value is int intVal && intVal == 0
            && isSimpleCheck
            && ((m.Method.Name is nameof(string.Compare) or nameof(string.CompareTo) && m.Method.DeclaringType == StaticType.String) || m.Method == VBCompareString.Value)
            && m.Arguments[m.Object is { } ? 0 : 1].TryGetValue(out var value))
        {
            var propExpr = m.Object is { } ob ? ob : m.Arguments[0];
            if (!propExpr.TryGetField(out var field))
                throw new NotSupportedException($"Expression {propExpr} in {expression} is currently not supported");

            return new QueryField(field,
                expression.NodeType switch
                {
                    ExpressionType.Equal => Operation.Equal,
                    ExpressionType.NotEqual => Operation.NotEqual,
                    ExpressionType.LessThan => Operation.LessThan,
                    ExpressionType.LessThanOrEqual => Operation.LessThanOrEqual,
                    ExpressionType.GreaterThan => Operation.GreaterThan,
                    ExpressionType.GreaterThanOrEqual => Operation.GreaterThanOrEqual,
                    _ => throw new InvalidOperationException()
                }, value, dbType: null);
        }
        else if (expression.Left is MethodCallExpression m2
            && m2.Method.Name is nameof(JsonQueryExtensions.ExtractValue) && m2.Method.DeclaringType == typeof(JsonQueryExtensions)
            && isSimpleCheck
            && m2.Arguments[0].TryGetField(out var propExpr)
            && m2.Arguments[1].TryGetValue(out var pathArg)
            && expression.Right.TryGetValue(out var vv))
        {
            var jsonPath = pathArg is Expression expr ? JsonExtractQueryField.ParsePath(expr) : (pathArg as string);
            ArgumentNullException.ThrowIfNull(jsonPath);

            var rightType = expression.Right.Type;
            return new JsonExtractQueryField(propExpr.FieldName, jsonPath, QueryField.GetOperation(expression.NodeType), vv, dbType: TypeMapCache.Get(rightType) ?? ClientTypeToDbTypeResolver.Instance.Resolve(rightType));
        }
        else if (expression.Left is MethodCallExpression m3
            && m3.Object is { }
            && m3.Method.DeclaringType == StaticType.String
            && m3.Method.Name is nameof(string.Trim) or nameof(string.TrimStart) or nameof(string.TrimEnd) or nameof(string.ToUpper) or nameof(string.ToLower) or nameof(string.ToUpperInvariant) or nameof(string.ToLowerInvariant) or nameof(string.Substring)
            && isSimpleCheck
            && m3.Object.TryGetField(out var propExpr3)
            && expression.Right.TryGetValue(out var rightValue2))
        {
            QueryField? qf = m3.Method.Name switch
            {
                nameof(string.Trim) => new TrimQueryField(propExpr3.FieldName, QueryField.GetOperation(expression.NodeType), rightValue2),
                nameof(string.TrimStart) => new LeftTrimQueryField(propExpr3.FieldName, QueryField.GetOperation(expression.NodeType), rightValue2),
                nameof(string.TrimEnd) => new RightTrimQueryField(propExpr3.FieldName, QueryField.GetOperation(expression.NodeType), rightValue2),
                nameof(string.ToUpper) or nameof(string.ToUpperInvariant) => new UpperQueryField(propExpr3.FieldName, QueryField.GetOperation(expression.NodeType), rightValue2),
                nameof(string.ToLower) or nameof(string.ToLowerInvariant) => new LowerQueryField(propExpr3.FieldName, QueryField.GetOperation(expression.NodeType), rightValue2),
                nameof(string.Substring) when m3.Arguments.Count == 2 && m3.Arguments[0].TryGetValue(out var a0) && m3.Arguments[1].TryGetValue(out var a1) && a0 is int v1 && v1 == 0 && a1 is int left => new LeftQueryField(propExpr3.FieldName, QueryField.GetOperation(expression.NodeType), rightValue2, dbType: null, left),
                _ => null
            };

            if (qf is { })
                return qf;
            // Fall through to cleaner error message
        }
        else if (expression.Left is MemberExpression m4
            && m4.Expression is { }
            && m4.Member.DeclaringType == StaticType.String
            && m4.Member.Name is nameof(string.Length)
            && isSimpleCheck
            && m4.Expression.TryGetField(out var propExpr4)
            && expression.Right.TryGetValue(out var rightValue4))
        {
            return new QueryGroup(new LengthQueryField(propExpr4.FieldName, QueryField.GetOperation(expression.NodeType), rightValue4).AsEnumerable());
        }
        else if (expression.Left is MemberExpression m5
            && m5.Expression is { }
            && m5.Member.DeclaringType?.IsDateTimeType() == true
            && m5.Member.Name is nameof(DateTime.Date) or nameof(DateTime.Year) or nameof(DateTime.Month) or nameof(DateTime.Day) or nameof(DateTime.Hour) or nameof(DateTime.Minute) or nameof(DateTime.Second) or nameof(DateTime.Millisecond)
            && m5.Expression.TryGetField(out var propExpr5)
            && expression.Right.TryGetValue(out var rightValue5))
        {
            return new DateTimePartQueryField(propExpr5.FieldName, QueryField.GetOperation(expression.NodeType), rightValue5, dateTimePart: m5.Member.Name switch
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
            });
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
            && expression.Right is ConstantExpression rightConst
            && rightConst.Type == typeof(bool)
            && rightConst.TryGetValue(out var rightRaw) && rightRaw is bool rightValue)
        {
            if ((expression.NodeType == ExpressionType.Equal && !rightValue) ||
                (expression.NodeType == ExpressionType.NotEqual && rightValue))
            {
                leftQueryGroup = new QueryGroup(leftQueryGroup, isNot: true);
            }
            return leftQueryGroup;
        }
        else
        {
            var rightQueryGroup = Parse<TEntity>(expression.Right) ?? throw new NotSupportedException($"Expression {expression.Right} in {expression} is currently not supported");
            return new QueryGroup([leftQueryGroup, rightQueryGroup],
                expression.NodeType switch
                {
                    ExpressionType.Or or ExpressionType.OrElse => Conjunction.Or,
                    ExpressionType.And or ExpressionType.AndAlso => Conjunction.And,
                    _ => throw new NotSupportedException($"Unsupported expression type {expression.NodeType} for conjunction: {expression}")
                });
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
                path = left.Replace(next, arg);

                return (p.Expression is { } ? p.Expression.TryGetField(out var field) ? field.FieldName : null : null, JsonExtractQueryField.ParsePath(Expression.Lambda(path, arg)));
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
        if (expression.NodeType is not ExpressionType.Not and not ExpressionType.Convert)
        {
            throw new NotSupportedException($"Unary operation '{expression.NodeType}' is currently not supported.");
        }

        if (Parse<TEntity>(expression.Operand) is { } r)
        {
            return r.Not(expression.NodeType == ExpressionType.Not);
        }
        else
            throw new NotSupportedException($"Expression '{expression.Operand}' is currently not supported.");
    }
}
