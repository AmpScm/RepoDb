using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace RepoDb;

/// <summary>
/// A generalized converter class.
/// </summary>
public static class Converter
{
    #region Methods

    /// <summary>
    /// Converts the value into <see cref="DBNull.Value"/> if it is null.
    /// </summary>
    /// <param name="value">The value to be checked for <see cref="DBNull.Value"/>.</param>
    /// <returns>The converted value.</returns>
    public static object NullToDbNull(object? value) =>
        value is null ? DBNull.Value : value;

    /// <summary>
    /// Converts the value into null if the value is equals to <see cref="DBNull.Value"/>.
    /// </summary>
    /// <param name="value">The value to be checked for <see cref="DBNull.Value"/>.</param>
    /// <returns>The converted value.</returns>
    public static object? DbNullToNull(object? value) =>
        Convert.IsDBNull(value) ? null : value;

    /// <summary>
    /// Converts a value to a target type if the value is equals to null or <see cref="DBNull.Value"/>.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="value">The value to be converted.</param>
    /// <returns>The converted value or null when <see cref="DBNull"/>.</returns>
    [return: NotNullIfNotNull(nameof(value))] // Except when DBNull
    public static T? ToType<T>(object? value)
    {
        value = Converter.DbNullToNull(value);

        if (value is null)
        {
            if (!typeof(T).IsValueType)
            {
                return default; // null for class types
            }
            else if (Nullable.GetUnderlyingType(typeof(T)) is { })
            {
                return default; // safe for nullable
            }
            else
            {
                return default; // or the default empty value
            }
        }

        if (value is T t)
        {
            return t;
        }


        var type = typeof(T);
        if (typeof(T).IsValueType)
        {
            if (Nullable.GetUnderlyingType(type) is { } ut)
                type = ut;

            if (type.IsEnum)
            {
                if (value is string sv)
                {
                    var mode = GlobalConfiguration.Options.EnumHandling;

#if NET
                    if (Enum.TryParse(type, sv, out var parsedValue))
                    {
                        if (mode != Enumerations.InvalidEnumValueHandling.ThrowError)
                            return (T)parsedValue;
                        else if (Enum.IsDefined(type, parsedValue))
                            return (T)parsedValue;

                        if (mode == Enumerations.InvalidEnumValueHandling.ThrowError)
                            throw new ArgumentOutOfRangeException("value", sv, $"The value '{sv}' is not defined in the enum '{type.FullName}'.");

                        return default!;
                    }
#else
                    object r = typeof(Converter).GetMethod(nameof(EnumTryParse), BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(type).Invoke(null, new object[] { sv });

                    if (r is {})
                    {
                        if (mode != Enumerations.InvalidEnumValueHandling.ThrowError)
                            return (T)r;
                        else if (Enum.IsDefined(type, r))
                            return (T)r;

                        if (mode == Enumerations.InvalidEnumValueHandling.ThrowError)
                            throw new ArgumentOutOfRangeException("value", sv, $"The value '{sv}' is not defined in the enum '{type.FullName}'.");

                        return default;
                    }
#endif

                    if (mode == Enumerations.InvalidEnumValueHandling.ThrowError)
                        throw new ArgumentOutOfRangeException("value", sv, $"The value '{sv}' is not defined in the enum '{type.FullName}'.");


                    return default!;
                }
                else if (value is int or short or long or byte or uint or ushort or ulong or sbyte)
                {
                    var underlyingType = Enum.GetUnderlyingType(type);

                    if (underlyingType != type.GetType())
                        value = Convert.ChangeType(value, underlyingType);

                    return (T)Enum.ToObject(type, value);
                }
                else if (value is decimal d)
                {
                    value = (long)d;

                    var underlyingType = Enum.GetUnderlyingType(type);

                    if (underlyingType != type.GetType())
                        value = Convert.ChangeType(value, underlyingType);

                    return (T)Enum.ToObject(type, value);
                }
                else
                {
                    if (GlobalConfiguration.Options.EnumHandling == Enumerations.InvalidEnumValueHandling.ThrowError)
                        throw new ArgumentOutOfRangeException("value", value, $"The value '{value}' is not defined in the enum '{type.FullName}'.");

                    return default!;
                }
            }
            else if (type == StaticType.Guid && value is string sv && Guid.TryParse(sv, out var gv))
            {
                return (T)(object)gv;
            }
            else if (type == StaticType.DateTime && value is string sv2 && DateTime.TryParse(sv2, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dv))
            {
                return (T)(object)dv;
            }
            else if (type == StaticType.DateTimeOffset && value is string sv3 && DateTimeOffset.TryParse(sv3, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dtv))
            {
                return (T)(object)dtv;
            }
            else if (type == StaticType.TimeSpan && value is string sv4 && TimeSpan.TryParse(sv4, CultureInfo.InvariantCulture, out var tsv))
            {
                return (T)(object)tsv;
            }
            else if (value is decimal d && type.IsPrimitive)
            {
                return (T)Convert.ChangeType((long)d, type, CultureInfo.InvariantCulture);
            }
#if NET
            else if (type == StaticType.DateOnly && value is string sv5 && DateOnly.TryParse(sv5, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dov))
            {
                return (T)(object)dov;
            }
            else if (type == StaticType.TimeOnly && value is string sv6 && TimeOnly.TryParse(sv6, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var tov))
            {
                return (T)(object)tov;
            }
#endif
        }

        try
        {
            return (T)Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
        }
        catch (InvalidCastException ex)
        {
            throw new InvalidCastException($"While converting '{value ?? "null"}' ({value?.GetType()}) to '{type.FullName}'", ex);
        }
    }


    static object? EnumTryParse<TEnum>(string value)
        where TEnum : struct, Enum
    {
        if (Enum.TryParse<TEnum>(value, out var result))
            return result;
        return null;
    }

    #endregion
}
