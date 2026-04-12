using System.Data;
using System.Data.Common;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Requests;

namespace RepoDb.Contexts;

internal static class MergeAllExecutionContextProvider
{
    public static MergeAllExecutionContext Create(Type entityType,
        IDbConnection connection,
        IEnumerable<object> entities,
        string tableName,
        FieldSet qualifiers,
        int batchSize,
        FieldSet fields,
        FieldSet? noUpdateFields,
        string? hints = null,
        IDbTransaction? transaction = null, IStatementBuilder? statementBuilder = null)
    {
        var key = (entityType, tableName, qualifiers.AsFieldSet(), fields.AsFieldSet(), noUpdateFields?.AsFieldSet(), batchSize, hints);

        // Get from cache
        if (MergeAllExecutionContextCache.Get(key) is { } context)
        {
            return context;
        }

        // Create
        var dbFields = DbFieldCache
            .Get(connection, tableName, transaction);

        return MergeAllExecutionContextCache.GetOrAdd(key, (_) => CreateInternal(entityType, connection, entities, tableName, qualifiers, batchSize, fields, noUpdateFields, hints, transaction, statementBuilder, dbFields));
    }

    public static async Task<MergeAllExecutionContext> CreateAsync(Type entityType,
        IDbConnection connection,
        IEnumerable<object> entities,
        string tableName,
        FieldSet qualifiers,
        int batchSize,
        FieldSet fields,
        FieldSet? noUpdateFields,
        string? hints = null,
        IDbTransaction? transaction = null,
        IStatementBuilder? statementBuilder = null, CancellationToken cancellationToken = default)
    {
        var key = (entityType, tableName, qualifiers.AsFieldSet(), fields.AsFieldSet(), noUpdateFields?.AsFieldSet(), batchSize, hints);

        if (MergeAllExecutionContextCache.Get(key) is { } context)
        {
            return context;
        }

        // Create
        var dbFields = await DbFieldCache.GetInternalAsync(connection, tableName, transaction, cancellationToken: cancellationToken).ConfigureAwait(false);

        context = CreateInternal(entityType, connection, entities, tableName, qualifiers, batchSize, fields, noUpdateFields, hints, transaction, statementBuilder, dbFields);

        return MergeAllExecutionContextCache.GetOrAdd(key, (_) => CreateInternal(entityType, connection, entities, tableName, qualifiers, batchSize, fields, noUpdateFields, hints, transaction, statementBuilder, dbFields));
    }

    private static MergeAllExecutionContext CreateInternal(Type entityType, IDbConnection connection, IEnumerable<object> entities, string tableName, IEnumerable<Field> qualifiers, int batchSize, FieldSet fields, IEnumerable<Field>? noUpdateFields, string? hints, IDbTransaction? transaction, IStatementBuilder? statementBuilder, DbFieldCollection dbFields)
    {
        string commandText;

        if (dbFields.Any(x => x.IsGenerated))
        {
            fields = fields.Where(f => dbFields.GetByFieldName(f.FieldName)?.IsGenerated != true).AsFieldSet();
        }

        // Create a different kind of requests
        if (batchSize > 1)
        {
            var request = new MergeAllRequest(tableName,
                connection,
                transaction,
                fields,
                noUpdateFields,
                qualifiers,
                batchSize,
                hints,
                statementBuilder);
            commandText = CommandTextCache.GetCachedWithDbFields(request, dbFields, CommandTextCache.GetMergeAllText);
        }
        else
        {
            var request = new MergeRequest(tableName,
                connection,
                transaction,
                fields,
                noUpdateFields,
                qualifiers,
                hints,
                statementBuilder);
            commandText = CommandTextCache.GetCachedWithDbFields(request, dbFields, CommandTextCache.GetMergeText);
        }

        var dbSetting = connection.GetDbSetting();
        var dbHelper = connection.GetDbHelper();
        IEnumerable<DbField> inputFields;

        // Check the fields
        if (fields.Count == 0)
        {
            fields = dbFields.AsFields();
        }

        if (qualifiers?.Any() != true)
            qualifiers = dbFields.Where(x => x.IsPrimary || x.IsIdentity);

        // Filter the actual properties for input fields
        inputFields = dbFields
            .Where(dbField =>
                fields.GetByFieldName(dbField.FieldName) is { } && (!dbField.IsIdentity || qualifiers.GetByFieldName(dbField.FieldName) is { }))
            .AsList();

        // Exclude the fields not on the actual entity
        if (!TypeCache.Get(entityType).IsClassType)
        {
            var entityFields = Field.Parse(entities?.FirstOrDefault());
            inputFields = inputFields
                .Where(field => entityFields.ContainsFieldName(field.FieldName))
                .AsList();
        }

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
                    string.Concat(entityType.FullName, ".", tableName, ".MergeAll"),
                    inputFields,
                    null,
                    dbSetting,
                    dbHelper);
        }
        else
        {
            multipleEntitiesParametersSetterFunc = FunctionCache
                .GetDataEntityListDbParameterSetterCompiledFunction(entityType,
                    string.Concat(entityType.FullName, ".", tableName, ".MergeAll"),
                    inputFields,
                    null,
                    batchSize,
                    dbSetting,
                    dbHelper);
        }

        // Return the value
        return new MergeAllExecutionContext
        {
            CommandText = commandText,
            BatchSize = batchSize,
            SingleDataEntityParametersSetterFunc = singleEntityParametersSetterFunc,
            MultipleDataEntitiesParametersSetterFunc = multipleEntitiesParametersSetterFunc,
            KeyPropertySetterFunc = keyPropertySetterFunc
        };
    }
}
