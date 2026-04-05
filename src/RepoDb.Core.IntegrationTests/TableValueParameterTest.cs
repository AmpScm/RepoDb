using System.Data;
using Microsoft.Data.SqlClient;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;

namespace RepoDb.IntegrationTests;

[TestClass]
public class TableValueParameterTest : TestBase
{
    #region Helpers

    private static DataTable CreateDataTable(string tableName)
    {
        var table = new DataTable
        {
            TableName = tableName
        };
        return table;
    }

    private static void CreateColumns(DataTable table)
    {
        table.Columns.Add(new DataColumn("Id", typeof(long)));
        table.Columns.Add(new DataColumn("RowGuid", typeof(Guid)));
        table.Columns.Add(new DataColumn("ColumnBit", typeof(bool)));
        table.Columns.Add(new DataColumn("ColumnDateTime", typeof(DateTime)));
        table.Columns.Add(new DataColumn("ColumnDateTime2", typeof(DateTime)));
        table.Columns.Add(new DataColumn("ColumnDecimal", typeof(decimal)));
        table.Columns.Add(new DataColumn("ColumnFloat", typeof(double)));
        table.Columns.Add(new DataColumn("ColumnInt", typeof(int)));
        table.Columns.Add(new DataColumn("ColumnNVarChar", typeof(string)));
    }

    private static void CreateRows(DataTable table,
        int count = 10)
    {
        var random = new Random();
        for (var i = 0; i < count; i++)
        {
            var row = table.NewRow();
            row["Id"] = i + 1;
            row["RowGuid"] = Guid.NewGuid();
            row["ColumnBit"] = true;
            row["ColumnDateTime"] = DateTime.UtcNow.Date;
            row["ColumnDateTime2"] = DateTime.UtcNow;
            row["ColumnDecimal"] = Convert.ToDecimal(random.Next(1000000));
            row["ColumnFloat"] = Convert.ToSingle(random.Next(1000000));
            row["ColumnInt"] = random.Next(int.MaxValue);
            row["ColumnNVarChar"] = $"NVarChar-{i}-{Guid.NewGuid()}";
            table.Rows.Add(row);
        }
    }

    private DataTable CreateIdentityTableType(int count = 10)
    {
        var table = CreateDataTable("IdentityTableType");
        CreateColumns(table);
        CreateRows(table, count);
        return table;
    }

    private static string GetSqlText()
    {
        return @"CREATE PROC sp_ExecuteQueryByType(@Table IdentityType READONLY)
                AS
                BEGIN
                    SELECT * FROM @Table;
                END;";
    }

    #endregion

    #region ExecuteQuery

    #region Sync

    [TestMethod]
    public void TestExecuteQueryForTableValuedParameter()
    {
        // Setup
        var dataTable = CreateIdentityTableType(10);

        using var connection = CreateConnection();
        // Act
        var tables = connection.ExecuteQuery<IdentityTable>("EXEC [sp_identity_table_type] @Table = @Table;",
            new { Table = dataTable })?.AsList();

        // Assert
        Assert.HasCount(dataTable.Rows.Count, tables);

        // Act
        var queryResult = connection.QueryAll<IdentityTable>().AsList();

        // Assert
        Assert.AreEqual(dataTable.Rows.Count, connection.CountAll<IdentityTable>());
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestExecuteNonQueryForTableValuedParameter()
    {
        // Setup
        var dataTable = CreateIdentityTableType(10);

        using var connection = CreateConnection();
        // Act
        var result = connection.ExecuteNonQuery("EXEC [sp_identity_table_type] @Table = @Table;",
            new { Table = dataTable });

        // Assert
        Assert.AreEqual(dataTable.Rows.Count, result);
        Assert.AreEqual(dataTable.Rows.Count, connection.CountAll<IdentityTable>());
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestExecuteQueryAsyncForTableValuedParameter()
    {
        // Setup
        var dataTable = CreateIdentityTableType(10);

        using var connection = CreateConnection();
        // Act
        var tables = (await connection.ExecuteQueryAsync<IdentityTable>("EXEC [sp_identity_table_type] @Table = @Table;",
            new { Table = dataTable }, cancellationToken: TestContext.CancellationToken))?.AsList();

        // Assert
        Assert.HasCount(dataTable.Rows.Count, tables);

        // Act
        var queryResult = connection.QueryAll<IdentityTable>().AsList();

        // Assert
        Assert.AreEqual(dataTable.Rows.Count, connection.CountAll<IdentityTable>());
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public async Task TestExecuteNonQueryAsyncForTableValuedParameter()
    {
        // Setup
        var dataTable = CreateIdentityTableType(10);

        using var connection = CreateConnection();
        // Act
        var result = await connection.ExecuteNonQueryAsync("EXEC [sp_identity_table_type] @Table = @Table;",
            new { Table = dataTable }, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(dataTable.Rows.Count, result);
        Assert.AreEqual(dataTable.Rows.Count, connection.CountAll<IdentityTable>());
    }

    #endregion

    #endregion
}
