using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.SqlServer.BulkOperations.IntegrationTests.Models;

namespace RepoDb.SqlServer.BulkOperations.IntegrationTests.Operations;

[TestClass]
public class MicrosoftSqlConnectionBulkDeleteOperationsTest
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

    #region BulkDelete<TEntity>

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkDeleteForEntitiesViaPrimaryKeys()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        var primaryKeys = tables.Select(e => (object)e.Id);

        // Act
        var bulkDeleteResult = connection.BulkDelete<BulkOperationIdentityTable>(primaryKeys);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = connection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkDeleteForEntities()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10).AsList();

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Act
        var bulkDeleteResult = connection.BulkDelete(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = connection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkDeleteForEntitiesWithQualifiers()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10).AsList();

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Act
        var bulkDeleteResult = connection.BulkDelete(tables,
            qualifiers: e => new { e.RowGuid, e.ColumnInt });

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = connection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkDeleteForEntitiesWithUsePhysicalPseudoTempTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10).AsList();

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Act
        var bulkDeleteResult = connection.BulkDelete(tables,
            usePhysicalPseudoTempTable: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = connection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkDeleteForEntitiesWithMappings()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);
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
        // Act
        connection.InsertAll(tables);

        // Act
        var bulkDeleteResult = connection.BulkDelete(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = connection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkDeleteForMappedEntities()
    {
        // Setup
        var tables = Helper.CreateBulkOperationMappedIdentityTables(10).AsList();

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Act
        var bulkDeleteResult = connection.BulkDelete(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = connection.CountAll<BulkOperationMappedIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkDeleteForMappedEntitiesWithQualifiers()
    {
        // Setup
        var tables = Helper.CreateBulkOperationMappedIdentityTables(10).AsList();

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Act
        var bulkDeleteResult = connection.BulkDelete(tables,
            qualifiers: e => new { e.RowGuidMapped, e.ColumnIntMapped });

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = connection.CountAll<BulkOperationMappedIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkDeleteForMappedEntitiesWithUsePhysicalPseudoTempTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationMappedIdentityTables(10).AsList();

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Act
        var bulkDeleteResult = connection.BulkDelete(tables,
            usePhysicalPseudoTempTable: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = connection.CountAll<BulkOperationMappedIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkDeleteForMappedEntitiesWithMappings()
    {
        // Setup
        var tables = Helper.CreateBulkOperationMappedIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add the mappings
            new BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.IdMapped), nameof(BulkOperationIdentityTable.Id)),
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

        // Act
        var bulkDeleteResult = connection.BulkDelete(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = connection.CountAll<BulkOperationMappedIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkDeleteForEntitiesIfTheMappingsAreInvalid()
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
        Assert.ThrowsExactly<InvalidOperationException>(() => connection.BulkDelete(tables, null, mappings));
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkDeleteForEntitiesDbDataReader()
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
        var bulkDeleteResult = destinationConnection.BulkDelete<BulkOperationIdentityTable>((DbDataReader)reader);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = destinationConnection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkDeleteForEntitiesDbDataReaderWithMappings()
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
        var bulkDeleteResult = destinationConnection.BulkDelete<BulkOperationIdentityTable>((DbDataReader)reader, null, mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = destinationConnection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkDeleteForEntitiesDbDataReaderIfTheMappingsAreInvalid()
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
        Assert.ThrowsExactly<InvalidOperationException>(() => destinationConnection.BulkDelete<BulkOperationIdentityTable>((DbDataReader)reader, null, mappings));
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkDeleteForEntitiesDataTable()
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
        var bulkDeleteResult = destinationConnection.BulkDelete<BulkOperationIdentityTable>(table);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = destinationConnection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkDeleteForEntitiesDataTableWithMappings()
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
        var bulkDeleteResult = destinationConnection.BulkDelete<BulkOperationIdentityTable>(table, null, DataRowState.Unchanged, mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = destinationConnection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkDeleteForEntitiesDataTableIfTheMappingsAreInvalid()
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
        Assert.ThrowsExactly<InvalidOperationException>(() => destinationConnection.BulkDelete<BulkOperationIdentityTable>(table, null, DataRowState.Unchanged, mappings));
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkDeleteForNullEntities()
    {
        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        Assert.ThrowsExactly<ArgumentNullException>(() => connection.BulkDelete((IEnumerable<BulkOperationIdentityTable>)null));
    }

    //[TestMethod, ExpectedException(typeof(EmptyException))]
    //public void ThrowExceptionOnMicrosoftSqlConnectionBulkDeleteForEmptyEntities()
    //{
    //    using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
    //    {
    //        connection.BulkDelete(Enumerable.Empty<BulkOperationIdentityTable>());
    //    }
    //}

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkDeleteForNullDataReader()
    {
        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        Assert.ThrowsExactly<ArgumentNullException>(() => connection.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            (DbDataReader)null));
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkDeleteForNullDataTable()
    {
        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        Assert.ThrowsExactly<ArgumentNullException>(() => connection.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            (DataTable)null));
    }

    #endregion

    #region BulkDelete<TEntity>(Extra Fields)

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkDeleteForEntitiesWithExtraFields()
    {
        // Setup
        var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Act
        var bulkDeleteResult = connection.BulkDelete(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = connection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkDeleteForEntitiesWithExtraFieldsWithMappings()
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
        // Act
        connection.InsertAll(tables);

        // Act
        var bulkDeleteResult = connection.BulkDelete(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = connection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    #endregion

    #region BulkDelete(TableName)

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkDeleteForTableNameEntitiesViaPrimaryKeys()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        var primaryKeys = tables.Select(e => (object)e.Id);

        // Act
        var bulkDeleteResult = connection.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            primaryKeys: primaryKeys);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = connection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkDeleteForTableNameExpandoObjects()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        var entities = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

        // Act
        var bulkDeleteResult = connection.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), entities);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = connection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkDeleteForTableNameAnonymousObjects()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        var entities = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

        // Act
        var bulkDeleteResult = connection.BulkDelete<object>(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), entities);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = connection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public void TestSystemSqlConnectionBulkDeleteForTableNameDataEntities()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Act
        var bulkDeleteResult = connection.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = connection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkDeleteForTableNameDataEntitiesWithQualifiers()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Act
        var bulkDeleteResult = connection.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            entities: tables,
            qualifiers: e => new { e.RowGuid, e.ColumnInt });

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = connection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkDeleteForTableNameDataEntitiesWithUsePhysicalPseudoTempTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Act
        var bulkDeleteResult = connection.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            entities: tables,
            usePhysicalPseudoTempTable: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = connection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkDeleteForTableNameDbDataReader()
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
        var bulkDeleteResult = destinationConnection.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), (DbDataReader)reader);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = destinationConnection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkDeleteForTableNameDbDataReaderWithMappings()
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
        var bulkDeleteResult = destinationConnection.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            (DbDataReader)reader,
            null,
            mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = destinationConnection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkDeleteForTableNameDbDataReaderIfTheMappingsAreInvalid()
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
        var bulkDeleteResult = destinationConnection.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            (DbDataReader)reader,
            null,
            mappings);

        // Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => Assert.AreEqual(tables.Count, bulkDeleteResult));
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkDeleteForTableNameDbDataTable()
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
        var bulkDeleteResult = destinationConnection.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = destinationConnection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkDeleteForTableNameDbDataTableWithMappings()
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
        var bulkDeleteResult = destinationConnection.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            table,
            null,
            DataRowState.Unchanged,
            mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = destinationConnection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkDeleteForTableNameDbDataTableIfTheMappingsAreInvalid()
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
        var bulkDeleteResult = destinationConnection.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            table,
            null,
            DataRowState.Unchanged,
            mappings);

        // Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => Assert.AreEqual(tables.Count, bulkDeleteResult));
    }

    #endregion

    #region BulkDeleteAsync<TEntity>

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkDeleteAsyncForEntitiesViaPrimaryKeys()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        var primaryKeys = tables.Select(e => (object)e.Id);

        // Act
        var bulkDeleteResult = await connection.BulkDeleteAsync<BulkOperationIdentityTable>(primaryKeys);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = connection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkDeleteAsyncForEntities()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Act
        var bulkDeleteResult = await connection.BulkDeleteAsync(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = connection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkDeleteAsyncForEntitiesWithQualifiers()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10).AsList();

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Act
        var bulkDeleteResult = await connection.BulkDeleteAsync(tables,
            qualifiers: e => new { e.RowGuid, e.ColumnInt });

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = connection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkDeleteAsyncForEntitiesWithUsePhysicalPseudoTempTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10).AsList();

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Act
        var bulkDeleteResult = await connection.BulkDeleteAsync(tables,
            usePhysicalPseudoTempTable: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = connection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkDeleteAsyncForEntitiesWithMappings()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);
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
        // Act
        connection.InsertAll(tables);

        // Act
        var bulkDeleteResult = await connection.BulkDeleteAsync(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = connection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkDeleteAsyncForMappedEntities()
    {
        // Setup
        var tables = Helper.CreateBulkOperationMappedIdentityTables(10).AsList();

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Act
        var bulkDeleteResult = await connection.BulkDeleteAsync(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = connection.CountAll<BulkOperationMappedIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkDeleteAsyncForMappedEntitiesWithQualifiers()
    {
        // Setup
        var tables = Helper.CreateBulkOperationMappedIdentityTables(10).AsList();

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Act
        var bulkDeleteResult = await connection.BulkDeleteAsync(tables,
            qualifiers: e => new { e.RowGuidMapped, e.ColumnIntMapped });

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = connection.CountAll<BulkOperationMappedIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkDeleteAsyncForMappedEntitiesWithUsePhysicalPseudoTempTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationMappedIdentityTables(10).AsList();

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Act
        var bulkDeleteResult = await connection.BulkDeleteAsync(tables,
            usePhysicalPseudoTempTable: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = connection.CountAll<BulkOperationMappedIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkDeleteAsyncForMappedEntitiesWithMappings()
    {
        // Setup
        var tables = Helper.CreateBulkOperationMappedIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add the mappings
            new BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.IdMapped), nameof(BulkOperationIdentityTable.Id)),
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

        // Act
        var bulkDeleteResult = await connection.BulkDeleteAsync(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = connection.CountAll<BulkOperationMappedIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkDeleteAsyncForEntitiesIfTheMappingsAreInvalid()
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

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(async () => await connection.BulkDeleteAsync(tables,
            null,
            mappings));
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkDeleteAsyncForEntitiesDbDataReader()
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
        var bulkDeleteResult = await destinationConnection.BulkDeleteAsync<BulkOperationIdentityTable>((DbDataReader)reader);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = destinationConnection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkDeleteAsyncForEntitiesDbDataReaderWithMappings()
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
        var bulkDeleteResult = await destinationConnection.BulkDeleteAsync<BulkOperationIdentityTable>((DbDataReader)reader,
            null,
            mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = destinationConnection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkDeleteAsyncForEntitiesDbDataReaderIfTheMappingsAreInvalid()
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
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(async () => await destinationConnection.BulkDeleteAsync<BulkOperationIdentityTable>((DbDataReader)reader,
            null,
            mappings));
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkDeleteAsyncForEntitiesDataTable()
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
        var bulkDeleteResult = await destinationConnection.BulkDeleteAsync<BulkOperationIdentityTable>(table);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = destinationConnection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkDeleteAsyncForEntitiesDataTableWithMappings()
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
        var bulkDeleteResult = await destinationConnection.BulkDeleteAsync<BulkOperationIdentityTable>(table,
            null,
            DataRowState.Unchanged,
            mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = destinationConnection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkDeleteAsyncForEntitiesDataTableIfTheMappingsAreInvalid()
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
        var bulkDeleteResult = await destinationConnection.BulkDeleteAsync<BulkOperationIdentityTable>(table,
            null,
            DataRowState.Unchanged,
            mappings);

        // Trigger
        Assert.ThrowsExactly<InvalidOperationException>(() => bulkDeleteResult);
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkDeleteAsyncForNullEntities()
    {
        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        Assert.ThrowsExactly<AggregateException>(() => connection.BulkDeleteAsync((IEnumerable<BulkOperationIdentityTable>)null).Wait());
    }

    //[TestMethod, ExpectedException(typeof(AggregateException))]
    //public void ThrowExceptionOnMicrosoftSqlConnectionBulkDeleteAsyncForEmptyEntities()
    //{
    //    using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
    //    {
    //        connection.BulkDeleteAsync(Enumerable.Empty<BulkOperationIdentityTable>()).Wait();
    //    }
    //}

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkDeleteAsyncForNullDataReader()
    {
        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        Assert.ThrowsExactly<AggregateException>(() => connection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            (DbDataReader)null).Wait());
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkDeleteAsyncForNullDataTable()
    {
        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        Assert.ThrowsExactly<AggregateException>(() => connection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            (DataTable)null).Wait());
    }

    #endregion

    #region BulkDeleteAsync<TEntity>(Extra Fields)

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkDeleteAsyncForEntitiesWithExtraFields()
    {
        // Setup
        var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Act
        var bulkDeleteResult = await connection.BulkDeleteAsync(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = connection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkDeleteAsyncForEntitiesWithExtraFieldsWithMappings()
    {
        // Setup
        var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);
        var mappings = new List<BulkInsertMapItem>
        {
            // Add the mappings
            new BulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)),
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
        // Act
        connection.InsertAll(tables);

        // Act
        var bulkDeleteResult = await connection.BulkDeleteAsync(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = connection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    #endregion

    #region BulkDeleteAsync(TableName)

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkDeleteAsyncForTableNameEntitiesViaPrimaryKeys()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        var primaryKeys = tables.Select(e => (object)e.Id);

        // Act
        var bulkDeleteResult = await connection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            primaryKeys: primaryKeys);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = connection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkDeleteAsyncForTableNameExpandoObjects()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        var entities = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

        // Act
        var bulkDeleteResult = await connection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), entities);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = connection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkDeleteAsyncForTableNameAnonymousObjects()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        var entities = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

        // Act
        var bulkDeleteResult = await connection.BulkDeleteAsync<object>(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), entities);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = connection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public async Task TestSystemSqlConnectionBulkDeleteAsyncForTableNameDataEntities()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Act
        var bulkDeleteResult = await connection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = connection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkDeleteAsyncForTableNameDataEntitiesWithQualifiers()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Act
        var bulkDeleteResult = await connection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            entities: tables,
            qualifiers: e => new { e.RowGuid, e.ColumnInt });

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = connection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkDeleteAsyncForTableNameDataEntitiesWithUsePhysicalPseudoTempTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Act
        var bulkDeleteResult = await connection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            entities: tables,
            usePhysicalPseudoTempTable: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = connection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkDeleteAsyncForTableNameDbDataReader()
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
        var bulkDeleteResult = await destinationConnection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), (DbDataReader)reader);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = destinationConnection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkDeleteAsyncForTableNameDbDataReaderWithMappings()
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
        var bulkDeleteResult = await destinationConnection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            (DbDataReader)reader,
            null,
            mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = destinationConnection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkDeleteAsyncForTableNameDbDataReaderIfTheMappingsAreInvalid()
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
        var bulkDeleteResult = await destinationConnection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            (DbDataReader)reader,
            null,
            mappings);

        // Trigger
        Assert.ThrowsExactly<InvalidOperationException>(() => bulkDeleteResult);
    }

    [TestMethod]
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkDeleteAsyncForTableNameDbDataReaderIfTheTableNameIsNotValid()
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
        var bulkDeleteResult = await destinationConnection.BulkDeleteAsync("InvalidTable", (DbDataReader)reader);

        // Trigger
        Assert.ThrowsExactly<MissingFieldsException>(() => bulkDeleteResult);
    }

    [TestMethod]
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkDeleteAsyncForTableNameDbDataReaderIfTheTableNameIsMissing()
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
        var bulkDeleteResult = await destinationConnection.BulkDeleteAsync("MissingTable", (DbDataReader)reader);

        // Trigger
        Assert.ThrowsExactly<MissingFieldsException>(() => bulkDeleteResult);
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkDeleteAsyncForTableNameDataTable()
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
        var bulkDeleteResult = await destinationConnection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = destinationConnection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkDeleteAsyncForTableNameDataTableWithMappings()
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
        var bulkDeleteResult = await destinationConnection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            table,
            null,
            DataRowState.Unchanged,
            mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkDeleteResult);

        // Act
        var countResult = destinationConnection.CountAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(0, countResult);
    }

    [TestMethod]
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkDeleteAsyncForTableNameDataTableIfTheMappingsAreInvalid()
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
        var bulkDeleteResult = await destinationConnection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            table,
            null,
            DataRowState.Unchanged,
            mappings);

        // Trigger
        Assert.ThrowsExactly<InvalidOperationException>(() => bulkDeleteResult);
    }

    [TestMethod]
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkDeleteAsyncForTableNameDataTableIfTheTableNameIsNotValid()
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
        var bulkDeleteResult = await destinationConnection.BulkDeleteAsync("InvalidTable", table);

        // Trigger
        Assert.ThrowsExactly<MissingFieldsException>(() => bulkDeleteResult);
    }

    [TestMethod]
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkDeleteAsyncForTableNameDataTableIfTheTableNameIsMissing()
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
        var bulkDeleteResult = await destinationConnection.BulkDeleteAsync("MissingTable",
            table,
            null,
            DataRowState.Unchanged);

        // Trigger
        Assert.ThrowsExactly<MissingFieldsException>(() => bulkDeleteResult);
    }

    #endregion
}
