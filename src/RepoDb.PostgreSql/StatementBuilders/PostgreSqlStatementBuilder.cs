using System.Data;
using Npgsql;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Resolvers;

namespace RepoDb.StatementBuilders;

/// <summary>
/// A class used to build a SQL Statement for PostgreSql.
/// </summary>
public sealed class PostgreSqlStatementBuilder : BaseStatementBuilder
{
    /// <summary>
    /// Creates a new instance of <see cref="PostgreSqlStatementBuilder"/> object.
    /// </summary>
    public PostgreSqlStatementBuilder()
        : this(DbSettingMapper.Get<NpgsqlConnection>(),
              new PostgreSqlConvertFieldResolver(),
              new ClientTypeToAverageableClientTypeResolver())
    { }

    /// <summary>
    /// Creates a new instance of <see cref="PostgreSqlStatementBuilder"/> class.
    /// </summary>
    /// <param name="dbSetting">The database settings object currently in used.</param>
    /// <param name="convertFieldResolver">The resolver used when converting a field in the database layer.</param>
    /// <param name="averageableClientTypeResolver">The resolver used to identity the type for average.</param>
    public PostgreSqlStatementBuilder(IDbSetting dbSetting,
        IResolver<Field, IDbSetting, string>? convertFieldResolver = null,
        IResolver<Type, Type> averageableClientTypeResolver = null)
        : base(dbSetting,
              convertFieldResolver,
              averageableClientTypeResolver)
    { }

    #region CreateBatchQuery

    /// <summary>
    /// Creates a SQL Statement for batch query operation.
    /// </summary>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="fields">The list of fields to be queried.</param>
    /// <param name="page">The page of the batch.</param>
    /// <param name="rowsPerBatch">The number of rows per batch.</param>
    /// <param name="orderBy">The list of fields for ordering.</param>
    /// <param name="where">The query expression.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <returns>A sql statement for batch query operation.</returns>
    public override string CreateBatchQuery(string tableName,
        IEnumerable<Field> fields,
        int page,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy = null,
        QueryGroup? where = null,
        string? hints = null)
    {
        // Ensure with guards
        GuardTableName(tableName);

        // Validate the hints
        GuardHints(hints);

        // There should be fields
        if (fields?.Any() != true)
        {
            throw new EmptyException($"The list of queryable fields must not be null for '{tableName}'.");
        }

        // Validate order by
        if (orderBy == null || orderBy.Any() != true)
        {
            throw new EmptyException("The argument 'orderBy' is required.");
        }

        // Validate the page
        if (page < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(page), "The page must be equals or greater than 0.");
        }

