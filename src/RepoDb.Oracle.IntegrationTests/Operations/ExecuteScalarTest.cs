using RepoDb.Oracle.IntegrationTests.Setup;

namespace RepoDb.Oracle.IntegrationTests.Operations;

[TestClass]
public class ExecuteScalarTest : TestBase
{
    #region Sync

    [TestMethod]
    public void TestOracleConnectionExecuteScalar()
    {
        // Setup
        IEnumerable<Models.CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = connection.ExecuteScalar("SELECT COUNT(*) FROM \"CompleteTable\"");

        // Assert
        Assert.AreEqual(tables.Count(), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestOracleConnectionExecuteScalarWithReturnType()
    {
        // Setup
        IEnumerable<Models.CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM \"CompleteTable\"");

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestOracleConnectionExecuteScalarAsync()
    {
        // Setup
        IEnumerable<Models.CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = await connection.ExecuteScalarAsync("SELECT COUNT(*) FROM \"CompleteTable\"", cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestOracleConnectionExecuteScalarAsyncWithReturnType()
    {
        // Setup
        IEnumerable<Models.CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM \"CompleteTable\"", cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    #endregion
}
