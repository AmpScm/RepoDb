using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Enumerations;
using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.PostgreSql.IntegrationTests.Operations;

[TestClass]
public class SumTest
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
    public void TestPostgreSqlConnectionSumWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = connection.Sum<CompleteTable>(e => e.ColumnInteger,
            (object?)null);

        // Assert
        Assert.AreEqual(tables.Sum(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionSumViaExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        long[] ids = new[] { tables.First().Id, tables.Last().Id };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = connection.Sum<CompleteTable>(e => e.ColumnInteger,
            e => ids.Contains(e.Id));

        // Assert
        Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionSumViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = connection.Sum<CompleteTable>(e => e.ColumnInteger,
            new { tables.First().Id });

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionSumViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = connection.Sum<CompleteTable>(e => e.ColumnInteger,
            new QueryField("Id", tables.First().Id));

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionSumViaQueryFields()
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
        object result = connection.Sum<CompleteTable>(e => e.ColumnInteger,
            queryFields);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionSumViaQueryGroup()
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
        object result = connection.Sum<CompleteTable>(e => e.ColumnInteger,
            queryGroup);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlConnectionSumWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.Sum<CompleteTable>(e => e.ColumnInteger,
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionSumAsyncWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = await connection.SumAsync<CompleteTable>(e => e.ColumnInteger,
            (object?)null);

        // Assert
        Assert.AreEqual(tables.Sum(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionSumAsyncViaExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        long[] ids = new[] { tables.First().Id, tables.Last().Id };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = await connection.SumAsync<CompleteTable>(e => e.ColumnInteger,
            e => ids.Contains(e.Id));

        // Assert
        Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionSumAsyncViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = await connection.SumAsync<CompleteTable>(e => e.ColumnInteger,
            new { tables.First().Id });

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionSumAsyncViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = await connection.SumAsync<CompleteTable>(e => e.ColumnInteger,
            new QueryField("Id", tables.First().Id));

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionSumAsyncViaQueryFields()
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
        object result = await connection.SumAsync<CompleteTable>(e => e.ColumnInteger,
            queryFields);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionSumAsyncViaQueryGroup()
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
        object result = await connection.SumAsync<CompleteTable>(e => e.ColumnInteger,
            queryGroup);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task ThrowExceptionOnPostgreSqlConnectionSumAsyncWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.SumAsync<CompleteTable>(e => e.ColumnInteger,
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestPostgreSqlConnectionSumViaTableNameWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = connection.Sum(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInteger", typeof(int)),
            (object?)null);

        // Assert
        Assert.AreEqual(tables.Sum(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionSumViaTableNameViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = connection.Sum(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInteger", typeof(int)),
            new { tables.First().Id });

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionSumViaTableNameViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = connection.Sum(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInteger", typeof(int)),
            new QueryField("Id", tables.First().Id));

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionSumViaTableNameViaQueryFields()
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
        object result = connection.Sum(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInteger", typeof(int)),
            queryFields);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionSumViaTableNameViaQueryGroup()
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
        object result = connection.Sum(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInteger", typeof(int)),
            queryGroup);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlConnectionSumViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.Sum(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInteger", typeof(int)),
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionSumAsyncViaTableNameWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = await connection.SumAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInteger", typeof(int)),
            (object?)null);

        // Assert
        Assert.AreEqual(tables.Sum(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionSumAsyncViaTableNameViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = await connection.SumAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInteger", typeof(int)),
            new { tables.First().Id });

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionSumAsyncViaTableNameViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = await connection.SumAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInteger", typeof(int)),
            new QueryField("Id", tables.First().Id));

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionSumAsyncViaTableNameViaQueryFields()
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
        object result = await connection.SumAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInteger", typeof(int)),
            queryFields);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionSumAsyncViaTableNameViaQueryGroup()
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
        object result = await connection.SumAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInteger", typeof(int)),
            queryGroup);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task ThrowExceptionOnPostgreSqlConnectionSumAsyncViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.SumAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInteger", typeof(int)),
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #endregion
}
