using System.Data;
using RepoDb.Interfaces;

namespace RepoDb.Resolvers;

/// <summary>
/// A class that is being used to resolve the <see cref="DbType"/> into its equivalent database string name.
/// </summary>
public class DbTypeToOracleStringNameResolver : IResolver<DbType, string?>
{
    /// <summary>
    /// Returns the equivalent <see cref="DbType"/> of the .NET CLR Types.
    /// </summary>
    /// <param name="dbType">The type of the database.</param>
    /// <returns>The equivalent string name.</returns>
    public virtual string? Resolve(DbType dbType)
    {
        return dbType switch
        {
            DbType.AnsiString => "VARCHAR2",
            DbType.AnsiStringFixedLength => "CHAR",
            DbType.String => "NVARCHAR2",
            DbType.StringFixedLength => "NCHAR",
            DbType.Binary => "BLOB",
            DbType.Boolean => "NUMBER(1)",
            DbType.Byte => "NUMBER(3)",
            DbType.SByte => "NUMBER(3)",
            DbType.Int16 => "NUMBER(5)",
            DbType.UInt16 => "NUMBER(5)",
            DbType.Int32 => "NUMBER(10)",
            DbType.UInt32 => "NUMBER(10)",
            DbType.Int64 => "NUMBER(19)",
            DbType.UInt64 => "NUMBER(19)",
            DbType.Single => "BINARY_FLOAT",
            DbType.Double => "BINARY_DOUBLE",
            DbType.Decimal => "NUMBER",
            DbType.VarNumeric => "NUMBER",
            DbType.Currency => "NUMBER(19,4)",
            DbType.Date => "DATE",
            DbType.DateTime  or DbType.DateTime2 => "TIMESTAMP",
            DbType.DateTimeOffset => "TIMESTAMP WITH TIME ZONE",
            DbType.Time => "INTERVAL DAY TO SECOND",
            DbType.Guid => "RAW(16)",
            DbType.Xml => "XMLTYPE",
            DbType.Object => "BLOB", // or custom object type
            _ => null
        };
    }
}
