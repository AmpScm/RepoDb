using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Oracle.ManagedDataAccess.Client;
using RepoDb.TestCore;

namespace RepoDb.Oracle.IntegrationTests.Setup;

public class OracleDbInstance : DbInstance<OracleConnection>
{
    static OracleDbInstance()
    {
        Environment.SetEnvironmentVariable("NLS_LANGUAGE", "AMERICAN.AL32UTF8");

        GlobalConfiguration.Setup().UseOracle();

        var csb = new OracleConnectionStringBuilder(
            Environment.GetEnvironmentVariable("REPODB_ORACLE_CONSTR_MASTER")
            ?? @"Data Source=127.0.0.1:41521/APPDB;User Id=system;Password=ddd53e85-b15e-4da8;"); // Docker Test Configuration

        csb.DataSource = csb.DataSource.IndexOf('/') is { } ix && ix > 0 ? csb.DataSource.Substring(0, ix) : csb.DataSource;

        // Create APPDB service if not exists
        try
        {
            using var connection = new OracleConnection(csb.ToString()).EnsureOpen();
            connection.ExecuteNonQuery($@"
            DECLARE
              v_count INTEGER;
            BEGIN
              SELECT COUNT(*) INTO v_count
              FROM dba_services
              WHERE name = 'APPDB';

              IF v_count = 0 THEN
                DBMS_SERVICE.CREATE_SERVICE(
                  service_name => 'APPDB',
                  network_name => 'APPDB'
                );
                DBMS_SERVICE.START_SERVICE('APPDB');
              END IF;
            END;
        ");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating APPDB service: {ex.Message}");
        }
    }

    [SetsRequiredMembers]
    public OracleDbInstance()
    {
        // Master connection
        AdminConnectionString =
            Environment.GetEnvironmentVariable("REPODB_ORACLE_CONSTR_MASTER")
            ?? @"Data Source=127.0.0.1:41521/APPDB;User Id=system;Password=ddd53e85-b15e-4da8;"; // Docker Test Configuration

        // Owner connection (repodb_owner with full privileges)
        ConnectionString = new OracleConnectionStringBuilder(AdminConnectionString)
            {
                UserID = "repodb_owner",
                Password = "8A5F2E1B-42C3-4D9E"
            }.ToString();

        // Limited connection (repodb_user with minimal privileges)
        LimitedConnectionString = new OracleConnectionStringBuilder(AdminConnectionString)
            {
                UserID = "repodb_user",
                Password = "0608B012-05D2-4023-A451"
            }.ToString();
    }

    protected override async Task CreateUserDatabase(DbConnection sql)
    {
        // Create owner user if it doesn't exist
        await sql.ExecuteNonQueryAsync($@"
        DECLARE
          v_count INTEGER;
        BEGIN
          SELECT COUNT(*) INTO v_count FROM all_users WHERE username = 'REPODB_OWNER';
          IF v_count = 0 THEN
            EXECUTE IMMEDIATE 'CREATE USER repodb_owner IDENTIFIED BY ""8A5F2E1B-42C3-4D9E""';
            EXECUTE IMMEDIATE 'GRANT CONNECT, RESOURCE, DBA TO repodb_owner';
            EXECUTE IMMEDIATE 'ALTER USER repodb_owner QUOTA UNLIMITED ON USERS';
          END IF;
        END;");

        // Create limited user if it doesn't exist
        await sql.ExecuteNonQueryAsync($@"
        DECLARE
          v_count INTEGER;
        BEGIN
          SELECT COUNT(*) INTO v_count FROM all_users WHERE username = 'REPODB_USER';
          IF v_count = 0 THEN
            EXECUTE IMMEDIATE 'CREATE USER repodb_user IDENTIFIED BY ""0608B012-05D2-4023-A451""';
            EXECUTE IMMEDIATE 'GRANT CONNECT, RESOURCE TO repodb_user';
            EXECUTE IMMEDIATE 'ALTER USER repodb_user QUOTA UNLIMITED ON USERS';
          END IF;
        END;");

        Database.Initialize();
    }

    public override DbConnection CreateOpenLimitedConnection()
    {
        var r = base.CreateOpenLimitedConnection();

        r.ExecuteNonQuery("ALTER SESSION SET CURRENT_SCHEMA = REPODB_OWNER;");

        return r;
    }

    public async override ValueTask<DbConnection> CreateOpenLimitedConnectionAsync(CancellationToken cancellationToken = default)
    {
        var r = await base.CreateOpenLimitedConnectionAsync(cancellationToken);

        await r.ExecuteNonQueryAsync("ALTER SESSION SET CURRENT_SCHEMA = REPODB_OWNER;", cancellationToken: cancellationToken);
        return r;
    }
}
