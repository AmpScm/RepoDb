using System.Data.Common;

namespace RepoDb.Contexts.Execution;

/// <summary>
///
/// </summary>
internal sealed record InsertExecutionContext
{
    /// <summary>
    ///
    /// </summary>
    public required string CommandText { get; init; }

    /// <summary>
    ///
    /// </summary>
    public required IEnumerable<DbField> InputFields { get; init; }

    /// <summary>
    ///
    /// </summary>
    public required Action<DbCommand, object?> ParametersSetterFunc { get; init; }

    /// <summary>
    ///
    /// </summary>
    public Action<object, object?>? IdentitySetterFunc { get; init; }
}
