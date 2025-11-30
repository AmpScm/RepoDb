using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.IntegrationTests.Setup;
using RepoDb.PostgreSql.BulkOperations.IntegrationTests.Models;

namespace RepoDb.PostgreSql.BulkOperations.IntegrationTests.Operations;

[TestClass]
public class BinaryBulkUpdateTest
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

    #region BinaryBulkUpdate<TEntity>

    [TestMethod]
    public void TestBinaryBulkUpdate()
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

            // Prepare
            entities = entities = Helper.UpdateBulkOperationLightIdentityTables(entities);

            // Act
            result = connection.BinaryBulkUpdate(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            Assert.HasCount(10, queryResult);
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkUpdateTableNameWithSchema()
    {
        using var connection = GetConnection();

        // Prepare
        var createdEntities = Helper.CreateBulkOperationLightIdentityTables(10, true);
        var tableName = "public.BulkOperationIdentityTable";

        // Act
        connection.BinaryBulkInsert(tableName,
            entities: createdEntities,
            identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

        // Prepare
        var updatedEntities = Helper.UpdateBulkOperationLightIdentityTables(createdEntities);

        // Act
        var result = connection.BinaryBulkUpdate(tableName, updatedEntities);

        // Assert
        Assert.AreEqual(updatedEntities.Count, result);

        // Assert
        var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
        Assert.HasCount(10, queryResult);

        var assertCount = Helper.AssertEntitiesEquality(updatedEntities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
        Assert.AreEqual(expected: updatedEntities.Count, assertCount);
    }

    [TestMethod]
    public void TestBinaryBulkUpdateWithBatchSize()
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

            // Prepare
            entities = Helper.UpdateBulkOperationLightIdentityTables(entities);

            // Act
            result = connection.BinaryBulkUpdate(
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
    public void TestBinaryBulkUpdateWithQualifiers()
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

            // Prepare
            entities = Helper.UpdateBulkOperationLightIdentityTables(entities);

            // Act
            result = connection.BinaryBulkUpdate(
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
    public void TestBinaryBulkUpdateWithKeepIdentity()
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

            // Prepare
            entities = Helper.UpdateBulkOperationLightIdentityTables(entities);

            // Act
            result = connection.BinaryBulkUpdate(
                tableName,
                entities: entities,
                keepIdentity: true);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkUpdateWithMappings()
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

            // Prepare
            entities = Helper.UpdateBulkOperationMappedIdentityTables(entities);

            // Act
            result = connection.BinaryBulkUpdate(
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
    public void TestBinaryBulkUpdateWithMappingsAndWithKeepIdentity()
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

            // Prepare
            entities = Helper.UpdateBulkOperationMappedIdentityTables(entities);

            // Act
            result = connection.BinaryBulkUpdate(
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
    public void TestBinaryBulkUpdateWithMappingsAndWithKeepIdentityViaPhysicalTable()
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

            // Prepare
            entities = Helper.UpdateBulkOperationMappedIdentityTables(entities);

            // Act
            result = connection.BinaryBulkUpdate(
                tableName,
                entities: entities,
                keepIdentity: true,
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
    public void TestBinaryBulkUpdateWithBulkInsertMapItems()
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

            // Prepare
            entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);

            // Act
            result = connection.BinaryBulkUpdate(
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
    public void TestBinaryBulkUpdateWithBulkInsertMapItemsAndWithKeepIdentity()
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

            // Prepare
            entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);

            // Act
            result = connection.BinaryBulkUpdate(
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
    public void TestBinaryBulkUpdateWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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

            // Prepare
            entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);

            // Act
            result = connection.BinaryBulkUpdate(
                tableName,
                entities: entities,
                mappings: mappings,
                keepIdentity: true,
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
    public void TestBinaryBulkUpdateOnEmptyTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkUpdate(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(0, result);
        }
    }

    #endregion

    #region BinaryBulkUpdate<Anonymous>

    [TestMethod]
    public void TestBinaryBulkUpdateViaAnonymous()
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

            // Prepare
            entities = Helper.UpdateBulkOperationAnonymousLightIdentityTables(entities);

            // Act
            result = connection.BinaryBulkUpdate(
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
    public void TestBinaryBulkUpdateViaAnonymousWithBatchSize()
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

            // Prepare
            entities = Helper.UpdateBulkOperationAnonymousLightIdentityTables(entities);

            // Act
            result = connection.BinaryBulkUpdate(
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
    public void TestBinaryBulkUpdateViaAnonymousWithQualifiers()
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

            // Prepare
            entities = Helper.UpdateBulkOperationAnonymousLightIdentityTables(entities);

            // Act
            result = connection.BinaryBulkUpdate(
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
    public void TestBinaryBulkUpdateViaAnonymousWithKeepIdentity()
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

            // Prepare
            entities = Helper.UpdateBulkOperationAnonymousLightIdentityTables(entities);

            // Act
            result = connection.BinaryBulkUpdate(
                tableName,
                entities: entities,
                keepIdentity: true);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkUpdateViaAnonymousWithBulkInsertMapItems()
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

            // Prepare
            entities = Helper.UpdateBulkOperationAnonymousUnmatchedIdentityTables(entities);

            // Act
            result = connection.BinaryBulkUpdate(
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
    public void TestBinaryBulkUpdateViaAnonymousWithBulkInsertMapItemsAndWithKeepIdentity()
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

            // Prepare
            entities = Helper.UpdateBulkOperationAnonymousUnmatchedIdentityTables(entities);

            // Act
            result = connection.BinaryBulkUpdate(
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
    public void TestBinaryBulkUpdateViaAnonymousWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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

            // Prepare
            entities = Helper.UpdateBulkOperationAnonymousUnmatchedIdentityTables(entities);

            // Act
            result = connection.BinaryBulkUpdate(
                tableName,
                entities: entities,
                mappings: mappings,
                keepIdentity: true,
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
    public void TestBinaryBulkUpdateViaAnonymousOnEmptyTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkUpdate(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(0, result);
        }
    }

    #endregion

    #region BinaryBulkUpdate<IDictionary<string, object?>>

    [TestMethod]
    public void TestBinaryBulkUpdateViaExpandoObject()
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

            // Prepare
            entities = Helper.UpdateBulkOperationExpandoObjectLightIdentityTables(entities);

            // Act
            result = connection.BinaryBulkUpdate(
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
    public void TestBinaryBulkUpdateViaExpandoObjectWithBatchSize()
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

            // Prepare
            entities = Helper.UpdateBulkOperationExpandoObjectLightIdentityTables(entities);

            // Act
            result = connection.BinaryBulkUpdate(
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
    public void TestBinaryBulkUpdateViaExpandoObjectWithQualifiers()
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

            // Prepare
            entities = Helper.UpdateBulkOperationExpandoObjectLightIdentityTables(entities);

            // Act
            result = connection.BinaryBulkUpdate(
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
    public void TestBinaryBulkUpdateViaExpandoObjectWithKeepIdentity()
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

            // Prepare
            entities = Helper.UpdateBulkOperationExpandoObjectLightIdentityTables(entities);

            // Act
            result = connection.BinaryBulkUpdate(
                tableName,
                entities: entities,
                keepIdentity: true);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName);
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkUpdateViaExpandoObjectWithBulkInsertMapItems()
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

            // Prepare
            entities = Helper.UpdateBulkOperationExpandoObjectUnmatchedIdentityTables(entities);

            // Act
            result = connection.BinaryBulkUpdate(
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
    public void TestBinaryBulkUpdateViaExpandoObjectWithBulkInsertMapItemsAndWithKeepIdentity()
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

            // Prepare
            entities = Helper.UpdateBulkOperationExpandoObjectUnmatchedIdentityTables(entities);

            // Act
            result = connection.BinaryBulkUpdate(
                tableName,
                entities: entities,
                mappings: mappings,
                keepIdentity: true);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName);
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkUpdateViaExpandoObjectWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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

            // Prepare
            entities = Helper.UpdateBulkOperationExpandoObjectUnmatchedIdentityTables(entities);

            // Act
            result = connection.BinaryBulkUpdate(
                tableName,
                entities: entities,
                mappings: mappings,
                keepIdentity: true,
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
    public void TestBinaryBulkUpdateViaExpandoObjectOnEmptyTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = connection.BinaryBulkUpdate(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(0, result);
        }
    }

    #endregion

    #region BinaryBulkUpdate<DataTable>

    [TestMethod]
    public void TestBinaryBulkUpdateViaDataTable()
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

            // Prepare
            entities = Helper.UpdateBulkOperationLightIdentityTables(entities);
            table = Helper.ToDataTable(tableName, entities);

            // Act
            result = connection.BinaryBulkUpdate(
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
    public void TestBinaryBulkUpdateViaDataTableWithBatchSize()
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

            // Prepare
            entities = Helper.UpdateBulkOperationLightIdentityTables(entities);
            table = Helper.ToDataTable(tableName, entities);

            // Act
            result = connection.BinaryBulkUpdate(
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
    public void TestBinaryBulkUpdateViaDataTableWithQualifiers()
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

            // Prepare
            entities = Helper.UpdateBulkOperationLightIdentityTables(entities);
            table = Helper.ToDataTable(tableName, entities);

            // Act
            result = connection.BinaryBulkUpdate(
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
    public void TestBinaryBulkUpdateViaDataTableWithKeepIdentity()
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
                table,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Prepare
            entities = Helper.UpdateBulkOperationLightIdentityTables(entities);
            table = Helper.ToDataTable(tableName, entities);

            // Act
            result = connection.BinaryBulkUpdate(
                tableName,
                table: table,
                keepIdentity: true);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkUpdateViaDataTableWithBulkInsertMapItems()
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

            // Prepare
            entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);
            table = Helper.ToDataTable(tableName, entities);

            // Act
            result = connection.BinaryBulkUpdate(
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
    public void TestBinaryBulkUpdateViaDataTableWithBulkInsertMapItemsAndWithKeepIdentity()
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

            // Prepare
            entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);
            table = Helper.ToDataTable(tableName, entities);

            // Act
            result = connection.BinaryBulkUpdate(
                tableName,
                table: table,
                mappings: mappings,
                keepIdentity: true);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName);
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkUpdateViaDataTableWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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

            // Prepare
            entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);
            table = Helper.ToDataTable(tableName, entities);

            // Act
            result = connection.BinaryBulkUpdate(
                tableName,
                table: table,
                mappings: mappings,
                keepIdentity: true,
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
    public void TestBinaryBulkUpdateViaDataTableOnEmptyTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = connection.BinaryBulkUpdate(
                tableName,
                table);

            // Assert
            Assert.AreEqual(0, result);
        }
    }

    #endregion

    #region BinaryBulkUpdate<DbDataReader>

    [TestMethod]
    public void TestBinaryBulkUpdateViaDbDataReader()
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

            // Prepare
            entities = Helper.UpdateBulkOperationLightIdentityTables(entities);

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = connection.BinaryBulkUpdate(
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
    public void TestBinaryBulkUpdateViaDbDataReaderWithQualifiers()
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

            // Prepare
            entities = Helper.UpdateBulkOperationLightIdentityTables(entities);

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = connection.BinaryBulkUpdate(
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
    public void TestBinaryBulkUpdateViaDbDataReaderWithKeepIdentity()
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

            // Prepare
            entities = Helper.UpdateBulkOperationLightIdentityTables(entities);

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = connection.BinaryBulkUpdate(
                    tableName,
                    reader,
                    keepIdentity: true);

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
    public void TestBinaryBulkUpdateViaDbDataReaderWithBulkInsertMapItems()
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

            // Prepare
            entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);

            using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
            {
                // Act
                var result = connection.BinaryBulkUpdate(
                    tableName,
                    reader,
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
    public void TestBinaryBulkUpdateViaDbDataReaderWithBulkInsertMapItemsAndWithKeepIdentity()
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

            // Prepare
            entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);

            using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
            {
                // Act
                var result = connection.BinaryBulkUpdate(
                    tableName,
                    reader,
                    mappings: mappings,
                    keepIdentity: true);

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
    public void TestBinaryBulkUpdateViaDbDataReaderWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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

            // Prepare
            entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);

            using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
            {
                // Act
                var result = connection.BinaryBulkUpdate(
                    tableName,
                    reader,
                    mappings: mappings,
                    keepIdentity: true,
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
    public void TestBinaryBulkUpdateViaDbDataReaderOnEmptyTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = connection.BinaryBulkUpdate(
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

    #region BinaryBulkUpdate<TEntity>

    [TestMethod]
    public async Task TestBinaryBulkUpdateAsync()
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

            // Prepare
            entities = entities = Helper.UpdateBulkOperationLightIdentityTables(entities);

            // Act
            result = await connection.BinaryBulkUpdateAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            Assert.HasCount(10, queryResult);
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkUpdateAsyncTableNameWithSchema()
    {
        await using var connection = GetConnection();

        // Prepare
        var createdEntities = Helper.CreateBulkOperationLightIdentityTables(10, true);
        var tableName = "public.BulkOperationIdentityTable";

        // Act
        await connection.BinaryBulkInsertAsync(tableName,
            entities: createdEntities,
            identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

        // Prepare
        var updatedEntities = Helper.UpdateBulkOperationLightIdentityTables(createdEntities);

        // Act
        var result = await connection.BinaryBulkUpdateAsync(tableName, updatedEntities);

        // Assert
        Assert.AreEqual(updatedEntities.Count, result);

        // Assert
        var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
        Assert.HasCount(10, queryResult);

        var assertCount = Helper.AssertEntitiesEquality(updatedEntities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
        Assert.AreEqual(expected: updatedEntities.Count, assertCount);
    }

    [TestMethod]
    public async Task TestBinaryBulkUpdateAsyncWithBatchSize()
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

            // Prepare
            entities = Helper.UpdateBulkOperationLightIdentityTables(entities);

            // Act
            result = await connection.BinaryBulkUpdateAsync(
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
    public async Task TestBinaryBulkUpdateAsyncWithQualifiers()
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

            // Prepare
            entities = Helper.UpdateBulkOperationLightIdentityTables(entities);

            // Act
            result = await connection.BinaryBulkUpdateAsync(
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
    public async Task TestBinaryBulkUpdateAsyncWithKeepIdentity()
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

            // Prepare
            entities = Helper.UpdateBulkOperationLightIdentityTables(entities);

            // Act
            result = await connection.BinaryBulkUpdateAsync(
                tableName,
                entities: entities,
                keepIdentity: true);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkUpdateAsyncWithMappings()
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

            // Prepare
            entities = Helper.UpdateBulkOperationMappedIdentityTables(entities);

            // Act
            result = await connection.BinaryBulkUpdateAsync(
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
    public async Task TestBinaryBulkUpdateAsyncWithMappingsAndWithKeepIdentity()
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

            // Prepare
            entities = Helper.UpdateBulkOperationMappedIdentityTables(entities);

            // Act
            result = await connection.BinaryBulkUpdateAsync(
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
    public async Task TestBinaryBulkUpdateAsyncWithMappingsAndWithKeepIdentityViaPhysicalTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true, 100);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Prepare
            entities = Helper.UpdateBulkOperationMappedIdentityTables(entities);

            // Act
            result = await connection.BinaryBulkUpdateAsync(
                tableName,
                entities: entities,
                keepIdentity: true,
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
    public async Task TestBinaryBulkUpdateAsyncWithBulkInsertMapItems()
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

            // Prepare
            entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);

            // Act
            result = await connection.BinaryBulkUpdateAsync(
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
    public async Task TestBinaryBulkUpdateAsyncWithBulkInsertMapItemsAndWithKeepIdentity()
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

            // Prepare
            entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);

            // Act
            result = await connection.BinaryBulkUpdateAsync(
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
    public async Task TestBinaryBulkUpdateAsyncWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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

            // Prepare
            entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);

            // Act
            result = await connection.BinaryBulkUpdateAsync(
                tableName,
                entities: entities,
                mappings: mappings,
                keepIdentity: true,
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
    public async Task TestBinaryBulkUpdateAsyncOnEmptyTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkUpdateAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(0, result);
        }
    }

    #endregion

    #region BinaryBulkUpdate<Anonymous>

    [TestMethod]
    public async Task TestBinaryBulkUpdateAsyncViaAnonymous()
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

            // Prepare
            entities = Helper.UpdateBulkOperationAnonymousLightIdentityTables(entities);

            // Act
            result = await connection.BinaryBulkUpdateAsync(
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
    public async Task TestBinaryBulkUpdateAsyncViaAnonymousWithBatchSize()
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

            // Prepare
            entities = Helper.UpdateBulkOperationAnonymousLightIdentityTables(entities);

            // Act
            result = await connection.BinaryBulkUpdateAsync(
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
    public async Task TestBinaryBulkUpdateAsyncViaAnonymousWithQualifiers()
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

            // Prepare
            entities = Helper.UpdateBulkOperationAnonymousLightIdentityTables(entities);

            // Act
            result = await connection.BinaryBulkUpdateAsync(
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
    public async Task TestBinaryBulkUpdateAsyncViaAnonymousWithKeepIdentity()
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

            // Prepare
            entities = Helper.UpdateBulkOperationAnonymousLightIdentityTables(entities);

            // Act
            result = await connection.BinaryBulkUpdateAsync(
                tableName,
                entities: entities,
                keepIdentity: true);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkUpdateAsyncViaAnonymousWithBulkInsertMapItems()
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

            // Prepare
            entities = Helper.UpdateBulkOperationAnonymousUnmatchedIdentityTables(entities);

            // Act
            result = await connection.BinaryBulkUpdateAsync(
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
    public async Task TestBinaryBulkUpdateAsyncViaAnonymousWithBulkInsertMapItemsAndWithKeepIdentity()
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

            // Prepare
            entities = Helper.UpdateBulkOperationAnonymousUnmatchedIdentityTables(entities);

            // Act
            result = await connection.BinaryBulkUpdateAsync(
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
    public async Task TestBinaryBulkUpdateAsyncViaAnonymousWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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

            // Prepare
            entities = Helper.UpdateBulkOperationAnonymousUnmatchedIdentityTables(entities);

            // Act
            result = await connection.BinaryBulkUpdateAsync(
                tableName,
                entities: entities,
                mappings: mappings,
                keepIdentity: true,
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
    public async Task TestBinaryBulkUpdateAsyncViaAnonymousOnEmptyTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkUpdateAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(0, result);
        }
    }

    #endregion

    #region BinaryBulkUpdate<IDictionary<string, object?>>

    [TestMethod]
    public async Task TestBinaryBulkUpdateAsyncViaExpandoObject()
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

            // Prepare
            entities = Helper.UpdateBulkOperationExpandoObjectLightIdentityTables(entities);

            // Act
            result = await connection.BinaryBulkUpdateAsync(
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
    public async Task TestBinaryBulkUpdateAsyncViaExpandoObjectWithBatchSize()
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

            // Prepare
            entities = Helper.UpdateBulkOperationExpandoObjectLightIdentityTables(entities);

            // Act
            result = await connection.BinaryBulkUpdateAsync(
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
    public async Task TestBinaryBulkUpdateAsyncViaExpandoObjectWithQualifiers()
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

            // Prepare
            entities = Helper.UpdateBulkOperationExpandoObjectLightIdentityTables(entities);

            // Act
            result = await connection.BinaryBulkUpdateAsync(
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
    public async Task TestBinaryBulkUpdateAsyncViaExpandoObjectWithKeepIdentity()
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

            // Prepare
            entities = Helper.UpdateBulkOperationExpandoObjectLightIdentityTables(entities);

            // Act
            result = await connection.BinaryBulkUpdateAsync(
                tableName,
                entities: entities,
                keepIdentity: true);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName);
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkUpdateAsyncViaExpandoObjectWithBulkInsertMapItems()
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

            // Prepare
            entities = Helper.UpdateBulkOperationExpandoObjectUnmatchedIdentityTables(entities);

            // Act
            result = await connection.BinaryBulkUpdateAsync(
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
    public async Task TestBinaryBulkUpdateAsyncViaExpandoObjectWithBulkInsertMapItemsAndWithKeepIdentity()
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

            // Prepare
            entities = Helper.UpdateBulkOperationExpandoObjectUnmatchedIdentityTables(entities);

            // Act
            result = await connection.BinaryBulkUpdateAsync(
                tableName,
                entities: entities,
                mappings: mappings,
                keepIdentity: true);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll(tableName);
            var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkUpdateAsyncViaExpandoObjectWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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

            // Prepare
            entities = Helper.UpdateBulkOperationExpandoObjectUnmatchedIdentityTables(entities);

            // Act
            result = await connection.BinaryBulkUpdateAsync(
                tableName,
                entities: entities,
                mappings: mappings,
                keepIdentity: true,
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
    public async Task TestBinaryBulkUpdateAsyncViaExpandoObjectOnEmptyTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            // Act
            var result = await connection.BinaryBulkUpdateAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(0, result);
        }
    }

    #endregion

    #region BinaryBulkUpdate<DataTable>

    [TestMethod]
    public async Task TestBinaryBulkUpdateAsyncViaDataTable()
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

            // Prepare
            entities = Helper.UpdateBulkOperationLightIdentityTables(entities);
            table = Helper.ToDataTable(tableName, entities);

            // Act
            result = await connection.BinaryBulkUpdateAsync(
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
    public async Task TestBinaryBulkUpdateAsyncViaDataTableWithBatchSize()
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

            // Prepare
            entities = Helper.UpdateBulkOperationLightIdentityTables(entities);
            table = Helper.ToDataTable(tableName, entities);

            // Act
            result = await connection.BinaryBulkUpdateAsync(
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
    public async Task TestBinaryBulkUpdateAsyncViaDataTableWithQualifiers()
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

            // Prepare
            entities = Helper.UpdateBulkOperationLightIdentityTables(entities);
            table = Helper.ToDataTable(tableName, entities);

            // Act
            result = await connection.BinaryBulkUpdateAsync(
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
    public async Task TestBinaryBulkUpdateAsyncViaDataTableWithKeepIdentity()
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
                table,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Prepare
            entities = Helper.UpdateBulkOperationLightIdentityTables(entities);
            table = Helper.ToDataTable(tableName, entities);

            // Act
            result = await connection.BinaryBulkUpdateAsync(
                tableName,
                table: table,
                keepIdentity: true);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkUpdateAsyncViaDataTableWithBulkInsertMapItems()
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

            // Prepare
            entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);
            table = Helper.ToDataTable(tableName, entities);

            // Act
            result = await connection.BinaryBulkUpdateAsync(
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
    public async Task TestBinaryBulkUpdateAsyncViaDataTableWithBulkInsertMapItemsAndWithKeepIdentity()
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

            // Prepare
            entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);
            table = Helper.ToDataTable(tableName, entities);

            // Act
            result = await connection.BinaryBulkUpdateAsync(
                tableName,
                table: table,
                mappings: mappings,
                keepIdentity: true);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName);
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkUpdateAsyncViaDataTableWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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

            // Prepare
            entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);
            table = Helper.ToDataTable(tableName, entities);

            // Act
            result = await connection.BinaryBulkUpdateAsync(
                tableName,
                table: table,
                mappings: mappings,
                keepIdentity: true,
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
    public async Task TestBinaryBulkUpdateAsyncViaDataTableOnEmptyTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = await connection.BinaryBulkUpdateAsync(
                tableName,
                table);

            // Assert
            Assert.AreEqual(0, result);
        }
    }

    #endregion

    #region BinaryBulkUpdate<DbDataReader>

    [TestMethod]
    public async Task TestBinaryBulkUpdateAsyncViaDbDataReader()
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

            // Prepare
            entities = Helper.UpdateBulkOperationLightIdentityTables(entities);

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = await connection.BinaryBulkUpdateAsync(
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
    public async Task TestBinaryBulkUpdateAsyncViaDbDataReaderWithQualifiers()
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

            // Prepare
            entities = Helper.UpdateBulkOperationLightIdentityTables(entities);

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = await connection.BinaryBulkUpdateAsync(
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
    public async Task TestBinaryBulkUpdateAsyncViaDbDataReaderWithKeepIdentity()
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

            // Prepare
            entities = Helper.UpdateBulkOperationLightIdentityTables(entities);

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = await connection.BinaryBulkUpdateAsync(
                    tableName,
                    reader,
                    keepIdentity: true);

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
    public async Task TestBinaryBulkUpdateAsyncViaDbDataReaderWithBulkInsertMapItems()
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

            // Prepare
            entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);

            using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
            {
                // Act
                var result = await connection.BinaryBulkUpdateAsync(
                    tableName,
                    reader,
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
    public async Task TestBinaryBulkUpdateAsyncViaDbDataReaderWithBulkInsertMapItemsAndWithKeepIdentity()
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

            // Prepare
            entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);

            using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
            {
                // Act
                var result = await connection.BinaryBulkUpdateAsync(
                    tableName,
                    reader,
                    mappings: mappings,
                    keepIdentity: true);

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
    public async Task TestBinaryBulkUpdateAsyncViaDbDataReaderWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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

            // Prepare
            entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);

            using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
            {
                // Act
                var result = await connection.BinaryBulkUpdateAsync(
                    tableName,
                    reader,
                    mappings: mappings,
                    keepIdentity: true,
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
    public async Task TestBinaryBulkUpdateAsyncViaDbDataReaderOnEmptyTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "BulkOperationIdentityTable";

            using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
            {
                // Act
                var result = await connection.BinaryBulkUpdateAsync(
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
