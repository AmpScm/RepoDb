using System.Data;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb.Requests;

/// <summary>
/// A class that holds the value of the 'Merge' operation arguments.
/// </summary>
internal sealed class MergeRequest : BaseRequest
{
    /// <summary>
    /// Creates a new instance of <see cref="MergeRequest"/> object.
    /// </summary>
    /// <param name="type">The target type.</param>
    /// <param name="connection">The connection object.</param>
    /// <param name="transaction">The transaction object.</param>
    /// <param name="fields">The list of the target fields.</param>
    /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects.</param>
    /// <param name="hints">The hints for the table.</param>
    /// <param name="statementBuilder">The statement builder.</param>
    public MergeRequest(Type type,
        IDbConnection connection,
        IDbTransaction? transaction,
        IEnumerable<Field> fields,
        IEnumerable<Field>? noUpdateFields,
        IEnumerable<Field> qualifiers,
        string? hints = null,
        IStatementBuilder? statementBuilder = null)
        : this(type,
            ClassMappedNameCache.Get(type),
            connection,
            transaction,
            fields,
            noUpdateFields,
            qualifiers,
            hints,
            statementBuilder)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="MergeRequest"/> object.
    /// </summary>
    /// <param name="name">The name of the request.</param>
    /// <param name="connection">The connection object.</param>
    /// <param name="transaction">The transaction object.</param>
    /// <param name="fields">The list of the target fields.</param>
    /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects.</param>
    /// <param name="hints">The hints for the table.</param>
    /// <param name="statementBuilder">The statement builder.</param>
    public MergeRequest(string name,
        IDbConnection connection,
        IDbTransaction? transaction,
        IEnumerable<Field> fields,
        IEnumerable<Field>? noUpdateFields,
        IEnumerable<Field> qualifiers,
        string? hints = null,
        IStatementBuilder? statementBuilder = null)
        : this(null,
            name,
            connection,
            transaction,
            fields,
            noUpdateFields,
            qualifiers,
            hints,
            statementBuilder)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="MergeRequest"/> object.
    /// </summary>
    /// <param name="type">The target type.</param>
    /// <param name="name">The name of the request.</param>
    /// <param name="connection">The connection object.</param>
    /// <param name="transaction">The transaction object.</param>
    /// <param name="fields">The list of the target fields.</param>
    /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects.</param>
    /// <param name="hints">The hints for the table.</param>
    /// <param name="statementBuilder">The statement builder.</param>
    public MergeRequest(Type? type,
        string? name,
        IDbConnection connection,
        IDbTransaction? transaction,
        IEnumerable<Field> fields,
        IEnumerable<Field>? noUpdateFields,
        IEnumerable<Field> qualifiers,
        string? hints = null,
        IStatementBuilder? statementBuilder = null)
        : base(name ?? ClassMappedNameCache.Get(type!)!,
            connection,
            transaction,
            statementBuilder)
    {
        Type = type;
        Fields = fields.AsList();
        noUpdateFields = noUpdateFields?.AsList();
        Qualifiers = qualifiers.AsList();
        Hints = hints;
    }

    /// <summary>
    /// Gets the list of the target fields.
    /// </summary>
    public IEnumerable<Field> Fields { get; init; }

    public IEnumerable<Field>? NoUpdateFields { get; init; }

    /// <summary>
    /// Gets the qualifier <see cref="Field"/> objects.
    /// </summary>
    public IEnumerable<Field> Qualifiers { get; init; }

    /// <summary>
    /// Gets the hints for the table.
    /// </summary>
    public string? Hints { get; }

    #region Equality and comparers

    /// <summary>
    /// Returns the hashcode for this <see cref="MergeRequest"/>.
    /// </summary>
    /// <returns>The hashcode value.</returns>
    public override int GetHashCode()
    {
        if (this.HashCode is not { } hashCode)
        {
            // Get first the entity hash code
            hashCode = System.HashCode.Combine(
                typeof(MergeRequest),
                Name,
                Hints,
                Fields.Count(),
                NoUpdateFields?.Count() ?? 0);

            // Get the qualifier <see cref="Field"/> objects
            if (Fields != null)
            {
                foreach (var field in Fields)
                {
                    hashCode = System.HashCode.Combine(hashCode, field);
                }
            }

            if (NoUpdateFields != null)
            {
                foreach (var field in NoUpdateFields)
                {
                    hashCode = System.HashCode.Combine(hashCode, field);
                }
            }

            // Get the qualifier <see cref="Field"/> objects
            if (Qualifiers != null) // Much faster than Qualifiers?.<Methods|Properties>
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
        return other is MergeRequest;
    }

    #endregion
}
