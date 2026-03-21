using Microsoft.Data.Sqlite;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Models;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Setup;

namespace RepoDb.Sqlite.Microsoft.IntegrationTests.Operations.MDS;

[TestClass]
public class DeleteAllTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionDeleteAll()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.DeleteAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void TestSqLiteConnectionDeleteAllViaPrimaryKeys()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        var primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

        // Act
        var result = connection.DeleteAll<CompleteTable>(primaryKeys);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    //[TestMethod]
    //public void TestSqLiteConnectionDeleteAllViaPrimaryKeysBeyondLimits()
    //{
    //    using (var connection = new SqliteConnection(Database.ConnectionString))
    //    {
    //        // Setup
    //        var tables = Database.CreateCompleteTables(1000, connection);
    //        var primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

    //        // Act
    //        var result = connection.DeleteAll<CompleteTable>(primaryKeys);

    //        // Assert
    //        Assert.AreEqual(tables.Count(), result);
    //    }
    //}

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionDeleteAllAsync()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.DeleteAllAsync<CompleteTable>(cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionDeleteAllAsyncViaPrimaryKeys()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        var primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

        // Act
        var result = await connection.DeleteAllAsync<CompleteTable>(primaryKeys, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    //[TestMethod]
    //public async Task TestSqLiteConnectionDeleteAllAsyncViaPrimaryKeysBeyondLimits()
    //{
    //    using (var connection = new SqliteConnection(Database.ConnectionString))
    //    {
    //        // Setup
    //        var tables = Database.CreateCompleteTables(1000, connection);
    //        var primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

    //        // Act
    //        var result = connection.DeleteAllAsync<CompleteTable>(primaryKeys);

    //        // Assert
    //        Assert.AreEqual(tables.Count(), result);
    //    }
    //}

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionDeleteAllViaTableName()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.DeleteAll(ClassMappedNameCache.Get<CompleteTable>());

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void TestSqLiteConnectionDeleteAllViaTableNameViaPrimaryKeys()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        var primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

        // Act
        var result = connection.DeleteAll(ClassMappedNameCache.Get<CompleteTable>(), primaryKeys);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    //[TestMethod]
    //public void TestSqLiteConnectionDeleteAllViaTableNameViaPrimaryKeysBeyondLimits()
    //{
    //    using (var connection = new SqliteConnection(Database.ConnectionString))
    //    {
    //        // Setup
    //        var tables = Database.CreateCompleteTables(1000, connection);
    //        var primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

    //        // Act
    //        var result = connection.DeleteAll(ClassMappedNameCache.Get<CompleteTable>(), primaryKeys);

    //        // Assert
    //        Assert.AreEqual(tables.Count(), result);
    //    }
    //}

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionDeleteAllAsyncViaTableName()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.DeleteAllAsync(ClassMappedNameCache.Get<CompleteTable>(), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionDeleteAllAsyncViaTableNameViaPrimaryKeys()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        var primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

        // Act
        var result = await connection.DeleteAllAsync(ClassMappedNameCache.Get<CompleteTable>(), primaryKeys, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    //[TestMethod]
    //public async Task TestSqLiteConnectionDeleteAllAsyncViaTableNameViaPrimaryKeysBeyondLimits()
    //{
    //    using (var connection = new SqliteConnection(Database.ConnectionString))
    //    {
    //        // Setup
    //        var tables = Database.CreateCompleteTables(1000, connection);
    //        var primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

    //        // Act
    //        var result = connection.DeleteAllAsync(ClassMappedNameCache.Get<CompleteTable>(), primaryKeys);

    //        // Assert
    //        Assert.AreEqual(tables.Count(), result);
    //    }
    //}

    #endregion

    #endregion
}
