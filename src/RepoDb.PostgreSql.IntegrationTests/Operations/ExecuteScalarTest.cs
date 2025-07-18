using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.PostgreSql.IntegrationTests.Operations;

[TestClass]
public class ExecuteScalarTest
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
    public void TestPostgreSqlConnectionExecuteScalar()
    {
        // Setup
        IEnumerable<Models.CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = connection.ExecuteScalar("SELECT COUNT(*) FROM \"CompleteTable\";");

        // Assert
        Assert.AreEqual(tables.Count(), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionExecuteScalarWithReturnType()
    {
        // Setup
        IEnumerable<Models.CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM \"CompleteTable\";");

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionExecuteScalarAsync()
    {
        // Setup
        IEnumerable<Models.CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = await connection.ExecuteScalarAsync("SELECT COUNT(*) FROM \"CompleteTable\";");

        // Assert
        Assert.AreEqual(tables.Count(), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionExecuteScalarAsyncWithReturnType()
    {
        // Setup
        IEnumerable<Models.CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM \"CompleteTable\";");

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    #endregion
}
