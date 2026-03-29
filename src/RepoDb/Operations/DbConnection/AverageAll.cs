using System.Data;
using System.Linq.Expressions;
using RepoDb.Interfaces;

namespace RepoDb;

/// <averagemary>
/// Contains the extension methods for <see cref="IDbConnection"/> object.
/// </averagemary>
public static partial class DbConnectionExtension
{
    #region AverageAll<TEntity>

    /// <averagemary>
    /// Computes the average value of the target field.
    /// </averagemary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="field">The field to be averaged.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The average value of the target field.</returns>
    public static object? AverageAll<TEntity>(this IDbConnection connection,
        Field field,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.AverageAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return AverageInternal<TEntity, object>(connection: connection,
            where: null,
            field: field,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <averagemary>
    /// Computes the average value of the target field.
    /// </averagemary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="field">The field to be averaged.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The average value of the target field.</returns>
    public static object? AverageAll<TEntity>(this IDbConnection connection,
        Expression<Func<TEntity, object?>> field,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.AverageAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return AverageInternal<TEntity, object>(connection: connection,
            where: null,
            field: Field.Parse(field).First(),
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <averagemary>
    /// Computes the average value of the target field.
    /// </averagemary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="field">The field to be averaged.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The average value of the target field.</returns>
    public static TResult AverageAll<TEntity, TResult>(this IDbConnection connection,
        Field field,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.AverageAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return AverageInternal<TEntity, TResult>(connection: connection,
            where: null,
            field: field,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <averagemary>
    /// Computes the average value of the target field.
    /// </averagemary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="field">The field to be averaged.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The average value of the target field.</returns>
    public static TResult AverageAll<TEntity, TResult>(this IDbConnection connection,
        Expression<Func<TEntity, TResult?>> field,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.AverageAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(field);
        return AverageInternal<TEntity, TResult>(connection: connection,
            where: null,
            field: Field.Parse(field).First(),
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    #endregion

    #region AverageAllAsync<TEntity>

    /// <averagemary>
    /// Computes the average value of the target field in an asynchronous way.
    /// </averagemary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="field">The field to be averaged.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The average value of the target field.</returns>
    public static async Task<object?> AverageAllAsync<TEntity>(this IDbConnection connection,
        Field field,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.AverageAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await AverageInternalAsync<TEntity, object>(connection: connection,
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

    /// <averagemary>
    /// Computes the average value of the target field in an asynchronous way.
    /// </averagemary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="field">The field to be averaged.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The average value of the target field.</returns>
    public static async Task<object?> AverageAllAsync<TEntity>(this IDbConnection connection,
        Expression<Func<TEntity, object?>> field,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.AverageAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(field);
        return await AverageInternalAsync<TEntity, object>(connection: connection,
            where: null,
            field: Field.Parse(field).First(),
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <averagemary>
    /// Computes the average value of the target field in an asynchronous way.
    /// </averagemary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="field">The field to be averaged.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The average value of the target field.</returns>
    public static async Task<TResult> AverageAllAsync<TEntity, TResult>(this IDbConnection connection,
        Field field,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.AverageAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await AverageInternalAsync<TEntity, TResult>(connection: connection,
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

    /// <averagemary>
    /// Computes the average value of the target field in an asynchronous way.
    /// </averagemary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="field">The field to be averaged.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The average value of the target field.</returns>
    public static async Task<TResult> AverageAllAsync<TEntity, TResult>(this IDbConnection connection,
        Expression<Func<TEntity, TResult?>> field,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.AverageAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(field);
        return await AverageInternalAsync<TEntity, TResult>(connection: connection,
            where: null,
            field: Field.Parse(field).First(),
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #region AverageAll(TableName)

    /// <averagemary>
    /// Computes the average value of the target field.
    /// </averagemary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="field">The field to be averaged.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The average value of the target field.</returns>
    public static object? AverageAll(this IDbConnection connection,
        string tableName,
        Field field,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.AverageAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return AverageInternal<object>(connection: connection,
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

    /// <averagemary>
    /// Computes the average value of the target field.
    /// </averagemary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="field">The field to be averaged.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The average value of the target field.</returns>
    public static TResult AverageAll<TResult>(this IDbConnection connection,
        string tableName,
        Field field,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.AverageAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return AverageInternal<TResult>(connection: connection,
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

    #region AverageAllAsync(TableName)

    /// <averagemary>
    /// Computes the average value of the target field in an asynchronous way.
    /// </averagemary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="field">The field to be averaged.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The average value of the target field.</returns>
    public static async Task<object?> AverageAllAsync(this IDbConnection connection,
        string tableName,
        Field field,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.AverageAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await AverageInternalAsync<object>(connection: connection,
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

    /// <averagemary>
    /// Computes the average value of the target field in an asynchronous way.
    /// </averagemary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="field">The field to be averaged.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The average value of the target field.</returns>
    public static async Task<TResult> AverageAllAsync<TResult>(this IDbConnection connection,
        string tableName,
        Field field,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.AverageAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await AverageInternalAsync<TResult>(connection: connection,
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
