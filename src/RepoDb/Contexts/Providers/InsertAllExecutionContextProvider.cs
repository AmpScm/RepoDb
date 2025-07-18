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
internal static class InsertAllExecutionContextProvider
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="tableName"></param>
    /// <param name="fields"></param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="hints"></param>
    /// <returns></returns>
    private static string GetKey(Type entityType,
        string tableName,
        IEnumerable<Field> fields,
        int batchSize,
        string? hints)
    {
        return string.Concat(entityType.FullName,
            ";",
            tableName,
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
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields"></param>
    /// <param name="hints"></param>
    /// <param name="transaction"></param>
    /// <param name="statementBuilder"></param>
    /// <returns></returns>
    public static InsertAllExecutionContext Create(Type entityType,
        IDbConnection connection,
        string tableName,
        int batchSize,
        IEnumerable<Field> fields,
        string? hints = null,
        IDbTransaction? transaction = null,
        IStatementBuilder? statementBuilder = null)
    {
        var key = GetKey(entityType, tableName, fields, batchSize, hints);

        // Get from cache
        var context = InsertAllExecutionContextCache.Get(key);
        if (context != null)
        {
            return context;
        }

        // Create
        var dbFields = DbFieldCache.Get(connection, tableName, transaction);
        string commandText;

        if (dbFields.Any(x => x.IsReadOnly))
        {
            fields = fields.Where(f => dbFields.GetByFieldName(f.FieldName)?.IsReadOnly != true);
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
            commandText = CommandTextCache.GetInsertAllText(request);
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
            commandText = CommandTextCache.GetInsertText(request);
        }

        // Call
        context = CreateInternal(entityType,
            connection,
            tableName,
            dbFields,
            batchSize,
            fields,
            commandText);

        // Add to cache
        InsertAllExecutionContextCache.Add(key, context);

        // Return
        return context;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="connection"></param>
    /// <param name="tableName"></param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields"></param>
    /// <param name="hints"></param>
    /// <param name="transaction"></param>
    /// <param name="statementBuilder"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<InsertAllExecutionContext> CreateAsync(Type entityType,
        IDbConnection connection,
        string tableName,
        int batchSize,
        IEnumerable<Field> fields,
        string? hints = null,
        IDbTransaction? transaction = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        var key = GetKey(entityType, tableName, fields, batchSize, hints);

        // Get from cache
        var context = InsertAllExecutionContextCache.Get(key);
        if (context != null)
        {
            return context;
        }

        // Create
        var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken).ConfigureAwait(false);
        string commandText;

        if (dbFields.Any(x => x.IsReadOnly))
        {
            fields = fields.Where(f => dbFields.GetByFieldName(f.FieldName)?.IsReadOnly != true);
        }

        // Create a different kind of requests
        if (batchSize > 1)
        {
            var request = new InsertAllRequest(tableName,
                connection,
                transaction,
                fields,
                batchSize,
                hints,
                statementBuilder);
            commandText = await CommandTextCache.GetInsertAllTextAsync(request, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            var request = new InsertRequest(tableName,
                connection,
                transaction,
                fields,
                hints,
                statementBuilder);
            commandText = await CommandTextCache.GetInsertTextAsync(request, cancellationToken).ConfigureAwait(false);
        }

        // Call
        context = CreateInternal(entityType,
            connection,
            tableName,
            dbFields,
            batchSize,
            fields,
            commandText);

        // Add to cache
        InsertAllExecutionContextCache.Add(key, context);

        // Return
        return context;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="connection"></param>
    /// <param name="tableName"></param>
    /// <param name="dbFields"></param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="fields"></param>
    /// <param name="commandText"></param>
    /// <returns></returns>
    private static InsertAllExecutionContext CreateInternal(Type entityType,
        IDbConnection connection,
        string tableName,
        DbFieldCollection dbFields,
        int batchSize,
        IEnumerable<Field> fields,
        string commandText)
    {
        var dbSetting = connection.GetDbSetting();
        var dbHelper = connection.GetDbHelper();
        IEnumerable<DbField>? inputFields = null;

        // Filter the actual properties for input fields
        inputFields = dbFields
            .Where(dbField => !dbField.IsIdentity)
            .Where(dbField =>
                fields.FirstOrDefault(field =>
                    string.Equals(field.FieldName.AsUnquoted(true, dbSetting), dbField.FieldName, StringComparison.OrdinalIgnoreCase)) != null)
            .AsList();

        // Variables for the context
        Action<object, object?>? keyPropertySetterFunc = null;
        var keyField = ExecutionContextProvider
            .GetTargetReturnColumnAsField(entityType, dbFields);
        if (keyField != null)
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
            InputFields = inputFields,
            BatchSize = batchSize,
            SingleDataEntityParametersSetterFunc = singleEntityParametersSetterFunc,
            MultipleDataEntitiesParametersSetterFunc = multipleEntitiesParametersSetterFunc,
            IdentitySetterFunc = keyPropertySetterFunc
        };
    }
}
