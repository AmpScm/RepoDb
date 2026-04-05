using System.Diagnostics.Tracing;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Trace;

namespace RepoDb.Oracle.IntegrationTests.Operations;

[TestClass]
public class InsertAllTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestOracleConnectionInsertAllForIdentity()
    {
        // Setup
        List<CompleteTable> tables = Helper.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public void TestOracleConnectionInsertAllForNonIdentity()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Helper.CreateNonIdentityCompleteTables(10);

        using var connection = CreateConnection();
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
    public async Task TestOracleConnectionInsertAllAsyncForIdentity()
    {
        // Setup
        List<CompleteTable> tables = Helper.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public async Task TestOracleConnectionInsertAllAsyncForNonIdentity()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Helper.CreateNonIdentityCompleteTables(10);

        using var connection = CreateConnection();
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
    public void TestOracleConnectionInsertAllViaTableNameForIdentity()
    {
        // Setup
        List<CompleteTable> tables = Helper.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public void TestOracleConnectionInsertAllViaTableNameAsDynamicsForIdentity()
    {
        // Setup
        List<dynamic> tables = Helper.CreateCompleteTablesAsDynamics(10);

        using var connection = CreateConnection();
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
    public void TestOracleConnectionInsertAllViaTableNameAsExpandoObjectsForIdentity()
    {
        // Setup
        List<System.Dynamic.ExpandoObject> tables = Helper.CreateCompleteTablesAsExpandoObjects(10);

        using var connection = CreateConnection();
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
    public void TestOracleConnectionInsertAllViaTableNameForNonIdentity()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Helper.CreateNonIdentityCompleteTables(10);

        using var connection = CreateConnection();
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
    public void TestOracleConnectionInsertAllViaTableNameAsDynamicsForNonIdentity()
    {
        // Setup
        List<dynamic> tables = Helper.CreateNonIdentityCompleteTablesAsDynamics(10);

        using var connection = CreateConnection();
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
    public void TestOracleConnectionInsertAllViaTableNameAsExpandoObjectsForNonIdentity()
    {
        // Setup
        List<System.Dynamic.ExpandoObject> tables = Helper.CreateNonIdentityCompleteTablesAsExpandoObjects(10);

        using var connection = CreateConnection();
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
    public async Task TestOracleConnectionInsertAllViaTableNameAsyncForIdentity()
    {
        // Setup
        List<CompleteTable> tables = Helper.CreateCompleteTables(10);

        using var connection = CreateConnection();
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
    public async Task TestOracleConnectionInsertAllAsyncViaTableNameAsDynamicsForIdentity()
    {
        // Setup
        List<dynamic> tables = Helper.CreateCompleteTablesAsDynamics(10);

        using var connection = CreateConnection();
        // Act
        int result = await connection.InsertAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken, trace: new DiagnosticsTracer());

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestOracleConnectionInsertAllAsyncViaTableNameAsExpandoObjectsForIdentity()
    {
        // Setup
        List<System.Dynamic.ExpandoObject> tables = Helper.CreateCompleteTablesAsExpandoObjects(10);

        using var connection = CreateConnection();
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
    public async Task TestOracleConnectionInsertAllViaTableNameAsyncForNonIdentity()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Helper.CreateNonIdentityCompleteTables(10);

        using var connection = CreateConnection();
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
    public async Task TestOracleConnectionInsertAllAsyncViaTableNameAsDynamicsForNonIdentity()
    {
        // Setup
        List<dynamic> tables = Helper.CreateNonIdentityCompleteTablesAsDynamics(10);

        using var connection = CreateConnection();
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
    public async Task TestOracleConnectionInsertAllAsyncViaTableNameAsExpandoObjectsForNonIdentity()
    {
        // Setup
        List<System.Dynamic.ExpandoObject> tables = Helper.CreateNonIdentityCompleteTablesAsExpandoObjects(10);

        using var connection = CreateConnection();
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

    #endregion

    #endregion
}
