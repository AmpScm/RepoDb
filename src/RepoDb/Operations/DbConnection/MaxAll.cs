using System.Data;
using System.Linq.Expressions;
using RepoDb.Interfaces;
using RepoDb.Requests;

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
        return MaxAllInternal<TEntity, object>(connection: connection,
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
        return MaxAllInternal<TEntity, object>(connection: connection,
            field: Field.Parse(field).First(),
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
        return MaxAllInternal<TEntity, TResult>(connection: connection,
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
        return MaxAllInternal<TEntity, TResult>(connection: connection,
            field: Field.Parse(field).First(),
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
    internal static TResult MaxAllInternal<TEntity, TResult>(this IDbConnection connection,
        Field field,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MaxAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        // Variables
        var request = new MaxAllRequest(typeof(TEntity),
            connection,
            transaction,
            field,
            hints,
            statementBuilder);
        object? param = null;

        // Return the result
        return MaxAllInternalBase<TResult>(connection: connection,
            request: request,
            param: param,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace);
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
        return await MaxAllAsyncInternal<TEntity, object>(connection: connection,
            field: field,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
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
        return await MaxAllAsyncInternal<TEntity, object>(connection: connection,
            field: Field.Parse(field).First(),
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
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
        return await MaxAllAsyncInternal<TEntity, TResult>(connection: connection,
            field: field,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
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
        return await MaxAllAsyncInternal<TEntity, TResult>(connection: connection,
            field: Field.Parse(field).First(),
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
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
    internal static ValueTask<TResult> MaxAllAsyncInternal<TEntity, TResult>(this IDbConnection connection,
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
        // Variables
        var request = new MaxAllRequest(typeof(TEntity),
            connection,
            transaction,
            field,
            hints,
            statementBuilder);
        object? param = null;

        // Return the result
        return MaxAllAsyncInternalBase<TResult>(connection: connection,
            request: request,
            param: param,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            cancellationToken: cancellationToken);
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
        return MaxAllInternal<object>(connection: connection,
            tableName: tableName,
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
        return MaxAllInternal<TResult>(connection: connection,
            tableName: tableName,
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
    internal static TResult MaxAllInternal<TResult>(this IDbConnection connection,
        string tableName,
        Field field,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MaxAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        // Variables
        var request = new MaxAllRequest(tableName,
            connection,
            transaction,
            field,
            hints,
            statementBuilder);

        // Return the result
        return MaxAllInternalBase<TResult>(connection: connection,
            request: request,
            param: null,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace);
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
        return await MaxAllAsyncInternal<object>(connection: connection,
            tableName: tableName,
            field: field,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
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
        return await MaxAllAsyncInternal<TResult>(connection: connection,
            tableName: tableName,
            field: field,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
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
    internal static ValueTask<TResult> MaxAllAsyncInternal<TResult>(this IDbConnection connection,
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
        // Variables
        var request = new MaxAllRequest(tableName,
            connection,
            transaction,
            field,
            hints,
            statementBuilder);

        // Return the result
        return MaxAllAsyncInternalBase<TResult>(connection: connection,
            request: request,
            param: null,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            cancellationToken: cancellationToken);
    }

    #endregion

    #region MaxAllInternalBase

    /// <summary>
    /// Computes the max value of the target field.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="request">The actual <see cref="MaxAllRequest"/> object.</param>
    /// <param name="param">The mapped object parameters.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <returns>The max value of the target field.</returns>
    internal static TResult MaxAllInternalBase<TResult>(this IDbConnection connection,
        MaxAllRequest request,
        object? param,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MaxAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null)
    {
        // Variables
        var commandType = CommandType.Text;
        var commandText = CommandTextCache.GetMaxAllText(request);

        // Actual Execution
        var result = ExecuteScalarInternal<TResult>(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            commandTimeout: commandTimeout,
            transaction: transaction,
            entityType: request.Type,
            dbFields: DbFieldCache.Get(connection, request.Name, transaction, true),
            trace: trace,
            traceKey: traceKey)!;

        // Result
        return result;
    }

    #endregion

    #region MaxAllAsyncInternalBase

    /// <summary>
    /// Computes the max value of the target field in an asynchronous way.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="request">The actual <see cref="MaxAllRequest"/> object.</param>
    /// <param name="param">The mapped object parameters.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The max value of the target field.</returns>
    internal static async ValueTask<TResult> MaxAllAsyncInternalBase<TResult>(this IDbConnection connection,
        MaxAllRequest request,
        object? param,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MaxAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        CancellationToken cancellationToken = default)
    {
        // Variables
        var commandType = CommandType.Text;
        var commandText = CommandTextCache.GetMaxAllText(request);

        // Actual Execution
        var result = await ExecuteScalarAsyncInternal<TResult>(connection: connection,
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
        return result!;
    }

    #endregion
}
