using Microsoft.Data.Sqlite;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Models;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Setup;

namespace RepoDb.Sqlite.Microsoft.IntegrationTests.Operations.MDS;

[TestClass]
public class SumAllTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionSumAll()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.SumAll<CompleteTable>(e => e.ColumnInt);

        // Assert
        Assert.AreEqual(tables.Sum(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void ThrowExceptionOnSqLiteConnectionSumAllWithHints()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.SumAll<CompleteTable>(e => e.ColumnInt,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionSumAllAsync()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.SumAllAsync<CompleteTable>(e => e.ColumnInt, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Sum(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task ThrowExceptionOnSqLiteConnectionSumAllAsyncWithHints()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.SumAllAsync<CompleteTable>(e => e.ColumnInt,
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionSumAllViaTableName()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.SumAll(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInt).First());

        // Assert
        Assert.AreEqual(tables.Sum(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void ThrowExceptionOnSqLiteConnectionSumAllViaTableNameWithHints()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.SumAll(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionSumAllAsyncViaTableName()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.SumAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInt).First(), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Sum(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task ThrowExceptionOnSqLiteConnectionSumAllAsyncViaTableNameWithHints()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.SumAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion
}
