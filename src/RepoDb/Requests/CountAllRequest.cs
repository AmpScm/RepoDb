using System.Data;
using RepoDb.Interfaces;

namespace RepoDb.Requests;

/// <summary>
/// A class that holds the value of the 'CountAll' operation arguments.
/// </summary>
internal sealed class CountAllRequest : BaseRequest
{
    /// <summary>
    /// Creates a new instance of <see cref="CountAllRequest"/> object.
    /// </summary>
    /// <param name="type">The target type.</param>
    /// <param name="connection">The connection object.</param>
    /// <param name="transaction">The transaction object.</param>
    /// <param name="hints">The hints for the table.</param>
    /// <param name="statementBuilder">The statement builder.</param>
    public CountAllRequest(Type type,
        IDbConnection connection,
        IDbTransaction? transaction,
        string? hints = null,
        IStatementBuilder? statementBuilder = null)
        : this(ClassMappedNameCache.Get(type),
              connection,
              transaction,
              hints,
              statementBuilder)
    {
        Type = type;
    }

    /// <summary>
    /// Creates a new instance of <see cref="CountAllRequest"/> object.
    /// </summary>
    /// <param name="name">The name of the request.</param>
    /// <param name="connection">The connection object.</param>
    /// <param name="transaction">The transaction object.</param>
    /// <param name="hints">The hints for the table.</param>
    /// <param name="statementBuilder">The statement builder.</param>
    public CountAllRequest(string name,
        IDbConnection connection,
        IDbTransaction? transaction,
        string? hints = null,
        IStatementBuilder? statementBuilder = null)
        : base(name,
              connection,
              transaction,
              statementBuilder)
    {
        Hints = hints;
    }

    /// <summary>
    /// Gets the hints for the table.
    /// </summary>
    public string? Hints { get; }

    #region Equality and comparers

    /// <summary>
    /// Returns the hashcode for this <see cref="CountAllRequest"/>.
    /// </summary>
    /// <returns>The hashcode value.</returns>
    public override int GetHashCode()
    {
        if (HashCode is not { } hashCode)
        {
            HashCode = hashCode = System.HashCode.Combine(
                typeof(CountAllRequest),
                Name,
                Hints);
        }

        return hashCode;
    }

    protected override bool StrictEquals(BaseRequest other)
    {
        // TODO: Implement Equals() and use from here.
        return other is CountAllRequest;
    }

    #endregion
}
