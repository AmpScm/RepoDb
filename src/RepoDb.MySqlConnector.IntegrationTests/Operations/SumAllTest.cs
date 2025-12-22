using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySqlConnector;
using RepoDb.MySqlConnector.IntegrationTests.Models;
using RepoDb.MySqlConnector.IntegrationTests.Setup;

namespace RepoDb.MySqlConnector.IntegrationTests.Operations;

[TestClass]
public class SumAllTest
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
    public void TestMySqlConnectionSumAll()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new MySqlConnection(Database.ConnectionString);
        // Act
        var result = connection.SumAll<CompleteTable>(e => e.ColumnInt);

        // Assert
        Assert.AreEqual(tables.Sum(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void ThrowExceptionOnMySqlConnectionSumAllWithHints()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        Assert.ThrowsExactly<NotSupportedException>(() =>
        {
            using var connection = new MySqlConnection(Database.ConnectionString);
            // Act
            connection.SumAll<CompleteTable>(e => e.ColumnInt,
                hints: "WhatEver");
        });
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestMySqlConnectionSumAllAsync()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new MySqlConnection(Database.ConnectionString);
        // Act
        var result = await connection.SumAllAsync<CompleteTable>(e => e.ColumnInt, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Sum(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task ThrowExceptionOnMySqlConnectionSumAllAsyncWithHints()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () =>
        {
            using var connection = new MySqlConnection(Database.ConnectionString);
            // Act
            await connection.SumAllAsync<CompleteTable>(e => e.ColumnInt,
                hints: "WhatEver", cancellationToken: TestContext.CancellationToken);
        });
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestMySqlConnectionSumAllViaTableName()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new MySqlConnection(Database.ConnectionString);
        // Act
        var result = connection.SumAll(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInt).First());

        // Assert
        Assert.AreEqual(tables.Sum(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void ThrowExceptionOnMySqlConnectionSumAllViaTableNameWithHints()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        Assert.ThrowsExactly<NotSupportedException>(() =>
        {
            using var connection = new MySqlConnection(Database.ConnectionString);
            // Act
            connection.SumAll(ClassMappedNameCache.Get<CompleteTable>(),
                Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
                hints: "WhatEver");
        });
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestMySqlConnectionSumAllAsyncViaTableName()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new MySqlConnection(Database.ConnectionString);
        // Act
        var result = await connection.SumAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInt).First(), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Sum(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task ThrowExceptionOnMySqlConnectionSumAllAsyncViaTableNameWithHints()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () =>
        {
            using var connection = new MySqlConnection(Database.ConnectionString);
            // Act
            await connection.SumAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
                hints: "WhatEver", cancellationToken: TestContext.CancellationToken);
        });
    }

    public TestContext TestContext { get; set; }

    #endregion

    #endregion
}
