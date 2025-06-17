#nullable enable
using System.Collections;
using System.Linq.Expressions;
using RepoDb.Extensions;

namespace RepoDb;

public readonly struct FieldSet : IEnumerable<Field>
{
    readonly IEnumerable<Field>? _fields;

    public FieldSet(params IEnumerable<Field>? fields)
    {
        _fields = fields;
    }

    public FieldSet(params Field[]? fields)
    {
        _fields = fields;
    }

    public IEnumerable<Field>? AsEnumerable() => _fields;

    public static FieldSet From(IEnumerable<Field>? fields) => new(fields);
    public static FieldSet From(IEnumerable<DbField>? fields) => new(fields?.AsFields());
    public bool IsSet => _fields is not null;
    public bool Any() => _fields?.Any() == true;

    IEnumerator<Field> IEnumerable<Field>.GetEnumerator()
    {
        return (_fields ?? []).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return (_fields ?? []).GetEnumerator();
    }

    public static implicit operator FieldSet(Field field)
    {
        if (field is null)
        {
            return new FieldSet([]);
        }
        else
        {
            return new([field]);
        }
    }

    public static implicit operator FieldSet(Field[] fields) => new(fields);
    public static implicit operator FieldSet(List<Field> fields) => new(fields);
    public static implicit operator FieldSet(System.Collections.ObjectModel.Collection<Field> fields) => new(fields);
    public static implicit operator bool(FieldSet fieldSet) => fieldSet.IsSet;

    public override int GetHashCode()
    {
        return _fields?.GetHashCode() ?? 0;
    }
}

public readonly struct FieldSet<TEntity> : IEnumerable<Field>
    where TEntity : class
{
    #region Specific for FieldSet<TEntity>
    public FieldSet(Expression<Func<TEntity, object?>> declare)
    {
        _fields = Field.Parse(declare);
    }

    public static explicit operator FieldSet<TEntity>(FieldSet fieldSet)
    {
        return new(fieldSet.AsEnumerable());
    }

    public static implicit operator FieldSet(FieldSet<TEntity> fieldSet)
    {
        return new(fieldSet.AsEnumerable());
    }
    #endregion

    #region Copy of FieldSet
    readonly IEnumerable<Field>? _fields;
    public FieldSet(params IEnumerable<Field>? fields)
    {
        _fields = fields;
    }

    public FieldSet(params Field[]? fields)
    {
        _fields = fields;
    }

    public IEnumerable<Field>? AsEnumerable() => _fields;

    public static FieldSet From(IEnumerable<Field>? fields) => new(fields);

    public bool IsSet => _fields is not null;
    public bool Any() => _fields?.Any() == true;

    IEnumerator<Field> IEnumerable<Field>.GetEnumerator()
    {
        return (_fields ?? []).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return (_fields ?? []).GetEnumerator();
    }

    public static implicit operator FieldSet<TEntity>(Field field)
    {
        if (field is null)
        {
            return new([]);
        }
        else
        {
            return new([field]);
        }
    }

    public static implicit operator FieldSet<TEntity>(Field[] fields) => new(fields);
    public static implicit operator FieldSet<TEntity>(List<Field> fields) => new(fields);
    public static implicit operator bool(FieldSet<TEntity> fieldSet) => fieldSet.IsSet;
    #endregion

    public override int GetHashCode()
    {
        return _fields?.GetHashCode() ?? 0;
    }
}
