#if MYSQLPLAIN
using MySql.Data.MySqlClient;
#else
using MySqlConnector;
#endif
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb.StatementBuilders;

/// <summary>
/// A class that is being used to build a SQL Statement for MySql.
/// </summary>
#if MYSQLPLAIN
public sealed class MySqlStatementBuilder : BaseStatementBuilder
#else
public sealed class MySqlConnectorStatementBuilder : BaseStatementBuilder
#endif
{
#if MYSQLPLAIN
    public MySqlStatementBuilder()
        : this(DbSettingMapper.Get<MySqlConnection>()!,
              null,
              null)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="MySqlStatementBuilder"/> class.
    /// </summary>
    /// <param name="dbSetting">The database settings object currently in used.</param>
    /// <param name="convertFieldResolver">The resolver used when converting a field in the database layer.</param>
    /// <param name="averageableClientTypeResolver">The resolver used to identity the type for average.</param>
    public MySqlStatementBuilder(IDbSetting dbSetting,
        IResolver<Field, IDbSetting, string?>? convertFieldResolver = null,
        IResolver<Type, Type?>? averageableClientTypeResolver = null)
        : base(dbSetting,
              convertFieldResolver,
              averageableClientTypeResolver)
    { }
#else
    /// <summary>
    /// Creates a new instance of <see cref="MySqlStatementBuilder"/> object.
    /// </summary>
    public MySqlConnectorStatementBuilder()
        : this(DbSettingMapper.Get<MySqlConnection>()!,
              null,
              null)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="MySqlConnectorStatementBuilder"/> class.
    /// </summary>
    /// <param name="dbSetting">The database settings object currently in used.</param>
    /// <param name="convertFieldResolver">The resolver used when converting a field in the database layer.</param>
    /// <param name="averageableClientTypeResolver">The resolver used to identity the type for average.</param>
    public MySqlConnectorStatementBuilder(IDbSetting dbSetting,
        IResolver<Field, IDbSetting, string?>? convertFieldResolver = null,
        IResolver<Type, Type?>? averageableClientTypeResolver = null)
        : base(dbSetting,
              convertFieldResolver,
              averageableClientTypeResolver)
    { }
#endif

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
        ArgumentNullException.ThrowIfNullOrWhiteSpace(tableName);

        // Validate the hints
        GuardHints(hints);

        // There should be fields
        if (fields?.Any() != true)
        {
            throw new ArgumentNullException($"The list of queryable fields must not be null for '{tableName}'.");
        }

        // Validate order by
        if (orderBy == null || orderBy.Any() != true)
        {
            throw new EmptyException(nameof(orderBy), "The argument 'orderBy' is required.");
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
            .LimitTake(rowsPerBatch, skip)
            .End();

        // Return the query
        return builder.ToString();
    }

    #endregion

    #region CreateCount

    /// <summary>
    /// Creates a SQL Statement for count operation.
    /// </summary>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <returns>A sql statement for count operation.</returns>
    public override string CreateCount(string tableName,
        QueryGroup? where = null,
        string? hints = null)
    {
        var result = base.CreateCount(tableName,
            where,
            hints);

        // Return the query
        return result.Replace("COUNT (", "COUNT(");
    }

    #endregion

    #region CreateCountAll

    /// <summary>
    /// Creates a SQL Statement for count-all operation.
    /// </summary>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <returns>A sql statement for count-all operation.</returns>
    public override string CreateCountAll(string tableName,
        string? hints = null)
    {
        var result = base.CreateCountAll(tableName,
            hints);

        // Return the query
        return result.Replace("COUNT (", "COUNT(");
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
        ArgumentNullException.ThrowIfNullOrWhiteSpace(tableName);

        // Validate the hints
        GuardHints(hints);

        // Initialize the builder
        var builder = new QueryBuilder();

        // Build the query
        builder
            .Select()
            .WriteText("1")
            .As("ExistsValue", DbSetting)
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
        // Initialize the builder
        var builder = new QueryBuilder();

        // Call the base
        builder.WriteText(
            base.CreateInsert(tableName,
                fields,
                keyFields,
                hints));

        if (keyFields.Any())
        {
            builder
                .Select();

            bool firstField = true;
            foreach (var kf in keyFields)
            {
                if (firstField)
                    firstField = false;
                else
                    builder.Comma();

                if (kf.IsIdentity)
                {
                    builder
                        .WriteText("LAST_INSERT_ID()");
                }
                else
                {
                    builder
                        .WriteText(kf.FieldName.AsParameter(DbSetting));
                }

                builder.As(kf.FieldName, DbSetting);
            }
            builder
                .End(DbSetting);
        }

        // Return the query
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
        IEnumerable<Field>? fields,
        int batchSize,
        IEnumerable<DbField> keyFields,
        string? hints = null)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(tableName);
        GuardHints(hints);

        // Validate the multiple statement execution
        ValidateMultipleStatementExecution(batchSize);

        // Verify the fields
        if (fields?.Any() != true)
        {
            throw new EmptyException(nameof(fields), "The list of fields cannot be null or empty.");
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
                .Row()
                .OpenParen()
                .ParametersFrom(insertableFields, index, DbSetting)
                .CloseParen();
        }

        // Close
        builder
            .End();

        if (keyFields.Any())
        {
            builder
                .Select()
                .WriteText("*")
                .From()
                .OpenParen()
                .Values();

            for (var index = 0; index < batchSize; index++)
            {
                if (index > 0)
                    builder.Comma();

                builder
                    .Row()
                    .OpenParen();

                bool firstField = true;
                foreach (var kf in keyFields)
                {
                    if (firstField)
                        firstField = false;
                    else
                        builder.Comma();

                    if (kf.IsIdentity)
                    {
                        builder
                            .WriteText("LAST_INSERT_ID() +")
                            .WriteText($"{index}");
                    }
                    else
                    {
                        builder
                            .WriteText(kf.FieldName.AsParameter(index, DbSetting));
                    }
                }

                builder.CloseParen();
            }

            builder
                .CloseParen()
                .As()
                .WriteText("RESULT")
                .OpenParen()
                .FieldsFrom(keyFields, DbSetting)
                .CloseParen()
                .End();
        }

        return builder.ToString();
    }

    #endregion

    #region CreateMax

    /// <summary>
    /// Creates a SQL Statement for maximum operation.
    /// </summary>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="field">The field to be maximumd.</param>
    /// <param name="where">The query expression.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <returns>A sql statement for maximum operation.</returns>
    public override string CreateMax(string tableName,
        Field field,
        QueryGroup? where = null,
        string? hints = null)
    {
        var result = base.CreateMax(tableName,
            field,
            where,
            hints);

        // Return the query
        return result.Replace("MAX (", "MAX(");
    }

    #endregion

    #region CreateMaxAll

    /// <summary>
    /// Creates a SQL Statement for maximum-all operation.
    /// </summary>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="field">The field to be maximumd.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <returns>A sql statement for maximum-all operation.</returns>
    public override string CreateMaxAll(string tableName,
        Field field,
        string? hints = null)
    {
        var result = base.CreateMaxAll(tableName,
            field,
            hints);

        // Return the query
        return result.Replace("MAX (", "MAX(");
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
        IEnumerable<Field>? noUpdateFields,
        IEnumerable<DbField> keyFields,
        IEnumerable<Field> qualifiers, string? hints = null)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(tableName);
        GuardHints(hints);

        // Verify the fields
        if (fields?.Any() != true)
        {
            throw new ArgumentNullException(nameof(fields), $"The list of fields cannot be null or empty.");
        }

        // Set the qualifiers
        if (qualifiers?.Any() != true)
            qualifiers = keyFields;

        // Validate the qualifiers
        if (qualifiers?.Any() != true)
        {
            var primaryField = keyFields.FirstOrDefault(f => f.IsPrimary);

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
        var insertableFields = fields;
        var updatableFields = EnumerableExtension.AsList(fields.Where(f => qualifiers.GetByFieldName(f.FieldName) is null && noUpdateFields?.GetByFieldName(f.FieldName) is null && keyFields.GetByFieldName(f.FieldName) is not { IsIdentity: true }));

        bool insertingIdentity = qualifiers.Any(x => keyFields.GetByFieldName(x.FieldName) is { IsIdentity: true }) && fields.Any(f => keyFields.GetByFieldName(f.FieldName) is { IsIdentity: true }) && GlobalConfiguration.Options.SqlServerIdentityInsert;

        if (!insertingIdentity)
        {
            insertableFields = fields.Where(f => keyFields.GetByFieldName(f.FieldName) is not { IsIdentity: true }).AsFieldSet();
        }

        // Build the query
        builder
            .Insert();

        if (updatableFields.Count == 0)
            builder.Ignore();

        builder
            .Into()
            .TableNameFrom(tableName, DbSetting)
            .OpenParen()
            .FieldsFrom(insertableFields, DbSetting)
            .CloseParen()
            .Values()
            .OpenParen()
            .ParametersFrom(insertableFields, 0, DbSetting)
            .CloseParen();

        if (updatableFields.Count > 0)
        {
            builder
                .WriteText("ON DUPLICATE KEY")
                .Update();

            IdentityFieldsAndParametersFrom(builder, fields, updatableFields, 0, keyFields.FirstOrDefault(x => x.IsIdentity && qualifiers.GetByFieldName(x.FieldName) is null));
        }
        builder
            .End();

        if (keyFields.Any())
        {
            builder
                .Select();

            bool firstField = true;
            foreach (var kf in keyFields)
            {
                if (firstField)
                    firstField = false;
                else
                    builder.Comma();

                if (kf.IsIdentity && qualifiers.GetByFieldName(kf.FieldName) is null)
                {
                    builder
                        .WriteText("LAST_INSERT_ID()");
                }
                else
                {
                    builder
                        .WriteText(kf.FieldName.AsParameter(DbSetting));
                }

                builder.As(kf.FieldName, DbSetting);
            }
            builder
                .End(DbSetting);
        }

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
        ArgumentNullException.ThrowIfNullOrWhiteSpace(tableName);
        GuardHints(hints);

        // Verify the fields
        if (fields?.Any() != true)
        {
            throw new ArgumentNullException(nameof(fields), $"The list of fields cannot be null or empty.");
        }

        // Set the qualifiers
        if (qualifiers?.Any() != true)
            qualifiers = keyFields;

        // Validate the qualifiers
        if (qualifiers?.Any() != true)
        {
            var primaryField = keyFields.FirstOrDefault(f => f.IsPrimary);

            if (primaryField == null)
            {
                throw new PrimaryFieldNotFoundException($"The is no primary field from the table '{tableName}' that can be used as qualifier.");
            }
            else
            {
                throw new InvalidQualifiersException($"There are no defined qualifier fields.");
            }
        }

        var builder = new QueryBuilder();

        // Remove the qualifiers from the fields
        var insertableFields = fields;
        var updatableFields = EnumerableExtension.AsList(fields.Where(f => qualifiers.GetByFieldName(f.FieldName) is null && noUpdateFields?.GetByFieldName(f.FieldName) is null && keyFields.GetByFieldName(f.FieldName) is not { IsIdentity: true }));

        bool insertingIdentity = qualifiers.Any(x => keyFields.GetByFieldName(x.FieldName) is { IsIdentity: true }) && fields.Any(f => keyFields.GetByFieldName(f.FieldName) is { IsIdentity: true }) && GlobalConfiguration.Options.SqlServerIdentityInsert;

        if (!insertingIdentity)
        {
            insertableFields = fields.Where(f => keyFields.GetByFieldName(f.FieldName) is not { IsIdentity: true }).AsFieldSet();
        }

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
                .CloseParen()
                .Values()
                .OpenParen()
                .ParametersFrom(fields, index, DbSetting)
                .CloseParen()
                .WriteText("ON DUPLICATE KEY")
                .Update();

            IdentityFieldsAndParametersFrom(builder, fields, updatableFields, index, keyFields.FirstOrDefault(x => x.IsIdentity && qualifiers.GetByFieldName(x.FieldName) is null));

            builder
                .End(DbSetting);

            if (keyFields.Any())
            {
                builder
                    .Select();
                bool first = true;

                foreach (var kf in keyFields)
                {
                    if (first)
                        first = false;
                    else
                        builder.Comma();

                    if (kf.IsIdentity && qualifiers.GetByFieldName(kf.FieldName) is null)
                    {
                        builder
                            .WriteText("COALESCE")
                            .OpenParen()
                            .WriteText(kf.FieldName.AsParameter(index, DbSetting))
                            .Comma()
                            .WriteText("LAST_INSERT_ID()")
                            .CloseParen();
                    }
                    else
                    {
                        builder.WriteText(kf.FieldName.AsParameter(index, DbSetting));
                    }
                    builder.As(kf.FieldName, DbSetting);
                }
                builder.End();
            }
        }

        // Return the query
        return builder.ToString();
    }

    private void IdentityFieldsAndParametersFrom(QueryBuilder builder, IEnumerable<Field> fields, IEnumerable<Field> updateFields, int index, DbField? identityField)
    {
        if (identityField is null || fields.GetByFieldName(identityField.FieldName) is null)
        {
            builder.FieldsAndParametersFrom(updateFields, index, DbSetting);
        }
        else
        {
            var id = identityField.AsField();
            // We want to have the LAST_INSERT_ID, and we have to set it ourselves here

            builder.FieldFrom(id, DbSetting);
            builder.WriteText("= LAST_INSERT_ID(");
            builder.WriteText(id.FieldName.AsField(DbSetting));
            builder.CloseParen();

            var filteredFields = updateFields.Where(x => !string.Equals(x.FieldName, id.FieldName, StringComparison.OrdinalIgnoreCase));
            if (filteredFields.Any())
            {
                builder.Comma();
                builder.FieldsAndParametersFrom(filteredFields, index, DbSetting);
            }
        }
    }

    #endregion

    #region CreateMin

    /// <summary>
    /// Creates a SQL Statement for minimum operation.
    /// </summary>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="field">The field to be minimumd.</param>
    /// <param name="where">The query expression.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <returns>A sql statement for minimum operation.</returns>
    public override string CreateMin(string tableName,
        Field field,
        QueryGroup? where = null,
        string? hints = null)
    {
        var result = base.CreateMin(tableName,
            field,
            where,
            hints);

        // Return the query
        return result.Replace("MIN (", "MIN(");
    }

    #endregion

    #region CreateMinAll

    /// <summary>
    /// Creates a SQL Statement for minimum-all operation.
    /// </summary>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="field">The field to be minimumd.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <returns>A sql statement for minimum-all operation.</returns>
    public override string CreateMinAll(string tableName,
        Field field,
        string? hints = null)
    {
        var result = base.CreateMinAll(tableName,
            field,
            hints);

        // Return the query
        return result.Replace("MIN (", "MIN(");
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
        ArgumentNullException.ThrowIfNullOrWhiteSpace(tableName);

        // Validate the hints
        GuardHints(hints);

        // There should be fields
        if (fields?.Any() != true)
        {
            throw new ArgumentNullException($"The list of queryable fields must not be null for '{tableName}'.");
        }

        // Initialize the builder
        var builder = new QueryBuilder();

        // Build the query
        builder
            .Select()
            .FieldsFrom(fields, DbSetting)
            .From()
            .TableNameFrom(tableName, DbSetting)
            .HintsFrom(hints)
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
    /// <param name="take">The number of rows to take.</param>
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
        ArgumentNullException.ThrowIfNullOrWhiteSpace(tableName);

        // Validate the hints
        GuardHints(hints);

        // There should be fields
        if (fields?.Any() != true)
        {
            throw new ArgumentNullException($"The list of queryable fields must not be null for '{tableName}'.");
        }

        // Validate order by
        if (orderBy == null || orderBy.Any() != true)
        {
            throw new EmptyException(nameof(orderBy), "The argument 'orderBy' is required.");
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
            .LimitTake(take, skip)
            .End();

        // Return the query
        return builder.ToString();
    }

    #endregion

    #region CreateSum

    /// <summary>
    /// Creates a SQL Statement for sum operation.
    /// </summary>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="field">The field to be sumd.</param>
    /// <param name="where">The query expression.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <returns>A sql statement for sum operation.</returns>
    public override string CreateSum(string tableName,
        Field field,
        QueryGroup? where = null,
        string? hints = null)
    {
        var result = base.CreateSum(tableName,
            field,
            where,
            hints);

        // Return the query
        return result.Replace("SUM (", "SUM(");
    }

    #endregion

    #region CreateSumAll

    /// <summary>
    /// Creates a SQL Statement for sum-all operation.
    /// </summary>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="field">The field to be sumd.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <returns>A sql statement for sum-all operation.</returns>
    public override string CreateSumAll(string tableName,
        Field field,
        string? hints = null)
    {
        var result = base.CreateSumAll(tableName,
            field,
            hints);

        // Return the query
        return result.Replace("SUM (", "SUM(");
    }

    #endregion
}
