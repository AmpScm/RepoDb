using RepoDb.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.PostgreSql.IntegrationTests.Common;

[TestClass]
public class EnumTests : RepoDb.TestCore.EnumTestsBase<PostgreSqlDbInstance>
{
}
