using System.Data;

namespace RepoDb;

public abstract partial class BaseRepository<TEntity, TDbConnection> : IDisposable
{
    #region InsertAll<TEntity>

    /// <summary>
    /// Insert multiple rows in the table.
    /// </summary>
    /// <param name="entities">The data entity objects to be inserted.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <returns>The number of inserted rows in the table.</returns>
    public int InsertAll(IEnumerable<TEntity> entities,
        int batchSize = 0,
        FieldSet fields = default,
        string? hints = null,
        string? traceKey = TraceKeys.InsertAll,
        IDbTransaction? transaction = null)
    {
        return DbRepository.InsertAll(entities: entities,
            batchSize: batchSize,
            fields: fields,
            hints: hints,
            traceKey: traceKey,
            transaction: transaction);
    }

    #endregion

    #region InsertAllAsync<TEntity>

    /// <summary>
    /// Insert multiple rows in the table in an asynchronous way.
    /// </summary>
    /// <param name="entities">The data entity objects to be inserted.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of inserted rows in the table.</returns>
    public Task<int> InsertAllAsync(IEnumerable<TEntity> entities,
        int batchSize = 0,
        FieldSet fields = default,
        string? hints = null,
        string? traceKey = TraceKeys.InsertAll,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default)
    {
        return DbRepository.InsertAllAsync(entities: entities,
            batchSize: batchSize,
            fields: fields,
            hints: hints,
            traceKey: traceKey,
            transaction: transaction,
            cancellationToken: cancellationToken);
    }

    #endregion
}
