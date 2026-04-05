using System.Diagnostics;

namespace RepoDb;

/// <summary>
/// A class that is being used to handle the field definition of the data reader.
/// </summary>
[DebuggerDisplay($"{{{nameof(Ordinal)}}}: {{{nameof(Name)},nq}} ({{{nameof(Type)}}}) - {{{nameof(DbField)}}}")]
internal record DataReaderField
{
    /// <summary>
    /// Gets or sets the name value.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets or sets the column ordinal value.
    /// </summary>
    public int Ordinal { get; init; }

    /// <summary>
    /// Gets or sets the <see cref="DbField"/> value.
    /// </summary>
    public DbField? DbField { get; init; }

    /// <summary>
    /// Gets or sets the type value.
    /// </summary>
    public required Type Type { get; init; }
}
