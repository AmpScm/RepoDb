using RepoDb.Enumerations;

namespace RepoDb;

public partial class QueryGroup
{
    /// <summary>
    /// Combine the current <see cref="QueryGroup"/> with another <see cref="QueryGroup"/> using the AND conjunction.
    /// </summary>
    /// <param name="queryGroup"></param>
    /// <returns></returns>
    public QueryGroup And(QueryGroup queryGroup)
    {
        ArgumentNullException.ThrowIfNull(queryGroup);

        return new QueryGroup([this, queryGroup], Conjunction.And);
    }

    /// <summary>
    /// Combine the querygroups using an AND conjunction.
    /// </summary>
    /// <param name="queryGroups"></param>
    /// <returns></returns>
    public static QueryGroup And(params IEnumerable<QueryGroup> queryGroups)
    {
        ArgumentNullException.ThrowIfNull(queryGroups);
        return new QueryGroup(queryGroups, Conjunction.And);
    }

    /// <summary>
    /// Combine the queryfields using an AND conjunction.
    /// </summary>
    /// <param name="queryFields"></param>
    /// <returns></returns>
    public static QueryGroup And(params IEnumerable<QueryField> queryFields)
    {
        ArgumentNullException.ThrowIfNull(queryFields);
        return new QueryGroup(queryFields, Conjunction.And);
    }

    /// <summary>
    /// Combine the current <see cref="QueryGroup"/> with another <see cref="QueryGroup"/> using the OR conjunction.
    /// </summary>
    /// <param name="queryGroup"></param>
    /// <returns></returns>
    public QueryGroup Or(QueryGroup queryGroup)
    {
        ArgumentNullException.ThrowIfNull(queryGroup);
        return new QueryGroup([this, queryGroup], Conjunction.Or);
    }

    /// <summary>
    /// Combine the querygroups using an OR conjunction.
    /// </summary>
    /// <param name="queryGroups"></param>
    /// <returns></returns>
    public static QueryGroup Or(params IEnumerable<QueryGroup> queryGroups)
    {
        ArgumentNullException.ThrowIfNull(queryGroups);
        return new QueryGroup(queryGroups, Conjunction.Or);
    }

    /// <summary>
    /// Combine the queryfields using an OR conjunction.
    /// </summary>
    /// <param name="queryFields"></param>
    /// <returns></returns>
    public static QueryGroup Or(params IEnumerable<QueryField> queryFields)
    {
        ArgumentNullException.ThrowIfNull(queryFields);
        return new QueryGroup(queryFields, Conjunction.Or);
    }

    /// <summary>
    /// Negate the current <see cref="QueryGroup"/>.
    /// </summary>
    /// <returns></returns>
    public QueryGroup Not(bool isNot = true)
    {
        if (!isNot)
            return this;
        else
            return new QueryGroup([this], isNot: true);
    }

    /// <summary>
    /// Combine one <see cref="QueryGroup"/> with another <see cref="QueryGroup"/> using the AND conjunction.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static QueryGroup operator &(QueryGroup left, QueryGroup right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        return left.And(right);
    }

    /// <summary>
    /// Combine one <see cref="QueryGroup"/> with another <see cref="QueryGroup"/> using the OR conjunction.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static QueryGroup operator |(QueryGroup left, QueryGroup right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        return left.Or(right);
    }

    /// <summary>
    /// Negate the current <see cref="QueryGroup"/>.
    /// </summary>
    /// <param name="queryGroup"></param>
    /// <returns></returns>
    public static QueryGroup operator !(QueryGroup queryGroup)
    {
        ArgumentNullException.ThrowIfNull(queryGroup);
        return queryGroup.Not();
    }
}
