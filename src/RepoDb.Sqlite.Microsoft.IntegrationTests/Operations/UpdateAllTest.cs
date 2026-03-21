using Microsoft.Data.Sqlite;
using RepoDb.Extensions;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Models;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Setup;

namespace RepoDb.Sqlite.Microsoft.IntegrationTests.Operations.MDS;

[TestClass]
public class UpdateAllTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionUpdateAll()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        tables.AsList().ForEach(table => Helper.UpdateCompleteTableProperties(table));

        // Act
        var result = connection.UpdateAll<CompleteTable>(tables);

        // Assert
        Assert.AreEqual(10, result);

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.AsList().ForEach(table =>
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionUpdateAllAsync()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        tables.AsList().ForEach(table => Helper.UpdateCompleteTableProperties(table));

        // Act
        var result = await connection.UpdateAllAsync<CompleteTable>(tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(10, result);

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.AsList().ForEach(table =>
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionUpdateAllViaTableName()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        tables.AsList().ForEach(table => Helper.UpdateCompleteTableProperties(table));

        // Act
        var result = connection.UpdateAll(ClassMappedNameCache.Get<CompleteTable>(), tables);

        // Assert
        Assert.AreEqual(10, result);

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.AsList().ForEach(table =>
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestSqLiteConnectionUpdateAllAsExpandoObjectViaTableName()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        Database.CreateCompleteTables(10, connection);
        var tables = Helper.CreateCompleteTablesAsExpandoObjects(10);

        // Act
        var result = connection.UpdateAll(ClassMappedNameCache.Get<CompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(10, result);

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.AsList().ForEach(table =>
            Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionUpdateAllAsyncViaTableName()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        tables.AsList().ForEach(table => Helper.UpdateCompleteTableProperties(table));

        // Act
        var result = await connection.UpdateAllAsync(ClassMappedNameCache.Get<CompleteTable>(), tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(10, result);

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.AsList().ForEach(table =>
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestSqLiteConnectionUpdateAllAsyncAsExpandoObjectViaTableName()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        Database.CreateCompleteTables(10, connection);
        var tables = Helper.CreateCompleteTablesAsExpandoObjects(10);

        // Act
        var result = await connection.UpdateAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(10, result);

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.AsList().ForEach(table =>
            Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
    }

    #endregion

    #endregion
}
