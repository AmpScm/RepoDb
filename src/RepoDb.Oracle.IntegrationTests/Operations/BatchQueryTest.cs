using RepoDb.Extensions;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;

namespace RepoDb.Oracle.IntegrationTests.Operations;

[TestClass]
public class BatchQueryTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestOracleConnectionBatchQueryFirstBatchAscending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public void TestOracleConnectionBatchQueryFirstBatchDescending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public void TestOracleConnectionBatchQueryThirdBatchAscending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public void TestOracleConnectionBatchQueryThirdBatchDescending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public void ThrowExceptionOnOracleConnectionBatchQueryWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public async Task TestOracleConnectionBatchQueryAsyncFirstBatchAscending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        IEnumerable<CompleteTable> result = await connection.BatchQueryAsync<CompleteTable>(0,
            3,
            OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Helper.AssertPropertiesEquality(tables.ElementAt(0), result.ElementAt(0));
        Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(2));
    }

    [TestMethod]
    public async Task TestOracleConnectionBatchQueryAsyncFirstBatchDescending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        IEnumerable<CompleteTable> result = await connection.BatchQueryAsync<CompleteTable>(0,
            3,
            OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Helper.AssertPropertiesEquality(tables.ElementAt(9), result.ElementAt(0));
        Helper.AssertPropertiesEquality(tables.ElementAt(7), result.ElementAt(2));
    }

    [TestMethod]
    public async Task TestOracleConnectionBatchQueryAsyncThirdBatchAscending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        IEnumerable<CompleteTable> result = await connection.BatchQueryAsync<CompleteTable>(2,
            3,
            OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(0));
        Helper.AssertPropertiesEquality(tables.ElementAt(8), result.ElementAt(2));
    }

    [TestMethod]
    public async Task TestOracleConnectionBatchQueryAsyncThirdBatchDescending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        IEnumerable<CompleteTable> result = await connection.BatchQueryAsync<CompleteTable>(2,
            3,
            OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Helper.AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(0));
        Helper.AssertPropertiesEquality(tables.ElementAt(1), result.ElementAt(2));
    }

    [TestMethod]
    public async Task ThrowExceptionOnOracleConnectionBatchQueryAsyncWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.BatchQueryAsync<CompleteTable>(0,
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
    public void TestOracleConnectionBatchQueryViaTableNameFirstBatchAscending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public void TestOracleConnectionBatchQueryViaTableNameFirstBatchDescending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public void TestOracleConnectionBatchQueryViaTableNameThirdBatchAscending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public void TestOracleConnectionBatchQueryViaTableNameThirdBatchDescending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public void ThrowExceptionOnOracleConnectionBatchQueryViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public async Task TestOracleConnectionBatchQueryViaTableNameAsyncFirstBatchAscending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        IEnumerable<dynamic> result = await connection.BatchQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
            0,
            3,
            OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Helper.AssertMembersEquality(tables.ElementAt(0), result.ElementAt(0));
        Helper.AssertMembersEquality(tables.ElementAt(2), result.ElementAt(2));
    }

    [TestMethod]
    public async Task TestOracleConnectionBatchQueryViaTableNameAsyncFirstBatchDescending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        IEnumerable<dynamic> result = await connection.BatchQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
            0,
            3,
            OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Helper.AssertMembersEquality(tables.ElementAt(9), result.ElementAt(0));
        Helper.AssertMembersEquality(tables.ElementAt(7), result.ElementAt(2));
    }

    [TestMethod]
    public async Task TestOracleConnectionBatchQueryViaTableNameAsyncThirdBatchAscending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        IEnumerable<dynamic> result = await connection.BatchQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
            2,
            3,
            OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Helper.AssertMembersEquality(tables.ElementAt(6), result.ElementAt(0));
        Helper.AssertMembersEquality(tables.ElementAt(8), result.ElementAt(2));
    }

    [TestMethod]
    public async Task TestOracleConnectionBatchQueryViaTableNameAsyncThirdBatchDescending()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        IEnumerable<dynamic> result = await connection.BatchQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
            2,
            3,
            OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Helper.AssertMembersEquality(tables.ElementAt(3), result.ElementAt(0));
        Helper.AssertMembersEquality(tables.ElementAt(1), result.ElementAt(2));
    }

    [TestMethod]
    public async Task ThrowExceptionOnOracleConnectionBatchQueryAsyncViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.BatchQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
            0,
            3,
            OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
            (object?)null,
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion
}
