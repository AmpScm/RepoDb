using RepoDb.Enumerations;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;

namespace RepoDb.Oracle.IntegrationTests.Operations;

[TestClass]
public class AverageTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestOracleConnectionAverageWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = connection.Average<CompleteTable>(e => e.ColumnInt,
            (object?)null);

        // Assert
        Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
    }

    [TestMethod]
    public void TestOracleConnectionAverageWithExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        long[] ids = new[] { tables.First().Id, tables.Last().Id };
        object result = connection.Average<CompleteTable>(e => e.ColumnInt,
            e => ids.Contains(e.Id));

        // Assert
        Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Average(e => e.ColumnInt), Convert.ToDouble(result));
    }

    [TestMethod]
    public void TestOracleConnectionAverageWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.Average<CompleteTable>(e => e.ColumnInt,
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestOracleConnectionAverageAsyncWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = await connection.AverageAsync<CompleteTable>(e => e.ColumnInt,
            (object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
    }

    [TestMethod]
    public async Task TestOracleConnectionAverageAsyncWithExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        long[] ids = new[] { tables.First().Id, tables.Last().Id };
        object result = await connection.AverageAsync<CompleteTable>(e => e.ColumnInt,
            e => ids.Contains(e.Id), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Average(e => e.ColumnInt), Convert.ToDouble(result));
    }

    [TestMethod]
    public async Task TestOracleConnectionAverageAsyncWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.AverageAsync<CompleteTable>(e => e.ColumnInt,
            (object?)null,
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestOracleConnectionAverageViaTableNameWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = connection.Average(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
            (object?)null);

        // Assert
        Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
    }

    [TestMethod]
    public void TestOracleConnectionAverageViaTableNameWithExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        long[] ids = new[] { tables.First().Id, tables.Last().Id };
        object result = connection.Average(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
            new QueryField("Id", Operation.In, ids));

        // Assert
        Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Average(e => e.ColumnInt), Convert.ToDouble(result));
    }

    [TestMethod]
    public void TestOracleConnectionAverageViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.Average(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestOracleConnectionAverageAsyncViaTableNameWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = await connection.AverageAsync(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
            (object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
    }

    [TestMethod]
    public async Task TestOracleConnectionAverageAsyncViaTableNameWithExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        long[] ids = new[] { tables.First().Id, tables.Last().Id };
        object result = await connection.AverageAsync(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
            new QueryField("Id", Operation.In, ids), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Average(e => e.ColumnInt), Convert.ToDouble(result));
    }

    [TestMethod]
    public async Task TestOracleConnectionAverageAsyncViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.AverageAsync(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
            (object?)null,
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion
}
