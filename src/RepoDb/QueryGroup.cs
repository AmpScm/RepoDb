using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb;

/// <summary>
/// A widely-used class for defining the groupings when composing the query expression. This object is used by most operations
/// to define the filters and expressions on the actual execution.
/// </summary>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)},nq}}")]
public partial class QueryGroup : IEquatable<QueryGroup>
{
    private bool isFixed;
    private int? hashCode;

    #region Constructors

    /** QueryField **/

    /// <summary>
    /// Creates a new instance of <see cref="QueryGroup"/> object.
    /// </summary>
    /// <param name="queryField">The field to be grouped for the query expression.</param>
    public QueryGroup(QueryField queryField) :
        this(queryField,
            Conjunction.And,
            false)
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="QueryGroup"/> object.
    /// </summary>
    /// <param name="queryField">The field to be grouped for the query expression.</param>
    /// <param name="queryGroup">The child query group to be grouped for the query expression.</param>
    public QueryGroup(QueryField? queryField, QueryGroup? queryGroup) :
        this(queryField?.AsEnumerable(),
            queryGroup?.AsEnumerable(),
            Conjunction.And,
            false)
    {
        ArgumentNullException.ThrowIfNull(queryField);
    }

    /// <summary>
    /// Creates a new instance of <see cref="QueryGroup"/> object.
    /// </summary>
    /// <param name="queryField">The field to be grouped for the query expression.</param>
    /// <param name="conjunction">The conjunction to be used for every group separation.</param>
    public QueryGroup(QueryField queryField, Conjunction conjunction) :
        this(queryField,
            conjunction,
            false)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="QueryGroup"/> object.
    /// </summary>
    /// <param name="queryField">The field to be grouped for the query expression.</param>
    /// <param name="isNot">The prefix to be added whether the field value is in opposite state.</param>
    public QueryGroup(QueryField queryField, bool isNot) :
        this([queryField ?? throw new ArgumentNullException(nameof(queryField))],
            (IEnumerable<QueryGroup>?)null,
            Conjunction.And,
            isNot)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="QueryGroup"/> object.
    /// </summary>
    /// <param name="queryField">The field to be grouped for the query expression.</param>
    /// <param name="conjunction">The conjunction to be used for every group separation.</param>
    /// <param name="isNot">The prefix to be added whether the field value is in opposite state.</param>
    public QueryGroup(QueryField queryField,
        Conjunction conjunction,
        bool isNot)
    {
        ArgumentNullException.ThrowIfNull(queryField);
        QueryGroups = null;
        if (isNot && GlobalConfiguration.Options.QueryGroupOptimization > QueryGroupOptimization.None && queryField.Negate() is { } negated)
        {
            queryField = negated;
            isNot = false;
        }

        QueryFields = [queryField];
        Conjunction = conjunction;
        IsNot = isNot;
    }

    /** QueryFields **/

    /// <summary>
    /// Creates a new instance of <see cref="QueryGroup"/> object.
    /// </summary>
    /// <param name="queryFields">The list of fields to be grouped for the query expression.</param>
    public QueryGroup(IEnumerable<QueryField> queryFields)
        : this(queryFields ?? throw new ArgumentNullException(nameof(queryFields)),
            (IEnumerable<QueryGroup>?)null,
            Conjunction.And,
            false)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="QueryGroup"/> object.
    /// </summary>
    /// <param name="queryFields">The list of fields to be grouped for the query expression.</param>
    /// <param name="queryGroup">The child query group to be grouped for the query expression.</param>
    public QueryGroup(IEnumerable<QueryField>? queryFields, QueryGroup? queryGroup)
        : this(queryFields,
            queryGroup?.AsEnumerable(),
            Conjunction.And,
            false)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="QueryGroup"/> object.
    /// </summary>
    /// <param name="queryFields">The list of fields to be grouped for the query expression.</param>
    /// <param name="conjunction">The conjunction to be used for every group separation.</param>
    public QueryGroup(IEnumerable<QueryField> queryFields, Conjunction conjunction)
        : this(queryFields,
            (IEnumerable<QueryGroup>?)null,
            conjunction,
            false)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="QueryGroup"/> object.
    /// </summary>
    /// <param name="queryFields">The list of fields to be grouped for the query expression.</param>
    /// <param name="isNot">The prefix to be added whether the field value is in opposite state.</param>
    public QueryGroup(IEnumerable<QueryField> queryFields, bool isNot)
        : this(queryFields,
            (IEnumerable<QueryGroup>?)null,
            Conjunction.And,
            isNot)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="QueryGroup"/> object.
    /// </summary>
    /// <param name="queryFields">The list of fields to be grouped for the query expression.</param>
    /// <param name="conjunction">The conjunction to be used for every group separation.</param>
    /// <param name="isNot">The prefix to be added whether the field value is in opposite state.</param>
    public QueryGroup(IEnumerable<QueryField> queryFields, Conjunction conjunction, bool isNot) :
        this(queryFields,
            (IEnumerable<QueryGroup>?)null,
            conjunction,
            isNot)
    { }

