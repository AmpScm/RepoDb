using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;

namespace RepoDb.Oracle.IntegrationTests.Operations;

[TestClass]
public class TruncateTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestOracleConnectionTruncate()
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
    public async Task TestOracleConnectionTruncateAsyncWithoutExpression()
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
    public void TestOracleConnectionTruncateViaTableNameWithoutExpression()
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
    public async Task TestOracleConnectionTruncateAsyncViaTableNameWithoutExpression()
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
