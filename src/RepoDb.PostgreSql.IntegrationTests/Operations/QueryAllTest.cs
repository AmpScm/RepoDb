using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Extensions;
using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.PostgreSql.IntegrationTests.Operations;

[TestClass]
public class QueryAllTest
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
    public void TestPostgreSqlConnectionQueryAll()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
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

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.QueryAll<CompleteTable>(hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionQueryAllAsync()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<CompleteTable> queryResult = await connection.QueryAllAsync<CompleteTable>();

        // Assert
        tables.AsList().ForEach(table =>
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task ThrowExceptionQueryAllAsyncWithHints()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.QueryAllAsync<CompleteTable>(hints: "WhatEver"));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestPostgreSqlConnectionQueryAllViaTableName()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
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

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.Query(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionQueryAllAsyncViaTableName()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<dynamic> queryResult = await connection.QueryAllAsync(ClassMappedNameCache.Get<CompleteTable>());

        // Assert
        tables.AsList().ForEach(table =>
            Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task ThrowExceptionQueryAllAsyncViaTableNameWithHints()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #endregion
}
