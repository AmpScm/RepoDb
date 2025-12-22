using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.PostgreSql.IntegrationTests.Operations;

[TestClass]
public class ExecuteNonQueryTest
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

    #region Sync

    [TestMethod]
    public void TestPostgreSqlConnectionExecuteNonQuery()
    {
        // Setup
        IEnumerable<Models.CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.ExecuteNonQuery("DELETE FROM \"CompleteTable\";");

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionExecuteNonQueryWithParameters()
    {
        // Setup
        IEnumerable<Models.CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.ExecuteNonQuery("DELETE FROM \"CompleteTable\" WHERE \"Id\" = @Id;",
            new { tables.Last().Id });

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionExecuteNonQueryWithMultipleStatement()
    {
        // Setup
        IEnumerable<Models.CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.ExecuteNonQuery("DELETE FROM \"CompleteTable\"; DELETE FROM \"CompleteTable\";");

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionExecuteNonQueryAsync()
    {
        // Setup
        IEnumerable<Models.CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.ExecuteNonQueryAsync("DELETE FROM \"CompleteTable\";", cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionExecuteNonQueryAsyncWithParameters()
    {
        // Setup
        IEnumerable<Models.CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.ExecuteNonQueryAsync("DELETE FROM \"CompleteTable\" WHERE \"Id\" = @Id;",
            new { tables.Last().Id }, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionExecuteNonQueryAsyncWithMultipleStatement()
    {
        // Setup
        IEnumerable<Models.CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.ExecuteNonQueryAsync("DELETE FROM \"CompleteTable\"; DELETE FROM \"CompleteTable\";", cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    public TestContext TestContext { get; set; }

    #endregion
}
