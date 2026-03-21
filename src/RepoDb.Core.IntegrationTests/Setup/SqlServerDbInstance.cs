using System.Data.Common;
using Microsoft.Data.SqlClient;
using RepoDb.TestCore;

namespace RepoDb.IntegrationTests.Setup;

public class SqlServerDbInstance : DbInstance<SqlConnection>
{
    static SqlServerDbInstance()
    {
        GlobalConfiguration.Setup(GlobalConfiguration.Options).UseSqlServer();

        TypeMapper.Add(typeof(DateTime), System.Data.DbType.DateTime2, true);
    }

    public SqlServerDbInstance()
    {
        // System connection (sa on master)
        AdminConnectionString =
            Environment.GetEnvironmentVariable("REPODB_SQLSERVER_CONSTR_MASTER")
            ?? @"Server=tcp:127.0.0.1,41433;Database=master;User ID=sa;Password=ddd53e85-b15e-4da8-91e5-a7d3b00a0ab2;TrustServerCertificate=True;"; // Docker Test Configuration

        // Owner connection (repodb_core_owner with db_owner privileges)
        ConnectionString =
            Environment.GetEnvironmentVariable("REPODB_SQLSERVER_CONSTR_REPODB")
            ?? new SqlConnectionStringBuilder(AdminConnectionString)
            {
                InitialCatalog = DatabaseName,
                UserID = "repodb_core_owner",
                Password = "8A5F2E1B-42C3-4D9E-B156-7C3F9D0A2E4B"
            }.ToString();

        // Limited connection (repodb_core_user with minimal privileges)
        LimitedConnectionString =
            Environment.GetEnvironmentVariable("REPODB_SQLSERVER_CONSTR_REPODB_LIMITED")
            ?? new SqlConnectionStringBuilder(AdminConnectionString)
            {
                InitialCatalog = DatabaseName,
                UserID = "repodb_core_user",
                Password = "0608B012-05D2-4023-A451"
            }.ToString();
    }

    protected override async Task CreateUserDatabase(DbConnection sql)
    {
        // Create database if it doesn't exist
        await sql.ExecuteNonQueryAsync($@"IF (NOT EXISTS(SELECT * FROM sys.databases WHERE name = '{DatabaseName}'))
                BEGIN
                    CREATE DATABASE [{DatabaseName}];
                END");

        // Create owner user login if it doesn't exist
        await sql.ExecuteNonQueryAsync(@"
        IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'repodb_core_owner')
        BEGIN
            CREATE LOGIN [repodb_core_owner] WITH PASSWORD = '8A5F2E1B-42C3-4D9E-B156-7C3F9D0A2E4B';
        END");

        // Create limited user login if it doesn't exist
        await sql.ExecuteNonQueryAsync(@"
        IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'repodb_core_user')
        BEGIN
            CREATE LOGIN [repodb_core_user] WITH PASSWORD = '0608B012-05D2-4023-A451';
        END");

        await sql.ChangeDatabaseAsync(DatabaseName);

        // Create owner user in the test database with db_owner-like permissions
        await sql.ExecuteNonQueryAsync($@"
        IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'repodb_core_owner')
        BEGIN
            CREATE USER [repodb_core_owner] FOR LOGIN [repodb_core_owner];
            -- Add user to db_owner role for setup operations
            ALTER ROLE [db_owner] ADD MEMBER [repodb_core_owner];
        END");

        // Create limited user in the test database with minimal permissions
        await sql.ExecuteNonQueryAsync($@"
        IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'repodb_core_user')
        BEGIN
            CREATE USER [repodb_core_user] FOR LOGIN [repodb_core_user];
            -- Grant permission to connect to database
            GRANT CONNECT TO [repodb_core_user];
            -- Add user to db_datawriter and db_datareader roles for basic data operations only
            ALTER ROLE [db_datawriter] ADD MEMBER [repodb_core_user];
            ALTER ROLE [db_datareader] ADD MEMBER [repodb_core_user];
        END");

        Database.Initialize();
    }
}
