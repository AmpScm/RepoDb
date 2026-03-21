using RepoDb.SQLite.System.IntegrationTests.Setup;
using RepoDb.TestCore;

namespace RepoDb.SQLite.System.IntegrationTests;

public abstract class TestBase : DbTestBase<SQLiteDbInstance>
{
    protected override void InitializeCore()
    {
        Database.Initialize();
        Database.Cleanup();
    }
}
