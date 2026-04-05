using RepoDb.Enumerations;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;

namespace RepoDb.Oracle.IntegrationTests.Operations;

[TestClass]
public class DeleteTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestOracleConnectionDeleteWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = connection.Delete<CompleteTable>((object?)null);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void TestOracleConnectionDeleteViaPrimaryKey()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = connection.Delete<CompleteTable>(tables.First().Id);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestOracleConnectionDeleteViaDataEntity()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = connection.Delete<CompleteTable>(tables.First());

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestOracleConnectionDeleteViaExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = connection.Delete<CompleteTable>(e => e.Id == tables.First().Id);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestOracleConnectionDeleteViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = connection.Delete<CompleteTable>(new { Id = tables.First().Id });

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestOracleConnectionDeleteViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = connection.Delete<CompleteTable>(new QueryField("Id", tables.First().Id));

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestOracleConnectionDeleteViaQueryFields()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };

        using var connection = CreateConnection();
        // Act
        int result = connection.Delete<CompleteTable>(queryFields);

        // Assert
        Assert.AreEqual(8, result);
    }

    [TestMethod]
    public void TestOracleConnectionDeleteViaQueryGroup()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };
        QueryGroup queryGroup = new QueryGroup(queryFields);

        using var connection = CreateConnection();
        // Act
        int result = connection.Delete<CompleteTable>(queryGroup);

        // Assert
        Assert.AreEqual(8, result);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestOracleConnectionDeleteAsyncWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = await connection.DeleteAsync<CompleteTable>((object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task TestOracleConnectionDeleteAsyncViaPrimaryKey()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = await connection.DeleteAsync<CompleteTable>(tables.First().Id, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestOracleConnectionDeleteAsyncViaDataEntity()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = await connection.DeleteAsync<CompleteTable>(tables.First(), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestOracleConnectionDeleteAsyncViaExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = await connection.DeleteAsync<CompleteTable>(e => e.Id == tables.First().Id, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestOracleConnectionDeleteAsyncViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = await connection.DeleteAsync<CompleteTable>(new { Id = tables.First().Id }, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestOracleConnectionDeleteAsyncViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = await connection.DeleteAsync<CompleteTable>(new QueryField("Id", tables.First().Id), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestOracleConnectionDeleteAsyncViaQueryFields()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };

        using var connection = CreateConnection();
        // Act
        int result = await connection.DeleteAsync<CompleteTable>(queryFields, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(8, result);
    }

    [TestMethod]
    public async Task TestOracleConnectionDeleteAsyncViaQueryGroup()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };
        QueryGroup queryGroup = new QueryGroup(queryFields);

        using var connection = CreateConnection();
        // Act
        int result = await connection.DeleteAsync<CompleteTable>(queryGroup, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(8, result);
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestOracleConnectionDeleteViaTableNameWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), (object?)null);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void TestOracleConnectionDeleteViaTableNameViaPrimaryKey()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = connection.Delete<CompleteTable>(tables.First().Id);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestOracleConnectionDeleteViaTableNameViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), new { Id = tables.First().Id });

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestOracleConnectionDeleteViaTableNameViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", tables.First().Id));

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestOracleConnectionDeleteViaTableNameViaQueryFields()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };

        using var connection = CreateConnection();
        // Act
        int result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), queryFields);

        // Assert
        Assert.AreEqual(8, result);
    }

    [TestMethod]
    public void TestOracleConnectionDeleteViaTableNameViaQueryGroup()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };
        QueryGroup queryGroup = new QueryGroup(queryFields);

        using var connection = CreateConnection();
        // Act
        int result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), queryGroup);

        // Assert
        Assert.AreEqual(8, result);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestOracleConnectionDeleteAsyncViaTableNameWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), (object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task TestOracleConnectionDeleteAsyncViaTableNameViaPrimaryKey()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = await connection.DeleteAsync<CompleteTable>(tables.First().Id, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestOracleConnectionDeleteAsyncViaTableNameViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), new { Id = tables.First().Id }, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestOracleConnectionDeleteAsyncViaTableNameViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", tables.First().Id), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestOracleConnectionDeleteAsyncViaTableNameViaQueryFields()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };

        using var connection = CreateConnection();
        // Act
        int result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), queryFields, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(8, result);
    }

    [TestMethod]
    public async Task TestOracleConnectionDeleteAsyncViaTableNameViaQueryGroup()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };
        QueryGroup queryGroup = new QueryGroup(queryFields);

        using var connection = CreateConnection();
        // Act
        int result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), queryGroup, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(8, result);
    }

    #endregion

    #endregion
}
