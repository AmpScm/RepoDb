using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.PostgreSql.IntegrationTests.Operations;

[TestClass]
public class MaxAllTest
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
    public void TestPostgreSqlConnectionMaxAll()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = connection.MaxAll<CompleteTable>(e => e.ColumnInteger);

        // Assert
        Assert.AreEqual(tables.Max(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlConnectionMaxAllWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.MaxAll<CompleteTable>(e => e.ColumnInteger,
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionMaxAllAsync()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = await connection.MaxAllAsync<CompleteTable>(e => e.ColumnInteger);

        // Assert
        Assert.AreEqual(tables.Max(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task ThrowExceptionOnPostgreSqlConnectionMaxAllAsyncWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.MaxAllAsync<CompleteTable>(e => e.ColumnInteger,
            hints: "WhatEver"));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestPostgreSqlConnectionMaxAllViaTableName()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = connection.MaxAll(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInteger).First());

        // Assert
        Assert.AreEqual(tables.Max(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlConnectionMaxAllViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => connection.MaxAll(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInteger).First(),
            hints: "WhatEver"));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionMaxAllAsyncViaTableName()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object result = await connection.MaxAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInteger).First());

        // Assert
        Assert.AreEqual(tables.Max(e => e.ColumnInteger), Convert.ToInt32(result));
    }

    [TestMethod]
    public async Task ThrowExceptionOnPostgreSqlConnectionMaxAllAsyncViaTableNameWithHints()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => await connection.MaxAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInteger).First(),
            hints: "WhatEver"));
    }

    #endregion

    #endregion
}
