using System.Data.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySqlConnector;
using RepoDb.MySqlConnector.IntegrationTests.Setup;

namespace RepoDb.MySqlConnector.IntegrationTests.Common;

[TestClass]
public class JsonTests : RepoDb.TestCore.JsonTestsBase<MysqlDbInstance>
{
    protected override void InitializeCore() => Database.Initialize();

    public override DbConnection CreateConnection() => new MySqlConnection(Database.ConnectionString);
}
