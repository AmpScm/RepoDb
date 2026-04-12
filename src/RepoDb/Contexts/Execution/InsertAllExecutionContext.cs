using System.Data.Common;

namespace RepoDb.Contexts;

/// <summary>
///
/// </summary>
internal sealed record InsertAllExecutionContext
{
    public required string CommandText { get; init; }
    public int BatchSize { get; init; }
    public Action<DbCommand, object?>? SingleDataEntityParametersSetterFunc { get; init; }
    public Action<DbCommand, IList<object?>>? MultipleDataEntitiesParametersSetterFunc { get; init; }
    public Action<object, object?>? IdentitySetterFunc { get; init; }
}
