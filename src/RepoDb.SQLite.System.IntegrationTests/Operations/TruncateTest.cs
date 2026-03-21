using RepoDb.SQLite.System.IntegrationTests.Models;
using RepoDb.SQLite.System.IntegrationTests.Setup;
using System.Data.SQLite;

namespace RepoDb.SQLite.System.IntegrationTests.Operations.SDS;

[TestClass]
public class TruncateTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionTruncate()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
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
        using var connection = new SQLiteConnection(Database.ConnectionString);
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
        using var connection = new SQLiteConnection(Database.ConnectionString);
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
        using var connection = new SQLiteConnection(Database.ConnectionString);
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
