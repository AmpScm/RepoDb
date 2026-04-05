using RepoDb.Enumerations;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;

namespace RepoDb.Oracle.IntegrationTests.Operations;

[TestClass]
public class CountTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestOracleConnectionCountWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = this.CreateConnection();
        // Act
        long result = connection.Count<CompleteTable>((object?)null);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void TestOracleConnectionCountViaExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        long[] ids = new[] { tables.First().Id, tables.Last().Id };

        using var connection = this.CreateConnection();
        // Act
        long result = connection.Count<CompleteTable>(e => ids.Contains(e.Id));

        // Assert
        Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Count(), result);
    }

    [TestMethod]
    public void TestOracleConnectionCountViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = this.CreateConnection();
        // Act
        long result = connection.Count<CompleteTable>(new { tables.First().Id });

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
    }

    [TestMethod]
    public void TestOracleConnectionCountViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = this.CreateConnection();
        // Act
        long result = connection.Count<CompleteTable>(new QueryField("Id", tables.First().Id));

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
    }

    [TestMethod]
    public void TestOracleConnectionCountViaQueryFields()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };

        using var connection = this.CreateConnection();
        // Act
        long result = connection.Count<CompleteTable>(queryFields);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
    }

    [TestMethod]
    public void TestOracleConnectionCountViaQueryGroup()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };
        QueryGroup queryGroup = new QueryGroup(queryFields);

        using var connection = this.CreateConnection();
        // Act
        long result = connection.Count<CompleteTable>(queryGroup);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
    }

    [TestMethod]
    public void ThrowExceptionOnOracleConnectionCountWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = this.CreateConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.Count<CompleteTable>((object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestOracleConnectionCountAsyncWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = this.CreateConnection();
        // Act
        long result = await connection.CountAsync<CompleteTable>((object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task TestOracleConnectionCountAsyncViaExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        long[] ids = new[] { tables.First().Id, tables.Last().Id };

        using var connection = this.CreateConnection();
        // Act
        long result = await connection.CountAsync<CompleteTable>(e => ids.Contains(e.Id), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Count(), result);
    }

    [TestMethod]
    public async Task TestOracleConnectionCountAsyncViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = this.CreateConnection();
        // Act
        long result = await connection.CountAsync<CompleteTable>(new { tables.First().Id }, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
    }

    [TestMethod]
    public async Task TestOracleConnectionCountAsyncViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = this.CreateConnection();
        // Act
        long result = await connection.CountAsync<CompleteTable>(new QueryField("Id", tables.First().Id), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
    }

    [TestMethod]
    public async Task TestOracleConnectionCountAsyncViaQueryFields()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };

        using var connection = this.CreateConnection();
        // Act
        long result = await connection.CountAsync<CompleteTable>(queryFields, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
    }

    [TestMethod]
    public async Task TestOracleConnectionCountAsyncViaQueryGroup()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };
        QueryGroup queryGroup = new QueryGroup(queryFields);

        using var connection = this.CreateConnection();
        // Act
        long result = await connection.CountAsync<CompleteTable>(queryGroup, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
    }

    [TestMethod]
    public async Task ThrowExceptionOnOracleConnectionCountAsyncWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = this.CreateConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.CountAsync<CompleteTable>((object?)null,
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestOracleConnectionCountViaTableNameWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = this.CreateConnection();
        // Act
        long result = connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void TestOracleConnectionCountViaTableNameViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = this.CreateConnection();
        // Act
        long result = connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
            new { tables.First().Id });

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
    }

    [TestMethod]
    public void TestOracleConnectionCountViaTableNameViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = this.CreateConnection();
        // Act
        long result = connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
            new QueryField("Id", tables.First().Id));

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
    }

    [TestMethod]
    public void TestOracleConnectionCountViaTableNameViaQueryFields()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };

        using var connection = this.CreateConnection();
        // Act
        long result = connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
            queryFields);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
    }

    [TestMethod]
    public void TestOracleConnectionCountViaTableNameViaQueryGroup()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };
        QueryGroup queryGroup = new QueryGroup(queryFields);

        using var connection = this.CreateConnection();
        // Act
        long result = connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
            queryGroup);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
    }

    [TestMethod]
    public void ThrowExceptionOnOracleConnectionCountViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = this.CreateConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestOracleConnectionCountAsyncViaTableNameWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = this.CreateConnection();
        // Act
        long result = await connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task TestOracleConnectionCountAsyncViaTableNameViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = this.CreateConnection();
        // Act
        long result = await connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new { tables.First().Id }, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
    }

    [TestMethod]
    public async Task TestOracleConnectionCountAsyncViaTableNameViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = this.CreateConnection();
        // Act
        long result = await connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new QueryField("Id", tables.First().Id), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
    }

    [TestMethod]
    public async Task TestOracleConnectionCountAsyncViaTableNameViaQueryFields()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };

        using var connection = this.CreateConnection();
        // Act
        long result = await connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
            queryFields, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
    }

    [TestMethod]
    public async Task TestOracleConnectionCountAsyncViaTableNameViaQueryGroup()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        QueryField[] queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };
        QueryGroup queryGroup = new QueryGroup(queryFields);

        using var connection = this.CreateConnection();
        // Act
        long result = await connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
            queryGroup, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
    }

    [TestMethod]
    public async Task ThrowExceptionOnOracleConnectionCountAsyncViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = this.CreateConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null,
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion
}
