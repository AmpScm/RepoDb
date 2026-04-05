using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb.StatementBuilders;

/// <summary>
/// A class that is being used to build a SQL Statement for Sqlite.
/// </summary>
#if !SQLITESYSTEM
public sealed class SqliteStatementBuilder : BaseStatementBuilder
#else
public sealed class SQLiteStatementBuilder : BaseStatementBuilder
#endif
{
#if !SQLITESYSTEM
    /// <summary>
    /// Creates a new instance of <see cref="SqliteStatementBuilder"/> class.
    /// </summary>
    /// <param name="dbSetting">The database settings object currently in used.</param>
    /// <param name="convertFieldResolver">The resolver used when converting a field in the database layer.</param>
    /// <param name="averageableClientTypeResolver">The resolver used to identity the type for average.</param>
    public SqliteStatementBuilder(IDbSetting dbSetting,
        IResolver<Field, IDbSetting, string?>? convertFieldResolver = null,
        IResolver<Type, Type?>? averageableClientTypeResolver = null)
        : base(dbSetting,
              convertFieldResolver,
              averageableClientTypeResolver)
    { }
#else
    /// <summary>
    /// Creates a new instance of <see cref="SQLiteStatementBuilder"/> class.
    /// </summary>
    /// <param name="dbSetting">The database settings object currently in used.</param>
    /// <param name="convertFieldResolver">The resolver used when converting a field in the database layer.</param>
    /// <param name="averageableClientTypeResolver">The resolver used to identity the type for average.</param>
    public SQLiteStatementBuilder(IDbSetting dbSetting,
        IResolver<Field, IDbSetting, string?>? convertFieldResolver = null,
        IResolver<Type, Type?>? averageableClientTypeResolver = null)
        : base(dbSetting,
              convertFieldResolver,
              averageableClientTypeResolver)
    { }
#endif

    #region CreateExists

    /// <summary>
    /// Creates a SQL Statement for exists operation.
    /// </summary>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <returns>A sql statement for exists operation.</returns>
    public override string CreateExists(string tableName,
        QueryGroup? where = null,
        string? hints = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);

        // Validate the hints
        GuardHints(hints);

        // Initialize the builder
        var builder = new QueryBuilder();

        // Build the query
        builder
            .Select()
            .WriteText("1 AS [ExistsValue]")
            .From()
            .TableNameFrom(tableName, DbSetting)
            .HintsFrom(hints)
            .WhereFrom(where, DbSetting)
            .Limit(1)
            .End(DbSetting);

        // Return the query
        return builder.ToString();
    }

    #endregion

    #region CreateInsert

    /// <inheritdoc />
    public override string CreateInsert(string tableName,
        IEnumerable<Field> fields,
        IEnumerable<DbField> keyFields,
        string? hints = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);
        GuardHints(hints);

        // Verify the fields
        if (fields?.Any() != true)
        {
            throw new EmptyException(nameof(fields), "The list of fields cannot be null or empty.");
        }

        // Primary Key
        foreach (var keyField in keyFields)
        {
            if (!keyField.IsPrimary || keyField.IsGenerated || keyField.IsIdentity || keyField.IsNullable)
                continue;

            if (fields.GetByFieldName(keyField.FieldName) is null)
            {
                throw new PrimaryFieldNotFoundException($"Primary field '{keyField.FieldName}' must be present in the field list.");
            }
        }

        // Insertable fields
        var insertableFields = fields
            .Where(f => keyFields.GetByFieldName(f.FieldName) is not { } x || !(x.IsGenerated || x.IsIdentity));

        // Initialize the builder
        var builder = new QueryBuilder();

        // Build the query
        builder
            .Insert()
            .Into()
            .TableNameFrom(tableName, DbSetting)
            .HintsFrom(hints)
            .OpenParen()
            .FieldsFrom(insertableFields, DbSetting)
            .CloseParen()
            .Values()
            .OpenParen()
            .ParametersFrom(insertableFields, 0, DbSetting)
            .CloseParen();

        if (keyFields?.Any() == true)
        {
            builder
                .Returning()
                .FieldsFrom(keyFields, DbSetting);
        }

        builder.End(DbSetting);
        return builder.ToString();
    }

    #endregion

    #region CreateInsertAll

    /// <inheritdoc />
    public override string CreateInsertAll(string tableName,
        IEnumerable<Field>? fields,
        int batchSize,
        IEnumerable<DbField> keyFields,
        string? hints = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);
        GuardHints(hints);
        var primaryField = keyFields.FirstOrDefault(f => f.IsPrimary);
        var identityField = keyFields.FirstOrDefault(f => f.IsIdentity);
        GuardPrimary(primaryField);
        GuardIdentity(identityField);

        // Validate the multiple statement execution
        ValidateMultipleStatementExecution(batchSize);

        // Verify the fields
        if (fields?.Any() != true)
        {
            throw new EmptyException(nameof(fields), "The list of fields cannot be null or empty.");
        }

        // Primary Key
        foreach (var keyField in keyFields)
        {
            if (!keyField.IsPrimary || keyField.IsGenerated || keyField.IsIdentity || keyField.IsNullable)
                continue;

            if (fields.GetByFieldName(keyField.FieldName) is null)
            {
                throw new PrimaryFieldNotFoundException($"Primary field '{keyField.FieldName}' must be present in the field list.");
            }
        }

        // Insertable fields
        var insertableFields = fields
            .Where(f => keyFields.GetByFieldName(f.FieldName) is not { } x || !(x.IsGenerated || x.IsIdentity));

        // Initialize the builder
        var builder = new QueryBuilder();

        // Compose
        builder
            .Insert()
            .Into()
            .TableNameFrom(tableName, DbSetting)
            .HintsFrom(hints)
            .OpenParen()
            .FieldsFrom(insertableFields, DbSetting)
            .CloseParen()
            .Values();

        // Iterate the indexes
        for (var index = 0; index < batchSize; index++)
        {
            if (index > 0)
                builder.Comma();

            builder
                .OpenParen()
                .ParametersFrom(insertableFields, index, DbSetting)
                .CloseParen();
        }

        if (keyFields?.Any() == true)
        {
            builder
                .Returning()
                .FieldsFrom(keyFields, DbSetting);
        }

        builder.End(DbSetting);

        // Return the query
        return builder
            .ToString();
    }

    #endregion

    #region CreateMerge

    /// <inheritdoc />
    public override string CreateMerge(string tableName,
        IEnumerable<Field> fields,
        IEnumerable<Field>? noUpdateFields,
        IEnumerable<DbField> keyFields,
        IEnumerable<Field> qualifiers, string? hints = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);
        GuardHints(hints);
        var primaryField = keyFields.FirstOrDefault(f => f.IsPrimary);
        var identityField = keyFields.FirstOrDefault(f => f.IsIdentity);
        GuardPrimary(primaryField);
        GuardIdentity(identityField);

        // Verify the fields
        if (fields?.Any() != true)
        {
            throw new EmptyException($"The list of fields cannot be null or empty.");
        }

        // Set the qualifiers
        if (qualifiers?.Any() != true && primaryField != null)
        {
            qualifiers = primaryField.AsField().AsEnumerable();
        }

        // Validate the qualifiers
        if (qualifiers?.Any() != true)
        {
            if (primaryField is null)
            {
                throw new PrimaryFieldNotFoundException($"The is no primary field from the table '{tableName}' that can be used as qualifier.");
            }
            else
            {
                throw new InvalidQualifiersException($"There are no defined qualifier fields.");
            }
        }

        // Initialize the builder
        var builder = new QueryBuilder();

        // Remove the qualifiers and identity field from the fields to update
        var updatableFields = EnumerableExtension.AsList(fields.Where(f => qualifiers.GetByFieldName(f.FieldName) is null && noUpdateFields?.GetByFieldName(f.FieldName) is null && keyFields.GetByFieldName(f.FieldName) is not { IsIdentity: true }));

        var insertableFields = fields;

        if (keyFields.FirstOrDefault(x => x.IsIdentity) is { } identity
            && qualifiers.GetByFieldName(identity.FieldName) is null)
        {
            insertableFields = fields.Where(f => f.FieldName != identity.FieldName);
        }

        // Build the query
        builder
            .Insert()
            .Into()
            .TableNameFrom(tableName, DbSetting)
            .OpenParen()
            .FieldsFrom(insertableFields, DbSetting)
            .CloseParen();

        // Continue
        builder
            .Values()
            .OpenParen()
            .ParametersFrom(insertableFields, 0, DbSetting)
            .CloseParen()
            .OnConflict(qualifiers, DbSetting);

        if (updatableFields.Count != 0)
        {
            builder
                .DoUpdate()
                .Set()
                .FieldsAndParametersFrom(updatableFields, 0, DbSetting);
        }
        else
        {
            builder
                .DoNothing();
        }

        if (keyFields.Any())
        {
            builder
                .Returning()
                .FieldsFrom(keyFields, DbSetting);
        }

        // End the builder
        builder.End(DbSetting);

        // Return the query
        return builder.ToString();
    }

    #endregion

    #region CreateMergeAll

    /// <inheritdoc />
    public override string CreateMergeAll(string tableName,
        IEnumerable<Field> fields,
        IEnumerable<Field>? noUpdateFields,
        IEnumerable<Field> qualifiers,
        int batchSize,
        IEnumerable<DbField> keyFields, string? hints = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);
        GuardHints(hints);

        // Verify the fields
        if (fields?.Any() != true)
        {
            throw new EmptyException($"The list of fields cannot be null or empty.");
        }

        // Set the qualifiers
        if (qualifiers?.Any() != true)
        {
            qualifiers = keyFields;
        }

        // Validate the qualifiers
        if (qualifiers?.Any() != true)
        {
            var primaryField = keyFields.FirstOrDefault(f => f.IsPrimary);
            var identityField = keyFields.FirstOrDefault(f => f.IsIdentity);

            if (primaryField is null)
            {
                throw new PrimaryFieldNotFoundException($"The is no primary field from the table '{tableName}' that can be used as qualifier.");
            }
            else
            {
                throw new InvalidQualifiersException($"There are no defined qualifier fields.");
            }
        }

        // Initialize the builder
        var builder = new QueryBuilder();

        // Remove the qualifiers from the fields
        var updatableFields = EnumerableExtension.AsList(fields.Where(f => qualifiers.GetByFieldName(f.FieldName) is null && noUpdateFields?.GetByFieldName(f.FieldName) is null && keyFields.GetByFieldName(f.FieldName) is not { IsIdentity: true })
);

        // Iterate the indexes
        for (var index = 0; index < batchSize; index++)
        {
            // Build the query
            builder
                .Insert()
                .Into()
                .TableNameFrom(tableName, DbSetting)
                .OpenParen()
                .FieldsFrom(fields, DbSetting)
                .CloseParen();

            // Continue
            builder
                .Values()
                .OpenParen()
                .ParametersFrom(fields, index, DbSetting)
                .CloseParen()
                .OnConflict(qualifiers, DbSetting)
                .DoUpdate()
                .Set()
                .FieldsAndParametersFrom(updatableFields, index, DbSetting);

            if (keyFields.Any())
            {
                builder
                    .Returning()
                    .FieldsFrom(keyFields, DbSetting);
            }

            // End the builder
            builder.End(DbSetting);
        }

        // Return the query
        return builder.ToString();
    }

    #endregion

    #region CreateQuery

    /// <summary>
    /// Creates a SQL Statement for query operation.
    /// </summary>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="fields">The list of fields.</param>
    /// <param name="where">The query expression.</param>
    /// <param name="orderBy">The list of fields for ordering.</param>
    /// <param name="offset"></param>
    /// <param name="take">The number of rows to be returned.</param>
    /// <returns>A sql statement for query operation.</returns>
    /// <param name="hints">The table hints to be used.</param>
    public override string CreateQuery(string tableName,
        IEnumerable<Field> fields,
        QueryGroup? where = null,
        IEnumerable<OrderField>? orderBy = null,
        int offset = 0,
        int take = 0,
        string? hints = null)
    {
        ArgumentNullException.ThrowIfNull(fields);
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);
        ArgumentOutOfRangeException.ThrowIfLessThan(offset, 0);
        ArgumentOutOfRangeException.ThrowIfLessThan(take, 0);

        // Validate the hints
        GuardHints(hints);

        // Initialize the builder
        var builder = new QueryBuilder();

        // Build the query
        builder
            .Select()
            .FieldsFrom(fields, DbSetting)
            .From()
            .TableNameFrom(tableName, DbSetting)
            .WhereFrom(where, DbSetting)
            .OrderByFrom(orderBy, DbSetting)
            .LimitTake(take, offset)
            .End(DbSetting);

        // Return the query
        return builder.ToString();
    }

    #endregion


    #region CreateTruncate

    /// <summary>
    /// Creates a SQL Statement for truncate operation.
    /// </summary>
    /// <param name="tableName">The name of the target table.</param>
    /// <returns>A sql statement for truncate operation.</returns>
    public override string CreateTruncate(string tableName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);

        // Initialize the builder
        var builder = new QueryBuilder();

        // Build the query
        builder
            .Delete()
            .From()
            .TableNameFrom(tableName, DbSetting)
            .End(DbSetting);

        // Return the query
        return builder.ToString();
    }

    #endregion

    /// <inheritdoc />
    public override string? JsonColumnType => base.JsonColumnType;
    /// <inheritdoc />
    public override string IdentityDefinition => "AUTOINCREMENT";
}
