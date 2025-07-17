using System.Data.Common;

namespace RepoDb;

public readonly struct DbSession : IAsyncDisposable, IDisposable, IEquatable<DbSession>
{
    private readonly object _value; // Either DbConnection or DbTransaction
    private readonly bool _owns;

    public DbSession(DbConnection connection, bool ownsConnection = false)
    {
        ArgumentNullException.ThrowIfNull(connection);

        _value = connection;
        _owns = ownsConnection;
    }

    public DbSession(DbTransaction transaction, bool ownsTransaction = false)
    {
        ArgumentNullException.ThrowIfNull(transaction);

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

    public override bool Equals(object? obj)
    {
        return obj is DbSession other && Equals(other);
    }

    public bool Equals(DbSession other)
    {
        if (_value is DbTransaction tx1 && other._value is DbTransaction tx2)
        {
            return tx1.Equals(tx2);
        }
        else if (_value is DbConnection conn1 && other._value is DbConnection conn2)
        {
            return conn1.Equals(conn2);
        }
        return false;
    }

    public override int GetHashCode() => _value.GetHashCode();

    public static bool operator ==(DbSession left, DbSession right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(DbSession left, DbSession right)
    {
        return !(left == right);
    }
}
