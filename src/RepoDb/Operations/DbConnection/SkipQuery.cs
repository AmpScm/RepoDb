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
    #region SkipQuery<TEntity>

    /// <summary>
    /// Query the rows from the database by batch.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> SkipQuery<TEntity>(this IDbConnection connection,
        string tableName,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return SkipQueryInternal<TEntity>(connection,
            tableName: tableName,
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            where: (QueryGroup?)null,
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the rows from the database by batch.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="where">The dynamic expression to be used.</param>
    /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> SkipQuery<TEntity>(this IDbConnection connection,
        string tableName,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        object? where = null,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return SkipQueryInternal<TEntity>(connection: connection,
            tableName: tableName,
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            where: ToQueryGroup(where),
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the rows from the database by batch.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> SkipQuery<TEntity>(this IDbConnection connection,
        string tableName,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        Expression<Func<TEntity, bool>>? where = null,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return SkipQueryInternal<TEntity>(connection: connection,
            tableName: tableName,
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            where: connection.ToQueryGroup(where, transaction, tableName),
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the rows from the database by batch.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> SkipQuery<TEntity>(this IDbConnection connection,
        string tableName,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        QueryField? where = null,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return SkipQueryInternal<TEntity>(connection: connection,
            tableName: tableName,
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            where: ToQueryGroup(where),
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the rows from the database by batch.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> SkipQuery<TEntity>(this IDbConnection connection,
        string tableName,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        IEnumerable<QueryField>? where = null,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return SkipQueryInternal<TEntity>(connection: connection,
            tableName: tableName,
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            where: ToQueryGroup(where),
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the rows from the database by batch.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> SkipQuery<TEntity>(this IDbConnection connection,
        string tableName,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        QueryGroup? where = null,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return SkipQueryInternal<TEntity>(connection: connection,
            tableName: tableName,
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            where: where,
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the rows from the database by batch.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="skip">The skip of the batch to be used. This is a zero-based index (the first skip is 0).</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> SkipQuery<TEntity>(this IDbConnection connection,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return SkipQueryInternal<TEntity>(connection,
            tableName: ClassMappedNameCache.Get<TEntity>() ?? throw new ArgumentException($"Can't map {typeof(TEntity)} to tablename"),
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            where: (QueryGroup?)null,
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the rows from the database by batch.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="where">The dynamic expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> SkipQuery<TEntity>(this IDbConnection connection,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        object? where = null,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return SkipQueryInternal<TEntity>(connection: connection,
            tableName: ClassMappedNameCache.Get<TEntity>() ?? throw new ArgumentException($"Can't map {typeof(TEntity)} to tablename"),
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            where: ToQueryGroup(where),
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the rows from the database by batch.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> SkipQuery<TEntity>(this IDbConnection connection,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        Expression<Func<TEntity, bool>>? where = null,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return SkipQueryInternal<TEntity>(connection: connection,
            tableName: ClassMappedNameCache.Get<TEntity>() ?? throw new ArgumentException($"Can't map {typeof(TEntity)} to tablename"),
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            where: connection.ToQueryGroup(where, transaction),
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the rows from the database by batch.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> SkipQuery<TEntity>(this IDbConnection connection,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        QueryField? where = null,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return SkipQueryInternal<TEntity>(connection: connection,
            tableName: ClassMappedNameCache.Get<TEntity>() ?? throw new ArgumentException($"Can't map {typeof(TEntity)} to tablename"),
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            where: ToQueryGroup(where),
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the rows from the database by batch.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> SkipQuery<TEntity>(this IDbConnection connection,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        IEnumerable<QueryField>? where = null,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return SkipQueryInternal<TEntity>(connection: connection,
            tableName: ClassMappedNameCache.Get<TEntity>() ?? throw new ArgumentException($"Can't map {typeof(TEntity)} to tablename"),
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            where: ToQueryGroup(where),
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the rows from the database by batch.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> SkipQuery<TEntity>(this IDbConnection connection,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        QueryGroup? where = null,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return SkipQueryInternal<TEntity>(connection: connection,
            tableName: ClassMappedNameCache.Get<TEntity>() ?? throw new ArgumentException($"Can't map {typeof(TEntity)} to tablename"),
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            where: where,
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the rows from the database by batch.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    internal static IEnumerable<TEntity> SkipQueryInternal<TEntity>(this IDbConnection connection,
        string tableName,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        QueryGroup? where = null,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        // Ensure the fields
        fields ??= GetQualifiedFields<TEntity>() ??
            DbFieldCache.Get(connection, tableName, transaction, true).AsFields();

        // Return
        return SkipQueryInternalBase<TEntity>(connection: connection,
            tableName: tableName,
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            where: where,
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    #endregion

    #region SkipQueryAsync<TEntity>

    /// <summary>
    /// Query the rows from the database by batch in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static async Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(this IDbConnection connection,
        string tableName,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await SkipQueryInternalAsync<TEntity>(connection,
            tableName: tableName,
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            where: (QueryGroup?)null,
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the rows from the database by batch in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="where">The dynamic expression to be used.</param>
    /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static async Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(this IDbConnection connection,
        string tableName,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        object? where = null,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await SkipQueryInternalAsync<TEntity>(connection: connection,
            tableName: tableName,
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            where: ToQueryGroup(where),
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the rows from the database by batch in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static async Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(this IDbConnection connection,
        string tableName,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        Expression<Func<TEntity, bool>>? where = null,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await SkipQueryInternalAsync<TEntity>(connection: connection,
            tableName: tableName,
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            where: connection.ToQueryGroup(where, transaction, tableName),
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the rows from the database by batch in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static async Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(this IDbConnection connection,
        string tableName,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        QueryField? where = null,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await SkipQueryInternalAsync<TEntity>(connection: connection,
            tableName: tableName,
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            where: ToQueryGroup(where),
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the rows from the database by batch in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static async Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(this IDbConnection connection,
        string tableName,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        IEnumerable<QueryField>? where = null,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await SkipQueryInternalAsync<TEntity>(connection: connection,
            tableName: tableName,
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            where: ToQueryGroup(where),
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the rows from the database by batch in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static async Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(this IDbConnection connection,
        string tableName,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        QueryGroup? where = null,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await SkipQueryInternalAsync<TEntity>(connection: connection,
            tableName: tableName,
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            where: where,
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the rows from the database by batch in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static async Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(this IDbConnection connection,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await SkipQueryInternalAsync<TEntity>(connection,
            tableName: ClassMappedNameCache.Get<TEntity>() ?? throw new ArgumentException($"Can't map {typeof(TEntity)} to tablename"),
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            where: (QueryGroup?)null,
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the rows from the database by batch in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="where">The dynamic expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static async Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(this IDbConnection connection,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        object? where = null,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await SkipQueryInternalAsync<TEntity>(connection: connection,
            tableName: ClassMappedNameCache.Get<TEntity>() ?? throw new ArgumentException($"Can't map {typeof(TEntity)} to tablename"),
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            where: ToQueryGroup(where),
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the rows from the database by batch in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static async Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(this IDbConnection connection,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        Expression<Func<TEntity, bool>>? where = null,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await SkipQueryInternalAsync<TEntity>(connection: connection,
            tableName: ClassMappedNameCache.Get<TEntity>() ?? throw new ArgumentException($"Can't map {typeof(TEntity)} to tablename"),
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            where: connection.ToQueryGroup(where, transaction),
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the rows from the database by batch in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static async Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(this IDbConnection connection,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        QueryField? where = null,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await SkipQueryInternalAsync<TEntity>(connection: connection,
            tableName: ClassMappedNameCache.Get<TEntity>() ?? throw new ArgumentException($"Can't map {typeof(TEntity)} to tablename"),
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            where: ToQueryGroup(where),
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the rows from the database by batch in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static async Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(this IDbConnection connection,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        IEnumerable<QueryField>? where = null,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await SkipQueryInternalAsync<TEntity>(connection: connection,
            tableName: ClassMappedNameCache.Get<TEntity>() ?? throw new ArgumentException($"Can't map {typeof(TEntity)} to tablename"),
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            where: ToQueryGroup(where),
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the rows from the database by batch in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static async Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(this IDbConnection connection,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        QueryGroup? where = null,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await SkipQueryInternalAsync<TEntity>(connection: connection,
            tableName: ClassMappedNameCache.Get<TEntity>() ?? throw new ArgumentException($"Can't map {typeof(TEntity)} to tablename"),
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            where: where,
            fields: fields,
            orderBy: orderBy,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the rows from the database by batch in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    internal static async ValueTask<IEnumerable<TEntity>> SkipQueryInternalAsync<TEntity>(this IDbConnection connection,
        string tableName,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        QueryGroup? where = null,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        // Ensure the fields
        fields ??= GetQualifiedFields<TEntity>() ??
            (await DbFieldCache.GetAsync(connection, tableName, transaction, true, cancellationToken).ConfigureAwait(false)).AsFields();

        // Return
        return await SkipQueryInternalBaseAsync<TEntity>(connection: connection,
            tableName: tableName,
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            where: where,
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #region SkipQuery(TableName)

    /// <summary>
    /// Query the rows from the database by batch.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of dynamic objects.</returns>
    public static IEnumerable<dynamic> SkipQuery(this IDbConnection connection,
        string tableName,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return SkipQueryInternal<dynamic>(connection,
            tableName: tableName,
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            fields: fields,
            where: (QueryGroup?)null,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the rows from the database by batch.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="where">The dynamic expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of dynamic objects.</returns>
    public static IEnumerable<dynamic> SkipQuery(this IDbConnection connection,
        string tableName,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        object? where = null,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return SkipQueryInternal<dynamic>(connection: connection,
            tableName: tableName,
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            where: ToQueryGroup(where),
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the rows from the database by batch.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of dynamic objects.</returns>
    public static IEnumerable<dynamic> SkipQuery(this IDbConnection connection,
        string tableName,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        QueryField? where = null,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return SkipQueryInternal<dynamic>(connection: connection,
            tableName: tableName,
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            where: ToQueryGroup(where),
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the rows from the database by batch.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of dynamic objects.</returns>
    public static IEnumerable<dynamic> SkipQuery(this IDbConnection connection,
        string tableName,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        IEnumerable<QueryField>? where = null,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return SkipQueryInternal<dynamic>(connection: connection,
            tableName: tableName,
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            where: ToQueryGroup(where),
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Query the rows from the database by batch.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of dynamic objects.</returns>
    public static IEnumerable<dynamic> SkipQuery(this IDbConnection connection,
        string tableName,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        QueryGroup? where = null,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return SkipQueryInternal<dynamic>(connection: connection,
            tableName: tableName,
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            where: where,
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    #endregion

    #region SkipQueryAsync(TableName)

    /// <summary>
    /// Query the rows from the database by batch in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of dynamic objects.</returns>
    public static async Task<IEnumerable<dynamic>> SkipQueryAsync(this IDbConnection connection,
        string tableName,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await SkipQueryInternalAsync<dynamic>(connection,
            tableName: tableName,
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            fields: fields,
            where: (QueryGroup?)null,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the rows from the database by batch in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="where">The dynamic expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of dynamic objects.</returns>
    public static async Task<IEnumerable<dynamic>> SkipQueryAsync(this IDbConnection connection,
        string tableName,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        object? where = null,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await SkipQueryInternalAsync<dynamic>(connection: connection,
            tableName: tableName,
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            where: ToQueryGroup(where),
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the rows from the database by batch in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of dynamic objects.</returns>
    public static async Task<IEnumerable<dynamic>> SkipQueryAsync(this IDbConnection connection,
        string tableName,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        QueryField? where = null,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await SkipQueryInternalAsync<dynamic>(connection: connection,
            tableName: tableName,
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            where: ToQueryGroup(where),
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the rows from the database by batch in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of dynamic objects.</returns>
    public static async Task<IEnumerable<dynamic>> SkipQueryAsync(this IDbConnection connection,
        string tableName,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        IEnumerable<QueryField>? where = null,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await SkipQueryInternalAsync<dynamic>(connection: connection,
            tableName: tableName,
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            where: ToQueryGroup(where),
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the rows from the database by batch in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of dynamic objects.</returns>
    public static async Task<IEnumerable<dynamic>> SkipQueryAsync(this IDbConnection connection,
        string tableName,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        QueryGroup? where = null,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await SkipQueryInternalAsync<dynamic>(connection: connection,
            tableName: tableName,
            skip: skip,
            rowsPerBatch: rowsPerBatch,
            orderBy: orderBy,
            where: where,
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
    }

    #endregion

    #region SkipQueryInternalBase<TEntity>

    /// <summary>
    /// Query the rows from the database by batch.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    internal static IEnumerable<TEntity> SkipQueryInternalBase<TEntity>(this IDbConnection connection,
        string tableName,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        QueryGroup? where,
        IEnumerable<Field> fields,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        // Variables
        var commandType = CommandType.Text;
        var request = new SkipQueryRequest(tableName,
            connection,
            transaction,
            fields,
            skip,
            rowsPerBatch,
            orderBy,
            where,
            hints,
            statementBuilder);

        // Converts to property mapped object
        var param = (where != null) ? QueryGroup.AsMappedObject([where.MapTo<TEntity>()], connection, transaction, tableName) : null;

        var commandText = CommandTextCache.GetSkipQueryText(request);

        // Actual Execution
        var result = ExecuteQueryInternal<TEntity>(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            cacheKey: null,
            cacheItemExpiration: Constant.DefaultCacheItemExpirationInMinutes,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: null,
            trace: trace,
            tableName: tableName,
            skipCommandArrayParametersCheck: true);

        // Result
        return result;
    }

    #endregion

    #region SkipQueryInternalBaseAsync<TEntity>

    /// <summary>
    /// Query the rows from the database by batch in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="skip">The skip of the batch to be used. This is a zero-based index (the first skip is 0).</param>
    /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
    /// <param name="orderBy">The order definition of the fields to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    internal static async ValueTask<IEnumerable<TEntity>> SkipQueryInternalBaseAsync<TEntity>(this IDbConnection connection,
        string tableName,
        int skip,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy,
        QueryGroup? where,
        IEnumerable<Field> fields,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.SkipQuery,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        // Variables
        var commandType = CommandType.Text;
        var request = new SkipQueryRequest(tableName,
            connection,
            transaction,
            fields,
            skip,
            rowsPerBatch,
            orderBy,
            where,
            hints,
            statementBuilder);

        // Converts to property mapped object
        var param = (where != null) ? await QueryGroup.AsMappedObjectAsync([where.MapTo<TEntity>()], connection, transaction, tableName, cancellationToken) : null;

        var commandText = await CommandTextCache.GetSkipQueryTextAsync(request, cancellationToken).ConfigureAwait(false);


        // Actual Execution
        var result = await ExecuteQueryInternalAsync<TEntity>(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            cacheKey: null,
            cacheItemExpiration: Constant.DefaultCacheItemExpirationInMinutes,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            cache: null,
            trace: trace,
            cancellationToken: cancellationToken,
            tableName: tableName,
            skipCommandArrayParametersCheck: true).ConfigureAwait(false);

        // Result
        return result;
    }

    #endregion
}
