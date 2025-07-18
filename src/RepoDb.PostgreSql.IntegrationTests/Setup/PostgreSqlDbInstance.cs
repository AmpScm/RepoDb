using System.Data.Common;
using Npgsql;
using RepoDb.TestCore;

namespace RepoDb.PostgreSql.IntegrationTests.Setup;

public class PostgreSqlDbInstance : DbInstance<NpgsqlConnection>
{
    static PostgreSqlDbInstance()
    {
        GlobalConfiguration.Setup(GlobalConfiguration.Options).UsePostgreSql();
    }

    public PostgreSqlDbInstance()
    {
        // Master connection
        AdminConnectionString =
            Environment.GetEnvironmentVariable("REPODB_POSTGRESQL_CONSTR_POSTGRESDB")
            ?? "Server=127.0.0.1;Port=45432;Database=postgres;User Id=postgres;Password=ddd53e85-b15e-4da8-91e5-a7d3b00a0ab2;"; // Docker test configuration

        // RepoDb connection
        ConnectionString =
            Environment.GetEnvironmentVariable("REPODB_POSTGRESQL_CONSTR")
            ?? new NpgsqlConnectionStringBuilder(AdminConnectionString) { Database = DatabaseName }.ToString();
    }

    protected override async Task CreateUserDatabase(DbConnection sql)
    {
        int recordCount = await sql.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM pg_database WHERE datname = '{DatabaseName}';");
        if (recordCount <= 0)
        {
            await sql.ExecuteNonQueryAsync($@"CREATE DATABASE ""{DatabaseName}""
                        WITH OWNER = ""postgres""
                        ENCODING = ""UTF8""
                        CONNECTION LIMIT = -1;");
        }
    }
}
