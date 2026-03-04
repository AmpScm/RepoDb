using System.Data.Common;
using System.Data.SQLite;
using RepoDb.SQLite.System.IntegrationTests.Setup;

namespace RepoDb.SQLite.System.IntegrationTests.Common;

[TestClass]
public class EnumTests : RepoDb.TestCore.EnumTestsBase<SQLiteDbInstance>
{
    protected override void InitializeCore() => Database.Initialize(TestContext);

    public override DbConnection CreateConnection() => new SQLiteConnection(Database.GetConnectionString(TestContext));
}
