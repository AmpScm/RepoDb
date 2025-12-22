using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using RepoDb.MySql.IntegrationTests.Models;
using RepoDb.MySql.IntegrationTests.Setup;

namespace RepoDb.MySql.IntegrationTests.Operations;

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
    public void TestMySqlConnectionCountAll()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new MySqlConnection(Database.ConnectionString);
        // Act
        var result = connection.CountAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void ThrowExceptionOnMySqlConnectionCountAllWithHints()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new MySqlConnection(Database.ConnectionString);
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.CountAll<CompleteTable>(hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestMySqlConnectionCountAllAsync()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new MySqlConnection(Database.ConnectionString);
        // Act
        var result = await connection.CountAllAsync<CompleteTable>(cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task ThrowExceptionOnMySqlConnectionCountAllAsyncWithHints()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new MySqlConnection(Database.ConnectionString);
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.CountAllAsync<CompleteTable>(hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestMySqlConnectionCountAllViaTableName()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new MySqlConnection(Database.ConnectionString);
        // Act
        var result = connection.CountAll(ClassMappedNameCache.Get<CompleteTable>());

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void ThrowExceptionOnMySqlConnectionCountAllViaTableNameWithHints()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new MySqlConnection(Database.ConnectionString);
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.CountAll(ClassMappedNameCache.Get<CompleteTable>(),
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestMySqlConnectionCountAllAsyncViaTableName()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new MySqlConnection(Database.ConnectionString);
        // Act
        var result = await connection.CountAllAsync(ClassMappedNameCache.Get<CompleteTable>(), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task ThrowExceptionOnMySqlConnectionCountAllAsyncViaTableNameWithHints()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new MySqlConnection(Database.ConnectionString);
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.CountAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    public TestContext TestContext { get; set; }

    #endregion

    #endregion
}
