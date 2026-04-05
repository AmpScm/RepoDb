using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.PostgreSql.IntegrationTests.Operations;

[TestClass]
public class CountAllTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestPostgreSqlConnectionCountAll()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        long result = connection.CountAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlConnectionCountAllWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.CountAll<CompleteTable>(hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionCountAllAsync()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        long result = await connection.CountAllAsync<CompleteTable>(cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task ThrowExceptionOnPostgreSqlConnectionCountAllAsyncWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.CountAllAsync<CompleteTable>(hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestPostgreSqlConnectionCountAllViaTableName()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        long result = connection.CountAll(ClassMappedNameCache.Get<CompleteTable>());

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlConnectionCountAllViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.CountAll(ClassMappedNameCache.Get<CompleteTable>(),
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionCountAllAsyncViaTableName()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        long result = await connection.CountAllAsync(ClassMappedNameCache.Get<CompleteTable>(), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task ThrowExceptionOnPostgreSqlConnectionCountAllAsyncViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.CountAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion
}
