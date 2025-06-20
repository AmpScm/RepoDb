#if Q
using System.Collections;
using System.Data.Common;
using System.Linq.Expressions;
using RepoDb.Extensions;

namespace RepoDb;

public class FieldSet : IReadOnlyCollection<Field>
#if NET
    , IReadOnlySet<Field>
#endif
{
    HashSet<Field> _fields;

    public FieldSet(IEnumerable<Field> fields)
    {
        if (fields is null)
            throw new ArgumentNullException(nameof(fields));

        _fields = new HashSet<Field>(fields);
    }

    public int Count => ((IReadOnlyCollection<Field>)_fields).Count;

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
        return _fields.GetEnumerator();
    }
}

public struct FieldSetBuilder<TEntity>
    where TEntity : class
{
    IEnumerable<Field>? _fields;

    public FieldSetBuilder(IEnumerable<Field>? fields)
    {
        _fields = fields;
    }

    public static implicit operator FieldSetBuilder<TEntity>(Expression<Func<BuildArg, IEnumerable<Field>>> listBuilder)
    {
        return new(listBuilder.Compile().Invoke(new BuildArg()));
    }

    public IEnumerable<Field>? AsEnumerable()
    {
        return _fields;
    }

    //public static implicit operator FieldSetBuilder<TEntity>(Expression<Func<TEntity, dynamic?>> instanceBuilder)
    //{
    //    return new(Field.Parse<TEntity>(instanceBuilder));
    //}

    public class BuildArg : IEnumerable<Field>
    {
        Lazy<IEnumerable<Field>> _all = new(() => PropertyCache.Get<TEntity>().AsFields());
        public BuildArg() : base()
        {
        }

        public TEntity R => throw new InvalidOperationException();


        public IEnumerable<Field> From(object v) => throw new InvalidOperationException();

        public IEnumerator<Field> GetEnumerator() => _all.Value.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerable<Field> All => this;
    }
}

public static partial class DbSessionExtension
{
    public static async Task<object> InsertAsync<TEntity>(
        this DbSession session,
        TEntity entity,
        FieldSetBuilder<TEntity> fields,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        return await session.Connection.InsertAsync<TEntity>(
            entity,
            fields.AsEnumerable(),
            transaction: session.Transaction,
            cancellationToken: cancellationToken);
    }

    public static async Task<object> InsertAsync<TEntity>(
        this DbSession session,
        TEntity entity,
        Expression<Func<FieldSetBuilder<TEntity>.BuildArg, IEnumerable<Field>?>> fields,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        return await session.Connection.InsertAsync<TEntity>(
            entity,
            fields.AsEnumerable(),
            transaction: session.Transaction,
            cancellationToken: cancellationToken);
    }

    public static async Task<object> InsertAsync<TEntity>(
        this DbSession session,
        TEntity entity,
        Expression<Func<TEntity, object>> fields,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        return await session.Connection.InsertAsync<TEntity>(
            entity,
            fields.AsEnumerable(),
            transaction: session.Transaction,
            cancellationToken: cancellationToken);
    }
}

internal class TestSome
{

    static async Task Test()
    {
        var db = default(DbConnection)!;
        var sess = new DbSession(db);

        await sess.InsertAsync(db, f => f.From(f.R.ConnectionString), default);
        await sess.InsertAsync(db, f => new { f.ConnectionString }, default);
        await sess.InsertAsync(db, default, default);

    }
}

#endif
