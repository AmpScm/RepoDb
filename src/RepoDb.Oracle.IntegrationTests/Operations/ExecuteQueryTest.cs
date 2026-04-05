using System.Diagnostics.Tracing;
using RepoDb.Extensions;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;
using RepoDb.Trace;

namespace RepoDb.Oracle.IntegrationTests.Operations;

[TestClass]
public class ExecuteQueryTest : TestBase
{
    #region Sync

    [TestMethod]
    public void TestOracleConnectionExecuteQuery()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        IEnumerable<CompleteTable> result = connection.ExecuteQuery<CompleteTable>("SELECT * FROM \"CompleteTable\"");

        // Assert
        Assert.AreEqual(tables.Count(), result.Count());
        tables.AsList().ForEach(table => Helper.AssertPropertiesEquality(table, result.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestOracleConnectionExecuteQueryWithParameters()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        IEnumerable<CompleteTable> result = connection.ExecuteQuery<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id",
            new { tables.Last().Id }, trace: new DiagnosticsTracer());

        // Assert
        Assert.AreEqual(1, result.Count());
        Helper.AssertPropertiesEquality(tables.Last(), result.First());
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestOracleConnectionExecuteQueryAsync()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        IEnumerable<CompleteTable> result = await connection.ExecuteQueryAsync<CompleteTable>("SELECT * FROM \"CompleteTable\"", cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Count(), result.Count());
        tables.AsList().ForEach(table => Helper.AssertPropertiesEquality(table, result.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestOracleConnectionExecuteQueryAsyncWithParameters()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        IEnumerable<CompleteTable> result = await connection.ExecuteQueryAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id",
            new { tables.Last().Id }, cancellationToken: TestContext.CancellationToken, trace: new DiagnosticsTracer());

        // Assert
        Assert.AreEqual(1, result.Count());
        Helper.AssertPropertiesEquality(tables.Last(), result.First());
    }

    #endregion
}
