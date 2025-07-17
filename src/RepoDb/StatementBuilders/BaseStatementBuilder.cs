using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb.StatementBuilders;

/// <summary>
/// A base class for all <see cref="IStatementBuilder"/>-based objects.
/// </summary>
public abstract class BaseStatementBuilder : IStatementBuilder
{
    public const string RepoDbOrderColumn = "__RepoDb_OrderColumn";

    /// <summary>
    /// Creates a new instance of <see cref="BaseStatementBuilder"/> class.
    /// </summary>
    /// <param name="dbSetting">The database settings object currently in used.</param>
    /// <param name="convertFieldResolver">The resolver used when converting a field in the database layer.</param>
    /// <param name="averageableClientTypeResolver">The resolver used to identity the type for average.</param>
    protected BaseStatementBuilder(IDbSetting dbSetting,
        IResolver<Field, IDbSetting, string>? convertFieldResolver = null,
        IResolver<Type, Type?>? averageableClientTypeResolver = null)
    {
        ArgumentNullException.ThrowIfNull(dbSetting);
        DbSetting = dbSetting;
        ConvertFieldResolver = convertFieldResolver;
        AverageableClientTypeResolver = averageableClientTypeResolver;
    }

    #region Properties

    /// <summary>
    /// Gets the database setting object that is currently in used.
    /// </summary>
    protected IDbSetting DbSetting { get; }

    /// <summary>
    /// Gets the resolver used to convert the <see cref="Field"/> object.
    /// </summary>
    protected IResolver<Field, IDbSetting, string>? ConvertFieldResolver { get; }

    /// <summary>
    /// Gets the resolver that is being used to resolve the type to be averageable type.
    /// </summary>
    protected IResolver<Type, Type?>? AverageableClientTypeResolver { get; }

    #endregion

    #region Virtual/Common

    #region CreateAverage

    /// <inheritdoc />
    public virtual string CreateAverage(
        string tableName,
        Field field,
        QueryGroup? where = null,
        string? hints = null)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(tableName);
        ArgumentNullException.ThrowIfNull(field);

        // Validate the hints
        GuardHints(hints);

        // Check the field
        field = new Field(field.FieldName, AverageableClientTypeResolver?.Resolve(field.Type ?? DbSetting.AverageableType));

        // Initialize the builder
        var builder = new QueryBuilder();

        // Build the query
        builder
            .Select()
            .Average(field, DbSetting, ConvertFieldResolver)
            .WriteText($"AS {"AverageValue".AsQuoted(DbSetting)}")
            .From()
            .TableNameFrom(tableName, DbSetting)
            .HintsFrom(hints)
            .WhereFrom(where, DbSetting)
            .End(DbSetting);

