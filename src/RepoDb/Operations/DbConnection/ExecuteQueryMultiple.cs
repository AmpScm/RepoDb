#nullable enable
using System.Data;
using System.Data.Common;
using RepoDb.Interfaces;

namespace RepoDb;
public static partial class DbConnectionExtension
{
    #region ExecuteQueryMultiple(Results)

    /// <summary>
    /// Execute the multiple SQL statements from the database.
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
    /// <returns>An instance of <see cref="QueryMultipleExtractor"/> used to extract the results.</returns>
    public static QueryMultipleExtractor ExecuteQueryMultiple(this IDbConnection connection,
        string commandText,
        object? param = null,
        CommandType commandType = default,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        string? traceKey = TraceKeys.ExecuteQueryMultiple,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null) =>
        ExecuteQueryMultipleInternal(connection,
            commandText,
            param,
            commandType,
            cacheKey,
            cacheItemExpiration,
            traceKey,
            commandTimeout,
            transaction,
            cache,
            trace,
            false);

    public static (IEnumerable<T1>, IEnumerable<T2> v2) ExecuteQueryMultiple<T1, T2>(
        this IDbConnection connection,
        string commandText,
        object? param = null,
        CommandType commandType = default,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        string? traceKey = TraceKeys.ExecuteQueryMultiple,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null) where T1 : class
        where T2 : class
    {
        using var extractor = ExecuteQueryMultiple(
            connection,
            commandText,
            param,
            commandType,
            cacheKey,
            cacheItemExpiration,
            traceKey,
            commandTimeout,
            transaction,
            cache,
            trace);

        return (
            extractor.Extract<T1>(),
            extractor.Extract<T2>()
            );
    }

    public static (IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3> v3) ExecuteQueryMultiple<T1, T2, T3>(
        this IDbConnection connection,
        string commandText,
        object? param = null,
        CommandType commandType = default,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        string? traceKey = TraceKeys.ExecuteQueryMultiple,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null)
            where T1 : class
            where T2 : class
            where T3 : class
    {
        using var extractor = ExecuteQueryMultiple(
            connection,
            commandText,
            param,
            commandType,
            cacheKey,
            cacheItemExpiration,
            traceKey,
            commandTimeout,
            transaction,
            cache,
            trace);

        return (
            extractor.Extract<T1>(),
            extractor.Extract<T2>(),
            extractor.Extract<T3>()
            );
    }

