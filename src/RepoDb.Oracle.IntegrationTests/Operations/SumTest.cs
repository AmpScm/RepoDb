using RepoDb.Enumerations;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;

namespace RepoDb.Oracle.IntegrationTests.Operations;

[TestClass]
public class SumTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestOracleConnectionSumWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = connection.Sum<CompleteTable>(e => e.ColumnInt,
            (object?)null);

        // Assert
        Assert.AreEqual(tables.Sum(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestOracleConnectionSumViaExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        long[] ids = new[] { tables.First().Id, tables.Last().Id };

        using var connection = CreateConnection();
        // Act
        object result = connection.Sum<CompleteTable>(e => e.ColumnInt,
            e => ids.Contains(e.Id));

        // Assert
        Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Sum(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestOracleConnectionSumViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = connection.Sum<CompleteTable>(e => e.ColumnInt,
            new { tables.First().Id });

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestOracleConnectionSumViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = connection.Sum<CompleteTable>(e => e.ColumnInt,
            new QueryField("Id", tables.First().Id));

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestOracleConnectionSumViaQueryFields()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };

        using var connection = CreateConnection();
        // Act
        object result = connection.Sum<CompleteTable>(e => e.ColumnInt,
            queryFields);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestOracleConnectionSumViaQueryGroup()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };
        QueryGroup queryGroup = new QueryGroup(queryFields);

        using var connection = CreateConnection();
        // Act
        object result = connection.Sum<CompleteTable>(e => e.ColumnInt,
            queryGroup);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void ThrowExceptionOnOracleConnectionSumWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.Sum<CompleteTable>(e => e.ColumnInt,
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestOracleConnectionSumAsyncWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = await connection.SumAsync<CompleteTable>(e => e.ColumnInt,
            (object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Sum(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestOracleConnectionSumAsyncViaExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        long[] ids = new[] { tables.First().Id, tables.Last().Id };

        using var connection = CreateConnection();
        // Act
        object result = await connection.SumAsync<CompleteTable>(e => e.ColumnInt,
            e => ids.Contains(e.Id), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Sum(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestOracleConnectionSumAsyncViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = await connection.SumAsync<CompleteTable>(e => e.ColumnInt,
            new { tables.First().Id }, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestOracleConnectionSumAsyncViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = await connection.SumAsync<CompleteTable>(e => e.ColumnInt,
            new QueryField("Id", tables.First().Id), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestOracleConnectionSumAsyncViaQueryFields()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };

        using var connection = CreateConnection();
        // Act
        object result = await connection.SumAsync<CompleteTable>(e => e.ColumnInt,
            queryFields, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestOracleConnectionSumAsyncViaQueryGroup()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };
        QueryGroup queryGroup = new QueryGroup(queryFields);

        using var connection = CreateConnection();
        // Act
        object result = await connection.SumAsync<CompleteTable>(e => e.ColumnInt,
            queryGroup, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task ThrowExceptionOnOracleConnectionSumAsyncWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.SumAsync<CompleteTable>(e => e.ColumnInt,
            (object?)null,
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestOracleConnectionSumViaTableNameWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = connection.Sum(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            (object?)null);

        // Assert
        Assert.AreEqual(tables.Sum(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestOracleConnectionSumViaTableNameViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = connection.Sum(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            new { tables.First().Id });

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestOracleConnectionSumViaTableNameViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = connection.Sum(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            new QueryField("Id", tables.First().Id));

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestOracleConnectionSumViaTableNameViaQueryFields()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };

        using var connection = CreateConnection();
        // Act
        object result = connection.Sum(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            queryFields);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestOracleConnectionSumViaTableNameViaQueryGroup()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };
        QueryGroup queryGroup = new QueryGroup(queryFields);

        using var connection = CreateConnection();
        // Act
        object result = connection.Sum(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            queryGroup);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void ThrowExceptionOnOracleConnectionSumViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.Sum(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestOracleConnectionSumAsyncViaTableNameWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = await connection.SumAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            (object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Sum(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestOracleConnectionSumAsyncViaTableNameViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = await connection.SumAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            new { tables.First().Id }, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestOracleConnectionSumAsyncViaTableNameViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = await connection.SumAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            new QueryField("Id", tables.First().Id), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestOracleConnectionSumAsyncViaTableNameViaQueryFields()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };

        using var connection = CreateConnection();
        // Act
        object result = await connection.SumAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            queryFields, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestOracleConnectionSumAsyncViaTableNameViaQueryGroup()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };
        QueryGroup queryGroup = new QueryGroup(queryFields);

        using var connection = CreateConnection();
        // Act
        object result = await connection.SumAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            queryGroup, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task ThrowExceptionOnOracleConnectionSumAsyncViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.SumAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            (object?)null,
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion
}
