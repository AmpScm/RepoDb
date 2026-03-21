using System.Data.Common;

namespace RepoDb.TestCore;

public abstract class DbTestBase<TDbInstance> where TDbInstance : DbInstance, new()
{
    public required TestContext TestContext { get; init; }
    public virtual string VarCharName => "varchar";
    public virtual string AltVarCharName => "varchar";
    public virtual string DecimalName => "decimal";

    public TDbInstance DbInstance = new();

    [ClassInitialize(InheritanceBehavior.BeforeEachDerivedClass)]
    public static async Task TestClassInitialize(TestContext context)
    {
        var q = new TDbInstance();
        await q.ClassInitializeAsync(context).ConfigureAwait(false);
    }

    [TestInitialize]
    public void Initialize()
    {
        InitializeCore();
        DbInstance.PostInitialize();
        PostInitialize();
    }

    protected virtual void InitializeCore()
    {

    }

    protected virtual void PostInitialize()
    {

    }

    public virtual DbConnection CreateConnection() => DbInstance.CreateConnection();

    public virtual DbConnection CreateLimitedConnection() => DbInstance.CreateLimitedConnection();

    public async Task<DbConnection> CreateOpenConnectionAsync()
    {
        var db = CreateConnection();
        try
        {
            await db.OpenAsync(TestContext.CancellationToken);
            return db;
        }
        catch
        {
#if NET
            await db.DisposeAsync();
#else
            db.Dispose();
#endif
            throw;
        }
    }

    public DbConnection CreateOpenConnection()
    {
        var db = CreateConnection();
        try
        {
            db.Open();
            return db;
        }
        catch
        {
            db.Dispose();
            throw;
        }
    }

    public async Task<DbConnection> CreateOpenLimitedConnectionAsync()
    {
        var db = CreateLimitedConnection();
        try
        {
            await db.OpenAsync(TestContext.CancellationToken);
            return db;
        }
        catch
        {
#if NET
            await db.DisposeAsync();
#else
            db.Dispose();
#endif
            throw;
        }
    }

    public DbConnection CreateOpenLimitedConnection()
    {
        var db = CreateLimitedConnection();
        try
        {
            db.Open();
            return db;
        }
        catch
        {
            db.Dispose();
            throw;
        }
    }

    protected static async Task<string> PerformCreateTableAsync(System.Data.Common.DbConnection sql, string sqlText)
    {
        sqlText = ApplySqlRules(sql, sqlText);

        try
        {
            await sql.ExecuteNonQueryAsync(sqlText);
        }
        catch (Exception e)
        {
            throw new Exception($"While performing: {sqlText}", e);
        }
        return sqlText;
    }

    protected static string PerformCreateTable(System.Data.Common.DbConnection sql, string sqlText)
    {
        sqlText = ApplySqlRules(sql, sqlText);

        try
        {
            sql.ExecuteNonQuery(sqlText);
        }
        catch (Exception e)
        {
            throw new Exception($"While performing: {sqlText}", e);
        }
        return sqlText;
    }

    protected static string ApplySqlRules(System.Data.Common.DbConnection sql, string sqlText)
    {
        var set = sql.GetDbSetting();

        if (set.OpeningQuote != "[")
            sqlText = sqlText.Replace("[", set.OpeningQuote);
        if (set.ClosingQuote != "]")
            sqlText = sqlText.Replace("]", set.ClosingQuote);
        if (set.ParameterPrefix != "@")
            sqlText = sqlText.Replace("@", set.ParameterPrefix);
        return sqlText;
    }
}
