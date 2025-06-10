using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace RepoDb.Extensions;

/// <summary>
/// Contains the extension methods for <see cref="Object"/>.
/// </summary>
internal static class ObjectExtension
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    internal static void ThrowIfNull<T>(T obj) =>
        ThrowIfNull(obj, null);

    /// <summary>
    ///
    /// </summary>
    /// <param name="argument"></param>
    /// <param name="parameterName"></param>
    internal static void ThrowIfNull(
#if NET
        [NotNull]
#endif
    object? argument,
#if NET
        [CallerArgumentExpression(nameof(argument))]
#endif
        string? paramName = null)
    {
#if NET
        ArgumentNullException.ThrowIfNull(argument, paramName);
#else
        if (argument != null)
        {
            return;
        }
        throw new ArgumentNullException(paramName: paramName);
#endif
    }
}
