using System.Data.Common;
using RepoDb.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.PostgreSql.IntegrationTests.Common;

[TestClass]
public class NullTests : RepoDb.TestCore.NullTestsBase<PostgreSqlDbInstance>
{
    public override string UuidDbType => "CHAR(38)";
    public override string DateTimeDbType => "TIMESTAMP";
    public override string DateTimeOffsetDbType => "TIMESTAMPTZ";

    public override string BlobDbType => "BYTEA";

    public override string VarCharName => "character varying";

    public override string GeneratedColumnDefinition(string expression, string type)
    {
        if (expression.StartsWith("CONCAT("))
            expression = expression.Substring(7).Replace(",", " || ").TrimEnd(')');

        return $"{type} GENERATED ALWAYS AS ({expression}) STORED";
    }
}
