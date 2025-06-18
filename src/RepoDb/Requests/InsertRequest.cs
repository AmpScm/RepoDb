using System.Data;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb.Requests;

/// <summary>
/// A class that holds the value of the 'Insert' operation arguments.
/// </summary>
internal sealed class InsertRequest : BaseRequest
{
    /// <summary>
    /// Creates a new instance of <see cref="InsertRequest"/> object.
    /// </summary>
    /// <param name="type">The target type.</param>
    /// <param name="connection">The connection object.</param>
    /// <param name="transaction">The transaction object.</param>
    /// <param name="fields">The list of the target fields.</param>
    /// <param name="hints">The hints for the table.</param>
    /// <param name="statementBuilder">The statement builder.</param>
    public InsertRequest(Type type,
        IDbConnection connection,
        IDbTransaction? transaction,
        IEnumerable<Field> fields,
        string? hints = null,
        IStatementBuilder? statementBuilder = null)
        : this(type,
            ClassMappedNameCache.Get(type),
            connection,
            transaction,
            fields,
            hints,
            statementBuilder)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="InsertRequest"/> object.
    /// </summary>
    /// <param name="name">The name of the request.</param>
    /// <param name="connection">The connection object.</param>
    /// <param name="transaction">The transaction object.</param>
    /// <param name="fields">The list of the target fields.</param>
    /// <param name="hints">The hints for the table.</param>
    /// <param name="statementBuilder">The statement builder.</param>
    public InsertRequest(string name,
        IDbConnection connection,
        IDbTransaction? transaction,
        IEnumerable<Field> fields,
        string? hints = null,
        IStatementBuilder? statementBuilder = null)
        : this(null,
            name,
            connection,
            transaction,
            fields,
            hints,
            statementBuilder)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="InsertRequest"/> object.
    /// </summary>
    /// <param name="type">The target type.</param>
    /// <param name="name">The name of the request.</param>
    /// <param name="connection">The connection object.</param>
    /// <param name="transaction">The transaction object.</param>
    /// <param name="fields">The list of the target fields.</param>
    /// <param name="hints">The hints for the table.</param>
    /// <param name="statementBuilder">The statement builder.</param>
    public InsertRequest(Type? type,
        string name,
        IDbConnection connection,
        IDbTransaction? transaction,
        IEnumerable<Field> fields,
        string? hints = null,
        IStatementBuilder? statementBuilder = null)
        : base(name ?? ClassMappedNameCache.Get(type),
            connection,
            transaction,
            statementBuilder)
    {
        Type = type;
        Fields = fields.AsList();
        Hints = hints;
    }

    /// <summary>
    /// Gets the target fields.
    /// </summary>
    public IEnumerable<Field> Fields { get; init; }

    /// <summary>
    /// Gets the hints for the table.
    /// </summary>
    public string? Hints { get; }

    #region Equality and comparers

    /// <summary>
    /// Returns the hashcode for this <see cref="InsertRequest"/>.
    /// </summary>
    /// <returns>The hashcode value.</returns>
    public override int GetHashCode()
    {
        if (HashCode is not { } hashCode)
        {
            // Get first the entity hash code
            hashCode = System.HashCode.Combine(
                typeof(InsertRequest),
                Name,
                Hints);

            // Get the qualifier <see cref="Field"/> objects
            if (Fields != null)
            {
                foreach (var field in Fields)
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
        return other is InsertRequest;
    }

    #endregion
}
