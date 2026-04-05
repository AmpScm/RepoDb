using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.PostgreSql.IntegrationTests.Operations;

[TestClass]
public class AverageAllTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestPostgreSqlConnectionAverageAll()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = connection.AverageAll<CompleteTable>(e => e.ColumnInteger);

        // Assert
        Assert.AreEqual(tables.Average(e => e.ColumnInteger), Convert.ToDouble(result));
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlConnectionAverageAllWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.AverageAll<CompleteTable>(e => e.ColumnInteger,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionAverageAllAsync()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = await connection.AverageAllAsync<CompleteTable>(e => e.ColumnInteger, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Average(e => e.ColumnInteger), Convert.ToDouble(result));
    }

    [TestMethod]
    public async Task ThrowExceptionOnPostgreSqlConnectionAverageAllAsyncWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.AverageAllAsync<CompleteTable>(e => e.ColumnInteger,
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestPostgreSqlConnectionAverageAllViaTableName()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = connection.AverageAll(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInteger).First());

        // Assert
        Assert.AreEqual(tables.Average(e => e.ColumnInteger), Convert.ToDouble(result));
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlConnectionAverageAllViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.AverageAll(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInteger).First(),
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionAverageAllAsyncViaTableName()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = await connection.AverageAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInteger).First(), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Average(e => e.ColumnInteger), Convert.ToDouble(result));
    }

    [TestMethod]
    public async Task ThrowExceptionOnPostgreSqlConnectionAverageAllAsyncViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.AverageAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInteger).First(),
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion
}
