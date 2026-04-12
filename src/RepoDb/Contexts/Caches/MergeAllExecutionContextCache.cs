using KeyType = (System.Type entityType, string tableName, RepoDb.FieldSet qualifiers, RepoDb.FieldSet fields, RepoDb.FieldSet? noUpdateFields, int batchSize, string? hints);

namespace RepoDb.Contexts;

/// <summary>
/// A class that is being used to cache the execution context of the MergeAll operation.
/// </summary>
internal sealed class MergeAllExecutionContextCache : ExecutionContextCache<KeyType, MergeAllExecutionContext>
{
}
