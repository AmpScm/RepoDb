using System.Data.Common;
using System.Reflection;
using System.Runtime.Versioning;

namespace RepoDb.TestCore;

public abstract class DbInstance : IAsyncDisposable
{
    private bool _initialized;
    internal DbInstance()
    {

    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
#if NET
        return ValueTask.CompletedTask;
#else
        return new();
#endif
    }

    public async Task ClassInitializeAsync(TestContext? context)
    {
        if (!_initialized)
        {
#if NET
            await using var sql = CreateAdminConnection();
#else
            using var sql = CreateAdminConnection();
#endif
            await sql.EnsureOpenAsync(context.CancellationTokenSource.Token);

            await CreateUserDatabase(sql);

            _initialized = true;
        }
    }

    protected abstract Task CreateUserDatabase(DbConnection sql);

    public abstract DbConnection CreateConnection();

    public abstract DbConnection CreateAdminConnection();

    public DbConnection CreateOpenConnection()
    {
        var c = CreateConnection();
        try
        {
            c.EnsureOpen();
            return c;
        }
        catch
        {
            c.Dispose();
            throw;
        }
    }

    public async ValueTask<DbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var c = CreateConnection();
        try
        {
            await c.EnsureOpenAsync(cancellationToken);
            return c;
        }
        catch
        {
            c.Dispose();
            throw;
        }
    }

    public string AdminConnectionString { get; protected set; }
    public string ConnectionString { get; protected set; }
}

public abstract class DbInstance<TDbConnection> : DbInstance where TDbConnection : DbConnection, new()
{
    private string? _databaseName;
    public override DbConnection CreateConnection()
    {
        var c = new TDbConnection
        {
            ConnectionString = ConnectionString
        };
        return c;
    }

    public override DbConnection CreateAdminConnection()
    {
        var c = new TDbConnection
        {
            ConnectionString = AdminConnectionString
        };
        return c;
    }

    public virtual string DatabaseName
    {
        get => _databaseName ??= (GetType().Assembly is { } a ? (a.GetName().Name + "for" + a.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName) : "RepoDbTest")
            .Replace("Integration", "")
            .Replace("Unit", "")
            .Replace("NETCore", "")
            .Replace("App", "")
            .Replace("Version", "")
            .Replace("Tests", "").Replace(".", "").Replace(",", "").Replace("=", "");
    }
}
