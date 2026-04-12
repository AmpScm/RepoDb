using System.Data.Common;

namespace RepoDb.Contexts;

internal sealed record InsertExecutionContext
{
    public required string CommandText { get; init; }
    public required Action<DbCommand, object?> ParametersSetterFunc { get; init; }
    public Action<object, object?>? IdentitySetterFunc { get; init; }
}
