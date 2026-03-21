using Microsoft.Data.Sqlite;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Models;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Setup;

namespace RepoDb.Sqlite.Microsoft.IntegrationTests.Operations.MDS;

[TestClass]
public class InsertTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionInsertForIdentity()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateMdsTables(connection);

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
    public void TestSqLiteConnectionInsertForNonIdentity()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateMdsTables(connection);

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
    public async Task TestSqLiteConnectionInsertAsyncForIdentity()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateMdsTables(connection);

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
    public async Task TestSqLiteConnectionInsertAsyncForNonIdentity()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateMdsTables(connection);

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
    public void TestSqLiteConnectionInsertViaTableNameForIdentity()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateMdsTables(connection);

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
    public void TestSqliteConnectionInsertViaTableNameAsExpandoObjectForIdentity()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateMdsTables(connection);

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
    public void TestSqLiteConnectionInsertViaTableNameAsDynamicForIdentity()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateMdsTables(connection);

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
    public void TestSqLiteConnectionInsertViaTableNameForNonIdentity()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateMdsTables(connection);

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
    public void TestSqliteConnectionInsertViaTableNameAsExpandoObjectForNonIdentity()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateMdsTables(connection);

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
    public void TestSqLiteConnectionInsertViaTableNameAsDynamicForNonIdentity()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateMdsTables(connection);

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
    public async Task TestSqLiteConnectionInsertViaTableNameAsyncForIdentity()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateMdsTables(connection);

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
    public async Task TestSqLiteConnectionInsertAsyncViaTableNameAsExpandoObjectForIdentity()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateMdsTables(connection);

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
    public async Task TestSqLiteConnectionInsertAsyncViaTableNameAsDynamicForIdentity()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateMdsTables(connection);

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
    public async Task TestSqLiteConnectionInsertViaTableNameAsyncForNonIdentity()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateMdsTables(connection);

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
    public async Task TestSqLiteConnectionInsertAsyncViaTableNameAsExpandoObjectForNonIdentity()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateMdsTables(connection);

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
    public async Task TestSqLiteConnectionInsertAsyncViaTableNameAsDynamicForNonIdentity()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Create the tables
        Database.CreateMdsTables(connection);

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
