using System.Data;
using RepoDb.Interfaces;

namespace RepoDb.Requests;

/// <summary>
/// A class that holds the value of the 'SumAll' operation arguments.
/// </summary>
internal sealed class SumAllRequest : BaseRequest
{
    /// <summary>
    /// Creates a new instance of <see cref="SumAllRequest"/> object.
    /// </summary>
    /// <param name="type">The target type.</param>
    /// <param name="connection">The connection object.</param>
    /// <param name="transaction">The transaction object.</param>
    /// <param name="field">The field object.</param>
    /// <param name="hints">The hints for the table.</param>
    /// <param name="statementBuilder">The statement builder.</param>
    public SumAllRequest(Type type,
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
    /// Creates a new instance of <see cref="SumAllRequest"/> object.
    /// </summary>
    /// <param name="name">The name of the request.</param>
    /// <param name="connection">The connection object.</param>
    /// <param name="transaction">The transaction object.</param>
    /// <param name="field">The field object.</param>
    /// <param name="hints">The hints for the table.</param>
    /// <param name="statementBuilder">The statement builder.</param>
    public SumAllRequest(string name,
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
    /// Gets the field to be summarized.
    /// </summary>
    public Field Field { get; }

    /// <summary>
    /// Gets the hints for the table.
    /// </summary>
    public string? Hints { get; }

    #region Equality and comparers

    /// <summary>
    /// Returns the hashcode for this <see cref="SumAllRequest"/>.
    /// </summary>
    /// <returns>The hashcode value.</returns>
    public override int GetHashCode()
    {
        if (this.HashCode is not { } hashCode)
        {
            // Get first the entity hash code
            hashCode = System.HashCode.Combine(
                typeof(SumAllRequest),
                Name,
                Field,
                Hints);
        }

        return this.HashCode ??= hashCode;
    }

    protected override bool StrictEquals(BaseRequest other)
    {
        // TODO: Implement Equals() and use from here.
        return other is SumAllRequest;
    }

    #endregion
}
