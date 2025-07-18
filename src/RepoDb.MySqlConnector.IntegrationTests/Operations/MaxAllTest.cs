using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySqlConnector;
using RepoDb.MySqlConnector.IntegrationTests.Models;
using RepoDb.MySqlConnector.IntegrationTests.Setup;

namespace RepoDb.MySqlConnector.IntegrationTests.Operations;

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
    public void TestMySqlConnectionMaxAll()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using (var connection = new MySqlConnection(Database.ConnectionString))
        {
            // Act
            var result = connection.MaxAll<CompleteTable>(e => e.ColumnInt);

            // Assert
            Assert.AreEqual(tables.Max(e => e.ColumnInt), Convert.ToInt32(result));
        }
    }

    [TestMethod]
    public void ThrowExceptionOnMySqlConnectionMaxAllWithHints()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        Assert.ThrowsExactly<NotSupportedException>(() =>
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
        {
            // Act
            connection.MaxAll<CompleteTable>(e => e.ColumnInt,
                hints: "WhatEver");
        }
        });
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestMySqlConnectionMaxAllAsync()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using (var connection = new MySqlConnection(Database.ConnectionString))
        {
            // Act
            var result = await connection.MaxAllAsync<CompleteTable>(e => e.ColumnInt);

            // Assert
            Assert.AreEqual(tables.Max(e => e.ColumnInt), Convert.ToInt32(result));
        }
    }

    [TestMethod]
    public async Task ThrowExceptionOnMySqlConnectionMaxAllAsyncWithHints()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () =>
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
        {
            // Act
            await connection.MaxAllAsync<CompleteTable>(e => e.ColumnInt,
                hints: "WhatEver");
        }
        });
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestMySqlConnectionMaxAllViaTableName()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using (var connection = new MySqlConnection(Database.ConnectionString))
        {
            // Act
            var result = connection.MaxAll(ClassMappedNameCache.Get<CompleteTable>(),
                Field.Parse<CompleteTable>(e => e.ColumnInt).First());

            // Assert
            Assert.AreEqual(tables.Max(e => e.ColumnInt), Convert.ToInt32(result));
        }
    }

    [TestMethod]
    public void ThrowExceptionOnMySqlConnectionMaxAllViaTableNameWithHints()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        Assert.ThrowsExactly<NotSupportedException>(() =>
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
        {
            // Act
            connection.MaxAll(ClassMappedNameCache.Get<CompleteTable>(),
                Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
                hints: "WhatEver");
        }
        });
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestMySqlConnectionMaxAllAsyncViaTableName()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using (var connection = new MySqlConnection(Database.ConnectionString))
        {
            // Act
            var result = await connection.MaxAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                Field.Parse<CompleteTable>(e => e.ColumnInt).First());

            // Assert
            Assert.AreEqual(tables.Max(e => e.ColumnInt), Convert.ToInt32(result));
        }
    }

    [TestMethod]
    public async Task ThrowExceptionOnMySqlConnectionMaxAllAsyncViaTableNameWithHints()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () =>
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
        {
            // Act
            await connection.MaxAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
                hints: "WhatEver");
        }
        });
    }

    #endregion

    #endregion
}
