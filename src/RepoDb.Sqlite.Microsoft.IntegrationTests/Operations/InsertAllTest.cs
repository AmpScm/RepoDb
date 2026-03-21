using Microsoft.Data.Sqlite;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Models;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Setup;

namespace RepoDb.Sqlite.Microsoft.IntegrationTests.Operations.MDS;

[TestClass]
public class InsertAllTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionInsertAllForIdentity()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateMdsTables(connection);

        // Setup
        var tables = Helper.CreateCompleteTables(10);

        // Act
        var result = connection.InsertAll<CompleteTable>(tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);
        Assert.IsTrue(tables.All(table => table.Id > 0));

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.ForEach(table =>
        {
            Helper.AssertPropertiesEquality(table, queryResult.First(item => item.Id == table.Id));
        });
    }

    [TestMethod]
    public void TestSqLiteConnectionInsertAllForNonIdentity()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateMdsTables(connection);

        // Setup
        var tables = Helper.CreateNonIdentityCompleteTables(10);

        // Act
        var result = connection.InsertAll<NonIdentityCompleteTable>(tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        tables.ForEach(table =>
        {
            Helper.AssertPropertiesEquality(table, queryResult.First(item => item.Id == table.Id));
        });
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionInsertAllAsyncForIdentity()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateMdsTables(connection);

        // Setup
        var tables = Helper.CreateCompleteTables(10);

        // Act
        var result = await connection.InsertAllAsync<CompleteTable>(tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);
        Assert.IsTrue(tables.All(table => table.Id > 0));

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.ForEach(table =>
        {
            Helper.AssertPropertiesEquality(table, queryResult.First(item => item.Id == table.Id));
        });
    }

    [TestMethod]
    public async Task TestSqLiteConnectionInsertAllAsyncForNonIdentity()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateMdsTables(connection);

        // Setup
        var tables = Helper.CreateNonIdentityCompleteTables(10);

        // Act
        var result = await connection.InsertAllAsync<NonIdentityCompleteTable>(tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        tables.ForEach(table =>
        {
            Helper.AssertPropertiesEquality(table, queryResult.First(item => item.Id == table.Id));
        });
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionInsertAllViaTableNameForIdentity()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateMdsTables(connection);

        // Setup
        var tables = Helper.CreateCompleteTables(10);

        // Act
        var result = connection.InsertAll(ClassMappedNameCache.Get<CompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.ForEach(table =>
        {
            Helper.AssertMembersEquality(table, queryResult.ElementAt(tables.IndexOf(table)));
        });
    }

    [TestMethod]
    public void TestSqLiteConnectionInsertAllViaTableNameAsExpandoObjectForIdentity()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateMdsTables(connection);

        // Setup
        var tables = Helper.CreateCompleteTablesAsExpandoObjects(10);

        // Act
        var result = connection.InsertAll(ClassMappedNameCache.Get<CompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);
        Assert.IsTrue(tables.All(table => ((dynamic)table).Id > 0));

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.ForEach(table =>
        {
            Helper.AssertMembersEquality(queryResult.ElementAt(tables.IndexOf(table)), table);
        });
    }

    [TestMethod]
    public void TestSqLiteConnectionInsertAllViaTableNameAsDynamicsForIdentity()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateMdsTables(connection);

        // Setup
        var tables = Helper.CreateCompleteTablesAsDynamics(10);

        // Act
        var result = connection.InsertAll(ClassMappedNameCache.Get<CompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.ForEach(table =>
        {
            Helper.AssertMembersEquality(table, queryResult.ElementAt((int)tables.IndexOf(table)));
        });
    }

    [TestMethod]
    public void TestSqLiteConnectionInsertAllViaTableNameForNonIdentity()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateMdsTables(connection);

        // Setup
        var tables = Helper.CreateNonIdentityCompleteTables(10);

        // Act
        var result = connection.InsertAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        tables.ForEach(table =>
        {
            Helper.AssertMembersEquality(table, queryResult.ElementAt(tables.IndexOf(table)));
        });
    }

    [TestMethod]
    public void TestSqLiteConnectionInsertAllViaTableNameAsExpandoObjectForNonIdentity()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateMdsTables(connection);

        // Setup
        var tables = Helper.CreateNonIdentityCompleteTablesAsExpandoObjects(10);

        // Act
        var result = connection.InsertAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        tables.ForEach(table =>
        {
            Helper.AssertMembersEquality(queryResult.ElementAt(tables.IndexOf(table)), table);
        });
    }

    [TestMethod]
    public void TestSqLiteConnectionInsertAllViaTableNameAsDynamicsForNonIdentity()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateMdsTables(connection);

        // Setup
        var tables = Helper.CreateNonIdentityCompleteTablesAsDynamics(10);

        // Act
        var result = connection.InsertAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        tables.ForEach(table =>
        {
            Helper.AssertMembersEquality(table, queryResult.ElementAt((int)tables.IndexOf(table)));
        });
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionInsertAllViaTableNameAsyncForIdentity()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateMdsTables(connection);

        // Setup
        var tables = Helper.CreateCompleteTables(10);

        // Act
        var result = await connection.InsertAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.ForEach(table =>
        {
            Helper.AssertMembersEquality(table, queryResult.ElementAt(tables.IndexOf(table)));
        });
    }

    [TestMethod]
    public async Task TestSqLiteConnectionInsertAllViaTableNameAsyncAsExpandoObjectForIdentity()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateMdsTables(connection);

        // Setup
        var tables = Helper.CreateCompleteTablesAsExpandoObjects(10);

        // Act
        var result = await connection.InsertAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);
        Assert.IsTrue(tables.All(table => ((dynamic)table).Id > 0));

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.ForEach(table =>
        {
            Helper.AssertMembersEquality(queryResult.ElementAt(tables.IndexOf(table)), table);
        });
    }

    [TestMethod]
    public async Task TestSqLiteConnectionInsertAllAsyncViaTableNameAsDynamicsForIdentity()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateMdsTables(connection);

        // Setup
        var tables = Helper.CreateCompleteTablesAsDynamics(10);

        // Act
        var result = await connection.InsertAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.ForEach(table =>
        {
            Helper.AssertMembersEquality(table, queryResult.ElementAt((int)tables.IndexOf(table)));
        });
    }

    [TestMethod]
    public async Task TestSqLiteConnectionInsertAllViaTableNameAsyncForNonIdentity()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateMdsTables(connection);

        // Setup
        var tables = Helper.CreateNonIdentityCompleteTables(10);

        // Act
        var result = await connection.InsertAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        tables.ForEach(table =>
        {
            Helper.AssertMembersEquality(table, queryResult.ElementAt(tables.IndexOf(table)));
        });
    }

    [TestMethod]
    public async Task TestSqLiteConnectionInsertAllAsyncViaTableNameAsExpandoObjectForNonIdentity()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateMdsTables(connection);

        // Setup
        var tables = Helper.CreateNonIdentityCompleteTablesAsExpandoObjects(10);

        // Act
        var result = await connection.InsertAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        tables.ForEach(table =>
        {
            Helper.AssertMembersEquality(queryResult.ElementAt(tables.IndexOf(table)), table);
        });
    }

    [TestMethod]
    public async Task TestSqLiteConnectionInsertAllAsyncViaTableNameAsDynamicsForNonIdentity()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateMdsTables(connection);

        // Setup
        var tables = Helper.CreateNonIdentityCompleteTablesAsDynamics(10);

        // Act
        var result = await connection.InsertAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        tables.ForEach(table =>
        {
            Helper.AssertMembersEquality(table, queryResult.ElementAt((int)tables.IndexOf(table)));
        });
    }

    #endregion

    #endregion
}
