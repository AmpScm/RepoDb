using RepoDb.Enumerations;

namespace RepoDb;

public sealed record class DbSchemaObject
{
    public DbSchemaType Type { get; init; }
    public required string Name { get; init; }
    public string? Schema { get; init; }
}


