using System.Data.Common;

namespace RepoDb.Contexts.Execution;

internal sealed record MergeAllExecutionContext
{
    public required string CommandText { get; init; }

    public required IEnumerable<DbField> InputFields { get; init; }

    public int BatchSize { get; init; }

    public Action<DbCommand, object?>? SingleDataEntityParametersSetterFunc { get; init; }

    public Action<DbCommand, IList<object?>>? MultipleDataEntitiesParametersSetterFunc { get; init; }

    public Action<object, object?>? KeyPropertySetterFunc { get; init; }
}
