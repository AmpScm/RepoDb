using RepoDb.IntegrationTests.Setup;
using RepoDb.TestCore;

namespace RepoDb.IntegrationTests;

public abstract class TestBase : DbTestBase<SqlServerDbInstance>
{
    protected override void InitializeCore()
    {
        base.InitializeCore();
        Database.Cleanup();
    }
}
