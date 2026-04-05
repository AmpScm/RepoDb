
using RepoDb.Extensions;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;

namespace RepoDb.Oracle.IntegrationTests.Operations;

[TestClass]
public class SkipQueryTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestOracleConnectionSkipQueryFirstBatchAscending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public void TestOracleConnectionSkipQueryFirstBatchDescending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public void TestOracleConnectionSkipQueryThirdBatchAscending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public void TestOracleConnectionSkipQueryThirdBatchDescending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public void ThrowExceptionOnOracleConnectionSkipQueryWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public async Task TestOracleConnectionSkipQueryAsyncFirstBatchAscending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public async Task TestOracleConnectionSkipQueryAsyncFirstBatchDescending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public async Task TestOracleConnectionSkipQueryAsyncThirdBatchAscending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public async Task TestOracleConnectionSkipQueryAsyncThirdBatchDescending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public async Task ThrowExceptionOnOracleConnectionSkipQueryAsyncWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public void TestOracleConnectionSkipQueryViaTableNameFirstBatchAscending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public void TestOracleConnectionSkipQueryViaTableNameFirstBatchDescending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public void TestOracleConnectionSkipQueryViaTableNameThirdBatchAscending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public void TestOracleConnectionSkipQueryViaTableNameThirdBatchDescending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public void ThrowExceptionOnOracleConnectionSkipQueryViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public async Task TestOracleConnectionSkipQueryViaTableNameAsyncFirstBatchAscending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public async Task TestOracleConnectionSkipQueryViaTableNameAsyncFirstBatchDescending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public async Task TestOracleConnectionSkipQueryViaTableNameAsyncThirdBatchAscending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public async Task TestOracleConnectionSkipQueryViaTableNameAsyncThirdBatchDescending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public async Task ThrowExceptionOnOracleConnectionSkipQueryAsyncViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.SkipQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
            0,
            3,
            OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null,
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion
}
