using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.PostgreSql.IntegrationTests.Operations;

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
    public void TestPostgreSqlConnectionDeleteAll()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.DeleteAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionDeleteAllViaPrimaryKeys()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        IEnumerable<object> primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.DeleteAll<CompleteTable>(primaryKeys);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionDeleteAllViaPrimaryKeysBeyondLimits()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(5000);
        IEnumerable<object> primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.DeleteAll<CompleteTable>(primaryKeys);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionDeleteAllAsync()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.DeleteAllAsync<CompleteTable>(cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionDeleteAllAsyncViaPrimaryKeys()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        IEnumerable<object> primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.DeleteAllAsync<CompleteTable>(primaryKeys, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionDeleteAllAsyncViaPrimaryKeysBeyondLimits()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(5000);
        IEnumerable<object> primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.DeleteAllAsync<CompleteTable>(primaryKeys, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestPostgreSqlConnectionDeleteAllViaTableName()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.DeleteAll(ClassMappedNameCache.Get<CompleteTable>());

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionDeleteAllViaTableNameViaPrimaryKeys()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        IEnumerable<object> primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.DeleteAll(ClassMappedNameCache.Get<CompleteTable>(), primaryKeys);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionDeleteAllViaTableNameViaPrimaryKeysBeyondLimits()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(5000);
        IEnumerable<object> primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.DeleteAll(ClassMappedNameCache.Get<CompleteTable>(), primaryKeys);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionDeleteAllAsyncViaTableName()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.DeleteAllAsync(ClassMappedNameCache.Get<CompleteTable>(), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionDeleteAllAsyncViaTableNameViaPrimaryKeys()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        IEnumerable<object> primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.DeleteAllAsync(ClassMappedNameCache.Get<CompleteTable>(), primaryKeys, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionDeleteAllAsyncViaTableNameViaPrimaryKeysBeyondLimits()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(5000);
        IEnumerable<object> primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.DeleteAllAsync(ClassMappedNameCache.Get<CompleteTable>(), primaryKeys, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    public TestContext TestContext { get; set; }

    #endregion

    #endregion
}
