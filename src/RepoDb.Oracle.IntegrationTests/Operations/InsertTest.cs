using RepoDb.Oracle.IntegrationTests.Models;

namespace RepoDb.Oracle.IntegrationTests.Operations;

[TestClass]
public class InsertTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestOracleConnectionInsertForIdentity()
    {
        // Setup
        CompleteTable table = Helper.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Act
        object result = connection.Insert<CompleteTable>(table);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.IsGreaterThan(0, Convert.ToInt64(result));
        Assert.IsGreaterThan(0, table.Id);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Assert.AreEqual(1, queryResult?.Count());
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public void TestOracleConnectionInsertForNonIdentity()
    {
        // Setup
        NonIdentityCompleteTable table = Helper.CreateNonIdentityCompleteTables(1).First();

        using var connection = CreateConnection();
        // Act
        object result = connection.Insert<NonIdentityCompleteTable>(table);

        // Assert
        Assert.AreEqual(1, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.Query<NonIdentityCompleteTable>(result);

        // Assert
        Assert.AreEqual(1, queryResult?.Count());
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestOracleConnectionInsertAsyncForIdentity()
    {
        // Setup
        CompleteTable table = Helper.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Act
        object result = await connection.InsertAsync<CompleteTable>(table, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.IsGreaterThan(0, Convert.ToInt64(result));
        Assert.IsGreaterThan(0, table.Id);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Assert.AreEqual(1, queryResult?.Count());
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public async Task TestOracleConnectionInsertAsyncForNonIdentity()
    {
        // Setup
        NonIdentityCompleteTable table = Helper.CreateNonIdentityCompleteTables(1).First();

        using var connection = CreateConnection();
        // Act
        object result = await connection.InsertAsync<NonIdentityCompleteTable>(table, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.Query<NonIdentityCompleteTable>(result);

        // Assert
        Assert.AreEqual(1, queryResult?.Count());
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestOracleConnectionInsertViaTableNameForIdentity()
    {
        // Setup
        CompleteTable table = Helper.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Act
        object result = connection.Insert(ClassMappedNameCache.Get<CompleteTable>(),
            table);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.IsGreaterThan(0, Convert.ToInt64(result));

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Assert.AreEqual(1, queryResult?.Count());
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public void TestOracleConnectionInsertViaTableNameAsDynamicForIdentity()
    {
        // Setup
        dynamic table = Helper.CreateCompleteTablesAsDynamics(1).First();

        using var connection = CreateConnection();
        // Act
        object result = connection.Insert(ClassMappedNameCache.Get<CompleteTable>(),
            (object)table);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.IsGreaterThan(0, Convert.ToInt64(result));

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Assert.AreEqual(1, queryResult?.Count());
        Helper.AssertMembersEquality(queryResult.First(), table);
    }

    [TestMethod]
    public void TestOracleConnectionInsertViaTableNameAsExpandoObjectForIdentity()
    {
        // Setup
        System.Dynamic.ExpandoObject table = Helper.CreateCompleteTablesAsExpandoObjects(1).First();

        using var connection = CreateConnection();
        // Act
        object result = connection.Insert(ClassMappedNameCache.Get<CompleteTable>(),
            table);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.IsGreaterThan(0, Convert.ToInt64(result));
        Assert.AreEqual(((dynamic)table).Id, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Assert.AreEqual(1, queryResult?.Count());
        Helper.AssertMembersEquality(queryResult.First(), table);
    }

    [TestMethod]
    public void TestOracleConnectionInsertViaTableNameForNonIdentity()
    {
        // Setup
        NonIdentityCompleteTable table = Helper.CreateNonIdentityCompleteTables(1).First();

        using var connection = CreateConnection();
        // Act
        object result = connection.Insert(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            table);

        // Assert
        Assert.AreEqual(1, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.Query<NonIdentityCompleteTable>(result);

        // Assert
        Assert.AreEqual(1, queryResult?.Count());
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public void TestOracleConnectionInsertViaTableNameAsDynamicForNonIdentity()
    {
        // Setup
        dynamic table = Helper.CreateNonIdentityCompleteTablesAsDynamics(1).First();

        using var connection = CreateConnection();
        // Act
        object result = connection.Insert(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            (object)table);

        // Assert
        Assert.AreEqual(1, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.Query<NonIdentityCompleteTable>(result);

        // Assert
        Assert.AreEqual(1, queryResult?.Count());
        Helper.AssertMembersEquality(queryResult.First(), table);
    }

    [TestMethod]
    public void TestOracleConnectionInsertViaTableNameAsExpandoObjectForNonIdentity()
    {
        // Setup
        System.Dynamic.ExpandoObject table = Helper.CreateNonIdentityCompleteTablesAsExpandoObjects(1).First();

        using var connection = CreateConnection();
        // Act
        object result = connection.Insert(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            table);


        var f = DbFieldCache.Get(connection, "NonIdentityCompleteTable", null);
        foreach(var ff in f)
        {
            Console.WriteLine($"{ff.FieldName} - {ff.IsIdentity} {ff.IsPrimary}");
        }

        // Assert
        Assert.AreEqual(1, connection.CountAll<NonIdentityCompleteTable>());
        Assert.IsGreaterThan(0, Convert.ToInt64(result));

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.Query<NonIdentityCompleteTable>(result);

        // Assert
        Assert.AreEqual(1, queryResult?.Count());
        Helper.AssertMembersEquality(queryResult.First(), table);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestOracleConnectionInsertViaTableNameAsyncForIdentity()
    {
        // Setup
        CompleteTable table = Helper.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Act
        object result = await connection.InsertAsync(ClassMappedNameCache.Get<CompleteTable>(),
            table, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.IsGreaterThan(0, Convert.ToInt64(result));

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Assert.AreEqual(1, queryResult?.Count());
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public async Task TestOracleConnectionInsertAsyncViaTableNameAsDynamicForIdentity()
    {
        // Setup
        dynamic table = Helper.CreateCompleteTablesAsDynamics(1).First();

        using var connection = CreateConnection();
        // Act
        object result = await connection.InsertAsync(ClassMappedNameCache.Get<CompleteTable>(),
            (object)table, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.IsGreaterThan(0, Convert.ToInt64(result));

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Assert.AreEqual(1, queryResult?.Count());
        Helper.AssertMembersEquality(queryResult.First(), table);
    }

    [TestMethod]
    public async Task TestOracleConnectionInsertAsyncViaTableNameAsExpandoObjectForIdentity()
    {
        // Setup
        System.Dynamic.ExpandoObject table = Helper.CreateCompleteTablesAsExpandoObjects(1).First();

        using var connection = CreateConnection();
        // Act
        object result = await connection.InsertAsync(ClassMappedNameCache.Get<CompleteTable>(),
            table, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        Assert.IsGreaterThan(0, Convert.ToInt64(result));
        Assert.AreEqual(((dynamic)table).Id, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(result);

        // Assert
        Assert.AreEqual(1, queryResult?.Count());
        Helper.AssertMembersEquality(queryResult.First(), table);
    }

    [TestMethod]
    public async Task TestOracleConnectionInsertViaTableNameAsyncForNonIdentity()
    {
        // Setup
        NonIdentityCompleteTable table = Helper.CreateNonIdentityCompleteTables(1).First();

        using var connection = CreateConnection();
        // Act
        object result = await connection.InsertAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            table, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, connection.CountAll<NonIdentityCompleteTable>());
        Assert.IsGreaterThan(0, Convert.ToInt64(result));

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.Query<NonIdentityCompleteTable>(result);

        // Assert
        Assert.AreEqual(1, queryResult?.Count());
        Helper.AssertPropertiesEquality(table, queryResult.First());
    }

    [TestMethod]
    public async Task TestOracleConnectionInsertAsyncViaTableNameAsDynamicForNonIdentity()
    {
        // Setup
        dynamic table = Helper.CreateNonIdentityCompleteTablesAsDynamics(1).First();

        using var connection = CreateConnection();
        // Act
        object result = await connection.InsertAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            (object)table, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(table.Id, result);

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.Query<NonIdentityCompleteTable>(result);

        // Assert
        Assert.AreEqual(1, queryResult?.Count());
        Helper.AssertMembersEquality(queryResult.First(), table);
    }

    [TestMethod]
    public async Task TestOracleConnectionInsertAsyncViaTableNameAsExpandoObjectForNonIdentity()
    {
        // Setup
        System.Dynamic.ExpandoObject table = Helper.CreateNonIdentityCompleteTablesAsExpandoObjects(1).First();

        using var connection = CreateConnection();
        // Act
        object result = await connection.InsertAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            table, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, connection.CountAll<NonIdentityCompleteTable>());
        Assert.IsGreaterThan(0, Convert.ToInt64(result));

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.Query<NonIdentityCompleteTable>(result);

        // Assert
        Assert.AreEqual(1, queryResult?.Count());
        Helper.AssertMembersEquality(queryResult.First(), table);
    }

    #endregion

    #endregion
}
