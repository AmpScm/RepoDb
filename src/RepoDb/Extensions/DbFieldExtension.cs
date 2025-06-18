
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

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
    public static IEnumerable<DbField> AsEnumerable(this DbField dbField)
    {
        yield return dbField;
    }

    /// <summary>
    /// Converts an instance of a <see cref="DbField"/> into <see cref="Field"/> object.
    /// </summary>
    /// <param name="dbField">The <see cref="DbField"/> to be converted.</param>
    /// <returns>An instance of <see cref="Field"/> object.</returns>
    [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Defined on DbField directly")]
    public static Field AsField(this DbField dbField) => dbField.AsField();

    /// <summary>
    /// Converts the list of <see cref="DbField"/> objects into an <see cref="IEnumerable{T}"/> of <see cref="Field"/> objects.
    /// </summary>
    /// <param name="dbFields">The <see cref="DbField"/> to be converted.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> list of <see cref="Field"/> object.</returns>
    public static IEnumerable<Field> AsFields(this IEnumerable<DbField> dbFields) => dbFields.Select(x => x.AsField());

    /// <summary>
    /// Converts the list of <see cref="DbField"/> objects into an <see cref="IReadOnlyList{T}"/> of <see cref="Field"/> objects.
    /// </summary>
    /// <param name="source">The <see cref="DbField"/> to be converted.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> list of <see cref="Field"/> object.</returns>
#if NET
    [return: NotNullIfNotNull(nameof(source))]
#endif

    public static TItem? OneOrDefault<TItem>(this IEnumerable<TItem> source)
    {
        if (source is IReadOnlyCollection<TItem> col && col.Count == 1)
            return source.FirstOrDefault()!;
        else
            return DoOne(source)!;
    }

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

    public static DbField? GetByName(this IEnumerable<DbField> dbFields, string name, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
    {
        return dbFields.FirstOrDefault(dbField => string.Equals(dbField.Name, name, stringComparison));
    }
}

