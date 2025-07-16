using System.Diagnostics.CodeAnalysis;

namespace RepoDb.Extensions;

/// <summary>
/// Contains the extension methods for <see cref="ClassProperty"/>.
/// </summary>
public static class ClassPropertyExtension
{
    /// <summary>
    /// Converts the list of <see cref="ClassProperty"/> into a a list of <see cref="Field"/> objects.
    /// </summary>
    /// <param name="properties">The current instance of <see cref="ClassProperty"/>.</param>
    /// <returns>A list of <see cref="string"/> objects.</returns>
    [return: NotNullIfNotNull(nameof(properties))]
    public static FieldSet? AsFields(this IEnumerable<ClassProperty>? properties)
        => properties is { } pp ? new FieldSet(pp.Select(p => p.AsField())) : null;

    /// <summary>
    /// Retrieves the first ClassProperty from a collection that matches a specified mapped name using the specified comparison (case-insensitive by default)
    /// </summary>
    /// <param name="source">The collection of ClassProperty instances to search through.</param>
    /// <param name="name">The mapped name to match against the properties in the collection.</param>
    /// <param name="stringComparison">Specifies how to compare the mapped name with the property names, allowing for case sensitivity options.</param>
    /// <returns>Returns the first matching ClassProperty or null if no match is found.</returns>
    public static ClassProperty? GetByFieldName(this IEnumerable<ClassProperty>? source, string? name, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        => source?.FirstOrDefault(p => string.Equals(p.FieldName, name, stringComparison));


    /// <summary>
    /// Retrieves the first ClassProperty from a collection that matches a specified mapped name using the specified comparison (case-insensitive by default)
    /// </summary>
    /// <param name="source">The collection of ClassProperty instances to search through.</param>
    /// <param name="name">The mapped name to match against the properties in the collection.</param>
    /// <param name="stringComparison">Specifies how to compare the mapped name with the property names, allowing for case sensitivity options.</param>
    /// <returns>Returns the first matching ClassProperty or null if no match is found.</returns>
    public static ClassProperty? GetByPropertyName(this IEnumerable<ClassProperty>? source, string? name, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        => source?.FirstOrDefault(p => string.Equals(p.PropertyName, name, stringComparison));
}
