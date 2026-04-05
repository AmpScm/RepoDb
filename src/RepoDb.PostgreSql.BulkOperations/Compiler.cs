using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Npgsql;
using NpgsqlTypes;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb.PostgreSql.BulkOperations;

/// <summary>
/// An internal compiler class used to compile necessary expressions that is needed to enhance the code execution.
/// </summary>
internal static class Compiler
{
    #region GetNpgsqlBinaryImporterWriteFunc (Mappings)

    internal static Action<NpgsqlBinaryImporter, TEntity> GetNpgsqlBinaryImporterWriteFunc<TEntity>(string tableName,
        IEnumerable<NpgsqlBulkInsertMapItem> mappings,
        Type entityType)
        where TEntity : class =>
        GetNpgsqlBinaryImporterWriteFuncCache<TEntity>.Get(tableName, mappings, entityType);

    private static class GetNpgsqlBinaryImporterWriteFuncCache<TEntity>
        where TEntity : class
    {
        private static readonly ConcurrentDictionary<int, Action<NpgsqlBinaryImporter, TEntity>> cache = new();

        /// <summary>
        ///
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="mappings"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static Action<NpgsqlBinaryImporter, TEntity> Get(string tableName,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            Type entityType) =>
            GetFunc(tableName, mappings, entityType);

        private static Action<NpgsqlBinaryImporter, TEntity> GetFunc(string tableName,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            Type entityType)
        {
            var targetTableName = tableName ?? ClassMappedNameCache.Get<TEntity>();
            var hashCode = GetHashCode<TEntity>(targetTableName, mappings);

            if (cache.TryGetValue(hashCode, out var value))
            {
                return value;
            }

            // Entity types (covering the anonymous)
            var typeOfEntity = typeof(TEntity);
            entityType ??= typeOfEntity;

            // Variables
            var importerType = typeof(NpgsqlBinaryImporter);
            var importerParameterExpression = Expression.Parameter(importerType, "importer");
            var entityParameterExpression = Expression.Parameter(typeOfEntity, "entity");
            var expressions = new List<Expression>();

            // Anonymous
            var entityExpression = (Expression)entityParameterExpression;
            if (typeOfEntity != entityType)
            {
                entityExpression = Expression.Convert(entityParameterExpression, entityType);
            }

            // Mappings
            foreach (var mapping in mappings)
            {
                var entityPropertyExpression = GetEntityPropertyExpression(entityExpression, entityType, mapping);
                var propertyExpression = Expression.Convert(entityPropertyExpression, typeof(object));
                var parameters = mapping.NpgsqlDbType is not null and not NpgsqlDbType.Unknown ?
                    new Expression[]
                    {
                        propertyExpression,
                        Expression.Constant(mapping.NpgsqlDbType.Value)
                    } :
                    new[] { propertyExpression };
                var writeMethod = mapping.NpgsqlDbType is not null and not NpgsqlDbType.Unknown ?
                    GetNpgsqlBinaryImporterWriteWithNpgsqlDbTypeMethod() : GetNpgsqlBinaryImporterWriteMethod();

                expressions.Add(Expression.Call(importerParameterExpression, writeMethod.MakeGenericMethod(new[] { typeof(object) }), parameters));
            }

            // Check
            Action<NpgsqlBinaryImporter, TEntity> func;
            if (expressions.Count > 0)
            {
                func = Expression
                    .Lambda<Action<NpgsqlBinaryImporter, TEntity>>(Expression.Block(expressions), importerParameterExpression, entityParameterExpression)
                    .Compile();
            }
            else
            {
                throw new InvalidOperationException($"There are no compiled expressions found for '{entityType.FullName}'. " +
                    $"Please check whether you had provided the proper 'mappings' or ensure that the entity properties are " +
                    $"matching with the table columns.");
            }

            // Cache
            if (cache.TryAdd(hashCode, func))
            {
                return func;
            }

            // Throw an error
            throw new InvalidOperationException($"Failed to add a compiled '{importerType.FullName}.Write' function for '{tableName}'.");
        }
    }

    #endregion

    #region GetNpgsqlBinaryImporterWriteAsyncFunc (Mappings)

    internal static Func<NpgsqlBinaryImporter, TEntity, CancellationToken, Task> GetNpgsqlBinaryImporterWriteAsyncFunc<TEntity>(string tableName,
        IEnumerable<NpgsqlBulkInsertMapItem> mappings,
        Type entityType)
        where TEntity : class =>
        GetNpgsqlBinaryImporterWriteAsyncFuncCache<TEntity>.Get(tableName, mappings, entityType);

