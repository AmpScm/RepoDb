#nullable enable
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace RepoDb.Extensions;

/// <summary>
/// An extension class for <see cref="IEnumerable{T}"/>.
/// </summary>
public static class EnumerableExtension
{
    /// <summary>
    /// Split the enumerable into multiple enumerables.
    /// </summary>
    /// <typeparam name="T">The target dynamic type of the enumerable.</typeparam>
    /// <param name="value">The actual enumerable instance.</param>
    /// <param name="sizePerSplit">The sizes of the items per split.</param>
    /// <returns>An enumerable of enumerables.</returns>
    public static IEnumerable<T[]> Split<T>(this IEnumerable<T> value,
        int sizePerSplit)
    {
        var count = value.Count();
        if (sizePerSplit <= 0 || count <= sizePerSplit)
        {
            return new[] { value.ToArray() };
        }
#if !NET
        else
        {
            var batchCount = (count / sizePerSplit) + ((count % sizePerSplit) != 0 ? 1 : 0);
            var array = new T[batchCount][];
            for (var i = 0; i < batchCount; i++)
            {
                array[i] = value.Where((_, index) =>
                    {
                        return index >= (sizePerSplit * i) &&
                            index < (sizePerSplit * i) + sizePerSplit;
                    })
                    .ToArray();
            }
            return array;
        }
#else
        return value.Chunk(sizePerSplit);
#endif
    }

    private const uint MinOptimalChunkSize = 10;
    private static readonly uint[] DefaultChunkSizes = [100000, 50000, 20000, 10000, 5000, 2000, 1000, 500, 200, 100, 50, 20, MinOptimalChunkSize];

