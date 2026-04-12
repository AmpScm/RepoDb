using KeyType = (System.Type entityType, string tableName, RepoDb.FieldSet qualifiers, RepoDb.FieldSet fields, RepoDb.FieldSet? noUpdateFields, string? hints);

namespace RepoDb.Contexts;

/// <summary>
/// A class that is being used to cache the execution context of the Merge operation.
/// </summary>
internal sealed class MergeExecutionContextCache : ExecutionContextCache<KeyType, MergeExecutionContext>
{
}