        // Return the query
        return builder.ToString();
    }

    #endregion

    #region CreateAverageAll

    /// <inheritdoc />
    public virtual string CreateAverageAll(
        string tableName,
        Field field,
        string? hints = null)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(tableName);
        ArgumentNullException.ThrowIfNull(field);

        // Validate the hints
        GuardHints(hints);

        // Check the field
        field = new(field.FieldName, AverageableClientTypeResolver?.Resolve(field.Type ?? DbSetting.AverageableType));

        // Initialize the builder
        var builder = new QueryBuilder();

        // Build the query
        builder
            .Select()
            .Average(field, DbSetting, ConvertFieldResolver)
            .WriteText($"AS {"AverageValue".AsQuoted(DbSetting)}")
            .From()
            .TableNameFrom(tableName, DbSetting)
            .HintsFrom(hints)
            .End(DbSetting);

        // Return the query
        return builder.ToString();
    }

    #endregion

    #region CreateCount

    /// <inheritdoc />
    public virtual string CreateCount(
        string tableName,
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
            .Count(null, DbSetting)
            .WriteText($"AS {"CountValue".AsQuoted(DbSetting)}")
            .From()
            .TableNameFrom(tableName, DbSetting)
            .HintsFrom(hints)
            .WhereFrom(where, DbSetting)
            .End(DbSetting);

        // Return the query
        return builder.ToString();
    }

    #endregion

    #region CreateCountAll

    /// <inheritdoc />
    public virtual string CreateCountAll(
        string tableName,
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
            .Count(null, DbSetting)
            .WriteText($"AS {"CountValue".AsQuoted(DbSetting)}")
            .From()
            .TableNameFrom(tableName, DbSetting)
            .HintsFrom(hints)
            .End(DbSetting);

        // Return the query
        return builder.ToString();
    }

    #endregion

    #region CreateDelete

    /// <inheritdoc />
    public virtual string CreateDelete(
        string tableName,
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
            .Delete()
            .From()
            .TableNameFrom(tableName, DbSetting)
            .HintsFrom(hints)
            .WhereFrom(where, DbSetting)
            .End(DbSetting);

        // Return the query
        return builder.ToString();
    }

    #endregion

    #region CreateDeleteAll

    /// <inheritdoc />
    public virtual string CreateDeleteAll(
        string tableName,
        string? hints = null)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(tableName);

        // Validate the hints
        GuardHints(hints);

        // Initialize the builder
        var builder = new QueryBuilder();

        // Build the query
        builder
            .Delete()
            .From()
            .TableNameFrom(tableName, DbSetting)
            .HintsFrom(hints)
            .End(DbSetting);

        // Return the query
        return builder.ToString();
    }

    #endregion

    #region CreateExists

    /// <inheritdoc />
    public virtual string CreateExists(
        string tableName,
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
            .TopFrom(1)
            .WriteText($"1 AS {("ExistsValue").AsQuoted(DbSetting)}")
            .From()
            .TableNameFrom(tableName, DbSetting)
            .HintsFrom(hints)
            .WhereFrom(where, DbSetting)
            .End(DbSetting);

        // Return the query
        return builder.ToString();
    }

    #endregion

    #region CreateInsert

    ///<inheritdoc/>
    public virtual string CreateInsert(
        string tableName,
        IEnumerable<Field> fields,
        IEnumerable<DbField> keyFields,
        string? hints = null)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(tableName);
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
            .CloseParen()
            .End(DbSetting);

        // Return the query
        return builder.ToString();
    }

    #endregion

    #region CreateInsertAll

    /// <inheritdoc/>
    public virtual string CreateInsertAll(
        string tableName,
        IEnumerable<Field> fields,
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
            .Select()
            .FieldsFrom(insertableFields, DbSetting)
            .From()
            .OpenParen()
            .Values();

        // Iterate the indexes
        for (var index = 0; index < batchSize; index++)
        {
            if (index > 0)
                builder.Comma();

            builder
                .OpenParen()
                .ParametersFrom(insertableFields, index, DbSetting)
                .Comma()
                .WriteText($"{index}")
                .CloseParen();
        }

        // Close
        builder
            .CloseParen()
            .As("T")
            .OpenParen()
            .FieldsFrom(insertableFields, DbSetting)
            .Comma()
            .WriteText(RepoDbOrderColumn.AsQuoted(DbSetting))
            .CloseParen()
            .OrderBy()
            .WriteText(RepoDbOrderColumn.AsQuoted(DbSetting))
            .End(DbSetting);

        // Return the query
        return builder.ToString();
    }

    #endregion

    #region CreateMax

    /// <inheritdoc />
    public virtual string CreateMax(
        string tableName,
        Field field,
        QueryGroup? where = null,
        string? hints = null)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(tableName);

        // Validate the hints
        GuardHints(hints);

        // Check the field
        ArgumentNullException.ThrowIfNull(field);

        // Initialize the builder
        var builder = new QueryBuilder();

        // Build the query
        builder
            .Select()
            .Max(field, DbSetting)
            .WriteText($"AS {"MaxValue".AsQuoted(DbSetting)}")
            .From()
            .TableNameFrom(tableName, DbSetting)
            .HintsFrom(hints)
            .WhereFrom(where, DbSetting)
            .End(DbSetting);

        // Return the query
        return builder.ToString();
    }

    #endregion

    #region CreateMaxAll

    /// <inheritdoc />
    public virtual string CreateMaxAll(
        string tableName,
        Field field,
        string? hints = null)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(tableName);
        ArgumentNullException.ThrowIfNull(field);

        // Validate the hints
        GuardHints(hints);

        // Initialize the builder
        var builder = new QueryBuilder();

        // Build the query
        builder
            .Select()
            .Max(field, DbSetting)
            .WriteText($"AS {"MaxValue".AsQuoted(DbSetting)}")
            .From()
            .TableNameFrom(tableName, DbSetting)
            .HintsFrom(hints)
            .End(DbSetting);

        // Return the query
        return builder.ToString();
    }

    #endregion

    #region CreateMin

    /// <inheritdoc />
    public virtual string CreateMin(
        string tableName,
        Field field,
        QueryGroup? where = null,
        string? hints = null)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(tableName);
        ArgumentNullException.ThrowIfNull(field);

        // Validate the hints
        GuardHints(hints);

        // Initialize the builder
        var builder = new QueryBuilder();

        // Build the query
        builder
            .Select()
            .Min(field, DbSetting)
            .WriteText($"AS {"MinValue".AsQuoted(DbSetting)}")
            .From()
            .TableNameFrom(tableName, DbSetting)
            .HintsFrom(hints)
            .WhereFrom(where, DbSetting)
            .End(DbSetting);

        // Return the query
        return builder.ToString();
    }

    #endregion

    #region CreateMinAll

    /// <inheritdoc />
    public virtual string CreateMinAll(
        string tableName,
        Field field,
        string? hints = null)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(tableName);
        ArgumentNullException.ThrowIfNull(field);

        // Validate the hints
        GuardHints(hints);

        // Initialize the builder
        var builder = new QueryBuilder();

        // Build the query
        builder
            .Select()
            .Min(field, DbSetting)
            .WriteText($"AS {"MinValue".AsQuoted(DbSetting)}")
            .From()
            .TableNameFrom(tableName, DbSetting)
            .HintsFrom(hints)
            .End(DbSetting);

        // Return the query
        return builder.ToString();
    }

    #endregion

    #region CreateQuery

    /// <inheritdoc />
    public virtual string CreateQuery(
        string tableName,
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
            .TopFrom(top)
            .FieldsFrom(fields, DbSetting)
            .From()
            .TableNameFrom(tableName, DbSetting)
            .HintsFrom(hints)
            .WhereFrom(where, DbSetting)
            .OrderByFrom(orderBy, DbSetting)
            .End(DbSetting);

        // Return the query
        return builder.ToString();
    }

    #endregion

    #region CreateQueryAll

    /// <inheritdoc />
    public virtual string CreateQueryAll(
        string tableName,
        IEnumerable<Field> fields,
        IEnumerable<OrderField>? orderBy = null,
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
            .OrderByFrom(orderBy, DbSetting)
            .End(DbSetting);

        // Return the query
        return builder.ToString();
    }

    #endregion

    #region CreateSum

    /// <inheritdoc />
    public virtual string CreateSum(
        string tableName,
        Field field,
        QueryGroup? where = null,
        string? hints = null)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(tableName);
        ArgumentNullException.ThrowIfNull(field);

        // Validate the hints
        GuardHints(hints);

        // Initialize the builder
        var builder = new QueryBuilder();

        // Build the query
        builder
            .Select()
            .Sum(field, DbSetting)
            .WriteText($"AS {"SumValue".AsQuoted(DbSetting)}")
            .From()
            .TableNameFrom(tableName, DbSetting)
            .HintsFrom(hints)
            .WhereFrom(where, DbSetting)
            .End(DbSetting);

        // Return the query
        return builder.ToString();
    }

    #endregion

    #region CreateSumAll

    /// <inheritdoc />
    public virtual string CreateSumAll(
        string tableName,
        Field field,
        string? hints = null)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(tableName);
        ArgumentNullException.ThrowIfNull(field);

        // Validate the hints
        GuardHints(hints);

        // Initialize the builder
        var builder = new QueryBuilder();

        // Build the query
        builder
            .Select()
            .Sum(field, DbSetting)
            .WriteText($"AS {"SumValue".AsQuoted(DbSetting)}")
            .From()
            .TableNameFrom(tableName, DbSetting)
            .HintsFrom(hints)
            .End(DbSetting);

        // Return the query
        return builder.ToString();
    }

    #endregion

    #region CreateTruncate

    /// <summary>
    /// Creates a SQL Statement for 'Truncate' operation.
    /// </summary>
    /// <param name="tableName">The name of the target table.</param>
    /// <returns>A sql statement for truncate operation.</returns>
    public virtual string CreateTruncate(string tableName)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(tableName);

        // Initialize the builder
        var builder = new QueryBuilder();

        // Build the query
        builder
            .Truncate()
            .Table()
            .TableNameFrom(tableName, DbSetting)
            .End(DbSetting);

        // Return the query
        return builder.ToString();
    }

    #endregion

    #region CreateUpdate

    /// <inheritdoc/>
    public virtual string CreateUpdate(
        string tableName,
        IEnumerable<Field> fields,
        QueryGroup? where,
        IEnumerable<DbField> keyFields,
        string? hints = null)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(tableName);
        GuardHints(hints);

        var primaryField = keyFields.FirstOrDefault(f => f.IsPrimary);
        var identityField = keyFields.FirstOrDefault(f => f.IsIdentity);
        GuardPrimary(primaryField);
        GuardIdentity(identityField);

        // Gets the updatable fields
        var updatableFields = fields
            .Where(f => keyFields.GetByFieldName(f.FieldName) is null);

        // Check if there are updatable fields
        if (updatableFields.Any() != true)
        {
            throw new EmptyException("The list of updatable fields cannot be null or empty.");
        }

        // Initialize the builder
        var builder = new QueryBuilder();

        // Build the query
        builder
            .Update()
            .TableNameFrom(tableName, DbSetting)
            .HintsFrom(hints)
            .Set()
            .FieldsAndParametersFrom(updatableFields, 0, DbSetting)
            .WhereFrom(where, DbSetting)
            .End(DbSetting);

        // Return the query
        return builder.ToString();
    }

    #endregion

    #region CreateUpdateAll

    /// <summary>
    /// Creates a SQL Statement for 'UpdateAll' operation.
    /// </summary>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="fields">The list of fields to be updated.</param>
    /// <param name="qualifiers">The list of the qualifier <see cref="Field"/> objects.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="primaryField">The primary field from the database.</param>
    /// <param name="identityField">The identity field from the database.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <returns>A sql statement for update-all operation.</returns>
    public virtual string CreateUpdateAll(
        string tableName,
        IEnumerable<Field> fields,
        IEnumerable<Field> qualifiers,
        int batchSize,
        IEnumerable<DbField> keyFields,
        string? hints = null)
    {
        // Ensure with guards
        ArgumentNullException.ThrowIfNullOrWhiteSpace(tableName);
        GuardHints(hints);

        // Validate the multiple statement execution
        ValidateMultipleStatementExecution(batchSize);

        // Ensure the fields
        if (fields?.Any() != true)
        {
            throw new EmptyException(nameof(fields), $"The list of fields cannot be null or empty.");
        }

        // Gets the updatable fields
        var updateFields = fields
            .Where(f => keyFields.GetByFieldName(f.FieldName) is null && qualifiers?.GetByFieldName(f.FieldName) is null);

        // Check if there are updatable fields
        if (updateFields.Any() != true)
        {
            throw new EmptyException(nameof(fields), "The list of updatable fields cannot be null or empty.");
        }

        // Initialize the builder
        var builder = new QueryBuilder();

        // Iterate the indexes
        for (var index = 0; index < batchSize; index++)
        {
            builder
                .Update()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .Set()
                .FieldsAndParametersFrom(updateFields, index, DbSetting)
                .WhereFrom(qualifiers, index, DbSetting)
                .End(DbSetting);
        }

        // Return the query
        return builder.ToString();
    }

    #endregion

    #endregion

    #region Abstract

    #region CreateBatchQuery

    /// <summary>
    /// Creates a SQL Statement for 'BatchQuery' operation.
    /// </summary>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="page">The page of the batch.</param>
    /// <param name="rowsPerBatch">The number of rows per batch.</param>
    /// <param name="orderBy">The list of fields for ordering.</param>
    /// <param name="where">The query expression.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <returns>A sql statement for batch query operation.</returns>
    public abstract string CreateBatchQuery(
        string tableName,
        IEnumerable<Field> fields,
        int page,
        int rowsPerBatch,
        IEnumerable<OrderField>? orderBy = null,
        QueryGroup? where = null,
        string? hints = null);

    #endregion

    #region CreateMerge

    /// <summary>
    /// Creates a SQL Statement for 'Merge' operation.
    /// </summary>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="fields">The list of fields to be merged.</param>
    /// <param name="qualifiers">The list of the qualifier <see cref="Field"/> objects.</param>
    /// <param name="primaryField">The primary field from the database.</param>
    /// <param name="identityField">The identity field from the database.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <returns>A sql statement for merge operation.</returns>
    public abstract string CreateMerge(
        string tableName,
        IEnumerable<Field> fields,
        IEnumerable<Field>? noUpdateFields,
        IEnumerable<DbField> keyFields,
        IEnumerable<Field> qualifiers, string? hints = null);

    #endregion

    #region CreateMergeAll

    /// <summary>
    /// Creates a SQL Statement for 'MergeAll' operation.
    /// </summary>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="fields">The list of fields to be merged.</param>
    /// <param name="qualifiers">The list of the qualifier <see cref="Field"/> objects.</param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="primaryField">The primary field from the database.</param>
    /// <param name="identityField">The identity field from the database.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <returns>A sql statement for merge operation.</returns>
    public abstract string CreateMergeAll(
        string tableName,
        IEnumerable<Field> fields,
        IEnumerable<Field>? noUpdateFields,
        IEnumerable<Field> qualifiers,
        int batchSize,
        IEnumerable<DbField> keyFields, string? hints = null);

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
    public abstract string CreateSkipQuery(
        string tableName,
        IEnumerable<Field> fields,
        int skip,
        int take,
        IEnumerable<OrderField>? orderBy = null,
        QueryGroup? where = null,
        string? hints = null);

    #endregion

    #endregion

    #region Helpers

    /// <summary>
    ///
    /// </summary>
    /// <param name="field"></param>
    /// <exception cref="InvalidOperationException"></exception>
    protected static void GuardPrimary(DbField? field)
    {
        if (field?.IsPrimary == false)
        {
            throw new InvalidOperationException("The field is not defined as primary.");
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="field"></param>
    /// <exception cref="InvalidOperationException"></exception>
    protected static void GuardIdentity(DbField? field)
    {
        if (field?.IsIdentity == false)
        {
            throw new InvalidOperationException("The field is not defined as identity.");
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="hints"></param>
    /// <exception cref="NotSupportedException"></exception>
    protected void GuardHints(string? hints = null)
    {
        if (!string.IsNullOrWhiteSpace(hints) && !DbSetting.AreTableHintsSupported)
        {
            throw new NotSupportedException("The table hints are not supported on this database provider statement builder.");
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <exception cref="NotSupportedException"></exception>
    protected void ValidateMultipleStatementExecution(int batchSize)
    {
        if (DbSetting.IsMultiStatementExecutable == false && batchSize > 1)
        {
            throw new NotSupportedException($"Multiple execution is not supported based on the current database setting '{DbSetting.GetType().FullName}'. Consider setting the batchSize to 1.");
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="primaryDbField"></param>
    /// <param name="identityDbField"></param>
    protected static DbField? GetReturnKeyColumnAsDbField(DbField? primaryDbField,
        DbField? identityDbField)
    {
        switch (GlobalConfiguration.Options.KeyColumnReturnBehavior)
        {
            case KeyColumnReturnBehavior.Primary:
                return primaryDbField;
            case KeyColumnReturnBehavior.Identity:
                return identityDbField;
            case KeyColumnReturnBehavior.PrimaryOrElseIdentity:
                return primaryDbField ?? identityDbField;
            case KeyColumnReturnBehavior.IdentityOrElsePrimary:
                return identityDbField ?? primaryDbField;
            default:
                throw new InvalidOperationException(nameof(GlobalConfiguration.Options.KeyColumnReturnBehavior));
        }
    }

    #endregion
}