    /** QueryGroup **/

    /// <summary>
    /// Creates a new instance of <see cref="QueryGroup"/> object.
    /// </summary>
    /// <param name="queryGroup">The child query group to be grouped for the query expression.</param>
    public QueryGroup(QueryGroup queryGroup)
        : this((IEnumerable<QueryField>?)null,
            [queryGroup ?? throw new ArgumentNullException(nameof(queryGroup))],
            Conjunction.And,
            false)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="QueryGroup"/> object.
    /// </summary>
    /// <param name="queryGroup">The child query group to be grouped for the query expression.</param>
    /// <param name="conjunction">The conjunction to be used for every group separation.</param>
    [Obsolete("Don't set a conjunction if there is only one member")]
    public QueryGroup(QueryGroup queryGroup, Conjunction conjunction)
        : this((IEnumerable<QueryField>?)null,
            [queryGroup ?? throw new ArgumentNullException(nameof(queryGroup))],
            conjunction,
            false)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="QueryGroup"/> object.
    /// </summary>
    /// <param name="queryGroup">The child query group to be grouped for the query expression.</param>
    /// <param name="isNot">The prefix to be added whether the field value is in opposite state.</param>
    public QueryGroup(QueryGroup queryGroup,
        bool isNot) :
        this((IEnumerable<QueryField>?)null,
            [queryGroup ?? throw new ArgumentNullException(nameof(queryGroup))],
            Conjunction.And,
            isNot)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="QueryGroup"/> object.
    /// </summary>
    /// <param name="queryGroup">The child query group to be grouped for the query expression.</param>
    /// <param name="conjunction">The conjunction to be used for every group separation.</param>
    /// <param name="isNot">The prefix to be added whether the field value is in opposite state.</param>
    [Obsolete("Don't set a conjunction if there is only one member")]
    public QueryGroup(QueryGroup queryGroup,
        Conjunction conjunction,
        bool isNot) :
        this((IEnumerable<QueryField>?)null,
            [queryGroup ?? throw new ArgumentNullException(nameof(queryGroup))],
            conjunction,
            isNot)
    { }

    /** QueryGroups **/

    /// <summary>
    /// Creates a new instance of <see cref="QueryGroup"/> object.
    /// </summary>
    /// <param name="queryGroups">The child query groups to be grouped for the query expression.</param>
    public QueryGroup(IEnumerable<QueryGroup> queryGroups) :
        this((IEnumerable<QueryField>?)null,
            queryGroups ?? throw new ArgumentNullException(nameof(queryGroups)),
            Conjunction.And,
            false)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="QueryGroup"/> object.
    /// </summary>
    /// <param name="queryGroups">The child query groups to be grouped for the query expression.</param>
    /// <param name="conjunction">The conjunction to be used for every group separation.</param>
    public QueryGroup(IEnumerable<QueryGroup> queryGroups,
        Conjunction conjunction) :
        this((IEnumerable<QueryField>?)null,
            queryGroups ?? throw new ArgumentNullException(nameof(queryGroups)),
            conjunction,
            false)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="QueryGroup"/> object.
    /// </summary>
    /// <param name="queryGroups">The child query groups to be grouped for the query expression.</param>
    /// <param name="isNot">The prefix to be added whether the field value is in opposite state.</param>
    public QueryGroup(IEnumerable<QueryGroup> queryGroups,
        bool isNot) :
        this((IEnumerable<QueryField>?)null,
            queryGroups ?? throw new ArgumentNullException(nameof(queryGroups)),
            Conjunction.And,
            isNot)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="QueryGroup"/> object.
    /// </summary>
    /// <param name="queryGroups">The child query groups to be grouped for the query expression.</param>
    /// <param name="conjunction">The conjunction to be used for every group separation.</param>
    /// <param name="isNot">The prefix to be added whether the field value is in opposite state.</param>
    public QueryGroup(IEnumerable<QueryGroup> queryGroups,
        Conjunction conjunction,
        bool isNot) :
        this((IEnumerable<QueryField>?)null,
            queryGroups ?? throw new ArgumentNullException(nameof(queryGroups)),
            conjunction,
            isNot)
    { }

    /** QueryField / QueryGroup **/

