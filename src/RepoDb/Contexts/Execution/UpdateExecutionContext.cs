using System.Data.Common;

namespace RepoDb.Contexts;

internal sealed record UpdateExecutionContext
{
    public required string CommandText { get; init; }
    public required Action<DbCommand, object?> ParametersSetterFunc { get; init; }
}
