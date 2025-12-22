using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.IntegrationTests.Setup;

namespace RepoDb.PostgreSql.BulkOperations.IntegrationTests.Operations;

[TestClass]
public class BinaryBulkDeleteByKeyTest
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

    [TestMethod]
    public void TestBinaryBulkDeleteByKey()
    {
        using var connection = GetConnection();
        // Prepare
        var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
        var primaryKeys = entities.Select(entity => entity.Id);
        var tableName = "BulkOperationIdentityTable";

        // Act
        var result = connection.BinaryBulkInsert(
            tableName,
            entities: entities,
            identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

        // Act
        result = connection.BinaryBulkDeleteByKey(
            tableName,
            primaryKeys: primaryKeys);

        // Assert
        Assert.AreEqual(entities.Count, result);

        // Assert
        var countResult = connection.CountAll(tableName);
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public void TestBinaryBulkDeleteByKeyWithBatchSize()
    {
        using var connection = GetConnection();
        // Prepare
        var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
        var primaryKeys = entities.Select(entity => entity.Id);
        var tableName = "BulkOperationIdentityTable";

        // Act
        var result = connection.BinaryBulkInsert(
            tableName,
            entities: entities,
            identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

        // Act
        result = connection.BinaryBulkDeleteByKey(
            tableName,
            primaryKeys: primaryKeys,
            batchSize: 3);

        // Assert
        Assert.AreEqual(entities.Count, result);

        // Assert
        var countResult = connection.CountAll(tableName);
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public void TestBinaryBulkDeleteByKeyViaPhysicalTable()
    {
        using var connection = GetConnection();
        // Prepare
        var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
        var primaryKeys = entities.Select(entity => entity.Id);
        var tableName = "BulkOperationIdentityTable";

        // Act
        var result = connection.BinaryBulkInsert(
            tableName,
            entities: entities,
            identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

        // Act
        result = connection.BinaryBulkDeleteByKey(
            tableName,
            primaryKeys: primaryKeys,
            pseudoTableType: BulkImportPseudoTableType.Physical);

        // Assert
        Assert.AreEqual(entities.Count, result);

        // Assert
        var countResult = connection.CountAll(tableName);
        Assert.AreEqual(0, countResult);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestBinaryBulkDeleteByKeyAsync()
    {
        using var connection = GetConnection();
        // Prepare
        var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
        var primaryKeys = entities.Select(entity => entity.Id);
        var tableName = "BulkOperationIdentityTable";

        // Act
        var result = await connection.BinaryBulkInsertAsync(
            tableName,
            entities: entities,
            identityBehavior: BulkImportIdentityBehavior.KeepIdentity, cancellationToken: TestContext.CancellationToken);

        // Act
        result = await connection.BinaryBulkDeleteByKeyAsync(
            tableName,
            primaryKeys: primaryKeys, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(entities.Count, result);

        // Assert
        var countResult = connection.CountAll(tableName);
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteByKeyAsyncWithBatchSize()
    {
        using var connection = GetConnection();
        // Prepare
        var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
        var primaryKeys = entities.Select(entity => entity.Id);
        var tableName = "BulkOperationIdentityTable";

        // Act
        var result = await connection.BinaryBulkInsertAsync(
            tableName,
            entities: entities,
            identityBehavior: BulkImportIdentityBehavior.KeepIdentity, cancellationToken: TestContext.CancellationToken);

        // Act
        result = await connection.BinaryBulkDeleteByKeyAsync(
            tableName,
            primaryKeys: primaryKeys,
            batchSize: 3, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(entities.Count, result);

        // Assert
        var countResult = connection.CountAll(tableName);
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteByKeyAsyncViaPhysicalTable()
    {
        using var connection = GetConnection();
        // Prepare
        var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
        var primaryKeys = entities.Select(entity => entity.Id);
        var tableName = "BulkOperationIdentityTable";

        // Act
        var result = connection.BinaryBulkInsert(
            tableName,
            entities: entities,
            identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

        // Act
        result = await connection.BinaryBulkDeleteByKeyAsync(
            tableName,
            primaryKeys: primaryKeys,
            pseudoTableType: BulkImportPseudoTableType.Physical, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(entities.Count, result);

        // Assert
        var countResult = connection.CountAll(tableName);
        Assert.AreEqual(0, countResult);
    }

    public TestContext TestContext { get; set; }

    #endregion
}
