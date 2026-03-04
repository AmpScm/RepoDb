using System.Data.Common;
using Microsoft.Data.SqlClient;
using RepoDb.SqlServer.IntegrationTests.Setup;

namespace RepoDb.SqlServer.IntegrationTests.Common;

[TestClass]
public class NullTests : RepoDb.TestCore.NullTestsBase<SqlServerDbInstance>
{
    protected override void InitializeCore() => Database.Initialize();

    public override DbConnection CreateConnection() => new SqlConnection(Database.ConnectionString);

    public override string GeneratedColumnDefinition(string expression, string type) => $"AS ({expression})";

    public override string AltVarChar => "nvarchar";

    protected override string IdentityDefinition => "INT IDENTITY(1,1) NOT NULL";

}
