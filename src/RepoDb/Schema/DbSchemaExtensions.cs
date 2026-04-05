using System.Data.Common;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb.Schema;

/// <summary>
/// Provides extension methods for checking the existence of schema objects in a database connection.
/// </summary>
/// <remarks>These methods require an implementation of the IDbHelper interface for the target database
/// connection. The extensions support both synchronous and asynchronous operations, and allow filtering by object name,
/// schema, and type. They are designed to work with various database providers that support schema
/// inspection.</remarks>
public static class DbSchemaExtensions
{
    /// <summary>
    /// Checks if the schema object exists in the current database.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="name"></param>
    /// <param name="schemaName"></param>
    /// <param name="type"></param>
    /// <param name="transaction"></param>
    /// <returns></returns>
    /// <remarks>Requires a <see cref="IDbHelper"/> implementation for this connection</remarks>
    public static bool SchemaObjectExists(this DbConnection connection,
        string name,
        string? schemaName = null,
        DbSchemaType? type = null,
        DbTransaction? transaction = null)
    {
        var schemaObjects = connection.GetDbHelper().GetSchemaObjects(connection, transaction);

        if (type != null && schemaName != null)
            return schemaObjects.Any(x => x.Name == name && x.Type == type && schemaName == x.Schema);
        else if (type is not null)
            return schemaObjects.Any(x => x.Name == name && x.Type == type);
        else if (schemaName is not null)
            return schemaObjects.Any(x => x.Name == name && schemaName == x.Schema);
        else
            return schemaObjects.Any(x => x.Name == name);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="connection"></param>
    /// <param name="type"></param>
    /// <param name="transaction"></param>
    /// <returns></returns>
    public static bool SchemaObjectExists<TEntity>(this DbConnection connection, DbSchemaType? type = null, DbTransaction? transaction = null)
    {
        return SchemaObjectExists<TEntity>(connection, null, type, transaction);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="connection"></param>
    /// <param name="tableName"></param>
    /// <param name="type"></param>
    /// <param name="transaction"></param>
    /// <returns></returns>
    public static bool SchemaObjectExists<TEntity>(this DbConnection connection, string? tableName, DbSchemaType? type = null, DbTransaction? transaction = null)
    {
        tableName ??= ClassMappedNameCache.Get<TEntity>();

        var setting = connection.GetDbSetting();
        return SchemaObjectExists(connection, DataEntityExtension.GetTableName(tableName, setting), DataEntityExtension.GetSchema(tableName, setting), type, transaction);
    }

    /// <summary>
    /// Checks if the schema object exists in the current database.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="name"></param>
    /// <param name="schemaName"></param>
    /// <param name="type"></param>
    /// <param name="transaction"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>Requires a <see cref="IDbHelper"/> implementation for this connection</remarks>
    public static async ValueTask<bool> SchemaObjectExistsAsync(this DbConnection connection,
        string name,
        string? schemaName = null,
        DbSchemaType? type = null,
        DbTransaction? transaction = null,
        CancellationToken cancellationToken = default)
    {
        var schemaObjects = await connection.GetDbHelper().GetSchemaObjectsAsync(connection, transaction, cancellationToken).ConfigureAwait(false);

        if (type != null && schemaName != null)
            return schemaObjects.Any(x => x.Name == name && x.Type == type && x.Schema == schemaName);
        else if (type is not null)
            return schemaObjects.Any(x => x.Name == name && x.Type == type);
        else if (schemaName is not null)
            return schemaObjects.Any(x => x.Name == name && x.Schema == schemaName);
        else
            return schemaObjects.Any(x => x.Name == name);
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="connection"></param>
    /// <param name="type"></param>
    /// <param name="transaction"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask<bool> SchemaObjectExistsAsync<TEntity>(this DbConnection connection, DbSchemaType? type = null, DbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        return await SchemaObjectExistsAsync<TEntity>(connection, null, type, transaction, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="connection"></param>
    /// <param name="tableName"></param>
    /// <param name="type"></param>
    /// <param name="transaction"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask<bool> SchemaObjectExistsAsync<TEntity>(this DbConnection connection, string? tableName, DbSchemaType? type = null, DbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        tableName ??= ClassMappedNameCache.Get<TEntity>();

        var setting = connection.GetDbSetting();
        return await SchemaObjectExistsAsync(connection, DataEntityExtension.GetTableName(tableName, setting), DataEntityExtension.GetSchema(tableName, setting), type, transaction, cancellationToken).ConfigureAwait(false);
    }
}
