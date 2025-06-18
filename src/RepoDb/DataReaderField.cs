namespace RepoDb;

/// <summary>
/// A class that is being used to handle the field definition of the data reader.
/// </summary>
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

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>The name of the field and the type.</returns>
    public override string ToString() =>
        string.Concat(Name, " (", Type?.FullName, ")");
}
