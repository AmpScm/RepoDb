using System.Data;
using System.Data.Common;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Requests;

namespace RepoDb.Contexts;

/// <summary>
///
/// </summary>
internal static class UpdateAllExecutionContextProvider
{
    public static UpdateAllExecutionContext Create(Type entityType,
        IDbConnection connection,
        string tableName,
        IEnumerable<object> entities,
        FieldSet? qualifiers,
        int batchSize,
        FieldSet fields,
        string? hints = null,
        IDbTransaction? transaction = null,
        IStatementBuilder? statementBuilder = null)
    {
        var key = (entityType, tableName, qualifiers, fields, batchSize, hints);

        if (UpdateAllExecutionContextCache.Get(key) is { } context)
        {
            return context;
        }

        // Create
        var dbFields = DbFieldCache.Get(connection, tableName, transaction);

        return UpdateAllExecutionContextCache.GetOrAdd(key, (_) => CreateInternal(entityType, connection, tableName, entities, qualifiers, batchSize, fields, hints, transaction, statementBuilder, dbFields));
    }

    public static async Task<UpdateAllExecutionContext> CreateAsync(Type entityType,
        IDbConnection connection,
        string tableName,
        IEnumerable<object> entities,
        FieldSet? qualifiers,
        int batchSize,
        FieldSet fields,
        string? hints = null,
        IDbTransaction? transaction = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        var key = (entityType, tableName, qualifiers, fields, batchSize, hints);

        // Get from cache
        if (UpdateAllExecutionContextCache.Get(key) is { } context)
        {
            return context;
        }

        // Create
        var dbFields = await DbFieldCache.GetInternalAsync(connection, tableName, transaction, cancellationToken: cancellationToken).ConfigureAwait(false);

        return UpdateAllExecutionContextCache.GetOrAdd(key, (_) => CreateInternal(entityType, connection, tableName, entities, qualifiers, batchSize, fields, hints, transaction, statementBuilder, dbFields));
    }

    private static UpdateAllExecutionContext CreateInternal(Type entityType, IDbConnection connection, string tableName, IEnumerable<object> entities, FieldSet? qualifiers, int batchSize, FieldSet fields, string? hints, IDbTransaction? transaction, IStatementBuilder? statementBuilder, DbFieldCollection dbFields)
    {
        if (dbFields.Any(x => x.IsGenerated))
        {
            fields = fields.Where(f => dbFields.GetByFieldName(f.FieldName)?.IsGenerated != true).AsFieldSet();
        }

        var request = new UpdateAllRequest(tableName,
            connection,
            transaction,
            fields,
            qualifiers,
            batchSize,
            hints,
            statementBuilder);
        var commandText = CommandTextCache.GetCachedWithDbFields(request, dbFields, CommandTextCache.GetUpdateAllText);

        // Variables needed
        var dbSetting = connection.GetDbSetting();
        var dbHelper = connection.GetDbHelper();
        var inputFields = new List<DbField>();

        // Filter the actual properties for input fields
        inputFields = dbFields
            .Where(dbField => fields.ContainsFieldName(dbField.FieldName))
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
        Action<DbCommand, IList<object?>>? multipleEntitiesParametersSetterFunc = null;
        Action<DbCommand, object?>? singleEntityParametersSetterFunc = null;

        // Identity which objects to set
        if (batchSize <= 1)
        {
            singleEntityParametersSetterFunc = FunctionCache.GetDataEntityDbParameterSetterCompiledFunction(entityType,
                string.Concat(entityType.FullName, ".", tableName, ".UpdateAll"),
                inputFields,
                null,
                dbSetting,
                dbHelper);
        }
        else
        {
            multipleEntitiesParametersSetterFunc = FunctionCache.GetDataEntityListDbParameterSetterCompiledFunction(entityType,
                string.Concat(entityType.FullName, ".", tableName, ".UpdateAll"),
                inputFields,
                null,
                batchSize,
                dbSetting,
                dbHelper);
        }

        // Return the value
        return new UpdateAllExecutionContext
        {
            CommandText = commandText,
            BatchSize = batchSize,
            SingleDataEntityParametersSetterFunc = singleEntityParametersSetterFunc,
            MultipleDataEntitiesParametersSetterFunc = multipleEntitiesParametersSetterFunc
        };
    }
}
