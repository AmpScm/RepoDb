using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.PostgreSql.IntegrationTests.Operations;

[TestClass]
public class UpdateTest
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
    public void TestPostgreSqlConnectionUpdateViaDataEntity()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        int result = connection.Update<CompleteTable>(table);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        CompleteTable queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionUpdateViaExpression()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        int result = connection.Update<CompleteTable>(table, e => e.Id == table.Id);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        CompleteTable queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionUpdateViaDynamic()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        int result = connection.Update<CompleteTable>(table, new { table.Id });

        // Assert
        Assert.AreEqual(1, result);

        // Act
        CompleteTable queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionUpdateViaQueryField()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        int result = connection.Update<CompleteTable>(table, new QueryField("Id", table.Id));

        // Assert
        Assert.AreEqual(1, result);

        // Act
        CompleteTable queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionUpdateViaQueryFields()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInteger", table.ColumnInteger)
        };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        int result = connection.Update<CompleteTable>(table, queryFields);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        CompleteTable queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionUpdateViaQueryGroup()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInteger", table.ColumnInteger)
        };
        QueryGroup queryGroup = new QueryGroup(queryFields);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        int result = connection.Update<CompleteTable>(table, queryGroup);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        CompleteTable queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionUpdateAsyncViaDataEntity()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        int result = await connection.UpdateAsync<CompleteTable>(table);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        CompleteTable queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionUpdateAsyncViaExpression()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        int result = await connection.UpdateAsync<CompleteTable>(table, e => e.Id == table.Id);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        CompleteTable queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionUpdateAsyncViaDynamic()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        int result = await connection.UpdateAsync<CompleteTable>(table, new { table.Id });

        // Assert
        Assert.AreEqual(1, result);

        // Act
        CompleteTable queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionUpdateAsyncViaQueryField()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        int result = await connection.UpdateAsync<CompleteTable>(table, new QueryField("Id", table.Id));

        // Assert
        Assert.AreEqual(1, result);

        // Act
        CompleteTable queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionUpdateAsyncViaQueryFields()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInteger", table.ColumnInteger)
        };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        int result = await connection.UpdateAsync<CompleteTable>(table, queryFields);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        CompleteTable queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionUpdateAsyncViaQueryGroup()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInteger", table.ColumnInteger)
        };
        QueryGroup queryGroup = new QueryGroup(queryFields);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        int result = await connection.UpdateAsync<CompleteTable>(table, queryGroup);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        CompleteTable queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestPostgreSqlConnectionUpdateViaTableNameViaExpandoObject()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        System.Dynamic.ExpandoObject entity = Helper.CreateCompleteTablesAsExpandoObjects(1).First();
        ((IDictionary<string, object>)entity)["Id"] = table.Id;

        // Act
        int result = connection.Update(ClassMappedNameCache.Get<CompleteTable>(),
            entity);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        CompleteTable queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(queryResult, entity);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionUpdateViaTableNameViaDataEntity()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        int result = connection.Update(ClassMappedNameCache.Get<CompleteTable>(),
            table);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        CompleteTable queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionUpdateViaTableNameViaDynamic()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        int result = connection.Update(ClassMappedNameCache.Get<CompleteTable>(),
            table,
            new { table.Id });

        // Assert
        Assert.AreEqual(1, result);

        // Act
        CompleteTable queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionUpdateViaTableNameViaQueryField()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        int result = connection.Update(ClassMappedNameCache.Get<CompleteTable>(),
            table,
            new QueryField("Id", table.Id));

        // Assert
        Assert.AreEqual(1, result);

        // Act
        CompleteTable queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionUpdateViaTableNameViaQueryFields()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInteger", table.ColumnInteger)
        };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        int result = connection.Update(ClassMappedNameCache.Get<CompleteTable>(),
            table,
            queryFields);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        CompleteTable queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionUpdateViaTableNameViaQueryGroup()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInteger", table.ColumnInteger)
        };
        QueryGroup queryGroup = new QueryGroup(queryFields);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        int result = connection.Update(ClassMappedNameCache.Get<CompleteTable>(),
            table,
            queryGroup);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        CompleteTable queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionUpdateAsyncViaTableNameViaExpandoObject()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        System.Dynamic.ExpandoObject entity = Helper.CreateCompleteTablesAsExpandoObjects(1).First();
        ((IDictionary<string, object>)entity)["Id"] = table.Id;

        // Act
        int result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(),
            entity);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        CompleteTable queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(queryResult, entity);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionUpdateAsyncViaTableNameViaDataEntity()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        int result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        CompleteTable queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionUpdateAsyncViaTableNameViaDynamic()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        int result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, new { table.Id });

        // Assert
        Assert.AreEqual(1, result);

        // Act
        CompleteTable queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionUpdateAsyncViaTableNameViaQueryField()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        int result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, new QueryField("Id", table.Id));

        // Assert
        Assert.AreEqual(1, result);

        // Act
        CompleteTable queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionUpdateAsyncViaTableNameViaQueryFields()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInteger", table.ColumnInteger)
        };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        int result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, queryFields);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        CompleteTable queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionUpdateAsyncViaTableNameViaQueryGroup()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInteger", table.ColumnInteger)
        };
        QueryGroup queryGroup = new QueryGroup(queryFields);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        int result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, queryGroup);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        CompleteTable queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    #endregion

    #endregion
}
