using RepoDb.SQLite.System.IntegrationTests.Models;
using RepoDb.SQLite.System.IntegrationTests.Setup;
using System.Data.SQLite;

namespace RepoDb.SQLite.System.IntegrationTests.Operations.SDS;

[TestClass]
public class MinAllTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionMinAll()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.MinAll<CompleteTable>(e => e.ColumnInt);

        // Assert
        Assert.AreEqual(tables.Min(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void ThrowExceptionOnSqLiteConnectionMinAllWithHints()
    {
        Assert.ThrowsExactly<NotSupportedException>(() =>
        {
            using var connection = new SQLiteConnection(Database.ConnectionString);
            // Setup
            var tables = Database.CreateCompleteTables(10, connection);

            // Act
            connection.MinAll<CompleteTable>(e => e.ColumnInt,
                hints: "WhatEver");
        });
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionMinAllAsync()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.MinAllAsync<CompleteTable>(e => e.ColumnInt, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Min(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task ThrowExceptionOnSqLiteConnectionMinAllAsyncWithHints()
    {
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () =>
        {
            using var connection = new SQLiteConnection(Database.ConnectionString);
            // Setup
            var tables = Database.CreateCompleteTables(10, connection);

            // Act
            await connection.MinAllAsync<CompleteTable>(e => e.ColumnInt,
                hints: "WhatEver", cancellationToken: TestContext.CancellationToken);
        });
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionMinAllViaTableName()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.MinAll(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInt).First());

        // Assert
        Assert.AreEqual(tables.Min(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void ThrowExceptionOnSqLiteConnectionMinAllViaTableNameWithHints()
    {
        Assert.ThrowsExactly<NotSupportedException>(() =>
        {
            using var connection = new SQLiteConnection(Database.ConnectionString);
            // Setup
            var tables = Database.CreateCompleteTables(10, connection);

            // Act
            connection.MinAll(ClassMappedNameCache.Get<CompleteTable>(),
                Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
                hints: "WhatEver");
        });
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionMinAllAsyncViaTableName()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.MinAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInt).First(), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Min(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task ThrowExceptionOnSqLiteConnectionMinAllAsyncViaTableNameWithHints()
    {
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () =>
        {
            using var connection = new SQLiteConnection(Database.ConnectionString);
            // Setup
            var tables = Database.CreateCompleteTables(10, connection);

            // Act
            await connection.MinAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
                hints: "WhatEver", cancellationToken: TestContext.CancellationToken);
        });
    }
    #endregion

    #endregion
}
