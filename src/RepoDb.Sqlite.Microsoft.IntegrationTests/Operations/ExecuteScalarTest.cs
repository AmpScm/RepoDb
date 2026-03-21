using Microsoft.Data.Sqlite;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Setup;

namespace RepoDb.Sqlite.Microsoft.IntegrationTests.Operations.MDS;

[TestClass]
public class ExecuteScalarTest : TestBase
{
    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionExecuteScalar()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.ExecuteScalar("SELECT COUNT(*) FROM [CompleteTable];");

        // Assert
        Assert.AreEqual(tables.Count(), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestSqLiteConnectionExecuteScalarWithReturnType()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM [CompleteTable];");

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionExecuteScalarAsync()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.ExecuteScalarAsync("SELECT COUNT(*) FROM [CompleteTable];", cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestSqLiteConnectionExecuteScalarAsyncWithReturnType()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM [CompleteTable];", cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    #endregion
}
