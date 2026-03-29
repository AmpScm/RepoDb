using System.Data;
using System.Linq.Expressions;
using RepoDb.Interfaces;

namespace RepoDb;

/// <summary>
/// Contains the extension methods for <see cref="IDbConnection"/> object.
/// </summary>
public static partial class DbConnectionExtension
{
    #region MaxAll<TEntity>

    /// <summary>
    /// Computes the max value of the target field.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="field">The field to be maximized.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The max value of the target field.</returns>
    public static object? MaxAll<TEntity>(this IDbConnection connection,
        Field field,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MaxAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return MaxInternal<TEntity, object>(connection: connection,
            field: field,
            where: null,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Computes the max value of the target field.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="field">The field to be maximized.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The max value of the target field.</returns>
    public static object? MaxAll<TEntity>(this IDbConnection connection,
        Expression<Func<TEntity, object?>> field,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MaxAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return MaxInternal<TEntity, object>(connection: connection,
            field: Field.Parse(field).First(),
            where: null,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Computes the max value of the target field.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="field">The field to be maximized.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The max value of the target field.</returns>
    public static TResult MaxAll<TEntity, TResult>(this IDbConnection connection,
        Field field,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MaxAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return MaxInternal<TEntity, TResult>(connection: connection,
            field: field,
            where: null,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Computes the max value of the target field.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="field">The field to be maximized.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The max value of the target field.</returns>
    public static TResult MaxAll<TEntity, TResult>(this IDbConnection connection,
        Expression<Func<TEntity, TResult?>> field,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MaxAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(field);
        return MaxInternal<TEntity, TResult>(connection: connection,
            field: Field.Parse(field).First(),
            where: null,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    #endregion

    #region MaxAllAsync<TEntity>

    /// <summary>
    /// Computes the max value of the target field in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="field">The field to be maximized.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The max value of the target field.</returns>
    public static async Task<object?> MaxAllAsync<TEntity>(this IDbConnection connection,
        Field field,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MaxAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await MaxInternalAsync<TEntity, object>(connection: connection,
            field: field,
            where: null,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Computes the max value of the target field in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="field">The field to be maximized.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The max value of the target field.</returns>
    public static async Task<object?> MaxAllAsync<TEntity>(this IDbConnection connection,
        Expression<Func<TEntity, object?>> field,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MaxAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await MaxInternalAsync<TEntity, object>(connection: connection,
            field: Field.Parse(field).First(),
            where: null,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Computes the max value of the target field in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="field">The field to be maximized.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The max value of the target field.</returns>
    public static async Task<TResult> MaxAllAsync<TEntity, TResult>(this IDbConnection connection,
        Field field,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MaxAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await MaxInternalAsync<TEntity, TResult>(connection: connection,
            field: field,
            where: null,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Computes the max value of the target field in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="field">The field to be maximized.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The max value of the target field.</returns>
    public static async Task<TResult> MaxAllAsync<TEntity, TResult>(this IDbConnection connection,
        Expression<Func<TEntity, TResult?>> field,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MaxAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(field);
        return await MaxInternalAsync<TEntity, TResult>(connection: connection,
            field: Field.Parse(field).First(),
            where: null,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #region MaxAll(TableName)

    /// <summary>
    /// Computes the max value of the target field.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="field">The field to be maximized.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The max value of the target field.</returns>
    public static object? MaxAll(this IDbConnection connection,
        string tableName,
        Field field,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MaxAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return MaxInternal<object>(connection: connection,
            tableName: tableName,
            where: null,
            field: field,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Computes the max value of the target field.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="field">The field to be maximized.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The max value of the target field.</returns>
    public static TResult MaxAll<TResult>(this IDbConnection connection,
        string tableName,
        Field field,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MaxAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return MaxInternal<TResult>(connection: connection,
            tableName: tableName,
            where: null,
            field: field,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }
    #endregion

    #region MaxAllAsync(TableName)

    /// <summary>
    /// Computes the max value of the target field in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="field">The field to be maximized.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The max value of the target field.</returns>
    public static async Task<object?> MaxAllAsync(this IDbConnection connection,
        string tableName,
        Field field,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MaxAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await MaxInternalAsync<object>(connection: connection,
            tableName: tableName,
            where: null,
            field: field,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Computes the max value of the target field in an asynchronous way.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="field">The field to be maximized.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The max value of the target field.</returns>
    public static async Task<TResult> MaxAllAsync<TResult>(this IDbConnection connection,
        string tableName,
        Field field,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MaxAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await MaxInternalAsync<TResult>(connection: connection,
            tableName: tableName,
            where: null,
            field: field,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
    #endregion
}
