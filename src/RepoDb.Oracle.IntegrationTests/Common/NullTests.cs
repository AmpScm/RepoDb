using System.Data.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Oracle.IntegrationTests.Setup;

namespace RepoDb.Oracle.IntegrationTests.Common;

[TestClass]
public class NullTests : RepoDb.TestCore.NullTestsBase<OracleDbInstance>
{
    protected override void InitializeCore() => Database.Initialize();

    public override DbConnection CreateConnection() => new OracleConnection(Database.ConnectionString);

    public override string GeneratedColumnDefinition(string expression, string type) => $"GENERATED ALWAYS AS ({expression})";

#if NET
    public override string TimeOnlyDbType => "INTERVAL DAY TO SECOND";
#endif
    public override string DateTimeOffsetDbType => "TIMESTAMP WITH TIME ZONE";
    public override string DateTimeDbType => "TIMESTAMP";

    public override string VarCharName => "VARCHAR2";
    public override string AltVarChar => "NVARCHAR2";

    public override string BlobDbType => "RAW(128)";
    public override string UuidDbType => "RAW(16)";
    public override string TextDbType => "CLOB";
    public override string IntDbType => "NUMBER";

    protected override string IdentityDefinition => "NUMBER GENERATED ALWAYS AS IDENTITY (START WITH 1 INCREMENT BY 1)";


}
