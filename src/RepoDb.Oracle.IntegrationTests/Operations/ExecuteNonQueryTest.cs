using RepoDb.Oracle.IntegrationTests.Setup;

namespace RepoDb.Oracle.IntegrationTests.Operations;

[TestClass]
public class ExecuteNonQueryTest : TestBase
{
    #region Sync

    [TestMethod]
    public void TestOracleConnectionExecuteNonQuery()
    {
        // Setup
        IEnumerable<Models.CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = connection.ExecuteNonQuery("DELETE FROM \"CompleteTable\"");

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void TestOracleConnectionExecuteNonQueryWithParameters()
    {
        // Setup
        IEnumerable<Models.CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = connection.ExecuteNonQuery("DELETE FROM \"CompleteTable\" WHERE \"Id\" = :Id",
            new { tables.Last().Id });

        // Assert
        Assert.AreEqual(1, result);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestOracleConnectionExecuteNonQueryAsync()
    {
        // Setup
        IEnumerable<Models.CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = await connection.ExecuteNonQueryAsync("DELETE FROM \"CompleteTable\"", cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task TestOracleConnectionExecuteNonQueryAsyncWithParameters()
    {
        // Setup
        IEnumerable<Models.CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = await connection.ExecuteNonQueryAsync("DELETE FROM \"CompleteTable\" WHERE \"Id\" = :Id",
            new { tables.Last().Id }, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);
    }

    #endregion
}
