using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Extensions;
using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.PostgreSql.IntegrationTests.Operations;

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
    public void TestPostgreSqlConnectionQueryViaPrimaryKey()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        CompleteTable result = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionQueryViaExpression()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        CompleteTable result = connection.Query<CompleteTable>(e => e.Id == table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionQueryViaDynamic()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        CompleteTable result = connection.Query<CompleteTable>(new { table.Id }).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionQueryViaQueryField()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        CompleteTable result = connection.Query<CompleteTable>(new QueryField("Id", table.Id)).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionQueryViaQueryFields()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInteger", table.ColumnInteger)
        };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        CompleteTable result = connection.Query<CompleteTable>(queryFields).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionQueryViaQueryGroup()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInteger", table.ColumnInteger)
        };
        QueryGroup queryGroup = new QueryGroup(queryFields);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        CompleteTable result = connection.Query<CompleteTable>(queryGroup).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionQueryWithTop()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
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

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.Query<CompleteTable>((object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionQueryAsyncViaPrimaryKey()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        CompleteTable result = (await connection.QueryAsync<CompleteTable>(table.Id, cancellationToken: TestContext.CancellationToken)).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionQueryAsyncViaExpression()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        CompleteTable result = (await connection.QueryAsync<CompleteTable>(e => e.Id == table.Id, cancellationToken: TestContext.CancellationToken)).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionQueryAsyncViaDynamic()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        CompleteTable result = (await connection.QueryAsync<CompleteTable>(new { table.Id }, cancellationToken: TestContext.CancellationToken)).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionQueryAsyncViaQueryField()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        CompleteTable result = (await connection.QueryAsync<CompleteTable>(new QueryField("Id", table.Id), cancellationToken: TestContext.CancellationToken)).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionQueryAsyncViaQueryFields()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInteger", table.ColumnInteger)
        };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        CompleteTable result = (await connection.QueryAsync<CompleteTable>(queryFields, cancellationToken: TestContext.CancellationToken)).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionQueryAsyncViaQueryGroup()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInteger", table.ColumnInteger)
        };
        QueryGroup queryGroup = new QueryGroup(queryFields);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        CompleteTable result = (await connection.QueryAsync<CompleteTable>(queryGroup, cancellationToken: TestContext.CancellationToken)).First();

        // Assert
        Helper.AssertPropertiesEquality(table, result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionQueryAsyncWithTop()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
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

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.QueryAsync<CompleteTable>((object?)null,
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestPostgreSqlConnectionQueryViaTableNameViaPrimaryKey()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        dynamic result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), table.Id).First();

        // Assert
        Helper.AssertMembersEquality(table, result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionQueryViaTableNameViaDynamic()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        dynamic result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), new { table.Id }).First();

        // Assert
        Helper.AssertMembersEquality(table, result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionQueryViaTableNameViaQueryField()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        dynamic result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", table.Id)).First();

        // Assert
        Helper.AssertMembersEquality(table, result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionQueryViaTableNameViaQueryFields()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInteger", table.ColumnInteger)
        };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        dynamic result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), queryFields).First();

        // Assert
        Helper.AssertMembersEquality(table, result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionQueryViaTableNameViaQueryGroup()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInteger", table.ColumnInteger)
        };
        QueryGroup queryGroup = new QueryGroup(queryFields);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        dynamic result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), queryGroup).First();

        // Assert
        Helper.AssertMembersEquality(table, result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionQueryViaTableNameWithTop()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
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

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.Query(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionQueryAsyncViaTableNameViaPrimaryKey()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        dynamic result = (await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), table.Id, cancellationToken: TestContext.CancellationToken)).First();

        // Assert
        Helper.AssertMembersEquality(table, result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionQueryAsyncViaTableNameViaDynamic()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        dynamic result = (await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), new { table.Id }, cancellationToken: TestContext.CancellationToken)).First();

        // Assert
        Helper.AssertMembersEquality(table, result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionQueryAsyncViaTableNameViaQueryField()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        dynamic result = (await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", table.Id), cancellationToken: TestContext.CancellationToken)).First();

        // Assert
        Helper.AssertMembersEquality(table, result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionQueryAsyncViaTableNameViaQueryFields()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInteger", table.ColumnInteger)
        };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        dynamic result = (await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), queryFields, cancellationToken: TestContext.CancellationToken)).First();

        // Assert
        Helper.AssertMembersEquality(table, result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionQueryAsyncViaTableNameViaQueryGroup()
    {
        // Setup
        CompleteTable table = Database.CreateCompleteTables(1).First();
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", table.Id),
            new QueryField("ColumnInteger", table.ColumnInteger)
        };
        QueryGroup queryGroup = new QueryGroup(queryFields);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        dynamic result = (await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), queryGroup, cancellationToken: TestContext.CancellationToken)).First();

        // Assert
        Helper.AssertMembersEquality(table, result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionQueryAsyncViaTableNameWithTop()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
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

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null,
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    public TestContext TestContext { get; set; }

    #endregion

    #endregion
}
