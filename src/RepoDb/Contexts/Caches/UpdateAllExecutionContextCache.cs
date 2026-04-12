using KeyType = (System.Type entityType, string tableName, RepoDb.FieldSet? qualifiers, RepoDb.FieldSet fields, int batchSize, string? hints);

namespace RepoDb.Contexts;

/// <summary>
/// A class that is being used to cache the execution context of the UpdateAll operation.
/// </summary>
internal sealed class UpdateAllExecutionContextCache : ExecutionContextCache<KeyType, UpdateAllExecutionContext>
{
}
