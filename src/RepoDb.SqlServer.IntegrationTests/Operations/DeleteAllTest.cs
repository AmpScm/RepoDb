using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SqlServer.IntegrationTests.Models;
using RepoDb.SqlServer.IntegrationTests.Setup;
using RepoDb.Trace;

namespace RepoDb.SqlServer.IntegrationTests.Operations;

[TestClass]
public class DeleteAllTest
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
    public void TestSqlServerConnectionDeleteAll()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.DeleteAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void TestSqlServerConnectionDeleteAllViaPrimaryKeys()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);
        var primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

        using var connection = new SqlConnection(Database.ConnectionString).EnsureOpen();
        // Act
        var result = connection.DeleteAll<CompleteTable>(primaryKeys);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void TestSqlServerConnectionDeleteAllViaPrimaryKeysBeyondLimits()
    {
        // Setup
        var tables = Database.CreateCompleteTables(5000);
        var primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

        using var connection = new SqlConnection(Database.ConnectionString).EnsureOpen();
        // Act
        var result = connection.DeleteAll<CompleteTable>(primaryKeys, trace: new DiagnosticsTracer());

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void TestSqlServerConnectionDeleteAllValues()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.DeleteAll<CompleteTable>(tables.Take(5), trace: new DiagnosticsTracer());

        // Assert
        Assert.AreEqual(5, result);
    }

    [TestMethod]
    public void TestSqlServerConnectionDeleteAllValues10()
    {
        // Setup
        var tables = Database.CreateCompleteTables(15);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.DeleteAll<CompleteTable>(tables.Take(10), trace: new DiagnosticsTracer());

        // Assert
        Assert.AreEqual(10, result);
    }

    [TestMethod]
    public void TestSqlServerConnectionDeleteAllValues20()
    {
        // Setup
        var tables = Database.CreateCompleteTables(30);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.DeleteAll<CompleteTable>(tables.Take(20), trace: new DiagnosticsTracer());

        // Assert
        Assert.AreEqual(20, result);
    }

    [TestMethod]
    public void TestSqlServerConnectionDeleteAllMultiKey()
    {
        // Setup
        var tables = Database.CreateMultiKeyTables(10);

        using var connection = new SqlConnection(Database.ConnectionString).EnsureOpen();
        Assert.AreEqual(5, connection.DeleteAll(tables.Take(5), trace: new DiagnosticsTracer()));
    }


    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqlServerConnectionDeleteAllAsync()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = await connection.DeleteAllAsync<CompleteTable>(cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task TestSqlServerConnectionDeleteAllAsyncViaPrimaryKeys()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);
        var primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

        using var connection = new SqlConnection(Database.ConnectionString).EnsureOpen();
        // Act
        var result = await connection.DeleteAllAsync<CompleteTable>(primaryKeys, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task TestSqlServerConnectionDeleteAllAsyncViaPrimaryKeysBeyondLimits()
    {
        // Setup
        var tables = Database.CreateCompleteTables(5000);
        var primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

        using var connection = new SqlConnection(Database.ConnectionString).EnsureOpen();
        // Act
        var result = await connection.DeleteAllAsync<CompleteTable>(primaryKeys, trace: new DiagnosticsTracer(), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestSqlServerConnectionDeleteAllViaTableName()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.DeleteAll(ClassMappedNameCache.Get<CompleteTable>());

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void TestSqlServerConnectionDeleteAllViaTableNameViaPrimaryKeys()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);
        var primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

        using var connection = new SqlConnection(Database.ConnectionString).EnsureOpen();
        // Act
        var result = connection.DeleteAll(ClassMappedNameCache.Get<CompleteTable>(), primaryKeys);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void TestSqlServerConnectionDeleteAllViaTableNameViaPrimaryKeysBeyondLimits()
    {
        // Setup
        var tables = Database.CreateCompleteTables(5000);
        var primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

        using var connection = new SqlConnection(Database.ConnectionString).EnsureOpen();
        // Act
        var result = connection.DeleteAll(ClassMappedNameCache.Get<CompleteTable>(), primaryKeys);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqlServerConnectionDeleteAllAsyncViaTableName()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = await connection.DeleteAllAsync(ClassMappedNameCache.Get<CompleteTable>(), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task TestSqlServerConnectionDeleteAllAsyncViaTableNameViaPrimaryKeys()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);
        var primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

        using var connection = new SqlConnection(Database.ConnectionString).EnsureOpen();
        // Act
        var result = await connection.DeleteAllAsync(ClassMappedNameCache.Get<CompleteTable>(), primaryKeys, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task TestSqlServerConnectionDeleteAllAsyncViaTableNameViaPrimaryKeysBeyondLimits()
    {
        // Setup
        var tables = Database.CreateCompleteTables(5000);
        var primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

        using var connection = new SqlConnection(Database.ConnectionString).EnsureOpen();
        // Act
        var result = await connection.DeleteAllAsync(ClassMappedNameCache.Get<CompleteTable>(), primaryKeys, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    public TestContext TestContext { get; set; }

    #endregion

    #endregion
}
