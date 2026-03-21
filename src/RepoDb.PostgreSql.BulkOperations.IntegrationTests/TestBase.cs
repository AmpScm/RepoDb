using System;
using System.Collections.Generic;
using System.Text;
using RepoDb.IntegrationTests.Setup;
using RepoDb.TestCore;

namespace RepoDb.PostgreSql.BulkOperations.IntegrationTests;

public abstract class TestBase : DbTestBase<PostgreSqlDbInstance>
{
    protected override void InitializeCore()
    {
        Database.Cleanup();
    }
}
