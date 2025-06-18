using System.Data.Common;

namespace RepoDb;

public readonly struct DbSession : IAsyncDisposable, IDisposable
{
    private readonly object _value; // Either DbConnection or DbTransaction
    private readonly bool _owns;

    public DbSession(DbConnection connection, bool ownsConnection = false)
    {
#if NET
        ArgumentNullException.ThrowIfNull(connection);
#else
        if (connection is null)
            throw new ArgumentNullException(nameof(connection));
#endif
        _value = connection;
        _owns = ownsConnection;
    }

    public DbSession(DbTransaction transaction, bool ownsTransaction = false)
    {
#if NET
        ArgumentNullException.ThrowIfNull(transaction);
#else
        if (transaction is null)
            throw new ArgumentNullException(nameof(transaction));
#endif
        _value = transaction;
        _owns = ownsTransaction;
    }

    public DbConnection Connection =>
        _value is DbTransaction tx ? tx.Connection! : (DbConnection)_value;

    public DbTransaction? Transaction =>
        _value as DbTransaction;

    public void Dispose()
    {
        if (_owns)
        {
            if (_value is DbTransaction tx)
                tx.Dispose();
            else
                ((DbConnection)_value).Dispose();
        }
    }

#if NET
    public async ValueTask DisposeAsync()
    {
        if (_owns)
        {
            if (_value is DbTransaction tx)
                await tx.DisposeAsync().ConfigureAwait(false);
            else
                await ((DbConnection)_value).DisposeAsync().ConfigureAwait(false);
        }
    }
#else
    public ValueTask DisposeAsync()
    {
        Dispose();
        return new();
    }
#endif
}
