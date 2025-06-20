using System.Data;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb.Requests;

/// <summary>
/// A class that holds the value of the 'UpdateAll' operation arguments.
/// </summary>
internal sealed class UpdateAllRequest : BaseRequest
{
    /// <summary>
    /// Creates a new instance of <see cref="UpdateAllRequest"/> object.
    /// </summary>
    /// <param name="type">The target type.</param>
    /// <param name="connection">The connection object.</param>
    /// <param name="transaction">The transaction object.</param>
    /// <param name="fields">The list of the target fields.</param>
    /// <param name="qualifiers">The list of the qualifier <see cref="Field"/> objects.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="hints">The hints for the table.</param>
    /// <param name="statementBuilder">The statement builder.</param>
    public UpdateAllRequest(Type type,
        IDbConnection connection,
        IDbTransaction? transaction,
        IEnumerable<Field> fields,
        IEnumerable<Field>? qualifiers,
        int batchSize,
        string? hints = null,
        IStatementBuilder? statementBuilder = null)
        : this(ClassMappedNameCache.Get(type),
            connection,
            transaction,
            fields,
            qualifiers,
            batchSize,
            hints,
            statementBuilder)
    {
        Type = type;
    }

    /// <summary>
    /// Creates a new instance of <see cref="UpdateAllRequest"/> object.
    /// </summary>
    /// <param name="name">The name of the request.</param>
    /// <param name="connection">The connection object.</param>
    /// <param name="transaction">The transaction object.</param>
    /// <param name="fields">The list of the target fields.</param>
    /// <param name="qualifiers">The list of the qualifier <see cref="Field"/> objects.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="hints">The hints for the table.</param>
    /// <param name="statementBuilder">The statement builder.</param>
    public UpdateAllRequest(string name,
        IDbConnection connection,
        IDbTransaction? transaction,
        IEnumerable<Field> fields,
        IEnumerable<Field>? qualifiers,
        int batchSize,
        string? hints = null,
        IStatementBuilder? statementBuilder = null)
        : base(name,
            connection,
            transaction,
            statementBuilder)
    {
        Fields = fields.AsList();
        Qualifiers = qualifiers?.AsList();
        BatchSize = batchSize;
        Hints = hints;
    }

    /// <summary>
    /// Gets the target fields.
    /// </summary>
    public List<Field> Fields { get; init; }

    /// <summary>
    /// Gets the qualifiers fields.
    /// </summary>
    public List<Field>? Qualifiers { get; init; }

    /// <summary>
    /// Gets the size batch of the update operation.
    /// </summary>
    public int BatchSize { get; init; }

    /// <summary>
    /// Gets the hints for the table.
    /// </summary>
    public string? Hints { get; }

    #region Equality and comparers

    /// <summary>
    /// Returns the hashcode for this <see cref="UpdateAllRequest"/>.
    /// </summary>
    /// <returns>The hashcode value.</returns>
    public override int GetHashCode()
    {
        if (this.HashCode is not { } hashCode)
        {
            // Get first the entity hash code
            hashCode = System.HashCode.Combine(
                typeof(UpdateAllRequest),
                Name,
                BatchSize,
                Hints);

            // Get the fields
            if (Fields != null)
            {
                foreach (var field in Fields)
                {
                    hashCode = System.HashCode.Combine(hashCode, field);
                }
            }

            // Get the qualifier <see cref="Field"/> objects
            if (Qualifiers != null)
            {
                foreach (var field in Qualifiers)
                {
                    hashCode = System.HashCode.Combine(hashCode, field);
                }
            }

            HashCode = hashCode;
        }

        return hashCode;
    }

    protected override bool StrictEquals(BaseRequest other)
    {
        // TODO: Implement Equals() and use from here.
        return other is UpdateAllRequest;
    }

    #endregion
}
