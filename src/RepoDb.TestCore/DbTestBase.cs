using System.Data.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.TestCore;

public abstract class DbTestBase<TDbInstance> where TDbInstance : DbInstance, new()
{
    public TestContext TestContext { get; set; }

    public TDbInstance DbInstance = new();

    [ClassInitialize(InheritanceBehavior.BeforeEachDerivedClass)]
    public static async Task TestClassInitialize(TestContext context)
    {
        await using var q = new TDbInstance();

        await q.ClassInitializeAsync(context);
    }

    [TestInitialize]
    public void Initialize()
    {
        InitializeCore();
    }

    protected abstract void InitializeCore();

    public abstract DbConnection CreateConnection();

    public async Task<DbConnection> CreateOpenConnectionAsync()
    {
        var db = CreateConnection();
        try
        {
            await db.OpenAsync();
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
