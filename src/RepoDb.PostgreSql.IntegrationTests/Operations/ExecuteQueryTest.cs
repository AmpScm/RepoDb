using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Extensions;
using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.PostgreSql.IntegrationTests.Operations;

[TestClass]
public class ExecuteQueryTest
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

    #region Sync

    [TestMethod]
    public void TestPostgreSqlConnectionExecuteQuery()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<CompleteTable> result = connection.ExecuteQuery<CompleteTable>("SELECT * FROM \"CompleteTable\";");

        // Assert
        Assert.AreEqual(tables.Count(), result.Count());
        tables.AsList().ForEach(table => Helper.AssertPropertiesEquality(table, result.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionExecuteQueryWithParameters()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<CompleteTable> result = connection.ExecuteQuery<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;",
            new { tables.Last().Id });

        // Assert
        Assert.AreEqual(1, result.Count());
        Helper.AssertPropertiesEquality(tables.Last(), result.First());
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionExecuteQueryAsync()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<CompleteTable> result = await connection.ExecuteQueryAsync<CompleteTable>("SELECT * FROM \"CompleteTable\";");

        // Assert
        Assert.AreEqual(tables.Count(), result.Count());
        tables.AsList().ForEach(table => Helper.AssertPropertiesEquality(table, result.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionExecuteQueryAsyncWithParameters()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        IEnumerable<CompleteTable> result = await connection.ExecuteQueryAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;",
            new { tables.Last().Id });

        // Assert
        Assert.AreEqual(1, result.Count());
        Helper.AssertPropertiesEquality(tables.Last(), result.First());
    }

    #endregion
}
