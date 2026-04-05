using Npgsql;
using RepoDb.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.PostgreSql.IntegrationTests.Common;

[TestClass]
public class JsonTests : RepoDb.TestCore.JsonTestsBase<PostgreSqlDbInstance>
{
    [TestInitialize]
    public void InitJson()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson();
#pragma warning restore CS0618 // Type or member is obsolete
    }

    public override string VarCharName => "character varying";
}
