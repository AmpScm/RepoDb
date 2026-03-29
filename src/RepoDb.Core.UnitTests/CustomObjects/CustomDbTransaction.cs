using System.Data;
using System.Data.Common;

namespace RepoDb.UnitTests.CustomObjects;

public class CustomDbTransaction : DbTransaction, IDbTransaction
{
    private readonly DbConnection connection;
    private bool disposed;

    public CustomDbTransaction()
        : this(null)
    {
    }

    public CustomDbTransaction(DbConnection connection)
    {
        this.connection = connection;
    }

    public override IsolationLevel IsolationLevel { get; }

    protected override DbConnection DbConnection => connection;

    /// <summary>
    /// Gets a value indicating whether the transaction has been disposed.
    /// </summary>
    public bool IsDisposed => disposed;

    public override void Commit()
    {
        /* do nothing */
    }

    public new void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public override void Rollback()
    {
        /* do nothing */
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="DbTransaction"/>.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            disposed = true;
        }
    }
}
