using RepoDb.SQLite.System.IntegrationTests.Models;
using RepoDb.SQLite.System.IntegrationTests.Setup;
using System.Data.SQLite;

namespace RepoDb.SQLite.System.IntegrationTests.Operations.SDS;

[TestClass]
public class InsertTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestSQLiteConnectionInsertForIdentity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

        // Setup
        var table = Helper.CreateCompleteTables(1).First();

        // Act
        var result = connection.Insert<CompleteTable>(table);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.IsGreaterThan(0, Convert.ToInt64(result));
        Assert.IsGreaterThan(0, table.Id);

        // Act
        var queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Assert.AreEqual(1, queryResult?.Count());
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public void TestSQLiteConnectionInsertForNonIdentity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

        // Setup
        var table = Helper.CreateNonIdentityCompleteTables(1).First();

        // Act
        var result = connection.Insert<NonIdentityCompleteTable>(table);

        // Assert
        Assert.AreEqual(1, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(table.Id.ToString(), result?.ToString(), true);

        // Act
        var queryResult = connection.Query<NonIdentityCompleteTable>(result);

        // Assert
        Assert.AreEqual(1, queryResult?.Count());
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSQLiteConnectionInsertAsyncForIdentity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

        // Setup
        var table = Helper.CreateCompleteTables(1).First();

        // Act
        var result = await connection.InsertAsync<CompleteTable>(table, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.IsGreaterThan(0, Convert.ToInt64(result));
        Assert.IsGreaterThan(0, table.Id);

        // Act
        var queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Assert.AreEqual(1, queryResult?.Count());
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public async Task TestSQLiteConnectionInsertAsyncForNonIdentity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

        // Setup
        var table = Helper.CreateNonIdentityCompleteTables(1).First();

        // Act
        var result = await connection.InsertAsync<NonIdentityCompleteTable>(table, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(table.Id.ToString(), result?.ToString(), true);

        // Act
        var queryResult = connection.Query<NonIdentityCompleteTable>(result);

        // Assert
        Assert.AreEqual(1, queryResult?.Count());
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestSQLiteConnectionInsertViaTableNameForIdentity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

        // Setup
        var table = Helper.CreateCompleteTables(1).First();

        // Act
        var result = connection.Insert(ClassMappedNameCache.Get<CompleteTable>(),
            table);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.IsGreaterThan(0, Convert.ToInt64(result));

        // Act
        var queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Assert.AreEqual(1, queryResult?.Count());
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public void TestSQLiteConnectionInsertViaTableNameAsExpandoObjectForIdentity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

        // Setup
        var table = Helper.CreateCompleteTablesAsExpandoObjects(1).First();

        // Act
        var result = connection.Insert(ClassMappedNameCache.Get<CompleteTable>(),
            table);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.IsGreaterThan(0, Convert.ToInt64(result));
        Assert.AreEqual(((dynamic)table).Id, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Assert.AreEqual(1, queryResult?.Count());
        Helper.AssertMembersEquality(queryResult.First(), table);
    }

    [TestMethod]
    public void TestSQLiteConnectionInsertViaTableNameAsDynamicForIdentity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

        // Setup
        var table = Helper.CreateCompleteTablesAsDynamics(1).First();

        // Act
        var result = connection.Insert(ClassMappedNameCache.Get<CompleteTable>(),
            (object)table);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.IsGreaterThan(0, Convert.ToInt64(result));

        // Act
        var queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Assert.AreEqual(1, queryResult?.Count());
        Helper.AssertMembersEquality(queryResult.First(), table);
    }

    [TestMethod]
    public void TestSQLiteConnectionInsertViaTableNameForNonIdentity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

        // Setup
        var table = Helper.CreateNonIdentityCompleteTables(1).First();

        // Act
        var result = connection.Insert(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            table);

        // Assert
        Assert.AreEqual(1, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(table.Id.ToString(), result?.ToString(), true);

        // Act
        var queryResult = connection.Query<NonIdentityCompleteTable>(result);

        // Assert
        Assert.AreEqual(1, queryResult?.Count());
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public void TestSQLiteConnectionInsertViaTableNameAsExpandoObjectForNonIdentity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

        // Setup
        var table = Helper.CreateNonIdentityCompleteTablesAsExpandoObjects(1).First();

        // Act
        var result = connection.Insert(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            table);

        // Assert
        Assert.AreEqual(1, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(((dynamic)table).Id.ToString(), result?.ToString(), true);

        // Act
        var queryResult = connection.Query<NonIdentityCompleteTable>(result);

        // Assert
        Assert.AreEqual(1, queryResult?.Count());
        Helper.AssertMembersEquality(queryResult.First(), table);
    }

    [TestMethod]
    public void TestSQLiteConnectionInsertViaTableNameAsDynamicForNonIdentity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

        // Setup
        var table = Helper.CreateNonIdentityCompleteTablesAsDynamics(1).First();

        // Act
        var result = connection.Insert(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            (object)table);

        // Assert
        Assert.AreEqual(1, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(table.Id.ToString(), result?.ToString(), true);

        // Act
        var queryResult = connection.Query<NonIdentityCompleteTable>(result);

        // Assert
        Assert.AreEqual(1, queryResult?.Count());
        Helper.AssertMembersEquality(queryResult.First(), table);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSQLiteConnectionInsertViaTableNameAsyncForIdentity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

        // Setup
        var table = Helper.CreateCompleteTables(1).First();

        // Act
        var result = await connection.InsertAsync(ClassMappedNameCache.Get<CompleteTable>(),
            table, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.IsGreaterThan(0, Convert.ToInt64(result));

        // Act
        var queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Assert.AreEqual(1, queryResult?.Count());
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public async Task TestSQLiteConnectionInsertAsyncViaTableNameAsExpandoObjectForIdentity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

        // Setup
        var table = Helper.CreateCompleteTablesAsExpandoObjects(1).First();

        // Act
        var result = await connection.InsertAsync(ClassMappedNameCache.Get<CompleteTable>(),
            table, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.IsGreaterThan(0, Convert.ToInt64(result));
        Assert.AreEqual(((dynamic)table).Id, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Assert.AreEqual(1, queryResult?.Count());
        Helper.AssertMembersEquality(queryResult.First(), table);
    }

    [TestMethod]
    public async Task TestSQLiteConnectionInsertAsyncViaTableNameAsDynamicForIdentity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

        // Setup
        var table = Helper.CreateCompleteTablesAsDynamics(1).First();

        // Act
        var result = await connection.InsertAsync(ClassMappedNameCache.Get<CompleteTable>(),
            (object)table, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.IsGreaterThan(0, Convert.ToInt64(result));

        // Act
        var queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Assert.AreEqual(1, queryResult?.Count());
        Helper.AssertMembersEquality(queryResult.First(), table);
    }

    [TestMethod]
    public async Task TestSQLiteConnectionInsertViaTableNameAsyncForNonIdentity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

        // Setup
        var table = Helper.CreateNonIdentityCompleteTables(1).First();

        // Act
        var result = await connection.InsertAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            table, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(table.Id.ToString(), result?.ToString(), true);

        // Act
        var queryResult = connection.Query<NonIdentityCompleteTable>(result);

        // Assert
        Assert.AreEqual(1, queryResult?.Count());
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public async Task TestSQLiteConnectionInsertAsyncViaTableNameAsExpandoObjectForNonIdentity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

        // Setup
        var table = Helper.CreateNonIdentityCompleteTablesAsExpandoObjects(1).First();

        // Act
        var result = await connection.InsertAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            table, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(((dynamic)table).Id.ToString(), result?.ToString(), true);

        // Act
        var queryResult = connection.Query<NonIdentityCompleteTable>(result);

        // Assert
        Assert.AreEqual(1, queryResult?.Count());
        Helper.AssertMembersEquality(queryResult.First(), table);
    }

    [TestMethod]
    public async Task TestSQLiteConnectionInsertAsyncViaTableNameAsDynamicForNonIdentity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateSdsTables(connection);

        // Setup
        var table = Helper.CreateNonIdentityCompleteTablesAsDynamics(1).First();

        // Act
        var result = await connection.InsertAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            (object)table, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(table.Id.ToString(), result?.ToString(), true);

        // Act
        var queryResult = connection.Query<NonIdentityCompleteTable>(result);

        // Assert
        Assert.AreEqual(1, queryResult?.Count());
        Helper.AssertMembersEquality(queryResult.First(), table);
    }
    #endregion

    #endregion
}
