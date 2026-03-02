using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
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
    /// <param name="jsonNode">The JSON node to extract from.</param>
    /// <param name="name">The JSON path (e.g., "age", "address.street", "items[0].name").</param>
    /// <returns>The extracted and converted value, or default if not found.</returns>
    public static T? ExtractValue<T>(this JsonNode jsonNode, string name) where T : notnull
    {
        if (jsonNode == null || string.IsNullOrEmpty(name))
            return default;

        try
        {
            var node = NavigateJsonPath(jsonNode, name);
            if (node == null)
                return default;

            return node.GetValue<T>();
        }
        catch
        {
            return default;
        }
    }

    public static TResult? ExtractValue<TEntity, TResult>(this JsonNode jsonNode, Expression<Func<TEntity, TResult>> mapping) where TResult: notnull where TEntity : notnull
    {
        return ExtractValue<TResult>(jsonNode, JsonExtractQueryField.ParsePath(mapping));
    }

    public static TResult? ExtractValue<TEntity, TResult>(this DbJsonValue<TEntity> jsonNode, Expression<Func<TEntity, TResult>> mapping) where TResult : notnull where TEntity : notnull
    {
        return ExtractValue<TResult>(jsonNode.Json, JsonExtractQueryField.ParsePath(mapping));
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
                if (n == null)
                    return null;

                // Check if segment has array indexing
                var arrayMatch = Regex.Match(segment, @"^(\w+)\[(\d+)\]$");
                if (arrayMatch.Success)
                {
                    var propertyName = arrayMatch.Groups[1].Value;
                    if (!int.TryParse(arrayMatch.Groups[2].Value, out var index))
                        return null;

                    n = n[propertyName];
                    if (n == null)
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
