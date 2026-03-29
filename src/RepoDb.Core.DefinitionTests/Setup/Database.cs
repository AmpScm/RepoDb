using Microsoft.Data.SqlClient;

namespace RepoDb.IntegrationTests.Setup;

/// <summary>
/// A class used as a startup setup for for RepoDb test database.
/// </summary>
public static class Database
{
    private static readonly SqlServerDbInstance Instance = new SqlServerDbInstance();


    /// <summary>
    /// Initialize the creation of the database.
    /// </summary>
    public static void Initialize()
    {
        // Initialize the SqlServer
        GlobalConfiguration.Setup(new()).UseSqlServer();

        // Create the tables
        CreateTables();
    }

    /// <summary>
    /// Gets the connection string for master.
    /// </summary>
    public static string ConnectionStringForMaster => Instance.AdminConnectionString;

    /// <summary>
    /// Gets the connection string for RepoDb.
    /// </summary>
    public static string ConnectionStringForRepoDb => Instance.ConnectionString;

    #region Methods
    /// <summary>
    /// Create the necessary tables for testing.
    /// </summary>
    private static void CreateTables()
    {
        CreateIdentityTable();
    }

    /// <summary>
    /// Clean up all the table.
    /// </summary>
    public static void Cleanup()
    {
        using var connection = new SqlConnection(ConnectionStringForRepoDb);
        connection.ExecuteNonQuery(@"
            TRUNCATE TABLE [dbo].[IdentityTable];
        ");
    }

    #endregion

    

    #region CreateTables

    /// <summary>
    /// Creates an identity table that has some important fields. All fields are nullable.
    /// </summary>
    private static void CreateIdentityTable()
    {
        var commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'IdentityTable'))
                BEGIN
                    CREATE TABLE [dbo].[IdentityTable]
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
                    ) ON [PRIMARY];
                END";
        using var connection = new SqlConnection(ConnectionStringForRepoDb).EnsureOpen();
        connection.ExecuteNonQuery(commandText);
    }
    #endregion

}