    /// <summary>
    /// Creates a new instance of <see cref="QueryGroup"/> object.
    /// </summary>
    /// <param name="queryField">The field to be grouped for the query expression.</param>
    /// <param name="queryGroup">The child query group to be grouped for the query expression.</param>
    /// <param name="conjunction">The conjunction to be used for every group separation.</param>
    public QueryGroup(QueryField? queryField,
        QueryGroup? queryGroup,
        Conjunction conjunction) :
        this(queryField?.AsEnumerable(),
            queryGroup?.AsEnumerable(),
            conjunction,
            false)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="QueryGroup"/> object.
    /// </summary>
    /// <param name="queryField">The field to be grouped for the query expression.</param>
    /// <param name="queryGroup">The child query group to be grouped for the query expression.</param>
    /// <param name="isNot">The prefix to be added whether the field value is in opposite state.</param>
    public QueryGroup(QueryField? queryField,
        QueryGroup? queryGroup,
        bool isNot) :
        this(queryField?.AsEnumerable(),
            queryGroup?.AsEnumerable(),
            Conjunction.And,
            isNot)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="QueryGroup"/> object.
    /// </summary>
    /// <param name="queryField">The field to be grouped for the query expression.</param>
    /// <param name="queryGroup">The child query group to be grouped for the query expression.</param>
    /// <param name="conjunction">The conjunction to be used for every group separation.</param>
    /// <param name="isNot">The prefix to be added whether the field value is in opposite state.</param>
    public QueryGroup(QueryField? queryField,
        QueryGroup? queryGroup,
        Conjunction conjunction,
        bool isNot) :
        this(queryField?.AsEnumerable(),
            queryGroup?.AsEnumerable(),
            conjunction,
            isNot)
    { }

    /** QueryField / QueryGroups **/

    /// <summary>
    /// Creates a new instance of <see cref="QueryGroup"/> object.
    /// </summary>
    /// <param name="queryField">The field to be grouped for the query expression.</param>
    /// <param name="queryGroups">The child query groups to be grouped for the query expression.</param>
    public QueryGroup(QueryField? queryField,
        IEnumerable<QueryGroup>? queryGroups) :
        this(queryField?.AsEnumerable(),
            queryGroups,
            Conjunction.And,
            false)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="QueryGroup"/> object.
    /// </summary>
    /// <param name="queryField">The field to be grouped for the query expression.</param>
    /// <param name="queryGroups">The child query groups to be grouped for the query expression.</param>
    /// <param name="conjunction">The conjunction to be used for every group separation.</param>
    public QueryGroup(QueryField? queryField,
        IEnumerable<QueryGroup>? queryGroups,
        Conjunction conjunction) :
        this(queryField?.AsEnumerable(),
            queryGroups,
            conjunction,
            false)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="QueryGroup"/> object.
    /// </summary>
    /// <param name="queryField">The field to be grouped for the query expression.</param>
    /// <param name="queryGroups">The child query groups to be grouped for the query expression.</param>
    /// <param name="isNot">The prefix to be added whether the field value is in opposite state.</param>
    public QueryGroup(QueryField? queryField,
        IEnumerable<QueryGroup>? queryGroups,
        bool isNot) :
        this(queryField?.AsEnumerable(),
            queryGroups,
            Conjunction.And,
            isNot)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="QueryGroup"/> object.
    /// </summary>
    /// <param name="queryField">The field to be grouped for the query expression.</param>
    /// <param name="queryGroups">The child query groups to be grouped for the query expression.</param>
    /// <param name="conjunction">The conjunction to be used for every group separation.</param>
    /// <param name="isNot">The prefix to be added whether the field value is in opposite state.</param>
    public QueryGroup(QueryField? queryField,
        IEnumerable<QueryGroup>? queryGroups,
        Conjunction conjunction,
        bool isNot) :
        this(queryField?.AsEnumerable(),
            queryGroups,
            conjunction,
            isNot)
    { }

    ///** QueryFields / QueryGroup **/

