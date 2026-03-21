using RepoDb.PostgreSql.IntegrationTests.Setup;
using RepoDb.TestCore;

namespace RepoDb.PostgreSql.IntegrationTests;

public abstract class TestBase : DbTestBase<PostgreSqlDbInstance>
{
    protected override void InitializeCore()
    {
        Database.Cleanup();
    }
}
