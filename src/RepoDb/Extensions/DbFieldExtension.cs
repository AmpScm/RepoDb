using RepoDb.Enumerations;
using RepoDb.Options;

namespace RepoDb.Extensions;

/// <summary>
/// Contains the extension methods for <see cref="Field"/> object.
/// </summary>
public static class DbFieldExtension
{
    /// <summary>
    /// Converts an instance of a <see cref="DbField"/> into an <see cref="IEnumerable{T}"/> of <see cref="DbField"/> object.
    /// </summary>
    /// <param name="dbField">The <see cref="DbField"/> to be converted.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> list of <see cref="DbField"/> object.</returns>
    public static DbFieldCollection AsEnumerable(this DbField dbField)
    {
        return new([dbField]);
    }

    /// <summary>
    /// Converts the list of <see cref="DbField"/> objects into an <see cref="IEnumerable{T}"/> of <see cref="Field"/> objects.
    /// </summary>
    /// <param name="dbFields">The <see cref="DbField"/> to be converted.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> list of <see cref="Field"/> object.</returns>
    public static IEnumerable<Field> AsFields(this IEnumerable<DbField> dbFields) => dbFields;

    /// <summary>
    /// Like <see cref="Enumerable.SingleOrDefault{TSource}(IEnumerable{TSource})"/>, but handles a list of more that one item as default instead of an exception
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static TItem? OneOrDefault<TItem>(this IEnumerable<TItem> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (source is IReadOnlyCollection<TItem> col && col.Count == 1)
            return col.First();
        else
            return DoOne(source)!;
    }

    /// <summary>
    /// Like <see cref="Enumerable.SingleOrDefault{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>, but handles a list of more that one item as default instead of an exception
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static TItem? OneOrDefault<TItem>(this IEnumerable<TItem> source, Func<TItem, bool> predicate)
    {
        return source.Where(predicate).OneOrDefault();
    }

    private static TItem? DoOne<TItem>(IEnumerable<TItem> source)
    {
        using var v = source.GetEnumerator();

        if (!v.MoveNext() || v.Current is not { } item || v.MoveNext())
            return default;

        return item;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="dbFields"></param>
    /// <param name="name"></param>
    /// <param name="stringComparison"></param>
    /// <returns></returns>
    public static DbField? GetByFieldName(this IEnumerable<DbField> dbFields, string? name, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
    {
        return dbFields.FirstOrDefault(dbField => string.Equals(dbField.FieldName, name, stringComparison));
    }

    /// <summary>
    /// Gets the column that should be returned
    /// </summary>
    /// <returns></returns>
    /// <remarks>The value depends on the keys of the database and the <see cref="GlobalConfiguration" />'s <see cref="GlobalConfigurationOptions.KeyColumnReturnBehavior"/> setting</remarks>
    public static DbField? GetReturnColumn(this IEnumerable<DbField> fields) =>
        GlobalConfiguration.Options.KeyColumnReturnBehavior switch
        {
            KeyColumnReturnBehavior.Primary => fields.FirstOrDefault(x => x.IsPrimary),
            KeyColumnReturnBehavior.Identity => fields.FirstOrDefault(x => x.IsIdentity),
            KeyColumnReturnBehavior.PrimaryOrElseIdentity => fields.FirstOrDefault(x => x.IsPrimary) ?? fields.FirstOrDefault(x => x.IsIdentity),
            KeyColumnReturnBehavior.Default or
            KeyColumnReturnBehavior.IdentityOrElsePrimary => fields.FirstOrDefault(x => x.IsIdentity) ?? fields.FirstOrDefault(x => x.IsPrimary),
            _ => throw new NotSupportedException($"The key column return behavior '{GlobalConfiguration.Options.KeyColumnReturnBehavior}' is not supported."),
        };
}

