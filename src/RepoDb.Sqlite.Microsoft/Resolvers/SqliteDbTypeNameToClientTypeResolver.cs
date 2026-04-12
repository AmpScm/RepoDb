using RepoDb.Interfaces;

namespace RepoDb.Resolvers;

/// <summary>
/// A class that is being used to resolve the Sqlite Database Types into its equivalent .NET CLR Types. This is only used for 'Microsoft.Data.Sqlite' library.
/// </summary>
public class SqliteDbTypeNameToClientTypeResolver : IResolver<string, Type>
{
    /// <summary>
    /// Returns the equivalent .NET CLR Types of the Database Type.
    /// </summary>
    /// <param name="dbTypeName">The name of the database type.</param>
    /// <returns>The equivalent .NET CLR type.</returns>
    public virtual Type Resolve(string dbTypeName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dbTypeName);

        return dbTypeName.ToLowerInvariant() switch
        {
            "integer" or "int" or "bigint" => typeof(long),
            "blob" or "binary" or "varbinary" or "bytea" => typeof(byte[]),
            "text" or "boolean" or "char" or "string" or "varchar" or "nvarchar" or "varchar2" or "none" => typeof(string),
#if NET
            "date" when GlobalConfiguration.Options.DateOnlyAndTimeOnly => typeof(DateOnly),
#endif
            "date" or "datetime" => typeof(DateTime),
            "datetimeoffset" => typeof(DateTimeOffset),
            "time" => typeof(DateTime),
            "decimal" or "numeric" => typeof(decimal),
            "double" or "real" or "float" => typeof(double),
            "tinyint" or "smallint" or "bit" => typeof(int),
            _ when dbTypeName.IndexOfAny(['(', ']']) is { } p && p > 0 => Resolve(dbTypeName.Substring(0, p).Trim()), // varchar(3) => varchar, etc.
            _ => typeof(object),
        };
    }

    /// <summary>
    /// 
    /// </summary>
    public static readonly SqliteDbTypeNameToClientTypeResolver Instance = new();
}
