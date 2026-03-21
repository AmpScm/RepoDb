using System.Data.Common;
using RepoDb.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.PostgreSql.IntegrationTests.Common;

[TestClass]
public class JsonTests : RepoDb.TestCore.JsonTestsBase<PostgreSqlDbInstance>
{
    public override string VarCharName => "character varying";
}
