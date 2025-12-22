using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Models;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Setup;

namespace RepoDb.Sqlite.Microsoft.IntegrationTests.Operations.MDS;

[TestClass]
public class CountAllTest
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

    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionCountAll()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateMdsCompleteTables(10, connection);

        // Act
        var result = connection.CountAll<MdsCompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void ThrowExceptionOnSqLiteConnectionCountAllWithHints()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateMdsCompleteTables(10, connection);

        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.CountAll<MdsCompleteTable>(hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionCountAllAsync()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateMdsCompleteTables(10, connection);

        // Act
        var result = await connection.CountAllAsync<MdsCompleteTable>(cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task ThrowExceptionOnSqLiteConnectionCountAllAsyncWithHints()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateMdsCompleteTables(10, connection);

        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.CountAllAsync<MdsCompleteTable>(hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionCountAllViaTableName()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateMdsCompleteTables(10, connection);

        // Act
        var result = connection.CountAll(ClassMappedNameCache.Get<MdsCompleteTable>());

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void ThrowExceptionOnSqLiteConnectionCountAllViaTableNameWithHints()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateMdsCompleteTables(10, connection);

        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.CountAll(ClassMappedNameCache.Get<MdsCompleteTable>(),
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionCountAllAsyncViaTableName()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateMdsCompleteTables(10, connection);

        // Act
        var result = await connection.CountAllAsync(ClassMappedNameCache.Get<MdsCompleteTable>(), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task ThrowExceptionOnSqLiteConnectionCountAllAsyncViaTableNameWithHints()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateMdsCompleteTables(10, connection);

        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.CountAllAsync(ClassMappedNameCache.Get<MdsCompleteTable>(),
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    public TestContext TestContext { get; set; }

    #endregion

    #endregion
}
