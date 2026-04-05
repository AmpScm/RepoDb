using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;

namespace RepoDb.Oracle.IntegrationTests.Operations;

[TestClass]
public class CountAllTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestOracleConnectionCountAll()
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
    public void ThrowExceptionOnOracleConnectionCountAllWithHints()
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
    public async Task TestOracleConnectionCountAllAsync()
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
    public async Task ThrowExceptionOnOracleConnectionCountAllAsyncWithHints()
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
    public void TestOracleConnectionCountAllViaTableName()
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
    public void ThrowExceptionOnOracleConnectionCountAllViaTableNameWithHints()
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
    public async Task TestOracleConnectionCountAllAsyncViaTableName()
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
    public async Task ThrowExceptionOnOracleConnectionCountAllAsyncViaTableNameWithHints()
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
