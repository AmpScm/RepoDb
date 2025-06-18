using System.Collections.Concurrent;
using RepoDb.Interfaces;
using RepoDb.Resolvers;

namespace RepoDb;

/// <summary>
/// A class that is being used to cache the mappings between a class property and a <see cref="IClassHandler{TEntity}"/> object.
/// </summary>
public static class ClassHandlerCache
{
    private static readonly ConcurrentDictionary<Type, object?> cache = new();
    private static readonly ClassHandlerResolver resolver = new();

    /// <summary>
    /// Gets the cached <see cref="IClassHandler{TEntity}"/> object that is being mapped to a specific .NET CLR type.
    /// </summary>
    /// <typeparam name="TType">The .NET CLR type.</typeparam>
    /// <typeparam name="TClassHandler">The type of the handler.</typeparam>
    /// <returns>The mapped <see cref="IClassHandler{TEntity}"/> object of the .NET CLR type.</returns>
    public static TClassHandler? Get<TType, TClassHandler>() =>
        Get<TClassHandler>(typeof(TType));

    /// <summary>
    /// Gets the cached <see cref="IClassHandler{TEntity}"/> object that is being mapped to a specific .NET CLR type.
    /// </summary>
    /// <typeparam name="TClassHandler">The type of the handler.</typeparam>
    /// <param name="type">The target .NET CLR type.</param>
    /// <returns>The mapped <see cref="IClassHandler{TEntity}"/> object of the .NET CLR type.</returns>
    public static TClassHandler? Get<TClassHandler>(Type type)
    {
        var value = cache.GetOrAdd(type, resolver.Resolve);

        return Converter.ToType<TClassHandler>(value);
    }

    /// <summary>
    /// Flushes all the existing cached <see cref="IClassHandler{TEntity}"/> objects.
    /// </summary>
    public static void Flush() =>
        cache.Clear();
}
