using RepoDb.Enumerations;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;

namespace RepoDb.Oracle.IntegrationTests.Operations;

[TestClass]
public class MinTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestOracleConnectionMinWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = connection.Min<CompleteTable>(e => e.ColumnInt,
            (object?)null);

        // Assert
        Assert.AreEqual(tables.Min(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestOracleConnectionMinViaExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        long[] ids = new[] { tables.First().Id, tables.Last().Id };

        using var connection = CreateConnection();
        // Act
        object result = connection.Min<CompleteTable>(e => e.ColumnInt,
            e => ids.Contains(e.Id));

        // Assert
        Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Min(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestOracleConnectionMinViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = connection.Min<CompleteTable>(e => e.ColumnInt,
            new { tables.First().Id });

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Min(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestOracleConnectionMinViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = connection.Min<CompleteTable>(e => e.ColumnInt,
            new QueryField("Id", tables.First().Id));

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Min(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestOracleConnectionMinViaQueryFields()
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
        object result = connection.Min<CompleteTable>(e => e.ColumnInt,
            queryFields);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Min(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestOracleConnectionMinViaQueryGroup()
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
        object result = connection.Min<CompleteTable>(e => e.ColumnInt,
            queryGroup);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Min(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void ThrowExceptionOnOracleConnectionMinWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.Min<CompleteTable>(e => e.ColumnInt,
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestOracleConnectionMinAsyncWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = await connection.MinAsync<CompleteTable>(e => e.ColumnInt,
            (object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Min(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestOracleConnectionMinAsyncViaExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        long[] ids = new[] { tables.First().Id, tables.Last().Id };

        using var connection = CreateConnection();
        // Act
        object result = await connection.MinAsync<CompleteTable>(e => e.ColumnInt,
            e => ids.Contains(e.Id), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Min(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestOracleConnectionMinAsyncViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = await connection.MinAsync<CompleteTable>(e => e.ColumnInt,
            new { tables.First().Id }, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Min(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestOracleConnectionMinAsyncViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = await connection.MinAsync<CompleteTable>(e => e.ColumnInt,
            new QueryField("Id", tables.First().Id), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Min(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestOracleConnectionMinAsyncViaQueryFields()
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
        object result = await connection.MinAsync<CompleteTable>(e => e.ColumnInt,
            queryFields, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Min(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestOracleConnectionMinAsyncViaQueryGroup()
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
        object result = await connection.MinAsync<CompleteTable>(e => e.ColumnInt,
            queryGroup, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Min(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task ThrowExceptionOnOracleConnectionMinAsyncWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.MinAsync<CompleteTable>(e => e.ColumnInt,
            (object?)null,
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestOracleConnectionMinViaTableNameWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = connection.Min(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            (object?)null);

        // Assert
        Assert.AreEqual(tables.Min(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestOracleConnectionMinViaTableNameViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = connection.Min(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            new { tables.First().Id });

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Min(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestOracleConnectionMinViaTableNameViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = connection.Min(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            new QueryField("Id", tables.First().Id));

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Min(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestOracleConnectionMinViaTableNameViaQueryFields()
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
        object result = connection.Min(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            queryFields);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Min(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestOracleConnectionMinViaTableNameViaQueryGroup()
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
        object result = connection.Min(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            queryGroup);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Min(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void ThrowExceptionOnOracleConnectionMinViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.Min(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestOracleConnectionMinAsyncViaTableNameWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = await connection.MinAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            (object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Min(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestOracleConnectionMinAsyncViaTableNameViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = await connection.MinAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            new { tables.First().Id }, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Min(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestOracleConnectionMinAsyncViaTableNameViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = await connection.MinAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            new QueryField("Id", tables.First().Id), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Min(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestOracleConnectionMinAsyncViaTableNameViaQueryFields()
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
        object result = await connection.MinAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            queryFields, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Min(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestOracleConnectionMinAsyncViaTableNameViaQueryGroup()
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
        object result = await connection.MinAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            queryGroup, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Min(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task ThrowExceptionOnOracleConnectionMinAsyncViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.MinAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            (object?)null,
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion
}
