using Microsoft.Data.Sqlite;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Setup;

namespace RepoDb.Sqlite.Microsoft.IntegrationTests.Operations.MDS;

[TestClass]
public class ExecuteNonQueryTest : TestBase
{
    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionExecuteNonQuery()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.ExecuteNonQuery("DELETE FROM [CompleteTable];");

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void TestSqLiteConnectionExecuteNonQueryWithParameters()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.ExecuteNonQuery("DELETE FROM [CompleteTable] WHERE Id = @Id;",
            new { tables.Last().Id });

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestSqLiteConnectionExecuteNonQueryWithMultipleStatement()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.ExecuteNonQuery("DELETE FROM [CompleteTable]; VACUUM;");

        // Assert
        Assert.AreEqual((tables.Count() * 2), result);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionExecuteNonQueryAsync()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.ExecuteNonQueryAsync("DELETE FROM [CompleteTable];", cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionExecuteNonQueryAsyncWithParameters()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.ExecuteNonQueryAsync("DELETE FROM [CompleteTable] WHERE Id = @Id;",
            new { tables.Last().Id }, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionExecuteNonQueryAsyncWithMultipleStatement()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.ExecuteNonQueryAsync("DELETE FROM [CompleteTable]; VACUUM;", cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual((tables.Count() * 2), result);
    }

    #endregion
}
