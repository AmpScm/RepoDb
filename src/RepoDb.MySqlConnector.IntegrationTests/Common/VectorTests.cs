using System.Data.Common;
using MySqlConnector;
using RepoDb.MySqlConnector.IntegrationTests.Setup;

namespace RepoDb.MySqlConnector.IntegrationTests.Common;

[TestClass]
public class VectorTests : TestCore.VectorTestsBase<MysqlDbInstance>
{
    protected override void InitializeCore() => Database.Initialize();


}
