using System.Data.SQLite;
using RepoDb.SQLite.System.IntegrationTests.Models;
using RepoDb.SQLite.System.IntegrationTests.Setup;

namespace RepoDb.SQLite.System.IntegrationTests.Operations.SDS;

[TestClass]
public class UpdateTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionUpdateViaDataEntity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();
        Helper.UpdateCompleteTableProperties(table);

        // Act
        var result = connection.Update<CompleteTable>(table);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public void TestSqLiteConnectionUpdateViaExpression()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();
        Helper.UpdateCompleteTableProperties(table);

        // Act
        var result = connection.Update<CompleteTable>(table, e => e.Id == table.Id);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public void TestSqLiteConnectionUpdateViaDynamic()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();
        Helper.UpdateCompleteTableProperties(table);

        // Act
        var result = connection.Update<CompleteTable>(table, new { table.Id });

        // Assert
        Assert.AreEqual(1, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public void TestSqLiteConnectionUpdateViaQueryField()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();
        Helper.UpdateCompleteTableProperties(table);

        // Act
        var result = connection.Update<CompleteTable>(table, new QueryField("Id", table.Id));

        // Assert
        Assert.AreEqual(1, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public void TestSqLiteConnectionUpdateViaQueryFields()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();
        var queryFields = new[]
        {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
        Helper.UpdateCompleteTableProperties(table);

        // Act
        var result = connection.Update<CompleteTable>(table, queryFields);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public void TestSqLiteConnectionUpdateViaQueryGroup()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();
        var queryFields = new[]
        {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
        var queryGroup = new QueryGroup(queryFields);
        Helper.UpdateCompleteTableProperties(table);

        // Act
        var result = connection.Update<CompleteTable>(table, queryGroup);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionUpdateAsyncViaDataEntity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();
        Helper.UpdateCompleteTableProperties(table);

        // Act
        var result = await connection.UpdateAsync<CompleteTable>(table, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionUpdateAsyncViaExpression()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();
        Helper.UpdateCompleteTableProperties(table);

        // Act
        var result = await connection.UpdateAsync<CompleteTable>(table, e => e.Id == table.Id, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionUpdateAsyncViaDynamic()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();
        Helper.UpdateCompleteTableProperties(table);

        // Act
        var result = await connection.UpdateAsync<CompleteTable>(table, new { table.Id }, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionUpdateAsyncViaQueryField()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();
        Helper.UpdateCompleteTableProperties(table);

        // Act
        var result = await connection.UpdateAsync<CompleteTable>(table, new QueryField("Id", table.Id), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionUpdateAsyncViaQueryFields()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();
        var queryFields = new[]
        {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
        Helper.UpdateCompleteTableProperties(table);

        // Act
        var result = await connection.UpdateAsync<CompleteTable>(table, queryFields, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionUpdateAsyncViaQueryGroup()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();
        var queryFields = new[]
        {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
        var queryGroup = new QueryGroup(queryFields);
        Helper.UpdateCompleteTableProperties(table);

        // Act
        var result = await connection.UpdateAsync<CompleteTable>(table, queryGroup, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionUpdateViaTableNameAsExpandoObjectViaDataEntity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        Database.CreateCompleteTables(1, connection);
        var table = Helper.CreateCompleteTablesAsExpandoObjects(1).First();

        // Act
        var result = DbConnectionExtension.Update(connection, ClassMappedNameCache.Get<CompleteTable>(),
            table);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        var queryResult = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), result).First();

        // Assert
        Helper.AssertMembersEquality(queryResult, table);
    }

    [TestMethod]
    public void TestSqLiteConnectionUpdateViaTableNameViaDataEntity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();
        Helper.UpdateCompleteTableProperties(table);

        // Act
        var result = DbConnectionExtension.Update(connection, ClassMappedNameCache.Get<CompleteTable>(), table);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        var queryResult = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public void TestSqLiteConnectionUpdateViaTableNameViaDynamic()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();
        Helper.UpdateCompleteTableProperties(table);

        // Act
        var result = DbConnectionExtension.Update(connection, ClassMappedNameCache.Get<CompleteTable>(), table, new { table.Id });

        // Assert
        Assert.AreEqual(1, result);

        // Act
        var queryResult = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public void TestSqLiteConnectionUpdateViaTableNameViaQueryField()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();
        Helper.UpdateCompleteTableProperties(table);

        // Act
        var result = DbConnectionExtension.Update(connection, ClassMappedNameCache.Get<CompleteTable>(), table, new QueryField("Id", table.Id));

        // Assert
        Assert.AreEqual(1, result);

        // Act
        var queryResult = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public void TestSqLiteConnectionUpdateViaTableNameViaQueryFields()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();
        var queryFields = new[]
        {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
        Helper.UpdateCompleteTableProperties(table);

        // Act
        var result = DbConnectionExtension.Update(connection, ClassMappedNameCache.Get<CompleteTable>(), table, queryFields);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        var queryResult = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public void TestSqLiteConnectionUpdateViaTableNameViaQueryGroup()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();
        var queryFields = new[]
        {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
        var queryGroup = new QueryGroup(queryFields);
        Helper.UpdateCompleteTableProperties(table);

        // Act
        var result = DbConnectionExtension.Update(connection, ClassMappedNameCache.Get<CompleteTable>(), table, queryGroup);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        var queryResult = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionUpdateAsyncViaTableNameAsExpandoObjectViaDataEntity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        Database.CreateCompleteTables(1, connection);
        var table = Helper.CreateCompleteTablesAsExpandoObjects(1).First();

        // Act
        var result = await DbConnectionExtension.UpdateAsync(connection, ClassMappedNameCache.Get<CompleteTable>(),
            table, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        var queryResult = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), result).First();

        // Assert
        Helper.AssertMembersEquality(queryResult, table);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionUpdateAsyncViaTableNameViaDataEntity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();
        Helper.UpdateCompleteTableProperties(table);

        // Act
        var result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        var queryResult = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionUpdateAsyncViaTableNameViaDynamic()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();
        Helper.UpdateCompleteTableProperties(table);

        // Act
        var result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, new { table.Id }, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        var queryResult = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionUpdateAsyncViaTableNameViaQueryField()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();
        Helper.UpdateCompleteTableProperties(table);

        // Act
        var result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, new QueryField("Id", table.Id), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        var queryResult = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionUpdateAsyncViaTableNameViaQueryFields()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();
        var queryFields = new[]
        {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
        Helper.UpdateCompleteTableProperties(table);

        // Act
        var result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, queryFields, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        var queryResult = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionUpdateAsyncViaTableNameViaQueryGroup()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var table = Database.CreateCompleteTables(1, connection).First();
        var queryFields = new[]
        {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
        var queryGroup = new QueryGroup(queryFields);
        Helper.UpdateCompleteTableProperties(table);

        // Act
        var result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, queryGroup, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        var queryResult = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }
    #endregion

    #endregion
}
