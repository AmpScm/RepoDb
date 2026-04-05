using RepoDb.Enumerations;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;

namespace RepoDb.Oracle.IntegrationTests.Operations;

[TestClass]
public class ExistsTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestOracleConnectionExistsWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        bool result = connection.Exists<CompleteTable>((object?)null);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestOracleConnectionExistsViaExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        long[] ids = new[] { tables.First().Id, tables.Last().Id };

        using var connection = CreateConnection();
        // Act
        bool result = connection.Exists<CompleteTable>(e => ids.Contains(e.Id));

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestOracleConnectionExistsViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        bool result = connection.Exists<CompleteTable>(new { tables.First().Id });

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestOracleConnectionExistsViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        bool result = connection.Exists<CompleteTable>(new QueryField("Id", tables.First().Id));

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestOracleConnectionExistsViaQueryFields()
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
        bool result = connection.Exists<CompleteTable>(queryFields);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestOracleConnectionExistsViaQueryGroup()
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
        bool result = connection.Exists<CompleteTable>(queryGroup);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ThrowExceptionOnOracleConnectionExistsWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.Exists<CompleteTable>((object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestOracleConnectionExistsAsyncWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        bool result = await connection.ExistsAsync<CompleteTable>((object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task TestOracleConnectionExistsAsyncViaExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        long[] ids = new[] { tables.First().Id, tables.Last().Id };

        using var connection = CreateConnection();
        // Act
        bool result = await connection.ExistsAsync<CompleteTable>(e => ids.Contains(e.Id), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task TestOracleConnectionExistsAsyncViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        bool result = await connection.ExistsAsync<CompleteTable>(new { tables.First().Id }, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task TestOracleConnectionExistsAsyncViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        bool result = await connection.ExistsAsync<CompleteTable>(new QueryField("Id", tables.First().Id), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task TestOracleConnectionExistsAsyncViaQueryFields()
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
        bool result = await connection.ExistsAsync<CompleteTable>(queryFields, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task TestOracleConnectionExistsAsyncViaQueryGroup()
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
        bool result = await connection.ExistsAsync<CompleteTable>(queryGroup, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task ThrowExceptionOnOracleConnectionExistsAsyncWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.ExistsAsync<CompleteTable>((object?)null,
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestOracleConnectionExistsViaTableNameWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        bool result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestOracleConnectionExistsViaTableNameViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        bool result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
            new { tables.First().Id });

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestOracleConnectionExistsViaTableNameViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        bool result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
            new QueryField("Id", tables.First().Id));

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestOracleConnectionExistsViaTableNameViaQueryFields()
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
        bool result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
            queryFields);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestOracleConnectionExistsViaTableNameViaQueryGroup()
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
        bool result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
            queryGroup);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ThrowExceptionOnOracleConnectionExistsViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestOracleConnectionExistsAsyncViaTableNameWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        bool result = await connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task TestOracleConnectionExistsAsyncViaTableNameViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        bool result = await connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new { tables.First().Id }, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task TestOracleConnectionExistsAsyncViaTableNameViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        bool result = await connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new QueryField("Id", tables.First().Id), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task TestOracleConnectionExistsAsyncViaTableNameViaQueryFields()
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
        bool result = await connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
            queryFields, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task TestOracleConnectionExistsAsyncViaTableNameViaQueryGroup()
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
        bool result = await connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
            queryGroup, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task ThrowExceptionOnOracleConnectionExistsAsyncViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null,
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion
}