    private static class GetNpgsqlBinaryImporterWriteAsyncFuncCache<TEntity>
        where TEntity : class
    {
        private static readonly ConcurrentDictionary<int,
            Func<NpgsqlBinaryImporter, TEntity, CancellationToken, Task>> cache = new();

        /// <summary>
        ///
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="mappings"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static Func<NpgsqlBinaryImporter, TEntity, CancellationToken, Task> Get(string tableName,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            Type entityType) =>
            GetFunc(tableName, mappings, entityType);

        private static Func<NpgsqlBinaryImporter, TEntity, CancellationToken, Task> GetFunc(string tableName,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            Type entityType)
        {
            var targetTableName = tableName ?? ClassMappedNameCache.Get<TEntity>();
            var hashCode = GetHashCode<TEntity>(targetTableName, mappings);

            if (cache.TryGetValue(hashCode, out var value))
            {
                return value;
            }

            // Entity types (covering the anonymous)
            var typeOfEntity = typeof(TEntity);
            entityType ??= typeOfEntity;

            // Variables
            var importerType = typeof(NpgsqlBinaryImporter);
            var importerParameterExpression = Expression.Parameter(importerType, "importer");
            var entityParameterExpression = Expression.Parameter(typeOfEntity, "entity");
            var cancellationTokenExpression = Expression.Parameter(typeof(CancellationToken), "cancellationToken");
            var expressions = new List<Expression>();

            // Anonymous
            var entityExpression = (Expression)entityParameterExpression;
            if (typeOfEntity != entityType)
            {
                entityExpression = Expression.Convert(entityParameterExpression, entityType);
            }

            // Mappings
            foreach (var mapping in mappings)
            {
                var entityPropertyExpression = GetEntityPropertyExpression(entityExpression, entityType, mapping);
                var propertyExpression = Expression.Convert(entityPropertyExpression, typeof(object));
                var parameters = mapping.NpgsqlDbType is not null and not NpgsqlDbType.Unknown ?
                    new Expression[] { propertyExpression, Expression.Constant(mapping.NpgsqlDbType), cancellationTokenExpression } :
                    [propertyExpression, cancellationTokenExpression];
                var writeMethod = mapping.NpgsqlDbType is not null and not NpgsqlDbType.Unknown ?
                    GetNpgsqlBinaryImporterWriteAsyncWithNpgsqlDbTypeMethod() : GetNpgsqlBinaryImporterWriteAsyncMethod();

                expressions.Add(Expression.Call(importerParameterExpression, writeMethod.MakeGenericMethod(new[] { typeof(object) }), parameters));
            }

            // Check
            Func<NpgsqlBinaryImporter, TEntity, CancellationToken, Task> func;
            if (expressions.Count > 0)
            {
                func = Expression
                    .Lambda<Func<NpgsqlBinaryImporter, TEntity, CancellationToken, Task>>(Expression.Block(expressions),
                        importerParameterExpression, entityParameterExpression, cancellationTokenExpression)
                    .Compile();
            }
            else
            {
                throw new InvalidOperationException($"There are no compiled expressions found for '{entityType.FullName}'. " +
                    $"Please check whether you had provided the proper 'mappings' or ensure that the entity properties are " +
                    $"matching with the table columns.");
            }

            // Cache
            if (cache.TryAdd(hashCode, func))
            {
                return func;
            }

            // Throw an error
            throw new InvalidOperationException($"Failed to add a compiled '{importerType.FullName}.Write' function for '{tableName}'.");
        }
    }

    #endregion

    #region Helpers

    private static MethodInfo GetMethodInfo<TFrom>(Expression<Action<TFrom>> call) => ((MethodCallExpression)call.Body).Method;
    private static MethodInfo GetMethodInfo(Expression<Action> call) => ((MethodCallExpression)call.Body).Method;

    private static MethodInfo GetNpgsqlBinaryImporterWriteMethod() =>
        GetMethodInfo<NpgsqlBinaryImporter>(v => v.Write<int>(default!)).GetGenericMethodDefinition();

    private static MethodInfo GetNpgsqlBinaryImporterWriteWithNpgsqlDbTypeMethod() =>
        GetMethodInfo<NpgsqlBinaryImporter>(v => v.Write<int>(default, NpgsqlDbType.Integer)).GetGenericMethodDefinition();

    private static MethodInfo GetNpgsqlBinaryImporterWriteAsyncMethod() =>
        GetMethodInfo<NpgsqlBinaryImporter>(v => v.WriteAsync<int>(default, cancellationToken: default)).GetGenericMethodDefinition();
    private static MethodInfo GetNpgsqlBinaryImporterWriteAsyncWithNpgsqlDbTypeMethod() =>
        GetMethodInfo<NpgsqlBinaryImporter>(v => v.WriteAsync<int>(default, NpgsqlDbType.Integer, cancellationToken: default)).GetGenericMethodDefinition();

    private static Expression GetEntityPropertyExpression(Expression entityExpression,
        Type entityType,
        NpgsqlBulkInsertMapItem mapping)
    {
        // Property
        var classProperty = PropertyCache.Get(entityType, mapping.SourceColumn) ?? throw new PropertyNotFoundException(nameof(mapping), $"Property '{mapping.SourceColumn}' is not found from type '{entityType.FullName}'.");
        var propertyExpression = (Expression)Expression.Property(entityExpression, mapping.SourceColumn);

        // Enum
        if (TypeCache.Get(classProperty.PropertyInfo.PropertyType).UnderlyingType.IsEnum)
        {
            propertyExpression = GetEntityPropertyExpressionForEnum(propertyExpression, mapping.NpgsqlDbType);
        }

        // Return
        return propertyExpression;
    }

