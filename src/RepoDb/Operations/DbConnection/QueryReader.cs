using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Reflection;
using RepoDb.Requests;

namespace RepoDb;

/// <summary>
/// Contains the extension methods for <see cref="IDbConnection"/> object.
/// </summary>
public static partial class DbConnectionExtension
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="connection"></param>
    /// <param name="tableName"></param>
    /// <param name="where"></param>
    /// <param name="fields"></param>
    /// <param name="orderBy"></param>
    /// <param name="top"></param>
    /// <param name="offset"></param>
    /// <param name="hints"></param>
    /// <param name="commandTimeout"></param>
    /// <param name="traceKey"></param>
    /// <param name="transaction"></param>
    /// <param name="trace"></param>
    /// <param name="statementBuilder"></param>
    /// <returns></returns>
    public static IEnumerable<TEntity> QueryReader<TEntity>(
        this IDbConnection connection,
        string tableName,
        QueryGroup? where,
        IEnumerable<Field>? fields = null,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        int offset = 0,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryReader,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        fields ??= GetQualifiedFields<TEntity>() ??
            DbFieldCache.Get(connection, tableName, transaction)?.AsFields();

        // Variables
        var commandType = CommandType.Text;
        var request = new QueryRequest(tableName,
            connection,
            transaction,
            fields,
            where,
            orderBy,
            top,
            offset,
            hints,
            statementBuilder);

        // Converts to property mapped object
        var param = (where != null) ? QueryGroup.AsMappedObject([where.MapTo<TEntity>(tableName)], connection, transaction, tableName) : null;

        var commandText = CommandTextCache.GetCached(request, CommandTextCache.GetQueryText);

        // DB Fields
        var dbFields = DbFieldCache.Get(connection, tableName, transaction);

        // Execute the actual method
        using var command = CreateDbCommandForExecution(connection: (DbConnection)connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            commandTimeout: commandTimeout,
            transaction: transaction,
            entityType: typeof(TEntity),
            dbFields: dbFields);


        // Execute
        using var reader = command.ExecuteReaderInternal(trace, traceKey);
        foreach (var el in DataReader.ToEnumerable<TEntity>(reader, dbFields))
        {
            yield return el;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="connection"></param>
    /// <param name="where"></param>
    /// <param name="fields"></param>
    /// <param name="orderBy"></param>
    /// <param name="top"></param>
    /// <param name="offset"></param>
    /// <param name="hints"></param>
    /// <param name="commandTimeout"></param>
    /// <param name="traceKey"></param>
    /// <param name="transaction"></param>
    /// <param name="trace"></param>
    /// <param name="statementBuilder"></param>
    /// <returns></returns>
    public static IEnumerable<TEntity> QueryReader<TEntity>(
        this IDbConnection connection,
        QueryGroup? where = null,
        IEnumerable<Field>? fields = null,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        int offset = 0,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryReader,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return QueryReader<TEntity>(
            connection,
            ClassMappedNameCache.Get<TEntity>(),
            where,
            fields,
            orderBy,
            top,
            offset,
            hints,
            commandTimeout,
            traceKey,
            transaction,
            trace,
            statementBuilder);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="connection"></param>
    /// <param name="where"></param>
    /// <param name="fields"></param>
    /// <param name="orderBy"></param>
    /// <param name="top"></param>
    /// <param name="offset"></param>
    /// <param name="hints"></param>
    /// <param name="commandTimeout"></param>
    /// <param name="traceKey"></param>
    /// <param name="transaction"></param>
    /// <param name="trace"></param>
    /// <param name="statementBuilder"></param>
    /// <returns></returns>
    public static IEnumerable<TEntity> QueryReader<TEntity>(
        this IDbConnection connection,
        Expression<Func<TEntity, bool>> where,
        IEnumerable<Field>? fields = null,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        int offset = 0,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryReader,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        return QueryReader<TEntity>(
            connection,
            tableName: ClassMappedNameCache.Get<TEntity>(),
            where: connection.ToQueryGroup(where, transaction),
            fields: fields,
            orderBy: orderBy,
            top: top,
            offset: offset,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="connection"></param>
    /// <param name="tableName"></param>
    /// <param name="where"></param>
    /// <param name="fields"></param>
    /// <param name="orderBy"></param>
    /// <param name="top"></param>
    /// <param name="offset"></param>
    /// <param name="hints"></param>
    /// <param name="commandTimeout"></param>
    /// <param name="traceKey"></param>
    /// <param name="transaction"></param>
    /// <param name="trace"></param>
    /// <param name="statementBuilder"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async IAsyncEnumerable<TEntity> QueryReaderAsync<TEntity>(
        this IDbConnection connection,
        string tableName,
        QueryGroup? where,
        IEnumerable<Field>? fields = null,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        int offset = 0,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryReader,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
        where TEntity : class
    {
        fields ??= GetQualifiedFields<TEntity>() ??
            (await DbFieldCache.GetInternalAsync(connection, tableName, transaction, cancellationToken: cancellationToken).ConfigureAwait(false))?.AsFields();

        // Variables
        var commandType = CommandType.Text;
        var request = new QueryRequest(tableName,
            connection,
            transaction,
            fields,
            where,
            orderBy,
            top,
            offset,
            hints,
            statementBuilder);

        // Converts to property mapped object
        var param = (where != null) ? await QueryGroup.AsMappedObjectAsync([where.MapTo<TEntity>(tableName)], connection, transaction, tableName, cancellationToken).ConfigureAwait(false) : null;

        var commandText = CommandTextCache.GetCached(request, CommandTextCache.GetQueryText);

        // DB Fields
        var dbFields = await DbFieldCache.GetInternalAsync(connection, tableName, transaction, false, cancellationToken).ConfigureAwait(false);

        // Execute the actual method
#if NET
        await
#endif
        using var command = await CreateDbCommandForExecutionAsync(connection: (DbConnection)connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            commandTimeout: commandTimeout,
            transaction: transaction,
            entityType: typeof(TEntity),
            dbFields: dbFields,
            cancellationToken: cancellationToken);


        // Execute
#if NET
        await
#endif
        using var reader = await command.ExecuteReaderInternalAsync(trace, traceKey, cancellationToken).ConfigureAwait(false);
        await foreach (var el in DataReader.ToEnumerableAsync<TEntity>(reader, dbFields, cancellationToken).ConfigureAwait(false))
        {
            yield return el;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="connection"></param>
    /// <param name="where"></param>
    /// <param name="fields"></param>
    /// <param name="orderBy"></param>
    /// <param name="top"></param>
    /// <param name="offset"></param>
    /// <param name="hints"></param>
    /// <param name="commandTimeout"></param>
    /// <param name="traceKey"></param>
    /// <param name="transaction"></param>
    /// <param name="trace"></param>
    /// <param name="statementBuilder"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IAsyncEnumerable<TEntity> QueryReaderAsync<TEntity>(
        this IDbConnection connection,
        QueryGroup? where = null,
        IEnumerable<Field>? fields = null,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        int offset = 0,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryReader,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return QueryReaderAsync<TEntity>(
            connection,
            ClassMappedNameCache.Get<TEntity>(),
            where,
            fields,
            orderBy,
            top,
            offset,
            hints,
            commandTimeout,
            traceKey,
            transaction,
            trace,
            statementBuilder,
            cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="connection"></param>
    /// <param name="where"></param>
    /// <param name="fields"></param>
    /// <param name="orderBy"></param>
    /// <param name="top"></param>
    /// <param name="offset"></param>
    /// <param name="hints"></param>
    /// <param name="commandTimeout"></param>
    /// <param name="traceKey"></param>
    /// <param name="transaction"></param>
    /// <param name="trace"></param>
    /// <param name="statementBuilder"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IAsyncEnumerable<TEntity> QueryReaderAsync<TEntity>(
        this IDbConnection connection,
        Expression<Func<TEntity, bool>> where,
        IEnumerable<Field>? fields = null,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        int offset = 0,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.QueryReader,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return QueryReaderAsync<TEntity>(
            connection,
            tableName: ClassMappedNameCache.Get<TEntity>(),
            where: connection.ToQueryGroup(where, transaction),
            fields: fields,
            orderBy: orderBy,
            top: top,
            offset: offset,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken);
    }
}
