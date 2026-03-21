using System.Data.Common;
using System.Reflection;
using System.Runtime.Versioning;

namespace RepoDb.TestCore;

public abstract class DbInstance
{
    private Task? _initialized;

    public async Task ClassInitializeAsync(TestContext context)
    {
        _initialized ??= ((Func<Task>)(async () =>
         {
#if NET
             await using var sql = CreateAdminConnection();
#else
            using var sql = CreateAdminConnection();
#endif
             await sql.EnsureOpenAsync(CancellationToken.None);

             await CreateUserDatabase(sql);
         }))();

        await _initialized;
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

    public virtual IDisposable? SetIdentityInsert(bool value)
    {
        return null;
    }

    internal protected virtual void PostInitialize()
    {
    }

    /// <summary>
    /// System connection string (used for database/schema creation as admin/system user on master/postgres/etc)
    /// </summary>
    public string AdminConnectionString { get; protected set; }

    /// <summary>
    /// Owner connection string (used for setup operations like creating schemas, types, procedures)
    /// Typically same privilege level as db_owner in SQL Server
    /// This is the default ConnectionString for backward compatibility with existing tests
    /// </summary>
    public string ConnectionString { get; protected set; }

    /// <summary>
    /// Limited user connection string (used for actual test operations with restricted privileges)
    /// </summary>
    public string LimitedConnectionString { get; protected set; }

    /// <summary>
    /// Creates a connection using the limited user connection string
    /// </summary>
    public abstract DbConnection CreateLimitedConnection();

    /// <summary>
    /// Creates an open connection using the limited user connection string
    /// </summary>
    public virtual DbConnection CreateOpenLimitedConnection()
    {
        var c = CreateLimitedConnection();
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

    /// <summary>
    /// Creates an open connection asynchronously using the limited user connection string
    /// </summary>
    public virtual async ValueTask<DbConnection> CreateOpenLimitedConnectionAsync(CancellationToken cancellationToken = default)
    {
        var c = CreateLimitedConnection();
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

    public override DbConnection CreateLimitedConnection()
    {
        var c = new TDbConnection
        {
            ConnectionString = LimitedConnectionString
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
