using System.Data;
using System.Data.Common;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Reflection;

namespace RepoDb;

/// <averagemary>
/// Contains the extension methods for <see cref="IDbConnection"/> object.
/// </averagemary>
public static partial class DbConnectionExtension
{
    #region ExecuteQuery

    /// <summary>
    /// Executes a SQL statement from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
    /// converts the result back to an enumerable list of dynamic objects.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="commandText">The command text to be used.</param>
    /// <param name="param">
    /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
    /// defined in the <see cref="IDbCommand.CommandText"/> property.
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
    /// <returns>
    /// An enumerable list of dynamic objects containing the converted results of the underlying <see cref="IDataReader"/> object.
    /// </returns>
    public static IEnumerable<dynamic> ExecuteQuery(this IDbConnection connection,
        string commandText,
        object? param = null,
        CommandType commandType = default,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        string? traceKey = TraceKeys.ExecuteQuery,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null)
    {
        return ExecuteQueryInternal(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            commandTimeout: commandTimeout,
            tableName: null,
            transaction: transaction,
            trace: trace,
            traceKey: traceKey,
            cache: cache,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            skipCommandArrayParametersCheck: false);
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
    /// <param name="tableName"></param>
    /// <param name="skipCommandArrayParametersCheck"></param>
    /// <returns></returns>
    internal static IEnumerable<dynamic> ExecuteQueryInternal(this IDbConnection connection,
        string commandText,
        object? param = null,
        CommandType commandType = default,
        int commandTimeout = 0,
        string? tableName = null,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        string? traceKey = TraceKeys.ExecuteQuery,
        ICache? cache = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        bool skipCommandArrayParametersCheck = true)
    {
        // Get Cache
        if (cache != null && cacheKey != null)
        {
            var item = cache.Get<IEnumerable<dynamic>>(cacheKey, false);
            if (item != null)
            {
                return item.Value;
            }
        }

        // DB Fields
        var dbFields = !string.IsNullOrWhiteSpace(tableName) ?
            DbFieldCache.Get(connection, tableName, transaction, false) : null;

        // Execute the actual method
        using var command = CreateDbCommandForExecution(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            commandTimeout: commandTimeout,
            transaction: transaction,
            entityType: null,
            dbFields: dbFields,
            skipCommandArrayParametersCheck: skipCommandArrayParametersCheck);

        // Variables
        IEnumerable<dynamic>? result = null;

        // Execute
        using (var reader = command.ExecuteReaderInternal(trace, traceKey))
        {
            result = DataReader.ToEnumerable(reader, dbFields).AsList();

            // Set Cache
            if (cache != null && cacheKey != null)
            {
                cache.Add(cacheKey, result, cacheItemExpiration, false);
            }
        }

        // Set the output parameters
        SetOutputParameters(param);

        // Return
        return result;
    }

    #endregion

    #region ExecuteQueryAsync

    /// <summary>
    /// Executes a SQL statement from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
    /// converts the result back to an enumerable list of dynamic objects.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="commandText">The command text to be used.</param>
    /// <param name="param">
    /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
    /// defined in the <see cref="IDbCommand.CommandText"/> property.
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
    /// <returns>
    /// An enumerable list of dynamic objects containing the converted results of the underlying <see cref="IDataReader"/> object.
    /// </returns>
    public static async Task<IEnumerable<dynamic>> ExecuteQueryAsync(this IDbConnection connection,
        string commandText,
        object? param = null,
        CommandType commandType = default,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        string? traceKey = TraceKeys.ExecuteQuery,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteQueryInternalAsync(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            tableName: null,
            commandTimeout: commandTimeout,
            transaction: transaction,
            trace: trace,
            traceKey: traceKey,
            cache: cache,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            skipCommandArrayParametersCheck: false,
            cancellationToken: cancellationToken).ConfigureAwait(false);
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
    /// <param name="cancellationToken"></param>
    /// <param name="tableName"></param>
    /// <param name="skipCommandArrayParametersCheck"></param>
    /// <returns></returns>
    internal static async ValueTask<IEnumerable<dynamic>> ExecuteQueryInternalAsync(this IDbConnection connection,
        string commandText,
        object? param = null,
        CommandType commandType = default,
        string? tableName = null,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        string? traceKey = TraceKeys.ExecuteQuery,
        ICache? cache = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        bool skipCommandArrayParametersCheck = true,
        CancellationToken cancellationToken = default)
    {
        // Get Cache
        if (cache != null && cacheKey != null)
        {
            var item = await cache.GetAsync<IEnumerable<dynamic>>(cacheKey, false, cancellationToken).ConfigureAwait(false);
            if (item != null)
            {
                return item.Value;
            }
        }

        // DB Fields
        var dbFields = !string.IsNullOrWhiteSpace(tableName) ?
            await DbFieldCache.GetAsync(connection, tableName, transaction, false, cancellationToken).ConfigureAwait(false) : null;

        // Execute the actual method
#if NET
        await
#endif
            using var command = await CreateDbCommandForExecutionAsync(connection: (DbConnection)connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            commandTimeout: commandTimeout,
            transaction: transaction,
            entityType: null,
            dbFields: dbFields,
            skipCommandArrayParametersCheck: skipCommandArrayParametersCheck,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        // Variables
        IEnumerable<dynamic>? result = null;

        // Execute
#if NET
        await
#endif
        using (var reader = await command.ExecuteReaderInternalAsync(trace, traceKey, cancellationToken).ConfigureAwait(false))
        {
            result = await DataReader.ToEnumerableAsync(reader, dbFields, cancellationToken)
                .ToListAsync(cancellationToken).ConfigureAwait(false);

            // Set Cache
            if (cache != null && cacheKey != null)
            {
                await cache.AddAsync(cacheKey, result, cacheItemExpiration, false, cancellationToken).ConfigureAwait(false);
            }
        }

        // Set the output parameters
        SetOutputParameters(param);

        // Return
        return result;
    }

    #endregion

    #region ExecuteQuery<TResult>

    /// <summary>
    /// Executes a SQL statement from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
    /// converts the result back to an enumerable list of the target result type.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
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
    /// <returns>
    /// An enumerable list of the target result type instances containing the converted results of the underlying <see cref="IDataReader"/> object.
    /// </returns>
    public static IEnumerable<TResult> ExecuteQuery<TResult>(this IDbConnection connection,
        string commandText,
        object? param = null,
        CommandType commandType = default,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        string? traceKey = TraceKeys.ExecuteQuery,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null)
    {
        return ExecuteQueryInternal<TResult>(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            traceKey: traceKey,
            commandTimeout: commandTimeout,
            transaction: transaction,
            cache: cache,
            trace: trace,
            tableName: ClassMappedNameCache.Get<TResult>(false),
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
    /// <param name="cacheKey"></param>
    /// <param name="cacheItemExpiration"></param>
    /// <param name="commandTimeout"></param>
    /// <param name="traceKey"></param>
    /// <param name="transaction"></param>
    /// <param name="cache"></param>
    /// <param name="trace"></param>
    /// <param name="tableName"></param>
    /// <param name="skipCommandArrayParametersCheck"></param>
    /// <returns></returns>
    internal static IEnumerable<TResult> ExecuteQueryInternal<TResult>(this IDbConnection connection,
        string commandText,
        object? param = null,
        CommandType commandType = default,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        string? traceKey = TraceKeys.ExecuteQuery,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        string? tableName = null,
        bool skipCommandArrayParametersCheck = true)
    {
        // Get Cache
        if (cache != null && cacheKey != null)
        {
            var item = cache.Get<IEnumerable<TResult>>(cacheKey, false);
            if (item != null)
            {
                return item.Value;
            }
        }

        // Variables
        var typeOfResult = typeof(TResult);

        // Identify
        if (TypeCache.Get(typeOfResult).IsDictionaryStringObject || typeOfResult.IsObjectType())
        {
            return ExecuteQueryInternalForDictionaryStringObject<TResult>(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                traceKey: traceKey,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                tableName: tableName,
                skipCommandArrayParametersCheck: skipCommandArrayParametersCheck);
        }
        else
        {
            return ExecuteQueryInternalForType<TResult>(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                traceKey: traceKey,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                tableName: tableName,
                skipCommandArrayParametersCheck: skipCommandArrayParametersCheck);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
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
    /// <param name="tableName"></param>
    /// <param name="skipCommandArrayParametersCheck"></param>
    /// <returns></returns>
    private static IEnumerable<TResult> ExecuteQueryInternalForDictionaryStringObject<TResult>(this IDbConnection connection,
        string commandText,
        object? param = null,
        CommandType commandType = default,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        string? traceKey = TraceKeys.ExecuteQuery,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        string? tableName = null,
        bool skipCommandArrayParametersCheck = true)
    {
        // Get Cache
        if (cache != null && cacheKey != null)
        {
            var item = cache.Get<IEnumerable<TResult>>(cacheKey, false);
            if (item != null)
            {
                return item.Value;
            }
        }

        // Call
        var result = ExecuteQueryInternal(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            commandTimeout: commandTimeout,
            transaction: transaction,
            trace: trace,
            traceKey: traceKey,
            cacheKey: null,
            cacheItemExpiration: cacheItemExpiration,
            cache: null,
            tableName: tableName,
            skipCommandArrayParametersCheck: skipCommandArrayParametersCheck).WithType<TResult>();

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

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
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
    /// <param name="tableName"></param>
    /// <param name="skipCommandArrayParametersCheck"></param>
    /// <returns></returns>
    private static IEnumerable<TResult> ExecuteQueryInternalForType<TResult>(this IDbConnection connection,
        string commandText,
        object? param = null,
        CommandType commandType = default,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        string? traceKey = TraceKeys.ExecuteQuery,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        string? tableName = null,
        bool skipCommandArrayParametersCheck = true)
    {
        // Get Cache
        if (cache != null && cacheKey != null)
        {
            var item = cache.Get<IEnumerable<TResult>>(cacheKey, false);
            if (item != null)
            {
                return item.Value;
            }
        }

        // DB Fields
        var dbFields = !string.IsNullOrWhiteSpace(tableName) ?
            DbFieldCache.Get(connection, tableName, transaction, false) : null;

        // Execute the actual method
        using var command = CreateDbCommandForExecution(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            commandTimeout: commandTimeout,
            transaction: transaction,
            entityType: typeof(TResult),
            dbFields: dbFields,
            skipCommandArrayParametersCheck: skipCommandArrayParametersCheck);

        // Variables
        IEnumerable<TResult>? result = null;

        // Execute
        using (var reader = command.ExecuteReaderInternal(trace, traceKey))
        {
            result = DataReader.ToEnumerable<TResult>(reader, dbFields).AsList();

            // Set Cache
            if (cache != null && cacheKey != null)
            {
                cache.Add(cacheKey, result, cacheItemExpiration, false);
            }
        }

        // Set the output parameters
        SetOutputParameters(param);

        // Return
        return result;
    }

    #endregion

    #region ExecuteQueryAsync<TResult>

    /// <summary>
    /// Executes a SQL statement from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
    /// converts the result back to an enumerable list of the target result type.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
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
    /// <returns>
    /// An enumerable list of the target result type instances containing the converted results of the underlying <see cref="IDataReader"/> object.
    /// </returns>
    public static async Task<IEnumerable<TResult>> ExecuteQueryAsync<TResult>(this IDbConnection connection,
        string commandText,
        object? param = null,
        CommandType commandType = default,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        string? traceKey = TraceKeys.ExecuteQuery,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteQueryInternalAsync<TResult>(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            cacheKey: cacheKey,
            cacheItemExpiration: cacheItemExpiration,
            traceKey: traceKey,
            commandTimeout: commandTimeout,
            transaction: transaction,
            cache: cache,
            trace: trace,
            tableName: ClassMappedNameCache.Get<TResult>(false),
            skipCommandArrayParametersCheck: false,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="connection"></param>
    /// <param name="commandText"></param>
    /// <param name="param"></param>
    /// <param name="commandType"></param>
    /// <param name="cacheKey"></param>
    /// <param name="cacheItemExpiration"></param>
    /// <param name="commandTimeout"></param>
    /// <param name="traceKey"></param>
    /// <param name="transaction"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="cache"></param>
    /// <param name="trace"></param>
    /// <param name="tableName"></param>
    /// <param name="skipCommandArrayParametersCheck"></param>
    /// <returns></returns>
    internal static async ValueTask<IEnumerable<TResult>> ExecuteQueryInternalAsync<TResult>(this IDbConnection connection,
        string commandText,
        object? param = null,
        CommandType commandType = default,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        string? traceKey = TraceKeys.ExecuteQuery,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        string? tableName = null,
        bool skipCommandArrayParametersCheck = true,
        CancellationToken cancellationToken = default)
    {
        // Get Cache
        if (cache != null && cacheKey != null)
        {
            var item = await cache.GetAsync<IEnumerable<TResult>>(cacheKey, false, cancellationToken).ConfigureAwait(false);
            if (item != null)
            {
                return item.Value;
            }
        }

        // Variables
        var typeOfResult = typeof(TResult);

        // Identify
        if (TypeCache.Get(typeOfResult).IsDictionaryStringObject || typeOfResult.IsObjectType())
        {
            return await ExecuteQueryInternalAsyncForDictionaryStringObject<TResult>(connection: connection,
               commandText: commandText,
               param: param,
               commandType: commandType,
               cacheKey: cacheKey,
               cacheItemExpiration: cacheItemExpiration,
               commandTimeout: commandTimeout,
               traceKey: traceKey,
               transaction: transaction,
               cache: cache,
               trace: trace,
               cancellationToken: cancellationToken,
               tableName: tableName,
               skipCommandArrayParametersCheck: skipCommandArrayParametersCheck).ConfigureAwait(false);
        }
        else
        {
            return await ExecuteQueryInternalAsyncForType<TResult>(connection: connection,
               commandText: commandText,
               param: param,
               commandType: commandType,
               cacheKey: cacheKey,
               cacheItemExpiration: cacheItemExpiration,
               commandTimeout: commandTimeout,
               traceKey: traceKey,
               transaction: transaction,
               cache: cache,
               trace: trace,
               cancellationToken: cancellationToken,
               tableName: tableName,
               skipCommandArrayParametersCheck: skipCommandArrayParametersCheck).ConfigureAwait(false);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="connection"></param>
    /// <param name="commandText"></param>
    /// <param name="param"></param>
    /// <param name="commandType"></param>
    /// <param name="cacheKey"></param>
    /// <param name="cacheItemExpiration"></param>
    /// <param name="commandTimeout"></param>
    /// <param name="traceKey"></param>
    /// <param name="transaction"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="cache"></param>
    /// <param name="trace"></param>
    /// <param name="tableName"></param>
    /// <param name="skipCommandArrayParametersCheck"></param>
    /// <returns></returns>
    private static async ValueTask<IEnumerable<TResult>> ExecuteQueryInternalAsyncForDictionaryStringObject<TResult>(this IDbConnection connection,
        string commandText,
        object? param = null,
        CommandType commandType = default,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        string? traceKey = TraceKeys.ExecuteQuery,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        string? tableName = null,
        bool skipCommandArrayParametersCheck = true,
        CancellationToken cancellationToken = default)
    {
        // Get Cache
        if (cache != null && cacheKey != null)
        {
            var item = await cache.GetAsync<IEnumerable<TResult>>(cacheKey, false, cancellationToken).ConfigureAwait(false);
            if (item != null)
            {
                return item.Value;
            }
        }

        // Call
        var result = (await ExecuteQueryInternalAsync(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            commandTimeout: commandTimeout,
            transaction: transaction,
            trace: trace,
            traceKey: traceKey,
            cacheKey: null,
            cacheItemExpiration: cacheItemExpiration,
            cache: null,
            cancellationToken: cancellationToken,
            tableName: tableName,
            skipCommandArrayParametersCheck: skipCommandArrayParametersCheck).ConfigureAwait(false)).WithType<TResult>();

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

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
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
    /// <param name="cancellationToken"></param>
    /// <param name="tableName"></param>
    /// <param name="skipCommandArrayParametersCheck"></param>
    /// <returns></returns>
    private static async ValueTask<IEnumerable<TResult>> ExecuteQueryInternalAsyncForType<TResult>(this IDbConnection connection,
        string commandText,
        object? param = null,
        CommandType commandType = default,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        string? traceKey = TraceKeys.ExecuteQuery,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        string? tableName = null,
        bool skipCommandArrayParametersCheck = true,
        CancellationToken cancellationToken = default)
    {
        // Get Cache
        if (cache != null && cacheKey != null)
        {
            var item = await cache.GetAsync<IEnumerable<TResult>>(cacheKey, false, cancellationToken).ConfigureAwait(false);
            if (item != null)
            {
                return item.Value;
            }
        }

        // DB Fields
        var dbFields = !string.IsNullOrWhiteSpace(tableName) ?
            await DbFieldCache.GetAsync(connection, tableName, transaction, false, cancellationToken).ConfigureAwait(false) : null;

        // Execute the actual method
#if NET
        await
#endif
            using var command = await CreateDbCommandForExecutionAsync(connection: (DbConnection)connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            commandTimeout: commandTimeout,
            transaction: transaction,
            entityType: typeof(TResult),
            dbFields: dbFields,
            skipCommandArrayParametersCheck: skipCommandArrayParametersCheck,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        IEnumerable<TResult>? result = null;

        // Execute
#if NET
        await
#endif
        using (var reader = await command.ExecuteReaderInternalAsync(trace, traceKey, cancellationToken).ConfigureAwait(false))
        {
            result = await DataReader.ToEnumerableAsync<TResult>(reader, dbFields, cancellationToken)
                .ToListAsync(cancellationToken).ConfigureAwait(false);

            // Set Cache
            if (cache != null && cacheKey != null)
            {
                await cache.AddAsync(cacheKey, result, cacheItemExpiration, false, cancellationToken).ConfigureAwait(false);
            }
        }

        // Set the output parameters
        SetOutputParameters(param);

        // Return
        return result;
    }

    #endregion
}
