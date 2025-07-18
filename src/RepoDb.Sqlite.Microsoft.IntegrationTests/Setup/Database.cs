using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Models;

namespace RepoDb.Sqlite.Microsoft.IntegrationTests.Setup;

public static class Database
{
    private static readonly SqliteDbInstance Instance = new();

    public static string ConnectionString => Instance.ConnectionString;


    public static void Initialize()
    {
        Instance.ClassInitializeAsync(null).GetAwaiter().GetResult();

        // Initialize SqLite
        GlobalConfiguration.Setup(new())
            .UseSqlite();

        // Create tables
        CreateMdsTables();
    }

    public static void Cleanup()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.DeleteAll<MdsCompleteTable>();
        connection.DeleteAll<MdsNonIdentityCompleteTable>();
    }

    #region MdsCompleteTable

    public static IEnumerable<MdsCompleteTable> CreateMdsCompleteTables(int count,
        SqliteConnection connection = null)
    {
        var hasConnection = (connection != null);
        if (hasConnection == false)
        {
            connection = new SqliteConnection(ConnectionString);
        }
        try
        {
            var tables = Helper.CreateMdsCompleteTables(count);
            CreateMdsCompleteTable(connection);
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

    #region MdsNonIdentityCompleteTable

    public static IEnumerable<MdsNonIdentityCompleteTable> CreateMdsNonIdentityCompleteTables(int count,
        SqliteConnection connection = null)
    {
        var hasConnection = (connection != null);
        if (hasConnection == false)
        {
            connection = new SqliteConnection(ConnectionString);
        }
        try
        {
            var tables = Helper.CreateMdsNonIdentityCompleteTables(count);
            CreateMdsNonIdentityCompleteTable(connection);
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

    #region CreateMdsTables

    public static void CreateMdsTables(SqliteConnection connection = null)
    {
        CreateMdsCompleteTable(connection);
        CreateMdsNonIdentityCompleteTable(connection);
    }

    public static void CreateMdsCompleteTable(SqliteConnection connection = null)
    {
        var hasConnection = (connection != null);
        if (hasConnection == false)
        {
            connection = new SqliteConnection(ConnectionString);
        }
        try
        {
            /*
             * Stated here: If the type if 'INTEGER PRIMARY KEY', it is automatically an identity table.
             * No need to explicity specify the 'AUTOINCREMENT' keyword to avoid extra CPU and memory space.
             * Link: https://sqlite.org/autoinc.html
             */
            connection.ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS [MdsCompleteTable]
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

    public static void CreateMdsNonIdentityCompleteTable(SqliteConnection connection = null)
    {
        var hasConnection = (connection != null);
        if (hasConnection == false)
        {
            connection = new SqliteConnection(ConnectionString);
        }
        try
        {
            connection.ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS [MdsNonIdentityCompleteTable]
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
        using var db = new SqliteConnection(GetConnectionString(testContext));
        db.EnsureOpen();
    }

    internal static string GetConnectionString(TestContext testContext)
    {
        return "Datasource=" + GetDbPath(testContext).Replace(Path.DirectorySeparatorChar, '/');
    }

    #endregion
}
