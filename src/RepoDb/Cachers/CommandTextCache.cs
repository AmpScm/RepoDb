using System.Collections.Concurrent;
using System.Data;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Requests;

namespace RepoDb;

/// <summary>
/// A class that is being used to cache the already-built command texts.
/// </summary>
public static class CommandTextCache
{
    private static readonly ConcurrentDictionary<BaseRequest, string> cache = new();

    #region GetAverageText

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    internal static string GetAverageText(AverageRequest request)
    {
        var commandText = cache.GetOrAdd(request, (_) =>
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            return statementBuilder.CreateAverage(request.Name,
                request.Field,
                request.Where,
                request.Hints);
        });
        return commandText;
    }

    #endregion

    #region GetAverageAllText

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    internal static string GetAverageAllText(AverageAllRequest request)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            commandText = statementBuilder.CreateAverageAll(request.Name,
                request.Field,
                request.Hints);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    #endregion

    #region GetBatchQueryText

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    internal static string GetBatchQueryText(BatchQueryRequest request)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var fields = GetTargetFields(request.Connection,
                request.Name,
                request.Fields,
                request.Transaction);
            ValidateOrderFields(request.Connection,
                request.Name,
                request.OrderBy,
                request.Transaction);
            commandText = GetBatchQueryTextInternal(request, fields);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal static async ValueTask<string> GetBatchQueryTextAsync(BatchQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var fields = await GetTargetFieldsAsync(request.Connection,
                request.Name,
                request.Fields,
                request.Transaction,
                cancellationToken).ConfigureAwait(false);
            await ValidateOrderFieldsAsync(request.Connection,
                request.Name,
                request.OrderBy,
                request.Transaction,
                cancellationToken).ConfigureAwait(false);
            commandText = GetBatchQueryTextInternal(request, fields);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    internal static string GetBatchQueryTextInternal(BatchQueryRequest request,
        IEnumerable<Field> fields)
    {
        var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
        return statementBuilder.CreateBatchQuery(request.Name,
            fields,
            request.Page,
            request.RowsPerBatch,
            request.OrderBy,
            request.Where,
            request.Hints);
    }

    #endregion

    #region GetCountText

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    internal static string GetCountText(CountRequest request)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            commandText = statementBuilder.CreateCount(request.Name,
                request.Where,
                request.Hints);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    #endregion

    #region GetCountAllText

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    internal static string GetCountAllText(CountAllRequest request)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            commandText = statementBuilder.CreateCountAll(request.Name,
                request.Hints);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    #endregion

    #region GetDeleteText

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    internal static string GetDeleteText(DeleteRequest request)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            commandText = statementBuilder.CreateDelete(request.Name,
                request.Where,
                request.Hints);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    #endregion

    #region GetDeleteAllText

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    internal static string GetDeleteAllText(DeleteAllRequest request)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            commandText = statementBuilder.CreateDeleteAll(request.Name,
                request.Hints);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    #endregion

    #region GetExistsText

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    internal static string GetExistsText(ExistsRequest request)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            commandText = statementBuilder.CreateExists(request.Name,
                request.Where,
                request.Hints);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    #endregion

    #region GetInsertText

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    internal static string GetInsertText(InsertRequest request)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var fields = GetTargetFields(request.Connection,
                request.Name,
                request.Fields,
                request.Transaction);
            var keyFields = GetKeyFields(request);
            commandText = GetInsertTextInternal(request, fields, keyFields);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal static async ValueTask<string> GetInsertTextAsync(InsertRequest request,
        CancellationToken cancellationToken = default)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var fields = await GetTargetFieldsAsync(request.Connection,
                request.Name,
                request.Fields,
                request.Transaction,
                cancellationToken).ConfigureAwait(false);
            var keyFields = await GetKeyFieldsAsync(request).ConfigureAwait(false);
            commandText = GetInsertTextInternal(request, fields, keyFields);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <param name="fields"></param>
    /// <param name="primaryField"></param>
    /// <param name="identityField"></param>
    /// <returns></returns>
    private static string GetInsertTextInternal(InsertRequest request,
        IEnumerable<Field> fields,
        IEnumerable<DbField> keyFields)
    {
        var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
        return statementBuilder.CreateInsert(request.Name,
            fields,
            keyFields,
            request.Hints);
    }

    #endregion

    #region GetInsertAllText

    internal static string GetInsertAllText(InsertAllRequest request)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var fields = GetTargetFields(request.Connection,
                request.Name,
                request.Fields,
                request.Transaction);
            var keyFields = GetKeyFields(request);
            commandText = GetInsertAllTextInternal(request, fields, keyFields);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    internal static async ValueTask<string> GetInsertAllTextAsync(InsertAllRequest request,
        CancellationToken cancellationToken = default)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var fields = await GetTargetFieldsAsync(request.Connection,
                request.Name,
                request.Fields,
                request.Transaction,
                cancellationToken).ConfigureAwait(false);
            var keyFields = await GetKeyFieldsAsync(request, cancellationToken).ConfigureAwait(false);
            commandText = GetInsertAllTextInternal(request, fields, keyFields);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    private static string GetInsertAllTextInternal(InsertAllRequest request,
        IEnumerable<Field> fields,
        IEnumerable<DbField> keyFields)
    {
        var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
        return statementBuilder.CreateInsertAll(request.Name,
            fields,
            request.BatchSize,
            keyFields,
            request.Hints);
    }

    #endregion

    #region GetMaxText

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    internal static string GetMaxText(MaxRequest request)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            commandText = statementBuilder.CreateMax(request.Name,
                request.Field,
                request.Where,
                request.Hints);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    #endregion

    #region GetMaxAllText

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    internal static string GetMaxAllText(MaxAllRequest request)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            commandText = statementBuilder.CreateMaxAll(request.Name,
                request.Field,
                request.Hints);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    #endregion

    #region GetMergeText

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    internal static string GetMergeText(MergeRequest request)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var fields = GetTargetFields(request.Connection,
                request.Name,
                request.Fields,
                request.Transaction);

            var noUpdateFields = request.NoUpdateFields is { } ?
                GetTargetFields(request.Connection, request.Name, request.NoUpdateFields, request.Transaction)
                : null;

            var keyFields = GetKeyFields(request);
            commandText = GetMergeTextInternal(request, fields, noUpdateFields, keyFields);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal static async ValueTask<string> GetMergeTextAsync(MergeRequest request,
        CancellationToken cancellationToken = default)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var fields = await GetTargetFieldsAsync(request.Connection,
                request.Name,
                request.Fields,
                request.Transaction,
                cancellationToken).ConfigureAwait(false);

            var noUpdateFields = request.Fields is { } ?
                await GetTargetFieldsAsync(request.Connection, request.Name, request.NoUpdateFields, request.Transaction).ConfigureAwait(false)
                : null;

            var keyFields = await GetKeyFieldsAsync(request, cancellationToken).ConfigureAwait(false);
            commandText = GetMergeTextInternal(request, fields, noUpdateFields, keyFields);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <param name="fields"></param>
    /// <param name="primaryField"></param>
    /// <param name="identityField"></param>
    /// <returns></returns>
    private static string GetMergeTextInternal(MergeRequest request,
        IEnumerable<Field> fields,
        IEnumerable<Field>? noUpdateFields,
        IEnumerable<DbField> keyFields)
    {
        var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
        return statementBuilder.CreateMerge(request.Name,
            fields,
            noUpdateFields,
            keyFields,
            request.Qualifiers,
            request.Hints);
    }

    #endregion

    #region GetMergeAllText

    /// <summary>
    /// Gets the cached command text for the 'MergeAll' operation.
    /// </summary>
    /// <param name="request">The request object.</param>
    /// <returns>The cached command text.</returns>
    internal static string GetMergeAllText(MergeAllRequest request)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var fields = GetTargetFields(request.Connection,
                request.Name,
                request.Fields,
                request.Transaction);
            var noUpdateFields = request.NoUpdateFields is { } ?
                GetTargetFields(request.Connection, request.Name, request.NoUpdateFields, request.Transaction)
                : null;

            var keyFields = GetKeyFields(request);
            commandText = GetMergeAllTextInternal(request, fields!, noUpdateFields, keyFields);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal static async ValueTask<string> GetMergeAllTextAsync(MergeAllRequest request,
        CancellationToken cancellationToken = default)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var fields = await GetTargetFieldsAsync(request.Connection,
                request.Name,
                request.Fields,
                request.Transaction,
                cancellationToken).ConfigureAwait(false);

            var noUpdateFields = request.NoUpdateFields is { } ? await GetTargetFieldsAsync(request.Connection,
                request.Name,
                request.Fields,
                request.Transaction,
                cancellationToken).ConfigureAwait(false) : null;

            var keyFields = await GetKeyFieldsAsync(request, cancellationToken).ConfigureAwait(false);
            commandText = GetMergeAllTextInternal(request, fields!, noUpdateFields, keyFields);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <param name="fields"></param>
    /// <param name="primaryField"></param>
    /// <param name="identityField"></param>
    /// <returns></returns>
    private static string GetMergeAllTextInternal(MergeAllRequest request,
        IEnumerable<Field> fields,
        IEnumerable<Field>? noUpdateFields,
        IEnumerable<DbField> keyFields)
    {
        var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
        return statementBuilder.CreateMergeAll(request.Name,
            fields,
            noUpdateFields,
            request.Qualifiers,
            request.BatchSize,
            keyFields,
            request.Hints);
    }

    #endregion

    #region GetMinText

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    internal static string GetMinText(MinRequest request)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            commandText = statementBuilder.CreateMin(request.Name,
                request.Field,
                request.Where,
                request.Hints);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    #endregion

    #region GetMinAllText

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    internal static string GetMinAllText(MinAllRequest request)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            commandText = statementBuilder.CreateMinAll(request.Name,
                request.Field,
                request.Hints);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    #endregion

    #region GetQueryText

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    internal static string GetQueryText(QueryRequest request)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var fields = GetTargetFields(request.Connection,
                request.Name,
                request.Fields,
                request.Transaction);
            ValidateOrderFields(request.Connection,
                request.Name,
                request.OrderBy,
                request.Transaction);
            commandText = GetQueryTextInternal(request, fields);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal static async ValueTask<string> GetQueryTextAsync(QueryRequest request,
        CancellationToken cancellationToken = default)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var fields = await GetTargetFieldsAsync(request.Connection,
                request.Name,
                request.Fields,
                request.Transaction,
                cancellationToken).ConfigureAwait(false);
            await ValidateOrderFieldsAsync(request.Connection,
                request.Name,
                request.OrderBy,
                request.Transaction,
                cancellationToken).ConfigureAwait(false);
            commandText = GetQueryTextInternal(request, fields);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    private static string GetQueryTextInternal(QueryRequest request,
        IEnumerable<Field> fields)
    {
        var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
        return statementBuilder.CreateQuery(request.Name,
            fields,
            request.Where,
            request.OrderBy,
            request.Top,
            request.Hints);
    }

    #endregion

    #region GetQueryAllText

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    internal static string GetQueryAllText(QueryAllRequest request)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var fields = GetTargetFields(request.Connection,
                request.Name,
                request.Fields,
                request.Transaction);
            ValidateOrderFields(request.Connection,
                request.Name,
                request.OrderBy,
                request.Transaction);
            commandText = GetQueryAllTextInternal(request, fields);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal static async ValueTask<string> GetQueryAllTextAsync(QueryAllRequest request,
        CancellationToken cancellationToken = default)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var fields = await GetTargetFieldsAsync(request.Connection,
                request.Name,
                request.Fields,
                request.Transaction,
                cancellationToken).ConfigureAwait(false);
            await ValidateOrderFieldsAsync(request.Connection,
                request.Name,
                request.OrderBy,
                request.Transaction,
                cancellationToken).ConfigureAwait(false);
            commandText = GetQueryAllTextInternal(request, fields);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    private static string GetQueryAllTextInternal(QueryAllRequest request,
        IEnumerable<Field> fields)
    {
        var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
        return statementBuilder.CreateQueryAll(request.Name,
            fields,
            request.OrderBy,
            request.Hints);
    }

    #endregion

    #region GetQueryMultipleText

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    internal static string GetQueryMultipleText(QueryMultipleRequest request)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var fields = GetTargetFields(request.Connection,
                request.Name,
                request.Fields,
                request.Transaction);
            ValidateOrderFields(request.Connection,
                request.Name,
                request.OrderBy,
                request.Transaction);
            commandText = GetQueryMultipleTextInternal(request, fields);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal static async ValueTask<string> GetQueryMultipleTextAsync(QueryMultipleRequest request,
        CancellationToken cancellationToken = default)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var fields = await GetTargetFieldsAsync(request.Connection,
                request.Name,
                request.Fields,
                request.Transaction,
                cancellationToken).ConfigureAwait(false);
            await ValidateOrderFieldsAsync(request.Connection,
                request.Name,
                request.OrderBy,
                request.Transaction,
                cancellationToken).ConfigureAwait(false);
            commandText = GetQueryMultipleTextInternal(request, fields);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    private static string GetQueryMultipleTextInternal(QueryMultipleRequest request,
        IEnumerable<Field> fields)
    {
        var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
        return statementBuilder.CreateQuery(request.Name,
            fields,
            request.Where,
            request.OrderBy,
            request.Top,
            request.Hints);
    }

    #endregion

    #region GetSkipQueryText

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    internal static string GetSkipQueryText(SkipQueryRequest request)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var fields = GetTargetFields(request.Connection,
                request.Name,
                request.Fields,
                request.Transaction);
            ValidateOrderFields(request.Connection,
                request.Name,
                request.OrderBy,
                request.Transaction);
            commandText = GetSkipQueryTextInternal(request, fields);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal static async ValueTask<string> GetSkipQueryTextAsync(SkipQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var fields = await GetTargetFieldsAsync(request.Connection,
                request.Name,
                request.Fields,
                request.Transaction,
                cancellationToken).ConfigureAwait(false);
            await ValidateOrderFieldsAsync(request.Connection,
                request.Name,
                request.OrderBy,
                request.Transaction,
                cancellationToken).ConfigureAwait(false);
            commandText = GetSkipQueryTextInternal(request, fields);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    internal static string GetSkipQueryTextInternal(SkipQueryRequest request,
        IEnumerable<Field> fields)
    {
        var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
        return statementBuilder.CreateSkipQuery(request.Name,
            fields,
            request.Skip,
            request.RowsPerBatch,
            request.OrderBy,
            request.Where,
            request.Hints);
    }

    #endregion


    #region GetSumText

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    internal static string GetSumText(SumRequest request)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            commandText = statementBuilder.CreateSum(request.Name,
                request.Field,
                request.Where,
                request.Hints);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    #endregion

    #region GetSumAllText

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    internal static string GetSumAllText(SumAllRequest request)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            commandText = statementBuilder.CreateSumAll(request.Name,
                request.Field,
                request.Hints);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    #endregion

    #region GetTruncateText

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    internal static string GetTruncateText(TruncateRequest request)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            commandText = statementBuilder.CreateTruncate(request.Name);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    #endregion

    #region GetUpdateText

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    internal static string GetUpdateText(UpdateRequest request)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var fields = GetTargetFields(request.Connection,
                request.Name,
                request.Fields,
                request.Transaction);
            var keyFields = GetKeyFields(request);
            commandText = GetUpdateTextInternal(request, fields, keyFields);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal static async ValueTask<string> GetUpdateTextAsync(UpdateRequest request,
        CancellationToken cancellationToken = default)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var fields = await GetTargetFieldsAsync(request.Connection,
                request.Name,
                request.Fields,
                request.Transaction,
                cancellationToken).ConfigureAwait(false);
            var keyFields = await GetKeyFieldsAsync(request, cancellationToken).ConfigureAwait(false);
            commandText = GetUpdateTextInternal(request, fields, keyFields);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <param name="fields"></param>
    /// <param name="primaryField"></param>
    /// <param name="identityField"></param>
    /// <returns></returns>
    private static string GetUpdateTextInternal(UpdateRequest request,
        IEnumerable<Field> fields,
        IEnumerable<DbField> keyFields)
    {
        var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
        return statementBuilder.CreateUpdate(request.Name,
            fields,
            request.Where,
            keyFields,
            request.Hints);
    }

    #endregion

    #region GetUpdateAllText

    internal static string GetUpdateAllText(UpdateAllRequest request)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var fields = GetTargetFields(request.Connection,
                request.Name,
                request.Fields,
                request.Transaction);
            var keyFields = GetKeyFields(request);
            commandText = GetUpdateAllTextInternal(request, fields, keyFields);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    internal static async ValueTask<string> GetUpdateAllTextAsync(UpdateAllRequest request,
        CancellationToken cancellationToken = default)
    {
        if (cache.TryGetValue(request, out var commandText) == false)
        {
            var fields = await GetTargetFieldsAsync(request.Connection,
                request.Name,
                request.Fields,
                request.Transaction,
                cancellationToken).ConfigureAwait(false);
            var keyFields = await GetKeyFieldsAsync(request, cancellationToken).ConfigureAwait(false);
            commandText = GetUpdateAllTextInternal(request, fields, keyFields);
            cache.TryAdd(request, commandText);
        }
        return commandText;
    }

    private static string GetUpdateAllTextInternal(UpdateAllRequest request,
        IEnumerable<Field> fields,
        IEnumerable<DbField> keyFields)
    {
        var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
        return statementBuilder.CreateUpdateAll(request.Name,
            fields,
            request.Qualifiers ?? keyFields.Where(x => x.IsPrimary).AsFields(),
            request.BatchSize,
            keyFields,
            request.Hints);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Flushes all the existing cached command texts.
    /// </summary>
    public static void Flush() =>
        cache.Clear();

    /// <summary>
    ///
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="tableName"></param>
    /// <param name="orderFields"></param>
    /// <param name="transaction"></param>
    private static void ValidateOrderFields(IDbConnection connection,
        string tableName,
        IEnumerable<OrderField>? orderFields,
        IDbTransaction? transaction)
    {
        if (orderFields?.Any() == true)
        {
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            ValidateOrderFieldsInternal(orderFields, dbFields, connection.GetDbSetting());
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="tableName"></param>
    /// <param name="orderFields"></param>
    /// <param name="transaction"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private static async ValueTask ValidateOrderFieldsAsync(IDbConnection connection,
        string tableName,
        IEnumerable<OrderField>? orderFields,
        IDbTransaction? transaction,
        CancellationToken cancellationToken = default)
    {
        if (orderFields?.Any() == true)
        {
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken).ConfigureAwait(false);
            ValidateOrderFieldsInternal(orderFields, dbFields, connection.GetDbSetting());
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="orderFields"></param>
    /// <param name="dbFields"></param>
    /// <param name="dbSetting"></param>
    private static void ValidateOrderFieldsInternal(IEnumerable<OrderField> orderFields,
        DbFieldCollection dbFields,
        IDbSetting dbSetting)
    {
        var unmatchesOrderFields = dbFields?.IsEmpty() == false ?
            orderFields
                .Where(of =>
                    dbFields.GetByName(of.Name.AsUnquoted(true, dbSetting)) == null) : null;
        if (unmatchesOrderFields?.Any() == true)
        {
            throw new MissingFieldsException($"The order fields '{unmatchesOrderFields.Select(of => of.Name).Join(", ")}' are not present from the actual table.");
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="tableName"></param>
    /// <param name="fields"></param>
    /// <param name="transaction"></param>
    /// <returns></returns>
    private static IEnumerable<Field> GetTargetFields(IDbConnection connection,
        string tableName,
        IEnumerable<Field>? fields,
        IDbTransaction? transaction)
    {
        if (fields?.Any() != true)
        {
            return [];
        }
        var dbFields = DbFieldCache.Get(connection, tableName, transaction);
        return GetTargetFieldsInternal(fields, dbFields, connection.GetDbSetting());
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="tableName"></param>
    /// <param name="fields"></param>
    /// <param name="transaction"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private static async ValueTask<IEnumerable<Field>> GetTargetFieldsAsync(IDbConnection connection,
        string tableName,
        IEnumerable<Field>? fields,
        IDbTransaction? transaction,
        CancellationToken cancellationToken = default)
    {
        if (fields?.Any() != true)
        {
            return [];
        }
        var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken).ConfigureAwait(false);
        return GetTargetFieldsInternal(fields, dbFields, connection.GetDbSetting());
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="fields"></param>
    /// <param name="dbFields"></param>
    /// <param name="dbSetting"></param>
    /// <returns></returns>
    private static IEnumerable<Field> GetTargetFieldsInternal(IEnumerable<Field> fields,
        DbFieldCollection dbFields,
        IDbSetting dbSetting)
    {
        return dbFields?.IsEmpty() == false ?
            fields
                .Where(f =>
                    dbFields.GetByName(f.Name.AsUnquoted(true, dbSetting)) != null) :
            fields;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    private static IEnumerable<DbField> GetKeyFields(BaseRequest request)
    {
        var dbFields = DbFieldCache.Get(request.Connection, request.Name, request.Transaction, true);
        return GetKeyFields(request, dbFields);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private static async ValueTask<IEnumerable<DbField>> GetKeyFieldsAsync(BaseRequest request,
        CancellationToken cancellationToken = default)
    {
        var dbFields = await DbFieldCache.GetAsync(request.Connection, request.Name, request.Transaction, true, cancellationToken).ConfigureAwait(false);
        return GetKeyFields(request, dbFields);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <param name="dbFields"></param>
    /// <returns></returns>
    private static IEnumerable<DbField> GetKeyFields(BaseRequest request,
        DbFieldCollection dbFields)
    {
        IEnumerable<ClassProperty>? primary;
        ClassProperty? identity;

        if (request.Type != null && request.Type.IsObjectType() == false)
        {
            primary = PrimaryCache.GetPrimaryKeys(request.Type);
            identity = IdentityCache.Get(request.Type);
        }
        else
        {
            primary = null;
            identity = null;
        }

        IEnumerable<DbField> list = dbFields;
        List<DbField>? dbFieldListUpdated = null;

        if (primary?.Any() == true)
        {
            foreach (var p in primary)
            {
                if (dbFields.GetByName(p.Name) is { } dbPrimary && !dbPrimary.IsPrimary)
                {
                    // Attribute-based primary key differs from the database primary key

                    list = dbFieldListUpdated ??= dbFields.ToList();

                    if (dbFieldListUpdated.IndexOf(dbPrimary) is { } ix && ix < 0)
                        continue;

                    dbFieldListUpdated[ix] = new DbField(
                        name: dbPrimary.Name,
                        isPrimary: true,
                        isIdentity: dbPrimary.IsIdentity || (identity is { } && identity.Name == p.Name),
                        dbPrimary.IsNullable,
                        dbPrimary.Type,
                        dbPrimary.Size,
                        dbPrimary.Precision,
                        dbPrimary.Scale,
                        dbPrimary.DatabaseType,
                        dbPrimary.HasDefaultValue,
                        dbPrimary.IsGenerated || (identity is { } && identity.Name == p.Name),
                        dbPrimary.Provider);
                }
            }
        }

        if (identity is { } && identity.Name is { } identityName)
        {
            if (dbFields.GetByName(identityName) is { } dbIdentity && !dbIdentity.IsIdentity
                && !list.Any(x => x.Name == identityName && x.IsIdentity))
            {
                // Attribute-based identity differs from what is in the database
                // *and* not already fixed by the primary key loop above

                list = dbFieldListUpdated ??= dbFields.ToList();
                if (dbFieldListUpdated.IndexOf(dbIdentity) is { } ix && ix < 0)
                    return list;

                dbFieldListUpdated[ix] = new DbField(
                    name: dbIdentity.Name,
                    isPrimary: dbIdentity.IsPrimary,
                    isIdentity: true,
                    dbIdentity.IsNullable,
                    dbIdentity.Type,
                    dbIdentity.Size,
                    dbIdentity.Precision,
                    dbIdentity.Scale,
                    dbIdentity.DatabaseType,
                    dbIdentity.HasDefaultValue,
                    isGenerated: true,
                    dbIdentity.Provider);
            }
        }

        dbFieldListUpdated = list.Where(x => x.IsPrimary || x.IsIdentity).ToList();

        if (dbFieldListUpdated.Count > 1
            && dbFields.GetKeyColumnReturn(GlobalConfiguration.Options.KeyColumnReturnBehavior) is { } returnField)
        {
            for (int i = 0; i < dbFieldListUpdated.Count; i++)
            {
                if (dbFieldListUpdated[i] is { } move && move.Name == returnField.Name)
                {
                    if (i > 0)
                    {

                        dbFieldListUpdated.RemoveAt(i);
                        dbFieldListUpdated.Insert(0, move);
                    }
                    break;
                }
            }
        }

        return dbFieldListUpdated;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="builder"></param>
    /// <returns></returns>
    private static IStatementBuilder EnsureStatementBuilder(IDbConnection connection,
        IStatementBuilder? builder) =>
        builder ?? connection.GetStatementBuilder();

    #endregion
}
