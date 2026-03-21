using RepoDb.Extensions;
using RepoDb.SQLite.System.IntegrationTests.Models;
using RepoDb.SQLite.System.IntegrationTests.Setup;
using System.Data.SQLite;

namespace RepoDb.SQLite.System.IntegrationTests.Operations.SDS;

[TestClass]
public class MergeAllTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestSQLiteConnectionMergeAllForIdentityForEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

        // Setup
        var tables = Helper.CreateCompleteTables(10);

        // Act
        var result = connection.MergeAll<CompleteTable>(tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(10, queryResult.Count());
        Helper.AssertPropertiesEquality(tables.Last(), queryResult.Last());
    }

    [TestMethod]
    public void TestSQLiteConnectionMergeAllForIdentityForNonEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection).AsList();

        // Setup
        tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

        // Act
        var result = connection.MergeAll<CompleteTable>(tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestSQLiteConnectionMergeAllForIdentityForNonEmptyTableWithQualifiers()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection).AsList();
        var qualifiers = new[]
        {
                new Field("Id", typeof(long))
            };

        // Setup
        tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

        // Act
        var result = connection.MergeAll<CompleteTable>(tables,
            qualifiers);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestSQLiteConnectionMergeAllForNonIdentityForEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

        // Setup
        var tables = Helper.CreateNonIdentityCompleteTables(10);

        // Act
        var result = connection.MergeAll<NonIdentityCompleteTable>(tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestSQLiteConnectionMergeAllForNonIdentityForNonEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateNonIdentityCompleteTables(10, connection).AsList();

        // Setup
        tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

        // Act
        var result = connection.MergeAll<NonIdentityCompleteTable>(tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestSQLiteConnectionMergeAllForNonIdentityForNonEmptyTableWithQualifiers()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateNonIdentityCompleteTables(10, connection).AsList();
        var qualifiers = new[]
        {
                new Field("Id", typeof(string))
            };

        // Setup
        tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

        // Act
        var result = connection.MergeAll<NonIdentityCompleteTable>(tables,
            qualifiers);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSQLiteConnectionMergeAllAsyncForIdentityForEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

        // Setup
        var tables = Helper.CreateCompleteTables(10);

        // Act
        var result = await connection.MergeAllAsync<CompleteTable>(tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(10, queryResult.Count());
        Helper.AssertPropertiesEquality(tables.Last(), queryResult.Last());
    }

    [TestMethod]
    public async Task TestSQLiteConnectionMergeAllAsyncForIdentityForNonEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection).AsList();

        // Setup
        tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

        // Act
        var result = await connection.MergeAllAsync<CompleteTable>(tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestSQLiteConnectionMergeAllAsyncForIdentityForNonEmptyTableWithQualifiers()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection).AsList();
        var qualifiers = new[]
        {
                new Field("Id", typeof(long))
            };

        // Setup
        tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

        // Act
        var result = await connection.MergeAllAsync<CompleteTable>(tables,
            qualifiers, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestSQLiteConnectionMergeAllAsyncForNonIdentityForEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

        // Setup
        var tables = Helper.CreateNonIdentityCompleteTables(10);

        // Act
        var result = await connection.MergeAllAsync<NonIdentityCompleteTable>(tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestSQLiteConnectionMergeAllAsyncForNonIdentityForNonEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateNonIdentityCompleteTables(10, connection).AsList();

        // Setup
        tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

        // Act
        var result = await connection.MergeAllAsync<NonIdentityCompleteTable>(tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestSQLiteConnectionMergeAllAsyncForNonIdentityForNonEmptyTableWithQualifiers()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateNonIdentityCompleteTables(10, connection).AsList();
        var qualifiers = new[]
        {
                new Field("Id", typeof(string))
            };

        // Setup
        tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

        // Act
        var result = await connection.MergeAllAsync<NonIdentityCompleteTable>(tables,
            qualifiers, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestSQLiteConnectionMergeAllViaTableNameForIdentityForEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

        // Setup
        var tables = Helper.CreateCompleteTables(10);

        // Act
        var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(10, queryResult.Count());
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestSQLiteConnectionMergeAllAsExpandoObjectViaTableNameForIdentityForEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

        // Setup
        var tables = Helper.CreateCompleteTablesAsExpandoObjects(10);

        // Act
        var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);
        Assert.IsTrue(tables.All(table => ((dynamic)table).Id > 0));

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(10, queryResult.Count());
        tables.ForEach(table => Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
    }

    [TestMethod]
    public void TestSQLiteConnectionMergeAllViaTableNameForIdentityForNonEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection).AsList();

        // Setup
        tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

        // Act
        var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestSQLiteConnectionMergeAllAsExpandoObjectViaTableNameForIdentityForNonEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        Database.CreateCompleteTables(10, connection).AsList();

        // Setup
        var tables = Helper.CreateCompleteTablesAsExpandoObjects(10);

        // Act
        var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);
        Assert.IsTrue(tables.All(table => ((dynamic)table).Id > 0));

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
    }

    [TestMethod]
    public void TestSQLiteConnectionMergeAllViaTableNameForIdentityForNonEmptyTableWithQualifiers()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection).AsList();
        var qualifiers = new[]
        {
                new Field("Id", typeof(long))
            };

        // Setup
        tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

        // Act
        var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
            tables,
            qualifiers);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestSQLiteConnectionMergeAllAsDynamicsViaTableNameForIdentityForEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

        // Setup
        var tables = Helper.CreateCompleteTablesAsDynamics(10);

        // Act
        var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestSQLiteConnectionMergeAllAsDynamicsViaTableNameForIdentityForNonEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection).AsList();
        tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

        // Act
        var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestSQLiteConnectionMergeAllAsDynamicsViaTableNameForIdentityForNonEmptyTableWithQualifiers()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection).AsList();
        tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));
        var qualifiers = new[]
        {
                new Field("Id", typeof(long))
            };

        // Act
        var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
            tables,
            qualifiers);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestSQLiteConnectionMergeAllViaTableNameForNonIdentityForEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

        // Setup
        var tables = Helper.CreateNonIdentityCompleteTables(10);

        // Act
        var result = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestSQLiteConnectionMergeAllViaTableNameForNonIdentityForNonEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateNonIdentityCompleteTables(10, connection).AsList();

        // Setup
        tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

        // Act
        var result = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.ElementAt(tables.IndexOf(table))));
    }

    [TestMethod]
    public void TestSQLiteConnectionMergeAllViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateNonIdentityCompleteTables(10, connection).AsList();
        var qualifiers = new[]
        {
                new Field("Id", typeof(string))
            };

        // Setup
        tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

        // Act
        var result = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables,
            qualifiers);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestSQLiteConnectionMergeAllAsDynamicsViaTableNameForNonIdentityForEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

        // Setup
        var tables = Helper.CreateNonIdentityCompleteTablesAsDynamics(10);

        // Act
        var result = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.ElementAt((int)tables.IndexOf(table))));
    }

    [TestMethod]
    public void TestSQLiteConnectionMergeAllAsDynamicsViaTableNameForNonIdentityForNonEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateNonIdentityCompleteTables(10, connection).AsList();
        tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

        // Act
        var result = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestSQLiteConnectionMergeAllAsDynamicsViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateNonIdentityCompleteTables(10, connection).AsList();
        var qualifiers = new[]
        {
                new Field("Id", typeof(string))
            };
        tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

        // Act
        var result = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables,
            qualifiers);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSQLiteConnectionMergeAllViaTableNameAsyncForIdentityForEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

        // Setup
        var tables = Helper.CreateCompleteTables(10);

        // Act
        var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(10, queryResult.Count());
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestSQLiteConnectionMergeAllAsyncAsExpandoObjectViaTableNameForIdentityForEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

        // Setup
        var tables = Helper.CreateCompleteTablesAsExpandoObjects(10);

        // Act
        var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);
        Assert.IsTrue(tables.All(table => ((dynamic)table).Id > 0));

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(10, queryResult.Count());
        tables.ForEach(table => Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
    }

    [TestMethod]
    public async Task TestSQLiteConnectionMergeAllViaTableNameAsyncForIdentityForNonEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection).AsList();

        // Setup
        tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

        // Act
        var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestSQLiteConnectionMergeAllAsyncAsExpandoObjectViaTableNameForIdentityForNonEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        Database.CreateCompleteTables(10, connection).AsList();

        // Setup
        var tables = Helper.CreateCompleteTablesAsExpandoObjects(10);

        // Act
        var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);
        Assert.IsTrue(tables.All(table => ((dynamic)table).Id > 0));

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
    }

    [TestMethod]
    public async Task TestSQLiteConnectionMergeAllViaTableNameAsyncForIdentityForNonEmptyTableWithQualifiers()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection).AsList();
        var qualifiers = new[]
        {
                new Field("Id", typeof(long))
            };

        // Setup
        tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

        // Act
        var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            tables,
            qualifiers, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestSQLiteConnectionMergeAllAsyncAsDynamicsViaTableNameForIdentityForEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

        // Setup
        var tables = Helper.CreateCompleteTablesAsDynamics(10);

        // Act
        var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestSQLiteConnectionMergeAllAsyncAsDynamicsViaTableNameForIdentityForNonEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection).AsList();
        tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

        // Act
        var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestSQLiteConnectionMergeAllAsyncAsDynamicsViaTableNameForIdentityForNonEmptyTableWithQualifiers()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection).AsList();
        var qualifiers = new[]
        {
                new Field("Id", typeof(long))
            };
        tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

        // Act
        var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            tables,
            qualifiers, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestSQLiteConnectionMergeAllAsyncViaTableNameForNonIdentityForEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

        // Setup
        var tables = Helper.CreateNonIdentityCompleteTables(10);

        // Act
        var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestSQLiteConnectionMergeAllAsyncViaTableNameForNonIdentityForNonEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateNonIdentityCompleteTables(10, connection).AsList();

        // Setup
        tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

        // Act
        var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestSQLiteConnectionMergeAllAsyncViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateNonIdentityCompleteTables(10, connection).AsList();
        var qualifiers = new[]
        {
                new Field("Id", typeof(string))
            };

        // Setup
        tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

        // Act
        var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables,
            qualifiers, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestSQLiteConnectionMergeAllAsyncAsDynamicsViaTableNameForNonIdentityForEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

        // Setup
        var tables = Helper.CreateNonIdentityCompleteTablesAsDynamics(10);

        // Act
        var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestSQLiteConnectionMergeAllAsyncAsDynamicsViaTableNameForNonIdentityForNonEmptyTable()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateNonIdentityCompleteTables(10, connection).AsList();
        tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

        // Act
        var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestSQLiteConnectionMergeAllAsyncAsDynamicsViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateNonIdentityCompleteTables(10, connection).AsList();
        var qualifiers = new[]
        {
                new Field("Id", typeof(string))
            };
        tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

        // Act
        var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables,
            qualifiers, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }
    #endregion

    #endregion
}
