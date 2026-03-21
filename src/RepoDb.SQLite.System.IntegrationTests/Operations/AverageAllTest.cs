using System.Data.SQLite;
using RepoDb.SQLite.System.IntegrationTests.Models;
using RepoDb.SQLite.System.IntegrationTests.Setup;

namespace RepoDb.SQLite.System.IntegrationTests.Operations.SDS;

[TestClass]
public class AverageAllTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionAverageAll()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.AverageAll<CompleteTable>(e => e.ColumnInt);

        // Assert
        Assert.AreEqual(tables.Average(e => e.ColumnInt), result);
    }

    [TestMethod]
    public void ThrowExceptionOnSqLiteConnectionAverageAllWithHints()
    {
        Assert.ThrowsExactly<NotSupportedException>(() =>
        {
            using var connection = new SQLiteConnection(Database.ConnectionString);
            // Setup
            var tables = Database.CreateCompleteTables(10, connection);

            // Act
            connection.AverageAll<CompleteTable>(e => e.ColumnInt,
                hints: "WhatEver");
        });
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionAverageAllAsync()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.AverageAllAsync<CompleteTable>(e => e.ColumnInt, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Average(e => e.ColumnInt), result);
    }

    [TestMethod]
    public async Task ThrowExceptionOnSqLiteConnectionAverageAllAsyncWithHints()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.AverageAllAsync<CompleteTable>(e => e.ColumnInt,
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionAverageAllViaTableName()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.AverageAll(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInt).First());

        // Assert
        Assert.AreEqual(tables.Average(e => e.ColumnInt), result);
    }

    [TestMethod]
    public void ThrowExceptionOnSqLiteConnectionAverageAllViaTableNameWithHints()
    {
        Assert.ThrowsExactly<NotSupportedException>(() =>
        {
            using var connection = new SQLiteConnection(Database.ConnectionString);
            // Setup
            var tables = Database.CreateCompleteTables(10, connection);

            // Act
            connection.AverageAll(ClassMappedNameCache.Get<CompleteTable>(),
                Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
                hints: "WhatEver");
        });
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionAverageAllAsyncViaTableName()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.AverageAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInt).First(), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Average(e => e.ColumnInt), result);
    }

    [TestMethod]
    public async Task ThrowExceptionOnSqLiteConnectionAverageAllAsyncViaTableNameWithHints()
    {
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () =>
        {
            using var connection = new SQLiteConnection(Database.ConnectionString);
            // Setup
            var tables = Database.CreateCompleteTables(10, connection);

            // Act
            await connection.AverageAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
                hints: "WhatEver", cancellationToken: TestContext.CancellationToken);
        });
    }
    #endregion

    #endregion
}
