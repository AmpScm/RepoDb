using RepoDb.Interfaces;

namespace RepoDb.Extensions;

/// <summary>
/// Contains the extension methods for <see cref="Field"/> object.
/// </summary>
public static class FieldExtension
{
    /// <summary>
    /// Converts an instance of a <see cref="Field"/> into an <see cref="IEnumerable{T}"/> of <see cref="Field"/> object.
    /// </summary>
    /// <param name="field">The <see cref="Field"/> to be converted.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> list of <see cref="Field"/> object.</returns>
    public static FieldSet AsEnumerable(this Field field)
        => new([field]);

    /// <summary>
    /// Creates a string representation of the JOIN statement for the target qualifier <see cref="Field"/> objects.
    /// </summary>
    /// <param name="field">The instance of the <see cref="Field"/> object.</param>
    /// <param name="leftAlias">The left alias.</param>
    /// <param name="rightAlias">The right alias.</param>
    /// <param name="considerNulls">The value that defines whether the null values are being considered.</param>
    /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
    /// <returns>The currently in used database setting.</returns>
    public static string AsJoinQualifier(this Field field,
        string leftAlias,
        string rightAlias,
        bool considerNulls,
        IDbSetting dbSetting)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.FieldName.AsJoinQualifier(leftAlias, rightAlias, considerNulls, dbSetting);
    }

    public static Field? GetByFieldName(this IEnumerable<Field> source, string? name, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        => source.FirstOrDefault(p => string.Equals(p.FieldName, name, stringComparison));

    public static FieldSet AsFieldSet(this IEnumerable<Field> fields)
    {
        return fields as FieldSet ?? new FieldSet(fields);
    }
}

