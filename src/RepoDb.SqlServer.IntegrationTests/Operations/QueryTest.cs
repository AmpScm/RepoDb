using System.Globalization;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.SqlServer.IntegrationTests.Models;
using RepoDb.SqlServer.IntegrationTests.Setup;
using RepoDb.Trace;

namespace RepoDb.SqlServer.IntegrationTests.Operations;

[TestClass]
public class QueryTest
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
    public void TestSqlServerConnectionQueryViaPrimaryKey()
    {
        // Setup
        var table = Database.CreateCompleteTables(1).First();

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public void TestSqlServerConnectionQueryViaExpression()
    {
        // Setup
        var table = Database.CreateCompleteTables(1).First();

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.Query<CompleteTable>(e => e.Id == table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public void TestSqlServerConnectionQueryViaDynamic()
    {
        // Setup
        var table = Database.CreateCompleteTables(1).First();

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.Query<CompleteTable>(new { table.Id }).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public void TestSqlServerConnectionQueryViaQueryField()
    {
        // Setup
        var table = Database.CreateCompleteTables(1).First();

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.Query<CompleteTable>(new QueryField("Id", table.Id)).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public void TestSqlServerConnectionQueryViaQueryFields()
    {
        // Setup
        var table = Database.CreateCompleteTables(1).First();
        var queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInt", table.ColumnInt)
        };

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.Query<CompleteTable>(queryFields).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public void TestSqlServerConnectionQueryViaQueryGroup()
    {
        // Setup
        var table = Database.CreateCompleteTables(1).First();
        var queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInt", table.ColumnInt)
        };
        var queryGroup = new QueryGroup(queryFields);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.Query<CompleteTable>(queryGroup).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public void TestSqlServerConnectionQueryWithTop()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.Query<CompleteTable>((object?)null,
            top: 2);

        // Assert
        Assert.AreEqual(2, result.Count());
        result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
    }

    [TestMethod]
    public void TestSqlServerConnectionQueryViaPrimaryKeyWithHints()
    {
        // Setup
        var table = Database.CreateCompleteTables(1).First();

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.Query<CompleteTable>(table.Id,
            hints: SqlServerTableHints.NoLock).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqlServerConnectionQueryAsyncViaPrimaryKey()
    {
        // Setup
        var table = Database.CreateCompleteTables(1).First();

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = (await connection.QueryAsync<CompleteTable>(table.Id, cancellationToken: TestContext.CancellationToken)).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public async Task TestSqlServerConnectionQueryAsyncViaExpression()
    {
        // Setup
        var table = Database.CreateCompleteTables(1).First();

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = (await connection.QueryAsync<CompleteTable>(e => e.Id == table.Id, cancellationToken: TestContext.CancellationToken)).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public async Task TestSqlServerConnectionQueryAsyncViaDynamic()
    {
        // Setup
        var table = Database.CreateCompleteTables(1).First();

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = (await connection.QueryAsync<CompleteTable>(new { table.Id }, cancellationToken: TestContext.CancellationToken)).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public async Task TestSqlServerConnectionQueryAsyncViaQueryField()
    {
        // Setup
        var table = Database.CreateCompleteTables(1).First();

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = (await connection.QueryAsync<CompleteTable>(new QueryField("Id", table.Id), cancellationToken: TestContext.CancellationToken)).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public async Task TestSqlServerConnectionQueryAsyncViaQueryFields()
    {
        // Setup
        var table = Database.CreateCompleteTables(1).First();
        var queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInt", table.ColumnInt)
        };

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = (await connection.QueryAsync<CompleteTable>(queryFields, cancellationToken: TestContext.CancellationToken)).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public async Task TestSqlServerConnectionQueryAsyncViaQueryGroup()
    {
        // Setup
        var table = Database.CreateCompleteTables(1).First();
        var queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInt", table.ColumnInt)
        };
        var queryGroup = new QueryGroup(queryFields);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = (await connection.QueryAsync<CompleteTable>(queryGroup, cancellationToken: TestContext.CancellationToken)).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public async Task TestSqlServerConnectionQueryAsyncWithTop()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = await connection.QueryAsync<CompleteTable>((object?)null,
            top: 2, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(2, result.Count());
        result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
    }

    [TestMethod]
    public async Task TestSqlServerConnectionQueryAsyncViaPrimaryKeyWithHints()
    {
        // Setup
        var table = Database.CreateCompleteTables(1).First();

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = (await connection.QueryAsync<CompleteTable>(table.Id,
            hints: SqlServerTableHints.NoLock, cancellationToken: TestContext.CancellationToken)).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestSqlServerConnectionQueryViaTableNameViaPrimaryKey()
    {
        // Setup
        var table = Database.CreateCompleteTables(1).First();

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), table.Id).First();

        // Assert
        Helper.AssertMembersEquality(table, result);
    }

    [TestMethod]
    public void TestSqlServerConnectionQueryViaTableNameViaDynamic()
    {
        // Setup
        var table = Database.CreateCompleteTables(1).First();

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), new { table.Id }).First();

        // Assert
        Helper.AssertMembersEquality(table, result);
    }

    [TestMethod]
    public void TestSqlServerConnectionQueryViaTableNameViaQueryField()
    {
        // Setup
        var table = Database.CreateCompleteTables(1).First();

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", table.Id)).First();

        // Assert
        Helper.AssertMembersEquality(table, result);
    }

    [TestMethod]
    public void TestSqlServerConnectionQueryViaTableNameViaQueryFields()
    {
        // Setup
        var table = Database.CreateCompleteTables(1).First();
        var queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInt", table.ColumnInt)
        };

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), queryFields).First();

        // Assert
        Helper.AssertMembersEquality(table, result);
    }

    [TestMethod]
    public void TestSqlServerConnectionQueryViaTableNameViaQueryGroup()
    {
        // Setup
        var table = Database.CreateCompleteTables(1).First();
        var queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInt", table.ColumnInt)
        };
        var queryGroup = new QueryGroup(queryFields);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), queryGroup).First();

        // Assert
        Helper.AssertMembersEquality(table, result);
    }

    [TestMethod]
    public void TestSqlServerConnectionQueryViaTableNameWithTop()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null,
            top: 2);

        // Assert
        Assert.AreEqual(2, result.Count());
        result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
    }

    [TestMethod]
    public void TestSqlServerConnectionQueryViaTableNameViaPrimaryKeyWithHints()
    {
        // Setup
        var table = Database.CreateCompleteTables(1).First();

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(),
            table.Id,
            hints: SqlServerTableHints.NoLock).First();

        // Assert
        Helper.AssertMembersEquality(table, result);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqlServerConnectionQueryAsyncViaTableNameViaPrimaryKey()
    {
        // Setup
        var table = Database.CreateCompleteTables(1).First();

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = (await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), table.Id, cancellationToken: TestContext.CancellationToken)).First();

        // Assert
        Helper.AssertMembersEquality(table, result);
    }

    [TestMethod]
    public async Task TestSqlServerConnectionQueryAsyncViaTableNameViaDynamic()
    {
        // Setup
        var table = Database.CreateCompleteTables(1).First();

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = (await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), new { table.Id }, cancellationToken: TestContext.CancellationToken)).First();

        // Assert
        Helper.AssertMembersEquality(table, result);
    }

    [TestMethod]
    public async Task TestSqlServerConnectionQueryAsyncViaTableNameViaQueryField()
    {
        // Setup
        var table = Database.CreateCompleteTables(1).First();

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = (await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", table.Id), cancellationToken: TestContext.CancellationToken)).First();

        // Assert
        Helper.AssertMembersEquality(table, result);
    }

    [TestMethod]
    public async Task TestSqlServerConnectionQueryAsyncViaTableNameViaQueryFields()
    {
        // Setup
        var table = Database.CreateCompleteTables(1).First();
        var queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInt", table.ColumnInt)
        };

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = (await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), queryFields, cancellationToken: TestContext.CancellationToken)).First();

        // Assert
        Helper.AssertMembersEquality(table, result);
    }

    [TestMethod]
    public async Task TestSqlServerConnectionQueryAsyncViaTableNameViaQueryGroup()
    {
        // Setup
        var table = Database.CreateCompleteTables(1).First();
        var queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInt", table.ColumnInt)
        };
        var queryGroup = new QueryGroup(queryFields);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = (await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), queryGroup, cancellationToken: TestContext.CancellationToken)).First();

        // Assert
        Helper.AssertMembersEquality(table, result);
    }

    [TestMethod]
    public async Task TestSqlServerConnectionQueryAsyncViaTableNameWithTop()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null,
            top: 2, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(2, result.Count());
        result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
    }

    [TestMethod]
    public async Task TestSqlServerConnectionQueryAsyncViaTableNameViaPrimaryKeyWithHints()
    {
        // Setup
        var table = Database.CreateCompleteTables(1).First();

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = (await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
            table.Id,
            hints: SqlServerTableHints.NoLock, cancellationToken: TestContext.CancellationToken)).First();

        // Assert
        Helper.AssertMembersEquality(table, result);
    }

    #endregion

    #endregion

    #region TVP
    #region Sync
    [TestMethod]
    public void TestTVPSupport()
    {
        var table = Database.CreateCompleteTables(1).First();
        using var connection = new SqlConnection(Database.ConnectionString).EnsureOpen();

        var info = DbRuntimeSettingCache.Get(connection, null);

        Assert.IsNotNull(info);
        Assert.IsTrue(info.ParameterTypeMap.ContainsKey(typeof(int)), "INT TVP defined");
        Assert.IsTrue(info.ParameterTypeMap.ContainsKey(typeof(int?)), "INT? TVP defined");

        foreach (var n in new int[] { 12, 100 })
        {
            var values = Enumerable.Range(0, n).Concat([table.Id]);

            var items = connection.ExecuteQuery<CompleteTable>(
                "SELECT * FROM [dbo].[CompleteTable] WHERE Id IN (@Ids)",
                new
                {
                    Ids = values
                }, trace: new DiagnosticsTracer());


            Assert.AreEqual(1, items.Count());


            var strvalues = Enumerable.Range(0, n).Select(x => x.ToString(CultureInfo.InvariantCulture)).Concat([table.ColumnVarChar]);

            items = connection.ExecuteQuery<CompleteTable>(
                "SELECT * FROM [dbo].[CompleteTable] WHERE ColumnVarChar IN (@StrIds)",
                new
                {
                    StrIds = strvalues
                }, trace: new DiagnosticsTracer());


            Assert.AreEqual(1, items.Count());


            var longValues = Enumerable.Range(0, n).Select(x => (long)x).Concat([table.ColumnBigInt]);

            items = connection.ExecuteQuery<CompleteTable>(
                "SELECT * FROM [dbo].[CompleteTable] WHERE ColumnBigInt IN (@longIds)",
                new
                {
                    longIds = longValues
                }, trace: new DiagnosticsTracer());


            Assert.AreEqual(1, items.Count());


            var longNullValues = Enumerable.Range(0, n).Select(x => (long)x).Concat([table.ColumnBigInt]).Select(x => (long?)x).Append(null);

            items = connection.ExecuteQuery<CompleteTable>(
                "SELECT * FROM [dbo].[CompleteTable] WHERE ColumnBigInt IN (@longNullIds)",
                new
                {
                    longNullIds = longNullValues
                }, trace: new DiagnosticsTracer());


            Assert.AreEqual(1, items.Count());
        }
    }

    [TestMethod]
    public void TestValues30Support()
    {
        var table = Database.CreateCompleteTables(1).First();
        using var connection = new SqlConnection(Database.ConnectionString).EnsureOpen();

        var info = DbRuntimeSettingCache.Get(connection, null);

        Assert.IsNotNull(info);
        Assert.IsTrue(info.ParameterTypeMap.ContainsKey(typeof(int)), "INT TVP defined");
        Assert.IsTrue(info.ParameterTypeMap.ContainsKey(typeof(int?)), "INT? TVP defined");

        var values = Enumerable.Range(0, 30).Concat([table.Id]);

        var items = connection.Query<CompleteTable>(where: x => values.Contains(x.Id), fields: Field.Parse<CompleteTable>(x => x.ColumnDate), trace: new DiagnosticsTracer());

        Assert.AreEqual(1, items.Count());
    }

    [TestMethod]
    public void TestValues8Support()
    {
        var table = Database.CreateCompleteTables(1).First();
        using var connection = new SqlConnection(Database.ConnectionString).EnsureOpen();

        var info = DbRuntimeSettingCache.Get(connection, null);

        Assert.IsNotNull(info);
        Assert.IsTrue(info.ParameterTypeMap.ContainsKey(typeof(int)), "INT TVP defined");
        Assert.IsTrue(info.ParameterTypeMap.ContainsKey(typeof(int?)), "INT? TVP defined");

        var values = Enumerable.Range(0, 8).Concat([table.Id]);

        var items = connection.Query<CompleteTable>(where: x => values.Contains(x.Id), fields: Field.Parse<CompleteTable>(x => x.ColumnDate), trace: new DiagnosticsTracer());

        Assert.AreEqual(1, items.Count());
    }

    #endregion
    #region Async
    [TestMethod]
    public async Task TestTVPSupportAsync()
    {
        var table = Database.CreateCompleteTables(1).First();
        using var connection = new SqlConnection(Database.ConnectionString).EnsureOpen();


        foreach (var n in new int[] { 12, 100 })
        {
            var values = Enumerable.Range(0, n).Concat([table.Id]);

            var items = await connection.ExecuteQueryAsync<CompleteTable>(
                "SELECT * FROM [dbo].[CompleteTable] WHERE Id IN (@Ids)",
                new
                {
                    Ids = values
                }, trace: new DiagnosticsTracer(), cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(1, items.Count());

            var strvalues = Enumerable.Range(0, n).Select(x => x.ToString(CultureInfo.InvariantCulture)).Concat([table.ColumnVarChar]);

            items = await connection.ExecuteQueryAsync<CompleteTable>(
                "SELECT * FROM [dbo].[CompleteTable] WHERE ColumnVarChar IN (@StrIds)",
                new
                {
                    StrIds = strvalues
                }, trace: new DiagnosticsTracer(), cancellationToken: TestContext.CancellationToken);


            Assert.AreEqual(1, items.Count());
        }
    }

    [TestMethod]
    public async Task TestValuesSupportAsync()
    {
        var table = Database.CreateCompleteTables(1).First();
        using var connection = new SqlConnection(Database.ConnectionString).EnsureOpen();

        var values = Enumerable.Range(0, 30).Concat([table.Id]);

        var items = await connection.QueryAsync<CompleteTable>(where: x => values.Contains(x.Id), fields: Field.Parse<CompleteTable>(x => x.ColumnDate), trace: new DiagnosticsTracer(), cancellationToken: TestContext.CancellationToken);

        Assert.AreEqual(1, items.Count());
    }

    public TestContext TestContext { get; set; }
    #endregion
    #endregion
}
