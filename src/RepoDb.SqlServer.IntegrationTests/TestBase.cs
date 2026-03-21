using RepoDb.SqlServer.IntegrationTests.Setup;
using RepoDb.TestCore;

namespace RepoDb.SqlServer.IntegrationTests;

public abstract class TestBase : DbTestBase<SqlServerDbInstance>
{
    protected override void InitializeCore()
    {
        Database.Cleanup();
    }
}
