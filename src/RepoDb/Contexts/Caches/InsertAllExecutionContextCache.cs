using KeyType = (System.Type entityType, string tableName, RepoDb.FieldSet fields, int batchSize, string?);

namespace RepoDb.Contexts;

/// <summary>
/// A class that is being used to cache the execution context of the InsertAll operation.
/// </summary>
internal sealed class InsertAllExecutionContextCache : ExecutionContextCache<KeyType, InsertAllExecutionContext>
{
}
