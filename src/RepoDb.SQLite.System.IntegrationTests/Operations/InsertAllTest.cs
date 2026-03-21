using RepoDb.SQLite.System.IntegrationTests.Models;
using RepoDb.SQLite.System.IntegrationTests.Setup;
using System.Data.SQLite;

namespace RepoDb.SQLite.System.IntegrationTests.Operations.SDS;

[TestClass]
public class InsertAllTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestSQLiteConnectionInsertAllForIdentity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

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
    public void TestSQLiteConnectionInsertAllForNonIdentity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

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
    public async Task TestSQLiteConnectionInsertAllAsyncForIdentity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

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
    public async Task TestSQLiteConnectionInsertAllAsyncForNonIdentity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

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
    public void TestSQLiteConnectionInsertAllViaTableNameForIdentity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

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
    public void TestSQLiteConnectionInsertAllViaTableNameAsExpandoObjectForIdentity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

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
    public void TestSQLiteConnectionInsertAllViaTableNameAsDynamicsForIdentity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

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
    public void TestSQLiteConnectionInsertAllViaTableNameForNonIdentity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

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
    public void TestSQLiteConnectionInsertAllViaTableNameAsExpandoObjectForNonIdentity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

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
    public void TestSQLiteConnectionInsertAllViaTableNameAsDynamicsForNonIdentity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

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
    public async Task TestSQLiteConnectionInsertAllViaTableNameAsyncForIdentity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

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
    public async Task TestSQLiteConnectionInsertAllViaTableNameAsyncAsExpandoObjectForIdentity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

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
    public async Task TestSQLiteConnectionInsertAllAsyncViaTableNameAsDynamicsForIdentity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

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
    public async Task TestSQLiteConnectionInsertAllViaTableNameAsyncForNonIdentity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

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
    public async Task TestSQLiteConnectionInsertAllAsyncViaTableNameAsExpandoObjectForNonIdentity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

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
    public async Task TestSQLiteConnectionInsertAllAsyncViaTableNameAsDynamicsForNonIdentity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

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
