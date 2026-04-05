using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;

namespace RepoDb.Oracle.IntegrationTests.Operations;

[TestClass]
public class UpdateTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestOracleConnectionUpdateViaDataEntity()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
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
    public void TestOracleConnectionUpdateViaExpression()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
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
    public void TestOracleConnectionUpdateViaDynamic()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
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
    public void TestOracleConnectionUpdateViaQueryField()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
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
    public void TestOracleConnectionUpdateViaQueryFields()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInt", table.ColumnInt)
        };

        using var connection = CreateConnection();
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
    public void TestOracleConnectionUpdateViaQueryGroup()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInt", table.ColumnInt)
        };
        QueryGroup queryGroup = new QueryGroup(queryFields);

        using var connection = CreateConnection();
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
    public async Task TestOracleConnectionUpdateAsyncViaDataEntity()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        int result = await connection.UpdateAsync<CompleteTable>(table, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        CompleteTable queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public async Task TestOracleConnectionUpdateAsyncViaExpression()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        int result = await connection.UpdateAsync<CompleteTable>(table, e => e.Id == table.Id, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        CompleteTable queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public async Task TestOracleConnectionUpdateAsyncViaDynamic()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        int result = await connection.UpdateAsync<CompleteTable>(table, new { table.Id }, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        CompleteTable queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public async Task TestOracleConnectionUpdateAsyncViaQueryField()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        int result = await connection.UpdateAsync<CompleteTable>(table, new QueryField("Id", table.Id), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        CompleteTable queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public async Task TestOracleConnectionUpdateAsyncViaQueryFields()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInt", table.ColumnInt)
        };

        using var connection = CreateConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        int result = await connection.UpdateAsync<CompleteTable>(table, queryFields, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        CompleteTable queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public async Task TestOracleConnectionUpdateAsyncViaQueryGroup()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInt", table.ColumnInt)
        };
        QueryGroup queryGroup = new QueryGroup(queryFields);

        using var connection = CreateConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        int result = await connection.UpdateAsync<CompleteTable>(table, queryGroup, cancellationToken: TestContext.CancellationToken);

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
    public void TestOracleConnectionUpdateViaTableNameViaExpandoObject()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Setup
        System.Dynamic.ExpandoObject entity = Helper.CreateCompleteTablesAsExpandoObjects(1).First();
        ((IDictionary<string, object?>)entity)["Id"] = table.Id;

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
    public void TestOracleConnectionUpdateViaTableNameViaDataEntity()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
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
    public void TestOracleConnectionUpdateViaTableNameViaDynamic()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
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
    public void TestOracleConnectionUpdateViaTableNameViaQueryField()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
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
    public void TestOracleConnectionUpdateViaTableNameViaQueryFields()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInt", table.ColumnInt)
        };

        using var connection = CreateConnection();
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
    public void TestOracleConnectionUpdateViaTableNameViaQueryGroup()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInt", table.ColumnInt)
        };
        QueryGroup queryGroup = new QueryGroup(queryFields);

        using var connection = CreateConnection();
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
    public async Task TestOracleConnectionUpdateAsyncViaTableNameViaExpandoObject()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Setup
        System.Dynamic.ExpandoObject entity = Helper.CreateCompleteTablesAsExpandoObjects(1).First();
        ((IDictionary<string, object?>)entity)["Id"] = table.Id;

        // Act
        int result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(),
            entity, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        CompleteTable queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(queryResult, entity);
    }

    [TestMethod]
    public async Task TestOracleConnectionUpdateAsyncViaTableNameViaDataEntity()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        int result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        CompleteTable queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public async Task TestOracleConnectionUpdateAsyncViaTableNameViaDynamic()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        int result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, new { table.Id }, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        CompleteTable queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public async Task TestOracleConnectionUpdateAsyncViaTableNameViaQueryField()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        int result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, new QueryField("Id", table.Id), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        CompleteTable queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public async Task TestOracleConnectionUpdateAsyncViaTableNameViaQueryFields()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInt", table.ColumnInt)
        };

        using var connection = CreateConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        int result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, queryFields, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        CompleteTable queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public async Task TestOracleConnectionUpdateAsyncViaTableNameViaQueryGroup()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInt", table.ColumnInt)
        };
        QueryGroup queryGroup = new QueryGroup(queryFields);

        using var connection = CreateConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        int result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, queryGroup, cancellationToken: TestContext.CancellationToken);

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
