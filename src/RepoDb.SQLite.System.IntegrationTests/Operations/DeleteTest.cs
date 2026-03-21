using RepoDb.Enumerations;
using RepoDb.SQLite.System.IntegrationTests.Models;
using RepoDb.SQLite.System.IntegrationTests.Setup;
using System.Data.SQLite;

namespace RepoDb.SQLite.System.IntegrationTests.Operations.SDS;

[TestClass]
public class DeleteTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionDeleteWithoutExpression()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.Delete<CompleteTable>((object?)null);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void TestSqLiteConnectionDeleteViaPrimaryKey()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.Delete<CompleteTable>(tables.First().Id);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestSqLiteConnectionDeleteViaDataEntity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.Delete<CompleteTable>(tables.First());

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestSqLiteConnectionDeleteViaExpression()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.Delete<CompleteTable>(e => e.Id == tables.First().Id);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestSqLiteConnectionDeleteViaDynamic()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.Delete<CompleteTable>(new { Id = tables.First().Id });

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestSqLiteConnectionDeleteViaQueryField()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.Delete<CompleteTable>(new QueryField("Id", tables.First().Id));

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestSqLiteConnectionDeleteViaQueryFields()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        var queryFields = new[]
        {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

        // Act
        var result = connection.Delete<CompleteTable>(queryFields);

        // Assert
        Assert.AreEqual(8, result);
    }

    [TestMethod]
    public void TestSqLiteConnectionDeleteViaQueryGroup()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        var queryFields = new[]
        {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
        var queryGroup = new QueryGroup(queryFields);

        // Act
        var result = connection.Delete<CompleteTable>(queryGroup);

        // Assert
        Assert.AreEqual(8, result);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionDeleteAsyncWithoutExpression()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.DeleteAsync<CompleteTable>((object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionDeleteAsyncViaPrimaryKey()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.DeleteAsync<CompleteTable>(tables.First().Id, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionDeleteAsyncViaDataEntity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.DeleteAsync<CompleteTable>(tables.First(), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionDeleteAsyncViaExpression()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.DeleteAsync<CompleteTable>(e => e.Id == tables.First().Id, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionDeleteAsyncViaDynamic()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.DeleteAsync<CompleteTable>(new { Id = tables.First().Id }, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionDeleteAsyncViaQueryField()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.DeleteAsync<CompleteTable>(new QueryField("Id", tables.First().Id), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionDeleteAsyncViaQueryFields()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        var queryFields = new[]
        {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

        // Act
        var result = await connection.DeleteAsync<CompleteTable>(queryFields, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(8, result);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionDeleteAsyncViaQueryGroup()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        var queryFields = new[]
        {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
        var queryGroup = new QueryGroup(queryFields);

        // Act
        var result = await connection.DeleteAsync<CompleteTable>(queryGroup, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(8, result);
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionDeleteViaTableNameWithoutExpression()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), (object?)null);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void TestSqLiteConnectionDeleteViaTableNameViaPrimaryKey()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.Delete<CompleteTable>(tables.First().Id);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestSqLiteConnectionDeleteViaTableNameViaDynamic()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), new { Id = tables.First().Id });

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestSqLiteConnectionDeleteViaTableNameViaQueryField()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", tables.First().Id));

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestSqLiteConnectionDeleteViaTableNameViaQueryFields()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        var queryFields = new[]
        {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

        // Act
        var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), queryFields);

        // Assert
        Assert.AreEqual(8, result);
    }

    [TestMethod]
    public void TestSqLiteConnectionDeleteViaTableNameViaQueryGroup()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        var queryFields = new[]
        {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
        var queryGroup = new QueryGroup(queryFields);

        // Act
        var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), queryGroup);

        // Assert
        Assert.AreEqual(8, result);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionDeleteAsyncViaTableNameWithoutExpression()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), (object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionDeleteAsyncViaTableNameViaPrimaryKey()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.DeleteAsync<CompleteTable>(tables.First().Id, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionDeleteAsyncViaTableNameViaDynamic()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), new { Id = tables.First().Id }, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionDeleteAsyncViaTableNameViaQueryField()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", tables.First().Id), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionDeleteAsyncViaTableNameViaQueryFields()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        var queryFields = new[]
        {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

        // Act
        var result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), queryFields, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(8, result);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionDeleteAsyncViaTableNameViaQueryGroup()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        var queryFields = new[]
        {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
        var queryGroup = new QueryGroup(queryFields);

        // Act
        var result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), queryGroup, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(8, result);
    }
    #endregion

    #endregion
}
