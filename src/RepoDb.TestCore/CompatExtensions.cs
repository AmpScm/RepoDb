using System.Data.Common;

namespace RepoDb.TestCore;

public static class CompatExtensions
{
#if !NET
    public static ValueTask<DbTransaction> BeginTransactionAsync(this DbConnection connection, CancellationToken cancellationToken = default)
    {
        return new(connection.BeginTransaction());
    }

    public static Task RollbackAsync(this DbTransaction transaction, CancellationToken cancellationToken = default)
    {
        transaction.Rollback();

        return Task.CompletedTask;
    }
#endif
}
