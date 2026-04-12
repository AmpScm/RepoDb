using System.Data.Common;

namespace RepoDb.Contexts;

internal sealed record UpdateAllExecutionContext
{
    public required string CommandText { get; init; }
    public int BatchSize { get; init; }
    public Action<DbCommand, object?>? SingleDataEntityParametersSetterFunc { get; init; }
    public Action<DbCommand, IList<object?>>? MultipleDataEntitiesParametersSetterFunc { get; init; }
}
