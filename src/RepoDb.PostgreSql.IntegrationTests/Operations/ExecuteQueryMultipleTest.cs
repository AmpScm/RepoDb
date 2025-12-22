using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Extensions;
using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.PostgreSql.IntegrationTests.Operations;

[TestClass]
public class ExecuteQueryMultipleTest
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
    public void TestPostgreSqlConnectionExecuteQueryMultiple()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        using QueryMultipleExtractor extractor = connection.ExecuteQueryMultiple("SELECT * FROM \"CompleteTable\"; " +
            "SELECT * FROM \"CompleteTable\";");
        List<IEnumerable<CompleteTable>> list = new List<IEnumerable<CompleteTable>>
                {
                    // Act
                    extractor.Extract<CompleteTable>(),
                    extractor.Extract<CompleteTable>()
                };

        // Assert
        list.ForEach(item =>
        {
            Assert.AreEqual(tables.Count(), item.Count());
            tables.AsList().ForEach(table => Helper.AssertPropertiesEquality(table, item.First(e => e.Id == table.Id)));
        });
    }

    [TestMethod]
    public void TestPostgreSqlConnectionExecuteQueryMultipleWithParameters()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        using QueryMultipleExtractor extractor = connection.ExecuteQueryMultiple("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id1; " +
            "SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id2;",
            new
            {
                Id1 = tables.First().Id,
                Id2 = tables.Last().Id
            });
        List<IEnumerable<CompleteTable>> list = new List<IEnumerable<CompleteTable>>
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
    public void TestPostgreSqlConnectionExecuteQueryMultipleWithSharedParameters()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        using QueryMultipleExtractor extractor = connection.ExecuteQueryMultiple("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id; " +
            "SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;",
            new { Id = tables.Last().Id });
        List<IEnumerable<CompleteTable>> list = new List<IEnumerable<CompleteTable>>
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

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionExecuteQueryMultipleAsync()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        using QueryMultipleExtractor extractor = await connection.ExecuteQueryMultipleAsync("SELECT * FROM \"CompleteTable\"; " +
            "SELECT * FROM \"CompleteTable\";", cancellationToken: TestContext.CancellationToken);
        List<IEnumerable<CompleteTable>> list = new List<IEnumerable<CompleteTable>>
                {
                    // Act
                    extractor.Extract<CompleteTable>(),
                    extractor.Extract<CompleteTable>()
                };

        // Assert
        list.ForEach(item =>
        {
            Assert.AreEqual(tables.Count(), item.Count());
            tables.AsList().ForEach(table => Helper.AssertPropertiesEquality(table, item.First(e => e.Id == table.Id)));
        });
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionExecuteQueryMultipleAsyncWithParameters()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        using QueryMultipleExtractor extractor = await connection.ExecuteQueryMultipleAsync("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id1; " +
            "SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id2;",
            new
            {
                Id1 = tables.First().Id,
                Id2 = tables.Last().Id
            }, cancellationToken: TestContext.CancellationToken);
        List<IEnumerable<CompleteTable>> list = new List<IEnumerable<CompleteTable>>
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
    public async Task TestPostgreSqlConnectionExecuteQueryMultipleAsyncWithSharedParameters()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        using QueryMultipleExtractor extractor = await connection.ExecuteQueryMultipleAsync("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id; " +
            "SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;",
            new { Id = tables.Last().Id }, cancellationToken: TestContext.CancellationToken);
        List<IEnumerable<CompleteTable>> list = new List<IEnumerable<CompleteTable>>
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

    public TestContext TestContext { get; set; }

    #endregion
}
