using RepoDb.Interfaces;

namespace RepoDb.Resolvers;

/// <summary>
/// A class that is being used to resolve the SqLite Database Types into its equivalent .NET CLR Types. This is only used for 'Microsoft.Data.Sqlite' library.
/// </summary>
public class SqLiteDbTypeNameToClientTypeResolver : IResolver<string, Type>
{
    /// <summary>
    /// Returns the equivalent .NET CLR Types of the Database Type.
    /// </summary>
    /// <param name="dbTypeName">The name of the database type.</param>
    /// <returns>The equivalent .NET CLR type.</returns>
    public virtual Type Resolve(string dbTypeName)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(dbTypeName);

        return dbTypeName.ToLowerInvariant() switch
        {
            "integer" or "int" or "bigint" => typeof(long),
            "blob" or "binary" or "varbinary" or "bytea" => typeof(byte[]),
            "text" or "boolean" or "char" or "string" or "varchar" or "nvarchar" or "varchar2" or "none" => typeof(string),
            "date" or "datetime" => typeof(DateTime),
            "datetimeoffset" => typeof(DateTimeOffset),
            "time" => typeof(DateTime),
            "decimal" or "numeric" => typeof(decimal),
            "double" or "real" => typeof(double),
            "tinyint" or "smallint" or "bit" => typeof(int),
            _ when dbTypeName.IndexOfAny(['(', ']']) is { } p && p > 0 => Resolve(dbTypeName.Substring(0, p)), // varchar(3) => varchar, etc.
            _ => typeof(object),
        };
    }
}
