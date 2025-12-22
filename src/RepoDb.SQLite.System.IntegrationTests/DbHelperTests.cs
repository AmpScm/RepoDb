using System.Data.SQLite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SQLite.System.IntegrationTests.Setup;

namespace RepoDb.SQLite.System.IntegrationTests;

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
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var helper = connection.GetDbHelper();
        var tables = Database.CreateSdsCompleteTables(10, connection);

        // Act
        var fields = helper.GetFields(connection, "SdsCompleteTable", null);

        // Assert
        using var reader = connection.ExecuteReader("pragma table_info([SdsCompleteTable]);");
        var fieldCount = 0;

        while (reader.Read())
        {
            var name = reader.GetString(1);
            var field = fields.FirstOrDefault(f => string.Equals(f.FieldName, name, StringComparison.OrdinalIgnoreCase));

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
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var helper = connection.GetDbHelper();
        var tables = Database.CreateSdsCompleteTables(10, connection);

        // Act
        var fields = helper.GetFields(connection, "SdsCompleteTable", null);
        var primary = fields.FirstOrDefault(f => f.IsPrimary == true);

        // Assert
        Assert.IsNotNull(primary);
        Assert.AreEqual("Id", primary.FieldName);
    }

    [TestMethod]
    public void TestDbHelperGetFieldsIdentity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var helper = connection.GetDbHelper();
        var tables = Database.CreateSdsCompleteTables(10, connection);

        // Act
        var fields = helper.GetFields(connection, "SdsCompleteTable", null);
        var primary = fields.FirstOrDefault(f => f.IsIdentity == true);

        // Assert
        Assert.IsNotNull(primary);
        Assert.AreEqual("Id", primary.FieldName);
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestDbHelperGetFieldsAsync()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var helper = connection.GetDbHelper();
        var tables = Database.CreateSdsCompleteTables(10, connection);

        // Act
        var fields = await helper.GetFieldsAsync(connection, "SdsCompleteTable", null, TestContext.CancellationToken);

        // Assert
        using var reader = connection.ExecuteReader("pragma table_info([SdsCompleteTable]);");
        var fieldCount = 0;

        while (reader.Read())
        {
            var name = reader.GetString(1);
            var field = fields.FirstOrDefault(f => string.Equals(f.FieldName, name, StringComparison.OrdinalIgnoreCase));

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
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var helper = connection.GetDbHelper();
        var tables = Database.CreateSdsCompleteTables(10, connection);

        // Act
        var fields = await helper.GetFieldsAsync(connection, "SdsCompleteTable", null, TestContext.CancellationToken);
        var primary = fields.FirstOrDefault(f => f.IsPrimary == true);

        // Assert
        Assert.IsNotNull(primary);
        Assert.AreEqual("Id", primary.FieldName);
    }

    [TestMethod]
    public async Task TestDbHelperGetFieldsAsyncIdentity()
    {
        using var connection = new SQLiteConnection(Database.ConnectionString);
        // Setup
        var helper = connection.GetDbHelper();
        var tables = Database.CreateSdsCompleteTables(10, connection);

        // Act
        var fields = await helper.GetFieldsAsync(connection, "SdsCompleteTable", null, TestContext.CancellationToken);
        var primary = fields.FirstOrDefault(f => f.IsIdentity == true);

        // Assert
        Assert.IsNotNull(primary);
        Assert.AreEqual("Id", primary.FieldName);
    }

    public TestContext TestContext { get; set; }

    #endregion

    #endregion
}