    /// <summary>
    /// Creates a new instance of <see cref="QueryGroup"/> object.
    /// </summary>
    /// <param name="queryFields">The list of fields to be grouped for the query expression.</param>
    /// <param name="queryGroup">The child query group to be grouped for the query expression.</param>
    /// <param name="conjunction">The conjunction to be used for every group separation.</param>
    public QueryGroup(IEnumerable<QueryField>? queryFields,
        QueryGroup? queryGroup,
        Conjunction conjunction) :
        this(queryFields,
            queryGroup?.AsEnumerable(),
            conjunction,
            false)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="QueryGroup"/> object.
    /// </summary>
    /// <param name="queryFields">The list of fields to be grouped for the query expression.</param>
    /// <param name="queryGroup">The child query group to be grouped for the query expression.</param>
    /// <param name="isNot">The prefix to be added whether the field value is in opposite state.</param>
    public QueryGroup(IEnumerable<QueryField>? queryFields,
        QueryGroup? queryGroup,
        bool isNot) :
        this(queryFields,
            queryGroup?.AsEnumerable(),
            Conjunction.And,
            isNot)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="QueryGroup"/> object.
    /// </summary>
    /// <param name="queryFields">The list of fields to be grouped for the query expression.</param>
    /// <param name="queryGroup">The child query group to be grouped for the query expression.</param>
    /// <param name="conjunction">The conjunction to be used for every group separation.</param>
    /// <param name="isNot">The prefix to be added whether the field value is in opposite state.</param>
    public QueryGroup(IEnumerable<QueryField>? queryFields,
        QueryGroup? queryGroup,
        Conjunction conjunction,
        bool isNot) :
        this(queryFields,
            queryGroup?.AsEnumerable(),
            conjunction,
            isNot)
    { }

    /** QueryFields / QueryGroups **/

