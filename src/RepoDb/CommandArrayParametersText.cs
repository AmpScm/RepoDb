using System.Data;

namespace RepoDb;

/// <summary>
/// A class that is being used to handle the command text of the array value of the parameter.
/// </summary>
internal record class CommandArrayParametersText
{
    /// <summary>
    /// Gets the actual command string to be executed (derived from array parameters).
    /// </summary>
    public required string CommandText { get; init; }

    /// <summary>
    /// Gets the database type of the parameter.
    /// </summary>
    public DbType? DbType { get; init; }

    /// <summary>
    /// Gets the list of the command array parameters.
    /// </summary>
    public List<CommandArrayParameter> CommandArrayParameters { get; } = new();
}
