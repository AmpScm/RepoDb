using System.Data.SQLite;
using RepoDb.SQLite.System.IntegrationTests.Models;
using RepoDb.SQLite.System.IntegrationTests.Setup;

namespace RepoDb.SQLite.System.IntegrationTests.Operations.SDS;

[TestClass]
public class MergeTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestSQLiteConnectionMergeForIdentityForEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();

        // Act
        var result = connection.Merge<CompleteTable>(table);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());

        // Act
        var queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public void TestSQLiteConnectionMergeForIdentityForNonEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();

        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        var result = connection.Merge<CompleteTable>(table);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public void TestSQLiteConnectionMergeForIdentityForNonEmptyTableWithQualifiers()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();
        var qualifiers = new[]
        {
                new Field("Id", typeof(long))
            };
        Helper.UpdateCompleteTableProperties(table);
        table.ColumnInt = 0;
        table.ColumnChar = "C";

        // Act
        var result = connection.Merge<CompleteTable>(table,
            qualifiers: qualifiers);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSQLiteConnectionMergeAsyncForIdentityForEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();

        // Act
        var result = await connection.MergeAsync<CompleteTable>(table, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public async Task TestSQLiteConnectionMergeAsyncForIdentityForNonEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();

        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        var result = await connection.MergeAsync<CompleteTable>(table, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public async Task TestSQLiteConnectionMergeAsyncForIdentityForNonEmptyTableWithQualifiers()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();
        var qualifiers = new[]
        {
                new Field("Id", typeof(long))
            };
        Helper.UpdateCompleteTableProperties(table);
        table.ColumnInt = 0;
        table.ColumnChar = "C";

        // Act
        var result = await connection.MergeAsync<CompleteTable>(table,
            qualifiers: qualifiers, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestSQLiteConnectionMergeViaTableNameForIdentityForEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();

        // Act
        var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
            table);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public void TestSQLiteConnectionMergeViaTableNameForIdentityForNonEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();
        Helper.UpdateCompleteTableProperties(table);

        // Act
        var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
            table);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public void TestSQLiteConnectionMergeViaTableNameForIdentityForNonEmptyTableWithQualifiers()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();
        var qualifiers = new[]
        {
                new Field("Id", typeof(long))
            };
        Helper.UpdateCompleteTableProperties(table);
        table.ColumnInt = 0;
        table.ColumnChar = "C";

        // Act
        var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
            table,
            qualifiers: qualifiers);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public void TestSQLiteConnectionMergeAsDynamicViaTableNameForIdentityForEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

        // Setup
        var table = Helper.CreateCompleteTablesAsDynamics(1).First();

        // Act
        var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
            (object)table);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Assert.AreEqual(1, queryResult?.Count());
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public void TestSQLiteConnectionMergeAsDynamicViaTableNameForIdentityForNonEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();
        Helper.UpdateCompleteTableProperties(table);

        // Act
        var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
            table);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public void TestSQLiteConnectionMergeAsDynamicViaTableNameForIdentityForNonEmptyTableWithQualifiers()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();
        Helper.UpdateCompleteTableProperties(table);
        var qualifiers = new[]
        {
                new Field("Id", typeof(long))
            };

        // Act
        var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
            table,
            qualifiers: qualifiers);

        // Assert
        Assert.AreEqual(table.Id, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public void TestSQLiteConnectionMergeViaTableNameAsExpandoObjectForIdentityForEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        Database.CreateCompleteTables(1, connection);
        var table = Helper.CreateCompleteTablesAsExpandoObjects(1).First();

        // Act
        var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
            table);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.IsGreaterThan(0, (long)result);
        Assert.AreEqual(((dynamic)table).Id, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertMembersEquality(queryResult.First(), table);
    }

    [TestMethod]
    public void TestSQLiteConnectionMergeViaTableNameAsExpandoObjectForIdentityForNonEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        Database.CreateCompleteTables(1, connection);
        var table = Helper.CreateCompleteTablesAsExpandoObjects(1).First();

        // Act
        var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
            table);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(((dynamic)table).Id, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertMembersEquality(queryResult.First(), table);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSQLiteConnectionMergeAsyncViaTableNameForIdentityForEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();

        // Act
        var result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
            table, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());

        // Act
        var queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public async Task TestSQLiteConnectionMergeAsyncViaTableNameForIdentityForNonEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();
        Helper.UpdateCompleteTableProperties(table);

        // Act
        var result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
            table, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public async Task TestSQLiteConnectionMergeAsyncViaTableNameForIdentityForNonEmptyTableWithQualifiers()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();
        var qualifiers = new[]
        {
                new Field("Id", typeof(long))
            };
        Helper.UpdateCompleteTableProperties(table);

        // Act
        var result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
            table,
            qualifiers: qualifiers, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public async Task TestSQLiteConnectionMergeAsyncAsDynamicViaTableNameForIdentityForEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

        // Setup
        var table = Helper.CreateCompleteTablesAsDynamics(1).First();

        // Act
        var result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
            (object)table, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public async Task TestSQLiteConnectionMergeAsyncAsDynamicViaTableNameForIdentityForNonEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();
        Helper.UpdateCompleteTableProperties(table);

        // Act
        var result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
            table, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public async Task TestSQLiteConnectionMergeAsyncAsDynamicViaTableNameForIdentityForNonEmptyTableWithQualifiers()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();
        Helper.UpdateCompleteTableProperties(table);
        var qualifiers = new[]
        {
                new Field("Id", typeof(long))
            };

        // Act
        var result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
            table,
            qualifiers: qualifiers, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public async Task TestSQLiteConnectionMergeAsyncViaTableNameAsExpandoObjectForIdentityForEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        Database.CreateCompleteTables(1, connection);
        var table = Helper.CreateCompleteTablesAsExpandoObjects(1).First();

        // Act
        var result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
            table, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.IsGreaterThan(0, (long)result);
        Assert.AreEqual(((dynamic)table).Id, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertMembersEquality(queryResult.First(), table);
    }

    [TestMethod]
    public async Task TestSQLiteConnectionMergeAsyncViaTableNameAsExpandoObjectForIdentityForNonEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        Database.CreateCompleteTables(1, connection);
        var table = Helper.CreateCompleteTablesAsExpandoObjects(1).First();

        // Act
        var result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
            table, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(((dynamic)table).Id, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertMembersEquality(queryResult.First(), table);
    }
    #endregion

    #endregion
}
