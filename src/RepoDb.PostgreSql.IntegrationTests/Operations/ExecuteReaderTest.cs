using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Extensions;
using RepoDb.Reflection;
using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;
using System.Data.Common;

namespace RepoDb.PostgreSql.IntegrationTests.Operations;

[TestClass]
public class ExecuteReaderTest
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
    public void TestPostgreSqlConnectionExecuteReader()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        using System.Data.IDataReader reader = connection.ExecuteReader("SELECT \"Id\", \"ColumnInteger\", \"ColumnDate\" FROM \"CompleteTable\";");
        while (reader.Read())
        {
            // Act
            long id = reader.GetInt64(0);
            int columnInt = reader.GetInt32(1);
            DateTime columnDateTime = reader.GetDateTime(2);
            CompleteTable table = tables.FirstOrDefault(e => e.Id == id);

            // Assert
            Assert.IsNotNull(table);
            Assert.AreEqual(columnInt, table.ColumnInteger);
            Assert.AreEqual(columnDateTime, table.ColumnDate);
        }
    }

    [TestMethod]
    public void TestPostgreSqlConnectionExecuteReaderWithMultipleStatements()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        using System.Data.IDataReader reader = connection.ExecuteReader("SELECT \"Id\", \"ColumnInteger\", \"ColumnDate\" FROM \"CompleteTable\"; SELECT \"Id\", \"ColumnInteger\", \"ColumnDate\" FROM \"CompleteTable\";");
        do
        {
            while (reader.Read())
            {
                // Act
                long id = reader.GetInt64(0);
                int columnInt = reader.GetInt32(1);
                DateTime columnDateTime = reader.GetDateTime(2);
                CompleteTable table = tables.FirstOrDefault(e => e.Id == id);

                // Assert
                Assert.IsNotNull(table);
                Assert.AreEqual(columnInt, table.ColumnInteger);
                Assert.AreEqual(columnDateTime, table.ColumnDate);
            }
        } while (reader.NextResult());
    }

    [TestMethod]
    public void TestPostgreSqlConnectionExecuteReaderAsExtractedEntity()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        using System.Data.IDataReader reader = connection.ExecuteReader("SELECT * FROM \"CompleteTable\";");
        // Act
        List<CompleteTable> result = DataReader.ToEnumerable<CompleteTable>((DbDataReader)reader).AsList();

        // Assert
        tables.AsList().ForEach(table => Helper.AssertPropertiesEquality(table, result.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestPostgreSqlConnectionExecuteReaderAsExtractedDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        using System.Data.IDataReader reader = connection.ExecuteReader("SELECT * FROM \"CompleteTable\";");
        // Act
        List<dynamic> result = DataReader.ToEnumerable((DbDataReader)reader).AsList();

        // Assert
        tables.AsList().ForEach(table => Helper.AssertMembersEquality(table, result.First(e => e.Id == table.Id)));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestPostgreSqlConnectionExecuteReaderAsync()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        using System.Data.IDataReader reader = await connection.ExecuteReaderAsync("SELECT \"Id\", \"ColumnInteger\", \"ColumnDate\" FROM \"CompleteTable\";", cancellationToken: TestContext.CancellationToken);
        while (reader.Read())
        {
            // Act
            long id = reader.GetInt64(0);
            int columnInt = reader.GetInt32(1);
            DateTime columnDateTime = reader.GetDateTime(2);
            CompleteTable table = tables.FirstOrDefault(e => e.Id == id);

            // Assert
            Assert.IsNotNull(table);
            Assert.AreEqual(columnInt, table.ColumnInteger);
            Assert.AreEqual(columnDateTime, table.ColumnDate);
        }
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionExecuteReaderAsyncWithMultipleStatements()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        using System.Data.IDataReader reader = await connection.ExecuteReaderAsync("SELECT \"Id\", \"ColumnInteger\", \"ColumnDate\" FROM \"CompleteTable\"; SELECT \"Id\", \"ColumnInteger\", \"ColumnDate\" FROM \"CompleteTable\";", cancellationToken: TestContext.CancellationToken);
        do
        {
            while (reader.Read())
            {
                // Act
                long id = reader.GetInt64(0);
                int columnInt = reader.GetInt32(1);
                DateTime columnDateTime = reader.GetDateTime(2);
                CompleteTable table = tables.FirstOrDefault(e => e.Id == id);

                // Assert
                Assert.IsNotNull(table);
                Assert.AreEqual(columnInt, table.ColumnInteger);
                Assert.AreEqual(columnDateTime, table.ColumnDate);
            }
        } while (reader.NextResult());
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionExecuteReaderAsyncAsExtractedEntity()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        using System.Data.IDataReader reader = await connection.ExecuteReaderAsync("SELECT * FROM \"CompleteTable\";", cancellationToken: TestContext.CancellationToken);
        // Act
        List<CompleteTable> result = DataReader.ToEnumerable<CompleteTable>((DbDataReader)reader).AsList();

        // Assert
        tables.AsList().ForEach(table => Helper.AssertPropertiesEquality(table, result.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestPostgreSqlConnectionExecuteReaderAsyncAsExtractedDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        using System.Data.IDataReader reader = await connection.ExecuteReaderAsync("SELECT * FROM \"CompleteTable\";", cancellationToken: TestContext.CancellationToken);
        // Act
        List<dynamic> result = DataReader.ToEnumerable((DbDataReader)reader).AsList();

        // Assert
        tables.AsList().ForEach(table => Helper.AssertMembersEquality(table, result.First(e => e.Id == table.Id)));
    }

    public TestContext TestContext { get; set; }

    #endregion
}
