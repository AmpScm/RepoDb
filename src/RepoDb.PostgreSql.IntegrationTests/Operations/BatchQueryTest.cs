using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Extensions;
using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.PostgreSql.IntegrationTests.Operations;

[TestClass]
public class BatchQueryTest
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
    public void TestPostgreSqlConnectionBatchQueryFirstBatchAscending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<CompleteTable> result = connection.BatchQuery<CompleteTable>(0,
            3,
            OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null);

        // Assert
        Helper.AssertPropertiesEquality(tables.ElementAt(0), result.ElementAt(0));
        Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(2));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionBatchQueryFirstBatchDescending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<CompleteTable> result = connection.BatchQuery<CompleteTable>(0,
            3,
            OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null);

        // Assert
        Helper.AssertPropertiesEquality(tables.ElementAt(9), result.ElementAt(0));
        Helper.AssertPropertiesEquality(tables.ElementAt(7), result.ElementAt(2));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionBatchQueryThirdBatchAscending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<CompleteTable> result = connection.BatchQuery<CompleteTable>(2,
            3,
            OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null);

        // Assert
        Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(0));
        Helper.AssertPropertiesEquality(tables.ElementAt(8), result.ElementAt(2));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionBatchQueryThirdBatchDescending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<CompleteTable> result = connection.BatchQuery<CompleteTable>(2,
            3,
            OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null);

        // Assert
        Helper.AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(0));
        Helper.AssertPropertiesEquality(tables.ElementAt(1), result.ElementAt(2));
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlConnectionBatchQueryWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.BatchQuery<CompleteTable>(0,
            3,
            OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionBatchQueryAsyncFirstBatchAscending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<CompleteTable> result = await connection.BatchQueryAsync<CompleteTable>(0,
            3,
            OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null);

        // Assert
        Helper.AssertPropertiesEquality(tables.ElementAt(0), result.ElementAt(0));
        Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(2));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionBatchQueryAsyncFirstBatchDescending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<CompleteTable> result = await connection.BatchQueryAsync<CompleteTable>(0,
            3,
            OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null);

        // Assert
        Helper.AssertPropertiesEquality(tables.ElementAt(9), result.ElementAt(0));
        Helper.AssertPropertiesEquality(tables.ElementAt(7), result.ElementAt(2));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionBatchQueryAsyncThirdBatchAscending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<CompleteTable> result = await connection.BatchQueryAsync<CompleteTable>(2,
            3,
            OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null);

        // Assert
        Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(0));
        Helper.AssertPropertiesEquality(tables.ElementAt(8), result.ElementAt(2));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionBatchQueryAsyncThirdBatchDescending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<CompleteTable> result = await connection.BatchQueryAsync<CompleteTable>(2,
            3,
            OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null);

        // Assert
        Helper.AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(0));
        Helper.AssertPropertiesEquality(tables.ElementAt(1), result.ElementAt(2));
    }

    [TestMethod]
    public async Task ThrowExceptionOnPostgreSqlConnectionBatchQueryAsyncWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.BatchQueryAsync<CompleteTable>(0,
            3,
            OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestPostgreSqlConnectionBatchQueryViaTableNameFirstBatchAscending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<dynamic> result = connection.BatchQuery(ClassMappedNameCache.Get<CompleteTable>(),
            0,
            3,
            OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null);

        // Assert
        Helper.AssertMembersEquality(tables.ElementAt(0), result.ElementAt(0));
        Helper.AssertMembersEquality(tables.ElementAt(2), result.ElementAt(2));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionBatchQueryViaTableNameFirstBatchDescending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<dynamic> result = connection.BatchQuery(ClassMappedNameCache.Get<CompleteTable>(),
            0,
            3,
            OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null);

        // Assert
        Helper.AssertMembersEquality(tables.ElementAt(9), result.ElementAt(0));
        Helper.AssertMembersEquality(tables.ElementAt(7), result.ElementAt(2));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionBatchQueryViaTableNameThirdBatchAscending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<dynamic> result = connection.BatchQuery(ClassMappedNameCache.Get<CompleteTable>(),
            2,
            3,
            OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null);

        // Assert
        Helper.AssertMembersEquality(tables.ElementAt(6), result.ElementAt(0));
        Helper.AssertMembersEquality(tables.ElementAt(8), result.ElementAt(2));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionBatchQueryViaTableNameThirdBatchDescending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<dynamic> result = connection.BatchQuery(ClassMappedNameCache.Get<CompleteTable>(),
            2,
            3,
            OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null);

        // Assert
        Helper.AssertMembersEquality(tables.ElementAt(3), result.ElementAt(0));
        Helper.AssertMembersEquality(tables.ElementAt(1), result.ElementAt(2));
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlConnectionBatchQueryViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.BatchQuery(ClassMappedNameCache.Get<CompleteTable>(),
            0,
            3,
            OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionBatchQueryViaTableNameAsyncFirstBatchAscending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<dynamic> result = await connection.BatchQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
            0,
            3,
            OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null);

        // Assert
        Helper.AssertMembersEquality(tables.ElementAt(0), result.ElementAt(0));
        Helper.AssertMembersEquality(tables.ElementAt(2), result.ElementAt(2));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionBatchQueryViaTableNameAsyncFirstBatchDescending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<dynamic> result = await connection.BatchQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
            0,
            3,
            OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null);

        // Assert
        Helper.AssertMembersEquality(tables.ElementAt(9), result.ElementAt(0));
        Helper.AssertMembersEquality(tables.ElementAt(7), result.ElementAt(2));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionBatchQueryViaTableNameAsyncThirdBatchAscending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<dynamic> result = await connection.BatchQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
            2,
            3,
            OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null);

        // Assert
        Helper.AssertMembersEquality(tables.ElementAt(6), result.ElementAt(0));
        Helper.AssertMembersEquality(tables.ElementAt(8), result.ElementAt(2));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionBatchQueryViaTableNameAsyncThirdBatchDescending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<dynamic> result = await connection.BatchQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
            2,
            3,
            OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null);

        // Assert
        Helper.AssertMembersEquality(tables.ElementAt(3), result.ElementAt(0));
        Helper.AssertMembersEquality(tables.ElementAt(1), result.ElementAt(2));
    }

    [TestMethod]
    public async Task ThrowExceptionOnPostgreSqlConnectionBatchQueryAsyncViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.BatchQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
            0,
            3,
            OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #endregion
}
