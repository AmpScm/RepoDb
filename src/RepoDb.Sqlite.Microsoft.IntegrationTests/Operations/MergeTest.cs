using Microsoft.Data.Sqlite;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Models;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Setup;
using RepoDb.Trace;

namespace RepoDb.Sqlite.Microsoft.IntegrationTests.Operations.MDS;

[TestClass]
public class MergeTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionMergeForIdentityForEmptyTable()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
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
    public void TestSqLiteConnectionMergeForIdentityForNonEmptyTable()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
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
    public void TestSqLiteConnectionMergeForIdentityForNonEmptyTableWithQualifiers()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
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
    public async Task TestSqLiteConnectionMergeAsyncForIdentityForEmptyTable()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
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
    public async Task TestSqLiteConnectionMergeAsyncForIdentityForNonEmptyTable()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
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
    public async Task TestSqLiteConnectionMergeAsyncForIdentityForNonEmptyTableWithQualifiers()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
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
    public void TestSqLiteConnectionMergeViaTableNameForIdentityForEmptyTable()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
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
    public void TestSqLiteConnectionMergeViaTableNameForIdentityForNonEmptyTable()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
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
    public void TestSqLiteConnectionMergeViaTableNameForIdentityForNonEmptyTableWithQualifiers()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
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
    public void TestSqLiteConnectionMergeAsDynamicViaTableNameForIdentityForEmptyTable()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateMdsTables(connection);

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
    public void TestSqLiteConnectionMergeAsDynamicViaTableNameForIdentityForNonEmptyTable()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();
        Helper.UpdateCompleteTableProperties(table);

        // Act
        var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
            table, trace: new DiagnosticsTracer());

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public void TestSqLiteConnectionMergeAsDynamicViaTableNameForIdentityForNonEmptyTableWithQualifiers()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
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
    public void TestSqLiteConnectionMergeViaTableNameAsExpandoObjectForIdentityForEmptyTable()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
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
    public void TestSqLiteConnectionMergeViaTableNameAsExpandoObjectForIdentityForNonEmptyTable()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
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
    public async Task TestSqLiteConnectionMergeAsyncViaTableNameForIdentityForEmptyTable()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
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
    public async Task TestSqLiteConnectionMergeAsyncViaTableNameForIdentityForNonEmptyTable()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
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
    public async Task TestSqLiteConnectionMergeAsyncViaTableNameForIdentityForNonEmptyTableWithQualifiers()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
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
    public async Task TestSqLiteConnectionMergeAsyncAsDynamicViaTableNameForIdentityForEmptyTable()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateMdsTables(connection);

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
    public async Task TestSqLiteConnectionMergeAsyncAsDynamicViaTableNameForIdentityForNonEmptyTable()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
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
    public async Task TestSqLiteConnectionMergeAsyncAsDynamicViaTableNameForIdentityForNonEmptyTableWithQualifiers()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
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
    public async Task TestSqLiteConnectionMergeAsyncViaTableNameAsExpandoObjectForIdentityForEmptyTable()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
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

    [TestMethod]
    public async Task TestSqLiteConnectionMergeAsyncViaTableNameAsExpandoObjectForIdentityForNonEmptyTable()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
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
