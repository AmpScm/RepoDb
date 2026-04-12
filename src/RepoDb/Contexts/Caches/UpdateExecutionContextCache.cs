using KeyType = (System.Type entityType, string tableName, RepoDb.FieldSet fields, string? hints, RepoDb.QueryGroup? where);

namespace RepoDb.Contexts;

/// <summary>
/// A class that is being used to cache the execution context of the Update operation.
/// </summary>
internal sealed class UpdateExecutionContextCache : ExecutionContextCache<KeyType, UpdateExecutionContext>
{
}
