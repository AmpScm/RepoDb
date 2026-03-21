using RepoDb.Extensions;
using RepoDb.SQLite.System.IntegrationTests.Models;
using RepoDb.SQLite.System.IntegrationTests.Setup;
using System.Data.SQLite;

namespace RepoDb.SQLite.System.IntegrationTests.Operations.SDS;

[TestClass]
public class ExecuteQueryTest : TestBase
{
    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionExecuteQuery()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.ExecuteQuery<CompleteTable>("SELECT * FROM [CompleteTable];");

        // Assert
        Assert.AreEqual(tables.Count(), result.Count());
        tables.AsList().ForEach(table => Helper.AssertPropertiesEquality(table, result.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestSqLiteConnectionExecuteQueryWithParameters()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.ExecuteQuery<CompleteTable>("SELECT * FROM [CompleteTable] WHERE Id = @Id;",
            new { tables.Last().Id });

        // Assert
        Assert.AreEqual(1, result.Count());
        Helper.AssertPropertiesEquality(tables.Last(), result.First());
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionExecuteQueryAsync()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.ExecuteQueryAsync<CompleteTable>("SELECT * FROM [CompleteTable];", cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result.Count());
        tables.AsList().ForEach(table => Helper.AssertPropertiesEquality(table, result.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestSqLiteConnectionExecuteQueryAsyncWithParameters()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.ExecuteQueryAsync<CompleteTable>("SELECT * FROM [CompleteTable] WHERE Id = @Id;",
            new { tables.Last().Id }, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result.Count());
        Helper.AssertPropertiesEquality(tables.Last(), result.First());
    }
    #endregion
}
