using Oracle.ManagedDataAccess.Client;

namespace RepoDb.Oracle.IntegrationTests.Setup;

public static class Database
{
    private static readonly OracleDbInstance Instance = new();
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
            .UseOracle();

        // Create tables
        CreateTables();
    }

    public static void Cleanup()
    {
        using (var connection = new OracleConnection(ConnectionString))
        {
            //connection.Truncate<CompleteTable>();
            //connection.Truncate<NonIdentityCompleteTable>();
            //connection.Truncate<MultiKeyTable>();
        }
    }

    #endregion

    #region CreateTables

    private static void CreateTables()
    {
        CreateCompleteTable();
        //CreateNonIdentityCompleteTable();
        //CreateMultiKeyTable();
    }

    private static void CreateCompleteTable()
    {
        var commandTexts = new string[] {@"
            BEGIN
    EXECUTE IMMEDIATE '
        CREATE TABLE CompleteTable (
            Id NUMBER NOT NULL,
            SessionId RAW(16) NOT NULL,
            ColumnBigInt NUMBER(19),
            ColumnBinary RAW(2000),
            ColumnBit NUMBER(1),
            ColumnChar CHAR(1),
            ColumnDate DATE,
            ColumnDateTime TIMESTAMP,
            ColumnDateTime2 TIMESTAMP(7),
            ColumnDateTimeOffset TIMESTAMP(7) WITH TIME ZONE,
            ColumnDecimal NUMBER(18,2),
            ColumnFloat BINARY_DOUBLE,
            ColumnHierarchyId VARCHAR2(255),
            ColumnImage BLOB,
            ColumnInt NUMBER(10),
            ColumnMoney NUMBER(19,4),
            ColumnNChar NCHAR(1),
            ColumnNText NCLOB,
            ColumnNumeric NUMBER(18,2),
            ColumnNVarChar NVARCHAR2(2000),
            ColumnReal BINARY_FLOAT,
            ColumnSmallDateTime TIMESTAMP(0),
            ColumnSmallInt NUMBER(5),
            ColumnSmallMoney NUMBER(10,4),
            ColumnSqlVariant VARCHAR2(2000),
            ColumnText CLOB,
            ColumnTime INTERVAL DAY(0) TO SECOND(7),
            ColumnTimestamp TIMESTAMP,
            ColumnTinyInt NUMBER(3),
            ColumnUniqueIdentifier RAW(16),
            ColumnVarBinary BLOB,
            ColumnVarChar VARCHAR2(2000),
            ColumnXml XMLTYPE,
            CONSTRAINT CompleteTable_Id PRIMARY KEY (Id)
        )
    ';
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -955 THEN -- table exists
            RAISE;
        END IF;
END;",
@"
BEGIN
    EXECUTE IMMEDIATE 'CREATE SEQUENCE CompleteTable_Id_Seq START WITH 1 INCREMENT BY 1';
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -955 THEN -- sequence exists
            RAISE;
        END IF;
END;
",
@"
BEGIN
    EXECUTE IMMEDIATE '
        CREATE OR REPLACE TRIGGER CompleteTable_BIR
        BEFORE INSERT ON CompleteTable
        FOR EACH ROW
        WHEN (NEW.Id IS NULL)
        BEGIN
            SELECT CompleteTable_Id_Seq.NEXTVAL INTO :NEW.Id FROM dual;
        END;
    ';
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -4080 THEN -- trigger exists
            RAISE;
        END IF;
END;" };
        using (var connection = new OracleConnection(ConnectionString).EnsureOpen())
        {
            foreach (var commandText in commandTexts)
                connection.ExecuteNonQuery(commandText);
        }
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
                        [ColumnBinary] BINARY(2000) NULL,
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
        using (var connection = new OracleConnection(ConnectionString).EnsureOpen())
        {
            connection.ExecuteNonQuery(commandText);
        }
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
        using (var connection = new OracleConnection(ConnectionString).EnsureOpen())
        {
            connection.ExecuteNonQuery(commandText);
        }
    }

    #endregion

    #region CompleteTable

    //public static IEnumerable<CompleteTable> CreateCompleteTables(int count)
    //{
    //    using (var connection = new OracleConnection(ConnectionString))
    //    {
    //        var tables = Helper.CreateCompleteTables(count);
    //        connection.InsertAll(tables);
    //        return tables;
    //    }
    //}

    #endregion

    #region NonIdentityCompleteTable

    //public static IEnumerable<NonIdentityCompleteTable> CreateNonIdentityCompleteTables(int count)
    //{
    //    using (var connection = new OracleConnection(ConnectionString))
    //    {
    //        var tables = Helper.CreateNonIdentityCompleteTables(count);
    //        connection.InsertAll(tables);
    //        return tables;
    //    }
    //}

    #endregion

    //#region MultiKeyTable
    //public static IEnumerable<MultiKeyTable> CreateMultiKeyTables(int count)
    //{
    //    using (var connection = new OracleConnection(ConnectionString))
    //    {
    //        var tables = Helper.CreateMultiKeyTables(count);
    //        connection.InsertAll(tables);
    //        return tables;
    //    }
    //}
    //#endregion
}
