using System.Data;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Requests;

namespace RepoDb.Contexts;

internal static class InsertExecutionContextProvider
{
    public static InsertExecutionContext Create(Type entityType,
        IDbConnection connection,
        string tableName,
        FieldSet fields,
        string? hints = null,
        IDbTransaction? transaction = null,
        IStatementBuilder? statementBuilder = null)
    {
        var key = (entityType, tableName, fields, hints);

        if (InsertExecutionContextCache.Get(key) is { } context)
        {
            return context;
        }

        // Create
        var dbFields = DbFieldCache.Get(connection, tableName, transaction);
        return InsertExecutionContextCache.GetOrAdd(key, (_) => CreateInternal(entityType, connection, tableName, fields, hints, transaction, statementBuilder, dbFields));
    }

    public static async Task<InsertExecutionContext> CreateAsync(Type entityType,
        IDbConnection connection,
        string tableName,
        FieldSet fields,
        string? hints = null,
        IDbTransaction? transaction = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        var key = (entityType, tableName, fields, hints);

        if (InsertExecutionContextCache.Get(key) is { } context)
        {
            return context;
        }

        // Create
        var dbFields = await DbFieldCache.GetInternalAsync(connection, tableName, transaction, cancellationToken: cancellationToken).ConfigureAwait(false);

        return InsertExecutionContextCache.GetOrAdd(key, (_) => CreateInternal(entityType, connection, tableName, fields, hints, transaction, statementBuilder, dbFields));
    }

    private static InsertExecutionContext CreateInternal(Type entityType, IDbConnection connection, string tableName, FieldSet fields, string? hints, IDbTransaction? transaction, IStatementBuilder? statementBuilder, DbFieldCollection dbFields)
    {
        if (dbFields.Any(x => x.IsReadOnly))
        {
            fields = fields.Where(f => dbFields.GetByFieldName(f.FieldName)?.IsReadOnly != true).AsFieldSet();
        }

        var request = new InsertRequest(entityType,
            tableName,
            connection,
            transaction,
            fields,
            hints,
            statementBuilder);
        var commandText = CommandTextCache.GetCachedWithDbFields(request, dbFields, CommandTextCache.GetInsertText);

        var dbSetting = connection.GetDbSetting();
        var dbHelper = connection.GetDbHelper();
        var inputFields = dbFields
            .Where(dbField => !dbField.IsIdentity)
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
        return new InsertExecutionContext
        {
            CommandText = commandText,
            ParametersSetterFunc = FunctionCache
                .GetDataEntityDbParameterSetterCompiledFunction(entityType,
                    string.Concat(entityType.FullName, ".", tableName, ".Insert"),
                    inputFields,
                    null,
                    dbSetting,
                    dbHelper),
            IdentitySetterFunc = keyPropertySetterFunc
        };
    }
}
