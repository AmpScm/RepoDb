using System.Data.Common;

namespace RepoDb.Contexts.Execution;

internal sealed record InsertExecutionContext
{
    public required string CommandText { get; init; }

    public required IEnumerable<DbField> InputFields { get; init; }

    public required Action<DbCommand, object?> ParametersSetterFunc { get; init; }

    public Action<object, object?>? IdentitySetterFunc { get; init; }
}
