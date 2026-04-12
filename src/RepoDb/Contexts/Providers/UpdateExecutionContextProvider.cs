using System.Data;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Requests;

namespace RepoDb.Contexts;

internal static class UpdateExecutionContextProvider
{
    public static UpdateExecutionContext Create(Type entityType,
        IDbConnection connection,
        string tableName,
        QueryGroup? where,
        FieldSet fields,
        string? hints = null,
        IDbTransaction? transaction = null,
        IStatementBuilder? statementBuilder = null)
    {
        var key = (entityType, tableName, fields, hints, where);

        // Get from cache
        if (UpdateExecutionContextCache.Get(key) is { } context)
        {
            return context;
        }

        // Create
        var dbFields = DbFieldCache.Get(connection, tableName, transaction);

        // Return
        return UpdateExecutionContextCache.GetOrAdd(key, (_) => CreateInternal2(entityType, connection, tableName, where, fields, hints, transaction, statementBuilder, dbFields));
    }

    public static async Task<UpdateExecutionContext> CreateAsync(Type entityType,
        IDbConnection connection,
        string tableName,
        QueryGroup? where,
        FieldSet fields,
        string? hints = null,
        IDbTransaction? transaction = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
    {
        var key = (entityType, tableName, fields.AsFieldSet(), hints, where);

        // Get from cache
        var context = UpdateExecutionContextCache.Get(key);
        if (context is not null)
        {
            return context;
        }

        // Create
        var dbFields = await DbFieldCache.GetInternalAsync(connection, tableName, transaction, cancellationToken: cancellationToken).ConfigureAwait(false);

        return UpdateExecutionContextCache.GetOrAdd(key, (_) => CreateInternal2(entityType, connection, tableName, where, fields, hints, transaction, statementBuilder, dbFields));
    }

    private static UpdateExecutionContext CreateInternal2(Type entityType, IDbConnection connection, string tableName, QueryGroup? where, FieldSet fields, string? hints, IDbTransaction? transaction, IStatementBuilder? statementBuilder, DbFieldCollection dbFields)
    {
        if (dbFields.Any(x => x.IsReadOnly))
        {
            fields = fields.Where(f => dbFields.GetByFieldName(f.FieldName)?.IsReadOnly != true).AsFieldSet();
        }

        var request = new UpdateRequest(tableName,
            connection,
            transaction,
            where,
            fields,
            hints,
            statementBuilder);
        var commandText = CommandTextCache.GetCachedWithDbFields(request, dbFields, CommandTextCache.GetUpdateText);

        var dbSetting = connection.GetDbSetting();
        var dbHelper = connection.GetDbHelper();
        var inputFields = new List<DbField>();

        // Filter the actual properties for input fields
        inputFields = dbFields
            .Where(dbField => !dbField.IsReadOnly && !dbField.IsPrimary)
            .Where(dbField => fields.ContainsFieldName(dbField.FieldName))
            .AsList();

        // Return the value
        return new UpdateExecutionContext
        {
            CommandText = commandText,
            ParametersSetterFunc = FunctionCache.GetDataEntityDbParameterSetterCompiledFunction(entityType,
                string.Concat(entityType.FullName, ".", tableName, ".Update"),
                inputFields,
                null,
                dbSetting,
                dbHelper)
        };
    }
}
