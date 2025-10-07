using System.Data;
using System.Data.Common;
using System.Transactions;
using RepoDb.Contexts.Execution;
using RepoDb.Contexts.Providers;
using RepoDb.DbSettings;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.StatementBuilders;

namespace RepoDb;

/// <summary>
/// Contains the extension methods for <see cref="IDbConnection"/> object.
/// </summary>
public static partial class DbConnectionExtension
{
    #region InsertAll<TEntity>

    /// <summary>
    /// Insert multiple rows in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity objects.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of data entity objects to be inserted.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of inserted rows in the table.</returns>
    public static int InsertAll<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.InsertAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return InsertAllInternal(connection: connection,
            tableName: tableName,
            entities: entities,
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
    /// Insert multiple rows in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity objects.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entities">The list of data entity objects to be inserted.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of inserted rows in the table.</returns>
    public static int InsertAll<TEntity>(this IDbConnection connection,
        IEnumerable<TEntity> entities,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.InsertAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return InsertAllInternal(connection: connection,
            tableName: GetMappedName(entities),
            entities: entities,
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
    /// Insert multiple rows in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of data entity objects to be inserted.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of inserted rows in the table.</returns>
    internal static int InsertAllInternal<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.InsertAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        if (TypeCache.Get(GetEntityType(entities)).IsDictionaryStringObject)
        {
            return InsertAllInternalBase(connection: connection,
                tableName: tableName,
                entities: entities.WithType<IDictionary<string, object?>>(),
                batchSize: batchSize,
                fields: fields ?? GetQualifiedFields(entities?.FirstOrDefault()),
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }
        else
        {
            return InsertAllInternalBase(connection: connection,
                tableName: tableName,
                entities: entities,
                batchSize: batchSize,
                fields: fields ?? GetQualifiedFields(entities?.FirstOrDefault()),
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }
    }

    #endregion

    #region InsertAllAsync<TEntity>

    /// <summary>
    /// Insert multiple rows in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity objects.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of data entity objects to be inserted.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of inserted rows in the table.</returns>
    public static async Task<int> InsertAllAsync<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.InsertAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await InsertAllInternalAsync(connection: connection,
            tableName: tableName,
            entities: entities,
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
    /// Insert multiple rows in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entities">The list of data entity objects to be inserted.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of inserted rows in the table.</returns>
    public static async Task<int> InsertAllAsync<TEntity>(this IDbConnection connection,
        IEnumerable<TEntity> entities,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.InsertAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await InsertAllInternalAsync(connection: connection,
            tableName: GetMappedName(entities),
            entities: entities,
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
    /// Insert multiple rows in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of data entity objects to be inserted.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of inserted rows in the table.</returns>
    internal static ValueTask<int> InsertAllInternalAsync<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.InsertAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        if (TypeCache.Get(GetEntityType(entities)).IsDictionaryStringObject)
        {
            return InsertAllInternalBaseAsync(connection: connection,
                tableName: tableName,
                entities: entities.WithType<IDictionary<string, object?>>(),
                batchSize: batchSize,
                fields: fields ?? GetQualifiedFields(entities?.FirstOrDefault()),
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
            return InsertAllInternalBaseAsync(connection: connection,
                tableName: tableName,
                entities: entities,
                batchSize: batchSize,
                fields: fields ?? GetQualifiedFields(entities?.FirstOrDefault()),
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

    #region InsertAll(TableName)

    /// <summary>
    /// Insert multiple rows in the table. By default, the table fields are used unless the 'fields' argument is defined.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of dynamic objects to be inserted.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of inserted rows in the table.</returns>
    public static int InsertAll(this IDbConnection connection,
        string tableName,
        IEnumerable<object> entities,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.InsertAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return InsertAllInternal(connection: connection,
            tableName: tableName,
            entities: entities,
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

    #region InsertAllAsync(TableName)

    /// <summary>
    /// Insert multiple rows in the table in an asynchronous way. By default, the table fields are used unless the 'fields' argument is defined.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of dynamic objects to be inserted.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of inserted rows in the table.</returns>
    public static async Task<int> InsertAllAsync(this IDbConnection connection,
        string tableName,
        IEnumerable<object> entities,
        int batchSize = 0,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.InsertAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await InsertAllInternalAsync(connection: connection,
            tableName: tableName,
            entities: entities,
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

    #region InsertAllInternalBase<TEntity>

    /// <summary>
    /// Insert multiple rows in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the object (whether a data entity or a dynamic).</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of data entity or dynamic objects to be inserted.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of inserted rows in the table.</returns>
    internal static int InsertAllInternalBase<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        int batchSize,
        IEnumerable<Field> fields,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.InsertAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        // Variables needed
        var dbSetting = connection.GetDbSetting();

        entities = entities.AsList(); // Ensure the entities are enumerated as a list for performance

        // Guard the parameters
        if (!entities.Any())
        {
            return default;
        }

        // Validate the batch size
        int maxBatchSize = dbSetting.IsMultiStatementExecutable
            ? Math.Min(batchSize <= 0 ? dbSetting.MaxParameterCount / fields.Count() : batchSize, dbSetting.MaxQueriesInBatchCount)
            : 1;
        batchSize = Math.Min(batchSize <= 0 ? Constant.DefaultBatchOperationSize : batchSize, entities.Count());

        // Get the context
        var entityType = GetEntityType(entities);
        InsertAllExecutionContext context = InsertAllExecutionContextProvider.Create(entityType,
            connection,
            tableName,
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
                BaseDbHelper? dbh = null;

                foreach (var entity in entities.AsList())
                {
                    // Set the values
                    context.SingleDataEntityParametersSetterFunc?.Invoke(command, entity);

                    var fetchIdentity = (dbh ??= (BaseDbHelper)GetDbHelper(connection!)).PrepareForIdentityOutput(command);
                    // Prepare the command
                    if (dbSetting.IsPreparable)
                    {
                        command.Prepare();
                    }

                    // Actual Execution
                    object? returnValue;

                    if (fetchIdentity is not { })
                        returnValue = Converter.DbNullToNull(command.ExecuteScalarInternal(trace, traceKey));
                    else
                    {
                        command.ExecuteNonQueryInternal(trace, traceKey);

                        returnValue = fetchIdentity();
                    }

                    // Set the return value
                    if (returnValue != null)
                    {
                        context.IdentitySetterFunc?.Invoke(entity, returnValue);
                    }

                    // Iterate the result
                    result++;
                }
            }
            else
            {
                BaseDbHelper? dbh = null;
                int? positionIndex = null;
                bool doPrepare = dbSetting.IsPreparable;

                foreach (var batchItems in entities.ChunkOptimally(maxBatchSize))
                {
                    if (batchItems.Count != context.BatchSize)
                    {
                        // Get a new execution context from cache
                        context = InsertAllExecutionContextProvider.Create(entityType,
                            connection,
                            tableName,
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

                    var fetchIdentity = (dbh ??= (BaseDbHelper)GetDbHelper(command.Connection!)).PrepareForIdentityOutput(command);

                    // Prepare the command
                    if (doPrepare)
                    {
                        command.Prepare();
                        doPrepare = false;
                    }

                    // Actual Execution
                    if (context.IdentitySetterFunc == null || fetchIdentity is { })
                    {
                        // No identity setters
                        result += command.ExecuteNonQueryInternal(trace, traceKey);

                        if (context.IdentitySetterFunc is { } && fetchIdentity is { })
                        {
                            var position = 0;

                            foreach (var value in fetchIdentity() as System.Collections.IEnumerable ?? Array.Empty<object>())
                            {
                                context.IdentitySetterFunc.Invoke(batchItems.GetAt(position++), value);
                            }
                        }
                    }
                    else
                    {
                        // Set the identity back
                        using var reader = command.ExecuteReaderInternal(trace, traceKey);

                        // Get the results
                        var position = 0;
                        do
                        {
                            while (position < batchItems.Count && reader.Read())
                            {
                                var value = Converter.DbNullToNull(reader.GetValue(0));
                                if (value is { })
                                {
                                    positionIndex ??= (reader.FieldCount > 1) && string.Equals(BaseStatementBuilder.RepoDbOrderColumn, reader.GetName(reader.FieldCount - 1), StringComparison.OrdinalIgnoreCase) ? reader.FieldCount - 1 : -1;
                                    var index = positionIndex >= 0 && positionIndex < reader.FieldCount ? reader.GetInt32(positionIndex.Value) : position;
                                    context.IdentitySetterFunc.Invoke(batchItems.GetAt(index), value);
                                }
                                position++;
                            }
                        }
                        while (position < batchItems.Count && reader.NextResult());

                        // Set the result
                        result += batchItems.Count;
                    }
                }
            }
        }

        myTransaction?.Commit();

        // Return the result
        return result;
    }

    #endregion

    #region InsertAllInternalBaseAsync<TEntity>

    /// <summary>
    /// Insert multiple rows in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the object (whether a data entity or a dynamic).</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of data entity or dynamic objects to be inserted.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of inserted rows in the table.</returns>
    internal static async ValueTask<int> InsertAllInternalBaseAsync<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        int batchSize,
        IEnumerable<Field> fields,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.InsertAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        // Variables needed
        var dbSetting = connection.GetDbSetting();

        entities = entities.AsList(); // Ensure the entities are enumerated as a list for performance

        // Guard the parameters
        if (!entities.Any())
        {
            return default;
        }

        // Validate the batch size
        int maxBatchSize = dbSetting.IsMultiStatementExecutable
            ? Math.Min(batchSize <= 0 ? dbSetting.MaxParameterCount / fields.Count() : batchSize, dbSetting.MaxQueriesInBatchCount)
            : 1;
        batchSize = Math.Min(batchSize <= 0 ? Constant.DefaultBatchOperationSize : batchSize, entities.Count());

        await connection.EnsureOpenAsync(cancellationToken).ConfigureAwait(false);
        using var myTransaction = (transaction is null && Transaction.Current is null) ? await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false) : null;
        transaction ??= myTransaction;

        // Get the context
        var entityType = GetEntityType(entities);
        var context = await InsertAllExecutionContextProvider.CreateAsync(entityType,
            connection,
            tableName,
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
                BaseDbHelper? dbh = null;
                foreach (var entity in entities.AsList())
                {
                    // Set the values
                    context.SingleDataEntityParametersSetterFunc?.Invoke(command, entity);

                    var fetchIdentity = (dbh ??= (BaseDbHelper)GetDbHelper(command.Connection!)).PrepareForIdentityOutput(command);

                    // Prepare the command
                    if (dbSetting.IsPreparable)
                    {
                        await command.PrepareAsync(cancellationToken).ConfigureAwait(false);
                    }

                    // Actual Execution
                    object? returnValue;

                    if (fetchIdentity is not { })
                        returnValue = Converter.DbNullToNull(await command.ExecuteScalarInternalAsync(trace, traceKey, cancellationToken).ConfigureAwait(false));
                    else
                    {
                        await command.ExecuteNonQueryInternalAsync(trace, traceKey, cancellationToken).ConfigureAwait(false);
                        returnValue = Converter.DbNullToNull(fetchIdentity());
                    }

                    // Set the return value
                    if (returnValue != null)
                    {
                        context.IdentitySetterFunc?.Invoke(entity, returnValue);
                    }

                    // Iterate the result
                    result++;
                }
            }
            else
            {
                BaseDbHelper? dbh = null;
                int? positionIndex = null;
                bool doPrepare = dbSetting.IsPreparable;

                foreach (var batchItems in entities.ChunkOptimally(maxBatchSize))
                {
                    if (batchItems.Count != context.BatchSize)
                    {
                        // Get a new execution context from cache
                        context = await InsertAllExecutionContextProvider.CreateAsync(entityType,
                            connection,
                            tableName,
                            batchItems.Count,
                            fields,
                            hints,
                            transaction,
                            statementBuilder,
                            cancellationToken).ConfigureAwait(false);

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

                    var fetchIdentity = (dbh ??= (BaseDbHelper)GetDbHelper(command.Connection!)).PrepareForIdentityOutput(command);

                    if (doPrepare)
                    {
                        await command.PrepareAsync(cancellationToken).ConfigureAwait(false);
                        doPrepare = false;
                    }

                    // Actual Execution
                    if (context.IdentitySetterFunc == null || fetchIdentity is { })
                    {
                        // No identity setters
                        result += await command.ExecuteNonQueryInternalAsync(trace, traceKey, cancellationToken).ConfigureAwait(false);

                        if (context.IdentitySetterFunc is { } && fetchIdentity is { })
                        {
                            var position = 0;

                            foreach (var value in fetchIdentity() as System.Collections.IEnumerable ?? Array.Empty<object>())
                            {
                                context.IdentitySetterFunc.Invoke(batchItems.GetAt(position++), value);
                            }
                        }
                    }
                    else
                    {
                        // Set the identity back
#if NET
                        await
#endif
                        using var reader = await command.ExecuteReaderInternalAsync(trace, traceKey, cancellationToken).ConfigureAwait(false);

                        // Get the results
                        var position = 0;
                        do
                        {
                            while (position < batchItems.Count && await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                            {
                                var value = Converter.DbNullToNull(reader.GetValue(0));
                                if (value is { })
                                {
                                    positionIndex ??= (reader.FieldCount > 1) && string.Equals(BaseStatementBuilder.RepoDbOrderColumn, reader.GetName(reader.FieldCount - 1), StringComparison.OrdinalIgnoreCase) ? reader.FieldCount - 1 : -1;
                                    var index = positionIndex >= 0 && positionIndex < reader.FieldCount ? reader.GetInt32(positionIndex.Value) : position;
                                    context.IdentitySetterFunc.Invoke(batchItems.GetAt(index), value);
                                }
                                position++;
                            }
                        }
                        while (position < batchItems.Count && await reader.NextResultAsync(cancellationToken).ConfigureAwait(false));

                        // Set the result
                        result += batchItems.Count;
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
}
