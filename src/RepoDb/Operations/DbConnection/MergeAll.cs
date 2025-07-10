using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Transactions;
using RepoDb.Contexts.Providers;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.StatementBuilders;

namespace RepoDb;

/// <summary>
/// Contains the extension methods for <see cref="IDbConnection"/> object.
/// </summary>
public static partial class DbConnectionExtension
{
    #region MergeAll<TEntity>

    /// <summary>
    /// Insert multiple rows or update the existing rows in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of data entity objects to be merged.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the merge process.</returns>
    public static int MergeAll<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MergeAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return MergeAllInternal(connection: connection,
            tableName: tableName,
            entities: entities,
            qualifiers: null,
            noUpdateFields: null,
            batchSize: batchSize,
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Insert multiple rows or update the existing rows in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of data entity objects to be merged.</param>
    /// <param name="qualifier">The qualifier field to be used during merge operation.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the merge process.</returns>
    public static int MergeAll<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        Field qualifier,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MergeAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return MergeAllInternal(connection: connection,
            tableName: tableName,
            entities: entities,
            qualifiers: qualifier.AsEnumerable(),
            noUpdateFields: null,
            batchSize: batchSize,
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Insert multiple rows or update the existing rows in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of data entity objects to be merged.</param>
    /// <param name="qualifiers">The list of qualifier fields to be used.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the merge process.</returns>
    public static int MergeAll<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        IEnumerable<Field>? qualifiers,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        IEnumerable<Field>? noUpdateFields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MergeAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return MergeAllInternal(connection: connection,
            tableName: tableName,
            entities: entities,
            qualifiers: qualifiers,
            noUpdateFields: noUpdateFields,
            batchSize: batchSize,
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Insert multiple rows or update the existing rows in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of data entity objects to be merged.</param>
    /// <param name="qualifiers">The expression for the qualifier fields.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the merge process.</returns>
    public static int MergeAll<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        Expression<Func<TEntity, object?>> qualifiers,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        IEnumerable<Field>? noUpdateFields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MergeAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return MergeAllInternal(connection: connection,
            tableName: tableName,
            entities: entities,
            qualifiers: Field.Parse(qualifiers),
            noUpdateFields: noUpdateFields,
            batchSize: batchSize,
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Insert multiple rows or update the existing rows in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entities">The list of data entity objects to be merged.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the merge process.</returns>
    public static int MergeAll<TEntity>(this IDbConnection connection,
        IEnumerable<TEntity> entities,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MergeAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return MergeAllInternal(connection: connection,
            tableName: GetMappedName(entities),
            entities: entities,
            qualifiers: null,
            noUpdateFields: null,
            batchSize: batchSize,
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Insert multiple rows or update the existing rows in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entities">The list of data entity objects to be merged.</param>
    /// <param name="qualifier">The qualifier field to be used during merge operation.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the merge process.</returns>
    public static int MergeAll<TEntity>(this IDbConnection connection,
        IEnumerable<TEntity> entities,
        Field qualifier,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MergeAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return MergeAllInternal(connection: connection,
            tableName: GetMappedName(entities),
            entities: entities,
            qualifiers: qualifier.AsEnumerable(),
            noUpdateFields: null,
            batchSize: batchSize,
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder); ;
    }

    /// <summary>
    /// Insert multiple rows or update the existing rows in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entities">The list of data entity objects to be merged.</param>
    /// <param name="qualifiers">The list of qualifier fields to be used.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the merge process.</returns>
    public static int MergeAll<TEntity>(this IDbConnection connection,
        IEnumerable<TEntity> entities,
        IEnumerable<Field>? qualifiers,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        IEnumerable<Field>? noUpdateFields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MergeAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return MergeAllInternal(connection: connection,
            tableName: GetMappedName(entities),
            entities: entities,
            qualifiers: qualifiers,
            noUpdateFields: noUpdateFields,
            batchSize: batchSize,
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Insert multiple rows or update the existing rows in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entities">The list of data entity objects to be merged.</param>
    /// <param name="qualifiers">The expression for the qualifier fields.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the merge process.</returns>
    public static int MergeAll<TEntity>(this IDbConnection connection,
        IEnumerable<TEntity> entities,
        Expression<Func<TEntity, object?>> qualifiers,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        IEnumerable<Field>? noUpdateFields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MergeAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return MergeAllInternal(connection: connection,
            tableName: GetMappedName(entities),
            entities: entities,
            qualifiers: Field.Parse(qualifiers),
            noUpdateFields: noUpdateFields,
            batchSize: batchSize,
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Insert multiple rows or update the existing rows in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of data entity objects to be merged.</param>
    /// <param name="qualifiers">The list of qualifier fields to be used.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the merge process.</returns>
    internal static int MergeAllInternal<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        IEnumerable<Field>? qualifiers,
        IEnumerable<Field>? noUpdateFields,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MergeAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null, IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        // Check the qualifiers
        if (qualifiers?.Any() != true)
        {
            var keys = GetAndGuardPrimaryKeyOrIdentityKey(connection, tableName, transaction,
                GetEntityType(entities));
            qualifiers = keys;
        }

        // Variables needed
        var setting = connection.GetDbSetting();

        // Return the result
        if (setting.IsUseUpsert == false)
        {
            if (TypeCache.Get(GetEntityType(entities)).IsDictionaryStringObject())
            {
                return MergeAllInternalBase(connection: connection,
                    tableName: tableName,
                    entities: entities.WithType<IDictionary<string, object>>(),
                    qualifiers: qualifiers,
                    batchSize: batchSize,
                    fields: fields ?? GetQualifiedFields(entities?.FirstOrDefault()),
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
                return MergeAllInternalBase(connection: connection,
                    tableName: tableName,
                    entities: entities,
                    qualifiers: qualifiers,
                    batchSize: batchSize,
                    fields: fields ?? GetQualifiedFields(entities?.FirstOrDefault()),
                    noUpdateFields: noUpdateFields,
                    hints: hints,
                    commandTimeout: commandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder);
            }
        }
        else
        {
            if (TypeCache.Get(GetEntityType(entities)).IsDictionaryStringObject())
            {
                return UpsertAllInternalBase(connection: connection,
                    tableName: tableName,
                    entities: entities.WithType<IDictionary<string, object>>(),
                    qualifiers: qualifiers,
                    fields: fields ?? GetQualifiedFields(entities?.FirstOrDefault()),
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
                return UpsertAllInternalBase(connection: connection,
                    tableName: tableName,
                    entities: entities,
                    qualifiers: qualifiers,
                    fields: fields ?? GetQualifiedFields(entities?.FirstOrDefault()),
                    noUpdateFields: noUpdateFields,
                    hints: hints,
                    commandTimeout: commandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder);
            }
        }
    }

    #endregion

    #region MergeAllAsync<TEntity>

    /// <summary>
    /// Insert multiple rows or update the existing rows in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of data entity objects to be merged.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the merge process.</returns>
    public static async Task<int> MergeAllAsync<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MergeAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await MergeAllAsyncInternal(connection: connection,
            tableName: tableName,
            entities: entities,
            qualifiers: null,
            noUpdateFields: null,
            batchSize: batchSize,
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
    /// Insert multiple rows or update the existing rows in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of data entity objects to be merged.</param>
    /// <param name="qualifier">The field to be used during merge operation.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the merge process.</returns>
    public static async Task<int> MergeAllAsync<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        Field qualifier,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MergeAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await MergeAllAsyncInternal(connection: connection,
            tableName: tableName,
            entities: entities,
            qualifiers: qualifier.AsEnumerable(),
            noUpdateFields: null,
            batchSize: batchSize,
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
    /// Insert multiple rows or update the existing rows in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of data entity objects to be merged.</param>
    /// <param name="qualifiers">The list of qualifier fields to be used.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the merge process.</returns>
    public static async Task<int> MergeAllAsync<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        IEnumerable<Field>? qualifiers,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        IEnumerable<Field>? noUpdateFields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MergeAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await MergeAllAsyncInternal(connection: connection,
            tableName: tableName,
            entities: entities,
            qualifiers: qualifiers,
            noUpdateFields: noUpdateFields,
            batchSize: batchSize,
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
    /// Insert multiple rows or update the existing rows in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of data entity objects to be merged.</param>
    /// <param name="qualifiers">The expression for the qualifier fields.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the merge process.</returns>
    public static async Task<int> MergeAllAsync<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        Expression<Func<TEntity, object?>> qualifiers,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        IEnumerable<Field>? noUpdateFields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MergeAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await MergeAllAsyncInternal(connection: connection,
            tableName: tableName,
            entities: entities,
            qualifiers: Field.Parse(qualifiers),
            noUpdateFields: noUpdateFields,
            batchSize: batchSize,
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
    /// Insert multiple rows or update the existing rows in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entities">The list of data entity objects to be merged.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the merge process.</returns>
    public static async Task<int> MergeAllAsync<TEntity>(this IDbConnection connection,
        IEnumerable<TEntity> entities,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MergeAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await MergeAllAsyncInternal(connection: connection,
            tableName: GetMappedName(entities),
            entities: entities,
            qualifiers: null,
            noUpdateFields: null,
            batchSize: batchSize,
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
    /// Insert multiple rows or update the existing rows in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entities">The list of data entity objects to be merged.</param>
    /// <param name="qualifier">The field to be used during merge operation.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the merge process.</returns>
    public static async Task<int> MergeAllAsync<TEntity>(this IDbConnection connection,
        IEnumerable<TEntity> entities,
        Field qualifier,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MergeAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await MergeAllAsyncInternal(connection: connection,
            tableName: GetMappedName(entities),
            entities: entities,
            qualifiers: qualifier.AsEnumerable(),
            noUpdateFields: null,
            batchSize: batchSize,
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
    /// Insert multiple rows or update the existing rows in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entities">The list of data entity objects to be merged.</param>
    /// <param name="qualifiers">The list of qualifier fields to be used.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the merge process.</returns>
    public static async Task<int> MergeAllAsync<TEntity>(this IDbConnection connection,
        IEnumerable<TEntity> entities,
        IEnumerable<Field>? qualifiers,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        IEnumerable<Field>? noUpdateFields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MergeAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await MergeAllAsyncInternal(connection: connection,
            tableName: GetMappedName(entities),
            entities: entities,
            qualifiers: qualifiers,
            noUpdateFields: noUpdateFields, batchSize: batchSize,
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
    /// Insert multiple rows or update the existing rows in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entities">The list of data entity objects to be merged.</param>
    /// <param name="qualifiers">The expression for the qualifier fields.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the merge process.</returns>
    public static async Task<int> MergeAllAsync<TEntity>(this IDbConnection connection,
        IEnumerable<TEntity> entities,
        Expression<Func<TEntity, object?>> qualifiers,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        IEnumerable<Field>? noUpdateFields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MergeAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await MergeAllAsyncInternal(connection: connection,
            tableName: GetMappedName(entities),
            entities: entities,
            qualifiers: Field.Parse(qualifiers),
            noUpdateFields: noUpdateFields,
            batchSize: batchSize,
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
    /// Insert multiple rows or update the existing rows in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of data entity objects to be merged.</param>
    /// <param name="qualifiers">The list of qualifier fields to be used.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the merge process.</returns>
    internal static async ValueTask<int> MergeAllAsyncInternal<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        IEnumerable<Field>? qualifiers,
        IEnumerable<Field>? noUpdateFields,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MergeAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null, CancellationToken cancellationToken = default)
        where TEntity : class
    {
        // Check the qualifiers
        if (qualifiers?.Any() != true)
        {
            var keys = await GetAndGuardPrimaryKeyOrIdentityKeyAsync(connection, tableName, transaction,
                GetEntityType(entities), cancellationToken).ConfigureAwait(false);
            qualifiers = keys;
        }

        // Variables needed
        var setting = connection.GetDbSetting();

        // Return the result
        if (setting.IsUseUpsert == false)
        {
            if (TypeCache.Get(GetEntityType(entities)).IsDictionaryStringObject())
            {
                return await MergeAllAsyncInternalBase(connection: connection,
                    tableName: tableName,
                    entities: entities.WithType<IDictionary<string, object>>(),
                    qualifiers: qualifiers,
                    batchSize: batchSize,
                    fields: fields ?? GetQualifiedFields(entities?.FirstOrDefault()),
                    noUpdateFields: noUpdateFields,
                    hints: hints,
                    commandTimeout: commandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else
            {
                return await MergeAllAsyncInternalBase(connection: connection,
                    tableName: tableName,
                    entities: entities,
                    qualifiers: qualifiers,
                    batchSize: batchSize,
                    fields: fields ?? GetQualifiedFields(entities?.FirstOrDefault()),
                    noUpdateFields: noUpdateFields,
                    hints: hints,
                    commandTimeout: commandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
        }
        else
        {
            if (TypeCache.Get(GetEntityType(entities)).IsDictionaryStringObject())
            {
                return await UpsertAllAsyncInternalBase(connection: connection,
                    tableName: tableName,
                    entities: entities.WithType<IDictionary<string, object>>(),
                    qualifiers: qualifiers,
                    fields: fields ?? GetQualifiedFields(entities?.FirstOrDefault()),
                    hints: hints,
                    commandTimeout: commandTimeout,
            traceKey: traceKey,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else
            {
                return await UpsertAllAsyncInternalBase(connection: connection,
                    tableName: tableName,
                    entities: entities,
                    qualifiers: qualifiers,
                    fields: fields ?? GetQualifiedFields(entities?.FirstOrDefault()),
                    hints: hints,
                    commandTimeout: commandTimeout,
            traceKey: traceKey,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
        }
    }

    #endregion

    #region MergeAll(TableName)

    /// <summary>
    /// Insert the multiple dynamic objects (as new rows) or update the existing rows in the table. By default, the table fields are used unless the 'fields' argument is explicitly defined.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of dynamic objects to be merged.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the merge process.</returns>
    public static int MergeAll(this IDbConnection connection,
        string tableName,
        IEnumerable<object> entities,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MergeAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return MergeAllInternal(connection: connection,
            tableName: tableName,
            entities: entities,
            qualifiers: null,
            noUpdateFields: null,
            batchSize: batchSize,
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Insert the multiple dynamic objects (as new rows) or update the existing rows in the table. By default, the table fields are used unless the 'fields' argument is explicitly defined.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of dynamic objects to be merged.</param>
    /// <param name="qualifier">The qualifier field to be used.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the merge process.</returns>
    public static int MergeAll(this IDbConnection connection,
        string tableName,
        IEnumerable<object> entities,
        Field? qualifier,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        IEnumerable<Field>? noUpdateFields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MergeAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return MergeAllInternal(connection: connection,
            tableName: tableName,
            entities: entities,
            qualifiers: qualifier?.AsEnumerable(),
            noUpdateFields: noUpdateFields,
            batchSize: batchSize,
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Insert the multiple dynamic objects (as new rows) or update the existing rows in the table. By default, the table fields are used unless the 'fields' argument is explicitly defined.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of dynamic objects to be merged.</param>
    /// <param name="qualifiers">The qualifier <see cref="Field"/> objects to be used.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the merge process.</returns>
    public static int MergeAll(this IDbConnection connection,
        string tableName,
        IEnumerable<object> entities,
        IEnumerable<Field>? qualifiers,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        IEnumerable<Field>? noUpdateFields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MergeAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return MergeAllInternal(connection: connection,
            tableName: tableName,
            entities: entities,
            qualifiers: qualifiers,
            noUpdateFields: noUpdateFields,
            batchSize: batchSize,
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    #endregion

    #region MergeAllAsync(TableName)

    /// <summary>
    /// Merges the multiple dynamic objects into the database in an asynchronous way. By default, the table fields are used unless the 'fields' argument is explicitly defined.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of dynamic objects to be merged.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the merge process.</returns>
    public static async Task<int> MergeAllAsync(this IDbConnection connection,
        string tableName,
        IEnumerable<object> entities,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MergeAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await MergeAllAsyncInternal(connection: connection,
            tableName: tableName,
            entities: entities,
            qualifiers: null,
            noUpdateFields: null,
            batchSize: batchSize,
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
    /// Merges the multiple dynamic objects into the database in an asynchronous way. By default, the table fields are used unless the 'fields' argument is explicitly defined.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of dynamic objects to be merged.</param>
    /// <param name="qualifier">The qualifier field to be used.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the merge process.</returns>
    public static async Task<int> MergeAllAsync(this IDbConnection connection,
        string tableName,
        IEnumerable<object> entities,
        Field qualifier,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MergeAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await MergeAllAsyncInternal(connection: connection,
            tableName: tableName,
            entities: entities,
            qualifiers: qualifier?.AsEnumerable(),
            noUpdateFields: null,
            batchSize: batchSize,
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
    /// Merges the multiple dynamic objects into the database in an asynchronous way. By default, the table fields are used unless the 'fields' argument is explicitly defined.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of dynamic objects to be merged.</param>
    /// <param name="qualifiers">The qualifier <see cref="Field"/> objects to be used.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the merge process.</returns>
    public static async Task<int> MergeAllAsync(this IDbConnection connection,
        string tableName,
        IEnumerable<object> entities,
        IEnumerable<Field>? qualifiers,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        IEnumerable<Field>? noUpdateFields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MergeAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await MergeAllAsyncInternal(connection: connection,
            tableName: tableName,
            entities: entities,
            qualifiers: qualifiers,
            noUpdateFields: noUpdateFields,
            batchSize: batchSize,
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

    #region MergeAllInternalBase<TEntity>

    /// <summary>
    /// Merges the multiple data entity or dynamic objects into the database.
    /// </summary>
    /// <typeparam name="TEntity">The type of the object (whether a data entity or a dynamic).</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The data entity or dynamic object to be merged.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="qualifiers">The list of qualifier fields to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the merge process.</returns>
    internal static int MergeAllInternalBase<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        IEnumerable<Field> qualifiers,
        int batchSize,
        IEnumerable<Field> fields,
        IEnumerable<Field>? noUpdateFields,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MergeAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
#if NET
        ArgumentNullException.ThrowIfNull(entities);
#endif
        entities = entities.AsList();
        if (entities.Any() != true)
        {
            return default;
        }

        // Variables needed
        var dbSetting = connection.GetDbSetting();

        // Validate the batch size
        int maxBatchSize = (dbSetting.IsMultiStatementExecutable == true)
            ? Math.Min((batchSize <= 0 ? dbSetting.MaxParameterCount / (fields.Concat(qualifiers).Select(x => x.FieldName).Distinct().Count()) : batchSize), dbSetting.MaxQueriesInBatchCount)
            : 1;
        batchSize = Math.Min(batchSize <= 0 ? Constant.DefaultBatchOperationSize : batchSize, entities.Count());

        // Get the context
        var entityType = GetEntityType(entities);
        var context = MergeAllExecutionContextProvider.Create(entityType,
            connection,
            entities,
            tableName,
            qualifiers,
            batchSize,
            fields,
            noUpdateFields,
            hints,
            transaction,
            statementBuilder);
        var result = 0;

        connection.EnsureOpen();
        using var myTransaction = (transaction is null && Transaction.Current is null) ? connection.BeginTransaction() : null;
        transaction ??= myTransaction;

        // Create the command
        using (var command = (DbCommand)connection.CreateCommand(context.CommandText,
            CommandType.Text, commandTimeout, transaction))
        {
            // Directly execute if the entities is only 1 (performance)
            if (batchSize == 1)
            {
                // Much better to use the actual single-based setter (performance)
                foreach (var entity in entities.AsList())
                {
                    // Set the values
                    context.SingleDataEntityParametersSetterFunc?.Invoke(command, entity);

                    // Prepare the command
                    if (dbSetting.IsPreparable)
                    {
                        command.Prepare();
                    }

                    // Before Execution
                    var traceResult = Tracer
                        .InvokeBeforeExecution(traceKey, trace, command);

                    // Silent cancellation
                    if (traceResult?.CancellableTraceLog?.IsCancelled == true)
                    {
                        return result;
                    }

                    // Actual Execution
                    var returnValue = Converter.DbNullToNull(command.ExecuteScalar());

                    // After Execution
                    Tracer
                        .InvokeAfterExecution(traceResult, trace, result);

                    // Set the return value
                    if (returnValue != null)
                    {
                        context.KeyPropertySetterFunc?.Invoke(entity, returnValue);
                    }

                    // Iterate the result
                    result++;
                }
            }
            else
            {
                int? positionIndex = null;
                bool doPrepare = dbSetting.IsPreparable;

                foreach (var batchItems in entities.Split(maxBatchSize))
                {
                    if (batchItems.Length != context.BatchSize)
                    {
                        // Get a new execution context from cache
                        context = MergeAllExecutionContextProvider.Create(entityType,
                            connection,
                            batchItems,
                            tableName,
                            qualifiers,
                            batchItems.Length,
                            fields,
                            noUpdateFields,
                            hints,
                            transaction,
                            statementBuilder);

                        // Set the command properties
                        command.CommandText = context.CommandText;
                        doPrepare = dbSetting.IsPreparable;
                    }

                    // Set the values
                    if (batchItems.Length == 1)
                    {
                        context.SingleDataEntityParametersSetterFunc?.Invoke(command, batchItems.First());
                    }
                    else
                    {
                        context.MultipleDataEntitiesParametersSetterFunc?.Invoke(command, batchItems.OfType<object?>().AsList());
                    }

                    // Prepare the command
                    if (doPrepare)
                    {
                        command.Prepare();
                    }

                    // Actual Execution
                    if (context.KeyPropertySetterFunc == null)
                    {
                        // Before Execution
                        var traceResult = Tracer
                            .InvokeBeforeExecution(traceKey, trace, command);

                        // Silent cancellation
                        if (traceResult?.CancellableTraceLog?.IsCancelled == true)
                        {
                            return result;
                        }

                        // No identity setters
                        result += command.ExecuteNonQuery();

                        // After Execution
                        Tracer
                            .InvokeAfterExecution(traceResult, trace, result);
                    }
                    else
                    {
                        // Before Execution
                        var traceResult = Tracer
                            .InvokeBeforeExecution(traceKey, trace, command);

                        // Set the identity back
                        using var reader = command.ExecuteReader();

                        // Get the results
                        var position = 0;
                        do
                        {
                            while (position < batchItems.Length && reader.Read())
                            {
                                var value = Converter.DbNullToNull(reader.GetValue(0));
                                if (value != null)
                                {
                                    positionIndex ??= (reader.FieldCount > 1) && string.Equals(BaseStatementBuilder.RepoDbOrderColumn, reader.GetName(reader.FieldCount - 1), StringComparison.OrdinalIgnoreCase) ? reader.FieldCount - 1 : -1;

                                    var index = positionIndex >= 0 && positionIndex < reader.FieldCount ? reader.GetInt32(positionIndex.Value) : position;
                                    context.KeyPropertySetterFunc.Invoke(batchItems[index], value);
                                }
                                position++;
                            }
                        }
                        while (position < batchItems.Length && reader.NextResult());

                        result += batchItems.Length;

                        // After Execution
                        Tracer
                            .InvokeAfterExecution(traceResult, trace, result);
                    }
                }
            }
        }

        myTransaction?.Commit();

        // Return the result
        return result;
    }

    #endregion

    #region MergeAllAsyncInternalBase<TEntity>

    /// <summary>
    /// Merges the multiple data entity or dynamic objects into the database in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the object (whether a data entity or a dynamic).</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The data entity or dynamic object to be merged.</param>
    /// <param name="qualifiers">The list of qualifier fields to be used.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the merge process.</returns>
    internal static async ValueTask<int> MergeAllAsyncInternalBase<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        IEnumerable<Field> qualifiers,
        int batchSize,
        IEnumerable<Field> fields,
        IEnumerable<Field>? noUpdateFields,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MergeAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
#if NET
        ArgumentNullException.ThrowIfNull(entities);
#endif
        entities = entities.AsList();
        if (entities.Any() != true)
        {
            return default;
        }

        var dbSetting = connection.GetDbSetting();

        // Validate the batch size
        int maxBatchSize = (dbSetting.IsMultiStatementExecutable == true)
            ? Math.Min((batchSize <= 0 ? dbSetting.MaxParameterCount / (fields.Concat(qualifiers).Select(x => x.FieldName).Distinct().Count()) : batchSize), dbSetting.MaxQueriesInBatchCount)
            : 1;
        batchSize = Math.Min(batchSize <= 0 ? Constant.DefaultBatchOperationSize : batchSize, entities.Count());

        // Get the context
        var entityType = GetEntityType(entities);
        var context = await MergeAllExecutionContextProvider.CreateAsync(entityType,
            connection,
            entities,
            tableName,
            qualifiers,
            Math.Min(maxBatchSize, entities.Count()),
            fields,
            noUpdateFields,
            hints,
            transaction,
            statementBuilder,
            cancellationToken).ConfigureAwait(false);
        var result = 0;

        await connection.EnsureOpenAsync(cancellationToken).ConfigureAwait(false);

        using var myTransaction = (transaction is null && Transaction.Current is null) ? await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false) : null;
        transaction ??= myTransaction;

        // Create the command
        using (var command = (DbCommand)connection.CreateCommand(context.CommandText,
            CommandType.Text, commandTimeout, transaction))
        {
            // Directly execute if the entities is only 1 (performance)
            if (batchSize == 1)
            {
                // Much better to use the actual single-based setter (performance)
                foreach (var entity in entities.AsList())
                {
                    // Set the values
                    context.SingleDataEntityParametersSetterFunc?.Invoke(command, entity);

                    // Prepare the command
                    if (dbSetting.IsPreparable)
                    {
                        await command.PrepareAsync(cancellationToken).ConfigureAwait(false);
                    }

                    // Before Execution
                    var traceResult = await Tracer
                        .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken).ConfigureAwait(false);

                    // Silent cancellation
                    if (traceResult?.CancellableTraceLog?.IsCancelled == true)
                    {
                        return result;
                    }

                    // Actual Execution
                    var returnValue = Converter.DbNullToNull(await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false));

                    // After Execution
                    await Tracer
                        .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken).ConfigureAwait(false);

                    // Set the return value
                    if (returnValue != null)
                    {
                        context.KeyPropertySetterFunc?.Invoke(entity, returnValue);
                    }

                    // Iterate the result
                    result++;
                }
            }
            else
            {
                int? positionIndex = null;
                bool doPrepare = dbSetting.IsPreparable;

                // Iterate the batches
                foreach (var batchItems in entities.Split(batchSize))
                {
                    if (batchItems.Length != context.BatchSize)
                    {
                        // Get a new execution context from cache
                        context = await MergeAllExecutionContextProvider.CreateAsync(entityType,
                            connection,
                            entities,
                            tableName,
                            qualifiers,
                            batchItems.Length,
                            fields,
                            noUpdateFields,
                            hints,
                            transaction,
                            statementBuilder,
                            cancellationToken).ConfigureAwait(false);

                        // Set the command properties
                        command.CommandText = context.CommandText;
                        doPrepare = dbSetting.IsPreparable;
                    }

                    // Set the values
                    if (batchItems.Length == 1)
                    {
                        context.SingleDataEntityParametersSetterFunc?.Invoke(command, batchItems.First());
                    }
                    else
                    {
                        context.MultipleDataEntitiesParametersSetterFunc?.Invoke(command, batchItems.OfType<object?>().AsList());
                    }

                    // Prepare the command
                    if (doPrepare)
                    {
                        await command.PrepareAsync(cancellationToken).ConfigureAwait(false);
                        doPrepare = false;
                    }

                    // Actual Execution
                    if (context.KeyPropertySetterFunc == null)
                    {
                        // Before Execution
                        var traceResult = await Tracer
                            .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken).ConfigureAwait(false);

                        // Silent cancellation
                        if (traceResult?.CancellableTraceLog?.IsCancelled == true)
                        {
                            return result;
                        }

                        // No identity setters
                        result += await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

                        // After Execution
                        await Tracer
                            .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        // Before Execution
                        var traceResult = await Tracer
                            .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken).ConfigureAwait(false);

                        // Set the identity back
#if NET
                        await
#endif
                        using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

                        // Get the results.
                        var position = 0;
                        do
                        {
                            while (position < batchItems.Length && await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                            {
                                var value = Converter.DbNullToNull(reader.GetValue(0));
                                if (value != null)
                                {
                                    positionIndex ??= (reader.FieldCount > 1) && string.Equals(BaseStatementBuilder.RepoDbOrderColumn, reader.GetName(reader.FieldCount - 1), StringComparison.OrdinalIgnoreCase) ? reader.FieldCount - 1 : -1;
                                    var index = positionIndex >= 0 && positionIndex < reader.FieldCount ? reader.GetInt32(positionIndex.Value) : position;
                                    context.KeyPropertySetterFunc.Invoke(batchItems[index], value);
                                }
                                position++;
                            }
                        }
                        while (position < batchItems.Length && await reader.NextResultAsync(cancellationToken).ConfigureAwait(false));

                        result += batchItems.Length;

                        // After Execution
                        await Tracer
                            .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken).ConfigureAwait(false);
                    }
                }
            }
        }

        if (myTransaction is { })
            await myTransaction.CommitAsync(cancellationToken).ConfigureAwait(false);

        // Return the result
        return result;
    }

    #endregion

    #region UpsertAllInternalBase<TEntity>

    /// <summary>
    /// Upserts the multiple data entity or dynamic objects into the database.
    /// </summary>
    /// <typeparam name="TEntity">The type of the object (whether a data entity or a dynamic).</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The data entity or dynamic object to be merged.</param>
    /// <param name="qualifiers">The list of qualifier fields to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the merge process.</returns>
    internal static int UpsertAllInternalBase<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        IEnumerable<Field>? qualifiers,
        IEnumerable<Field> fields,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MergeAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        IEnumerable<Field>? noUpdateFields = null)
        where TEntity : class
    {
        // Variables needed
        var dbFields = DbFieldCache.Get(connection, tableName, transaction);

        qualifiers ??= dbFields.GetPrimaryFields()?.AsFields();

        // Check the qualifiers
        if (qualifiers?.Any() != true)
        {
            throw new PrimaryFieldNotFoundException($"There are no qualifiers, nor primary keys for '{tableName}'.");
        }

        // Execution variables
        var result = 0;

        // Make sure to create transaction if there is no passed one
        connection.EnsureOpen();
        using var myTransaction = (transaction is null && Transaction.Current is null) ? connection.BeginTransaction() : null;
        transaction ??= myTransaction;

        // Iterate the entities
        var immutableFields = EnumerableExtension.AsList(fields); // Fix for the IDictionary<string, object> object
        foreach (var entity in entities.AsList())
        {
            // Call the upsert
            var upsertResult = connection.UpsertInternalBase<TEntity, object>(tableName,
                entity,
                qualifiers,
                immutableFields,
                hints,
                commandTimeout,
                traceKey: traceKey,
                transaction,
                trace,
                statementBuilder);

            // Iterate the result
            if (Converter.DbNullToNull(upsertResult) != null)
            {
                result++;
            }
        }
        myTransaction?.Commit();

        // Return the result
        return result;
    }

    #endregion

    #region UpsertAllInternalBase<TEntity>

    /// <summary>
    /// Upserts the multiple data entity or dynamic objects into the database in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the object (whether a data entity or a dynamic).</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The data entity or dynamic object to be merged.</param>
    /// <param name="qualifiers">The list of qualifier fields to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the merge process.</returns>
    internal static async ValueTask<int> UpsertAllAsyncInternalBase<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        IEnumerable<Field>? qualifiers,
        IEnumerable<Field> fields,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.MergeAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        // Variables needed
        var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken).ConfigureAwait(false);

        qualifiers ??= dbFields.GetPrimaryFields()?.AsFields();

        // Check the qualifiers
        if (qualifiers?.Any() != true)
        {
            throw new PrimaryFieldNotFoundException($"There are no qualifiers, nor primary keys for '{tableName}'.");
        }

        // Execution variables
        var result = 0;

        await connection.EnsureOpenAsync(cancellationToken).ConfigureAwait(false);
        using var myTransaction = (transaction is null && Transaction.Current is null) ? await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false) : null;
        transaction ??= myTransaction;

        // Iterate the entities
        var immutableFields = EnumerableExtension.AsList(fields); // Fix for the IDictionary<string, object> object
        foreach (var entity in entities.AsList())
        {
            // Call the upsert
            var upsertResult = await connection.UpsertAsyncInternalBase<TEntity, object>(tableName,
                entity,
                qualifiers,
                immutableFields,
                hints,
                commandTimeout,
                traceKey,
                transaction,
                trace,
                statementBuilder,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            // Iterate the result
            if (Converter.DbNullToNull(upsertResult) != null)
            {
                result++;
            }
        }

        if (myTransaction is { })
            await myTransaction.CommitAsync(cancellationToken).ConfigureAwait(false);

        // Return the result
        return result;
    }

    #endregion
}
