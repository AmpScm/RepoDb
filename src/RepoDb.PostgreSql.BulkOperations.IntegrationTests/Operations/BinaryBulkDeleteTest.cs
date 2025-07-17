using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.IntegrationTests.Setup;
using RepoDb.PostgreSql.BulkOperations.IntegrationTests.Models;

namespace RepoDb.PostgreSql.BulkOperations.IntegrationTests.Operations;

[TestClass]
public class BinaryBulkDeleteTest
{
    [TestInitialize]
    public void Initialize()
    {
        Database.Initialize();
        Cleanup();
    }

    [TestCleanup]
    public void Cleanup()
    {
        Database.Cleanup();
    }

    private NpgsqlConnection GetConnection() =>
        (NpgsqlConnection)(new NpgsqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen());

    #region Sync

    #region BinaryBulkDelete<TEntity>

    [TestMethod]
    public void TestBinaryBulkDelete()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = connection.BinaryBulkDelete(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public void TestBinaryBulkDeleteTableNameWithSchema()
    {
        using var connection = GetConnection();

        // Prepare
        var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
        var tableName = "public.BulkOperationIdentityTable";

        // Act
        connection.BinaryBulkInsert(tableName,
            entities: entities,
            identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

        // Act
        var result = connection.BinaryBulkDelete(tableName, entities);

        // Assert
        Assert.AreEqual(entities.Count, result);

        // Assert
        var countResult = connection.CountAll(tableName);
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public void TestBinaryBulkDeleteWithBatchSize()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = connection.BinaryBulkDelete(
                tableName,
                entities: entities,
                batchSize: 3);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public void TestBinaryBulkDeleteWithKeepIdentityFalse()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = connection.BinaryBulkDelete(
                tableName,
                entities: entities,
                keepIdentity: false);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public void TestBinaryBulkDeleteWithQualifiers()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = connection.BinaryBulkDelete(
                tableName,
                entities: entities,
                qualifiers: Field.From(
                    nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                    nameof(BulkOperationLightIdentityTable.ColumnInteger)));

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public void TestBinaryBulkDeleteWithMappings()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = connection.BinaryBulkDelete(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public void TestBinaryBulkDeleteWithMappingsViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = connection.BinaryBulkDelete(
                tableName,
                entities: entities,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public void TestBinaryBulkDeleteWithBulkInsertMapItems()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";
            var mappings = new[]
            {
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
            };

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = connection.BinaryBulkDelete(
                tableName,
                entities: entities,
                mappings: mappings);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public void TestBinaryBulkDeleteWithBulkInsertMapItemsViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";
            var mappings = new[]
            {
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
            };

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = connection.BinaryBulkDelete(
                tableName,
                entities: entities,
                mappings: mappings,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public void TestBinaryBulkDeleteOnEmptyTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkDelete(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(0, result);
        }
    }

    #endregion

    #region BinaryBulkDelete<Anonymous>

    [TestMethod]
    public void TestBinaryBulkDeleteViaAnonymous()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = connection.BinaryBulkDelete(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public void TestBinaryBulkDeleteViaAnonymousWithBatchSize()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = connection.BinaryBulkDelete(
                tableName,
                entities: entities,
                batchSize: 3);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public void TestBinaryBulkDeleteViaAnonymousWithKeepIdentityFalse()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = connection.BinaryBulkDelete(
                tableName,
                entities: entities,
                keepIdentity: false);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public void TestBinaryBulkDeleteViaAnonymousWithQualifiers()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = connection.BinaryBulkDelete(
                tableName,
                entities: entities,
                qualifiers: Field.From(
                    nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                    nameof(BulkOperationLightIdentityTable.ColumnInteger)));

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public void TestBinaryBulkDeleteViaAnonymousWithBulkInsertMapItems()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousUnmatchedIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";
            var mappings = new[]
            {
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
            };

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = connection.BinaryBulkDelete(
                tableName,
                entities: entities,
                mappings: mappings);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public void TestBinaryBulkDeleteViaAnonymousWithBulkInsertMapItemsViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousUnmatchedIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";
            var mappings = new[]
            {
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
            };

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = connection.BinaryBulkDelete(
                tableName,
                entities: entities,
                mappings: mappings,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public void TestBinaryBulkDeleteViaAnonymousOnEmptyTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkDelete(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(0, result);
        }
    }

    #endregion

    #region BinaryBulkDelete<IDictionary<string, object>>

    [TestMethod]
    public void TestBinaryBulkDeleteViaExpandoObject()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = connection.BinaryBulkDelete(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public void TestBinaryBulkDeleteViaExpandoObjectWithBatchSize()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = connection.BinaryBulkDelete(
                tableName,
                entities: entities,
                batchSize: 3);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public void TestBinaryBulkDeleteViaExpandoObjectWithKeepIdentityFalse()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = connection.BinaryBulkDelete(
                tableName,
                entities: entities,
                keepIdentity: false);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public void TestBinaryBulkDeleteViaExpandoObjectWithQualifiers()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = connection.BinaryBulkDelete(
                tableName,
                entities: entities,
                qualifiers: Field.From(
                    nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                    nameof(BulkOperationLightIdentityTable.ColumnInteger)));

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public void TestBinaryBulkDeleteViaExpandoObjectWithBulkInsertMapItems()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectUnmatchedIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";
            var mappings = new[]
            {
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
            };

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = connection.BinaryBulkDelete(
                tableName,
                entities: entities,
                mappings: mappings);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public void TestBinaryBulkDeleteViaExpandoObjectWithBulkInsertMapItemsViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectUnmatchedIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";
            var mappings = new[]
            {
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
            };

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = connection.BinaryBulkDelete(
                tableName,
                entities: entities,
                mappings: mappings,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public void TestBinaryBulkDeleteViaExpandoObjectOnEmptyTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkDelete(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(0, result);
        }
    }

    #endregion

    #region BinaryBulkDelete<DataTable>

    [TestMethod]
    public void TestBinaryBulkDeleteViaDataTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                table,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = connection.BinaryBulkDelete(
                tableName,
                table);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public void TestBinaryBulkDeleteViaDataTableWithKeepIdentityFalse()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                table,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = connection.BinaryBulkDelete(
                tableName,
                table: table,
                keepIdentity: false);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public void TestBinaryBulkDeleteViaDataTableWithQualifiers()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                table,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = connection.BinaryBulkDelete(
                tableName,
                table: table,
                qualifiers: Field.From(
                    nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                    nameof(BulkOperationLightIdentityTable.ColumnInteger)));

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public void TestBinaryBulkDeleteViaDataTableWithBulkInsertMapItems()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);
            var mappings = new[]
            {
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
            };

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                table,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = connection.BinaryBulkDelete(
                tableName,
                table: table,
                mappings: mappings);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public void TestBinaryBulkDeleteViaDataTableWithBulkInsertMapItemsViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);
            var mappings = new[]
            {
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
            };

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                table,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = connection.BinaryBulkDelete(
                tableName,
                table: table,
                mappings: mappings,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public void TestBinaryBulkDeleteViaDataTableOnEmptyTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = connection.BinaryBulkDelete(
                tableName,
                table);

            // Assert
            Assert.AreEqual(0, result);
        }
    }

    #endregion

    #region BinaryBulkDelete<DbDataReader>

    [TestMethod]
    public void TestBinaryBulkDeleteViaDbDataReader()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                connection.BinaryBulkInsert(
                    tableName,
                    reader,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);
            }

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = connection.BinaryBulkDelete(
                    tableName,
                    reader);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public void TestBinaryBulkDeleteViaDbDataReaderWithKeepIdentityFalse()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                connection.BinaryBulkInsert(
                    tableName,
                    reader,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);
            }

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = connection.BinaryBulkDelete(
                    tableName,
                    reader,
                    keepIdentity: false);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public void TestBinaryBulkDeleteViaDbDataReaderWithQualifiers()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                connection.BinaryBulkInsert(
                    tableName,
                    reader,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);
            }

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = connection.BinaryBulkDelete(
                    tableName,
                    reader,
                    qualifiers: Field.From(
                        nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                        nameof(BulkOperationLightIdentityTable.ColumnInteger)));

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public void TestBinaryBulkDeleteViaDbDataReaderWithBulkInsertMapItems()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";
            var mappings = new[]
            {
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
            };

            using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
            {
                // Act
                connection.BinaryBulkInsert(
                    tableName,
                    reader,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);
            }

            using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
            {
                // Act
                var result = connection.BinaryBulkDelete(
                    tableName,
                    reader,
                    mappings: mappings);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public void TestBinaryBulkDeleteViaDbDataReaderWithBulkInsertMapItemsViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";
            var mappings = new[]
            {
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
            };

            using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
            {
                // Act
                connection.BinaryBulkInsert(
                    tableName,
                    reader,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);
            }

            using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
            {
                // Act
                var result = connection.BinaryBulkDelete(
                    tableName,
                    reader,
                    mappings: mappings,
                    pseudoTableType: BulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public void TestBinaryBulkDeleteViaDbDataReaderOnEmptyTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = connection.BinaryBulkDelete(
                    tableName,
                    reader);

                // Assert
                Assert.AreEqual(0, result);
            }
        }
    }
    #endregion

    #endregion

    #region Async

    #region BinaryBulkDelete<TEntity>

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsync()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = await connection.BinaryBulkDeleteAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncTableNameWithSchema()
    {
        await using var connection = GetConnection();

        // Prepare
        var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
        var tableName = "public.BulkOperationIdentityTable";

        // Act
        await connection.BinaryBulkInsertAsync(tableName,
            entities: entities,
            identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

        // Act
        var result = await connection.BinaryBulkDeleteAsync(tableName, entities);

        // Assert
        Assert.AreEqual(entities.Count, result);

        // Assert
        var countResult = await connection.CountAllAsync(tableName);
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncWithBatchSize()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = await connection.BinaryBulkDeleteAsync(
                tableName,
                entities: entities,
                batchSize: 3);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncWithKeepIdentityFalse()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = await connection.BinaryBulkDeleteAsync(
                tableName,
                entities: entities,
                keepIdentity: false);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncWithQualifiers()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = await connection.BinaryBulkDeleteAsync(
                tableName,
                entities: entities,
                qualifiers: Field.From(
                    nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                    nameof(BulkOperationLightIdentityTable.ColumnInteger)));

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncWithMappings()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = await connection.BinaryBulkDeleteAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncWithMappingsViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = await connection.BinaryBulkDeleteAsync(
                tableName,
                entities: entities,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncWithBulkInsertMapItems()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";
            var mappings = new[]
            {
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
            };

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = await connection.BinaryBulkDeleteAsync(
                tableName,
                entities: entities,
                mappings: mappings);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncWithBulkInsertMapItemsViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";
            var mappings = new[]
            {
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
            };

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = await connection.BinaryBulkDeleteAsync(
                tableName,
                entities: entities,
                mappings: mappings,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncOnEmptyTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkDeleteAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(0, result);
        }
    }

    #endregion

    #region BinaryBulkDelete<Anonymous>

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncViaAnonymous()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = await connection.BinaryBulkDeleteAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncViaAnonymousWithBatchSize()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = await connection.BinaryBulkDeleteAsync(
                tableName,
                entities: entities,
                batchSize: 3);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncViaAnonymousWithKeepIdentityFalse()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = await connection.BinaryBulkDeleteAsync(
                tableName,
                entities: entities,
                keepIdentity: false);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncViaAnonymousWithQualifiers()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = await connection.BinaryBulkDeleteAsync(
                tableName,
                entities: entities,
                qualifiers: Field.From(
                    nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                    nameof(BulkOperationLightIdentityTable.ColumnInteger)));

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncViaAnonymousWithBulkInsertMapItems()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousUnmatchedIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";
            var mappings = new[]
            {
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
            };

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = await connection.BinaryBulkDeleteAsync(
                tableName,
                entities: entities,
                mappings: mappings);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncViaAnonymousWithBulkInsertMapItemsViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousUnmatchedIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";
            var mappings = new[]
            {
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
            };

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = await connection.BinaryBulkDeleteAsync(
                tableName,
                entities: entities,
                mappings: mappings,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncViaAnonymousOnEmptyTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkDeleteAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(0, result);
        }
    }

    #endregion

    #region BinaryBulkDelete<IDictionary<string, object>>

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncViaExpandoObject()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = await connection.BinaryBulkDeleteAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncViaExpandoObjectWithBatchSize()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = await connection.BinaryBulkDeleteAsync(
                tableName,
                entities: entities,
                batchSize: 3);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncViaExpandoObjectWithKeepIdentityFalse()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = await connection.BinaryBulkDeleteAsync(
                tableName,
                entities: entities,
                keepIdentity: false);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncViaExpandoObjectWithQualifiers()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = await connection.BinaryBulkDeleteAsync(
                tableName,
                entities: entities,
                qualifiers: Field.From(
                    nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                    nameof(BulkOperationLightIdentityTable.ColumnInteger)));

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncViaExpandoObjectWithBulkInsertMapItems()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectUnmatchedIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";
            var mappings = new[]
            {
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
            };

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = await connection.BinaryBulkDeleteAsync(
                tableName,
                entities: entities,
                mappings: mappings);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncViaExpandoObjectWithBulkInsertMapItemsViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectUnmatchedIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";
            var mappings = new[]
            {
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
            };

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = await connection.BinaryBulkDeleteAsync(
                tableName,
                entities: entities,
                mappings: mappings,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncViaExpandoObjectOnEmptyTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkDeleteAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(0, result);
        }
    }

    #endregion

    #region BinaryBulkDelete<DataTable>

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncViaDataTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                table,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = await connection.BinaryBulkDeleteAsync(
                tableName,
                table);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncViaDataTableWithKeepIdentityFalse()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                table,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = await connection.BinaryBulkDeleteAsync(
                tableName,
                table: table,
                keepIdentity: false);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncViaDataTableWithQualifiers()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                table,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = await connection.BinaryBulkDeleteAsync(
                tableName,
                table: table,
                qualifiers: Field.From(
                    nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                    nameof(BulkOperationLightIdentityTable.ColumnInteger)));

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncViaDataTableWithBulkInsertMapItems()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);
            var mappings = new[]
            {
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
            };

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                table,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = await connection.BinaryBulkDeleteAsync(
                tableName,
                table: table,
                mappings: mappings);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncViaDataTableWithBulkInsertMapItemsViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);
            var mappings = new[]
            {
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
            };

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                table,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Act
            result = await connection.BinaryBulkDeleteAsync(
                tableName,
                table: table,
                mappings: mappings,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncViaDataTableOnEmptyTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = await connection.BinaryBulkDeleteAsync(
                tableName,
                table);

            // Assert
            Assert.AreEqual(0, result);
        }
    }

    #endregion

    #region BinaryBulkDelete<DbDataReader>

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncViaDbDataReader()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                connection.BinaryBulkInsert(
                    tableName,
                    reader,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);
            }

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = await connection.BinaryBulkDeleteAsync(
                    tableName,
                    reader);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncViaDbDataReaderWithKeepIdentityFalse()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                connection.BinaryBulkInsert(
                    tableName,
                    reader,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);
            }

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = await connection.BinaryBulkDeleteAsync(
                    tableName,
                    reader,
                    keepIdentity: false);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncViaDbDataReaderWithQualifiers()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                connection.BinaryBulkInsert(
                    tableName,
                    reader,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);
            }

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = await connection.BinaryBulkDeleteAsync(
                    tableName,
                    reader,
                    qualifiers: Field.From(
                        nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                        nameof(BulkOperationLightIdentityTable.ColumnInteger)));

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncViaDbDataReaderWithBulkInsertMapItems()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";
            var mappings = new[]
            {
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
            };

            using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
            {
                // Act
                connection.BinaryBulkInsert(
                    tableName,
                    reader,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);
            }

            using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
            {
                // Act
                var result = await connection.BinaryBulkDeleteAsync(
                    tableName,
                    reader,
                    mappings: mappings);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncViaDbDataReaderWithBulkInsertMapItemsViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";
            var mappings = new[]
            {
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
            };

            using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
            {
                // Act
                connection.BinaryBulkInsert(
                    tableName,
                    reader,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);
            }

            using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
            {
                // Act
                var result = await connection.BinaryBulkDeleteAsync(
                    tableName,
                    reader,
                    mappings: mappings,
                    pseudoTableType: BulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncViaDbDataReaderOnEmptyTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = await connection.BinaryBulkDeleteAsync(
                    tableName,
                    reader);

                // Assert
                Assert.AreEqual(0, result);
            }
        }
    }

    #endregion

    #endregion
}
