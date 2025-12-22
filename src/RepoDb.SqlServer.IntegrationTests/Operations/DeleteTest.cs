using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.SqlServer.IntegrationTests.Models;
using RepoDb.SqlServer.IntegrationTests.Setup;
using RepoDb.Trace;

namespace RepoDb.SqlServer.IntegrationTests.Operations;

[TestClass]
public class DeleteTest
{
    [TestInitialize]
    public void Initialize()
    {
        Database.Initialize();
        Cleanup();
    }

    [TestCleanup]
    public void Cleanup()
    {
        Database.Cleanup();
    }

    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestSqlServerConnectionDeleteWithoutExpression()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.Delete<CompleteTable>((object?)null);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void TestSqlServerConnectionDeleteViaPrimaryKey()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.Delete<CompleteTable>(tables.First().Id);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestSqlServerConnectionDeleteViaDataEntity()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.Delete<CompleteTable>(tables.First());

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestSqlServerConnectionDeleteViaExpression()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.Delete<CompleteTable>(e => e.Id == tables.First().Id);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestSqlServerConnectionDeleteViaDynamic()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.Delete<CompleteTable>(new { Id = tables.First().Id });

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestSqlServerConnectionDeleteViaQueryField()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.Delete<CompleteTable>(new QueryField("Id", tables.First().Id));

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestSqlServerConnectionDeleteViaQueryFields()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);
        var queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.Delete<CompleteTable>(queryFields);

        // Assert
        Assert.AreEqual(8, result);
    }

    [TestMethod]
    public void TestSqlServerConnectionDeleteViaQueryGroup()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);
        var queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };
        var queryGroup = new QueryGroup(queryFields);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.Delete<CompleteTable>(queryGroup);

        // Assert
        Assert.AreEqual(8, result);
    }

    [TestMethod]
    public void TestSqlServerConnectionDeleteMultiKey()
    {
        // Setup
        var tables = Database.CreateMultiKeyTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        Assert.AreEqual(1, connection.Delete(tables.FirstOrDefault(), trace: new DiagnosticsTracer()));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqlServerConnectionDeleteAsyncWithoutExpression()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = await connection.DeleteAsync<CompleteTable>((object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task TestSqlServerConnectionDeleteAsyncViaPrimaryKey()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = await connection.DeleteAsync<CompleteTable>(tables.First().Id, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestSqlServerConnectionDeleteAsyncViaDataEntity()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = await connection.DeleteAsync<CompleteTable>(tables.First(), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestSqlServerConnectionDeleteAsyncViaExpression()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = await connection.DeleteAsync<CompleteTable>(e => e.Id == tables.First().Id, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestSqlServerConnectionDeleteAsyncViaDynamic()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = await connection.DeleteAsync<CompleteTable>(new { Id = tables.First().Id }, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestSqlServerConnectionDeleteAsyncViaQueryField()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = await connection.DeleteAsync<CompleteTable>(new QueryField("Id", tables.First().Id), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestSqlServerConnectionDeleteAsyncViaQueryFields()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);
        var queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = await connection.DeleteAsync<CompleteTable>(queryFields, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(8, result);
    }

    [TestMethod]
    public async Task TestSqlServerConnectionDeleteAsyncViaQueryGroup()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);
        var queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };
        var queryGroup = new QueryGroup(queryFields);

        using var connection = new SqlConnection(Database.ConnectionString);
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
    public void TestSqlServerConnectionDeleteViaTableNameWithoutExpression()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), (object?)null);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void TestSqlServerConnectionDeleteViaTableNameViaPrimaryKey()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.Delete<CompleteTable>(tables.First().Id);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestSqlServerConnectionDeleteViaTableNameViaDynamic()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), new { Id = tables.First().Id });

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestSqlServerConnectionDeleteViaTableNameViaQueryField()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", tables.First().Id));

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestSqlServerConnectionDeleteViaTableNameViaQueryFields()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);
        var queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), queryFields);

        // Assert
        Assert.AreEqual(8, result);
    }

    [TestMethod]
    public void TestSqlServerConnectionDeleteViaTableNameViaQueryGroup()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);
        var queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };
        var queryGroup = new QueryGroup(queryFields);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), queryGroup);

        // Assert
        Assert.AreEqual(8, result);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqlServerConnectionDeleteAsyncViaTableNameWithoutExpression()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), (object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task TestSqlServerConnectionDeleteAsyncViaTableNameViaPrimaryKey()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = await connection.DeleteAsync<CompleteTable>(tables.First().Id, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestSqlServerConnectionDeleteAsyncViaTableNameViaDynamic()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), new { Id = tables.First().Id }, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestSqlServerConnectionDeleteAsyncViaTableNameViaQueryField()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", tables.First().Id), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestSqlServerConnectionDeleteAsyncViaTableNameViaQueryFields()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);
        var queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), queryFields, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(8, result);
    }

    [TestMethod]
    public async Task TestSqlServerConnectionDeleteAsyncViaTableNameViaQueryGroup()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);
        var queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };
        var queryGroup = new QueryGroup(queryFields);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), queryGroup, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(8, result);
    }

    public TestContext TestContext { get; set; }

    #endregion

    #endregion
}
