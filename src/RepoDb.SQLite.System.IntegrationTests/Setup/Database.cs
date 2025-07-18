using System.Data.SQLite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SQLite.System.IntegrationTests.Models;

namespace RepoDb.SQLite.System.IntegrationTests.Setup;

public static class Database
{
    private static readonly SQLiteDbInstance Instance = new();

    public static string ConnectionString => Instance.ConnectionString;

    public static void Initialize()
    {
        Instance.ClassInitializeAsync(null).GetAwaiter().GetResult();

        // Initialize SqLite
        GlobalConfiguration
            .Setup()
            .UseSQLite();


        // Create tables
        CreateSdsTables();
    }

    public static void Cleanup()
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.DeleteAll<SdsCompleteTable>();
            connection.DeleteAll<SdsNonIdentityCompleteTable>();
        }
    }

    #region SdsCompleteTable

    public static IEnumerable<SdsCompleteTable> CreateSdsCompleteTables(int count,
        SQLiteConnection connection = null)
    {
        var hasConnection = (connection != null);
        if (hasConnection == false)
        {
            connection = new SQLiteConnection(ConnectionString);
        }
        try
        {
            var tables = Helper.CreateSdsCompleteTables(count);
            CreateSdsCompleteTable(connection);
            connection.InsertAll(tables);
            return tables;
        }
        finally
        {
            if (hasConnection == false)
            {
                connection.Dispose();
            }
        }
    }

    #endregion

    #region SdsNonIdentityCompleteTable

    public static IEnumerable<SdsNonIdentityCompleteTable> CreateSdsNonIdentityCompleteTables(int count,
        SQLiteConnection connection = null)
    {
        var hasConnection = (connection != null);
        if (hasConnection == false)
        {
            connection = new SQLiteConnection(ConnectionString);
        }
        try
        {
            var tables = Helper.CreateSdsNonIdentityCompleteTables(count);
            CreateSdsNonIdentityCompleteTable(connection);
            connection.InsertAll(tables);
            return tables;
        }
        finally
        {
            if (hasConnection == false)
            {
                connection.Dispose();
            }
        }
    }

    #endregion

    #region SdsCreateTables

    public static void CreateSdsTables(SQLiteConnection connection = null)
    {
        CreateSdsCompleteTable(connection);
        CreateSdsNonIdentityCompleteTable(connection);
    }

    public static void CreateSdsCompleteTable(SQLiteConnection connection = null)
    {
        var hasConnection = (connection != null);
        if (hasConnection == false)
        {
            connection = new SQLiteConnection(ConnectionString);
        }
        try
        {
            /*
             * Stated here: If the type if 'INTEGER PRIMARY KEY', it is automatically an identity table.
             * No need to explicity specify the 'AUTOINCREMENT' keyword to avoid extra CPU and memory space.
             * Link: https://sqlite.org/autoinc.html
             */
            connection.ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS [SdsCompleteTable]
                    (
                        Id INTEGER PRIMARY KEY
                        , ColumnBigInt BIGINT
                        , ColumnBlob BLOB
                        , ColumnBoolean BOOLEAN
                        , ColumnChar CHAR
                        , ColumnDate DATE
                        , ColumnDateTime DATETIME
                        , ColumnDecimal DECIMAL
                        , ColumnDouble DOUBLE
                        , ColumnInteger INTEGER
                        , ColumnInt INT
                        , ColumnNone NONE
                        , ColumnNumeric NUMERIC
                        , ColumnReal REAL
                        , ColumnString STRING
                        , ColumnText TEXT
                        , ColumnTime TIME
                        , ColumnVarChar VARCHAR
                    );");
        }
        finally
        {
            if (hasConnection == false)
            {
                connection.Dispose();
            }
        }
    }

    public static void CreateSdsNonIdentityCompleteTable(SQLiteConnection connection = null)
    {
        var hasConnection = (connection != null);
        if (hasConnection == false)
        {
            connection = new SQLiteConnection(ConnectionString);
        }
        try
        {
            connection.ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS [SdsNonIdentityCompleteTable]
                    (
                        Id VARCHAR PRIMARY KEY
                        , ColumnBigInt BIGINT
                        , ColumnBlob BLOB
                        , ColumnBoolean BOOLEAN
                        , ColumnChar CHAR
                        , ColumnDate DATE
                        , ColumnDateTime DATETIME
                        , ColumnDecimal DECIMAL
                        , ColumnDouble DOUBLE
                        , ColumnInteger INTEGER
                        , ColumnInt INT
                        , ColumnNone NONE
                        , ColumnNumeric NUMERIC
                        , ColumnReal REAL
                        , ColumnString STRING
                        , ColumnText TEXT
                        , ColumnTime TIME
                        , ColumnVarChar VARCHAR
                    );");
        }
        finally
        {
            if (hasConnection == false)
            {
                connection.Dispose();
            }
        }
    }

    private static string GetDbPath(TestContext tc)
    {
        return Path.Combine(tc.TestRunDirectory, "sqlite.db");
    }

    internal static void Initialize(TestContext testContext)
    {
        Initialize();
        //throw new NotImplementedException();
        using (var db = new SQLiteConnection(GetConnectionString(testContext)))
        {
            db.EnsureOpen();
        }
    }

    internal static string GetConnectionString(TestContext testContext)
    {
        return "Datasource=" + GetDbPath(testContext).Replace(Path.DirectorySeparatorChar, '/');
    }

    #endregion
}
