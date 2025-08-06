using System.Data.Common;
using RepoDb.Enumerations;
using RepoDb.Extensions;

namespace RepoDb.Schema;
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
        else if (type != null)
            return schemaObjects.Any(x => x.Name == name && x.Type == type);
        else if (schemaName != null)
            return schemaObjects.Any(x => x.Name == name && schemaName == x.Schema);
        else
            return schemaObjects.Any(x => x.Name == name);
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
        else if (type != null)
            return schemaObjects.Any(x => x.Name == name && x.Type == type);
        else if (schemaName != null)
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
        var name = ClassMappedNameCache.Get<TEntity>();

        var setting = connection.GetDbSetting();
        return await SchemaObjectExistsAsync(connection, DataEntityExtension.GetTableName(name, setting), DataEntityExtension.GetSchema(name, setting), type, transaction, cancellationToken);
    }
}
