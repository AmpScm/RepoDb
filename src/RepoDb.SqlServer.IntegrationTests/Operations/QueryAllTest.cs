using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.SqlServer.IntegrationTests.Models;
using RepoDb.SqlServer.IntegrationTests.Setup;

namespace RepoDb.SqlServer.IntegrationTests.Operations;

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
    public void TestSqlServerConnectionQueryAll()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.AsList().ForEach(table =>
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestSqlServerConnectionQueryAllWithHints()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var queryResult = connection.QueryAll<CompleteTable>(hints: SqlServerTableHints.NoLock);

        // Assert
        tables.AsList().ForEach(table =>
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqlServerConnectionQueryAllAsync()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var queryResult = await connection.QueryAllAsync<CompleteTable>(cancellationToken: TestContext.CancellationToken);

        // Assert
        tables.AsList().ForEach(table =>
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestSqlServerConnectionQueryAllAsyncWithHints()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var queryResult = await connection.QueryAllAsync<CompleteTable>(hints: SqlServerTableHints.NoLock, cancellationToken: TestContext.CancellationToken);

        // Assert
        tables.AsList().ForEach(table =>
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestSqlServerConnectionQueryAllViaTableName()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var queryResult = connection.QueryAll(ClassMappedNameCache.Get<CompleteTable>());

        // Assert
        tables.AsList().ForEach(table =>
            Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestSqlServerConnectionQueryAllViaTableNameWithHints()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var queryResult = connection.QueryAll(ClassMappedNameCache.Get<CompleteTable>(),
            hints: SqlServerTableHints.NoLock);

        // Assert
        tables.AsList().ForEach(table =>
            Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqlServerConnectionQueryAllAsyncViaTableName()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var queryResult = await connection.QueryAllAsync(ClassMappedNameCache.Get<CompleteTable>(), cancellationToken: TestContext.CancellationToken);

        // Assert
        tables.AsList().ForEach(table =>
            Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestSqlServerConnectionQueryAllAsyncViaTableNameWithHints()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var queryResult = await connection.QueryAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            hints: SqlServerTableHints.NoLock, cancellationToken: TestContext.CancellationToken);

        // Assert
        tables.AsList().ForEach(table =>
            Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    public TestContext TestContext { get; set; }

    #endregion

    #endregion
}
