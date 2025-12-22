using Microsoft.Data.SqlClient;
using RepoDb.SqlServer.IntegrationTests.Models;

namespace RepoDb.SqlServer.IntegrationTests.Setup;

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

    #endregion

    #region Methods

    public static void Initialize()
    {
        Instance.ClassInitializeAsync(null).GetAwaiter().GetResult();

        GlobalConfiguration.Setup(new())
            .UseSqlServer();

        // Create tables
        CreateTables();
    }

    public static void Cleanup()
    {
        using var connection = new SqlConnection(ConnectionString);
        connection.Truncate<CompleteTable>();
        connection.Truncate<NonIdentityCompleteTable>();
        connection.Truncate<MultiKeyTable>();
    }

    #endregion

    #region CreateTables

    private static void CreateTables()
    {
        CreateCompleteTable();
        CreateNonIdentityCompleteTable();
        CreateMultiKeyTable();
    }

    private static void CreateCompleteTable()
    {
        var commandText = @"
    IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'CompleteTable'))
    BEGIN
        CREATE TABLE [dbo].[CompleteTable]
        (
            [Id] INT IDENTITY(1, 1),
            [SessionId] UNIQUEIDENTIFIER NOT NULL,
            [ColumnBigInt] BIGINT NULL,
            [ColumnBinary] BINARY(4000) NULL,
            [ColumnBit] BIT NULL,
            [ColumnChar] CHAR(1) NULL,
            [ColumnDate] DATE NULL,
            [ColumnDateTime] DATETIME NULL,
            [ColumnDateTime2] DATETIME2(7) NULL,
            [ColumnDateTimeOffset] DATETIMEOFFSET(7) NULL,
            [ColumnDecimal] DECIMAL(18, 2) NULL,
            [ColumnFloat] FLOAT NULL,
            [ColumnGeography] GEOGRAPHY NULL,
            [ColumnGeometry] GEOMETRY NULL,
            [ColumnHierarchyId] HIERARCHYID NULL,
            [ColumnImage] IMAGE NULL,
            [ColumnInt] INT NULL,
            [ColumnMoney] MONEY NULL,
            [ColumnNChar] NCHAR(1) NULL,
            [ColumnNText] NTEXT NULL,
            [ColumnNumeric] NUMERIC(18, 2) NULL,
            [ColumnNVarChar] NVARCHAR(MAX) NULL,
            [ColumnReal] REAL NULL,
            [ColumnSmallDateTime] SMALLDATETIME NULL,
            [ColumnSmallInt] SMALLINT NULL,
            [ColumnSmallMoney] SMALLMONEY NULL,
            [ColumnSqlVariant] SQL_VARIANT NULL,
            [ColumnText] TEXT NULL,
            [ColumnTime] TIME(7) NULL,
            [ColumnTimestamp] TIMESTAMP NULL,
            [ColumnTinyInt] TINYINT NULL,
            [ColumnUniqueIdentifier] UNIQUEIDENTIFIER NULL,
            [ColumnVarBinary] VARBINARY(MAX) NULL,
            [ColumnVarChar] VARCHAR(MAX) NULL,
            [ColumnXml] XML NULL,
            CONSTRAINT [CompleteTable_Id] PRIMARY KEY
            (
                [Id] ASC
            )
        ) ON [PRIMARY];
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.table_types WHERE name = 'RepoDB_TVP_int')
    BEGIN
        CREATE TYPE RepoDB_TVP_int AS TABLE (Value INT NOT NULL PRIMARY KEY WITH (IGNORE_DUP_KEY = ON));
    END

    IF NOT EXISTS (SELECT 1 FROM sys.table_types WHERE name = 'RepoDB_TVP_int_NULL')
    BEGIN
        CREATE TYPE RepoDB_TVP_int_NULL AS TABLE (Value INT);
    END

    IF NOT EXISTS (SELECT 1 FROM sys.table_types WHERE name = 'RepoDB_TVP_bigint')
    BEGIN
        CREATE TYPE RepoDB_TVP_bigint AS TABLE (Value bigint NOT NULL PRIMARY KEY WITH (IGNORE_DUP_KEY = ON));
    END

    IF NOT EXISTS (SELECT 1 FROM sys.table_types WHERE name = 'RepoDB_TVP_bigint_NULL')
    BEGIN
        CREATE TYPE RepoDB_TVP_bigint_NULL AS TABLE (Value bigint);
    END

    IF NOT EXISTS (SELECT 1 FROM sys.table_types WHERE name = 'RepoDB_TVP_tinyint')
    BEGIN
        CREATE TYPE RepoDB_TVP_tinyint AS TABLE (Value tinyint NOT NULL PRIMARY KEY WITH (IGNORE_DUP_KEY = ON));
    END

    IF NOT EXISTS (SELECT 1 FROM sys.table_types WHERE name = 'RepoDB_TVP_tinyint_NULL')
    BEGIN
        CREATE TYPE RepoDB_TVP_tinyint_NULL AS TABLE (Value tinyint);
    END

    IF NOT EXISTS (SELECT 1 FROM sys.table_types WHERE name = 'RepoDB_TVP_varchar')
    BEGIN
        CREATE TYPE RepoDB_TVP_varchar AS TABLE (Value varchar(50) NOT NULL PRIMARY KEY WITH (IGNORE_DUP_KEY = ON));
    END

    IF NOT EXISTS (SELECT 1 FROM sys.table_types WHERE name = 'RepoDB_TVP_varchar_NULL')
    BEGIN
        CREATE TYPE RepoDB_TVP_varchar_NULL AS TABLE (Value varchar(50));
    END

    IF NOT EXISTS (SELECT 1 FROM sys.table_types WHERE name = 'RepoDB_TVP_uuid')
    BEGIN
        CREATE TYPE RepoDB_TVP_uuid AS TABLE (Value uniqueidentifier NOT NULL PRIMARY KEY WITH (IGNORE_DUP_KEY = ON));
    END

    IF NOT EXISTS (SELECT 1 FROM sys.table_types WHERE name = 'RepoDB_TVP_uuid_NULL')
    BEGIN
        CREATE TYPE RepoDB_TVP_uuid_NULL AS TABLE (Value uniqueidentifier);
    END
    ";
        using var connection = new SqlConnection(ConnectionString).EnsureOpen();
        connection.ExecuteNonQuery(commandText);
    }

    private static void CreateNonIdentityCompleteTable()
    {
        var commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'NonIdentityCompleteTable'))
                BEGIN
                    CREATE TABLE [dbo].[NonIdentityCompleteTable]
                    (
                        [Id] INT NOT NULL,
                        [SessionId] UNIQUEIDENTIFIER NOT NULL,
                        [ColumnBigInt] BIGINT NULL,
                        [ColumnBinary] BINARY(4000) NULL,
                        [ColumnBit] BIT NULL,
                        [ColumnChar] CHAR(1) NULL,
                        [ColumnDate] DATE NULL,
                        [ColumnDateTime] DATETIME NULL,
                        [ColumnDateTime2] DATETIME2(7) NULL,
                        [ColumnDateTimeOffset] DATETIMEOFFSET(7) NULL,
                        [ColumnDecimal] DECIMAL(18, 2) NULL,
                        [ColumnFloat] FLOAT NULL,
                        [ColumnGeography] GEOGRAPHY NULL,
                        [ColumnGeometry] GEOMETRY NULL,
                        [ColumnHierarchyId] HIERARCHYID NULL,
                        [ColumnImage] IMAGE NULL,
                        [ColumnInt] INT NULL,
                        [ColumnMoney] MONEY NULL,
                        [ColumnNChar] NCHAR(1) NULL,
                        [ColumnNText] NTEXT NULL,
                        [ColumnNumeric] NUMERIC(18, 2) NULL,
                        [ColumnNVarChar] NVARCHAR(MAX) NULL,
                        [ColumnReal] REAL NULL,
                        [ColumnSmallDateTime] SMALLDATETIME NULL,
                        [ColumnSmallInt] SMALLINT NULL,
                        [ColumnSmallMoney] SMALLMONEY NULL,
                        [ColumnSqlVariant] SQL_VARIANT NULL,
                        [ColumnText] TEXT NULL,
                        [ColumnTime] TIME(7) NULL,
                        [ColumnTimestamp] TIMESTAMP NULL,
                        [ColumnTinyInt] TINYINT NULL,
                        [ColumnUniqueIdentifier] UNIQUEIDENTIFIER NULL,
                        [ColumnVarBinary] VARBINARY(MAX) NULL,
                        [ColumnVarChar] VARCHAR(MAX) NULL,
                        [ColumnXml] XML NULL,
                        CONSTRAINT [NonIdentityCompleteTable_Id] PRIMARY KEY
                        (
                            [Id] ASC
                        )
                    ) ON [PRIMARY];
                END";
        using var connection = new SqlConnection(ConnectionString).EnsureOpen();
        connection.ExecuteNonQuery(commandText);
    }

    private static void CreateMultiKeyTable()
    {
        var commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'MultiKeyTable'))
                BEGIN
                    CREATE TABLE [dbo].[MultiKeyTable]
                    (
                        [Pk1] INT NOT NULL,
                        [Pk2] VARCHAR(16) NOT NULL,
                        [ColumnVarChar] VARCHAR(MAX) NULL,
                        CONSTRAINT [PK_MultiKeyTable] PRIMARY KEY
                        (
                            [Pk1] ASC,
                            [Pk2] ASC
                        )
                    ) ON [PRIMARY];
                END";
        using var connection = new SqlConnection(ConnectionString).EnsureOpen();
        connection.ExecuteNonQuery(commandText);
    }

    #endregion

    #region CompleteTable

    public static IEnumerable<CompleteTable> CreateCompleteTables(int count)
    {
        using var connection = new SqlConnection(ConnectionString);
        var tables = Helper.CreateCompleteTables(count);
        connection.InsertAll(tables);
        return tables;
    }

    #endregion

    #region NonIdentityCompleteTable

    public static IEnumerable<NonIdentityCompleteTable> CreateNonIdentityCompleteTables(int count)
    {
        using var connection = new SqlConnection(ConnectionString);
        var tables = Helper.CreateNonIdentityCompleteTables(count);
        connection.InsertAll(tables);
        return tables;
    }

    #endregion

    #region MultiKeyTable
    public static IEnumerable<MultiKeyTable> CreateMultiKeyTables(int count)
    {
        using var connection = new SqlConnection(ConnectionString);
        var tables = Helper.CreateMultiKeyTables(count);
        connection.InsertAll(tables);
        return tables;
    }
    #endregion
}
