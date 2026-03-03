using System.Linq.Expressions;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using RepoDb.Extensions.QueryFields;

namespace RepoDb;

public static partial class JsonQueryExtensions
{
    /// <summary>
    /// Extracts the value of a property from the <see cref="JsonNode"/> using a JSON path and converts it to the specified type <typeparamref name="T"/>.
    /// Supports nested paths (e.g., "addresses.home.street") and array indexing (e.g., "addresses[0].street").
    /// </summary>
    /// <typeparam name="T">The type to convert the value to.</typeparam>
    /// <param name="source">The JSON node to extract from.</param>
    /// <param name="name">The JSON path (e.g., "age", "address.street", "items[0].name").</param>
    /// <returns>The extracted and converted value, or default if not found.</returns>
    public static T? ExtractValue<T>(this JsonNode source, string name) where T : notnull
    {
        if (source == null || string.IsNullOrEmpty(name))
            return default;

        try
        {
            var node = NavigateJsonPath(source, name);
            if (node is null)
                return default;

            return node.GetValue<T>();
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// Extracts the value from a <see cref="JsonNode"/> property using the specified constructed path
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="source"></param>
    /// <param name="mapping"></param>
    /// <returns></returns>
    public static TResult? ExtractValue<TEntity, TResult>(this JsonNode source, Expression<Func<TEntity, TResult>> mapping) where TResult: notnull where TEntity : notnull
    {
        return ExtractValue<TResult>(source, JsonExtractQueryField.ParsePath(mapping));
    }

    /// <summary>
    /// Extracts the value from a <see cref="JsonNode"/> property using the specified constructed path
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="source"></param>
    /// <param name="mapping"></param>
    /// <returns></returns>
    public static TResult? ExtractValue<TEntity, TResult>(this DbJsonValue<TEntity> source, Expression<Func<TEntity, TResult>> mapping) where TResult : notnull where TEntity : class
    {
        return ExtractValue<TResult>(source.Json, JsonExtractQueryField.ParsePath(mapping));
    }

    private static JsonNode? NavigateJsonPath(JsonNode node, string path)
    {
        ArgumentNullException.ThrowIfNull(node);
        ArgumentException.ThrowIfNullOrEmpty(path);

        try
        {
            JsonNode? n = node;
            // Split path by dots, but preserve array indexing like [0]
            var segments = Regex.Split(path, @"\.(?![^\[]*\])");

            foreach (var segment in segments)
            {
                if (n is null)
                    return null;

                // Check if segment has array indexing
                var arrayMatch = Regex.Match(segment, @"^(\w+)\[(\d+)\]$");
                if (arrayMatch.Success)
                {
                    var propertyName = arrayMatch.Groups[1].Value;
                    if (!int.TryParse(arrayMatch.Groups[2].Value, out var index))
                        return null;

                    n = n[propertyName];
                    if (n is null)
                        return null;

                    n = n[index];
                }
                else if (Regex.IsMatch(segment, @"^\[\d+\]$"))
                {
                    // Just array indexing without property name
                    var indexMatch = Regex.Match(segment, @"^\[(\d+)\]$");
                    if (indexMatch.Success && int.TryParse(indexMatch.Groups[1].Value, out var index))
                    {
                        n = n[index];
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    // Simple property access
                    n = n[segment];
                }
            }

            return n;
        }
        catch (ArgumentException)
        {
            return null;
        }
    }
}
