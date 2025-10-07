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
    #region Update<TEntity>

    /// <summary>
    /// Updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The data entity object to be used for update.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static int Update<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        var key = GetAndGuardPrimaryKeyOrIdentityKey(connection, tableName, transaction,
            GetEntityType(entity));
        return UpdateInternal(connection: connection,
            tableName: tableName,
            entity: entity,
            where: ToQueryGroup(key, entity),
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The data entity object to be updated.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static int Update<TEntity, TWhat>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        TWhat what,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
        where TWhat : notnull
    {
        return UpdateInternal(connection: connection,
            tableName: tableName,
            entity: entity,
            where: WhatToQueryGroup(connection, tableName, what, transaction),
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The data entity object to be updated.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static int Update<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        object what,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return UpdateInternal(connection: connection,
            tableName: tableName,
            entity: entity,
            where: WhatToQueryGroup(connection, tableName, what, transaction),
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The data entity object to be updated.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static int Update<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        Expression<Func<TEntity, bool>> where,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return UpdateInternal(connection: connection,
            tableName: tableName,
            entity: entity,
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
    /// Updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The data entity object to be updated.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static int Update<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        QueryField where,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return UpdateInternal(connection: connection,
            tableName: tableName,
            entity: entity,
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
    /// Updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The data entity object to be updated.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static int Update<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        IEnumerable<QueryField> where,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return UpdateInternal(connection: connection,
            tableName: tableName,
            entity: entity,
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
    /// Updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The data entity object to be updated.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static int Update<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        QueryGroup where,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return UpdateInternal(connection: connection,
            tableName: tableName,
            entity: entity,
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
    /// Updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be used for update.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static int Update<TEntity>(this IDbConnection connection,
        TEntity entity,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        var key = GetAndGuardPrimaryKeyOrIdentityKey(GetEntityType(entity), connection, transaction);
        return UpdateInternal(connection: connection,
            tableName: GetMappedName(entity),
            entity: entity,
            where: ToQueryGroup(key, entity),
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be updated.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static int Update<TEntity, TWhat>(this IDbConnection connection,
        TEntity entity,
        TWhat what,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
        where TWhat : notnull
    {
        return UpdateInternal(connection: connection,
            tableName: GetMappedName(entity),
            entity: entity,
            where: WhatToQueryGroup(GetEntityType(entity), connection, what, transaction),
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be updated.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static int Update<TEntity>(this IDbConnection connection,
        TEntity entity,
        object what,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return UpdateInternal(connection: connection,
            tableName: GetMappedName(entity),
            entity: entity,
            where: WhatToQueryGroup(GetEntityType(entity), connection, what, transaction),
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be updated.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static int Update<TEntity>(this IDbConnection connection,
        TEntity entity,
        Expression<Func<TEntity, bool>> where,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return UpdateInternal(connection: connection,
            tableName: GetMappedName(entity),
            entity: entity,
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
    /// Updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be updated.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static int Update<TEntity>(this IDbConnection connection,
        TEntity entity,
        QueryField where,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return UpdateInternal(connection: connection,
            tableName: GetMappedName(entity),
            entity: entity,
            where: where != null ? new QueryGroup(where.AsEnumerable()) : null,
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// Updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be updated.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static int Update<TEntity>(this IDbConnection connection,
        TEntity entity,
        IEnumerable<QueryField> where,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return UpdateInternal(connection: connection,
            tableName: GetMappedName(entity),
            entity: entity,
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
    /// Updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be updated.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static int Update<TEntity>(this IDbConnection connection,
        TEntity entity,
        QueryGroup where,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return UpdateInternal(connection: connection,
            tableName: GetMappedName(entity),
            entity: entity,
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
    /// Updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The data entity object to be updated.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    internal static int UpdateInternal<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        QueryGroup? where,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        if (TypeCache.Get(GetEntityType(entity)).IsDictionaryStringObject)
        {
            return UpdateInternalBase(connection: connection,
                tableName: tableName,
                entity: (IDictionary<string, object?>)entity,
                where: where,
                fields: fields ?? GetQualifiedFields(entity),
                hints: hints,
                commandTimeout: commandTimeout,
            traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }
        else
        {
            return UpdateInternalBase(connection: connection,
                tableName: tableName,
                entity: entity,
                where: where,
                fields: fields ?? GetQualifiedFields(entity),
                hints: hints,
                commandTimeout: commandTimeout,
            traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }
    }

    #endregion

    #region UpdateAsync<TEntity>

    /// <summary>
    /// Updates an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The data entity object to be updated.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static async Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        var key = await GetAndGuardPrimaryKeyOrIdentityKeyAsync(connection, tableName, transaction,
            GetEntityType(entity), cancellationToken).ConfigureAwait(false);
        return await UpdateInternalAsync(connection: connection,
            tableName: tableName,
            entity: entity,
            where: ToQueryGroup(key, entity),
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
    /// Updates an existing row in the table based on the given query expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The data entity object to be updated.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static async Task<int> UpdateAsync<TEntity, TWhat>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        TWhat what,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
        where TWhat : notnull
    {
        return await UpdateInternalAsync(connection: connection,
            tableName: tableName,
            entity: entity,
            where: await WhatToQueryGroupAsync(connection, tableName, what, transaction, cancellationToken).ConfigureAwait(false),
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
    /// Updates an existing row in the table based on the given query expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The data entity object to be updated.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static async Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        object what,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await UpdateInternalAsync(connection: connection,
            tableName: tableName,
            entity: entity,
            where: await WhatToQueryGroupAsync(connection, tableName, what, transaction, cancellationToken).ConfigureAwait(false),
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
    /// Updates an existing row in the table based on the given query expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The data entity object to be updated.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static async Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        Expression<Func<TEntity, bool>> where,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await UpdateInternalAsync(connection: connection,
            tableName: tableName,
            entity: entity,
            where: connection.ToQueryGroup(where, transaction),
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
    /// Updates an existing row in the table based on the given query expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The data entity object to be updated.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static async Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        QueryField where,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await UpdateInternalAsync(connection: connection,
            tableName: tableName,
            entity: entity,
            where: where != null ? new QueryGroup(where.AsEnumerable()) : null,
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
    /// Updates an existing row in the table based on the given query expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The data entity object to be updated.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static async Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        IEnumerable<QueryField> where,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await UpdateInternalAsync(connection: connection,
            tableName: tableName,
            entity: entity,
            where: ToQueryGroup(where),
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
    /// Updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The data entity object to be updated.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static async Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        QueryGroup where,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await UpdateInternalAsync(connection: connection,
            tableName: tableName,
            entity: entity,
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

    /// <summary>
    /// Updates an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be updated.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static async Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
        TEntity entity,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        var key = await GetAndGuardPrimaryKeyOrIdentityKeyAsync(GetEntityType(entity), connection, transaction, cancellationToken).ConfigureAwait(false);
        return await UpdateInternalAsync(connection: connection,
            tableName: GetMappedName(entity),
            entity: entity,
            where: ToQueryGroup(key, entity),
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
    /// Updates an existing row in the table based on the given query expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be updated.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static async Task<int> UpdateAsync<TEntity, TWhat>(this IDbConnection connection,
        TEntity entity,
        TWhat what,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
        where TWhat : notnull
    {
        return await UpdateInternalAsync(connection: connection,
            tableName: GetMappedName(entity),
            entity: entity,
            where: await WhatToQueryGroupAsync(GetEntityType(entity), connection, what, transaction, cancellationToken).ConfigureAwait(false),
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
    /// Updates an existing row in the table based on the given query expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be updated.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static async Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
        TEntity entity,
        object what,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await UpdateInternalAsync(connection: connection,
            tableName: GetMappedName(entity),
            entity: entity,
            where: await WhatToQueryGroupAsync(GetEntityType(entity), connection, what, transaction, cancellationToken).ConfigureAwait(false),
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
    /// Updates an existing row in the table based on the given query expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be updated.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static async Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
        TEntity entity,
        Expression<Func<TEntity, bool>> where,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await UpdateInternalAsync(connection: connection,
            tableName: GetMappedName(entity),
            entity: entity,
            where: connection.ToQueryGroup(where, transaction),
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
    /// Updates an existing row in the table based on the given query expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be updated.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static async Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
        TEntity entity,
        QueryField where,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await UpdateInternalAsync(connection: connection,
            tableName: GetMappedName(entity),
            entity: entity,
            where: ToQueryGroup(where),
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
    /// Updates an existing row in the table based on the given query expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be updated.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static async Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
        TEntity entity,
        IEnumerable<QueryField> where,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await UpdateInternalAsync(connection: connection,
            tableName: GetMappedName(entity),
            entity: entity,
            where: ToQueryGroup(where),
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
    /// Updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be updated.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static async Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
        TEntity entity,
        QueryGroup where,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await UpdateInternalAsync(connection: connection,
            tableName: GetMappedName(entity),
            entity: entity,
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

    /// <summary>
    /// Updates an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The data entity object to be updated.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    internal static ValueTask<int> UpdateInternalAsync<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        QueryGroup? where,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        if (TypeCache.Get(GetEntityType(entity)).IsDictionaryStringObject)
        {
            return UpdateInternalBaseAsync(connection: connection,
                tableName: tableName,
                entity: (IDictionary<string, object?>)entity,
                where: where,
                fields: fields ?? GetQualifiedFields(entity),
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
            return UpdateInternalBaseAsync(connection: connection,
                tableName: tableName,
                entity: entity,
                where: where,
                fields: fields ?? GetQualifiedFields(entity),
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

    #region Update(TableName)

    /// <summary>
    /// Updates an existing row in the table.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The dynamic object to be used for update.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static int Update(this IDbConnection connection,
        string tableName,
        object entity,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(tableName);
        ArgumentNullException.ThrowIfNull(entity);

        var key = GetAndGuardPrimaryKeyOrIdentityKey(connection, tableName, transaction, entity.GetType());
        return UpdateInternal(connection: connection,
            tableName: tableName,
            entity: entity,
            where: ToQueryGroup(key, entity),
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            trace: trace,
            statementBuilder: statementBuilder,
            transaction: transaction);
    }

    /// <summary>
    /// Updates an existing row in the table.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The dynamic object to be used for update.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static int Update(this IDbConnection connection,
        string tableName,
        object entity,
        object where,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return UpdateInternal(connection: connection,
            tableName: tableName,
            entity: entity,
            where: ToQueryGroup(where),
            fields: fields,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            trace: trace,
            statementBuilder: statementBuilder,
            transaction: transaction);
    }

    /// <summary>
    /// Updates an existing row in the table.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The dynamic object to be used for update.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static int Update(this IDbConnection connection,
        string tableName,
        object entity,
        QueryField where,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return UpdateInternal(connection: connection,
            tableName: tableName,
            entity: entity,
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
    /// Updates an existing row in the table.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The dynamic object to be used for update.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static int Update(this IDbConnection connection,
        string tableName,
        object entity,
        IEnumerable<QueryField> where,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return UpdateInternal(connection: connection,
            tableName: tableName,
            entity: entity,
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
    /// Updates an existing row in the table.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The dynamic object to be used for update.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static int Update(this IDbConnection connection,
        string tableName,
        object entity,
        QueryGroup where,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
    {
        return UpdateInternal(connection: connection,
            tableName: tableName,
            entity: entity,
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

    #region UpdateAsync(TableName)

    /// <summary>
    /// Updates an existing row in the table in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The dynamic object to be used for update.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static async Task<int> UpdateAsync(this IDbConnection connection,
        string tableName,
        object entity,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(tableName);

        var key = await GetAndGuardPrimaryKeyOrIdentityKeyAsync(connection, tableName, transaction,
            entity.GetType(), cancellationToken).ConfigureAwait(false);
        return await UpdateInternalAsync(connection: connection,
            tableName: tableName,
            entity: entity,
            where: ToQueryGroup(key, entity),
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
    /// Updates an existing row in the table in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The dynamic object to be used for update.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static async Task<int> UpdateAsync(this IDbConnection connection,
        string tableName,
        object entity,
        object where,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await UpdateInternalAsync(connection: connection,
            tableName: tableName,
            entity: entity,
            where: ToQueryGroup(where),
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
    /// Updates an existing row in the table in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The dynamic object to be used for update.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static async Task<int> UpdateAsync(this IDbConnection connection,
        string tableName,
        object entity,
        QueryField where,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await UpdateInternalAsync(connection: connection,
            tableName: tableName,
            entity: entity,
            where: ToQueryGroup(where),
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
    /// Updates an existing row in the table in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The dynamic object to be used for update.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static async Task<int> UpdateAsync(this IDbConnection connection,
        string tableName,
        object entity,
        IEnumerable<QueryField> where,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await UpdateInternalAsync(connection: connection,
            tableName: tableName,
            entity: entity,
            where: ToQueryGroup(where),
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
    /// Updates an existing row in the table in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The dynamic object to be used for update.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public static async Task<int> UpdateAsync(this IDbConnection connection,
        string tableName,
        object entity,
        QueryGroup where,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        return await UpdateInternalAsync(connection: connection,
            tableName: tableName,
            entity: entity,
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

    #region UpdateInternalBase<TEntity>

    /// <summary>
    /// Updates an existing row in the table.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <typeparam name="TEntity">The type of the object (whether a data entity or a dynamic).</typeparam>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The data entity or dynamic object to be updated.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    internal static int UpdateInternalBase<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        QueryGroup? where,
        IEnumerable<Field> fields,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        // Set the flags
        where?.IsForUpdate();

        // Get the context
        var entityType = GetEntityType(entity);
        var context = UpdateExecutionContextProvider.Create(entityType,
            connection,
            tableName,
            where,
            fields,
            hints,
            transaction,
            statementBuilder);
        var result = 0;

        // Create the command
        using (var command = (DbCommand)connection.EnsureOpen().CreateCommand(context.CommandText,
            CommandType.Text, commandTimeout, transaction))
        {
            // Set the values
            context.ParametersSetterFunc(command, entity);

            // Add the fields from the query group
            WhereToCommandParameters(command, where, entity.GetType(),
                DbFieldCache.Get(connection, tableName, transaction));

            // Actual Execution
            result = command.ExecuteNonQueryInternal(trace, traceKey);
        }

        // Return the result
        return result;
    }

    #endregion

    #region UpdateInternalBaseAsync<TEntity>

    /// <summary>
    /// Updates an existing row in the table.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <typeparam name="TEntity">The type of the object (whether a data entity or a dynamic).</typeparam>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The data entity or dynamic object to be updated.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    internal static async ValueTask<int> UpdateInternalBaseAsync<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        QueryGroup? where,
        IEnumerable<Field> fields,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Update,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        // Set the flags
        where?.IsForUpdate();

        // Get the context
        var entityType = GetEntityType(entity);
        var context = await UpdateExecutionContextProvider.CreateAsync(entityType,
            connection,
            tableName,
            where,
            fields,
            hints,
            transaction,
            statementBuilder,
            cancellationToken).ConfigureAwait(false);
        var result = 0;

        // Create the command
        using (var command = (DbCommand)(await connection.EnsureOpenAsync(cancellationToken).ConfigureAwait(false)).CreateCommand(context.CommandText,
            CommandType.Text, commandTimeout, transaction))
        {
            // Set the values
            context.ParametersSetterFunc(command, entity);

            // Add the fields from the query group
            WhereToCommandParameters(command, where, entity.GetType(),
                await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken).ConfigureAwait(false));

            // Actual Execution
            result = await command.ExecuteNonQueryInternalAsync(trace, traceKey, cancellationToken).ConfigureAwait(false);
        }

        // Return the result
        return result;
    }

    #endregion
}
