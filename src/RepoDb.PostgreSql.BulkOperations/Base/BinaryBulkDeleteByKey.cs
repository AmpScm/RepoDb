using Npgsql;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.Extensions;
using RepoDb.PostgreSql.BulkOperations;

namespace RepoDb;

public static partial class NpgsqlConnectionExtension
{
    #region Sync

    #region BinaryBulkDeleteByKeyBase<TPrimaryKey>

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <param name="connection"></param>
    /// <param name="tableName"></param>
    /// <param name="primaryKeys"></param>
    /// <param name="bulkCopyTimeout"></param>
    /// <param name="batchSize"></param>
    /// <param name="pseudoTableType"></param>
    /// <param name="transaction"></param>
    /// <returns></returns>
    private static int BinaryBulkDeleteByKeyBase<TPrimaryKey>(this NpgsqlConnection connection,
        string tableName,
        IEnumerable<TPrimaryKey> primaryKeys,
        int bulkCopyTimeout = 0,
        int batchSize = 0,
        BulkImportPseudoTableType pseudoTableType = default,
        NpgsqlTransaction? transaction = null)
    {
        var identityBehavior = BulkImportIdentityBehavior.Unspecified;
        var dbSetting = connection.GetDbSetting();
        var dbFields = DbFieldCache.Get(connection, tableName, transaction);
        var primaryKey = dbFields.PrimaryFields!.First();
        var pseudoTableName = tableName;
        IEnumerable<NpgsqlBulkInsertMapItem>? mappings = null;

        return PseudoBasedBinaryImport(connection,
            tableName,
            bulkCopyTimeout,
            dbFields,

            // getPseudoTableName
            () =>
                pseudoTableName = GetBinaryBulkDeleteByKeyPseudoTableName(tableName ?? ClassMappedNameCache.Get<TPrimaryKey>(), dbSetting),

            // getMappings
            () =>
                mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(primaryKey.FieldName, primaryKey.FieldName)
                },

            // binaryImport
            (tableName) =>
                connection.BinaryImport(tableName,
                    GetExpandoObjectData(primaryKeys, primaryKey.AsField()),
                    mappings,
                    dbFields,
                    bulkCopyTimeout,
                    batchSize,
                    identityBehavior,
                    dbSetting,
                    transaction),

            // getDeleteToPseudoCommandText
            (idenityField) =>
                GetDeleteByKeyCommandText(pseudoTableName,
                    tableName,
                    dbFields.PrimaryFields?.OneOrDefault(),
                    dbSetting),

            // setIdentities
            null,

            null,
            false,
            identityBehavior,
            pseudoTableType,
            dbSetting,
            transaction);
    }

    #endregion

    #endregion

    #region Async

    #region BinaryBulkDeleteByKeyBaseAsync<TPrimaryKey>

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <param name="connection"></param>
    /// <param name="tableName"></param>
    /// <param name="primaryKeys"></param>
    /// <param name="bulkCopyTimeout"></param>
    /// <param name="batchSize"></param>
    /// <param name="pseudoTableType"></param>
    /// <param name="transaction"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private static async Task<int> BinaryBulkDeleteByKeyBaseAsync<TPrimaryKey>(this NpgsqlConnection connection,
        string tableName,
        IEnumerable<TPrimaryKey> primaryKeys,
        int bulkCopyTimeout = 0,
        int batchSize = 0,
        BulkImportPseudoTableType pseudoTableType = default,
        NpgsqlTransaction? transaction = null,
        CancellationToken cancellationToken = default)
    {
        var identityBehavior = BulkImportIdentityBehavior.Unspecified;
        var dbSetting = connection.GetDbSetting();
        var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
        var primaryKey = dbFields.PrimaryFields!.First();
        var pseudoTableName = tableName;
        IEnumerable<NpgsqlBulkInsertMapItem>? mappings = null;

        return await PseudoBasedBinaryImportAsync(connection,
            tableName,
            bulkCopyTimeout,
            dbFields,

            // getPseudoTableName
            () =>
                pseudoTableName = GetBinaryBulkDeleteByKeyPseudoTableName(tableName ?? ClassMappedNameCache.Get<TPrimaryKey>(), dbSetting),

            // getMappings
            () =>
                mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(primaryKey.FieldName, primaryKey.FieldName)
                },

            // binaryImport
            async (tableName) =>
                await connection.BinaryImportAsync(tableName,
                    GetExpandoObjectData(primaryKeys, primaryKey.AsField()),
                    mappings,
                    dbFields,
                    bulkCopyTimeout,
                    batchSize,
                    identityBehavior,
                    dbSetting,
                    transaction,
                    cancellationToken),

            // getDeleteToPseudoCommandText
            (idenityField) =>
                GetDeleteByKeyCommandText(pseudoTableName,
                    tableName,
                    dbFields.PrimaryFields?.OneOrDefault(),
                    dbSetting),

            // setIdentities
            null,

            null,
            false,
            identityBehavior,
            pseudoTableType,
            dbSetting,
            transaction,
            cancellationToken);
    }

    #endregion

    #endregion
}
