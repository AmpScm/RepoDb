using RepoDb.Sqlite.Microsoft.IntegrationTests.Setup;
using RepoDb.TestCore;

namespace RepoDb.Sqlite.Microsoft.IntegrationTests;

public abstract class TestBase : DbTestBase<SqliteDbInstance>
{
    protected override void InitializeCore()
    {
        Database.Initialize();
        Database.Cleanup();
    }
}
