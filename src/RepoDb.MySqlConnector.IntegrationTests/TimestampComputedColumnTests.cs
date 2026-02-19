using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySqlConnector;
using RepoDb.MySqlConnector.IntegrationTests.Setup;

namespace RepoDb.MySqlConnector.IntegrationTests;

/// <summary>
/// Tests to demonstrate the distinction between DEFAULT CURRENT_TIMESTAMP and ON UPDATE CURRENT_TIMESTAMP
/// </summary>
[TestClass]
public class TimestampComputedColumnTests
{
    private const string TestTableName = "TimestampTestTable";

    [TestInitialize]
    public void Initialize()
    {
        Database.Initialize();
        CreateTestTable();
    }

    [TestCleanup]
    public void Cleanup()
    {
        DropTestTable();
    }

    private void CreateTestTable()
    {
        using var connection = new MySqlConnection(Database.ConnectionString);
        connection.ExecuteNonQuery($@"
            CREATE TABLE IF NOT EXISTS `{TestTableName}` (
                `Id` INT NOT NULL AUTO_INCREMENT,
                `Name` VARCHAR(100),
                `CreatedAt` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                `UpdatedAt` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                `OnUpdateOnly` TIMESTAMP NULL ON UPDATE CURRENT_TIMESTAMP,
                PRIMARY KEY (`Id`)
            ) ENGINE=InnoDB;");
    }

    private void DropTestTable()
    {
        using var connection = new MySqlConnection(Database.ConnectionString);
        connection.ExecuteNonQuery($"DROP TABLE IF EXISTS `{TestTableName}`;");
    }

    /// <summary>
    /// Test to verify that DEFAULT CURRENT_TIMESTAMP (DEFAULT_GENERATED) allows explicit values
    /// </summary>
    [TestMethod]
    public void TestDefaultCurrentTimestampCanBeOverridden()
    {
        using var connection = new MySqlConnection(Database.ConnectionString);
        
        // Insert with explicit timestamp for CreatedAt
        var explicitTimestamp = new DateTime(2020, 1, 1, 12, 0, 0);
        var insertSql = $@"
            INSERT INTO `{TestTableName}` (Name, CreatedAt, UpdatedAt)
            VALUES (@Name, @CreatedAt, @UpdatedAt);
            SELECT LAST_INSERT_ID();";
        
        var id = connection.ExecuteScalar<int>(insertSql, new
        {
            Name = "Test",
            CreatedAt = explicitTimestamp,
            UpdatedAt = explicitTimestamp
        });

        // Verify the explicit timestamp was used
        var querySql = $"SELECT CreatedAt FROM `{TestTableName}` WHERE Id = @Id";
        var createdAt = connection.ExecuteScalar<DateTime>(querySql, new { Id = id });
        
        // The CreatedAt should be the explicit timestamp we provided
        Assert.AreEqual(explicitTimestamp.ToString("yyyy-MM-dd HH:mm:ss"), createdAt.ToString("yyyy-MM-dd HH:mm:ss"),
            "DEFAULT CURRENT_TIMESTAMP should allow explicit values to be provided during INSERT");
    }

    /// <summary>
    /// Test to verify that ON UPDATE CURRENT_TIMESTAMP automatically updates
    /// </summary>
    [TestMethod]
    public void TestOnUpdateCurrentTimestampAutoUpdates()
    {
        using var connection = new MySqlConnection(Database.ConnectionString);
        
        // Insert a row
        var insertSql = $@"
            INSERT INTO `{TestTableName}` (Name)
            VALUES (@Name);
            SELECT LAST_INSERT_ID();";
        
        var id = connection.ExecuteScalar<int>(insertSql, new { Name = "Test" });

        // Get initial timestamps
        var selectSql = $"SELECT CreatedAt, UpdatedAt FROM `{TestTableName}` WHERE Id = @Id";
        var initial = connection.ExecuteQuery<dynamic>(selectSql, new { Id = id }).First();
        var initialCreatedAt = (DateTime)initial.CreatedAt;
        var initialUpdatedAt = (DateTime)initial.UpdatedAt;

        // Wait a moment to ensure timestamp difference
        System.Threading.Thread.Sleep(1500);

        // Update the row
        var updateSql = $"UPDATE `{TestTableName}` SET Name = @Name WHERE Id = @Id";
        connection.ExecuteNonQuery(updateSql, new { Name = "Updated", Id = id });

        // Get new timestamps
        var updated = connection.ExecuteQuery<dynamic>(selectSql, new { Id = id }).First();
        var finalCreatedAt = (DateTime)updated.CreatedAt;
        var finalUpdatedAt = (DateTime)updated.UpdatedAt;

        // CreatedAt should NOT have changed (DEFAULT CURRENT_TIMESTAMP doesn't auto-update)
        Assert.AreEqual(initialCreatedAt.ToString("yyyy-MM-dd HH:mm:ss"), finalCreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
            "CreatedAt with DEFAULT CURRENT_TIMESTAMP should NOT auto-update");

        // UpdatedAt SHOULD have changed (ON UPDATE CURRENT_TIMESTAMP auto-updates)
        Assert.AreNotEqual(initialUpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"), finalUpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
            "UpdatedAt with ON UPDATE CURRENT_TIMESTAMP should auto-update");
    }

    /// <summary>
    /// Test to verify DbHelper correctly identifies which columns are computed
    /// </summary>
    [TestMethod]
    public void TestDbHelperIdentifiesComputedColumns()
    {
        using var connection = new MySqlConnection(Database.ConnectionString);
        var helper = connection.GetDbHelper();

        // Get fields for the test table
        var fields = helper.GetFields(connection, TestTableName, null);

        // Find specific columns
        var createdAtField = fields.FirstOrDefault(f => f.FieldName == "CreatedAt");
        var updatedAtField = fields.FirstOrDefault(f => f.FieldName == "UpdatedAt");
        var onUpdateOnlyField = fields.FirstOrDefault(f => f.FieldName == "OnUpdateOnly");

        Assert.IsNotNull(createdAtField, "CreatedAt field should exist");
        Assert.IsNotNull(updatedAtField, "UpdatedAt field should exist");
        Assert.IsNotNull(onUpdateOnlyField, "OnUpdateOnly field should exist");

        // CreatedAt has DEFAULT CURRENT_TIMESTAMP only - should NOT be marked as generated
        // because it can be overridden during INSERT
        Assert.IsFalse(createdAtField.IsGenerated,
            "CreatedAt with only DEFAULT CURRENT_TIMESTAMP should NOT be marked as generated - it can be overridden during INSERT");

        // UpdatedAt has both DEFAULT CURRENT_TIMESTAMP and ON UPDATE CURRENT_TIMESTAMP
        // should be marked as generated because it auto-updates
        Assert.IsTrue(updatedAtField.IsGenerated,
            "UpdatedAt with ON UPDATE CURRENT_TIMESTAMP should be marked as generated");

        // OnUpdateOnly has only ON UPDATE CURRENT_TIMESTAMP - should be marked as generated
        Assert.IsTrue(onUpdateOnlyField.IsGenerated,
            "OnUpdateOnly with ON UPDATE CURRENT_TIMESTAMP should be marked as generated");
    }

    /// <summary>
    /// Test to verify EXTRA column values from MySQL
    /// </summary>
    [TestMethod]
    public void TestExtraColumnValues()
    {
        using var connection = new MySqlConnection(Database.ConnectionString);

        var sql = @"
            SELECT COLUMN_NAME, EXTRA, COLUMN_DEFAULT
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_SCHEMA = @Schema
                AND TABLE_NAME = @Table
                AND COLUMN_NAME IN ('CreatedAt', 'UpdatedAt', 'OnUpdateOnly')
            ORDER BY COLUMN_NAME";

        using var reader = connection.ExecuteReader(sql, new
        {
            Schema = connection.Database,
            Table = TestTableName
        });

        var results = new Dictionary<string, (string Extra, string ColumnDefault)>();
        while (reader.Read())
        {
            var columnName = reader.GetString(0);
            var extra = reader.IsDBNull(1) ? "" : reader.GetString(1);
            var columnDefault = reader.IsDBNull(2) ? "" : reader.GetString(2);
            results[columnName] = (extra, columnDefault);
        }

        // Verify we got all three columns
        Assert.IsTrue(results.ContainsKey("CreatedAt"), "Should find CreatedAt column");
        Assert.IsTrue(results.ContainsKey("UpdatedAt"), "Should find UpdatedAt column");
        Assert.IsTrue(results.ContainsKey("OnUpdateOnly"), "Should find OnUpdateOnly column");

        // Log the EXTRA values for debugging
        TestContext.WriteLine($"CreatedAt EXTRA: '{results["CreatedAt"].Extra}', DEFAULT: '{results["CreatedAt"].ColumnDefault}'");
        TestContext.WriteLine($"UpdatedAt EXTRA: '{results["UpdatedAt"].Extra}', DEFAULT: '{results["UpdatedAt"].ColumnDefault}'");
        TestContext.WriteLine($"OnUpdateOnly EXTRA: '{results["OnUpdateOnly"].Extra}', DEFAULT: '{results["OnUpdateOnly"].ColumnDefault}'");

        // CreatedAt should NOT have "ON UPDATE" in EXTRA
        Assert.IsFalse(results["CreatedAt"].Extra.Contains("ON UPDATE", StringComparison.OrdinalIgnoreCase),
            "CreatedAt should NOT have ON UPDATE in EXTRA column");

        // UpdatedAt and OnUpdateOnly SHOULD have "ON UPDATE" in EXTRA
        Assert.IsTrue(results["UpdatedAt"].Extra.Contains("ON UPDATE", StringComparison.OrdinalIgnoreCase),
            "UpdatedAt should have ON UPDATE in EXTRA column");
        Assert.IsTrue(results["OnUpdateOnly"].Extra.Contains("ON UPDATE", StringComparison.OrdinalIgnoreCase),
            "OnUpdateOnly should have ON UPDATE in EXTRA column");
    }

    public TestContext TestContext { get; set; }
}
