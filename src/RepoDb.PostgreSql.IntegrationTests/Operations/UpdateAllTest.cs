using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Extensions;
using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.PostgreSql.IntegrationTests.Operations;

[TestClass]
public class UpdateAllTest
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
    public void TestPostgreSqlConnectionUpdateAll()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        tables.AsList().ForEach(table => Helper.UpdateCompleteTableProperties(table));

        // Act
        int result = connection.UpdateAll<CompleteTable>(tables);

        // Assert
        Assert.AreEqual(10, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.AsList().ForEach(table =>
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionUpdateAllAsync()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        tables.AsList().ForEach(table => Helper.UpdateCompleteTableProperties(table));

        // Act
        int result = await connection.UpdateAllAsync<CompleteTable>(tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(10, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.AsList().ForEach(table =>
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestPostgreSqlConnectionUpdateAllViaTableName()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        tables.AsList().ForEach(table => Helper.UpdateCompleteTableProperties(table));

        // Act
        int result = connection.UpdateAll(ClassMappedNameCache.Get<CompleteTable>(), tables);

        // Assert
        Assert.AreEqual(10, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.AsList().ForEach(table =>
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionUpdateAllViaTableNameAsExpandoObjects()
    {
        // Setup
        List<CompleteTable> entities = Database.CreateCompleteTables(10).AsList();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        List<System.Dynamic.ExpandoObject> tables = Helper.CreateCompleteTablesAsExpandoObjects(10).AsList();
        tables.ForEach(e => ((IDictionary<string, object?>)e)["Id"] = entities[tables.IndexOf(e)].Id);

        // Act
        int result = connection.UpdateAll(ClassMappedNameCache.Get<CompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(10, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.AsList().ForEach(table =>
            Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionUpdateAllAsyncViaTableName()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        tables.AsList().ForEach(table => Helper.UpdateCompleteTableProperties(table));

        // Act
        int result = await connection.UpdateAllAsync(ClassMappedNameCache.Get<CompleteTable>(), tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(10, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.AsList().ForEach(table =>
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionUpdateAllAsyncViaTableNameAsExpandoObjects()
    {
        // Setup
        List<CompleteTable> entities = Database.CreateCompleteTables(10).AsList();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        List<System.Dynamic.ExpandoObject> tables = Helper.CreateCompleteTablesAsExpandoObjects(10).AsList();
        tables.ForEach(e => ((IDictionary<string, object?>)e)["Id"] = entities[tables.IndexOf(e)].Id);

        // Act
        int result = await connection.UpdateAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(10, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.AsList().ForEach(table =>
            Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
    }

    public TestContext TestContext { get; set; }

    #endregion

    #endregion
}
