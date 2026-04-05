using RepoDb.Enumerations;

namespace RepoDb;

/// <summary>
///
/// </summary>
public sealed record class DbSchemaObject
{
    /// <summary>
    /// The type of the database schema object (e.g. table, view, etc.).
    /// </summary>
    public DbSchemaType Type { get; init; }

    /// <summary>
    /// The object name of the database schema object (e.g. table name, view name, etc.).
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The schema name of the database schema object (e.g. dbo, etc.).
    /// </summary>
    public string? Schema { get; init; }
}