        // Validate the page
        if (rowsPerBatch < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(rowsPerBatch), "The rows per batch must be equals or greater than 1.");
        }

        // Skipping variables
        var skip = (page * rowsPerBatch);

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
            .LimitOffset(rowsPerBatch, skip)
            .End();

        // Return the query
        return builder.ToString();
    }

    #endregion

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
        // Ensure with guards
        GuardTableName(tableName);

        // Validate the hints
        GuardHints(hints);

        // Initialize the builder
        var builder = new QueryBuilder();

        // Build the query
        builder
            .Select()
            .WriteText("1 AS \"ExistsValue\"")
            .From()
            .TableNameFrom(tableName, DbSetting)
            .HintsFrom(hints)
            .WhereFrom(where, DbSetting)
            .Limit(1)
            .End();

        // Return the query
        return builder.ToString();
    }

    #endregion

    #region CreateInsert

    /// <summary>
    /// Creates a SQL Statement for insert operation.
    /// </summary>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="fields">The list of fields to be inserted.</param>
    /// <param name="primaryField">The primary field from the database.</param>
    /// <param name="identityField">The identity field from the database.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <returns>A sql statement for insert operation.</returns>
    public override string CreateInsert(string tableName,
        IEnumerable<Field> fields,
        IEnumerable<DbField> keyFields,
        string? hints = null)
    {
        // Ensure with guards
        GuardTableName(tableName);
        GuardHints(hints);

        // Verify the fields
        if (fields?.Any() != true)
        {
            throw new EmptyException(nameof(fields), $"The list of insertable fields must not be null or empty for '{tableName}'.");
        }

        foreach (var keyField in keyFields)
        {
            if (!keyField.IsPrimary || keyField.IsGenerated || keyField.IsIdentity || keyField.IsNullable)
                continue;

            if (fields.GetByName(keyField.Name) is null)
            {
                throw new PrimaryFieldNotFoundException($"Primary field '{keyField.Name}' must be present in the field list.");
            }
        }

        // Insertable fields
        var insertableFields = fields
            .Where(f => keyFields.GetByName(f.Name) is not { } x || !(x.IsGenerated || x.IsIdentity));

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

        if (keyFields.Any())
        {
            builder
                .Returning()
                .FieldsFrom(keyFields.AsFields(), DbSetting);
        }

        builder
            .End(DbSetting);

        return builder.ToString();
    }

    #endregion

    #region CreateInsertAll

    /// <summary>
    /// Creates a SQL Statement for insert-all operation.
    /// </summary>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="fields">The list of fields to be inserted.</param>
    /// <param name="batchSize">The batch size of the operation.</param>
    /// <param name="primaryField">The primary field from the database.</param>
    /// <param name="identityField">The identity field from the database.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <returns>A sql statement for insert operation.</returns>
    public override string CreateInsertAll(string tableName,
        IEnumerable<Field> fields,
        int batchSize,
        IEnumerable<DbField> keyFields,
        string? hints = null)
    {
        // Ensure with guards
        GuardTableName(tableName);
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
            throw new EmptyException("The list of fields cannot be null or empty.");
        }

        // Primary Key
        if (primaryField != null &&
            primaryField.HasDefaultValue == false &&
            !string.Equals(primaryField.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase))
        {
            var isPresent = fields
                .FirstOrDefault(f =>
                    string.Equals(f.Name, primaryField.Name, StringComparison.OrdinalIgnoreCase)) != null;

            if (isPresent == false)
            {
                throw new PrimaryFieldNotFoundException($"As the primary field '{primaryField.Name}' is not an identity nor has a default value, it must be present on the insert operation.");
            }
        }

        // Insertable fields
        var insertableFields = fields
            .Where(f =>
                !string.Equals(f.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase));

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

        if (keyFields.Any())
        {
            builder
                .Returning()
                .FieldsFrom(keyFields.AsFields(), DbSetting);
        }

        // Return the query
        return builder
            .End()
            .ToString();
    }

    #endregion

    #region CreateMerge

    /// <summary>
    /// Creates a SQL Statement for merge operation.
    /// </summary>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="fields">The list of fields to be merged.</param>
    /// <param name="qualifiers">The list of the qualifier <see cref="Field"/> objects.</param>
    /// <param name="primaryField">The primary field from the database.</param>
    /// <param name="identityField">The identity field from the database.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <returns>A sql statement for merge operation.</returns>
    public override string CreateMerge(string tableName,
        IEnumerable<Field> fields,
        IEnumerable<Field> noUpdateFields,
        IEnumerable<DbField> keyFields,
        IEnumerable<Field>? qualifiers,
        string? hints = null)
    {
        // Ensure with guards
        GuardTableName(tableName);
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
            if (primaryField == null)
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
        var updatableFields = fields.Where(f => qualifiers.GetByName(f.Name) is null && noUpdateFields?.GetByName(f.Name) is null && keyFields.GetByName(f.Name) is not { IsIdentity: true })
            .AsList();

        // Build the query
        builder
            .Insert()
            .Into()
            .TableNameFrom(tableName, DbSetting)
            .OpenParen()
            .FieldsFrom(fields, DbSetting)
            .CloseParen();

        // Override the system value
        if (identityField != null)
        {
            builder.WriteText("OVERRIDING SYSTEM VALUE");
        }

        // Continue
        builder
            .Values()
            .OpenParen()
            .ParametersFrom(fields, 0, DbSetting)
            .CloseParen()
            .OnConflict(qualifiers, DbSetting)
            .DoUpdate()
            .Set()
            .FieldsAndParametersFrom(updatableFields, 0, DbSetting);

        if (keyFields.Any())
        {
            builder
                .Returning()
                .FieldsFrom(keyFields.AsFields(), DbSetting);
        }

        // End the builder
        builder.End();

        // Return the query
        return builder.ToString();
    }

    #endregion

    #region CreateMergeAll

    /// <summary>
    /// Creates a SQL Statement for merge-all operation.
    /// </summary>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="fields">The list of fields to be merged.</param>
    /// <param name="qualifiers">The list of the qualifier <see cref="Field"/> objects.</param>
    /// <param name="batchSize">The batch size of the operation.</param>
    /// <param name="primaryField">The primary field from the database.</param>
    /// <param name="identityField">The identity field from the database.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <returns>A sql statement for merge operation.</returns>
    public override string CreateMergeAll(string tableName,
        IEnumerable<Field> fields,
        IEnumerable<Field>? noUpdateFields,
        IEnumerable<Field> qualifiers,
        int batchSize,
        IEnumerable<DbField> keyFields, string? hints = null)
    {
        // Ensure with guards
        GuardTableName(tableName);
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
            if (primaryField == null)
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

        var updatableFields = fields.Where(f => qualifiers.GetByName(f.Name) is null && noUpdateFields?.GetByName(f.Name) is null && keyFields.GetByName(f.Name) is not { IsIdentity: true })
            .AsList();

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

            // Override the system value
            if (identityField != null)
            {
                builder.WriteText("OVERRIDING SYSTEM VALUE");
            }

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
                    .FieldsFrom(keyFields.AsFields(), DbSetting);
            }

            // End the builder
            builder.End();
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
    /// <param name="top">The number of rows to be returned.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <returns>A sql statement for query operation.</returns>
    public override string CreateQuery(string tableName,
        IEnumerable<Field> fields,
        QueryGroup? where = null,
        IEnumerable<OrderField>? orderBy = null,
        int top = 0,
        string? hints = null)
    {
        // Ensure with guards
        GuardTableName(tableName);

        // Validate the hints
        GuardHints(hints);

        // There should be fields
        if (fields?.Any() != true)
        {
            throw new EmptyException($"The list of queryable fields must not be null for '{tableName}'.");
        }

        // Initialize the builder
        var builder = new QueryBuilder();

        // Build the query
        builder
            .Select()
            .FieldsFrom(fields, DbSetting)
            .From()
            .TableNameFrom(tableName, DbSetting)
            .WhereFrom(where, DbSetting)
            .OrderByFrom(orderBy, DbSetting);
        if (top > 0)
        {
            builder.Limit(top);
        }
        builder.End();

        // Return the query
        return builder.ToString();
    }

    #endregion

    #region CreateSkipQuery

    /// <summary>
    /// Creates a SQL Statement for 'BatchQuery' operation.
    /// </summary>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="take">The number of rows per batch.</param>
    /// <param name="orderBy">The list of fields for ordering.</param>
    /// <param name="where">The query expression.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <returns>A sql statement for batch query operation.</returns>
    public override string CreateSkipQuery(string tableName,
        IEnumerable<Field> fields,
        int skip,
        int take,
        IEnumerable<OrderField>? orderBy = null,
        QueryGroup? where = null,
        string? hints = null)
    {
        // Ensure with guards
        GuardTableName(tableName);

        // Validate the hints
        GuardHints(hints);

        // There should be fields
        if (fields?.Any() != true)
        {
            throw new EmptyException($"The list of queryable fields must not be null for '{tableName}'.");
        }

        // Validate order by
        if (orderBy == null || orderBy.Any() != true)
        {
            throw new EmptyException("The argument 'orderBy' is required.");
        }

        // Validate the skip
        if (skip < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(skip), "The rows skipped must be equals or greater than 0.");
        }

        // Validate the take
        if (take < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(take), "The rows per batch must be equals or greater than 1.");
        }

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
            .LimitOffset(take, skip)
            .End();

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
        // Ensure with guards
        GuardTableName(tableName);

        // Initialize the builder
        var builder = new QueryBuilder();

        // Build the query
        builder
            .Truncate()
            .Table()
            .TableNameFrom(tableName, DbSetting)
            .WriteText("RESTART IDENTITY")
            .End();

        // Return the query
        return builder.ToString();
    }

    #endregion

    #region CreateUpdateAll
    /// <inheritdoc/>
    public override string CreateUpdateAll(
        string tableName,
        IEnumerable<Field> fields,
        IEnumerable<Field>? qualifiers,
        int batchSize,
        IEnumerable<DbField> keyFields,
        string? hints = null)
    {
        // Use base implementation for batch size 1
        if (batchSize == 1)
        {
            return base.CreateUpdateAll(tableName, fields, qualifiers, batchSize, keyFields, hints);
        }

        GuardTableName(tableName);
        GuardHints(hints);

        var primaryField = keyFields.FirstOrDefault(f => f.IsPrimary);

        ValidateMultipleStatementExecution(batchSize);

        if (fields?.Any() != true)
        {
            throw new EmptyException(nameof(fields), "The list of fields cannot be null or empty.");
        }

        qualifiers ??= keyFields.Where(x => x.IsPrimary).AsFields();

        if (qualifiers?.Any() == true)
        {
            var unmatchedQualifiers = qualifiers.Where(field =>
                fields.FirstOrDefault(f =>
                    string.Equals(field.Name, f.Name, StringComparison.OrdinalIgnoreCase)) == null);

            if (unmatchedQualifiers.Any())
            {
                throw new InvalidQualifiersException($"The qualifiers '{unmatchedQualifiers.Select(f => f.Name).Join(", ")}' are not " +
                    $"present in the given fields '{fields.Select(f => f.Name).Join(", ")}'.");
            }
        }
        else
        {
            if (primaryField != null)
            {
                var isPresent = fields.Any(f =>
                    string.Equals(f.Name, primaryField.Name, StringComparison.OrdinalIgnoreCase));

                if (!isPresent)
                {
                    throw new InvalidQualifiersException($"There are no qualifier fields found for '{tableName}'. Ensure that the " +
                        $"primary field is present in the given fields '{fields.Select(f => f.Name).Join(", ")}'.");
                }

                qualifiers = keyFields.AsFields();
            }
            else
            {
                throw new ArgumentNullException(nameof(qualifiers), $"There are no qualifier fields found for '{tableName}'.");
            }
        }

        var updateFields = fields
            .Where(f => keyFields.GetByName(f.Name) is null && qualifiers.GetByName(f.Name) is null)
            .ToArray();

        if (!updateFields.Any())
        {
            throw new EmptyException(nameof(fields), "The list of updatable fields cannot be null or empty.");
        }

        var builder = new QueryBuilder();

        // UPDATE table AS T
        builder.Update()
               .TableNameFrom(tableName, DbSetting)
               .As("T")
               .Set();

        // SET field1 = S.field1, ...
        builder.AppendJoin(updateFields.Select(f =>
            $"{f.Name.AsField(DbSetting)} = S.{f.Name.AsField(DbSetting)}"), ", ");

        // FROM (VALUES (...), (...)) AS S(field1, field2, ...)
        builder.From()
               .OpenParen()
               .WriteText("VALUES");

        for (var i = 0; i < batchSize; i++)
        {
            if (i > 0)
                builder.Comma();

            builder
                .OpenParen()
                .ParametersFrom(fields, i, DbSetting)
                .CloseParen();
        }

        builder
            .CloseParen()
            .WriteText("AS S")
            .OpenParen()
            .FieldsFrom(fields, DbSetting)
            .CloseParen();

        // WHERE T.qualifier = S.qualifier AND ...
        builder
            .Where()
            .AppendJoin(qualifiers.Select(q => $"T.{q.Name.AsField(DbSetting)} = S.{q.Name.AsField(DbSetting)}"), " AND ")
            .End(DbSetting);

        return builder.ToString();
    }
    #endregion

    #region Helpers

    private string GetDatabaseType(DbField dbField)
    {
        var dbType = new ClientTypeToDbTypeResolver().Resolve(dbField.Type);
        return dbType.HasValue ?
            new DbTypeToPostgreSqlStringNameResolver().Resolve(dbType.Value) : null;
    }

    #endregion
}
