using Npgsql;

namespace RepoDb.IntegrationTests.Setup;

/// <summary>
/// A class used as a startup setup for for RepoDb test database.
/// </summary>
public static class Database
{
    private static readonly PostgreSqlDbInstance instance = new();

    /// <summary>
    /// Initialize the creation of the database.
    /// </summary>
    public static void Initialize()
    {
        instance.ClassInitializeAsync(null).GetAwaiter().GetResult();

        // Initialize PostgreSql
        GlobalConfiguration.Setup(new());

        // Create tables
        CreateTables();
    }

    /// <summary>
    /// Gets or sets the connection string to be used for Postgres database.
    /// </summary>
    public static string ConnectionStringForPostgres => instance.AdminConnectionString;

    /// <summary>
    /// Gets or sets the connection string to be used.
    /// </summary>
    public static string ConnectionStringForRepoDb => instance.ConnectionString;

    #region Methods

    /// <summary>
    /// Create the necessary tables for testing.
    /// </summary>
    public static void CreateTables()
    {
        CreateBulkOperationIdentityTable();
        CreateEnumTable();
    }

    /// <summary>
    /// Clean up all the table.
    /// </summary>
    public static void Cleanup()
    {
        using (var connection = new NpgsqlConnection(ConnectionStringForRepoDb))
        {
            connection.Truncate("BulkOperationIdentityTable");
            connection.Truncate("EnumTable");
        }
    }

    #endregion

    #region BulkOperationIdentityTable

    /// <summary>
    /// Creates an identity table that has some important fields. All fields are nullable.
    /// </summary>
    public static void CreateBulkOperationIdentityTable()
    {
        var commandText = @"CREATE TABLE IF NOT EXISTS public.""BulkOperationIdentityTable""
                (
                        ""Id"" bigint GENERATED ALWAYS AS IDENTITY,
                        ""ColumnChar"" ""char"",
                        ""ColumnBigInt"" bigint,
                        ""ColumnBit"" bit(1),
                        ""ColumnBoolean"" boolean,
                        ""ColumnDate"" date,
                        ""ColumnInteger"" integer,
                        ""ColumnMoney"" money,
                        ""ColumnNumeric"" numeric,
                        ""ColumnReal"" real,
                        ""ColumnSerial"" integer,
                        ""ColumnSmallInt"" smallint,
                        ""ColumnSmallSerial"" smallint,
                        ""ColumnText"" text COLLATE pg_catalog.""default"",
                        ""ColumnTimeWithTimeZone"" time with time zone,
                        ""ColumnTimeWithoutTimeZone"" time without time zone,
                        ""ColumnTimestampWithTimeZone"" timestamp with time zone,
                        ""ColumnTimestampWithoutTimeZone"" timestamp without time zone,
                        CONSTRAINT ""BulkOperationIdentityTable_PrimaryKey"" PRIMARY KEY (""Id"")
                    )

                    TABLESPACE pg_default;

                    ALTER TABLE public.""BulkOperationIdentityTable""
                        OWNER to postgres;";
        using (var connection = new NpgsqlConnection(ConnectionStringForRepoDb))
        {
            connection.ExecuteNonQuery(commandText);
        }
    }

    #endregion

    #region EnumTable

    private static void CreateEnumTable()
    {
        using (var connection = new NpgsqlConnection(ConnectionStringForRepoDb))
        {
            connection.ExecuteNonQuery(@"
                    DO $$
                    BEGIN
                        IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'hand') THEN
                            CREATE TYPE hand AS ENUM ('unidentified', 'left', 'right');
                        END IF;
                    END
                    $$;

                    CREATE TABLE IF NOT EXISTS public.""EnumTable""
                    (
                        ""Id"" bigint GENERATED ALWAYS AS IDENTITY,
                        ""ColumnEnumText"" text null COLLATE pg_catalog.""default"",
                        ""ColumnEnumInt"" integer null,
                        ""ColumnEnumHand"" hand null,
                        CONSTRAINT ""EnumTable_PrimaryKey"" PRIMARY KEY (""Id"")
                    );");
            connection.ReloadTypes();
        }
    }

    #endregion
}
