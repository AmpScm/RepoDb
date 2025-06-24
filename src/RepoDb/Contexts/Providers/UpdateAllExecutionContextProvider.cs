using System.Data;
using System.Data.Common;
using System.Globalization;
using RepoDb.Contexts.Cachers;
using RepoDb.Contexts.Execution;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Requests;

namespace RepoDb.Contexts.Providers;

/// <summary>
///
/// </summary>
internal static class UpdateAllExecutionContextProvider
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="tableName"></param>
    /// <param name="qualifiers"></param>
    /// <param name="fields"></param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="hints"></param>
    /// <returns></returns>
    private static string GetKey(Type entityType,
        string tableName,
        IEnumerable<Field>? qualifiers,
        IEnumerable<Field> fields,
        int batchSize,
        string? hints)
    {
        return string.Concat(entityType.FullName,
            ";",
            tableName,
            ";",
            qualifiers?.Select(f => f.FieldName).Join(","),
            ";",
            fields?.Select(f => f.FieldName).Join(","),
            ";",
            batchSize.ToString(CultureInfo.InvariantCulture),
            ";",
            hints);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="connection"></param>
    /// <param name="tableName"></param>
    /// <param name="entities"></param>
    /// <param name="qualifiers"></param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields"></param>
    /// <param name="hints"></param>
    /// <param name="transaction"></param>
    /// <param name="statementBuilder"></param>
    /// <returns></returns>
    public static UpdateAllExecutionContext Create(Type entityType,
        IDbConnection connection,
        string tableName,
        IEnumerable<object> entities,
        IEnumerable<Field>? qualifiers,
        int batchSize,
        IEnumerable<Field> fields,
        string? hints = null,
        IDbTransaction? transaction = null,
        IStatementBuilder? statementBuilder = null)
    {
        var key = GetKey(entityType, tableName, qualifiers, fields, batchSize, hints);

        // Get from cache
        var context = UpdateAllExecutionContextCache.Get(key);
        if (context != null)
        {
            return context;
        }

        // Create
        var dbFields = DbFieldCache.Get(connection, tableName, transaction);

        if (dbFields.Any(x => x.IsGenerated) == true)
        {
            fields = fields.Where(f => dbFields.GetByFieldName(f.FieldName)?.IsGenerated != true);
        }

        var request = new UpdateAllRequest(tableName,
            connection,
            transaction,
            fields,
            qualifiers,
            batchSize,
            hints,
            statementBuilder);
        var commandText = CommandTextCache.GetUpdateAllText(request);

        // Call
        context = CreateInternal(entityType,
            connection,
            tableName,
            entities,
            dbFields,
            batchSize,
            fields,
            commandText);

        // Add to cache
        UpdateAllExecutionContextCache.Add(key, context);

        // Return
        return context;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="connection"></param>
    /// <param name="tableName"></param>
    /// <param name="entities"></param>
    /// <param name="qualifiers"></param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields"></param>
    /// <param name="hints"></param>
    /// <param name="transaction"></param>
    /// <param name="statementBuilder"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<UpdateAllExecutionContext> CreateAsync(Type entityType,
        IDbConnection connection,
        string tableName,
        IEnumerable<object> entities,
        IEnumerable<Field>? qualifiers,
        int batchSize,
        IEnumerable<Field> fields,
        string? hints = null,
        IDbTransaction? transaction = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        var key = GetKey(entityType, tableName, qualifiers, fields, batchSize, hints);

        // Get from cache
        var context = UpdateAllExecutionContextCache.Get(key);
        if (context != null)
        {
            return context;
        }

        // Create
        var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken).ConfigureAwait(false);

        if (dbFields.Any(x => x.IsGenerated) == true)
        {
            fields = fields.Where(f => dbFields.GetByFieldName(f.FieldName)?.IsGenerated != true);
        }

        var request = new UpdateAllRequest(tableName,
            connection,
            transaction,
            fields,
            qualifiers,
            batchSize,
            hints,
            statementBuilder);
        var commandText = await CommandTextCache.GetUpdateAllTextAsync(request, cancellationToken).ConfigureAwait(false);

        // Call
        context = CreateInternal(entityType,
            connection,
            tableName,
            entities,
            dbFields,
            batchSize,
            fields,
            commandText);

        // Add to cache
        UpdateAllExecutionContextCache.Add(key, context);

        // Return
        return context;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="connection"></param>
    /// <param name="tableName"></param>
    /// <param name="entities"></param>
    /// <param name="dbFields"></param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields"></param>
    /// <param name="commandText"></param>
    /// <returns></returns>
    private static UpdateAllExecutionContext CreateInternal(Type entityType,
        IDbConnection connection,
        string tableName,
        IEnumerable<object> entities,
        DbFieldCollection dbFields,
        int batchSize,
        IEnumerable<Field> fields,
        string commandText)
    {
        // Variables needed
        var dbSetting = connection.GetDbSetting();
        var dbHelper = connection.GetDbHelper();
        var inputFields = new List<DbField>();

        // Filter the actual properties for input fields
        inputFields = dbFields
            .Where(dbField => fields.GetByFieldName(dbField.FieldName) != null)
            .AsList();

        // Exclude the fields not on the actual entity
        if (TypeCache.Get(entityType).IsClassType() == false)
        {
            var entityFields = Field.Parse(entities?.FirstOrDefault());
            inputFields = inputFields
                .Where(field =>
                    entityFields.FirstOrDefault(f => string.Equals(f.FieldName.AsUnquoted(true, dbSetting), field.FieldName.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase)) != null)
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
            InputFields = inputFields,
            BatchSize = batchSize,
            SingleDataEntityParametersSetterFunc = singleEntityParametersSetterFunc,
            MultipleDataEntitiesParametersSetterFunc = multipleEntitiesParametersSetterFunc
        };
    }
}
