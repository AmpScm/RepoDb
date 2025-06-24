using System.Data;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb.Requests;

/// <summary>
/// A class that holds the value of the 'Insert' operation arguments.
/// </summary>
internal sealed class InsertAllRequest : BaseRequest
{
    /// <summary>
    /// Creates a new instance of <see cref="InsertAllRequest"/> object.
    /// </summary>
    /// <param name="type">The target type.</param>
    /// <param name="connection">The connection object.</param>
    /// <param name="transaction">The transaction object.</param>
    /// <param name="fields">The list of the target fields.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="hints">The hints for the table.</param>
    /// <param name="statementBuilder">The statement builder.</param>
    public InsertAllRequest(Type type,
        IDbConnection connection,
        IDbTransaction? transaction,
        IEnumerable<Field> fields,
        int batchSize,
        string? hints = null,
        IStatementBuilder? statementBuilder = null)
        : this(type,
              ClassMappedNameCache.Get(type),
              connection,
              transaction,
              fields,
              batchSize,
              hints,
              statementBuilder)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="InsertAllRequest"/> object.
    /// </summary>
    /// <param name="name">The name of the request.</param>
    /// <param name="connection">The connection object.</param>
    /// <param name="transaction">The transaction object.</param>
    /// <param name="fields">The list of the target fields.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="hints">The hints for the table.</param>
    /// <param name="statementBuilder">The statement builder.</param>
    public InsertAllRequest(string name,
        IDbConnection connection,
        IDbTransaction? transaction,
        IEnumerable<Field> fields,
        int batchSize,
        string? hints = null,
        IStatementBuilder? statementBuilder = null)
        : this(null,
              name,
              connection,
              transaction,
              fields,
              batchSize,
              hints,
              statementBuilder)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="InsertAllRequest"/> object.
    /// </summary>
    /// <param name="type">The target type.</param>
    /// <param name="name">The name of the request.</param>
    /// <param name="connection">The connection object.</param>
    /// <param name="transaction">The transaction object.</param>
    /// <param name="fields">The list of the target fields.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="hints">The hints for the table.</param>
    /// <param name="statementBuilder">The statement builder.</param>
    public InsertAllRequest(Type? type,
        string name,
        IDbConnection connection,
        IDbTransaction? transaction,
        IEnumerable<Field> fields,
        int batchSize,
        string? hints = null,
        IStatementBuilder? statementBuilder = null)
        : base(name ?? ClassMappedNameCache.Get(type),
              connection,
              transaction,
              statementBuilder)
    {
        Type = type;
        Fields = fields.AsFieldSet();
        BatchSize = batchSize;
        Hints = hints;
    }

    /// <summary>
    /// Gets the target fields.
    /// </summary>
    public FieldSet Fields { get; init; }

    /// <summary>
    /// Gets the size batch of the insertion.
    /// </summary>
    public int BatchSize { get; init; }

    /// <summary>
    /// Gets the hints for the table.
    /// </summary>
    public string? Hints { get; }

    #region Equality and comparers

    /// <summary>
    /// Returns the hashcode for this <see cref="InsertAllRequest"/>.
    /// </summary>
    /// <returns>The hashcode value.</returns>
    public override int GetHashCode()
    {
        if (HashCode is not { } hashCode)
        {
            HashCode = hashCode = System.HashCode.Combine(
                typeof(InsertAllRequest),
                Name,
                BatchSize,
                Hints,
                Fields);
        }

        return hashCode;
    }

    protected override bool StrictEquals(BaseRequest other)
    {
        // TODO: Implement Equals() and use from here.
        return other is InsertAllRequest;
    }
}

#endregion
