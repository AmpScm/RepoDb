using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.PostgreSql.IntegrationTests.Operations;

[TestClass]
public class TruncateTest
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

    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestPostgreSqlConnectionTruncate()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.Truncate<CompleteTable>();
        long countResult = connection.CountAll<CompleteTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionTruncateAsyncWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.TruncateAsync<CompleteTable>(cancellationToken: TestContext.CancellationToken);
        long countResult = connection.CountAll<CompleteTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestPostgreSqlConnectionTruncateViaTableNameWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.Truncate(ClassMappedNameCache.Get<CompleteTable>());
        long countResult = connection.CountAll<CompleteTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionTruncateAsyncViaTableNameWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.TruncateAsync(ClassMappedNameCache.Get<CompleteTable>(), cancellationToken: TestContext.CancellationToken);
        long countResult = connection.CountAll<CompleteTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    public TestContext TestContext { get; set; }

    #endregion

    #endregion
}