    /// <summary>
    /// Creates a new instance of <see cref="QueryGroup"/> object.
    /// </summary>
    /// <param name="queryFields">The list of fields to be grouped for the query expression.</param>
    /// <param name="queryGroups">The child query groups to be grouped for the query expression.</param>
    public QueryGroup(IEnumerable<QueryField>? queryFields,
        IEnumerable<QueryGroup>? queryGroups) :
        this(queryFields,
            queryGroups,
            Conjunction.And,
            false)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="QueryGroup"/> object.
    /// </summary>
    /// <param name="queryFields">The list of fields to be grouped for the query expression.</param>
    /// <param name="queryGroups">The child query groups to be grouped for the query expression.</param>
    /// <param name="conjunction">The conjunction to be used for every group separation.</param>
    public QueryGroup(IEnumerable<QueryField>? queryFields,
        IEnumerable<QueryGroup>? queryGroups,
        Conjunction conjunction) :
        this(queryFields,
            queryGroups,
            conjunction,
            false)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="QueryGroup"/> object.
    /// </summary>
    /// <param name="queryFields">The list of fields to be grouped for the query expression.</param>
    /// <param name="queryGroups">The child query groups to be grouped for the query expression.</param>
    /// <param name="isNot">The prefix to be added whether the field value is in opposite state.</param>
    public QueryGroup(IEnumerable<QueryField>? queryFields,
        IEnumerable<QueryGroup>? queryGroups,
        bool isNot) :
        this(queryFields,
            queryGroups,
            Conjunction.And,
            isNot)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="QueryGroup"/> object.
    /// </summary>
    /// <param name="queryFields">The list of fields to be grouped for the query expression.</param>
    /// <param name="queryGroups">The child query groups to be grouped for the query expression.</param>
    /// <param name="conjunction">The conjunction to be used for every group separation.</param>
    /// <param name="isNot">The prefix to be added whether the field value is in opposite state.</param>
    public QueryGroup(IEnumerable<QueryField>? queryFields,
                  IEnumerable<QueryGroup>? queryGroups,
                  Conjunction conjunction,
                  bool isNot)
    {
        if (conjunction is not Conjunction.And and not Conjunction.Or)
            throw new ArgumentOutOfRangeException(nameof(conjunction));

        // Materialize inputs once
        var fields = queryFields?.ToList();
        var groups = queryGroups?.ToList();

        if ((fields == null || fields.Count == 0) &&
            (groups == null || groups.Count == 0))
            throw new ArgumentException("Can't create empty groups", nameof(queryFields));

        if (GlobalConfiguration.Options.QueryGroupOptimization is { } optimize &&
            optimize != QueryGroupOptimization.None)
        {
            //
            // STEP 0 — homogenize children: wrap fields into single-field groups
            // Use the pure leaf ctor so no normalization runs recursively
            //
            if (fields is { Count: > 0 })
            {
                var wrapped = fields.Select(f => new QueryGroup(f)); // leaf ctor
                groups = wrapped.Concat(groups ?? []).ToList();
                fields = null;
            }

            // Work with concrete lists from here on
            groups = groups!.ToList();

            //
            // 1 — Flatten trivial groups when safe
            // Safe when:
            //   - group is NOT negated
            //   - group has same conjunction as parent (associativity)
            //   - OR group contains exactly one element (idempotence)
            //
            if (groups is { Count: > 0 } &&
                groups.All(g =>
                    !g.IsNot &&
                    (g.Conjunction == conjunction ||
                     g.ItemCount == 1)))
            {
                fields = groups.SelectMany(g => g.QueryFields ?? []).ToList();
                groups = groups.SelectMany(g => g.QueryGroups ?? []).ToList();
            }

            //
            // 2 — NOT-related normalization (only when no fields present)
            //
            else if (fields == null || fields.Count == 0)
            {
                // CASE 1: NOT-NOT -> unwrap
                if (isNot && groups.Count == 1 && groups[0].IsNot)
                {
                    var inner = groups[0];
                    fields = inner.QueryFields?.ToList();
                    groups = inner.QueryGroups?.ToList();
                    conjunction = inner.Conjunction;
                    isNot = false;
                }
                // CASE 2: NOT (NOT A AND NOT B) -> A OR B
                else if (isNot && groups.Count > 1 && groups.All(g => g.IsNot))
                {
                    fields = groups.SelectMany(g => g.QueryFields ?? []).ToList();
                    groups = groups.SelectMany(g => g.QueryGroups ?? []).ToList();
                    isNot = false;
                    conjunction = conjunction == Conjunction.And ? Conjunction.Or : Conjunction.And;
                }
                // CASE 3/4: NOT (A OR B) or NOT (A AND B)
                // Push NOT into children
                else if (isNot && groups.Count > 1 && groups.All(g => !g.IsNot) &&
                         (conjunction == Conjunction.Or || conjunction == Conjunction.And))
                {
                    isNot = false;
                    conjunction = conjunction == Conjunction.And ? Conjunction.Or : Conjunction.And;

                    groups = groups.Select(g =>
                    {
                        var childFields = g.QueryFields;
                        var childGroups = g.QueryGroups;

                        // If child is a single-field leaf, negate the field immediately
                        var isLeafSingleField =
                            (childGroups == null || childGroups.Count == 0) &&
                            (childFields?.Count == 1);

                        if (isLeafSingleField)
                        {
                            var neg = childFields![0].Negate();
                            if (neg is not null)
                                return new QueryGroup(new[] { neg }); // leaf group with negated field
                        }

                        // Otherwise keep as a NOT group for later normalization
                        return new QueryGroup(childFields, childGroups, g.Conjunction, true);
                    }).ToList();
                }
                // CASE 5: single-group inheritance
                else if (groups.Count == 1)
                {
                    var from = groups[0];
                    fields = from.QueryFields?.ToList();
                    groups = from.QueryGroups?.ToList();
                    conjunction = from.Conjunction;
                    isNot ^= from.IsNot;
                }
            }

            //
            // 3 — Final flattening
            // Safe when:
            //   - group is NOT negated
            //   - group has same conjunction as parent
            //   - OR group has exactly one element (idempotence)
            //
            if (groups?.Any(g => !g.IsNot &&
                                 (g.Conjunction == conjunction || g.ItemCount == 1)) == true)
            {
                var flatten = groups
                    .Where(g => !g.IsNot &&
                                (g.Conjunction == conjunction || g.ItemCount == 1))
                    .ToList();

                groups = groups.Except(flatten)
                    .Concat(flatten.SelectMany(g => g.QueryGroups ?? []))
                    .ToList();

                fields = (fields ?? new List<QueryField>())
                    .Concat(flatten.SelectMany(g => g.QueryFields ?? []))
                    .ToList();
            }

            //
            // 4 — De Morgan for fields only (no groups)
            //
            if (isNot && (groups == null || groups.Count == 0) && fields is { Count: > 1 })
            {
                var altFields = fields.Select(f => f.Negate()).ToList();
                if (!altFields.Any(f => f is null))
                {
                    fields = altFields!;
                    isNot = false;
                    conjunction = conjunction == Conjunction.And ? Conjunction.Or : Conjunction.And;
                }
            }

            //
            // 5 — Single-field NOT simplification
            //
            if (isNot &&
                optimize == QueryGroupOptimization.Full &&
                fields is { Count: 1 } &&
                (groups == null || groups.Count == 0) &&
                fields[0].Negate() is { } negated)
            {
                fields = [negated];
                isNot = false;
                conjunction = Conjunction.And;
            }
        }

        // Final immutable assignment
        Conjunction = conjunction;
        QueryFields = fields?.Count > 0 ? fields : null;
        QueryGroups = groups?.Count > 0 ? groups : null;
        IsNot = isNot;
    }

    private int ItemCount =>
        (QueryFields?.Count ?? 0) + (QueryGroups?.Count ?? 0);


    #endregion

    #region Properties

    /// <summary>
    /// Gets the conjunction used by this object.
    /// </summary>
    public Conjunction Conjunction { get; }

    /// <summary>
    /// Gets the list of child <see cref="QueryField"/> objects.
    /// </summary>
    public IReadOnlyList<QueryField>? QueryFields { get; }

    /// <summary>
    /// Gets the list of child <see cref="QueryGroup"/> objects.
    /// </summary>
    public IReadOnlyList<QueryGroup>? QueryGroups { get; }

