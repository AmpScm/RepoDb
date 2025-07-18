using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.IntegrationTests.Setup;
using RepoDb.PostgreSql.BulkOperations.IntegrationTests.Models;

namespace RepoDb.PostgreSql.BulkOperations.IntegrationTests.Base;

[TestClass]
public class BinaryImportTest
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

    #region BinaryImport<TEntity>

    [TestMethod]
    public void TestBinaryImport()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryImport(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryImportWithBatchSize()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryImport(
                tableName,
                entities: entities,
                batchSize: 3);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryImportWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryImport(
                tableName,
                entities: entities,
                keepIdentity: true);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryImportWithMappings()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryImport(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryImportWithMappingsAndWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryImport(
                tableName,
                entities: entities,
                keepIdentity: true);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryImportWithBulkInsertMapItems()
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
            var result = connection.BinaryImport(
                tableName,
                entities: entities,
                mappings: mappings);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryImportWithBulkInsertMapItemsWithKeepIdentity()
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
            entities.ForEach(entity => entity.IdMapped += 100);

            // Act
            var result = connection.BinaryImport(
                tableName,
                entities: entities,
                mappings: mappings,
                keepIdentity: true);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void ThrowExceptionOnBinaryImportWithDuplicateIdentityOnKeepIdentity()
    {
        using var connection = GetConnection();
        // Prepare
        var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
        var tableName = "BulkOperationIdentityTable";

        // Act
        connection.BinaryImport(
            tableName,
            entities: entities,
            keepIdentity: true);

        // Act (Trigger)
        Assert.ThrowsExactly<PostgresException>(() => connection.BinaryImport(
            tableName,
            entities: entities,
            keepIdentity: true));
    }

    #endregion

    #region BinaryImport<Anonymous>

    [TestMethod]
    public void TestBinaryImportViaAnonymous()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryImport(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryImportViaAnoynymousWithBatchSize()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryImport(
                tableName,
                entities: entities,
                batchSize: 3);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryImportViaAnonymousWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryImport(
                tableName,
                entities: entities,
                keepIdentity: true);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryImportViaAnonymousWithBulkInsertMapItems()
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
            var result = connection.BinaryImport(
                tableName,
                entities: entities,
                mappings: mappings);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryImportViaAnonymousWithBulkInsertMapItemsAndWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousUnmatchedIdentityTables(10, true, 100);

            // Prepare
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
            var result = connection.BinaryImport(
                tableName,
                entities: entities,
                mappings: mappings);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void ThrowExceptionOnBinaryImportViaAnonymousWithDuplicateIdentityOnKeepIdentity()
    {
        Assert.ThrowsExactly<PostgresException>(() =>
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                connection.BinaryImport(
                    tableName,
                    entities: entities,
                    keepIdentity: true);

                // Act (Trigger)
                connection.BinaryImport(
                    tableName,
                    entities: entities,
                    keepIdentity: true);
            }
        });
    }

    #endregion

    #region BinaryImport<IDictionary<string,object>>

    [TestMethod]
    public void TestBinaryImportViaExpandoObject()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryImport(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName).ToList();
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryImportViaExpandoObjectWithBatchSize()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryImport(
                tableName,
                entities: entities,
                batchSize: 3);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName).ToList();
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryImportViaExpandoObjectWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryImport(
                tableName,
                entities: entities,
                keepIdentity: true);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName).ToList();
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryImportViaExpandoObjectWithBulkInsertMapItems()
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
            var result = connection.BinaryImport(
                tableName,
                entities: entities,
                mappings: mappings);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName).ToList();
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryImportViaExpandoObjectWithBulkInsertMapItemsAndWithKeepIdentity()
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
            var result = connection.BinaryImport(
                tableName,
                entities: entities,
                mappings: mappings,
                keepIdentity: true);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName).ToList();
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void ThrowExceptionOnBinaryImportViaExpandoObjectWithDuplicateIdentityOnKeepIdentity()
    {
        Assert.ThrowsExactly<PostgresException>(() =>
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                connection.BinaryImport(
                    tableName,
                    entities: entities,
                    keepIdentity: true);

                // Act (Trigger)
                connection.BinaryImport(
                    tableName,
                    entities: entities,
                    keepIdentity: true);
            }
        });
    }

    #endregion

    #region BinaryImport<DataTable>

    [TestMethod]
    public void TestBinaryImportViaDataTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = connection.BinaryImport(
                table.TableName,
                table: table);

            // Assert
            Assert.AreEqual(table.Rows.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryImportViaDataTableWithBatchSize()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = connection.BinaryImport(
                table.TableName,
                table: table,
                batchSize: 3);

            // Assert
            Assert.AreEqual(table.Rows.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryImportViaDataTableWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);

            // Prepare
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = connection.BinaryImport(
                table.TableName,
                table: table,
                keepIdentity: true);

            // Assert
            Assert.AreEqual(table.Rows.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryImportViaDataTableWithBulkInsertMapItems()
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
            var result = connection.BinaryImport(
                table.TableName,
                table: table,
                mappings: mappings);

            // Assert
            Assert.AreEqual(table.Rows.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryImportViaDataTableWithBulkInsertMapItemsAndWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true, 100);

            // Prepare
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
            var result = connection.BinaryImport(
                table.TableName,
                table: table,
                mappings: mappings);

            // Assert
            Assert.AreEqual(table.Rows.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void ThrowExceptionOnBinaryImportViaDataTableWithDuplicateIdentityOnKeepIdentity()
    {
        Assert.ThrowsExactly<PostgresException>(() =>
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var table = Helper.CreateBulkOperationDataTableLightIdentityTables(10, true);

                // Act
                connection.BinaryImport(
                    table.TableName,
                    table: table,
                    keepIdentity: true);

                // Act (Trigger)
                connection.BinaryImport(
                    table.TableName,
                    table: table,
                    keepIdentity: true);
            }
        });
    }

    #endregion

    #region BinaryImport<DbDataReader>

    [TestMethod]
    public void TestBinaryImportViaDbDataReader()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = connection.BinaryImport(
                    tableName,
                    reader: reader);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryImportViaDbDataReaderWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = connection.BinaryImport(
                    tableName,
                    reader: reader,
                    keepIdentity: true);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryImportViaDbDataReaderWithBulkInsertMapItems()
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
                var result = connection.BinaryImport(
                    tableName,
                    reader: reader,
                    mappings: mappings);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryImportViaDbDataReaderWithBulkInsertMapItemsWithKeepIdentity()
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
                var result = connection.BinaryImport(
                    tableName,
                    reader: reader,
                    mappings: mappings,
                    keepIdentity: true);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void ThrowExceptionOnBinaryImportViaDbDataReaderWithDuplicateIdentityOnKeepIdentity()
    {
        Assert.ThrowsExactly<PostgresException>(() =>
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    connection.BinaryImport(
                        tableName,
                        reader: reader,
                        keepIdentity: true);
                }

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act (Trigger)
                    connection.BinaryImport(
                        tableName,
                        reader: reader,
                        keepIdentity: true);
                }
            }
        });
    }

    #endregion

    #endregion

    #region Async

    #region BinaryImportAsync<TEntity>

    [TestMethod]
    public async Task TestBinaryImportAsync()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryImportAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryImportAsyncWithBatchSize()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryImportAsync(
                tableName,
                entities: entities,
                batchSize: 3);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryImportAsyncWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryImportAsync(
                tableName,
                entities: entities,
                keepIdentity: true);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryImportAsyncWithMappings()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryImportAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryImportAsyncWithMappingsAndWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryImportAsync(
                tableName,
                entities: entities,
                keepIdentity: true);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryImportAsyncWithBulkInsertMapItems()
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
            var result = await connection.BinaryImportAsync(
                tableName,
                entities: entities,
                mappings: mappings);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryImportAsyncWithBulkInsertMapItemsAndWithKeepIdentity()
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
            var result = await connection.BinaryImportAsync(
                tableName,
                entities: entities,
                mappings: mappings,
                keepIdentity: true);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void ThrowExceptionOnBinaryImportAsyncWithDuplicateIdentityOnKeepIdentity()
    {
        Assert.ThrowsExactly<AggregateException>(() =>
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                connection.BinaryImportAsync(
                    tableName,
                    entities: entities,
                    keepIdentity: true).Wait();

                // Act (Trigger)
                connection.BinaryImportAsync(
                    tableName,
                    entities: entities,
                    keepIdentity: true).Wait();
            }
        });
    }

    #endregion

    #region BinaryImportAsync<Anonymous>

    [TestMethod]
    public async Task TestBinaryImportAsyncViaAnonymous()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryImportAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryImportAsyncViaAnoynymousWithBatchSize()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryImportAsync(
                tableName,
                entities: entities,
                batchSize: 3);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryImportAsyncViaAnonymousAndWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryImportAsync(
                tableName,
                entities: entities,
                keepIdentity: true);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryImportAsyncViaAnonymousWithBulkInsertMapItems()
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
            var result = await connection.BinaryImportAsync(
                tableName,
                entities: entities,
                mappings: mappings);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryImportAsyncViaAnonymousWithBulkInsertMapItemsAndWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousUnmatchedIdentityTables(10, true, 100);

            // Prepare
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
            var result = await connection.BinaryImportAsync(
                tableName,
                entities: entities,
                mappings: mappings,
                keepIdentity: true);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void ThrowExceptionOnBinaryImportAsyncViaAnonymousWithDuplicateIdentityOnKeepIdentity()
    {
        Assert.ThrowsExactly<AggregateException>(() =>
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                connection.BinaryImportAsync(
                    tableName,
                    entities: entities,
                    keepIdentity: true).Wait();

                // Act (Trigger)
                connection.BinaryImportAsync(
                    tableName,
                    entities: entities,
                    keepIdentity: true).Wait();
            }
        });
    }

    #endregion

    #region BinaryImportAsync<IDictionary<string,object>>

    [TestMethod]
    public async Task TestBinaryImportAsyncViaExpandoObject()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryImportAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName).ToList();
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryImportAsyncViaExpandoObjectWithBatchSize()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryImportAsync(
                tableName,
                entities: entities,
                batchSize: 3);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName).ToList();
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryImportAsyncViaExpandoObjectAndWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryImportAsync(
                tableName,
                entities: entities,
                keepIdentity: true);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName).ToList();
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryImportAsyncViaExpandoObjectWithBulkInsertMapItems()
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
            var result = await connection.BinaryImportAsync(
                tableName,
                entities: entities,
                mappings: mappings);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName).ToList();
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryImportAsyncViaExpandoObjectWithBulkInsertMapItemsAndWithKeepIdentity()
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
            var result = await connection.BinaryImportAsync(
                tableName,
                entities: entities,
                mappings: mappings,
                keepIdentity: true);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName).ToList();
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void ThrowExceptionOnBinaryImportAsyncViaExpandoObjectWithDuplicateIdentityOnKeepIdentity()
    {
        Assert.ThrowsExactly<AggregateException>(() =>
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                connection.BinaryImportAsync(
                    tableName,
                    entities: entities,
                    keepIdentity: true).Wait();

                // Act (Trigger)
                connection.BinaryImportAsync(
                    tableName,
                    entities: entities,
                    keepIdentity: true).Wait();
            }
        });
    }

    #endregion

    #region BinaryImportAsync<DataTable>

    [TestMethod]
    public async Task TestBinaryImportAsyncViaDataTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = await connection.BinaryImportAsync(
                table.TableName,
                table: table);

            // Assert
            Assert.AreEqual(table.Rows.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryImportAsyncViaDataTableWithBatchSize()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = await connection.BinaryImportAsync(
                table.TableName,
                table: table,
                batchSize: 3);

            // Assert
            Assert.AreEqual(table.Rows.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryImportAsyncViaDataTableAndWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);

            // Prepare
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = await connection.BinaryImportAsync(
                table.TableName,
                table: table,
                keepIdentity: true);

            // Assert
            Assert.AreEqual(table.Rows.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryImportAsyncViaDataTableWithBulkInsertMapItems()
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
            var result = await connection.BinaryImportAsync(
                table.TableName,
                table: table,
                mappings: mappings);

            // Assert
            Assert.AreEqual(table.Rows.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryImportAsyncViaDataTableWithBulkInsertMapItemsAndWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true, 100);

            // Prepare
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
            var result = await connection.BinaryImportAsync(
                table.TableName,
                table: table,
                mappings: mappings);

            // Assert
            Assert.AreEqual(table.Rows.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void ThrowExceptionOnBinaryImportAsyncViaDataTableWithDuplicateIdentityOnKeepIdentity()
    {
        Assert.ThrowsExactly<AggregateException>(() =>
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var table = Helper.CreateBulkOperationDataTableLightIdentityTables(10, true);

                // Act
                connection.BinaryImportAsync(
                    table.TableName,
                    table: table,
                    keepIdentity: true).Wait();

                // Act (Trigger)
                connection.BinaryImportAsync(
                    table.TableName,
                    table: table,
                    keepIdentity: true).Wait();
            }
        });
    }

    #endregion

    #region BinaryImportAsync<DbDataReader>

    [TestMethod]
    public async Task TestBinaryImportAsyncViaDbDataReader()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = await connection.BinaryImportAsync(
                    tableName,
                    reader: reader);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryImportAsyncViaDbDataReaderAndWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = await connection.BinaryImportAsync(
                    tableName,
                    reader: reader,
                    keepIdentity: true);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryImportAsyncViaDbDataReaderWithBulkInsertMapItems()
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
                var result = await connection.BinaryImportAsync(
                    tableName,
                    reader: reader,
                    mappings: mappings);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryImportAsyncViaDbDataReaderWithBulkInsertMapItemsAndWithKeepIdentity()
    {

        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true, 100);

            // Prepare
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
                var result = await connection.BinaryImportAsync(
                    tableName,
                    reader: reader,
                    mappings: mappings,
                    keepIdentity: true);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void ThrowExceptionOnBinaryImportAsyncViaDbDataReaderWithDuplicateIdentityOnKeepIdentity()
    {
        Assert.ThrowsExactly<AggregateException>(() =>
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    connection.BinaryImportAsync(
                        tableName,
                        reader: reader,
                        keepIdentity: true).Wait();
                }

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act (Trigger)
                    connection.BinaryImportAsync(
                        tableName,
                        reader: reader,
                        keepIdentity: true).Wait();
                }
            }
        });
    }

    #endregion

    #endregion
}
