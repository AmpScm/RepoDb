using System.Data;
using System.Data.Common;

namespace RepoDb.UnitTests.CustomObjects;

public class CustomDbConnection : DbConnection, IDbConnection
{
    public override string ConnectionString { get; set; }

    public override string Database { get; }

    public override string DataSource { get; }

    public override string ServerVersion { get; }

    public override ConnectionState State { get; }

    /// <summary>
    /// Gets a value indicating whether the connection has been disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }

    public override void ChangeDatabase(string databaseName)
    {
    }

    public override void Close()
    {
    }

    public override void Open()
    {
    }

    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
    {
        return new CustomDbTransaction(this);
    }

    protected override DbCommand CreateDbCommand()
    {
        return new CustomDbCommand() { Connection = this };
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="DbConnection"/>.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            IsDisposed = true;
        }
    }
}
