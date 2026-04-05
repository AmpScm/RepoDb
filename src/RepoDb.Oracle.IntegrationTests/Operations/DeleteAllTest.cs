using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;
using RepoDb.Trace;

namespace RepoDb.Oracle.IntegrationTests.Operations;

[TestClass]
public class DeleteAllTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestOracleConnectionDeleteAll()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = connection.DeleteAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void TestOracleConnectionDeleteAllViaPrimaryKeys()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        IEnumerable<object> primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

        using var connection = CreateConnection();
        // Act
        int result = connection.DeleteAll<CompleteTable>(primaryKeys);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void TestOracleConnectionDeleteAllViaPrimaryKeysBeyondLimits()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(5000, fields: Field.Parse<CompleteTable>(x=>x.ColumnText));
        IEnumerable<object> primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

        using var connection = CreateConnection();
        // Act
        int result = connection.DeleteAll<CompleteTable>(primaryKeys);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestOracleConnectionDeleteAllAsync()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = await connection.DeleteAllAsync<CompleteTable>(cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task TestOracleConnectionDeleteAllAsyncViaPrimaryKeys()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        IEnumerable<object> primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

        using var connection = CreateConnection();
        // Act
        int result = await connection.DeleteAllAsync<CompleteTable>(primaryKeys, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task TestOracleConnectionDeleteAllAsyncViaPrimaryKeysBeyondLimits()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(5000, fields: Field.Parse<CompleteTable>(x => x.ColumnText));
        IEnumerable<object> primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

        using var connection = CreateConnection();
        // Act
        int result = await connection.DeleteAllAsync<CompleteTable>(primaryKeys, cancellationToken: TestContext.CancellationToken, trace: new DiagnosticsTracer());

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestOracleConnectionDeleteAllViaTableName()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = connection.DeleteAll(ClassMappedNameCache.Get<CompleteTable>());

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void TestOracleConnectionDeleteAllViaTableNameViaPrimaryKeys()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        IEnumerable<object> primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

        using var connection = CreateConnection();
        // Act
        int result = connection.DeleteAll(ClassMappedNameCache.Get<CompleteTable>(), primaryKeys);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void TestOracleConnectionDeleteAllViaTableNameViaPrimaryKeysBeyondLimits()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(5000, fields: Field.Parse<CompleteTable>(x => x.ColumnText));
        IEnumerable<object> primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

        using var connection = CreateConnection();
        // Act
        int result = connection.DeleteAll(ClassMappedNameCache.Get<CompleteTable>(), primaryKeys);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestOracleConnectionDeleteAllAsyncViaTableName()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = await connection.DeleteAllAsync(ClassMappedNameCache.Get<CompleteTable>(), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task TestOracleConnectionDeleteAllAsyncViaTableNameViaPrimaryKeys()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        IEnumerable<object> primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

        using var connection = CreateConnection();
        // Act
        int result = await connection.DeleteAllAsync(ClassMappedNameCache.Get<CompleteTable>(), primaryKeys, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task TestOracleConnectionDeleteAllAsyncViaTableNameViaPrimaryKeysBeyondLimits()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(5000, fields: Field.Parse<CompleteTable>(x => x.ColumnText));
        IEnumerable<object> primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

        using var connection = CreateConnection();
        // Act
        int result = await connection.DeleteAllAsync(ClassMappedNameCache.Get<CompleteTable>(), primaryKeys, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    #endregion

    #endregion
}
