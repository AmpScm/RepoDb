using System.Data;
using RepoDb.Interfaces;

namespace RepoDb.Resolvers;

/// <summary>
/// A class that is being used to resolve the <see cref="Field"/> name conversion for SqLite.
/// </summary>
public class SQLiteConvertFieldResolver : DbConvertFieldResolver
{
    /// <summary>
    /// Creates a new instance of <see cref="SQLiteConvertFieldResolver"/> class.
    /// </summary>
    public SQLiteConvertFieldResolver()
        : this(ClientTypeToDbTypeResolver.Instance,
             new DbTypeToSQLiteStringNameResolver())
    { }

    /// <summary>
    /// Creates a new instance of <see cref="SQLiteConvertFieldResolver"/> class.
    /// </summary>
    public SQLiteConvertFieldResolver(IResolver<Type, DbType?> dbTypeResolver,
        IResolver<DbType, string?> stringNameResolver)
        : base(dbTypeResolver,
              stringNameResolver)
    { }
}
