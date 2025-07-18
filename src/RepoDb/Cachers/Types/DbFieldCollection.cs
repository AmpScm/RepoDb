using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb;

/// <summary>
/// A class the holds the collection of column definitions of the table.
/// </summary>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)},nq}}")]
public sealed class DbFieldCollection : IReadOnlyCollection<DbField>, IEquatable<DbFieldCollection>
#if NET
    , IReadOnlySet<DbField>
#endif
{
    private readonly HashSet<DbField> _fields;
    private FieldSet? _asFieldset;
    private readonly Lazy<DbField?> lazyIdentity;
    private readonly Lazy<DbFieldCollection?> lazyPrimaryFields;
    private Dictionary<string, DbField>? _nameMap;
    private int? _hashCode;

    public int Count => _fields.Count;

    /// <summary>
    /// Creates a new instance of <see cref="DbFieldCollection" /> object.
    /// </summary>
    /// <param name="dbFields">A collection of column definitions of the table.</param>
    /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
    public DbFieldCollection(IEnumerable<DbField> dbFields)
    {
        ArgumentNullException.ThrowIfNull(dbFields);

        _fields = new(dbFields is DbFieldCollection fc ? fc._fields : dbFields, DbField.CompareByName);
        lazyPrimaryFields = new(GetPrimaryDbFields);
        lazyIdentity = new(GetIdentityDbField);
    }

    public DbFieldCollection? PrimaryFields => lazyPrimaryFields.Value;

    /// <summary>
    /// Gets the identity column of this table if there is ine
    /// </summary>
    /// <returns>A identity column definition.</returns>
    public DbField? Identity => lazyIdentity.Value;

    /// <summary>
    /// Get the list of <see cref="DbField" /> objects converted into an <see cref="FieldSet" /> of <see cref="Field" /> objects.
    /// </summary>
    /// <returns></returns>
    public FieldSet AsFields() => _asFieldset ??= _fields.AsFieldSet();

    /// <summary>
    /// Gets a value indicating whether the current column definitions of the table is empty.
    /// </summary>
    /// <returns>A value indicating whether the column definitions of the table is empty.</returns>
    public bool IsEmpty() => Count == 0;

    /// <summary>
    /// Gets column definition of the table based on the name of the database field.
    /// </summary>
    /// <param name="name">The name of the mapping that is equivalent to the column definition of the table.</param>
    /// <returns>A column definition of table.</returns>
    public DbField? GetByFieldName(string name)
    {
        if (_nameMap is null)
        {
            // If the collection is large, we will create a map for faster access
            if (_fields.Count > 10)
                _nameMap = _fields.ToDictionary(df => df.FieldName, df => df, StringComparer.OrdinalIgnoreCase);
            else
                return _fields.AsEnumerable().GetByFieldName(name);
        }

        _nameMap.TryGetValue(name, out var dbField);
        return dbField;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public DbField? GetByName(string name) => GetByFieldName(name);

    private DbField? GetPrimaryDbField() => this.OneOrDefault(df => df.IsPrimary);

    private DbFieldCollection? GetPrimaryDbFields() => this.Where(x => x.IsPrimary) is { } p && p.Any() ? new DbFieldCollection(p) : null;

    private DbField? GetIdentityDbField() => this.FirstOrDefault(df => df.IsIdentity);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public FieldSet GetAsFields() => AsFields();

    internal DbField? GetKeyColumnReturn(KeyColumnReturnBehavior keyColumnReturnBehavior) => keyColumnReturnBehavior switch
    {
        KeyColumnReturnBehavior.Primary => PrimaryFields?.FirstOrDefault(),
        KeyColumnReturnBehavior.Identity => Identity,
        KeyColumnReturnBehavior.PrimaryOrElseIdentity => PrimaryFields?.FirstOrDefault() ?? Identity,
        KeyColumnReturnBehavior.IdentityOrElsePrimary => Identity ?? PrimaryFields?.FirstOrDefault(),
        _ => throw new NotSupportedException($"The key column return behavior '{GlobalConfiguration.Options.KeyColumnReturnBehavior}' is not supported."),
    };

    public bool Contains(DbField item)
    {
        return _fields.Contains(item);
    }

    public bool IsProperSubsetOf(IEnumerable<DbField> other)
    {
        return _fields.IsProperSubsetOf(other);
    }

    public bool IsProperSupersetOf(IEnumerable<DbField> other)
    {
        return _fields.IsProperSupersetOf(other);
    }

    public bool IsSubsetOf(IEnumerable<DbField> other)
    {
        return _fields.IsSubsetOf(other);
    }

    public bool IsSupersetOf(IEnumerable<DbField> other)
    {
        return _fields.IsSupersetOf(other);
    }

    public bool Overlaps(IEnumerable<DbField> other)
    {
        return _fields.Overlaps(other);
    }

    public bool SetEquals(IEnumerable<DbField> other)
    {
        return _fields.SetEquals(other);
    }

    public IEnumerator<DbField> GetEnumerator()
    {
        return _fields.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override int GetHashCode()
    {
        return _hashCode ??= HashCode.Combine(Count, _fields.Aggregate(0, (current, field) => current ^ field.GetHashCode()));
    }

    public override bool Equals(object? obj)
    {
        return obj is DbFieldCollection fc && Equals(fc);
    }

    public bool Equals(DbFieldCollection? other)
    {
        if (other is null)
            return false;

        return Count == other.Count &&
            SetEquals(other);
    }

    private string DebuggerDisplay => $"{Count} fields: " + string.Join(",", _fields.Select(x => x.FieldName));
}
