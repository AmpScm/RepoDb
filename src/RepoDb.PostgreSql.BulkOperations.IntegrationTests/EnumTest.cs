using System.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.IntegrationTests.Setup;
using RepoDb.PostgreSql.BulkOperations.IntegrationTests.Enumerations;
using RepoDb.PostgreSql.BulkOperations.IntegrationTests.Models;

namespace RepoDb.PostgreSql.BulkOperations.IntegrationTests;

[TestClass]
public class EnumTest
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

    #region Methods

    private NpgsqlConnection GetConnection() =>
        this.CreateTestConnection().EnsureOpen();

    public static List<EnumTable> CreateEnumTablesWithNullValues(int count,
        bool hasId = false,
        long addToKey = 0)
    {
        var tables = new List<EnumTable>();
        for (var i = 0; i < count; i++)
        {
            var index = i + 1;
            tables.Add(new EnumTable
            {
                Id = (long)(hasId ? index + addToKey : 0),
                ColumnEnumHand = null,
                ColumnEnumInt = null,
                ColumnEnumText = null
            });
        }
        return tables;
    }

    public static List<dynamic> CreateEnumTablesForAnonymousWithNullValues(int count,
        bool hasId = false,
        long addToKey = 0)
    {
        var tables = new List<dynamic>();
        for (var i = 0; i < count; i++)
        {
            var index = i + 1;
            tables.Add(new
            {
                Id = (long)(hasId ? index + addToKey : 0),
                ColumnEnumHand = (Hands?)null,
                ColumnEnumInt = (Hands?)null,
                ColumnEnumText = (Hands?)null
            });
        }
        return tables;
    }

    public static List<dynamic> CreateEnumTablesForExpandoObjectWithNullValues(int count,
        bool hasId = false,
        long addToKey = 0)
    {
        var tables = new List<dynamic>();
        for (var i = 0; i < count; i++)
        {
            var expandoObject = new ExpandoObject() as IDictionary<string, object>;
            var index = i + 1;
            expandoObject["Id"] = (long)(hasId ? index + addToKey : 0);
            expandoObject["ColumnEnumHand"] = (Hands?)null;
            expandoObject["ColumnEnumInt"] = (Hands?)null;
            expandoObject["ColumnEnumText"] = (Hands?)null;
            tables.Add((ExpandoObject)expandoObject);
        }
        return tables;
    }

    public static List<dynamic> CreateEnumTablesForDataTable(int count,
        bool hasId = false,
        long addToKey = 0)
    {
        var tables = new List<dynamic>();
        for (var i = 0; i < count; i++)
        {
            var index = i + 1;
            tables.Add(new
            {
                Id = (long)(hasId ? index + addToKey : 0),
                ColumnEnumHand = Hands.Right,
                ColumnEnumInt = (int?)Hands.Left,
                ColumnEnumText = Hands.Unidentified.ToString()
            });
        }
        return tables;
    }

    public static List<dynamic> CreateEnumTablesForDataTableWithNullValues(int count,
        bool hasId = false,
        long addToKey = 0)
    {
        var tables = new List<dynamic>();
        for (var i = 0; i < count; i++)
        {
            var index = i + 1;
            tables.Add(new
            {
                Id = (long)(hasId ? index + addToKey : 0),
                ColumnEnumHand = (Hands?)null,
                ColumnEnumInt = (int?)null,
                ColumnEnumText = (string)null
            });
        }
        return tables;
    }

    #endregion

    #region Sync

    #region TEntity

    #region BinaryBulkInsert

    [TestMethod]
    public void TestBinaryBulkInsertForEnum()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateEnumTables(10, false);
            var tableName = "EnumTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertForEnumWithNullValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesWithNullValues(10, false);
            var tableName = "EnumTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    #endregion

    #region BinaryBulkDelete

    [TestMethod]
    public void TestBinaryBulkDeleteForEnum()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateEnumTables(10, false);
            var tableName = "EnumTable";

            // Act
            connection.InsertAll(entities);

            // Act
            var result = connection.BinaryBulkDelete(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll<EnumTable>();
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public void TestBinaryBulkDeleteForEnumWithNullValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesWithNullValues(10, false);
            var tableName = "EnumTable";

            // Act
            connection.InsertAll(entities);

            // Act
            var result = connection.BinaryBulkDelete(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll<EnumTable>();
            Assert.AreEqual(0, countResult);
        }
    }

    #endregion

    #region BinaryBulkMerge

    [TestMethod]
    public void TestBinaryBulkMergeForEnum()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateEnumTables(10, false);
            var tableName = "EnumTable";

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeForEnumWithNullValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesWithNullValues(10, false);
            var tableName = "EnumTable";

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    #endregion

    #region BinaryBulkUpdate

    [TestMethod]
    public void TestBinaryBulkUpdateForEnum()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateEnumTables(10, false);
            var tableName = "EnumTable";

            // Act
            connection.InsertAll(entities);

            // Act
            var result = connection.BinaryBulkUpdate(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkUpdateForEnumWithNullValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesWithNullValues(10, false);
            var tableName = "EnumTable";

            // Act
            connection.InsertAll(entities);

            // Act
            var result = connection.BinaryBulkUpdate(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    #endregion

    #endregion

    #region Anonymous

    #region BinaryBulkInsert

    [TestMethod]
    public void TestBinaryBulkInsertForEnumForAnonymous()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateEnumTableAnonymousTables(10, false);
            var tableName = "EnumTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertForEnumForAnonymousWithNullValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesForAnonymousWithNullValues(10, false);
            var tableName = "EnumTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    #endregion

    #region BinaryBulkDelete

    [TestMethod]
    public void TestBinaryBulkDeleteForEnumForAnonymous()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateEnumTableAnonymousTables(10, true);
            var tableName = "EnumTable";

            // Act
            connection.InsertAll(tableName, entities);

            // Act
            var result = connection.BinaryBulkDelete(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll<EnumTable>();
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public void TestBinaryBulkDeleteForEnumForAnonymousWithNullValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesForAnonymousWithNullValues(10, true);
            var tableName = "EnumTable";

            // Act
            connection.InsertAll(tableName, entities);

            // Act
            var result = connection.BinaryBulkDelete(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll<EnumTable>();
            Assert.AreEqual(0, countResult);
        }
    }

    #endregion

    #region BinaryBulkMerge

    [TestMethod]
    public void TestBinaryBulkMergeForEnumForAnonymous()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateEnumTableAnonymousTables(10, false);
            var tableName = "EnumTable";

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeForEnumForAnonymousWithNullValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesForAnonymousWithNullValues(10, false);
            var tableName = "EnumTable";

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    #endregion

    #region BinaryBulkUpdate

    [TestMethod]
    public void TestBinaryBulkUpdateForEnumForAnonymous()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateEnumTableAnonymousTables(10, true);
            var tableName = "EnumTable";

            // Act
            connection.InsertAll(tableName, entities);

            // Act
            var result = connection.BinaryBulkUpdate(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkUpdateForEnumForAnonymousWithNullValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesForAnonymousWithNullValues(10, true);
            var tableName = "EnumTable";

            // Act
            connection.InsertAll(tableName, entities);

            // Act
            var result = connection.BinaryBulkUpdate(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    #endregion

    #endregion

    #region IDictionary<string, object>

    #region BinaryBulkInsert

    [TestMethod]
    public void TestBinaryBulkInsertForEnumForExpandoObject()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateEnumTableExpandoObjectTables(10, false);
            var tableName = "EnumTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertForEnumForExpandoObjectWithNullValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesForExpandoObjectWithNullValues(10, false);
            var tableName = "EnumTable";

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    #endregion

    #region BinaryBulkDelete

    [TestMethod]
    public void TestBinaryBulkDeleteForEnumForExpandoObject()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateEnumTableExpandoObjectTables(10, true);
            var tableName = "EnumTable";

            // Act
            connection.InsertAll(tableName, entities);

            // Act
            var result = connection.BinaryBulkDelete(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll<EnumTable>();
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public void TestBinaryBulkDeleteForEnumForExpandoObjectWithNullValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesForExpandoObjectWithNullValues(10, true);
            var tableName = "EnumTable";

            // Act
            connection.InsertAll(tableName, entities);

            // Act
            var result = connection.BinaryBulkDelete(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll<EnumTable>();
            Assert.AreEqual(0, countResult);
        }
    }

    #endregion

    #region BinaryBulkMerge

    [TestMethod]
    public void TestBinaryBulkMergeForEnumForExpandoObject()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateEnumTableExpandoObjectTables(10, false);
            var tableName = "EnumTable";

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeForEnumForExpandoObjectWithNullValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesForExpandoObjectWithNullValues(10, false);
            var tableName = "EnumTable";

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    #endregion

    #region BinaryBulkUpdate

    [TestMethod]
    public void TestBinaryBulkUpdateForEnumForExpandoObject()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateEnumTableExpandoObjectTables(10, true);
            var tableName = "EnumTable";

            // Act
            connection.InsertAll(tableName, entities);

            // Act
            var result = connection.BinaryBulkUpdate(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public void TestBinaryBulkUpdateForEnumForExpandoObjectWithNullValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesForExpandoObjectWithNullValues(10, true);
            var tableName = "EnumTable";

            // Act
            connection.InsertAll(tableName, entities);

            // Act
            var result = connection.BinaryBulkUpdate(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    #endregion

    #endregion

    #region DataTable

    #region BinaryBulkInsert

    [TestMethod]
    public void TestBinaryBulkInsertForEnumForDataTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesForDataTable(10, false);
            var tableName = "EnumTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                table);

            // Assert
            Assert.AreEqual(entities.Count, result);
        }
    }

    [TestMethod]
    public void TestBinaryBulkInsertForEnumForDataTableWithNullValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesForDataTableWithNullValues(10, false);
            var tableName = "EnumTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = connection.BinaryBulkInsert(
                tableName,
                table);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    #endregion

    #region BinaryBulkDelete

    [TestMethod]
    public void TestBinaryBulkDeleteForEnumForDataTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesForDataTable(10, true);
            var tableName = "EnumTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            connection.InsertAll(tableName, entities);

            // Act
            var result = connection.BinaryBulkDelete(
                tableName,
                table);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll<EnumTable>();
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public void TestBinaryBulkDeleteForEnumForDataTableWithNullValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesForDataTableWithNullValues(10, true);
            var tableName = "EnumTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            connection.InsertAll(tableName, entities);

            // Act
            var result = connection.BinaryBulkDelete(
                tableName,
                table);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll<EnumTable>();
            Assert.AreEqual(0, countResult);
        }
    }

    #endregion

    #region BinaryBulkMerge

    [TestMethod]
    public void TestBinaryBulkMergeForEnumForDataTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesForDataTable(10, false);
            var tableName = "EnumTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                table);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll<EnumTable>();
            Assert.AreEqual(entities.Count, result);
        }
    }

    [TestMethod]
    public void TestBinaryBulkMergeForEnumForDataTableWithNullValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesForDataTableWithNullValues(10, false);
            var tableName = "EnumTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = connection.BinaryBulkMerge(
                tableName,
                table);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll<EnumTable>();
            Assert.AreEqual(entities.Count, result);
        }
    }

    #endregion

    #region BinaryBulkUpdate

    [TestMethod]
    public void TestBinaryBulkUpdateForEnumForDataTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesForDataTable(10, true);
            var tableName = "EnumTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            connection.InsertAll(tableName, entities);

            // Act
            var result = connection.BinaryBulkUpdate(
                tableName,
                table);

            // Assert
            Assert.AreEqual(entities.Count, result);
        }
    }

    [TestMethod]
    public void TestBinaryBulkUpdateForEnumForDataTableWithNullValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesForDataTableWithNullValues(10, true);
            var tableName = "EnumTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            connection.InsertAll(tableName, entities);

            // Act
            var result = connection.BinaryBulkUpdate(
                tableName,
                table);

            // Assert
            Assert.AreEqual(entities.Count, result);
        }
    }

    #endregion

    #endregion

    #endregion

    #region Async

    #region TEntity

    #region BinaryBulkInsertAsync

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncForEnum()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateEnumTables(10, false);
            var tableName = "EnumTable";

            // Act
            var result = await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncForEnumWithNullValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesWithNullValues(10, false);
            var tableName = "EnumTable";

            // Act
            var result = await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    #endregion

    #region BinaryBulkDeleteAsync

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncForEnum()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateEnumTables(10, false);
            var tableName = "EnumTable";

            // Act
            connection.InsertAll(entities);

            // Act
            var result = await connection.BinaryBulkDeleteAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll<EnumTable>();
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncForEnumWithNullValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesWithNullValues(10, false);
            var tableName = "EnumTable";

            // Act
            connection.InsertAll(entities);

            // Act
            var result = await connection.BinaryBulkDeleteAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll<EnumTable>();
            Assert.AreEqual(0, countResult);
        }
    }

    #endregion

    #region BinaryBulkMergeAsync

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncForEnum()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateEnumTables(10, false);
            var tableName = "EnumTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncForEnumWithNullValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesWithNullValues(10, false);
            var tableName = "EnumTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    #endregion

    #region BinaryBulkUpdateAsync

    [TestMethod]
    public async Task TestBinaryBulkUpdateAsyncForEnum()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateEnumTables(10, false);
            var tableName = "EnumTable";

            // Act
            connection.InsertAll(entities);

            // Act
            var result = await connection.BinaryBulkUpdateAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkUpdateAsyncForEnumWithNullValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesWithNullValues(10, false);
            var tableName = "EnumTable";

            // Act
            connection.InsertAll(entities);

            // Act
            var result = await connection.BinaryBulkUpdateAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    #endregion

    #endregion

    #region Anonymous

    #region BinaryBulkInsertAsync

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncForEnumForAnonymous()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateEnumTableAnonymousTables(10, false);
            var tableName = "EnumTable";

            // Act
            var result = await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncForEnumForAnonymousWithNullValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesForAnonymousWithNullValues(10, false);
            var tableName = "EnumTable";

            // Act
            var result = await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    #endregion

    #region BinaryBulkDeleteAsync

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncForEnumForAnonymous()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateEnumTableAnonymousTables(10, true);
            var tableName = "EnumTable";

            // Act
            connection.InsertAll(tableName, entities);

            // Act
            var result = await connection.BinaryBulkDeleteAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll<EnumTable>();
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncForEnumForAnonymousWithNullValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesForAnonymousWithNullValues(10, true);
            var tableName = "EnumTable";

            // Act
            connection.InsertAll(tableName, entities);

            // Act
            var result = await connection.BinaryBulkDeleteAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll<EnumTable>();
            Assert.AreEqual(0, countResult);
        }
    }

    #endregion

    #region BinaryBulkMergeAsync

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncForEnumForAnonymous()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateEnumTableAnonymousTables(10, false);
            var tableName = "EnumTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncForEnumForAnonymousWithNullValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesForAnonymousWithNullValues(10, false);
            var tableName = "EnumTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    #endregion

    #region BinaryBulkUpdateAsync

    [TestMethod]
    public async Task TestBinaryBulkUpdateAsyncForEnumForAnonymous()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateEnumTableAnonymousTables(10, true);
            var tableName = "EnumTable";

            // Act
            connection.InsertAll(tableName, entities);

            // Act
            var result = await connection.BinaryBulkUpdateAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkUpdateAsyncForEnumForAnonymousWithNullValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesForAnonymousWithNullValues(10, true);
            var tableName = "EnumTable";

            // Act
            connection.InsertAll(tableName, entities);

            // Act
            var result = await connection.BinaryBulkUpdateAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    #endregion

    #endregion

    #region IDictionary<string, object>

    #region BinaryBulkInsertAsync

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncForEnumForExpandoObject()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateEnumTableExpandoObjectTables(10, false);
            var tableName = "EnumTable";

            // Act
            var result = await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncForEnumForExpandoObjectWithNullValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesForExpandoObjectWithNullValues(10, false);
            var tableName = "EnumTable";

            // Act
            var result = await connection.BinaryBulkInsertAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    #endregion

    #region BinaryBulkDeleteAsync

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncForEnumForExpandoObject()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateEnumTableExpandoObjectTables(10, true);
            var tableName = "EnumTable";

            // Act
            connection.InsertAll(tableName, entities);

            // Act
            var result = await connection.BinaryBulkDeleteAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll<EnumTable>();
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncForEnumForExpandoObjectWithNullValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesForExpandoObjectWithNullValues(10, true);
            var tableName = "EnumTable";

            // Act
            connection.InsertAll(tableName, entities);

            // Act
            var result = await connection.BinaryBulkDeleteAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll<EnumTable>();
            Assert.AreEqual(0, countResult);
        }
    }

    #endregion

    #region BinaryBulkMergeAsync

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncForEnumForExpandoObject()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateEnumTableExpandoObjectTables(10, false);
            var tableName = "EnumTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncForEnumForExpandoObjectWithNullValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesForExpandoObjectWithNullValues(10, false);
            var tableName = "EnumTable";

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    #endregion

    #region BinaryBulkUpdateAsync

    [TestMethod]
    public async Task TestBinaryBulkUpdateAsyncForEnumForExpandoObject()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = Helper.CreateEnumTableExpandoObjectTables(10, true);
            var tableName = "EnumTable";

            // Act
            connection.InsertAll(tableName, entities);

            // Act
            var result = await connection.BinaryBulkUpdateAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkUpdateAsyncForEnumForExpandoObjectWithNullValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesForExpandoObjectWithNullValues(10, true);
            var tableName = "EnumTable";

            // Act
            connection.InsertAll(tableName, entities);

            // Act
            var result = await connection.BinaryBulkUpdateAsync(
                tableName,
                entities: entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    #endregion

    #endregion

    #region DataTable

    #region BinaryBulkInsertAsync

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncForEnumForDataTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesForDataTable(10, false);
            var tableName = "EnumTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = await connection.BinaryBulkInsertAsync(
                tableName,
                table);

            // Assert
            Assert.AreEqual(entities.Count, result);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkInsertAsyncForEnumForDataTableWithNullValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesForDataTableWithNullValues(10, false);
            var tableName = "EnumTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = await connection.BinaryBulkInsertAsync(
                tableName,
                table);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
    }

    #endregion

    #region BinaryBulkDeleteAsync

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncForEnumForDataTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesForDataTable(10, true);
            var tableName = "EnumTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            connection.InsertAll(tableName, entities);

            // Act
            var result = await connection.BinaryBulkDeleteAsync(
                tableName,
                table);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll<EnumTable>();
            Assert.AreEqual(0, countResult);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkDeleteAsyncForEnumForDataTableWithNullValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesForDataTableWithNullValues(10, true);
            var tableName = "EnumTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            connection.InsertAll(tableName, entities);

            // Act
            var result = await connection.BinaryBulkDeleteAsync(
                tableName,
                table);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll<EnumTable>();
            Assert.AreEqual(0, countResult);
        }
    }

    #endregion

    #region BinaryBulkMergeAsync

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncForEnumForDataTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesForDataTable(10, false);
            var tableName = "EnumTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                table);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll<EnumTable>();
            Assert.AreEqual(entities.Count, result);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkMergeAsyncForEnumForDataTableWithNullValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesForDataTableWithNullValues(10, false);
            var tableName = "EnumTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            var result = await connection.BinaryBulkMergeAsync(
                tableName,
                table);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll<EnumTable>();
            Assert.AreEqual(entities.Count, result);
        }
    }

    #endregion

    #region BinaryBulkUpdateAsync

    [TestMethod]
    public async Task TestBinaryBulkUpdateAsyncForEnumForDataTable()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesForDataTable(10, true);
            var tableName = "EnumTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            connection.InsertAll(tableName, entities);

            // Act
            var result = await connection.BinaryBulkUpdateAsync(
                tableName,
                table);

            // Assert
            Assert.AreEqual(entities.Count, result);
        }
    }

    [TestMethod]
    public async Task TestBinaryBulkUpdateAsyncForEnumForDataTableWithNullValues()
    {
        using (var connection = GetConnection())
        {
            // Prepare
            var entities = CreateEnumTablesForDataTableWithNullValues(10, true);
            var tableName = "EnumTable";
            var table = Helper.ToDataTable(tableName, entities);

            // Act
            connection.InsertAll(tableName, entities);

            // Act
            var result = await connection.BinaryBulkUpdateAsync(
                tableName,
                table);

            // Assert
            Assert.AreEqual(entities.Count, result);
        }
    }

    #endregion

    #endregion

    #endregion
}
