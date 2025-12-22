using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.PostgreSql.IntegrationTests.Operations;

[TestClass]
public class InsertAllTest
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
    public void TestPostgreSqlConnectionInsertAllForIdentity()
    {
        // Setup
        List<CompleteTable> tables = Helper.CreateCompleteTables(10);

        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.InsertAll<CompleteTable>(tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);
        Assert.IsTrue(tables.All(table => table.Id > 0));

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionInsertAllForNonIdentity()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Helper.CreateNonIdentityCompleteTables(10);

        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.InsertAll<NonIdentityCompleteTable>(tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionInsertAllAsyncForIdentity()
    {
        // Setup
        List<CompleteTable> tables = Helper.CreateCompleteTables(10);

        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.InsertAllAsync<CompleteTable>(tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);
        Assert.IsTrue(tables.All(table => table.Id > 0));

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionInsertAllAsyncForNonIdentity()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Helper.CreateNonIdentityCompleteTables(10);

        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.InsertAllAsync<NonIdentityCompleteTable>(tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestPostgreSqlConnectionInsertAllViaTableNameForIdentity()
    {
        // Setup
        List<CompleteTable> tables = Helper.CreateCompleteTables(10);

        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.InsertAll(ClassMappedNameCache.Get<CompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionInsertAllViaTableNameAsDynamicsForIdentity()
    {
        // Setup
        List<dynamic> tables = Helper.CreateCompleteTablesAsDynamics(10);

        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.InsertAll(ClassMappedNameCache.Get<CompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionInsertAllViaTableNameAsExpandoObjectsForIdentity()
    {
        // Setup
        List<System.Dynamic.ExpandoObject> tables = Helper.CreateCompleteTablesAsExpandoObjects(10);

        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.InsertAll(ClassMappedNameCache.Get<CompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);
        Assert.IsTrue(tables.All(table => ((dynamic)table).Id > 0));

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionInsertAllViaTableNameForNonIdentity()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Helper.CreateNonIdentityCompleteTables(10);

        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.InsertAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionInsertAllViaTableNameAsDynamicsForNonIdentity()
    {
        // Setup
        List<dynamic> tables = Helper.CreateNonIdentityCompleteTablesAsDynamics(10);

        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.InsertAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionInsertAllViaTableNameAsExpandoObjectsForNonIdentity()
    {
        // Setup
        List<System.Dynamic.ExpandoObject> tables = Helper.CreateNonIdentityCompleteTablesAsExpandoObjects(10);

        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = connection.InsertAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);
        Assert.IsTrue(tables.All(table => ((dynamic)table).Id > 0));

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionInsertAllViaTableNameAsyncForIdentity()
    {
        // Setup
        List<CompleteTable> tables = Helper.CreateCompleteTables(10);

        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.InsertAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionInsertAllAsyncViaTableNameAsDynamicsForIdentity()
    {
        // Setup
        List<dynamic> tables = Helper.CreateCompleteTablesAsDynamics(10);

        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.InsertAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionInsertAllAsyncViaTableNameAsExpandoObjectsForIdentity()
    {
        // Setup
        List<System.Dynamic.ExpandoObject> tables = Helper.CreateCompleteTablesAsExpandoObjects(10);

        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.InsertAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);
        Assert.IsTrue(tables.All(table => ((dynamic)table).Id > 0));

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionInsertAllViaTableNameAsyncForNonIdentity()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Helper.CreateNonIdentityCompleteTables(10);

        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.InsertAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionInsertAllAsyncViaTableNameAsDynamicsForNonIdentity()
    {
        // Setup
        List<dynamic> tables = Helper.CreateNonIdentityCompleteTablesAsDynamics(10);

        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.InsertAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionInsertAllAsyncViaTableNameAsExpandoObjectsForNonIdentity()
    {
        // Setup
        List<System.Dynamic.ExpandoObject> tables = Helper.CreateNonIdentityCompleteTablesAsExpandoObjects(10);

        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        int result = await connection.InsertAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
    }

    public TestContext TestContext { get; set; }

    #endregion

    #endregion
}
