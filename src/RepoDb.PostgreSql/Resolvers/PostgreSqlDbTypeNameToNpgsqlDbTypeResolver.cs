using NpgsqlTypes;
using RepoDb.Interfaces;

namespace RepoDb.Resolvers;

/// <summary>
/// A class that is being used to resolve the PostgreSql Database Types into its <see cref="NpgsqlDbType"/>.
/// </summary>
public class PostgreSqlDbTypeNameToNpgsqlDbTypeResolver : IResolver<string, NpgsqlDbType?>
{
    /// <summary>
    /// Returns the equivalent <see cref="NpgsqlDbType"/> of the Database Type.
    /// </summary>
    /// <param name="dbTypeName">The name of the database type.</param>
    /// <returns>The equivalent <see cref="NpgsqlDbType"/>.</returns>
    public virtual NpgsqlDbType? Resolve(string dbTypeName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dbTypeName);

        // Try parse
        if (Enum.TryParse<NpgsqlDbType>(dbTypeName, true, out var result))
        {
            return result;
        }

        // User-Defined
        if ("USER-DEFINED".Equals(dbTypeName, StringComparison.OrdinalIgnoreCase))
        {
            return NpgsqlDbType.Unknown;
        }

        // Covert to .NET CLR Type
        var clientTypeResolver = new PostgreSqlDbTypeNameToClientTypeResolver()
            .Resolve(dbTypeName);

        // Try resolve
        return new ClientTypeToNpgsqlDbTypeResolver().Resolve(clientTypeResolver);
    }

    /// <summary>
    /// The default instance of <see cref="PostgreSqlDbTypeNameToNpgsqlDbTypeResolver"/> to be used as the default resolver.
    /// </summary>
    public static readonly PostgreSqlDbTypeNameToNpgsqlDbTypeResolver Instance = new();
}
