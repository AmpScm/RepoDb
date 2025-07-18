using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SQLite.System.IntegrationTests.Models;
using RepoDb.SQLite.System.IntegrationTests.Setup;
using System.Data.SQLite;

namespace RepoDb.SQLite.System.IntegrationTests.Operations.SDS;

[TestClass]
public class MaxAllTest
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
    public void TestSqLiteConnectionMaxAll()
    {
        using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            var result = connection.MaxAll<SdsCompleteTable>(e => e.ColumnInt);

            // Assert
            Assert.AreEqual(tables.Max(e => e.ColumnInt), Convert.ToInt32(result));
        }
    }

    [TestMethod]
    public void ThrowExceptionOnSqLiteConnectionMaxAllWithHints()
    {
        Assert.ThrowsExactly<NotSupportedException>(() =>
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            connection.MaxAll<SdsCompleteTable>(e => e.ColumnInt,
                hints: "WhatEver");
        }
        });
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionMaxAllAsync()
    {
        using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            var result = await connection.MaxAllAsync<SdsCompleteTable>(e => e.ColumnInt);

            // Assert
            Assert.AreEqual(tables.Max(e => e.ColumnInt), Convert.ToInt32(result));
        }
    }

    [TestMethod]
    public async Task ThrowExceptionOnSqLiteConnectionMaxAllAsyncWithHints()
    {
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () =>
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            await connection.MaxAllAsync<SdsCompleteTable>(e => e.ColumnInt,
                hints: "WhatEver");
        }
        });
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionMaxAllViaTableName()
    {
        using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            var result = connection.MaxAll(ClassMappedNameCache.Get<SdsCompleteTable>(),
                Field.Parse<SdsCompleteTable>(e => e.ColumnInt).First());

            // Assert
            Assert.AreEqual(tables.Max(e => e.ColumnInt), Convert.ToInt32(result));
        }
    }

    [TestMethod]
    public void ThrowExceptionOnSqLiteConnectionMaxAllViaTableNameWithHints()
    {
        Assert.ThrowsExactly<NotSupportedException>(() =>
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            connection.MaxAll(ClassMappedNameCache.Get<SdsCompleteTable>(),
                Field.Parse<SdsCompleteTable>(e => e.ColumnInt).First(),
                hints: "WhatEver");
        }
        });
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionMaxAllAsyncViaTableName()
    {
        using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            var result = await connection.MaxAllAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                Field.Parse<SdsCompleteTable>(e => e.ColumnInt).First());

            // Assert
            Assert.AreEqual(tables.Max(e => e.ColumnInt), Convert.ToInt32(result));
        }
    }

    [TestMethod]
    public async Task ThrowExceptionOnSqLiteConnectionMaxAllAsyncViaTableNameWithHints()
    {
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () =>
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            await connection.MaxAllAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                Field.Parse<SdsCompleteTable>(e => e.ColumnInt).First(),
                hints: "WhatEver");
        }
        });
    }

    #endregion

    #endregion
}