    /// <summary>
    /// Gets the value whether the grouping is in opposite field-value state.
    /// </summary>
    public bool IsNot { get; }

    #endregion

    #region Methods (Internal)

    /// <summary>
    /// Prepend an underscore on every parameter object.
    /// </summary>
    internal void PrependAnUnderscoreAtTheParameters(IDbSetting setting)
    {
        var queryFields = GetFields(true);
        if (queryFields?.Any() != true)
        {
            return;
        }
        foreach (var queryField in queryFields)
        {
            queryField.PrependAnUnderscoreAtParameter(setting);
        }
    }

    /// <summary>
    /// Fix the names of the parameters in every <see cref="QueryField"/> object of the target list of <see cref="QueryGroup"/>s.
    /// </summary>
    /// <param name="queryGroups">The list of query groups.</param>
    /// <returns>An instance of <see cref="QueryGroup"/> object containing all the fields.</returns>
    internal static void FixForQueryMultiple(IReadOnlyList<QueryGroup?> queryGroups)
    {
        for (var i = 0; i < queryGroups.Count; i++)
        {
            var fields = queryGroups[i]?.GetFields(true);

            if (fields?.Any() == true)
            {
                foreach (var field in fields)
                {
                    field.Parameter.SetName(string.Format(CultureInfo.InvariantCulture, "T{0}_{1}", i, field.Parameter.Name));
                }
            }
        }
    }

    /// <summary>
    /// Forces to set the <see cref="isFixed"/> variable to True.
    /// </summary>
    private void ForceIsFixedVariables()
    {
        if (QueryGroups?.Count > 0)
        {
            foreach (var queryGroup in QueryGroups)
            {
                queryGroup.ForceIsFixedVariables();
            }
        }
        isFixed = true;
    }

    /// <summary>
    /// Reset all the query fields.
    /// </summary>
    private void ResetQueryFields()
    {
        if (QueryFields?.Any() != true)
        {
            return;
        }
        foreach (var field in QueryFields)
        {
            field.Reset();
        }
    }

    /// <summary>
    /// Reset all the query groups.
    /// </summary>
    private void ResetQueryGroups()
    {
        if (QueryGroups is null || QueryGroups.Count == 0)
        {
            return;
        }
        foreach (var group in QueryGroups)
        {
            group.Reset();
        }
    }

    /// <summary>
    /// Fix the query fields names.
    /// </summary>
    /// <param name="fields"></param>
    private static void FixQueryFields(IEnumerable<QueryField> fields)
    {
        var firstList = fields
            .OrderBy(queryField => queryField.Parameter.Name, StringComparer.OrdinalIgnoreCase)
            .AsList();
        var secondList = new List<QueryField>(firstList);

        foreach (var firstQueryField in firstList)
        {
            for (var fieldIndex = 0; fieldIndex < secondList.Count; fieldIndex++)
            {
                var secondQueryField = secondList[fieldIndex];
                if (ReferenceEquals(firstQueryField, secondQueryField))
                {
                    continue;
                }
                if (firstQueryField.Field.Equals(secondQueryField.Field))
                {
                    var fieldValue = secondQueryField.Parameter;
                    fieldValue.SetName(string.Concat(secondQueryField.Parameter.Name, "_", fieldIndex.ToString(CultureInfo.InvariantCulture)));
                }
            }
            secondList.RemoveAll(qf => qf.Field.Equals(firstQueryField.Field));
        }
    }

    #endregion

    #region Methods (Public)

    /// <summary>
    /// Reset the <see cref="QueryGroup"/> back to its default state (as is newly instantiated).
    /// </summary>
    public void Reset()
    {
        ResetQueryFields();
        ResetQueryGroups();
        isFixed = false;
        hashCode = null;
    }

    /// <summary>
    /// Fix the names of the <see cref="Parameter"/> on every <see cref="QueryField"/> (and on every child <see cref="QueryGroup"/>) of the current <see cref="QueryGroup"/>.
    /// </summary>
    /// <returns>The current instance.</returns>
    public QueryGroup Fix()
    {
        Fix(null, null, null);
        return this;
    }

    internal void Fix(IDbConnection? connection, IDbTransaction? transaction, string? tableName)
    {
        if (isFixed)
            return;

        // Check the presence
        var fields = GetFields(true);

        // Check any item
        if (fields?.Any() != true)
            return;

        if (connection is { })
        {
            DbFieldCollection? dbFields = null;
            FixForDb(connection, transaction, tableName, ref dbFields);
        }

        // Fix the fields
        FixQueryFields(fields);

        // Force the variables
        ForceIsFixedVariables();
    }

