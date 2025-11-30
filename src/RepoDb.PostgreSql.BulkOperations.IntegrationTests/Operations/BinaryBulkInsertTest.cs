using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.IntegrationTests.Setup;
using RepoDb.PostgreSql.BulkOperations.IntegrationTests.Models;
using System.Data;

namespace RepoDb.PostgreSql.BulkOperations.IntegrationTests.Operations;

[TestClass]
public class BinaryBulkInsertTest
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

    #region BinaryBulkInsert<TEntity>

    [TestMethod]
    public void TestBinaryBulkInsert()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertTableNameWithSchema()
    {
        using var connection = GetConnection();

        // Prepare
        var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
        var tableName = "public.BulkOperationIdentityTable";

        // Act
        var result = connection.BinaryBulkInsert(tableName, entities);

        // Assert
        Assert.AreEqual(entities.Count, result);

        // Assert
        var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
        var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
        Assert.AreEqual(entities.Count, assertCount);
    }

    [TestMethod]
    public void TestBinaryBulkInsertWithIdentityValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertWithBatchSize()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                batchSize: 3);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertWithReturnIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.IsTrue(entities.All(e => e.Id > 0));

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            Assert.HasCount(entities.Count, queryResult);
            foreach (var entity in entities)
            {
                var target = queryResult.First(item => item.Id == entity.Id);
                Helper.AssertEntityEquality(entity, target);
            }
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertWithReturnIdentityViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.IsTrue(entities.All(e => e.Id > 0));

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertWithMappings()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationMappedIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertWithMappingsAndWithKeepIdentity()
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

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertWithMappingsAndWithKeepIdentityViaPhysicalTable()
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
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertWithMappingsAndWithReturnIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationMappedIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.IsTrue(entities.All(e => e.IdMapped > 0));

            // Assert
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertWithMappingsAndWithReturnIdentityViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationMappedIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.IsTrue(entities.All(e => e.IdMapped > 0));

            // Assert
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertWithBulkInsertMapItems()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var mappings = new[]
            {
                //new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
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
                mappings: mappings);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertWithBulkInsertMapItemsAndWithKeepIdentity()
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

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertWithBulkInsertMapItemsAndWithReturnIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, false);
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
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.IsTrue(entities.All(e => e.IdMapped > 0));

            // Assert
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertWithBulkInsertMapItemsAndWithReturnIdentityViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, false);
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
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.IsTrue(entities.All(e => e.IdMapped > 0));

            // Assert
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertWithExistingData()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities);

            // Prepare (Elimination)
            entities = entities
                .Where((entity, index) => index % 2 == 0)
                .ToList();

            // Act
            result = connection.BinaryBulkInsert(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2) - 10, false);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertWithExistingDataAndWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities);

            // Prepare
            entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);

            // Act
            result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    #endregion

    #region BinaryBulkInsert<Anonymous>

    [TestMethod]
    public void TestBinaryBulkInsertViaAnonymous()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaAnonymousWithIdentityValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaAnonymousWithBatchSize()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                batchSize: 3);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaAnonymousWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaAnonymousWithBulkInsertMapItems()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousUnmatchedIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var mappings = new[]
            {
                //new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
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
                mappings: mappings);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaAnonymousWithBulkInsertMapItemsAndWithKeepIdentity()
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

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaAnonymousWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaAnonymousWithExistingData()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities);

            // Prepare (Elimination)
            entities = entities
                .Where((entity, index) => index % 2 == 0)
                .ToList();

            // Act
            result = connection.BinaryBulkInsert(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2) - 10, false);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaAnonymousWithExistingDataAndWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities);

            // Prepare
            entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true, 100);

            // Act
            result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    #endregion

    #region BinaryBulkInsert<IDictionary<string, object?>>

    [TestMethod]
    public void TestBinaryBulkInsertViaExpandoObject()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName).ToList();
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaExpandoObjectWithIdentityValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName).ToList();
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaExpandoObjectWithBatchSize()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                batchSize: 3);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName).ToList();
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaExpandoObjectWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName);
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaExpandoObjectWithReturnIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.IsTrue(entities.All(e => e.Id > 0));

            // Assert
            var queryResult = connection.QueryAll(tableName);
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaExpandoObjectWithReturnIdentityViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.IsTrue(entities.All(e => e.Id > 0));

            // Assert
            var queryResult = connection.QueryAll(tableName);
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaExpandoObjectWithBulkInsertMapItems()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectUnmatchedIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var mappings = new[]
            {
                //new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
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
                mappings: mappings);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName).ToList();
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaExpandoObjectWithBulkInsertMapItemsAndWithKeepIdentity()
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

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName);
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaExpandoObjectWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName);
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaExpandoObjectWithBulkInsertMapItemsAndWithReturnIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectUnmatchedIdentityTables(10, false);
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
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);
            //Assert.IsTrue(entities.All(e => e.IdMapped > 0));
            Assert.IsTrue(entities.All(e => e.Id > 0));

            // Assert
            var queryResult = connection.QueryAll(tableName);
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaExpandoObjectWithBulkInsertMapItemsAndWithReturnIdentityViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectUnmatchedIdentityTables(10, false);
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
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);
            //Assert.IsTrue(entities.All(e => e.IdMapped > 0));
            Assert.IsTrue(entities.All(e => e.Id > 0));

            // Assert
            var queryResult = connection.QueryAll(tableName);
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaExpandoObjectWithExistingData()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities);

            // Prepare (Elimination)
            entities = entities
                .Where((entity, index) => index % 2 == 0)
                .ToList();

            // Act
            result = connection.BinaryBulkInsert(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName).ToList();
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2) - 10, false);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaExpandoObjectWithExistingDataAndWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities);

            // Prepare
            entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true, 100);

            // Act
            result = connection.BinaryBulkInsert(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName).ToList();
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    #endregion

    #region BinaryBulkInsert<DataTable>

    [TestMethod]
    public void TestBinaryBulkInsertViaDataTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                table);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaDataTableWithIdentityValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                table);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaDataTableWithBatchSize()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                table: table,
                batchSize: 3);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaDataTableWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                table: table,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName);
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaDataTableWithReturnIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                table: table,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);
            foreach (DataRow row in table.Rows)
            {
                Assert.IsGreaterThan(0, Convert.ToInt32(row["Id"]));
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaDataTableWithReturnIdentityViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                table: table,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);
            foreach (DataRow row in table.Rows)
            {
                Assert.IsGreaterThan(0, Convert.ToInt32(row["Id"]));
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaDataTableWithBulkInsertMapItems()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);
            var mappings = new[]
            {
                //new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
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
                table: table,
                mappings: mappings);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaDataTableWithBulkInsertMapItemsAndWithKeepIdentity()
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
                table: table,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName);
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaDataTableWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
                table: table,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName);
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaDataTableWithBulkInsertMapItemsAndWithReturnIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, false);
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
                table: table,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);
            foreach (DataRow row in table.Rows)
            {
                Assert.IsGreaterThan(0, Convert.ToInt32(row["Id"]));
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaDataTableWithBulkInsertMapItemsAndWithReturnIdentityViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, false);
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
                table: table,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);
            foreach (DataRow row in table.Rows)
            {
                Assert.IsGreaterThan(0, Convert.ToInt32(row["Id"]));
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaDataTableWithExistingData()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                table: table);

            // Prepare (Elimination)
            entities = entities
                .Where((entity, index) => index % 2 == 0)
                .ToList();
            table = Helper.ToDataTable(tableName, entities);

            // Act
            result = connection.BinaryBulkInsert(
                tableName,
                table: table);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2) - 10, false);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaDataTableWithExistingDataAndWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                table: table);

            // Prepare
            entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
            table = Helper.ToDataTable(tableName, entities);

            // Act
            result = connection.BinaryBulkInsert(
                tableName,
                table: table,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    #endregion

    #region BinaryBulkInsert<DbDataReader>

    [TestMethod]
    public void TestBinaryBulkInsertViaDbDataReader()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = connection.BinaryBulkInsert(
                    tableName,
                    reader);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaDbDataReaderWithIdentityValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = connection.BinaryBulkInsert(
                    tableName,
                    reader);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaDbDataReaderWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = connection.BinaryBulkInsert(
                    tableName,
                    reader: reader,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName);
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaDbDataReaderWithBulkInsertMapItems()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var mappings = new[]
            {
                //new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
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
                var result = connection.BinaryBulkInsert(
                    tableName,
                    reader: reader,
                    mappings: mappings);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaDbDataReaderWithBulkInsertMapItemsAndWithKeepIdentity()
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
                var result = connection.BinaryBulkInsert(
                    tableName,
                    reader: reader,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName);
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaDbDataReaderWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
                var result = connection.BinaryBulkInsert(
                    tableName,
                    reader: reader,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                    pseudoTableType: BulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName);
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaDbDataReaderWithExistingData()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = connection.BinaryBulkInsert(
                    tableName,
                    reader: reader);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Prepare (Elimination)
            entities = entities
                .Where((entity, index) => index % 2 == 0)
                .ToList();

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = connection.BinaryBulkInsert(
                    tableName,
                    reader: reader);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2) - 10, false);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertViaDbDataReaderWithExistingDataAndWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = connection.BinaryBulkInsert(
                    tableName,
                    reader: reader);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Prepare
            entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = connection.BinaryBulkInsert(
                    tableName,
                    reader: reader,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    #endregion

    #endregion

    #region Async

    #region BinaryBulkInsert<TEntity>

    [TestMethod]
    public async Task TestBinaryBulkInsertAsync()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncTableNameWithSchema()
    {
        await using var connection = GetConnection();

        // Prepare
        var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
        var tableName = "public.BulkOperationIdentityTable";

        // Act
        var result = await connection.BinaryBulkInsertAsync(tableName, entities);

        // Assert
        Assert.AreEqual(entities.Count, result);

        // Assert
        var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
        var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
        Assert.AreEqual(entities.Count, assertCount);
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncWithIdentityValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncWithBatchSize()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities,
                batchSize: 3);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncWithReturnIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.IsTrue(entities.All(e => e.Id > 0));

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            Assert.HasCount(entities.Count, queryResult);
            foreach (var entity in entities)
            {
                var target = queryResult.First(item => item.Id == entity.Id);
                Helper.AssertEntityEquality(entity, target);
            }
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncWithReturnIdentityViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.IsTrue(entities.All(e => e.Id > 0));

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncWithMappings()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationMappedIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncWithMappingsAndWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncWithMappingsAndWithKeepIdentityViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncWithMappingsAndWithReturnIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationMappedIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.IsTrue(entities.All(e => e.IdMapped > 0));

            // Assert
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncWithMappingsAndWithReturnIdentityViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationMappedIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.IsTrue(entities.All(e => e.IdMapped > 0));

            // Assert
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncWithBulkInsertMapItems()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var mappings = new[]
            {
                //new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
            };

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities,
                mappings: mappings);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncWithBulkInsertMapItemsAndWithKeepIdentity()
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
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncWithBulkInsertMapItemsAndWithReturnIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, false);
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
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.IsTrue(entities.All(e => e.IdMapped > 0));

            // Assert
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncWithBulkInsertMapItemsAndWithReturnIdentityViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, false);
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
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.IsTrue(entities.All(e => e.IdMapped > 0));

            // Assert
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncWithExistingData()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities);

            // Prepare (Elimination)
            entities = entities
                .Where((entity, index) => index % 2 == 0)
                .ToList();

            // Act
            result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2) - 10, false);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncWithExistingDataAndWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities);

            // Prepare
            entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);

            // Act
            result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    #endregion

    #region BinaryBulkInsert<Anonymous>

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaAnonymous()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaAnonymousWithIdentityValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaAnonymousWithBatchSize()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities,
                batchSize: 3);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaAnonymousWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaAnonymousWithBulkInsertMapItems()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousUnmatchedIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var mappings = new[]
            {
                //new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
            };

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities,
                mappings: mappings);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaAnonymousWithBulkInsertMapItemsAndWithKeepIdentity()
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
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaAnonymousWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaAnonymousWithExistingData()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities);

            // Prepare (Elimination)
            entities = entities
                .Where((entity, index) => index % 2 == 0)
                .ToList();

            // Act
            result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2) - 10, false);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaAnonymousWithExistingDataAndWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities);

            // Prepare
            entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true, 100);

            // Act
            result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    #endregion

    #region BinaryBulkInsert<IDictionary<string, object?>>

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaExpandoObject()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName).ToList();
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaExpandoObjectWithIdentityValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName).ToList();
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaExpandoObjectWithBatchSize()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities,
                batchSize: 3);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName).ToList();
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaExpandoObjectWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName);
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaExpandoObjectWithReturnIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.IsTrue(entities.All(e => e.Id > 0));

            // Assert
            var queryResult = connection.QueryAll(tableName);
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaExpandoObjectWithReturnIdentityViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.IsTrue(entities.All(e => e.Id > 0));

            // Assert
            var queryResult = connection.QueryAll(tableName);
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaExpandoObjectWithBulkInsertMapItems()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectUnmatchedIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var mappings = new[]
            {
                //new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
            };

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities,
                mappings: mappings);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName).ToList();
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaExpandoObjectWithBulkInsertMapItemsAndWithKeepIdentity()
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
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName);
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaExpandoObjectWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName);
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaExpandoObjectWithBulkInsertMapItemsAndWithReturnIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectUnmatchedIdentityTables(10, false);
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
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);
            //Assert.IsTrue(entities.All(e => e.IdMapped > 0));
            Assert.IsTrue(entities.All(e => e.Id > 0));

            // Assert
            var queryResult = connection.QueryAll(tableName);
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaExpandoObjectWithBulkInsertMapItemsAndWithReturnIdentityViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectUnmatchedIdentityTables(10, false);
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
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);
            //Assert.IsTrue(entities.All(e => e.IdMapped > 0));
            Assert.IsTrue(entities.All(e => e.Id > 0));

            // Assert
            var queryResult = connection.QueryAll(tableName);
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaExpandoObjectWithExistingData()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities);

            // Prepare (Elimination)
            entities = entities
                .Where((entity, index) => index % 2 == 0)
                .ToList();

            // Act
            result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName).ToList();
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2) - 10, false);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaExpandoObjectWithExistingDataAndWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities);

            // Prepare
            entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true, 100);

            // Act
            result =  await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName).ToList();
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    #endregion

    #region BinaryBulkInsert<DataTable>

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaDataTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                table);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaDataTableWithIdentityValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                table);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaDataTableWithBatchSize()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                table: table,
                batchSize: 3);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaDataTableWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                table: table,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName);
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaDataTableWithReturnIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                table: table,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);
            foreach (DataRow row in table.Rows)
            {
                Assert.IsGreaterThan(0, Convert.ToInt32(row["Id"]));
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaDataTableWithReturnIdentityViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                table: table,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);
            foreach (DataRow row in table.Rows)
            {
                Assert.IsGreaterThan(0, Convert.ToInt32(row["Id"]));
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaDataTableWithBulkInsertMapItems()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);
            var mappings = new[]
            {
                //new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
            };

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                table: table,
                mappings: mappings);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaDataTableWithBulkInsertMapItemsAndWithKeepIdentity()
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
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                table: table,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName);
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaDataTableWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                table: table,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName);
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaDataTableWithBulkInsertMapItemsAndWithReturnIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, false);
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
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                table: table,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);
            foreach (DataRow row in table.Rows)
            {
                Assert.IsGreaterThan(0, Convert.ToInt32(row["Id"]));
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaDataTableWithBulkInsertMapItemsAndWithReturnIdentityViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, false);
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
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                table: table,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);
            foreach (DataRow row in table.Rows)
            {
                Assert.IsGreaterThan(0, Convert.ToInt32(row["Id"]));
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaDataTableWithExistingData()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                table: table);

            // Prepare (Elimination)
            entities = entities
                .Where((entity, index) => index % 2 == 0)
                .ToList();
            table = Helper.ToDataTable(tableName, entities);

            // Act
            result =  await connection.BinaryBulkInsertAsync(
                tableName,
                table: table);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2) - 10, false);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaDataTableWithExistingDataAndWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result =  await connection.BinaryBulkInsertAsync(
                tableName,
                table: table);

            // Prepare
            entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
            table = Helper.ToDataTable(tableName, entities);

            // Act
            result =  await connection.BinaryBulkInsertAsync(
                tableName,
                table: table,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    #endregion

    #region BinaryBulkInsert<DbDataReader>

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaDbDataReader()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result =  await connection.BinaryBulkInsertAsync(
                    tableName,
                    reader);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaDbDataReaderWithIdentityValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result =  await connection.BinaryBulkInsertAsync(
                    tableName,
                    reader);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaDbDataReaderWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result =  await connection.BinaryBulkInsertAsync(
                    tableName,
                    reader: reader,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName);
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaDbDataReaderWithBulkInsertMapItems()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var mappings = new[]
            {
                //new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
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
                var result =  await connection.BinaryBulkInsertAsync(
                    tableName,
                    reader: reader,
                    mappings: mappings);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaDbDataReaderWithBulkInsertMapItemsAndWithKeepIdentity()
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
                var result =  await connection.BinaryBulkInsertAsync(
                    tableName,
                    reader: reader,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName);
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaDbDataReaderWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
                var result =  await connection.BinaryBulkInsertAsync(
                    tableName,
                    reader: reader,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                    pseudoTableType: BulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName);
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaDbDataReaderWithExistingData()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result =  await connection.BinaryBulkInsertAsync(
                    tableName,
                    reader: reader);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Prepare (Elimination)
            entities = entities
                .Where((entity, index) => index % 2 == 0)
                .ToList();

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result =  await connection.BinaryBulkInsertAsync(
                    tableName,
                    reader: reader);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2) - 10, false);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncViaDbDataReaderWithExistingDataAndWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result =  await connection.BinaryBulkInsertAsync(
                    tableName,
                    reader: reader);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Prepare
            entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result =  await connection.BinaryBulkInsertAsync(
                    tableName,
                    reader: reader,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    #endregion

    #endregion
}
