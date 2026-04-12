using KeyType = (System.Type entityType, string tableName, RepoDb.FieldSet fields, string? hints);

namespace RepoDb.Contexts;

/// <summary>
/// A class that is being used to cache the execution context of the Insert operation.
/// </summary>
internal sealed class InsertExecutionContextCache : ExecutionContextCache<KeyType, InsertExecutionContext>
{
}
