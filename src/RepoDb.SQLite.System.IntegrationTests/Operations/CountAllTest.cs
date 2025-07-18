using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SQLite.System.IntegrationTests.Models;
using RepoDb.SQLite.System.IntegrationTests.Setup;
using System.Data.SQLite;

namespace RepoDb.SQLite.System.IntegrationTests.Operations.SDS;

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
        using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            var result = connection.CountAll<SdsCompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count(), result);
        }
    }

    [TestMethod]
    public void ThrowExceptionOnSqLiteConnectionCountAllWithHints()
    {
        Assert.ThrowsExactly<NotSupportedException>(() =>
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            connection.CountAll<SdsCompleteTable>(hints: "WhatEver");
        }
        });
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionCountAllAsync()
    {
        using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            var result = await connection.CountAllAsync<SdsCompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count(), result);
        }
    }

    [TestMethod]
    public async Task ThrowExceptionOnSqLiteConnectionCountAllAsyncWithHints()
    {
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () =>
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            await connection.CountAllAsync<SdsCompleteTable>(hints: "WhatEver");
        }
        });
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionCountAllViaTableName()
    {
        using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            var result = connection.CountAll(ClassMappedNameCache.Get<SdsCompleteTable>());

            // Assert
            Assert.AreEqual(tables.Count(), result);
        }
    }

    [TestMethod]
    public void ThrowExceptionOnSqLiteConnectionCountAllViaTableNameWithHints()
    {
        Assert.ThrowsExactly<NotSupportedException>(() =>
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            connection.CountAll(ClassMappedNameCache.Get<SdsCompleteTable>(),
                hints: "WhatEver");
        }
        });
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionCountAllAsyncViaTableName()
    {
        using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            var result = await connection.CountAllAsync(ClassMappedNameCache.Get<SdsCompleteTable>());

            // Assert
            Assert.AreEqual(tables.Count(), result);
        }
    }

    [TestMethod]
    public async Task ThrowExceptionOnSqLiteConnectionCountAllAsyncViaTableNameWithHints()
    {
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () =>
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            await connection.CountAllAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                hints: "WhatEver");
        }
        });
    }

    #endregion

    #endregion
}
