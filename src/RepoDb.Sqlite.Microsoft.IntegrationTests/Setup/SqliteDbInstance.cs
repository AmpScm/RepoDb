using System.Data.Common;
using Microsoft.Data.Sqlite;
using RepoDb.TestCore;

namespace RepoDb.Sqlite.Microsoft.IntegrationTests.Setup;

public class SqliteDbInstance : DbInstance<SqliteConnection>
{
    private readonly SqliteConnection _conn;
    private static readonly Guid _cacheKey = Guid.NewGuid();

    static SqliteDbInstance()
    {
        GlobalConfiguration.Setup(GlobalConfiguration.Options).UseSqlite();
    }

    public SqliteDbInstance()
    {
        // Database is shared when cache key is shared, until last connection dies
        AdminConnectionString = ConnectionString = $"Data Source=file:{_cacheKey}.db?mode=memory&cache=shared;";

        // SQLite doesn't have user-level security; limited connection uses same database
        LimitedConnectionString = ConnectionString;

        // Keep one connection open, but don't use it
        _conn = new SqliteConnection(AdminConnectionString);
        _conn.Open();
    }

    public override string DatabaseName => "sqlite";

    protected override Task CreateUserDatabase(DbConnection sql)
    {
        Database.Initialize();
        return Task.CompletedTask;
    }
}