    private static Expression GetEntityPropertyExpressionForEnum(Expression propertyExpression,
        NpgsqlDbType? npgsqlDbType)
    {
        var expression = npgsqlDbType switch
        {
            NpgsqlDbType.Text => ConvertEnumExpressionToString(propertyExpression),
            NpgsqlDbType.Integer => ConvertEnumExpressionToIntBasedType(propertyExpression, typeof(int)),
            NpgsqlDbType.Bigint => ConvertEnumExpressionToIntBasedType(propertyExpression, typeof(long)),
            NpgsqlDbType.Smallint => ConvertEnumExpressionToIntBasedType(propertyExpression, typeof(short)),
            _ => propertyExpression
        };

        if (TypeCache.Get(propertyExpression.Type).IsNullable)
        {
            var underlyingType = TypeCache.Get(expression.Type).UnderlyingType;
            var nullableType = underlyingType.IsValueType ?
                typeof(Nullable<>).MakeGenericType(underlyingType) : underlyingType;
            var testExpression = Expression.Equal(Expression.Constant(null), propertyExpression);
            var trueExpression = Expression.Default(nullableType);
            var falseExpression = underlyingType.IsValueType && npgsqlDbType != NpgsqlDbType.Unknown ?
                Expression.New(nullableType.GetConstructor(new[] { underlyingType })!, expression)! : expression;
            expression = Expression.Condition(testExpression, trueExpression, falseExpression);
        }

        return expression;
    }

    private static MethodCallExpression ConvertEnumExpressionToString(Expression propertyExpression) =>
        Expression.Call(GetConvertToTypeMethod(typeof(string)),
            Expression.Convert(propertyExpression, typeof(object)));

    private static MethodCallExpression ConvertEnumExpressionToIntBasedType(Expression propertyExpression,
        Type intBasedType)
    {
        var cachedType = TypeCache.Get(propertyExpression.Type);
        var typeExpression = Expression.Constant(cachedType.UnderlyingType);
        var nameExpression = Expression.Call(GetEnumGetNameMethod(),
            Expression.Constant(cachedType.UnderlyingType),
            Expression.Convert(propertyExpression, typeof(object)));
        var ignoreCaseExpression = Expression.Constant(true);
        var valueExpression = Expression.Call(GetEnumParseMethod(), typeExpression, nameExpression, ignoreCaseExpression);
        return Expression.Call(GetConvertToTypeMethod(intBasedType), valueExpression);
    }

    private static MethodInfo GetEnumParseMethod() =>
        GetMethodInfo(() => Enum.Parse(default!, default!, default));

    private static MethodInfo GetEnumGetNameMethod() =>
        GetMethodInfo(() => Enum.GetName(default!, default!));

    private static MethodInfo GetConvertToTypeMethod(Type type) =>
        typeof(Convert).GetMethod($"To{type.Name}", new[] { typeof(object) })!;

    private static int GetHashCode(Type entityType,
        string? tableName) =>
        (tableName ?? ClassMappedNameCache.Get(entityType)).GetHashCode();

    private static int GetHashCode<TEntity>(string tableName,
        IEnumerable<NpgsqlBulkInsertMapItem> mappings) =>
        GetHashCode(typeof(TEntity), tableName, mappings);

    private static int GetHashCode(Type entityType,
        string tableName,
        IEnumerable<NpgsqlBulkInsertMapItem> mappings)
    {
        var hashCode = GetHashCode(entityType, tableName);

        if (mappings?.Any() == true)
        {
            foreach (var mapping in mappings)
            {
                hashCode = HashCode.Combine(hashCode, mapping);
            }
        }

        return hashCode;
    }


    #endregion


    #region GetPropertySetterFunc

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static Action<TEntity, object>? GetPropertySetterFunc<TEntity>(string propertyName)
        where TEntity : class =>
        PropertySetterFuncCache<TEntity>.GetFunc(PropertyCache.Get<TEntity>(propertyName, true) ?? throw new Exceptions.PropertyNotFoundException($"Property {propertyName} not found"));

    private static class PropertySetterFuncCache<TEntity>
        where TEntity : class
    {
        private static readonly ConcurrentDictionary<ClassProperty, Action<TEntity, object>> cache = new();

        public static Action<TEntity, object>? GetFunc(ClassProperty classProperty)
        {
            ArgumentNullException.ThrowIfNull(classProperty);

            return cache.GetOrAdd(classProperty, static classProperty =>
            {
                var entity = Expression.Parameter(typeof(TEntity), "entity");
                var value = Expression.Parameter(typeof(object), "value");
                var converted = Expression.Convert(value, classProperty.PropertyInfo.PropertyType);
                var body = (Expression)Expression.Call(entity, classProperty.PropertyInfo.SetMethod!, converted);

                return Expression
                    .Lambda<Action<TEntity, object>>(body, entity, value)
                    .Compile();
            });
        }
    }

    #endregion


}
