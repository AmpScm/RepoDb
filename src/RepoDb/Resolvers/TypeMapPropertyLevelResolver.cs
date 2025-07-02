using System.Data;
using System.Reflection;
using RepoDb.Attributes;
using RepoDb.Attributes.Parameter;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb.Resolvers;

/// <summary>
/// A class that is being used to resolve the equivalent <see cref="DbType"/> object of the property.
/// </summary>
public class TypeMapPropertyLevelResolver : IResolver<PropertyInfo, DbType?>
{
    /// <summary>
    /// Resolves the equivalent <see cref="DbType"/> object of the property.
    /// </summary>
    /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/> to be resolved.</param>
    /// <returns>The equivalent <see cref="DbType"/> object of the property.</returns>
    public DbType? Resolve(PropertyInfo propertyInfo)
    {
        DbType? dbType = null;

        // Attribute Level
        var attribute = propertyInfo.GetCustomAttribute<TypeMapAttribute>() ??
            propertyInfo.GetCustomAttribute<DbTypeAttribute>();
        if (attribute != null)
        {
            dbType = attribute.DbType;
        }

        // Property Level
        dbType ??= TypeMapper.Get(propertyInfo.DeclaringType!, propertyInfo);

        // Return the value
        return dbType;
    }
}
