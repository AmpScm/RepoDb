using System.Globalization;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Options;
using RepoDb.Resolvers;

namespace RepoDb.StatementBuilders;

/// <summary>
/// A class used to build a SQL Statement for SQL Server. This is the default statement builder used by the library.
/// </summary>
public sealed class SqlServerStatementBuilder : BaseStatementBuilder
{
    /// <summary>
    /// Creates a new instance of <see cref="SqlServerStatementBuilder"/> object.
    /// </summary>
    /// <param name="dbSetting">The database settings object currently in used.</param>
    public SqlServerStatementBuilder(IDbSetting dbSetting)
        : this(dbSetting,
            SqlServerConvertFieldResolver.Instance,
            ClientTypeToAverageableClientTypeResolver.Instance)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="SqlServerStatementBuilder"/> class.
    /// </summary>
    /// <param name="dbSetting">The database settings object currently in used.</param>
    /// <param name="convertFieldResolver">The resolver used when converting a field in the database layer.</param>
    /// <param name="averageableClientTypeResolver">The resolver used to identity the type for average.</param>
    public SqlServerStatementBuilder(IDbSetting dbSetting,
        IResolver<Field, IDbSetting, string?>? convertFieldResolver = null,
        IResolver<Type, Type?>? averageableClientTypeResolver = null)
        : base(dbSetting,
              convertFieldResolver ?? SqlServerConvertFieldResolver.Instance,
              averageableClientTypeResolver ?? ClientTypeToAverageableClientTypeResolver.Instance)
    { }

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
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);

        // Validate the hints
        GuardHints(hints);

        // Initialize the builder
        var builder = new QueryBuilder();

        // Build the query
        builder
            .Select()
            .CountBig(null, DbSetting)
            .WriteText("AS [CountValue]")
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

    /// <inheritdoc />
    public override string CreateInsert(string tableName,
        IEnumerable<Field> fields,
        IEnumerable<DbField> keyFields,
        string? hints = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);
        GuardHints(hints);

        // Verify the fields
        if (!fields.Any())
        {
            throw new EmptyException(nameof(fields), $"The list of insertable fields must not be null or empty for '{tableName}'.");
        }

        foreach (var keyField in keyFields)
        {
            if (!keyField.IsPrimary || keyField.IsGenerated || keyField.IsIdentity || keyField.IsNullable || keyField.HasDefaultValue)
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
            .CloseParen();

        if (keyFields.Any(c => c.IsIdentity || c.IsGenerated || c.HasDefaultValue))
        {
            builder
                .Output()
                .AsAliasFieldsFrom(keyFields, "INSERTED", DbSetting);
        }

        builder
            .Values()
            .OpenParen()
            .ParametersFrom(insertableFields, 0, DbSetting)
            .CloseParen();

        builder.End(DbSetting);
        return builder.ToString();
    }

    #endregion

    #region CreateInsertAll

    /// <inheritdoc />
    public override string CreateInsertAll(string tableName,
        IEnumerable<Field> fields,
        int batchSize,
        IEnumerable<DbField> keyFields,
        string? hints = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);
        GuardHints(hints);

        // Validate the multiple statement execution
        ValidateMultipleStatementExecution(batchSize);

        if (!fields.Any())
        {
            throw new EmptyException(nameof(fields), "The list of fields cannot be null or empty.");
        }

        // Validate primary key presence
        foreach (var keyField in keyFields)
        {
            if (!keyField.IsPrimary || keyField.IsGenerated || keyField.IsIdentity || keyField.IsNullable || keyField.HasDefaultValue)
                continue;

            if (fields.GetByFieldName(keyField.FieldName) is null)
            {
                throw new PrimaryFieldNotFoundException($"Primary field '{keyField.FieldName}' must be present in the field list.");
            }
        }

        // Determine insertable fields (excluding identity/generated)
        var insertableFields = fields
            .Where(f => keyFields.GetByFieldName(f.FieldName) is not { } x || !(x.IsGenerated || x.IsIdentity))
            .ToArray();

        var builder = new QueryBuilder();

        // CASE A: Super efficient. No output or batch small enough to avoid parallelism issues.
        if (!keyFields.Any() || batchSize < 4)
        {
            builder
                .Insert()
                .Into()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .OpenParen()
                .FieldsFrom(insertableFields, DbSetting)
                .CloseParen();

            if (keyFields.Any(c => c.IsGenerated || c.IsIdentity || c.HasDefaultValue))
            {
                builder
                    .Output()
                    .AsAliasFieldsFrom(keyFields, "INSERTED", DbSetting);
            }

            builder
                .WriteText("VALUES");

            for (var index = 0; index < batchSize; index++)
            {
                if (index > 0)
                    builder.Comma();

                builder
                    .OpenParen()
                    .ParametersFrom(insertableFields, index, DbSetting)
                    .CloseParen();
            }

            builder.End(DbSetting);
        }
        // CASE B: Use a merge which guarantees output ordering via added column
        else
        {
            builder
                .Merge().Into()
                .TableNameFrom(tableName, DbSetting)
                .As().WriteText("T")
                .Using().OpenParen().Values();

            for (var index = 0; index < batchSize; index++)
            {
                if (index > 0)
                    builder.Comma();

                builder
                    .OpenParen()
                    .ParametersFrom(insertableFields, index, DbSetting)
                    .Comma()
                    .WriteText(index.ToString(CultureInfo.InvariantCulture))
                    .CloseParen();
            }

            builder
                .CloseParen()
                .As()
                .WriteText("S")
                .OpenParen()
                .FieldsFrom(insertableFields, DbSetting)
                .Comma()
                .WriteQuoted(RepoDbOrderColumn, DbSetting)
                .CloseParen()
                .On().WriteText("1=0") // always insert
                .When().Not().Matched().Then()
                .Insert()
                .OpenParen()
                .FieldsFrom(insertableFields, DbSetting)
                .CloseParen()
                .Values()
                .OpenParen()
                .AsAliasFieldsFrom(insertableFields, "S", DbSetting)
                .CloseParen();

            if (keyFields.Any(c => c.IsIdentity || c.IsGenerated || c.HasDefaultValue))
            {
                builder
                    .WriteText("OUTPUT")
                    .AsAliasFieldsFrom(keyFields, "INSERTED", DbSetting)
                    .Comma()
                    .WriteText("S.")
                    .WriteQuoted(RepoDbOrderColumn, DbSetting, false);
            }

            builder.End(DbSetting);
        }

        return builder.ToString();
    }

    #endregion

    #region CreateMerge

    /// <inheritdoc />
    public override string CreateMerge(string tableName,
        IEnumerable<Field> fields,
        IEnumerable<Field>? noUpdateFields,
        IEnumerable<DbField> keyFields,
        IEnumerable<Field>? qualifiers, string? hints = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);
        GuardHints(hints);
        var primaryField = keyFields.FirstOrDefault(f => f.IsPrimary);
        var identityField = keyFields.FirstOrDefault(f => f.IsIdentity);
        GuardPrimary(primaryField);
        GuardIdentity(identityField);

        // Verify the fields
        if (!fields.Any())
        {
            throw new MissingFieldsException();
        }

        // Check the qualifiers
        if (qualifiers?.Any() != true)
            qualifiers = keyFields;

        // Check the qualifiers
        if (qualifiers?.Any() == true)
        {
            // Check if the qualifiers are present in the given fields
            var unmatchedQualifiers = qualifiers.Where(field => fields.GetByFieldName(field.FieldName) is null);

            // Throw an error we found any unmatched
            if (unmatchedQualifiers.Any())
            {
                throw new InvalidQualifiersException($"The qualifiers '{unmatchedQualifiers.Select(field => field.FieldName).Join(", ")}' are not " +
                    $"present at the given fields '{fields.Select(field => field.FieldName).Join(", ")}'.");
            }
        }
        else
        {
            if (primaryField is not null)
            {
                // Make sure that primary is present in the list of fields before qualifying to become a qualifier
                if (fields.GetByFieldName(primaryField.FieldName) is null)
                {
                    throw new InvalidQualifiersException($"There are no qualifier field objects found for '{tableName}'. Ensure that the " +
                        $"primary field is present at the given fields '{fields.Select(field => field.FieldName).Join(", ")}'.");
                }

                // The primary is present, use it as a default if there are no qualifiers given
                qualifiers = primaryField.AsField().AsEnumerable();
            }
            else
            {
                // Throw exception, qualifiers are not defined
                throw new MissingQualifierFieldsException($"There are no qualifier fields found for '{tableName}'.");
            }
        }

        // Get the insertable and updateable fields
        var insertableFields = fields;
        var updatableFields = fields.Where(f => qualifiers.GetByFieldName(f.FieldName) is null && noUpdateFields?.GetByFieldName(f.FieldName) is null && keyFields.GetByFieldName(f.FieldName) is not { IsIdentity: true }).ToList();

        bool insertingIdentity = qualifiers.Any(x => keyFields.GetByFieldName(x.FieldName) is { IsIdentity: true }) && fields.Any(f => keyFields.GetByFieldName(f.FieldName) is { IsIdentity: true });

        // Initialize the builder
        var builder = new QueryBuilder();

        if (insertingIdentity && SqlServerOptions.Current.UseIdentityInsert)
        {
            builder
                .WriteText("BEGIN TRY")
                .Set()
                .WriteText("IDENTITY_INSERT")
                .TableNameFrom(tableName, DbSetting)
                .On()
                .End(DbSetting)
                .WriteText("END TRY BEGIN CATCH END CATCH");
        }
        else
            insertableFields = fields.Where(f => keyFields.GetByFieldName(f.FieldName) is not { IsIdentity: true }).AsFieldSet();

        var parameterFields = fields;
        if (!insertingIdentity && keyFields.FirstOrDefault(x => x.IsIdentity) is { } idField)
            parameterFields = fields.Where(f => !string.Equals(f.FieldName, idField.FieldName, StringComparison.OrdinalIgnoreCase)).AsFieldSet();

        // Build the query
        builder
                // MERGE T USING S
                .Merge()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .As("T")
                .Using()
                .OpenParen()
                .Select()
                .ParametersAsFieldsFrom(parameterFields, 0, DbSetting)
                .CloseParen()
                .As("S")
                // QUALIFIERS
                .On()
                .OpenParen()
                .WriteText(qualifiers?
                    .Select(
                        field => field.AsJoinQualifier("S", "T", true, DbSetting))
                            .Join(" AND "))
                .CloseParen()
                // WHEN NOT MATCHED THEN INSERT VALUES
                .When()
                .Not()
                .Matched()
                .Then()
                .Insert()
                .OpenParen()
                .FieldsFrom(insertableFields, DbSetting)
                .CloseParen()
                .Values()
                .OpenParen()
                .AsAliasFieldsFrom(insertableFields, "S", DbSetting)
                .CloseParen();

        if (updatableFields.Count > 0)
        {
            builder
                // WHEN MATCHED THEN UPDATE SET
                .When()
                .Matched()
                .Then()
                .Update()
                .Set()
                .FieldsAndAliasFieldsFrom(updatableFields, "T", "S", DbSetting);
        }

        // Variables needed
        if (keyFields.Any())
        {
            builder
                .Output()
                .AsAliasFieldsFrom(keyFields, "INSERTED", DbSetting);
        }

        // End the builder
        builder.End(DbSetting);

        if (insertingIdentity && SqlServerOptions.Current.UseIdentityInsert)
        {
            builder
                .WriteText("BEGIN TRY")
                .Set()
                .WriteText("IDENTITY_INSERT")
                .TableNameFrom(tableName, DbSetting)
                .Off()
                .End(DbSetting)
                .WriteText("END TRY BEGIN CATCH END CATCH");
        }

        // Return the query
        return builder.ToString();
    }

    #endregion

    #region CreateMergeAll

    /// <inheritdoc />
    public override string CreateMergeAll(string tableName,
        IEnumerable<Field> fields,
        IEnumerable<Field>? noUpdateFields,
        IEnumerable<Field>? qualifiers,
        int batchSize,
        IEnumerable<DbField> keyFields, string? hints = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);
        GuardHints(hints);

        // Verify the fields
        if (!fields.Any())
        {
            throw new MissingFieldsException();
        }

        // Check the qualifiers
        if (qualifiers?.Any() != true)
            qualifiers = keyFields;

        if (qualifiers.Any())
        {
            // Check if the qualifiers are present in the given fields
            var unmatchedQualifiers = qualifiers.Where(field => fields.GetByFieldName(field.FieldName) is null);

            // Throw an error we found any unmatched
            if (unmatchedQualifiers.Any())
            {
                throw new InvalidQualifiersException($"The qualifiers '{unmatchedQualifiers.Select(field => field.FieldName).Join(", ")}' are not " +
                    $"present at the given fields '{fields.Select(field => field.FieldName).Join(", ")}'.");
            }
        }
        else
        {
            var primaryField = keyFields.FirstOrDefault(f => f.IsPrimary);
            var identityField = keyFields.FirstOrDefault(f => f.IsIdentity);

            if (primaryField is not null)
            {
                // Make sure that primary is present in the list of fields before qualifying to become a qualifier
                if (!fields.ContainsFieldName(primaryField.FieldName))
                {
                    throw new InvalidQualifiersException($"There are no qualifier field objects found for '{tableName}'. Ensure that the " +
                        $"primary field is present at the given fields '{fields.Select(field => field.FieldName).Join(", ")}'.");
                }

                // The primary is present, use it as a default if there are no qualifiers given
                qualifiers = primaryField.AsField().AsEnumerable();
            }
            else
            {
                // Throw exception, qualifiers are not defined
                throw new MissingQualifierFieldsException($"There are no qualifier fields found for '{tableName}'.");
            }
        }

        // Get the insertable and updateable fields
        var insertableFields = fields;
        var updatableFields = EnumerableExtension.AsList(fields.Where(f => qualifiers.GetByFieldName(f.FieldName) is null && noUpdateFields?.GetByFieldName(f.FieldName) is null && keyFields.GetByFieldName(f.FieldName) is not { IsIdentity: true }));

        // Initialize the builder
        var builder = new QueryBuilder();

        bool insertingIdentity = qualifiers.Any(x => keyFields.GetByFieldName(x.FieldName) is { IsIdentity: true }) && fields.Any(f => keyFields.GetByFieldName(f.FieldName) is { IsIdentity: true });

        if (insertingIdentity && SqlServerOptions.Current.UseIdentityInsert)
        {
            builder
                .WriteText("BEGIN TRY")
                .Set()
                .WriteText("IDENTITY_INSERT")
                .TableNameFrom(tableName, DbSetting)
                .On()
                .End(DbSetting)
                .WriteText("END TRY BEGIN CATCH END CATCH");
        }
        else
        {
            insertableFields = fields.Where(f => keyFields.GetByFieldName(f.FieldName) is not { IsIdentity: true }).ToList();
            updatableFields = updatableFields.Where(f => keyFields.GetByFieldName(f.FieldName) is not { IsIdentity: true }).ToList();
        }

        // Iterate the indexes
        // MERGE T USING S
        builder.Merge()
            .TableNameFrom(tableName, DbSetting)
            .HintsFrom(hints)
            .As("T")
            .Using()
            .OpenParen()
            .Values();

        var parameterFields = fields;
        if (!insertingIdentity && keyFields.FirstOrDefault(x => x.IsIdentity) is { } idField)
            parameterFields = fields.Where(f => !string.Equals(f.FieldName, idField.FieldName, StringComparison.OrdinalIgnoreCase)).AsFieldSet();

        for (var index = 0; index < batchSize; index++)
        {
            if (index > 0)
                builder.Comma();

            builder
                .OpenParen()
                .ParametersFrom(parameterFields, index, DbSetting);

            if (batchSize > 1 && keyFields.Any())
            {
                builder
                    .Comma()
                    .WriteText($"{index}");
            }

            builder
                .CloseParen();
        }


        builder
            .CloseParen()
            .As("S")
            .OpenParen()
            .FieldsFrom(parameterFields, DbSetting);

        if (batchSize > 1 && keyFields.Any())
        {
            builder
                .Comma()
                .WriteText(RepoDbOrderColumn.AsField(DbSetting));
        }

        builder
            .CloseParen()
            // QUALIFIERS
            .On()
            .OpenParen()
            .WriteText(qualifiers?
                .Select(
                    field => field.AsJoinQualifier("S", "T", true, DbSetting))
                        .Join(" AND "))
            .CloseParen()
            // WHEN NOT MATCHED THEN INSERT VALUES
            .When()
            .Not()
            .Matched()
            .Then()
            .Insert()
            .OpenParen()
            .FieldsFrom(insertableFields, DbSetting)
            .CloseParen()
            .Values()
            .OpenParen()
            .AsAliasFieldsFrom(insertableFields, "S", DbSetting)
            .CloseParen();

        if (updatableFields.Count > 0)
        {
            builder
                // WHEN MATCHED THEN UPDATE SET
                .When()
                .Matched()
                .Then()
                .Update()
                .Set()
                .FieldsAndAliasFieldsFrom(updatableFields, "T", "S", DbSetting);
        }

        // Set the output
        if (keyFields.Any())
        {
            builder
                .Output()
                .AsAliasFieldsFrom(keyFields, "INSERTED", DbSetting);

            if (batchSize > 1)
            {
                builder
                    .Comma()
                    .WriteText("S.")
                    .WriteQuoted(RepoDbOrderColumn, DbSetting, false);
            }
        }

        // End the builder
        builder.End(DbSetting);

        if (insertingIdentity && SqlServerOptions.Current.UseIdentityInsert)
        {
            builder
                .WriteText("BEGIN TRY")
                .Set()
                .WriteText("IDENTITY_INSERT")
                .TableNameFrom(tableName, DbSetting)
                .Off()
                .End(DbSetting)
                .WriteText("END TRY BEGIN CATCH END CATCH");
        }

        // Return the query
        return builder.ToString();
    }

    #endregion

    /// <inheritdoc />
    public override string CreateQuery(
        string tableName,
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
            .If(offset == 0, b => b.TopFrom(take)) // Prefer `TOP 12` syntax over FETCH NEXT 12 rows only
            .FieldsFrom(fields, DbSetting)
            .From()
            .TableNameFrom(tableName, DbSetting)
            .HintsFrom(hints)
            .WhereFrom(where, DbSetting)
            .OrderByFrom(orderBy, DbSetting)
            .If(offset != 0, b => b.OffsetRowsFetchNextRowsOnly(offset, take))
            .End(DbSetting);

        // Return the query
        return builder.ToString();
    }

    /// <inheritdoc />
    public override string CreateUpdateAll(
    string tableName,
    IEnumerable<Field> fields,
    IEnumerable<Field> qualifiers,
    int batchSize,
    IEnumerable<DbField> keyFields,
    string? hints = null)
    {
        // Use base implementation for batch size 1
        if (batchSize == 1)
        {
            return base.CreateUpdateAll(tableName, fields, qualifiers, batchSize, keyFields, hints);
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);
        GuardHints(hints);

        var primaryField = keyFields.FirstOrDefault(f => f.IsPrimary);

        ValidateMultipleStatementExecution(batchSize);

        if (!fields.Any())
        {
            throw new EmptyException(nameof(fields), "The list of fields cannot be null or empty.");
        }

        var updateFields = fields
            .Where(f => keyFields.GetByFieldName(f.FieldName) is null && qualifiers.GetByFieldName(f.FieldName) is null)
            .ToArray();

        if (updateFields.Length == 0)
        {
            throw new EmptyException(nameof(fields), "The list of updatable fields cannot be null or empty.");
        }

        var builder = new QueryBuilder();

        builder
            .Merge().Into()
            .TableNameFrom(tableName, DbSetting)
            .WriteText("AS T")
            .Using().OpenParen().Values();

        for (var index = 0; index < batchSize; index++)
        {
            if (index > 0)
                builder.Comma();

            builder
                .OpenParen()
                .ParametersFrom(fields, index, DbSetting)
                .CloseParen();
        }

        builder
            .CloseParen()
            .As()
            .WriteText("S")
            .OpenParen()
            .FieldsFrom(fields, DbSetting)
            .CloseParen()
            .On()
            .AppendJoin(qualifiers.Select(q =>
                $"T.{q.FieldName.AsField(DbSetting)} = S.{q.FieldName.AsField(DbSetting)}"), " AND ");

        // No WHEN NOT MATCHED clause: unmatched rows are ignored (no update, no insert)
        builder
            .When().Matched().Then()
            .Update().Set()
            .AppendJoin(updateFields.Select(q =>
                $"T.{q.FieldName.AsField(DbSetting)} = S.{q.FieldName.AsField(DbSetting)}"), ", ")
            .End(DbSetting);

        return builder.ToString();
    }

    /// <inheritdoc />
    public override string? JsonColumnType => "VARCHAR(max)";
    /// <inheritdoc />
    public override string? VectorColumnType => "VECTOR";
    /// <inheritdoc />
    public override string IdentityDefinition => "IDENTITY(1,1)";
}
