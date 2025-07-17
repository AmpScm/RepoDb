using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Setup;

namespace RepoDb.Sqlite.Microsoft.IntegrationTests;

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
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var helper = connection.GetDbHelper();
        var tables = Database.CreateMdsCompleteTables(10, connection);

        // Act
        var fields = helper.GetFields(connection, "MdsCompleteTable", null);

        // Assert
        using var reader = connection.ExecuteReader("pragma table_info([MdsCompleteTable]);");
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
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var helper = connection.GetDbHelper();
        var tables = Database.CreateMdsCompleteTables(10, connection);

        // Act
        var fields = helper.GetFields(connection, "MdsCompleteTable", null);
        var primary = fields.FirstOrDefault(f => f.IsPrimary == true);

        // Assert
        Assert.IsNotNull(primary);
        Assert.AreEqual("Id", primary.FieldName);
    }

    [TestMethod]
    public void TestDbHelperGetFieldsIdentity()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var helper = connection.GetDbHelper();
        var tables = Database.CreateMdsCompleteTables(10, connection);

        // Act
        var fields = helper.GetFields(connection, "MdsCompleteTable", null);
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
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var helper = connection.GetDbHelper();
        var tables = Database.CreateMdsCompleteTables(10, connection);

        // Act
        var fields = await helper.GetFieldsAsync(connection, "MdsCompleteTable", null);

        // Assert
        using var reader = connection.ExecuteReader("pragma table_info([MdsCompleteTable]);");
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
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var helper = connection.GetDbHelper();
        var tables = Database.CreateMdsCompleteTables(10, connection);

        // Act
        var fields = await helper.GetFieldsAsync(connection, "MdsCompleteTable", null);
        var primary = fields.FirstOrDefault(f => f.IsPrimary == true);

        // Assert
        Assert.IsNotNull(primary);
        Assert.AreEqual("Id", primary.FieldName);
    }

    [TestMethod]
    public async Task TestDbHelperGetFieldsAsyncIdentity()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        // Setup
        var helper = connection.GetDbHelper();
        var tables = Database.CreateMdsCompleteTables(10, connection);

        // Act
        var fields = await helper.GetFieldsAsync(connection, "MdsCompleteTable", null);
        var primary = fields.FirstOrDefault(f => f.IsIdentity == true);

        // Assert
        Assert.IsNotNull(primary);
        Assert.AreEqual("Id", primary.FieldName);
    }

    [TestMethod]
    public async Task TestDbHelperPrimaryKeyIdentity()
    {
        // Issue #802
        //
        var td = @"
         CREATE TABLE [Articles] (
          [ID] INTEGER NOT NULL UNIQUE,
          [ArticleID] TEXT,
          [Title] TEXT NOT NULL,
          [Description] TEXT,
          [Date_Added] INTEGER NOT NULL,
          [Date_Fetched] INTEGER,
          PRIMARY KEY([ID] AUTOINCREMENT)
          )
        ";

        // ID must be ident
        using var connection = new SqliteConnection(Database.ConnectionString);
        await connection.ExecuteNonQueryAsync(td.Replace("Articles", "ArtV1"));
        await connection.ExecuteNonQueryAsync(td.Replace("Articles", "ArtV2").Replace('[', '"').Replace(']', '"'));
        await connection.ExecuteNonQueryAsync(td.Replace("Articles", "ArtV3").Replace("[", "").Replace("]", ""));

        // Setup
        var helper = connection.GetDbHelper();

        var v1Fields = helper.GetFields(connection, "ArtV1");
        var v2Fields = helper.GetFields(connection, "ArtV2");
        var v3Fields = helper.GetFields(connection, "ArtV3");

        Assert.AreEqual(v1Fields.Count, v2Fields.Count);
        Assert.AreEqual(v1Fields.Count, v3Fields.Count);

#if NET
        foreach (var i in v1Fields.Zip(v2Fields, v3Fields))
        {
            Assert.AreEqual(i.First.FieldName, i.Second.FieldName);
            Assert.AreEqual(i.First.FieldName, i.Third.FieldName);

            Assert.AreEqual(i.First.IsPrimary, i.Second.IsPrimary);
            Assert.AreEqual(i.First.IsPrimary, i.Third.IsPrimary);

            Assert.AreEqual(i.First.IsIdentity, i.Second.IsIdentity);
            Assert.AreEqual(i.First.IsIdentity, i.Third.IsIdentity);
        }
#endif

        Assert.IsNotNull(v1Fields.FirstOrDefault(x => x.IsIdentity));
        Assert.IsNotNull(v2Fields.FirstOrDefault(x => x.IsIdentity));
        Assert.IsNotNull(v3Fields.FirstOrDefault(x => x.IsIdentity));
    }

    [TestMethod]
    public async Task TestDbHelperPrimaryKeyIdentityV2()
    {
        // Using varchar in fields
        //
        var td = @"
         CREATE TABLE [Articles] (
          [ID] INTEGER NOT NULL UNIQUE,
          [ArticleID] varchar(10),
          [Title] [varchar](10) NOT NULL,
          [Description] TEXT,
          [Date_Added] INTEGER NOT NULL,
          [Date_Fetched] INTEGER,
          PRIMARY KEY([ID] AUTOINCREMENT)
          )
        ";

        // ID must be ident
        using var connection = new SqliteConnection(Database.ConnectionString);
        await connection.ExecuteNonQueryAsync(td.Replace("Articles", "ArtV4"));
        await connection.ExecuteNonQueryAsync(td.Replace("Articles", "ArtV5").Replace('[', '"').Replace(']', '"'));
        await connection.ExecuteNonQueryAsync(td.Replace("Articles", "ArtV6").Replace("[", "").Replace("]", ""));

        // Setup
        var helper = connection.GetDbHelper();

        var v1Fields = helper.GetFields(connection, "ArtV4");
        var v2Fields = helper.GetFields(connection, "ArtV5");
        var v3Fields = helper.GetFields(connection, "ArtV6");

        Assert.AreEqual(v1Fields.Count, v2Fields.Count);
        Assert.AreEqual(v1Fields.Count, v3Fields.Count);

#if NET
        foreach (var i in v1Fields.Zip(v2Fields, v3Fields))
        {
            Assert.AreEqual(i.First.FieldName, i.Second.FieldName);
            Assert.AreEqual(i.First.FieldName, i.Third.FieldName);

            Assert.AreEqual(i.First.IsPrimary, i.Second.IsPrimary);
            Assert.AreEqual(i.First.IsPrimary, i.Third.IsPrimary);

            Assert.AreEqual(i.First.IsIdentity, i.Second.IsIdentity);
            Assert.AreEqual(i.First.IsIdentity, i.Third.IsIdentity);

            Console.WriteLine(i.First.DatabaseType);
        }
#endif

        Assert.IsNotNull(v1Fields.FirstOrDefault(x => x.IsIdentity));
        Assert.IsNotNull(v2Fields.FirstOrDefault(x => x.IsIdentity));
        Assert.IsNotNull(v3Fields.FirstOrDefault(x => x.IsIdentity));
    }

    #endregion

    #endregion
}
