using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.PostgreSql.IntegrationTests.Operations;

[TestClass]
public class TruncateTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestPostgreSqlConnectionTruncate()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
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

        using var connection = CreateConnection();
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

        using var connection = CreateConnection();
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

        using var connection = CreateConnection();
        // Act
        int result = await connection.TruncateAsync(ClassMappedNameCache.Get<CompleteTable>(), cancellationToken: TestContext.CancellationToken);
        long countResult = connection.CountAll<CompleteTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    #endregion

    #endregion
}
