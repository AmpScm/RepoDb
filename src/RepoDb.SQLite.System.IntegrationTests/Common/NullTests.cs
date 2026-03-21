using System.Data.Common;
using System.Data.SQLite;
using RepoDb.SQLite.System.IntegrationTests.Setup;

namespace RepoDb.SQLite.System.IntegrationTests.Common;

[TestClass]
public class NullTests : RepoDb.TestCore.NullTestsBase<SQLiteDbInstance>
{
#if NET
    public override string TimeOnlyDbType => "TEXT";
#endif

    protected override string IdentityDefinition => "INTEGER PRIMARY KEY NOT NULL";
}
