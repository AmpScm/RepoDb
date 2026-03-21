using RepoDb.MySql.IntegrationTests.Setup;
using RepoDb.TestCore;

namespace RepoDb.MySql.IntegrationTests;

public abstract class TestBase : DbTestBase<MysqlDbInstance>
{
    protected override void InitializeCore()
    {
        Database.Cleanup();
    }
}
