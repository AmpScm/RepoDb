using System.Collections.Concurrent;
using System.Data;

namespace RepoDb;
public static class DbRuntimeSettingCache
{
    private static readonly ConcurrentDictionary<int, DbRuntimeSetting> cache = new();

    #region Helpers

    /// <summary>
    /// Flushes all the existing cached enumerable of <see cref="DbField"/> objects.
    /// </summary>
    public static void Flush() =>
        cache.Clear();


    #endregion

    #region Methods

    #region Sync

    /// <summary>
    /// Gets the cached list of <see cref="DbField"/> objects of the table based on the data entity mapped name.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="transaction">The transaction object that is currently in used.</param>
    /// <returns>The cached field definitions of the entity.</returns>
    public static DbRuntimeSetting Get(IDbConnection connection, IDbTransaction? transaction) =>
        GetInternal(connection, transaction);

    /// <summary>
    /// Gets the cached field definitions of the entity.
    /// </summary>
    /// <typeparam name="TDbConnection">The type of <see cref="IDbConnection"/> object.</typeparam>
    /// <param name="connection">The instance of the <see cref="IDbConnection"/> object.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="transaction">The transaction object that is currently in used.</param>
    /// <param name="enableValidation">Enables the validation after retrieving the database fields.</param>
    /// <returns>The cached field definitions of the entity.</returns>
    internal static DbRuntimeSetting GetInternal(IDbConnection connection,
        IDbTransaction? transaction)
    {
        var key = HashCode.Combine(connection.GetType(), connection.Database);

        var result = cache.GetOrAdd(key,
            (_) => connection.GetDbHelper().GetDbConnectionRuntimeInformation(connection, transaction));

        // Validate
        if (result is null)
        {
            throw new InvalidOperationException($"There is no database engine version available");
        }

        return result;
    }

    #endregion

    #region Async

    /// <summary>
    /// Gets the cached list of <see cref="DbField"/> objects of the table based on the data entity mapped name in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="transaction">The transaction object that is currently in used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The cached field definitions of the entity.</returns>
    public static ValueTask<DbRuntimeSetting> GetAsync(IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken = default) =>
        GetAsyncInternal(connection, transaction, cancellationToken);

    /// <summary>
    /// Gets the cached field definitions of the entity in an asynchronous way.
    /// </summary>
    /// <typeparam name="TDbConnection">The type of <see cref="IDbConnection"/> object.</typeparam>
    /// <param name="connection">The instance of the <see cref="IDbConnection"/> object.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="transaction">The transaction object that is currently in used.</param>
    /// <param name="enableValidation">Enables the validation after retrieving the database fields.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The cached field definitions of the entity.</returns>
    internal static async ValueTask<DbRuntimeSetting> GetAsyncInternal(IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken = default)
    {
        var key = HashCode.Combine(connection.GetType(), connection.Database);

        // Try get the value
        if (cache.TryGetValue(key, out var result) == false)
        {
            // Get from DB
            result = await connection
                .GetDbHelper()
                .GetDbConnectionRuntimeInformationAsync(connection, transaction, cancellationToken).ConfigureAwait(false);

            // Add to cache
            cache.TryAdd(key, result);
        }

        // Validate
        if (result is null)
        {
            throw new InvalidOperationException($"There is no database engine version available");
        }

        // Return the value
        return result;
    }

    #endregion

    #endregion

}
