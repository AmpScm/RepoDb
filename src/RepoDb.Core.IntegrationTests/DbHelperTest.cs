using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.IntegrationTests;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;

namespace RepoDb.SqlServer.IntegrationTests;

[TestClass]
public class DbHelperTest
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
        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Setup
        var tables = Helper.CreateIdentityTables(10);
        var helper = connection.GetDbHelper();

        // Act
        var fields = helper.GetFields(connection, "[sc].[IdentityTable]".AsUnquoted(connection.GetDbSetting()), null);

        // Assert
        using var reader = connection.ExecuteReader("SELECT name FROM sys.columns WHERE object_id = OBJECT_ID(@TableName);",
            new { TableName = ClassMappedNameCache.Get<IdentityTable>() });
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

    [TestMethod]
    public void TestDbHelperGetFieldsPrimary()
    {
        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Setup
        var tables = Helper.CreateIdentityTables(10);
        var helper = connection.GetDbHelper();

        // Act
        var fields = helper.GetFields(connection, "[NonIdentityTable]".AsUnquoted(connection.GetDbSetting()), null);
        var primary = fields.FirstOrDefault(f => f.IsPrimary == true);

        // Assert
        Assert.IsNotNull(primary);
        Assert.AreEqual("Id", primary.FieldName);
    }

    [TestMethod]
    public void TestDbHelperGetFieldsIdentity()
    {
        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Setup
        var tables = Helper.CreateIdentityTables(10);
        var helper = connection.GetDbHelper();

        // Act
        var fields = helper.GetFields(connection, "[sc].[IdentityTable]".AsUnquoted(connection.GetDbSetting()), null);
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
        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Setup
        var tables = Helper.CreateIdentityTables(10);
        var helper = connection.GetDbHelper();

        // Act
        var fields = await helper.GetFieldsAsync(connection, "[sc].[IdentityTable]".AsUnquoted(connection.GetDbSetting()), null, TestContext.CancellationToken);

        // Assert
        using var reader = connection.ExecuteReader("SELECT name FROM sys.columns WHERE object_id = OBJECT_ID(@TableName);",
            new { TableName = ClassMappedNameCache.Get<IdentityTable>() });
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

    [TestMethod]
    public async Task TestDbHelperGetFieldsAsyncPrimary()
    {
        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Setup
        var tables = Helper.CreateIdentityTables(10);
        var helper = connection.GetDbHelper();

        // Act
        var fields = await helper.GetFieldsAsync(connection, "[NonIdentityTable]".AsUnquoted(connection.GetDbSetting()), null, TestContext.CancellationToken);
        var primary = fields.FirstOrDefault(f => f.IsPrimary == true);

        // Assert
        Assert.IsNotNull(primary);
        Assert.AreEqual("Id", primary.FieldName);
    }

    [TestMethod]
    public async Task TestDbHelperGetFieldsAsyncIdentity()
    {
        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Setup
        var tables = Helper.CreateIdentityTables(10);
        var helper = connection.GetDbHelper();

        // Act
        var fields = await helper.GetFieldsAsync(connection, "[sc].[IdentityTable]".AsUnquoted(connection.GetDbSetting()), null, TestContext.CancellationToken);
        var primary = fields.FirstOrDefault(f => f.IsIdentity == true);

        // Assert
        Assert.IsNotNull(primary);
        Assert.AreEqual("Id", primary.FieldName);
    }

    public TestContext TestContext { get; set; }

    #endregion

    #endregion
}
