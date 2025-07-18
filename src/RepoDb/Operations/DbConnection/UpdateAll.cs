using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Transactions;
using RepoDb.Contexts.Providers;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb;

/// <summary>
/// Contains the extension methods for <see cref="IDbConnection"/> object.
/// </summary>
public static partial class DbConnectionExtension
{
    #region UpdateAll<TEntity>

    /// <summary>
    /// Update the existing rows in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of data entity objects to be used for update.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static int UpdateAll<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return UpdateAllInternal(connection: connection,
            tableName: tableName,
            entities: entities,
            qualifiers: null,
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
    /// Update the existing rows in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of data entity objects to be used for update.</param>
    /// <param name="qualifier">The qualifier <see cref="Field"/> object to be used for update.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static int UpdateAll<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        Field qualifier,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return UpdateAllInternal(connection: connection,
            tableName: tableName,
            entities: entities,
            qualifiers: qualifier?.AsEnumerable(),
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
    /// Update the existing rows in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of data entity objects to be used for update.</param>
    /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static int UpdateAll<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        IEnumerable<Field>? qualifiers,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return UpdateAllInternal(connection: connection,
            tableName: tableName,
            entities: entities,
            qualifiers: qualifiers,
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
    /// Update the existing rows in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of data entity objects to be used for update.</param>
    /// <param name="qualifiers">The expression for the qualifier fields.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static int UpdateAll<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        Expression<Func<TEntity, object?>> qualifiers,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return UpdateAllInternal(connection: connection,
            tableName: tableName,
            entities: entities,
            qualifiers: Field.Parse(qualifiers),
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
    /// Update the existing rows in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entities">The list of data entity objects to be used for update.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static int UpdateAll<TEntity>(this IDbConnection connection,
        IEnumerable<TEntity> entities,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return UpdateAllInternal(connection: connection,
            tableName: GetMappedName(entities),
            entities: entities,
            qualifiers: null,
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
    /// Update the existing rows in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entities">The list of data entity objects to be used for update.</param>
    /// <param name="qualifier">The qualifier <see cref="Field"/> object to be used for update.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static int UpdateAll<TEntity>(this IDbConnection connection,
        IEnumerable<TEntity> entities,
        Field qualifier,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return UpdateAllInternal(connection: connection,
            tableName: GetMappedName(entities),
            entities: entities,
            qualifiers: qualifier?.AsEnumerable(),
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
    /// Update the existing rows in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entities">The list of data entity objects to be used for update.</param>
    /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static int UpdateAll<TEntity>(this IDbConnection connection,
        IEnumerable<TEntity> entities,
        IEnumerable<Field>? qualifiers,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return UpdateAllInternal(connection: connection,
            tableName: GetMappedName(entities),
            entities: entities,
            qualifiers: qualifiers,
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
    /// Update the existing rows in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entities">The list of data entity objects to be used for update.</param>
    /// <param name="qualifiers">The expression for the qualifier fields.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static int UpdateAll<TEntity>(this IDbConnection connection,
        IEnumerable<TEntity> entities,
        Expression<Func<TEntity, object?>> qualifiers,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return UpdateAllInternal(connection: connection,
            tableName: GetMappedName(entities),
            entities: entities,
            qualifiers: Field.Parse(qualifiers),
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
    /// Update the existing rows in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of data entity objects to be used for update.</param>
    /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    internal static int UpdateAllInternal<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        IEnumerable<Field>? qualifiers,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        if (qualifiers?.Any() != true)
        {
            qualifiers = GetAndGuardPrimaryKeyOrIdentityKey(connection, tableName, transaction,
                GetEntityType(entities));
        }
        if (TypeCache.Get(GetEntityType(entities)).IsDictionaryStringObject)
        {
            return UpdateAllInternalBase(connection: connection,
                tableName: tableName,
                entities: entities.WithType<IDictionary<string, object?>>(),
                batchSize: batchSize,
                fields: fields ?? GetQualifiedFields(entities?.FirstOrDefault()),
                qualifiers: qualifiers,
                hints: hints,
                commandTimeout: commandTimeout,
            traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }
        else
        {
            return UpdateAllInternalBase(connection: connection,
                tableName: tableName,
                entities: entities,
                batchSize: batchSize,
                fields: fields ?? GetQualifiedFields(entities?.FirstOrDefault()),
                qualifiers: qualifiers,
                hints: hints,
                commandTimeout: commandTimeout,
            traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }
    }

    #endregion

    #region UpdateAllAsync<TEntity>

    /// <summary>
    /// Update the existing rows in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of data entity objects to be used for update.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static async Task<int> UpdateAllAsync<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await UpdateAllInternalAsync(connection: connection,
            tableName: tableName,
            entities: entities,
            qualifiers: null,
            batchSize: batchSize,
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Update the existing rows in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of data entity objects to be used for update.</param>
    /// <param name="qualifier">The qualifier <see cref="Field"/> object to be used for update.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static async Task<int> UpdateAllAsync<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        Field qualifier,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await UpdateAllInternalAsync(connection: connection,
            tableName: tableName,
            entities: entities,
            qualifiers: qualifier?.AsEnumerable(),
            batchSize: batchSize,
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Update the existing rows in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of data entity objects to be used for update.</param>
    /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static async Task<int> UpdateAllAsync<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        IEnumerable<Field>? qualifiers,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await UpdateAllInternalAsync(connection: connection,
            tableName: tableName,
            entities: entities,
            qualifiers: qualifiers,
            batchSize: batchSize,
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Update the existing rows in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of data entity objects to be used for update.</param>
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
    /// <returns>The number of affected rows during the update process.</returns>
    public static async Task<int> UpdateAllAsync<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        Expression<Func<TEntity, object?>> qualifiers,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await UpdateAllInternalAsync(connection: connection,
            tableName: tableName,
            entities: entities,
            qualifiers: Field.Parse(qualifiers),
            batchSize: batchSize,
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Update the existing rows in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entities">The list of data entity objects to be used for update.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static async Task<int> UpdateAllAsync<TEntity>(this IDbConnection connection,
        IEnumerable<TEntity> entities,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await UpdateAllInternalAsync(connection: connection,
            tableName: GetMappedName(entities),
            entities: entities,
            qualifiers: null,
            batchSize: batchSize,
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Update the existing rows in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entities">The list of data entity objects to be used for update.</param>
    /// <param name="qualifier">The qualifier <see cref="Field"/> object to be used for update.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static async Task<int> UpdateAllAsync<TEntity>(this IDbConnection connection,
        IEnumerable<TEntity> entities,
        Field qualifier,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await UpdateAllInternalAsync(connection: connection,
            tableName: GetMappedName(entities),
            entities: entities,
            qualifiers: qualifier?.AsEnumerable(),
            batchSize: batchSize,
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Update the existing rows in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entities">The list of data entity objects to be used for update.</param>
    /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static async Task<int> UpdateAllAsync<TEntity>(this IDbConnection connection,
        IEnumerable<TEntity> entities,
        IEnumerable<Field>? qualifiers,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await UpdateAllInternalAsync(connection: connection,
            tableName: GetMappedName(entities),
            entities: entities,
            qualifiers: qualifiers,
            batchSize: batchSize,
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Update the existing rows in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entities">The list of data entity objects to be used for update.</param>
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
    /// <returns>The number of affected rows during the update process.</returns>
    public static async Task<int> UpdateAllAsync<TEntity>(this IDbConnection connection,
        IEnumerable<TEntity> entities,
        Expression<Func<TEntity, object?>> qualifiers,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await UpdateAllInternalAsync(connection: connection,
            tableName: GetMappedName(entities),
            entities: entities,
            qualifiers: Field.Parse(qualifiers),
            batchSize: batchSize,
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Update the existing rows in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of data entity objects to be used for update.</param>
    /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    internal static async ValueTask<int> UpdateAllInternalAsync<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        IEnumerable<Field>? qualifiers,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        if (qualifiers?.Any() != true)
        {
            qualifiers = await GetAndGuardPrimaryKeyOrIdentityKeyAsync(connection, tableName, transaction,
                GetEntityType(entities), cancellationToken).ConfigureAwait(false);
        }
        if (TypeCache.Get(GetEntityType(entities)).IsDictionaryStringObject)
        {
            return await UpdateAllInternalBaseAsync(connection: connection,
                tableName: tableName,
                entities: entities.WithType<IDictionary<string, object?>>(),
                batchSize: batchSize,
                fields: fields ?? GetQualifiedFields(entities?.FirstOrDefault()),
                qualifiers: qualifiers,
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
            return await UpdateAllInternalBaseAsync(connection: connection,
                tableName: tableName,
                entities: entities,
                batchSize: batchSize,
                fields: fields ?? GetQualifiedFields(entities?.FirstOrDefault()),
                qualifiers: qualifiers,
                hints: hints,
                commandTimeout: commandTimeout,
            traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }

    #endregion

    #region UpdateAll(TableName)

    /// <summary>
    /// Update the existing rows in the table. By default, the table fields are used unless the 'fields' argument is defined.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entities">The list of dynamic objects to be used for update.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static int UpdateAll(this IDbConnection connection,
        string tableName,
        IEnumerable<object> entities,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return UpdateAllInternal(connection: connection,
            tableName: tableName,
            entities: entities,
            qualifiers: null,
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
    /// Update the existing rows in the table. By default, the table fields are used unless the 'fields' argument is defined.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entities">The list of dynamic objects to be used for update.</param>
    /// <param name="qualifier">The qualifier <see cref="Field"/> object to be used for update.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static int UpdateAll(this IDbConnection connection,
        string tableName,
        IEnumerable<object> entities,
        Field qualifier,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return UpdateAllInternal(connection: connection,
            tableName: tableName,
            entities: entities,
            qualifiers: qualifier?.AsEnumerable(),
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
    /// Update the existing rows in the table. By default, the table fields are used unless the 'fields' argument is defined.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entities">The list of dynamic objects to be used for update.</param>
    /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static int UpdateAll(this IDbConnection connection,
        string tableName,
        IEnumerable<object> entities,
        IEnumerable<Field>? qualifiers,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return UpdateAllInternal(connection: connection,
            tableName: tableName,
            entities: entities,
            qualifiers: qualifiers,
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

    #region UpdateAllAsync(TableName)

    /// <summary>
    /// Update the existing rows in the table in an asynchronous way. By default, the table fields are used unless the 'fields' argument is defined.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entities">The list of dynamic objects to be used for update.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static async Task<int> UpdateAllAsync(this IDbConnection connection,
        string tableName,
        IEnumerable<object> entities,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await UpdateAllInternalAsync(connection: connection,
            tableName: tableName,
            entities: entities,
            qualifiers: null,
            batchSize: batchSize,
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Update the existing rows in the table in an asynchronous way. By default, the table fields are used unless the 'fields' argument is defined.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entities">The list of dynamic objects to be used for update.</param>
    /// <param name="qualifier">The qualifier <see cref="Field"/> object to be used for update.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static async Task<int> UpdateAllAsync(this IDbConnection connection,
        string tableName,
        IEnumerable<object> entities,
        Field? qualifier,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await UpdateAllInternalAsync(connection: connection,
            tableName: tableName,
            entities: entities,
            qualifiers: qualifier?.AsEnumerable(),
            batchSize: batchSize,
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Update the existing rows in the table in an asynchronous way. By default, the table fields are used unless the 'fields' argument is defined.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entities">The list of dynamic objects to be used for update.</param>
    /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static async Task<int> UpdateAllAsync(this IDbConnection connection,
        string tableName,
        IEnumerable<object> entities,
        IEnumerable<Field>? qualifiers,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await UpdateAllInternalAsync(connection: connection,
            tableName: tableName,
            entities: entities,
            qualifiers: qualifiers,
            batchSize: batchSize,
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

    #region UpdateAllInternalBase<TEntity>

    /// <summary>
    /// Update the existing rows in the table.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <typeparam name="TEntity">The type of the object (whether a data entity or a dynamic).</typeparam>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of data entity or dynamic objects to be updated.</param>
    /// <param name="fields">The list of <see cref="Field"/> objects to be updated.</param>
    /// <param name="qualifiers">The list of the qualifier <see cref="Field"/> objects.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    internal static int UpdateAllInternalBase<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        IEnumerable<Field>? qualifiers,
        int batchSize,
        IEnumerable<Field> fields,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
#if NET
        ArgumentNullException.ThrowIfNull(entities);
#endif
        entities = entities.AsList();
        if (!entities.Any())
        {
            return default;
        }

        var dbSetting = connection.GetDbSetting();

        // Validate the batch size
        int maxBatchSize = dbSetting.IsMultiStatementExecutable
            ? Math.Min(batchSize <= 0 ? dbSetting.MaxParameterCount / fields.Concat(qualifiers ?? []).DistinctBy(x => x.FieldName).Count() : batchSize, dbSetting.MaxQueriesInBatchCount)
            : 1;
        batchSize = Math.Min(batchSize <= 0 ? Constant.DefaultBatchOperationSize : batchSize, entities.Count());

        // Get the context
        var entityType = GetEntityType(entities);
        var context = UpdateAllExecutionContextProvider.Create(entityType,
            connection,
            tableName,
            entities,
            qualifiers,
            batchSize,
            fields,
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
                    result += command.ExecuteNonQuery();

                    // After Execution
                    Tracer
                        .InvokeAfterExecution(traceResult, trace, result);
                }
            }
            else
            {
                bool doPrepare = dbSetting.IsPreparable;

                foreach (var batchItems in entities.ChunkOptimally(maxBatchSize))
                {
                    if (batchItems.Count != context.BatchSize)
                    {
                        // Get a new execution context from cache
                        context = UpdateAllExecutionContextProvider.Create(entityType,
                            connection,
                            tableName,
                            batchItems,
                            qualifiers,
                            batchItems.Count,
                            fields,
                            hints,
                            transaction,
                            statementBuilder);

                        // Set the command properties
                        command.CommandText = context.CommandText;
                        doPrepare = dbSetting.IsPreparable;
                    }

                    // Set the values
                    if (batchItems.Count == 1)
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
                        doPrepare = false;
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
                    result += command.ExecuteNonQuery();

                    // After Execution
                    Tracer
                        .InvokeAfterExecution(traceResult, trace, result);
                }
            }
        }

        myTransaction?.Commit();

        // Return the result
        return result;
    }

    #endregion

    #region UpdateAllInternalBaseAsync<TEntity>

    /// <summary>
    /// Update the existing rows in the table in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <typeparam name="TEntity">The type of the object (whether a data entity or a dynamic).</typeparam>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of data entity or dynamic objects to be updated.</param>
    /// <param name="qualifiers">The list of the qualifier <see cref="Field"/> objects.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The list of <see cref="Field"/> objects to be updated.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    internal static async ValueTask<int> UpdateAllInternalBaseAsync<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        IEnumerable<Field>? qualifiers,
        int batchSize,
        IEnumerable<Field> fields,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entities);

        entities = entities.AsList();
        if (!entities.Any())
        {
            return default;
        }

        var dbSetting = connection.GetDbSetting();

        // Validate the batch size
        int maxBatchSize = dbSetting.IsMultiStatementExecutable
            ? Math.Min(batchSize <= 0 ? dbSetting.MaxParameterCount / fields.Concat(qualifiers ?? []).Select(x => x.FieldName).Distinct().Count() : batchSize, dbSetting.MaxQueriesInBatchCount)
            : 1;
        batchSize = Math.Min(batchSize <= 0 ? Constant.DefaultBatchOperationSize : batchSize, entities.Count());

        await connection.EnsureOpenAsync(cancellationToken).ConfigureAwait(false);
        using var myTransaction = (transaction is null && Transaction.Current is null) ? await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false) : null;
        transaction ??= myTransaction;

        // Get the context
        var entityType = GetEntityType(entities);
        var context = await UpdateAllExecutionContextProvider.CreateAsync(entityType,
            connection,
            tableName,
            entities,
            qualifiers,
            batchSize,
            fields,
            hints,
            transaction,
            statementBuilder,
            cancellationToken).ConfigureAwait(false);
        var result = 0;

        // Create the command
        using (var command = (DbCommand)connection.CreateCommand(context.CommandText,
            CommandType.Text, commandTimeout, transaction))
        {
            // Directly execute if the entities is only 1 (performance)
            if (batchSize == 1)
            {
                // Much better to use the actual single-based setter (performance)
                foreach (var entity in entities)
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
                    result += await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

                    // After Execution
                    await Tracer
                        .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken).ConfigureAwait(false);
                }
            }
            else
            {
                bool doPrepare = dbSetting.IsPreparable;
                foreach (var batchItems in entities.ChunkOptimally(maxBatchSize))
                {
                    if (batchItems.Count != context.BatchSize)
                    {
                        // Get a new execution context from cache
                        context = await UpdateAllExecutionContextProvider.CreateAsync(entityType,
                            connection,
                            tableName,
                            batchItems,
                            qualifiers,
                            batchItems.Count,
                            fields,
                            hints,
                            transaction,
                            statementBuilder,
                            cancellationToken).ConfigureAwait(false);

                        // Set the command properties
                        command.CommandText = context.CommandText;
                    }

                    // Set the values
                    if (batchItems.Count == 1)
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

                    // Before Execution
                    var traceResult = await Tracer
                        .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken).ConfigureAwait(false);

                    // Silent cancellation
                    if (traceResult?.CancellableTraceLog?.IsCancelled == true)
                    {
                        return result;
                    }

                    // Actual Execution
                    result += await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

                    // After Execution
                    await Tracer
                        .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        if (myTransaction is { })
            await myTransaction.CommitAsync(cancellationToken).ConfigureAwait(false);

        // Return the result
        return result;
    }

    #endregion
}
