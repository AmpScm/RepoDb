using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using RepoDb.Contexts.Providers;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb;

/// <summary>
/// Contains the extension methods for <see cref="IDbConnection"/> object.
/// </summary>
public static partial class DbConnectionExtension
{
    #region Merge<TEntity>

    /// <summary>
    /// Inserts a new row or updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The object to be merged.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static object Merge<TEntity>(
        this IDbConnection connection,
        string tableName,
        TEntity entity,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return MergeInternal<TEntity, object>(connection: connection,
            tableName: tableName,
            entity: entity,
            qualifiers: null,
            fields: fields,
            noUpdateFields: null,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Inserts a new row or updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The object to be merged.</param>
    /// <param name="qualifier">The qualifier field to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static object Merge<TEntity>(
        this IDbConnection connection,
        string tableName,
        TEntity entity,
        Field qualifier,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return MergeInternal<TEntity, object>(connection: connection,
            tableName: tableName,
            entity: entity,
            qualifiers: qualifier?.AsEnumerable(),
            fields: fields,
            noUpdateFields: null,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Inserts a new row or updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The object to be merged.</param>
    /// <param name="qualifiers">The qualifier <see cref="Field"/> objects to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static object Merge<TEntity>(
        this IDbConnection connection,
        string tableName,
        TEntity entity,
        IEnumerable<Field>? qualifiers,
        IEnumerable<Field>? fields = null,
        IEnumerable<Field>? noUpdateFields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return MergeInternal<TEntity, object>(connection: connection,
            tableName: tableName,
            entity: entity,
            qualifiers: qualifiers,
            fields: fields,
            noUpdateFields: noUpdateFields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Inserts a new row or updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The object to be merged.</param>
    /// <param name="qualifiers">The expression for the qualifier fields.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static object Merge<TEntity>(
        this IDbConnection connection,
        string tableName,
        TEntity entity,
        Expression<Func<TEntity, object?>> qualifiers,
        IEnumerable<Field>? fields = null,
        IEnumerable<Field>? noUpdateFields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return MergeInternal<TEntity, object>(connection: connection,
            tableName: tableName,
            entity: entity,
            qualifiers: Field.Parse(qualifiers),
            fields: fields,
            noUpdateFields: noUpdateFields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Inserts a new row or updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The object to be merged.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static TResult Merge<TEntity, TResult>(
        this IDbConnection connection,
        string tableName,
        TEntity entity,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return MergeInternal<TEntity, TResult>(connection: connection,
            tableName: tableName,
            entity: entity,
            qualifiers: null,
            fields: fields,
            noUpdateFields: null,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Inserts a new row or updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The object to be merged.</param>
    /// <param name="qualifier">The qualifier field to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static TResult Merge<TEntity, TResult>(
        this IDbConnection connection,
        string tableName,
        TEntity entity,
        Field qualifier,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return MergeInternal<TEntity, TResult>(connection: connection,
            tableName: tableName,
            entity: entity,
            qualifiers: qualifier?.AsEnumerable(),
            fields: fields,
            noUpdateFields: null,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Inserts a new row or updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The object to be merged.</param>
    /// <param name="qualifiers">The qualifier <see cref="Field"/> objects to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static TResult Merge<TEntity, TResult>(
        this IDbConnection connection,
        string tableName,
        TEntity entity,
        IEnumerable<Field>? qualifiers,
        IEnumerable<Field>? fields = null,
        IEnumerable<Field>? noUpdateFields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return MergeInternal<TEntity, TResult>(connection: connection,
            tableName: tableName,
            entity: entity,
            qualifiers: qualifiers,
            fields: fields,
            noUpdateFields: noUpdateFields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Inserts a new row or updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The object to be merged.</param>
    /// <param name="qualifiers">The expression for the qualifier fields.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static TResult Merge<TEntity, TResult>(
        this IDbConnection connection,
        string tableName,
        TEntity entity,
        Expression<Func<TEntity, object?>> qualifiers,
        IEnumerable<Field>? fields = null,
        IEnumerable<Field>? noUpdateFields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return MergeInternal<TEntity, TResult>(connection: connection,
            tableName: tableName,
            entity: entity,
            qualifiers: Field.Parse(qualifiers),
            fields: fields,
            noUpdateFields: noUpdateFields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Inserts a new row or updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The object to be merged.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static object Merge<TEntity>(
        this IDbConnection connection,
        TEntity entity,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return MergeInternal<TEntity, object>(connection: connection,
            tableName: GetMappedName(entity),
            entity: entity,
            qualifiers: null,
            fields: fields,
            noUpdateFields: null,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Inserts a new row or updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The object to be merged.</param>
    /// <param name="qualifier">The qualifier field to be used during merge operation.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static object Merge<TEntity>(
        this IDbConnection connection,
        TEntity entity,
        Field qualifier,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return MergeInternal<TEntity, object>(connection: connection,
            tableName: GetMappedName(entity),
            entity: entity,
            qualifiers: qualifier?.AsEnumerable(),
            fields: fields,
            noUpdateFields: null,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Inserts a new row or updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The object to be merged.</param>
    /// <param name="qualifiers">The list of qualifier fields to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static object Merge<TEntity>(
        this IDbConnection connection,
        TEntity entity,
        IEnumerable<Field>? qualifiers,
        IEnumerable<Field>? fields = null,
        IEnumerable<Field>? noUpdateFields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return MergeInternal<TEntity, object>(connection: connection,
            tableName: GetMappedName(entity),
            entity: entity,
            qualifiers: qualifiers,
            fields: fields,
            noUpdateFields: noUpdateFields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Inserts a new row or updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The object to be merged.</param>
    /// <param name="qualifiers">The expression for the qualifier fields.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static object Merge<TEntity>(this IDbConnection connection,
        TEntity entity,
        Expression<Func<TEntity, object?>> qualifiers,
        IEnumerable<Field>? fields = null,
        IEnumerable<Field>? noUpdateFields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return MergeInternal<TEntity, object>(connection: connection,
            tableName: GetMappedName(entity),
            entity: entity,
            qualifiers: Field.Parse(qualifiers),
            fields: fields,
            noUpdateFields: noUpdateFields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Inserts a new row or updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The object to be merged.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static TResult Merge<TEntity, TResult>(this IDbConnection connection,
        TEntity entity,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return MergeInternal<TEntity, TResult>(connection: connection,
            tableName: GetMappedName(entity),
            entity: entity,
            qualifiers: null,
            fields: fields,
            noUpdateFields: null,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Inserts a new row or updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The object to be merged.</param>
    /// <param name="qualifier">The qualifier field to be used during merge operation.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static TResult Merge<TEntity, TResult>(this IDbConnection connection,
        TEntity entity,
        Field qualifier,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return MergeInternal<TEntity, TResult>(connection: connection,
            tableName: GetMappedName(entity),
            entity: entity,
            qualifiers: qualifier?.AsEnumerable(),
            fields: fields,
            noUpdateFields: null,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Inserts a new row or updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The object to be merged.</param>
    /// <param name="qualifiers">The list of qualifier fields to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static TResult Merge<TEntity, TResult>(this IDbConnection connection,
        TEntity entity,
        IEnumerable<Field>? qualifiers,
        IEnumerable<Field>? fields = null,
        IEnumerable<Field>? noUpdateFields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return MergeInternal<TEntity, TResult>(connection: connection,
            tableName: GetMappedName(entity),
            entity: entity,
            qualifiers: qualifiers,
            fields: fields,
            noUpdateFields: noUpdateFields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Inserts a new row or updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The object to be merged.</param>
    /// <param name="qualifiers">The expression for the qualifier fields.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static TResult Merge<TEntity, TResult>(this IDbConnection connection,
        TEntity entity,
        Expression<Func<TEntity, object?>> qualifiers,
        IEnumerable<Field>? fields = null,
        IEnumerable<Field>? noUpdateFields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return MergeInternal<TEntity, TResult>(connection: connection,
            tableName: GetMappedName(entity),
            entity: entity,
            qualifiers: Field.Parse(qualifiers),
            fields: fields,
            noUpdateFields: noUpdateFields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Inserts a new row or updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The object to be merged.</param>
    /// <param name="qualifiers">The list of qualifier fields to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    internal static TResult MergeInternal<TEntity, TResult>(
        this IDbConnection connection,
        string tableName,
        TEntity entity,
        IEnumerable<Field>? qualifiers,
        IEnumerable<Field>? fields,
        IEnumerable<Field>? noUpdateFields,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null, IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        // Return the result
        if (TypeCache.Get(GetEntityType(entity)).IsDictionaryStringObject)
        {
            return MergeInternalBase<IDictionary<string, object>, TResult>(connection: connection,
                tableName: tableName,
                entity: (IDictionary<string, object>)entity,
                qualifiers: qualifiers,
                fields: fields ?? GetQualifiedFields(entity),
                noUpdateFields: noUpdateFields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }
        else
        {
            return MergeInternalBase<TEntity, TResult>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: qualifiers,
                fields: fields ?? GetQualifiedFields(entity),
                noUpdateFields: noUpdateFields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }
    }

    #endregion

    #region MergeAsync<TEntity>

    /// <summary>
    /// Inserts a new row or updates an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The object to be merged.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static async Task<object> MergeAsync<TEntity>(
        this IDbConnection connection,
        string tableName,
        TEntity entity,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await MergeInternalAsync<TEntity, object>(
            connection: connection,
            tableName: tableName,
            entity: entity,
            qualifiers: null,
            fields: fields,
            noUpdateFields: null,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Inserts a new row or updates an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The object to be merged.</param>
    /// <param name="qualifier">The qualifier field to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static async Task<object> MergeAsync<TEntity>(
        this IDbConnection connection,
        string tableName,
        TEntity entity,
        Field qualifier,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await MergeInternalAsync<TEntity, object>(connection: connection,
            tableName: tableName,
            entity: entity,
            qualifiers: qualifier?.AsEnumerable(),
            fields: fields,
            noUpdateFields: null,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Inserts a new row or updates an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The object to be merged.</param>
    /// <param name="qualifiers">The qualifier <see cref="Field"/> objects to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static async Task<object> MergeAsync<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        IEnumerable<Field>? qualifiers,
        IEnumerable<Field>? fields = null,
        IEnumerable<Field>? noUpdateFields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await MergeInternalAsync<TEntity, object>(connection: connection,
            tableName: tableName,
            entity: entity,
            qualifiers: qualifiers,
            fields: fields,
            noUpdateFields: noUpdateFields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Inserts a new row or updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The object to be merged.</param>
    /// <param name="qualifiers">The expression for the qualifier fields.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static async Task<object> MergeAsync<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        Expression<Func<TEntity, object?>> qualifiers,
        IEnumerable<Field>? fields = null,
        IEnumerable<Field>? noUpdateFields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await MergeInternalAsync<TEntity, object>(connection: connection,
            tableName: tableName,
            entity: entity,
            qualifiers: Field.Parse(qualifiers),
            fields: fields,
            noUpdateFields: noUpdateFields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Inserts a new row or updates an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The object to be merged.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static async Task<TResult> MergeAsync<TEntity, TResult>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await MergeInternalAsync<TEntity, TResult>(connection: connection,
            tableName: tableName,
            entity: entity,
            qualifiers: null,
            fields: fields,
            noUpdateFields: null,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Inserts a new row or updates an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The object to be merged.</param>
    /// <param name="qualifier">The qualifier field to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static async Task<TResult> MergeAsync<TEntity, TResult>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        Field qualifier,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await MergeInternalAsync<TEntity, TResult>(connection: connection,
            tableName: tableName,
            entity: entity,
            qualifiers: qualifier?.AsEnumerable(),
            fields: fields,
            noUpdateFields: null,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Inserts a new row or updates an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The object to be merged.</param>
    /// <param name="qualifiers">The qualifier <see cref="Field"/> objects to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static async Task<TResult> MergeAsync<TEntity, TResult>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        IEnumerable<Field>? qualifiers,
        IEnumerable<Field>? fields = null,
        IEnumerable<Field>? noUpdateFields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await MergeInternalAsync<TEntity, TResult>(connection: connection,
            tableName: tableName,
            entity: entity,
            qualifiers: qualifiers,
            fields: fields,
            noUpdateFields: noUpdateFields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Inserts a new row or updates an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The object to be merged.</param>
    /// <param name="qualifiers">The expression for the qualifier fields.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static async Task<TResult> MergeAsync<TEntity, TResult>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        Expression<Func<TEntity, object?>> qualifiers,
        IEnumerable<Field>? fields = null,
        IEnumerable<Field>? noUpdateFields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await MergeInternalAsync<TEntity, TResult>(connection: connection,
            tableName: tableName,
            entity: entity,
            qualifiers: Field.Parse(qualifiers),
            fields: fields,
            noUpdateFields: noUpdateFields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Inserts a new row or updates an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The object to be merged.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static async Task<object> MergeAsync<TEntity>(this IDbConnection connection,
        TEntity entity,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await MergeInternalAsync<TEntity, object>(connection: connection,
            tableName: GetMappedName(entity),
            entity: entity,
            qualifiers: null,
            fields: fields,
            noUpdateFields: null,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Inserts a new row or updates an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The object to be merged.</param>
    /// <param name="qualifier">The field to be used during merge operation.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static async Task<object> MergeAsync<TEntity>(this IDbConnection connection,
        TEntity entity,
        Field qualifier,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await MergeInternalAsync<TEntity, object>(connection: connection,
            tableName: GetMappedName(entity),
            entity: entity,
            qualifiers: qualifier?.AsEnumerable(),
            fields: fields,
            noUpdateFields: null,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Inserts a new row or updates an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The object to be merged.</param>
    /// <param name="qualifiers">The list of qualifier fields to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static async Task<object> MergeAsync<TEntity>(this IDbConnection connection,
        TEntity entity,
        IEnumerable<Field>? qualifiers,
        IEnumerable<Field>? fields = null,
        IEnumerable<Field>? noUpdateFields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await MergeInternalAsync<TEntity, object>(connection: connection,
            tableName: GetMappedName(entity),
            entity: entity,
            qualifiers: qualifiers,
            fields: fields,
            noUpdateFields: noUpdateFields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Inserts a new row or updates an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The object to be merged.</param>
    /// <param name="qualifiers">The expression for the qualifier fields.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static async Task<object> MergeAsync<TEntity>(this IDbConnection connection,
        TEntity entity,
        Expression<Func<TEntity, object?>> qualifiers,
        IEnumerable<Field>? fields = null,
        IEnumerable<Field>? noUpdateFields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await MergeInternalAsync<TEntity, object>(connection: connection,
            tableName: GetMappedName(entity),
            entity: entity,
            qualifiers: Field.Parse(qualifiers),
            fields: fields,
            noUpdateFields: noUpdateFields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Inserts a new row or updates an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The object to be merged.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static async Task<TResult> MergeAsync<TEntity, TResult>(this IDbConnection connection,
        TEntity entity,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await MergeInternalAsync<TEntity, TResult>(connection: connection,
            tableName: GetMappedName(entity),
            entity: entity,
            qualifiers: null,
            fields: fields,
            noUpdateFields: null,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Inserts a new row or updates an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The object to be merged.</param>
    /// <param name="qualifier">The field to be used during merge operation.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static async Task<TResult> MergeAsync<TEntity, TResult>(this IDbConnection connection,
        TEntity entity,
        Field qualifier,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await MergeInternalAsync<TEntity, TResult>(connection: connection,
            tableName: GetMappedName(entity),
            entity: entity,
            qualifiers: qualifier?.AsEnumerable(),
            fields: fields,
            noUpdateFields: null,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Inserts a new row or updates an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The object to be merged.</param>
    /// <param name="qualifiers">The list of qualifier fields to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static async Task<TResult> MergeAsync<TEntity, TResult>(this IDbConnection connection,
        TEntity entity,
        IEnumerable<Field>? qualifiers,
        IEnumerable<Field>? fields = null,
        IEnumerable<Field>? noUpdateFields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await MergeInternalAsync<TEntity, TResult>(connection: connection,
            tableName: GetMappedName(entity),
            entity: entity,
            qualifiers: qualifiers,
            fields: fields,
            noUpdateFields: noUpdateFields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Inserts a new row or updates an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The object to be merged.</param>
    /// <param name="qualifiers">The expression for the qualifier fields.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static async Task<TResult> MergeAsync<TEntity, TResult>(this IDbConnection connection,
        TEntity entity,
        Expression<Func<TEntity, object?>> qualifiers,
        IEnumerable<Field>? fields = null,
        IEnumerable<Field>? noUpdateFields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await MergeInternalAsync<TEntity, TResult>(connection: connection,
            tableName: GetMappedName(entity),
            entity: entity,
            qualifiers: Field.Parse(qualifiers),
            fields: fields,
            noUpdateFields: noUpdateFields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Inserts a new row or updates an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The object to be merged.</param>
    /// <param name="qualifiers">The list of qualifier fields to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    internal static ValueTask<TResult> MergeInternalAsync<TEntity, TResult>(
        this IDbConnection connection,
        string tableName,
        TEntity entity,
        IEnumerable<Field>? qualifiers,
        IEnumerable<Field>? fields,
        IEnumerable<Field>? noUpdateFields,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null, CancellationToken cancellationToken = default)
        where TEntity : class
    {
        // Return the result
        if (TypeCache.Get(GetEntityType(entity)).IsDictionaryStringObject)
        {
            return MergeInternalBaseAsync<IDictionary<string, object>, TResult>(
                connection: connection,
                tableName: tableName,
                entity: (IDictionary<string, object>)entity,
                fields: fields ?? GetQualifiedFields(entity),
                noUpdateFields: noUpdateFields,
                qualifiers: qualifiers,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }
        else
        {
            return MergeInternalBaseAsync<TEntity, TResult>(
                connection: connection,
                tableName: tableName,
                entity: entity,
                fields: fields ?? GetQualifiedFields(entity),
                noUpdateFields: noUpdateFields,
                qualifiers: qualifiers,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }
    }

    #endregion

    #region Merge(TableName)

    /// <summary>
    /// Merges a dynamic object into an existing data in the database.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The dynamic object to be merged.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static object Merge(this IDbConnection connection,
        string tableName,
        object entity,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return MergeInternal<object, object>(connection: connection,
            tableName: tableName,
            entity: entity,
            qualifiers: null,
            fields: fields,
            noUpdateFields: null,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Merges a dynamic object into an existing data in the database.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The dynamic object to be merged.</param>
    /// <param name="qualifier">The qualifier field to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static object Merge(this IDbConnection connection,
        string tableName,
        object entity,
        Field qualifier,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return MergeInternal<object, object>(connection: connection,
            tableName: tableName,
            entity: entity,
            qualifiers: qualifier?.AsEnumerable(),
            fields: fields,
            noUpdateFields: null,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Merges a dynamic object into an existing data in the database.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The dynamic object to be merged.</param>
    /// <param name="qualifiers">The qualifier <see cref="Field"/> objects to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static object Merge(this IDbConnection connection,
        string tableName,
        object entity,
        IEnumerable<Field>? qualifiers,
        IEnumerable<Field>? fields = null,
        IEnumerable<Field>? noUpdateFields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return MergeInternal<object, object>(connection: connection,
            tableName: tableName,
            entity: entity,
            qualifiers: qualifiers,
            fields: fields,
            noUpdateFields: noUpdateFields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Merges a dynamic object into an existing data in the database.
    /// </summary>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The dynamic object to be merged.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static TResult Merge<TResult>(this IDbConnection connection,
        string tableName,
        object entity,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return MergeInternal<object, TResult>(connection: connection,
            tableName: tableName,
            entity: entity,
            qualifiers: null,
            fields: fields,
            noUpdateFields: null,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Merges a dynamic object into an existing data in the database.
    /// </summary>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The dynamic object to be merged.</param>
    /// <param name="qualifier">The qualifier field to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static TResult Merge<TResult>(this IDbConnection connection,
        string tableName,
        object entity,
        Field qualifier,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return MergeInternal<object, TResult>(connection: connection,
            tableName: tableName,
            entity: entity,
            qualifiers: qualifier?.AsEnumerable(),
            fields: fields,
            noUpdateFields: null,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Merges a dynamic object into an existing data in the database.
    /// </summary>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The dynamic object to be merged.</param>
    /// <param name="qualifiers">The qualifier <see cref="Field"/> objects to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static TResult Merge<TResult>(this IDbConnection connection,
        string tableName,
        object entity,
        IEnumerable<Field>? qualifiers,
        IEnumerable<Field>? fields = null,
        IEnumerable<Field>? noUpdateFields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return MergeInternal<object, TResult>(connection: connection,
            tableName: tableName,
            entity: entity,
            qualifiers: qualifiers,
            fields: fields,
            noUpdateFields: noUpdateFields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    #endregion

    #region MergeAsync(TableName)

    /// <summary>
    /// Merges a dynamic object into an existing data in the database in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The dynamic object to be merged.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static async Task<object> MergeAsync(this IDbConnection connection,
        string tableName,
        object entity,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await MergeInternalAsync<object, object>(connection: connection,
            tableName: tableName,
            entity: entity,
            qualifiers: null,
            fields: fields,
            noUpdateFields: null,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Merges a dynamic object into an existing data in the database in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The dynamic object to be merged.</param>
    /// <param name="qualifier">The qualifier field to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static async Task<object> MergeAsync(this IDbConnection connection,
        string tableName,
        object entity,
        Field qualifier,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await MergeInternalAsync<object, object>(connection: connection,
            tableName: tableName,
            entity: entity,
            qualifiers: qualifier?.AsEnumerable(),
            fields: fields,
            noUpdateFields: null,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Merges a dynamic object into an existing data in the database in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The dynamic object to be merged.</param>
    /// <param name="qualifiers">The qualifier <see cref="Field"/> objects to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static async Task<object> MergeAsync(this IDbConnection connection,
        string tableName,
        object entity,
        IEnumerable<Field>? qualifiers,
        IEnumerable<Field>? fields = null,
        IEnumerable<Field>? noUpdateFields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await MergeInternalAsync<object, object>(connection: connection,
            tableName: tableName,
            entity: entity,
            qualifiers: qualifiers,
            fields: fields,
            noUpdateFields: noUpdateFields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Merges a dynamic object into an existing data in the database in an asynchronous way.
    /// </summary>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The dynamic object to be merged.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static async Task<TResult> MergeAsync<TResult>(this IDbConnection connection,
        string tableName,
        object entity,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await MergeInternalAsync<object, TResult>(connection: connection,
            tableName: tableName,
            entity: entity,
            qualifiers: null,
            fields: fields,
            noUpdateFields: null,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Merges a dynamic object into an existing data in the database in an asynchronous way.
    /// </summary>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The dynamic object to be merged.</param>
    /// <param name="qualifier">The qualifier field to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static async Task<TResult> MergeAsync<TResult>(this IDbConnection connection,
        string tableName,
        object entity,
        Field qualifier,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await MergeInternalAsync<object, TResult>(connection: connection,
            tableName: tableName,
            entity: entity,
            qualifiers: qualifier?.AsEnumerable(),
            fields: fields,
            noUpdateFields: null,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Merges a dynamic object into an existing data in the database in an asynchronous way.
    /// </summary>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The dynamic object to be merged.</param>
    /// <param name="qualifiers">The qualifier <see cref="Field"/> objects to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static async Task<TResult> MergeAsync<TResult>(this IDbConnection connection,
        string tableName,
        object entity,
        IEnumerable<Field>? qualifiers,
        IEnumerable<Field>? fields = null,
        IEnumerable<Field>? noUpdateFields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await MergeInternalAsync<object, TResult>(connection: connection,
            tableName: tableName,
            entity: entity,
            qualifiers: qualifiers,
            fields: fields,
            noUpdateFields: noUpdateFields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #region MergeInternalBase<TEntity>

    /// <summary>
    /// Merges a data entity or dynamic object into an existing data in the database.
    /// </summary>
    /// <typeparam name="TEntity">The type of the object (whether a data entity or a dynamic).</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The data entity or dynamic object to be merged.</param>
    /// <param name="qualifiers">The list of qualifier fields to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    internal static TResult MergeInternalBase<TEntity, TResult>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        IEnumerable<Field>? qualifiers,
        IEnumerable<Field> fields,
        IEnumerable<Field>? noUpdateFields,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null, IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        // Check the qualifiers
        if (qualifiers?.Any() != true)
        {
            var keys = GetAndGuardPrimaryKeyOrIdentityKey(connection, tableName, transaction,
                entity.GetType());
            qualifiers = keys;
        }

        // Get the context
        var entityType = entity.GetType() ?? typeof(TEntity);
        var context = MergeExecutionContextProvider.Create(entityType,
            connection,
            tableName,
            qualifiers,
            fields,
            noUpdateFields,
            hints,
            transaction,
            statementBuilder);
        var result = default(TResult)!;

        // Create the command
        using (var command = (DbCommand)connection.EnsureOpen().CreateCommand(context.CommandText,
            CommandType.Text, commandTimeout, transaction))
        {
            // Set the values
            context.ParametersSetterFunc(command, entity);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            // Silent cancellation
            if (traceResult?.CancellableTraceLog?.IsCancelled == true)
            {
                return result;
            }

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                result = Converter.ToType<TResult>(reader.GetValue(0))!;
            }
            else if (DbFieldCache.Get(connection, tableName, transaction).GetKeyColumnReturn(GlobalConfiguration.Options.KeyColumnReturnBehavior) is { } returnField
                && PropertyCache.Get(entityType, returnField.FieldName) is { } pcv)
            {
                result = Converter.ToType<TResult>(pcv.PropertyInfo.GetValue(entity))!;
            }

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            // Set the return value
            if (result != null)
            {
                context.KeyPropertySetterFunc?.Invoke(entity, result);
            }
        }

        // Return the result
        return result;
    }

    #endregion

    #region MergeInternalBaseAsync<TEntity>

    /// <summary>
    /// Merges a data entity or dynamic object into an existing data in the database in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the object (whether a data entity or a dynamic).</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The data entity or dynamic object to be merged.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="qualifiers">The list of qualifier fields to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    internal static async ValueTask<TResult> MergeInternalBaseAsync<TEntity, TResult>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        IEnumerable<Field> fields,
        IEnumerable<Field>? noUpdateFields,
        IEnumerable<Field>? qualifiers = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null, CancellationToken cancellationToken = default)
        where TEntity : class
    {
        // Check the qualifiers
        if (qualifiers?.Any() != true)
        {
            var keys = await GetAndGuardPrimaryKeyOrIdentityKeyAsync(connection, tableName, transaction,
                entity.GetType() ?? typeof(TEntity), cancellationToken).ConfigureAwait(false);
            qualifiers = keys;
        }

        // Get the context
        var entityType = entity.GetType() ?? typeof(TEntity);
        var context = await MergeExecutionContextProvider.CreateAsync(entityType,
            connection,
            tableName,
            qualifiers,
            fields,
            noUpdateFields,
            hints,
            transaction,
            statementBuilder,
            cancellationToken).ConfigureAwait(false);
        var result = default(TResult)!;

        // Create the command
        using (var command = (DbCommand)(await connection.EnsureOpenAsync(cancellationToken).ConfigureAwait(false)).CreateCommand(context.CommandText,
            CommandType.Text, commandTimeout, transaction))
        {
            // Set the values
            context.ParametersSetterFunc(command, entity);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken).ConfigureAwait(false);

            // Silent cancellation
            if (traceResult?.CancellableTraceLog?.IsCancelled == true)
            {
                return result;
            }

            // Actual Execution
#if NET
            await
#endif
            using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

            if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                result = Converter.ToType<TResult>(reader.GetValue(0))!;
            }
            else if ((await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken).ConfigureAwait(false)).GetKeyColumnReturn(GlobalConfiguration.Options.KeyColumnReturnBehavior) is { } returnField
                && PropertyCache.Get(entityType, returnField.FieldName) is { } pcv)
            {
                result = Converter.ToType<TResult>(pcv.PropertyInfo.GetValue(entity))!;
            }

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken).ConfigureAwait(false);

            // Set the return value
            if (result != null)
            {
                context.KeyPropertySetterFunc?.Invoke(entity, result);
            }
        }

        // Return the result
        return result;
    }

    #endregion
}
