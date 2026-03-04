using System.Data.Common;
using Microsoft.Data.SqlClient;
using RepoDb.SqlServer.IntegrationTests.Setup;

namespace RepoDb.SqlServer.IntegrationTests.Common;

[TestClass]
public class EnumTests : RepoDb.TestCore.EnumTestsBase<SqlServerDbInstance>
{
    protected override void InitializeCore() => Database.Initialize();

    public override DbConnection CreateConnection() => new SqlConnection(Database.ConnectionString);
}
