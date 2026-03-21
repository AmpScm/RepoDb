using System.Data.Common;
using Microsoft.Data.Sqlite;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Setup;

namespace RepoDb.Sqlite.Microsoft.IntegrationTests.Common;

[TestClass]
public class NullTests : RepoDb.TestCore.NullTestsBase<SqliteDbInstance>
{
#if NET
    public override string TimeOnlyDbType => "TEXT";
#endif

    protected override string IdentityDefinition => "INTEGER PRIMARY KEY NOT NULL";
}
