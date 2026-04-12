using System.Data;
using System.Data.Common;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Reflection;

namespace RepoDb;

/// <summary>
/// Contains the extension methods for <see cref="IDbConnection"/> object.
/// </summary>
public static partial class DbConnectionExtension
{

    #region Helpers

    private static IEnumerable<T>? QueryMultipleInternal<T>(
        string? cacheKey,
        ICache? cache,
        QueryGroup? where,
        List<QueryGroup?> queryGroups)
        where T : class
    {
        IEnumerable<T>? item = null;

        if (cache != null && !string.IsNullOrEmpty(cacheKey))
        {
            item = cache.Get<IEnumerable<T>>(cacheKey, false)?.Value;
        }

        if (item is null)
        {
            queryGroups.Add(where);
        }

        return item;
    }

    private static IEnumerable<dynamic>? QueryDynamicMultipleInternal(string? cacheKey,
        ICache? cache,
        QueryGroup? where,
        List<QueryGroup?> queryGroups)
    {
        IEnumerable<dynamic>? item = null;

        if (cache != null && !string.IsNullOrEmpty(cacheKey))
        {
            item = cache.Get<IEnumerable<dynamic>>(cacheKey, false)?.Value;
        }

        if (item is null)
        {
            queryGroups.Add(where);
        }

        return item;
    }

    private static IEnumerable<T> QueryMultipleInternal<T>(IDbConnection connection,
        DbDataReader reader,
        IEnumerable<T>? items,
        string? tableName = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        bool forwardToNextResult = true)
    {
        if (forwardToNextResult)
        {
            reader.NextResult();
        }

        if (items is null)
        {
            var dbFields = DbFieldCache.Get(connection, tableName ?? ClassMappedNameCache.Get<T>(), transaction, true);
            items = DataReader.ToEnumerable<T>(reader, dbFields).AsList();
        }

        if (cache != null && !string.IsNullOrEmpty(cacheKey))
        {
            cache.Add(cacheKey, items, cacheItemExpiration, false);
        }

        return items;
    }

    private static IEnumerable<dynamic> QueryDynamicMultipleInternal(IDbConnection connection,
        DbDataReader reader,
        IEnumerable<dynamic>? items,
        string tableName,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        bool forwardToNextResult = true)
    {
        if (forwardToNextResult)
        {
            reader.NextResult();
        }

        if (items is null)
        {
            var dbFields = DbFieldCache.Get(connection, tableName, transaction, true);
            items = DataReader.ToEnumerable(reader, dbFields).AsList();
        }

        if (cache != null && !string.IsNullOrEmpty(cacheKey))
        {
            cache.Add(cacheKey, items, cacheItemExpiration, false);
        }

        return items;
    }

    private static async ValueTask<IEnumerable<T>?> QueryMultipleInternalAsync<T>(
        string? cacheKey,
        ICache? cache,
        QueryGroup? where,
        List<QueryGroup?> queryGroups,
        CancellationToken cancellationToken = default)
        where T : class
    {
        IEnumerable<T>? item = null;

        if (cache != null && !string.IsNullOrEmpty(cacheKey))
        {
            item = (await cache.GetAsync<IEnumerable<T>>(cacheKey, false, cancellationToken).ConfigureAwait(false))?.Value;
        }

        if (item is null)
        {
            queryGroups.Add(where);
        }

        return item;
    }

    private static async ValueTask<IEnumerable<dynamic>?> QueryDynamicMultipleInternalAsync(string? cacheKey,
        ICache? cache,
        QueryGroup? where,
        List<QueryGroup?> queryGroups,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<dynamic>? item = null;

        if (cache != null && !string.IsNullOrEmpty(cacheKey))
        {
            item = (await cache.GetAsync<IEnumerable<dynamic>>(cacheKey, false, cancellationToken).ConfigureAwait(false))?.Value;
        }

        if (item is null)
        {
            queryGroups.Add(where);
        }

        return item;
    }

    private static async ValueTask<IEnumerable<T>> QueryMultipleInternalAsync<T>(IDbConnection connection,
        DbDataReader reader,
        IEnumerable<T>? items,
        string? tableName = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        bool forwardToNextResult = true,
        CancellationToken cancellationToken = default)
    {
        if (forwardToNextResult)
        {
            await reader.NextResultAsync(cancellationToken).ConfigureAwait(false);
        }

        if (items is null)
        {
            var dbFields = await DbFieldCache.GetAsync(connection, tableName ?? ClassMappedNameCache.Get<T>(), transaction, true, cancellationToken).ConfigureAwait(false);
            items = await DataReader.ToEnumerableAsync<T>(reader, dbFields, cancellationToken).ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        if (cache != null && !string.IsNullOrEmpty(cacheKey))
        {
            await cache.AddAsync(cacheKey, items, cacheItemExpiration, false, cancellationToken).ConfigureAwait(false);
        }

        return items;
    }

    private static async ValueTask<IEnumerable<dynamic>> QueryDynamicMultipleInternalAsync(IDbConnection connection,
        DbDataReader reader,
        IEnumerable<dynamic>? items,
        string tableName,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        bool forwardToNextResult = true,
        CancellationToken cancellationToken = default)
    {
        if (forwardToNextResult)
        {
            await reader.NextResultAsync(cancellationToken).ConfigureAwait(false);
        }

        if (items is null)
        {
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, true, cancellationToken).ConfigureAwait(false);
            items = await DataReader.ToEnumerableAsync(reader, dbFields, cancellationToken).ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        if (cache != null && !string.IsNullOrEmpty(cacheKey))
        {
            await cache.AddAsync(cacheKey, items, cacheItemExpiration, false, cancellationToken).ConfigureAwait(false);
        }

        return items;
    }

    private static (string commandText, object param) CombineQueryMultiple(IDbConnection connection, IDbTransaction? transaction, List<string> commandTexts, List<QueryGroupTypeMap> maps, IStatementBuilder? statementBuilder)
    {
        var commandText =
            (statementBuilder ?? connection.GetStatementBuilder()).CombineQueries(commandTexts);
        var param = QueryGroup.AsMappedObject(maps, connection, transaction);

        return (commandText, param);
    }

    private static async ValueTask<(string commandText, object param)> CombineQueryMultipleAsync(IDbConnection connection, IDbTransaction? transaction, List<string> commandTexts, List<QueryGroupTypeMap> maps, IStatementBuilder? statementBuilder, CancellationToken cancellationToken)
    {
        var commandText =
            (statementBuilder ?? connection.GetStatementBuilder()).CombineQueries(commandTexts);
        var param = await QueryGroup.AsMappedObjectAsync(maps, connection, transaction, cancellationToken: cancellationToken).ConfigureAwait(false);

        return (commandText, param);
    }

    private static Func<DbCommand, TraceResult?> CreateCallback(Func<DbCommand, TraceResult?> func) => func;
    private static Func<DbCommand, CancellationToken, ValueTask<TraceResult?>> CreateCallbackAsync(Func<DbCommand, CancellationToken, ValueTask<TraceResult?>> func) => func;


    #endregion
}
