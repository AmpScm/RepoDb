using RepoDb.Extensions;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;
using RepoDb.Trace;

namespace RepoDb.Oracle.IntegrationTests.Operations;

[TestClass]
public class ExecuteQueryMultipleTest : TestBase
{
    #region Sync

    [TestMethod]
    public void TestOracleConnectionExecuteQueryMultiple()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        using QueryMultipleExtractor extractor = connection.ExecuteQueryMultiple(
            connection.CombineQueries([
            "SELECT * FROM \"CompleteTable\"",
            "SELECT * FROM \"CompleteTable\""
            ]));
        List<IEnumerable<CompleteTable>> list =
                [
                    // Act
                    extractor.Extract<CompleteTable>(),
                    extractor.Extract<CompleteTable>()
                ];

        // Assert
        list.ForEach(item =>
        {
            Assert.AreEqual(tables.Count(), item.Count());
            tables.AsList().ForEach(table => Helper.AssertPropertiesEquality(table, item.First(e => e.Id == table.Id)));
        });
    }

    [TestMethod]
    public void TestOracleConnectionExecuteQueryMultipleWithParameters()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        using QueryMultipleExtractor extractor = connection.ExecuteQueryMultiple(
            connection.CombineQueries(
            "SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id1",
            "SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id2"),
            new
            {
                Id1 = tables.First().Id,
                Id2 = tables.Last().Id
            });
        var list = new List<IEnumerable<CompleteTable>>
                {
                    // Act
                    extractor.Extract<CompleteTable>(),
                    extractor.Extract<CompleteTable>()
                };

        // Assert
        list.ForEach(item =>
        {
            item.AsList().ForEach(current => Helper.AssertPropertiesEquality(current, tables.First(e => e.Id == current.Id)));
        });
    }

    [TestMethod]
    public void TestOracleConnectionExecuteQueryMultipleWithSharedParameters()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        using QueryMultipleExtractor extractor = connection.ExecuteQueryMultiple(
            [
                "SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                "SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id"
            ],
            new { Id = tables.Last().Id });
        List<IEnumerable<CompleteTable>> list =
                [
                    // Act
                    extractor.Extract<CompleteTable>(),
                    extractor.Extract<CompleteTable>()
                ];

        // Assert
        list.ForEach(item =>
        {
            item.AsList().ForEach(current => Helper.AssertPropertiesEquality(current, tables.First(e => e.Id == current.Id)));
        });
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestOracleConnectionExecuteQueryMultipleAsync()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        using QueryMultipleExtractor extractor = await connection.ExecuteQueryMultipleAsync(
            [
                "SELECT * FROM \"CompleteTable\"",
                "SELECT * FROM \"CompleteTable\""
            ], cancellationToken: TestContext.CancellationToken);
        List<IEnumerable<CompleteTable>> list =
                [
                    // Act
                    extractor.Extract<CompleteTable>(),
                    extractor.Extract<CompleteTable>()
                ];

        // Assert
        list.ForEach(item =>
        {
            Assert.AreEqual(tables.Count(), item.Count());
            tables.AsList().ForEach(table => Helper.AssertPropertiesEquality(table, item.First(e => e.Id == table.Id)));
        });
    }

    [TestMethod]
    public async Task TestOracleConnectionExecuteQueryMultipleAsyncWithParameters()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        using QueryMultipleExtractor extractor = await connection.ExecuteQueryMultipleAsync(
            [
                "SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id1",
                "SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id2",
            ],
            new
            {
                Id1 = tables.First().Id,
                Id2 = tables.Last().Id
            }, cancellationToken: TestContext.CancellationToken, trace: new DiagnosticsTracer());
        var list = new List<IEnumerable<CompleteTable>>
                {
                    // Act
                    extractor.Extract<CompleteTable>(),
                    extractor.Extract<CompleteTable>()
                };

        // Assert
        list.ForEach(item =>
        {
            item.AsList().ForEach(current => Helper.AssertPropertiesEquality(current, tables.First(e => e.Id == current.Id)));
        });
    }

    [TestMethod]
    public async Task TestOracleConnectionExecuteQueryMultipleAsyncWithSharedParameters()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        using QueryMultipleExtractor extractor = await connection.ExecuteQueryMultipleAsync(
            [
                "SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                "SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id"
            ],
            new { Id = tables.Last().Id }, cancellationToken: TestContext.CancellationToken);
        List<IEnumerable<CompleteTable>> list =
                [
                    // Act
                    extractor.Extract<CompleteTable>(),
                    extractor.Extract<CompleteTable>()
                ];

        // Assert
        list.ForEach(item =>
        {
            item.AsList().ForEach(current => Helper.AssertPropertiesEquality(current, tables.First(e => e.Id == current.Id)));
        });
    }

    #endregion
}
