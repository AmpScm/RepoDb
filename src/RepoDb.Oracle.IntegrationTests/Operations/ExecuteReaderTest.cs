using RepoDb.Extensions;
using RepoDb.Reflection;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;
using System.Data.Common;
using RepoDb.Trace;

namespace RepoDb.Oracle.IntegrationTests.Operations;

[TestClass]
public class ExecuteReaderTest : TestBase
{
    #region Sync

    [TestMethod]
    public void TestOracleConnectionExecuteReader()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        using System.Data.IDataReader reader = connection.ExecuteReader("SELECT \"Id\", \"ColumnInt\", \"ColumnDate\" FROM \"CompleteTable\"");
        while (reader.Read())
        {
            // Act
            long id = reader.GetInt64(0);
            int columnInt = reader.GetInt32(1);
            DateTime columnDateTime = reader.GetDateTime(2);
            CompleteTable table = tables.FirstOrDefault(e => e.Id == id);

            // Assert
            Assert.IsNotNull(table);
            Assert.AreEqual(columnInt, table.ColumnInt);
            Assert.AreEqual(columnDateTime, table.ColumnDate);
        }
    }

    [TestMethod]
    public void TestOracleConnectionExecuteReaderWithMultipleStatements()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        using System.Data.IDataReader reader = connection.ExecuteReader(connection.CombineQueries(["SELECT \"Id\", \"ColumnInt\", \"ColumnDate\" FROM \"CompleteTable\"", "SELECT \"Id\", \"ColumnInt\", \"ColumnDate\" FROM \"CompleteTable\""]));
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
                Assert.AreEqual(columnInt, table.ColumnInt);
                Assert.AreEqual(columnDateTime, table.ColumnDate);
            }
        } while (reader.NextResult());
    }

    [TestMethod]
    public void TestOracleConnectionExecuteReaderAsExtractedEntity()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        using System.Data.IDataReader reader = connection.ExecuteReader("SELECT * FROM \"CompleteTable\"");
        // Act
        List<CompleteTable> result = DataReader.ToEnumerable<CompleteTable>((DbDataReader)reader).AsList();

        // Assert
        tables.AsList().ForEach(table => Helper.AssertPropertiesEquality(table, result.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestOracleConnectionExecuteReaderAsExtractedDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        using System.Data.IDataReader reader = connection.ExecuteReader("SELECT * FROM \"CompleteTable\"");
        // Act
        List<dynamic> result = DataReader.ToEnumerable((DbDataReader)reader).AsList();

        // Assert
        tables.AsList().ForEach(table => Helper.AssertMembersEquality(table, result.First(e => e.Id == table.Id)));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestOracleConnectionExecuteReaderAsync()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        using System.Data.IDataReader reader = await connection.ExecuteReaderAsync("SELECT \"Id\", \"ColumnInt\", \"ColumnDate\" FROM \"CompleteTable\"", cancellationToken: TestContext.CancellationToken);
        while (reader.Read())
        {
            // Act
            long id = reader.GetInt64(0);
            int columnInt = reader.GetInt32(1);
            DateTime columnDateTime = reader.GetDateTime(2);
            CompleteTable table = tables.FirstOrDefault(e => e.Id == id);

            // Assert
            Assert.IsNotNull(table);
            Assert.AreEqual(columnInt, table.ColumnInt);
            Assert.AreEqual(columnDateTime, table.ColumnDate);
        }
    }

    [TestMethod]
    public async Task TestOracleConnectionExecuteReaderAsyncWithMultipleStatements()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        using System.Data.IDataReader reader = await connection.ExecuteReaderAsync(connection.CombineQueries(["SELECT \"Id\", \"ColumnInt\", \"ColumnDate\" FROM \"CompleteTable\"", "SELECT \"Id\", \"ColumnInt\", \"ColumnDate\" FROM \"CompleteTable\""]), cancellationToken: TestContext.CancellationToken, trace: new DiagnosticsTracer());
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
                Assert.AreEqual(columnInt, table.ColumnInt);
                Assert.AreEqual(columnDateTime, table.ColumnDate);
            }
        } while (reader.NextResult());
    }

    [TestMethod]
    public async Task TestOracleConnectionExecuteReaderAsyncAsExtractedEntity()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        using System.Data.IDataReader reader = await connection.ExecuteReaderAsync("SELECT * FROM \"CompleteTable\"", cancellationToken: TestContext.CancellationToken);
        // Act
        List<CompleteTable> result = DataReader.ToEnumerable<CompleteTable>((DbDataReader)reader).AsList();

        // Assert
        tables.AsList().ForEach(table => Helper.AssertPropertiesEquality(table, result.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestOracleConnectionExecuteReaderAsyncAsExtractedDynamic()
    {
        // Setup
        IEnumerable<CompleteTable> tables = Database.CreateCompleteTables(10);

        using var connection = CreateConnection();
        // Act
        using System.Data.IDataReader reader = await connection.ExecuteReaderAsync("SELECT * FROM \"CompleteTable\"", cancellationToken: TestContext.CancellationToken);
        // Act
        List<dynamic> result = DataReader.ToEnumerable((DbDataReader)reader).AsList();

        // Assert
        tables.AsList().ForEach(table => Helper.AssertMembersEquality(table, result.First(e => e.Id == table.Id)));
    }

    #endregion
}
