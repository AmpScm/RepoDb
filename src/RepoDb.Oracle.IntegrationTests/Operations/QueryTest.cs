using RepoDb.Extensions;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;
using RepoDb.Trace;

namespace RepoDb.Oracle.IntegrationTests.Operations;

[TestClass]
public class QueryTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestOracleConnectionQueryViaPrimaryKey()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Act
        CompleteTable result = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public void TestOracleConnectionQueryViaExpression()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Act
        CompleteTable result = connection.Query<CompleteTable>(e => e.Id == table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public void TestOracleConnectionQueryViaDynamic()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Act
        CompleteTable result = connection.Query<CompleteTable>(new { table.Id }).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public void TestOracleConnectionQueryViaQueryField()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Act
        CompleteTable result = connection.Query<CompleteTable>(new QueryField("Id", table.Id)).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public void TestOracleConnectionQueryViaQueryFields()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInt", table.ColumnInt)
        };

        using var connection = CreateConnection();
        // Act
        CompleteTable result = connection.Query<CompleteTable>(queryFields).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public void TestOracleConnectionQueryViaQueryGroup()
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
        // Act
        CompleteTable result = connection.Query<CompleteTable>(queryGroup).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public void TestOracleConnectionQueryWithTop()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        IEnumerable<CompleteTable> result = connection.Query<CompleteTable>((object?)null,
            top: 2);

        // Assert
        Assert.AreEqual(2, result.Count());
        result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
    }

    [TestMethod]
    public void ThrowExceptionQueryWithHints()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.Query<CompleteTable>((object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestOracleConnectionQueryAsyncViaPrimaryKey()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Act
        CompleteTable result = (await connection.QueryAsync<CompleteTable>(table.Id, cancellationToken: TestContext.CancellationToken)).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public async Task TestOracleConnectionQueryAsyncViaExpression()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Act
        CompleteTable result = (await connection.QueryAsync<CompleteTable>(e => e.Id == table.Id, cancellationToken: TestContext.CancellationToken)).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public async Task TestOracleConnectionQueryAsyncViaDynamic()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Act
        CompleteTable result = (await connection.QueryAsync<CompleteTable>(new { table.Id }, cancellationToken: TestContext.CancellationToken)).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public async Task TestOracleConnectionQueryAsyncViaQueryField()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Act
        CompleteTable result = (await connection.QueryAsync<CompleteTable>(new QueryField("Id", table.Id), cancellationToken: TestContext.CancellationToken)).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public async Task TestOracleConnectionQueryAsyncViaQueryFields()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInt", table.ColumnInt)
        };

        using var connection = CreateConnection();
        // Act
        CompleteTable result = (await connection.QueryAsync<CompleteTable>(queryFields, cancellationToken: TestContext.CancellationToken, trace: new DiagnosticsTracer())).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public async Task TestOracleConnectionQueryAsyncViaQueryGroup()
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
        // Act
        CompleteTable result = (await connection.QueryAsync<CompleteTable>(queryGroup, cancellationToken: TestContext.CancellationToken)).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public async Task TestOracleConnectionQueryAsyncWithTop()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        IEnumerable<CompleteTable> result = await connection.QueryAsync<CompleteTable>((object?)null,
            top: 2, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(2, result.Count());
        result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
    }

    [TestMethod]
    public async Task ThrowExceptionQueryAsyncWithHints()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.QueryAsync<CompleteTable>((object?)null,
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestOracleConnectionQueryViaTableNameViaPrimaryKey()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Act
        dynamic result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), table.Id).First();

        // Assert
        Helper.AssertMembersEquality(table, result);
    }

    [TestMethod]
    public void TestOracleConnectionQueryViaTableNameViaDynamic()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Act
        dynamic result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), new { table.Id }).First();

        // Assert
        Helper.AssertMembersEquality(table, result);
    }

    [TestMethod]
    public void TestOracleConnectionQueryViaTableNameViaQueryField()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Act
        dynamic result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", table.Id)).First();

        // Assert
        Helper.AssertMembersEquality(table, result);
    }

    [TestMethod]
    public void TestOracleConnectionQueryViaTableNameViaQueryFields()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInt", table.ColumnInt)
        };

        using var connection = CreateConnection();
        // Act
        dynamic result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), queryFields).First();

        // Assert
        Helper.AssertMembersEquality(table, result);
    }

    [TestMethod]
    public void TestOracleConnectionQueryViaTableNameViaQueryGroup()
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
        // Act
        dynamic result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), queryGroup).First();

        // Assert
        Helper.AssertMembersEquality(table, result);
    }

    [TestMethod]
    public void TestOracleConnectionQueryViaTableNameWithTop()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        IEnumerable<dynamic> result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null,
            top: 2);

        // Assert
        Assert.AreEqual(2, result.Count());
        result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
    }

    [TestMethod]
    public void ThrowExceptionQueryViaTableNameWithHints()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.Query(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestOracleConnectionQueryAsyncViaTableNameViaPrimaryKey()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Act
        dynamic result = (await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), table.Id, cancellationToken: TestContext.CancellationToken)).First();

        // Assert
        Helper.AssertMembersEquality(table, result);
    }

    [TestMethod]
    public async Task TestOracleConnectionQueryAsyncViaTableNameViaDynamic()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Act
        dynamic result = (await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), new { table.Id }, cancellationToken: TestContext.CancellationToken)).First();

        // Assert
        Helper.AssertMembersEquality(table, result);
    }

    [TestMethod]
    public async Task TestOracleConnectionQueryAsyncViaTableNameViaQueryField()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Act
        dynamic result = (await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", table.Id), cancellationToken: TestContext.CancellationToken)).First();

        // Assert
        Helper.AssertMembersEquality(table, result);
    }

    [TestMethod]
    public async Task TestOracleConnectionQueryAsyncViaTableNameViaQueryFields()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInt", table.ColumnInt)
        };

        using var connection = CreateConnection();
        // Act
        dynamic result = (await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), queryFields, cancellationToken: TestContext.CancellationToken)).First();

        // Assert
        Helper.AssertMembersEquality(table, result);
    }

    [TestMethod]
    public async Task TestOracleConnectionQueryAsyncViaTableNameViaQueryGroup()
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
        // Act
        dynamic result = (await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), queryGroup, cancellationToken: TestContext.CancellationToken)).First();

        // Assert
        Helper.AssertMembersEquality(table, result);
    }

    [TestMethod]
    public async Task TestOracleConnectionQueryAsyncViaTableNameWithTop()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        IEnumerable<dynamic> result = await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null,
            top: 2, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(2, result.Count());
        result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
    }

    [TestMethod]
    public async Task ThrowExceptionQueryAsyncViaTableNameWithHints()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using var connection = CreateConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null,
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion
}
