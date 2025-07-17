using System.Reflection;
using RepoDb.Attributes;
using RepoDb.Interfaces;

namespace RepoDb.Resolvers;

/// <summary>
/// A class that is being used to resolve the equivalent <see cref="IClassHandler{TEntity}"/> object of the .NET CLR type.
/// </summary>
public class ClassHandlerResolver : IResolver<Type, object?>
{
    /// <summary>
    /// Resolves the equivalent <see cref="IClassHandler{TEntity}"/> object of the .NET CLR type.
    /// </summary>
    /// <param name="type">The .NET CLR type</param>
    /// <returns>The equivalent <see cref="IClassHandler{TEntity}"/> object of the .NET CLR type.</returns>
    public object? Resolve(Type type)
    {
        object? classHandler = null;

        var attribute = type.GetCustomAttribute<ClassHandlerAttribute>();
        if (attribute is not null)
        {
            classHandler = Activator.CreateInstance(attribute.HandlerType);
        }

        return classHandler ?? ClassHandlerMapper.Get<object>(type);
    }
}
