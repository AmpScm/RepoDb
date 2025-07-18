using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.PostgreSql.IntegrationTests;

[TestClass]
public class DbHelperTests
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

    #region GetFields

    #region Sync

    [TestMethod]
    public void TestDbHelperGetFields()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Interfaces.IDbHelper helper = connection.GetDbHelper();

        // Act
        DbFieldCollection fields = helper.GetFields(connection, "CompleteTable", null);

        // Assert
        using System.Data.IDataReader reader = connection.ExecuteReader(@"SELECT COLUMN_NAME AS ColumnName
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE
                        TABLE_NAME = @TableName
                    ORDER BY ORDINAL_POSITION;", new { TableName = "CompleteTable" });
        int fieldCount = 0;

        while (reader.Read())
        {
            string name = reader.GetString(0);
            DbField field = fields.FirstOrDefault(f => string.Equals(f.FieldName, name, StringComparison.OrdinalIgnoreCase));

            // Assert
            Assert.IsNotNull(field);

            fieldCount++;
        }

        // Assert
        Assert.AreEqual(fieldCount, fields.Count);
    }

    [TestMethod]
    public void TestDbHelperGetFieldsPrimary()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Interfaces.IDbHelper helper = connection.GetDbHelper();

        // Act
        DbFieldCollection fields = helper.GetFields(connection, "CompleteTable", null);
        DbField primary = fields.FirstOrDefault(f => f.IsPrimary == true);

        // Assert
        Assert.IsNotNull(primary);
        Assert.AreEqual("Id", primary.FieldName);
    }

    [TestMethod]
    public void TestDbHelperGetFieldsIdentity()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Interfaces.IDbHelper helper = connection.GetDbHelper();

        // Act
        DbFieldCollection fields = helper.GetFields(connection, "CompleteTable", null);
        DbField primary = fields.FirstOrDefault(f => f.IsIdentity == true);

        // Assert
        Assert.IsNotNull(primary);
        Assert.AreEqual("Id", primary.FieldName);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestDbHelperGetFieldsAsync()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Interfaces.IDbHelper helper = connection.GetDbHelper();

        // Act
        DbFieldCollection fields = await helper.GetFieldsAsync(connection, "CompleteTable", null);

        // Assert
        using System.Data.IDataReader reader = connection.ExecuteReader(@"SELECT COLUMN_NAME AS ColumnName
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE
                        TABLE_NAME = @TableName
                    ORDER BY ORDINAL_POSITION;", new { TableName = "CompleteTable" });
        int fieldCount = 0;

        while (reader.Read())
        {
            string name = reader.GetString(0);
            DbField field = fields.FirstOrDefault(f => string.Equals(f.FieldName, name, StringComparison.OrdinalIgnoreCase));

            // Assert
            Assert.IsNotNull(field);

            fieldCount++;
        }

        // Assert
        Assert.AreEqual(fieldCount, fields.Count);
    }

    [TestMethod]
    public async Task TestDbHelperGetFieldsAsyncPrimary()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Interfaces.IDbHelper helper = connection.GetDbHelper();

        // Act
        DbFieldCollection fields = await helper.GetFieldsAsync(connection, "CompleteTable", null);
        DbField primary = fields.FirstOrDefault(f => f.IsPrimary == true);

        // Assert
        Assert.IsNotNull(primary);
        Assert.AreEqual("Id", primary.FieldName);
    }

    [TestMethod]
    public async Task TestDbHelperGetFieldsAsyncIdentity()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        Interfaces.IDbHelper helper = connection.GetDbHelper();

        // Act
        DbFieldCollection fields = await helper.GetFieldsAsync(connection, "CompleteTable", null);
        DbField primary = fields.FirstOrDefault(f => f.IsIdentity == true);

        // Assert
        Assert.IsNotNull(primary);
        Assert.AreEqual("Id", primary.FieldName);
    }

    #endregion

    #endregion
}
