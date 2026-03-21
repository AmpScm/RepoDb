using Microsoft.Data.Sqlite;
using RepoDb.Enumerations;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Models;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Setup;

namespace RepoDb.Sqlite.Microsoft.IntegrationTests.Operations.MDS;

[TestClass]
public class ExistsTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionExistsWithoutExpression()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.Exists<CompleteTable>((object?)null);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestSqLiteConnectionExistsViaExpression()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        var ids = new[] { tables.First().Id, tables.Last().Id };

        // Act
        var result = connection.Exists<CompleteTable>(e => ids.Contains(e.Id));

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestSqLiteConnectionExistsViaDynamic()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.Exists<CompleteTable>(new { tables.First().Id });

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestSqLiteConnectionExistsViaQueryField()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.Exists<CompleteTable>(new QueryField("Id", tables.First().Id));

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestSqLiteConnectionExistsViaQueryFields()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        var queryFields = new[]
        {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

        // Act
        var result = connection.Exists<CompleteTable>(queryFields);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestSqLiteConnectionExistsViaQueryGroup()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        var queryFields = new[]
        {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
        var queryGroup = new QueryGroup(queryFields);

        // Act
        var result = connection.Exists<CompleteTable>(queryGroup);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ThrowExceptionOnSqLiteConnectionExistsWithHints()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.Exists<CompleteTable>((object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionExistsAsyncWithoutExpression()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.ExistsAsync<CompleteTable>((object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionExistsAsyncViaExpression()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        var ids = new[] { tables.First().Id, tables.Last().Id };

        // Act
        var result = await connection.ExistsAsync<CompleteTable>(e => ids.Contains(e.Id), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionExistsAsyncViaDynamic()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.ExistsAsync<CompleteTable>(new { tables.First().Id }, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionExistsAsyncViaQueryField()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.ExistsAsync<CompleteTable>(new QueryField("Id", tables.First().Id), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionExistsAsyncViaQueryFields()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        var queryFields = new[]
        {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

        // Act
        var result = await connection.ExistsAsync<CompleteTable>(queryFields, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionExistsAsyncViaQueryGroup()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        var queryFields = new[]
        {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
        var queryGroup = new QueryGroup(queryFields);

        // Act
        var result = await connection.ExistsAsync<CompleteTable>(queryGroup, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task ThrowExceptionOnSqLiteConnectionExistsAsyncWithHints()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.ExistsAsync<CompleteTable>((object?)null,
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionExistsViaTableNameWithoutExpression()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestSqLiteConnectionExistsViaTableNameViaDynamic()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
            new { tables.First().Id });

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestSqLiteConnectionExistsViaTableNameViaQueryField()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
            new QueryField("Id", tables.First().Id));

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestSqLiteConnectionExistsViaTableNameViaQueryFields()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        var queryFields = new[]
        {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

        // Act
        var result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
            queryFields);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestSqLiteConnectionExistsViaTableNameViaQueryGroup()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        var queryFields = new[]
        {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
        var queryGroup = new QueryGroup(queryFields);

        // Act
        var result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
            queryGroup);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ThrowExceptionOnSqLiteConnectionExistsViaTableNameWithHints()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionExistsAsyncViaTableNameWithoutExpression()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionExistsAsyncViaTableNameViaDynamic()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new { tables.First().Id }, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionExistsAsyncViaTableNameViaQueryField()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new QueryField("Id", tables.First().Id), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionExistsAsyncViaTableNameViaQueryFields()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        var queryFields = new[]
        {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

        // Act
        var result = await connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
            queryFields, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionExistsAsyncViaTableNameViaQueryGroup()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        var queryFields = new[]
        {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
        var queryGroup = new QueryGroup(queryFields);

        // Act
        var result = await connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
            queryGroup, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task ThrowExceptionOnSqLiteConnectionExistsAsyncViaTableNameWithHints()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null,
            hints: "WhatEver", cancellationToken: TestContext.CancellationToken));
    }

    #endregion

    #endregion
}
