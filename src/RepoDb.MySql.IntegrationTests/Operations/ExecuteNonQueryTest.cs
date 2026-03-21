using MySql.Data.MySqlClient;
using RepoDb.MySql.IntegrationTests.Setup;

namespace RepoDb.MySql.IntegrationTests.Operations;

[TestClass]
public class ExecuteNonQueryTest : TestBase
{
    #region Sync

    [TestMethod]
    public void TestMySqlConnectionExecuteNonQuery()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new MySqlConnection(Database.ConnectionString);
        // Act
        var result = connection.ExecuteNonQuery("DELETE FROM `CompleteTable`;");

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void TestMySqlConnectionExecuteNonQueryWithParameters()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new MySqlConnection(Database.ConnectionString);
        // Act
        var result = connection.ExecuteNonQuery("DELETE FROM `CompleteTable` WHERE Id = @Id;",
            new { tables.Last().Id });

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestMySqlConnectionExecuteNonQueryWithMultipleStatement()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new MySqlConnection(Database.ConnectionString);
        // Act
        var result = connection.ExecuteNonQuery("DELETE FROM `CompleteTable`; DELETE FROM `CompleteTable`;");

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestMySqlConnectionExecuteNonQueryAsync()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new MySqlConnection(Database.ConnectionString);
        // Act
        var result = await connection.ExecuteNonQueryAsync("DELETE FROM `CompleteTable`;", cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task TestMySqlConnectionExecuteNonQueryAsyncWithParameters()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new MySqlConnection(Database.ConnectionString);
        // Act
        var result = await connection.ExecuteNonQueryAsync("DELETE FROM `CompleteTable` WHERE Id = @Id;",
            new { tables.Last().Id }, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestMySqlConnectionExecuteNonQueryAsyncWithMultipleStatement()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new MySqlConnection(Database.ConnectionString);
        // Act
        var result = await connection.ExecuteNonQueryAsync("DELETE FROM `CompleteTable`; DELETE FROM `CompleteTable`;", cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }
    #endregion
}
