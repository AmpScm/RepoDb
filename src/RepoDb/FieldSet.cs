using System.Collections;
using System.Diagnostics;

namespace RepoDb;

[DebuggerDisplay($"{{{nameof(DebuggerDisplay)},nq}}")]
public sealed class FieldSet : IReadOnlyCollection<Field>
#if NET
    , IReadOnlySet<Field>
#endif
{
    private readonly HashSet<Field> _fields;
    static readonly HashSet<Field> EmptyFields = new();
    int? _hashCode;

    public FieldSet()
    {
        _fields = EmptyFields;
    }

    public FieldSet(IEnumerable<Field> fields)
    {
#if NET
        ArgumentNullException.ThrowIfNull(fields);
#endif
        // Copy inner hashset to avoid using unneeded intermediates
        _fields = new HashSet<Field>(fields is FieldSet fs ? fs._fields : fields, Field.CompareByName);
    }

    public int Count => _fields.Count;

    public bool Contains(Field item)
    {
        return _fields.Contains(item);
    }

    public IEnumerator<Field> GetEnumerator()
    {
        return _fields.GetEnumerator();
    }

    public bool IsProperSubsetOf(IEnumerable<Field> other)
    {
        return _fields.IsProperSubsetOf(other);
    }

    public bool IsProperSupersetOf(IEnumerable<Field> other)
    {
        return _fields.IsProperSupersetOf(other);
    }

    public bool IsSubsetOf(IEnumerable<Field> other)
    {
        return _fields.IsSubsetOf(other);
    }

    public bool IsSupersetOf(IEnumerable<Field> other)
    {
        return _fields.IsSupersetOf(other);
    }

    public bool Overlaps(IEnumerable<Field> other)
    {
        return _fields.Overlaps(other);
    }

    public bool SetEquals(IEnumerable<Field> other)
    {
        return _fields.SetEquals(other);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public FieldSet Union(IEnumerable<Field> other)
    {
        if (IsSupersetOf(other))
            return this;

        return new FieldSet(_fields.Concat(other));
    }

    public override bool Equals(object? obj)
    {
        if (obj is not FieldSet fs || fs.Count != Count)
            return false;

        return SetEquals(fs);
    }

    public static bool operator ==(FieldSet left, FieldSet right)
    {
        if (left is null && right is null)
            return true;
        if (left is null || right is null)
            return false;
        return left.Equals(right);
    }
    public static bool operator !=(FieldSet left, FieldSet right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        return _hashCode ??= HashCode.Combine(Count, _fields.Aggregate(0, (current, field) => current ^ field.GetHashCode()));
    }

    public static bool operator ==(FieldSet left, FieldSet right)
    {
        if (left is null && right is null)
            return true;
        if (left is null || right is null)
            return false;
        return left.Equals(right);
    }
    public static bool operator !=(FieldSet left, FieldSet right)
    {
        return !(left == right);
    }

    private string DebuggerDisplay => $"{Count} fields: " + string.Join(",", _fields.Select(x => x.FieldName));
}
