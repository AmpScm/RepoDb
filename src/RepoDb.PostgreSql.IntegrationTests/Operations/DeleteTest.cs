using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Enumerations;
using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.PostgreSql.IntegrationTests.Operations;

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
    public void TestPostgreSqlConnectionDeleteWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.Delete<CompleteTable>((object?)null);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionDeleteViaPrimaryKey()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.Delete<CompleteTable>(tables.First().Id);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionDeleteViaDataEntity()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.Delete<CompleteTable>(tables.First());

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionDeleteViaExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.Delete<CompleteTable>(e => e.Id == tables.First().Id);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionDeleteViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.Delete<CompleteTable>(new { Id = tables.First().Id });

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionDeleteViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.Delete<CompleteTable>(new QueryField("Id", tables.First().Id));

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionDeleteViaQueryFields()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.Delete<CompleteTable>(queryFields);

        // Assert
        Assert.AreEqual(8, result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionDeleteViaQueryGroup()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };
        QueryGroup queryGroup = new QueryGroup(queryFields);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.Delete<CompleteTable>(queryGroup);

        // Assert
        Assert.AreEqual(8, result);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionDeleteAsyncWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.DeleteAsync<CompleteTable>((object?)null);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionDeleteAsyncViaPrimaryKey()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.DeleteAsync<CompleteTable>(tables.First().Id);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionDeleteAsyncViaDataEntity()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.DeleteAsync<CompleteTable>(tables.First());

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionDeleteAsyncViaExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.DeleteAsync<CompleteTable>(e => e.Id == tables.First().Id);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionDeleteAsyncViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.DeleteAsync<CompleteTable>(new { Id = tables.First().Id });

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionDeleteAsyncViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.DeleteAsync<CompleteTable>(new QueryField("Id", tables.First().Id));

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionDeleteAsyncViaQueryFields()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.DeleteAsync<CompleteTable>(queryFields);

        // Assert
        Assert.AreEqual(8, result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionDeleteAsyncViaQueryGroup()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };
        QueryGroup queryGroup = new QueryGroup(queryFields);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.DeleteAsync<CompleteTable>(queryGroup);

        // Assert
        Assert.AreEqual(8, result);
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestPostgreSqlConnectionDeleteViaTableNameWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), (object?)null);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionDeleteViaTableNameViaPrimaryKey()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.Delete<CompleteTable>(tables.First().Id);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionDeleteViaTableNameViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), new { Id = tables.First().Id });

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionDeleteViaTableNameViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", tables.First().Id));

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionDeleteViaTableNameViaQueryFields()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), queryFields);

        // Assert
        Assert.AreEqual(8, result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionDeleteViaTableNameViaQueryGroup()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };
        QueryGroup queryGroup = new QueryGroup(queryFields);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), queryGroup);

        // Assert
        Assert.AreEqual(8, result);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionDeleteAsyncViaTableNameWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), (object?)null);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionDeleteAsyncViaTableNameViaPrimaryKey()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.DeleteAsync<CompleteTable>(tables.First().Id);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionDeleteAsyncViaTableNameViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), new { Id = tables.First().Id });

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionDeleteAsyncViaTableNameViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", tables.First().Id));

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionDeleteAsyncViaTableNameViaQueryFields()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), queryFields);

        // Assert
        Assert.AreEqual(8, result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionDeleteAsyncViaTableNameViaQueryGroup()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };
        QueryGroup queryGroup = new QueryGroup(queryFields);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), queryGroup);

        // Assert
        Assert.AreEqual(8, result);
    }

    #endregion

    #endregion
}
