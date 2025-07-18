using System.Data;
using System.Data.Common;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Requests;

namespace RepoDb;

/// <summary>
/// Contains the extension methods for <see cref="IDbConnection"/> object.
/// </summary>
public static partial class DbConnectionExtension
{
    #region DeleteAll<TEntity>

    /// <summary>
    /// Delete the rows from the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entities">The list of data entity objects to be deleted.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static int DeleteAll<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.DeleteAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        var key = GetAndGuardPrimaryKeyOrIdentityKey(connection, tableName, transaction, GetEntityType(entities));

        if (key.OneOrDefault() is { } one)
        {
            var keys = ExtractPropertyValues(entities, one).AsList();

            return DeleteAllInternal(connection: (DbConnection)connection,
                tableName: tableName,
                keys: keys,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }
        else
        {
            int chunkSize = connection.GetDbSetting().MaxParameterCount / key.Count();
            using var myTransaction = transaction is null && chunkSize < entities.Count() ? connection.EnsureOpen().BeginTransaction() : null;
            transaction ??= myTransaction;
            int deleted = 0;

            foreach (var group in entities.Split(chunkSize))
            {
                var where = new QueryGroup(group.Select(entity => ToQueryGroup(key, entity)), Conjunction.Or);

                where.Fix(connection, transaction, tableName);

                deleted += DeleteInternal(
                    connection: (DbConnection)connection,
                    tableName: tableName,
                    where: where,
                    hints: hints,
                    commandTimeout: commandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder);
            }

            myTransaction?.Commit();
            return deleted;
        }
    }

