using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
using RepoDb.Interfaces;

namespace RepoDb;

public struct DbJsonValue<T> : IFormattable, IDbJsonValue
#if NET
    , IParsable<DbJsonValue<T>>
#endif
    where T : class
{
    JsonNode? _json;
    T? _value;

    public JsonNode Json => _json ??= Converter.ToJsonObject(_value)!;

    public T Value => _value ?? (_value = Converter.FromJsonToObject<T>(_json))!;

    public DbJsonValue(JsonNode json)
    {
        ArgumentNullException.ThrowIfNull(json);
        _json = json;
        _value = default;
    }

    public DbJsonValue(T value)
    {
        ArgumentNullException.ThrowIfNull(value);
        _json = null;
        _value = value;
    }

    JsonNode? IDbJsonValue.JsonNode => Json;

    public override bool Equals(object? obj)
    {
        if (obj is JsonNode node)
            return node.ToJsonString() == Json.ToJsonString();
        else if (obj is T t)
            return Value.Equals(t);
        else if (obj is DbJsonValue<T> vt)
        {
            if (vt._json is { } jv)
                return Equals(jv);
            else
                return Equals(vt.Json);
        }

        return false;
    }

    public override int GetHashCode() => 1;

    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return Json?.ToJsonString(Converter.JsonSerializerOptions) ?? "null";
    }

    /// <inheritdoc/>
#pragma warning disable CA1000 // Do not declare static members on generic types
    public static DbJsonValue<T> Parse(string s, IFormatProvider? provider)
#pragma warning restore CA1000 // Do not declare static members on generic types
    {
        if (TryParse(s, provider, out var v))
            return v;
        else
        {
            throw new FormatException();
        }
    }

    /// <inheritdoc/>
#pragma warning disable CA1000 // Do not declare static members on generic types
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out DbJsonValue<T> result)
#pragma warning restore CA1000 // Do not declare static members on generic types
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            result = default;
            return false;
        }
        try
        {
            result = new() { _json = JsonObject.Parse(s, Converter.JsonNodeOptions, Converter.JsonDocumentOptions) };
            return true;

        }
        catch (Exception)
        {
            result = default;
            return false;
        }
    }

    public override string ToString()
    {
        return Json?.ToString() ?? "";
    }

    public static bool operator==(DbJsonValue<T> v1, DbJsonValue<T> v2) => v1.Equals(v2);
    public static bool operator!=(DbJsonValue<T> v1, DbJsonValue<T> v2) => !v1.Equals(v2);
    public static bool operator ==(DbJsonValue<T> v1, JsonNode v2) => v1.Equals(v2);
    public static bool operator !=(DbJsonValue<T> v1, JsonNode v2) => !v1.Equals(v2);
    public static bool operator ==(JsonNode v1, DbJsonValue<T> v2) => v2.Equals(v1);
    public static bool operator !=(JsonNode v1, DbJsonValue<T> v2) => !v2.Equals(v1);

    public static implicit operator DbJsonValue<T>(T value) => new DbJsonValue<T> { _value = value };
}