    internal async ValueTask FixAsync(IDbConnection? connection, IDbTransaction? transaction, string? tableName, CancellationToken cancellationToken)
    {
        if (isFixed)
        {
            return;
        }

        // Check the presence
        var fields = GetFields(true);

        // Check any item
        if (fields?.Any() != true)
        {
            return;
        }

        if (connection is { } && tableName is { })
        {
            // Preload list async then use non async code. One time hit on first use of table
            DbFieldCollection? dbFields = await DbFieldCache.GetInternalAsync(connection, tableName, transaction, cancellationToken: cancellationToken).ConfigureAwait(false);

            FixForDb(connection, transaction, tableName, ref dbFields);
        }

        // Fix the fields
        FixQueryFields(fields);

        // Force the variables
        ForceIsFixedVariables();
    }

    private void FixForDb(IDbConnection connection, IDbTransaction? transaction, string? tableName, ref DbFieldCollection? dbFields)
    {
        if (QueryGroups is not null)
        {
            foreach (var qg in QueryGroups)
            {
                qg.FixForDb(connection, transaction, tableName, ref dbFields);
            }
        }

        if (QueryFields?.Count > 1 && Conjunction is Conjunction.Or or Conjunction.And)
        {
            bool isOr = Conjunction == Conjunction.Or;

            foreach (QueryField qf in QueryFields)
            {
                if (qf.CanSkip)
                {
                    dbFields ??= tableName is { } ? DbFieldCache.Get(connection, tableName, transaction) : null;

                    if (dbFields?.GetByFieldName(qf.Field.FieldName)?.IsNullable == false)
                    {
                        bool isIsNull = qf.Operation == Operation.IsNull;

                        // IsNotNul within OR -> Ignore
                        if (isOr && isIsNull)
                            qf.Skip = true;
                        else if (!isOr && !isIsNull)
                            qf.Skip = true;
                    }
                }
            }
        }

        if (QueryFields?.Count >= 1)
        {
            IDbSetting? dbs = null;
            IDbHelper? dbh = null;

            foreach (var qf in QueryFields)
            {
                if (qf.Operation is Operation.In or Operation.NotIn)
                {
                    var e = qf.Parameter.Value as System.Collections.IEnumerable;
                    var cnt = e?.AsTypedSet().Count;

                    if (cnt > 5 && (dbs ??= connection?.GetDbSetting()) is { } dbSetting)
                    {
                        qf.TableParameterMode =
                            dbSetting.UseArrayParameterTreshold < cnt
                            && (dbh ??= connection?.GetDbHelper()) is { } h
                            && h.CanCreateTableParameter(connection!, transaction, qf.Field.Type, e!);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Make the current instance of <see cref="QueryGroup"/> object to become an expression for 'Update' operations.
    /// </summary>
    public void IsForUpdate(IDbSetting setting) =>
        PrependAnUnderscoreAtTheParameters(setting);


    /// <summary>
    /// Gets the stringified query expression format of the current instance. A formatted string for field-operation-parameter will be
    /// conjuncted by the value of the <see cref="Conjunction"/> property.
    /// </summary>
    /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
    /// <returns>A stringified formatted-text of the current instance.</returns>
    public virtual string GetString(IDbSetting dbSetting) =>
        GetString(0, dbSetting);

    /// <summary>
    /// Gets the stringified query expression format of the current instance. A formatted string for field-operation-parameter will be
    /// conjuncted by the value of the <see cref="Enumerations.Conjunction"/> property.
    /// </summary>
    /// <param name="index">The parameter index for batch operation.</param>
    /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
    /// <returns>A stringified formatted-text of the current instance.</returns>
    public string GetString(int index, IDbSetting dbSetting)
    {
        // Fix first the parameters
        Fix();

        return RawGetString(index, dbSetting);
    }

    private string RawGetString(int index, IDbSetting? dbSetting)
    {
        // Variables
        var groupList = new List<string>();
        var conjunction = Conjunction.GetText();
        var separator = string.Concat(" ", conjunction, " ");

        // Check the instance fields
        if (QueryFields?.Count > 0)
        {
            var fields = QueryFields
                .Where(qf => !qf.Skip)
                .Select(qf =>
                    qf.GetString(index, dbSetting)).Join(separator);
            groupList.Add(fields);
        }

        // Check the instance groups
        if (QueryGroups?.Count > 0)
        {
            var groups = QueryGroups
                .Select(qg =>
                    qg.RawGetString(index, dbSetting!)).Join(separator);
            groupList.Add(groups);
        }

        // Return the value
        return string.Concat(IsNot ? "NOT (" : "(", groupList.Join(separator), ")");
    }

    /// <summary>
    /// Gets all the child <see cref="QueryField"/> objects associated on the current instance.
    /// </summary>
    /// <param name="traverse">Identify whether to explore all the children of the child <see cref="QueryGroup"/> objects.</param>
    /// <returns>An enumerable list of <see cref="QueryField"/> objects.</returns>
    public IEnumerable<QueryField>? GetFields(bool traverse)
    {
        if (!traverse)
        {
            return QueryFields?.Count > 0 ? QueryFields : null;
        }

        return RecursiveGetFields(this);

        static IEnumerable<QueryField> RecursiveGetFields(QueryGroup queryGroup)
        {
            if (queryGroup.QueryFields is { Count: > 0 } fields)
            {
                foreach (var field in fields)
                {
                    yield return field;
                }
            }

            if (queryGroup.QueryGroups is { Count: > 0 } groups)
            {
                foreach (var qg in groups)
                {
                    foreach (var childField in RecursiveGetFields(qg))
                    {
                        yield return childField;
                    }
                }
            }
        }
    }

    #endregion

    #region Equality and comparers

    /// <summary>
    /// Returns the hashcode for this <see cref="QueryGroup"/>.
    /// </summary>
    /// <returns>The hashcode value.</returns>
    public override int GetHashCode()
    {
        // Make sure to check if this is already taken
        if (this.hashCode != null)
        {
            return this.hashCode.Value;
        }

        var hashCode = 0;

        // Iterates the child query field
        if (QueryFields is not null)
        {
            foreach (var queryField in QueryFields)
            {
                hashCode = HashCode.Combine(hashCode, queryField);
            }
        }

        // Iterates the child query groups
        if (QueryGroups?.Count > 0)
        {
            foreach (var queryGroup in QueryGroups)
            {
                hashCode = HashCode.Combine(hashCode, queryGroup);
            }
        }

        // Set with conjunction
        hashCode = HashCode.Combine(hashCode, Conjunction);

        // Set the IsNot
        hashCode = HashCode.Combine(hashCode, IsNot);

        // Set and return the hashcode
        return this.hashCode ??= hashCode;
    }

    /// <summary>
    /// Compares the <see cref="QueryGroup"/> object equality against the given target object.
    /// </summary>
    /// <param name="obj">The object to be compared to the current object.</param>
    /// <returns>True if the instances are equals.</returns>
    public override bool Equals(object? obj)
    {
        return Equals(obj as QueryGroup);
    }

    /// <summary>
    /// Compares the <see cref="QueryGroup"/> object equality against the given target object.
    /// </summary>
    /// <param name="other">The object to be compared to the current object.</param>
    /// <returns>True if the instances are equal.</returns>
    public bool Equals(QueryGroup? other)
    {
        return other is not null
            && other.QueryFields?.Count == QueryFields?.Count
            && other.QueryGroups?.Count == QueryGroups?.Count
            && other.Conjunction == Conjunction
            && other.IsNot == IsNot
            && ((other.QueryFields == null && QueryFields == null) || (other.QueryFields is { } q1 && QueryFields is { } q2 && q1.Count == q2.Count && q1.Zip(q2, Equals).All(v => v)))
            && ((other.QueryGroups == null && QueryGroups == null) || (other.QueryGroups is { } g1 && QueryGroups is { } g2 && g1.Count == g2.Count && g1.Zip(g2, Equals).All(v => v)));
    }

    /// <summary>
    /// Compares the equality of the two <see cref="QueryGroup"/> objects.
    /// </summary>
    /// <param name="objA">The first <see cref="QueryGroup"/> object.</param>
    /// <param name="objB">The second <see cref="QueryGroup"/> object.</param>
    /// <returns>True if the instances are equal.</returns>
    public static bool operator ==(QueryGroup? objA,
        QueryGroup? objB)
    {
        if (objA is null)
        {
            return objB is null;
        }
        return objA.Equals(objB);
    }

    /// <summary>
    /// Compares the inequality of the two <see cref="QueryGroup"/> objects.
    /// </summary>
    /// <param name="objA">The first <see cref="QueryGroup"/> object.</param>
    /// <param name="objB">The second <see cref="QueryGroup"/> object.</param>
    /// <returns>True if the instances are not equal.</returns>
    public static bool operator !=(QueryGroup? objA, QueryGroup? objB)
        => !(objA == objB);

    #endregion


    /// <summary>
    /// 
    /// </summary>
    /// <param name="queryField"></param>
    [return: NotNullIfNotNull(nameof(queryField))]
    public static implicit operator QueryGroup?(QueryField? queryField) =>
        queryField is { } ? new QueryGroup([queryField], queryGroups: null, Conjunction.And, false) : null;

    private string DebuggerDisplay => RawGetString(0, null);
}