    /// <summary>
    /// Delete the rows from the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TKey">The type of the key column.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="keys">The list of the keys to be deleted.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static int DeleteAll<TEntity, TKey>(this IDbConnection connection,
        string tableName,
        IEnumerable<TKey> keys,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.DeleteAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return DeleteAllInternal(connection: (DbConnection)connection,
            tableName: tableName,
            keys: keys.WithType<object>(),
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Delete the rows from the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="keys">The list of the keys to be deleted.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static int DeleteAll<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<object> keys,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.DeleteAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return DeleteAllInternal(connection: (DbConnection)connection,
            tableName: tableName,
            keys: keys,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Delete the rows from the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entities">The list of data entity objects to be deleted.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static int DeleteAll<TEntity>(this IDbConnection connection,
        IEnumerable<TEntity> entities,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.DeleteAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        var key = GetAndGuardPrimaryKeyOrIdentityKey(GetEntityType(entities), connection, transaction);
        if (key.OneOrDefault() is { } one)
        {
            var keys = ExtractPropertyValues(entities, one).AsList();

            return DeleteAllInternal(connection: (DbConnection)connection,
            tableName: GetMappedName(entities),
            keys: keys.WithType<object>(),
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
        }
        else
        {
            int chunkSize = connection.GetDbSetting().MaxParameterCount / key.Count();
            using var myTransaction = transaction is null && chunkSize < entities.Count() ? connection.EnsureOpen().BeginTransaction() : null;
            transaction ??= myTransaction;
            int deleted = 0;

            foreach (var group in entities.Split(chunkSize))
            {
                var where = new QueryGroup(group.Select(entity => ToQueryGroup(key, entity)), Conjunction.Or);

                string tableName = GetMappedName(entities);
                where.Fix(connection, transaction, tableName);

                deleted += DeleteInternal(
                    connection: (DbConnection)connection,
                    tableName: tableName,
                    where: where,
                    hints: hints,
                    commandTimeout: commandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder);
            }

            myTransaction?.Commit();
            return deleted;
        }
    }

    /// <summary>
    /// Delete the rows from the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TKey">The type of the key column.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="keys">The list of the keys to be deleted.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static int DeleteAll<TEntity, TKey>(this IDbConnection connection,
        IEnumerable<TKey> keys,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.DeleteAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return DeleteAllInternal(connection: (DbConnection)connection,
            tableName: ClassMappedNameCache.Get<TEntity>() ?? throw new ArgumentException($"Can't map {typeof(TEntity)} to tablename"),
            keys: keys.WithType<object>(),
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Delete the rows from the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="keys">The list of the keys to be deleted.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static int DeleteAll<TEntity>(this IDbConnection connection,
        IEnumerable<object> keys,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.DeleteAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return DeleteAllInternal(connection: (DbConnection)connection,
            tableName: ClassMappedNameCache.Get<TEntity>() ?? throw new ArgumentException($"Can't map {typeof(TEntity)} to tablename"),
            keys: keys,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Delete all the rows from the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static int DeleteAll<TEntity>(this IDbConnection connection,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.DeleteAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return DeleteAllInternal<TEntity>(connection: (DbConnection)connection,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Delete all the rows from the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    internal static int DeleteAllInternal<TEntity>(this IDbConnection connection,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.DeleteAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        // Variables
        var request = new DeleteAllRequest(typeof(TEntity),
            connection,
            transaction,
            hints,
            statementBuilder);

        // Return the result
        return DeleteAllInternalBase(connection: (DbConnection)connection,
            request: request,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace);
    }

    #endregion

    #region DeleteAllAsync<TEntity>

    /// <summary>
    /// Delete the rows from the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entities">The list of data entity objects to be deleted.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static async Task<int> DeleteAllAsync<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.DeleteAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entities);

        entities = entities.AsList();
        if (entities.Any() != true)
        {
            return default;
        }

        var key = await GetAndGuardPrimaryKeyOrIdentityKeyAsync(connection, tableName, transaction, GetEntityType(entities), cancellationToken).ConfigureAwait(false);
        if (key.OneOrDefault() is { } one)
        {
            var keys = ExtractPropertyValues(entities, one).AsList();

            return await DeleteAllInternalAsync(connection: (DbConnection)connection,
            tableName: tableName,
            keys: keys,
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
            int chunkSize = connection.GetDbSetting().MaxParameterCount / key.Count();

            await connection.EnsureOpenAsync(cancellationToken).ConfigureAwait(false);
            using var myTransaction = transaction is null && chunkSize < entities.Count() ? await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false) : null;

            transaction ??= myTransaction;
            int deleted = 0;

            foreach (var group in entities.Split(chunkSize))
            {
                var where = new QueryGroup(group.Select(entity => ToQueryGroup(key, entity)), Conjunction.Or);

                where.Fix(connection, transaction, tableName);

                deleted += await DeleteInternalAsync(
                    connection: (DbConnection)connection,
                    tableName: tableName,
                    where: where,
                    hints: hints,
                    commandTimeout: commandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            if (myTransaction is { })
                await myTransaction.CommitAsync(cancellationToken).ConfigureAwait(false);

            return deleted;
        }
    }

    /// <summary>
    /// Delete the rows from the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TKey">The type of the key column.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="keys">The list of the keys to be deleted.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static async Task<int> DeleteAllAsync<TEntity, TKey>(this IDbConnection connection,
        string tableName,
        IEnumerable<TKey> keys,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.DeleteAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await DeleteAllInternalAsync(connection: (DbConnection)connection,
            tableName: tableName,
            keys: keys.WithType<object>(),
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Delete the rows from the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="keys">The list of the keys to be deleted.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static async Task<int> DeleteAllAsync<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<object> keys,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.DeleteAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await DeleteAllInternalAsync(connection: (DbConnection)connection,
            tableName: tableName,
            keys: keys,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Delete all the rows from the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entities">The list of data entity objects to be deleted.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static async Task<int> DeleteAllAsync<TEntity>(this IDbConnection connection,
        IEnumerable<TEntity> entities,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.DeleteAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entities);

        entities = entities.AsList();
        if (entities.Any() != true)
        {
            return default;
        }

        string tableName = GetMappedName(entities);
        var key = await GetAndGuardPrimaryKeyOrIdentityKeyAsync(GetEntityType(entities), connection, transaction, cancellationToken).ConfigureAwait(false);
        if (key.OneOrDefault() is { } one)
        {
            var keys = ExtractPropertyValues(entities, one).AsList();

            return await DeleteAllInternalAsync(connection: (DbConnection)connection,
                tableName: tableName,
                keys: keys.WithType<object>(),
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
            int chunkSize = connection.GetDbSetting().MaxParameterCount / key.Count();

            await connection.EnsureOpenAsync(cancellationToken).ConfigureAwait(false);
            using var myTransaction = transaction is null && chunkSize < entities.Count() ? await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false) : null;

            transaction ??= myTransaction;
            int deleted = 0;

            foreach (var group in entities.Split(chunkSize))
            {
                var where = new QueryGroup(group.Select(entity => ToQueryGroup(key, entity)), Conjunction.Or);

                where.Fix(connection, transaction, tableName);

                deleted += await DeleteInternalAsync(
                    connection: (DbConnection)connection,
                    tableName: tableName,
                    where: where,
                    hints: hints,
                    commandTimeout: commandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            if (myTransaction is { })
                await myTransaction.CommitAsync(cancellationToken).ConfigureAwait(false);

            return deleted;
        }
    }

    /// <summary>
    /// Delete all the rows from the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TKey">The type of the key column.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="keys">The list of the keys to be deleted.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static async Task<int> DeleteAllAsync<TEntity, TKey>(this IDbConnection connection,
        IEnumerable<TKey> keys,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.DeleteAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await DeleteAllInternalAsync(
            connection: (DbConnection)connection,
            tableName: ClassMappedNameCache.Get<TEntity>() ?? throw new ArgumentException($"Can't map {typeof(TEntity)} to tablename"),
            keys: keys.WithType<object>(),
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Delete all the rows from the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="keys">The list of the keys to be deleted.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static async Task<int> DeleteAllAsync<TEntity>(this IDbConnection connection,
        IEnumerable<object> keys,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.DeleteAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await DeleteAllInternalAsync(connection: (DbConnection)connection,
            tableName: ClassMappedNameCache.Get<TEntity>() ?? throw new ArgumentException($"Can't map {typeof(TEntity)} to tablename"),
            keys: keys,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Delete all the rows from the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static async Task<int> DeleteAllAsync<TEntity>(this IDbConnection connection,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.DeleteAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await DeleteAllInternalAsync<TEntity>(connection: (DbConnection)connection,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Delete all the rows from the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    internal static ValueTask<int> DeleteAllInternalAsync<TEntity>(this IDbConnection connection,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.DeleteAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        // Variables
        var request = new DeleteAllRequest(typeof(TEntity),
            connection,
            transaction,
            hints,
            statementBuilder);

        // Return the result
        return DeleteAllInternalBaseAsync(connection: (DbConnection)connection,
            request: request,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            cancellationToken: cancellationToken);
    }

    #endregion

    #region DeleteAll(TableName)

    /// <summary>
    /// Delete the rows from the table.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="keys">The list of the keys to be deleted.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static int DeleteAll(this IDbConnection connection,
        string tableName,
        IEnumerable<object> keys,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.DeleteAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return DeleteAllInternal(connection: (DbConnection)connection,
            tableName: tableName,
            keys: keys,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Delete all the rows from the table.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static int DeleteAll(this IDbConnection connection,
        string tableName,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.DeleteAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return DeleteAllInternal(connection: (DbConnection)connection,
            tableName: tableName,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Delete all the rows from the table.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    internal static int DeleteAllInternal(this IDbConnection connection,
        string tableName,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.DeleteAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        // Variables
        var request = new DeleteAllRequest(tableName,
            connection,
            transaction,
            hints,
            statementBuilder);

        // Return the result
        return DeleteAllInternalBase(connection: (DbConnection)connection,
            request: request,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace);
    }

    /// <summary>
    /// Delete the rows from the table.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="keys">The list of the keys to be deleted.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    internal static int DeleteAllInternal(this IDbConnection connection,
        string tableName,
        IEnumerable<object> keys,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.DeleteAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        ArgumentNullException.ThrowIfNull(keys);

        keys = keys.AsList();
        if (keys.Any() != true)
        {
            return default;
        }

        var keyField = GetAndGuardPrimaryKeyOrIdentityKey(connection, tableName, transaction).Single();
        var dbSetting = connection.GetDbSetting();
        var count = keys.Count();
        var deletedRows = 0;

        var parameterBatchCount = connection.GetDbSetting().MaxParameterCount;

        using var myTransaction = transaction is null && count > parameterBatchCount ? connection.EnsureOpen().BeginTransaction() : null;
        transaction ??= myTransaction;

        if (count > dbSetting.UseArrayParameterTreshold
            && connection.GetDbHelper().CanCreateTableParameter(connection, transaction, keyField.Type, keys))
        {
            parameterBatchCount = connection.GetDbSetting().MaxArrayParameterValueCount;
        }

        // Call the underlying method
        foreach (var keyValues in keys.Split(parameterBatchCount) ?? [])
        {
            if (keyValues.Length == 0)
                continue;

            var where = new QueryGroup(new QueryField(keyField, Operation.In, keyValues, null, false));

            where.Fix(connection, transaction, tableName);

            deletedRows += DeleteInternal(connection: (DbConnection)connection,
                tableName: tableName,
                where: where,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        // Commit the transaction
        myTransaction?.Commit();


        // Return the value
        return deletedRows;
    }

    #endregion

    #region DeleteAllAsync(TableName)

    /// <summary>
    /// Delete all the rows from the table in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="keys">The list of the keys to be deleted.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static async Task<int> DeleteAllAsync(this IDbConnection connection,
        string tableName,
        IEnumerable<object> keys,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.DeleteAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await DeleteAllInternalAsync(connection: (DbConnection)connection,
            tableName: tableName,
            keys: keys,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Delete all the rows from the table in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static async Task<int> DeleteAllAsync(this IDbConnection connection,
        string tableName,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.DeleteAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await DeleteAllInternalAsync(connection: (DbConnection)connection,
            tableName: tableName,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Delete all the rows from the table in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    internal static ValueTask<int> DeleteAllInternalAsync(this IDbConnection connection,
        string tableName,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.DeleteAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        // Variables
        var request = new DeleteAllRequest(tableName,
            connection,
            transaction,
            hints,
            statementBuilder);

        // Return the result
        return DeleteAllInternalBaseAsync(connection: (DbConnection)connection,
            request: request,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Delete all the rows from the table in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="keys">The list of the keys to be deleted.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    internal static async ValueTask<int> DeleteAllInternalAsync(this IDbConnection connection,
        string tableName,
        IEnumerable<object> keys,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.DeleteAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);

        keys = keys.AsList();
        if (keys.Any() != true)
        {
            return default;
        }

        var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken).ConfigureAwait(false);

        var keyField = dbFields.PrimaryFields?.OneOrDefault() ?? dbFields?.Identity ?? dbFields.PrimaryFields?.FirstOrDefault() ?? throw GetKeyFieldNotFoundException(tableName);

        var dbSetting = connection.GetDbSetting();
        var count = keys.Count();
        var deletedRows = 0;

        await connection.EnsureOpenAsync(cancellationToken).ConfigureAwait(false);
        var parameterBatchCount = connection.GetDbSetting().MaxParameterCount;
        using var myTransaction = transaction is null && count > parameterBatchCount ? await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false) : null;
        transaction ??= myTransaction;

        if (count > dbSetting.UseArrayParameterTreshold
            && connection.GetDbHelper().CanCreateTableParameter(connection, transaction, keyField.Type, keys))
        {
            parameterBatchCount = connection.GetDbSetting().MaxArrayParameterValueCount;
        }

        // Call the underlying method
        foreach (var keyValues in keys.Split(parameterBatchCount) ?? [])
        {
            var where = new QueryGroup(new QueryField(keyField, Operation.In, keyValues.AsList(), null, false));

            where.Fix(connection, transaction, tableName);

            deletedRows += await DeleteInternalAsync(connection: (DbConnection)connection,
                tableName: tableName,
                where: where,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        if (myTransaction is { })
            await myTransaction.CommitAsync(cancellationToken).ConfigureAwait(false);

        // Return the value
        return deletedRows;
    }

    #endregion

    #region DeleteAllInternalBase

    /// <summary>
    /// Delete all the rows from the table.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="request">The actual <see cref="DeleteAllRequest"/> object.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    internal static int DeleteAllInternalBase(this IDbConnection connection,
        DeleteAllRequest request,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.DeleteAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null)
    {
        // Variables
        var commandType = CommandType.Text;
        var commandText = CommandTextCache.GetDeleteAllText(request);

        // Actual Execution
        var result = ExecuteNonQueryInternal(connection: (DbConnection)connection,
            commandText: commandText,
            param: null,
            commandType: commandType,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            entityType: request.Type,
            dbFields: DbFieldCache.Get(connection, request.Name, transaction, true),
            skipCommandArrayParametersCheck: true);

        // Result
        return result;
    }

    #endregion

    #region DeleteAllInternalBaseAsync

    /// <summary>
    /// Delete all the rows from the table.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="request">The actual <see cref="DeleteAllRequest"/> object.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    internal static async ValueTask<int> DeleteAllInternalBaseAsync(this IDbConnection connection,
        DeleteAllRequest request,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.DeleteAll,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        CancellationToken cancellationToken = default)
    {
        // Variables
        var commandType = CommandType.Text;
        var commandText = CommandTextCache.GetDeleteAllText(request);

        // Actual Execution
        var result = await ExecuteNonQueryInternalAsync(connection: (DbConnection)connection,
            commandText: commandText,
            param: null,
            commandType: commandType,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            entityType: request.Type,
            dbFields: await DbFieldCache.GetAsync(connection, request.Name, transaction, true, cancellationToken).ConfigureAwait(false),
            skipCommandArrayParametersCheck: true,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        // Result
        return result;
    }

    #endregion
}
