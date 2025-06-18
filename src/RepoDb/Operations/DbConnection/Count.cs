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
    #region Count<TEntity>

    /// <summary>
    /// Count the number of rows from the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where">The dynamic expression to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An integer value that holds the number of rows from the table.</returns>
    public static long Count<TEntity>(this IDbConnection connection,
        object? where = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Count,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return CountInternal<TEntity>(connection: connection,
            where: ToQueryGroup(where),
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
    /// <param name="where">The query expression to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An integer value that holds the number of rows from the table.</returns>
    public static long Count<TEntity>(this IDbConnection connection,
        Expression<Func<TEntity, bool>>? where = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Count,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return CountInternal<TEntity>(connection: connection,
            where: connection.ToQueryGroup(where, transaction),
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
    /// <param name="where">The query expression to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An integer value that holds the number of rows from the table.</returns>
    public static long Count<TEntity>(this IDbConnection connection,
        QueryField? where = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Count,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return CountInternal<TEntity>(connection: connection,
            where: ToQueryGroup(where),
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
    /// <param name="where">The query expression to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An integer value that holds the number of rows from the table.</returns>
    public static long Count<TEntity>(this IDbConnection connection,
        IEnumerable<QueryField>? where = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Count,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return CountInternal<TEntity>(connection: connection,
            where: ToQueryGroup(where),
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
    /// <param name="where">The query expression to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An integer value that holds the number of rows from the table.</returns>
    public static long Count<TEntity>(this IDbConnection connection,
        QueryGroup? where = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Count,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return CountInternal<TEntity>(connection: connection,
            where: where,
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
    /// <param name="where">The query expression to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An integer value that holds the number of rows from the table.</returns>
    internal static long CountInternal<TEntity>(this IDbConnection connection,
        QueryGroup? where = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Count,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        // Variables
        var request = new CountRequest(typeof(TEntity),
            connection,
            transaction,
            where,
            hints,
            statementBuilder);
        object? param = null;

        // Converts to property mapped object
        if (where != null)
        {
            param = QueryGroup.AsMappedObject([where.MapTo<TEntity>()], connection, transaction, ClassMappedNameCache.Get<TEntity>());
        }

        // Return the result
        return CountInternalBase(connection: connection,
            request: request,
            param: param,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace);
    }

    #endregion

    #region CountAsync<TEntity>

    /// <summary>
    /// Count the number of rows from the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where">The dynamic expression to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An integer value that holds the number of rows from the table.</returns>
    public static async Task<long> CountAsync<TEntity>(this IDbConnection connection,
        object? where = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Count,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await CountAsyncInternal<TEntity>(connection: connection,
            where: ToQueryGroup(where),
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
    /// <param name="where">The query expression to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An integer value that holds the number of rows from the table.</returns>
    public static async Task<long> CountAsync<TEntity>(this IDbConnection connection,
        Expression<Func<TEntity, bool>>? where,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Count,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await CountAsyncInternal<TEntity>(connection: connection,
            where: connection.ToQueryGroup(where, transaction),
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
    /// <param name="where">The query expression to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An integer value that holds the number of rows from the table.</returns>
    public static async Task<long> CountAsync<TEntity>(this IDbConnection connection,
        QueryField? where,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Count,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await CountAsyncInternal<TEntity>(connection: connection,
            where: ToQueryGroup(where),
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
    /// <param name="where">The query expression to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An integer value that holds the number of rows from the table.</returns>
    public static async Task<long> CountAsync<TEntity>(this IDbConnection connection,
        IEnumerable<QueryField>? where,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Count,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await CountAsyncInternal<TEntity>(connection: connection,
            where: ToQueryGroup(where),
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
    /// <param name="where">The query expression to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An integer value that holds the number of rows from the table.</returns>
    public static async Task<long> CountAsync<TEntity>(this IDbConnection connection,
        QueryGroup? where,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Count,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await CountAsyncInternal<TEntity>(connection: connection,
            where: where,
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
    /// <param name="where">The query expression to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An integer value that holds the number of rows from the table.</returns>
    internal static ValueTask<long> CountAsyncInternal<TEntity>(this IDbConnection connection,
        QueryGroup? where,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Count,
        string? hints = null,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        // Variables
        var request = new CountRequest(typeof(TEntity),
            connection,
            transaction,
            where,
            hints,
            statementBuilder);
        object? param = null;

        // Converts to property mapped object
        if (where != null)
        {
            param = QueryGroup.AsMappedObject([where.MapTo<TEntity>()], connection, transaction, ClassMappedNameCache.Get<TEntity>());
        }

        // Return the result
        return CountAsyncInternalBase(connection: connection,
            request: request,
            param: param,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            cancellationToken: cancellationToken);
    }

    #endregion

    #region Count(TableName)

    /// <summary>
    /// Count the number of rows from the table.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="where">The dynamic expression to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An integer value that holds the number of rows from the table.</returns>
    public static long Count(this IDbConnection connection,
        string tableName,
        object? where = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Count,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return CountInternal(connection: connection,
            tableName: tableName,
            where: ToQueryGroup(where),
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
    /// <param name="where">The query expression to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An integer value that holds the number of rows from the table.</returns>
    public static long Count(this IDbConnection connection,
        string tableName,
        QueryField? where = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Count,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return CountInternal(connection: connection,
            tableName: tableName,
            where: ToQueryGroup(where),
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
    /// <param name="where">The query expression to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An integer value that holds the number of rows from the table.</returns>
    public static long Count(this IDbConnection connection,
        string tableName,
        IEnumerable<QueryField>? where = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Count,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return CountInternal(connection: connection,
            tableName: tableName,
            where: ToQueryGroup(where),
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
    /// <param name="where">The query expression to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An integer value that holds the number of rows from the table.</returns>
    public static long Count(this IDbConnection connection,
        string tableName,
        QueryGroup? where = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Count,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return CountInternal(connection: connection,
            tableName: tableName,
            where: where,
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
    /// <param name="where">The query expression to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An integer value that holds the number of rows from the table.</returns>
    internal static long CountInternal(this IDbConnection connection,
        string tableName,
        QueryGroup? where = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Count,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        // Variables
        var request = new CountRequest(tableName,
            connection,
            transaction,
            where,
            hints,
            statementBuilder);

        // Converts to property mapped object
        var param = (where != null) ? QueryGroup.AsMappedObject([where.MapTo(null)], connection, transaction, tableName) : null;

        // Return the result
        return CountInternalBase(connection: connection,
            request: request,
            param: param,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace);
    }

    #endregion

    #region CountAsync(TableName)

    /// <summary>
    /// Count the number of rows from the table in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="where">The dynamic expression to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An integer value that holds the number of rows from the table.</returns>
    public static async Task<long> CountAsync(this IDbConnection connection,
        string tableName,
        object? where = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Count,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await CountAsyncInternal(connection: connection,
            tableName: tableName,
            where: ToQueryGroup(where),
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
    /// <param name="where">The query expression to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An integer value that holds the number of rows from the table.</returns>
    public static async Task<long> CountAsync(this IDbConnection connection,
        string tableName,
        QueryField? where = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Count,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await CountAsyncInternal(connection: connection,
            tableName: tableName,
            where: ToQueryGroup(where),
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
    /// <param name="where">The query expression to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An integer value that holds the number of rows from the table.</returns>
    public static async Task<long> CountAsync(this IDbConnection connection,
        string tableName,
        IEnumerable<QueryField>? where = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Count,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await CountAsyncInternal(connection: connection,
            tableName: tableName,
            where: ToQueryGroup(where),
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
    /// <param name="where">The query expression to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An integer value that holds the number of rows from the table.</returns>
    public static async Task<long> CountAsync(this IDbConnection connection,
        string tableName,
        QueryGroup? where = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Count,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await CountAsyncInternal(connection: connection,
            tableName: tableName,
            where: where,
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
    /// <param name="where">The query expression to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An integer value that holds the number of rows from the table.</returns>
    internal static ValueTask<long> CountAsyncInternal(this IDbConnection connection,
        string tableName,
        QueryGroup? where = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Count,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        // Variables
        var request = new CountRequest(tableName,
            connection,
            transaction,
            where,
            hints,
            statementBuilder);

        // Converts to property mapped object
        var param = (where != null) ? QueryGroup.AsMappedObject([where.MapTo(null)], connection, transaction, tableName) : null;

        // Return the result
        return CountAsyncInternalBase(connection: connection,
            request: request,
            param: param,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            cancellationToken: cancellationToken);
    }

    #endregion

    #region CounterInternalBase

    /// <summary>
    /// Count the number of rows from the table.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="request">The actual <see cref="CountRequest"/> object.</param>
    /// <param name="param">The mapped object parameters.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <returns>An integer value that holds the number of rows from the table.</returns>
    internal static long CountInternalBase(this IDbConnection connection,
        CountRequest request,
        object? param,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Count,
        IDbTransaction? transaction = null,
        ITrace? trace = null)
    {
        // Variables
        var commandType = CommandType.Text;
        var commandText = CommandTextCache.GetCountText(request);

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

    #region CountAsyncInternalBase

    /// <summary>
    /// Count the number of rows from the table in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="request">The actual <see cref="CountRequest"/> object.</param>
    /// <param name="param">The mapped object parameters.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An integer value that holds the number of rows from the table.</returns>
    internal static async ValueTask<long> CountAsyncInternalBase(this IDbConnection connection,
        CountRequest request,
        object? param,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Count,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        CancellationToken cancellationToken = default)
    {
        // Variables
        var commandType = CommandType.Text;
        var commandText = CommandTextCache.GetCountText(request);

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
