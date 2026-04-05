using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;

namespace RepoDb.Oracle.IntegrationTests.Operations;

[TestClass]
public class MaxAllTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestOracleConnectionMaxAll()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = connection.MaxAll<CompleteTable>(e => e.ColumnInt);

        // Assert
        Assert.AreEqual(tables.Max(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void ThrowExceptionOnOracleConnectionMaxAllWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.MaxAll<CompleteTable>(e => e.ColumnInt,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestOracleConnectionMaxAllAsync()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = await connection.MaxAllAsync<CompleteTable>(e => e.ColumnInt, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Max(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task ThrowExceptionOnOracleConnectionMaxAllAsyncWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.MaxAllAsync<CompleteTable>(e => e.ColumnInt,
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestOracleConnectionMaxAllViaTableName()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = connection.MaxAll(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInt).First());

        // Assert
        Assert.AreEqual(tables.Max(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void ThrowExceptionOnOracleConnectionMaxAllViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.MaxAll(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestOracleConnectionMaxAllAsyncViaTableName()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = await connection.MaxAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInt).First(), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Max(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task ThrowExceptionOnOracleConnectionMaxAllAsyncViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.MaxAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion
}
