using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Extensions;
using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.PostgreSql.IntegrationTests.Operations;

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
    public void TestPostgreSqlConnectionSkipQueryFirstBatchAscending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<CompleteTable> result = connection.SkipQuery<CompleteTable>(
            0,
            3,
            OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null);

        // Assert
        Helper.AssertPropertiesEquality(tables.ElementAt(0), result.ElementAt(0));
        Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(2));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionSkipQueryFirstBatchDescending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<CompleteTable> result = connection.SkipQuery<CompleteTable>(
            0,
            3,
            OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null);

        // Assert
        Helper.AssertPropertiesEquality(tables.ElementAt(9), result.ElementAt(0));
        Helper.AssertPropertiesEquality(tables.ElementAt(7), result.ElementAt(2));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionSkipQueryThirdBatchAscending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<CompleteTable> result = connection.SkipQuery<CompleteTable>(
            6,
            3,
            OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null);

        // Assert
        Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(0));
        Helper.AssertPropertiesEquality(tables.ElementAt(8), result.ElementAt(2));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionSkipQueryThirdBatchDescending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<CompleteTable> result = connection.SkipQuery<CompleteTable>(
            6,
            3,
            OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null);

        // Assert
        Helper.AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(0));
        Helper.AssertPropertiesEquality(tables.ElementAt(1), result.ElementAt(2));
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlConnectionSkipQueryWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.SkipQuery<CompleteTable>(
            0,
            3,
            OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionSkipQueryAsyncFirstBatchAscending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<CompleteTable> result = await connection.SkipQueryAsync<CompleteTable>(
            0,
            3,
            OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Helper.AssertPropertiesEquality(tables.ElementAt(0), result.ElementAt(0));
        Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(2));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionSkipQueryAsyncFirstBatchDescending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<CompleteTable> result = await connection.SkipQueryAsync<CompleteTable>(
            0,
            3,
            OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Helper.AssertPropertiesEquality(tables.ElementAt(9), result.ElementAt(0));
        Helper.AssertPropertiesEquality(tables.ElementAt(7), result.ElementAt(2));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionSkipQueryAsyncThirdBatchAscending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<CompleteTable> result = await connection.SkipQueryAsync<CompleteTable>(
            6,
            3,
            OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(0));
        Helper.AssertPropertiesEquality(tables.ElementAt(8), result.ElementAt(2));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionSkipQueryAsyncThirdBatchDescending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<CompleteTable> result = await connection.SkipQueryAsync<CompleteTable>(
            6,
            3,
            OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Helper.AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(0));
        Helper.AssertPropertiesEquality(tables.ElementAt(1), result.ElementAt(2));
    }

    [TestMethod]
    public async Task ThrowExceptionOnPostgreSqlConnectionSkipQueryAsyncWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.SkipQueryAsync<CompleteTable>(
            0,
            3,
            OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null,
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestPostgreSqlConnectionSkipQueryViaTableNameFirstBatchAscending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<dynamic> result = connection.SkipQuery(ClassMappedNameCache.Get<CompleteTable>(),
            0,
            3,
            OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null);

        // Assert
        Helper.AssertMembersEquality(tables.ElementAt(0), result.ElementAt(0));
        Helper.AssertMembersEquality(tables.ElementAt(2), result.ElementAt(2));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionSkipQueryViaTableNameFirstBatchDescending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<dynamic> result = connection.SkipQuery(ClassMappedNameCache.Get<CompleteTable>(),
            0,
            3,
            OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null);

        // Assert
        Helper.AssertMembersEquality(tables.ElementAt(9), result.ElementAt(0));
        Helper.AssertMembersEquality(tables.ElementAt(7), result.ElementAt(2));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionSkipQueryViaTableNameThirdBatchAscending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<dynamic> result = connection.SkipQuery(ClassMappedNameCache.Get<CompleteTable>(),
            6,
            3,
            OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null);

        // Assert
        Helper.AssertMembersEquality(tables.ElementAt(6), result.ElementAt(0));
        Helper.AssertMembersEquality(tables.ElementAt(8), result.ElementAt(2));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionSkipQueryViaTableNameThirdBatchDescending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<dynamic> result = connection.SkipQuery(ClassMappedNameCache.Get<CompleteTable>(),
            6,
            3,
            OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null);

        // Assert
        Helper.AssertMembersEquality(tables.ElementAt(3), result.ElementAt(0));
        Helper.AssertMembersEquality(tables.ElementAt(1), result.ElementAt(2));
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlConnectionSkipQueryViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.SkipQuery(ClassMappedNameCache.Get<CompleteTable>(),
            5,
            3,
            OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionSkipQueryViaTableNameAsyncFirstBatchAscending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<dynamic> result = await connection.SkipQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
            0,
            3,
            OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Helper.AssertMembersEquality(tables.ElementAt(0), result.ElementAt(0));
        Helper.AssertMembersEquality(tables.ElementAt(2), result.ElementAt(2));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionSkipQueryViaTableNameAsyncFirstBatchDescending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<dynamic> result = await connection.SkipQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
            0,
            3,
            OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Helper.AssertMembersEquality(tables.ElementAt(9), result.ElementAt(0));
        Helper.AssertMembersEquality(tables.ElementAt(7), result.ElementAt(2));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionSkipQueryViaTableNameAsyncThirdBatchAscending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<dynamic> result = await connection.SkipQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
            6,
            3,
            OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Helper.AssertMembersEquality(tables.ElementAt(6), result.ElementAt(0));
        Helper.AssertMembersEquality(tables.ElementAt(8), result.ElementAt(2));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionSkipQueryViaTableNameAsyncThirdBatchDescending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<dynamic> result = await connection.SkipQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
            6,
            3,
            OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Helper.AssertMembersEquality(tables.ElementAt(3), result.ElementAt(0));
        Helper.AssertMembersEquality(tables.ElementAt(1), result.ElementAt(2));
    }

    [TestMethod]
    public async Task ThrowExceptionOnPostgreSqlConnectionSkipQueryAsyncViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.SkipQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
            0,
            3,
            OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null,
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    public TestContext TestContext { get; set; }

    #endregion

    #endregion
}
