using System.Data;
using RepoDb.Contexts.Cachers;
using RepoDb.Contexts.Execution;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Requests;

namespace RepoDb.Contexts.Providers;

/// <summary>
///
/// </summary>
internal static class InsertExecutionContextProvider
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="type"></param>
    /// <param name="tableName"></param>
    /// <param name="fields"></param>
    /// <param name="hints"></param>
    /// <returns></returns>
    private static string GetKey(Type type,
        string tableName,
        IEnumerable<Field> fields,
        string? hints)
    {
        return string.Concat(type.FullName,
            ";",
            tableName,
            ";",
            fields?.Select(f => f.Name).Join(","),
            ";",
            hints);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="connection"></param>
    /// <param name="tableName"></param>
    /// <param name="fields"></param>
    /// <param name="hints"></param>
    /// <param name="transaction"></param>
    /// <param name="statementBuilder"></param>
    /// <returns></returns>
    public static InsertExecutionContext Create(Type entityType,
        IDbConnection connection,
        string tableName,
        IEnumerable<Field> fields,
        string? hints = null,
        IDbTransaction? transaction = null,
        IStatementBuilder? statementBuilder = null)
    {
        var key = GetKey(entityType, tableName, fields, hints);

        // Get from cache
        var context = InsertExecutionContextCache.Get(key);
        if (context != null)
        {
            return context;
        }

        // Create
        var dbFields = DbFieldCache.Get(connection, tableName, transaction);

        if (dbFields.Any(x => x.IsReadOnly) == true)
        {
            fields = fields.Where(f => dbFields.GetByName(f.Name)?.IsReadOnly != true);
        }

        var request = new InsertRequest(entityType,
            tableName,
            connection,
            transaction,
            fields,
            hints,
            statementBuilder);
        var commandText = CommandTextCache.GetInsertText(request);

        // Call
        context = CreateInternal(entityType, connection,
            dbFields,
            tableName,
            fields,
            commandText);

        // Add to cache
        InsertExecutionContextCache.Add(entityType, key, context);

        // Return
        return context;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="connection"></param>
    /// <param name="tableName"></param>
    /// <param name="fields"></param>
    /// <param name="hints"></param>
    /// <param name="transaction"></param>
    /// <param name="statementBuilder"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<InsertExecutionContext> CreateAsync(Type entityType,
        IDbConnection connection,
        string tableName,
        IEnumerable<Field> fields,
        string? hints = null,
        IDbTransaction? transaction = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        var key = GetKey(entityType, tableName, fields, hints);

        // Get from cache
        var context = InsertExecutionContextCache.Get(key);
        if (context != null)
        {
            return context;
        }

        // Create
        var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken).ConfigureAwait(false);

        if (dbFields.Any(x => x.IsReadOnly) == true)
        {
            fields = fields.Where(f => dbFields.GetByName(f.Name)?.IsReadOnly != true);
        }

        var request = new InsertRequest(tableName,
            connection,
            transaction,
            fields,
            hints,
            statementBuilder);
        var commandText = await CommandTextCache.GetInsertTextAsync(request, cancellationToken).ConfigureAwait(false);

        // Call
        context = CreateInternal(entityType,
            connection,
            dbFields,
            tableName,
            fields,
            commandText);

        // Add to cache
        InsertExecutionContextCache.Add(entityType, key, context);

        // Return
        return context;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="connection"></param>
    /// <param name="dbFields"></param>
    /// <param name="tableName"></param>
    /// <param name="fields"></param>
    /// <param name="commandText"></param>
    /// <returns></returns>
    private static InsertExecutionContext CreateInternal(Type entityType,
        IDbConnection connection,
        DbFieldCollection dbFields,
        string tableName,
        IEnumerable<Field> fields,
        string commandText)
    {
        var dbSetting = connection.GetDbSetting();
        var dbHelper = connection.GetDbHelper();
        var inputFields = dbFields
            .Where(dbField =>
                dbField.IsIdentity == false)
            .Where(dbField =>
                fields.FirstOrDefault(field =>
                    string.Equals(field.Name.AsUnquoted(true, dbSetting), dbField.Name, StringComparison.OrdinalIgnoreCase)) != null)
            .AsList();

        // Variables for the entity action
        Action<object, object?>? keyPropertySetterFunc = null;
        var keyField = ExecutionContextProvider
            .GetTargetReturnColumnAsField(entityType, dbFields);

        // Get the key setter
        if (keyField != null)
        {
            keyPropertySetterFunc = FunctionCache
                .GetDataEntityPropertySetterCompiledFunction(entityType, keyField);
        }

        // Return the value
        return new InsertExecutionContext
        {
            CommandText = commandText,
            InputFields = inputFields,
            ParametersSetterFunc = FunctionCache
                .GetDataEntityDbParameterSetterCompiledFunction(entityType,
                    string.Concat(entityType.FullName, ".", tableName, ".Insert"),
                    inputFields?.AsList(),
                    null,
                    dbSetting,
                    dbHelper),
            IdentitySetterFunc = keyPropertySetterFunc
        };
    }
}
