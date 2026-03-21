using System.Data.Common;
using MySql.Data.MySqlClient;
using RepoDb.TestCore;

namespace RepoDb.MySql.IntegrationTests.Setup;

public class MysqlDbInstance : DbInstance<MySqlConnection>
{
    static MysqlDbInstance()
    {
        GlobalConfiguration.Setup().UseMySql();
    }

    public MysqlDbInstance()
    {
        AdminConnectionString =
            Environment.GetEnvironmentVariable("REPODB_MYSQL_CONSTR_SYS")
            ?? @"Server=127.0.0.1;Port=43306;Database=sys;User ID=root;Password=ddd53e85-b15e-4da8-91e5-a7d3b00a0ab2;";

        // Owner connection (repodb_mysql_owner with full database privileges)
        ConnectionString =
            Environment.GetEnvironmentVariable("REPODB_MYSQL_CONSTR_REPODB")
            ?? new MySqlConnectionStringBuilder(AdminConnectionString)
            {
                Database = DatabaseName,
                UserID = "repodb_mysql_owner",
                Password = "8A5F2E1B-42C3-4D9E-B156-7C3F9D0A2E4B"
            }.ToString();

        // Limited connection (repodb_mysql_user with minimal privileges)
        LimitedConnectionString =
            Environment.GetEnvironmentVariable("REPODB_MYSQL_CONSTR_REPODB_LIMITED")
            ?? new MySqlConnectionStringBuilder(AdminConnectionString)
            {
                Database = DatabaseName,
                UserID = "repodb_mysql_user",
                Password = "0608B012-05D2-4023-A451"
            }.ToString();
    }

    protected override async Task CreateUserDatabase(DbConnection sql)
    {
        // Create database if it doesn't exist
        await sql.ExecuteNonQueryAsync($@"CREATE DATABASE IF NOT EXISTS `{DatabaseName}`;");

        // Create owner user if it doesn't exist
        await sql.ExecuteNonQueryAsync($@"CREATE USER IF NOT EXISTS 'repodb_mysql_owner'@'%' IDENTIFIED BY '8A5F2E1B-42C3-4D9E-B156-7C3F9D0A2E4B';");

        // Create limited user if it doesn't exist
        await sql.ExecuteNonQueryAsync($@"CREATE USER IF NOT EXISTS 'repodb_mysql_user'@'%' IDENTIFIED BY '0608B012-05D2-4023-A451';");

        // Grant owner permissions: all privileges on database
        await sql.ExecuteNonQueryAsync($@"
        GRANT ALL PRIVILEGES ON `{DatabaseName}`.* TO 'repodb_mysql_owner'@'%';
        FLUSH PRIVILEGES;");

        // Grant limited user permissions: CRUD operations only (no DDL)
        await sql.ExecuteNonQueryAsync($@"
        GRANT
            SELECT,
            INSERT,
            UPDATE,
            DELETE
        ON `{DatabaseName}`.* TO 'repodb_mysql_user'@'%';
        FLUSH PRIVILEGES;");

        Database.Initialize();
    }
}
