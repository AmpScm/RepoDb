using System.Data.SQLite;
using RepoDb.Extensions;
using RepoDb.SQLite.System.IntegrationTests.Models;
using RepoDb.SQLite.System.IntegrationTests.Setup;

namespace RepoDb.SQLite.System.IntegrationTests.Operations.SDS;

[TestClass]
public class EnumerableTest : TestBase
{
    #region List

    #region Sync

    [TestMethod]
    public void TestSQLiteConnectionQueryListContains()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateNonIdentityCompleteTables(10, connection).AsList();
        var ids = tables.Select(e => e.Id).AsList();

        // Act
        var result = connection.Query<NonIdentityCompleteTable>(e => ids.Contains(e.Id));

        // Assert
        Assert.AreEqual(tables.Count, result.Count());
        tables.ForEach(table =>
            Helper.AssertPropertiesEquality(table, result.First(item => item.Id == table.Id)));
    }

    [TestMethod]
    public void TestSQLiteConnectionQueryEmptyList()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateNonIdentityCompleteTables(10, connection).AsList();

        // Act
        var result = connection.Query<NonIdentityCompleteTable>(e => Enumerable.Empty<Guid>().Contains(e.Id));

        // Assert
        Assert.AreEqual(0, result.Count());
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSQLiteConnectionQueryAsyncListContains()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateNonIdentityCompleteTables(10, connection).AsList();
        var ids = tables.Select(e => e.Id).AsList();

        // Act
        var result = await connection.QueryAsync<NonIdentityCompleteTable>(e => ids.Contains(e.Id), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, result.Count());
        tables.ForEach(table =>
            Helper.AssertPropertiesEquality(table, result.First(item => item.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestSQLiteConnectionQueryAsyncEmptyList()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateNonIdentityCompleteTables(10, connection).AsList();

        // Act
        var result = await connection.QueryAsync<NonIdentityCompleteTable>(e => Enumerable.Empty<Guid>().Contains(e.Id), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(0, result.Count());
    }
    #endregion

    #endregion
}
