using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SqlServer.BulkOperations.IntegrationTests.Models;

namespace RepoDb.SqlServer.BulkOperations.IntegrationTests;

/// <summary>
/// A class used as a startup setup for for RepoDb test database.
/// </summary>
public static class Database
{
    private static readonly SqlServerDbInstance Instance = new();
    #region Properties

    /// <summary>
    /// Gets or sets the connection string to be used for SQL Server database.
    /// </summary>
    public static string ConnectionStringForMaster => Instance.AdminConnectionString;

    /// <summary>
    /// Gets or sets the connection string to be used.
    /// </summary>
    public static string ConnectionString => Instance.ConnectionString;

    public static string ConnectionStringForRepoDb => ConnectionString;

    #endregion

    public static void Initialize()
    {
        Instance.ClassInitializeAsync(null).GetAwaiter().GetResult();


        GlobalConfiguration.Setup(new())
            .UseSqlServer();

        // Create tables
        CreateTables();
    }

    #region Methods

    /// <summary>
    /// Create the necessary tables for testing.
    /// </summary>
    public static void CreateTables()
    {
        CreateBulkOperationIdentityTable();
    }

    /// <summary>
    /// Clean up all the table.
    /// </summary>
    public static void Cleanup()
    {
        using var connection = new SqlConnection(ConnectionString);
        connection.Truncate<BulkOperationIdentityTable>();
    }

    #endregion

    #region CreateTables

    /// <summary>
    /// Creates an identity table that has some important fields. All fields are nullable.
    /// </summary>
    public static void CreateBulkOperationIdentityTable()
    {
        var commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'BulkOperationIdentityTable'))
                BEGIN
                    CREATE TABLE [dbo].[BulkOperationIdentityTable]
                    (
                        [Id] BIGINT NOT NULL IDENTITY(1, 1),
                        [RowGuid] UNIQUEIDENTIFIER NOT NULL,
                        [ColumnBit] BIT NULL,
                        [ColumnDateTime] DATETIME NULL,
                        [ColumnDateTime2] DATETIME2(7) NULL,
                        [ColumnDecimal] DECIMAL(18, 2) NULL,
                        [ColumnFloat] FLOAT NULL,
                        [ColumnInt] INT NULL,
                        [ColumnNVarChar] NVARCHAR(MAX) NULL,
                        CONSTRAINT [PK_BulkOperationIdentityTable] PRIMARY KEY CLUSTERED
                        (
                            [Id] ASC
                        )
                        WITH (FILLFACTOR = 90) ON [PRIMARY]
                    ) ON [PRIMARY];
                END";
        using var connection = new SqlConnection(ConnectionString).EnsureOpen();
        connection.ExecuteNonQuery(commandText);
    }

    #endregion
}
