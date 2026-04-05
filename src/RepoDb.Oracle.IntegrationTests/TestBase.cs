using RepoDb.Caches;
using RepoDb.Oracle.IntegrationTests.Setup;
using RepoDb.TestCore;

namespace RepoDb.Oracle.IntegrationTests;

public abstract class TestBase : DbTestBase<OracleDbInstance>
{
    protected override void InitializeCore()
    {
        Database.Cleanup();
    }

    public void ResetIdentity(string table)
    {
        using var db = CreateOpenConnection();

        db.ExecuteNonQuery(@"
DECLARE
    seqName VARCHAR2(128);
    currVal NUMBER;
BEGIN
    -- Zoek de sequence die bij de identity hoort
    SELECT sequence_name INTO seqName
    FROM user_tab_identity_cols
    WHERE table_name = :pTableName;

    -- Huidige waarde ophalen
    SELECT last_number INTO currVal
    FROM user_sequences
    WHERE sequence_name = seqName;

    -- Sequence terugduwen naar 1
    EXECUTE IMMEDIATE 'ALTER SEQUENCE ' || seqName || ' INCREMENT BY -' || (currVal - 1);
    EXECUTE IMMEDIATE 'SELECT ' || seqName || '.NEXTVAL FROM dual';

    -- Increment herstellen
    EXECUTE IMMEDIATE 'ALTER SEQUENCE ' || seqName || ' INCREMENT BY 1';
END;", new
        {
            TableName = table,
        });
    }

    static bool? _v23OrLater;
    public bool V23cOrLater => _v23OrLater ??= GetV23cOrLater();

    private bool GetV23cOrLater()
    {
        using var db = CreateConnection();
        return DbRuntimeSettingCache.Get(db, null)?.EngineVersion.Major >= 23;
    }
}
