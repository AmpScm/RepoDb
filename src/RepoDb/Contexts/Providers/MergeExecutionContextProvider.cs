using System.Data;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Requests;

namespace RepoDb.Contexts;

/// <summary>
///
/// </summary>
internal static class MergeExecutionContextProvider
{
    public static MergeExecutionContext Create(Type entityType,
        IDbConnection connection,
        string tableName,
        FieldSet qualifiers,
        FieldSet fields,
        FieldSet? noUpdateFields,
        string? hints = null,
        IDbTransaction? transaction = null, IStatementBuilder? statementBuilder = null)
    {
        var key = (entityType, tableName, qualifiers.AsFieldSet(), fields.AsFieldSet(), noUpdateFields?.AsFieldSet(), hints);

        // Get from cache
        if (MergeExecutionContextCache.Get(key) is { } context)
        {
            return context;
        }

        // Create
        var dbFields = DbFieldCache.Get(connection, tableName, transaction);

        return MergeExecutionContextCache.GetOrAdd(key, (_) => NewMethod(entityType, connection, tableName, qualifiers, fields, noUpdateFields, hints, transaction, statementBuilder, dbFields));
    }

    public static async Task<MergeExecutionContext> CreateAsync(Type entityType,
        IDbConnection connection,
        string tableName,
        FieldSet qualifiers,
        FieldSet fields,
        FieldSet? noUpdateFields,
        string? hints = null,
        IDbTransaction? transaction = null,
        IStatementBuilder? statementBuilder = null, CancellationToken cancellationToken = default)
    {
        var key = (entityType, tableName, qualifiers.AsFieldSet(), fields.AsFieldSet(), noUpdateFields?.AsFieldSet(), hints);

        // Get from cache
        var context = MergeExecutionContextCache.Get(key);
        if (context is not null)
        {
            return context;
        }

        // Create
        var dbFields = await DbFieldCache.GetInternalAsync(connection, tableName, transaction, cancellationToken: cancellationToken).ConfigureAwait(false);

        return MergeExecutionContextCache.GetOrAdd(key, (_) => NewMethod(entityType, connection, tableName, qualifiers, fields, noUpdateFields, hints, transaction, statementBuilder, dbFields));
    }

    private static MergeExecutionContext NewMethod(Type entityType, IDbConnection connection, string tableName, FieldSet qualifiers, FieldSet fields, FieldSet? noUpdateFields, string? hints, IDbTransaction? transaction, IStatementBuilder? statementBuilder, DbFieldCollection dbFields)
    {
        if (dbFields.Any(x => x.IsGenerated))
        {
            fields = fields.Where(f => dbFields.GetByFieldName(f.FieldName)?.IsGenerated != true).AsFieldSet();
        }

        var request = new MergeRequest(tableName,
            connection,
            transaction,
            fields,
            noUpdateFields,
            qualifiers,
            hints,
            statementBuilder);
        var commandText = CommandTextCache.GetCachedWithDbFields(request, dbFields, CommandTextCache.GetMergeText);

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
