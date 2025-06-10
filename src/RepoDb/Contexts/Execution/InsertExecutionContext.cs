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
    public string CommandText { get; init; }

    /// <summary>
    ///
    /// </summary>
    public IEnumerable<DbField> InputFields { get; init; }

    /// <summary>
    ///
    /// </summary>
    public Action<DbCommand, object?> ParametersSetterFunc { get; init; }

    /// <summary>
    ///
    /// </summary>
    public Action<object, object?> IdentitySetterFunc { get; init; }
}
