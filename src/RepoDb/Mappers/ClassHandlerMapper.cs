using System.Collections.Concurrent;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb;

/// <summary>
/// A class that is being used to map a .NET CLR type into a <see cref="IClassHandler{TEntity}"/> object.
/// </summary>
public static class ClassHandlerMapper
{
    #region Privates

    private static readonly ConcurrentDictionary<int, object> maps = new();

    #endregion

    #region Methods

    #region Type Level

    /*
     * Add
     */

    /// <summary>
    /// Adds a mapping between a class and a <see cref="IClassHandler{TEntity}"/> object. It uses the <see cref="Activator.CreateInstance(Type)"/> method to create the instance of target <see cref="IClassHandler{TEntity}"/>.
    /// Make sure a default constructor is available for the type of <see cref="IClassHandler{TEntity}"/>, otherwise an exception will be thrown.
    /// </summary>
    /// <typeparam name="TType">The target .NET CLR type.</typeparam>
    /// <typeparam name="TClassHandler">The type of the handler.</typeparam>
    /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
    public static void Add<TType, TClassHandler>(bool force = false)
        where TClassHandler : notnull, new() =>
        Add(typeof(TType), new TClassHandler(), force);

    /// <summary>
    /// Adds a mapping between a .NET CLR type and a <see cref="IClassHandler{TEntity}"/> object.
    /// </summary>
    /// <typeparam name="TType">The target .NET CLR type.</typeparam>
    /// <typeparam name="TClassHandler">The type of the handler.</typeparam>
    /// <param name="classHandler">The instance of the class handler. The type must implement the <see cref="IClassHandler{TEntity}"/> interface.</param>
    /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
    public static void Add<TType, TClassHandler>(TClassHandler classHandler,
        bool force = false) where TClassHandler : notnull, new() =>
        Add(typeof(TType), classHandler, force);

    /// <summary>
    /// Adds a mapping between a .NET CLR type and a <see cref="IClassHandler{TEntity}"/> object.
    /// </summary>
    /// <param name="type">The target .NET CLR type.</param>
    /// <param name="classHandler">The instance of the class handler. The type must implement the <see cref="IClassHandler{TEntity}"/> interface.</param>
    /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
    public static void Add(Type type,
        object classHandler,
        bool force = false)
    {
        // Guard
#if NET
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(classHandler);
#else
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }
        if (classHandler is null)
        {
            throw new ArgumentNullException(nameof(classHandler));
        }
#endif

        if (classHandler.GetType()?.IsInterfacedTo(StaticType.IClassHandler) != true)
        {
            throw new InvalidTypeException($"The type '{type.FullName}' must implement the '{StaticType.IClassHandler.FullName}' interface.");
        }

        // Variables for cache
        var key = GenerateHashCode(type);

        // Try get the mappings
        if (maps.TryGetValue(key, out var value))
        {
            if (force)
            {
                maps.TryUpdate(key, classHandler, value);
            }
            else
            {
                throw new MappingExistsException($"The class handler mapping for '{type.FullName}' already exists.");
            }
        }
        else
        {
            maps.TryAdd(key, classHandler);
        }
    }

    /*
     * Get
     */

    /// <summary>
    /// Get the existing mapped class handler of the .NET CLR type.
    /// </summary>
    /// <typeparam name="TType">The target .NET CLR type.</typeparam>
    /// <typeparam name="TClassHandler">The type of the handler.</typeparam>
    /// <returns>An instance of mapped class handler for .NET CLR type.</returns>
    public static TClassHandler? Get<TType, TClassHandler>() =>
        Get<TClassHandler>(typeof(TType));

    /// <summary>
    /// Get the existing mapped class handler of the .NET CLR type.
    /// </summary>
    /// <typeparam name="TClassHandler">The type of the handler.</typeparam>
    /// <param name="type">The target .NET CLR type.</param>
    /// <returns>An instance of mapped class handler for .NET CLR type.</returns>
    public static TClassHandler? Get<TClassHandler>(Type type)
    {
        // Check the presence
        ObjectExtension.ThrowIfNull(type, nameof(type));

        // get the value
        maps.TryGetValue(GenerateHashCode(type), out var value);

        // Check the result
        return value switch
        {
            null => default,
            TClassHandler classHandler => classHandler,

            // Throw an exception
            _ => throw new InvalidTypeException($"The cache item is not convertible to '{typeof(TClassHandler).FullName}' type.")
        };
    }

    /*
     * Remove
     */

    /// <summary>
    /// Removes the existing mapped class handler of the .NET CLR type.
    /// </summary>
    /// <typeparam name="T">The target .NET CLR type.</typeparam>
    public static void Remove<T>() =>
        Remove(typeof(T));

    /// <summary>
    /// Removes the existing mapped class handler of the .NET CLR type.
    /// </summary>
    /// <param name="type">The target .NET CLR type.</param>
    public static void Remove(Type type)
    {
        // Check the presence
        ObjectExtension.ThrowIfNull(type, nameof(type));

        // Variables for cache
        var key = GenerateHashCode(type);

        // Try get the value
        maps.TryRemove(key, out var _);
    }

    #endregion

    /*
     * Clear
     */

    /// <summary>
    /// Clears all the existing cached <see cref="IClassHandler{TEntity}"/> objects.
    /// </summary>
    public static void Clear()
    {
        maps.Clear();
    }

    #region Helpers

    /// <summary>
    /// Generates a hashcode for caching.
    /// </summary>
    /// <param name="type">The type of the data entity.</param>
    /// <returns>The generated hashcode.</returns>
    private static int GenerateHashCode(Type type) =>
        TypeExtension.GenerateHashCode(type);

    #endregion

    #endregion
}
