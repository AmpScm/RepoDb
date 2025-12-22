using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#if !NET
#pragma warning disable CA1018 // SpecifyAttributeUsage
namespace System.Runtime.CompilerServices
{

    // Required to allow init properties in netstandard
    internal static class IsExternalInit
    {
    }

    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Field | System.AttributeTargets.Property | System.AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    internal sealed class RequiredMemberAttribute : Attribute
    {
        public RequiredMemberAttribute()
        {
        }
    }

    [System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    internal sealed class CompilerFeatureRequiredAttribute : Attribute
    {
        public CompilerFeatureRequiredAttribute(string featureName)
        {
            FeatureName = featureName ?? throw new ArgumentNullException(nameof(featureName));
        }

        public string FeatureName { get; }
    }

    [System.AttributeUsage(System.AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    internal sealed class CallerArgumentExpressionAttribute : Attribute
    {
        public CallerArgumentExpressionAttribute(string parameterName)
        {
            ParameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
        }
        public string ParameterName { get; }
    }
}

namespace System
{

    internal static class CompatExtensions
    {
        public static bool StartsWith(this string v, char value)
        {
            return v.Length > 0 && v[0] == value;
        }

        public static bool EndsWith(this string v, char value)
        {
            return v.Length > 0 && v[v.Length - 1] == value;
        }

        public static bool Contains(this string value,
            string stringToSeek,
            StringComparison comparisonType)
        {
            if (comparisonType == StringComparison.Ordinal)
            {
                return value.Contains(stringToSeek);
            }
            else
            {
                return value.IndexOf(stringToSeek, comparisonType) >= 0;
            }
        }

        public static int IndexOf(this string value,
            char charToSeek,
            StringComparison comparisonType)
        {
            if (comparisonType == StringComparison.Ordinal)
                return value.IndexOf(charToSeek);
            else
                return value.IndexOf(charToSeek.ToString(), comparisonType) ;
        }


        public static string[] Split(this string value,
            char separator,
            int count,
            StringSplitOptions options = StringSplitOptions.None)
        {
            return value.Split([separator], count, options);
        }

        public static bool Contains(this string value,
            char charToSeek,
            StringComparison comparisonType)
        {
            if (comparisonType == StringComparison.Ordinal)
                return value.Contains(charToSeek);
            else
                throw new NotSupportedException($"String comparison '{comparisonType}' is not supported for char search.");
        }

        public static string Replace(this string value, string needle, string replacement, StringComparison comparisonType)
        {
            if (comparisonType == StringComparison.Ordinal)
                return value.Replace(needle, replacement);
            else
                throw new NotSupportedException($"String replacement '{comparisonType}' is not supported for char search.");
        }
    }

}

namespace System.Diagnostics.CodeAnalysis
{
    [System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Parameter | System.AttributeTargets.Property | System.AttributeTargets.ReturnValue, Inherited=false)]
    internal sealed class NotNullAttribute : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    internal sealed class NotNullWhenAttribute : Attribute
    {
        //
        // Summary:
        //     Initializes the attribute with the specified return value condition.
        //
        // Parameters:
        //   returnValue:
        //     The return value condition. If the method returns this value, the associated
        //     parameter will not be null.
        public NotNullWhenAttribute(bool returnValue)
        {
            ReturnValue = returnValue;
        }

        //
        // Summary:
        //     Gets the return value condition.
        //
        // Returns:
        //     The return value condition. If the method returns this value, the associated
        //     parameter will not be null.
        public bool ReturnValue { get; }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    internal sealed class DoesNotReturnAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    internal sealed class SetsRequiredMembersAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
    internal sealed class NotNullIfNotNullAttribute : Attribute
    {
        public NotNullIfNotNullAttribute(string parameterName)
        {
            ParameterName = parameterName;
        }
        public string ParameterName { get; }
    }

    //
    // Summary:
    //     Specifies that null is disallowed as an input even if the corresponding type
    //     allows it.
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, Inherited = false)]
    internal sealed class DisallowNullAttribute : Attribute
    {
    }
}
#endif

namespace RepoDb
{
    internal static class NetCompatExtensions
    {
#if !NET
        [DoesNotReturn]
        private static void Throw<TException>(TException value) where TException : Exception => throw value;

