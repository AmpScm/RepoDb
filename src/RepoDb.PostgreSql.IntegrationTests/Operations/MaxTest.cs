using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Enumerations;
using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.PostgreSql.IntegrationTests.Operations;

[TestClass]
public class MaxTest
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
    public void TestPostgreSqlConnectionMaxWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = connection.Max<CompleteTable>(e => e.ColumnInteger,
            (object?)null);

        // Assert
        Assert.AreEqual(tables.Max(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMaxViaExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        long[] ids = new[] { tables.First().Id, tables.Last().Id };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = connection.Max<CompleteTable>(e => e.ColumnInteger,
            e => ids.Contains(e.Id));

        // Assert
        Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Max(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMaxViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = connection.Max<CompleteTable>(e => e.ColumnInteger,
            new { tables.First().Id });

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMaxViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = connection.Max<CompleteTable>(e => e.ColumnInteger,
            new QueryField("Id", tables.First().Id));

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMaxViaQueryFields()
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
        object result = connection.Max<CompleteTable>(e => e.ColumnInteger,
            queryFields);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMaxViaQueryGroup()
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
        object result = connection.Max<CompleteTable>(e => e.ColumnInteger,
            queryGroup);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlConnectionMaxWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.Max<CompleteTable>(e => e.ColumnInteger,
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionMaxAsyncWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = await connection.MaxAsync<CompleteTable>(e => e.ColumnInteger,
            (object?)null);

        // Assert
        Assert.AreEqual(tables.Max(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMaxAsyncViaExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        long[] ids = new[] { tables.First().Id, tables.Last().Id };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = await connection.MaxAsync<CompleteTable>(e => e.ColumnInteger,
            e => ids.Contains(e.Id));

        // Assert
        Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Max(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMaxAsyncViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = await connection.MaxAsync<CompleteTable>(e => e.ColumnInteger,
            new { tables.First().Id });

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMaxAsyncViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = await connection.MaxAsync<CompleteTable>(e => e.ColumnInteger,
            new QueryField("Id", tables.First().Id));

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMaxAsyncViaQueryFields()
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
        object result = await connection.MaxAsync<CompleteTable>(e => e.ColumnInteger,
            queryFields);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMaxAsyncViaQueryGroup()
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
        object result = await connection.MaxAsync<CompleteTable>(e => e.ColumnInteger,
            queryGroup);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task ThrowExceptionOnPostgreSqlConnectionMaxAsyncWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.MaxAsync<CompleteTable>(e => e.ColumnInteger,
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestPostgreSqlConnectionMaxViaTableNameWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = connection.Max(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInteger", typeof(int)),
            (object?)null);

        // Assert
        Assert.AreEqual(tables.Max(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMaxViaTableNameViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = connection.Max(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInteger", typeof(int)),
            new { tables.First().Id });

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMaxViaTableNameViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = connection.Max(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInteger", typeof(int)),
            new QueryField("Id", tables.First().Id));

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMaxViaTableNameViaQueryFields()
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
        object result = connection.Max(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInteger", typeof(int)),
            queryFields);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionMaxViaTableNameViaQueryGroup()
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
        object result = connection.Max(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInteger", typeof(int)),
            queryGroup);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlConnectionMaxViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.Max(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInteger", typeof(int)),
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionMaxAsyncViaTableNameWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = await connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInteger", typeof(int)),
            (object?)null);

        // Assert
        Assert.AreEqual(tables.Max(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMaxAsyncViaTableNameViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = await connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInteger", typeof(int)),
            new { tables.First().Id });

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMaxAsyncViaTableNameViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = await connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInteger", typeof(int)),
            new QueryField("Id", tables.First().Id));

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMaxAsyncViaTableNameViaQueryFields()
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
        object result = await connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInteger", typeof(int)),
            queryFields);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionMaxAsyncViaTableNameViaQueryGroup()
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
        object result = await connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInteger", typeof(int)),
            queryGroup);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task ThrowExceptionOnPostgreSqlConnectionMaxAsyncViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInteger", typeof(int)),
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #endregion
}
