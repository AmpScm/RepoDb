using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;

namespace RepoDb.Oracle.IntegrationTests.Operations;

[TestClass]
public class AverageAllTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestOracleConnectionAverageAll()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = connection.AverageAll<CompleteTable>(e => e.ColumnInt);

        // Assert
        Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
    }

    [TestMethod]
    public void ThrowExceptionOnOracleConnectionAverageAllWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.AverageAll<CompleteTable>(e => e.ColumnInt,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestOracleConnectionAverageAllAsync()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = await connection.AverageAllAsync<CompleteTable>(e => e.ColumnInt, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
    }

    [TestMethod]
    public async Task ThrowExceptionOnOracleConnectionAverageAllAsyncWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.AverageAllAsync<CompleteTable>(e => e.ColumnInt,
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestOracleConnectionAverageAllViaTableName()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = connection.AverageAll(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInt).First());

        // Assert
        Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
    }

    [TestMethod]
    public void ThrowExceptionOnOracleConnectionAverageAllViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.AverageAll(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestOracleConnectionAverageAllAsyncViaTableName()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = await connection.AverageAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInt).First(), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
    }

    [TestMethod]
    public async Task ThrowExceptionOnOracleConnectionAverageAllAsyncViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.AverageAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion
}
