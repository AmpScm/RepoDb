using RepoDb.Extensions;
using RepoDb.Oracle.IntegrationTests.Setup;
using RepoDb.Oracle.IntegrationTests.Models;

namespace RepoDb.Oracle.IntegrationTests.Operations;

[TestClass]
public class MergeAllTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestOracleConnectionMergeAllForIdentityForEmptyTable()
    {
        // Setup
        List<CompleteTable> tables = Helper.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = connection.MergeAll<CompleteTable>(tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Before Orcle 23c, RepoDb's merge doesn't return the updated identity values, so apply offset
        long offset = V23cOrLater ? 0 : connection.Min<CompleteTable, long>(x => x.Id, where: (object?)null) - 1;

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        foreach (var table in tables)
        {
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id + offset));
        }
    }

    [TestMethod]
    public void TestOracleConnectionMergeAllForIdentityForNonEmptyTable()
    {
        // Setup
        List<CompleteTable> tables = Database.CreateCompleteTables(10).AsList();

        using var connection = CreateConnection();
        // Setup
        foreach (var table in tables) { Helper.UpdateCompleteTableProperties(table); }

        // Act
        int result = connection.MergeAll<CompleteTable>(tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        foreach (var table in tables)
        {
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id));
        }
    }

    [TestMethod]
    public void TestOracleConnectionMergeAllForIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        List<CompleteTable> tables = Database.CreateCompleteTables(10).AsList();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using var connection = CreateConnection();
        // Setup
        foreach (var table in tables) { Helper.UpdateCompleteTableProperties(table); }

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
        foreach (var table in tables)
        {
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id));
        }
    }

    [TestMethod]
    public void TestOracleConnectionMergeAllForNonIdentityForEmptyTable()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Helper.CreateNonIdentityCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = connection.MergeAll<NonIdentityCompleteTable>(tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        foreach (var table in tables)
        {
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id));
        }
    }

    [TestMethod]
    public void TestOracleConnectionMergeAllForNonIdentityForNonEmptyTable()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Database.CreateNonIdentityCompleteTables(10).AsList();

        using var connection = CreateConnection();
        // Setup
        foreach (var table in tables) Helper.UpdateNonIdentityCompleteTableProperties(table);

        // Act
        int result = connection.MergeAll<NonIdentityCompleteTable>(tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        foreach (var table in tables) Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id));
    }

    [TestMethod]
    public void TestOracleConnectionMergeAllForNonIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Database.CreateNonIdentityCompleteTables(10).AsList();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using var connection = CreateConnection();
        // Setup
        foreach (var table in tables) Helper.UpdateNonIdentityCompleteTableProperties(table);

        // Act
        int result = connection.MergeAll<NonIdentityCompleteTable>(tables,
            qualifiers);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        foreach (var table in tables) Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestOracleConnectionMergeAllAsyncForIdentityForEmptyTable()
    {
        // Setup
        List<CompleteTable> tables = Helper.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = await connection.MergeAllAsync<CompleteTable>(tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();
        // Before Orcle 23c, RepoDb's merge doesn't return the updated identity values, so apply offset
        long offset = V23cOrLater ? 0 : connection.Min<CompleteTable, long>(x => x.Id, where: (object?)null) - 1;

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        foreach (var table in tables)
        {
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id + offset));
        }
    }

    [TestMethod]
    public async Task TestOracleConnectionMergeAllAsyncForIdentityForNonEmptyTable()
    {
        // Setup
        List<CompleteTable> tables = Database.CreateCompleteTables(10).AsList();

        using var connection = CreateConnection();
        // Setup
        foreach (var table in tables) { Helper.UpdateCompleteTableProperties(table); }

        // Act
        int result = await connection.MergeAllAsync<CompleteTable>(tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        foreach (var table in tables) Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id));
    }

    [TestMethod]
    public async Task TestOracleConnectionMergeAllAsyncForIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        List<CompleteTable> tables = Database.CreateCompleteTables(10).AsList();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using var connection = CreateConnection();
        // Setup
        foreach (var table in tables) { Helper.UpdateCompleteTableProperties(table); }

        // Act
        int result = await connection.MergeAllAsync<CompleteTable>(tables,
            qualifiers, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        foreach (var table in tables) Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id));
    }

    [TestMethod]
    public async Task TestOracleConnectionMergeAllAsyncForNonIdentityForEmptyTable()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Helper.CreateNonIdentityCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = await connection.MergeAllAsync<NonIdentityCompleteTable>(tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        foreach (var table in tables) Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id));
    }

    [TestMethod]
    public async Task TestOracleConnectionMergeAllAsyncForNonIdentityForNonEmptyTable()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Database.CreateNonIdentityCompleteTables(10).AsList();

        using var connection = CreateConnection();
        // Setup
        foreach (var table in tables) Helper.UpdateNonIdentityCompleteTableProperties(table);

        // Act
        int result = await connection.MergeAllAsync<NonIdentityCompleteTable>(tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        foreach (var table in tables) Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id));
    }

    [TestMethod]
    public async Task TestOracleConnectionMergeAllAsyncForNonIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Database.CreateNonIdentityCompleteTables(10).AsList();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using var connection = CreateConnection();
        // Setup
        foreach (var table in tables) Helper.UpdateNonIdentityCompleteTableProperties(table);

        // Act
        int result = await connection.MergeAllAsync<NonIdentityCompleteTable>(tables,
            qualifiers, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        foreach (var table in tables) Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestOracleConnectionMergeAllViaTableNameForIdentityForEmptyTable()
    {
        // Setup
        List<CompleteTable> tables = Helper.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Before Orcle 23c, RepoDb's merge doesn't return the updated identity values, so apply offset
        long offset = V23cOrLater ? 0 : connection.Min<CompleteTable, long>(x => x.Id, where: (object?)null) - 1;

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        foreach (var table in tables)
        {
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id + offset));
        }
    }

    [TestMethod]
    public void TestOracleConnectionMergeAllAsExpandoObjectViaTableNameForIdentityForEmptyTable()
    {
        // Setup
        List<System.Dynamic.ExpandoObject> tables = Helper.CreateCompleteTablesAsExpandoObjects(10);

        using var connection = CreateConnection();
        // Act
        int result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);
        Assert.IsTrue(tables.All(table => ((dynamic)table).Id > 0));

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Before Orcle 23c, RepoDb's merge doesn't return the updated identity values, so apply offset
        long offset = V23cOrLater ? 0 : connection.Min<CompleteTable, long>(x => x.Id, where: (object?)null) - 1;

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        foreach (var table in tables)
        {
            Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id + offset), table);
        }
    }

    [TestMethod]
    public void TestOracleConnectionMergeAllViaTableNameForIdentityForNonEmptyTable()
    {
        // Setup
        List<CompleteTable> tables = Database.CreateCompleteTables(10).AsList();

        using var connection = CreateConnection();
        // Setup
        foreach (var table in tables) { Helper.UpdateCompleteTableProperties(table); }

        // Act
        int result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        foreach (var table in tables)
        {
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id));
        }
    }

    [TestMethod]
    public void TestOracleConnectionMergeAllAsExpandoObjectViaTableNameForIdentityForNonEmptyTable()
    {
        // Setup
        List<CompleteTable> entities = Database.CreateCompleteTables(10).AsList();

        using var connection = CreateConnection();
        // Setup
        List<System.Dynamic.ExpandoObject> tables = Helper.CreateCompleteTablesAsExpandoObjects(10).AsList();
        tables.ForEach(e => ((IDictionary<string, object?>)e)["Id"] = entities[tables.IndexOf(e)].Id);

        // Act
        int result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(entities.Count, connection.CountAll<CompleteTable>());

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(entities.Count, queryResult.Count());
        foreach (var table in tables) Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table);
    }

    [TestMethod]
    public void TestOracleConnectionMergeAllViaTableNameForIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        List<CompleteTable> tables = Database.CreateCompleteTables(10).AsList();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using var connection = CreateConnection();
        // Setup
        foreach (var table in tables) { Helper.UpdateCompleteTableProperties(table); }

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
        foreach (var table in tables)
        {
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id));
        }
    }

    [TestMethod]
    public void TestOracleConnectionMergeAllAsDynamicsViaTableNameForIdentityForEmptyTable()
    {
        // Setup
        List<dynamic> tables = Helper.CreateCompleteTablesAsDynamics(10);

        using var connection = CreateConnection();
        // Act
        int result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Before Orcle 23c, RepoDb's merge doesn't return the updated identity values, so apply offset
        long offset = V23cOrLater ? 0 : connection.Min<CompleteTable, long>(x => x.Id, where: (object?)null) - 1;

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        foreach (var table in tables)
        {
            Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id + offset));
        }
    }

    [TestMethod]
    public void TestOracleConnectionMergeAllAsDynamicsViaTableNameForIdentityForNonEmptyTable()
    {
        // Setup
        List<CompleteTable> tables = Database.CreateCompleteTables(10).AsList();

        using var connection = CreateConnection();
        // Setup
        foreach (var table in tables) { Helper.UpdateCompleteTableProperties(table); }

        // Act
        int result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        foreach (var table in tables)
        {
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id));
        }
    }

    [TestMethod]
    public void TestOracleConnectionMergeAllAsDynamicsViaTableNameForIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        List<CompleteTable> tables = Database.CreateCompleteTables(10).AsList();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using var connection = CreateConnection();
        // Setup
        foreach (var table in tables) { Helper.UpdateCompleteTableProperties(table); }

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
        foreach (var table in tables)
        {
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id));
        }
    }

    [TestMethod]
    public void TestOracleConnectionMergeAllViaTableNameForNonIdentityForEmptyTable()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Helper.CreateNonIdentityCompleteTables(10);

        using var connection = CreateConnection();
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
        foreach (var table in tables)
        {
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id));
        }
    }

    [TestMethod]
    public void TestOracleConnectionMergeAllViaTableNameForNonIdentityForNonEmptyTable()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Database.CreateNonIdentityCompleteTables(10).AsList();

        using var connection = CreateConnection();
        // Setup
        foreach (var table in tables) Helper.UpdateNonIdentityCompleteTableProperties(table);

        // Act
        int result = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        foreach (var table in tables)
        {
            Helper.AssertMembersEquality(table, queryResult.ElementAt(tables.IndexOf(table)));
        }
    }

    [TestMethod]
    public void TestOracleConnectionMergeAllViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Database.CreateNonIdentityCompleteTables(10).AsList();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using var connection = CreateConnection();
        // Setup
        foreach (var table in tables) Helper.UpdateNonIdentityCompleteTableProperties(table);

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
        foreach (var table in tables)
        {
            Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id));
        }
    }

    [TestMethod]
    public void TestOracleConnectionMergeAllAsDynamicsViaTableNameForNonIdentityForEmptyTable()
    {
        // Setup
        List<dynamic> tables = Helper.CreateNonIdentityCompleteTablesAsDynamics(10);

        using var connection = CreateConnection();
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
        foreach (var table in tables) Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id));
    }

    [TestMethod]
    public void TestOracleConnectionMergeAllAsDynamicsViaTableNameForNonIdentityForNonEmptyTable()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Database.CreateNonIdentityCompleteTables(10).AsList();

        using var connection = CreateConnection();
        // Setup
        foreach (var table in tables) Helper.UpdateNonIdentityCompleteTableProperties(table);

        // Act
        int result = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        foreach (var table in tables)
        {
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id));
        }
    }

    [TestMethod]
    public void TestOracleConnectionMergeAllAsDynamicsViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Database.CreateNonIdentityCompleteTables(10).AsList();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using var connection = CreateConnection();
        // Setup
        foreach (var table in tables) Helper.UpdateNonIdentityCompleteTableProperties(table);

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
        foreach (var table in tables)
        {
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id));
        }
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestOracleConnectionMergeAllViaTableNameAsyncForIdentityForEmptyTable()
    {
        // Setup
        List<CompleteTable> tables = Helper.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Before Orcle 23c, RepoDb's merge doesn't return the updated identity values, so apply offset
        long offset = V23cOrLater ? 0 : connection.Min<CompleteTable, long>(x => x.Id, where: (object?)null) - 1;

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        foreach (var table in tables)
        {
            Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id + offset));
        }
    }

    [TestMethod]
    public async Task TestOracleConnectionMergeAllAsyncAsExpandoObjectViaTableNameForIdentityForEmptyTable()
    {
        // Setup
        List<System.Dynamic.ExpandoObject> tables = Helper.CreateCompleteTablesAsExpandoObjects(10);

        using var connection = CreateConnection();
        // Act
        int result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
        Assert.AreEqual(tables.Count, result);
        Assert.IsTrue(tables.All(table => ((dynamic)table).Id > 0));

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();
        // Before Orcle 23c, RepoDb's merge doesn't return the updated identity values, so apply offset
        long offset = V23cOrLater ? 0 : connection.Min<CompleteTable, long>(x => x.Id, where: (object?)null) - 1;

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        foreach (var table in tables)
        {
            Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id + offset), table);
        }
    }

    [TestMethod]
    public async Task TestOracleConnectionMergeAllViaTableNameAsyncForIdentityForNonEmptyTable()
    {
        // Setup
        List<CompleteTable> tables = Database.CreateCompleteTables(10).AsList();

        using var connection = CreateConnection();
        // Setup
        foreach (var table in tables) { Helper.UpdateCompleteTableProperties(table); }

        // Act
        int result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        foreach (var table in tables)
        {
            Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id));
        }
    }

    [TestMethod]
    public async Task TestOracleConnectionMergeAllAsyncAsExpandoObjectViaTableNameForIdentityForNonEmptyTable()
    {
        // Setup
        List<CompleteTable> entities = Database.CreateCompleteTables(10).AsList();

        using var connection = CreateConnection();
        // Setup
        List<System.Dynamic.ExpandoObject> tables = Helper.CreateCompleteTablesAsExpandoObjects(10).AsList();
        tables.ForEach(e => ((IDictionary<string, object?>)e)["Id"] = entities[tables.IndexOf(e)].Id);

        // Act
        int result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(entities.Count, connection.CountAll<CompleteTable>());

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(entities.Count, queryResult.Count());
        foreach (var table in tables)
        {
            Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table);
        }
    }

    [TestMethod]
    public async Task TestOracleConnectionMergeAllViaTableNameAsyncForIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        List<CompleteTable> tables = Database.CreateCompleteTables(10).AsList();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using var connection = CreateConnection();
        // Setup
        foreach (var table in tables) { Helper.UpdateCompleteTableProperties(table); }

        // Act
        int result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            tables,
            qualifiers, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        foreach (var table in tables)
        {
            Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id));
        }
    }

    [TestMethod]
    public async Task TestOracleConnectionMergeAllAsyncAsDynamicsViaTableNameForIdentityForEmptyTable()
    {
        // Setup
        List<dynamic> tables = Helper.CreateCompleteTablesAsDynamics(10);

        using var connection = CreateConnection();
        // Act
        int result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();
        // Before Orcle 23c, RepoDb's merge doesn't return the updated identity values, so apply offset
        long offset = V23cOrLater ? 0 : connection.Min<CompleteTable, long>(x => x.Id, where: (object?)null) - 1;

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        foreach (var table in tables)
        {
            Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id + offset));
        }
    }

    [TestMethod]
    public async Task TestOracleConnectionMergeAllAsyncAsDynamicsViaTableNameForIdentityForNonEmptyTable()
    {
        // Setup
        List<CompleteTable> tables = Database.CreateCompleteTables(10).AsList();

        using var connection = CreateConnection();
        // Setup
        foreach (var table in tables) { Helper.UpdateCompleteTableProperties(table); }

        // Act
        int result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        foreach (var table in tables)
        {
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id));
        }
    }

    [TestMethod]
    public async Task TestOracleConnectionMergeAllAsyncAsDynamicsViaTableNameForIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        List<CompleteTable> tables = Database.CreateCompleteTables(10).AsList();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using var connection = CreateConnection();
        // Setup
        foreach (var table in tables) { Helper.UpdateCompleteTableProperties(table); }

        // Act
        int result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            tables,
            qualifiers, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        foreach (var table in tables)
        {
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id));
        }
    }

    [TestMethod]
    public async Task TestOracleConnectionMergeAllAsyncViaTableNameForNonIdentityForEmptyTable()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Helper.CreateNonIdentityCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        int result = await connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        foreach (var table in tables)
        {
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id));
        }
    }

    [TestMethod]
    public async Task TestOracleConnectionMergeAllAsyncViaTableNameForNonIdentityForNonEmptyTable()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Database.CreateNonIdentityCompleteTables(10).AsList();

        using var connection = CreateConnection();
        // Setup
        foreach (var table in tables) Helper.UpdateNonIdentityCompleteTableProperties(table);

        // Act
        int result = await connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        foreach (var table in tables)
        {
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id));
        }
    }

    [TestMethod]
    public async Task TestOracleConnectionMergeAllAsyncViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Database.CreateNonIdentityCompleteTables(10).AsList();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using var connection = CreateConnection();
        // Setup
        foreach (var table in tables) Helper.UpdateNonIdentityCompleteTableProperties(table);

        // Act
        int result = await connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables,
            qualifiers, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        foreach (var table in tables)
        {
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id));
        }
    }

    [TestMethod]
    public async Task TestOracleConnectionMergeAllAsyncAsDynamicsViaTableNameForNonIdentityForEmptyTable()
    {
        // Setup
        List<dynamic> tables = Helper.CreateNonIdentityCompleteTablesAsDynamics(10);

        using var connection = CreateConnection();
        // Act
        int result = await connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        foreach (var table in tables)
        {
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id));
        }
    }

    [TestMethod]
    public async Task TestOracleConnectionMergeAllAsyncAsDynamicsViaTableNameForNonIdentityForNonEmptyTable()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Database.CreateNonIdentityCompleteTables(10).AsList();

        using var connection = CreateConnection();
        // Setup
        foreach (var table in tables) Helper.UpdateNonIdentityCompleteTableProperties(table);

        // Act
        int result = await connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        foreach (var table in tables)
        {
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id));
        }
    }

    [TestMethod]
    public async Task TestOracleConnectionMergeAllAsyncAsDynamicsViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
    {
        // Setup
        List<NonIdentityCompleteTable> tables = Database.CreateNonIdentityCompleteTables(10).AsList();
        Field[] qualifiers = new[]
        {
            new Field("Id", typeof(long))
        };

        using var connection = CreateConnection();
        // Setup
        foreach (var table in tables) Helper.UpdateNonIdentityCompleteTableProperties(table);

        // Act
        int result = await connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
            tables,
            qualifiers, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

        // Act
        IEnumerable<NonIdentityCompleteTable> queryResult = connection.QueryAll<NonIdentityCompleteTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        foreach (var table in tables)
        {
            Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id));
        }
    }

    #endregion

    #endregion
}
