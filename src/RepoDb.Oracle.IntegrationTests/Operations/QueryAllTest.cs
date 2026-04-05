using RepoDb.Extensions;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;

namespace RepoDb.Oracle.IntegrationTests.Operations;

[TestClass]
public class QueryAllTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestOracleConnectionQueryAll()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.AsList().ForEach(table =>
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void ThrowExceptionQueryAllWithHints()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.QueryAll<CompleteTable>(hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestOracleConnectionQueryAllAsync()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        IEnumerable<CompleteTable> queryResult = await connection.QueryAllAsync<CompleteTable>(cancellationToken: TestContext.CancellationToken);

        // Assert
        tables.AsList().ForEach(table =>
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task ThrowExceptionQueryAllAsyncWithHints()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.QueryAllAsync<CompleteTable>(hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestOracleConnectionQueryAllViaTableName()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        IEnumerable<dynamic> queryResult = connection.QueryAll(ClassMappedNameCache.Get<CompleteTable>());

        // Assert
        tables.AsList().ForEach(table =>
            Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void ThrowExceptionQueryAllViaTableNameWithHints()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.Query(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestOracleConnectionQueryAllAsyncViaTableName()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        IEnumerable<dynamic> queryResult = await connection.QueryAllAsync(ClassMappedNameCache.Get<CompleteTable>(), cancellationToken: TestContext.CancellationToken);

        // Assert
        tables.AsList().ForEach(table =>
            Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task ThrowExceptionQueryAllAsyncViaTableNameWithHints()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null,
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion
}
