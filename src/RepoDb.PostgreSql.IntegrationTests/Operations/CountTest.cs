using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Enumerations;
using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.PostgreSql.IntegrationTests.Operations;

[TestClass]
public class CountTest
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
    public void TestPostgreSqlConnectionCountWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        long result = connection.Count<CompleteTable>((object?)null);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionCountViaExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        long[] ids = new[] { tables.First().Id, tables.Last().Id };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        long result = connection.Count<CompleteTable>(e => ids.Contains(e.Id));

        // Assert
        Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Count(), result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionCountViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        long result = connection.Count<CompleteTable>(new { tables.First().Id });

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionCountViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        long result = connection.Count<CompleteTable>(new QueryField("Id", tables.First().Id));

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionCountViaQueryFields()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        long result = connection.Count<CompleteTable>(queryFields);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionCountViaQueryGroup()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };
        QueryGroup queryGroup = new QueryGroup(queryFields);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        long result = connection.Count<CompleteTable>(queryGroup);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlConnectionCountWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.Count<CompleteTable>((object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionCountAsyncWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        long result = await connection.CountAsync<CompleteTable>((object?)null);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionCountAsyncViaExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        long[] ids = new[] { tables.First().Id, tables.Last().Id };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        long result = await connection.CountAsync<CompleteTable>(e => ids.Contains(e.Id));

        // Assert
        Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Count(), result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionCountAsyncViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        long result = await connection.CountAsync<CompleteTable>(new { tables.First().Id });

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionCountAsyncViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        long result = await connection.CountAsync<CompleteTable>(new QueryField("Id", tables.First().Id));

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionCountAsyncViaQueryFields()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        long result = await connection.CountAsync<CompleteTable>(queryFields);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionCountAsyncViaQueryGroup()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };
        QueryGroup queryGroup = new QueryGroup(queryFields);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        long result = await connection.CountAsync<CompleteTable>(queryGroup);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
    }

    [TestMethod]
    public async Task ThrowExceptionOnPostgreSqlConnectionCountAsyncWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.CountAsync<CompleteTable>((object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestPostgreSqlConnectionCountViaTableNameWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        long result = connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionCountViaTableNameViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        long result = connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
            new { tables.First().Id });

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionCountViaTableNameViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        long result = connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
            new QueryField("Id", tables.First().Id));

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionCountViaTableNameViaQueryFields()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        long result = connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
            queryFields);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionCountViaTableNameViaQueryGroup()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };
        QueryGroup queryGroup = new QueryGroup(queryFields);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        long result = connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
            queryGroup);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlConnectionCountViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionCountAsyncViaTableNameWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        long result = await connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionCountAsyncViaTableNameViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        long result = await connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new { tables.First().Id });

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionCountAsyncViaTableNameViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        long result = await connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new QueryField("Id", tables.First().Id));

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionCountAsyncViaTableNameViaQueryFields()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        long result = await connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
            queryFields);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionCountAsyncViaTableNameViaQueryGroup()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };
        QueryGroup queryGroup = new QueryGroup(queryFields);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        long result = await connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
            queryGroup);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
    }

    [TestMethod]
    public async Task ThrowExceptionOnPostgreSqlConnectionCountAsyncViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #endregion
}
