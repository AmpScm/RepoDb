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
    #region Exists<TEntity>

    /// <summary>
    /// Check whether the rows are existing in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
    public static bool Exists<TEntity>(this IDbConnection connection,
        object? what,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Exists,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return ExistsInternal<TEntity>(connection: connection,
            where: WhatToQueryGroup(typeof(TEntity), connection, what, transaction),
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Check whether the rows are existing in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to check the key of.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
    public static bool Exists<TEntity>(this IDbConnection connection,
        TEntity entity,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Exists,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        var key = GetAndGuardPrimaryKeyOrIdentityKey(GetEntityType(entity), connection, transaction);
        return ExistsInternal<TEntity>(connection: connection,
            where: ToQueryGroup(key, entity),
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Check whether the rows are existing in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
    public static bool Exists<TEntity, TWhat>(this IDbConnection connection,
        TWhat what,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Exists,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
        where TWhat : notnull
    {
        return ExistsInternal<TEntity>(connection: connection,
            where: WhatToQueryGroup(typeof(TEntity), connection, what, transaction),
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Check whether the rows are existing in the table.
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
    /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
    public static bool Exists<TEntity>(this IDbConnection connection,
        Expression<Func<TEntity, bool>> where,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Exists,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return ExistsInternal<TEntity>(connection: connection,
            where: connection.ToQueryGroup(where, transaction),
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Check whether the rows are existing in the table.
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
    /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
    public static bool Exists<TEntity>(this IDbConnection connection,
        QueryField where,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Exists,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return ExistsInternal<TEntity>(connection: connection,
            where: ToQueryGroup(where),
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Check whether the rows are existing in the table.
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
    /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
    public static bool Exists<TEntity>(this IDbConnection connection,
        IEnumerable<QueryField> where,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Exists,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return ExistsInternal<TEntity>(connection: connection,
            where: ToQueryGroup(where),
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Check whether the rows are existing in the table.
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
    /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
    public static bool Exists<TEntity>(this IDbConnection connection,
        QueryGroup where,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Exists,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return ExistsInternal<TEntity>(connection: connection,
            where: where,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Check whether the rows are existing in the table.
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
    /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
    internal static bool ExistsInternal<TEntity>(this IDbConnection connection,
        QueryGroup? where,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Exists,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        // Variables
        var request = new ExistsRequest(typeof(TEntity),
            connection,
            transaction,
            where,
            hints,
            statementBuilder);

        // Converts to property mapped object
        var param = (where != null) ? QueryGroup.AsMappedObject([where.MapTo<TEntity>()], connection, transaction, ClassMappedNameCache.Get<TEntity>()) : null;

        // Return the result
        return ExistsInternalBase(connection: connection,
            request: request,
            param: param,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace);
    }

    #endregion

    #region ExistsAsync<TEntity>

    /// <summary>
    /// Check whether the rows are existing in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
    public static async Task<bool> ExistsAsync<TEntity>(this IDbConnection connection,
        object what,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Exists,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await ExistsAsyncInternal<TEntity>(connection: connection,
            where: await WhatToQueryGroupAsync(typeof(TEntity), connection, what, transaction, cancellationToken).ConfigureAwait(false),
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Check whether the rows are existing in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
    public static async Task<bool> ExistsAsync<TEntity>(this IDbConnection connection,
        TEntity entity,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Exists,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        var key = await GetAndGuardPrimaryKeyOrIdentityKeyAsync(GetEntityType(entity), connection, transaction, cancellationToken);
        return await ExistsAsyncInternal<TEntity>(connection: connection,
            where: ToQueryGroup(key, entity),
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Check whether the rows are existing in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    public static async Task<bool> ExistsAsync<TEntity, TWhat>(this IDbConnection connection,
        TWhat what,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Exists,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
        where TWhat : notnull
    {
        return await ExistsAsyncInternal<TEntity>(connection: connection,
            where: await WhatToQueryGroupAsync(typeof(TEntity), connection, what, transaction, cancellationToken).ConfigureAwait(false),
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Check whether the rows are existing in the table in an asynchronous way.
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
    /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
    public static async Task<bool> ExistsAsync<TEntity>(this IDbConnection connection,
        Expression<Func<TEntity, bool>> where,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Exists,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await ExistsAsyncInternal<TEntity>(connection: connection,
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
    /// Check whether the rows are existing in the table in an asynchronous way.
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
    /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
    public static async Task<bool> ExistsAsync<TEntity>(this IDbConnection connection,
        QueryField where,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Exists,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await ExistsAsyncInternal<TEntity>(connection: connection,
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
    /// Check whether the rows are existing in the table in an asynchronous way.
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
    /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
    public static async Task<bool> ExistsAsync<TEntity>(this IDbConnection connection,
        IEnumerable<QueryField> where,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Exists,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await ExistsAsyncInternal<TEntity>(connection: connection,
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
    /// Check whether the rows are existing in the table in an asynchronous way.
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
    /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
    public static async Task<bool> ExistsAsync<TEntity>(this IDbConnection connection,
        QueryGroup where,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Exists,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await ExistsAsyncInternal<TEntity>(connection: connection,
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
    /// Check whether the rows are existing in the table in an asynchronous way.
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
    /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
    internal static ValueTask<bool> ExistsAsyncInternal<TEntity>(this IDbConnection connection,
        QueryGroup? where,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Exists,
        string? hints = null,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        // Variables
        var request = new ExistsRequest(typeof(TEntity),
            connection,
            transaction,
            where,
            hints,
            statementBuilder);

        // Converts to property mapped object
        var param = (where != null) ? QueryGroup.AsMappedObject([where.MapTo<TEntity>()], connection, transaction, ClassMappedNameCache.Get<TEntity>()) : null;

        // Return the result
        return ExistsAsyncInternalBase(connection: connection,
            request: request,
            param: param,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            cancellationToken: cancellationToken);
    }

    #endregion

    #region Exists(TableName)

    /// <summary>
    /// Check whether the rows are existing in the table.
    /// </summary>
    /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
    public static bool Exists<TWhat>(this IDbConnection connection,
        string tableName,
        TWhat what,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Exists,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TWhat : notnull
    {
        return ExistsInternal(connection: connection,
            tableName: tableName,
            where: WhatToQueryGroup(connection, tableName, what, transaction),
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Check whether the rows are existing in the table.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
    public static bool Exists(this IDbConnection connection,
        string tableName,
        object what,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Exists,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return ExistsInternal(connection: connection,
            tableName: tableName,
            where: WhatToQueryGroup(connection, tableName, what, transaction),
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Check whether the rows are existing in the table.
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
    /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
    public static bool Exists(this IDbConnection connection,
        string tableName,
        QueryField where,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Exists,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return ExistsInternal(connection: connection,
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
    /// Check whether the rows are existing in the table.
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
    /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
    public static bool Exists(this IDbConnection connection,
        string tableName,
        IEnumerable<QueryField> where,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Exists,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return ExistsInternal(connection: connection,
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
    /// Check whether the rows are existing in the table.
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
    /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
    public static bool Exists(this IDbConnection connection,
        string tableName,
        QueryGroup where,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Exists,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return ExistsInternal(connection: connection,
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
    /// Check whether the rows are existing in the table.
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
    /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
    internal static bool ExistsInternal(this IDbConnection connection,
        string tableName,
        QueryGroup? where,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Exists,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        // Variables
        var request = new ExistsRequest(tableName,
            connection,
            transaction,
            where,
            hints,
            statementBuilder);

        // Converts to property mapped object
        var param = (where != null) ? QueryGroup.AsMappedObject([where.MapTo(null)], connection, transaction, tableName) : null;

        // Return the result
        return ExistsInternalBase(connection: connection,
            request: request,
            param: param,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace);
    }

    #endregion

    #region ExistsAsync(TableName)

    /// <summary>
    /// Check whether the rows are existing in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
    public static async Task<bool> ExistsAsync<TWhat>(this IDbConnection connection,
        string tableName,
        TWhat what,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Exists,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TWhat : notnull
    {
        return await ExistsAsyncInternal(connection: connection,
            tableName: tableName,
            where: await WhatToQueryGroupAsync(connection, tableName, what, transaction, cancellationToken).ConfigureAwait(false),
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Check whether the rows are existing in the table in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
    public static async Task<bool> ExistsAsync(this IDbConnection connection,
        string tableName,
        object what,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Exists,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await ExistsAsyncInternal(connection: connection,
            tableName: tableName,
            where: await WhatToQueryGroupAsync(connection, tableName, what, transaction, cancellationToken).ConfigureAwait(false),
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Check whether the rows are existing in the table in an asynchronous way.
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
    /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
    public static async Task<bool> ExistsAsync(this IDbConnection connection,
        string tableName,
        QueryField where,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Exists,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await ExistsAsyncInternal(connection: connection,
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
    /// Check whether the rows are existing in the table in an asynchronous way.
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
    /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
    public static async Task<bool> ExistsAsync(this IDbConnection connection,
        string tableName,
        IEnumerable<QueryField> where,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Exists,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await ExistsAsyncInternal(connection: connection,
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
    /// Check whether the rows are existing in the table in an asynchronous way.
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
    /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
    public static async Task<bool> ExistsAsync(this IDbConnection connection,
        string tableName,
        QueryGroup where,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Exists,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await ExistsAsyncInternal(connection: connection,
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
    /// Check whether the rows are existing in the table in an asynchronous way.
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
    /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
    internal static ValueTask<bool> ExistsAsyncInternal(this IDbConnection connection,
        string tableName,
        QueryGroup? where,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Exists,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        // Variables
        var request = new ExistsRequest(tableName,
            connection,
            transaction,
            where,
            hints,
            statementBuilder);

        // Converts to property mapped object
        var param = (where != null) ? QueryGroup.AsMappedObject([where.MapTo(null)], connection, transaction, tableName) : null;

        // Return the result
        return ExistsAsyncInternalBase(connection: connection,
            request: request,
            param: param,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            cancellationToken: cancellationToken);
    }

    #endregion

    #region ExistsInternalBase

    /// <summary>
    /// Check whether the rows are existing in the table.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="request">The actual <see cref="ExistsRequest"/> object.</param>
    /// <param name="param">The mapped object parameters.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
    internal static bool ExistsInternalBase(this IDbConnection connection,
        ExistsRequest request,
        object? param,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Exists,
        IDbTransaction? transaction = null,
        ITrace? trace = null)
    {
        // Variables
        var commandType = CommandType.Text;
        var commandText = CommandTextCache.GetExistsText(request);

        // Actual Execution
        var result = ExecuteScalarInternal<bool>(connection: connection,
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

    #region ExistsAsyncInternalBase

    /// <summary>
    /// Check whether the rows are existing in the table in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="request">The actual <see cref="ExistsRequest"/> object.</param>
    /// <param name="param">The mapped object parameters.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
    internal static async ValueTask<bool> ExistsAsyncInternalBase(this IDbConnection connection,
        ExistsRequest request,
        object? param,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Exists,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        CancellationToken cancellationToken = default)
    {
        // Variables
        var commandType = CommandType.Text;
        var commandText = CommandTextCache.GetExistsText(request);

        // Actual Execution
        var result = await ExecuteScalarAsyncInternal<bool>(connection: connection,
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
