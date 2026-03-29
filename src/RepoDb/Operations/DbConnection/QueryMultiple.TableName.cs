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
    #region QueryMultiple(TableName)

    #region TEntity

    #region T1, T2

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 2 target types.
    /// </summary>
    /// <typeparam name="T1">The first target type.</typeparam>
    /// <typeparam name="T2">The second target type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="what1">The dynamic expression or the key value to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
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
        string tableName1,
        object what1,
        string tableName2,
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
            tableName1: tableName1,
            where1: WhatToQueryGroup(typeof(T1), connection, what1, transaction),
            tableName2: tableName2,
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
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
        string tableName1,
        Expression<Func<T1, bool>> where1,
        string tableName2,
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
            tableName1: tableName1,
            where1: connection.ToQueryGroup(where1, transaction),
            tableName2: tableName2,
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
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
        string tableName1,
        QueryField where1,
        string tableName2,
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
            tableName1: tableName1,
            where1: ToQueryGroup(where1),
            tableName2: tableName2,
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
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
        string tableName1,
        IEnumerable<QueryField> where1,
        string tableName2,
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
            tableName1: tableName1,
            where1: ToQueryGroup(where1),
            tableName2: tableName2,
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
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
        string tableName1,
        QueryGroup? where1,
        string tableName2,
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
            tableName1: tableName1,
            where1: where1,
            tableName2: tableName2,
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for T1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
    /// <param name="top1">The number of rows to be returned (for T1).</param>
    /// <param name="hints1">The table hints to be used (for T1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
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
        string tableName1,
        QueryGroup? where1,
        string tableName2,
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
        // Variables
        var commandType = CommandType.Text;
        var queryGroups = new List<QueryGroup?>();
        var maps = new List<QueryGroupTypeMap>();
        var commandTexts = new List<string>();

        // Items
        var item1 = QueryMultipleInternal<T1>(cacheKey1,
            cache,
            where1,
            queryGroups);

        var item2 = QueryMultipleInternal<T2>(cacheKey2,
            cache,
            where2,
            queryGroups);

        // Fix
        QueryGroup.FixForQueryMultiple(queryGroups);

        // Item1 Request
        if (item1 is null)
        {
            var request1 = new QueryMultipleRequest(1,
                tableName1,
                connection,
                transaction,
                fields1 ?? FieldCache.Get<T1>(),
                where1,
                orderBy1,
                top1,
                hints1,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request1, CommandTextCache.GetQueryMultipleText));
            if (where1 is not null)
                maps.Add(where1.MapTo<T1>(tableName1));
        }

        // Item2 Request
        if (item2 is null)
        {
            var request2 = new QueryMultipleRequest(2,
                tableName2,
                connection,
                transaction,
                fields2 ?? FieldCache.Get<T2>(),
                where2,
                orderBy2,
                top2,
                hints2,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request2, CommandTextCache.GetQueryMultipleText));
            if (where2 is not null)
                maps.Add(where2.MapTo<T2>(tableName2));
        }


        var (commandText, param) = CombineQueryMultiple(connection, transaction, commandTexts, maps, statementBuilder);
        Tuple<IEnumerable<T1>, IEnumerable<T2>> result;
        TraceResult? traceResult = null;

        // Before Execution
        var beforeExecutionCallback = CreateCallback(command =>
            traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command));

        // Actual Execution
        using (var reader = ExecuteReaderInternal(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            commandTimeout: commandTimeout,
            traceKey: null,
            transaction: transaction,
            trace: null,
            entityType: null,
            dbFields: null,
            skipCommandArrayParametersCheck: true,
            beforeExecutionCallback: beforeExecutionCallback))
        {
            // Silent cancellation
            if (traceResult?.CancellableTraceLog?.IsCancelled == true)
            {
                return new([], []);
            }

            // DB setting
            var dbSetting = connection.GetDbSetting();

            // T1
            item1 = QueryMultipleInternal(connection,
                reader,
                item1,
                tableName1,
                cacheKey1,
                cacheItemExpiration,
                transaction,
                cache,
                false);

            // T2
            item2 = QueryMultipleInternal(connection,
                reader,
                item2,
                tableName2,
                cacheKey2,
                cacheItemExpiration,
                transaction,
                cache,
                true);

            // Result
            result = Tuple.Create(item1, item2);

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);
        }

        // Result
        return result;
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="what1">The dynamic query expression or the key value to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
    /// <param name="what2">The dynamic query expression or the key value to be used (for T2).</param>
    /// <param name="tableName3">The name of the target table (for T3).</param>
    /// <param name="what3">The dynamic expression or the key value to be used (for T3).</param>
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
        string tableName1,
        object what1,
        string tableName2,
        object what2,
        string tableName3,
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
            tableName1: tableName1,
            where1: WhatToQueryGroup(typeof(T1), connection, what1, transaction),
            tableName2: tableName2,
            where2: WhatToQueryGroup(typeof(T2), connection, what2, transaction),
            tableName3: tableName3,
            where3: WhatToQueryGroup(typeof(T3), connection, what3, transaction),
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="tableName3">The name of the target table (for T3).</param>
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
        string tableName1,
        Expression<Func<T1, bool>> where1,
        string tableName2,
        Expression<Func<T2, bool>> where2,
        string tableName3,
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
            tableName1: tableName1,
            where1: connection.ToQueryGroup(where1, transaction),
            tableName2: tableName2,
            where2: connection.ToQueryGroup(where2, transaction),
            tableName3: tableName3,
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="tableName3">The name of the target table (for T3).</param>
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
        string tableName1,
        QueryField where1,
        string tableName2,
        QueryField where2,
        string tableName3,
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
            tableName1: tableName1,
            where1: ToQueryGroup(where1),
            tableName2: tableName2,
            where2: ToQueryGroup(where2),
            tableName3: tableName3,
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="tableName3">The name of the target table (for T3).</param>
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
        string tableName1,
        IEnumerable<QueryField> where1,
        string tableName2,
        IEnumerable<QueryField> where2,
        string tableName3,
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
            tableName1: tableName1,
            where1: ToQueryGroup(where1),
            tableName2: tableName2,
            where2: ToQueryGroup(where2),
            tableName3: tableName3,
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="tableName3">The name of the target table (for T3).</param>
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
        string tableName1,
        QueryGroup? where1,
        string tableName2,
        QueryGroup? where2,
        string tableName3,
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
            tableName1: tableName1,
            where1: where1,
            tableName2: tableName2,
            where2: where2,
            tableName3: tableName3,
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="tableName3">The name of the target table (for T3).</param>
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
        string tableName1,
        QueryGroup? where1,
        string tableName2,
        QueryGroup? where2,
        string tableName3,
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
        // Variables
        var commandType = CommandType.Text;
        var queryGroups = new List<QueryGroup?>();
        var maps = new List<QueryGroupTypeMap>();
        var commandTexts = new List<string>();

        // Items
        var item1 = QueryMultipleInternal<T1>(cacheKey1,
            cache,
            where1,
            queryGroups);

        var item2 = QueryMultipleInternal<T2>(cacheKey2,
            cache,
            where2,
            queryGroups);

        var item3 = QueryMultipleInternal<T3>(cacheKey3,
            cache,
            where3,
            queryGroups);

        // Fix
        QueryGroup.FixForQueryMultiple(queryGroups);

        // Item1 Request
        if (item1 is null)
        {
            var request1 = new QueryMultipleRequest(1,
                tableName1,
                connection,
                transaction,
                fields1 ?? FieldCache.Get<T1>(),
                where1,
                orderBy1,
                top1,
                hints1,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request1, CommandTextCache.GetQueryMultipleText));
            if (where1 is not null)
                maps.Add(where1.MapTo<T1>(tableName1));
        }

        // Item2 Request
        if (item2 is null)
        {
            var request2 = new QueryMultipleRequest(2,
                tableName2,
                connection,
                transaction,
                fields2 ?? FieldCache.Get<T2>(),
                where2,
                orderBy2,
                top2,
                hints2,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request2, CommandTextCache.GetQueryMultipleText));
            if (where2 is not null)
                maps.Add(where2.MapTo<T2>(tableName2));
        }

        // Item3 Request
        if (item3 is null)
        {
            var request3 = new QueryMultipleRequest(3,
                tableName3,
                connection,
                transaction,
                fields3 ?? FieldCache.Get<T3>(),
                where3,
                orderBy3,
                top3,
                hints3,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request3, CommandTextCache.GetQueryMultipleText));
            if (where3 is not null)
                maps.Add(where3.MapTo<T3>(tableName3));
        }


        var (commandText, param) = CombineQueryMultiple(connection, transaction, commandTexts, maps, statementBuilder);
        Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> result;
        TraceResult? traceResult = null;

        // Before Execution
        var beforeExecutionCallback = CreateCallback(command =>
            traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command));

        // Actual Execution
        using (var reader = ExecuteReaderInternal(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            commandTimeout: commandTimeout,
            traceKey: null,
            transaction: transaction,
            trace: null,
            entityType: null,
            dbFields: null,
            skipCommandArrayParametersCheck: true,
            beforeExecutionCallback: beforeExecutionCallback))
        {
            // Silent cancellation
            if (traceResult?.CancellableTraceLog?.IsCancelled == true)
            {
                return new([], [], []);
            }

            // DB setting
            var dbSetting = connection.GetDbSetting();

            // T1
            item1 = QueryMultipleInternal(connection,
                reader,
                item1,
                tableName1,
                cacheKey1,
                cacheItemExpiration,
                transaction,
                cache,
                false);

            // T2
            item2 = QueryMultipleInternal(connection,
                reader,
                item2,
                tableName2,
                cacheKey2,
                cacheItemExpiration,
                transaction,
                cache,
                true);

            // T3
            item3 = QueryMultipleInternal(connection,
                reader,
                item3,
                tableName3,
                cacheKey3,
                cacheItemExpiration,
                transaction,
                cache,
                true);


            // Result
            result = Tuple.Create(item1, item2, item3);

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);
        }

        // Result
        return result;
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="what1">The dynamic query expression or the key value to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
    /// <param name="what2">The dynamic query expression or the key value to be used (for T2).</param>
    /// <param name="tableName3">The name of the target table (for T3).</param>
    /// <param name="what3">The dynamic expression or the key value to be used (for T3).</param>
    /// <param name="tableName4">The name of the target table (for T4).</param>
    /// <param name="what4">The dynamic expression or the key value to be used (for T4).</param>
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
        string tableName1,
        object what1,
        string tableName2,
        object what2,
        string tableName3,
        object what3,
        string tableName4,
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
            tableName1: tableName1,
            where1: WhatToQueryGroup(typeof(T1), connection, what1, transaction),
            tableName2: tableName2,
            where2: WhatToQueryGroup(typeof(T2), connection, what2, transaction),
            tableName3: tableName3,
            where3: WhatToQueryGroup(typeof(T3), connection, what3, transaction),
            tableName4: tableName4,
            where4: WhatToQueryGroup(typeof(T4), connection, what4, transaction),
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="tableName3">The name of the target table (for T3).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="tableName4">The name of the target table (for T4).</param>
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
        string tableName1,
        Expression<Func<T1, bool>> where1,
        string tableName2,
        Expression<Func<T2, bool>> where2,
        string tableName3,
        Expression<Func<T3, bool>> where3,
        string tableName4,
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
            tableName1: tableName1,
            where1: connection.ToQueryGroup(where1, transaction),
            tableName2: tableName2,
            where2: connection.ToQueryGroup(where2, transaction),
            tableName3: tableName3,
            where3: connection.ToQueryGroup(where3, transaction),
            tableName4: tableName4,
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="tableName3">The name of the target table (for T3).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="tableName4">The name of the target table (for T4).</param>
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
        string tableName1,
        QueryField where1,
        string tableName2,
        QueryField where2,
        string tableName3,
        QueryField where3,
        string tableName4,
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
            tableName1: tableName1,
            where1: ToQueryGroup(where1),
            tableName2: tableName2,
            where2: ToQueryGroup(where2),
            tableName3: tableName3,
            where3: ToQueryGroup(where3),
            tableName4: tableName4,
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="tableName3">The name of the target table (for T3).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="tableName4">The name of the target table (for T4).</param>
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
        string tableName1,
        IEnumerable<QueryField> where1,
        string tableName2,
        IEnumerable<QueryField> where2,
        string tableName3,
        IEnumerable<QueryField> where3,
        string tableName4,
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
            tableName1: tableName1,
            where1: ToQueryGroup(where1),
            tableName2: tableName2,
            where2: ToQueryGroup(where2),
            tableName3: tableName3,
            where3: ToQueryGroup(where3),
            tableName4: tableName4,
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="tableName3">The name of the target table (for T3).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="tableName4">The name of the target table (for T4).</param>
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
        string tableName1,
        QueryGroup? where1,
        string tableName2,
        QueryGroup? where2,
        string tableName3,
        QueryGroup? where3,
        string tableName4,
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
            tableName1: tableName1,
            where1: where1,
            tableName2: tableName2,
            where2: where2,
            tableName3: tableName3,
            where3: where3,
            tableName4: tableName4,
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="tableName3">The name of the target table (for T3).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="tableName4">The name of the target table (for T4).</param>
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
        string tableName1,
        QueryGroup? where1,
        string tableName2,
        QueryGroup? where2,
        string tableName3,
        QueryGroup? where3,
        string tableName4,
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
        // Variables
        var commandType = CommandType.Text;
        var queryGroups = new List<QueryGroup?>();
        var maps = new List<QueryGroupTypeMap>();
        var commandTexts = new List<string>();

        // Items
        var item1 = QueryMultipleInternal<T1>(cacheKey1,
            cache,
            where1,
            queryGroups);

        var item2 = QueryMultipleInternal<T2>(cacheKey2,
            cache,
            where2,
            queryGroups);

        var item3 = QueryMultipleInternal<T3>(cacheKey3,
            cache,
            where3,
            queryGroups);

        var item4 = QueryMultipleInternal<T4>(cacheKey4,
            cache,
            where4,
            queryGroups);

        // Fix
        QueryGroup.FixForQueryMultiple(queryGroups);

        // Item1 Request
        if (item1 is null)
        {
            var request1 = new QueryMultipleRequest(1,
                tableName1,
                connection,
                transaction,
                fields1 ?? FieldCache.Get<T1>(),
                where1,
                orderBy1,
                top1,
                hints1,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request1, CommandTextCache.GetQueryMultipleText));
            if (where1 is not null)
                maps.Add(where1.MapTo<T1>(tableName1));
        }

        // Item2 Request
        if (item2 is null)
        {
            var request2 = new QueryMultipleRequest(2,
                tableName2,
                connection,
                transaction,
                fields2 ?? FieldCache.Get<T2>(),
                where2,
                orderBy2,
                top2,
                hints2,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request2, CommandTextCache.GetQueryMultipleText));
            if (where2 is not null)
                maps.Add(where2.MapTo<T2>(tableName2));
        }

        // Item3 Request
        if (item3 is null)
        {
            var request3 = new QueryMultipleRequest(3,
                tableName3,
                connection,
                transaction,
                fields3 ?? FieldCache.Get<T3>(),
                where3,
                orderBy3,
                top3,
                hints3,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request3, CommandTextCache.GetQueryMultipleText));
            if (where3 is not null)
                maps.Add(where3.MapTo<T3>(tableName3));
        }

        // Item4 Request
        if (item4 is null)
        {
            var request4 = new QueryMultipleRequest(4,
                tableName4,
                connection,
                transaction,
                fields4 ?? FieldCache.Get<T4>(),
                where4,
                orderBy4,
                top4,
                hints4,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request4, CommandTextCache.GetQueryMultipleText));
            if (where4 is not null)
                maps.Add(where4.MapTo<T4>(tableName4));
        }


        var (commandText, param) = CombineQueryMultiple(connection, transaction, commandTexts, maps, statementBuilder);
        Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>> result;
        TraceResult? traceResult = null;

        // Before Execution
        var beforeExecutionCallback = CreateCallback(command =>
            traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command));

        // Actual Execution
        using (var reader = ExecuteReaderInternal(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            commandTimeout: commandTimeout,
            traceKey: null,
            transaction: transaction,
            trace: null,
            entityType: null,
            dbFields: null,
            skipCommandArrayParametersCheck: true,
            beforeExecutionCallback: beforeExecutionCallback))
        {
            // Silent cancellation
            if (traceResult?.CancellableTraceLog?.IsCancelled == true)
            {
                return new([], [], [], []);
            }

            // DB setting
            var dbSetting = connection.GetDbSetting();

            // T1
            item1 = QueryMultipleInternal(connection,
                reader,
                item1,
                tableName1,
                cacheKey1,
                cacheItemExpiration,
                transaction,
                cache,
                false);

            // T2
            item2 = QueryMultipleInternal(connection,
                reader,
                item2,
                tableName2,
                cacheKey2,
                cacheItemExpiration,
                transaction,
                cache,
                true);

            // T3
            item3 = QueryMultipleInternal(connection,
                reader,
                item3,
                tableName3,
                cacheKey3,
                cacheItemExpiration,
                transaction,
                cache,
                true);


            // T4
            item4 = QueryMultipleInternal(connection,
                reader,
                item4,
                tableName4,
                cacheKey4,
                cacheItemExpiration,
                transaction,
                cache,
                true);

            // Result
            result = Tuple.Create(item1, item2, item3, item4);

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);
        }

        // Result
        return result;
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="what1">The dynamic query expression or the key value to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
    /// <param name="what2">The dynamic query expression or the key value to be used (for T2).</param>
    /// <param name="tableName3">The name of the target table (for T3).</param>
    /// <param name="what3">The dynamic expression or the key value to be used (for T3).</param>
    /// <param name="tableName4">The name of the target table (for T4).</param>
    /// <param name="what4">The dynamic expression or the key value to be used (for T4).</param>
    /// <param name="tableName5">The name of the target table (for T5).</param>
    /// <param name="what5">The dynamic expression or the key value to be used (for T5).</param>
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
        string tableName1,
        object what1,
        string tableName2,
        object what2,
        string tableName3,
        object what3,
        string tableName4,
        object what4,
        string tableName5,
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
            tableName1: tableName1,
            where1: WhatToQueryGroup(typeof(T1), connection, what1, transaction),
            tableName2: tableName2,
            where2: WhatToQueryGroup(typeof(T2), connection, what2, transaction),
            tableName3: tableName3,
            where3: WhatToQueryGroup(typeof(T3), connection, what3, transaction),
            tableName4: tableName4,
            where4: WhatToQueryGroup(typeof(T4), connection, what4, transaction),
            tableName5: tableName5,
            where5: WhatToQueryGroup(typeof(T5), connection, what5, transaction),
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="tableName3">The name of the target table (for T3).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="tableName4">The name of the target table (for T4).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="tableName5">The name of the target table (for T5).</param>
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
        string tableName1,
        Expression<Func<T1, bool>> where1,
        string tableName2,
        Expression<Func<T2, bool>> where2,
        string tableName3,
        Expression<Func<T3, bool>> where3,
        string tableName4,
        Expression<Func<T4, bool>> where4,
        string tableName5,
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
            tableName1: tableName1,
            where1: connection.ToQueryGroup(where1, transaction),
            tableName2: tableName2,
            where2: connection.ToQueryGroup(where2, transaction),
            tableName3: tableName3,
            where3: connection.ToQueryGroup(where3, transaction),
            tableName4: tableName4,
            where4: connection.ToQueryGroup(where4, transaction),
            tableName5: tableName5,
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="tableName3">The name of the target table (for T3).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="tableName4">The name of the target table (for T4).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="tableName5">The name of the target table (for T5).</param>
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
        string tableName1,
        QueryField where1,
        string tableName2,
        QueryField where2,
        string tableName3,
        QueryField where3,
        string tableName4,
        QueryField where4,
        string tableName5,
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
            tableName1: tableName1,
            where1: ToQueryGroup(where1),
            tableName2: tableName2,
            where2: ToQueryGroup(where2),
            tableName3: tableName3,
            where3: ToQueryGroup(where3),
            tableName4: tableName4,
            where4: ToQueryGroup(where4),
            tableName5: tableName5,
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="tableName3">The name of the target table (for T3).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="tableName4">The name of the target table (for T4).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="tableName5">The name of the target table (for T5).</param>
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
        string tableName1,
        IEnumerable<QueryField> where1,
        string tableName2,
        IEnumerable<QueryField> where2,
        string tableName3,
        IEnumerable<QueryField> where3,
        string tableName4,
        IEnumerable<QueryField> where4,
        string tableName5,
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
            tableName1: tableName1,
            where1: ToQueryGroup(where1),
            tableName2: tableName2,
            where2: ToQueryGroup(where2),
            tableName3: tableName3,
            where3: ToQueryGroup(where3),
            tableName4: tableName4,
            where4: ToQueryGroup(where4),
            tableName5: tableName5,
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="tableName3">The name of the target table (for T3).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="tableName4">The name of the target table (for T4).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="tableName5">The name of the target table (for T5).</param>
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
        string tableName1,
        QueryGroup? where1,
        string tableName2,
        QueryGroup? where2,
        string tableName3,
        QueryGroup? where3,
        string tableName4,
        QueryGroup? where4,
        string tableName5,
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
            tableName1: tableName1,
            where1: where1,
            tableName2: tableName2,
            where2: where2,
            tableName3: tableName3,
            where3: where3,
            tableName4: tableName4,
            where4: where4,
            tableName5: tableName5,
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="tableName3">The name of the target table (for T3).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="tableName4">The name of the target table (for T4).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="tableName5">The name of the target table (for T5).</param>
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
        string tableName1,
        QueryGroup? where1,
        string tableName2,
        QueryGroup? where2,
        string tableName3,
        QueryGroup? where3,
        string tableName4,
        QueryGroup? where4,
        string tableName5,
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
        // Variables
        var commandType = CommandType.Text;
        var queryGroups = new List<QueryGroup?>();
        var maps = new List<QueryGroupTypeMap>();
        var commandTexts = new List<string>();

        // Items
        var item1 = QueryMultipleInternal<T1>(cacheKey1,
            cache,
            where1,
            queryGroups);

        var item2 = QueryMultipleInternal<T2>(cacheKey2,
            cache,
            where2,
            queryGroups);

        var item3 = QueryMultipleInternal<T3>(cacheKey3,
            cache,
            where3,
            queryGroups);

        var item4 = QueryMultipleInternal<T4>(cacheKey4,
            cache,
            where4,
            queryGroups);

        var item5 = QueryMultipleInternal<T5>(cacheKey5,
            cache,
            where5,
            queryGroups);

        // Fix
        QueryGroup.FixForQueryMultiple(queryGroups);

        // Item1 Request
        if (item1 is null)
        {
            var request1 = new QueryMultipleRequest(1,
                tableName1,
                connection,
                transaction,
                fields1 ?? FieldCache.Get<T1>(),
                where1,
                orderBy1,
                top1,
                hints1,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request1, CommandTextCache.GetQueryMultipleText));
            if (where1 is not null)
                maps.Add(where1.MapTo<T1>(tableName1));
        }

        // Item2 Request
        if (item2 is null)
        {
            var request2 = new QueryMultipleRequest(2,
                tableName2,
                connection,
                transaction,
                fields2 ?? FieldCache.Get<T2>(),
                where2,
                orderBy2,
                top2,
                hints2,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request2, CommandTextCache.GetQueryMultipleText));
            if (where2 is not null)
                maps.Add(where2.MapTo<T2>(tableName2));
        }

        // Item3 Request
        if (item3 is null)
        {
            var request3 = new QueryMultipleRequest(3,
                tableName3,
                connection,
                transaction,
                fields3 ?? FieldCache.Get<T3>(),
                where3,
                orderBy3,
                top3,
                hints3,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request3, CommandTextCache.GetQueryMultipleText));
            if (where3 is not null)
                maps.Add(where3.MapTo<T3>(tableName3));
        }

        // Item4 Request
        if (item4 is null)
        {
            var request4 = new QueryMultipleRequest(4,
                tableName4,
                connection,
                transaction,
                fields4 ?? FieldCache.Get<T4>(),
                where4,
                orderBy4,
                top4,
                hints4,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request4, CommandTextCache.GetQueryMultipleText));
            if (where4 is not null)
                maps.Add(where4.MapTo<T4>(tableName4));
        }

        // Item5 Request
        if (item5 is null)
        {
            var request5 = new QueryMultipleRequest(5,
                tableName5,
                connection,
                transaction,
                fields5 ?? FieldCache.Get<T5>(),
                where5,
                orderBy5,
                top5,
                hints5,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request5, CommandTextCache.GetQueryMultipleText));
            if (where5 is not null)
                maps.Add(where5.MapTo<T5>(tableName5));
        }


        var (commandText, param) = CombineQueryMultiple(connection, transaction, commandTexts, maps, statementBuilder);
        Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>> result;
        TraceResult? traceResult = null;

        // Before Execution
        var beforeExecutionCallback = CreateCallback(command =>
            traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command));

        // Actual Execution
        using (var reader = ExecuteReaderInternal(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            commandTimeout: commandTimeout,
            traceKey: null,
            transaction: transaction,
            trace: null,
            entityType: null,
            dbFields: null,
            skipCommandArrayParametersCheck: true,
            beforeExecutionCallback: beforeExecutionCallback))
        {
            // Silent cancellation
            if (traceResult?.CancellableTraceLog?.IsCancelled == true)
            {
                return new([], [], [], [], []);
            }

            // DB setting
            var dbSetting = connection.GetDbSetting();

            // T1
            item1 = QueryMultipleInternal(connection,
                reader,
                item1,
                tableName1,
                cacheKey1,
                cacheItemExpiration,
                transaction,
                cache,
                false);

            // T2
            item2 = QueryMultipleInternal(connection,
                reader,
                item2,
                tableName2,
                cacheKey2,
                cacheItemExpiration,
                transaction,
                cache,
                true);

            // T3
            item3 = QueryMultipleInternal(connection,
                reader,
                item3,
                tableName3,
                cacheKey3,
                cacheItemExpiration,
                transaction,
                cache,
                true);


            // T4
            item4 = QueryMultipleInternal(connection,
                reader,
                item4,
                tableName4,
                cacheKey4,
                cacheItemExpiration,
                transaction,
                cache,
                true);

            // T5
            item5 = QueryMultipleInternal(connection,
                reader,
                item5,
                tableName5,
                cacheKey5,
                cacheItemExpiration,
                transaction,
                cache,
                true);

            // Result
            result = Tuple.Create(item1, item2, item3, item4, item5);

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);
        }

        // Result
        return result;
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="what1">The dynamic query expression or the key value to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
    /// <param name="what2">The dynamic query expression or the key value to be used (for T2).</param>
    /// <param name="tableName3">The name of the target table (for T3).</param>
    /// <param name="what3">The dynamic expression or the key value to be used (for T3).</param>
    /// <param name="tableName4">The name of the target table (for T4).</param>
    /// <param name="what4">The dynamic expression or the key value to be used (for T4).</param>
    /// <param name="tableName5">The name of the target table (for T5).</param>
    /// <param name="what5">The dynamic expression or the key value to be used (for T5).</param>
    /// <param name="tableName6">The name of the target table (for T6).</param>
    /// <param name="what6">The dynamic expression or the key value to be used (for T6).</param>
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
        string tableName1,
        object what1,
        string tableName2,
        object what2,
        string tableName3,
        object what3,
        string tableName4,
        object what4,
        string tableName5,
        object what5,
        string tableName6,
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
            tableName1: tableName1,
            where1: WhatToQueryGroup(typeof(T1), connection, what1, transaction),
            tableName2: tableName2,
            where2: WhatToQueryGroup(typeof(T2), connection, what2, transaction),
            tableName3: tableName3,
            where3: WhatToQueryGroup(typeof(T3), connection, what3, transaction),
            tableName4: tableName4,
            where4: WhatToQueryGroup(typeof(T4), connection, what4, transaction),
            tableName5: tableName5,
            where5: WhatToQueryGroup(typeof(T5), connection, what5, transaction),
            tableName6: tableName6,
            where6: WhatToQueryGroup(typeof(T6), connection, what6, transaction),
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="tableName3">The name of the target table (for T3).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="tableName4">The name of the target table (for T4).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="tableName5">The name of the target table (for T5).</param>
    /// <param name="where5">The query expression to be used (for T5).</param>
    /// <param name="tableName6">The name of the target table (for T6).</param>
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
        string tableName1,
        Expression<Func<T1, bool>> where1,
        string tableName2,
        Expression<Func<T2, bool>> where2,
        string tableName3,
        Expression<Func<T3, bool>> where3,
        string tableName4,
        Expression<Func<T4, bool>> where4,
        string tableName5,
        Expression<Func<T5, bool>> where5,
        string tableName6,
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
            tableName1: tableName1,
            where1: connection.ToQueryGroup(where1, transaction),
            tableName2: tableName2,
            where2: connection.ToQueryGroup(where2, transaction),
            tableName3: tableName3,
            where3: connection.ToQueryGroup(where3, transaction),
            tableName4: tableName4,
            where4: connection.ToQueryGroup(where4, transaction),
            tableName5: tableName5,
            where5: connection.ToQueryGroup(where5, transaction),
            tableName6: tableName6,
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="tableName3">The name of the target table (for T3).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="tableName4">The name of the target table (for T4).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="tableName5">The name of the target table (for T5).</param>
    /// <param name="where5">The query expression to be used (for T5).</param>
    /// <param name="tableName6">The name of the target table (for T6).</param>
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
        string tableName1,
        QueryField where1,
        string tableName2,
        QueryField where2,
        string tableName3,
        QueryField where3,
        string tableName4,
        QueryField where4,
        string tableName5,
        QueryField where5,
        string tableName6,
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
            tableName1: tableName1,
            where1: ToQueryGroup(where1),
            tableName2: tableName2,
            where2: ToQueryGroup(where2),
            tableName3: tableName3,
            where3: ToQueryGroup(where3),
            tableName4: tableName4,
            where4: ToQueryGroup(where4),
            tableName5: tableName5,
            where5: ToQueryGroup(where5),
            tableName6: tableName6,
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="tableName3">The name of the target table (for T3).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="tableName4">The name of the target table (for T4).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="tableName5">The name of the target table (for T5).</param>
    /// <param name="where5">The query expression to be used (for T5).</param>
    /// <param name="tableName6">The name of the target table (for T6).</param>
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
        string tableName1,
        IEnumerable<QueryField> where1,
        string tableName2,
        IEnumerable<QueryField> where2,
        string tableName3,
        IEnumerable<QueryField> where3,
        string tableName4,
        IEnumerable<QueryField> where4,
        string tableName5,
        IEnumerable<QueryField> where5,
        string tableName6,
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
            tableName1: tableName1,
            where1: ToQueryGroup(where1),
            tableName2: tableName2,
            where2: ToQueryGroup(where2),
            tableName3: tableName3,
            where3: ToQueryGroup(where3),
            tableName4: tableName4,
            where4: ToQueryGroup(where4),
            tableName5: tableName5,
            where5: ToQueryGroup(where5),
            tableName6: tableName6,
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="tableName3">The name of the target table (for T3).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="tableName4">The name of the target table (for T4).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="tableName5">The name of the target table (for T5).</param>
    /// <param name="where5">The query expression to be used (for T5).</param>
    /// <param name="tableName6">The name of the target table (for T6).</param>
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
        string tableName1,
        QueryGroup? where1,
        string tableName2,
        QueryGroup? where2,
        string tableName3,
        QueryGroup? where3,
        string tableName4,
        QueryGroup? where4,
        string tableName5,
        QueryGroup? where5,
        string tableName6,
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
            tableName1: tableName1,
            where1: where1,
            tableName2: tableName2,
            where2: where2,
            tableName3: tableName3,
            where3: where3,
            tableName4: tableName4,
            where4: where4,
            tableName5: tableName5,
            where5: where5,
            tableName6: tableName6,
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="tableName3">The name of the target table (for T3).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="tableName4">The name of the target table (for T4).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="tableName5">The name of the target table (for T5).</param>
    /// <param name="where5">The query expression to be used (for T5).</param>
    /// <param name="tableName6">The name of the target table (for T6).</param>
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
        string tableName1,
        QueryGroup? where1,
        string tableName2,
        QueryGroup? where2,
        string tableName3,
        QueryGroup? where3,
        string tableName4,
        QueryGroup? where4,
        string tableName5,
        QueryGroup? where5,
        string tableName6,
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
        // Variables
        var commandType = CommandType.Text;
        var queryGroups = new List<QueryGroup?>();
        var maps = new List<QueryGroupTypeMap>();
        var commandTexts = new List<string>();

        // Items
        var item1 = QueryMultipleInternal<T1>(cacheKey1,
            cache,
            where1,
            queryGroups);

        var item2 = QueryMultipleInternal<T2>(cacheKey2,
            cache,
            where2,
            queryGroups);

        var item3 = QueryMultipleInternal<T3>(cacheKey3,
            cache,
            where3,
            queryGroups);

        var item4 = QueryMultipleInternal<T4>(cacheKey4,
            cache,
            where4,
            queryGroups);

        var item5 = QueryMultipleInternal<T5>(cacheKey5,
            cache,
            where5,
            queryGroups);

        var item6 = QueryMultipleInternal<T6>(cacheKey6,
            cache,
            where6,
            queryGroups);

        // Fix
        QueryGroup.FixForQueryMultiple(queryGroups);

        // Item1 Request
        if (item1 is null)
        {
            var request1 = new QueryMultipleRequest(1,
                tableName1,
                connection,
                transaction,
                fields1 ?? FieldCache.Get<T1>(),
                where1,
                orderBy1,
                top1,
                hints1,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request1, CommandTextCache.GetQueryMultipleText));
            if (where1 is not null)
                maps.Add(where1.MapTo<T1>(tableName1));
        }

        // Item2 Request
        if (item2 is null)
        {
            var request2 = new QueryMultipleRequest(2,
                tableName2,
                connection,
                transaction,
                fields2 ?? FieldCache.Get<T2>(),
                where2,
                orderBy2,
                top2,
                hints2,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request2, CommandTextCache.GetQueryMultipleText));
            if (where2 is not null)
                maps.Add(where2.MapTo<T2>(tableName2));
        }

        // Item3 Request
        if (item3 is null)
        {
            var request3 = new QueryMultipleRequest(3,
                tableName3,
                connection,
                transaction,
                fields3 ?? FieldCache.Get<T3>(),
                where3,
                orderBy3,
                top3,
                hints3,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request3, CommandTextCache.GetQueryMultipleText));
            if (where3 is not null)
                maps.Add(where3.MapTo<T3>(tableName3));
        }

        // Item4 Request
        if (item4 is null)
        {
            var request4 = new QueryMultipleRequest(4,
                tableName4,
                connection,
                transaction,
                fields4 ?? FieldCache.Get<T4>(),
                where4,
                orderBy4,
                top4,
                hints4,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request4, CommandTextCache.GetQueryMultipleText));
            if (where4 is not null)
                maps.Add(where4.MapTo<T4>(tableName4));
        }

        // Item5 Request
        if (item5 is null)
        {
            var request5 = new QueryMultipleRequest(5,
                tableName5,
                connection,
                transaction,
                fields5 ?? FieldCache.Get<T5>(),
                where5,
                orderBy5,
                top5,
                hints5,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request5, CommandTextCache.GetQueryMultipleText));
            if (where5 is not null)
                maps.Add(where5.MapTo<T5>(tableName5));
        }

        // Item6 Request
        if (item6 is null)
        {
            var request6 = new QueryMultipleRequest(6,
                tableName6,
                connection,
                transaction,
                fields6 ?? FieldCache.Get<T6>(),
                where6,
                orderBy6,
                top6,
                hints6,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request6, CommandTextCache.GetQueryMultipleText));
            if (where6 is not null)
                maps.Add(where6.MapTo<T6>(tableName6));
        }


        var (commandText, param) = CombineQueryMultiple(connection, transaction, commandTexts, maps, statementBuilder);
        Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>> result;
        TraceResult? traceResult = null;

        // Before Execution
        var beforeExecutionCallback = CreateCallback(command =>
            traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command));

        // Actual Execution
        using (var reader = ExecuteReaderInternal(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            commandTimeout: commandTimeout,
            traceKey: null,
            transaction: transaction,
            trace: null,
            entityType: null,
            dbFields: null,
            skipCommandArrayParametersCheck: true,
            beforeExecutionCallback: beforeExecutionCallback))
        {
            // Silent cancellation
            if (traceResult?.CancellableTraceLog?.IsCancelled == true)
            {
                return new([], [], [], [], [], []);
            }

            // DB setting
            var dbSetting = connection.GetDbSetting();

            // T1
            item1 = QueryMultipleInternal(connection,
                reader,
                item1,
                tableName1,
                cacheKey1,
                cacheItemExpiration,
                transaction,
                cache,
                false);

            // T2
            item2 = QueryMultipleInternal(connection,
                reader,
                item2,
                tableName2,
                cacheKey2,
                cacheItemExpiration,
                transaction,
                cache,
                true);

            // T3
            item3 = QueryMultipleInternal(connection,
                reader,
                item3,
                tableName3,
                cacheKey3,
                cacheItemExpiration,
                transaction,
                cache,
                true);


            // T4
            item4 = QueryMultipleInternal(connection,
                reader,
                item4,
                tableName4,
                cacheKey4,
                cacheItemExpiration,
                transaction,
                cache,
                true);

            // T5
            item5 = QueryMultipleInternal(connection,
                reader,
                item5,
                tableName5,
                cacheKey5,
                cacheItemExpiration,
                transaction,
                cache,
                true);

            // T6
            item6 = QueryMultipleInternal(connection,
                reader,
                item6,
                tableName6,
                cacheKey6,
                cacheItemExpiration,
                transaction,
                cache,
                true);

            // Result
            result = Tuple.Create(item1, item2, item3, item4, item5, item6);

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);
        }

        // Result
        return result;
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="what1">The dynamic query expression or the key value to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
    /// <param name="what2">The dynamic query expression or the key value to be used (for T2).</param>
    /// <param name="tableName3">The name of the target table (for T3).</param>
    /// <param name="what3">The dynamic expression or the key value to be used (for T3).</param>
    /// <param name="tableName4">The name of the target table (for T4).</param>
    /// <param name="what4">The dynamic expression or the key value to be used (for T4).</param>
    /// <param name="tableName5">The name of the target table (for T5).</param>
    /// <param name="what5">The dynamic expression or the key value to be used (for T5).</param>
    /// <param name="tableName6">The name of the target table (for T6).</param>
    /// <param name="what6">The dynamic expression or the key value to be used (for T6).</param>
    /// <param name="tableName7">The name of the target table (for T7).</param>
    /// <param name="what7">The dynamic expression or the key value to be used (for T7).</param>
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
        string tableName1,
        object what1,
        string tableName2,
        object what2,
        string tableName3,
        object what3,
        string tableName4,
        object what4,
        string tableName5,
        object what5,
        string tableName6,
        object what6,
        string tableName7,
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
            tableName1: tableName1,
            where1: WhatToQueryGroup(typeof(T1), connection, what1, transaction),
            tableName2: tableName2,
            where2: WhatToQueryGroup(typeof(T2), connection, what2, transaction),
            tableName3: tableName3,
            where3: WhatToQueryGroup(typeof(T3), connection, what3, transaction),
            tableName4: tableName4,
            where4: WhatToQueryGroup(typeof(T4), connection, what4, transaction),
            tableName5: tableName5,
            where5: WhatToQueryGroup(typeof(T5), connection, what5, transaction),
            tableName6: tableName6,
            where6: WhatToQueryGroup(typeof(T6), connection, what6, transaction),
            tableName7: tableName7,
            where7: WhatToQueryGroup(typeof(T7), connection, what7, transaction),
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="tableName3">The name of the target table (for T3).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="tableName4">The name of the target table (for T4).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="tableName5">The name of the target table (for T5).</param>
    /// <param name="where5">The query expression to be used (for T5).</param>
    /// <param name="tableName6">The name of the target table (for T6).</param>
    /// <param name="where6">The query expression to be used (for T6).</param>
    /// <param name="tableName7">The name of the target table (for T7).</param>
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
        string tableName1,
        Expression<Func<T1, bool>> where1,
        string tableName2,
        Expression<Func<T2, bool>> where2,
        string tableName3,
        Expression<Func<T3, bool>> where3,
        string tableName4,
        Expression<Func<T4, bool>> where4,
        string tableName5,
        Expression<Func<T5, bool>> where5,
        string tableName6,
        Expression<Func<T6, bool>> where6,
        string tableName7,
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
            tableName1: tableName1,
            where1: connection.ToQueryGroup(where1, transaction),
            tableName2: tableName2,
            where2: connection.ToQueryGroup(where2, transaction),
            tableName3: tableName3,
            where3: connection.ToQueryGroup(where3, transaction),
            tableName4: tableName4,
            where4: connection.ToQueryGroup(where4, transaction),
            tableName5: tableName5,
            where5: connection.ToQueryGroup(where5, transaction),
            tableName6: tableName6,
            where6: connection.ToQueryGroup(where6, transaction),
            tableName7: tableName7,
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="tableName3">The name of the target table (for T3).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="tableName4">The name of the target table (for T4).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="tableName5">The name of the target table (for T5).</param>
    /// <param name="where5">The query expression to be used (for T5).</param>
    /// <param name="tableName6">The name of the target table (for T6).</param>
    /// <param name="where6">The query expression to be used (for T6).</param>
    /// <param name="tableName7">The name of the target table (for T7).</param>
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
        string tableName1,
        QueryField where1,
        string tableName2,
        QueryField where2,
        string tableName3,
        QueryField where3,
        string tableName4,
        QueryField where4,
        string tableName5,
        QueryField where5,
        string tableName6,
        QueryField where6,
        string tableName7,
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
            tableName1: tableName1,
            where1: ToQueryGroup(where1),
            tableName2: tableName2,
            where2: ToQueryGroup(where2),
            tableName3: tableName3,
            where3: ToQueryGroup(where3),
            tableName4: tableName4,
            where4: ToQueryGroup(where4),
            tableName5: tableName5,
            where5: ToQueryGroup(where5),
            tableName6: tableName6,
            where6: ToQueryGroup(where6),
            tableName7: tableName7,
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="tableName3">The name of the target table (for T3).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="tableName4">The name of the target table (for T4).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="tableName5">The name of the target table (for T5).</param>
    /// <param name="where5">The query expression to be used (for T5).</param>
    /// <param name="tableName6">The name of the target table (for T6).</param>
    /// <param name="where6">The query expression to be used (for T6).</param>
    /// <param name="tableName7">The name of the target table (for T7).</param>
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
        string tableName1,
        IEnumerable<QueryField> where1,
        string tableName2,
        IEnumerable<QueryField> where2,
        string tableName3,
        IEnumerable<QueryField> where3,
        string tableName4,
        IEnumerable<QueryField> where4,
        string tableName5,
        IEnumerable<QueryField> where5,
        string tableName6,
        IEnumerable<QueryField> where6,
        string tableName7,
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
            tableName1: tableName1,
            where1: ToQueryGroup(where1),
            tableName2: tableName2,
            where2: ToQueryGroup(where2),
            tableName3: tableName3,
            where3: ToQueryGroup(where3),
            tableName4: tableName4,
            where4: ToQueryGroup(where4),
            tableName5: tableName5,
            where5: ToQueryGroup(where5),
            tableName6: tableName6,
            where6: ToQueryGroup(where6),
            tableName7: tableName7,
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="tableName3">The name of the target table (for T3).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="tableName4">The name of the target table (for T4).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="tableName5">The name of the target table (for T5).</param>
    /// <param name="where5">The query expression to be used (for T5).</param>
    /// <param name="tableName6">The name of the target table (for T6).</param>
    /// <param name="where6">The query expression to be used (for T6).</param>
    /// <param name="tableName7">The name of the target table (for T7).</param>
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
        string tableName1,
        QueryGroup? where1,
        string tableName2,
        QueryGroup? where2,
        string tableName3,
        QueryGroup? where3,
        string tableName4,
        QueryGroup? where4,
        string tableName5,
        QueryGroup? where5,
        string tableName6,
        QueryGroup? where6,
        string tableName7,
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
            tableName1: tableName1,
            where1: where1,
            tableName2: tableName2,
            where2: where2,
            tableName3: tableName3,
            where3: where3,
            tableName4: tableName4,
            where4: where4,
            tableName5: tableName5,
            where5: where5,
            tableName6: tableName6,
            where6: where6,
            tableName7: tableName7,
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
    /// <param name="tableName1">The name of the target table (for T1).</param>
    /// <param name="where1">The query expression to be used (for T1).</param>
    /// <param name="tableName2">The name of the target table (for T2).</param>
    /// <param name="where2">The query expression to be used (for T2).</param>
    /// <param name="tableName3">The name of the target table (for T3).</param>
    /// <param name="where3">The query expression to be used (for T3).</param>
    /// <param name="tableName4">The name of the target table (for T4).</param>
    /// <param name="where4">The query expression to be used (for T4).</param>
    /// <param name="tableName5">The name of the target table (for T5).</param>
    /// <param name="where5">The query expression to be used (for T5).</param>
    /// <param name="tableName6">The name of the target table (for T6).</param>
    /// <param name="where6">The query expression to be used (for T6).</param>
    /// <param name="tableName7">The name of the target table (for T7).</param>
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
        string tableName1,
        QueryGroup? where1,
        string tableName2,
        QueryGroup? where2,
        string tableName3,
        QueryGroup? where3,
        string tableName4,
        QueryGroup? where4,
        string tableName5,
        QueryGroup? where5,
        string tableName6,
        QueryGroup? where6,
        string tableName7,
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
        // Variables
        var commandType = CommandType.Text;
        var queryGroups = new List<QueryGroup?>();
        var maps = new List<QueryGroupTypeMap>();
        var commandTexts = new List<string>();

        // Items
        var item1 = QueryMultipleInternal<T1>(cacheKey1,
            cache,
            where1,
            queryGroups);

        var item2 = QueryMultipleInternal<T2>(cacheKey2,
            cache,
            where2,
            queryGroups);

        var item3 = QueryMultipleInternal<T3>(cacheKey3,
            cache,
            where3,
            queryGroups);

        var item4 = QueryMultipleInternal<T4>(cacheKey4,
            cache,
            where4,
            queryGroups);

        var item5 = QueryMultipleInternal<T5>(cacheKey5,
            cache,
            where5,
            queryGroups);

        var item6 = QueryMultipleInternal<T6>(cacheKey6,
            cache,
            where6,
            queryGroups);

        var item7 = QueryMultipleInternal<T7>(cacheKey7,
            cache,
            where7,
            queryGroups);

        // Fix
        QueryGroup.FixForQueryMultiple(queryGroups);

        // Item1 Request
        if (item1 is null)
        {
            var request1 = new QueryMultipleRequest(1,
                tableName1,
                connection,
                transaction,
                fields1 ?? FieldCache.Get<T1>(),
                where1,
                orderBy1,
                top1,
                hints1,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request1, CommandTextCache.GetQueryMultipleText));
            if (where1 is not null)
                maps.Add(where1.MapTo<T1>(tableName1));
        }

        // Item2 Request
        if (item2 is null)
        {
            var request2 = new QueryMultipleRequest(2,
                tableName2,
                connection,
                transaction,
                fields2 ?? FieldCache.Get<T2>(),
                where2,
                orderBy2,
                top2,
                hints2,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request2, CommandTextCache.GetQueryMultipleText));
            if (where2 is not null)
                maps.Add(where2.MapTo<T2>(tableName2));
        }

        // Item3 Request
        if (item3 is null)
        {
            var request3 = new QueryMultipleRequest(3,
                tableName3,
                connection,
                transaction,
                fields3 ?? FieldCache.Get<T3>(),
                where3,
                orderBy3,
                top3,
                hints3,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request3, CommandTextCache.GetQueryMultipleText));
            if (where3 is not null)
                maps.Add(where3.MapTo<T3>(tableName3));
        }

        // Item4 Request
        if (item4 is null)
        {
            var request4 = new QueryMultipleRequest(4,
                tableName4,
                connection,
                transaction,
                fields4 ?? FieldCache.Get<T4>(),
                where4,
                orderBy4,
                top4,
                hints4,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request4, CommandTextCache.GetQueryMultipleText));
            if (where4 is not null)
                maps.Add(where4.MapTo<T4>(tableName4));
        }

        // Item5 Request
        if (item5 is null)
        {
            var request5 = new QueryMultipleRequest(5,
                tableName5,
                connection,
                transaction,
                fields5 ?? FieldCache.Get<T5>(),
                where5,
                orderBy5,
                top5,
                hints5,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request5, CommandTextCache.GetQueryMultipleText));
            if (where5 is not null)
                maps.Add(where5.MapTo<T5>(tableName5));
        }

        // Item6 Request
        if (item6 is null)
        {
            var request6 = new QueryMultipleRequest(6,
                tableName6,
                connection,
                transaction,
                fields6 ?? FieldCache.Get<T6>(),
                where6,
                orderBy6,
                top6,
                hints6,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request6, CommandTextCache.GetQueryMultipleText));
            if (where6 is not null)
                maps.Add(where6.MapTo<T6>(tableName6));
        }

        // Item7 Request
        if (item7 is null)
        {
            var request7 = new QueryMultipleRequest(7,
                tableName7,
                connection,
                transaction,
                fields7 ?? FieldCache.Get<T7>(),
                where7,
                orderBy7,
                top7,
                hints7,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request7, CommandTextCache.GetQueryMultipleText));
            if (where7 is not null)
                maps.Add(where7.MapTo<T7>(tableName7));
        }


        var (commandText, param) = CombineQueryMultiple(connection, transaction, commandTexts, maps, statementBuilder);
        Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>> result;
        TraceResult? traceResult = null;

        // Before Execution
        var beforeExecutionCallback = CreateCallback(command =>
            traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command));

        // Actual Execution
        using (var reader = ExecuteReaderInternal(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            commandTimeout: commandTimeout,
            traceKey: null,
            transaction: transaction,
            trace: null,
            entityType: null,
            dbFields: null,
            skipCommandArrayParametersCheck: true,
            beforeExecutionCallback: beforeExecutionCallback))
        {
            // Silent cancellation
            if (traceResult?.CancellableTraceLog?.IsCancelled == true)
            {
                return new([], [], [], [], [], [], []);
            }

            // DB setting
            var dbSetting = connection.GetDbSetting();

            // T1
            item1 = QueryMultipleInternal(connection,
                reader,
                item1,
                tableName1,
                cacheKey1,
                cacheItemExpiration,
                transaction,
                cache,
                false);

            // T2
            item2 = QueryMultipleInternal(connection,
                reader,
                item2,
                tableName2,
                cacheKey2,
                cacheItemExpiration,
                transaction,
                cache,
                true);

            // T3
            item3 = QueryMultipleInternal(connection,
                reader,
                item3,
                tableName3,
                cacheKey3,
                cacheItemExpiration,
                transaction,
                cache,
                true);


            // T4
            item4 = QueryMultipleInternal(connection,
                reader,
                item4,
                tableName4,
                cacheKey4,
                cacheItemExpiration,
                transaction,
                cache,
                true);

            // T5
            item5 = QueryMultipleInternal(connection,
                reader,
                item5,
                tableName5,
                cacheKey5,
                cacheItemExpiration,
                transaction,
                cache,
                true);

            // T6
            item6 = QueryMultipleInternal(connection,
                reader,
                item6,
                tableName6,
                cacheKey6,
                cacheItemExpiration,
                transaction,
                cache,
                true);

            // T7
            item7 = QueryMultipleInternal(connection,
                reader,
                item7,
                tableName7,
                cacheKey7,
                cacheItemExpiration,
                transaction,
                cache,
                true);

            // Result
            result = Tuple.Create(item1, item2, item3, item4, item5, item6, item7);

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);
        }

        // Result
        return result;
    }

    #endregion

    #endregion

    #region Dynamic

    #region T1, T2

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 2 target types.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName1">The name of the target table (for dynamic type 1).</param>
    /// <param name="what1">The dynamic expression or the key value to be used (for dynamic type 1).</param>
    /// <param name="tableName2">The name of the target table (for dynamic type 2).</param>
    /// <param name="what2">The dynamic expression or the key value to be used (for dynamic type 2).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for dynamic type 1).</param>
    /// <param name="top1">The number of rows to be returned (for dynamic type 1).</param>
    /// <param name="hints1">The table hints to be used (for dynamic type 1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for dynamic type 2).</param>
    /// <param name="top2">The number of rows to be returned (for dynamic type 2).</param>
    /// <param name="hints2">The table hints to be used (for dynamic type 2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 2 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>> QueryMultiple(this IDbConnection connection,
        string tableName1,
        object what1,
        string tableName2,
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
    {
        return QueryMultipleInternal(connection: connection,
            tableName1: tableName1,
            where1: WhatToQueryGroup(what1),
            tableName2: tableName2,
            where2: WhatToQueryGroup(what2),
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
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName1">The name of the target table (for dynamic type 1).</param>
    /// <param name="where1">The query expression to be used (for dynamic type 1).</param>
    /// <param name="tableName2">The name of the target table (for dynamic type 2).</param>
    /// <param name="where2">The query expression to be used (for dynamic type 2).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for dynamic type 1).</param>
    /// <param name="top1">The number of rows to be returned (for dynamic type 1).</param>
    /// <param name="hints1">The table hints to be used (for dynamic type 1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for dynamic type 2).</param>
    /// <param name="top2">The number of rows to be returned (for dynamic type 2).</param>
    /// <param name="hints2">The table hints to be used (for dynamic type 2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 2 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>> QueryMultiple(this IDbConnection connection,
        string tableName1,
        QueryField where1,
        string tableName2,
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
    {
        return QueryMultipleInternal(connection: connection,
            tableName1: tableName1,
            where1: ToQueryGroup(where1),
            tableName2: tableName2,
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
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName1">The name of the target table (for dynamic type 1).</param>
    /// <param name="where1">The query expression to be used (for dynamic type 1).</param>
    /// <param name="tableName2">The name of the target table (for dynamic type 2).</param>
    /// <param name="where2">The query expression to be used (for dynamic type 2).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for dynamic type 1).</param>
    /// <param name="top1">The number of rows to be returned (for dynamic type 1).</param>
    /// <param name="hints1">The table hints to be used (for dynamic type 1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for dynamic type 2).</param>
    /// <param name="top2">The number of rows to be returned (for dynamic type 2).</param>
    /// <param name="hints2">The table hints to be used (for dynamic type 2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 2 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>> QueryMultiple(this IDbConnection connection,
        string tableName1,
        IEnumerable<QueryField> where1,
        string tableName2,
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
    {
        return QueryMultipleInternal(connection: connection,
            tableName1: tableName1,
            where1: ToQueryGroup(where1),
            tableName2: tableName2,
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
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName1">The name of the target table (for dynamic type 1).</param>
    /// <param name="where1">The query expression to be used (for dynamic type 1).</param>
    /// <param name="tableName2">The name of the target table (for dynamic type 2).</param>
    /// <param name="where2">The query expression to be used (for dynamic type 2).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for dynamic type 1).</param>
    /// <param name="top1">The number of rows to be returned (for dynamic type 1).</param>
    /// <param name="hints1">The table hints to be used (for dynamic type 1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for dynamic type 2).</param>
    /// <param name="top2">The number of rows to be returned (for dynamic type 2).</param>
    /// <param name="hints2">The table hints to be used (for dynamic type 2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 2 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>> QueryMultiple(this IDbConnection connection,
        string tableName1,
        QueryGroup? where1,
        string tableName2,
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
    {
        return QueryMultipleInternal(connection: connection,
            tableName1: tableName1,
            where1: where1,
            tableName2: tableName2,
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
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName1">The name of the target table (for dynamic type 1).</param>
    /// <param name="where1">The query expression to be used (for dynamic type 1).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for dynamic type 1).</param>
    /// <param name="top1">The number of rows to be returned (for dynamic type 1).</param>
    /// <param name="hints1">The table hints to be used (for dynamic type 1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="tableName2">The name of the target table (for dynamic type 2).</param>
    /// <param name="where2">The query expression to be used (for dynamic type 2).</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for dynamic type 2).</param>
    /// <param name="top2">The number of rows to be returned (for dynamic type 2).</param>
    /// <param name="hints2">The table hints to be used (for dynamic type 2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 2 enumerable target data entity types.</returns>
    internal static Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>> QueryMultipleInternal(this IDbConnection connection,
        string tableName1,
        QueryGroup? where1,
        string tableName2,
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
    {
        // Variables
        var commandType = CommandType.Text;
        var queryGroups = new List<QueryGroup?>();
        var maps = new List<QueryGroupTypeMap>();
        var commandTexts = new List<string>();

        // Items
        var item1 = QueryDynamicMultipleInternal(cacheKey1,
            cache,
            where1,
            queryGroups);

        var item2 = QueryDynamicMultipleInternal(cacheKey2,
            cache,
            where2,
            queryGroups);

        // Fix
        QueryGroup.FixForQueryMultiple(queryGroups);

        // Item1 Request
        if (item1 is null)
        {
            var request1 = new QueryMultipleRequest(1,
                tableName1,
                connection,
                transaction,
                fields1 ?? DbFieldCache.Get(connection, tableName1, transaction).AsFields(),
                where1,
                orderBy1,
                top1,
                hints1,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request1, CommandTextCache.GetQueryMultipleText));

            if (where1 is not null)
                maps.Add(where1.MapTo());
        }

        // Item2 Request
        if (item2 is null)
        {
            var request2 = new QueryMultipleRequest(2,
                tableName2,
                connection,
                transaction,
                fields2 ?? DbFieldCache.Get(connection, tableName2, transaction).AsFields(),
                where2,
                orderBy2,
                top2,
                hints2,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request2, CommandTextCache.GetQueryMultipleText));

            if (where2 is not null)
                maps.Add(where2.MapTo());
        }


        var (commandText, param) = CombineQueryMultiple(connection, transaction, commandTexts, maps, statementBuilder);
        Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>> result;
        TraceResult? traceResult = null;

        // Before Execution
        var beforeExecutionCallback = CreateCallback(command =>
            traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command));

        // Actual Execution
        using (var reader = ExecuteReaderInternal(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            commandTimeout: commandTimeout,
            traceKey: null,
            transaction: transaction,
            trace: null,
            entityType: null,
            dbFields: null,
            skipCommandArrayParametersCheck: true,
            beforeExecutionCallback: beforeExecutionCallback))
        {
            // Silent cancellation
            if (traceResult?.CancellableTraceLog?.IsCancelled == true)
            {
                return new([], []);
            }

            // DB setting
            var dbSetting = connection.GetDbSetting();

            // T1
            item1 = QueryDynamicMultipleInternal(connection,
                reader,
                item1,
                tableName1,
                cacheKey1,
                cacheItemExpiration,
                transaction,
                cache,
                false);

            // T2
            item2 = QueryDynamicMultipleInternal(connection,
                reader,
                item2,
                tableName2,
                cacheKey2,
                cacheItemExpiration,
                transaction,
                cache,
                true);

            // Result
            result = Tuple.Create(item1, item2);

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);
        }

        // Result
        return result;
    }

    #endregion

    #region T1, T2, T3

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 3 target types.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName1">The name of the target table (for dynamic type 1).</param>
    /// <param name="what1">The dynamic query expression or the key value to be used (for dynamic type 1).</param>
    /// <param name="tableName2">The name of the target table (for dynamic type 2).</param>
    /// <param name="what2">The dynamic query expression or the key value to be used (for dynamic type 2).</param>
    /// <param name="tableName3">The name of the target table (for dynamic type 3).</param>
    /// <param name="what3">The dynamic expression or the key value to be used (for dynamic type 3).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for dynamic type 1).</param>
    /// <param name="top1">The number of rows to be returned (for dynamic type 1).</param>
    /// <param name="hints1">The table hints to be used (for dynamic type 1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for dynamic type 2).</param>
    /// <param name="top2">The number of rows to be returned (for dynamic type 2).</param>
    /// <param name="hints2">The table hints to be used (for dynamic type 2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for dynamic type 3).</param>
    /// <param name="top3">The number of rows to be returned (for dynamic type 3).</param>
    /// <param name="hints3">The table hints to be used (for dynamic type 3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 3 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>> QueryMultiple(this IDbConnection connection,
        string tableName1,
        object what1,
        string tableName2,
        object what2,
        string tableName3,
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
    {
        return QueryMultipleInternal(connection: connection,
            tableName1: tableName1,
            where1: WhatToQueryGroup(what1),
            tableName2: tableName2,
            where2: WhatToQueryGroup(what2),
            tableName3: tableName3,
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
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName1">The name of the target table (for dynamic type 1).</param>
    /// <param name="where1">The query expression to be used (for dynamic type 1).</param>
    /// <param name="tableName2">The name of the target table (for dynamic type 2).</param>
    /// <param name="where2">The query expression to be used (for dynamic type 2).</param>
    /// <param name="tableName3">The name of the target table (for dynamic type 3).</param>
    /// <param name="where3">The query expression to be used (for dynamic type 3).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for dynamic type 1).</param>
    /// <param name="top1">The number of rows to be returned (for dynamic type 1).</param>
    /// <param name="hints1">The table hints to be used (for dynamic type 1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for dynamic type 2).</param>
    /// <param name="top2">The number of rows to be returned (for dynamic type 2).</param>
    /// <param name="hints2">The table hints to be used (for dynamic type 2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for dynamic type 3).</param>
    /// <param name="top3">The number of rows to be returned (for dynamic type 3).</param>
    /// <param name="hints3">The table hints to be used (for dynamic type 3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 3 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>> QueryMultiple(this IDbConnection connection,
        string tableName1,
        QueryField where1,
        string tableName2,
        QueryField where2,
        string tableName3,
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
    {
        return QueryMultipleInternal(connection: connection,
            tableName1: tableName1,
            where1: ToQueryGroup(where1),
            tableName2: tableName2,
            where2: ToQueryGroup(where2),
            tableName3: tableName3,
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
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName1">The name of the target table (for dynamic type 1).</param>
    /// <param name="where1">The query expression to be used (for dynamic type 1).</param>
    /// <param name="tableName2">The name of the target table (for dynamic type 2).</param>
    /// <param name="where2">The query expression to be used (for dynamic type 2).</param>
    /// <param name="tableName3">The name of the target table (for dynamic type 3).</param>
    /// <param name="where3">The query expression to be used (for dynamic type 3).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for dynamic type 1).</param>
    /// <param name="top1">The number of rows to be returned (for dynamic type 1).</param>
    /// <param name="hints1">The table hints to be used (for dynamic type 1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for dynamic type 2).</param>
    /// <param name="top2">The number of rows to be returned (for dynamic type 2).</param>
    /// <param name="hints2">The table hints to be used (for dynamic type 2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for dynamic type 3).</param>
    /// <param name="top3">The number of rows to be returned (for dynamic type 3).</param>
    /// <param name="hints3">The table hints to be used (for dynamic type 3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 3 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>> QueryMultiple(this IDbConnection connection,
        string tableName1,
        IEnumerable<QueryField> where1,
        string tableName2,
        IEnumerable<QueryField> where2,
        string tableName3,
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
    {
        return QueryMultipleInternal(connection: connection,
            tableName1: tableName1,
            where1: ToQueryGroup(where1),
            tableName2: tableName2,
            where2: ToQueryGroup(where2),
            tableName3: tableName3,
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
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName1">The name of the target table (for dynamic type 1).</param>
    /// <param name="where1">The query expression to be used (for dynamic type 1).</param>
    /// <param name="tableName2">The name of the target table (for dynamic type 2).</param>
    /// <param name="where2">The query expression to be used (for dynamic type 2).</param>
    /// <param name="tableName3">The name of the target table (for dynamic type 3).</param>
    /// <param name="where3">The query expression to be used (for dynamic type 3).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for dynamic type 1).</param>
    /// <param name="top1">The number of rows to be returned (for dynamic type 1).</param>
    /// <param name="hints1">The table hints to be used (for dynamic type 1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for dynamic type 2).</param>
    /// <param name="top2">The number of rows to be returned (for dynamic type 2).</param>
    /// <param name="hints2">The table hints to be used (for dynamic type 2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for dynamic type 3).</param>
    /// <param name="top3">The number of rows to be returned (for dynamic type 3).</param>
    /// <param name="hints3">The table hints to be used (for dynamic type 3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 3 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>> QueryMultiple(this IDbConnection connection,
        string tableName1,
        QueryGroup? where1,
        string tableName2,
        QueryGroup? where2,
        string tableName3,
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
    {
        return QueryMultipleInternal(connection: connection,
            tableName1: tableName1,
            where1: where1,
            tableName2: tableName2,
            where2: where2,
            tableName3: tableName3,
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
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName1">The name of the target table (for dynamic type 1).</param>
    /// <param name="where1">The query expression to be used (for dynamic type 1).</param>
    /// <param name="tableName2">The name of the target table (for dynamic type 2).</param>
    /// <param name="where2">The query expression to be used (for dynamic type 2).</param>
    /// <param name="tableName3">The name of the target table (for dynamic type 3).</param>
    /// <param name="where3">The query expression to be used (for dynamic type 3).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for dynamic type 1).</param>
    /// <param name="top1">The number of rows to be returned (for dynamic type 1).</param>
    /// <param name="hints1">The table hints to be used (for dynamic type 1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for dynamic type 2).</param>
    /// <param name="top2">The number of rows to be returned (for dynamic type 2).</param>
    /// <param name="hints2">The table hints to be used (for dynamic type 2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for dynamic type 3).</param>
    /// <param name="top3">The number of rows to be returned (for dynamic type 3).</param>
    /// <param name="hints3">The table hints to be used (for dynamic type 3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 3 enumerable target data entity types.</returns>
    internal static Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>> QueryMultipleInternal(this IDbConnection connection,
        string tableName1,
        QueryGroup? where1,
        string tableName2,
        QueryGroup? where2,
        string tableName3,
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
    {
        // Variables
        var commandType = CommandType.Text;
        var queryGroups = new List<QueryGroup?>();
        var maps = new List<QueryGroupTypeMap>();
        var commandTexts = new List<string>();

        // Items
        var item1 = QueryDynamicMultipleInternal(cacheKey1,
            cache,
            where1,
            queryGroups);

        var item2 = QueryDynamicMultipleInternal(cacheKey2,
            cache,
            where2,
            queryGroups);

        var item3 = QueryDynamicMultipleInternal(cacheKey3,
            cache,
            where3,
            queryGroups);

        // Fix
        QueryGroup.FixForQueryMultiple(queryGroups);

        // Item1 Request
        if (item1 is null)
        {
            var request1 = new QueryMultipleRequest(1,
                tableName1,
                connection,
                transaction,
                fields1 ?? DbFieldCache.Get(connection, tableName1, transaction).AsFields(),
                where1,
                orderBy1,
                top1,
                hints1,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request1, CommandTextCache.GetQueryMultipleText));

            if (where1 is not null)
                maps.Add(where1.MapTo());
        }

        // Item2 Request
        if (item2 is null)
        {
            var request2 = new QueryMultipleRequest(2,
                tableName2,
                connection,
                transaction,
                fields2 ?? DbFieldCache.Get(connection, tableName2, transaction).AsFields(),
                where2,
                orderBy2,
                top2,
                hints2,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request2, CommandTextCache.GetQueryMultipleText));

            if (where2 is not null)
                maps.Add(where2.MapTo());
        }

        // Item3 Request
        if (item3 is null)
        {
            var request3 = new QueryMultipleRequest(3,
                tableName3,
                connection,
                transaction,
                fields3 ?? DbFieldCache.Get(connection, tableName3, transaction).AsFields(),
                where3,
                orderBy3,
                top3,
                hints3,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request3, CommandTextCache.GetQueryMultipleText));

            if (where3 is not null)
                maps.Add(where3.MapTo());
        }

        var (commandText, param) = CombineQueryMultiple(connection, transaction, commandTexts, maps, statementBuilder);
        Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>> result;
        TraceResult? traceResult = null;

        // Before Execution
        var beforeExecutionCallback = CreateCallback(command =>
            traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command));

        // Actual Execution
        using (var reader = ExecuteReaderInternal(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            commandTimeout: commandTimeout,
            traceKey: null,
            transaction: transaction,
            trace: null,
            entityType: null,
            dbFields: null,
            skipCommandArrayParametersCheck: true,
            beforeExecutionCallback: beforeExecutionCallback))
        {
            // Silent cancellation
            if (traceResult?.CancellableTraceLog?.IsCancelled == true)
            {
                return new([], [], []);
            }

            // DB setting
            var dbSetting = connection.GetDbSetting();

            // T1
            item1 = QueryDynamicMultipleInternal(connection,
                reader,
                item1,
                tableName1,
                cacheKey1,
                cacheItemExpiration,
                transaction,
                cache,
                false);

            // T2
            item2 = QueryDynamicMultipleInternal(connection,
                reader,
                item2,
                tableName2,
                cacheKey2,
                cacheItemExpiration,
                transaction,
                cache,
                true);

            // T3
            item3 = QueryDynamicMultipleInternal(connection,
                reader,
                item3,
                tableName3,
                cacheKey3,
                cacheItemExpiration,
                transaction,
                cache,
                true);


            // Result
            result = Tuple.Create(item1, item2, item3);

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);
        }

        // Result
        return result;
    }

    #endregion

    #region T1, T2, T3, T4

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 4 target types.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName1">The name of the target table (for dynamic type 1).</param>
    /// <param name="what1">The dynamic query expression or the key value to be used (for dynamic type 1).</param>
    /// <param name="tableName2">The name of the target table (for dynamic type 2).</param>
    /// <param name="what2">The dynamic query expression or the key value to be used (for dynamic type 2).</param>
    /// <param name="tableName3">The name of the target table (for dynamic type 3).</param>
    /// <param name="what3">The dynamic expression or the key value to be used (for dynamic type 3).</param>
    /// <param name="tableName4">The name of the target table (for dynamic type 4).</param>
    /// <param name="what4">The dynamic expression or the key value to be used (for dynamic type 4).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for dynamic type 1).</param>
    /// <param name="top1">The number of rows to be returned (for dynamic type 1).</param>
    /// <param name="hints1">The table hints to be used (for dynamic type 1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for dynamic type 2).</param>
    /// <param name="top2">The number of rows to be returned (for dynamic type 2).</param>
    /// <param name="hints2">The table hints to be used (for dynamic type 2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for dynamic type 3).</param>
    /// <param name="top3">The number of rows to be returned (for dynamic type 3).</param>
    /// <param name="hints3">The table hints to be used (for dynamic type 3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for dynamic type 4).</param>
    /// <param name="top4">The number of rows to be returned (for dynamic type 4).</param>
    /// <param name="hints4">The table hints to be used (for dynamic type 4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 4 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>> QueryMultiple(this IDbConnection connection,
        string tableName1,
        object what1,
        string tableName2,
        object what2,
        string tableName3,
        object what3,
        string tableName4,
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
    {
        return QueryMultipleInternal(connection: connection,
            tableName1: tableName1,
            where1: WhatToQueryGroup(what1),
            tableName2: tableName2,
            where2: WhatToQueryGroup(what2),
            tableName3: tableName3,
            where3: WhatToQueryGroup(what3),
            tableName4: tableName4,
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
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName1">The name of the target table (for dynamic type 1).</param>
    /// <param name="where1">The query expression to be used (for dynamic type 1).</param>
    /// <param name="tableName2">The name of the target table (for dynamic type 2).</param>
    /// <param name="where2">The query expression to be used (for dynamic type 2).</param>
    /// <param name="tableName3">The name of the target table (for dynamic type 3).</param>
    /// <param name="where3">The query expression to be used (for dynamic type 3).</param>
    /// <param name="tableName4">The name of the target table (for dynamic type 4).</param>
    /// <param name="where4">The query expression to be used (for dynamic type 4).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for dynamic type 1).</param>
    /// <param name="top1">The number of rows to be returned (for dynamic type 1).</param>
    /// <param name="hints1">The table hints to be used (for dynamic type 1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for dynamic type 2).</param>
    /// <param name="top2">The number of rows to be returned (for dynamic type 2).</param>
    /// <param name="hints2">The table hints to be used (for dynamic type 2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for dynamic type 3).</param>
    /// <param name="top3">The number of rows to be returned (for dynamic type 3).</param>
    /// <param name="hints3">The table hints to be used (for dynamic type 3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for dynamic type 4).</param>
    /// <param name="top4">The number of rows to be returned (for dynamic type 4).</param>
    /// <param name="hints4">The table hints to be used (for dynamic type 4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 4 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>> QueryMultiple(this IDbConnection connection,
        string tableName1,
        QueryField where1,
        string tableName2,
        QueryField where2,
        string tableName3,
        QueryField where3,
        string tableName4,
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
    {
        return QueryMultipleInternal(connection: connection,
            tableName1: tableName1,
            where1: ToQueryGroup(where1),
            tableName2: tableName2,
            where2: ToQueryGroup(where2),
            tableName3: tableName3,
            where3: ToQueryGroup(where3),
            tableName4: tableName4,
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
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName1">The name of the target table (for dynamic type 1).</param>
    /// <param name="where1">The query expression to be used (for dynamic type 1).</param>
    /// <param name="tableName2">The name of the target table (for dynamic type 2).</param>
    /// <param name="where2">The query expression to be used (for dynamic type 2).</param>
    /// <param name="tableName3">The name of the target table (for dynamic type 3).</param>
    /// <param name="where3">The query expression to be used (for dynamic type 3).</param>
    /// <param name="tableName4">The name of the target table (for dynamic type 4).</param>
    /// <param name="where4">The query expression to be used (for dynamic type 4).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for dynamic type 1).</param>
    /// <param name="top1">The number of rows to be returned (for dynamic type 1).</param>
    /// <param name="hints1">The table hints to be used (for dynamic type 1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for dynamic type 2).</param>
    /// <param name="top2">The number of rows to be returned (for dynamic type 2).</param>
    /// <param name="hints2">The table hints to be used (for dynamic type 2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for dynamic type 3).</param>
    /// <param name="top3">The number of rows to be returned (for dynamic type 3).</param>
    /// <param name="hints3">The table hints to be used (for dynamic type 3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for dynamic type 4).</param>
    /// <param name="top4">The number of rows to be returned (for dynamic type 4).</param>
    /// <param name="hints4">The table hints to be used (for dynamic type 4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 4 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>> QueryMultiple(this IDbConnection connection,
        string tableName1,
        IEnumerable<QueryField> where1,
        string tableName2,
        IEnumerable<QueryField> where2,
        string tableName3,
        IEnumerable<QueryField> where3,
        string tableName4,
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
    {
        return QueryMultipleInternal(connection: connection,
            tableName1: tableName1,
            where1: ToQueryGroup(where1),
            tableName2: tableName2,
            where2: ToQueryGroup(where2),
            tableName3: tableName3,
            where3: ToQueryGroup(where3),
            tableName4: tableName4,
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
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName1">The name of the target table (for dynamic type 1).</param>
    /// <param name="where1">The query expression to be used (for dynamic type 1).</param>
    /// <param name="tableName2">The name of the target table (for dynamic type 2).</param>
    /// <param name="where2">The query expression to be used (for dynamic type 2).</param>
    /// <param name="tableName3">The name of the target table (for dynamic type 3).</param>
    /// <param name="where3">The query expression to be used (for dynamic type 3).</param>
    /// <param name="tableName4">The name of the target table (for dynamic type 4).</param>
    /// <param name="where4">The query expression to be used (for dynamic type 4).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for dynamic type 1).</param>
    /// <param name="top1">The number of rows to be returned (for dynamic type 1).</param>
    /// <param name="hints1">The table hints to be used (for dynamic type 1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for dynamic type 2).</param>
    /// <param name="top2">The number of rows to be returned (for dynamic type 2).</param>
    /// <param name="hints2">The table hints to be used (for dynamic type 2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for dynamic type 3).</param>
    /// <param name="top3">The number of rows to be returned (for dynamic type 3).</param>
    /// <param name="hints3">The table hints to be used (for dynamic type 3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for dynamic type 4).</param>
    /// <param name="top4">The number of rows to be returned (for dynamic type 4).</param>
    /// <param name="hints4">The table hints to be used (for dynamic type 4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 4 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>> QueryMultiple(this IDbConnection connection,
        string tableName1,
        QueryGroup? where1,
        string tableName2,
        QueryGroup? where2,
        string tableName3,
        QueryGroup? where3,
        string tableName4,
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
    {
        return QueryMultipleInternal(connection: connection,
            tableName1: tableName1,
            where1: where1,
            tableName2: tableName2,
            where2: where2,
            tableName3: tableName3,
            where3: where3,
            tableName4: tableName4,
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
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName1">The name of the target table (for dynamic type 1).</param>
    /// <param name="where1">The query expression to be used (for dynamic type 1).</param>
    /// <param name="tableName2">The name of the target table (for dynamic type 2).</param>
    /// <param name="where2">The query expression to be used (for dynamic type 2).</param>
    /// <param name="tableName3">The name of the target table (for dynamic type 3).</param>
    /// <param name="where3">The query expression to be used (for dynamic type 3).</param>
    /// <param name="tableName4">The name of the target table (for dynamic type 4).</param>
    /// <param name="where4">The query expression to be used (for dynamic type 4).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for dynamic type 1).</param>
    /// <param name="top1">The number of rows to be returned (for dynamic type 1).</param>
    /// <param name="hints1">The table hints to be used (for dynamic type 1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for dynamic type 2).</param>
    /// <param name="top2">The number of rows to be returned (for dynamic type 2).</param>
    /// <param name="hints2">The table hints to be used (for dynamic type 2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for dynamic type 3).</param>
    /// <param name="top3">The number of rows to be returned (for dynamic type 3).</param>
    /// <param name="hints3">The table hints to be used (for dynamic type 3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for dynamic type 4).</param>
    /// <param name="top4">The number of rows to be returned (for dynamic type 4).</param>
    /// <param name="hints4">The table hints to be used (for dynamic type 4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 4 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>> QueryMultipleInternal(this IDbConnection connection,
        string tableName1,
        QueryGroup? where1,
        string tableName2,
        QueryGroup? where2,
        string tableName3,
        QueryGroup? where3,
        string tableName4,
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
    {
        // Variables
        var commandType = CommandType.Text;
        var queryGroups = new List<QueryGroup?>();
        var maps = new List<QueryGroupTypeMap>();
        var commandTexts = new List<string>();

        // Items
        var item1 = QueryDynamicMultipleInternal(cacheKey1,
            cache,
            where1,
            queryGroups);

        var item2 = QueryDynamicMultipleInternal(cacheKey2,
            cache,
            where2,
            queryGroups);

        var item3 = QueryDynamicMultipleInternal(cacheKey3,
            cache,
            where3,
            queryGroups);

        var item4 = QueryDynamicMultipleInternal(cacheKey4,
            cache,
            where4,
            queryGroups);

        // Fix
        QueryGroup.FixForQueryMultiple(queryGroups);

        // Item1 Request
        if (item1 is null)
        {
            var request1 = new QueryMultipleRequest(1,
                tableName1,
                connection,
                transaction,
                fields1 ?? DbFieldCache.Get(connection, tableName1, transaction).AsFields(),
                where1,
                orderBy1,
                top1,
                hints1,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request1, CommandTextCache.GetQueryMultipleText));

            if (where1 is not null)
                maps.Add(where1.MapTo());
        }

        // Item2 Request
        if (item2 is null)
        {
            var request2 = new QueryMultipleRequest(2,
                tableName2,
                connection,
                transaction,
                fields2 ?? DbFieldCache.Get(connection, tableName2, transaction).AsFields(),
                where2,
                orderBy2,
                top2,
                hints2,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request2, CommandTextCache.GetQueryMultipleText));

            if (where2 is not null)
                maps.Add(where2.MapTo());
        }

        // Item3 Request
        if (item3 is null)
        {
            var request3 = new QueryMultipleRequest(3,
                tableName3,
                connection,
                transaction,
                fields3 ?? DbFieldCache.Get(connection, tableName3, transaction).AsFields(),
                where3,
                orderBy3,
                top3,
                hints3,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request3, CommandTextCache.GetQueryMultipleText));

            if (where3 is not null)
                maps.Add(where3.MapTo());
        }

        // Item4 Request
        if (item4 is null)
        {
            var request4 = new QueryMultipleRequest(4,
                tableName4,
                connection,
                transaction,
                fields4 ?? DbFieldCache.Get(connection, tableName4, transaction).AsFields(),
                where4,
                orderBy4,
                top4,
                hints4,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request4, CommandTextCache.GetQueryMultipleText));

            if (where4 is not null)
                maps.Add(where4.MapTo());
        }


        var (commandText, param) = CombineQueryMultiple(connection, transaction, commandTexts, maps, statementBuilder);
        Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>> result;
        TraceResult? traceResult = null;

        // Before Execution
        var beforeExecutionCallback = CreateCallback(command =>
            traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command));

        // Actual Execution
        using (var reader = ExecuteReaderInternal(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            commandTimeout: commandTimeout,
            traceKey: null,
            transaction: transaction,
            trace: null,
            entityType: null,
            dbFields: null,
            skipCommandArrayParametersCheck: true,
            beforeExecutionCallback: beforeExecutionCallback))
        {
            // Silent cancellation
            if (traceResult?.CancellableTraceLog?.IsCancelled == true)
            {
                return new([], [], [], []);
            }

            // DB setting
            var dbSetting = connection.GetDbSetting();

            // T1
            item1 = QueryDynamicMultipleInternal(connection,
                reader,
                item1,
                tableName1,
                cacheKey1,
                cacheItemExpiration,
                transaction,
                cache,
                false);

            // T2
            item2 = QueryDynamicMultipleInternal(connection,
                reader,
                item2,
                tableName2,
                cacheKey2,
                cacheItemExpiration,
                transaction,
                cache,
                true);

            // T3
            item3 = QueryDynamicMultipleInternal(connection,
                reader,
                item3,
                tableName3,
                cacheKey3,
                cacheItemExpiration,
                transaction,
                cache,
                true);

            // T4
            item4 = QueryDynamicMultipleInternal(connection,
                reader,
                item4,
                tableName4,
                cacheKey4,
                cacheItemExpiration,
                transaction,
                cache,
                true);

            // Result
            result = Tuple.Create(item1, item2, item3, item4);

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);
        }

        // Result
        return result;
    }

    #endregion

    #region T1, T2, T3, T4, T5

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 5 target types.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName1">The name of the target table (for dynamic type 1).</param>
    /// <param name="what1">The dynamic query expression or the key value to be used (for dynamic type 1).</param>
    /// <param name="tableName2">The name of the target table (for dynamic type 2).</param>
    /// <param name="what2">The dynamic query expression or the key value to be used (for dynamic type 2).</param>
    /// <param name="tableName3">The name of the target table (for dynamic type 3).</param>
    /// <param name="what3">The dynamic expression or the key value to be used (for dynamic type 3).</param>
    /// <param name="tableName4">The name of the target table (for dynamic type 4).</param>
    /// <param name="what4">The dynamic expression or the key value to be used (for dynamic type 4).</param>
    /// <param name="tableName5">The name of the target table (for dynamic type 5).</param>
    /// <param name="what5">The dynamic expression or the key value to be used (for dynamic type 5).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for dynamic type 1).</param>
    /// <param name="top1">The number of rows to be returned (for dynamic type 1).</param>
    /// <param name="hints1">The table hints to be used (for dynamic type 1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for dynamic type 2).</param>
    /// <param name="top2">The number of rows to be returned (for dynamic type 2).</param>
    /// <param name="hints2">The table hints to be used (for dynamic type 2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for dynamic type 3).</param>
    /// <param name="top3">The number of rows to be returned (for dynamic type 3).</param>
    /// <param name="hints3">The table hints to be used (for dynamic type 3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for dynamic type 4).</param>
    /// <param name="top4">The number of rows to be returned (for dynamic type 4).</param>
    /// <param name="hints4">The table hints to be used (for dynamic type 4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields5">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 5).</param>
    /// <param name="orderBy5">The order definition of the fields to be used (for dynamic type 5).</param>
    /// <param name="top5">The number of rows to be returned (for dynamic type 5).</param>
    /// <param name="hints5">The table hints to be used (for dynamic type 5).</param>
    /// <param name="cacheKey5">The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 5 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>> QueryMultiple(this IDbConnection connection,
        string tableName1,
        object what1,
        string tableName2,
        object what2,
        string tableName3,
        object what3,
        string tableName4,
        object what4,
        string tableName5,
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
    {
        return QueryMultipleInternal(connection: connection,
            tableName1: tableName1,
            where1: WhatToQueryGroup(what1),
            tableName2: tableName2,
            where2: WhatToQueryGroup(what2),
            tableName3: tableName3,
            where3: WhatToQueryGroup(what3),
            tableName4: tableName4,
            where4: WhatToQueryGroup(what4),
            tableName5: tableName5,
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
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName1">The name of the target table (for dynamic type 1).</param>
    /// <param name="where1">The query expression to be used (for dynamic type 1).</param>
    /// <param name="tableName2">The name of the target table (for dynamic type 2).</param>
    /// <param name="where2">The query expression to be used (for dynamic type 2).</param>
    /// <param name="tableName3">The name of the target table (for dynamic type 3).</param>
    /// <param name="where3">The query expression to be used (for dynamic type 3).</param>
    /// <param name="tableName4">The name of the target table (for dynamic type 4).</param>
    /// <param name="where4">The query expression to be used (for dynamic type 4).</param>
    /// <param name="tableName5">The name of the target table (for dynamic type 5).</param>
    /// <param name="where5">The query expression to be used (for dynamic type 5).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for dynamic type 1).</param>
    /// <param name="top1">The number of rows to be returned (for dynamic type 1).</param>
    /// <param name="hints1">The table hints to be used (for dynamic type 1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for dynamic type 2).</param>
    /// <param name="top2">The number of rows to be returned (for dynamic type 2).</param>
    /// <param name="hints2">The table hints to be used (for dynamic type 2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for dynamic type 3).</param>
    /// <param name="top3">The number of rows to be returned (for dynamic type 3).</param>
    /// <param name="hints3">The table hints to be used (for dynamic type 3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for dynamic type 4).</param>
    /// <param name="top4">The number of rows to be returned (for dynamic type 4).</param>
    /// <param name="hints4">The table hints to be used (for dynamic type 4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields5">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 5).</param>
    /// <param name="orderBy5">The order definition of the fields to be used (for dynamic type 5).</param>
    /// <param name="top5">The number of rows to be returned (for dynamic type 5).</param>
    /// <param name="hints5">The table hints to be used (for dynamic type 5).</param>
    /// <param name="cacheKey5">The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 5 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>> QueryMultiple(this IDbConnection connection,
        string tableName1,
        QueryField where1,
        string tableName2,
        QueryField where2,
        string tableName3,
        QueryField where3,
        string tableName4,
        QueryField where4,
        string tableName5,
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
    {
        return QueryMultipleInternal(connection: connection,
            tableName1: tableName1,
            where1: ToQueryGroup(where1),
            tableName2: tableName2,
            where2: ToQueryGroup(where2),
            tableName3: tableName3,
            where3: ToQueryGroup(where3),
            tableName4: tableName4,
            where4: ToQueryGroup(where4),
            tableName5: tableName5,
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
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName1">The name of the target table (for dynamic type 1).</param>
    /// <param name="where1">The query expression to be used (for dynamic type 1).</param>
    /// <param name="tableName2">The name of the target table (for dynamic type 2).</param>
    /// <param name="where2">The query expression to be used (for dynamic type 2).</param>
    /// <param name="tableName3">The name of the target table (for dynamic type 3).</param>
    /// <param name="where3">The query expression to be used (for dynamic type 3).</param>
    /// <param name="tableName4">The name of the target table (for dynamic type 4).</param>
    /// <param name="where4">The query expression to be used (for dynamic type 4).</param>
    /// <param name="tableName5">The name of the target table (for dynamic type 5).</param>
    /// <param name="where5">The query expression to be used (for dynamic type 5).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for dynamic type 1).</param>
    /// <param name="top1">The number of rows to be returned (for dynamic type 1).</param>
    /// <param name="hints1">The table hints to be used (for dynamic type 1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for dynamic type 2).</param>
    /// <param name="top2">The number of rows to be returned (for dynamic type 2).</param>
    /// <param name="hints2">The table hints to be used (for dynamic type 2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for dynamic type 3).</param>
    /// <param name="top3">The number of rows to be returned (for dynamic type 3).</param>
    /// <param name="hints3">The table hints to be used (for dynamic type 3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for dynamic type 4).</param>
    /// <param name="top4">The number of rows to be returned (for dynamic type 4).</param>
    /// <param name="hints4">The table hints to be used (for dynamic type 4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields5">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 5).</param>
    /// <param name="orderBy5">The order definition of the fields to be used (for dynamic type 5).</param>
    /// <param name="top5">The number of rows to be returned (for dynamic type 5).</param>
    /// <param name="hints5">The table hints to be used (for dynamic type 5).</param>
    /// <param name="cacheKey5">The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 5 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>> QueryMultiple(this IDbConnection connection,
        string tableName1,
        IEnumerable<QueryField> where1,
        string tableName2,
        IEnumerable<QueryField> where2,
        string tableName3,
        IEnumerable<QueryField> where3,
        string tableName4,
        IEnumerable<QueryField> where4,
        string tableName5,
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
    {
        return QueryMultipleInternal(connection: connection,
            tableName1: tableName1,
            where1: ToQueryGroup(where1),
            tableName2: tableName2,
            where2: ToQueryGroup(where2),
            tableName3: tableName3,
            where3: ToQueryGroup(where3),
            tableName4: tableName4,
            where4: ToQueryGroup(where4),
            tableName5: tableName5,
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
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName1">The name of the target table (for dynamic type 1).</param>
    /// <param name="where1">The query expression to be used (for dynamic type 1).</param>
    /// <param name="tableName2">The name of the target table (for dynamic type 2).</param>
    /// <param name="where2">The query expression to be used (for dynamic type 2).</param>
    /// <param name="tableName3">The name of the target table (for dynamic type 3).</param>
    /// <param name="where3">The query expression to be used (for dynamic type 3).</param>
    /// <param name="tableName4">The name of the target table (for dynamic type 4).</param>
    /// <param name="where4">The query expression to be used (for dynamic type 4).</param>
    /// <param name="tableName5">The name of the target table (for dynamic type 5).</param>
    /// <param name="where5">The query expression to be used (for dynamic type 5).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for dynamic type 1).</param>
    /// <param name="top1">The number of rows to be returned (for dynamic type 1).</param>
    /// <param name="hints1">The table hints to be used (for dynamic type 1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for dynamic type 2).</param>
    /// <param name="top2">The number of rows to be returned (for dynamic type 2).</param>
    /// <param name="hints2">The table hints to be used (for dynamic type 2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for dynamic type 3).</param>
    /// <param name="top3">The number of rows to be returned (for dynamic type 3).</param>
    /// <param name="hints3">The table hints to be used (for dynamic type 3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for dynamic type 4).</param>
    /// <param name="top4">The number of rows to be returned (for dynamic type 4).</param>
    /// <param name="hints4">The table hints to be used (for dynamic type 4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields5">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 5).</param>
    /// <param name="orderBy5">The order definition of the fields to be used (for dynamic type 5).</param>
    /// <param name="top5">The number of rows to be returned (for dynamic type 5).</param>
    /// <param name="hints5">The table hints to be used (for dynamic type 5).</param>
    /// <param name="cacheKey5">The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 5 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>> QueryMultiple(this IDbConnection connection,
        string tableName1,
        QueryGroup? where1,
        string tableName2,
        QueryGroup? where2,
        string tableName3,
        QueryGroup? where3,
        string tableName4,
        QueryGroup? where4,
        string tableName5,
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
    {
        return QueryMultipleInternal(connection: connection,
            tableName1: tableName1,
            where1: where1,
            tableName2: tableName2,
            where2: where2,
            tableName3: tableName3,
            where3: where3,
            tableName4: tableName4,
            where4: where4,
            tableName5: tableName5,
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
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName1">The name of the target table (for dynamic type 1).</param>
    /// <param name="where1">The query expression to be used (for dynamic type 1).</param>
    /// <param name="tableName2">The name of the target table (for dynamic type 2).</param>
    /// <param name="where2">The query expression to be used (for dynamic type 2).</param>
    /// <param name="tableName3">The name of the target table (for dynamic type 3).</param>
    /// <param name="where3">The query expression to be used (for dynamic type 3).</param>
    /// <param name="tableName4">The name of the target table (for dynamic type 4).</param>
    /// <param name="where4">The query expression to be used (for dynamic type 4).</param>
    /// <param name="tableName5">The name of the target table (for dynamic type 5).</param>
    /// <param name="where5">The query expression to be used (for dynamic type 5).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for dynamic type 1).</param>
    /// <param name="top1">The number of rows to be returned (for dynamic type 1).</param>
    /// <param name="hints1">The table hints to be used (for dynamic type 1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for dynamic type 2).</param>
    /// <param name="top2">The number of rows to be returned (for dynamic type 2).</param>
    /// <param name="hints2">The table hints to be used (for dynamic type 2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for dynamic type 3).</param>
    /// <param name="top3">The number of rows to be returned (for dynamic type 3).</param>
    /// <param name="hints3">The table hints to be used (for dynamic type 3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for dynamic type 4).</param>
    /// <param name="top4">The number of rows to be returned (for dynamic type 4).</param>
    /// <param name="hints4">The table hints to be used (for dynamic type 4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields5">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 5).</param>
    /// <param name="orderBy5">The order definition of the fields to be used (for dynamic type 5).</param>
    /// <param name="top5">The number of rows to be returned (for dynamic type 5).</param>
    /// <param name="hints5">The table hints to be used (for dynamic type 5).</param>
    /// <param name="cacheKey5">The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 5 enumerable target data entity types.</returns>
    internal static Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>> QueryMultipleInternal(this IDbConnection connection,
        string tableName1,
        QueryGroup? where1,
        string tableName2,
        QueryGroup? where2,
        string tableName3,
        QueryGroup? where3,
        string tableName4,
        QueryGroup? where4,
        string tableName5,
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
    {
        // Variables
        var commandType = CommandType.Text;
        var queryGroups = new List<QueryGroup?>();
        var maps = new List<QueryGroupTypeMap>();
        var commandTexts = new List<string>();

        // Items
        var item1 = QueryDynamicMultipleInternal(cacheKey1,
            cache,
            where1,
            queryGroups);

        var item2 = QueryDynamicMultipleInternal(cacheKey2,
            cache,
            where2,
            queryGroups);

        var item3 = QueryDynamicMultipleInternal(cacheKey3,
            cache,
            where3,
            queryGroups);

        var item4 = QueryDynamicMultipleInternal(cacheKey4,
            cache,
            where4,
            queryGroups);

        var item5 = QueryDynamicMultipleInternal(cacheKey5,
            cache,
            where5,
            queryGroups);

        // Fix
        QueryGroup.FixForQueryMultiple(queryGroups);

        // Item1 Request
        if (item1 is null)
        {
            var request1 = new QueryMultipleRequest(1,
                tableName1,
                connection,
                transaction,
                fields1 ?? DbFieldCache.Get(connection, tableName1, transaction).AsFields(),
                where1,
                orderBy1,
                top1,
                hints1,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request1, CommandTextCache.GetQueryMultipleText));

            if (where1 is not null)
                maps.Add(where1.MapTo());
        }

        // Item2 Request
        if (item2 is null)
        {
            var request2 = new QueryMultipleRequest(2,
                tableName2,
                connection,
                transaction,
                fields2 ?? DbFieldCache.Get(connection, tableName2, transaction).AsFields(),
                where2,
                orderBy2,
                top2,
                hints2,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request2, CommandTextCache.GetQueryMultipleText));

            if (where2 is not null)
                maps.Add(where2.MapTo());
        }

        // Item3 Request
        if (item3 is null)
        {
            var request3 = new QueryMultipleRequest(3,
                tableName3,
                connection,
                transaction,
                fields3 ?? DbFieldCache.Get(connection, tableName3, transaction).AsFields(),
                where3,
                orderBy3,
                top3,
                hints3,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request3, CommandTextCache.GetQueryMultipleText));

            if (where3 is not null)
                maps.Add(where3.MapTo());
        }

        // Item4 Request
        if (item4 is null)
        {
            var request4 = new QueryMultipleRequest(4,
                tableName4,
                connection,
                transaction,
                fields4 ?? DbFieldCache.Get(connection, tableName4, transaction).AsFields(),
                where4,
                orderBy4,
                top4,
                hints4,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request4, CommandTextCache.GetQueryMultipleText));

            if (where4 is not null)
                maps.Add(where4.MapTo());
        }

        // Item5 Request
        if (item5 is null)
        {
            var request5 = new QueryMultipleRequest(5,
                tableName5,
                connection,
                transaction,
                fields5 ?? DbFieldCache.Get(connection, tableName5, transaction).AsFields(),
                where5,
                orderBy5,
                top5,
                hints5,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request5, CommandTextCache.GetQueryMultipleText));

            if (where5 is not null)
                maps.Add(where5.MapTo());
        }


        var (commandText, param) = CombineQueryMultiple(connection, transaction, commandTexts, maps, statementBuilder);
        Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>> result;
        TraceResult? traceResult = null;

        // Before Execution
        var beforeExecutionCallback = CreateCallback(command =>
            traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command));

        // Actual Execution
        using (var reader = ExecuteReaderInternal(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            commandTimeout: commandTimeout,
            traceKey: null,
            transaction: transaction,
            trace: null,
            entityType: null,
            dbFields: null,
            skipCommandArrayParametersCheck: true,
            beforeExecutionCallback: beforeExecutionCallback))
        {
            // Silent cancellation
            if (traceResult?.CancellableTraceLog?.IsCancelled == true)
            {
                return new([], [], [], [], []);
            }

            // DB setting
            var dbSetting = connection.GetDbSetting();

            // T1
            item1 = QueryDynamicMultipleInternal(connection,
                reader,
                item1,
                tableName1,
                cacheKey1,
                cacheItemExpiration,
                transaction,
                cache,
                false);

            // T2
            item2 = QueryDynamicMultipleInternal(connection,
                reader,
                item2,
                tableName2,
                cacheKey2,
                cacheItemExpiration,
                transaction,
                cache,
                true);

            // T3
            item3 = QueryDynamicMultipleInternal(connection,
                reader,
                item3,
                tableName3,
                cacheKey3,
                cacheItemExpiration,
                transaction,
                cache,
                true);


            // T4
            item4 = QueryDynamicMultipleInternal(connection,
                reader,
                item4,
                tableName4,
                cacheKey4,
                cacheItemExpiration,
                transaction,
                cache,
                true);

            // T5
            item5 = QueryDynamicMultipleInternal(connection,
                reader,
                item5,
                tableName5,
                cacheKey5,
                cacheItemExpiration,
                transaction,
                cache,
                true);

            // Result
            result = Tuple.Create(item1, item2, item3, item4, item5);

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);
        }

        // Result
        return result;
    }

    #endregion

    #region T1, T2, T3, T4, T5, T6

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 6 target types.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName1">The name of the target table (for dynamic type 1).</param>
    /// <param name="what1">The dynamic query expression or the key value to be used (for dynamic type 1).</param>
    /// <param name="tableName2">The name of the target table (for dynamic type 2).</param>
    /// <param name="what2">The dynamic query expression or the key value to be used (for dynamic type 2).</param>
    /// <param name="tableName3">The name of the target table (for dynamic type 3).</param>
    /// <param name="what3">The dynamic expression or the key value to be used (for dynamic type 3).</param>
    /// <param name="tableName4">The name of the target table (for dynamic type 4).</param>
    /// <param name="what4">The dynamic expression or the key value to be used (for dynamic type 4).</param>
    /// <param name="tableName5">The name of the target table (for dynamic type 5).</param>
    /// <param name="what5">The dynamic expression or the key value to be used (for dynamic type 5).</param>
    /// <param name="tableName6">The name of the target table (for dynamic type 6).</param>
    /// <param name="what6">The dynamic expression or the key value to be used (for dynamic type 6).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for dynamic type 1).</param>
    /// <param name="top1">The number of rows to be returned (for dynamic type 1).</param>
    /// <param name="hints1">The table hints to be used (for dynamic type 1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for dynamic type 2).</param>
    /// <param name="top2">The number of rows to be returned (for dynamic type 2).</param>
    /// <param name="hints2">The table hints to be used (for dynamic type 2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for dynamic type 3).</param>
    /// <param name="top3">The number of rows to be returned (for dynamic type 3).</param>
    /// <param name="hints3">The table hints to be used (for dynamic type 3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for dynamic type 4).</param>
    /// <param name="top4">The number of rows to be returned (for dynamic type 4).</param>
    /// <param name="hints4">The table hints to be used (for dynamic type 4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields5">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 5).</param>
    /// <param name="orderBy5">The order definition of the fields to be used (for dynamic type 5).</param>
    /// <param name="top5">The number of rows to be returned (for dynamic type 5).</param>
    /// <param name="hints5">The table hints to be used (for dynamic type 5).</param>
    /// <param name="cacheKey5">The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields6">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 6).</param>
    /// <param name="orderBy6">The order definition of the fields to be used (for dynamic type 6).</param>
    /// <param name="top6">The number of rows to be returned (for dynamic type 6).</param>
    /// <param name="hints6">The table hints to be used (for dynamic type 6).</param>
    /// <param name="cacheKey6">The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 6 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>> QueryMultiple(this IDbConnection connection,
        string tableName1,
        object what1,
        string tableName2,
        object what2,
        string tableName3,
        object what3,
        string tableName4,
        object what4,
        string tableName5,
        object what5,
        string tableName6,
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
    {
        return QueryMultipleInternal(connection: connection,
            tableName1: tableName1,
            where1: WhatToQueryGroup(what1),
            tableName2: tableName2,
            where2: WhatToQueryGroup(what2),
            tableName3: tableName3,
            where3: WhatToQueryGroup(what3),
            tableName4: tableName4,
            where4: WhatToQueryGroup(what4),
            tableName5: tableName5,
            where5: WhatToQueryGroup(what5),
            tableName6: tableName6,
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
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName1">The name of the target table (for dynamic type 1).</param>
    /// <param name="where1">The query expression to be used (for dynamic type 1).</param>
    /// <param name="tableName2">The name of the target table (for dynamic type 2).</param>
    /// <param name="where2">The query expression to be used (for dynamic type 2).</param>
    /// <param name="tableName3">The name of the target table (for dynamic type 3).</param>
    /// <param name="where3">The query expression to be used (for dynamic type 3).</param>
    /// <param name="tableName4">The name of the target table (for dynamic type 4).</param>
    /// <param name="where4">The query expression to be used (for dynamic type 4).</param>
    /// <param name="tableName5">The name of the target table (for dynamic type 5).</param>
    /// <param name="where5">The query expression to be used (for dynamic type 5).</param>
    /// <param name="tableName6">The name of the target table (for dynamic type 6).</param>
    /// <param name="where6">The query expression to be used (for dynamic type 6).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for dynamic type 1).</param>
    /// <param name="top1">The number of rows to be returned (for dynamic type 1).</param>
    /// <param name="hints1">The table hints to be used (for dynamic type 1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for dynamic type 2).</param>
    /// <param name="top2">The number of rows to be returned (for dynamic type 2).</param>
    /// <param name="hints2">The table hints to be used (for dynamic type 2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for dynamic type 3).</param>
    /// <param name="top3">The number of rows to be returned (for dynamic type 3).</param>
    /// <param name="hints3">The table hints to be used (for dynamic type 3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for dynamic type 4).</param>
    /// <param name="top4">The number of rows to be returned (for dynamic type 4).</param>
    /// <param name="hints4">The table hints to be used (for dynamic type 4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields5">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 5).</param>
    /// <param name="orderBy5">The order definition of the fields to be used (for dynamic type 5).</param>
    /// <param name="top5">The number of rows to be returned (for dynamic type 5).</param>
    /// <param name="hints5">The table hints to be used (for dynamic type 5).</param>
    /// <param name="cacheKey5">The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields6">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 6).</param>
    /// <param name="orderBy6">The order definition of the fields to be used (for dynamic type 6).</param>
    /// <param name="top6">The number of rows to be returned (for dynamic type 6).</param>
    /// <param name="hints6">The table hints to be used (for dynamic type 6).</param>
    /// <param name="cacheKey6">The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 6 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>> QueryMultiple(this IDbConnection connection,
        string tableName1,
        QueryField where1,
        string tableName2,
        QueryField where2,
        string tableName3,
        QueryField where3,
        string tableName4,
        QueryField where4,
        string tableName5,
        QueryField where5,
        string tableName6,
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
    {
        return QueryMultipleInternal(connection: connection,
            tableName1: tableName1,
            where1: ToQueryGroup(where1),
            tableName2: tableName2,
            where2: ToQueryGroup(where2),
            tableName3: tableName3,
            where3: ToQueryGroup(where3),
            tableName4: tableName4,
            where4: ToQueryGroup(where4),
            tableName5: tableName5,
            where5: ToQueryGroup(where5),
            tableName6: tableName6,
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
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName1">The name of the target table (for dynamic type 1).</param>
    /// <param name="where1">The query expression to be used (for dynamic type 1).</param>
    /// <param name="tableName2">The name of the target table (for dynamic type 2).</param>
    /// <param name="where2">The query expression to be used (for dynamic type 2).</param>
    /// <param name="tableName3">The name of the target table (for dynamic type 3).</param>
    /// <param name="where3">The query expression to be used (for dynamic type 3).</param>
    /// <param name="tableName4">The name of the target table (for dynamic type 4).</param>
    /// <param name="where4">The query expression to be used (for dynamic type 4).</param>
    /// <param name="tableName5">The name of the target table (for dynamic type 5).</param>
    /// <param name="where5">The query expression to be used (for dynamic type 5).</param>
    /// <param name="tableName6">The name of the target table (for dynamic type 6).</param>
    /// <param name="where6">The query expression to be used (for dynamic type 6).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for dynamic type 1).</param>
    /// <param name="top1">The number of rows to be returned (for dynamic type 1).</param>
    /// <param name="hints1">The table hints to be used (for dynamic type 1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for dynamic type 2).</param>
    /// <param name="top2">The number of rows to be returned (for dynamic type 2).</param>
    /// <param name="hints2">The table hints to be used (for dynamic type 2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for dynamic type 3).</param>
    /// <param name="top3">The number of rows to be returned (for dynamic type 3).</param>
    /// <param name="hints3">The table hints to be used (for dynamic type 3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for dynamic type 4).</param>
    /// <param name="top4">The number of rows to be returned (for dynamic type 4).</param>
    /// <param name="hints4">The table hints to be used (for dynamic type 4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields5">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 5).</param>
    /// <param name="orderBy5">The order definition of the fields to be used (for dynamic type 5).</param>
    /// <param name="top5">The number of rows to be returned (for dynamic type 5).</param>
    /// <param name="hints5">The table hints to be used (for dynamic type 5).</param>
    /// <param name="cacheKey5">The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields6">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 6).</param>
    /// <param name="orderBy6">The order definition of the fields to be used (for dynamic type 6).</param>
    /// <param name="top6">The number of rows to be returned (for dynamic type 6).</param>
    /// <param name="hints6">The table hints to be used (for dynamic type 6).</param>
    /// <param name="cacheKey6">The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 6 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>> QueryMultiple(this IDbConnection connection,
        string tableName1,
        IEnumerable<QueryField> where1,
        string tableName2,
        IEnumerable<QueryField> where2,
        string tableName3,
        IEnumerable<QueryField> where3,
        string tableName4,
        IEnumerable<QueryField> where4,
        string tableName5,
        IEnumerable<QueryField> where5,
        string tableName6,
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
    {
        return QueryMultipleInternal(connection: connection,
            tableName1: tableName1,
            where1: ToQueryGroup(where1),
            tableName2: tableName2,
            where2: ToQueryGroup(where2),
            tableName3: tableName3,
            where3: ToQueryGroup(where3),
            tableName4: tableName4,
            where4: ToQueryGroup(where4),
            tableName5: tableName5,
            where5: ToQueryGroup(where5),
            tableName6: tableName6,
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
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName1">The name of the target table (for dynamic type 1).</param>
    /// <param name="where1">The query expression to be used (for dynamic type 1).</param>
    /// <param name="tableName2">The name of the target table (for dynamic type 2).</param>
    /// <param name="where2">The query expression to be used (for dynamic type 2).</param>
    /// <param name="tableName3">The name of the target table (for dynamic type 3).</param>
    /// <param name="where3">The query expression to be used (for dynamic type 3).</param>
    /// <param name="tableName4">The name of the target table (for dynamic type 4).</param>
    /// <param name="where4">The query expression to be used (for dynamic type 4).</param>
    /// <param name="tableName5">The name of the target table (for dynamic type 5).</param>
    /// <param name="where5">The query expression to be used (for dynamic type 5).</param>
    /// <param name="tableName6">The name of the target table (for dynamic type 6).</param>
    /// <param name="where6">The query expression to be used (for dynamic type 6).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for dynamic type 1).</param>
    /// <param name="top1">The number of rows to be returned (for dynamic type 1).</param>
    /// <param name="hints1">The table hints to be used (for dynamic type 1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for dynamic type 2).</param>
    /// <param name="top2">The number of rows to be returned (for dynamic type 2).</param>
    /// <param name="hints2">The table hints to be used (for dynamic type 2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for dynamic type 3).</param>
    /// <param name="top3">The number of rows to be returned (for dynamic type 3).</param>
    /// <param name="hints3">The table hints to be used (for dynamic type 3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for dynamic type 4).</param>
    /// <param name="top4">The number of rows to be returned (for dynamic type 4).</param>
    /// <param name="hints4">The table hints to be used (for dynamic type 4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields5">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 5).</param>
    /// <param name="orderBy5">The order definition of the fields to be used (for dynamic type 5).</param>
    /// <param name="top5">The number of rows to be returned (for dynamic type 5).</param>
    /// <param name="hints5">The table hints to be used (for dynamic type 5).</param>
    /// <param name="cacheKey5">The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields6">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 6).</param>
    /// <param name="orderBy6">The order definition of the fields to be used (for dynamic type 6).</param>
    /// <param name="top6">The number of rows to be returned (for dynamic type 6).</param>
    /// <param name="hints6">The table hints to be used (for dynamic type 6).</param>
    /// <param name="cacheKey6">The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 6 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>> QueryMultiple(this IDbConnection connection,
        string tableName1,
        QueryGroup? where1,
        string tableName2,
        QueryGroup? where2,
        string tableName3,
        QueryGroup? where3,
        string tableName4,
        QueryGroup? where4,
        string tableName5,
        QueryGroup? where5,
        string tableName6,
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
    {
        return QueryMultipleInternal(connection: connection,
            tableName1: tableName1,
            where1: where1,
            tableName2: tableName2,
            where2: where2,
            tableName3: tableName3,
            where3: where3,
            tableName4: tableName4,
            where4: where4,
            tableName5: tableName5,
            where5: where5,
            tableName6: tableName6,
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
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName1">The name of the target table (for dynamic type 1).</param>
    /// <param name="where1">The query expression to be used (for dynamic type 1).</param>
    /// <param name="tableName2">The name of the target table (for dynamic type 2).</param>
    /// <param name="where2">The query expression to be used (for dynamic type 2).</param>
    /// <param name="tableName3">The name of the target table (for dynamic type 3).</param>
    /// <param name="where3">The query expression to be used (for dynamic type 3).</param>
    /// <param name="tableName4">The name of the target table (for dynamic type 4).</param>
    /// <param name="where4">The query expression to be used (for dynamic type 4).</param>
    /// <param name="tableName5">The name of the target table (for dynamic type 5).</param>
    /// <param name="where5">The query expression to be used (for dynamic type 5).</param>
    /// <param name="tableName6">The name of the target table (for dynamic type 6).</param>
    /// <param name="where6">The query expression to be used (for dynamic type 6).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for dynamic type 1).</param>
    /// <param name="top1">The number of rows to be returned (for dynamic type 1).</param>
    /// <param name="hints1">The table hints to be used (for dynamic type 1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for dynamic type 2).</param>
    /// <param name="top2">The number of rows to be returned (for dynamic type 2).</param>
    /// <param name="hints2">The table hints to be used (for dynamic type 2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for dynamic type 3).</param>
    /// <param name="top3">The number of rows to be returned (for dynamic type 3).</param>
    /// <param name="hints3">The table hints to be used (for dynamic type 3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for dynamic type 4).</param>
    /// <param name="top4">The number of rows to be returned (for dynamic type 4).</param>
    /// <param name="hints4">The table hints to be used (for dynamic type 4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields5">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 5).</param>
    /// <param name="orderBy5">The order definition of the fields to be used (for dynamic type 5).</param>
    /// <param name="top5">The number of rows to be returned (for dynamic type 5).</param>
    /// <param name="hints5">The table hints to be used (for dynamic type 5).</param>
    /// <param name="cacheKey5">The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields6">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 6).</param>
    /// <param name="orderBy6">The order definition of the fields to be used (for dynamic type 6).</param>
    /// <param name="top6">The number of rows to be returned (for dynamic type 6).</param>
    /// <param name="hints6">The table hints to be used (for dynamic type 6).</param>
    /// <param name="cacheKey6">The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 6 enumerable target data entity types.</returns>
    internal static Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>> QueryMultipleInternal(this IDbConnection connection,
        string tableName1,
        QueryGroup? where1,
        string tableName2,
        QueryGroup? where2,
        string tableName3,
        QueryGroup? where3,
        string tableName4,
        QueryGroup? where4,
        string tableName5,
        QueryGroup? where5,
        string tableName6,
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
    {
        // Variables
        var commandType = CommandType.Text;
        var queryGroups = new List<QueryGroup?>();
        var maps = new List<QueryGroupTypeMap>();
        var commandTexts = new List<string>();

        // Items
        var item1 = QueryDynamicMultipleInternal(cacheKey1,
            cache,
            where1,
            queryGroups);

        var item2 = QueryDynamicMultipleInternal(cacheKey2,
            cache,
            where2,
            queryGroups);

        var item3 = QueryDynamicMultipleInternal(cacheKey3,
            cache,
            where3,
            queryGroups);

        var item4 = QueryDynamicMultipleInternal(cacheKey4,
            cache,
            where4,
            queryGroups);

        var item5 = QueryDynamicMultipleInternal(cacheKey5,
            cache,
            where5,
            queryGroups);

        var item6 = QueryDynamicMultipleInternal(cacheKey6,
            cache,
            where6,
            queryGroups);

        // Fix
        QueryGroup.FixForQueryMultiple(queryGroups);

        // Item1 Request
        if (item1 is null)
        {
            var request1 = new QueryMultipleRequest(1,
                tableName1,
                connection,
                transaction,
                fields1 ?? DbFieldCache.Get(connection, tableName1, transaction).AsFields(),
                where1,
                orderBy1,
                top1,
                hints1,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request1, CommandTextCache.GetQueryMultipleText));

            if (where1 is not null)
                maps.Add(where1.MapTo());
        }

        // Item2 Request
        if (item2 is null)
        {
            var request2 = new QueryMultipleRequest(2,
                tableName2,
                connection,
                transaction,
                fields2 ?? DbFieldCache.Get(connection, tableName2, transaction).AsFields(),
                where2,
                orderBy2,
                top2,
                hints2,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request2, CommandTextCache.GetQueryMultipleText));

            if (where2 is not null)
                maps.Add(where2.MapTo());
        }

        // Item3 Request
        if (item3 is null)
        {
            var request3 = new QueryMultipleRequest(3,
                tableName3,
                connection,
                transaction,
                fields3 ?? DbFieldCache.Get(connection, tableName3, transaction).AsFields(),
                where3,
                orderBy3,
                top3,
                hints3,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request3, CommandTextCache.GetQueryMultipleText));

            if (where3 is not null)
                maps.Add(where3.MapTo());
        }

        // Item4 Request
        if (item4 is null)
        {
            var request4 = new QueryMultipleRequest(4,
                tableName4,
                connection,
                transaction,
                fields4 ?? DbFieldCache.Get(connection, tableName4, transaction).AsFields(),
                where4,
                orderBy4,
                top4,
                hints4,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request4, CommandTextCache.GetQueryMultipleText));

            if (where4 is not null)
                maps.Add(where4.MapTo());
        }

        // Item5 Request
        if (item5 is null)
        {
            var request5 = new QueryMultipleRequest(5,
                tableName5,
                connection,
                transaction,
                fields5 ?? DbFieldCache.Get(connection, tableName5, transaction).AsFields(),
                where5,
                orderBy5,
                top5,
                hints5,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request5, CommandTextCache.GetQueryMultipleText));

            if (where5 is not null)
                maps.Add(where5.MapTo());
        }

        // Item6 Request
        if (item6 is null)
        {
            var request6 = new QueryMultipleRequest(6,
                tableName6,
                connection,
                transaction,
                fields6 ?? DbFieldCache.Get(connection, tableName6, transaction).AsFields(),
                where6,
                orderBy6,
                top6,
                hints6,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request6, CommandTextCache.GetQueryMultipleText));

            if (where6 is not null)
                maps.Add(where6.MapTo());
        }


        var (commandText, param) = CombineQueryMultiple(connection, transaction, commandTexts, maps, statementBuilder);
        Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>> result;
        TraceResult? traceResult = null;

        // Before Execution
        var beforeExecutionCallback = CreateCallback(command =>
            traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command));

        // Actual Execution
        using (var reader = ExecuteReaderInternal(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            commandTimeout: commandTimeout,
            traceKey: null,
            transaction: transaction,
            trace: null,
            entityType: null,
            dbFields: null,
            skipCommandArrayParametersCheck: true,
            beforeExecutionCallback: beforeExecutionCallback))
        {
            // Silent cancellation
            if (traceResult?.CancellableTraceLog?.IsCancelled == true)
            {
                return new([], [], [], [], [], []);
            }

            // DB setting
            var dbSetting = connection.GetDbSetting();

            // T1
            item1 = QueryDynamicMultipleInternal(connection,
                reader,
                item1,
                tableName1,
                cacheKey1,
                cacheItemExpiration,
                transaction,
                cache,
                false);

            // T2
            item2 = QueryDynamicMultipleInternal(connection,
                reader,
                item2,
                tableName2,
                cacheKey2,
                cacheItemExpiration,
                transaction,
                cache,
                true);

            // T3
            item3 = QueryDynamicMultipleInternal(connection,
                reader,
                item3,
                tableName3,
                cacheKey3,
                cacheItemExpiration,
                transaction,
                cache,
                true);


            // T4
            item4 = QueryDynamicMultipleInternal(connection,
                reader,
                item4,
                tableName4,
                cacheKey4,
                cacheItemExpiration,
                transaction,
                cache,
                true);

            // T5
            item5 = QueryDynamicMultipleInternal(connection,
                reader,
                item5,
                tableName5,
                cacheKey5,
                cacheItemExpiration,
                transaction,
                cache,
                true);

            // T6
            item6 = QueryDynamicMultipleInternal(connection,
                reader,
                item6,
                tableName6,
                cacheKey6,
                cacheItemExpiration,
                transaction,
                cache,
                true);

            // Result
            result = Tuple.Create(item1, item2, item3, item4, item5, item6);

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);
        }

        // Result
        return result;
    }

    #endregion

    #region T1, T2, T3, T4, T5, T6, T7

    /// <summary>
    /// Query the data as multiple resultsets from the table based on the given 7 target types.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName1">The name of the target table (for dynamic type 1).</param>
    /// <param name="what1">The dynamic query expression or the key value to be used (for dynamic type 1).</param>
    /// <param name="tableName2">The name of the target table (for dynamic type 2).</param>
    /// <param name="what2">The dynamic query expression or the key value to be used (for dynamic type 2).</param>
    /// <param name="tableName3">The name of the target table (for dynamic type 3).</param>
    /// <param name="what3">The dynamic expression or the key value to be used (for dynamic type 3).</param>
    /// <param name="tableName4">The name of the target table (for dynamic type 4).</param>
    /// <param name="what4">The dynamic expression or the key value to be used (for dynamic type 4).</param>
    /// <param name="tableName5">The name of the target table (for dynamic type 5).</param>
    /// <param name="what5">The dynamic expression or the key value to be used (for dynamic type 5).</param>
    /// <param name="tableName6">The name of the target table (for dynamic type 6).</param>
    /// <param name="what6">The dynamic expression or the key value to be used (for dynamic type 6).</param>
    /// <param name="tableName7">The name of the target table (for dynamic type 7).</param>
    /// <param name="what7">The dynamic expression or the key value to be used (for dynamic type 7).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for dynamic type 1).</param>
    /// <param name="top1">The number of rows to be returned (for dynamic type 1).</param>
    /// <param name="hints1">The table hints to be used (for dynamic type 1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for dynamic type 2).</param>
    /// <param name="top2">The number of rows to be returned (for dynamic type 2).</param>
    /// <param name="hints2">The table hints to be used (for dynamic type 2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for dynamic type 3).</param>
    /// <param name="top3">The number of rows to be returned (for dynamic type 3).</param>
    /// <param name="hints3">The table hints to be used (for dynamic type 3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for dynamic type 4).</param>
    /// <param name="top4">The number of rows to be returned (for dynamic type 4).</param>
    /// <param name="hints4">The table hints to be used (for dynamic type 4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields5">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 5).</param>
    /// <param name="orderBy5">The order definition of the fields to be used (for dynamic type 5).</param>
    /// <param name="top5">The number of rows to be returned (for dynamic type 5).</param>
    /// <param name="hints5">The table hints to be used (for dynamic type 5).</param>
    /// <param name="cacheKey5">The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields6">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 6).</param>
    /// <param name="orderBy6">The order definition of the fields to be used (for dynamic type 6).</param>
    /// <param name="top6">The number of rows to be returned (for dynamic type 6).</param>
    /// <param name="hints6">The table hints to be used (for dynamic type 6).</param>
    /// <param name="cacheKey6">The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields7">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 7).</param>
    /// <param name="orderBy7">The order definition of the fields to be used (for dynamic type 7).</param>
    /// <param name="top7">The number of rows to be returned (for dynamic type 7).</param>
    /// <param name="hints7">The table hints to be used (for dynamic type 7).</param>
    /// <param name="cacheKey7">The key to the cache item 7. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 7 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>> QueryMultiple(this IDbConnection connection,
        string tableName1,
        object what1,
        string tableName2,
        object what2,
        string tableName3,
        object what3,
        string tableName4,
        object what4,
        string tableName5,
        object what5,
        string tableName6,
        object what6,
        string tableName7,
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
    {
        return QueryMultipleInternal(connection: connection,
            tableName1: tableName1,
            where1: WhatToQueryGroup(what1),
            tableName2: tableName2,
            where2: WhatToQueryGroup(what2),
            tableName3: tableName3,
            where3: WhatToQueryGroup(what3),
            tableName4: tableName4,
            where4: WhatToQueryGroup(what4),
            tableName5: tableName5,
            where5: WhatToQueryGroup(what5),
            tableName6: tableName6,
            where6: WhatToQueryGroup(what6),
            tableName7: tableName7,
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
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName1">The name of the target table (for dynamic type 1).</param>
    /// <param name="where1">The query expression to be used (for dynamic type 1).</param>
    /// <param name="tableName2">The name of the target table (for dynamic type 2).</param>
    /// <param name="where2">The query expression to be used (for dynamic type 2).</param>
    /// <param name="tableName3">The name of the target table (for dynamic type 3).</param>
    /// <param name="where3">The query expression to be used (for dynamic type 3).</param>
    /// <param name="tableName4">The name of the target table (for dynamic type 4).</param>
    /// <param name="where4">The query expression to be used (for dynamic type 4).</param>
    /// <param name="tableName5">The name of the target table (for dynamic type 5).</param>
    /// <param name="where5">The query expression to be used (for dynamic type 5).</param>
    /// <param name="tableName6">The name of the target table (for dynamic type 6).</param>
    /// <param name="where6">The query expression to be used (for dynamic type 6).</param>
    /// <param name="tableName7">The name of the target table (for dynamic type 7).</param>
    /// <param name="where7">The query expression to be used (for dynamic type 7).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for dynamic type 1).</param>
    /// <param name="top1">The number of rows to be returned (for dynamic type 1).</param>
    /// <param name="hints1">The table hints to be used (for dynamic type 1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for dynamic type 2).</param>
    /// <param name="top2">The number of rows to be returned (for dynamic type 2).</param>
    /// <param name="hints2">The table hints to be used (for dynamic type 2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for dynamic type 3).</param>
    /// <param name="top3">The number of rows to be returned (for dynamic type 3).</param>
    /// <param name="hints3">The table hints to be used (for dynamic type 3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for dynamic type 4).</param>
    /// <param name="top4">The number of rows to be returned (for dynamic type 4).</param>
    /// <param name="hints4">The table hints to be used (for dynamic type 4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields5">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 5).</param>
    /// <param name="orderBy5">The order definition of the fields to be used (for dynamic type 5).</param>
    /// <param name="top5">The number of rows to be returned (for dynamic type 5).</param>
    /// <param name="hints5">The table hints to be used (for dynamic type 5).</param>
    /// <param name="cacheKey5">The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields6">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 6).</param>
    /// <param name="orderBy6">The order definition of the fields to be used (for dynamic type 6).</param>
    /// <param name="top6">The number of rows to be returned (for dynamic type 6).</param>
    /// <param name="hints6">The table hints to be used (for dynamic type 6).</param>
    /// <param name="cacheKey6">The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields7">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 7).</param>
    /// <param name="orderBy7">The order definition of the fields to be used (for dynamic type 7).</param>
    /// <param name="top7">The number of rows to be returned (for dynamic type 7).</param>
    /// <param name="hints7">The table hints to be used (for dynamic type 7).</param>
    /// <param name="cacheKey7">The key to the cache item 7. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 7 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>> QueryMultiple(this IDbConnection connection,
        string tableName1,
        QueryField where1,
        string tableName2,
        QueryField where2,
        string tableName3,
        QueryField where3,
        string tableName4,
        QueryField where4,
        string tableName5,
        QueryField where5,
        string tableName6,
        QueryField where6,
        string tableName7,
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
    {
        return QueryMultipleInternal(connection: connection,
            tableName1: tableName1,
            where1: ToQueryGroup(where1),
            tableName2: tableName2,
            where2: ToQueryGroup(where2),
            tableName3: tableName3,
            where3: ToQueryGroup(where3),
            tableName4: tableName4,
            where4: ToQueryGroup(where4),
            tableName5: tableName5,
            where5: ToQueryGroup(where5),
            tableName6: tableName6,
            where6: ToQueryGroup(where6),
            tableName7: tableName7,
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
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName1">The name of the target table (for dynamic type 1).</param>
    /// <param name="where1">The query expression to be used (for dynamic type 1).</param>
    /// <param name="tableName2">The name of the target table (for dynamic type 2).</param>
    /// <param name="where2">The query expression to be used (for dynamic type 2).</param>
    /// <param name="tableName3">The name of the target table (for dynamic type 3).</param>
    /// <param name="where3">The query expression to be used (for dynamic type 3).</param>
    /// <param name="tableName4">The name of the target table (for dynamic type 4).</param>
    /// <param name="where4">The query expression to be used (for dynamic type 4).</param>
    /// <param name="tableName5">The name of the target table (for dynamic type 5).</param>
    /// <param name="where5">The query expression to be used (for dynamic type 5).</param>
    /// <param name="tableName6">The name of the target table (for dynamic type 6).</param>
    /// <param name="where6">The query expression to be used (for dynamic type 6).</param>
    /// <param name="tableName7">The name of the target table (for dynamic type 7).</param>
    /// <param name="where7">The query expression to be used (for dynamic type 7).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for dynamic type 1).</param>
    /// <param name="top1">The number of rows to be returned (for dynamic type 1).</param>
    /// <param name="hints1">The table hints to be used (for dynamic type 1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for dynamic type 2).</param>
    /// <param name="top2">The number of rows to be returned (for dynamic type 2).</param>
    /// <param name="hints2">The table hints to be used (for dynamic type 2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for dynamic type 3).</param>
    /// <param name="top3">The number of rows to be returned (for dynamic type 3).</param>
    /// <param name="hints3">The table hints to be used (for dynamic type 3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for dynamic type 4).</param>
    /// <param name="top4">The number of rows to be returned (for dynamic type 4).</param>
    /// <param name="hints4">The table hints to be used (for dynamic type 4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields5">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 5).</param>
    /// <param name="orderBy5">The order definition of the fields to be used (for dynamic type 5).</param>
    /// <param name="top5">The number of rows to be returned (for dynamic type 5).</param>
    /// <param name="hints5">The table hints to be used (for dynamic type 5).</param>
    /// <param name="cacheKey5">The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields6">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 6).</param>
    /// <param name="orderBy6">The order definition of the fields to be used (for dynamic type 6).</param>
    /// <param name="top6">The number of rows to be returned (for dynamic type 6).</param>
    /// <param name="hints6">The table hints to be used (for dynamic type 6).</param>
    /// <param name="cacheKey6">The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields7">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 7).</param>
    /// <param name="orderBy7">The order definition of the fields to be used (for dynamic type 7).</param>
    /// <param name="top7">The number of rows to be returned (for dynamic type 7).</param>
    /// <param name="hints7">The table hints to be used (for dynamic type 7).</param>
    /// <param name="cacheKey7">The key to the cache item 7. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 7 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>> QueryMultiple(this IDbConnection connection,
        string tableName1,
        IEnumerable<QueryField> where1,
        string tableName2,
        IEnumerable<QueryField> where2,
        string tableName3,
        IEnumerable<QueryField> where3,
        string tableName4,
        IEnumerable<QueryField> where4,
        string tableName5,
        IEnumerable<QueryField> where5,
        string tableName6,
        IEnumerable<QueryField> where6,
        string tableName7,
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
    {
        return QueryMultipleInternal(connection: connection,
            tableName1: tableName1,
            where1: ToQueryGroup(where1),
            tableName2: tableName2,
            where2: ToQueryGroup(where2),
            tableName3: tableName3,
            where3: ToQueryGroup(where3),
            tableName4: tableName4,
            where4: ToQueryGroup(where4),
            tableName5: tableName5,
            where5: ToQueryGroup(where5),
            tableName6: tableName6,
            where6: ToQueryGroup(where6),
            tableName7: tableName7,
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
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName1">The name of the target table (for dynamic type 1).</param>
    /// <param name="where1">The query expression to be used (for dynamic type 1).</param>
    /// <param name="tableName2">The name of the target table (for dynamic type 2).</param>
    /// <param name="where2">The query expression to be used (for dynamic type 2).</param>
    /// <param name="tableName3">The name of the target table (for dynamic type 3).</param>
    /// <param name="where3">The query expression to be used (for dynamic type 3).</param>
    /// <param name="tableName4">The name of the target table (for dynamic type 4).</param>
    /// <param name="where4">The query expression to be used (for dynamic type 4).</param>
    /// <param name="tableName5">The name of the target table (for dynamic type 5).</param>
    /// <param name="where5">The query expression to be used (for dynamic type 5).</param>
    /// <param name="tableName6">The name of the target table (for dynamic type 6).</param>
    /// <param name="where6">The query expression to be used (for dynamic type 6).</param>
    /// <param name="tableName7">The name of the target table (for dynamic type 7).</param>
    /// <param name="where7">The query expression to be used (for dynamic type 7).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for dynamic type 1).</param>
    /// <param name="top1">The number of rows to be returned (for dynamic type 1).</param>
    /// <param name="hints1">The table hints to be used (for dynamic type 1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for dynamic type 2).</param>
    /// <param name="top2">The number of rows to be returned (for dynamic type 2).</param>
    /// <param name="hints2">The table hints to be used (for dynamic type 2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for dynamic type 3).</param>
    /// <param name="top3">The number of rows to be returned (for dynamic type 3).</param>
    /// <param name="hints3">The table hints to be used (for dynamic type 3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for dynamic type 4).</param>
    /// <param name="top4">The number of rows to be returned (for dynamic type 4).</param>
    /// <param name="hints4">The table hints to be used (for dynamic type 4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields5">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 5).</param>
    /// <param name="orderBy5">The order definition of the fields to be used (for dynamic type 5).</param>
    /// <param name="top5">The number of rows to be returned (for dynamic type 5).</param>
    /// <param name="hints5">The table hints to be used (for dynamic type 5).</param>
    /// <param name="cacheKey5">The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields6">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 6).</param>
    /// <param name="orderBy6">The order definition of the fields to be used (for dynamic type 6).</param>
    /// <param name="top6">The number of rows to be returned (for dynamic type 6).</param>
    /// <param name="hints6">The table hints to be used (for dynamic type 6).</param>
    /// <param name="cacheKey6">The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields7">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 7).</param>
    /// <param name="orderBy7">The order definition of the fields to be used (for dynamic type 7).</param>
    /// <param name="top7">The number of rows to be returned (for dynamic type 7).</param>
    /// <param name="hints7">The table hints to be used (for dynamic type 7).</param>
    /// <param name="cacheKey7">The key to the cache item 7. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 7 enumerable target data entity types.</returns>
    public static Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>> QueryMultiple(this IDbConnection connection,
        string tableName1,
        QueryGroup? where1,
        string tableName2,
        QueryGroup? where2,
        string tableName3,
        QueryGroup? where3,
        string tableName4,
        QueryGroup? where4,
        string tableName5,
        QueryGroup? where5,
        string tableName6,
        QueryGroup? where6,
        string tableName7,
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
    {
        return QueryMultipleInternal(connection: connection,
            tableName1: tableName1,
            where1: where1,
            tableName2: tableName2,
            where2: where2,
            tableName3: tableName3,
            where3: where3,
            tableName4: tableName4,
            where4: where4,
            tableName5: tableName5,
            where5: where5,
            tableName6: tableName6,
            where6: where6,
            tableName7: tableName7,
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
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName1">The name of the target table (for dynamic type 1).</param>
    /// <param name="where1">The query expression to be used (for dynamic type 1).</param>
    /// <param name="tableName2">The name of the target table (for dynamic type 2).</param>
    /// <param name="where2">The query expression to be used (for dynamic type 2).</param>
    /// <param name="tableName3">The name of the target table (for dynamic type 3).</param>
    /// <param name="where3">The query expression to be used (for dynamic type 3).</param>
    /// <param name="tableName4">The name of the target table (for dynamic type 4).</param>
    /// <param name="where4">The query expression to be used (for dynamic type 4).</param>
    /// <param name="tableName5">The name of the target table (for dynamic type 5).</param>
    /// <param name="where5">The query expression to be used (for dynamic type 5).</param>
    /// <param name="tableName6">The name of the target table (for dynamic type 6).</param>
    /// <param name="where6">The query expression to be used (for dynamic type 6).</param>
    /// <param name="tableName7">The name of the target table (for dynamic type 7).</param>
    /// <param name="where7">The query expression to be used (for dynamic type 7).</param>
    /// <param name="fields1">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 1).</param>
    /// <param name="orderBy1">The order definition of the fields to be used (for dynamic type 1).</param>
    /// <param name="top1">The number of rows to be returned (for dynamic type 1).</param>
    /// <param name="hints1">The table hints to be used (for dynamic type 1).</param>
    /// <param name="cacheKey1">The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields2">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 2).</param>
    /// <param name="orderBy2">The order definition of the fields to be used (for dynamic type 2).</param>
    /// <param name="top2">The number of rows to be returned (for dynamic type 2).</param>
    /// <param name="hints2">The table hints to be used (for dynamic type 2).</param>
    /// <param name="cacheKey2">The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields3">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 3).</param>
    /// <param name="orderBy3">The order definition of the fields to be used (for dynamic type 3).</param>
    /// <param name="top3">The number of rows to be returned (for dynamic type 3).</param>
    /// <param name="hints3">The table hints to be used (for dynamic type 3).</param>
    /// <param name="cacheKey3">The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields4">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 4).</param>
    /// <param name="orderBy4">The order definition of the fields to be used (for dynamic type 4).</param>
    /// <param name="top4">The number of rows to be returned (for dynamic type 4).</param>
    /// <param name="hints4">The table hints to be used (for dynamic type 4).</param>
    /// <param name="cacheKey4">The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields5">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 5).</param>
    /// <param name="orderBy5">The order definition of the fields to be used (for dynamic type 5).</param>
    /// <param name="top5">The number of rows to be returned (for dynamic type 5).</param>
    /// <param name="hints5">The table hints to be used (for dynamic type 5).</param>
    /// <param name="cacheKey5">The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields6">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 6).</param>
    /// <param name="orderBy6">The order definition of the fields to be used (for dynamic type 6).</param>
    /// <param name="top6">The number of rows to be returned (for dynamic type 6).</param>
    /// <param name="hints6">The table hints to be used (for dynamic type 6).</param>
    /// <param name="cacheKey6">The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="fields7">The mapping list of <see cref="Field"/> objects to be used (for dynamic type 7).</param>
    /// <param name="orderBy7">The order definition of the fields to be used (for dynamic type 7).</param>
    /// <param name="top7">The number of rows to be returned (for dynamic type 7).</param>
    /// <param name="hints7">The table hints to be used (for dynamic type 7).</param>
    /// <param name="cacheKey7">The key to the cache item 7. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A tuple of 7 enumerable target data entity types.</returns>
    internal static Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>> QueryMultipleInternal(this IDbConnection connection,
        string tableName1,
        QueryGroup? where1,
        string tableName2,
        QueryGroup? where2,
        string tableName3,
        QueryGroup? where3,
        string tableName4,
        QueryGroup? where4,
        string tableName5,
        QueryGroup? where5,
        string tableName6,
        QueryGroup? where6,
        string tableName7,
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
    {
        // Variables
        var commandType = CommandType.Text;
        var queryGroups = new List<QueryGroup?>();
        var maps = new List<QueryGroupTypeMap>();
        var commandTexts = new List<string>();

        // Items
        var item1 = QueryDynamicMultipleInternal(cacheKey1,
            cache,
            where1,
            queryGroups);

        var item2 = QueryDynamicMultipleInternal(cacheKey2,
            cache,
            where2,
            queryGroups);

        var item3 = QueryDynamicMultipleInternal(cacheKey3,
            cache,
            where3,
            queryGroups);

        var item4 = QueryDynamicMultipleInternal(cacheKey4,
            cache,
            where4,
            queryGroups);

        var item5 = QueryDynamicMultipleInternal(cacheKey5,
            cache,
            where5,
            queryGroups);

        var item6 = QueryDynamicMultipleInternal(cacheKey6,
            cache,
            where6,
            queryGroups);

        var item7 = QueryDynamicMultipleInternal(cacheKey7,
            cache,
            where7,
            queryGroups);

        // Fix
        QueryGroup.FixForQueryMultiple(queryGroups);

        // Item1 Request
        if (item1 is null)
        {
            var request1 = new QueryMultipleRequest(1,
                tableName1,
                connection,
                transaction,
                fields1 ?? DbFieldCache.Get(connection, tableName1, transaction).AsFields(),
                where1,
                orderBy1,
                top1,
                hints1,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request1, CommandTextCache.GetQueryMultipleText));

            if (where1 is not null)
                maps.Add(where1.MapTo());
        }

        // Item2 Request
        if (item2 is null)
        {
            var request2 = new QueryMultipleRequest(2,
                tableName2,
                connection,
                transaction,
                fields2 ?? DbFieldCache.Get(connection, tableName2, transaction).AsFields(),
                where2,
                orderBy2,
                top2,
                hints2,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request2, CommandTextCache.GetQueryMultipleText));

            if (where2 is not null)
                maps.Add(where2.MapTo());
        }

        // Item3 Request
        if (item3 is null)
        {
            var request3 = new QueryMultipleRequest(3,
                tableName3,
                connection,
                transaction,
                fields3 ?? DbFieldCache.Get(connection, tableName3, transaction).AsFields(),
                where3,
                orderBy3,
                top3,
                hints3,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request3, CommandTextCache.GetQueryMultipleText));

            if (where3 is not null)
                maps.Add(where3.MapTo());
        }

        // Item4 Request
        if (item4 is null)
        {
            var request4 = new QueryMultipleRequest(4,
                tableName4,
                connection,
                transaction,
                fields4 ?? DbFieldCache.Get(connection, tableName4, transaction).AsFields(),
                where4,
                orderBy4,
                top4,
                hints4,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request4, CommandTextCache.GetQueryMultipleText));

            if (where4 is not null)
                maps.Add(where4.MapTo());
        }

        // Item5 Request
        if (item5 is null)
        {
            var request5 = new QueryMultipleRequest(5,
                tableName5,
                connection,
                transaction,
                fields5 ?? DbFieldCache.Get(connection, tableName5, transaction).AsFields(),
                where5,
                orderBy5,
                top5,
                hints5,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request5, CommandTextCache.GetQueryMultipleText));

            if (where5 is not null)
                maps.Add(where5.MapTo());
        }

        // Item6 Request
        if (item6 is null)
        {
            var request6 = new QueryMultipleRequest(6,
                tableName6,
                connection,
                transaction,
                fields6 ?? DbFieldCache.Get(connection, tableName6, transaction).AsFields(),
                where6,
                orderBy6,
                top6,
                hints6,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request6, CommandTextCache.GetQueryMultipleText));

            if (where6 is not null)
                maps.Add(where6.MapTo());
        }

        // Item7 Request
        if (item7 is null)
        {
            var request7 = new QueryMultipleRequest(7,
                tableName7,
                connection,
                transaction,
                fields7 ?? DbFieldCache.Get(connection, tableName7, transaction).AsFields(),
                where7,
                orderBy7,
                top7,
                hints7,
                statementBuilder);
            commandTexts.Add(CommandTextCache.GetCached(request7, CommandTextCache.GetQueryMultipleText));

            if (where7 is not null)
                maps.Add(where7.MapTo());
        }


        var (commandText, param) = CombineQueryMultiple(connection, transaction, commandTexts, maps, statementBuilder);
        Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>, IEnumerable<dynamic>> result;
        TraceResult? traceResult = null;

        // Before Execution
        var beforeExecutionCallback = CreateCallback(command =>
            traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command));

        // Actual Execution
        using (var reader = ExecuteReaderInternal(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            commandTimeout: commandTimeout,
            traceKey: null,
            transaction: transaction,
            trace: null,
            entityType: null,
            dbFields: null,
            skipCommandArrayParametersCheck: true,
            beforeExecutionCallback: beforeExecutionCallback))
        {
            // Silent cancellation
            if (traceResult?.CancellableTraceLog?.IsCancelled == true)
            {
                return new([], [], [], [], [], [], []);
            }

            // DB setting
            var dbSetting = connection.GetDbSetting();

            // T1
            item1 = QueryDynamicMultipleInternal(connection,
                reader,
                item1,
                tableName1,
                cacheKey1,
                cacheItemExpiration,
                transaction,
                cache,
                false);

            // T2
            item2 = QueryDynamicMultipleInternal(connection,
                reader,
                item2,
                tableName2,
                cacheKey2,
                cacheItemExpiration,
                transaction,
                cache,
                true);

            // T3
            item3 = QueryDynamicMultipleInternal(connection,
                reader,
                item3,
                tableName3,
                cacheKey3,
                cacheItemExpiration,
                transaction,
                cache,
                true);


            // T4
            item4 = QueryDynamicMultipleInternal(connection,
                reader,
                item4,
                tableName4,
                cacheKey4,
                cacheItemExpiration,
                transaction,
                cache,
                true);

            // T5
            item5 = QueryDynamicMultipleInternal(connection,
                reader,
                item5,
                tableName5,
                cacheKey5,
                cacheItemExpiration,
                transaction,
                cache,
                true);

            // T6
            item6 = QueryDynamicMultipleInternal(connection,
                reader,
                item6,
                tableName6,
                cacheKey6,
                cacheItemExpiration,
                transaction,
                cache,
                true);

            // T7
            item7 = QueryDynamicMultipleInternal(connection,
                reader,
                item7,
                tableName7,
                cacheKey7,
                cacheItemExpiration,
                transaction,
                cache,
                true);

            // Result
            result = Tuple.Create(item1, item2, item3, item4, item5, item6, item7);

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);
        }

        // Result
        return result;
    }

    #endregion

    #endregion

    #endregion

}
