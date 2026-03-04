using System.Data.Common;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Oracle.IntegrationTests.Setup;

namespace RepoDb.Oracle.IntegrationTests.Common;

[TestClass]
public class EnumTests : RepoDb.TestCore.EnumTestsBase<OracleDbInstance>
{
    protected override void InitializeCore() => Database.Initialize();

    public override DbConnection CreateConnection() => new OracleConnection(Database.ConnectionString);
}
