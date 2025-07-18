using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Enumerations;
using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.PostgreSql.IntegrationTests.Operations;

[TestClass]
public class ExistsTest
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
    public void TestPostgreSqlConnectionExistsWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        bool result = connection.Exists<CompleteTable>((object?)null);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionExistsViaExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        long[] ids = new[] { tables.First().Id, tables.Last().Id };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        bool result = connection.Exists<CompleteTable>(e => ids.Contains(e.Id));

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionExistsViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        bool result = connection.Exists<CompleteTable>(new { tables.First().Id });

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionExistsViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        bool result = connection.Exists<CompleteTable>(new QueryField("Id", tables.First().Id));

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionExistsViaQueryFields()
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
        bool result = connection.Exists<CompleteTable>(queryFields);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionExistsViaQueryGroup()
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
        bool result = connection.Exists<CompleteTable>(queryGroup);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlConnectionExistsWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.Exists<CompleteTable>((object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionExistsAsyncWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        bool result = await connection.ExistsAsync<CompleteTable>((object?)null);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionExistsAsyncViaExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);
        long[] ids = new[] { tables.First().Id, tables.Last().Id };

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        bool result = await connection.ExistsAsync<CompleteTable>(e => ids.Contains(e.Id));

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionExistsAsyncViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        bool result = await connection.ExistsAsync<CompleteTable>(new { tables.First().Id });

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionExistsAsyncViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        bool result = await connection.ExistsAsync<CompleteTable>(new QueryField("Id", tables.First().Id));

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionExistsAsyncViaQueryFields()
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
        bool result = await connection.ExistsAsync<CompleteTable>(queryFields);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionExistsAsyncViaQueryGroup()
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
        bool result = await connection.ExistsAsync<CompleteTable>(queryGroup);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task ThrowExceptionOnPostgreSqlConnectionExistsAsyncWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.ExistsAsync<CompleteTable>((object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestPostgreSqlConnectionExistsViaTableNameWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        bool result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionExistsViaTableNameViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        bool result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
            new { tables.First().Id });

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionExistsViaTableNameViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        bool result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
            new QueryField("Id", tables.First().Id));

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionExistsViaTableNameViaQueryFields()
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
        bool result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
            queryFields);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestPostgreSqlConnectionExistsViaTableNameViaQueryGroup()
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
        bool result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
            queryGroup);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlConnectionExistsViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionExistsAsyncViaTableNameWithoutExpression()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        bool result = await connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionExistsAsyncViaTableNameViaDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        bool result = await connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new { tables.First().Id });

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionExistsAsyncViaTableNameViaQueryField()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        bool result = await connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
            new QueryField("Id", tables.First().Id));

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionExistsAsyncViaTableNameViaQueryFields()
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
        bool result = await connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
            queryFields);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionExistsAsyncViaTableNameViaQueryGroup()
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
        bool result = await connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
            queryGroup);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task ThrowExceptionOnPostgreSqlConnectionExistsAsyncViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
            (object?)null,
            hints: "WhatEver"));
    }

    #endregion

    #endregion
}
