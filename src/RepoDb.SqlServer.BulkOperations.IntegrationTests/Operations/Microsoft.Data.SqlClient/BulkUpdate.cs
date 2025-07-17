using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.SqlServer.BulkOperations.IntegrationTests.Models;

namespace RepoDb.SqlServer.BulkOperations.IntegrationTests.Operations;

[TestClass]
public class MicrosoftSqlConnectionBulkUpdateOperationsTest
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

    #region BulkUpdate<TEntity>

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkUpdateForEntities()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationIdentityTables(tables);

        // Act
        var bulkUpdateResult = connection.BulkUpdate(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);

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
    public void TestMicrosoftSqlConnectionBulkUpdateForEntitiesWithQualifiers()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationIdentityTables(tables);

        // Act
        var bulkUpdateResult = connection.BulkUpdate(tables,
            qualifiers: e => new { e.RowGuid, e.ColumnInt });

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);

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
    public void TestMicrosoftSqlConnectionBulkUpdateForEntitiesWithUsePhysicalPseudoTempTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationIdentityTables(tables);

        // Act
        var bulkUpdateResult = connection.BulkUpdate(tables,
            usePhysicalPseudoTempTable: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);

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
    public void TestMicrosoftSqlConnectionBulkUpdateForEntitiesWithMappings()
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
        var bulkUpdateResult = connection.BulkUpdate(tables, mappings: mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);

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
    public void TestMicrosoftSqlConnectionBulkUpdateForMappedEntities()
    {
        // Setup
        var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationMappedIdentityTables(tables);

        // Act
        var bulkUpdateResult = connection.BulkUpdate(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);

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
    public void TestMicrosoftSqlConnectionBulkUpdateForMappedEntitiesWithQualifiers()
    {
        // Setup
        var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationMappedIdentityTables(tables);

        // Act
        var bulkUpdateResult = connection.BulkUpdate(tables,
            qualifiers: e => new { e.RowGuidMapped, e.ColumnIntMapped });

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);

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
    public void TestMicrosoftSqlConnectionBulkUpdateForMappedEntitiesWithUsePhysicalPseudoTempTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationMappedIdentityTables(tables);

        // Act
        var bulkUpdateResult = connection.BulkUpdate(tables,
            usePhysicalPseudoTempTable: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);

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
    public void TestMicrosoftSqlConnectionBulkUpdateForMappedEntitiesWithMappings()
    {
        // Setup
        var tables = Helper.CreateBulkOperationMappedIdentityTables(10);
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
        Helper.UpdateBulkOperationMappedIdentityTables(tables);

        // Act
        var bulkUpdateResult = connection.BulkUpdate(tables, mappings: mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);

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
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateForEntitiesIfTheMappingsAreInvalid()
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
        Assert.ThrowsExactly<InvalidOperationException>(() => connection.BulkUpdate(tables, mappings: mappings));
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkUpdateForEntitiesDbDataReader()
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
        var bulkUpdateResult = destinationConnection.BulkUpdate<BulkOperationIdentityTable>((DbDataReader)reader);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkUpdateForEntitiesDbDataReaderWithMappings()
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
        var bulkUpdateResult = destinationConnection.BulkUpdate<BulkOperationIdentityTable>((DbDataReader)reader,
            mappings: mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateForEntitiesDbDataReaderIfTheMappingsAreInvalid()
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
        Assert.ThrowsExactly<InvalidOperationException>(() => destinationConnection.BulkUpdate<BulkOperationIdentityTable>((DbDataReader)reader,
            mappings: mappings));
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkUpdateForEntitiesDataTable()
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
        var bulkUpdateResult = destinationConnection.BulkUpdate<BulkOperationIdentityTable>(table);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkUpdateForEntitiesDataTableWithMappings()
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
        var bulkUpdateResult = destinationConnection.BulkUpdate<BulkOperationIdentityTable>(table,
            mappings: mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateForEntitiesDataTableIfTheMappingsAreInvalid()
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
        Assert.ThrowsExactly<InvalidOperationException>(() => destinationConnection.BulkUpdate<BulkOperationIdentityTable>(table,
            mappings: mappings));
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateForNullEntities()
    {
        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        Assert.ThrowsExactly<ArgumentNullException>(() => connection.BulkUpdate((IEnumerable<BulkOperationIdentityTable>)null));
    }

    //[TestMethod, ExpectedException(typeof(EmptyException))]
    //public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateForEmptyEntities()
    //{
    //    using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
    //    {
    //        connection.BulkUpdate(Enumerable.Empty<BulkOperationIdentityTable>());
    //    }
    //}

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateForNullDataReader()
    {
        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        Assert.ThrowsExactly<ArgumentNullException>(() => connection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            (DbDataReader)null));
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateForNullDataTable()
    {
        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        Assert.ThrowsExactly<ArgumentNullException>(() => connection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            (DataTable)null));
    }

    #endregion

    #region BulkUpdate<TEntity>(Extra Fields)

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkUpdateForEntitiesWithExtraFields()
    {
        // Setup
        var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Setup
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateWithExtraFieldsBulkOperationIdentityTables(tables);

        // Act
        var bulkUpdateResult = connection.BulkUpdate(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);

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
    public void TestMicrosoftSqlConnectionBulkUpdateForEntitiesWithExtraFieldsWithMappings()
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
        var bulkUpdateResult = connection.BulkUpdate(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);

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

    #region BulkUpdate(TableName)

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkUpdateForTableNameExpandoObjects()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        var entities = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

        // Act
        var bulkUpdateResult = connection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), entities);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        entities.AsList().ForEach(t =>
        {
            Helper.AssertMembersEquality(t, queryResult.ElementAt(entities.IndexOf(t)));
        });
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkUpdateForTableNameAnonymousObjects()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        var entities = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

        // Act
        var bulkUpdateResult = connection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), entities);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        entities.AsList().ForEach(t =>
        {
            Helper.AssertMembersEquality(t, queryResult.ElementAt((int)entities.IndexOf(t)));
        });
    }

    [TestMethod]
    public void TestSystemSqlConnectionBulkUpdateForTableNameDataEntities()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationIdentityTables(tables);

        // Act
        var bulkUpdateResult = connection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);

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
    public void TestMicrosoftSqlConnectionBulkUpdateForTableNameDataEntitiesWithQualifiers()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationIdentityTables(tables);

        // Act
        var bulkUpdateResult = connection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            tables,
            qualifiers: e => new { e.RowGuid, e.ColumnInt });

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);

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
    public void TestMicrosoftSqlConnectionBulkUpdateForTableNameDataEntitiesWithUsePhysicalPseudoTempTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationIdentityTables(tables);

        // Act
        var bulkUpdateResult = connection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            tables,
            usePhysicalPseudoTempTable: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);

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
    public void TestMicrosoftSqlConnectionBulkUpdateForTableNameDbDataReader()
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
        var bulkUpdateResult = destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            (DbDataReader)reader);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkUpdateForTableNameDbDataReaderWithMappings()
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
        var bulkUpdateResult = destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            (DbDataReader)reader,
            mappings: mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateForTableNameDbDataReaderIfTheMappingsAreInvalid()
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
        Assert.ThrowsExactly<InvalidOperationException>(() => destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            (DbDataReader)reader,
            mappings: mappings));
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateForTableNameDbDataReaderIfTheTableNameIsNotValid()
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
        Assert.ThrowsExactly<MissingFieldsException>(() => destinationConnection.BulkUpdate("InvalidTable", (DbDataReader)reader));
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateForTableNameDbDataReaderIfTheTableNameIsMissing()
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
        Assert.ThrowsExactly<MissingFieldsException>(() => destinationConnection.BulkUpdate("MissingTable", (DbDataReader)reader));
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkUpdateForTableNameDbDataTable()
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
        var bulkUpdateResult = destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);
    }

    [TestMethod]
    public void TestMicrosoftSqlConnectionBulkUpdateForTableNameDbDataTableWithMappings()
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
        var bulkUpdateResult = destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            table,
            mappings: mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateForTableNameDbDataTableIfTheMappingsAreInvalid()
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
        Assert.ThrowsExactly<InvalidOperationException>(() => destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            table,
            mappings: mappings));
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateForTableNameDbDataTableIfTheTableNameIsNotValid()
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
        Assert.ThrowsExactly<MissingFieldsException>(() => destinationConnection.BulkUpdate("InvalidTable",
            table));
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateForTableNameDbDataTableIfTheTableNameIsMissing()
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
        Assert.ThrowsExactly<MissingFieldsException>(() => destinationConnection.BulkUpdate("MissingTable",
            table));
    }

    #endregion

    #region BulkUpdateAsync<TEntity>

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkUpdateAsyncForEntities()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationIdentityTables(tables);

        // Act
        var bulkUpdateResult = await connection.BulkUpdateAsync(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);

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
    public async Task TestMicrosoftSqlConnectionBulkUpdateAsyncForEntitiesWithQualifiers()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationIdentityTables(tables);

        // Act
        var bulkUpdateResult = await connection.BulkUpdateAsync(tables,
            qualifiers: e => new { e.RowGuid, e.ColumnInt });

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);

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
    public async Task TestMicrosoftSqlConnectionBulkUpdateAsyncForEntitiesWithUsePhysicalPseudoTempTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationIdentityTables(tables);

        // Act
        var bulkUpdateResult = await connection.BulkUpdateAsync(tables,
            usePhysicalPseudoTempTable: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);

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
    public async Task TestMicrosoftSqlConnectionBulkUpdateAsyncForEntitiesWithMappings()
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
        var bulkUpdateResult = await connection.BulkUpdateAsync(tables, mappings: mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);

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
    public async Task TestMicrosoftSqlConnectionBulkUpdateAsyncForMappedEntities()
    {
        // Setup
        var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationMappedIdentityTables(tables);

        // Act
        var bulkUpdateResult = await connection.BulkUpdateAsync(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);

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
    public async Task TestMicrosoftSqlConnectionBulkUpdateAsyncForMappedEntitiesWithQualifiers()
    {
        // Setup
        var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationMappedIdentityTables(tables);

        // Act
        var bulkUpdateResult = await connection.BulkUpdateAsync(tables,
            qualifiers: e => new { e.RowGuidMapped, e.ColumnIntMapped });

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);

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
    public async Task TestMicrosoftSqlConnectionBulkUpdateAsyncForMappedEntitiesWithUsePhysicalPseudoTempTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationMappedIdentityTables(tables);

        // Act
        var bulkUpdateResult = await connection.BulkUpdateAsync(tables,
            usePhysicalPseudoTempTable: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);

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
    public async Task TestMicrosoftSqlConnectionBulkUpdateAsyncForMappedEntitiesWithMappings()
    {
        // Setup
        var tables = Helper.CreateBulkOperationMappedIdentityTables(10);
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
        Helper.UpdateBulkOperationMappedIdentityTables(tables);

        // Act
        var bulkUpdateResult = await connection.BulkUpdateAsync(tables, mappings: mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);

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
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateAsyncForEntitiesIfTheMappingsAreInvalid()
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

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(async () => await connection.BulkUpdateAsync(tables,
            mappings: mappings));
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkUpdateAsyncForEntitiesDbDataReader()
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
        var bulkUpdateResult = await destinationConnection.BulkUpdateAsync<BulkOperationIdentityTable>((DbDataReader)reader);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkUpdateAsyncForEntitiesDbDataReaderWithMappings()
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
        var bulkUpdateResult = await destinationConnection.BulkUpdateAsync<BulkOperationIdentityTable>((DbDataReader)reader,
            mappings: mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);
    }

    [TestMethod]
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateAsyncForEntitiesDbDataReaderIfTheMappingsAreInvalid()
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
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(async () => await destinationConnection.BulkUpdateAsync<BulkOperationIdentityTable>((DbDataReader)reader,
            mappings: mappings));
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkUpdateAsyncForEntitiesDataTable()
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
        var bulkUpdateResult = await destinationConnection.BulkUpdateAsync<BulkOperationIdentityTable>(table);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkUpdateAsyncForEntitiesDataTableWithMappings()
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
        var bulkUpdateResult = await destinationConnection.BulkUpdateAsync<BulkOperationIdentityTable>(table,
            mappings: mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);
    }

    [TestMethod]
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateAsyncForEntitiesDataTableIfTheMappingsAreInvalid()
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
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(async () => await destinationConnection.BulkUpdateAsync<BulkOperationIdentityTable>(table,
            mappings: mappings));
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateAsyncForNullEntities()
    {
        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        Assert.ThrowsExactly<AggregateException>(() => connection.BulkUpdateAsync((IEnumerable<BulkOperationIdentityTable>)null).Wait());
    }

    //[TestMethod, ExpectedException(typeof(AggregateException))]
    //public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateAsyncForEmptyEntities()
    //{
    //    using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
    //    {
    //        connection.BulkUpdateAsync(Enumerable.Empty<BulkOperationIdentityTable>()).Wait();
    //    }
    //}

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateAsyncForNullDataReader()
    {
        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        Assert.ThrowsExactly<AggregateException>(() => connection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            (DbDataReader)null).Wait());
    }

    [TestMethod]
    public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateAsyncForNullDataTable()
    {
        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        Assert.ThrowsExactly<AggregateException>(() => connection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            (DataTable)null).Wait());
    }

    #endregion

    #region BulkUpdateAsync<TEntity>(Extra Fields)

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkUpdateAsyncForEntitiesWithExtraFields()
    {
        // Setup
        var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Setup
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateWithExtraFieldsBulkOperationIdentityTables(tables);

        // Act
        var bulkUpdateResult = await connection.BulkUpdateAsync(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);

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
    public async Task TestMicrosoftSqlConnectionBulkUpdateAsyncForEntitiesWithExtraFieldsWithMappings()
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
        var bulkUpdateResult = await connection.BulkUpdateAsync(tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);

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

    #region BulkUpdateAsync(TableName)

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkUpdateAsyncForTableNameExpandoObjects()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        var entities = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

        // Act
        var bulkUpdateResult = await connection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), entities);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        entities.AsList().ForEach(t =>
        {
            Helper.AssertMembersEquality(t, queryResult.ElementAt(entities.IndexOf(t)));
        });
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkUpdateAsyncForTableNameAnonymousObjects()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        var entities = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

        // Act
        var bulkUpdateResult = await connection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), entities);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);

        // Act
        var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
        entities.AsList().ForEach(t =>
        {
            Helper.AssertMembersEquality(t, queryResult.ElementAt((int)entities.IndexOf(t)));
        });
    }

    [TestMethod]
    public async Task TestSystemSqlConnectionBulkUpdateAsyncForTableNameDataEntities()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationIdentityTables(tables);

        // Act
        var bulkUpdateResult = await connection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);

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
    public async Task TestMicrosoftSqlConnectionBulkUpdateAsyncForTableNameDataEntitiesWithQualifiers()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationIdentityTables(tables);

        // Act
        var bulkUpdateResult = await connection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            tables,
            qualifiers: e => new { e.RowGuid, e.ColumnInt });

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);

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
    public async Task TestMicrosoftSqlConnectionBulkUpdateAsyncForTableNameDataEntitiesWithUsePhysicalPseudoTempTable()
    {
        // Setup
        var tables = Helper.CreateBulkOperationIdentityTables(10);

        using var connection = new SqlConnection(Database.ConnectionStringForRepoDb);
        // Act
        connection.InsertAll(tables);

        // Setup
        Helper.UpdateBulkOperationIdentityTables(tables);

        // Act
        var bulkUpdateResult = await connection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            tables,
            usePhysicalPseudoTempTable: true);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);

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
    public async Task TestMicrosoftSqlConnectionBulkUpdateAsyncForTableNameDbDataReader()
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
        var bulkUpdateResult = await destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), (DbDataReader)reader);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkUpdateAsyncForTableNameDbDataReaderWithMappings()
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
        var bulkUpdateResult = await destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            (DbDataReader)reader,
            mappings: mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);
    }

    [TestMethod]
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateAsyncForTableNameDbDataReaderIfTheMappingsAreInvalid()
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
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(async () => await destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            (DbDataReader)reader,
            mappings: mappings));
    }

    [TestMethod]
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateAsyncForTableNameDbDataReaderIfTheTableNameIsNotValid()
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
        await Assert.ThrowsExactlyAsync<MissingFieldsException>(async () => await destinationConnection.BulkUpdateAsync("InvalidTable", (DbDataReader)reader));
    }

    [TestMethod]
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateAsyncForTableNameDbDataReaderIfTheTableNameIsMissing()
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

        await Assert.ThrowsExactlyAsync<MissingFieldsException>(async () => await destinationConnection.BulkUpdateAsync("MissingTable", (DbDataReader)reader));
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkUpdateAsyncForTableNameDataTable()
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
        var bulkUpdateResult = await destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);
    }

    [TestMethod]
    public async Task TestMicrosoftSqlConnectionBulkUpdateAsyncForTableNameDataTableWithMappings()
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
        var bulkUpdateResult = await destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            table,
            mappings: mappings);

        // Assert
        Assert.AreEqual(tables.Count, bulkUpdateResult);
    }

    [TestMethod]
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateAsyncForTableNameDataTableIfTheMappingsAreInvalid()
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
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(async () => await destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
            table,
            mappings: mappings));
    }

    [TestMethod]
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateAsyncForTableNameDataTableIfTheTableNameIsNotValid()
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
        await Assert.ThrowsExactlyAsync<MissingFieldsException>(async () => await destinationConnection.BulkUpdateAsync("InvalidTable", table));
    }

    [TestMethod]
    public async Task ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateAsyncForTableNameDataTableIfTheTableNameIsMissing()
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
        await Assert.ThrowsExactlyAsync<MissingFieldsException>(async () => await destinationConnection.BulkUpdateAsync("MissingTable",
            table));
    }

    #endregion
}
