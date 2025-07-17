using System.Data;
using System.Data.Common;
using RepoDb.Interfaces;

namespace RepoDb;
public static partial class DbConnectionExtension
{
    #region ExecuteScalar

    /// <summary>
    /// Executes a SQL statement from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteScalar"/> and
    /// returns the first occurrence value (first column of first row) of the execution.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="commandText">The command text to be used.</param>
    /// <param name="param">
    /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
    /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
    /// </param>
    /// <param name="commandType">The command type to be used.</param>
    /// <param name="cacheKey">
    /// The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
    /// This will only work if the 'cache' argument is set.
    /// </param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <returns>An object that holds the first occurrence value (first column of first row) of the execution.</returns>
    public static object? ExecuteScalar(this IDbConnection connection,
        string commandText,
        object? param = null,
        CommandType commandType = default,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        string? traceKey = TraceKeys.ExecuteScalar,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null)
    {
        return ExecuteScalarInternal<object>(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            traceKey: traceKey,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            transaction: transaction,
            cache: cache,
            trace: trace,
            entityType: null,
            dbFields: null,
            skipCommandArrayParametersCheck: false);
    }

    #endregion

    #region ExecuteScalarAsync

    /// <summary>
    /// Executes a SQL statement from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteScalar"/> and
    /// returns the first occurrence value (first column of first row) of the execution.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="commandText">The command text to be used.</param>
    /// <param name="param">
    /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
    /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
    /// </param>
    /// <param name="commandType">The command type to be used.</param>
    /// <param name="cacheKey">
    /// The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
    /// This will only work if the 'cache' argument is set.
    /// </param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An object that holds the first occurrence value (first column of first row) of the execution.</returns>
    public static async Task<object?> ExecuteScalarAsync(this IDbConnection connection,
        string commandText,
        object? param = null,
        CommandType commandType = default,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        string? traceKey = TraceKeys.ExecuteScalar,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteScalarInternalAsync<object>(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            traceKey: traceKey,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            transaction: transaction,
            cache: cache,
            trace: trace,
            cancellationToken: cancellationToken,
            entityType: null,
            dbFields: null,
            skipCommandArrayParametersCheck: false).ConfigureAwait(false);
    }

    #endregion

    #region ExecuteScalar<TResult>

