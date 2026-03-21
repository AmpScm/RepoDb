using System.Data.Common;
using System.Data.SQLite;
using RepoDb.TestCore;

namespace RepoDb.SQLite.System.IntegrationTests.Setup;

public class SQLiteDbInstance : DbInstance<SQLiteConnection>
{
    private readonly SQLiteConnection _conn;
    static readonly Guid _cacheKey = Guid.NewGuid();

    static SQLiteDbInstance()
    {
        GlobalConfiguration.Setup(GlobalConfiguration.Options).UseSQLite();
    }

    public SQLiteDbInstance()
    {
        // Database is shared when cache key is shared, until last connection dies
        AdminConnectionString = ConnectionString = $"Data Source={Path.GetFullPath(Path.ChangeExtension(_cacheKey.ToString(), ".db"))};";

        // SQLite doesn't have user-level security; limited connection uses same database
        LimitedConnectionString = ConnectionString;

        // Keep one connection open, but don't use it
        _conn = new SQLiteConnection(AdminConnectionString);
        _conn.Open();
    }

    public override string DatabaseName => "sqlite";

    protected override Task CreateUserDatabase(DbConnection sql)
    {
        Database.Initialize();
        return Task.CompletedTask;
    }
}