    /// <summary>
    /// Splits a collection into optimally-sized chunks to minimize cached database plan overhead.
    /// Uses a small set of standard chunk sizes to maximize cache reuse while maintaining efficiency.
    /// </summary>
    /// <typeparam name="T">The type of elements in the source collection.</typeparam>
    /// <param name="source">The source collection to chunk.</param>
    /// <param name="maxChunkSize">The maximum size for any single chunk. Defaults to 2000.</param>
    /// <returns>
    /// An enumerable of arrays, where each array represents a chunk of the original collection.
    /// Small collections (≤10 items) are returned as-is without chunking.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    /// <remarks>
    /// <para>
    /// This method optimizes for database operations that cache execution plans based on parameter count.
    /// By using a limited set of standard chunk sizes (20, 50, 100, 200, 500, etc.), it minimizes
    /// the number of cached plans while maintaining good performance characteristics.
    /// </para>
    /// <para>
    /// Chunking strategy:
    /// <list type="bullet">
    /// <item><description>Collections with ≤10 items: No chunking (returned as single chunk)</description></item>
    /// <item><description>Larger collections: Uses standard sizes progressing through 20, 50, 100, 200, 500, 1000, 2000, 5000...</description></item>
    /// <item><description>Always includes the specified <paramref name="maxChunkSize"/> for optimal large dataset handling</description></item>
    /// <item><description>Automatically drops standard sizes that are too close to the maximum (within 20%) to avoid redundancy</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Performance characteristics:
    /// <list type="bullet">
    /// <item><description>Memory efficient: Copies source once, then uses array slicing</description></item>
    /// <item><description>Cache friendly: Typically generates only 5-8 different chunk sizes total</description></item>
    /// <item><description>Large dataset optimized: Always uses maximum chunk size when beneficial</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var items = Enumerable.Range(1, 2500);
    /// 
    /// // Creates chunks of sizes: 2000, 500 (2 different cached plans)
    /// var chunks = items.ChunkOptimally(maxChunkSize: 2000);
    /// 
    /// // Small collection optimization
    /// var smallItems = Enumerable.Range(1, 8);
    /// var smallChunks = smallItems.ChunkOptimally(); // Returns single chunk of 8 items
    /// </code>
    /// </example>
    public static IEnumerable<ArraySegment<T>> ChunkOptimally<T>(
        this IEnumerable<T> source,
        int maxChunkSize = 2000)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));
        else if (maxChunkSize < 1)
            throw new ArgumentOutOfRangeException(nameof(maxChunkSize));

        var array = source as T[] ?? source.ToArray();

        if (array.Length == 0)
            yield break;

        if (array.Length <= MinOptimalChunkSize)
        {
            yield return new ArraySegment<T>(array);
            yield break;
        }

        int i = 0;
        foreach (var chunkSize in OptimalChunkSizes(array.Length, maxChunkSize))
        {
            while (i + chunkSize <= array.Length)
            {
                yield return new ArraySegment<T>(array, i, chunkSize);
                i += chunkSize;
            }
        }

        if (i < array.Length)
        {
            yield return new ArraySegment<T>(array, i, array.Length - i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static IEnumerable<int> OptimalChunkSizes(int elementCount, int maxChunkSize)
        {
            if (maxChunkSize >= elementCount)
            {
                yield return elementCount;
                yield break;
            }

            yield return maxChunkSize;

            foreach (var c in DefaultChunkSizes)
            {
                if (c <= elementCount && c < maxChunkSize)
                    yield return (int)c;
            }
        }
    }

    /// <summary>
    /// Checks whether the instance of <see cref="IEnumerable"/> is of type <see cref="IEnumerable{T}"/>, then casts it, otherwise, 
    /// returns the instance of <see cref="IEnumerable{T}"/> with the specified items. The items that are not of type <typeparamref name="T"/> will be
    /// eliminated from the result. This method is using the underlying method <see cref="Enumerable.OfType{TResult}(IEnumerable)"/>.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="value">The actual enumerable instance.</param>
    /// <returns>The <see cref="IEnumerable{T}"/> object in which the items are of type <typeparamref name="T"/>.</returns>
    public static IEnumerable<T> WithType<T>(this IEnumerable value) =>
        value as IEnumerable<T> ?? value.OfType<T>();

    /// <summary>
    /// Checks whether the instance of <see cref="IEnumerable{T}"/> is of type <see cref="List{T}"/>, then casts it, otherwise, converts it.
    /// This method is using the underlying method <see cref="Enumerable.ToList{TSource}(IEnumerable{TSource})"/> method.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="value">The actual enumerable instance.</param>
    /// <returns>The converted <see cref="IList{T}"/> object.</returns>
    public static List<T> AsList<T>(this IEnumerable<T> value) =>
        value as List<T> ?? value.ToList();

    private static MethodInfo? _distinctMethod;
    private static MethodInfo? _toListMethod;

    public static ICollection AsTypedSet(this IEnumerable value, bool distinct = false)
    {
        if (value.GetEnumerableElementType() is not Type elementType)
        {
            Type? p = null;
            foreach (var v in value)
            {
                if (v is null)
                {
                    continue;
                }
                var vt = v.GetType();

                p ??= vt;

                while (!p.IsAssignableFrom(vt))
                    p = p.BaseType;

                if (p is null || p == StaticType.Object)
                    break;
            }
            elementType = p ?? StaticType.Object;

            var newValue = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType))!;

            foreach (var v in value)
            {
                if (v is not null)
                {
                    newValue.Add(v);
                }
            }
            value = newValue;
        }

        if (distinct)
        {
            _distinctMethod = typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static).Single(m => m.Name == nameof(Enumerable.Distinct) && m.IsGenericMethodDefinition
                && m.GetParameters() is { } p && p.Length == 1 && p[0].ParameterType is { } pt && pt.IsGenericType && pt.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            value = (IEnumerable)_distinctMethod.MakeGenericMethod([elementType]).Invoke(null, [value])!;
        }

        if (value is not ICollection collection)
        {
            _toListMethod ??= typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static).Single(m => m.Name == nameof(Enumerable.ToList) && m.IsGenericMethodDefinition
                && m.GetParameters() is { } p && p.Length == 1 && p[0].ParameterType is { } pt && pt.IsGenericType && pt.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            collection = (ICollection)_toListMethod.MakeGenericMethod([elementType]).Invoke(null, [value])!;
        }

        return collection;
    }

    public static Type GetElementType(this IEnumerable enumerable)
    {
        if (enumerable is null)
        {
            throw new ArgumentNullException(nameof(enumerable));
        }
        var elementType = enumerable.GetEnumerableElementType();

        if (elementType is null)
        {
            Type? p = null;
            foreach (var v in enumerable)
            {
                if (v is null)
                {
                    continue;
                }
                var vt = v.GetType();

                p ??= vt;

                while (!p.IsAssignableFrom(vt))
                    p = p.BaseType;

                if (p is null || p == StaticType.Object)
                    break;
            }
            elementType = p ?? StaticType.Object;
        }

        return elementType;
    }

    internal static Type? GetEnumerableElementType(this IEnumerable enumerable)
    {
        if (enumerable is null)
        {
            throw new ArgumentNullException(nameof(enumerable));
        }

        var enumerableType = enumerable.GetType();
        if (enumerableType.IsArray)
        {
            return enumerableType.GetElementType() ?? typeof(object);
        }
        else if (enumerableType.IsGenericType && enumerableType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            return enumerableType.GetGenericArguments()[0];
        }
        else if (enumerableType.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)) is Type interfaceType)
        {
            return interfaceType.GetGenericArguments()[0];
        }

        return null;
    }

    /// <summary>
    /// Checks whether the instance of <see cref="IEnumerable{T}"/> is an array of <typeparamref name="T"/>, then casts it, otherwise, converts it.
    /// This method is using the underlying method <see cref="Enumerable.ToArray{TSource}(IEnumerable{TSource})"/> method.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="value">The actual enumerable instance.</param>
    /// <returns>The converted <see cref="Array"/> object.</returns>
    public static T[] AsArray<T>(this IEnumerable<T> value) =>
        value as T[] ?? value.ToArray();

    /// <summary>
    /// Gets a value indicating whether the current collection is null or empty.
    /// </summary>
    /// <param name="value">The target type.</param>
    /// <typeparam name="T">The actual enumerable instance.</typeparam>
    /// <returns>A value indicating whether the collection is null or empty.</returns>
    internal static bool IsNullOrEmpty<T>(
        [NotNullWhen(true)] this IEnumerable<T>? value) => value?.Any() != true;

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
