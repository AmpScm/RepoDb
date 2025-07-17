using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.SqlServer.BulkOperations.IntegrationTests.Models;

namespace RepoDb.SqlServer.BulkOperations.IntegrationTests.Operations;

[TestClass]
public class MicrosoftSqlConnectionBulkInsertOperationsTest
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

    #region BulkInsert<TEntity>

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkInsertForEntities()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = connection.BulkInsert(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
        });
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkInsertForEntitiesWithReturnIdentity()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = connection.BulkInsert(tables, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);
        Assert.IsFalse(tables.Any(e => e.Id <= 0));

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            var item = queryResult.FirstOrDefault(e => e.Id == t.Id);
            Helper.AssertPropertiesEquality(t, item);
        });
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkInsertForEntitiesWithReturnIdentityAndWithHints()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = connection.BulkInsert(tables, hints: SqlServerTableHints.TabLock, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);
        Assert.IsFalse(tables.Any(e => e.Id <= 0));

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            var item = queryResult.FirstOrDefault(e => e.Id == t.Id);
            Helper.AssertPropertiesEquality(t, item);
        });
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkInsertForEntitiesWithReturnIdentityViaPhysicalPseudoTempTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = connection.BulkInsert(tables, isReturnIdentity: true, usePhysicalPseudoTempTable: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);
        Assert.IsFalse(tables.Any(e => e.Id <= 0));

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            var item = queryResult.FirstOrDefault(e => e.Id == t.Id);
            Helper.AssertPropertiesEquality(t, item);
        });
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkInsertForEntitiesWithMappings()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add the mappings
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar))
        };

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = connection.BulkInsert(tables, mappings: mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
        });
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkInsertForMappedEntities()
    {
        // Setup
        var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = connection.BulkInsert(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
        });
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkInsertForMappedEntitiesWithReturnIdentity()
    {
        // Setup
        var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = connection.BulkInsert(tables, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);
        Assert.IsFalse(tables.Any(e => e.IdMapped <= 0));

        // Act
        var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            var item = queryResult.FirstOrDefault(e => e.IdMapped == t.IdMapped);
            Helper.AssertPropertiesEquality(t, item);
        });
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkInsertForMappedEntitiesWithReturnIdentityAndWithHints()
    {
        // Setup
        var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = connection.BulkInsert(tables, hints: SqlServerTableHints.TabLock, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);
        Assert.IsFalse(tables.Any(e => e.IdMapped <= 0));

        // Act
        var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            var item = queryResult.FirstOrDefault(e => e.IdMapped == t.IdMapped);
            Helper.AssertPropertiesEquality(t, item);
        });
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkInsertForMappedEntitiesWithReturnIdentityViaPhysicalPseudoTempTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = connection.BulkInsert(tables, isReturnIdentity: true, usePhysicalPseudoTempTable: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);
        Assert.IsFalse(tables.Any(e => e.Id <= 0));

        // Act
        var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            var item = queryResult.FirstOrDefault(e => e.IdMapped == t.Id);
            Helper.AssertPropertiesEquality(t, item);
        });
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkInsertForMappedEntitiesWithMappings()
    {
        // Setup
        var tables = Helper.CreateBulkOperationMappedIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add the mappings
            new BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.RowGuidMapped), nameof(BulkOperationIdentityTable.RowGuid)),
            new BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnBitMapped), nameof(BulkOperationIdentityTable.ColumnBit)),
            new BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDateTimeMapped), nameof(BulkOperationIdentityTable.ColumnDateTime)),
            new BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDateTime2Mapped), nameof(BulkOperationIdentityTable.ColumnDateTime2)),
            new BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDecimalMapped), nameof(BulkOperationIdentityTable.ColumnDecimal)),
            new BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnFloatMapped), nameof(BulkOperationIdentityTable.ColumnFloat)),
            new BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnIntMapped), nameof(BulkOperationIdentityTable.ColumnInt)),
            new BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnNVarCharMapped), nameof(BulkOperationIdentityTable.ColumnNVarChar))
        };

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = connection.BulkInsert(tables, mappings: mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
        });
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkInsertForEntitiesIfTheMappingsAreInvalid()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add invalid mappings
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)),

            // Switched
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt))
        };

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        Assert.ThrowsExactly<InvalidOperationException>(() => connection.BulkInsert(tables, mappings));
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkInsertForEntitiesDbDataReader()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = destinationConnection.BulkInsert<BulkOperationIdentityTable>((DbDataReader)reader);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count * 2, queryResult.Count());
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkInsertForEntitiesDbDataReaderWithMappings()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add the mappings
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar))
        };

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = destinationConnection.BulkInsert<BulkOperationIdentityTable>((DbDataReader)reader, mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count * 2, queryResult.Count());
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkInsertForEntitiesDbDataReaderIfTheMappingsAreInvalid()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add invalid mappings
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)),

            // Switched
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt))
        };

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        Assert.ThrowsExactly<InvalidOperationException>(() => destinationConnection.BulkInsert<BulkOperationIdentityTable>((DbDataReader)reader, mappings));
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkInsertForEntitiesDataTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        using var table = new DataTable();
        table.Load(reader);

        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = destinationConnection.BulkInsert<BulkOperationIdentityTable>(table);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count * 2, queryResult.Count());
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkInsertForEntitiesDataTableWithReturnIdentity()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        using var table = new DataTable();
        table.Load(reader);

        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = destinationConnection.BulkInsert<BulkOperationIdentityTable>(table, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count * 2, queryResult.Count());

        // Assert
        var rows = table.Rows.OfType<DataRow>();
        queryResult.AsList().ForEach(item =>
        {
            var row = rows.Where(r => Equals(item.Id, r["Id"]));
            Assert.IsNotNull(row);
        });
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkInsertForEntitiesDataTableWithReturnIdentityAndWithHints()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        using var table = new DataTable();
        table.Load(reader);

        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = destinationConnection.BulkInsert<BulkOperationIdentityTable>(table, hints: SqlServerTableHints.TabLock, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count * 2, queryResult.Count());

        // Assert
        var rows = table.Rows.OfType<DataRow>();
        queryResult.AsList().ForEach(item =>
        {
            var row = rows.Where(r => Equals(item.Id, r["Id"]));
            Assert.IsNotNull(row);
        });
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkInsertForEntitiesDataTableWithReturnIdentityViaPhysicalPseudoTempTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        using var table = new DataTable();
        table.Load(reader);

        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = destinationConnection.BulkInsert<BulkOperationIdentityTable>(table, isReturnIdentity: true, usePhysicalPseudoTempTable: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count * 2, queryResult.Count());

        // Assert
        var rows = table.Rows.OfType<DataRow>();
        queryResult.AsList().ForEach(item =>
        {
            var row = rows.Where(r => Equals(item.Id, r["Id"]));
            Assert.IsNotNull(row);
        });
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkInsertForEntitiesDataTableWithMappings()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add the mappings
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar))
        };

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        using var table = new DataTable();
        table.Load(reader);

        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = destinationConnection.BulkInsert<BulkOperationIdentityTable>(table, DataRowState.Unchanged, mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count * 2, queryResult.Count());
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkInsertForEntitiesDataTableIfTheMappingsAreInvalid()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add invalid mappings
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)),

            // Switched
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt))
        };

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        using var table = new DataTable();
        table.Load(reader);

        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        Assert.ThrowsExactly<InvalidOperationException>(() => destinationConnection.BulkInsert<BulkOperationIdentityTable>(table, DataRowState.Unchanged, mappings));
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkInsertForNullEntities()
    {
        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        Assert.ThrowsExactly<ArgumentNullException>(() => connection.BulkInsert((IEnumerable<BulkOperationIdentityTable>)null));
    }

    //[TestMethod, ExpectedException(typeof(EmptyException))]
    //public void ThrowExceptionOnMicrosoftSqlConnectionBulkInsertForEmptyEntities()
    //{
    //    using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
    //    {
    //        connection.BulkInsert(Enumerable.Empty<BulkOperationIdentityTable>());
    //    }
    //}

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkInsertForNullDataReader()
    {
        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        Assert.ThrowsExactly<ArgumentNullException>(() => connection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            (DbDataReader)null));
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkInsertForNullDataTable()
    {
        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        Assert.ThrowsExactly<ArgumentNullException>(() => connection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            (DataTable)null));
    }

    #endregion

    #region BulkInsert<TEntity>(Extra Fields)

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkInsertForEntitiesWithExtraFields()
    {
        // Setup
        var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = connection.BulkInsert(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
        });
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkInsertForEntitiesWithExtraFieldsWithMappings()
    {
        // Setup
        var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add the mappings
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar))
        };

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = connection.BulkInsert(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
        });
    }

    #endregion

    #region BulkInsert(TableName)

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkInsertForTableNameExpandoObjects()
    {
        // Setup
        var tables = Helper.CreateBulkOperationExpandoObjectIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = connection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            Helper.AssertMembersEquality(queryResult.ElementAt(tables.IndexOf(t)), t);
        });
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkInsertForTableNameAnonymousObjects()
    {
        // Setup
        var tables = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = connection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            Helper.AssertPropertiesEquality(queryResult.ElementAt((int)tables.IndexOf(t)), t);
        });
    }

    [TestMethod]
    public void TestSystemSqlConnectionBulkInsertForTableNameDataEntities()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = connection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
        });
    }

    [TestMethod]
    public void TestSystemSqlConnectionBulkInsertForTableNameExpandoObjectsWithReturnIdentity()
    {
        // Setup
        var tables = Helper.CreateBulkOperationExpandoObjectIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = connection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);
        Assert.IsTrue(tables.All(e => ((dynamic)e).Id > 0));

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            Helper.AssertMembersEquality(queryResult.ElementAt(tables.IndexOf(t)), t);
        });
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkInsertForTableNameDataEntitiesWithReturnIdentity()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = connection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);
        Assert.IsFalse(tables.Any(e => e.Id <= 0));

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            var item = queryResult.FirstOrDefault(e => e.Id == t.Id);
            Helper.AssertPropertiesEquality(t, item);
        });
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkInsertForTableNameDataEntitiesWithReturnIdentityAndWithHints()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = connection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, hints: SqlServerTableHints.TabLock, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);
        Assert.IsFalse(tables.Any(e => e.Id <= 0));

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            var item = queryResult.FirstOrDefault(e => e.Id == t.Id);
            Helper.AssertPropertiesEquality(t, item);
        });
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkInsertForTableNameDataEntitiesWithReturnIdentityViaPhysicalPseudoTempTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = connection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, isReturnIdentity: true, usePhysicalPseudoTempTable: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);
        Assert.IsFalse(tables.Any(e => e.Id <= 0));

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            var item = queryResult.FirstOrDefault(e => e.Id == t.Id);
            Helper.AssertPropertiesEquality(t, item);
        });
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkInsertForTableNameDbDataReader()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), (DbDataReader)reader);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count * 2, queryResult.Count());
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkInsertForTableNameDbDataReaderWithMappings()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add the mappings
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar))
        };

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), (DbDataReader)reader, mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count * 2, queryResult.Count());
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkInsertForTableNameDbDataReaderIfTheMappingsAreInvalid()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add invalid mappings
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)),

            // Switched
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt))
        };

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        Assert.ThrowsExactly<InvalidOperationException>(() => destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), (DbDataReader)reader, mappings));
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkInsertForTableNameDbDataReaderIfTheTableNameIsNotValid()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        Assert.ThrowsExactly<MissingFieldsException>(() => destinationConnection.BulkInsert("InvalidTable", (DbDataReader)reader));
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkInsertForTableNameDbDataReaderIfTheTableNameIsMissing()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        Assert.ThrowsExactly<MissingFieldsException>(() => destinationConnection.BulkInsert("MissingTable", (DbDataReader)reader));
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkInsertForTableNameDbDataTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        using var table = new DataTable();
        table.Load(reader);

        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count * 2, queryResult.Count());
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkInsertForTableNameDbDataTableWithReturnIdentity()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        using var table = new DataTable();
        table.Load(reader);

        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count * 2, queryResult.Count());

        // Assert
        var rows = table.Rows.OfType<DataRow>();
        queryResult.AsList().ForEach(item =>
        {
            var row = rows.Where(r => Equals(item.Id, r["Id"]));
            Assert.IsNotNull(row);
        });
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkInsertForTableNameDbDataTableWithReturnIdentityAndWithHints()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        using var table = new DataTable();
        table.Load(reader);

        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, hints: SqlServerTableHints.TabLock, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count * 2, queryResult.Count());

        // Assert
        var rows = table.Rows.OfType<DataRow>();
        queryResult.AsList().ForEach(item =>
        {
            var row = rows.Where(r => Equals(item.Id, r["Id"]));
            Assert.IsNotNull(row);
        });
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkInsertForTableNameDbDataTableWithReturnIdentityViaPhysicalPseudoTempTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        using var table = new DataTable();
        table.Load(reader);

        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, isReturnIdentity: true, usePhysicalPseudoTempTable: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count * 2, queryResult.Count());

        // Assert
        var rows = table.Rows.OfType<DataRow>();
        queryResult.AsList().ForEach(item =>
        {
            var row = rows.Where(r => Equals(item.Id, r["Id"]));
            Assert.IsNotNull(row);
        });
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkInsertForTableNameDbDataTableWithMappings()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add the mappings
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar))
        };

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        using var table = new DataTable();
        table.Load(reader);

        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, DataRowState.Unchanged, mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count * 2, queryResult.Count());
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkInsertForTableNameDbDataTableIfTheMappingsAreInvalid()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add invalid mappings
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)),

            // Switched
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt))
        };

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        using var table = new DataTable();
        table.Load(reader);

        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        Assert.ThrowsExactly<InvalidOperationException>(() => destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, DataRowState.Unchanged, mappings));
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkInsertForTableNameDbDataTableIfTheTableNameIsNotValid()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        using var table = new DataTable();
        table.Load(reader);

        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        Assert.ThrowsExactly<MissingFieldsException>(() => destinationConnection.BulkInsert("InvalidTable", table, DataRowState.Unchanged));
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkInsertForTableNameDbDataTableIfTheTableNameIsMissing()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        using var table = new DataTable();
        table.Load(reader);

        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        Assert.ThrowsExactly<MissingFieldsException>(() => destinationConnection.BulkInsert("MissingTable", table, DataRowState.Unchanged));
    }

    #endregion

    #region BulkInsertAsync<TEntity>

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkInsertAsyncForEntities()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = await connection.BulkInsertAsync(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
        });
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkInsertAsyncForEntitiesWithReturnIdentity()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = await connection.BulkInsertAsync(tables, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);
        Assert.IsFalse(tables.Any(e => e.Id <= 0));

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            var item = queryResult.FirstOrDefault(e => e.Id == t.Id);
            Helper.AssertPropertiesEquality(t, item);
        });
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkInsertAsyncForEntitiesWithReturnIdentityWithHints()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = await connection.BulkInsertAsync(tables, hints: SqlServerTableHints.TabLock, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);
        Assert.IsFalse(tables.Any(e => e.Id <= 0));

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            var item = queryResult.FirstOrDefault(e => e.Id == t.Id);
            Helper.AssertPropertiesEquality(t, item);
        });
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkInsertAsyncForEntitiesWithReturnIdentityViaPhysicalPseudoTempTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = await connection.BulkInsertAsync(tables, isReturnIdentity: true, usePhysicalPseudoTempTable: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);
        Assert.IsFalse(tables.Any(e => e.Id <= 0));

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            var item = queryResult.FirstOrDefault(e => e.Id == t.Id);
            Helper.AssertPropertiesEquality(t, item);
        });
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkInsertAsyncForEntitiesWithMappings()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add the mappings
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar))
        };

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = await connection.BulkInsertAsync(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
        });
    }

    [TestMethod]
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkInsertAsyncForEntitiesIfTheMappingsAreInvalid()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add invalid mappings
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)),

            // Switched
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt))
        };

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(async () => await connection.BulkInsertAsync(tables, mappings));
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkInsertAsyncForEntitiesDbDataReader()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = await destinationConnection.BulkInsertAsync<BulkOperationIdentityTable>((DbDataReader)reader);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count * 2, queryResult.Count());
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkInsertAsyncForEntitiesDbDataReaderWithMappings()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add the mappings
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar))
        };

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = await destinationConnection.BulkInsertAsync<BulkOperationIdentityTable>((DbDataReader)reader, mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count * 2, queryResult.Count());
    }

    [TestMethod]
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkInsertAsyncForEntitiesDbDataReaderIfTheMappingsAreInvalid()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add invalid mappings
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)),

            // Switched
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt))
        };

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(async () => await destinationConnection.BulkInsertAsync<BulkOperationIdentityTable>((DbDataReader)reader, mappings));
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkInsertAsyncForEntitiesDataTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        using var table = new DataTable();
        table.Load(reader);

        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = await destinationConnection.BulkInsertAsync<BulkOperationIdentityTable>(table);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count * 2, queryResult.Count());
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkInsertAsyncForEntitiesDataTableWithReturnIdentity()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        using var table = new DataTable();
        table.Load(reader);

        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = await destinationConnection.BulkInsertAsync<BulkOperationIdentityTable>(table, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count * 2, queryResult.Count());

        // Assert
        var rows = table.Rows.OfType<DataRow>();
        queryResult.AsList().ForEach(item =>
        {
            var row = rows.Where(r => Equals(item.Id, r["Id"]));
            Assert.IsNotNull(row);
        });
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkInsertAsyncForEntitiesDataTableWithReturnIdentityAndWithHints()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        using var table = new DataTable();
        table.Load(reader);

        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = await destinationConnection.BulkInsertAsync<BulkOperationIdentityTable>(table, hints: SqlServerTableHints.TabLock, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count * 2, queryResult.Count());

        // Assert
        var rows = table.Rows.OfType<DataRow>();
        queryResult.AsList().ForEach(item =>
        {
            var row = rows.Where(r => Equals(item.Id, r["Id"]));
            Assert.IsNotNull(row);
        });
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkInsertAsyncForEntitiesDataTableWithReturnIdentityViaPhysicalPseudoTempTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        using var table = new DataTable();
        table.Load(reader);

        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = await destinationConnection.BulkInsertAsync<BulkOperationIdentityTable>(table, isReturnIdentity: true, usePhysicalPseudoTempTable: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count * 2, queryResult.Count());

        // Assert
        var rows = table.Rows.OfType<DataRow>();
        queryResult.AsList().ForEach(item =>
        {
            var row = rows.Where(r => Equals(item.Id, r["Id"]));
            Assert.IsNotNull(row);
        });
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkInsertAsyncForEntitiesDataTableWithMappings()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add the mappings
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar))
        };

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        using var table = new DataTable();
        table.Load(reader);

        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = await destinationConnection.BulkInsertAsync<BulkOperationIdentityTable>(table, DataRowState.Unchanged, mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count * 2, queryResult.Count());
    }

    [TestMethod]
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkInsertAsyncForEntitiesDataTableIfTheMappingsAreInvalid()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add invalid mappings
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)),

            // Switched
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt))
        };

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        using var table = new DataTable();
        table.Load(reader);

        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(async () => await destinationConnection.BulkInsertAsync<BulkOperationIdentityTable>(table, DataRowState.Unchanged, mappings));
    }

    //[TestMethod, ExpectedException(typeof(AggregateException))]
    //public void ThrowExceptionOnMicrosoftSqlConnectionBulkInsertAsyncForNullEntities()
    //{
    //    using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
    //    {
    //        connection.BulkInsertAsync((IEnumerable<BulkOperationIdentityTable>)null).Wait();
    //    }
    //}

    //[TestMethod, ExpectedException(typeof(AggregateException))]
    //public void ThrowExceptionOnMicrosoftSqlConnectionBulkInsertAsyncForEmptyEntities()
    //{
    //    using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
    //    {
    //        connection.BulkInsertAsync(Enumerable.Empty<BulkOperationIdentityTable>()).Wait();
    //    }
    //}

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkInsertAsyncForNullDataReader()
    {
        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        Assert.ThrowsExactly<AggregateException>(() => connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            (DbDataReader)null).Wait());
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkInsertAsyncForNullDataTable()
    {
        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        Assert.ThrowsExactly<AggregateException>(() => connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            (DataTable)null).Wait());
    }

    #endregion

    #region BulkInsertAsync<TEntity>(Extra Fields)

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkInsertAsyncForEntitiesWithExtraFields()
    {
        // Setup
        var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = await connection.BulkInsertAsync(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
        });
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkInsertAsyncForEntitiesWithExtraFieldsWithMappings()
    {
        // Setup
        var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add the mappings
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar))
        };

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = await connection.BulkInsertAsync(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
        });
    }

    #endregion

    #region BulkInsertAsync(TableName)

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkInsertAsyncForTableNameExpandoObjects()
    {
        // Setup
        var tables = Helper.CreateBulkOperationExpandoObjectIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = await connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            Helper.AssertMembersEquality(queryResult.ElementAt(tables.IndexOf(t)), t);
        });
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkInsertAsyncForTableNameAnonymousObjects()
    {
        // Setup
        var tables = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = await connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            Helper.AssertPropertiesEquality(queryResult.ElementAt((int)tables.IndexOf(t)), t);
        });
    }

    [TestMethod]
    public async Task TestSystemSqlConnectionBulkInsertAsyncForTableNameDataEntities()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = await connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
        });
    }

    [TestMethod]
    public async Task TestSystemSqlConnectionBulkInsertAsyncForTableNameExpandoObjectsWithReturnIdentity()
    {
        // Setup
        var tables = Helper.CreateBulkOperationExpandoObjectIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = await connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);
        Assert.IsTrue(tables.All(e => ((dynamic)e).Id > 0));

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            Helper.AssertMembersEquality(queryResult.ElementAt(tables.IndexOf(t)), t);
        });
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkInsertAsyncForTableNameDataEntitiesWithReturnIdentity()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = await connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);
        Assert.IsFalse(tables.Any(e => e.Id <= 0));

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            var item = queryResult.FirstOrDefault(e => e.Id == t.Id);
            Helper.AssertPropertiesEquality(t, item);
        });
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkInsertAsyncForTableNameDataEntitiesWithReturnIdentityAndWithHints()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = await connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, hints: SqlServerTableHints.TabLock, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);
        Assert.IsFalse(tables.Any(e => e.Id <= 0));

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            var item = queryResult.FirstOrDefault(e => e.Id == t.Id);
            Helper.AssertPropertiesEquality(t, item);
        });
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkInsertAsyncForTableNameDataEntitiesWithReturnIdentityViaPhysicalPseudoTempTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = await connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, isReturnIdentity: true, usePhysicalPseudoTempTable: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);
        Assert.IsFalse(tables.Any(e => e.Id <= 0));

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            var item = queryResult.FirstOrDefault(e => e.Id == t.Id);
            Helper.AssertPropertiesEquality(t, item);
        });
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkInsertAsyncForTableNameDbDataReader()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = await destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), (DbDataReader)reader);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count * 2, queryResult.Count());
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkInsertAsyncForTableNameDbDataReaderWithMappings()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add the mappings
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar))
        };

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = await destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), (DbDataReader)reader, mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count * 2, queryResult.Count());
    }

    [TestMethod]
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkInsertAsyncForTableNameDbDataReaderIfTheMappingsAreInvalid()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add invalid mappings
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)),

            // Switched
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt))
        };

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(async () => await destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), (DbDataReader)reader, mappings));
    }

    [TestMethod]
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkInsertAsyncForTableNameDbDataReaderIfTheTableNameIsNotValid()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        await Assert.ThrowsExactlyAsync<MissingFieldsException>(async () => await destinationConnection.BulkInsertAsync("InvalidTable", (DbDataReader)reader));
    }

    [TestMethod]
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkInsertAsyncForTableNameDbDataReaderIfTheTableNameIsMissing()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        await Assert.ThrowsExactlyAsync<MissingFieldsException>(async () => await destinationConnection.BulkInsertAsync("MissingTable", (DbDataReader)reader));
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkInsertAsyncForTableNameDataTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        using var table = new DataTable();
        table.Load(reader);

        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = await destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count * 2, queryResult.Count());
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkInsertAsyncForTableNameDataTableWithReturnIdentity()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        using var table = new DataTable();
        table.Load(reader);

        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = await destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count * 2, queryResult.Count());

        // Assert
        var rows = table.Rows.OfType<DataRow>();
        queryResult.AsList().ForEach(item =>
        {
            var row = rows.Where(r => Equals(item.Id, r["Id"]));
            Assert.IsNotNull(row);
        });
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkInsertAsyncForTableNameDataTableWithReturnIdentityAndWithHints()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        using var table = new DataTable();
        table.Load(reader);

        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = await destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, hints: SqlServerTableHints.TabLock, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count * 2, queryResult.Count());

        // Assert
        var rows = table.Rows.OfType<DataRow>();
        queryResult.AsList().ForEach(item =>
        {
            var row = rows.Where(r => Equals(item.Id, r["Id"]));
            Assert.IsNotNull(row);
        });
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkInsertAsyncForTableNameDataTableWithReturnIdentityViaPhysicalPseudoTempTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        using var table = new DataTable();
        table.Load(reader);

        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = await destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, isReturnIdentity: true, usePhysicalPseudoTempTable: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count * 2, queryResult.Count());

        // Assert
        var rows = table.Rows.OfType<DataRow>();
        queryResult.AsList().ForEach(item =>
        {
            var row = rows.Where(r => Equals(item.Id, r["Id"]));
            Assert.IsNotNull(row);
        });
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkInsertAsyncForTableNameDataTableWithMappings()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add the mappings
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar))
        };

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        using var table = new DataTable();
        table.Load(reader);

        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkInsertResult = await destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, DataRowState.Unchanged, mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkInsertResult);

        // Act
        var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count * 2, queryResult.Count());
    }

    [TestMethod]
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkInsertAsyncForTableNameDataTableIfTheMappingsAreInvalid()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add invalid mappings
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)),

            // Switched
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt))
        };

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        using var table = new DataTable();
        table.Load(reader);

        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(async () => await destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, DataRowState.Unchanged, mappings));
    }

    [TestMethod]
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkInsertAsyncForTableNameDataTableIfTheTableNameIsNotValid()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        using var table = new DataTable();
        table.Load(reader);

        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        await Assert.ThrowsExactlyAsync<MissingFieldsException>(async () => await destinationConnection.BulkInsertAsync("InvalidTable", table));
    }

    [TestMethod]
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkInsertAsyncForTableNameDataTableIfTheTableNameIsMissing()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        // Insert the records first
        using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        {
            connection.InsertAll(tables);
        }

        // Open the source connection
        using var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Read the data from source connection
        using var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];");
        using var table = new DataTable();
        table.Load(reader);

        // Open the destination connection
        using var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        await Assert.ThrowsExactlyAsync<MissingFieldsException>(async () => await destinationConnection.BulkInsertAsync("MissingTable", table, DataRowState.Unchanged));
    }

    #endregion
}
