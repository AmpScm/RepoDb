using System.Data;
using RepoDb.Interfaces;

namespace RepoDb.Resolvers;

/// <summary>
/// A class that is being used to resolve the <see cref="Field"/> name conversion for Sqlite.
/// </summary>
public class SqliteConvertFieldResolver : DbConvertFieldResolver
{
    /// <summary>
    /// Creates a new instance of <see cref="SqliteConvertFieldResolver"/> class.
    /// </summary>
    public SqliteConvertFieldResolver()
        : this(ClientTypeToDbTypeResolver.Instance,
             DbTypeToSqliteStringNameResolver.Instance)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="SqliteConvertFieldResolver"/> class.
    /// </summary>
    public SqliteConvertFieldResolver(IResolver<Type, DbType?> dbTypeResolver,
        IResolver<DbType, string?> stringNameResolver)
        : base(dbTypeResolver ?? ClientTypeToDbTypeResolver.Instance,
              stringNameResolver ?? DbTypeToSqliteStringNameResolver.Instance)
    { }
}
