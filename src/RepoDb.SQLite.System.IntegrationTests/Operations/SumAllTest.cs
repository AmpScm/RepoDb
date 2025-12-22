using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SQLite.System.IntegrationTests.Models;
using RepoDb.SQLite.System.IntegrationTests.Setup;
using System.Data.SQLite;

namespace RepoDb.SQLite.System.IntegrationTests.Operations.SDS;

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
    public void TestSqLiteConnectionSumAll()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateSdsCompleteTables(10, connection);

        // Act
        var result = connection.SumAll<SdsCompleteTable>(e => e.ColumnInt);

        // Assert
        Assert.AreEqual(tables.Sum(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void ThrowExceptionOnSqLiteConnectionSumAllWithHints()
    {
        Assert.ThrowsExactly<NotSupportedException>(() =>
        {
            using var connection = new SQLiteConnection(Database.ConnectionString);
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            connection.SumAll<SdsCompleteTable>(e => e.ColumnInt,
                hints: "WhatEver");
        });
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionSumAllAsync()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateSdsCompleteTables(10, connection);

        // Act
        var result = await connection.SumAllAsync<SdsCompleteTable>(e => e.ColumnInt, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Sum(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task ThrowExceptionOnSqLiteConnectionSumAllAsyncWithHints()
    {
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () =>
        {
            using var connection = new SQLiteConnection(Database.ConnectionString);
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            await connection.SumAllAsync<SdsCompleteTable>(e => e.ColumnInt,
                hints: "WhatEver", cancellationToken: TestContext.CancellationToken);
        });
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionSumAllViaTableName()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateSdsCompleteTables(10, connection);

        // Act
        var result = connection.SumAll(ClassMappedNameCache.Get<SdsCompleteTable>(),
            Field.Parse<SdsCompleteTable>(e => e.ColumnInt).First());

        // Assert
        Assert.AreEqual(tables.Sum(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void ThrowExceptionOnSqLiteConnectionSumAllViaTableNameWithHints()
    {
        Assert.ThrowsExactly<NotSupportedException>(() =>
        {
            using var connection = new SQLiteConnection(Database.ConnectionString);
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            connection.SumAll(ClassMappedNameCache.Get<SdsCompleteTable>(),
                Field.Parse<SdsCompleteTable>(e => e.ColumnInt).First(),
                hints: "WhatEver");
        });
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionSumAllAsyncViaTableName()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateSdsCompleteTables(10, connection);

        // Act
        var result = await connection.SumAllAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
            Field.Parse<SdsCompleteTable>(e => e.ColumnInt).First(), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Sum(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task ThrowExceptionOnSqLiteConnectionSumAllAsyncViaTableNameWithHints()
    {
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () =>
        {
            using var connection = new SQLiteConnection(Database.ConnectionString);
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            await connection.SumAllAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                Field.Parse<SdsCompleteTable>(e => e.ColumnInt).First(),
                hints: "WhatEver", cancellationToken: TestContext.CancellationToken);
        });
    }

    public TestContext TestContext { get; set; }

    #endregion

    #endregion
}
