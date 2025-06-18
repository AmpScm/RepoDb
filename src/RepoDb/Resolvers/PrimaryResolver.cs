using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb.Resolvers;

/// <summary>
/// A class that is being used to resolve the primary property of the data entity type.
/// </summary>
public class PrimaryResolver : IResolver<Type, IEnumerable<ClassProperty>>, IResolver<Type, ClassProperty>
{
    /// <summary>
    /// Resolves the primary <see cref="ClassProperty"/> of the data entity type.
    /// </summary>
    /// <param name="entityType">The type of the data entity.</param>
    /// <returns>The instance of the primary <see cref="ClassProperty"/> object.</returns>
    public IEnumerable<ClassProperty>? Resolve(Type entityType)
    {
        if (PropertyCache.Get(entityType) is not { } properties)
        {
            return null;
        }

        // Get the first entry with Primary or Key attribute
        var pkProperties = properties
            .Where(p => p.GetPrimaryAttribute() != null);

        if (pkProperties.Any())
            return pkProperties;

        // Get from the implicit mapping
        if (!pkProperties.Any())
        {
            if (PrimaryMapper.Get(entityType) is { } v)
                return [v];
        }

        // Id Property
        if (properties.GetByName("id") is { } idProperty)
        {
            return [idProperty];
        }

        // Type.Name + Id
        if (properties.GetByName(entityType.Name + "Id") is { } nameIdProperty)
        {
            return [nameIdProperty];
        }

        // Mapping.Name + Id
        if (ClassMappedNameCache.Get(entityType, false) is { } name && properties.GetByName(name + "Id") is { } mapIdProperty)
        {
            return [mapIdProperty];
        }

        return null;
    }

    ClassProperty? IResolver<Type, ClassProperty>.Resolve(Type input)
    {
        return Resolve(input)?.FirstOrDefault();
    }
}
