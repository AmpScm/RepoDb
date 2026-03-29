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

    internal static string GetCached<TRequest>(TRequest request, Func<TRequest, DbFieldCollection, string> creator)
        where TRequest : BaseRequest
    {
        return cache.GetOrAdd(request, (_) =>
        {
            DbFieldCollection dbFields = DbFieldCache.Get(request.Connection, request.TableName, request.Transaction, enableValidation: true);

            return creator(request, dbFields);
        });
    }

    internal static string GetCached<TRequest>(TRequest request, Func<TRequest, string> creator)
        where TRequest : BaseRequest
    {
        return cache.GetOrAdd(request, (_) =>
        {
            return creator(request);
        });
    }

    internal static async ValueTask<string> GetCachedAsync<TRequest>(TRequest request, Func<TRequest, DbFieldCollection, string> creator, CancellationToken cancellationToken)
        where TRequest : BaseRequest
    {
        if (cache.TryGetValue(request, out var v))
            return v;

        EnsureStatementBuilder(request.Connection, request.StatementBuilder);
        DbFieldCollection dbFields = await DbFieldCache.GetInternalAsync(request.Connection, request.TableName, request.Transaction, cancellationToken: cancellationToken).ConfigureAwait(false);

        return cache.GetOrAdd(request, (_) => creator(request, dbFields));
    }

    #region GetAverageText

    internal static string GetAverageText(AverageRequest request)
    {
        var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
        return statementBuilder.CreateAverage(request.TableName,
            request.Field,
            request.Where,
            request.Hints);
    }

    #endregion

    #region GetCountText

    internal static string GetCountText(CountRequest request)
    {
        var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
        return statementBuilder.CreateCount(request.TableName,
            request.Where,
            request.Hints);
    }

    #endregion

    #region GetDeleteText

    internal static string GetDeleteText(DeleteRequest request)
    {
        var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
        return statementBuilder.CreateDelete(request.TableName,
            request.Where,
            request.Hints);
    }

    #endregion

    #region GetExistsText

    internal static string GetExistsText(ExistsRequest request)
    {
        var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
        return statementBuilder.CreateExists(request.TableName,
            request.Where,
            request.Hints);
    }

    #endregion

    #region GetInsertText

    internal static string GetInsertText(InsertRequest request,
        DbFieldCollection dbFields)
    {
        var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
        var fields = GetTargetFields(request.Fields, dbFields);
        var keyFields = GetKeyFields(request, dbFields);

        return statementBuilder.CreateInsert(request.TableName,
            fields,
            keyFields,
            request.Hints);
    }

    #endregion

    #region GetInsertAllText

    internal static string GetInsertAllText(InsertAllRequest request,
        DbFieldCollection dbFields)
    {
        var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
        var fields = GetTargetFields(request.Fields, dbFields);
        var keyFields = GetKeyFields(request, dbFields);
        return statementBuilder.CreateInsertAll(request.TableName,
            fields,
            request.BatchSize,
            keyFields,
            request.Hints);
    }

    #endregion

    #region GetMaxText

    internal static string GetMaxText(MaxRequest request)
    {
        var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
        return statementBuilder.CreateMax(request.TableName,
            request.Field,
            request.Where,
            request.Hints);
    }

    #endregion

    #region GetMergeText

    internal static string GetMergeText(
        MergeRequest request,
        DbFieldCollection dbFields)
    {
        var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
        var fields = GetTargetFields(request.Fields, dbFields);

        var noUpdateFields = request.NoUpdateFields is { } ?
            GetTargetFields(request.NoUpdateFields, dbFields)
            : null;

        var keyFields = GetKeyFields(request, dbFields);
        return statementBuilder.CreateMerge(request.TableName,
            fields,
            noUpdateFields,
            keyFields,
            request.Qualifiers,
            request.Hints);
    }

    #endregion

    #region GetMergeAllText

    internal static string GetMergeAllText(MergeAllRequest request,
        DbFieldCollection dbFields)
    {
        var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
        var fields = GetTargetFields(request.Fields, dbFields);
        var noUpdateFields = request.NoUpdateFields is { } ?
            GetTargetFields(request.NoUpdateFields, dbFields)
            : null;

        var keyFields = GetKeyFields(request, dbFields);

        return statementBuilder.CreateMergeAll(request.TableName,
            fields,
            noUpdateFields,
            request.Qualifiers,
            request.BatchSize,
            keyFields,
            request.Hints);
    }

    #endregion

    #region GetMinText

    internal static string GetMinText(MinRequest request)
    {
        var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
        return statementBuilder.CreateMin(request.TableName,
            request.Field,
            request.Where,
            request.Hints);
    }

    #endregion

    #region GetQueryText

    internal static string GetQueryText(QueryRequest request,
        DbFieldCollection dbFields)
    {
        var fields = GetTargetFields(request.Fields, dbFields);
        ValidateOrderFields(request.OrderBy, dbFields);
        var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
        return statementBuilder.CreateQuery(request.TableName,
            fields,
            request.Where,
            request.OrderBy,
            request.Offset,
            request.Take,
            request.Hints);
    }

    #endregion

    #region GetQueryMultipleText

    internal static string GetQueryMultipleText(
        QueryMultipleRequest request,
        DbFieldCollection dbFields)
    {
        var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
        var fields = GetTargetFields(request.Fields, dbFields);
        ValidateOrderFields(request.OrderBy, dbFields);
        return statementBuilder.CreateQuery(request.TableName,
            fields,
            request.Where,
            request.OrderBy,
            0, request.Top,
            request.Hints);
    }

    #endregion

    #region GetSumText

    internal static string GetSumText(SumRequest request)
    {
        var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
        return statementBuilder.CreateSum(request.TableName,
            request.Field,
            request.Where,
            request.Hints);
    }

    #endregion

    #region GetTruncateText

    internal static string GetTruncateText(TruncateRequest request)
    {
        var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
        return statementBuilder.CreateTruncate(request.TableName);
    }

    #endregion

    #region GetUpdateText

    internal static string GetUpdateText(UpdateRequest request,
        DbFieldCollection dbFields)
    {
        var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
        var fields = GetTargetFields(request.Fields, dbFields);
        var keyFields = GetKeyFields(request, dbFields);

        return statementBuilder.CreateUpdate(request.TableName,
            fields,
            request.Where,
            keyFields,
            request.Hints);
    }

    #endregion

    #region GetUpdateAllText
    internal static string GetUpdateAllText(UpdateAllRequest request,
        DbFieldCollection dbFields)
    {
        var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);

        var fields = GetTargetFields(request.Fields, dbFields);
        var keyFields = GetKeyFields(request, dbFields);

        IEnumerable<Field> qualifiers = request.Qualifiers ?? keyFields.Where(x => x.IsPrimary || x.IsIdentity).AsFields();

        // Check the qualifiers
        if (qualifiers?.Any() == true)
        {
            // Check if the qualifiers are present in the given fields
            var unmatchesQualifiers = qualifiers.Where(field => !fields.ContainsFieldName(field.FieldName));
            if (unmatchesQualifiers.Any())
            {
                throw new InvalidQualifiersException($"The qualifiers '{unmatchesQualifiers.Select(field => field.FieldName).Join(", ")}' are not " +
                    $"present at the given fields '{fields.Select(field => field.FieldName).Join(", ")}'.");
            }
        }
        else
        {
            var primaryField = keyFields.FirstOrDefault(f => f.IsPrimary);

            if (primaryField is not null)
            {
                // Make sure that primary is present in the list of fields before qualifying to become a qualifier
                if (!fields.ContainsFieldName(primaryField.FieldName))
                {
                    throw new InvalidQualifiersException($"There are no qualifier field objects found for '{request.TableName}'. Ensure that the " +
                        $"primary field is present at the given fields '{fields.Select(field => field.FieldName).Join(", ")}'.");
                }

                // The primary is present, use it as a default if there are no qualifiers given
                qualifiers = keyFields;
            }
            else
            {
                // Throw exception, qualifiers are not defined
                throw new ArgumentException($"There are no qualifier field objects found for '{request.TableName}'.");
            }
        }

        return statementBuilder.CreateUpdateAll(request.TableName,
            fields,
            qualifiers,
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

    private static void ValidateOrderFields(IEnumerable<OrderField>? orderFields,
        DbFieldCollection dbFields)
    {
        var unmatchesOrderFields = !dbFields.IsEmpty() ?
            orderFields?.Where(of => dbFields.GetByFieldName(of.Name) == null) : null;
        if (unmatchesOrderFields?.Any() == true)
        {
            throw new MissingFieldsException($"The order fields '{unmatchesOrderFields.Select(of => of.Name).Join(", ")}' are not present from the actual table.");
        }
    }

    private static IEnumerable<Field> GetTargetFields(IEnumerable<Field>? fields,
        DbFieldCollection dbFields)
    {
        if (fields?.Any() != true)
            return [];

        return fields.Where(f => dbFields.GetByFieldName(f.FieldName) is { });
    }

    private static IEnumerable<DbField> GetKeyFields(BaseRequest request,
        DbFieldCollection dbFields)
    {
        IEnumerable<ClassProperty>? primary;
        ClassProperty? identity;

        if (request.Type != null && !request.Type.IsObjectType())
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
                if (dbFields.GetByFieldName(p.PropertyName) is { IsPrimary: false } dbPrimary)
                {
                    // Attribute-based primary key differs from the database primary key

                    list = dbFieldListUpdated ??= dbFields.ToList();

                    if (dbFieldListUpdated.IndexOf(dbPrimary) is { } ix && ix < 0)
                        continue;

                    dbFieldListUpdated[ix] = new DbField(
                        name: dbPrimary.FieldName,
                        isPrimary: true,
                        isIdentity: dbPrimary.IsIdentity || (identity is { } && identity.PropertyName == p.PropertyName),
                        dbPrimary.IsNullable,
                        dbPrimary.Type,
                        dbPrimary.Size,
                        dbPrimary.Precision,
                        dbPrimary.Scale,
                        dbPrimary.DatabaseType,
                        dbPrimary.HasDefaultValue,
                        dbPrimary.IsGenerated || (identity is { } && identity.PropertyName == p.PropertyName),
                        dbPrimary.Provider);
                }
            }
        }

        if (identity is { } && identity.PropertyName is { } identityName)
        {
            if (dbFields.GetByFieldName(identityName) is { } dbIdentity && !dbIdentity.IsIdentity
                && !list.Any(x => x.FieldName == identityName && x.IsIdentity))
            {
                // Attribute-based identity differs from what is in the database
                // *and* not already fixed by the primary key loop above

                list = dbFieldListUpdated ??= dbFields.ToList();
                if (dbFieldListUpdated.IndexOf(dbIdentity) is { } ix && ix < 0)
                    return list;

                dbFieldListUpdated[ix] = new DbField(
                    name: dbIdentity.FieldName,
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
                if (dbFieldListUpdated[i] is { } move && move.FieldName == returnField.FieldName)
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

    private static IStatementBuilder EnsureStatementBuilder(IDbConnection connection,
        IStatementBuilder? builder) =>
        builder ?? connection.GetStatementBuilder();

    #endregion
}
