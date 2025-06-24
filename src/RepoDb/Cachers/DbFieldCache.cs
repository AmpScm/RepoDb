using System.Collections.Concurrent;
using System.Data;
using RepoDb.Exceptions;

namespace RepoDb;

/// <summary>
/// A class that is being used to cache the list of <see cref="DbField"/> objects of the table.
/// </summary>
public static class DbFieldCache
{
    private static readonly ConcurrentDictionary<int, DbFieldCollection> cache = new();

    #region Helpers

    /// <summary>
    /// Flushes all the existing cached enumerable of <see cref="DbField"/> objects.
    /// </summary>
    public static void Flush() =>
        cache.Clear();

    /// <summary>
    /// Throws an exception of any of the validation needed is failing.
    /// </summary>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="dbFields">The list of <see cref="DbField"/> objects to be validated.</param>
    private static void ValidateDbFields(string tableName,
        DbFieldCollection dbFields)
    {
        if (dbFields is null || dbFields.IsEmpty())
        {
            throw new MissingFieldsException($"There are no database fields found for table '{tableName}'. Make sure that the target table '{tableName}' is present in the database and/or at least a single field is available.");
        }
    }

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
    public static DbFieldCollection Get(IDbConnection connection,
        string tableName,
        IDbTransaction? transaction) =>
        Get(connection, tableName, transaction, true);

    /// <summary>
    /// Gets the cached list of <see cref="DbField"/> objects of the table based on the data entity mapped name.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="transaction">The transaction object that is currently in used.</param>
    /// <param name="enableValidation">Enables the validation after retrieving the database fields.</param>
    /// <returns>The cached field definitions of the entity.</returns>
    public static DbFieldCollection Get(IDbConnection connection,
        string tableName,
        IDbTransaction? transaction,
        bool enableValidation) =>
        GetInternal(connection, tableName, transaction, enableValidation);

    /// <summary>
    /// Gets the cached field definitions of the entity.
    /// </summary>
    /// <typeparam name="TDbConnection">The type of <see cref="IDbConnection"/> object.</typeparam>
    /// <param name="connection">The instance of the <see cref="IDbConnection"/> object.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="transaction">The transaction object that is currently in used.</param>
    /// <param name="enableValidation">Enables the validation after retrieving the database fields.</param>
    /// <returns>The cached field definitions of the entity.</returns>
    internal static DbFieldCollection GetInternal<TDbConnection>(TDbConnection connection,
        string tableName,
        IDbTransaction? transaction,
        bool enableValidation)
        where TDbConnection : IDbConnection
    {
        // Note: For SqlConnection, the ConnectionString is changing if the (Integrated Security=False). Actually for this isolation, the database name is enough.
        var key = HashCode.Combine(connection.GetType(), connection.Database, tableName);

        var result = cache.GetOrAdd(key,
            (_) => connection.GetDbHelper().GetFields(connection, tableName, transaction));

        // Validate
        if (enableValidation)
        {
            ValidateDbFields(tableName, result);
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
    public static Task<DbFieldCollection> GetAsync(IDbConnection connection,
        string tableName,
        IDbTransaction? transaction,
        CancellationToken cancellationToken = default) =>
        GetAsync(connection, tableName, transaction, true, cancellationToken);

    /// <summary>
    /// Gets the cached list of <see cref="DbField"/> objects of the table based on the data entity mapped name in an asynchronous way.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="transaction">The transaction object that is currently in used.</param>
    /// <param name="enableValidation">Enables the validation after retrieving the database fields.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The cached field definitions of the entity.</returns>
    public static async Task<DbFieldCollection> GetAsync(IDbConnection connection,
        string tableName,
        IDbTransaction? transaction,
        bool enableValidation,
        CancellationToken cancellationToken = default) =>
        await GetAsyncInternal(connection, tableName, transaction, enableValidation, cancellationToken);

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
    internal static async ValueTask<DbFieldCollection> GetAsyncInternal<TDbConnection>(TDbConnection connection,
        string tableName,
        IDbTransaction? transaction,
        bool enableValidation,
        CancellationToken cancellationToken = default)
        where TDbConnection : IDbConnection
    {
        // Note: For SqlConnection, the ConnectionString is changing if the (Integrated Security=False). Actually for this isolation, the database name is enough.
        var key = HashCode.Combine(connection.GetType(), connection.Database, tableName);

        // Try get the value
        if (cache.TryGetValue(key, out var result) == false)
        {
            // Get from DB
            result = await connection
                .GetDbHelper()
                .GetFieldsAsync(connection, tableName, transaction, cancellationToken).ConfigureAwait(false);

            // Add to cache
            cache.TryAdd(key, result);
        }

        // Validate
        if (enableValidation)
        {
            ValidateDbFields(tableName, result);
        }

        // Return the value
        return result;
    }

    #endregion

    #endregion
}
