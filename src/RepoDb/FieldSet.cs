using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace RepoDb;

/// <summary>
/// Keeps a set of <see cref="Field"/>
/// </summary>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)},nq}}")]
public sealed class FieldSet : IReadOnlyCollection<Field>, IEquatable<FieldSet>
#if NET
    , IReadOnlySet<Field>
#endif
{
    private readonly HashSet<Field> _fields;
    private static readonly HashSet<Field> EmptyFields = new(Field.CompareByName);
    private int? _hashCode;
    private FieldSet()
    {
        _fields = EmptyFields;
    }

    /// <summary>
    /// Initializes a new set of <see cref="Field"/> instances
    /// </summary>
    /// <param name="fields"></param>
    public FieldSet(IEnumerable<Field> fields)
    {
        ArgumentNullException.ThrowIfNull(fields);

        // Copy inner hashset to avoid using unneeded intermediates
        _fields = new HashSet<Field>(fields is FieldSet fs ? fs._fields : fields, Field.CompareByName);
    }

    /// <summary>
    /// Initializes a new set of <see cref="Field"/> instances
    /// </summary>
    /// <param name="fields"></param>
    public FieldSet(params ReadOnlySpan<Field> fields)
    {
        if (fields.Length == 0)
            _fields = EmptyFields;
        else
        {
            _fields = new HashSet<Field>(fields.ToArray(), Field.CompareByName);
        }
    }

    /// <summary>
    /// Gets the standard empty <see cref="FieldSet"/>
    /// </summary>
    public static readonly FieldSet Empty = new();

    /// <summary>
    /// Gets the total number of fields in this set.
    /// </summary>
    public int Count => _fields.Count;

    /// <summary>
    /// Gets a field by its name using case-insensitive comparison.
    /// </summary>
    /// <param name="fieldName">The name of the field to retrieve.</param>
    /// <returns>A <see cref="Field"/> with the specified name, or <see langword="null"/> if not found.</returns>
    public Field? GetByFieldName(string fieldName)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(fieldName);

        return _fields.FirstOrDefault(f => f.FieldName.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Determines whether a field with the specified name exists in this set using case-insensitive comparison.
    /// </summary>
    /// <param name="fieldName">The name of the field to check for.</param>
    /// <returns><see langword="true"/> if a field with the specified name exists; otherwise, <see langword="false"/>.</returns>
    public bool ContainsFieldName(string fieldName)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(fieldName);
        return _fields.Any(f => f.FieldName.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
    }

    /// <inheritdoc/>>
    public bool Contains(Field item)
    {
        return _fields.Contains(item);
    }

    /// <inheritdoc/>>
    public IEnumerator<Field> GetEnumerator()
    {
        return _fields.GetEnumerator();
    }

    /// <inheritdoc/>>
    public bool IsProperSubsetOf(IEnumerable<Field> other)
    {
        return _fields.IsProperSubsetOf(other);
    }

    /// <inheritdoc/>>
    public bool IsProperSupersetOf(IEnumerable<Field> other)
    {
        return _fields.IsProperSupersetOf(other);
    }

    /// <inheritdoc/>>
    public bool IsSubsetOf(IEnumerable<Field> other)
    {
        return _fields.IsSubsetOf(other);
    }

    /// <inheritdoc/>>
    public bool IsSupersetOf(IEnumerable<Field> other)
    {
        return _fields.IsSupersetOf(other);
    }

    /// <inheritdoc/>>
    public bool Overlaps(IEnumerable<Field> other)
    {
        return _fields.Overlaps(other);
    }

    /// <inheritdoc/>>
    public bool SetEquals(IEnumerable<Field> other)
    {
        return _fields.SetEquals(other);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <inheritdoc/>>
    public FieldSet Union(IEnumerable<Field> other)
    {
        if (IsSupersetOf(other))
            return this;

        return new FieldSet(_fields.Concat(other));
    }

    /// <inheritdoc/>>
    public override bool Equals(object? obj)
    {
        return obj is FieldSet fs && Equals(fs);
    }

    /// <inheritdoc/>>
    public bool Equals([NotNullWhen(true)] FieldSet? other)
    {
        return other?.Count == Count
            && (ReferenceEquals(this, other) || SetEquals(other));
    }

    /// <inheritdoc/>>
    public override int GetHashCode()
    {
        return _hashCode ??= HashCode.Combine(Count, _fields.Aggregate(0, (current, field) => current ^ field.GetHashCode()));
    }

    /// <summary>
    /// Checks if both sets have the same fields
    /// </summary>
    /// <param name="objA"></param>
    /// <param name="objB"></param>
    /// <returns></returns>
    public static bool operator ==(FieldSet? objA, FieldSet? objB)
        => ReferenceEquals(objA, objB) || (objA?.Equals(objB) == true);

    /// <summary>
    ///
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(FieldSet? left, FieldSet? right)
        => !(left == right);

    /// <summary>
    /// Initializes Fieldset from the specified entity using <see cref="FieldCache"/>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public static FieldSet From<TEntity>()
        where TEntity : class
    {
        return FieldCache.Get<TEntity>();
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="from"></param>
    /// <returns></returns>
    public static FieldSet Parse<TEntity>(Expression<Func<TEntity, object?>> from)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(from);
        return Field.Parse<TEntity>(from);
    }

    private string DebuggerDisplay => $"{Count} fields: " + string.Join(",", _fields.Select(x => x.FieldName));

    /// <summary>
    /// Initializes a new set of <see cref="Field"/> instances
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    public static FieldSet Create(ReadOnlySpan<Field> items)
    {
        if (items.Length == 0)
            return Empty;
        else
            return new(items);
    }
}
