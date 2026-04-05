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
    /// Gets the JSON representation of the value.
    /// </summary>
    public JsonNode Json => _json ??= Converter.ToJsonObject(_value)!;

    /// <summary>
    /// Gets the value represented by the JSON.
    /// </summary>
    public T Value => _value ??= Converter.FromJsonToObject<T>(_json);

    /// <summary>
    /// Creates a new instance of <see cref="DbJsonValue{T}"/> from a JSON node.
    /// </summary>
    /// <param name="json">The JSON node to initialize the value from.</param>
    public DbJsonValue(JsonNode json)
    {
        ArgumentNullException.ThrowIfNull(json);
        _json = json;
        _value = default;
    }

    /// <summary>
    /// Creates a new instance of <see cref="DbJsonValue{T}"/> from a value of type <typeparamref name="T"/>.
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
    public override int GetHashCode() => Value.GetHashCode();

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
        GC.KeepAlive(provider); // Required for IParsable<>
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
    /// Compares two <see cref="DbJsonValue{T}"/> instances for equality.
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static bool operator ==(DbJsonValue<T> v1, DbJsonValue<T> v2) => v1.Equals(v2);

    /// <summary>
    /// Compares two <see cref="DbJsonValue{T}"/> instances for inequality.
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static bool operator !=(DbJsonValue<T> v1, DbJsonValue<T> v2) => !v1.Equals(v2);

    /// <summary>
    /// Compares a <see cref="DbJsonValue{T}"/> instance with a <see cref="JsonNode"/> for equality.
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static bool operator ==(DbJsonValue<T> v1, JsonNode v2) => v1.Equals(v2);

    /// <summary>
    /// Compares a <see cref="DbJsonValue{T}"/> instance with a <see cref="JsonNode"/> for inequality.
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static bool operator !=(DbJsonValue<T> v1, JsonNode v2) => !v1.Equals(v2);

    /// <summary>
    /// Compares a <see cref="DbJsonValue{T}"/> instance with a value of type <typeparamref name="T"/> for equality.
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static bool operator ==(JsonNode v1, DbJsonValue<T> v2) => v2.Equals(v1);

    /// <summary>
    /// Compares a <see cref="DbJsonValue{T}"/> instance with a value of type <typeparamref name="T"/> for inequality.
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static bool operator !=(JsonNode v1, DbJsonValue<T> v2) => !v2.Equals(v1);

    /// <summary>
    /// Compares a <see cref="DbJsonValue{T}"/> instance with a value of type <typeparamref name="T"/> for equality.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator DbJsonValue<T>(T value) => new DbJsonValue<T> { _value = value };

    // Convert here, to avoid side-effects
    private readonly string DebuggerDisplay => (_json ?? Converter.ToJsonObject<T>(_value))?.ToJsonString(Converter.JsonSerializerOptions) ?? "null";
}
