using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.PostgreSql.IntegrationTests.Operations;

[TestClass]
public class MergeTest
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
    public void TestPostgreSqlConnectionMergeForIdentityForEmptyTable()
    {
        // Setup
        CompleteTable table = Helper.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = connection.Merge<CompleteTable>(table);
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMergeForIdentityForNonEmptyTable()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        object result = connection.Merge<CompleteTable>(table);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMergeForIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);
        table.ColumnInteger = 0;
        table.ColumnCharacter = "C";

        // Act
        object result = connection.Merge<CompleteTable>(table,
            qualifiers: qualifiers);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionMergeAsyncForIdentityForEmptyTable()
    {
        // Setup
        CompleteTable table = Helper.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = await connection.MergeAsync<CompleteTable>(table);
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMergeAsyncForIdentityForNonEmptyTable()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        object result = await connection.MergeAsync<CompleteTable>(table);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMergeAsyncForIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);
        table.ColumnInteger = 0;
        table.ColumnCharacter = "C";

        // Act
        object result = await connection.MergeAsync<CompleteTable>(table,
            qualifiers: qualifiers);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestPostgreSqlConnectionMergeViaTableNameForIdentityForEmptyTable()
    {
        // Setup
        CompleteTable table = Helper.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
            table);
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMergeAsExpandoObjectViaTableNameForIdentityForEmptyTable()
    {
        // Setup
        System.Dynamic.ExpandoObject table = Helper.CreateCompleteTablesAsExpandoObjects(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
            table);
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(((dynamic)table).Id, result);
        Helper.AssertMembersEquality(queryResult.First(), table);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMergeViaTableNameForIdentityForNonEmptyTable()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        object result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
            table);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMergeAsExpandoObjectViaTableNameForIdentityForNonEmptyTable()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        System.Dynamic.ExpandoObject entity = Helper.CreateCompleteTablesAsExpandoObjects(1).First();
        ((IDictionary<string, object?>)entity)["Id"] = table.Id;

        // Act
        object result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
            entity);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertMembersEquality(queryResult.First(), entity);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMergeViaTableNameForIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);
        table.ColumnInteger = 0;
        table.ColumnCharacter = "C";

        // Act
        object result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
            table,
            qualifiers: qualifiers);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMergeAsDynamicViaTableNameForIdentityForEmptyTable()
    {
        // Setup
        dynamic table = Helper.CreateCompleteTablesAsDynamics(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
            (object)table);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.IsGreaterThan(0, Convert.ToInt64(result));

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMergeAsDynamicViaTableNameForIdentityForNonEmptyTable()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        object result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
            table);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMergeAsDynamicViaTableNameForIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        object result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
            table,
            qualifiers: qualifiers);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionMergeAsyncViaTableNameForIdentityForEmptyTable()
    {
        // Setup
        CompleteTable table = Helper.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
            table);
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMergeAsyncAsExpandoObjectViaTableNameForIdentityForEmptyTable()
    {
        // Setup
        System.Dynamic.ExpandoObject table = Helper.CreateCompleteTablesAsExpandoObjects(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
            table);
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(((dynamic)table).Id, result);
        Helper.AssertMembersEquality(queryResult.First(), table);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMergeAsyncViaTableNameForIdentityForNonEmptyTable()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        object result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
            table);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMergeAsyncAsExpandoObjectViaTableNameForIdentityForNonEmptyTable()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        System.Dynamic.ExpandoObject entity = Helper.CreateCompleteTablesAsExpandoObjects(1).First();
        ((IDictionary<string, object?>)entity)["Id"] = table.Id;

        // Act
        object result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
            entity);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertMembersEquality(queryResult.First(), entity);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMergeAsyncViaTableNameForIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        object result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
            table,
            qualifiers: qualifiers);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMergeAsyncAsDynamicViaTableNameForIdentityForEmptyTable()
    {
        // Setup
        dynamic table = Helper.CreateCompleteTablesAsDynamics(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
            (object)table);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.IsGreaterThan(0, Convert.ToInt64(result));

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMergeAsyncAsDynamicViaTableNameForIdentityForNonEmptyTable()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        object result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
            table);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMergeAsyncAsDynamicViaTableNameForIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        object result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
            table,
            qualifiers: qualifiers);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    #endregion

    #endregion
}
