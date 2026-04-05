using System.ComponentModel.DataAnnotations;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;
using RepoDb.Trace;

namespace RepoDb.Oracle.IntegrationTests.Operations;

[TestClass]
public class MergeTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestOracleConnectionMergeForIdentityForEmptyTable()
    {
        // Setup
        CompleteTable table = Helper.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Act
        object result = connection.Merge<CompleteTable>(table, trace: new DiagnosticsTracer());
        IEnumerable<CompleteTable> queryResult = V23cOrLater ? connection.Query<CompleteTable>(result) : connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public void TestOracleConnectionMergeForIdentityForNonEmptyTable()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
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
    public void TestOracleConnectionMergeForIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using var connection = CreateConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);
        table.ColumnInt = 0;
        table.ColumnChar = "C";

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
    public async Task TestOracleConnectionMergeAsyncForIdentityForEmptyTable()
    {
        // Setup
        CompleteTable table = Helper.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Act
        object result = await connection.MergeAsync<CompleteTable>(table, cancellationToken: TestContext.CancellationToken);
        IEnumerable<CompleteTable> queryResult = V23cOrLater ? connection.Query<CompleteTable>(result) : connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public async Task TestOracleConnectionMergeAsyncForIdentityForNonEmptyTable()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        object result = await connection.MergeAsync<CompleteTable>(table, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public async Task TestOracleConnectionMergeAsyncForIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using var connection = CreateConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);
        table.ColumnInt = 0;
        table.ColumnChar = "C";

        // Act
        object result = await connection.MergeAsync<CompleteTable>(table,
            qualifiers: qualifiers, cancellationToken: TestContext.CancellationToken);

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
    public void TestOracleConnectionMergeViaTableNameForIdentityForEmptyTable()
    {
        // Setup
        CompleteTable table = Helper.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Act
        object result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
            table);
        IEnumerable<CompleteTable> queryResult = V23cOrLater ? connection.Query<CompleteTable>(result) : connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public void TestOracleConnectionMergeAsExpandoObjectViaTableNameForIdentityForEmptyTable()
    {
        // Setup
        System.Dynamic.ExpandoObject table = Helper.CreateCompleteTablesAsExpandoObjects(1).First();

        using var connection = CreateConnection();
        // Act
        object result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
            table);
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        if (V23cOrLater)
            Assert.AreEqual(((dynamic)table).Id, result);
        Helper.AssertMembersEquality(queryResult.First(), table);
    }

    [TestMethod]
    public void TestOracleConnectionMergeViaTableNameForIdentityForNonEmptyTable()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
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
    public void TestOracleConnectionMergeAsExpandoObjectViaTableNameForIdentityForNonEmptyTable()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Setup
        System.Dynamic.ExpandoObject entity = Helper.CreateCompleteTablesAsExpandoObjects(1).First();
        ((IDictionary<string, object?>)entity)["Id"] = table.Id;

        // Act
        object result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
            entity);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        if (V23cOrLater)
            Assert.AreEqual(table.Id, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertMembersEquality(queryResult.First(), entity);
    }

    [TestMethod]
    public void TestOracleConnectionMergeViaTableNameForIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using var connection = CreateConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);
        table.ColumnInt = 0;
        table.ColumnChar = "C";

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
    public void TestOracleConnectionMergeAsDynamicViaTableNameForIdentityForEmptyTable()
    {
        // Setup
        dynamic table = Helper.CreateCompleteTablesAsDynamics(1).First();

        using var connection = CreateConnection();
        // Act
        object result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
            (object)table);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        if (V23cOrLater)
            Assert.IsGreaterThan(0, Convert.ToInt64(result));

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public void TestOracleConnectionMergeAsDynamicViaTableNameForIdentityForNonEmptyTable()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
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
    public void TestOracleConnectionMergeAsDynamicViaTableNameForIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using var connection = CreateConnection();
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
    public async Task TestOracleConnectionMergeAsyncViaTableNameForIdentityForEmptyTable()
    {
        // Setup
        CompleteTable table = Helper.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Act
        object result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
            table, cancellationToken: TestContext.CancellationToken);
        IEnumerable<CompleteTable> queryResult = V23cOrLater ? connection.Query<CompleteTable>(result) : connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public async Task TestOracleConnectionMergeAsyncAsExpandoObjectViaTableNameForIdentityForEmptyTable()
    {
        // Setup
        System.Dynamic.ExpandoObject table = Helper.CreateCompleteTablesAsExpandoObjects(1).First();

        using var connection = CreateConnection();
        // Act
        object result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
            table, cancellationToken: TestContext.CancellationToken);
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        if (V23cOrLater)
            Assert.AreEqual(((dynamic)table).Id, result);
        Helper.AssertMembersEquality(queryResult.First(), table);
    }

    [TestMethod]
    public async Task TestOracleConnectionMergeAsyncViaTableNameForIdentityForNonEmptyTable()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        object result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
            table, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        if (V23cOrLater)
            Assert.AreEqual(table.Id, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public async Task TestOracleConnectionMergeAsyncAsExpandoObjectViaTableNameForIdentityForNonEmptyTable()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Setup
        System.Dynamic.ExpandoObject entity = Helper.CreateCompleteTablesAsExpandoObjects(1).First();
        ((IDictionary<string, object?>)entity)["Id"] = table.Id;

        // Act
        object result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
            entity, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        if (V23cOrLater)
            Assert.AreEqual(table.Id, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertMembersEquality(queryResult.First(), entity);
    }

    [TestMethod]
    public async Task TestOracleConnectionMergeAsyncViaTableNameForIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using var connection = CreateConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        object result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
            table,
            qualifiers: qualifiers, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public async Task TestOracleConnectionMergeAsyncAsDynamicViaTableNameForIdentityForEmptyTable()
    {
        // Setup
        dynamic table = Helper.CreateCompleteTablesAsDynamics(1).First();

        using var connection = CreateConnection();
        // Act
        object result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
            (object)table, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        if (V23cOrLater)
            Assert.IsGreaterThan(0, Convert.ToInt64(result));

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public async Task TestOracleConnectionMergeAsyncAsDynamicViaTableNameForIdentityForNonEmptyTable()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        object result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
            table, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public async Task TestOracleConnectionMergeAsyncAsDynamicViaTableNameForIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using var connection = CreateConnection();
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        // Act
        object result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
            table,
            qualifiers: qualifiers, cancellationToken: TestContext.CancellationToken);

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
