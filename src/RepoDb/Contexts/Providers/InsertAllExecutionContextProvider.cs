using System.Data;
using System.Data.Common;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Requests;

namespace RepoDb.Contexts;

/// <summary>
///
/// </summary>
internal static class InsertAllExecutionContextProvider
{
    public static InsertAllExecutionContext Create(Type entityType,
       IDbConnection connection,
       string tableName,
       int batchSize,
       FieldSet fields,
       string? hints = null,
       IDbTransaction? transaction = null,
       IStatementBuilder? statementBuilder = null)
    {
        var key = (entityType, tableName, fields, batchSize, hints);

        // Get from cache
        if (InsertAllExecutionContextCache.Get(key) is { } context)
        {
            return context;
        }

        // Create
        var dbFields = DbFieldCache.Get(connection, tableName, transaction);
        return InsertAllExecutionContextCache.GetOrAdd(key, (_) => CreateInternal(entityType, connection, tableName, batchSize, fields, hints, transaction, statementBuilder, dbFields));
    }

    public static async Task<InsertAllExecutionContext> CreateAsync(Type entityType,
        IDbConnection connection,
        string tableName,
        int batchSize,
        FieldSet fields,
        string? hints = null,
        IDbTransaction? transaction = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        var key = (entityType, tableName, fields, batchSize, hints);

        // Get from cache
        if (InsertAllExecutionContextCache.Get(key) is { } context)
        {
            return context;
        }

        // Create
        var dbFields = await DbFieldCache.GetInternalAsync(connection, tableName, transaction, cancellationToken: cancellationToken).ConfigureAwait(false);
        return InsertAllExecutionContextCache.GetOrAdd(key, (_) => CreateInternal(entityType, connection, tableName, batchSize, fields, hints, transaction, statementBuilder, dbFields));
    }

    private static InsertAllExecutionContext CreateInternal(Type entityType, IDbConnection connection, string tableName, int batchSize, FieldSet fields, string? hints, IDbTransaction? transaction, IStatementBuilder? statementBuilder, DbFieldCollection dbFields)
    {
        string commandText;

        if (dbFields.Any(x => x.IsReadOnly))
        {
            fields = fields.Where(f => dbFields.GetByFieldName(f.FieldName)?.IsReadOnly != true).AsFieldSet();
        }

        // Create a different kind of requests
        if (batchSize > 1)
        {
            var request = new InsertAllRequest(entityType,
                tableName,
                connection,
                transaction,
                fields,
                batchSize,
                hints,
                statementBuilder);
            commandText = CommandTextCache.GetCachedWithDbFields(request, dbFields, CommandTextCache.GetInsertAllText);
        }
        else
        {
            var request = new InsertRequest(entityType,
                tableName,
                connection,
                transaction,
                fields,
                hints,
                statementBuilder);
            commandText = CommandTextCache.GetCachedWithDbFields(request, dbFields, CommandTextCache.GetInsertText);
        }

        var dbSetting = connection.GetDbSetting();
        var dbHelper = connection.GetDbHelper();
        IEnumerable<DbField>? inputFields = null;

        // Filter the actual properties for input fields
        inputFields = dbFields.Where(dbField => fields.ContainsFieldName(dbField.FieldName))
            .AsList();

        // Variables for the context
        Action<object, object?>? keyPropertySetterFunc = null;

        if (dbFields.GetReturnColumn() is { } keyField)
        {
            keyPropertySetterFunc = FunctionCache
                .GetDataEntityPropertySetterCompiledFunction(entityType, keyField);
        }

        // Identity which objects to set
        Action<DbCommand, IList<object?>>? multipleEntitiesParametersSetterFunc = null;
        Action<DbCommand, object?>? singleEntityParametersSetterFunc = null;

        if (batchSize <= 1)
        {
            singleEntityParametersSetterFunc = FunctionCache
                .GetDataEntityDbParameterSetterCompiledFunction(entityType,
                    string.Concat(entityType.FullName, ".", tableName, ".InsertAll"),
                    inputFields,
                    null,
                    dbSetting,
                    dbHelper);
        }
        else
        {
            multipleEntitiesParametersSetterFunc = FunctionCache
                .GetDataEntityListDbParameterSetterCompiledFunction(entityType,
                    string.Concat(entityType.FullName, ".", tableName, ".InsertAll"),
                    inputFields,
                    null,
                    batchSize,
                    dbSetting,
                    dbHelper);
        }

        // Return the value
        return new InsertAllExecutionContext
        {
            CommandText = commandText,
            BatchSize = batchSize,
            SingleDataEntityParametersSetterFunc = singleEntityParametersSetterFunc,
            MultipleDataEntitiesParametersSetterFunc = multipleEntitiesParametersSetterFunc,
            IdentitySetterFunc = keyPropertySetterFunc
        };
    }
}
