using Microsoft.Data.Sqlite;
using RepoDb.Extensions;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Models;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Setup;

namespace RepoDb.Sqlite.Microsoft.IntegrationTests.Operations.MDS;

[TestClass]
public class QueryAllTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionQueryAll()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.AsList().ForEach(table =>
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void ThrowExceptionQueryAllWithHints()
    {
        // Setup
        var table = Database.CreateCompleteTables(1).First();

        using var connection = new SqliteConnection(Database.ConnectionString);
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.QueryAll<CompleteTable>(hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionQueryAllAsync()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var queryResult = await connection.QueryAllAsync<CompleteTable>(cancellationToken: TestContext.CancellationToken);

        // Assert
        tables.AsList().ForEach(table =>
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task ThrowExceptionQueryAllAsyncWithHints()
    {
        // Setup
        var table = Database.CreateCompleteTables(1).First();

        using var connection = new SqliteConnection(Database.ConnectionString);
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.QueryAllAsync<CompleteTable>(hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionQueryAllViaTableName()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var queryResult = connection.QueryAll(ClassMappedNameCache.Get<CompleteTable>());

        // Assert
        tables.AsList().ForEach(table =>
            Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void ThrowExceptionQueryAllViaTableNameWithHints()
    {
        // Setup
        var table = Database.CreateCompleteTables(1).First();

        using var connection = new SqliteConnection(Database.ConnectionString);
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.Query(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionQueryAllAsyncViaTableName()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var queryResult = await connection.QueryAllAsync(ClassMappedNameCache.Get<CompleteTable>(), cancellationToken: TestContext.CancellationToken);

        // Assert
        tables.AsList().ForEach(table =>
            Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task ThrowExceptionQueryAllAsyncViaTableNameWithHints()
    {
        // Setup
        var table = Database.CreateCompleteTables(1).First();

        using var connection = new SqliteConnection(Database.ConnectionString);
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null,
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion
}
