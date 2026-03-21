using RepoDb.MySqlConnector.IntegrationTests.Setup;
using RepoDb.TestCore;

namespace RepoDb.MySqlConnector.IntegrationTests;

public abstract class TestBase : DbTestBase<MysqlDbInstance>
{
    protected override void InitializeCore()
    {
        Database.Cleanup();
    }
}
