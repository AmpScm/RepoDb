using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.SqlServer.BulkOperations.IntegrationTests.Models;

namespace RepoDb.SqlServer.BulkOperations.IntegrationTests.Operations;

[TestClass]
public class MicrosoftSqlConnectionBulkMergeOperationsTest
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

    #region BulkMerge<TEntity>

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkMergeForEntitiesForEmptyTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkMergeResult = connection.BulkMerge(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public void TestMicrosoftSqlConnectionBulkMergeForEntitiesForEmptyTableWithReturnIdentity()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkMergeResult = connection.BulkMerge(tables, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
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
    public void TestMicrosoftSqlConnectionBulkMergeForEntities()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationIdentityTables(tables);

        // Act
        var bulkMergeResult = connection.BulkMerge(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public void TestMicrosoftSqlConnectionBulkMergeForEntitiesWithReturnIdentity()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationIdentityTables(tables);

        // Act
        var bulkMergeResult = connection.BulkMerge(tables, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
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
    public void TestMicrosoftSqlConnectionBulkMergeForEntitiesWithQualifiers()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationIdentityTables(tables);

        // Act
        var bulkMergeResult = connection.BulkMerge(tables,
            qualifiers: e => new { e.RowGuid, e.ColumnInt });

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public void TestMicrosoftSqlConnectionBulkMergeForEntitiesWithUsePhysicalPseudoTempTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationIdentityTables(tables);

        // Act
        var bulkMergeResult = connection.BulkMerge(tables,
            usePhysicalPseudoTempTable: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public void TestMicrosoftSqlConnectionBulkMergeForEntitiesWithMappings()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add the mappings
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)),
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
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationIdentityTables(tables);

        // Act
        var bulkMergeResult = connection.BulkMerge(tables, mappings: mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public void TestMicrosoftSqlConnectionBulkMergeForMappedEntitiesForEmptyTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkMergeResult = connection.BulkMerge(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public void TestMicrosoftSqlConnectionBulkMergeForMappedEntitiesForEmptyTableWithReturnIdentity()
    {
        // Setup
        var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkMergeResult = connection.BulkMerge(tables, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
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
    public void TestMicrosoftSqlConnectionBulkMergeForMappedEntities()
    {
        // Setup
        var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationMappedIdentityTables(tables);

        // Act
        var bulkMergeResult = connection.BulkMerge(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public void TestMicrosoftSqlConnectionBulkMergeForMappedEntitiesWithReturnIdentity()
    {
        // Setup
        var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationMappedIdentityTables(tables);

        // Act
        var bulkMergeResult = connection.BulkMerge(tables, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
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
    public void TestMicrosoftSqlConnectionBulkMergeForMappedEntitiesWithQualifiers()
    {
        // Setup
        var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationMappedIdentityTables(tables);

        // Act
        var bulkMergeResult = connection.BulkMerge(tables,
            qualifiers: e => new { e.RowGuidMapped, e.ColumnIntMapped });

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public void TestMicrosoftSqlConnectionBulkMergeForMappedEntitiesWithUsePhysicalPseudoTempTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationMappedIdentityTables(tables);

        // Act
        var bulkMergeResult = connection.BulkMerge(tables,
            usePhysicalPseudoTempTable: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public void TestMicrosoftSqlConnectionBulkMergeForMappedEntitiesWithMappings()
    {
        // Setup
        var tables = Helper.CreateBulkOperationMappedIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add the mappings
            new BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.IdMapped), nameof(BulkOperationIdentityTable.Id)),
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
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationMappedIdentityTables(tables);

        // Act
        var bulkMergeResult = connection.BulkMerge(tables, mappings: mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkMergeForEntitiesIfTheMappingsAreInvalid()
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
        Assert.ThrowsExactly<InvalidOperationException>(() => connection.BulkMerge(tables, mappings: mappings));
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkMergeForEntitiesDbDataReader()
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
        var bulkMergeResult = destinationConnection.BulkMerge<BulkOperationIdentityTable>((DbDataReader)reader);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkMergeForEntitiesDbDataReaderWithMappings()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add the mappings
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)),
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
        var bulkMergeResult = destinationConnection.BulkMerge<BulkOperationIdentityTable>((DbDataReader)reader,
            mappings: mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkMergeForEntitiesDbDataReaderIfTheMappingsAreInvalid()
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
        Assert.ThrowsExactly<InvalidOperationException>(() => destinationConnection.BulkMerge<BulkOperationIdentityTable>((DbDataReader)reader,
            mappings: mappings));
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkMergeForEntitiesDataTable()
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
        var bulkMergeResult = destinationConnection.BulkMerge<BulkOperationIdentityTable>(table);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkMergeForEntitiesDataTableWithReturnIdentity()
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
        var bulkMergeResult = destinationConnection.BulkMerge<BulkOperationIdentityTable>(table, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

        // Act
        var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        var rows = table.Rows.OfType<DataRow>();
        queryResult.AsList().ForEach(item =>
        {
            var row = rows.Where(r => Equals(item.Id, r["Id"]));
            Assert.IsNotNull(row);
        });
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkMergeForEntitiesDataTableWithMappings()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add the mappings
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)),
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
        var bulkMergeResult = destinationConnection.BulkMerge<BulkOperationIdentityTable>(table,
            mappings: mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkMergeForEntitiesDataTableIfTheMappingsAreInvalid()
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
        Assert.ThrowsExactly<InvalidOperationException>(() => destinationConnection.BulkMerge<BulkOperationIdentityTable>(table,
            mappings: mappings));
    }

    //[TestMethod, ExpectedException(typeof(ArgumentNullException))]
    //public void ThrowExceptionOnMicrosoftSqlConnectionBulkMergeForNullEntities()
    //{
    //    using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
    //    {
    //        connection.BulkMerge((IEnumerable<BulkOperationIdentityTable>)null);
    //    }
    //}

    //[TestMethod, ExpectedException(typeof(EmptyException))]
    //public void ThrowExceptionOnMicrosoftSqlConnectionBulkMergeForEmptyEntities()
    //{
    //    using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
    //    {
    //        connection.BulkMerge(Enumerable.Empty<BulkOperationIdentityTable>());
    //    }
    //}

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkMergeForNullDataReader()
    {
        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        Assert.ThrowsExactly<ArgumentNullException>(() => connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            (DbDataReader)null));
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkMergeForNullDataTable()
    {
        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        Assert.ThrowsExactly<ArgumentNullException>(() => connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            (DataTable)null));
    }

    #endregion

    #region BulkMerge<TEntity>(Extra Fields)

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkMergeForEntitiesWithExtraFields()
    {
        // Setup
        var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Setup
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateWithExtraFieldsBulkOperationIdentityTables(tables);

        // Act
        var bulkMergeResult = connection.BulkMerge(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public void TestMicrosoftSqlConnectionBulkMergeForEntitiesWithExtraFieldsWithMappings()
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
        // Setup
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateWithExtraFieldsBulkOperationIdentityTables(tables);

        // Act
        var bulkMergeResult = connection.BulkMerge(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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

    #region BulkMerge(TableName)

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkMergeForTableNameExpandoObjectsForEmptyTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public void TestMicrosoftSqlConnectionBulkMergeForTableNameExpandoObjectsForEmptyTableWithReturnIdentity()
    {
        // Setup
        var tables = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
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
    public void TestMicrosoftSqlConnectionBulkMergeForTableNameExpandoObjectsForNonEmptyTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll<BulkOperationIdentityTable>(tables);

        // Setup
        var entities = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

        // Act
        var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), entities);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(entities.Count, queryResult.Count());
        entities.AsList().ForEach(t =>
        {
            Helper.AssertMembersEquality(queryResult.ElementAt(entities.IndexOf(t)), t);
        });
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkMergeForTableNameExpandoObjectsForNonEmptyTableWithReturnIdentity()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll<BulkOperationIdentityTable>(tables);

        // Setup
        var entities = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

        // Act
        var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
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
    public void TestMicrosoftSqlConnectionBulkMergeForTableNameAnonymousObjectsForEmptyTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            Helper.AssertMembersEquality(queryResult.ElementAt((int)tables.IndexOf(t)), t);
        });
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkMergeForTableNameAnonymousObjectsForEmptyTableWithReturnIdentity()
    {
        // Setup
        var tables = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
        Assert.IsTrue(tables.All(e => ((dynamic)e).Id > 0));

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            Helper.AssertMembersEquality(queryResult.ElementAt((int)tables.IndexOf(t)), t);
        });
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkMergeForTableNameAnonymousObjectsForNonEmptyTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll<BulkOperationIdentityTable>(tables);

        // Setup
        var entities = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

        // Act
        var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), entities);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(entities.Count, queryResult.Count());
        entities.AsList().ForEach(t =>
        {
            Helper.AssertMembersEquality(queryResult.ElementAt((int)entities.IndexOf(t)), t);
        });
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkMergeForTableNameAnonymousObjectsForNonEmptyTableWithReturnIdentity()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll<BulkOperationIdentityTable>(tables);

        // Setup
        var entities = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

        // Act
        var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
        Assert.IsTrue(tables.All(e => ((dynamic)e).Id > 0));

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            Helper.AssertMembersEquality(queryResult.ElementAt((int)tables.IndexOf(t)), t);
        });
    }

    [TestMethod]
    public void TestSystemSqlConnectionBulkMergeForTableNameDataEntitiesForEmptyTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public void TestMicrosoftSqlConnectionBulkMergeForTableNameDataEntitiesForEmptyTableWithReturnIdentity()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
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
    public void TestMicrosoftSqlConnectionBulkMergeForTableNameDataEntitiesForNonEmptyTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll<BulkOperationIdentityTable>(tables);

        // Setup
        Helper.UpdateBulkOperationIdentityTables(tables);

        // Act
        var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public void TestMicrosoftSqlConnectionBulkMergeForTableNameDataEntitiesForNonEmptyTableWithReturnIdentity()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll<BulkOperationIdentityTable>(tables);

        // Setup
        Helper.UpdateBulkOperationIdentityTables(tables);

        // Act
        var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
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
    public void TestMicrosoftSqlConnectionBulkMergeForTableNameDataEntities()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Setup
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationIdentityTables(tables);

        // Act
        var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public void TestMicrosoftSqlConnectionBulkMergeForTableNameDataEntitiesWithReturnIdentity()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Setup
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationIdentityTables(tables);

        // Act
        var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, isReturnIdentity: true);
        Assert.IsFalse(tables.Any(e => e.Id <= 0));

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public void TestMicrosoftSqlConnectionBulkMergeForTableNameDataEntitiesWithQualifiers()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Setup
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationIdentityTables(tables);

        // Act
        var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            tables,
            qualifiers: e => new { e.RowGuid, e.ColumnInt });

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public void TestMicrosoftSqlConnectionBulkMergeForTableNameDataEntitiesWithUsePhysicalPseudoTempTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Setup
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationIdentityTables(tables);

        // Act
        var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            tables,
            usePhysicalPseudoTempTable: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public void TestMicrosoftSqlConnectionBulkMergeForTableNameDbDataReader()
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
        var bulkMergeResult = destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            (DbDataReader)reader);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkMergeForTableNameDbDataReaderWithMappings()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add the mappings
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)),
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
        var bulkMergeResult = destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            (DbDataReader)reader,
            mappings: mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkMergeForTableNameDbDataReaderIfTheMappingsAreInvalid()
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
        var bulkMergeResult = destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            (DbDataReader)reader,
            mappings: mappings);

        // Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => Assert.AreEqual(tables.Count, bulkMergeResult));
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkMergeForTableNameDbDataReaderIfTheTableNameIsNotValid()
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
        Assert.ThrowsExactly<MissingFieldsException>(() => destinationConnection.BulkMerge("InvalidTable", (DbDataReader)reader));
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkMergeForTableNameDbDataReaderIfTheTableNameIsMissing()
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
        Assert.ThrowsExactly<MissingFieldsException>(() => destinationConnection.BulkMerge("MissingTable", (DbDataReader)reader));
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkMergeForTableNameDbDataTable()
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
        var bulkMergeResult = destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkMergeForTableNameDataTableWithReturnIdentity()
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
        var bulkMergeResult = destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

        // Act
        var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        var rows = table.Rows.OfType<DataRow>();
        queryResult.AsList().ForEach(item =>
        {
            var row = rows.Where(r => Equals(item.Id, r["Id"]));
            Assert.IsNotNull(row);
        });
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkMergeForTableNameDbDataTableWithMappings()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add the mappings
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)),
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
        var bulkMergeResult = destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            table,
            mappings: mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkMergeForTableNameDbDataTableIfTheMappingsAreInvalid()
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
        var bulkMergeResult = destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            table,
            mappings: mappings);

        // Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => Assert.AreEqual(tables.Count, bulkMergeResult));
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkMergeForTableNameDbDataTableIfTheTableNameIsNotValid()
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
        Assert.ThrowsExactly<MissingFieldsException>(() => destinationConnection.BulkMerge("InvalidTable",
            table));
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkMergeForTableNameDbDataTableIfTheTableNameIsMissing()
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
        Assert.ThrowsExactly<MissingFieldsException>(() => destinationConnection.BulkMerge("MissingTable",
            table));
    }

    #endregion

    #region BulkMergeAsync<TEntity>

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForEntitiesForEmptyTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkMergeResult = await connection.BulkMergeAsync(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForEntitiesForEmptyTableWithReturnIdentity()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkMergeResult = await connection.BulkMergeAsync(tables, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
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
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForEntities()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Setup
        connection.InsertAll(tables);

        // Act
        var bulkMergeResult = await connection.BulkMergeAsync(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForEntitiesWithReturnIdentity()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Setup
        connection.InsertAll(tables);

        // Act
        var bulkMergeResult = await connection.BulkMergeAsync(tables, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
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
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForEntitiesWithQualifiers()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationIdentityTables(tables);

        // Act
        var bulkMergeResult = await connection.BulkMergeAsync(tables,
            qualifiers: e => new { e.RowGuid, e.ColumnInt });

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForEntitiesWithUsePhysicalPseudoTempTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationIdentityTables(tables);

        // Act
        var bulkMergeResult = await connection.BulkMergeAsync(tables,
            usePhysicalPseudoTempTable: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForEntitiesWithMappings()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add the mappings
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)),
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
        // Setup
        connection.InsertAll(tables);

        // Act
        var bulkMergeResult = await connection.BulkMergeAsync(tables, mappings: mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForMappedEntitiesForEmptyTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkMergeResult = await connection.BulkMergeAsync(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForMappedEntitiesForEmptyTableWithReturnIdentity()
    {
        // Setup
        var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkMergeResult = await connection.BulkMergeAsync(tables, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
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
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForMappedEntities()
    {
        // Setup
        var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationMappedIdentityTables(tables);

        // Act
        var bulkMergeResult = await connection.BulkMergeAsync(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForMappedEntitiesWithReturnIdentity()
    {
        // Setup
        var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationMappedIdentityTables(tables);

        // Act
        var bulkMergeResult = await connection.BulkMergeAsync(tables, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
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
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForMappedEntitiesWithQualifiers()
    {
        // Setup
        var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationMappedIdentityTables(tables);

        // Act
        var bulkMergeResult = await connection.BulkMergeAsync(tables,
            qualifiers: e => new { e.RowGuidMapped, e.ColumnIntMapped });

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForMappedEntitiesWithUsePhysicalPseudoTempTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationMappedIdentityTables(tables);

        // Act
        var bulkMergeResult = await connection.BulkMergeAsync(tables,
            usePhysicalPseudoTempTable: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForMappedEntitiesWithMappings()
    {
        // Setup
        var tables = Helper.CreateBulkOperationMappedIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add the mappings
            new BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.IdMapped), nameof(BulkOperationIdentityTable.Id)),
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
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationMappedIdentityTables(tables);

        // Act
        var bulkMergeResult = await connection.BulkMergeAsync(tables, mappings: mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkMergeAsyncForEntitiesIfTheMappingsAreInvalid()
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
        var bulkMergeResult = await connection.BulkMergeAsync(tables,
            mappings: mappings);

        // Trigger
        Assert.ThrowsExactly<InvalidOperationException>(() => bulkMergeResult);
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForEntitiesDbDataReader()
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
        var bulkMergeResult = await destinationConnection.BulkMergeAsync<BulkOperationIdentityTable>((DbDataReader)reader);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForEntitiesDbDataReaderWithMappings()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add the mappings
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)),
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
        var bulkMergeResult = await destinationConnection.BulkMergeAsync<BulkOperationIdentityTable>((DbDataReader)reader,
            mappings: mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
    }

    [TestMethod]
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkMergeAsyncForEntitiesDbDataReaderIfTheMappingsAreInvalid()
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
        var bulkMergeResult = await destinationConnection.BulkMergeAsync<BulkOperationIdentityTable>((DbDataReader)reader,
            mappings: mappings);

        // Trigger
        Assert.ThrowsExactly<InvalidOperationException>(() => bulkMergeResult);
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForEntitiesDataTable()
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
        var bulkMergeResult = await destinationConnection.BulkMergeAsync<BulkOperationIdentityTable>(table);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForEntitiesDataTableWithReturnIdentity()
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
        var bulkMergeResult = await destinationConnection.BulkMergeAsync<BulkOperationIdentityTable>(table, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

        // Act
        var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        var rows = table.Rows.OfType<DataRow>();
        queryResult.AsList().ForEach(item =>
        {
            var row = rows.Where(r => Equals(item.Id, r["Id"]));
            Assert.IsNotNull(row);
        });
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForEntitiesDataTableWithMappings()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add the mappings
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)),
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
        var bulkMergeResult = await destinationConnection.BulkMergeAsync<BulkOperationIdentityTable>(table,
            mappings: mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
    }

    [TestMethod]
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkMergeAsyncForEntitiesDataTableIfTheMappingsAreInvalid()
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
        var bulkMergeResult = await destinationConnection.BulkMergeAsync<BulkOperationIdentityTable>(table,
            mappings: mappings);

        // Trigger
        Assert.ThrowsExactly<InvalidOperationException>(() => bulkMergeResult);
    }

    //[TestMethod, ExpectedException(typeof(AggregateException))]
    //public void ThrowExceptionOnMicrosoftSqlConnectionBulkMergeAsyncForNullEntities()
    //{
    //    using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
    //    {
    //        connection.BulkInsertAsync((IEnumerable<BulkOperationIdentityTable>)null).Wait();
    //    }
    //}

    //[TestMethod, ExpectedException(typeof(AggregateException))]
    //public void ThrowExceptionOnMicrosoftSqlConnectionBulkMergeAsyncForEmptyEntities()
    //{
    //    using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
    //    {
    //        connection.BulkInsertAsync(Enumerable.Empty<BulkOperationIdentityTable>()).Wait();
    //    }
    //}

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkMergeAsyncForNullDataReader()
    {
        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        Assert.ThrowsExactly<AggregateException>(() => connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            (DbDataReader)null).Wait());
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkMergeAsyncForNullDataTable()
    {
        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        Assert.ThrowsExactly<AggregateException>(() => connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            (DataTable)null).Wait());
    }

    #endregion

    #region BulkMergeAsync<TEntity>(Extra Fields)

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForEntitiesWithExtraFields()
    {
        // Setup
        var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Setup
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateWithExtraFieldsBulkOperationIdentityTables(tables);

        // Act
        var bulkMergeResult = await connection.BulkMergeAsync(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForEntitiesWithExtraFieldsWithMappings()
    {
        // Setup
        var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add the mappings
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)),
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar))
        };

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Setup
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateWithExtraFieldsBulkOperationIdentityTables(tables);

        // Act
        var bulkMergeResult = await connection.BulkMergeAsync(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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

    #region BulkMergeAsync(TableName)

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForTableNameExpandoObjectsForEmptyTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkMergeResult = await connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForTableNameExpandoObjectsForEmptyTableWithReturnIdentity()
    {
        // Setup
        var tables = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkMergeResult = await connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
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
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForTableNameExpandoObjectsForNonEmptyTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll<BulkOperationIdentityTable>(tables);

        // Setup
        var entities = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

        // Act
        var bulkMergeResult = await connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), entities);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(entities.Count, queryResult.Count());
        entities.AsList().ForEach(t =>
        {
            Helper.AssertMembersEquality(queryResult.ElementAt(entities.IndexOf(t)), t);
        });
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForTableNameExpandoObjectsForNonEmptyTableWithReturnIdentity()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll<BulkOperationIdentityTable>(tables);

        // Setup
        var entities = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

        // Act
        var bulkMergeResult = await connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
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
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForTableNameAnonymousObjectsForEmptyTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkMergeResult = await connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            Helper.AssertMembersEquality(queryResult.ElementAt((int)tables.IndexOf(t)), t);
        });
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForTableNameAnonymousObjectsForEmptyTableWithReturnIdentity()
    {
        // Setup
        var tables = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkMergeResult = await connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
        Assert.IsTrue(tables.All(e => ((dynamic)e).Id > 0));

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            Helper.AssertMembersEquality(queryResult.ElementAt((int)tables.IndexOf(t)), t);
        });
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForTableNameAnonymousObjectsForNonEmptyTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll<BulkOperationIdentityTable>(tables);

        // Setup
        var entities = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

        // Act
        var bulkMergeResult = await connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), entities);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(entities.Count, queryResult.Count());
        entities.AsList().ForEach(t =>
        {
            Helper.AssertMembersEquality(queryResult.ElementAt((int)entities.IndexOf(t)), t);
        });
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForTableNameAnonymousObjectsForNonEmptyTableWithReturnIdentity()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll<BulkOperationIdentityTable>(tables);

        // Setup
        var entities = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

        // Act
        var bulkMergeResult = await connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
        Assert.IsTrue(tables.All(e => ((dynamic)e).Id > 0));

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        tables.AsList().ForEach(t =>
        {
            Helper.AssertMembersEquality(queryResult.ElementAt((int)tables.IndexOf(t)), t);
        });
    }

    [TestMethod]
    public async Task TestSystemSqlConnectionBulkMergeAsyncForTableNameDataEntitiesForEmptyTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkMergeResult = await connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForTableNameDataEntitiesForEmptyTableWithReturnIdentity()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        var bulkMergeResult = await connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
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
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForTableNameDataEntitiesForNonEmptyTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll<BulkOperationIdentityTable>(tables);

        // Setup
        Helper.UpdateBulkOperationIdentityTables(tables);

        // Act
        var bulkMergeResult = await connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForTableNameDataEntitiesForNonEmptyTableWithReturnIdentity()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll<BulkOperationIdentityTable>(tables);

        // Setup
        Helper.UpdateBulkOperationIdentityTables(tables);

        // Act
        var bulkMergeResult = await connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
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
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForTableNameDataEntities()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Setup
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationIdentityTables(tables);

        // Act
        var bulkMergeResult = await connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForTableNameDataEntitiesWithReturnIdentity()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Setup
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationIdentityTables(tables);

        // Act
        var bulkMergeResult = await connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, isReturnIdentity: true);
        Assert.IsFalse(tables.Any(e => e.Id <= 0));

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForTableNameDataEntitiesWithQualifiers()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Setup
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationIdentityTables(tables);

        // Act
        var bulkMergeResult = await connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            tables,
            qualifiers: e => new { e.RowGuid, e.ColumnInt });

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForTableNameDataEntitiesWithUsePhysicalPseudoTempTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Setup
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationIdentityTables(tables);

        // Act
        var bulkMergeResult = await connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            tables,
            usePhysicalPseudoTempTable: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

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
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForTableNameDbDataReader()
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
        var bulkMergeResult = await destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), (DbDataReader)reader);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForTableNameDbDataReaderWithMappings()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add the mappings
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)),
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
        var bulkMergeResult = await destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            (DbDataReader)reader,
            mappings: mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
    }

    [TestMethod]
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkMergeAsyncForTableNameDbDataReaderIfTheMappingsAreInvalid()
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
        var bulkMergeResult = await destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            (DbDataReader)reader,
            mappings: mappings);

        // Trigger
        Assert.ThrowsExactly<InvalidOperationException>(() => bulkMergeResult);
    }

    [TestMethod]
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkMergeAsyncForTableNameDbDataReaderIfTheTableNameIsNotValid()
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
        var bulkMergeResult = await destinationConnection.BulkMergeAsync("InvalidTable", (DbDataReader)reader);

        // Trigger
        Assert.ThrowsExactly<MissingFieldsException>(() => bulkMergeResult);
    }

    [TestMethod]
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkMergeAsyncForTableNameDbDataReaderIfTheTableNameIsMissing()
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
        var bulkMergeResult = await destinationConnection.BulkMergeAsync("MissingTable", (DbDataReader)reader);

        // Trigger
        Assert.ThrowsExactly<MissingFieldsException>(() => bulkMergeResult);
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForTableNameDataTable()
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
        var bulkMergeResult = await destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForTableNameDataTableWithReturnIdentity()
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
        var bulkMergeResult = await destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, isReturnIdentity: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);

        // Act
        var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        var rows = table.Rows.OfType<DataRow>();
        queryResult.AsList().ForEach(item =>
        {
            var row = rows.Where(r => Equals(item.Id, r["Id"]));
            Assert.IsNotNull(row);
        });
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkMergeAsyncForTableNameDataTableWithMappings()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add the mappings
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)),
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
        var bulkMergeResult = await destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            table,
            mappings: mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkMergeResult);
    }

    [TestMethod]
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkMergeAsyncForTableNameDataTableIfTheMappingsAreInvalid()
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
        var bulkMergeResult = await destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            table,
            mappings: mappings);

        // Trigger
        Assert.ThrowsExactly<InvalidOperationException>(() => bulkMergeResult);
    }

    [TestMethod]
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkMergeAsyncForTableNameDataTableIfTheTableNameIsNotValid()
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
        var bulkMergeResult = await destinationConnection.BulkMergeAsync("InvalidTable", table);

        // Trigger
        Assert.ThrowsExactly<MissingFieldsException>(() => bulkMergeResult);
    }

    [TestMethod]
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkMergeAsyncForTableNameDataTableIfTheTableNameIsMissing()
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
        var bulkMergeResult = await destinationConnection.BulkMergeAsync("MissingTable",
            table);

        // Trigger
        Assert.ThrowsExactly<MissingFieldsException>(() => bulkMergeResult);
    }

    #endregion
}
