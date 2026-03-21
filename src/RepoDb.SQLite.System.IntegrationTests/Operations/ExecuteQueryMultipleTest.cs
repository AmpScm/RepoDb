using RepoDb.Extensions;
using RepoDb.SQLite.System.IntegrationTests.Models;
using RepoDb.SQLite.System.IntegrationTests.Setup;
using System.Data.SQLite;

namespace RepoDb.SQLite.System.IntegrationTests.Operations.SDS;

[TestClass]
public class ExecuteQueryMultipleTest : TestBase
{
    #region Sync

    [TestMethod]
    public void TestSqLiteConnectionExecuteQueryMultiple()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        using var extractor = connection.ExecuteQueryMultiple(@"SELECT * FROM [CompleteTable];
                    SELECT * FROM [CompleteTable];");
        var list = new List<IEnumerable<CompleteTable>>
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
    public void TestSqLiteConnectionExecuteQueryMultipleWithParameters()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        using var extractor = connection.ExecuteQueryMultiple(@"SELECT * FROM [CompleteTable] WHERE Id = @Id1;
                    SELECT * FROM [CompleteTable] WHERE Id = @Id2;",
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
    public void TestSqLiteConnectionExecuteQueryMultipleWithSharedParameters()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        using var extractor = connection.ExecuteQueryMultiple(@"SELECT * FROM [CompleteTable] WHERE Id = @Id;
                    SELECT * FROM [CompleteTable] WHERE Id = @Id;",
            new { Id = tables.Last().Id });
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

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqLiteConnectionExecuteQueryMultipleAsync()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        using var extractor = await connection.ExecuteQueryMultipleAsync(@"SELECT * FROM [CompleteTable];
                    SELECT * FROM [CompleteTable];", cancellationToken: TestContext.CancellationToken);
        var list = new List<IEnumerable<CompleteTable>>
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
    public async Task TestSqLiteConnectionExecuteQueryMultipleAsyncWithParameters()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        using var extractor = await connection.ExecuteQueryMultipleAsync(@"SELECT * FROM [CompleteTable] WHERE Id = @Id1;
                    SELECT * FROM [CompleteTable] WHERE Id = @Id2;",
            new
            {
                Id1 = tables.First().Id,
                Id2 = tables.Last().Id
            }, cancellationToken: TestContext.CancellationToken);
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
    public async Task TestSqLiteConnectionExecuteQueryMultipleAsyncWithSharedParameters()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var tables = Database.CreateCompleteTables(10, connection);

        // Act
        using var extractor = await connection.ExecuteQueryMultipleAsync(@"SELECT * FROM [CompleteTable] WHERE Id = @Id;
                    SELECT * FROM [CompleteTable] WHERE Id = @Id;",
            new { Id = tables.Last().Id }, cancellationToken: TestContext.CancellationToken);
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
    #endregion
}
