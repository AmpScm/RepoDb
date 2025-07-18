using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Enumerations;
using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.PostgreSql.IntegrationTests.Operations;

[TestClass]
public class AverageTest
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
    public void TestPostgreSqlConnectionAverageWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = connection.Average<CompleteTable>(e => e.ColumnInteger,
            (object?)null);

        // Assert
        Assert.AreEqual(tables.Average(e => e.ColumnInteger), Convert.ToDouble(result));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionAverageWithExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        long[] ids = new[] { tables.First().Id, tables.Last().Id };
        object result = connection.Average<CompleteTable>(e => e.ColumnInteger,
            e => ids.Contains(e.Id));

        // Assert
        Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Average(e => e.ColumnInteger), Convert.ToDouble(result));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionAverageWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.Average<CompleteTable>(e => e.ColumnInteger,
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionAverageAsyncWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = await connection.AverageAsync<CompleteTable>(e => e.ColumnInteger,
            (object?)null);

        // Assert
        Assert.AreEqual(tables.Average(e => e.ColumnInteger), Convert.ToDouble(result));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionAverageAsyncWithExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        long[] ids = new[] { tables.First().Id, tables.Last().Id };
        object result = await connection.AverageAsync<CompleteTable>(e => e.ColumnInteger,
            e => ids.Contains(e.Id));

        // Assert
        Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Average(e => e.ColumnInteger), Convert.ToDouble(result));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionAverageAsyncWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.AverageAsync<CompleteTable>(e => e.ColumnInteger,
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestPostgreSqlConnectionAverageViaTableNameWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = connection.Average(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInteger).First(),
            (object?)null);

        // Assert
        Assert.AreEqual(tables.Average(e => e.ColumnInteger), Convert.ToDouble(result));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionAverageViaTableNameWithExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        long[] ids = new[] { tables.First().Id, tables.Last().Id };
        object result = connection.Average(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInteger).First(),
            new QueryField("Id", Operation.In, ids));

        // Assert
        Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Average(e => e.ColumnInteger), Convert.ToDouble(result));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionAverageViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.Average(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInteger).First(),
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionAverageAsyncViaTableNameWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = await connection.AverageAsync(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInteger).First(),
            (object?)null);

        // Assert
        Assert.AreEqual(tables.Average(e => e.ColumnInteger), Convert.ToDouble(result));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionAverageAsyncViaTableNameWithExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        long[] ids = new[] { tables.First().Id, tables.Last().Id };
        object result = await connection.AverageAsync(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInteger).First(),
            new QueryField("Id", Operation.In, ids));

        // Assert
        Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Average(e => e.ColumnInteger), Convert.ToDouble(result));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionAverageAsyncViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.AverageAsync(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInteger).First(),
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #endregion
}
