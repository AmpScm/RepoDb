using RepoDb.Interfaces;

namespace RepoDb.Resolvers;

/// <summary>
/// A class that is being used to resolve the SqLite Database Types into its equivalent .NET CLR Types. This is only used for 'System.Data.SQLite.Core' library.
/// </summary>
public class SqLiteDbTypeNameToClientTypeResolver : IResolver<string, Type>
{
    /// <summary>
    /// Returns the equivalent .NET CLR Types of the Database Type.
    /// </summary>
    /// <param name="dbTypeName">The name of the database type.</param>
    /// <returns>The equivalent .NET CLR type.</returns>
    public Type Resolve(string dbTypeName)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(dbTypeName);

        return dbTypeName.ToLowerInvariant() switch
        {
            "bigint" or "integer" => typeof(long),
            "blob" => typeof(byte[]),
            "boolean" => typeof(long),
            "char" or "string" or "text" or "varchar" or "nvarchar" or "varchar2" => typeof(string),
            "date" or "datetime" => typeof(DateTime),
            "datetimeoffset" => typeof(DateTimeOffset),
            "time" => typeof(DateTime),
            "decimal" or "numeric" => typeof(decimal),
            "double" or "real" => typeof(double),
            "int" => typeof(int),
            "none" => typeof(object),
            _ when (dbTypeName.IndexOfAny(new char[] { '(', ']' }) is { } p && p > 0) => Resolve(dbTypeName.Substring(0, p)), // varchar(3) => varchar, etc.
            _ => typeof(object),
        };
    }
}
