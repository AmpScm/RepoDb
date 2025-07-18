using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.SQLite.System.IntegrationTests.Models;
using RepoDb.SQLite.System.IntegrationTests.Setup;
using System.Data.SQLite;

namespace RepoDb.SQLite.System.IntegrationTests.Operations.SDS;

[TestClass]
public class SkipQueryTest
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
    public void TestSqLiteConnectionSkipQueryFirstBatchAscending()
    {
        using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            var result = connection.SkipQuery<SdsCompleteTable>(
                0,
                3,
                OrderField.Ascending<SdsCompleteTable>(c => c.Id).AsEnumerable(),
                (object?)null);

            // Assert
            Helper.AssertPropertiesEquality(tables.ElementAt(0), result.ElementAt(0));
            Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(2));
        }
    }

    [TestMethod]
    public void TestSqLiteConnectionSkipQueryFirstBatchDescending()
    {
        using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            var result = connection.SkipQuery<SdsCompleteTable>(
                0,
                3,
                OrderField.Descending<SdsCompleteTable>(c => c.Id).AsEnumerable(),
                (object?)null);

            // Assert
            Helper.AssertPropertiesEquality(tables.ElementAt(9), result.ElementAt(0));
            Helper.AssertPropertiesEquality(tables.ElementAt(7), result.ElementAt(2));
        }
    }

    [TestMethod]
    public void TestSqLiteConnectionSkipQueryThirdBatchAscending()
    {
        using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            var result = connection.SkipQuery<SdsCompleteTable>(
                6,
                3,
                OrderField.Ascending<SdsCompleteTable>(c => c.Id).AsEnumerable(),
                (object?)null);

            // Assert
            Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(0));
            Helper.AssertPropertiesEquality(tables.ElementAt(8), result.ElementAt(2));
        }
    }

    [TestMethod]
    public void TestSqLiteConnectionSkipQueryThirdBatchDescending()
    {
        using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            var result = connection.SkipQuery<SdsCompleteTable>(
                6,
                3,
                OrderField.Descending<SdsCompleteTable>(c => c.Id).AsEnumerable(),
                (object?)null);

            // Assert
            Helper.AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(0));
            Helper.AssertPropertiesEquality(tables.ElementAt(1), result.ElementAt(2));
        }
    }

    [TestMethod]
    public void ThrowExceptionOnSqLiteConnectionSkipQueryWithHints()
    {
        Assert.ThrowsExactly<NotSupportedException>(() =>
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            connection.SkipQuery<SdsCompleteTable>(
                0,
                3,
                OrderField.Ascending<SdsCompleteTable>(c => c.Id).AsEnumerable(),
                (object?)null,
                hints: "WhatEver");
        }
        });
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionSkipQueryAsyncFirstBatchAscending()
    {
        using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            var result = await connection.SkipQueryAsync<SdsCompleteTable>(
                0,
                3,
                OrderField.Ascending<SdsCompleteTable>(c => c.Id).AsEnumerable(),
                (object?)null);

            // Assert
            Helper.AssertPropertiesEquality(tables.ElementAt(0), result.ElementAt(0));
            Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(2));
        }
    }

    [TestMethod]
    public async Task TestSqLiteConnectionSkipQueryAsyncFirstBatchDescending()
    {
        using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            var result = await connection.SkipQueryAsync<SdsCompleteTable>(
                0,
                3,
                OrderField.Descending<SdsCompleteTable>(c => c.Id).AsEnumerable(),
                (object?)null);

            // Assert
            Helper.AssertPropertiesEquality(tables.ElementAt(9), result.ElementAt(0));
            Helper.AssertPropertiesEquality(tables.ElementAt(7), result.ElementAt(2));
        }
    }

    [TestMethod]
    public async Task TestSqLiteConnectionSkipQueryAsyncThirdBatchAscending()
    {
        using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            var result = await connection.SkipQueryAsync<SdsCompleteTable>(
                6,
                3,
                OrderField.Ascending<SdsCompleteTable>(c => c.Id).AsEnumerable(),
                (object?)null);

            // Assert
            Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(0));
            Helper.AssertPropertiesEquality(tables.ElementAt(8), result.ElementAt(2));
        }
    }

    [TestMethod]
    public async Task TestSqLiteConnectionSkipQueryAsyncThirdBatchDescending()
    {
        using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            var result = await connection.SkipQueryAsync<SdsCompleteTable>(
                6,
                3,
                OrderField.Descending<SdsCompleteTable>(c => c.Id).AsEnumerable(),
                (object?)null);

            // Assert
            Helper.AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(0));
            Helper.AssertPropertiesEquality(tables.ElementAt(1), result.ElementAt(2));
        }
    }

    [TestMethod]
    public async Task ThrowExceptionOnSqLiteConnectionSkipQueryAsyncWithHints()
    {
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () =>
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            await connection.SkipQueryAsync<SdsCompleteTable>(
                0,
                3,
                OrderField.Ascending<SdsCompleteTable>(c => c.Id).AsEnumerable(),
                (object?)null,
                hints: "WhatEver");
        }
        });
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionSkipQueryViaTableNameFirstBatchAscending()
    {
        using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            var result = connection.SkipQuery(ClassMappedNameCache.Get<SdsCompleteTable>(),
                0,
                3,
                OrderField.Ascending<SdsCompleteTable>(c => c.Id).AsEnumerable(),
                (object?)null);

            // Assert
            Helper.AssertMembersEquality(tables.ElementAt(0), result.ElementAt(0));
            Helper.AssertMembersEquality(tables.ElementAt(2), result.ElementAt(2));
        }
    }

    [TestMethod]
    public void TestSqLiteConnectionSkipQueryViaTableNameFirstBatchDescending()
    {
        using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            var result = connection.SkipQuery(ClassMappedNameCache.Get<SdsCompleteTable>(),
                0,
                3,
                OrderField.Descending<SdsCompleteTable>(c => c.Id).AsEnumerable(),
                (object?)null);

            // Assert
            Helper.AssertMembersEquality(tables.ElementAt(9), result.ElementAt(0));
            Helper.AssertMembersEquality(tables.ElementAt(7), result.ElementAt(2));
        }
    }

    [TestMethod]
    public void TestSqLiteConnectionSkipQueryViaTableNameThirdBatchAscending()
    {
        using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            var result = connection.SkipQuery(ClassMappedNameCache.Get<SdsCompleteTable>(),
                6,
                3,
                OrderField.Ascending<SdsCompleteTable>(c => c.Id).AsEnumerable(),
                (object?)null);

            // Assert
            Helper.AssertMembersEquality(tables.ElementAt(6), result.ElementAt(0));
            Helper.AssertMembersEquality(tables.ElementAt(8), result.ElementAt(2));
        }
    }

    [TestMethod]
    public void TestSqLiteConnectionSkipQueryViaTableNameThirdBatchDescending()
    {
        using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            var result = connection.SkipQuery(ClassMappedNameCache.Get<SdsCompleteTable>(),
                6,
                3,
                OrderField.Descending<SdsCompleteTable>(c => c.Id).AsEnumerable(),
                (object?)null);

            // Assert
            Helper.AssertMembersEquality(tables.ElementAt(3), result.ElementAt(0));
            Helper.AssertMembersEquality(tables.ElementAt(1), result.ElementAt(2));
        }
    }

    [TestMethod]
    public void ThrowExceptionOnSqLiteConnectionSkipQueryViaTableNameWithHints()
    {
        Assert.ThrowsExactly<NotSupportedException>(() =>
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            connection.SkipQuery(ClassMappedNameCache.Get<SdsCompleteTable>(),
                0,
                3,
                OrderField.Ascending<SdsCompleteTable>(c => c.Id).AsEnumerable(),
                (object?)null,
                hints: "WhatEver");
        }
        });
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionSkipQueryViaTableNameAsyncFirstBatchAscending()
    {
        using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            var result = await connection.SkipQueryAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                0,
                3,
                OrderField.Ascending<SdsCompleteTable>(c => c.Id).AsEnumerable(),
                (object?)null);

            // Assert
            Helper.AssertMembersEquality(tables.ElementAt(0), result.ElementAt(0));
            Helper.AssertMembersEquality(tables.ElementAt(2), result.ElementAt(2));
        }
    }

    [TestMethod]
    public async Task TestSqLiteConnectionSkipQueryViaTableNameAsyncFirstBatchDescending()
    {
        using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            var result = await connection.SkipQueryAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                0,
                3,
                OrderField.Descending<SdsCompleteTable>(c => c.Id).AsEnumerable(),
                (object?)null);

            // Assert
            Helper.AssertMembersEquality(tables.ElementAt(9), result.ElementAt(0));
            Helper.AssertMembersEquality(tables.ElementAt(7), result.ElementAt(2));
        }
    }

    [TestMethod]
    public async Task TestSqLiteConnectionSkipQueryViaTableNameAsyncThirdBatchAscending()
    {
        using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            var result = await connection.SkipQueryAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                6,
                3,
                OrderField.Ascending<SdsCompleteTable>(c => c.Id).AsEnumerable(),
                (object?)null);

            // Assert
            Helper.AssertMembersEquality(tables.ElementAt(6), result.ElementAt(0));
            Helper.AssertMembersEquality(tables.ElementAt(8), result.ElementAt(2));
        }
    }

    [TestMethod]
    public async Task TestSqLiteConnectionSkipQueryViaTableNameAsyncThirdBatchDescending()
    {
        using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            var result = await connection.SkipQueryAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                6,
                3,
                OrderField.Descending<SdsCompleteTable>(c => c.Id).AsEnumerable(),
                (object?)null);

            // Assert
            Helper.AssertMembersEquality(tables.ElementAt(3), result.ElementAt(0));
            Helper.AssertMembersEquality(tables.ElementAt(1), result.ElementAt(2));
        }
    }

    [TestMethod]
    public async Task ThrowExceptionOnSqLiteConnectionSkipQueryAsyncViaTableNameWithHints()
    {
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () =>
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
        {
            // Setup
            var tables = Database.CreateSdsCompleteTables(10, connection);

            // Act
            await connection.SkipQueryAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                0,
                3,
                OrderField.Ascending<SdsCompleteTable>(c => c.Id).AsEnumerable(),
                (object?)null,
                hints: "WhatEver");
        }
        });
    }

    #endregion

    #endregion
}
