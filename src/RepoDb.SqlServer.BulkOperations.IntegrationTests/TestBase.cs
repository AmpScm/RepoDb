using System;
using System.Collections.Generic;
using System.Text;
using RepoDb.TestCore;

namespace RepoDb.SqlServer.BulkOperations.IntegrationTests;

public abstract class TestBase : DbTestBase<SqlServerDbInstance>
{
    protected override void InitializeCore()
    {
        Database.Cleanup();
    }
}
