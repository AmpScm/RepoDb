using System.Data.Common;
using System.Data.SQLite;
using RepoDb.TestCore;

namespace RepoDb.SQLite.System.IntegrationTests.Setup;

public class SQLiteDbInstance : DbInstance<SQLiteConnection>
{
    private readonly SQLiteConnection _conn;

    static SQLiteDbInstance()
    {
        GlobalConfiguration.Setup(GlobalConfiguration.Options).UseSQLite();
    }

    public SQLiteDbInstance()
    {
        var cacheKey = Guid.NewGuid();

        // Database is shared when cache key is shared, until last connection dies
#if NET
        AdminConnectionString = ConnectionString = $"Data Source=file:{cacheKey}?mode=memory&cache=shared;";
#else
        // The NetFramework version of System.Data.SQLite doesn't support Data Source=file: syntax yet. (Is compiletime option. Conservative default)
        AdminConnectionString = ConnectionString = "Data Source=" +Path.GetFullPath(Path.GetTempFileName()).Replace(Path.DirectorySeparatorChar, '/');
#endif

        // Keep one connection open, but don't use it
        _conn = new SQLiteConnection(AdminConnectionString);
        _conn.Open();
    }

    public override string DatabaseName => "sqlite";

    protected override Task CreateUserDatabase(DbConnection sql)
    {
        return Task.CompletedTask;
    }
}
