#nullable enable
using System.Data;
using System.Linq.Expressions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Requests;

namespace RepoDb;

/// <summary>
/// Contains the extension methods for <see cref="IDbConnection"/> object.
/// </summary>
public static partial class DbConnectionExtension
{
    #region Query<TEntity>

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
        string tableName,
        object what,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return QueryInternal<TEntity>(connection: connection,
            tableName: tableName,
            where: WhatToQueryGroup(typeof(TEntity), connection, what, transaction),
            fields: fields,
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity, TWhat>(this IDbConnection connection,
        string tableName,
        TWhat what,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
        where TWhat : notnull
    {
        return QueryInternal<TEntity>(connection: connection,
            tableName: tableName,
            where: WhatToQueryGroup(typeof(TEntity), connection, what, transaction),
            fields: fields,
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
        string tableName,
        Expression<Func<TEntity, bool>> where,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return QueryInternal<TEntity>(connection: connection,
            tableName: tableName,
            where: connection.ToQueryGroup(where, transaction, tableName),
            fields: fields,
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
        string tableName,
        QueryField where,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return QueryInternal<TEntity>(connection: connection,
            tableName: tableName,
            where: ToQueryGroup(where),
            fields: fields,
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<QueryField> where,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return QueryInternal<TEntity>(connection: connection,
            tableName: tableName,
            where: ToQueryGroup(where),
            fields: fields,
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
        string tableName,
        QueryGroup where,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return QueryInternal<TEntity>(connection: connection,
            tableName: tableName,
            where: where,
            fields: fields,
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
        object what,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return QueryInternal<TEntity>(connection: connection,
            tableName: ClassMappedNameCache.Get<TEntity>() ?? throw new ArgumentException($"Can't map {typeof(TEntity)} to tablename"),
            where: WhatToQueryGroup(typeof(TEntity), connection, what, transaction),
            fields: fields,
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity, TWhat>(this IDbConnection connection,
        TWhat what,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
        where TWhat : notnull
    {
        return QueryInternal<TEntity>(connection: connection,
            tableName: ClassMappedNameCache.Get<TEntity>() ?? throw new ArgumentException($"Can't map {typeof(TEntity)} to tablename"),
            where: WhatToQueryGroup(typeof(TEntity), connection, what, transaction),
            fields: fields,
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
        QueryField where,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return QueryInternal<TEntity>(connection: connection,
            tableName: ClassMappedNameCache.Get<TEntity>() ?? throw new ArgumentException($"Can't map {typeof(TEntity)} to tablename"),
            where: ToQueryGroup(where),
            fields: fields,
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
        Expression<Func<TEntity, bool>> where,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null) where TEntity : class
    {
        return QueryInternal<TEntity>(connection: connection,
            tableName: ClassMappedNameCache.Get<TEntity>() ?? throw new ArgumentException($"Can't map {typeof(TEntity)} to tablename"),
            where: connection.ToQueryGroup(where, transaction),
            fields: fields,
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
        IEnumerable<QueryField> where,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return QueryInternal<TEntity>(connection: connection,
            tableName: ClassMappedNameCache.Get<TEntity>() ?? throw new ArgumentException($"Can't map {typeof(TEntity)} to tablename"),
            where: ToQueryGroup(where),
            fields: fields,
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
        QueryGroup where,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return QueryInternal<TEntity>(connection: connection,
            tableName: ClassMappedNameCache.Get<TEntity>() ?? throw new ArgumentException($"Can't map {typeof(TEntity)} to tablename"),
            where: where,
            fields: fields,
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    internal static IEnumerable<TEntity> QueryInternal<TEntity>(this IDbConnection connection,
        string tableName,
        QueryGroup? where,
        IEnumerable<Field>? fields = null,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        // Ensure the fields
        fields ??= GetQualifiedFields<TEntity>() ??
            DbFieldCache.Get(connection, tableName, transaction)?.AsFields();

        // Return
        return QueryInternalBase<TEntity>(connection: connection,
            tableName: tableName,
            where: where,
            fields: fields,
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    #endregion

    #region QueryAsync<TEntity>

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
        string tableName,
        object what,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await QueryAsyncInternal<TEntity>(connection: connection,
            tableName: tableName,
            fields: fields,
            where: await WhatToQueryGroupAsync(typeof(TEntity), connection, what, transaction, cancellationToken).ConfigureAwait(false),
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static async Task<IEnumerable<TEntity>> QueryAsync<TEntity, TWhat>(this IDbConnection connection,
        string tableName,
        TWhat what,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
        where TWhat : notnull
    {
        return await QueryAsyncInternal<TEntity>(connection: connection,
            tableName: tableName,
            fields: fields,
            where: await WhatToQueryGroupAsync(typeof(TEntity), connection, what, transaction, cancellationToken).ConfigureAwait(false),
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
        string tableName,
        Expression<Func<TEntity, bool>> where,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await QueryAsyncInternal<TEntity>(connection: connection,
            tableName: tableName,
            fields: fields,
            where: connection.ToQueryGroup(where, transaction, tableName),
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
        string tableName,
        QueryField where,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await QueryAsyncInternal<TEntity>(connection: connection,
            tableName: tableName,
            fields: fields,
            where: ToQueryGroup(where),
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<QueryField> where,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await QueryAsyncInternal<TEntity>(connection: connection,
            tableName: tableName,
            fields: fields,
            where: ToQueryGroup(where),
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
        string tableName,
        QueryGroup where,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await QueryAsyncInternal<TEntity>(connection: connection,
            tableName: tableName,
            fields: fields,
            where: where,
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
        object what,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await QueryAsyncInternal<TEntity>(connection: connection,
            tableName: ClassMappedNameCache.Get<TEntity>() ?? throw new ArgumentException($"Can't map {typeof(TEntity)} to tablename"),
            where: await WhatToQueryGroupAsync(typeof(TEntity), connection, what, transaction, cancellationToken).ConfigureAwait(false),
            fields: fields,
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static async Task<IEnumerable<TEntity>> QueryAsync<TEntity, TWhat>(this IDbConnection connection,
        TWhat what,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
        where TWhat : notnull
    {
        return await QueryAsyncInternal<TEntity>(connection: connection,
            tableName: ClassMappedNameCache.Get<TEntity>() ?? throw new ArgumentException($"Can't map {typeof(TEntity)} to tablename"),
            where: await WhatToQueryGroupAsync(typeof(TEntity), connection, what, transaction, cancellationToken).ConfigureAwait(false),
            fields: fields,
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
        QueryField where,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await QueryAsyncInternal<TEntity>(connection: connection,
            tableName: ClassMappedNameCache.Get<TEntity>() ?? throw new ArgumentException($"Can't map {typeof(TEntity)} to tablename"),
            where: ToQueryGroup(where),
            fields: fields,
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
        IEnumerable<QueryField> where,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await QueryAsyncInternal<TEntity>(connection: connection,
            tableName: ClassMappedNameCache.Get<TEntity>() ?? throw new ArgumentException($"Can't map {typeof(TEntity)} to tablename"),
            where: ToQueryGroup(where),
            fields: fields,
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
        Expression<Func<TEntity, bool>> where,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await QueryAsyncInternal<TEntity>(connection: connection,
            tableName: ClassMappedNameCache.Get<TEntity>() ?? throw new ArgumentException($"Can't map {typeof(TEntity)} to tablename"),
            where: connection.ToQueryGroup(where, transaction),
            fields: fields,
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
        QueryGroup where,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await QueryAsyncInternal<TEntity>(connection: connection,
            tableName: ClassMappedNameCache.Get<TEntity>() ?? throw new ArgumentException($"Can't map {typeof(TEntity)} to tablename"),
            where: where,
            fields: fields,
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    internal static async ValueTask<IEnumerable<TEntity>> QueryAsyncInternal<TEntity>(this IDbConnection connection,
        string tableName,
        QueryGroup? where,
        IEnumerable<Field>? fields = null,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        // Ensure the fields
        fields ??= GetQualifiedFields<TEntity>() ??
            (await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken).ConfigureAwait(false))?.AsFields();

        // Return
        return await QueryAsyncInternalBase<TEntity>(connection: connection,
            tableName: tableName,
            fields: fields,
            where: where,
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #region Query(TableName)

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of dynamic objects.</returns>
    public static IEnumerable<dynamic> Query<TWhat>(this IDbConnection connection,
        string tableName,
        TWhat what,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TWhat : notnull
    {
        return QueryInternal(connection: connection,
            tableName: tableName,
            where: WhatToQueryGroup(connection, tableName, what, transaction),
            fields: fields,
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of dynamic objects.</returns>
    public static IEnumerable<dynamic> Query(this IDbConnection connection,
        string tableName,
        object what,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return QueryInternal(connection: connection,
            tableName: tableName,
            where: WhatToQueryGroup(connection, tableName, what, transaction),
            fields: fields,
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of dynamic objects.</returns>
    public static IEnumerable<dynamic> Query(this IDbConnection connection,
        string tableName,
        QueryField where,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return QueryInternal(connection: connection,
            tableName: tableName,
            where: ToQueryGroup(where),
            fields: fields,
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of dynamic objects.</returns>
    public static IEnumerable<dynamic> Query(this IDbConnection connection,
        string tableName,
        IEnumerable<QueryField> where,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return QueryInternal(connection: connection,
            tableName: tableName,
            where: ToQueryGroup(where),
            fields: fields,
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of dynamic objects.</returns>
    public static IEnumerable<dynamic> Query(this IDbConnection connection,
        string tableName,
        QueryGroup where,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return QueryInternal(connection: connection,
            tableName: tableName,
            where: where,
            fields: fields,
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of dynamic objects.</returns>
    internal static IEnumerable<dynamic> QueryInternal(this IDbConnection connection,
        string tableName,
        QueryGroup? where,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return QueryInternal<dynamic>(connection: connection,
            tableName: tableName,
            where: where,
            fields: fields,
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    #endregion

    #region QueryAsync(TableName)

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of dynamic objects.</returns>
    public static async Task<IEnumerable<dynamic>> QueryAsync<TWhat>(this IDbConnection connection,
        string tableName,
        TWhat what,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TWhat : notnull
    {
        return await QueryAsyncInternal(connection: connection,
            tableName: tableName,
            where: await WhatToQueryGroupAsync(connection, tableName, what, transaction, cancellationToken).ConfigureAwait(false),
            fields: fields,
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of dynamic objects.</returns>
    public static async Task<IEnumerable<dynamic>> QueryAsync(this IDbConnection connection,
        string tableName,
        object what,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await QueryAsyncInternal(connection: connection,
            tableName: tableName,
            where: await WhatToQueryGroupAsync(connection, tableName, what, transaction, cancellationToken).ConfigureAwait(false),
            fields: fields,
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of dynamic objects.</returns>
    public static async Task<IEnumerable<dynamic>> QueryAsync(this IDbConnection connection,
        string tableName,
        QueryField where,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await QueryAsyncInternal(connection: connection,
            tableName: tableName,
            where: ToQueryGroup(where),
            fields: fields,
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of dynamic objects.</returns>
    public static async Task<IEnumerable<dynamic>> QueryAsync(this IDbConnection connection,
        string tableName,
        IEnumerable<QueryField> where,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await QueryAsyncInternal(connection: connection,
            tableName: tableName,
            where: ToQueryGroup(where),
            fields: fields,
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of dynamic objects.</returns>
    public static async Task<IEnumerable<dynamic>> QueryAsync(this IDbConnection connection,
        string tableName,
        QueryGroup where,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await QueryAsyncInternal(connection: connection,
            tableName: tableName,
            where: where,
            fields: fields,
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of dynamic objects.</returns>
    internal static ValueTask<IEnumerable<dynamic>> QueryAsyncInternal(this IDbConnection connection,
        string tableName,
        QueryGroup? where,
        FieldSet fields = default,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return QueryAsyncInternal<dynamic>(connection: connection,
            tableName: tableName,
            where: where,
            fields: fields,
            orderBy: orderBy,
            top: top,
            hints: hints,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
    }

    #endregion

    #region QueryInternalBase<TEntity>

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    internal static IEnumerable<TEntity> QueryInternalBase<TEntity>(this IDbConnection connection,
        string tableName,
        QueryGroup? where,
        IEnumerable<Field>? fields = null,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        // Get Cache
        if (cache != null && cacheKey != null)
        {
            var item = cache.Get<IEnumerable<TEntity>>(cacheKey, false);
            if (item != null)
            {
                return item.Value;
            }
        }

        // Variables
        var commandType = CommandType.Text;
        var request = new QueryRequest(tableName,
            connection,
            transaction,
            fields,
            where,
            orderBy,
            top,
            hints,
            statementBuilder);

        // Converts to property mapped object
        var param = (where != null) ? QueryGroup.AsMappedObject([where.MapTo<TEntity>()], connection, transaction, tableName) : null;

        var commandText = CommandTextCache.GetQueryText(request);

        // Actual Execution
        var result = ExecuteQueryInternal<TEntity>(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            cacheKey: null,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: null,
            trace: trace,
            tableName: tableName,
            skipCommandArrayParametersCheck: true);

        // Set Cache
        if (cache != null && cacheKey != null)
        {
            cache.Add(cacheKey, result, cacheItemExpiration, false);
        }

        // Result
        return result;
    }

    #endregion

    #region QueryAsyncInternalBase<TEntity>

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    internal static async ValueTask<IEnumerable<TEntity>> QueryAsyncInternalBase<TEntity>(this IDbConnection connection,
        string tableName,
        QueryGroup? where,
        IEnumerable<Field>? fields = null,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Query,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        // Get Cache
        if (cache != null && cacheKey != null)
        {
            var item = await cache.GetAsync<IEnumerable<TEntity>>(cacheKey, false, cancellationToken).ConfigureAwait(false);
            if (item != null)
            {
                return item.Value;
            }
        }

        // Variables
        var commandType = CommandType.Text;
        var request = new QueryRequest(tableName,
            connection,
            transaction,
            fields,
            where,
            orderBy,
            top,
            hints,
            statementBuilder);

        // Converts to property mapped object
        var param = (where != null) ? QueryGroup.AsMappedObject([where.MapTo<TEntity>()], connection, transaction, tableName) : null;

        var commandText = await CommandTextCache.GetQueryTextAsync(request, cancellationToken).ConfigureAwait(false);

        // Actual Execution
        var result = await ExecuteQueryAsyncInternal<TEntity>(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            cacheKey: null,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cancellationToken: cancellationToken,
            cache: null,
            trace: trace,
            tableName: tableName,
            skipCommandArrayParametersCheck: true).ConfigureAwait(false);

        // Set Cache
        if (cache != null && cacheKey != null)
        {
            await cache.AddAsync(cacheKey, result, cacheItemExpiration, false, cancellationToken).ConfigureAwait(false);
        }

        // Result
        return result;
    }

    #endregion
}
