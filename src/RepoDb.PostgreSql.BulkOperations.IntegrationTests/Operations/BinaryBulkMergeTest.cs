using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.IntegrationTests.Setup;
using RepoDb.PostgreSql.BulkOperations.IntegrationTests.Models;
using System.Data;

namespace RepoDb.PostgreSql.BulkOperations.IntegrationTests.Operations;

[TestClass]
public class BinaryBulkMergeTest
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

    #region BinaryBulkMerge<TEntity>

    [TestMethod]
    public void TestBinaryBulkMerge()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeTableNameWithSchema()
    {
        using var connection = GetConnection();

        // Prepare
        var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
        var tableName = "public.BulkOperationIdentityTable";

        // Act
        var result = connection.BinaryBulkMerge(tableName, entities);

        // Assert
        Assert.AreEqual(entities.Count, result);

        // Assert
        var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
        var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
        Assert.AreEqual(entities.Count, assertCount);
    }

    [TestMethod]
    public void TestBinaryBulkMergeWithBatchSize()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeWithQualifiers()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                entities: entities,
                qualifiers: Field.From(
                    nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                    nameof(BulkOperationLightIdentityTable.ColumnInteger)));

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeWithReturnIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.IsTrue(entities.All(e => e.Id > 0));

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            Assert.AreEqual(entities.Count, queryResult.Count);
            foreach (var entity in entities)
            {
                var target = queryResult.First(item => item.Id == entity.Id);
                Helper.AssertEntityEquality(entity, target);
            }
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeWithReturnIdentityViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.IsTrue(entities.All(e => e.Id > 0));

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeWithMappings()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationMappedIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeWithMappingsAndWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeWithMappingsAndWithKeepIdentityViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeWithMappingsAndWithReturnIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationMappedIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeWithMappingsAndWithReturnIdentityViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationMappedIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeWithBulkInsertMapItems()
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
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeWithBulkInsertMapItemsAndWithKeepIdentity()
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
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeWithBulkInsertMapItemsAndWithReturnIdentity()
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
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeWithBulkInsertMapItemsAndWithReturnIdentityViaPhysicalTable()
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
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeWithOnConflictDoUpdateMergeCommandType()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                entities: entities,
                mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeWithOnConflictDoUpdateMergeCommandTypeAndWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentity()
    {
        using (var connection = GetConnection())
        {
            // Since the Id column is Primary/Identity, we are require to pass a value into it.
            // In order to ensure that the assertion that the identity value is returned, we need
            // create a series of records and validate the entities

            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            connection.BinaryBulkMerge(
                tableName,
                entities: entities);

            // Act (More)
            var result = connection.BinaryBulkMerge(
               tableName,
               entities: entities,
               identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
               mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.IsTrue(entities.All(e => e.Id > 0));

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentityViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Since the Id column is Primary/Identity, we are require to pass a value into it.
            // In order to ensure that the assertion that the identity value is returned, we need
            // create a series of records and validate the entities

            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            connection.BinaryBulkMerge(
                tableName,
                entities: entities);

            // Act (More)
            var result = connection.BinaryBulkMerge(
               tableName,
               entities: entities,
               identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
               mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate,
               pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.IsTrue(entities.All(e => e.Id > 0));

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeWithExistingData()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                entities: entities);

            // Prepare (Elimination)
            entities = entities
                .Where((entity, index) => index % 2 == 0)
                .ToList();

            // Act
            result = connection.BinaryBulkMerge(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            Assert.AreEqual(10, queryResult.Count);
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeWithNoIdentityValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeWithNoIdentityValuesAndWithExistingData()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                entities: entities);

            // Prepare (Elimination)
            entities = entities
                .Where((entity, index) => index % 2 == 0)
                .ToList();

            // Act
            result = connection.BinaryBulkMerge(
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

    #endregion

    #region BinaryBulkMerge<Anonymous>

    [TestMethod]
    public void TestBinaryBulkMergeViaAnonymous()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeViaAnonymousWithBatchSize()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeViaAnonymousWithQualifiers()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                entities: entities,
                qualifiers: Field.From(
                    nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                    nameof(BulkOperationLightIdentityTable.ColumnInteger)));

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeViaAnonymousWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeViaAnonymousWithBulkInsertMapItems()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousUnmatchedIdentityTables(10, false);
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
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeViaAnonymousWithBulkInsertMapItemsAndWithKeepIdentity()
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
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeViaAnonymousWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeViaAnonymousWithOnConflictDoUpdateMergeCommandType()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                entities: entities,
                mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeViaAnonymousWithOnConflictDoUpdateMergeCommandTypeAndWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeViaAnonymousWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentity()
    {
        using (var connection = GetConnection())
        {
            // Since the Id column is Primary/Identity, we are require to pass a value into it.
            // In order to ensure that the assertion that the identity value is returned, we need
            // create a series of records and validate the entities

            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            connection.BinaryBulkMerge(
                tableName,
                entities: entities);

            // Act (More)
            var result = connection.BinaryBulkMerge(
               tableName,
               entities: entities,
               identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
               mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

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
    public void TestBinaryBulkMergeViaAnonymousWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentityViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Since the Id column is Primary/Identity, we are require to pass a value into it.
            // In order to ensure that the assertion that the identity value is returned, we need
            // create a series of records and validate the entities

            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            connection.BinaryBulkMerge(
                tableName,
                entities: entities);

            // Act (More)
            var result = connection.BinaryBulkMerge(
               tableName,
               entities: entities,
               identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
               mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate,
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
    public void TestBinaryBulkMergeViaAnonymousWithExistingData()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                entities: entities);

            // Prepare (Elimination)
            entities = entities
                .Where((entity, index) => index % 2 == 0)
                .ToList();

            // Act
            result = connection.BinaryBulkMerge(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            Assert.AreEqual(10, queryResult.Count);
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeViaAnonymousWithNoIdentityValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeViaAnonymousWithNoIdentityValuesAndWithExistingData()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                entities: entities);

            // Prepare (Elimination)
            entities = entities
                .Where((entity, index) => index % 2 == 0)
                .ToList();

            // Act
            result = connection.BinaryBulkMerge(
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

    #endregion

    #region BinaryBulkMerge<IDictionary<string, object>>

    [TestMethod]
    public void TestBinaryBulkMergeViaExpandoObject()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeViaExpandoObjectWithBatchSize()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeViaExpandoObjectWithQualifiers()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                entities: entities,
                qualifiers: Field.From(
                    nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                    nameof(BulkOperationLightIdentityTable.ColumnInteger)));

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName).ToList();
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeViaExpandoObjectWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeViaExpandoObjectWithReturnIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeViaExpandoObjectWithReturnIdentityViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeViaExpandoObjectWithBulkInsertMapItems()
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
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeViaExpandoObjectWithBulkInsertMapItemsAndWithKeepIdentity()
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
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeViaExpandoObjectWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeViaExpandoObjectWithBulkInsertMapItemsAndWithReturnIdentity()
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
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeViaExpandoObjectWithBulkInsertMapItemsAndWithReturnIdentityViaPhysicalTable()
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
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeViaExpandoObjectWithOnConflictDoUpdateMergeCommandType()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeViaExpandoObjectWithOnConflictDoUpdateMergeCommandTypeAndWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName).ToList();
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeViaExpandoObjectWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentity()
    {
        using (var connection = GetConnection())
        {
            // Since the Id column is Primary/Identity, we are require to pass a value into it.
            // In order to ensure that the assertion that the identity value is returned, we need
            // create a series of records and validate the entities

            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            connection.BinaryBulkMerge(
                tableName,
                entities: entities);

            // Act (More)
            var result = connection.BinaryBulkMerge(
               tableName,
               entities: entities,
               identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
               mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.IsTrue(entities.All(e => e.Id > 0));

            // Assert
            var queryResult = connection.QueryAll(tableName).ToList();
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeViaExpandoObjectWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentityViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Since the Id column is Primary/Identity, we are require to pass a value into it.
            // In order to ensure that the assertion that the identity value is returned, we need
            // create a series of records and validate the entities

            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            connection.BinaryBulkMerge(
                tableName,
                entities: entities);

            // Act (More)
            var result = connection.BinaryBulkMerge(
               tableName,
               entities: entities,
               identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
               mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate,
               pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.IsTrue(entities.All(e => e.Id > 0));

            // Assert
            var queryResult = connection.QueryAll(tableName).ToList();
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeViaExpandoObjectWithExistingData()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                entities: entities);

            // Prepare (Elimination)
            entities = entities
                .Where((entity, index) => index % 2 == 0)
                .ToList();

            // Act
            result = connection.BinaryBulkMerge(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName).ToList();
            Assert.AreEqual(10, queryResult.Count);
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeViaExpandoObjectWithNoIdentityValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeViaExpandoObjectWithNoIdentityValuesAndWithExistingData()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                entities: entities);

            // Prepare (Elimination)
            entities = entities
                .Where((entity, index) => index % 2 == 0)
                .ToList();

            // Act
            result = connection.BinaryBulkMerge(
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

    #endregion

    #region BinaryBulkMerge<DataTable>

    [TestMethod]
    public void TestBinaryBulkMergeViaDataTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeViaDataTableWithBatchSize()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeViaDataTableWithQualifiers()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                table: table,
                qualifiers: Field.From(
                    nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                    nameof(BulkOperationLightIdentityTable.ColumnInteger)));

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeViaDataTableWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                table: table,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeViaDataTableWithReturnIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                table: table,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);
            foreach (DataRow row in table.Rows)
            {
                Assert.IsTrue(Convert.ToInt32(row["Id"]) > 0);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeViaDataTableWithReturnIdentityViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                table: table,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);
            foreach (DataRow row in table.Rows)
            {
                Assert.IsTrue(Convert.ToInt32(row["Id"]) > 0);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeViaDataTableWithBulkInsertMapItems()
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
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeViaDataTableWithBulkInsertMapItemsAndWithKeepIdentity()
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
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeViaDataTableWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
            var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeViaDataTableWithBulkInsertMapItemsAndWithReturnIdentity()
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
            var result = connection.BinaryBulkMerge(
                tableName,
                table: table,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);
            foreach (DataRow row in table.Rows)
            {
                Assert.IsTrue(Convert.ToInt32(row["Id"]) > 0);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeViaDataTableWithBulkInsertMapItemsAndWithReturnIdentityViaPhysicalTable()
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
            var result = connection.BinaryBulkMerge(
                tableName,
                table: table,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);
            foreach (DataRow row in table.Rows)
            {
                Assert.IsTrue(Convert.ToInt32(row["Id"]) > 0);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeViaDataTableWithOnConflictDoUpdateMergeCommandType()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                table: table,
                mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeViaDataTableWithOnConflictDoUpdateMergeCommandTypeAndWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                table: table,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeViaDataTableWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentity()
    {
        using (var connection = GetConnection())
        {
            // Since the Id column is Primary/Identity, we are require to pass a value into it.
            // In order to ensure that the assertion that the identity value is returned, we need
            // create a series of records and validate the entities

            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            connection.BinaryBulkMerge(
                tableName,
                table: table);

            // Act (More)
            var result = connection.BinaryBulkMerge(
               tableName,
               table: table,
               identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
               mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

            // Assert
            Assert.AreEqual(entities.Count, result);
            foreach (DataRow row in table.Rows)
            {
                Assert.IsTrue(Convert.ToInt32(row["Id"]) > 0);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeViaDataTableWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentityViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Since the Id column is Primary/Identity, we are require to pass a value into it.
            // In order to ensure that the assertion that the identity value is returned, we need
            // create a series of records and validate the entities

            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            connection.BinaryBulkMerge(
                tableName,
                table: table);

            // Act (More)
            var result = connection.BinaryBulkMerge(
               tableName,
               table: table,
               identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
               mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate,
               pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);
            foreach (DataRow row in table.Rows)
            {
                Assert.IsTrue(Convert.ToInt32(row["Id"]) > 0);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeViaDataTableWithExistingData()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                table: table);

            // Prepare (Elimination)
            entities = entities
                .Where((entity, index) => index % 2 == 0)
                .ToList();
            table = Helper.ToDataTable(tableName, entities);

            // Act
            result = connection.BinaryBulkMerge(
                tableName,
                table: table);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            Assert.AreEqual(10, queryResult.Count);
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeViaDataTableWithNoIdentityValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                table: table);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeViaDataTableWithNoIdentityValuesAndWithExistingData()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                table: table);

            // Prepare (Elimination)
            entities = entities
                .Where((entity, index) => index % 2 == 0)
                .ToList();
            table = Helper.ToDataTable(tableName, entities);

            // Act
            result = connection.BinaryBulkMerge(
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

    #endregion

    #region BinaryBulkMerge<DbDataReader>

    [TestMethod]
    public void TestBinaryBulkMergeViaDbDataReader()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeViaDbDataReaderWithQualifiers()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = connection.BinaryBulkMerge(
                    tableName,
                    reader,
                    qualifiers: Field.From(
                        nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                        nameof(BulkOperationLightIdentityTable.ColumnInteger)));

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
    public void TestBinaryBulkMergeViaDbDataReaderWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = connection.BinaryBulkMerge(
                    tableName,
                    reader: reader,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

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
    public void TestBinaryBulkMergeViaDbDataReaderWithBulkInsertMapItems()
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

            using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
            {
                // Act
                var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeViaDbDataReaderWithBulkInsertMapItemsAndWithKeepIdentity()
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
                var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeViaDbDataReaderWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
                var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeViaDbDataReaderWithOnConflictDoUpdateMergeCommandType()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = connection.BinaryBulkMerge(
                    tableName,
                    reader,
                    mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

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
    public void TestBinaryBulkMergeViaDbDataReaderWithOnConflictDoUpdateMergeCommandTypeAndWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = connection.BinaryBulkMerge(
                    tableName,
                    reader,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                    mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

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
    public void TestBinaryBulkMergeViaDbDataReaderWithExistingData()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = connection.BinaryBulkMerge(
                    tableName,
                    reader);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Prepare (Elimination)
            entities = entities
                .Where((entity, index) => index % 2 == 0)
                .ToList();

            // Act
            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = connection.BinaryBulkMerge(
                    tableName,
                    reader);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            Assert.AreEqual(10, queryResult.Count);
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeViaDbDataReaderWithNoIdentityValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = connection.BinaryBulkMerge(
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
    public void TestBinaryBulkMergeViaDbDataReaderWithNoIdentityValuesAndWithExistingData()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = connection.BinaryBulkMerge(
                    tableName,
                    reader);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Prepare (Elimination)
            entities = entities
                .Where((entity, index) => index % 2 == 0)
                .ToList();

            // Act
            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = connection.BinaryBulkMerge(
                    tableName,
                    reader);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2) - 10, false);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    #endregion

    #endregion

    #region Async

    #region BinaryBulkMergeAsync<TEntity>

    [TestMethod]
    public async Task TestBinaryBulkMergeAsync()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncTableNameWithSchema()
    {
        await using var connection = GetConnection();

        // Prepare
        var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
        var tableName = "public.BulkOperationIdentityTable";

        // Act
        var result = await connection.BinaryBulkMergeAsync(tableName, entities);

        // Assert
        Assert.AreEqual(entities.Count, result);

        // Assert
        var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
        var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
        Assert.AreEqual(entities.Count, assertCount);
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncWithBatchSize()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncWithQualifiers()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                entities: entities,
                qualifiers: Field.From(
                    nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                    nameof(BulkOperationLightIdentityTable.ColumnInteger)));

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncWithReturnIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.IsTrue(entities.All(e => e.Id > 0));

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            Assert.AreEqual(entities.Count, queryResult.Count);
            foreach (var entity in entities)
            {
                var target = queryResult.First(item => item.Id == entity.Id);
                Helper.AssertEntityEquality(entity, target);
            }
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncWithReturnIdentityViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.IsTrue(entities.All(e => e.Id > 0));

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncWithMappings()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationMappedIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncWithMappingsAndWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncWithMappingsAndWithKeepIdentityViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncWithMappingsAndWithReturnIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationMappedIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncWithMappingsAndWithReturnIdentityViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationMappedIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncWithBulkInsertMapItems()
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
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncWithBulkInsertMapItemsAndWithKeepIdentity()
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
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncWithBulkInsertMapItemsAndWithReturnIdentity()
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
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncWithBulkInsertMapItemsAndWithReturnIdentityViaPhysicalTable()
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
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncWithOnConflictDoUpdateMergeCommandType()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                entities: entities,
                mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncWithOnConflictDoUpdateMergeCommandTypeAndWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentity()
    {
        using (var connection = GetConnection())
        {
            // Since the Id column is Primary/Identity, we are require to pass a value into it.
            // In order to ensure that the assertion that the identity value is returned, we need
            // create a series of records and validate the entities

            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            connection.BinaryBulkMergeAsync(
                tableName,
                entities: entities).Wait();

            // Act (More)
            var result = await connection.BinaryBulkMergeAsync(
               tableName,
               entities: entities,
               identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
               mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.IsTrue(entities.All(e => e.Id > 0));

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentityViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Since the Id column is Primary/Identity, we are require to pass a value into it.
            // In order to ensure that the assertion that the identity value is returned, we need
            // create a series of records and validate the entities

            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            connection.BinaryBulkMergeAsync(
                tableName,
                entities: entities).Wait();

            // Act (More)
            var result = await connection.BinaryBulkMergeAsync(
               tableName,
               entities: entities,
               identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
               mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate,
               pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.IsTrue(entities.All(e => e.Id > 0));

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncWithExistingData()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                entities: entities);

            // Prepare (Elimination)
            entities = entities
                .Where((entity, index) => index % 2 == 0)
                .ToList();

            // Act
            result =  await connection.BinaryBulkMergeAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            Assert.AreEqual(10, queryResult.Count);
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncWithNoIdentityValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncWithNoIdentityValuesAndWithExistingData()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                entities: entities);

            // Prepare (Elimination)
            entities = entities
                .Where((entity, index) => index % 2 == 0)
                .ToList();

            // Act
            result =  await connection.BinaryBulkMergeAsync(
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

    #endregion

    #region BinaryBulkMergeAsync<Anonymous>

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncViaAnonymous()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncViaAnonymousWithBatchSize()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncViaAnonymousWithQualifiers()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                entities: entities,
                qualifiers: Field.From(
                    nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                    nameof(BulkOperationLightIdentityTable.ColumnInteger)));

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncViaAnonymousWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncViaAnonymousWithBulkInsertMapItems()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousUnmatchedIdentityTables(10, false);
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
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncViaAnonymousWithBulkInsertMapItemsAndWithKeepIdentity()
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
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncViaAnonymousWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncViaAnonymousWithOnConflictDoUpdateMergeCommandType()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                entities: entities,
                mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncViaAnonymousWithOnConflictDoUpdateMergeCommandTypeAndWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncViaAnonymousWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentity()
    {
        using (var connection = GetConnection())
        {
            // Since the Id column is Primary/Identity, we are require to pass a value into it.
            // In order to ensure that the assertion that the identity value is returned, we need
            // create a series of records and validate the entities

            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            connection.BinaryBulkMergeAsync(
                tableName,
                entities: entities).Wait();

            // Act (More)
            var result = await connection.BinaryBulkMergeAsync(
               tableName,
               entities: entities,
               identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
               mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

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
    public async Task TestBinaryBulkMergeAsyncViaAnonymousWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentityViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Since the Id column is Primary/Identity, we are require to pass a value into it.
            // In order to ensure that the assertion that the identity value is returned, we need
            // create a series of records and validate the entities

            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            connection.BinaryBulkMergeAsync(
                tableName,
                entities: entities).Wait();

            // Act (More)
            var result = await connection.BinaryBulkMergeAsync(
               tableName,
               entities: entities,
               identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
               mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate,
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
    public async Task TestBinaryBulkMergeAsyncViaAnonymousWithExistingData()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                entities: entities);

            // Prepare (Elimination)
            entities = entities
                .Where((entity, index) => index % 2 == 0)
                .ToList();

            // Act
            result =  await connection.BinaryBulkMergeAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            Assert.AreEqual(10, queryResult.Count);
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncViaAnonymousWithNoIdentityValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncViaAnonymousWithNoIdentityValuesAndWithExistingData()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                entities: entities);

            // Prepare (Elimination)
            entities = entities
                .Where((entity, index) => index % 2 == 0)
                .ToList();

            // Act
            result =  await connection.BinaryBulkMergeAsync(
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

    #endregion

    #region BinaryBulkMergeAsync<IDictionary<string, object>>

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncViaExpandoObject()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncViaExpandoObjectWithBatchSize()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncViaExpandoObjectWithQualifiers()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                entities: entities,
                qualifiers: Field.From(
                    nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                    nameof(BulkOperationLightIdentityTable.ColumnInteger)));

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName).ToList();
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncViaExpandoObjectWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncViaExpandoObjectWithReturnIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncViaExpandoObjectWithReturnIdentityViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncViaExpandoObjectWithBulkInsertMapItems()
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
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncViaExpandoObjectWithBulkInsertMapItemsAndWithKeepIdentity()
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
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncViaExpandoObjectWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncViaExpandoObjectWithBulkInsertMapItemsAndWithReturnIdentity()
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
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncViaExpandoObjectWithBulkInsertMapItemsAndWithReturnIdentityViaPhysicalTable()
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
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncViaExpandoObjectWithOnConflictDoUpdateMergeCommandType()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncViaExpandoObjectWithOnConflictDoUpdateMergeCommandTypeAndWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName).ToList();
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncViaExpandoObjectWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentity()
    {
        using (var connection = GetConnection())
        {
            // Since the Id column is Primary/Identity, we are require to pass a value into it.
            // In order to ensure that the assertion that the identity value is returned, we need
            // create a series of records and validate the entities

            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            connection.BinaryBulkMergeAsync(
                tableName,
                entities: entities).Wait();

            // Act (More)
            var result = await connection.BinaryBulkMergeAsync(
               tableName,
               entities: entities,
               identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
               mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.IsTrue(entities.All(e => e.Id > 0));

            // Assert
            var queryResult = connection.QueryAll(tableName).ToList();
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncViaExpandoObjectWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentityViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Since the Id column is Primary/Identity, we are require to pass a value into it.
            // In order to ensure that the assertion that the identity value is returned, we need
            // create a series of records and validate the entities

            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            connection.BinaryBulkMergeAsync(
                tableName,
                entities: entities).Wait();

            // Act (More)
            var result = await connection.BinaryBulkMergeAsync(
               tableName,
               entities: entities,
               identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
               mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate,
               pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.IsTrue(entities.All(e => e.Id > 0));

            // Assert
            var queryResult = connection.QueryAll(tableName).ToList();
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncViaExpandoObjectWithExistingData()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                entities: entities);

            // Prepare (Elimination)
            entities = entities
                .Where((entity, index) => index % 2 == 0)
                .ToList();

            // Act
            result =  await connection.BinaryBulkMergeAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName).ToList();
            Assert.AreEqual(10, queryResult.Count);
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncViaExpandoObjectWithNoIdentityValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncViaExpandoObjectWithNoIdentityValuesAndWithExistingData()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                entities: entities);

            // Prepare (Elimination)
            entities = entities
                .Where((entity, index) => index % 2 == 0)
                .ToList();

            // Act
            result =  await connection.BinaryBulkMergeAsync(
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

    #endregion

    #region BinaryBulkMergeAsync<DataTable>

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncViaDataTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncViaDataTableWithBatchSize()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncViaDataTableWithQualifiers()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                table: table,
                qualifiers: Field.From(
                    nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                    nameof(BulkOperationLightIdentityTable.ColumnInteger)));

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncViaDataTableWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                table: table,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncViaDataTableWithReturnIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                table: table,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);
            foreach (DataRow row in table.Rows)
            {
                Assert.IsTrue(Convert.ToInt32(row["Id"]) > 0);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncViaDataTableWithReturnIdentityViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                table: table,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);
            foreach (DataRow row in table.Rows)
            {
                Assert.IsTrue(Convert.ToInt32(row["Id"]) > 0);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncViaDataTableWithBulkInsertMapItems()
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
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncViaDataTableWithBulkInsertMapItemsAndWithKeepIdentity()
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
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncViaDataTableWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
            var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncViaDataTableWithBulkInsertMapItemsAndWithReturnIdentity()
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
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                table: table,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);
            foreach (DataRow row in table.Rows)
            {
                Assert.IsTrue(Convert.ToInt32(row["Id"]) > 0);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncViaDataTableWithBulkInsertMapItemsAndWithReturnIdentityViaPhysicalTable()
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
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                table: table,
                mappings: mappings,
                identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);
            foreach (DataRow row in table.Rows)
            {
                Assert.IsTrue(Convert.ToInt32(row["Id"]) > 0);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncViaDataTableWithOnConflictDoUpdateMergeCommandType()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                table: table,
                mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncViaDataTableWithOnConflictDoUpdateMergeCommandTypeAndWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                table: table,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncViaDataTableWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentity()
    {
        using (var connection = GetConnection())
        {
            // Since the Id column is Primary/Identity, we are require to pass a value into it.
            // In order to ensure that the assertion that the identity value is returned, we need
            // create a series of records and validate the entities

            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            connection.BinaryBulkMergeAsync(
                tableName,
                table: table).Wait();

            // Act (More)
            var result = await connection.BinaryBulkMergeAsync(
               tableName,
               table: table,
               identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
               mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

            // Assert
            Assert.AreEqual(entities.Count, result);
            foreach (DataRow row in table.Rows)
            {
                Assert.IsTrue(Convert.ToInt32(row["Id"]) > 0);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncViaDataTableWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentityViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Since the Id column is Primary/Identity, we are require to pass a value into it.
            // In order to ensure that the assertion that the identity value is returned, we need
            // create a series of records and validate the entities

            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            connection.BinaryBulkMergeAsync(
                tableName,
                table: table).Wait();

            // Act (More)
            var result = await connection.BinaryBulkMergeAsync(
               tableName,
               table: table,
               identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
               mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate,
               pseudoTableType: BulkImportPseudoTableType.Physical);

            // Assert
            Assert.AreEqual(entities.Count, result);
            foreach (DataRow row in table.Rows)
            {
                Assert.IsTrue(Convert.ToInt32(row["Id"]) > 0);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncViaDataTableWithExistingData()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                table: table);

            // Prepare (Elimination)
            entities = entities
                .Where((entity, index) => index % 2 == 0)
                .ToList();
            table = Helper.ToDataTable(tableName, entities);

            // Act
            result =  await connection.BinaryBulkMergeAsync(
                tableName,
                table: table);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            Assert.AreEqual(10, queryResult.Count);
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncViaDataTableWithNoIdentityValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                table: table);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncViaDataTableWithNoIdentityValuesAndWithExistingData()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                table: table);

            // Prepare (Elimination)
            entities = entities
                .Where((entity, index) => index % 2 == 0)
                .ToList();
            table = Helper.ToDataTable(tableName, entities);

            // Act
            result =  await connection.BinaryBulkMergeAsync(
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

    #endregion

    #region BinaryBulkMergeAsync<DbDataReader>

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncViaDbDataReader()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncViaDbDataReaderWithQualifiers()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = await connection.BinaryBulkMergeAsync(
                    tableName,
                    reader,
                    qualifiers: Field.From(
                        nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                        nameof(BulkOperationLightIdentityTable.ColumnInteger)));

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
    public async Task TestBinaryBulkMergeAsyncViaDbDataReaderWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = await connection.BinaryBulkMergeAsync(
                    tableName,
                    reader: reader,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

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
    public async Task TestBinaryBulkMergeAsyncViaDbDataReaderWithBulkInsertMapItems()
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

            using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
            {
                // Act
                var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncViaDbDataReaderWithBulkInsertMapItemsAndWithKeepIdentity()
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
                var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncViaDbDataReaderWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
                var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncViaDbDataReaderWithOnConflictDoUpdateMergeCommandType()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = await connection.BinaryBulkMergeAsync(
                    tableName,
                    reader,
                    mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

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
    public async Task TestBinaryBulkMergeAsyncViaDbDataReaderWithOnConflictDoUpdateMergeCommandTypeAndWithKeepIdentity()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = await connection.BinaryBulkMergeAsync(
                    tableName,
                    reader,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                    mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

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
    public async Task TestBinaryBulkMergeAsyncViaDbDataReaderWithExistingData()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = await connection.BinaryBulkMergeAsync(
                    tableName,
                    reader);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Prepare (Elimination)
            entities = entities
                .Where((entity, index) => index % 2 == 0)
                .ToList();

            // Act
            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = await connection.BinaryBulkMergeAsync(
                    tableName,
                    reader);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            Assert.AreEqual(10, queryResult.Count);
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncViaDbDataReaderWithNoIdentityValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = await connection.BinaryBulkMergeAsync(
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
    public async Task TestBinaryBulkMergeAsyncViaDbDataReaderWithNoIdentityValuesAndWithExistingData()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "BulkOperationIdentityTable";

            // Act
            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = await connection.BinaryBulkMergeAsync(
                    tableName,
                    reader);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Prepare (Elimination)
            entities = entities
                .Where((entity, index) => index % 2 == 0)
                .ToList();

            // Act
            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = await connection.BinaryBulkMergeAsync(
                    tableName,
                    reader);

                // Assert
                Assert.AreEqual(entities.Count, result);
            }

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2) - 10, false);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    #endregion

    #endregion
}
