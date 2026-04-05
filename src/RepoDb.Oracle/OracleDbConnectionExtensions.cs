using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using RepoDb.Interfaces;
using RepoDb.StatementBuilders;

namespace RepoDb;

/// <summary>
/// 
/// </summary>
public static class OracleDbConnectionExtensions
{
    /// <summary>
    /// Combines multiple SQL query strings into a single query statement appropriate for the specified database
    /// connection.
    /// </summary>
    /// <remarks>The method uses the statement builder associated with the provided connection to combine the
    /// queries, ensuring compatibility with the underlying database provider.</remarks>
    /// <param name="connection">The database connection used to determine the appropriate statement builder for combining the queries. Cannot be
    /// null.</param>
    /// <param name="queries">An array of SQL query strings to combine. If the array is empty or null, an empty string is returned.</param>
    /// <returns>A single SQL query string that represents the combination of the provided queries. Returns an empty string if no
    /// queries are provided, or the original query if only one is specified.</returns>
    public static string CombineQueries(this IDbConnection connection, params string[] queries)
    {
        if (queries == null || queries.Length == 0)
        {
            return "";
        }
        else if (queries.Length == 1)
        {
            return queries[0];
        }
        else
        {
            return (connection.GetStatementBuilder() as BaseStatementBuilder)?.CombineQueries(queries) ?? string.Join(";", queries);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="commandTexts"></param>
    /// <param name="param"></param>
    /// <param name="commandType"></param>
    /// <param name="cacheKey"></param>
    /// <param name="cacheItemExpiration"></param>
    /// <param name="traceKey"></param>
    /// <param name="commandTimeout"></param>
    /// <param name="transaction"></param>
    /// <param name="cache"></param>
    /// <param name="trace"></param>
    /// <returns></returns>
    public static QueryMultipleExtractor ExecuteQueryMultiple(this IDbConnection connection,
        string[] commandTexts,
        object? param = null,
        CommandType commandType = default,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        string? traceKey = TraceKeys.ExecuteQueryMultiple,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null) =>
        connection.ExecuteQueryMultiple(
            connection.CombineQueries(commandTexts),
            param,
            commandType,
            cacheKey,
            cacheItemExpiration,
            traceKey,
            commandTimeout,
            transaction,
            cache,
            trace);


    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="commandTexts"></param>
    /// <param name="param"></param>
    /// <param name="commandType"></param>
    /// <param name="cacheKey"></param>
    /// <param name="cacheItemExpiration"></param>
    /// <param name="traceKey"></param>
    /// <param name="commandTimeout"></param>
    /// <param name="transaction"></param>
    /// <param name="cache"></param>
    /// <param name="trace"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<QueryMultipleExtractor> ExecuteQueryMultipleAsync(this IDbConnection connection,
        string[] commandTexts,
        object? param = null,
        CommandType commandType = default,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        string? traceKey = TraceKeys.ExecuteQueryMultiple,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ICache? cache = null,
        ITrace? trace = null,
        CancellationToken cancellationToken = default) =>
        connection.ExecuteQueryMultipleAsync(
            connection.CombineQueries(commandTexts),
            param,
            commandType,
            cacheKey,
            cacheItemExpiration,
            traceKey,
            commandTimeout,
            transaction,
            cache,
            trace,
            cancellationToken);
}
