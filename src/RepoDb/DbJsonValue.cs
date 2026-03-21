using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
using RepoDb.Interfaces;

namespace RepoDb;

#pragma warning disable CA1000 // Do not declare static members on generic types

/// <summary>
/// Database representation of a Json node with the specified format. RepoDb will take care of converting to and from json
/// </summary>
/// <typeparam name="T"></typeparam>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)},nq}}")]
public struct DbJsonValue<T> : IFormattable, IDbJsonValue, IEquatable<T>, IEquatable<DbJsonValue<T>>
#if NET
    , IParsable<DbJsonValue<T>>, IUtf8SpanParsable<DbJsonValue<T>>
#endif
    where T : class
{
    private JsonNode? _json;
    private T? _value;

    /// <summary>
    ///
    /// </summary>
    public JsonNode Json => _json ??= Converter.ToJsonObject(_value)!;

    /// <summary>
    ///
    /// </summary>
    public T Value => _value ?? (_value = Converter.FromJsonToObject<T>(_json));

    /// <summary>
    ///
    /// </summary>
    /// <param name="json"></param>
    public DbJsonValue(JsonNode json)
    {
        ArgumentNullException.ThrowIfNull(json);
        _json = json;
        _value = default;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="value"></param>
    public DbJsonValue(T value)
    {
        ArgumentNullException.ThrowIfNull(value);
        _json = null;
        _value = value;
    }

    JsonNode? IDbJsonValue.JsonNode => Json;

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is JsonNode node)
            return Equals(node);
        else if (obj is T t)
            return Equals(t);
        else if (obj is DbJsonValue<T> vt)
            return Equals(vt);

        return false;
    }

    /// <inheritdoc/>
    public override readonly int GetHashCode() => 1;

    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return Json?.ToJsonString(Converter.JsonSerializerOptions) ?? "null";
    }

    /// <inheritdoc/>
    public static DbJsonValue<T> Parse(string s, IFormatProvider? provider)
    {
        if (TryParse(s, provider, out var v))
            return v;
        else
        {
            throw new FormatException();
        }
    }

    /// <inheritdoc/>
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out DbJsonValue<T> result)
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

    /// <inheritdoc />
    public override string ToString()
    {
        return Json?.ToJsonString(Converter.JsonSerializerOptions) ?? "null";
    }

    /// <inheritdoc />
    public static DbJsonValue<T> Parse(ReadOnlySpan<byte> s, IFormatProvider? provider)
    {
        if (TryParse(s, provider, out var v))
            return v;
        else
        {
            throw new FormatException();
        }
    }

    /// <inheritdoc />
    public static bool TryParse(ReadOnlySpan<byte> s, IFormatProvider? provider, [MaybeNullWhen(false)] out DbJsonValue<T> result)
    {
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

    /// <inheritdoc/>
    public bool Equals(JsonNode other)
    {
        return other?.ToJsonString() == Json.ToJsonString();
    }

    /// <inheritdoc/>
    public bool Equals(T? other)
    {
        return Value.Equals(other);
    }

    /// <inheritdoc/>
    public bool Equals(DbJsonValue<T> other)
    {
        if (other._json is { } jv)
            return Equals(jv);
        else
            return Equals(other.Json);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static bool operator ==(DbJsonValue<T> v1, DbJsonValue<T> v2) => v1.Equals(v2);

    /// <summary>
    ///
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static bool operator !=(DbJsonValue<T> v1, DbJsonValue<T> v2) => !v1.Equals(v2);

    /// <summary>
    ///
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static bool operator ==(DbJsonValue<T> v1, JsonNode v2) => v1.Equals(v2);

    /// <summary>
    ///
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static bool operator !=(DbJsonValue<T> v1, JsonNode v2) => !v1.Equals(v2);

    /// <summary>
    ///
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static bool operator ==(JsonNode v1, DbJsonValue<T> v2) => v2.Equals(v1);

    /// <summary>
    ///
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static bool operator !=(JsonNode v1, DbJsonValue<T> v2) => !v2.Equals(v1);

    /// <summary>
    ///
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator DbJsonValue<T>(T value) => new DbJsonValue<T> { _value = value };

    // Convert here, to avoid side-effects
    private readonly string DebuggerDisplay => (_json ?? Converter.ToJsonObject<T>(_value))?.ToJsonString(Converter.JsonSerializerOptions) ?? "null";
}
