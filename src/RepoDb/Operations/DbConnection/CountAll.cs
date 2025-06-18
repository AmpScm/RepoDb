using System.Data;
using RepoDb.Interfaces;
using RepoDb.Requests;

namespace RepoDb;

/// <summary>
/// Contains the extension methods for <see cref="IDbConnection"/> object.
/// </summary>
public static partial class DbConnectionExtension
{
    #region CountAll<TEntity>

    /// <summary>
    /// Count the number of rows from the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An integer value that holds the number of rows from the table.</returns>
    public static long CountAll<TEntity>(this IDbConnection connection,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.CountAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return CountAllInternal<TEntity>(connection: connection,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Count the number of rows from the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An integer value that holds the number of rows from the table.</returns>
    internal static long CountAllInternal<TEntity>(this IDbConnection connection,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.CountAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        // Variables
        var request = new CountAllRequest(typeof(TEntity),
            connection,
            transaction,
            hints,
            statementBuilder);
        object? param = null;

        // Return the result
        return CountAllInternalBase(connection: connection,
            request: request,
            param: param,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace);
    }

    #endregion

    #region CountAllAsync<TEntity>

    /// <summary>
    /// Count the number of rows from the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An integer value that holds the number of rows from the table.</returns>
    public static async Task<long> CountAllAsync<TEntity>(this IDbConnection connection,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.CountAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await CountAllAsyncInternal<TEntity>(connection: connection,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Count the number of rows from the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An integer value that holds the number of rows from the table.</returns>
    internal static ValueTask<long> CountAllAsyncInternal<TEntity>(this IDbConnection connection,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.CountAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        // Variables
        var request = new CountAllRequest(typeof(TEntity),
            connection,
            transaction,
            hints,
            statementBuilder);
        object? param = null;

        // Return the result
        return CountAllAsyncInternalBase(connection: connection,
            request: request,
            param: param,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            cancellationToken: cancellationToken);
    }

    #endregion

    #region CountAll(TableName)

    /// <summary>
    /// Count the number of rows from the table.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An integer value that holds the number of rows from the table.</returns>
    public static long CountAll(this IDbConnection connection,
        string tableName,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.CountAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return CountAllInternal(connection: connection,
            tableName: tableName,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Count the number of rows from the table.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An integer value that holds the number of rows from the table.</returns>
    internal static long CountAllInternal(this IDbConnection connection,
        string tableName,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.CountAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        // Variables
        var request = new CountAllRequest(tableName,
            connection,
            transaction,
            hints,
            statementBuilder);
        object? param = null;

        // Return the result
        return CountAllInternalBase(connection: connection,
            request: request,
            param: param,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace);
    }

    #endregion

    #region CountAllAsync(TableName)

    /// <summary>
    /// Count the number of rows from the table in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An integer value that holds the number of rows from the table.</returns>
    public static async Task<long> CountAllAsync(this IDbConnection connection,
        string tableName,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.CountAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await CountAllAsyncInternal(connection: connection,
            tableName: tableName,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Count the number of rows from the table in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An integer value that holds the number of rows from the table.</returns>
    internal static ValueTask<long> CountAllAsyncInternal(this IDbConnection connection,
        string tableName,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.CountAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        // Variables
        var request = new CountAllRequest(tableName,
            connection,
            transaction,
            hints,
            statementBuilder);
        object? param = null;

        // Return the result
        return CountAllAsyncInternalBase(connection: connection,
            request: request,
            param: param,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            cancellationToken: cancellationToken);
    }

    #endregion

    #region CountAllInternalBase

    /// <summary>
    /// Count the number of rows from the table.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="request">The actual <see cref="CountAllRequest"/> object.</param>
    /// <param name="param">The mapped object parameters.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <returns>An integer value that holds the number of rows from the table.</returns>
    internal static long CountAllInternalBase(this IDbConnection connection,
        CountAllRequest request,
        object? param,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.CountAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null)
    {
        // Variables
        var commandType = CommandType.Text;
        var commandText = CommandTextCache.GetCountAllText(request);

        // Actual Execution
        var result = ExecuteScalarInternal<long>(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            commandTimeout: commandTimeout,
            transaction: transaction,
            entityType: request.Type,
            dbFields: DbFieldCache.Get(connection, request.Name, transaction, true),
            trace: trace,
            traceKey: traceKey);

        // Result
        return result;
    }

    #endregion

    #region CountAllAsyncInternalBase

    /// <summary>
    /// Count the number of rows from the table in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="request">The actual <see cref="CountAllRequest"/> object.</param>
    /// <param name="param">The mapped object parameters.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An integer value that holds the number of rows from the table.</returns>
    internal static async ValueTask<long> CountAllAsyncInternalBase(this IDbConnection connection,
        CountAllRequest request,
        object? param,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.CountAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        CancellationToken cancellationToken = default)
    {
        // Variables
        var commandType = CommandType.Text;
        var commandText = CommandTextCache.GetCountAllText(request);

        // Actual Execution
        var result = await ExecuteScalarAsyncInternal<long>(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            commandTimeout: commandTimeout,
            transaction: transaction,
            entityType: request.Type,
            dbFields: await DbFieldCache.GetAsync(connection, request.Name, transaction, true, cancellationToken).ConfigureAwait(false),
            trace: trace,
            traceKey: traceKey,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        // Result
        return result;
    }

    #endregion
}
