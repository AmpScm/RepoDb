using System.Data;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb.Requests;

/// <summary>
/// A class that holds the value of the 'QueryMultiple' operation arguments.
/// </summary>
internal sealed class QueryMultipleRequest : BaseRequest
{
    /// <summary>
    /// Creates a new instance of <see cref="QueryMultipleRequest"/> object.
    /// </summary>
    /// <param name="index">The index value.</param>
    /// <param name="type">The target type.</param>
    /// <param name="connection">The connection object.</param>
    /// <param name="transaction">The transaction object.</param>
    /// <param name="fields">The list of the target fields.</param>
    /// <param name="where">The query expression.</param>
    /// <param name="orderBy">The list of order fields.</param>
    /// <param name="top">The filter for the rows.</param>
    /// <param name="hints">The hints for the table.</param>
    /// <param name="statementBuilder">The statement builder.</param>
    public QueryMultipleRequest(int? index,
        Type type,
        IDbConnection connection,
        IDbTransaction? transaction,
        IEnumerable<Field> fields,
        QueryGroup? where = null,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        IStatementBuilder? statementBuilder = null)
        : this(index,
              ClassMappedNameCache.Get(type),
              connection,
              transaction,
              fields,
              where,
              orderBy,
              top,
              hints,
              statementBuilder)
    {
        Type = type;
    }

    /// <summary>
    /// Creates a new instance of <see cref="QueryMultipleRequest"/> object.
    /// </summary>
    /// <param name="index">The index value.</param>
    /// <param name="name">The name of the request.</param>
    /// <param name="connection">The connection object.</param>
    /// <param name="transaction">The transaction object.</param>
    /// <param name="fields">The list of the target fields.</param>
    /// <param name="where">The query expression.</param>
    /// <param name="orderBy">The list of order fields.</param>
    /// <param name="top">The filter for the rows.</param>
    /// <param name="hints">The hints for the table.</param>
    /// <param name="statementBuilder">The statement builder.</param>
    public QueryMultipleRequest(int? index,
        string name,
        IDbConnection connection,
        IDbTransaction? transaction,
        IEnumerable<Field> fields,
        QueryGroup? where = null,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null,
        IStatementBuilder? statementBuilder = null)
        : base(name,
              connection,
              transaction,
              statementBuilder)
    {
        Index = index;
        Fields = fields.AsList();
        Where = where;
        OrderBy = orderBy?.AsList();
        Top = top;
        Hints = hints;
    }

    /// <summary>
    /// Gets the index used.
    /// </summary>
    public int? Index { get; }

    /// <summary>
    /// Gets the list of the target fields.
    /// </summary>
    public List<Field> Fields { get; init; }

    /// <summary>
    /// Gets the query expression used.
    /// </summary>
    public QueryGroup? Where { get; }

    /// <summary>
    /// Gets the list of the order fields.
    /// </summary>
    public IEnumerable<OrderField>? OrderBy { get; }

    /// <summary>
    /// Gets the filter for the rows.
    /// </summary>
    public int Top { get; }

    /// <summary>
    /// Gets the hints for the table.
    /// </summary>
    public string? Hints { get; }

    #region Equality and comparers

    /// <summary>
    /// Returns the hashcode for this <see cref="QueryMultipleRequest"/>.
    /// </summary>
    /// <returns>The hashcode value.</returns>
    public override int GetHashCode()
    {
        if (HashCode is not { } hashCode)
        {
            // Get first the entity hash code
            hashCode = System.HashCode.Combine(
                typeof(QueryMultipleRequest),
                Name,
                Index,
                Where,
                Top,
                Hints);


            // Get the qualifier <see cref="Field"/> objects
            if (Fields != null)
            {
                foreach (var field in Fields)
                {
                    hashCode = System.HashCode.Combine(hashCode, field);
                }
            }

            // Add the order fields
            if (OrderBy != null)
            {
                foreach (var orderField in OrderBy)
                {
                    hashCode = System.HashCode.Combine(hashCode, orderField);
                }
            }

            HashCode = hashCode;
        }

        return hashCode;
    }

    protected override bool StrictEquals(BaseRequest other)
    {
        // TODO: Implement Equals() and use from here.
        return other is QueryMultipleRequest;
    }

    #endregion
}
