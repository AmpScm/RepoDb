using System.Data;
using RepoDb.Contexts.Caches;
using RepoDb.Contexts.Execution;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Requests;

namespace RepoDb.Contexts.Providers;

/// <summary>
///
/// </summary>
internal static class MergeExecutionContextProvider
{
    private static string GetKey(Type entityType,
        string tableName,
        IEnumerable<Field> qualifiers,
        IEnumerable<Field> fields,
        IEnumerable<Field>? noUpdateFields,
        string? hints)
    {
        return string.Concat(entityType.FullName,
            ";",
            tableName,
            ";",
            qualifiers?.Select(f => f.FieldName).Join(","),
            ";",
            fields.Select(f => f.FieldName).Join(","),
            ";",
            noUpdateFields?.Select(f => f.FieldName).Join(","),
            ";",
            hints);
    }

    public static MergeExecutionContext Create(Type entityType,
        IDbConnection connection,
        string tableName,
        IEnumerable<Field> qualifiers,
        IEnumerable<Field> fields,
        IEnumerable<Field>? noUpdateFields,
        string? hints = null,
        IDbTransaction? transaction = null, IStatementBuilder? statementBuilder = null)
    {
        var key = GetKey(entityType, tableName, qualifiers, fields, noUpdateFields, hints);

        // Get from cache
        var context = MergeExecutionContextCache.Get(key);
        if (context is not null)
        {
            return context;
        }

        // Create
        var dbFields = DbFieldCache.Get(connection, tableName, transaction);

        if (dbFields.Any(x => x.IsGenerated))
        {
            fields = fields.Where(f => dbFields.GetByFieldName(f.FieldName)?.IsGenerated != true);
        }

        var request = new MergeRequest(tableName,
            connection,
            transaction,
            fields,
            noUpdateFields,
            qualifiers,
            hints,
            statementBuilder);
        var commandText = CommandTextCache.GetCached(request, CommandTextCache.GetMergeText);

        // Call
        context = CreateInternal(entityType,
            connection,
            dbFields,
            tableName,
            fields,
            commandText);

        // Add to cache
        MergeExecutionContextCache.Add(key, context);

        // Return
        return context;
    }

    public static async Task<MergeExecutionContext> CreateAsync(Type entityType,
        IDbConnection connection,
        string tableName,
        IEnumerable<Field> qualifiers,
        IEnumerable<Field> fields,
        IEnumerable<Field>? noUpdateFields,
        string? hints = null,
        IDbTransaction? transaction = null,
        IStatementBuilder? statementBuilder = null, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entityType, tableName, qualifiers, fields, noUpdateFields, hints);

        // Get from cache
        var context = MergeExecutionContextCache.Get(key);
        if (context is not null)
        {
            return context;
        }

        // Create
        var dbFields = await DbFieldCache.GetInternalAsync(connection, tableName, transaction, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (dbFields.Any(x => x.IsGenerated))
        {
            fields = fields.Where(f => dbFields.GetByFieldName(f.FieldName)?.IsGenerated != true);
        }

        var request = new MergeRequest(tableName,
            connection,
            transaction,
            fields,
            noUpdateFields,
            qualifiers,
            hints,
            statementBuilder);
        var commandText = await CommandTextCache.GetCachedAsync(request, CommandTextCache.GetMergeText, cancellationToken).ConfigureAwait(false);

        // Call
        context = CreateInternal(entityType,
            connection,
            dbFields,
            tableName,
            fields,
            commandText);

        // Add to cache
        MergeExecutionContextCache.Add(key, context);

        // Return
        return context;
    }

    private static MergeExecutionContext CreateInternal(Type entityType,
        IDbConnection connection,
        DbFieldCollection dbFields,
        string tableName,
        IEnumerable<Field> fields,
        string commandText)
    {
        var dbSetting = connection.GetDbSetting();
        var dbHelper = connection.GetDbHelper();
        var inputFields = dbFields
            .Where(dbField => fields.ContainsFieldName(dbField.FieldName))
            .AsList();

        // Variables for the entity action
        Action<object, object?>? keyPropertySetterFunc = null;

        if (dbFields.GetReturnColumn() is { } keyField)
        {
            keyPropertySetterFunc = FunctionCache
                .GetDataEntityPropertySetterCompiledFunction(entityType, keyField);
        }

        // Return the value
        return new MergeExecutionContext
        {
            CommandText = commandText,
            InputFields = inputFields,
            ParametersSetterFunc = FunctionCache
                .GetDataEntityDbParameterSetterCompiledFunction(entityType,
                    string.Concat(entityType.FullName, ".", tableName, ".Merge"),
                    inputFields,
                    null,
                    dbSetting,
                    dbHelper),
            KeyPropertySetterFunc = keyPropertySetterFunc
        };
    }
}
