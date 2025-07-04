using System.Data;
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
internal static class UpdateExecutionContextProvider
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="tableName"></param>
    /// <param name="fields"></param>
    /// <param name="hints"></param>
    /// <param name="where"></param>
    /// <returns></returns>
    private static string GetKey(Type entityType,
        string tableName,
        IEnumerable<Field> fields,
        string? hints,
        QueryGroup? where)
    {
        return string.Concat(entityType.FullName,
            ";",
            tableName,
            ";",
            fields?.Select(f => f.FieldName).Join(","),
            ";",
            hints,
            ";",
            where?.GetHashCode().ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="connection"></param>
    /// <param name="tableName"></param>
    /// <param name="where"></param>
    /// <param name="fields"></param>
    /// <param name="hints"></param>
    /// <param name="transaction"></param>
    /// <param name="statementBuilder"></param>
    /// <returns></returns>
    public static UpdateExecutionContext Create(Type entityType,
        IDbConnection connection,
        string tableName,
        QueryGroup? where,
        IEnumerable<Field> fields,
        string? hints = null,
        IDbTransaction? transaction = null,
        IStatementBuilder? statementBuilder = null)
    {
        var key = GetKey(entityType, tableName, fields, hints, where);

        // Get from cache
        var context = UpdateExecutionContextCache.Get(key);
        if (context != null)
        {
            return context;
        }

        // Create
        var dbFields = DbFieldCache.Get(connection, tableName, transaction);

        if (dbFields.Any(x => x.IsReadOnly) == true)
        {
            fields = fields.Where(f => dbFields.GetByFieldName(f.FieldName)?.IsReadOnly != true);
        }

        var request = new UpdateRequest(tableName,
            connection,
            transaction,
            where,
            fields,
            hints,
            statementBuilder);
        var commandText = CommandTextCache.GetUpdateText(request);

        // Call
        context = CreateInternal(entityType,
            connection,
            tableName,
            dbFields,
            fields,
            commandText);

        // Add to cache
        UpdateExecutionContextCache.Add(key, context);

        // Return
        return context;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="connection"></param>
    /// <param name="tableName"></param>
    /// <param name="where"></param>
    /// <param name="fields"></param>
    /// <param name="hints"></param>
    /// <param name="transaction"></param>
    /// <param name="statementBuilder"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<UpdateExecutionContext> CreateAsync(Type entityType,
        IDbConnection connection,
        string tableName,
        QueryGroup? where,
        IEnumerable<Field> fields,
        string? hints = null,
        IDbTransaction? transaction = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        var key = GetKey(entityType, tableName, fields, hints, where);

        // Get from cache
        var context = UpdateExecutionContextCache.Get(key);
        if (context != null)
        {
            return context;
        }

        // Create
        var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken).ConfigureAwait(false);

        if (dbFields.Any(x => x.IsReadOnly) == true)
        {
            fields = fields.Where(f => dbFields.GetByFieldName(f.FieldName)?.IsReadOnly != true);
        }

        var request = new UpdateRequest(tableName,
            connection,
            transaction,
            where,
            fields,
            hints,
            statementBuilder);
        var commandText = await CommandTextCache.GetUpdateTextAsync(request, cancellationToken).ConfigureAwait(false);

        // Call
        context = CreateInternal(entityType,
            connection,
            tableName,
            dbFields,
            fields,
            commandText);

        // Add to cache
        UpdateExecutionContextCache.Add(key, context);

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
    /// <param name="fields"></param>
    /// <param name="commandText"></param>
    /// <returns></returns>
    private static UpdateExecutionContext CreateInternal(Type entityType,
        IDbConnection connection,
        string tableName,
        DbFieldCollection dbFields,
        IEnumerable<Field> fields,
        string commandText)
    {
        var dbSetting = connection.GetDbSetting();
        var dbHelper = connection.GetDbHelper();
        var inputFields = new List<DbField>();

        // Filter the actual properties for input fields
        inputFields = dbFields
            .Where(dbField => dbField.IsIdentity == false)
            .Where(dbField =>
                fields.FirstOrDefault(field => string.Equals(field.FieldName.AsUnquoted(true, dbSetting), dbField.FieldName, StringComparison.OrdinalIgnoreCase)) != null)
            .AsList();

        // Return the value
        return new UpdateExecutionContext
        {
            CommandText = commandText,
            InputFields = inputFields,
            ParametersSetterFunc = FunctionCache.GetDataEntityDbParameterSetterCompiledFunction(entityType,
                string.Concat(entityType.FullName, ".", tableName, ".Update"),
                inputFields,
                null,
                dbSetting,
                dbHelper)
        };
    }
}
