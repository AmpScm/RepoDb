using System.Collections;
using System.Data;
using System.Data.Common;
using RepoDb.DbSettings;

namespace RepoDb.Interfaces;

/// <summary>
/// An interface that is used to mark a class be a database helper object.
/// </summary>
public interface IDbHelper
{
    /// <summary>
    /// Gets the type resolver used by this <see cref="IDbHelper"/> instance.
    /// </summary>
    IResolver<string, Type> DbTypeResolver { get; }

    #region GetFields

    /// <summary>
    /// Gets the list of <see cref="DbField"/> objects of the table.
    /// </summary>
    /// <param name="connection">The instance of the connection object.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="transaction">The transaction object that is currently in used.</param>
    /// <returns>A list of <see cref="DbField"/> of the target table.</returns>
    DbFieldCollection GetFields(IDbConnection connection,
        string tableName,
        IDbTransaction? transaction = null);

    /// <summary>
    /// Gets the list of <see cref="DbField"/> objects of the table in an asynchronous way.
    /// </summary>
    /// <param name="connection">The instance of the connection object.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="transaction">The transaction object that is currently in used.</param>
    /// <param name="cancellationToken"> A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>A list of <see cref="DbField"/> of the target table.</returns>
    ValueTask<DbFieldCollection> GetFieldsAsync(IDbConnection connection,
        string tableName,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region GetSchemaObjects

    /// <summary>
    /// Retrieves a collection of schema objects from the specified database connection.
    /// </summary>
    /// <param name="connection">The open database connection from which to retrieve schema objects. Must not be null and must be open.</param>
    /// <param name="transaction">An optional transaction context to associate with the schema retrieval operation. If null, the operation is
    /// performed outside of a transaction.</param>
    /// <returns>An enumerable collection of schema objects representing database metadata. The collection is empty if no schema
    /// objects are found.</returns>
    IEnumerable<DbSchemaObject> GetSchemaObjects(IDbConnection connection,
        IDbTransaction? transaction = null);

    /// <summary>
    /// Asynchronously retrieves a collection of database schema objects from the specified connection.
    /// </summary>
    /// <param name="connection">The open database connection used to query schema information. Must not be null and must remain open for the
    /// duration of the operation.</param>
    /// <param name="transaction">An optional transaction context to associate with the schema query. If null, the operation executes outside of a
    /// transaction.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A value task representing the asynchronous operation. The result contains an enumerable collection of schema
    /// objects found in the database. The collection is empty if no schema objects are present.</returns>
    ValueTask<IEnumerable<DbSchemaObject>> GetSchemaObjectsAsync(IDbConnection connection,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default);
    #endregion

    #region DynamicHandler

    /// <summary>
    ///
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="transaction"></param>
    /// <returns></returns>
    DbRuntimeSetting GetDbConnectionRuntimeInformation(IDbConnection connection, IDbTransaction? transaction);

    /// <summary>
    ///
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="transaction"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask<DbRuntimeSetting> GetDbConnectionRuntimeInformationAsync(IDbConnection connection, IDbTransaction? transaction, CancellationToken cancellationToken);

    /// <summary>
    ///
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="transaction"></param>
    /// <param name="fieldType"></param>
    /// <param name="values"></param>
    /// <param name="parameterName"></param>
    /// <returns></returns>
    DbParameter? CreateTableParameter(IDbConnection connection, IDbTransaction? transaction, Type? fieldType, IEnumerable values, string parameterName);

    /// <summary>
    ///
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="transaction"></param>
    /// <param name="fieldType"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    bool CanCreateTableParameter(IDbConnection connection, IDbTransaction? transaction, Type? fieldType, IEnumerable values);

    /// <summary>
    ///
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="transaction"></param>
    /// <param name="fieldType"></param>
    /// <param name="values"></param>
    /// <param name="parameterName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask<DbParameter?> CreateTableParameterAsync(IDbConnection connection, IDbTransaction? transaction, Type? fieldType, IEnumerable values, string parameterName, CancellationToken cancellationToken = default);

    /// <summary>
    ///
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="transaction"></param>
    /// <param name="fieldType"></param>
    /// <param name="parameterName"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    string? CreateTableParameterText(IDbConnection connection, IDbTransaction? transaction, Type? fieldType, string parameterName, IEnumerable values);

    #endregion
}
