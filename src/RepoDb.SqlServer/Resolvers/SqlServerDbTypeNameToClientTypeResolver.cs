using RepoDb.Interfaces;
using RepoDb.Types;

namespace RepoDb.Resolvers;

/// <summary>
/// A class used to resolve the SQL Database Types into its equivalent .NET CLR Types.
/// </summary>
public class SqlServerDbTypeNameToClientTypeResolver : IResolver<string, Type>
{
    /*
     * Taken:
     * https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings
     */

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
            "bigint" => typeof(long),
            "attribute" or "binary" or "filestream" or "image" or "rowversion" or "timestamp" or "varbinary" or "varbinary(max)" => typeof(byte[]),
            "bit" => typeof(bool),
            "char" or "nchar" or "ntext" or "nvarchar" or "text" or "varchar" or "xml" => typeof(string),
            "date" =>
#if NET
            GlobalConfiguration.Options.DateOnlyAndTimeOnly ? typeof(DateOnly) :
#endif
            typeof(DateTime),
            "datetime" or "datetime2" or "smalldatetime" => typeof(DateTime),
            "datetimeoffset" => typeof(DateTimeOffset),
            "decimal" or "money" or "numeric" or "smallmoney" => typeof(decimal),
            "float" => typeof(double),
            "int" => typeof(int),
            "real" => typeof(float),
            "smallint" => typeof(short),
            "sql_variant" => typeof(SqlVariant),
            "time" =>
#if NET
            GlobalConfiguration.Options.DateOnlyAndTimeOnly ? typeof(TimeOnly) :
#endif
            typeof(TimeSpan),
            "tinyint" => typeof(byte),
            "uniqueidentifier" => typeof(Guid),
            _ => typeof(object),
        };
    }
}
