
using RepoDb.Oracle.IntegrationTests.Setup;

namespace RepoDb.Oracle.IntegrationTests.Common;

[TestClass]
public class VectorTests : TestCore.VectorTestsBase<OracleDbInstance>
{
    protected override void InitializeCore() => Database.Initialize();


}
