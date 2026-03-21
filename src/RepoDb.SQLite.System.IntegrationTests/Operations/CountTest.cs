using RepoDb.Enumerations;
using RepoDb.SQLite.System.IntegrationTests.Models;
using RepoDb.SQLite.System.IntegrationTests.Setup;
using System.Data.SQLite;

namespace RepoDb.SQLite.System.IntegrationTests.Operations.SDS;

[TestClass]
public class CountTest : TestBase
{
    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionCountWithoutExpression()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.Count<CompleteTable>((object?)null);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void TestSqLiteConnectionCountViaExpression()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        var ids = new[] { tables.First().Id, tables.Last().Id };

        // Act
        var result = connection.Count<CompleteTable>(e => ids.Contains(e.Id));

        // Assert
        Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Count(), result);
    }

    [TestMethod]
    public void TestSqLiteConnectionCountViaDynamic()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.Count<CompleteTable>(new { tables.First().Id });

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
    }

    [TestMethod]
    public void TestSqLiteConnectionCountViaQueryField()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.Count<CompleteTable>(new QueryField("Id", tables.First().Id));

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
    }

    [TestMethod]
    public void TestSqLiteConnectionCountViaQueryFields()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        var queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };

        // Act
        var result = connection.Count<CompleteTable>(queryFields);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
    }

    [TestMethod]
    public void TestSqLiteConnectionCountViaQueryGroup()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        var queryFields = new[]
        {
            new QueryField("Id", Operation.GreaterThan, tables.First().Id),
            new QueryField("Id", Operation.LessThan, tables.Last().Id)
        };
        var queryGroup = new QueryGroup(queryFields);

        // Act
        var result = connection.Count<CompleteTable>(queryGroup);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
    }

    [TestMethod]
    public void ThrowExceptionOnSqLiteConnectionCountWithHints()
    {
        Assert.ThrowsExactly<NotSupportedException>(() =>
        {
            using var connection = new SQLiteConnection(Database.ConnectionString);
            // Setup
            var tables = Database.CreateCompleteTables(10, connection);

            // Act
            connection.Count<CompleteTable>((object?)null,
                hints: "WhatEver");
        });
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionCountAsyncWithoutExpression()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.CountAsync<CompleteTable>((object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionCountAsyncViaExpression()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        var ids = new[] { tables.First().Id, tables.Last().Id };

        // Act
        var result = await connection.CountAsync<CompleteTable>(e => ids.Contains(e.Id), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Count(), result);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionCountAsyncViaDynamic()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.CountAsync<CompleteTable>(new { tables.First().Id }, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionCountAsyncViaQueryField()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.CountAsync<CompleteTable>(new QueryField("Id", tables.First().Id), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionCountAsyncViaQueryFields()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        var queryFields = new[]
        {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

        // Act
        var result = await connection.CountAsync<CompleteTable>(queryFields, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionCountAsyncViaQueryGroup()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        var queryFields = new[]
        {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
        var queryGroup = new QueryGroup(queryFields);

        // Act
        var result = await connection.CountAsync<CompleteTable>(queryGroup, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
    }

    [TestMethod]
    public async Task ThrowExceptionOnSqLiteConnectionCountAsyncWithHints()
    {
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () =>
        {
            using var connection = new SQLiteConnection(Database.ConnectionString);
            // Setup
            var tables = Database.CreateCompleteTables(10, connection);

            // Act
            await connection.CountAsync<CompleteTable>((object?)null,
                hints: "WhatEver", cancellationToken: TestContext.CancellationToken);
        });
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionCountViaTableNameWithoutExpression()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public void TestSqLiteConnectionCountViaTableNameViaDynamic()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
            new { tables.First().Id });

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
    }

    [TestMethod]
    public void TestSqLiteConnectionCountViaTableNameViaQueryField()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
            new QueryField("Id", tables.First().Id));

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
    }

    [TestMethod]
    public void TestSqLiteConnectionCountViaTableNameViaQueryFields()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        var queryFields = new[]
        {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

        // Act
        var result = connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
            queryFields);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
    }

    [TestMethod]
    public void TestSqLiteConnectionCountViaTableNameViaQueryGroup()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        var queryFields = new[]
        {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
        var queryGroup = new QueryGroup(queryFields);

        // Act
        var result = connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
            queryGroup);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
    }

    [TestMethod]
    public void ThrowExceptionOnSqLiteConnectionCountViaTableNameWithHints()
    {
        Assert.ThrowsExactly<NotSupportedException>(() =>
        {
            using var connection = new SQLiteConnection(Database.ConnectionString);
            // Setup
            var tables = Database.CreateCompleteTables(10, connection);

            // Act
            connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
                (object?)null,
                hints: "WhatEver");
        });
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionCountAsyncViaTableNameWithoutExpression()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionCountAsyncViaTableNameViaDynamic()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new { tables.First().Id }, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionCountAsyncViaTableNameViaQueryField()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        var result = await connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new QueryField("Id", tables.First().Id), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionCountAsyncViaTableNameViaQueryFields()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        var queryFields = new[]
        {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

        // Act
        var result = await connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
            queryFields, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
    }

    [TestMethod]
    public async Task TestSqLiteConnectionCountAsyncViaTableNameViaQueryGroup()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);
        var queryFields = new[]
        {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
        var queryGroup = new QueryGroup(queryFields);

        // Act
        var result = await connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
            queryGroup, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
    }

    [TestMethod]
    public async Task ThrowExceptionOnSqLiteConnectionCountAsyncViaTableNameWithHints()
    {
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () =>
        {
            using var connection = new SQLiteConnection(Database.ConnectionString);
            // Setup
            var tables = Database.CreateCompleteTables(10, connection);

            // Act
            await connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
                (object?)null,
                hints: "WhatEver", cancellationToken: TestContext.CancellationToken);
        });
    }
    #endregion

    #endregion
}
