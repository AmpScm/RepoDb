using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Extensions;
using RepoDb.PostgreSql.IntegrationTests.Setup;
using RepoDb.PostgreSql.IntegrationTests.Models;

namespace RepoDb.PostgreSql.IntegrationTests.Operations;

[TestClass]
public class MergeAllTest
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
    public void TestPostgreSqlConnectionMergeAllForIdentityForEmptyTable()
    {
        // Setup
        List<CompleteTable> tables = Helper.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.MergeAll<CompleteTable>(tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMergeAllForIdentityForNonEmptyTable()
    {
        // Setup
        List<CompleteTable> tables = Database.CreateCompleteTables(10).AsList();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

        // Act
        int result = connection.MergeAll<CompleteTable>(tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMergeAllForIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        List<CompleteTable> tables = Database.CreateCompleteTables(10).AsList();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

        // Act
        int result = connection.MergeAll<CompleteTable>(tables,
            qualifiers);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMergeAllForNonIdentityForEmptyTable()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Helper.CreateNonIdentityCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.MergeAll<NonIdentityCompleteTable>(tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMergeAllForNonIdentityForNonEmptyTable()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Database.CreateNonIdentityCompleteTables(10).AsList();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

        // Act
        int result = connection.MergeAll<NonIdentityCompleteTable>(tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMergeAllForNonIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Database.CreateNonIdentityCompleteTables(10).AsList();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

        // Act
        int result = connection.MergeAll<NonIdentityCompleteTable>(tables,
            qualifiers);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionMergeAllAsyncForIdentityForEmptyTable()
    {
        // Setup
        List<CompleteTable> tables = Helper.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.MergeAllAsync<CompleteTable>(tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMergeAllAsyncForIdentityForNonEmptyTable()
    {
        // Setup
        List<CompleteTable> tables = Database.CreateCompleteTables(10).AsList();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

        // Act
        int result = await connection.MergeAllAsync<CompleteTable>(tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMergeAllAsyncForIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        List<CompleteTable> tables = Database.CreateCompleteTables(10).AsList();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

        // Act
        int result = await connection.MergeAllAsync<CompleteTable>(tables,
            qualifiers);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMergeAllAsyncForNonIdentityForEmptyTable()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Helper.CreateNonIdentityCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.MergeAllAsync<NonIdentityCompleteTable>(tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMergeAllAsyncForNonIdentityForNonEmptyTable()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Database.CreateNonIdentityCompleteTables(10).AsList();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

        // Act
        int result = await connection.MergeAllAsync<NonIdentityCompleteTable>(tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMergeAllAsyncForNonIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Database.CreateNonIdentityCompleteTables(10).AsList();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

        // Act
        int result = await connection.MergeAllAsync<NonIdentityCompleteTable>(tables,
            qualifiers);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestPostgreSqlConnectionMergeAllViaTableNameForIdentityForEmptyTable()
    {
        // Setup
        List<CompleteTable> tables = Helper.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMergeAllAsExpandoObjectViaTableNameForIdentityForEmptyTable()
    {
        // Setup
        List<System.Dynamic.ExpandoObject> tables = Helper.CreateCompleteTablesAsExpandoObjects(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);
        Assert.IsTrue(tables.All(table => ((dynamic)table).Id > 0));

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMergeAllViaTableNameForIdentityForNonEmptyTable()
    {
        // Setup
        List<CompleteTable> tables = Database.CreateCompleteTables(10).AsList();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

        // Act
        int result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMergeAllAsExpandoObjectViaTableNameForIdentityForNonEmptyTable()
    {
        // Setup
        List<CompleteTable> entities = Database.CreateCompleteTables(10).AsList();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        List<System.Dynamic.ExpandoObject> tables = Helper.CreateCompleteTablesAsExpandoObjects(10).AsList();
        tables.ForEach(e => ((IDictionary<string, object>)e)["Id"] = entities[tables.IndexOf(e)].Id);

        // Act
        int result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(entities.Count, connection.CountAll<CompleteTable>());

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(entities.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMergeAllViaTableNameForIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        List<CompleteTable> tables = Database.CreateCompleteTables(10).AsList();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

        // Act
        int result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
            tables,
            qualifiers);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMergeAllAsDynamicsViaTableNameForIdentityForEmptyTable()
    {
        // Setup
        List<dynamic> tables = Helper.CreateCompleteTablesAsDynamics(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMergeAllAsDynamicsViaTableNameForIdentityForNonEmptyTable()
    {
        // Setup
        List<CompleteTable> tables = Database.CreateCompleteTables(10).AsList();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

        // Act
        int result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMergeAllAsDynamicsViaTableNameForIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        List<CompleteTable> tables = Database.CreateCompleteTables(10).AsList();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

        // Act
        int result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
            tables,
            qualifiers);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMergeAllViaTableNameForNonIdentityForEmptyTable()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Helper.CreateNonIdentityCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMergeAllViaTableNameForNonIdentityForNonEmptyTable()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Database.CreateNonIdentityCompleteTables(10).AsList();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

        // Act
        int result = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.ElementAt(tables.IndexOf(table))));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMergeAllViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Database.CreateNonIdentityCompleteTables(10).AsList();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

        // Act
        int result = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables,
            qualifiers);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMergeAllAsDynamicsViaTableNameForNonIdentityForEmptyTable()
    {
        // Setup
        List<dynamic> tables = Helper.CreateNonIdentityCompleteTablesAsDynamics(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

        // Assert
        Assert.AreEqual(tables.Count, result);

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMergeAllAsDynamicsViaTableNameForNonIdentityForNonEmptyTable()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Database.CreateNonIdentityCompleteTables(10).AsList();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

        // Act
        int result = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMergeAllAsDynamicsViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Database.CreateNonIdentityCompleteTables(10).AsList();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

        // Act
        int result = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables,
            qualifiers);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionMergeAllViaTableNameAsyncForIdentityForEmptyTable()
    {
        // Setup
        List<CompleteTable> tables = Helper.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMergeAllAsyncAsExpandoObjectViaTableNameForIdentityForEmptyTable()
    {
        // Setup
        List<System.Dynamic.ExpandoObject> tables = Helper.CreateCompleteTablesAsExpandoObjects(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);
        Assert.IsTrue(tables.All(table => ((dynamic)table).Id > 0));

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMergeAllViaTableNameAsyncForIdentityForNonEmptyTable()
    {
        // Setup
        List<CompleteTable> tables = Database.CreateCompleteTables(10).AsList();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

        // Act
        int result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMergeAllAsyncAsExpandoObjectViaTableNameForIdentityForNonEmptyTable()
    {
        // Setup
        List<CompleteTable> entities = Database.CreateCompleteTables(10).AsList();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        List<System.Dynamic.ExpandoObject> tables = Helper.CreateCompleteTablesAsExpandoObjects(10).AsList();
        tables.ForEach(e => ((IDictionary<string, object>)e)["Id"] = entities[tables.IndexOf(e)].Id);

        // Act
        int result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(entities.Count, connection.CountAll<CompleteTable>());

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(entities.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMergeAllViaTableNameAsyncForIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        List<CompleteTable> tables = Database.CreateCompleteTables(10).AsList();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

        // Act
        int result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            tables,
            qualifiers);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMergeAllAsyncAsDynamicsViaTableNameForIdentityForEmptyTable()
    {
        // Setup
        List<dynamic> tables = Helper.CreateCompleteTablesAsDynamics(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMergeAllAsyncAsDynamicsViaTableNameForIdentityForNonEmptyTable()
    {
        // Setup
        List<CompleteTable> tables = Database.CreateCompleteTables(10).AsList();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

        // Act
        int result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMergeAllAsyncAsDynamicsViaTableNameForIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        List<CompleteTable> tables = Database.CreateCompleteTables(10).AsList();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

        // Act
        int result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            tables,
            qualifiers);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMergeAllAsyncViaTableNameForNonIdentityForEmptyTable()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Helper.CreateNonIdentityCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMergeAllAsyncViaTableNameForNonIdentityForNonEmptyTable()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Database.CreateNonIdentityCompleteTables(10).AsList();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

        // Act
        int result = await connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMergeAllAsyncViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Database.CreateNonIdentityCompleteTables(10).AsList();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

        // Act
        int result = await connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables,
            qualifiers);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMergeAllAsyncAsDynamicsViaTableNameForNonIdentityForEmptyTable()
    {
        // Setup
        List<dynamic> tables = Helper.CreateNonIdentityCompleteTablesAsDynamics(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMergeAllAsyncAsDynamicsViaTableNameForNonIdentityForNonEmptyTable()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Database.CreateNonIdentityCompleteTables(10).AsList();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

        // Act
        int result = await connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMergeAllAsyncAsDynamicsViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Database.CreateNonIdentityCompleteTables(10).AsList();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

        // Act
        int result = await connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables,
            qualifiers);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    #endregion

    #endregion
}
