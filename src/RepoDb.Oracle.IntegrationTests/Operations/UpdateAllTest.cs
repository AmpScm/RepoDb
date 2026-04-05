using RepoDb.Extensions;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;

namespace RepoDb.Oracle.IntegrationTests.Operations;

[TestClass]
public class UpdateAllTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestOracleConnectionUpdateAll()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public async Task TestOracleConnectionUpdateAllAsync()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public void TestOracleConnectionUpdateAllViaTableName()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public void TestOracleConnectionUpdateAllViaTableNameAsExpandoObjects()
    {
        // Setup
        List<CompleteTable> entities = Database.CreateCompleteTables(10).AsList();

        using var connection = CreateConnection();
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
    public async Task TestOracleConnectionUpdateAllAsyncViaTableName()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public async Task TestOracleConnectionUpdateAllAsyncViaTableNameAsExpandoObjects()
    {
        // Setup
        List<CompleteTable> entities = Database.CreateCompleteTables(10).AsList();

        using var connection = CreateConnection();
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

    #endregion

    #endregion
}
