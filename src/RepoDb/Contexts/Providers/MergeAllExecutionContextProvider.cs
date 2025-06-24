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
internal static class MergeAllExecutionContextProvider
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
        IEnumerable<Field>? noUpdateFields,
        int batchSize,
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
            batchSize.ToString(CultureInfo.InvariantCulture),
            ";",
            hints);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="connection"></param>
    /// <param name="entities"></param>
    /// <param name="tableName"></param>
    /// <param name="qualifiers"></param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields"></param>
    /// <param name="noUpdateFields"></param>
    /// <param name="hints"></param>
    /// <param name="transaction"></param>
    /// <returns></returns>
    /// <param name="statementBuilder"></param>
    public static MergeAllExecutionContext Create(Type entityType,
        IDbConnection connection,
        IEnumerable<object> entities,
        string tableName,
        IEnumerable<Field> qualifiers,
        int batchSize,
        IEnumerable<Field> fields,
        IEnumerable<Field>? noUpdateFields,
        string? hints = null,
        IDbTransaction? transaction = null, IStatementBuilder? statementBuilder = null)
    {
        var key = GetKey(entityType, tableName, qualifiers, fields, noUpdateFields, batchSize, hints);

        // Get from cache
        var context = MergeAllExecutionContextCache.Get(key);
        if (context != null)
        {
            return context;
        }

        // Create
        var dbFields = DbFieldCache
            .Get(connection, tableName, transaction);
        string commandText;

        if (dbFields.Any(x => x.IsGenerated) == true)
        {
            fields = fields.Where(f => dbFields.GetByFieldName(f.FieldName)?.IsGenerated != true);
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
            commandText = CommandTextCache.GetMergeAllText(request);
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
            commandText = CommandTextCache.GetMergeText(request);
        }

        // Call
        context = CreateInternal(entityType,
            connection,
            entities,
            dbFields,
            tableName,
            qualifiers,
            batchSize,
            fields,
            commandText);

        // Add to cache
        MergeAllExecutionContextCache.Add(key, context);

        // Return
        return context;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="connection"></param>
    /// <param name="entities"></param>
    /// <param name="tableName"></param>
    /// <param name="qualifiers"></param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields"></param>
    /// <param name="noUpdateFields"></param>
    /// <param name="hints"></param>
    /// <param name="transaction"></param>
    /// <param name="statementBuilder"></param>
    /// <returns></returns>
    /// <param name="cancellationToken"></param>
    public static async Task<MergeAllExecutionContext> CreateAsync(Type entityType,
        IDbConnection connection,
        IEnumerable<object> entities,
        string tableName,
        IEnumerable<Field> qualifiers,
        int batchSize,
        IEnumerable<Field> fields,
        IEnumerable<Field>? noUpdateFields,
        string? hints = null,
        IDbTransaction? transaction = null,
        IStatementBuilder? statementBuilder = null, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entityType, tableName, qualifiers, fields, noUpdateFields, batchSize, hints);

        // Get from cache
        var context = MergeAllExecutionContextCache.Get(key);
        if (context != null)
        {
            return context;
        }

        // Create
        var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken).ConfigureAwait(false);
        string commandText;

        // On Merge we do want to have the identity key in the fields
        if (dbFields.Any(x => x.IsGenerated) == true)
        {
            fields = fields.Where(f => dbFields.GetByFieldName(f.FieldName)?.IsGenerated != true);
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
            commandText = await CommandTextCache.GetMergeAllTextAsync(request, cancellationToken).ConfigureAwait(false);
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
            commandText = await CommandTextCache.GetMergeTextAsync(request, cancellationToken).ConfigureAwait(false);
        }

        // Call
        context = CreateInternal(entityType,
            connection,
            entities,
            dbFields,
            tableName,
            qualifiers,
            batchSize,
            fields,
            commandText);

        // Add to cache
        MergeAllExecutionContextCache.Add(key, context);

        // Return
        return context;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="connection"></param>
    /// <param name="entities"></param>
    /// <param name="dbFields"></param>
    /// <param name="tableName"></param>
    /// <param name="qualifiers"></param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields"></param>
    /// <param name="commandText"></param>
    /// <returns></returns>
    private static MergeAllExecutionContext CreateInternal(Type entityType,
        IDbConnection connection,
        IEnumerable<object> entities,
        DbFieldCollection dbFields,
        string tableName,
        IEnumerable<Field>? qualifiers,
        int batchSize,
        IEnumerable<Field> fields,
        string commandText)
    {
        var dbSetting = connection.GetDbSetting();
        var dbHelper = connection.GetDbHelper();
        IEnumerable<DbField> inputFields;

        // Check the fields
        if (fields?.Any() != true)
        {
            fields = dbFields.AsFields();
        }

        // Filter the actual properties for input fields
        inputFields = dbFields
            .Where(dbField =>
                fields.FirstOrDefault(field =>
                    string.Equals(field.FieldName.AsUnquoted(true, dbSetting), dbField.FieldName, StringComparison.OrdinalIgnoreCase)) != null)
            .AsList();

        // Exclude the fields not on the actual entity
        if (TypeCache.Get(entityType).IsClassType() == false)
        {
            var entityFields = Field.Parse(entities?.FirstOrDefault());
            inputFields = inputFields
                .Where(field =>
                    entityFields.FirstOrDefault(f =>
                        string.Equals(f.FieldName.AsUnquoted(true, dbSetting), field.FieldName.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase)) != null)
                .AsList();
        }

        // Variables for the context
        Action<object, object?>? keyPropertySetterFunc = null;
        var keyField = ExecutionContextProvider
            .GetTargetReturnColumnAsField(entityType, dbFields);

        if (keyField != null)
        {
            keyPropertySetterFunc = FunctionCache
                .GetDataEntityPropertySetterCompiledFunction(entityType, keyField);
        }

        // Check the qualifiers
        if (qualifiers?.Any() != true)
        {
            qualifiers = dbFields.GetPrimaryFields()?.AsFields() ?? keyField?.AsEnumerable();
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
            InputFields = inputFields,
            BatchSize = batchSize,
            SingleDataEntityParametersSetterFunc = singleEntityParametersSetterFunc,
            MultipleDataEntitiesParametersSetterFunc = multipleEntitiesParametersSetterFunc,
            KeyPropertySetterFunc = keyPropertySetterFunc
        };
    }
}
