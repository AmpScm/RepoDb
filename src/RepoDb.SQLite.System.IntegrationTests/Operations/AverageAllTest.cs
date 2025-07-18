using System.Data.SQLite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SQLite.System.IntegrationTests.Models;
using RepoDb.SQLite.System.IntegrationTests.Setup;

namespace RepoDb.SQLite.System.IntegrationTests.Operations.SDS;

[TestClass]
public class AverageAllTest
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
    public void TestSqLiteConnectionAverageAll()
    {
        using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            var result = connection.AverageAll<SdsCompleteTable>(e => e.ColumnInt);

            // Assert
            Assert.AreEqual(tables.Average(e => e.ColumnInt), result);
        }
    }

    [TestMethod]
    public void ThrowExceptionOnSqLiteConnectionAverageAllWithHints()
    {
        Assert.ThrowsExactly<NotSupportedException>(() =>
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                connection.AverageAll<SdsCompleteTable>(e => e.ColumnInt,
                    hints: "WhatEver");
            }
        });
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionAverageAllAsync()
    {
        using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            var result = await connection.AverageAllAsync<SdsCompleteTable>(e => e.ColumnInt);

            // Assert
            Assert.AreEqual(tables.Average(e => e.ColumnInt), result);
        }
    }

    [TestMethod]
    public async Task ThrowExceptionOnSqLiteConnectionAverageAllAsyncWithHints()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateSdsCompleteTables(10, connection);

        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.AverageAllAsync<SdsCompleteTable>(e => e.ColumnInt,
            hints: "WhatEver"));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionAverageAllViaTableName()
    {
        using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            var result = connection.AverageAll(ClassMappedNameCache.Get<SdsCompleteTable>(),
                Field.Parse<SdsCompleteTable>(e => e.ColumnInt).First());

            // Assert
            Assert.AreEqual(tables.Average(e => e.ColumnInt), result);
        }
    }

    [TestMethod]
    public void ThrowExceptionOnSqLiteConnectionAverageAllViaTableNameWithHints()
    {
        Assert.ThrowsExactly<NotSupportedException>(() =>
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                connection.AverageAll(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    Field.Parse<SdsCompleteTable>(e => e.ColumnInt).First(),
                    hints: "WhatEver");
            }
        });
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionAverageAllAsyncViaTableName()
    {
        using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            var result = await connection.AverageAllAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                Field.Parse<SdsCompleteTable>(e => e.ColumnInt).First());

            // Assert
            Assert.AreEqual(tables.Average(e => e.ColumnInt), result);
        }
    }

    [TestMethod]
    public async Task ThrowExceptionOnSqLiteConnectionAverageAllAsyncViaTableNameWithHints()
    {
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () =>
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                await connection.AverageAllAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    Field.Parse<SdsCompleteTable>(e => e.ColumnInt).First(),
                    hints: "WhatEver");
            }
        });
    }

    #endregion

    #endregion
}
