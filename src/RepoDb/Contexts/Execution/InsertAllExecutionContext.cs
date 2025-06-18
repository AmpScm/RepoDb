using System.Data.Common;

namespace RepoDb.Contexts.Execution;

/// <summary>
///
/// </summary>
internal sealed record InsertAllExecutionContext
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
    public int BatchSize { get; init; }

    /// <summary>
    ///
    /// </summary>
    public Action<DbCommand, object?>? SingleDataEntityParametersSetterFunc { get; init; }

    /// <summary>
    ///
    /// </summary>
    public Action<DbCommand, IList<object?>>? MultipleDataEntitiesParametersSetterFunc { get; init; }

    /// <summary>
    ///
    /// </summary>
    public Action<object, object?>? IdentitySetterFunc { get; init; }
}
