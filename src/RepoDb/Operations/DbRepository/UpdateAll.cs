using System.Data;
using System.Data.Common;
using System.Linq.Expressions;

namespace RepoDb;

public partial class DbRepository<TDbConnection> : IDisposable
    where TDbConnection : DbConnection, new()
{
    #region UpdateAll<TEntity>

    /// <summary>
    /// Update the existing rows in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of data entity objects to be used for update.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public int UpdateAll<TEntity>(string tableName,
        IEnumerable<TEntity> entities,
        int batchSize = 0,
        FieldSet fields = default,
        string? hints = null,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null)
        where TEntity : class
    {
        // Create a connection
        var connection = (transaction?.Connection ?? CreateConnection());

        try
        {
            // Call the method
            return connection.UpdateAll(tableName: tableName,
                entities: entities,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: CommandTimeout ?? 0,
                traceKey: traceKey,
                transaction: transaction,
                trace: Trace,
                statementBuilder: StatementBuilder);
        }
        finally
        {
            // Dispose the connection
            DisposeConnectionForPerCall(connection, transaction);
        }
    }

    /// <summary>
    /// Update the existing rows in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of data entity objects to be used for update.</param>
    /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public int UpdateAll<TEntity>(string tableName,
        IEnumerable<TEntity> entities,
        FieldSet? qualifiers,
        int batchSize = 0,
        FieldSet fields = default,
        string? hints = null,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null)
        where TEntity : class
    {
        // Create a connection
        var connection = (transaction?.Connection ?? CreateConnection());

        try
        {
            // Call the method
            return connection.UpdateAll(tableName: tableName,
                entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: CommandTimeout ?? 0,
                traceKey: traceKey,
                transaction: transaction,
                trace: Trace,
                statementBuilder: StatementBuilder);
        }
        finally
        {
            // Dispose the connection
            DisposeConnectionForPerCall(connection, transaction);
        }
    }

    /// <summary>
    /// Update the existing rows in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of data entity objects to be used for update.</param>
    /// <param name="qualifiers">The expression for the qualifier fields.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public int UpdateAll<TEntity>(string tableName,
        IEnumerable<TEntity> entities,
        Expression<Func<TEntity, object?>> qualifiers,
        int batchSize = 0,
        FieldSet fields = default,
        string? hints = null,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null)
        where TEntity : class
    {
        // Create a connection
        var connection = (transaction?.Connection ?? CreateConnection());

        try
        {
            // Call the method
            return connection.UpdateAll(tableName: tableName,
                entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: CommandTimeout ?? 0,
                traceKey: traceKey,
                transaction: transaction,
                trace: Trace,
                statementBuilder: StatementBuilder);
        }
        finally
        {
            // Dispose the connection
            DisposeConnectionForPerCall(connection, transaction);
        }
    }

    /// <summary>
    /// Update the existing rows in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="entities">The list of data entity objects to be used for update.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public int UpdateAll<TEntity>(IEnumerable<TEntity> entities,
        int batchSize = 0,
        FieldSet fields = default,
        string? hints = null,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null)
        where TEntity : class
    {
        // Create a connection
        var connection = (transaction?.Connection ?? CreateConnection());

        try
        {
            // Call the method
            return connection.UpdateAll(entities: entities,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: CommandTimeout ?? 0,
                traceKey: traceKey,
                transaction: transaction,
                trace: Trace,
                statementBuilder: StatementBuilder);
        }
        finally
        {
            // Dispose the connection
            DisposeConnectionForPerCall(connection, transaction);
        }
    }

    /// <summary>
    /// Update the existing rows in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="entities">The list of data entity objects to be used for update.</param>
    /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public int UpdateAll<TEntity>(IEnumerable<TEntity> entities,
        FieldSet? qualifiers,
        int batchSize = 0,
        FieldSet fields = default,
        string? hints = null,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null)
        where TEntity : class
    {
        // Create a connection
        var connection = (transaction?.Connection ?? CreateConnection());

        try
        {
            // Call the method
            return connection.UpdateAll(entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: CommandTimeout ?? 0,
                traceKey: traceKey,
                transaction: transaction,
                trace: Trace,
                statementBuilder: StatementBuilder);
        }
        finally
        {
            // Dispose the connection
            DisposeConnectionForPerCall(connection, transaction);
        }
    }

    /// <summary>
    /// Update the existing rows in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="entities">The list of data entity objects to be used for update.</param>
    /// <param name="qualifiers">The expression for the qualifier fields.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public int UpdateAll<TEntity>(IEnumerable<TEntity> entities,
        Expression<Func<TEntity, object?>> qualifiers,
        int batchSize = 0,
        FieldSet fields = default,
        string? hints = null,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null)
        where TEntity : class
    {
        // Create a connection
        var connection = (transaction?.Connection ?? CreateConnection());

        try
        {
            // Call the method
            return connection.UpdateAll(entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: CommandTimeout ?? 0,
                traceKey: traceKey,
                transaction: transaction,
                trace: Trace,
                statementBuilder: StatementBuilder);
        }
        finally
        {
            // Dispose the connection
            DisposeConnectionForPerCall(connection, transaction);
        }
    }

    #endregion

    #region UpdateAllAsync<TEntity>

    /// <summary>
    /// Update the existing rows in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of data entity objects to be used for update.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public async Task<int> UpdateAllAsync<TEntity>(string tableName,
        IEnumerable<TEntity> entities,
        int batchSize = 0,
        FieldSet fields = default,
        string? hints = null,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        // Create a connection
        var connection = (transaction?.Connection ?? CreateConnection());

        try
        {
            // Call the method
            return await connection.UpdateAllAsync(tableName: tableName,
                entities: entities,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: CommandTimeout ?? 0,
                traceKey: traceKey,
                transaction: transaction,
                trace: Trace,
                statementBuilder: StatementBuilder,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            // Dispose the connection
            DisposeConnectionForPerCall(connection, transaction);
        }
    }

    /// <summary>
    /// Update the existing rows in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of data entity objects to be used for update.</param>
    /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public async Task<int> UpdateAllAsync<TEntity>(string tableName,
        IEnumerable<TEntity> entities,
        FieldSet? qualifiers,
        int batchSize = 0,
        FieldSet fields = default,
        string? hints = null,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        // Create a connection
        var connection = (transaction?.Connection ?? CreateConnection());

        try
        {
            // Call the method
            return await connection.UpdateAllAsync(tableName: tableName,
                entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: CommandTimeout ?? 0,
                traceKey: traceKey,
                transaction: transaction,
                trace: Trace,
                statementBuilder: StatementBuilder,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            // Dispose the connection
            DisposeConnectionForPerCall(connection, transaction);
        }
    }

    /// <summary>
    /// Update the existing rows in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entities">The list of data entity objects to be used for update.</param>
    /// <param name="qualifiers">The expression for the qualifier fields.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public async Task<int> UpdateAllAsync<TEntity>(string tableName,
        IEnumerable<TEntity> entities,
        Expression<Func<TEntity, object?>> qualifiers,
        int batchSize = 0,
        FieldSet fields = default,
        string? hints = null,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        // Create a connection
        var connection = (transaction?.Connection ?? CreateConnection());

        try
        {
            // Call the method
            return await connection.UpdateAllAsync(tableName: tableName,
                entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: CommandTimeout ?? 0,
                traceKey: traceKey,
                transaction: transaction,
                trace: Trace,
                statementBuilder: StatementBuilder,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            // Dispose the connection
            DisposeConnectionForPerCall(connection, transaction);
        }
    }

    /// <summary>
    /// Update the existing rows in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="entities">The list of data entity objects to be used for update.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public async Task<int> UpdateAllAsync<TEntity>(IEnumerable<TEntity> entities,
        int batchSize = 0,
        FieldSet fields = default,
        string? hints = null,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        // Create a connection
        var connection = (transaction?.Connection ?? CreateConnection());

        try
        {
            // Call the method
            return await connection.UpdateAllAsync(entities: entities,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: CommandTimeout ?? 0,
                traceKey: traceKey,
                transaction: transaction,
                trace: Trace,
                statementBuilder: StatementBuilder,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            // Dispose the connection
            DisposeConnectionForPerCall(connection, transaction);
        }
    }

    /// <summary>
    /// Update the existing rows in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="entities">The list of data entity objects to be used for update.</param>
    /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public async Task<int> UpdateAllAsync<TEntity>(IEnumerable<TEntity> entities,
        FieldSet? qualifiers,
        int batchSize = 0,
        FieldSet fields = default,
        string? hints = null,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        // Create a connection
        var connection = (transaction?.Connection ?? CreateConnection());

        try
        {
            // Call the method
            return await connection.UpdateAllAsync(entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: CommandTimeout ?? 0,
                traceKey: traceKey,
                transaction: transaction,
                trace: Trace,
                statementBuilder: StatementBuilder,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            // Dispose the connection
            DisposeConnectionForPerCall(connection, transaction);
        }
    }

    /// <summary>
    /// Update the existing rows in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="entities">The list of data entity objects to be used for update.</param>
    /// <param name="qualifiers">The expression for the qualifier fields.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public async Task<int> UpdateAllAsync<TEntity>(IEnumerable<TEntity> entities,
        Expression<Func<TEntity, object?>> qualifiers,
        int batchSize = 0,
        FieldSet fields = default,
        string? hints = null,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        // Create a connection
        var connection = (transaction?.Connection ?? CreateConnection());

        try
        {
            // Call the method
            return await connection.UpdateAllAsync(entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: CommandTimeout ?? 0,
                traceKey: traceKey,
                transaction: transaction,
                trace: Trace,
                statementBuilder: StatementBuilder,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            // Dispose the connection
            DisposeConnectionForPerCall(connection, transaction);
        }
    }

    #endregion

    #region UpdateAll(TableName)

    /// <summary>
    /// Update the existing rows in the table. By default, the table fields are used unless the 'fields' argument is defined.
    /// </summary>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entities">The list of dynamic objects to be used for update.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public int UpdateAll(string tableName,
        IEnumerable<object> entities,
        int batchSize = 0,
        FieldSet fields = default,
        string? hints = null,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null)
    {
        // Create a connection
        var connection = (transaction?.Connection ?? CreateConnection());

        try
        {
            // Call the method
            return connection.UpdateAll(tableName: tableName,
                entities: entities,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: CommandTimeout ?? 0,
                traceKey: traceKey,
                transaction: transaction,
                trace: Trace,
                statementBuilder: StatementBuilder);
        }
        finally
        {
            // Dispose the connection
            DisposeConnectionForPerCall(connection, transaction);
        }
    }

    /// <summary>
    /// Update the existing rows in the table. By default, the table fields are used unless the 'fields' argument is defined.
    /// </summary>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entities">The list of dynamic objects to be used for update.</param>
    /// <param name="qualifier">The qualifier <see cref="Field"/> object to be used for update.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public int UpdateAll(string tableName,
        IEnumerable<object> entities,
        Field qualifier,
        int batchSize = 0,
        FieldSet fields = default,
        string? hints = null,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null)
    {
        // Create a connection
        var connection = (transaction?.Connection ?? CreateConnection());

        try
        {
            // Call the method
            return connection.UpdateAll(tableName: tableName,
                entities: entities,
                qualifier: qualifier,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: CommandTimeout ?? 0,
                traceKey: traceKey,
                transaction: transaction,
                trace: Trace,
                statementBuilder: StatementBuilder);
        }
        finally
        {
            // Dispose the connection
            DisposeConnectionForPerCall(connection, transaction);
        }
    }

    /// <summary>
    /// Update the existing rows in the table. By default, the table fields are used unless the 'fields' argument is defined.
    /// </summary>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entities">The list of dynamic objects to be used for update.</param>
    /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public int UpdateAll(string tableName,
        IEnumerable<object> entities,
        FieldSet? qualifiers,
        int batchSize = 0,
        FieldSet fields = default,
        string? hints = null,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null)
    {
        // Create a connection
        var connection = (transaction?.Connection ?? CreateConnection());

        try
        {
            // Call the method
            return connection.UpdateAll(tableName: tableName,
                entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: CommandTimeout ?? 0,
                traceKey: traceKey,
                transaction: transaction,
                trace: Trace,
                statementBuilder: StatementBuilder);
        }
        finally
        {
            // Dispose the connection
            DisposeConnectionForPerCall(connection, transaction);
        }
    }

    #endregion

    #region UpdateAllAsync(TableName)

    /// <summary>
    /// Update the existing rows in the table in an asynchronous way. By default, the table fields are used unless the 'fields' argument is defined.
    /// </summary>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entities">The list of dynamic objects to be used for update.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public async Task<int> UpdateAllAsync(string tableName,
        IEnumerable<object> entities,
        int batchSize = 0,
        FieldSet fields = default,
        string? hints = null,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default)
    {
        // Create a connection
        var connection = (transaction?.Connection ?? CreateConnection());

        try
        {
            // Call the method
            return await connection.UpdateAllAsync(tableName: tableName,
                entities: entities,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: CommandTimeout ?? 0,
                traceKey: traceKey,
                transaction: transaction,
                trace: Trace,
                statementBuilder: StatementBuilder,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            // Dispose the connection
            DisposeConnectionForPerCall(connection, transaction);
        }
    }

    /// <summary>
    /// Update the existing rows in the table in an asynchronous way. By default, the table fields are used unless the 'fields' argument is defined.
    /// </summary>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entities">The list of dynamic objects to be used for update.</param>
    /// <param name="qualifier">The qualifier <see cref="Field"/> object to be used for update.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public async Task<int> UpdateAllAsync(string tableName,
        IEnumerable<object> entities,
        Field qualifier,
        int batchSize = 0,
        FieldSet fields = default,
        string? hints = null,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default)
    {
        // Create a connection
        var connection = (transaction?.Connection ?? CreateConnection());

        try
        {
            // Call the method
            return await connection.UpdateAllAsync(tableName: tableName,
                entities: entities,
                qualifier: qualifier,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: CommandTimeout ?? 0,
                traceKey: traceKey,
                transaction: transaction,
                trace: Trace,
                statementBuilder: StatementBuilder,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            // Dispose the connection
            DisposeConnectionForPerCall(connection, transaction);
        }
    }

    /// <summary>
    /// Update the existing rows in the table in an asynchronous way. By default, the table fields are used unless the 'fields' argument is defined.
    /// </summary>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entities">The list of dynamic objects to be used for update.</param>
    /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of affected rows during the update process.</returns>
    public async Task<int> UpdateAllAsync(string tableName,
        IEnumerable<object> entities,
        FieldSet? qualifiers,
        int batchSize = 0,
        FieldSet fields = default,
        string? hints = null,
        string? traceKey = TraceKeys.UpdateAll,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default)
    {
        // Create a connection
        var connection = (transaction?.Connection ?? CreateConnection());

        try
        {
            // Call the method
            return await connection.UpdateAllAsync(tableName: tableName,
                entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: CommandTimeout ?? 0,
                traceKey: traceKey,
                transaction: transaction,
                trace: Trace,
                statementBuilder: StatementBuilder,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            // Dispose the connection
            DisposeConnectionForPerCall(connection, transaction);
        }
    }

    #endregion
}