        extension(ArgumentNullException)
        {
            public static void ThrowIfNull([NotNull] object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
            {
                if (argument is null)
                {
                    Throw(new ArgumentNullException(paramName));
                }
            }

            public static void ThrowIfNullOrWhiteSpace([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
            {
                if (string.IsNullOrWhiteSpace(argument))
                {
                    Throw(new ArgumentNullException(paramName, "Argument cannot be null or whitespace."));
                }
                argument = ""; // Suppress stupid warning
            }

            public static void ThrowIfNullOrEmpty([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
            {
                if (string.IsNullOrEmpty(argument))
                {
                    Throw(new ArgumentNullException(paramName, "Argument cannot be null or empty."));
                }
                argument = ""; // Suppress stupid warning
            }
        }

        extension(ArgumentOutOfRangeException)
        {
            public static void ThrowIfLessThan<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T>
            {
                if (value.CompareTo(other) < 0)
                {
                    Throw(new ArgumentOutOfRangeException(paramName, $"Value must be greater than or equal to {other}."));
                }
            }
        }
#endif

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
#pragma warning disable CS1998 // Async function should await
        public static async ValueTask<IDbTransaction> BeginTransactionAsync(this IDbConnection dbConnection, CancellationToken cancellationToken = default)
#pragma warning restore CS1998 // Async function should await
        {
#if NET
            if (dbConnection is DbConnection dbc)
                return await dbc.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
#endif
            return dbConnection.BeginTransaction();
        }

        public static async ValueTask CommitAsync(this IDbTransaction dbTransaction, CancellationToken cancellationToken = default)
        {
            if (dbTransaction is DbTransaction dbTransaction1)
                await dbTransaction1.CommitAsync(cancellationToken).ConfigureAwait(false);
            else
                dbTransaction.Commit();
        }

#if !NET
        internal static IEnumerable<TResult> DistinctBy<TElement, TResult>(this IEnumerable<TElement> source, Func<TElement, TResult> keySelector) => source.Select(keySelector).Distinct();
#endif

#if !NET
        public static Task PrepareAsync(this DbCommand dbCommand, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dbCommand);
            GC.KeepAlive(cancellationToken);
            dbCommand.Prepare();
            // PrepareAsync is not available in netstandard2.0
            // This is a no-op to maintain compatibility
            return Task.CompletedTask;
        }
#endif


#if NETSTANDARD
        /// <summary>
        /// CCreates a new <see cref="HashSet{T}"/> from an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="source">The actual enumerable instance.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare keys.</param>
        /// <returns>The created <see cref="HashSet{T}"/> object.</returns>
        internal static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer) =>
            new(source, comparer);

        /// <summary>
        /// Creates a new <see cref="HashSet{T}"/> from an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements=.</typeparam>
        /// <param name="source">The actual enumerable instance.</param>
        /// <returns>The created <see cref="HashSet{T}"/> object.</returns>
        internal static HashSet<T> ToHashSet<T>(this IEnumerable<T> source) => [.. source];
#endif

    }


    internal static partial class AsyncEnumerable
    {
        /// <summary>Creates a list from an <see cref="IAsyncEnumerable{T}"/>.</summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to create a list from.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests. The default is <see cref="CancellationToken.None"/>.</param>
        /// <returns>A list that contains the elements from the input sequence.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source" /> is <see langword="null" />.</exception>
        public static ValueTask<List<TSource>> ToListAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(source);

            return Impl(source.WithCancellation(cancellationToken));

            static async ValueTask<List<TSource>> Impl(
                ConfiguredCancelableAsyncEnumerable<TSource> source)
            {
                List<TSource> list = [];
                await foreach (TSource element in source)
                {
                    list.Add(element);
                }

                return list;
            }
        }

        // TODO: Migrate to Extension Members when C# 14 is available
        internal static T GetAt<T>(this ArraySegment<T> segment, int index)
        {
#if NET
            return segment[index];
#else
            if (index < 0 || index >= segment.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range of the segment.");
            }
            return segment.Array![segment.Offset + index];
#endif
        }
    }
}
