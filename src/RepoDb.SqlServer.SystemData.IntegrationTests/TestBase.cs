using RepoDb.SqlServer.SystemData.IntegrationTests.Setup;
using RepoDb.TestCore;

namespace RepoDb.SqlServer.SystemData.IntegrationTests;

public abstract class TestBase : DbTestBase<SqlServerDbInstance>
{
    protected override void InitializeCore()
    {
        Database.Cleanup();
    }
}