    /// <summary>
    /// Executes a SQL statement from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteScalar"/> and
    /// returns the first occurrence value (first column of first row) of the execution.
    /// </summary>
    /// <typeparam name="TResult">The target return type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="commandText">The command text to be used.</param>
    /// <param name="param">
    /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
    /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
    /// </param>
    /// <param name="commandType">The command type to be used.</param>
    /// <param name="cacheKey">
    /// The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
    /// This will only work if the 'cache' argument is set.
    /// </param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <returns>A first occurrence value (first column of first row) of the execution.</returns>
    public static TResult? ExecuteScalar<TResult>(this IDbConnection connection,
        string commandText,
        object? param = null,
        CommandType commandType = default,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        string? traceKey = TraceKeys.ExecuteScalar,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null)
    {
        return ExecuteScalarInternal<TResult>(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            traceKey: traceKey,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            transaction: transaction,
            cache: cache,
            trace: trace,
            entityType: null,
            dbFields: null,
            skipCommandArrayParametersCheck: false);
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="connection"></param>
    /// <param name="commandText"></param>
    /// <param name="param"></param>
    /// <param name="commandType"></param>
    /// <param name="commandTimeout"></param>
    /// <param name="transaction"></param>
    /// <param name="entityType"></param>
    /// <param name="dbFields"></param>
    /// <param name="skipCommandArrayParametersCheck"></param>
    /// <param name="trace"></param>
    /// <param name="traceKey"></param>
    /// <param name="cache"></param>
    /// <param name="cacheKey"></param>
    /// <param name="cacheItemExpiration"></param>
    /// <returns></returns>
    internal static TResult? ExecuteScalarInternal<TResult>(this IDbConnection connection,
        string commandText,
        object? param,
        CommandType commandType,
        int commandTimeout,
        IDbTransaction? transaction,
        Type? entityType,
        DbFieldCollection? dbFields,
        bool skipCommandArrayParametersCheck = true,
        ITrace? trace = null,
        string? traceKey = null,
        ICache? cache = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes)
    {
        // Get Cache
        if (cache != null && cacheKey != null)
        {
            var item = cache.Get<TResult>(cacheKey, false);
            if (item != null)
            {
                return item.Value;
            }
        }

        using var command = CreateDbCommandForExecution(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            commandTimeout: commandTimeout,
            transaction: transaction,
            entityType: entityType,
            dbFields: dbFields,
            skipCommandArrayParametersCheck: skipCommandArrayParametersCheck);

        // Before Execution
        var traceResult = Tracer
            .InvokeBeforeExecution(traceKey, trace, command);

        // Silent cancellation
        if (traceResult?.CancellableTraceLog?.IsCancelled == true)
        {
            return default;
        }

        // Execute
        var result = Converter.ToType<TResult>(command.ExecuteScalar());

        // After Execution
        Tracer
            .InvokeAfterExecution(traceResult, trace, result);

        // Set Cache
        if (cache != null && cacheKey != null)
        {
            cache.Add(cacheKey, result, cacheItemExpiration, false);
        }

        // Set the output parameters
        SetOutputParameters(param);

        // Return
        return result;
    }

    #endregion

    #region ExecuteScalarAsync<TResult>

    /// <summary>
    /// Executes a SQL statement from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteScalar"/> and
    /// returns the first occurrence value (first column of first row) of the execution.
    /// </summary>
    /// <typeparam name="TResult">The target return type.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="commandText">The command text to be used.</param>
    /// <param name="param">
    /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
    /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
    /// </param>
    /// <param name="commandType">The command type to be used.</param>
    /// <param name="cacheKey">
    /// The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
    /// This will only work if the 'cache' argument is set.
    /// </param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>A first occurrence value (first column of first row) of the execution.</returns>
    public static async Task<TResult?> ExecuteScalarAsync<TResult>(this IDbConnection connection,
        string commandText,
        object? param = null,
        CommandType commandType = default,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        string? traceKey = TraceKeys.ExecuteScalar,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteScalarInternalAsync<TResult>(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            traceKey: traceKey,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            commandTimeout: commandTimeout,
            transaction: transaction,
            cache: cache,
            trace: trace,
            cancellationToken: cancellationToken,
            entityType: null,
            dbFields: null,
            skipCommandArrayParametersCheck: false).ConfigureAwait(false);
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="connection"></param>
    /// <param name="commandText"></param>
    /// <param name="param"></param>
    /// <param name="commandType"></param>
    /// <param name="commandTimeout"></param>
    /// <param name="transaction"></param>
    /// <param name="entityType"></param>
    /// <param name="dbFields"></param>
    /// <param name="skipCommandArrayParametersCheck"></param>
    /// <param name="trace"></param>
    /// <param name="traceKey"></param>
    /// <param name="cache"></param>
    /// <param name="cacheKey"></param>
    /// <param name="cacheItemExpiration"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal static async ValueTask<TResult?> ExecuteScalarInternalAsync<TResult>(
        this IDbConnection connection,
        string commandText,
        object? param,
        CommandType commandType,
        int commandTimeout,
        IDbTransaction? transaction,
        Type? entityType,
        DbFieldCollection? dbFields,
        bool skipCommandArrayParametersCheck = true,
        ITrace? trace = null,
        string? traceKey = null,
        ICache? cache = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        CancellationToken cancellationToken = default)
    {
        // Get Cache
        if (cache != null && cacheKey != null)
        {
            var item = await cache.GetAsync<TResult>(cacheKey, false, cancellationToken).ConfigureAwait(false);
            if (item != null)
            {
                return item.Value;
            }
        }

#if NET
        await
#endif
            using var command = await CreateDbCommandForExecutionAsync(connection: (DbConnection)connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            commandTimeout: commandTimeout,
            transaction: transaction,
            entityType: entityType,
            dbFields: dbFields,
            skipCommandArrayParametersCheck: skipCommandArrayParametersCheck,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        // Before Execution
        var traceResult = await Tracer
            .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken).ConfigureAwait(false);

        // Silent cancellation
        if (traceResult?.CancellableTraceLog?.IsCancelled == true)
        {
            return default;
        }

        // Execution
        var result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false) is { } v ? Converter.ToType<TResult>(v) : default;

        // After Execution
        await Tracer
            .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken).ConfigureAwait(false);

        // Set Cache
        if (cache != null && cacheKey != null)
        {
            await cache.AddAsync(cacheKey, result, cacheItemExpiration, false, cancellationToken).ConfigureAwait(false);
        }

        // Set the output parameters
        SetOutputParameters(param);

        // Return
        return result;
    }

    #endregion
}
