using Microsoft.Data.Sqlite;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Models;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Setup;

namespace RepoDb.Sqlite.Microsoft.IntegrationTests.Operations.MDS;

[TestClass]
public class TruncateTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionTruncate()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.Truncate<CompleteTable>();
        var countResult = connection.CountAll<CompleteTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionTruncateAsyncWithoutExpression()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.TruncateAsync<CompleteTable>(cancellationToken: TestContext.CancellationToken);
        var countResult = connection.CountAll<CompleteTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionTruncateViaTableNameWithoutExpression()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.Truncate(ClassMappedNameCache.Get<CompleteTable>());
        var countResult = connection.CountAll<CompleteTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionTruncateAsyncViaTableNameWithoutExpression()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.TruncateAsync(ClassMappedNameCache.Get<CompleteTable>(), cancellationToken: TestContext.CancellationToken);
        var countResult = connection.CountAll<CompleteTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    #endregion

    #endregion
}
