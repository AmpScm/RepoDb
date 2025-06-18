using System.Data;
using RepoDb.Interfaces;
using RepoDb.Requests;

namespace RepoDb;

/// <summary>
/// Contains the extension methods for <see cref="IDbConnection"/> object.
/// </summary>
public static partial class DbConnectionExtension
{
    #region QueryAll<TEntity>

    /// <summary>
    /// Query all the data from the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
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
    public static IEnumerable<TEntity> QueryAll<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<Field>? fields = null,
        IEnumerable<OrderField>? orderBy = null,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryAll,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return QueryAllInternal<TEntity>(connection: connection,
            tableName: tableName,
            fields: fields,
            orderBy: orderBy,
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
    /// Query all the data from the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
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
    public static IEnumerable<TEntity> QueryAll<TEntity>(this IDbConnection connection,
        IEnumerable<Field>? fields = null,
        IEnumerable<OrderField>? orderBy = null,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryAll,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return QueryAllInternal<TEntity>(connection: connection,
            tableName: ClassMappedNameCache.Get<TEntity>() ?? throw new ArgumentException($"Can't map {typeof(TEntity)} to tablename"),
            fields: fields,
            orderBy: orderBy,
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
    /// Query all the data from the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
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
    internal static IEnumerable<TEntity> QueryAllInternal<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<Field>? fields = null,
        IEnumerable<OrderField>? orderBy = null,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryAll,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        // Ensure the fields
        fields ??= GetQualifiedFields<TEntity>() ??
            DbFieldCache.Get(connection, tableName, transaction).AsFields();

        // Return
        return QueryAllInternalBase<TEntity>(connection: connection,
            tableName: tableName,
            fields: fields,
            orderBy: orderBy,
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

    #region QueryAllAsync<TEntity>

    /// <summary>
    /// Query all the data from the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
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
    public static async Task<IEnumerable<TEntity>> QueryAllAsync<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<Field>? fields = null,
        IEnumerable<OrderField>? orderBy = null,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryAll,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await QueryAllAsyncInternal<TEntity>(connection: connection,
            tableName: tableName,
            fields: fields,
            orderBy: orderBy,
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
    /// Query all the data from the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
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
    public static async Task<IEnumerable<TEntity>> QueryAllAsync<TEntity>(this IDbConnection connection,
        IEnumerable<Field>? fields = null,
        IEnumerable<OrderField>? orderBy = null,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryAll,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await QueryAllAsyncInternal<TEntity>(connection: connection,
            tableName: ClassMappedNameCache.Get<TEntity>() ?? throw new ArgumentException($"Can't map {typeof(TEntity)} to tablename"),
            fields: fields,
            orderBy: orderBy,
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
    /// Query all the data from the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
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
    internal static async ValueTask<IEnumerable<TEntity>> QueryAllAsyncInternal<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<Field>? fields = null,
        IEnumerable<OrderField>? orderBy = null,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryAll,
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
        return await QueryAllAsyncInternalBase<TEntity>(connection: connection,
            tableName: tableName,
            fields: fields,
            orderBy: orderBy,
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

    #region QueryAll(TableName)

    /// <summary>
    /// Query all the data from the table.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
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
    public static IEnumerable<dynamic> QueryAll(this IDbConnection connection,
        string tableName,
        IEnumerable<Field>? fields = null,
        IEnumerable<OrderField>? orderBy = null,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryAll,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return QueryAllInternal<dynamic>(connection: connection,
            tableName: tableName,
            fields: fields,
            orderBy: orderBy,
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
    /// Query all the data from the table.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
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
    internal static IEnumerable<dynamic> QueryAllInternal(this IDbConnection connection,
        string tableName,
        IEnumerable<Field>? fields = null,
        IEnumerable<OrderField>? orderBy = null,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryAll,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return QueryAllInternal<dynamic>(connection: connection,
            tableName: tableName,
            fields: fields,
            orderBy: orderBy,
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

    #region QueryAllAsync(TableName)

    /// <summary>
    /// Query all the data from the table in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
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
    public static async Task<IEnumerable<dynamic>> QueryAllAsync(this IDbConnection connection,
        string tableName,
        IEnumerable<Field>? fields = null,
        IEnumerable<OrderField>? orderBy = null,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryAll,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await QueryAllAsyncInternal<dynamic>(connection: connection,
            tableName: tableName,
            fields: fields,
            orderBy: orderBy,
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
    /// Query all the data from the table in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
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
    internal static ValueTask<IEnumerable<dynamic>> QueryAllAsyncInternal(this IDbConnection connection,
        string tableName,
        IEnumerable<Field>? fields = null,
        IEnumerable<OrderField>? orderBy = null,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryAll,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return QueryAllAsyncInternal<dynamic>(connection: connection,
            tableName: tableName,
            fields: fields,
            orderBy: orderBy,
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

    #region QueryAllInternalBase<TEntity>

    /// <summary>
    /// Query all the data from the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
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
    internal static IEnumerable<TEntity> QueryAllInternalBase<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<Field> fields,
        IEnumerable<OrderField>? orderBy = null,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryAll,
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
        var request = new QueryAllRequest(tableName,
            connection,
            transaction,
            fields,
            orderBy,
            hints,
            statementBuilder);
        var commandText = CommandTextCache.GetQueryAllText(request);

        // Actual Execution
        var result = ExecuteQueryInternal<TEntity>(connection: connection,
            commandText: commandText,
            param: null,
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

    #region QueryAllAsyncInternalBase<TEntity>

    /// <summary>
    /// Query all the data from the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
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
    internal static async ValueTask<IEnumerable<TEntity>> QueryAllAsyncInternalBase<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<Field>? fields = null,
        IEnumerable<OrderField>? orderBy = null,
        string? hints = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryAll,
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
        var request = new QueryAllRequest(tableName,
            connection,
            transaction,
            fields,
            orderBy,
            hints,
            statementBuilder);
        var commandText = await CommandTextCache.GetQueryAllTextAsync(request, cancellationToken).ConfigureAwait(false);

        // Actual Execution
        var result = await ExecuteQueryAsyncInternal<TEntity>(connection: connection,
            commandText: commandText,
            param: null,
            commandType: commandType,
            cacheKey: null,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: null,
            trace: trace,
            cancellationToken: cancellationToken,
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
