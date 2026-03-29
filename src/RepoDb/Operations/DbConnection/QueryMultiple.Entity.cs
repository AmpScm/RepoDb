using System.Data;
using System.Linq.Expressions;
using RepoDb.Interfaces;

namespace RepoDb;

/// <summary>
/// Contains the extension methods for <see cref="IDbConnection"/> object.
/// </summary>
public static partial class DbConnectionExtension
{
    #region QueryMultiple<TEntity>

    #region T1, T2

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 2 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="what1">The dynamic expression or the key value to be used (for T1).</param>
    /// <param name="what2">The dynamic expression or the key value to be used (for T2).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 2 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<T1>, IEnumerable<T2>> QueryMultiple<T1, T2>(this IDbConnection connection,
        object what1,
        object what2,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        int top2 = 0,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        string? hints2 = null,
        string? cacheKey2 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
    {
        return QueryMultipleInternal<T1, T2>(connection: connection,
            where1: WhatToQueryGroup(typeof(T1), connection, what1, transaction),
            where2: WhatToQueryGroup(typeof(T2), connection, what2, transaction),
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            top2: top2,
            fields2: fields2,
            orderBy2: orderBy2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 2 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 2 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<T1>, IEnumerable<T2>> QueryMultiple<T1, T2>(this IDbConnection connection,
        Expression<Func<T1, bool>> where1,
        Expression<Func<T2, bool>> where2,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        int top2 = 0,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        string? hints2 = null,
        string? cacheKey2 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
    {
        return QueryMultipleInternal<T1, T2>(connection: connection,
            where1: connection.ToQueryGroup(where1, transaction),
            where2: connection.ToQueryGroup(where2, transaction),
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            top2: top2,
            fields2: fields2,
            orderBy2: orderBy2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 2 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 2 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<T1>, IEnumerable<T2>> QueryMultiple<T1, T2>(this IDbConnection connection,
        QueryField where1,
        QueryField where2,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        int top2 = 0,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        string? hints2 = null,
        string? cacheKey2 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
    {
        return QueryMultipleInternal<T1, T2>(connection: connection,
            where1: ToQueryGroup(where1),
            where2: ToQueryGroup(where2),
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            top2: top2,
            fields2: fields2,
            orderBy2: orderBy2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 2 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 2 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<T1>, IEnumerable<T2>> QueryMultiple<T1, T2>(this IDbConnection connection,
        IEnumerable<QueryField> where1,
        IEnumerable<QueryField> where2,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        int top2 = 0,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        string? hints2 = null,
        string? cacheKey2 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
    {
        return QueryMultipleInternal<T1, T2>(connection: connection,
            where1: ToQueryGroup(where1),
            where2: ToQueryGroup(where2),
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            top2: top2,
            fields2: fields2,
            orderBy2: orderBy2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 2 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 2 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<T1>, IEnumerable<T2>> QueryMultiple<T1, T2>(this IDbConnection connection,
        QueryGroup? where1,
        QueryGroup? where2,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        int top2 = 0,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        string? hints2 = null,
        string? cacheKey2 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
    {
        return QueryMultipleInternal<T1, T2>(connection: connection,
            where1: where1,
            where2: where2,
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            top2: top2,
            fields2: fields2,
            orderBy2: orderBy2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 2 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 2 enumerable target data entity types.</returns>
    internal static Tuple<IEnumerable<T1>, IEnumerable<T2>> QueryMultipleInternal<T1, T2>(this IDbConnection connection,
        QueryGroup? where1,
        QueryGroup? where2,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        int top2 = 0,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        string? hints2 = null,
        string? cacheKey2 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
    {
        return QueryMultipleInternal<T1, T2>(connection: connection,
            tableName1: ClassMappedNameCache.Get<T1>(),
            where1: where1,
            tableName2: ClassMappedNameCache.Get<T2>(),
            where2: where2,
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            top2: top2,
            fields2: fields2,
            orderBy2: orderBy2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    #endregion

    #region T1, T2, T3

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 3 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <typeparam name="T3">The third target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="what1">The dynamic query expression or the key value to be used (for T1).</param>
    /// <param name="what2">The dynamic query expression or the key value to be used (for T2).</param>
    /// <param name="what3">The dynamic query expression or the key value to be used (for T3).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for T3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
    /// <param name="top3">The number of rows to be returned (for T3).</param>
    /// <param name="hints3">The table hints to be used (for T3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 3 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> QueryMultiple<T1, T2, T3>(this IDbConnection connection,
        object what1,
        object what2,
        object what3,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        int top2 = 0,
        string? hints2 = null,
        string? cacheKey2 = null,
        IEnumerable<Field>? fields3 = null,
        IEnumerable<OrderField>? orderBy3 = null,
        int top3 = 0,
        string? hints3 = null,
        string? cacheKey3 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
        where T3 : class
    {
        return QueryMultipleInternal<T1, T2, T3>(connection: connection,
            where1: WhatToQueryGroup(what1),
            where2: WhatToQueryGroup(what2),
            where3: WhatToQueryGroup(what3),
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            fields2: fields2,
            orderBy2: orderBy2,
            top2: top2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            fields3: fields3,
            orderBy3: orderBy3,
            top3: top3,
            hints3: hints3,
            cacheKey3: cacheKey3,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 3 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <typeparam name="T3">The third target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for T3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
    /// <param name="top3">The number of rows to be returned (for T3).</param>
    /// <param name="hints3">The table hints to be used (for T3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 3 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> QueryMultiple<T1, T2, T3>(this IDbConnection connection,
        Expression<Func<T1, bool>> where1,
        Expression<Func<T2, bool>> where2,
        Expression<Func<T3, bool>> where3,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        int top2 = 0,
        string? hints2 = null,
        string? cacheKey2 = null,
        IEnumerable<Field>? fields3 = null,
        IEnumerable<OrderField>? orderBy3 = null,
        int top3 = 0,
        string? hints3 = null,
        string? cacheKey3 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
        where T3 : class
    {
        return QueryMultipleInternal<T1, T2, T3>(connection: connection,
            where1: connection.ToQueryGroup(where1, transaction),
            where2: connection.ToQueryGroup(where2, transaction),
            where3: connection.ToQueryGroup(where3, transaction),
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            fields2: fields2,
            orderBy2: orderBy2,
            top2: top2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            fields3: fields3,
            orderBy3: orderBy3,
            top3: top3,
            hints3: hints3,
            cacheKey3: cacheKey3,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 3 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <typeparam name="T3">The third target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for T3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
    /// <param name="top3">The number of rows to be returned (for T3).</param>
    /// <param name="hints3">The table hints to be used (for T3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 3 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> QueryMultiple<T1, T2, T3>(this IDbConnection connection,
        QueryField where1,
        QueryField where2,
        QueryField where3,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        int top2 = 0,
        string? hints2 = null,
        string? cacheKey2 = null,
        IEnumerable<Field>? fields3 = null,
        IEnumerable<OrderField>? orderBy3 = null,
        int top3 = 0,
        string? hints3 = null,
        string? cacheKey3 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
        where T3 : class
    {
        return QueryMultipleInternal<T1, T2, T3>(connection: connection,
            where1: ToQueryGroup(where1),
            where2: ToQueryGroup(where2),
            where3: ToQueryGroup(where3),
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            fields2: fields2,
            orderBy2: orderBy2,
            top2: top2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            fields3: fields3,
            orderBy3: orderBy3,
            top3: top3,
            hints3: hints3,
            cacheKey3: cacheKey3,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 3 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <typeparam name="T3">The third target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for T3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
    /// <param name="top3">The number of rows to be returned (for T3).</param>
    /// <param name="hints3">The table hints to be used (for T3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 3 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> QueryMultiple<T1, T2, T3>(this IDbConnection connection,
        IEnumerable<QueryField> where1,
        IEnumerable<QueryField> where2,
        IEnumerable<QueryField> where3,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        int top2 = 0,
        string? hints2 = null,
        string? cacheKey2 = null,
        IEnumerable<Field>? fields3 = null,
        IEnumerable<OrderField>? orderBy3 = null,
        int top3 = 0,
        string? hints3 = null,
        string? cacheKey3 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
        where T3 : class
    {
        return QueryMultipleInternal<T1, T2, T3>(connection: connection,
            where1: ToQueryGroup(where1),
            where2: ToQueryGroup(where2),
            where3: ToQueryGroup(where3),
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            fields2: fields2,
            orderBy2: orderBy2,
            top2: top2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            fields3: fields3,
            orderBy3: orderBy3,
            top3: top3,
            hints3: hints3,
            cacheKey3: cacheKey3,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 3 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <typeparam name="T3">The third target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for T3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
    /// <param name="top3">The number of rows to be returned (for T3).</param>
    /// <param name="hints3">The table hints to be used (for T3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 3 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> QueryMultiple<T1, T2, T3>(this IDbConnection connection,
        QueryGroup? where1,
        QueryGroup? where2,
        QueryGroup? where3,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        int top2 = 0,
        string? hints2 = null,
        string? cacheKey2 = null,
        IEnumerable<Field>? fields3 = null,
        IEnumerable<OrderField>? orderBy3 = null,
        int top3 = 0,
        string? hints3 = null,
        string? cacheKey3 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
        where T3 : class
    {
        return QueryMultipleInternal<T1, T2, T3>(connection: connection,
            where1: where1,
            where2: where2,
            where3: where3,
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            fields2: fields2,
            orderBy2: orderBy2,
            top2: top2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            fields3: fields3,
            orderBy3: orderBy3,
            top3: top3,
            hints3: hints3,
            cacheKey3: cacheKey3,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 3 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <typeparam name="T3">The third target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for T3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
    /// <param name="top3">The number of rows to be returned (for T3).</param>
    /// <param name="hints3">The table hints to be used (for T3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 3 enumerable target data entity types.</returns>
    internal static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> QueryMultipleInternal<T1, T2, T3>(this IDbConnection connection,
        QueryGroup? where1,
        QueryGroup? where2,
        QueryGroup? where3,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        int top2 = 0,
        string? hints2 = null,
        string? cacheKey2 = null,
        IEnumerable<Field>? fields3 = null,
        IEnumerable<OrderField>? orderBy3 = null,
        int top3 = 0,
        string? hints3 = null,
        string? cacheKey3 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
        where T3 : class
    {
        return QueryMultipleInternal<T1, T2, T3>(connection: connection,
            tableName1: ClassMappedNameCache.Get<T1>(),
            where1: where1,
            tableName2: ClassMappedNameCache.Get<T2>(),
            where2: where2,
            tableName3: ClassMappedNameCache.Get<T3>(),
            where3: where3,
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            fields2: fields2,
            orderBy2: orderBy2,
            top2: top2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            fields3: fields3,
            orderBy3: orderBy3,
            top3: top3,
            hints3: hints3,
            cacheKey3: cacheKey3,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    #endregion

    #region T1, T2, T3, T4

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 4 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <typeparam name="T3">The third target type.</typeparam>
    /// <typeparam name="T4">The fourth target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="what1">The dynamic query expression or the key value to be used (for T1).</param>
    /// <param name="what2">The dynamic query expression or the key value to be used (for T2).</param>
    /// <param name="what3">The dynamic query expression or the key value to be used (for T3).</param>
    /// <param name="what4">The dynamic query expression or the key value to be used (for T4).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for T3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
    /// <param name="top3">The number of rows to be returned (for T3).</param>
    /// <param name="hints3">The table hints to be used (for T3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for T4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
    /// <param name="top4">The number of rows to be returned (for T4).</param>
    /// <param name="hints4">The table hints to be used (for T4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 4 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>> QueryMultiple<T1, T2, T3, T4>(this IDbConnection connection,
        object what1,
        object what2,
        object what3,
        object what4,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        int top2 = 0,
        string? hints2 = null,
        string? cacheKey2 = null,
        IEnumerable<Field>? fields3 = null,
        IEnumerable<OrderField>? orderBy3 = null,
        int top3 = 0,
        string? hints3 = null,
        string? cacheKey3 = null,
        IEnumerable<Field>? fields4 = null,
        IEnumerable<OrderField>? orderBy4 = null,
        int top4 = 0,
        string? hints4 = null,
        string? cacheKey4 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
    {
        return QueryMultipleInternal<T1, T2, T3, T4>(connection: connection,
            where1: WhatToQueryGroup(what1),
            where2: WhatToQueryGroup(what2),
            where3: WhatToQueryGroup(what3),
            where4: WhatToQueryGroup(what4),
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            fields2: fields2,
            orderBy2: orderBy2,
            top2: top2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            fields3: fields3,
            orderBy3: orderBy3,
            top3: top3,
            hints3: hints3,
            cacheKey3: cacheKey3,
            fields4: fields4,
            orderBy4: orderBy4,
            top4: top4,
            hints4: hints4,
            cacheKey4: cacheKey4,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 4 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <typeparam name="T3">The third target type.</typeparam>
    /// <typeparam name="T4">The fourth target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for T3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
    /// <param name="top3">The number of rows to be returned (for T3).</param>
    /// <param name="hints3">The table hints to be used (for T3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for T4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
    /// <param name="top4">The number of rows to be returned (for T4).</param>
    /// <param name="hints4">The table hints to be used (for T4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 4 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>> QueryMultiple<T1, T2, T3, T4>(this IDbConnection connection,
        Expression<Func<T1, bool>> where1,
        Expression<Func<T2, bool>> where2,
        Expression<Func<T3, bool>> where3,
        Expression<Func<T4, bool>> where4,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        int top2 = 0,
        string? hints2 = null,
        string? cacheKey2 = null,
        IEnumerable<Field>? fields3 = null,
        IEnumerable<OrderField>? orderBy3 = null,
        int top3 = 0,
        string? hints3 = null,
        string? cacheKey3 = null,
        IEnumerable<Field>? fields4 = null,
        IEnumerable<OrderField>? orderBy4 = null,
        int top4 = 0,
        string? hints4 = null,
        string? cacheKey4 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
    {
        return QueryMultipleInternal<T1, T2, T3, T4>(connection: connection,
            where1: connection.ToQueryGroup(where1, transaction),
            where2: connection.ToQueryGroup(where2, transaction),
            where3: connection.ToQueryGroup(where3, transaction),
            where4: connection.ToQueryGroup(where4, transaction),
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            fields2: fields2,
            orderBy2: orderBy2,
            top2: top2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            fields3: fields3,
            orderBy3: orderBy3,
            top3: top3,
            hints3: hints3,
            cacheKey3: cacheKey3,
            fields4: fields4,
            orderBy4: orderBy4,
            top4: top4,
            hints4: hints4,
            cacheKey4: cacheKey4,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 4 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <typeparam name="T3">The third target type.</typeparam>
    /// <typeparam name="T4">The fourth target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for T3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
    /// <param name="top3">The number of rows to be returned (for T3).</param>
    /// <param name="hints3">The table hints to be used (for T3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for T4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
    /// <param name="top4">The number of rows to be returned (for T4).</param>
    /// <param name="hints4">The table hints to be used (for T4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 4 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>> QueryMultiple<T1, T2, T3, T4>(this IDbConnection connection,
        QueryField where1,
        QueryField where2,
        QueryField where3,
        QueryField where4,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        int top2 = 0,
        string? hints2 = null,
        string? cacheKey2 = null,
        IEnumerable<Field>? fields3 = null,
        IEnumerable<OrderField>? orderBy3 = null,
        int top3 = 0,
        string? hints3 = null,
        string? cacheKey3 = null,
        IEnumerable<Field>? fields4 = null,
        IEnumerable<OrderField>? orderBy4 = null,
        int top4 = 0,
        string? hints4 = null,
        string? cacheKey4 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
    {
        return QueryMultipleInternal<T1, T2, T3, T4>(connection: connection,
            where1: ToQueryGroup(where1),
            where2: ToQueryGroup(where2),
            where3: ToQueryGroup(where3),
            where4: ToQueryGroup(where4),
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            fields2: fields2,
            orderBy2: orderBy2,
            top2: top2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            fields3: fields3,
            orderBy3: orderBy3,
            top3: top3,
            hints3: hints3,
            cacheKey3: cacheKey3,
            fields4: fields4,
            orderBy4: orderBy4,
            top4: top4,
            hints4: hints4,
            cacheKey4: cacheKey4,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 4 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <typeparam name="T3">The third target type.</typeparam>
    /// <typeparam name="T4">The fourth target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for T3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
    /// <param name="top3">The number of rows to be returned (for T3).</param>
    /// <param name="hints3">The table hints to be used (for T3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for T4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
    /// <param name="top4">The number of rows to be returned (for T4).</param>
    /// <param name="hints4">The table hints to be used (for T4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 4 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>> QueryMultiple<T1, T2, T3, T4>(this IDbConnection connection,
        IEnumerable<QueryField> where1,
        IEnumerable<QueryField> where2,
        IEnumerable<QueryField> where3,
        IEnumerable<QueryField> where4,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        int top2 = 0,
        string? hints2 = null,
        string? cacheKey2 = null,
        IEnumerable<Field>? fields3 = null,
        IEnumerable<OrderField>? orderBy3 = null,
        int top3 = 0,
        string? hints3 = null,
        string? cacheKey3 = null,
        IEnumerable<Field>? fields4 = null,
        IEnumerable<OrderField>? orderBy4 = null,
        int top4 = 0,
        string? hints4 = null,
        string? cacheKey4 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
    {
        return QueryMultipleInternal<T1, T2, T3, T4>(connection: connection,
            where1: ToQueryGroup(where1),
            where2: ToQueryGroup(where2),
            where3: ToQueryGroup(where3),
            where4: ToQueryGroup(where4),
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            fields2: fields2,
            orderBy2: orderBy2,
            top2: top2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            fields3: fields3,
            orderBy3: orderBy3,
            top3: top3,
            hints3: hints3,
            cacheKey3: cacheKey3,
            fields4: fields4,
            orderBy4: orderBy4,
            top4: top4,
            hints4: hints4,
            cacheKey4: cacheKey4,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 4 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <typeparam name="T3">The third target type.</typeparam>
    /// <typeparam name="T4">The fourth target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for T3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
    /// <param name="top3">The number of rows to be returned (for T3).</param>
    /// <param name="hints3">The table hints to be used (for T3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for T4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
    /// <param name="top4">The number of rows to be returned (for T4).</param>
    /// <param name="hints4">The table hints to be used (for T4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 4 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>> QueryMultiple<T1, T2, T3, T4>(this IDbConnection connection,
        QueryGroup? where1,
        QueryGroup? where2,
        QueryGroup? where3,
        QueryGroup? where4,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        int top2 = 0,
        string? hints2 = null,
        string? cacheKey2 = null,
        IEnumerable<Field>? fields3 = null,
        IEnumerable<OrderField>? orderBy3 = null,
        int top3 = 0,
        string? hints3 = null,
        string? cacheKey3 = null,
        IEnumerable<Field>? fields4 = null,
        IEnumerable<OrderField>? orderBy4 = null,
        int top4 = 0,
        string? hints4 = null,
        string? cacheKey4 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
    {
        return QueryMultipleInternal<T1, T2, T3, T4>(connection: connection,
            where1: where1,
            where2: where2,
            where3: where3,
            where4: where4,
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            fields2: fields2,
            orderBy2: orderBy2,
            top2: top2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            fields3: fields3,
            orderBy3: orderBy3,
            top3: top3,
            hints3: hints3,
            cacheKey3: cacheKey3,
            fields4: fields4,
            orderBy4: orderBy4,
            top4: top4,
            hints4: hints4,
            cacheKey4: cacheKey4,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 3 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <typeparam name="T3">The third target type.</typeparam>
    /// <typeparam name="T4">The fourth target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for T3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
    /// <param name="top3">The number of rows to be returned (for T3).</param>
    /// <param name="hints3">The table hints to be used (for T3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for T4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
    /// <param name="top4">The number of rows to be returned (for T4).</param>
    /// <param name="hints4">The table hints to be used (for T4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 4 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>> QueryMultipleInternal<T1, T2, T3, T4>(this IDbConnection connection,
        QueryGroup? where1,
        QueryGroup? where2,
        QueryGroup? where3,
        QueryGroup? where4,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        int top2 = 0,
        string? hints2 = null,
        string? cacheKey2 = null,
        IEnumerable<Field>? fields3 = null,
        IEnumerable<OrderField>? orderBy3 = null,
        int top3 = 0,
        string? hints3 = null,
        string? cacheKey3 = null,
        IEnumerable<Field>? fields4 = null,
        IEnumerable<OrderField>? orderBy4 = null,
        int top4 = 0,
        string? hints4 = null,
        string? cacheKey4 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
    {
        return QueryMultipleInternal<T1, T2, T3, T4>(connection: connection,
            tableName1: ClassMappedNameCache.Get<T1>(),
            where1: where1,
            tableName2: ClassMappedNameCache.Get<T2>(),
            where2: where2,
            tableName3: ClassMappedNameCache.Get<T3>(),
            where3: where3,
            tableName4: ClassMappedNameCache.Get<T4>(),
            where4: where4,
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            fields2: fields2,
            orderBy2: orderBy2,
            top2: top2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            fields3: fields3,
            orderBy3: orderBy3,
            top3: top3,
            hints3: hints3,
            cacheKey3: cacheKey3,
            fields4: fields4,
            orderBy4: orderBy4,
            top4: top4,
            hints4: hints4,
            cacheKey4: cacheKey4,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    #endregion

    #region T1, T2, T3, T4, T5

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 5 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <typeparam name="T3">The third target type.</typeparam>
    /// <typeparam name="T4">The fourth target type.</typeparam>
    /// <typeparam name="T5">The fifth target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="what1">The dynamic query expression or the key value to be used (for T1).</param>
    /// <param name="what2">The dynamic query expression or the key value to be used (for T2).</param>
    /// <param name="what3">The dynamic query expression or the key value to be used (for T3).</param>
    /// <param name="what4">The dynamic query expression or the key value to be used (for T4).</param>
    /// <param name="what5">The dynamic query expression or the key value to be used (for T5).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for T3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
    /// <param name="top3">The number of rows to be returned (for T3).</param>
    /// <param name="hints3">The table hints to be used (for T3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for T4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
    /// <param name="top4">The number of rows to be returned (for T4).</param>
    /// <param name="hints4">The table hints to be used (for T4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields5">The mapping list of <see cref="Field"/> objects to be used (for T5).</param>
    /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
    /// <param name="top5">The number of rows to be returned (for T5).</param>
    /// <param name="hints5">The table hints to be used (for T5).</param>
    /// <param name="cacheKey5">The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 5 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>> QueryMultiple<T1, T2, T3, T4, T5>(this IDbConnection connection,
        object what1,
        object what2,
        object what3,
        object what4,
        object what5,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        int top2 = 0,
        string? hints2 = null,
        string? cacheKey2 = null,
        IEnumerable<Field>? fields3 = null,
        IEnumerable<OrderField>? orderBy3 = null,
        int top3 = 0,
        string? hints3 = null,
        string? cacheKey3 = null,
        IEnumerable<Field>? fields4 = null,
        IEnumerable<OrderField>? orderBy4 = null,
        int top4 = 0,
        string? hints4 = null,
        string? cacheKey4 = null,
        IEnumerable<Field>? fields5 = null,
        IEnumerable<OrderField>? orderBy5 = null,
        int top5 = 0,
        string? hints5 = null,
        string? cacheKey5 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
    {
        return QueryMultipleInternal<T1, T2, T3, T4, T5>(connection: connection,
            where1: WhatToQueryGroup(what1),
            where2: WhatToQueryGroup(what2),
            where3: WhatToQueryGroup(what3),
            where4: WhatToQueryGroup(what4),
            where5: WhatToQueryGroup(what5),
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            fields2: fields2,
            orderBy2: orderBy2,
            top2: top2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            fields3: fields3,
            orderBy3: orderBy3,
            top3: top3,
            hints3: hints3,
            cacheKey3: cacheKey3,
            fields4: fields4,
            orderBy4: orderBy4,
            top4: top4,
            hints4: hints4,
            cacheKey4: cacheKey4,
            fields5: fields5,
            orderBy5: orderBy5,
            top5: top5,
            hints5: hints5,
            cacheKey5: cacheKey5,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 5 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <typeparam name="T3">The third target type.</typeparam>
    /// <typeparam name="T4">The fourth target type.</typeparam>
    /// <typeparam name="T5">The fifth target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="where5">The query expression to be used (for T5).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for T3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
    /// <param name="top3">The number of rows to be returned (for T3).</param>
    /// <param name="hints3">The table hints to be used (for T3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for T4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
    /// <param name="top4">The number of rows to be returned (for T4).</param>
    /// <param name="hints4">The table hints to be used (for T4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields5">The mapping list of <see cref="Field"/> objects to be used (for T5).</param>
    /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
    /// <param name="top5">The number of rows to be returned (for T5).</param>
    /// <param name="hints5">The table hints to be used (for T5).</param>
    /// <param name="cacheKey5">The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 5 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>> QueryMultiple<T1, T2, T3, T4, T5>(this IDbConnection connection,
        Expression<Func<T1, bool>> where1,
        Expression<Func<T2, bool>> where2,
        Expression<Func<T3, bool>> where3,
        Expression<Func<T4, bool>> where4,
        Expression<Func<T5, bool>> where5,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        int top2 = 0,
        string? hints2 = null,
        string? cacheKey2 = null,
        IEnumerable<Field>? fields3 = null,
        IEnumerable<OrderField>? orderBy3 = null,
        int top3 = 0,
        string? hints3 = null,
        string? cacheKey3 = null,
        IEnumerable<Field>? fields4 = null,
        IEnumerable<OrderField>? orderBy4 = null,
        int top4 = 0,
        string? hints4 = null,
        string? cacheKey4 = null,
        IEnumerable<Field>? fields5 = null,
        IEnumerable<OrderField>? orderBy5 = null,
        int top5 = 0,
        string? hints5 = null,
        string? cacheKey5 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
    {
        return QueryMultipleInternal<T1, T2, T3, T4, T5>(connection: connection,
            where1: connection.ToQueryGroup(where1, transaction),
            where2: connection.ToQueryGroup(where2, transaction),
            where3: connection.ToQueryGroup(where3, transaction),
            where4: connection.ToQueryGroup(where4, transaction),
            where5: connection.ToQueryGroup(where5, transaction),
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            fields2: fields2,
            orderBy2: orderBy2,
            top2: top2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            fields3: fields3,
            orderBy3: orderBy3,
            top3: top3,
            hints3: hints3,
            cacheKey3: cacheKey3,
            fields4: fields4,
            orderBy4: orderBy4,
            top4: top4,
            hints4: hints4,
            cacheKey4: cacheKey4,
            fields5: fields5,
            orderBy5: orderBy5,
            top5: top5,
            hints5: hints5,
            cacheKey5: cacheKey5,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 5 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <typeparam name="T3">The third target type.</typeparam>
    /// <typeparam name="T4">The fourth target type.</typeparam>
    /// <typeparam name="T5">The fifth target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="where5">The query expression to be used (for T5).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for T3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
    /// <param name="top3">The number of rows to be returned (for T3).</param>
    /// <param name="hints3">The table hints to be used (for T3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for T4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
    /// <param name="top4">The number of rows to be returned (for T4).</param>
    /// <param name="hints4">The table hints to be used (for T4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields5">The mapping list of <see cref="Field"/> objects to be used (for T5).</param>
    /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
    /// <param name="top5">The number of rows to be returned (for T5).</param>
    /// <param name="hints5">The table hints to be used (for T5).</param>
    /// <param name="cacheKey5">The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 5 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>> QueryMultiple<T1, T2, T3, T4, T5>(this IDbConnection connection,
        QueryField where1,
        QueryField where2,
        QueryField where3,
        QueryField where4,
        QueryField where5,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        int top2 = 0,
        string? hints2 = null,
        string? cacheKey2 = null,
        IEnumerable<Field>? fields3 = null,
        IEnumerable<OrderField>? orderBy3 = null,
        int top3 = 0,
        string? hints3 = null,
        string? cacheKey3 = null,
        IEnumerable<Field>? fields4 = null,
        IEnumerable<OrderField>? orderBy4 = null,
        int top4 = 0,
        string? hints4 = null,
        string? cacheKey4 = null,
        IEnumerable<Field>? fields5 = null,
        IEnumerable<OrderField>? orderBy5 = null,
        int top5 = 0,
        string? hints5 = null,
        string? cacheKey5 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
    {
        return QueryMultipleInternal<T1, T2, T3, T4, T5>(connection: connection,
            where1: ToQueryGroup(where1),
            where2: ToQueryGroup(where2),
            where3: ToQueryGroup(where3),
            where4: ToQueryGroup(where4),
            where5: ToQueryGroup(where5),
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            fields2: fields2,
            orderBy2: orderBy2,
            top2: top2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            fields3: fields3,
            orderBy3: orderBy3,
            top3: top3,
            hints3: hints3,
            cacheKey3: cacheKey3,
            fields4: fields4,
            orderBy4: orderBy4,
            top4: top4,
            hints4: hints4,
            cacheKey4: cacheKey4,
            fields5: fields5,
            orderBy5: orderBy5,
            top5: top5,
            hints5: hints5,
            cacheKey5: cacheKey5,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 5 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <typeparam name="T3">The third target type.</typeparam>
    /// <typeparam name="T4">The fourth target type.</typeparam>
    /// <typeparam name="T5">The fifth target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="where5">The query expression to be used (for T5).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for T3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
    /// <param name="top3">The number of rows to be returned (for T3).</param>
    /// <param name="hints3">The table hints to be used (for T3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for T4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
    /// <param name="top4">The number of rows to be returned (for T4).</param>
    /// <param name="hints4">The table hints to be used (for T4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields5">The mapping list of <see cref="Field"/> objects to be used (for T5).</param>
    /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
    /// <param name="top5">The number of rows to be returned (for T5).</param>
    /// <param name="hints5">The table hints to be used (for T5).</param>
    /// <param name="cacheKey5">The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 5 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>> QueryMultiple<T1, T2, T3, T4, T5>(this IDbConnection connection,
        IEnumerable<QueryField> where1,
        IEnumerable<QueryField> where2,
        IEnumerable<QueryField> where3,
        IEnumerable<QueryField> where4,
        IEnumerable<QueryField> where5,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        int top2 = 0,
        string? hints2 = null,
        string? cacheKey2 = null,
        IEnumerable<Field>? fields3 = null,
        IEnumerable<OrderField>? orderBy3 = null,
        int top3 = 0,
        string? hints3 = null,
        string? cacheKey3 = null,
        IEnumerable<Field>? fields4 = null,
        IEnumerable<OrderField>? orderBy4 = null,
        int top4 = 0,
        string? hints4 = null,
        string? cacheKey4 = null,
        IEnumerable<Field>? fields5 = null,
        IEnumerable<OrderField>? orderBy5 = null,
        int top5 = 0,
        string? hints5 = null,
        string? cacheKey5 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
    {
        return QueryMultipleInternal<T1, T2, T3, T4, T5>(connection: connection,
            where1: ToQueryGroup(where1),
            where2: ToQueryGroup(where2),
            where3: ToQueryGroup(where3),
            where4: ToQueryGroup(where4),
            where5: ToQueryGroup(where5),
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            fields2: fields2,
            orderBy2: orderBy2,
            top2: top2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            fields3: fields3,
            orderBy3: orderBy3,
            top3: top3,
            hints3: hints3,
            cacheKey3: cacheKey3,
            fields4: fields4,
            orderBy4: orderBy4,
            top4: top4,
            hints4: hints4,
            cacheKey4: cacheKey4,
            fields5: fields5,
            orderBy5: orderBy5,
            top5: top5,
            hints5: hints5,
            cacheKey5: cacheKey5,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 5 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <typeparam name="T3">The third target type.</typeparam>
    /// <typeparam name="T4">The fourth target type.</typeparam>
    /// <typeparam name="T5">The fifth target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="where5">The query expression to be used (for T5).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for T3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
    /// <param name="top3">The number of rows to be returned (for T3).</param>
    /// <param name="hints3">The table hints to be used (for T3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for T4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
    /// <param name="top4">The number of rows to be returned (for T4).</param>
    /// <param name="hints4">The table hints to be used (for T4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields5">The mapping list of <see cref="Field"/> objects to be used (for T5).</param>
    /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
    /// <param name="top5">The number of rows to be returned (for T5).</param>
    /// <param name="hints5">The table hints to be used (for T5).</param>
    /// <param name="cacheKey5">The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 5 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>> QueryMultiple<T1, T2, T3, T4, T5>(this IDbConnection connection,
        QueryGroup? where1,
        QueryGroup? where2,
        QueryGroup? where3,
        QueryGroup? where4,
        QueryGroup? where5,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        int top2 = 0,
        string? hints2 = null,
        string? cacheKey2 = null,
        IEnumerable<Field>? fields3 = null,
        IEnumerable<OrderField>? orderBy3 = null,
        int top3 = 0,
        string? hints3 = null,
        string? cacheKey3 = null,
        IEnumerable<Field>? fields4 = null,
        IEnumerable<OrderField>? orderBy4 = null,
        int top4 = 0,
        string? hints4 = null,
        string? cacheKey4 = null,
        IEnumerable<Field>? fields5 = null,
        IEnumerable<OrderField>? orderBy5 = null,
        int top5 = 0,
        string? hints5 = null,
        string? cacheKey5 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
    {
        return QueryMultipleInternal<T1, T2, T3, T4, T5>(connection: connection,
            where1: where1,
            where2: where2,
            where3: where3,
            where4: where4,
            where5: where5,
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            fields2: fields2,
            orderBy2: orderBy2,
            top2: top2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            fields3: fields3,
            orderBy3: orderBy3,
            top3: top3,
            hints3: hints3,
            cacheKey3: cacheKey3,
            fields4: fields4,
            orderBy4: orderBy4,
            top4: top4,
            hints4: hints4,
            cacheKey4: cacheKey4,
            fields5: fields5,
            orderBy5: orderBy5,
            top5: top5,
            hints5: hints5,
            cacheKey5: cacheKey5,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 5 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <typeparam name="T3">The third target type.</typeparam>
    /// <typeparam name="T4">The fourth target type.</typeparam>
    /// <typeparam name="T5">The fifth target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="where5">The query expression to be used (for T5).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for T3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
    /// <param name="top3">The number of rows to be returned (for T3).</param>
    /// <param name="hints3">The table hints to be used (for T3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for T4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
    /// <param name="top4">The number of rows to be returned (for T4).</param>
    /// <param name="hints4">The table hints to be used (for T4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields5">The mapping list of <see cref="Field"/> objects to be used (for T5).</param>
    /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
    /// <param name="top5">The number of rows to be returned (for T5).</param>
    /// <param name="hints5">The table hints to be used (for T5).</param>
    /// <param name="cacheKey5">The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 5 enumerable target data entity types.</returns>
    internal static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>> QueryMultipleInternal<T1, T2, T3, T4, T5>(this IDbConnection connection,
        QueryGroup? where1,
        QueryGroup? where2,
        QueryGroup? where3,
        QueryGroup? where4,
        QueryGroup? where5,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        int top2 = 0,
        string? hints2 = null,
        string? cacheKey2 = null,
        IEnumerable<Field>? fields3 = null,
        IEnumerable<OrderField>? orderBy3 = null,
        int top3 = 0,
        string? hints3 = null,
        string? cacheKey3 = null,
        IEnumerable<Field>? fields4 = null,
        IEnumerable<OrderField>? orderBy4 = null,
        int top4 = 0,
        string? hints4 = null,
        string? cacheKey4 = null,
        IEnumerable<Field>? fields5 = null,
        IEnumerable<OrderField>? orderBy5 = null,
        int top5 = 0,
        string? hints5 = null,
        string? cacheKey5 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
    {
        return QueryMultipleInternal<T1, T2, T3, T4, T5>(connection: connection,
            tableName1: ClassMappedNameCache.Get<T1>(),
            where1: where1,
            tableName2: ClassMappedNameCache.Get<T2>(),
            where2: where2,
            tableName3: ClassMappedNameCache.Get<T3>(),
            where3: where3,
            tableName4: ClassMappedNameCache.Get<T4>(),
            where4: where4,
            tableName5: ClassMappedNameCache.Get<T5>(),
            where5: where5,
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            fields2: fields2,
            orderBy2: orderBy2,
            top2: top2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            fields3: fields3,
            orderBy3: orderBy3,
            top3: top3,
            hints3: hints3,
            cacheKey3: cacheKey3,
            fields4: fields4,
            orderBy4: orderBy4,
            top4: top4,
            hints4: hints4,
            cacheKey4: cacheKey4,
            fields5: fields5,
            orderBy5: orderBy5,
            top5: top5,
            hints5: hints5,
            cacheKey5: cacheKey5,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    #endregion

    #region T1, T2, T3, T4, T5, T6

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 6 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <typeparam name="T3">The third target type.</typeparam>
    /// <typeparam name="T4">The fourth target type.</typeparam>
    /// <typeparam name="T5">The fifth target type.</typeparam>
    /// <typeparam name="T6">The sixth target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="what1">The dynamic query expression or the key value to be used (for T1).</param>
    /// <param name="what2">The dynamic query expression or the key value to be used (for T2).</param>
    /// <param name="what3">The dynamic query expression or the key value to be used (for T3).</param>
    /// <param name="what4">The dynamic query expression or the key value to be used (for T4).</param>
    /// <param name="what5">The dynamic query expression or the key value to be used (for T5).</param>
    /// <param name="what6">The dynamic query expression or the key value to be used (for T6).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for T3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
    /// <param name="top3">The number of rows to be returned (for T3).</param>
    /// <param name="hints3">The table hints to be used (for T3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for T4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
    /// <param name="top4">The number of rows to be returned (for T4).</param>
    /// <param name="hints4">The table hints to be used (for T4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields5">The mapping list of <see cref="Field"/> objects to be used (for T5).</param>
    /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
    /// <param name="top5">The number of rows to be returned (for T5).</param>
    /// <param name="hints5">The table hints to be used (for T5).</param>
    /// <param name="cacheKey5">The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields6">The mapping list of <see cref="Field"/> objects to be used (for T6).</param>
    /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
    /// <param name="top6">The number of rows to be returned (for T6).</param>
    /// <param name="hints6">The table hints to be used (for T6).</param>
    /// <param name="cacheKey6">The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 6 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>> QueryMultiple<T1, T2, T3, T4, T5, T6>(this IDbConnection connection,
        object what1,
        object what2,
        object what3,
        object what4,
        object what5,
        object what6,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        int top2 = 0,
        string? hints2 = null,
        string? cacheKey2 = null,
        IEnumerable<Field>? fields3 = null,
        IEnumerable<OrderField>? orderBy3 = null,
        int top3 = 0,
        string? hints3 = null,
        string? cacheKey3 = null,
        IEnumerable<Field>? fields4 = null,
        IEnumerable<OrderField>? orderBy4 = null,
        int top4 = 0,
        string? hints4 = null,
        string? cacheKey4 = null,
        IEnumerable<Field>? fields5 = null,
        IEnumerable<OrderField>? orderBy5 = null,
        int top5 = 0,
        string? hints5 = null,
        string? cacheKey5 = null,
        IEnumerable<Field>? fields6 = null,
        IEnumerable<OrderField>? orderBy6 = null,
        int top6 = 0,
        string? hints6 = null,
        string? cacheKey6 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
    {
        return QueryMultipleInternal<T1, T2, T3, T4, T5, T6>(connection: connection,
            where1: WhatToQueryGroup(what1),
            where2: WhatToQueryGroup(what2),
            where3: WhatToQueryGroup(what3),
            where4: WhatToQueryGroup(what4),
            where5: WhatToQueryGroup(what5),
            where6: WhatToQueryGroup(what6),
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            fields2: fields2,
            orderBy2: orderBy2,
            top2: top2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            fields3: fields3,
            orderBy3: orderBy3,
            top3: top3,
            hints3: hints3,
            cacheKey3: cacheKey3,
            fields4: fields4,
            orderBy4: orderBy4,
            top4: top4,
            hints4: hints4,
            cacheKey4: cacheKey4,
            fields5: fields5,
            orderBy5: orderBy5,
            top5: top5,
            hints5: hints5,
            cacheKey5: cacheKey5,
            fields6: fields6,
            orderBy6: orderBy6,
            top6: top6,
            hints6: hints6,
            cacheKey6: cacheKey6,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 6 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <typeparam name="T3">The third target type.</typeparam>
    /// <typeparam name="T4">The fourth target type.</typeparam>
    /// <typeparam name="T5">The fifth target type.</typeparam>
    /// <typeparam name="T6">The sixth target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="where5">The query expression to be used (for T5).</param>
    /// <param name="where6">The query expression to be used (for T6).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for T3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
    /// <param name="top3">The number of rows to be returned (for T3).</param>
    /// <param name="hints3">The table hints to be used (for T3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for T4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
    /// <param name="top4">The number of rows to be returned (for T4).</param>
    /// <param name="hints4">The table hints to be used (for T4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields5">The mapping list of <see cref="Field"/> objects to be used (for T5).</param>
    /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
    /// <param name="top5">The number of rows to be returned (for T5).</param>
    /// <param name="hints5">The table hints to be used (for T5).</param>
    /// <param name="cacheKey5">The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields6">The mapping list of <see cref="Field"/> objects to be used (for T6).</param>
    /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
    /// <param name="top6">The number of rows to be returned (for T6).</param>
    /// <param name="hints6">The table hints to be used (for T6).</param>
    /// <param name="cacheKey6">The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 6 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>> QueryMultiple<T1, T2, T3, T4, T5, T6>(this IDbConnection connection,
        Expression<Func<T1, bool>> where1,
        Expression<Func<T2, bool>> where2,
        Expression<Func<T3, bool>> where3,
        Expression<Func<T4, bool>> where4,
        Expression<Func<T5, bool>> where5,
        Expression<Func<T6, bool>> where6,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        int top2 = 0,
        string? hints2 = null,
        string? cacheKey2 = null,
        IEnumerable<Field>? fields3 = null,
        IEnumerable<OrderField>? orderBy3 = null,
        int top3 = 0,
        string? hints3 = null,
        string? cacheKey3 = null,
        IEnumerable<Field>? fields4 = null,
        IEnumerable<OrderField>? orderBy4 = null,
        int top4 = 0,
        string? hints4 = null,
        string? cacheKey4 = null,
        IEnumerable<Field>? fields5 = null,
        IEnumerable<OrderField>? orderBy5 = null,
        int top5 = 0,
        string? hints5 = null,
        string? cacheKey5 = null,
        IEnumerable<Field>? fields6 = null,
        IEnumerable<OrderField>? orderBy6 = null,
        int top6 = 0,
        string? hints6 = null,
        string? cacheKey6 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
    {
        return QueryMultipleInternal<T1, T2, T3, T4, T5, T6>(connection: connection,
            where1: connection.ToQueryGroup(where1, transaction),
            where2: connection.ToQueryGroup(where2, transaction),
            where3: connection.ToQueryGroup(where3, transaction),
            where4: connection.ToQueryGroup(where4, transaction),
            where5: connection.ToQueryGroup(where5, transaction),
            where6: connection.ToQueryGroup(where6, transaction),
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            fields2: fields2,
            orderBy2: orderBy2,
            top2: top2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            fields3: fields3,
            orderBy3: orderBy3,
            top3: top3,
            hints3: hints3,
            cacheKey3: cacheKey3,
            fields4: fields4,
            orderBy4: orderBy4,
            top4: top4,
            hints4: hints4,
            cacheKey4: cacheKey4,
            fields5: fields5,
            orderBy5: orderBy5,
            top5: top5,
            hints5: hints5,
            cacheKey5: cacheKey5,
            fields6: fields6,
            orderBy6: orderBy6,
            top6: top6,
            hints6: hints6,
            cacheKey6: cacheKey6,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 6 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <typeparam name="T3">The third target type.</typeparam>
    /// <typeparam name="T4">The fourth target type.</typeparam>
    /// <typeparam name="T5">The fifth target type.</typeparam>
    /// <typeparam name="T6">The sixth target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="where5">The query expression to be used (for T5).</param>
    /// <param name="where6">The query expression to be used (for T6).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for T3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
    /// <param name="top3">The number of rows to be returned (for T3).</param>
    /// <param name="hints3">The table hints to be used (for T3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for T4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
    /// <param name="top4">The number of rows to be returned (for T4).</param>
    /// <param name="hints4">The table hints to be used (for T4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields5">The mapping list of <see cref="Field"/> objects to be used (for T5).</param>
    /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
    /// <param name="top5">The number of rows to be returned (for T5).</param>
    /// <param name="hints5">The table hints to be used (for T5).</param>
    /// <param name="cacheKey5">The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields6">The mapping list of <see cref="Field"/> objects to be used (for T6).</param>
    /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
    /// <param name="top6">The number of rows to be returned (for T6).</param>
    /// <param name="hints6">The table hints to be used (for T6).</param>
    /// <param name="cacheKey6">The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 6 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>> QueryMultiple<T1, T2, T3, T4, T5, T6>(this IDbConnection connection,
        QueryField where1,
        QueryField where2,
        QueryField where3,
        QueryField where4,
        QueryField where5,
        QueryField where6,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        int top2 = 0,
        string? hints2 = null,
        string? cacheKey2 = null,
        IEnumerable<Field>? fields3 = null,
        IEnumerable<OrderField>? orderBy3 = null,
        int top3 = 0,
        string? hints3 = null,
        string? cacheKey3 = null,
        IEnumerable<Field>? fields4 = null,
        IEnumerable<OrderField>? orderBy4 = null,
        int top4 = 0,
        string? hints4 = null,
        string? cacheKey4 = null,
        IEnumerable<Field>? fields5 = null,
        IEnumerable<OrderField>? orderBy5 = null,
        int top5 = 0,
        string? hints5 = null,
        string? cacheKey5 = null,
        IEnumerable<Field>? fields6 = null,
        IEnumerable<OrderField>? orderBy6 = null,
        int top6 = 0,
        string? hints6 = null,
        string? cacheKey6 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
    {
        return QueryMultipleInternal<T1, T2, T3, T4, T5, T6>(connection: connection,
            where1: ToQueryGroup(where1),
            where2: ToQueryGroup(where2),
            where3: ToQueryGroup(where3),
            where4: ToQueryGroup(where4),
            where5: ToQueryGroup(where5),
            where6: ToQueryGroup(where6),
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            fields2: fields2,
            orderBy2: orderBy2,
            top2: top2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            fields3: fields3,
            orderBy3: orderBy3,
            top3: top3,
            hints3: hints3,
            cacheKey3: cacheKey3,
            fields4: fields4,
            orderBy4: orderBy4,
            top4: top4,
            hints4: hints4,
            cacheKey4: cacheKey4,
            fields5: fields5,
            orderBy5: orderBy5,
            top5: top5,
            hints5: hints5,
            cacheKey5: cacheKey5,
            fields6: fields6,
            orderBy6: orderBy6,
            top6: top6,
            hints6: hints6,
            cacheKey6: cacheKey6,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 6 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <typeparam name="T3">The third target type.</typeparam>
    /// <typeparam name="T4">The fourth target type.</typeparam>
    /// <typeparam name="T5">The fifth target type.</typeparam>
    /// <typeparam name="T6">The sixth target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="where5">The query expression to be used (for T5).</param>
    /// <param name="where6">The query expression to be used (for T6).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for T3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
    /// <param name="top3">The number of rows to be returned (for T3).</param>
    /// <param name="hints3">The table hints to be used (for T3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for T4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
    /// <param name="top4">The number of rows to be returned (for T4).</param>
    /// <param name="hints4">The table hints to be used (for T4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields5">The mapping list of <see cref="Field"/> objects to be used (for T5).</param>
    /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
    /// <param name="top5">The number of rows to be returned (for T5).</param>
    /// <param name="hints5">The table hints to be used (for T5).</param>
    /// <param name="cacheKey5">The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields6">The mapping list of <see cref="Field"/> objects to be used (for T6).</param>
    /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
    /// <param name="top6">The number of rows to be returned (for T6).</param>
    /// <param name="hints6">The table hints to be used (for T6).</param>
    /// <param name="cacheKey6">The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 6 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>> QueryMultiple<T1, T2, T3, T4, T5, T6>(this IDbConnection connection,
        IEnumerable<QueryField> where1,
        IEnumerable<QueryField> where2,
        IEnumerable<QueryField> where3,
        IEnumerable<QueryField> where4,
        IEnumerable<QueryField> where5,
        IEnumerable<QueryField> where6,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        int top2 = 0,
        string? hints2 = null,
        string? cacheKey2 = null,
        IEnumerable<Field>? fields3 = null,
        IEnumerable<OrderField>? orderBy3 = null,
        int top3 = 0,
        string? hints3 = null,
        string? cacheKey3 = null,
        IEnumerable<Field>? fields4 = null,
        IEnumerable<OrderField>? orderBy4 = null,
        int top4 = 0,
        string? hints4 = null,
        string? cacheKey4 = null,
        IEnumerable<Field>? fields5 = null,
        IEnumerable<OrderField>? orderBy5 = null,
        int top5 = 0,
        string? hints5 = null,
        string? cacheKey5 = null,
        IEnumerable<Field>? fields6 = null,
        IEnumerable<OrderField>? orderBy6 = null,
        int top6 = 0,
        string? hints6 = null,
        string? cacheKey6 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
    {
        return QueryMultipleInternal<T1, T2, T3, T4, T5, T6>(connection: connection,
            where1: ToQueryGroup(where1),
            where2: ToQueryGroup(where2),
            where3: ToQueryGroup(where3),
            where4: ToQueryGroup(where4),
            where5: ToQueryGroup(where5),
            where6: ToQueryGroup(where6),
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            fields2: fields2,
            orderBy2: orderBy2,
            top2: top2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            fields3: fields3,
            orderBy3: orderBy3,
            top3: top3,
            hints3: hints3,
            cacheKey3: cacheKey3,
            fields4: fields4,
            orderBy4: orderBy4,
            top4: top4,
            hints4: hints4,
            cacheKey4: cacheKey4,
            fields5: fields5,
            orderBy5: orderBy5,
            top5: top5,
            hints5: hints5,
            cacheKey5: cacheKey5,
            fields6: fields6,
            orderBy6: orderBy6,
            top6: top6,
            hints6: hints6,
            cacheKey6: cacheKey6,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 6 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <typeparam name="T3">The third target type.</typeparam>
    /// <typeparam name="T4">The fourth target type.</typeparam>
    /// <typeparam name="T5">The fifth target type.</typeparam>
    /// <typeparam name="T6">The sixth target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="where5">The query expression to be used (for T5).</param>
    /// <param name="where6">The query expression to be used (for T6).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for T3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
    /// <param name="top3">The number of rows to be returned (for T3).</param>
    /// <param name="hints3">The table hints to be used (for T3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for T4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
    /// <param name="top4">The number of rows to be returned (for T4).</param>
    /// <param name="hints4">The table hints to be used (for T4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields5">The mapping list of <see cref="Field"/> objects to be used (for T5).</param>
    /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
    /// <param name="top5">The number of rows to be returned (for T5).</param>
    /// <param name="hints5">The table hints to be used (for T5).</param>
    /// <param name="cacheKey5">The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields6">The mapping list of <see cref="Field"/> objects to be used (for T6).</param>
    /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
    /// <param name="top6">The number of rows to be returned (for T6).</param>
    /// <param name="hints6">The table hints to be used (for T6).</param>
    /// <param name="cacheKey6">The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 6 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>> QueryMultiple<T1, T2, T3, T4, T5, T6>(this IDbConnection connection,
        QueryGroup? where1,
        QueryGroup? where2,
        QueryGroup? where3,
        QueryGroup? where4,
        QueryGroup? where5,
        QueryGroup? where6,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        int top2 = 0,
        string? hints2 = null,
        string? cacheKey2 = null,
        IEnumerable<Field>? fields3 = null,
        IEnumerable<OrderField>? orderBy3 = null,
        int top3 = 0,
        string? hints3 = null,
        string? cacheKey3 = null,
        IEnumerable<Field>? fields4 = null,
        IEnumerable<OrderField>? orderBy4 = null,
        int top4 = 0,
        string? hints4 = null,
        string? cacheKey4 = null,
        IEnumerable<Field>? fields5 = null,
        IEnumerable<OrderField>? orderBy5 = null,
        int top5 = 0,
        string? hints5 = null,
        string? cacheKey5 = null,
        IEnumerable<Field>? fields6 = null,
        IEnumerable<OrderField>? orderBy6 = null,
        int top6 = 0,
        string? hints6 = null,
        string? cacheKey6 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
    {
        return QueryMultipleInternal<T1, T2, T3, T4, T5, T6>(connection: connection,
            where1: where1,
            where2: where2,
            where3: where3,
            where4: where4,
            where5: where5,
            where6: where6,
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            fields2: fields2,
            orderBy2: orderBy2,
            top2: top2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            fields3: fields3,
            orderBy3: orderBy3,
            top3: top3,
            hints3: hints3,
            cacheKey3: cacheKey3,
            fields4: fields4,
            orderBy4: orderBy4,
            top4: top4,
            hints4: hints4,
            cacheKey4: cacheKey4,
            fields5: fields5,
            orderBy5: orderBy5,
            top5: top5,
            hints5: hints5,
            cacheKey5: cacheKey5,
            fields6: fields6,
            orderBy6: orderBy6,
            top6: top6,
            hints6: hints6,
            cacheKey6: cacheKey6,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 6 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <typeparam name="T3">The third target type.</typeparam>
    /// <typeparam name="T4">The fourth target type.</typeparam>
    /// <typeparam name="T5">The fifth target type.</typeparam>
    /// <typeparam name="T6">The sixth target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="where5">The query expression to be used (for T5).</param>
    /// <param name="where6">The query expression to be used (for T6).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for T3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
    /// <param name="top3">The number of rows to be returned (for T3).</param>
    /// <param name="hints3">The table hints to be used (for T3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for T4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
    /// <param name="top4">The number of rows to be returned (for T4).</param>
    /// <param name="hints4">The table hints to be used (for T4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields5">The mapping list of <see cref="Field"/> objects to be used (for T5).</param>
    /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
    /// <param name="top5">The number of rows to be returned (for T5).</param>
    /// <param name="hints5">The table hints to be used (for T5).</param>
    /// <param name="cacheKey5">The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields6">The mapping list of <see cref="Field"/> objects to be used (for T6).</param>
    /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
    /// <param name="top6">The number of rows to be returned (for T6).</param>
    /// <param name="hints6">The table hints to be used (for T6).</param>
    /// <param name="cacheKey6">The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 6 enumerable target data entity types.</returns>
    internal static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>> QueryMultipleInternal<T1, T2, T3, T4, T5, T6>(this IDbConnection connection,
        QueryGroup? where1,
        QueryGroup? where2,
        QueryGroup? where3,
        QueryGroup? where4,
        QueryGroup? where5,
        QueryGroup? where6,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        int top2 = 0,
        string? hints2 = null,
        string? cacheKey2 = null,
        IEnumerable<Field>? fields3 = null,
        IEnumerable<OrderField>? orderBy3 = null,
        int top3 = 0,
        string? hints3 = null,
        string? cacheKey3 = null,
        IEnumerable<Field>? fields4 = null,
        IEnumerable<OrderField>? orderBy4 = null,
        int top4 = 0,
        string? hints4 = null,
        string? cacheKey4 = null,
        IEnumerable<Field>? fields5 = null,
        IEnumerable<OrderField>? orderBy5 = null,
        int top5 = 0,
        string? hints5 = null,
        string? cacheKey5 = null,
        IEnumerable<Field>? fields6 = null,
        IEnumerable<OrderField>? orderBy6 = null,
        int top6 = 0,
        string? hints6 = null,
        string? cacheKey6 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
    {
        return QueryMultipleInternal<T1, T2, T3, T4, T5, T6>(connection: connection,
            tableName1: ClassMappedNameCache.Get<T1>(),
            where1: where1,
            tableName2: ClassMappedNameCache.Get<T2>(),
            where2: where2,
            tableName3: ClassMappedNameCache.Get<T3>(),
            where3: where3,
            tableName4: ClassMappedNameCache.Get<T4>(),
            where4: where4,
            tableName5: ClassMappedNameCache.Get<T5>(),
            where5: where5,
            tableName6: ClassMappedNameCache.Get<T6>(),
            where6: where6,
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            fields2: fields2,
            orderBy2: orderBy2,
            top2: top2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            fields3: fields3,
            orderBy3: orderBy3,
            top3: top3,
            hints3: hints3,
            cacheKey3: cacheKey3,
            fields4: fields4,
            orderBy4: orderBy4,
            top4: top4,
            hints4: hints4,
            cacheKey4: cacheKey4,
            fields5: fields5,
            orderBy5: orderBy5,
            top5: top5,
            hints5: hints5,
            cacheKey5: cacheKey5,
            fields6: fields6,
            orderBy6: orderBy6,
            top6: top6,
            hints6: hints6,
            cacheKey6: cacheKey6,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    #endregion

    #region T1, T2, T3, T4, T5, T6, T7

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 7 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <typeparam name="T3">The third target type.</typeparam>
    /// <typeparam name="T4">The fourth target type.</typeparam>
    /// <typeparam name="T5">The fifth target type.</typeparam>
    /// <typeparam name="T6">The sixth target type.</typeparam>
    /// <typeparam name="T7">The seventh target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="what1">The dynamic query expression or the key value to be used (for T1).</param>
    /// <param name="what2">The dynamic query expression or the key value to be used (for T2).</param>
    /// <param name="what3">The dynamic query expression or the key value to be used (for T3).</param>
    /// <param name="what4">The dynamic query expression or the key value to be used (for T4).</param>
    /// <param name="what5">The dynamic query expression or the key value to be used (for T5).</param>
    /// <param name="what6">The dynamic query expression or the key value to be used (for T6).</param>
    /// <param name="what7">The dynamic query expression or the key value to be used (for T7).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for T3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
    /// <param name="top3">The number of rows to be returned (for T3).</param>
    /// <param name="hints3">The table hints to be used (for T3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for T4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
    /// <param name="top4">The number of rows to be returned (for T4).</param>
    /// <param name="hints4">The table hints to be used (for T4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields5">The mapping list of <see cref="Field"/> objects to be used (for T5).</param>
    /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
    /// <param name="top5">The number of rows to be returned (for T5).</param>
    /// <param name="hints5">The table hints to be used (for T5).</param>
    /// <param name="cacheKey5">The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields6">The mapping list of <see cref="Field"/> objects to be used (for T6).</param>
    /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
    /// <param name="top6">The number of rows to be returned (for T6).</param>
    /// <param name="hints6">The table hints to be used (for T6).</param>
    /// <param name="cacheKey6">The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields7">The mapping list of <see cref="Field"/> objects to be used (for T7).</param>
    /// <param name="orderBy7">The order definition of the fields to be used (for T7).</param>
    /// <param name="top7">The number of rows to be returned (for T7).</param>
    /// <param name="hints7">The table hints to be used (for T7).</param>
    /// <param name="cacheKey7">The key to the cache item 7. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 7 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>> QueryMultiple<T1, T2, T3, T4, T5, T6, T7>(this IDbConnection connection,
        object what1,
        object what2,
        object what3,
        object what4,
        object what5,
        object what6,
        object what7,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        int top2 = 0,
        string? hints2 = null,
        string? cacheKey2 = null,
        IEnumerable<Field>? fields3 = null,
        IEnumerable<OrderField>? orderBy3 = null,
        int top3 = 0,
        string? hints3 = null,
        string? cacheKey3 = null,
        IEnumerable<Field>? fields4 = null,
        IEnumerable<OrderField>? orderBy4 = null,
        int top4 = 0,
        string? hints4 = null,
        string? cacheKey4 = null,
        IEnumerable<Field>? fields5 = null,
        IEnumerable<OrderField>? orderBy5 = null,
        int top5 = 0,
        string? hints5 = null,
        string? cacheKey5 = null,
        IEnumerable<Field>? fields6 = null,
        IEnumerable<OrderField>? orderBy6 = null,
        int top6 = 0,
        string? hints6 = null,
        string? cacheKey6 = null,
        IEnumerable<Field>? fields7 = null,
        IEnumerable<OrderField>? orderBy7 = null,
        int top7 = 0,
        string? hints7 = null,
        string? cacheKey7 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
        where T7 : class
    {
        return QueryMultipleInternal<T1, T2, T3, T4, T5, T6, T7>(connection: connection,
            where1: WhatToQueryGroup(what1),
            where2: WhatToQueryGroup(what2),
            where3: WhatToQueryGroup(what3),
            where4: WhatToQueryGroup(what4),
            where5: WhatToQueryGroup(what5),
            where6: WhatToQueryGroup(what6),
            where7: WhatToQueryGroup(what7),
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            fields2: fields2,
            orderBy2: orderBy2,
            top2: top2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            fields3: fields3,
            orderBy3: orderBy3,
            top3: top3,
            hints3: hints3,
            cacheKey3: cacheKey3,
            fields4: fields4,
            orderBy4: orderBy4,
            top4: top4,
            hints4: hints4,
            cacheKey4: cacheKey4,
            fields5: fields5,
            orderBy5: orderBy5,
            top5: top5,
            hints5: hints5,
            cacheKey5: cacheKey5,
            fields6: fields6,
            orderBy6: orderBy6,
            top6: top6,
            hints6: hints6,
            cacheKey6: cacheKey6,
            fields7: fields7,
            orderBy7: orderBy7,
            top7: top7,
            hints7: hints7,
            cacheKey7: cacheKey7,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 7 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <typeparam name="T3">The third target type.</typeparam>
    /// <typeparam name="T4">The fourth target type.</typeparam>
    /// <typeparam name="T5">The fifth target type.</typeparam>
    /// <typeparam name="T6">The sixth target type.</typeparam>
    /// <typeparam name="T7">The seventh target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="where5">The query expression to be used (for T5).</param>
    /// <param name="where6">The query expression to be used (for T6).</param>
    /// <param name="where7">The query expression to be used (for T7).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for T3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
    /// <param name="top3">The number of rows to be returned (for T3).</param>
    /// <param name="hints3">The table hints to be used (for T3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for T4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
    /// <param name="top4">The number of rows to be returned (for T4).</param>
    /// <param name="hints4">The table hints to be used (for T4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields5">The mapping list of <see cref="Field"/> objects to be used (for T5).</param>
    /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
    /// <param name="top5">The number of rows to be returned (for T5).</param>
    /// <param name="hints5">The table hints to be used (for T5).</param>
    /// <param name="cacheKey5">The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields6">The mapping list of <see cref="Field"/> objects to be used (for T6).</param>
    /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
    /// <param name="top6">The number of rows to be returned (for T6).</param>
    /// <param name="hints6">The table hints to be used (for T6).</param>
    /// <param name="cacheKey6">The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields7">The mapping list of <see cref="Field"/> objects to be used (for T7).</param>
    /// <param name="orderBy7">The order definition of the fields to be used (for T7).</param>
    /// <param name="top7">The number of rows to be returned (for T7).</param>
    /// <param name="hints7">The table hints to be used (for T7).</param>
    /// <param name="cacheKey7">The key to the cache item 7. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 7 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>> QueryMultiple<T1, T2, T3, T4, T5, T6, T7>(this IDbConnection connection,
        Expression<Func<T1, bool>> where1,
        Expression<Func<T2, bool>> where2,
        Expression<Func<T3, bool>> where3,
        Expression<Func<T4, bool>> where4,
        Expression<Func<T5, bool>> where5,
        Expression<Func<T6, bool>> where6,
        Expression<Func<T7, bool>> where7,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        int top2 = 0,
        string? hints2 = null,
        string? cacheKey2 = null,
        IEnumerable<Field>? fields3 = null,
        IEnumerable<OrderField>? orderBy3 = null,
        int top3 = 0,
        string? hints3 = null,
        string? cacheKey3 = null,
        IEnumerable<Field>? fields4 = null,
        IEnumerable<OrderField>? orderBy4 = null,
        int top4 = 0,
        string? hints4 = null,
        string? cacheKey4 = null,
        IEnumerable<Field>? fields5 = null,
        IEnumerable<OrderField>? orderBy5 = null,
        int top5 = 0,
        string? hints5 = null,
        string? cacheKey5 = null,
        IEnumerable<Field>? fields6 = null,
        IEnumerable<OrderField>? orderBy6 = null,
        int top6 = 0,
        string? hints6 = null,
        string? cacheKey6 = null,
        IEnumerable<Field>? fields7 = null,
        IEnumerable<OrderField>? orderBy7 = null,
        int top7 = 0,
        string? hints7 = null,
        string? cacheKey7 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
        where T7 : class
    {
        return QueryMultipleInternal<T1, T2, T3, T4, T5, T6, T7>(connection: connection,
            where1: connection.ToQueryGroup(where1, transaction),
            where2: connection.ToQueryGroup(where2, transaction),
            where3: connection.ToQueryGroup(where3, transaction),
            where4: connection.ToQueryGroup(where4, transaction),
            where5: connection.ToQueryGroup(where5, transaction),
            where6: connection.ToQueryGroup(where6, transaction),
            where7: connection.ToQueryGroup(where7, transaction),
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            fields2: fields2,
            orderBy2: orderBy2,
            top2: top2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            fields3: fields3,
            orderBy3: orderBy3,
            top3: top3,
            hints3: hints3,
            cacheKey3: cacheKey3,
            fields4: fields4,
            orderBy4: orderBy4,
            top4: top4,
            hints4: hints4,
            cacheKey4: cacheKey4,
            fields5: fields5,
            orderBy5: orderBy5,
            top5: top5,
            hints5: hints5,
            cacheKey5: cacheKey5,
            fields6: fields6,
            orderBy6: orderBy6,
            top6: top6,
            hints6: hints6,
            cacheKey6: cacheKey6,
            fields7: fields7,
            orderBy7: orderBy7,
            top7: top7,
            hints7: hints7,
            cacheKey7: cacheKey7,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 7 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <typeparam name="T3">The third target type.</typeparam>
    /// <typeparam name="T4">The fourth target type.</typeparam>
    /// <typeparam name="T5">The fifth target type.</typeparam>
    /// <typeparam name="T6">The sixth target type.</typeparam>
    /// <typeparam name="T7">The seventh target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="where5">The query expression to be used (for T5).</param>
    /// <param name="where6">The query expression to be used (for T6).</param>
    /// <param name="where7">The query expression to be used (for T7).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for T3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
    /// <param name="top3">The number of rows to be returned (for T3).</param>
    /// <param name="hints3">The table hints to be used (for T3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for T4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
    /// <param name="top4">The number of rows to be returned (for T4).</param>
    /// <param name="hints4">The table hints to be used (for T4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields5">The mapping list of <see cref="Field"/> objects to be used (for T5).</param>
    /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
    /// <param name="top5">The number of rows to be returned (for T5).</param>
    /// <param name="hints5">The table hints to be used (for T5).</param>
    /// <param name="cacheKey5">The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields6">The mapping list of <see cref="Field"/> objects to be used (for T6).</param>
    /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
    /// <param name="top6">The number of rows to be returned (for T6).</param>
    /// <param name="hints6">The table hints to be used (for T6).</param>
    /// <param name="cacheKey6">The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields7">The mapping list of <see cref="Field"/> objects to be used (for T7).</param>
    /// <param name="orderBy7">The order definition of the fields to be used (for T7).</param>
    /// <param name="top7">The number of rows to be returned (for T7).</param>
    /// <param name="hints7">The table hints to be used (for T7).</param>
    /// <param name="cacheKey7">The key to the cache item 7. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 7 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>> QueryMultiple<T1, T2, T3, T4, T5, T6, T7>(this IDbConnection connection,
        QueryField where1,
        QueryField where2,
        QueryField where3,
        QueryField where4,
        QueryField where5,
        QueryField where6,
        QueryField where7,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        int top2 = 0,
        string? hints2 = null,
        string? cacheKey2 = null,
        IEnumerable<Field>? fields3 = null,
        IEnumerable<OrderField>? orderBy3 = null,
        int top3 = 0,
        string? hints3 = null,
        string? cacheKey3 = null,
        IEnumerable<Field>? fields4 = null,
        IEnumerable<OrderField>? orderBy4 = null,
        int top4 = 0,
        string? hints4 = null,
        string? cacheKey4 = null,
        IEnumerable<Field>? fields5 = null,
        IEnumerable<OrderField>? orderBy5 = null,
        int top5 = 0,
        string? hints5 = null,
        string? cacheKey5 = null,
        IEnumerable<Field>? fields6 = null,
        IEnumerable<OrderField>? orderBy6 = null,
        int top6 = 0,
        string? hints6 = null,
        string? cacheKey6 = null,
        IEnumerable<Field>? fields7 = null,
        IEnumerable<OrderField>? orderBy7 = null,
        int top7 = 0,
        string? hints7 = null,
        string? cacheKey7 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
        where T7 : class
    {
        return QueryMultipleInternal<T1, T2, T3, T4, T5, T6, T7>(connection: connection,
            where1: ToQueryGroup(where1),
            where2: ToQueryGroup(where2),
            where3: ToQueryGroup(where3),
            where4: ToQueryGroup(where4),
            where5: ToQueryGroup(where5),
            where6: ToQueryGroup(where6),
            where7: ToQueryGroup(where7),
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            fields2: fields2,
            orderBy2: orderBy2,
            top2: top2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            fields3: fields3,
            orderBy3: orderBy3,
            top3: top3,
            hints3: hints3,
            cacheKey3: cacheKey3,
            fields4: fields4,
            orderBy4: orderBy4,
            top4: top4,
            hints4: hints4,
            cacheKey4: cacheKey4,
            fields5: fields5,
            orderBy5: orderBy5,
            top5: top5,
            hints5: hints5,
            cacheKey5: cacheKey5,
            fields6: fields6,
            orderBy6: orderBy6,
            top6: top6,
            hints6: hints6,
            cacheKey6: cacheKey6,
            fields7: fields7,
            orderBy7: orderBy7,
            top7: top7,
            hints7: hints7,
            cacheKey7: cacheKey7,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 7 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <typeparam name="T3">The third target type.</typeparam>
    /// <typeparam name="T4">The fourth target type.</typeparam>
    /// <typeparam name="T5">The fifth target type.</typeparam>
    /// <typeparam name="T6">The sixth target type.</typeparam>
    /// <typeparam name="T7">The seventh target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="where5">The query expression to be used (for T5).</param>
    /// <param name="where6">The query expression to be used (for T6).</param>
    /// <param name="where7">The query expression to be used (for T7).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for T3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
    /// <param name="top3">The number of rows to be returned (for T3).</param>
    /// <param name="hints3">The table hints to be used (for T3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for T4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
    /// <param name="top4">The number of rows to be returned (for T4).</param>
    /// <param name="hints4">The table hints to be used (for T4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields5">The mapping list of <see cref="Field"/> objects to be used (for T5).</param>
    /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
    /// <param name="top5">The number of rows to be returned (for T5).</param>
    /// <param name="hints5">The table hints to be used (for T5).</param>
    /// <param name="cacheKey5">The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields6">The mapping list of <see cref="Field"/> objects to be used (for T6).</param>
    /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
    /// <param name="top6">The number of rows to be returned (for T6).</param>
    /// <param name="hints6">The table hints to be used (for T6).</param>
    /// <param name="cacheKey6">The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields7">The mapping list of <see cref="Field"/> objects to be used (for T7).</param>
    /// <param name="orderBy7">The order definition of the fields to be used (for T7).</param>
    /// <param name="top7">The number of rows to be returned (for T7).</param>
    /// <param name="hints7">The table hints to be used (for T7).</param>
    /// <param name="cacheKey7">The key to the cache item 7. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 7 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>> QueryMultiple<T1, T2, T3, T4, T5, T6, T7>(this IDbConnection connection,
        IEnumerable<QueryField> where1,
        IEnumerable<QueryField> where2,
        IEnumerable<QueryField> where3,
        IEnumerable<QueryField> where4,
        IEnumerable<QueryField> where5,
        IEnumerable<QueryField> where6,
        IEnumerable<QueryField> where7,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        int top2 = 0,
        string? hints2 = null,
        string? cacheKey2 = null,
        IEnumerable<Field>? fields3 = null,
        IEnumerable<OrderField>? orderBy3 = null,
        int top3 = 0,
        string? hints3 = null,
        string? cacheKey3 = null,
        IEnumerable<Field>? fields4 = null,
        IEnumerable<OrderField>? orderBy4 = null,
        int top4 = 0,
        string? hints4 = null,
        string? cacheKey4 = null,
        IEnumerable<Field>? fields5 = null,
        IEnumerable<OrderField>? orderBy5 = null,
        int top5 = 0,
        string? hints5 = null,
        string? cacheKey5 = null,
        IEnumerable<Field>? fields6 = null,
        IEnumerable<OrderField>? orderBy6 = null,
        int top6 = 0,
        string? hints6 = null,
        string? cacheKey6 = null,
        IEnumerable<Field>? fields7 = null,
        IEnumerable<OrderField>? orderBy7 = null,
        int top7 = 0,
        string? hints7 = null,
        string? cacheKey7 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
        where T7 : class
    {
        return QueryMultipleInternal<T1, T2, T3, T4, T5, T6, T7>(connection: connection,
            where1: ToQueryGroup(where1),
            where2: ToQueryGroup(where2),
            where3: ToQueryGroup(where3),
            where4: ToQueryGroup(where4),
            where5: ToQueryGroup(where5),
            where6: ToQueryGroup(where6),
            where7: ToQueryGroup(where7),
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            fields2: fields2,
            orderBy2: orderBy2,
            top2: top2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            fields3: fields3,
            orderBy3: orderBy3,
            top3: top3,
            hints3: hints3,
            cacheKey3: cacheKey3,
            fields4: fields4,
            orderBy4: orderBy4,
            top4: top4,
            hints4: hints4,
            cacheKey4: cacheKey4,
            fields5: fields5,
            orderBy5: orderBy5,
            top5: top5,
            hints5: hints5,
            cacheKey5: cacheKey5,
            fields6: fields6,
            orderBy6: orderBy6,
            top6: top6,
            hints6: hints6,
            cacheKey6: cacheKey6,
            fields7: fields7,
            orderBy7: orderBy7,
            top7: top7,
            hints7: hints7,
            cacheKey7: cacheKey7,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 7 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <typeparam name="T3">The third target type.</typeparam>
    /// <typeparam name="T4">The fourth target type.</typeparam>
    /// <typeparam name="T5">The fifth target type.</typeparam>
    /// <typeparam name="T6">The sixth target type.</typeparam>
    /// <typeparam name="T7">The seventh target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="where5">The query expression to be used (for T5).</param>
    /// <param name="where6">The query expression to be used (for T6).</param>
    /// <param name="where7">The query expression to be used (for T7).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for T3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
    /// <param name="top3">The number of rows to be returned (for T3).</param>
    /// <param name="hints3">The table hints to be used (for T3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for T4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
    /// <param name="top4">The number of rows to be returned (for T4).</param>
    /// <param name="hints4">The table hints to be used (for T4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields5">The mapping list of <see cref="Field"/> objects to be used (for T5).</param>
    /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
    /// <param name="top5">The number of rows to be returned (for T5).</param>
    /// <param name="hints5">The table hints to be used (for T5).</param>
    /// <param name="cacheKey5">The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields6">The mapping list of <see cref="Field"/> objects to be used (for T6).</param>
    /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
    /// <param name="top6">The number of rows to be returned (for T6).</param>
    /// <param name="hints6">The table hints to be used (for T6).</param>
    /// <param name="cacheKey6">The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields7">The mapping list of <see cref="Field"/> objects to be used (for T7).</param>
    /// <param name="orderBy7">The order definition of the fields to be used (for T7).</param>
    /// <param name="top7">The number of rows to be returned (for T7).</param>
    /// <param name="hints7">The table hints to be used (for T7).</param>
    /// <param name="cacheKey7">The key to the cache item 7. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 7 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>> QueryMultiple<T1, T2, T3, T4, T5, T6, T7>(this IDbConnection connection,
        QueryGroup? where1,
        QueryGroup? where2,
        QueryGroup? where3,
        QueryGroup? where4,
        QueryGroup? where5,
        QueryGroup? where6,
        QueryGroup? where7,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        int top2 = 0,
        string? hints2 = null,
        string? cacheKey2 = null,
        IEnumerable<Field>? fields3 = null,
        IEnumerable<OrderField>? orderBy3 = null,
        int top3 = 0,
        string? hints3 = null,
        string? cacheKey3 = null,
        IEnumerable<Field>? fields4 = null,
        IEnumerable<OrderField>? orderBy4 = null,
        int top4 = 0,
        string? hints4 = null,
        string? cacheKey4 = null,
        IEnumerable<Field>? fields5 = null,
        IEnumerable<OrderField>? orderBy5 = null,
        int top5 = 0,
        string? hints5 = null,
        string? cacheKey5 = null,
        IEnumerable<Field>? fields6 = null,
        IEnumerable<OrderField>? orderBy6 = null,
        int top6 = 0,
        string? hints6 = null,
        string? cacheKey6 = null,
        IEnumerable<Field>? fields7 = null,
        IEnumerable<OrderField>? orderBy7 = null,
        int top7 = 0,
        string? hints7 = null,
        string? cacheKey7 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
        where T7 : class
    {
        return QueryMultipleInternal<T1, T2, T3, T4, T5, T6, T7>(connection: connection,
            where1: where1,
            where2: where2,
            where3: where3,
            where4: where4,
            where5: where5,
            where6: where6,
            where7: where7,
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            fields2: fields2,
            orderBy2: orderBy2,
            top2: top2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            fields3: fields3,
            orderBy3: orderBy3,
            top3: top3,
            hints3: hints3,
            cacheKey3: cacheKey3,
            fields4: fields4,
            orderBy4: orderBy4,
            top4: top4,
            hints4: hints4,
            cacheKey4: cacheKey4,
            fields5: fields5,
            orderBy5: orderBy5,
            top5: top5,
            hints5: hints5,
            cacheKey5: cacheKey5,
            fields6: fields6,
            orderBy6: orderBy6,
            top6: top6,
            hints6: hints6,
            cacheKey6: cacheKey6,
            fields7: fields7,
            orderBy7: orderBy7,
            top7: top7,
            hints7: hints7,
            cacheKey7: cacheKey7,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 7 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <typeparam name="T3">The third target type.</typeparam>
    /// <typeparam name="T4">The fourth target type.</typeparam>
    /// <typeparam name="T5">The fifth target type.</typeparam>
    /// <typeparam name="T6">The sixth target type.</typeparam>
    /// <typeparam name="T7">The seventh target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="where5">The query expression to be used (for T5).</param>
    /// <param name="where6">The query expression to be used (for T6).</param>
    /// <param name="where7">The query expression to be used (for T7).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for T2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
    /// <param name="top2">The number of rows to be returned (for T2).</param>
    /// <param name="hints2">The table hints to be used (for T2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for T3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
    /// <param name="top3">The number of rows to be returned (for T3).</param>
    /// <param name="hints3">The table hints to be used (for T3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for T4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
    /// <param name="top4">The number of rows to be returned (for T4).</param>
    /// <param name="hints4">The table hints to be used (for T4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields5">The mapping list of <see cref="Field"/> objects to be used (for T5).</param>
    /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
    /// <param name="top5">The number of rows to be returned (for T5).</param>
    /// <param name="hints5">The table hints to be used (for T5).</param>
    /// <param name="cacheKey5">The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields6">The mapping list of <see cref="Field"/> objects to be used (for T6).</param>
    /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
    /// <param name="top6">The number of rows to be returned (for T6).</param>
    /// <param name="hints6">The table hints to be used (for T6).</param>
    /// <param name="cacheKey6">The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields7">The mapping list of <see cref="Field"/> objects to be used (for T7).</param>
    /// <param name="orderBy7">The order definition of the fields to be used (for T7).</param>
    /// <param name="top7">The number of rows to be returned (for T7).</param>
    /// <param name="hints7">The table hints to be used (for T7).</param>
    /// <param name="cacheKey7">The key to the cache item 7. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 7 enumerable target data entity types.</returns>
    internal static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>> QueryMultipleInternal<T1, T2, T3, T4, T5, T6, T7>(this IDbConnection connection,
        QueryGroup? where1,
        QueryGroup? where2,
        QueryGroup? where3,
        QueryGroup? where4,
        QueryGroup? where5,
        QueryGroup? where6,
        QueryGroup? where7,
        IEnumerable<Field>? fields1 = null,
        IEnumerable<OrderField>? orderBy1 = null,
        int top1 = 0,
        string? hints1 = null,
        string? cacheKey1 = null,
        IEnumerable<Field>? fields2 = null,
        IEnumerable<OrderField>? orderBy2 = null,
        int top2 = 0,
        string? hints2 = null,
        string? cacheKey2 = null,
        IEnumerable<Field>? fields3 = null,
        IEnumerable<OrderField>? orderBy3 = null,
        int top3 = 0,
        string? hints3 = null,
        string? cacheKey3 = null,
        IEnumerable<Field>? fields4 = null,
        IEnumerable<OrderField>? orderBy4 = null,
        int top4 = 0,
        string? hints4 = null,
        string? cacheKey4 = null,
        IEnumerable<Field>? fields5 = null,
        IEnumerable<OrderField>? orderBy5 = null,
        int top5 = 0,
        string? hints5 = null,
        string? cacheKey5 = null,
        IEnumerable<Field>? fields6 = null,
        IEnumerable<OrderField>? orderBy6 = null,
        int top6 = 0,
        string? hints6 = null,
        string? cacheKey6 = null,
        IEnumerable<Field>? fields7 = null,
        IEnumerable<OrderField>? orderBy7 = null,
        int top7 = 0,
        string? hints7 = null,
        string? cacheKey7 = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryMultiple,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
        where T7 : class
    {
        return QueryMultipleInternal<T1, T2, T3, T4, T5, T6, T7>(connection: connection,
            tableName1: ClassMappedNameCache.Get<T1>(),
            where1: where1,
            tableName2: ClassMappedNameCache.Get<T2>(),
            where2: where2,
            tableName3: ClassMappedNameCache.Get<T3>(),
            where3: where3,
            tableName4: ClassMappedNameCache.Get<T4>(),
            where4: where4,
            tableName5: ClassMappedNameCache.Get<T5>(),
            where5: where5,
            tableName6: ClassMappedNameCache.Get<T6>(),
            where6: where6,
            tableName7: ClassMappedNameCache.Get<T7>(),
            where7: where7,
            fields1: fields1,
            orderBy1: orderBy1,
            top1: top1,
            hints1: hints1,
            cacheKey1: cacheKey1,
            fields2: fields2,
            orderBy2: orderBy2,
            top2: top2,
            hints2: hints2,
            cacheKey2: cacheKey2,
            fields3: fields3,
            orderBy3: orderBy3,
            top3: top3,
            hints3: hints3,
            cacheKey3: cacheKey3,
            fields4: fields4,
            orderBy4: orderBy4,
            top4: top4,
            hints4: hints4,
            cacheKey4: cacheKey4,
            fields5: fields5,
            orderBy5: orderBy5,
            top5: top5,
            hints5: hints5,
            cacheKey5: cacheKey5,
            fields6: fields6,
            orderBy6: orderBy6,
            top6: top6,
            hints6: hints6,
            cacheKey6: cacheKey6,
            fields7: fields7,
            orderBy7: orderBy7,
            top7: top7,
            hints7: hints7,
            cacheKey7: cacheKey7,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: cache,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    #endregion

    #endregion
}
