using System.Data;
using RepoDb.Interfaces;

namespace RepoDb.Requests;

/// <summary>
/// A class that holds the value of the 'MinAll' operation arguments.
/// </summary>
internal sealed class MinAllRequest : BaseRequest
{
    /// <summary>
    /// Creates a new instance of <see cref="MinAllRequest"/> object.
    /// </summary>
    /// <param name="type">The target type.</param>
    /// <param name="connection">The connection object.</param>
    /// <param name="transaction">The transaction object.</param>
    /// <param name="field">The field object.</param>
    /// <param name="hints">The hints for the table.</param>
    /// <param name="statementBuilder">The statement builder.</param>
    public MinAllRequest(Type type,
        IDbConnection connection,
        IDbTransaction? transaction,
        Field field,
        string? hints = null,
        IStatementBuilder? statementBuilder = null)
        : this(ClassMappedNameCache.Get(type),
              connection,
              transaction,
              field,
              hints,
              statementBuilder)
    {
        Type = type;
    }

    /// <summary>
    /// Creates a new instance of <see cref="MinAllRequest"/> object.
    /// </summary>
    /// <param name="name">The name of the request.</param>
    /// <param name="connection">The connection object.</param>
    /// <param name="transaction">The transaction object.</param>
    /// <param name="field">The field object.</param>
    /// <param name="hints">The hints for the table.</param>
    /// <param name="statementBuilder">The statement builder.</param>
    public MinAllRequest(string name,
        IDbConnection connection,
        IDbTransaction? transaction,
        Field field,
        string? hints = null,
        IStatementBuilder? statementBuilder = null)
        : base(name,
              connection,
              transaction,
              statementBuilder)
    {
        Field = field;
        Hints = hints;
    }

    /// <summary>
    /// Gets the field to be minimized.
    /// </summary>
    public Field Field { get; }

    /// <summary>
    /// Gets the hints for the table.
    /// </summary>
    public string? Hints { get; }

    #region Equality and comparers

    /// <summary>
    /// Returns the hashcode for this <see cref="MinAllRequest"/>.
    /// </summary>
    /// <returns>The hashcode value.</returns>
    public override int GetHashCode()
    {
        if (this.HashCode is not { } hashCode)
        {
            // Get first the entity hash code
            HashCode = hashCode = System.HashCode.Combine(
                typeof(MinAllRequest),
                Name,
                Field,
                Hints);
        }

        return hashCode;
    }

    protected override bool StrictEquals(BaseRequest other)
    {
        // TODO: Implement Equals() and use from here.
        return other is MinAllRequest;
    }

    #endregion
}
