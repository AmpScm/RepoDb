using RepoDb.Enumerations;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;

namespace RepoDb.Oracle.IntegrationTests.Operations;

[TestClass]
public class MaxTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestOracleConnectionMaxWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = connection.Max<CompleteTable>(e => e.ColumnInt,
            (object?)null);

        // Assert
        Assert.AreEqual(tables.Max(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestOracleConnectionMaxViaExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        long[] ids = new[] { tables.First().Id, tables.Last().Id };

        using var connection = CreateConnection();
        // Act
        object result = connection.Max<CompleteTable>(e => e.ColumnInt,
            e => ids.Contains(e.Id));

        // Assert
        Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Max(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestOracleConnectionMaxViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = connection.Max<CompleteTable>(e => e.ColumnInt,
            new { tables.First().Id });

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestOracleConnectionMaxViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = connection.Max<CompleteTable>(e => e.ColumnInt,
            new QueryField("Id", tables.First().Id));

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestOracleConnectionMaxViaQueryFields()
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
        object result = connection.Max<CompleteTable>(e => e.ColumnInt,
            queryFields);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestOracleConnectionMaxViaQueryGroup()
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
        object result = connection.Max<CompleteTable>(e => e.ColumnInt,
            queryGroup);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void ThrowExceptionOnOracleConnectionMaxWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.Max<CompleteTable>(e => e.ColumnInt,
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestOracleConnectionMaxAsyncWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = await connection.MaxAsync<CompleteTable>(e => e.ColumnInt,
            (object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Max(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestOracleConnectionMaxAsyncViaExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        long[] ids = new[] { tables.First().Id, tables.Last().Id };

        using var connection = CreateConnection();
        // Act
        object result = await connection.MaxAsync<CompleteTable>(e => e.ColumnInt,
            e => ids.Contains(e.Id), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Max(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestOracleConnectionMaxAsyncViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = await connection.MaxAsync<CompleteTable>(e => e.ColumnInt,
            new { tables.First().Id }, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestOracleConnectionMaxAsyncViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = await connection.MaxAsync<CompleteTable>(e => e.ColumnInt,
            new QueryField("Id", tables.First().Id), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestOracleConnectionMaxAsyncViaQueryFields()
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
        object result = await connection.MaxAsync<CompleteTable>(e => e.ColumnInt,
            queryFields, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestOracleConnectionMaxAsyncViaQueryGroup()
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
        object result = await connection.MaxAsync<CompleteTable>(e => e.ColumnInt,
            queryGroup, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task ThrowExceptionOnOracleConnectionMaxAsyncWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.MaxAsync<CompleteTable>(e => e.ColumnInt,
            (object?)null,
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestOracleConnectionMaxViaTableNameWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = connection.Max(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            (object?)null);

        // Assert
        Assert.AreEqual(tables.Max(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestOracleConnectionMaxViaTableNameViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = connection.Max(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            new { tables.First().Id });

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestOracleConnectionMaxViaTableNameViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = connection.Max(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            new QueryField("Id", tables.First().Id));

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestOracleConnectionMaxViaTableNameViaQueryFields()
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
        object result = connection.Max(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            queryFields);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void TestOracleConnectionMaxViaTableNameViaQueryGroup()
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
        object result = connection.Max(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            queryGroup);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public void ThrowExceptionOnOracleConnectionMaxViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.Max(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestOracleConnectionMaxAsyncViaTableNameWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = await connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            (object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Max(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestOracleConnectionMaxAsyncViaTableNameViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = await connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            new { tables.First().Id }, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestOracleConnectionMaxAsyncViaTableNameViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        object result = await connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            new QueryField("Id", tables.First().Id), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestOracleConnectionMaxAsyncViaTableNameViaQueryFields()
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
        object result = await connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            queryFields, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task TestOracleConnectionMaxAsyncViaTableNameViaQueryGroup()
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
        object result = await connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            queryGroup, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task ThrowExceptionOnOracleConnectionMaxAsyncViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new Field("ColumnInt", typeof(int)),
            (object?)null,
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion
}