    public static (IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4> v4) ExecuteQueryMultiple<T1, T2, T3, T4>(
        this IDbConnection connection,
        string commandText,
        object? param = null,
        CommandType commandType = default,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        string? traceKey = TraceKeys.ExecuteQueryMultiple,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
    {
        using var extractor = ExecuteQueryMultiple(
            connection,
            commandText,
            param,
            commandType,
            cacheKey,
            cacheItemExpiration,
            traceKey,
            commandTimeout,
            transaction,
            cache,
            trace);

        return (
            extractor.Extract<T1>(),
            extractor.Extract<T2>(),
            extractor.Extract<T3>(),
            extractor.Extract<T4>()
            );
    }

    public static (IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5> v5) ExecuteQueryMultiple<T1, T2, T3, T4, T5>(
        this IDbConnection connection,
        string commandText,
        object? param = null,
        CommandType commandType = default,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        string? traceKey = TraceKeys.ExecuteQueryMultiple,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
    {
        using var extractor = ExecuteQueryMultiple(
            connection,
            commandText,
            param,
            commandType,
            cacheKey,
            cacheItemExpiration,
            traceKey,
            commandTimeout,
            transaction,
            cache,
            trace);

        return (
            extractor.Extract<T1>(),
            extractor.Extract<T2>(),
            extractor.Extract<T3>(),
            extractor.Extract<T4>(),
            extractor.Extract<T5>()
            );
    }

    public static (IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6> v6) ExecuteQueryMultiple<T1, T2, T3, T4, T5, T6>(
        this IDbConnection connection,
        string commandText,
        object? param = null,
        CommandType commandType = default,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        string? traceKey = TraceKeys.ExecuteQueryMultiple,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
    {
        using var extractor = ExecuteQueryMultiple(
            connection,
            commandText,
            param,
            commandType,
            cacheKey,
            cacheItemExpiration,
            traceKey,
            commandTimeout,
            transaction,
            cache,
            trace);

        return (
            extractor.Extract<T1>(),
            extractor.Extract<T2>(),
            extractor.Extract<T3>(),
            extractor.Extract<T4>(),
            extractor.Extract<T5>(),
            extractor.Extract<T6>()
            );
    }

    public static (IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7> v7) ExecuteQueryMultiple<T1, T2, T3, T4, T5, T6, T7>(
        this IDbConnection connection,
        string commandText,
        object? param = null,
        CommandType commandType = default,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        string? traceKey = TraceKeys.ExecuteQueryMultiple,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
    {
        using var extractor = ExecuteQueryMultiple(
            connection,
            commandText,
            param,
            commandType,
            cacheKey,
            cacheItemExpiration,
            traceKey,
            commandTimeout,
            transaction,
            cache,
            trace);

        return (
            extractor.Extract<T1>(),
            extractor.Extract<T2>(),
            extractor.Extract<T3>(),
            extractor.Extract<T4>(),
            extractor.Extract<T5>(),
            extractor.Extract<T6>(),
            extractor.Extract<T7>()
            );
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="commandText"></param>
    /// <param name="param"></param>
    /// <param name="commandType"></param>
    /// <param name="cacheKey"></param>
    /// <param name="cacheItemExpiration"></param>
    /// <param name="commandTimeout"></param>
    /// <param name="traceKey"></param>
    /// <param name="transaction"></param>
    /// <param name="cache"></param>
    /// <param name="trace"></param>
    /// <param name="isDisposeConnection"></param>
    /// <returns></returns>
    internal static QueryMultipleExtractor ExecuteQueryMultipleInternal(this IDbConnection connection,
        string commandText,
        object? param = null,
        CommandType commandType = default,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        string? traceKey = TraceKeys.ExecuteQueryMultiple,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        bool isDisposeConnection = false)
    {
        IDataReader? reader = null;

        // Get Cache
        if (cacheKey == null || cache?.Contains(cacheKey) != true)
        {
            // Call
            reader = ExecuteReaderInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                traceKey: traceKey,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                entityType: null,
                dbFields: null,
                skipCommandArrayParametersCheck: false);
        }

        // Return
        return new QueryMultipleExtractor((DbConnection)connection,
            (DbDataReader?)reader,
            param,
            cacheKey,
            cacheItemExpiration,
            cache,
            isDisposeConnection);
    }

    #endregion

    #region ExecuteQueryMultipleAsync(Results)

    /// <summary>
    /// Execute the multiple SQL statements from the database in an asynchronous way.
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
    /// <returns>An instance of <see cref="QueryMultipleExtractor"/> used to extract the results.</returns>
    public static async Task<QueryMultipleExtractor> ExecuteQueryMultipleAsync(this IDbConnection connection,
        string commandText,
        object? param = null,
        CommandType commandType = default,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        string? traceKey = TraceKeys.ExecuteQueryMultiple,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        CancellationToken cancellationToken = default) =>
        await ExecuteQueryMultipleAsyncInternal(connection,
            commandText,
            param,
            commandType,
            cacheKey,
            cacheItemExpiration,
            traceKey,
            commandTimeout,
            transaction,
            cache,
            trace,
            false,
            cancellationToken).ConfigureAwait(false);

    #region Q
    public static async ValueTask<Tuple<IEnumerable<T1>, IEnumerable<T2>>> ExecuteQueryMultipleAsync<T1, T2>(
        this IDbConnection connection,
        string commandText,
        object? param = null,
        CommandType commandType = default,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        string? traceKey = TraceKeys.ExecuteQueryMultiple,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        CancellationToken cancellationToken = default)
        where T1 : class
        where T2 : class
    {
        await using var extractor = await ExecuteQueryMultipleAsync(
            connection,
            commandText,
            param,
            commandType,
            cacheKey,
            cacheItemExpiration,
            traceKey,
            commandTimeout,
            transaction,
            cache,
            trace,
            cancellationToken).ConfigureAwait(false);

        return new(
            await extractor.ExtractAsync<T1>(true, cancellationToken).ConfigureAwait(false),
            await extractor.ExtractAsync<T2>(true, cancellationToken).ConfigureAwait(false)
            );
    }

    public static async ValueTask<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>> ExecuteQueryMultipleAsync<T1, T2, T3>(
        this IDbConnection connection,
        string commandText,
        object? param = null,
        CommandType commandType = default,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        string? traceKey = TraceKeys.ExecuteQueryMultiple,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
    {
        await using var extractor = await ExecuteQueryMultipleAsync(
            connection,
            commandText,
            param,
            commandType,
            cacheKey,
            cacheItemExpiration,
            traceKey,
            commandTimeout,
            transaction,
            cache,
            trace,
            cancellationToken).ConfigureAwait(false);

        return new(
            await extractor.ExtractAsync<T1>(true, cancellationToken).ConfigureAwait(false),
            await extractor.ExtractAsync<T2>(true, cancellationToken).ConfigureAwait(false),
            await extractor.ExtractAsync<T3>(true, cancellationToken).ConfigureAwait(false)
            );
    }

    public static async ValueTask<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>> ExecuteQueryMultipleAsync<T1, T2, T3, T4>(
        this IDbConnection connection,
        string commandText,
        object? param = null,
        CommandType commandType = default,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        string? traceKey = TraceKeys.ExecuteQueryMultiple,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
    {
        await using var extractor = await ExecuteQueryMultipleAsync(
            connection,
            commandText,
            param,
            commandType,
            cacheKey,
            cacheItemExpiration,
            traceKey,
            commandTimeout,
            transaction,
            cache,
            trace,
            cancellationToken).ConfigureAwait(false);

        return new(
            await extractor.ExtractAsync<T1>(true, cancellationToken).ConfigureAwait(false),
            await extractor.ExtractAsync<T2>(true, cancellationToken).ConfigureAwait(false),
            await extractor.ExtractAsync<T3>(true, cancellationToken).ConfigureAwait(false),
            await extractor.ExtractAsync<T4>(true, cancellationToken).ConfigureAwait(false)
            );
    }

    public static async ValueTask<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>> ExecuteQueryMultiple<T1, T2, T3, T4, T5>(
        this IDbConnection connection,
        string commandText,
        object? param = null,
        CommandType commandType = default,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        string? traceKey = TraceKeys.ExecuteQueryMultiple,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
    {
        using var extractor = await ExecuteQueryMultipleAsync(
            connection,
            commandText,
            param,
            commandType,
            cacheKey,
            cacheItemExpiration,
            traceKey,
            commandTimeout,
            transaction,
            cache,
            trace,
            cancellationToken).ConfigureAwait(false);

        return new(
            await extractor.ExtractAsync<T1>(true, cancellationToken).ConfigureAwait(false),
            await extractor.ExtractAsync<T2>(true, cancellationToken).ConfigureAwait(false),
            await extractor.ExtractAsync<T3>(true, cancellationToken).ConfigureAwait(false),
            await extractor.ExtractAsync<T4>(true, cancellationToken).ConfigureAwait(false),
            await extractor.ExtractAsync<T5>(true, cancellationToken).ConfigureAwait(false)
            );
    }

    public static async ValueTask<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>> ExecuteQueryMultiple<T1, T2, T3, T4, T5, T6>(
        this IDbConnection connection,
        string commandText,
        object? param = null,
        CommandType commandType = default,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        string? traceKey = TraceKeys.ExecuteQueryMultiple,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
    {
        using var extractor = await ExecuteQueryMultipleAsync(
            connection,
            commandText,
            param,
            commandType,
            cacheKey,
            cacheItemExpiration,
            traceKey,
            commandTimeout,
            transaction,
            cache,
            trace,
            cancellationToken).ConfigureAwait(false);

        return new(
            await extractor.ExtractAsync<T1>(true, cancellationToken).ConfigureAwait(false),
            await extractor.ExtractAsync<T2>(true, cancellationToken).ConfigureAwait(false),
            await extractor.ExtractAsync<T3>(true, cancellationToken).ConfigureAwait(false),
            await extractor.ExtractAsync<T4>(true, cancellationToken).ConfigureAwait(false),
            await extractor.ExtractAsync<T5>(true, cancellationToken).ConfigureAwait(false),
            await extractor.ExtractAsync<T6>(true, cancellationToken).ConfigureAwait(false)
            );
    }

    public static async ValueTask<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>>> ExecuteQueryMultipleAsync<T1, T2, T3, T4, T5, T6, T7>(
        this IDbConnection connection,
        string commandText,
        object? param = null,
        CommandType commandType = default,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        string? traceKey = TraceKeys.ExecuteQueryMultiple,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
    {
        using var extractor = await ExecuteQueryMultipleAsync(
            connection,
            commandText,
            param,
            commandType,
            cacheKey,
            cacheItemExpiration,
            traceKey,
            commandTimeout,
            transaction,
            cache,
            trace,
            cancellationToken).ConfigureAwait(false);

        return new(
            await extractor.ExtractAsync<T1>(true, cancellationToken).ConfigureAwait(false),
            await extractor.ExtractAsync<T2>(true, cancellationToken).ConfigureAwait(false),
            await extractor.ExtractAsync<T3>(true, cancellationToken).ConfigureAwait(false),
            await extractor.ExtractAsync<T4>(true, cancellationToken).ConfigureAwait(false),
            await extractor.ExtractAsync<T5>(true, cancellationToken).ConfigureAwait(false),
            await extractor.ExtractAsync<T6>(true, cancellationToken).ConfigureAwait(false),
            await extractor.ExtractAsync<T7>(true, cancellationToken).ConfigureAwait(false)
            );
    }
    #endregion

    /// <summary>
    ///
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="commandText"></param>
    /// <param name="param"></param>
    /// <param name="commandType"></param>
    /// <param name="cacheKey"></param>
    /// <param name="cacheItemExpiration"></param>
    /// <param name="commandTimeout"></param>
    /// <param name="traceKey"></param>
    /// <param name="transaction"></param>
    /// <param name="cache"></param>
    /// <param name="trace"></param>
    /// <param name="isDisposeConnection"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal static async ValueTask<QueryMultipleExtractor> ExecuteQueryMultipleAsyncInternal(this IDbConnection connection,
        string commandText,
        object? param = null,
        CommandType commandType = default,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        string? traceKey = TraceKeys.ExecuteQueryMultiple,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        bool isDisposeConnection = false,
        CancellationToken cancellationToken = default)
    {
        DbDataReader? reader = null;

        // Get Cache
        if (cacheKey == null || cache?.Contains(cacheKey) != true)
        {
            // Call
            reader = await ExecuteReaderAsyncInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                traceKey: traceKey,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                cancellationToken: cancellationToken,
                entityType: null,
                dbFields: null,
                skipCommandArrayParametersCheck: false).ConfigureAwait(false);
        }

        // Return
        return new QueryMultipleExtractor((DbConnection)connection,
            (DbDataReader?)reader,
            param,
            cacheKey,
            cacheItemExpiration,
            cache,
            isDisposeConnection,
            cancellationToken);
    }

    #endregion

}
