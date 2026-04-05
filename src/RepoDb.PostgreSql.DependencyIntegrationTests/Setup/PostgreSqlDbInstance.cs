using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Npgsql;
using RepoDb.TestCore;

namespace RepoDb.DependencyIntegrationTests.Setup;

public class PostgreSqlDbInstance : DbInstance<NpgsqlConnection>
{
    static PostgreSqlDbInstance()
    {
        GlobalConfiguration.Setup(GlobalConfiguration.Options).UsePostgreSql();
    }

    [SetsRequiredMembers]
    public PostgreSqlDbInstance()
    {
        // Master connection
        AdminConnectionString =
            Environment.GetEnvironmentVariable("REPODB_POSTGRESQL_CONSTR_POSTGRESDB")
            ?? "Server=127.0.0.1;Port=45432;Database=postgres;User Id=postgres;Password=ddd53e85-b15e-4da8-91e5-a7d3b00a0ab2;"; // Docker test configuration

        // Owner connection (repodb_dependencies_owner with owner privileges)
        ConnectionString =
            Environment.GetEnvironmentVariable("REPODB_POSTGRESQL_CONSTR")
            ?? new NpgsqlConnectionStringBuilder(AdminConnectionString)
            {
                Database = DatabaseName,
                Username = "repodb_dependencies_owner",
                Password = "8A5F2E1B-42C3-4D9E-B156-7C3F9D0A2E4B"
            }.ToString();

        // Limited connection (repodb_dependencies_user with minimal privileges)
        LimitedConnectionString =
            Environment.GetEnvironmentVariable("REPODB_POSTGRESQL_CONSTR_LIMITED")
            ?? new NpgsqlConnectionStringBuilder(AdminConnectionString)
            {
                Database = DatabaseName,
                Username = "repodb_dependencies_user",
                Password = "0608B012-05D2-4023-A451"
            }.ToString();
    }

    protected override async Task CreateUserDatabase(DbConnection sql)
    {
        Console.WriteLine(DatabaseName);
        var recordCount = await sql.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM pg_database WHERE datname = '{DatabaseName}';");
        if (recordCount <= 0)
        {
            await sql.ExecuteNonQueryAsync($@"CREATE DATABASE ""{DatabaseName}""
                        WITH OWNER = ""postgres""
                        ENCODING = ""UTF8""
                        CONNECTION LIMIT = -1;");
        }

        // Create owner user if it doesn't exist
        await sql.ExecuteNonQueryAsync($@"
        DO $$
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM pg_user WHERE usename = 'repodb_dependencies_owner') THEN
                CREATE USER ""repodb_dependencies_owner"" WITH PASSWORD '8A5F2E1B-42C3-4D9E-B156-7C3F9D0A2E4B' LOGIN;
            END IF;
        END
        $$;");

        // Create limited user if it doesn't exist
        await sql.ExecuteNonQueryAsync($@"
        DO $$
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM pg_user WHERE usename = 'repodb_dependencies_user') THEN
                CREATE USER ""repodb_dependencies_user"" WITH PASSWORD '0608B012-05D2-4023-A451' LOGIN;
            END IF;
        END
        $$;");

        // Grant database-level permissions (from master database connection)
        await sql.ExecuteNonQueryAsync($@"
        GRANT CONNECT ON DATABASE ""{DatabaseName}"" TO ""repodb_dependencies_owner"";
        GRANT CONNECT ON DATABASE ""{DatabaseName}"" TO ""repodb_dependencies_user"";");

        await sql.ChangeDatabaseAsync(DatabaseName);

        // Grant schema permissions in the context of the new database
        await sql.ExecuteNonQueryAsync($@"
        GRANT ALL PRIVILEGES ON SCHEMA public TO ""repodb_dependencies_owner"";
        ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON TABLES TO ""repodb_dependencies_owner"";
        ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON SEQUENCES TO ""repodb_dependencies_owner"";
        ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT EXECUTE ON FUNCTIONS TO ""repodb_dependencies_owner"";");

        // Grant limited user permissions in the context of the new database
        await sql.ExecuteNonQueryAsync($@"
        GRANT USAGE ON SCHEMA public TO ""repodb_dependencies_user"";
        ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO ""repodb_dependencies_user"";
        ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT USAGE ON SEQUENCES TO ""repodb_dependencies_user"";
        GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO ""repodb_dependencies_user"";
        GRANT USAGE ON ALL SEQUENCES IN SCHEMA public TO ""repodb_dependencies_user"";"
        );
    }
}
