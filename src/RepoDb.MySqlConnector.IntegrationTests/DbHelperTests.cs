using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySqlConnector;
using RepoDb.MySqlConnector.IntegrationTests.Setup;

namespace RepoDb.MySqlConnector.IntegrationTests;

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
        using (var connection = new MySqlConnection(Database.ConnectionString))
        {
            // Setup
            var helper = connection.GetDbHelper();

            // Act
            var fields = helper.GetFields(connection, "CompleteTable", null);

            // Assert
            using (var reader = connection.ExecuteReader(@"SELECT COLUMN_NAME AS ColumnName
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE
                        TABLE_NAME = @TableName
                        AND TABLE_SCHEMA = @TableSchema
                    ORDER BY ORDINAL_POSITION;", new { TableName = "CompleteTable", TableSchema = connection.Database }))
            {
                var fieldCount = 0;

                while (reader.Read())
                {
                    var name = reader.GetString(0);
                    var field = fields.FirstOrDefault(f => string.Equals(f.FieldName, name, StringComparison.OrdinalIgnoreCase));

                    // Assert
                    Assert.IsNotNull(field);

                    fieldCount++;
                }

                // Assert
                Assert.AreEqual(fieldCount, fields.Count);
            }
        }
    }

    [TestMethod]
    public void TestDbHelperGetFieldsPrimary()
    {
        using (var connection = new MySqlConnection(Database.ConnectionString))
        {
            // Setup
            var helper = connection.GetDbHelper();

            // Act
            var fields = helper.GetFields(connection, "CompleteTable", null);
            var primary = fields.FirstOrDefault(f => f.IsPrimary == true);

            // Assert
            Assert.IsNotNull(primary);
            Assert.AreEqual("Id", primary.FieldName);
        }
    }

    [TestMethod]
    public void TestDbHelperGetFieldsIdentity()
    {
        using (var connection = new MySqlConnection(Database.ConnectionString))
        {
            // Setup
            var helper = connection.GetDbHelper();

            // Act
            var fields = helper.GetFields(connection, "CompleteTable", null);
            var primary = fields.FirstOrDefault(f => f.IsIdentity == true);

            // Assert
            Assert.IsNotNull(primary);
            Assert.AreEqual("Id", primary.FieldName);
        }
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestDbHelperGetFieldsAsync()
    {
        using (var connection = new MySqlConnection(Database.ConnectionString))
        {
            // Setup
            var helper = connection.GetDbHelper();

            // Act
            var fields = await helper.GetFieldsAsync(connection, "CompleteTable", null);

            // Assert
            using (var reader = connection.ExecuteReader(@"SELECT COLUMN_NAME AS ColumnName
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE
                        TABLE_NAME = @TableName
                        AND TABLE_SCHEMA = @TableSchema
                    ORDER BY ORDINAL_POSITION;", new { TableName = "CompleteTable", TableSchema = connection.Database }))
            {
                var fieldCount = 0;

                while (reader.Read())
                {
                    var name = reader.GetString(0);
                    var field = fields.FirstOrDefault(f => string.Equals(f.FieldName, name, StringComparison.OrdinalIgnoreCase));

                    // Assert
                    Assert.IsNotNull(field);

                    fieldCount++;
                }

                // Assert
                Assert.AreEqual(fieldCount, fields.Count);
            }
        }
    }

    [TestMethod]
    public async Task TestDbHelperGetFieldsAsyncPrimary()
    {
        using (var connection = new MySqlConnection(Database.ConnectionString))
        {
            // Setup
            var helper = connection.GetDbHelper();

            // Act
            var fields = await helper.GetFieldsAsync(connection, "CompleteTable", null);
            var primary = fields.FirstOrDefault(f => f.IsPrimary == true);

            // Assert
            Assert.IsNotNull(primary);
            Assert.AreEqual("Id", primary.FieldName);
        }
    }

    [TestMethod]
    public async Task TestDbHelperGetFieldsAsyncIdentity()
    {
        using (var connection = new MySqlConnection(Database.ConnectionString))
        {
            // Setup
            var helper = connection.GetDbHelper();

            // Act
            var fields = await helper.GetFieldsAsync(connection, "CompleteTable", null);
            var primary = fields.FirstOrDefault(f => f.IsIdentity == true);

            // Assert
            Assert.IsNotNull(primary);
            Assert.AreEqual("Id", primary.FieldName);
        }
    }

    #endregion

    #endregion
}
