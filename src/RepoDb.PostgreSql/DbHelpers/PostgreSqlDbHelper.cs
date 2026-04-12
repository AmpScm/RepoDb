using System.Collections;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Npgsql;
using NpgsqlTypes;
using RepoDb.DbSettings;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Resolvers;

namespace RepoDb.DbHelpers;

/// <summary>
/// A helper class for database specially for the direct access. This class is only meant for PostgreSql.
/// </summary>
public sealed class PostgreSqlDbHelper : BaseDbHelper
{
    private readonly IDbSetting m_dbSetting = DbSettingMapper.Get<NpgsqlConnection>()!;

    /// <summary>
    /// Creates a new instance of <see cref="PostgreSqlDbHelper"/> class.
    /// </summary>
    public PostgreSqlDbHelper()
        : this(new PostgreSqlDbTypeNameToClientTypeResolver())
    { }

    /// <summary>
    /// Creates a new instance of <see cref="PostgreSqlDbHelper"/> class.
    /// </summary>
    /// <param name="dbTypeResolver">The type resolver to be used.</param>
    public PostgreSqlDbHelper(IResolver<string, Type> dbTypeResolver)
        : base(dbTypeResolver)
    {
    }

    #region Helpers

    private const string CommandText = @"
                SELECT
                    C.column_name,
                    (PK.column_name IS NOT NULL) AS IsPrimary,
                    (
                        C.is_identity = 'YES'
                        OR POSITION('NEXTVAL' IN UPPER(C.column_default)) >= 1
                    ) AS IsIdentity,
                    CAST(C.is_nullable AS BOOLEAN) AS IsNullable,
                    C.data_type AS DataType,
                    C.character_maximum_length AS Size,
                    COALESCE(C.numeric_precision, C.datetime_precision) AS Precision,
                    C.numeric_scale AS Scale,
                    (C.column_default IS NOT NULL) AS HasDefaultValue,
                    (C.is_generated = 'ALWAYS') AS IsComputed
                FROM information_schema.columns C
                LEFT JOIN (
                    SELECT
                        KU.table_schema,
                        KU.table_name,
                        KU.column_name
                    FROM information_schema.table_constraints TC
                    JOIN information_schema.key_column_usage KU
                         ON KU.constraint_name = TC.constraint_name
                        AND KU.constraint_schema = TC.constraint_schema
                    WHERE TC.constraint_type = 'PRIMARY KEY'
                ) PK
                    ON PK.table_schema = C.table_schema
                   AND PK.table_name  = C.table_name
                   AND PK.column_name = C.column_name
                WHERE C.table_name = @TableName
                  AND C.table_schema = @Schema
                ORDER BY C.ordinal_position;
";

    private DbField ReaderToDbField(DbDataReader reader)
    {
        var dbType = reader.IsDBNull(4) ? "text" : reader.GetString(4);

        return new DbField(reader.GetString(0),
            !reader.IsDBNull(1) && reader.GetBoolean(1),
            !reader.IsDBNull(2) && reader.GetBoolean(2),
            !reader.IsDBNull(3) && reader.GetBoolean(3),
            DbTypeResolver.Resolve(dbType)!,
            reader.IsDBNull(5) ? null : reader.GetInt32(5),
            reader.IsDBNull(6) ? null : (byte)reader.GetInt32(6),
            reader.IsDBNull(7) ? null : (byte)reader.GetInt32(7),
            dbType,
            !reader.IsDBNull(8) && reader.GetBoolean(8),
            !reader.IsDBNull(9) && reader.GetBoolean(9),
            "PGSQL");
    }

    private async Task<DbField> ReaderToDbFieldAsync(DbDataReader reader,
        CancellationToken cancellationToken = default)
    {
        var dbType = await reader.IsDBNullAsync(4, cancellationToken) ? "text" : await reader.GetFieldValueAsync<string>(4, cancellationToken);

        return new DbField(await reader.GetFieldValueAsync<string>(0, cancellationToken),
            !await reader.IsDBNullAsync(1, cancellationToken) && await reader.GetFieldValueAsync<bool>(1, cancellationToken),
            !await reader.IsDBNullAsync(2, cancellationToken) && await reader.GetFieldValueAsync<bool>(2, cancellationToken),
            !await reader.IsDBNullAsync(3, cancellationToken) && await reader.GetFieldValueAsync<bool>(3, cancellationToken),
            DbTypeResolver.Resolve(dbType)!,
            await reader.IsDBNullAsync(5, cancellationToken) ? null : await reader.GetFieldValueAsync<int>(5, cancellationToken),
            await reader.IsDBNullAsync(6, cancellationToken) ? null : (byte?)await reader.GetFieldValueAsync<int>(6, cancellationToken),
            await reader.IsDBNullAsync(7, cancellationToken) ? null : (byte?)await reader.GetFieldValueAsync<int>(7, cancellationToken),
            dbType,
            !await reader.IsDBNullAsync(8, cancellationToken) && await reader.GetFieldValueAsync<bool>(8, cancellationToken),
            !await reader.IsDBNullAsync(9, cancellationToken) && await reader.GetFieldValueAsync<bool>(9, cancellationToken),
            "PGSQL");
    }

    #endregion

    #region Methods
    #region GetFields

    /// <summary>
    /// Gets the list of <see cref="DbField"/> of the table.
    /// </summary>
    /// <param name="connection">The instance of the connection object.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="transaction">The transaction object that is currently in used.</param>
    /// <returns>A list of <see cref="DbField"/> of the target table.</returns>
    public override DbFieldCollection GetFields(IDbConnection connection, string tableName, IDbTransaction? transaction = null)
    {
        // Variables
        var param = new
        {
            Schema = DataEntityExtension.GetSchema(tableName, m_dbSetting),
            TableName = DataEntityExtension.GetTableName(tableName, m_dbSetting)
        };

        // Iterate and extract
        using var reader = (DbDataReader)connection.ExecuteReader(CommandText, param, transaction: transaction);

        var dbFields = new List<DbField>();

        // Iterate the list of the fields
        while (reader.Read())
        {
            dbFields.Add(ReaderToDbField(reader));
        }

        // Return the list of fields
        return new(dbFields);
    }

    /// <summary>
    /// Gets the list of <see cref="DbField"/> of the table in an asynchronous way.
    /// </summary>
    /// <param name="connection">The instance of the connection object.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="transaction">The transaction object that is currently in used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>A list of <see cref="DbField"/> of the target table.</returns>
    public override async ValueTask<DbFieldCollection> GetFieldsAsync(IDbConnection connection, string tableName, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        // Variables
        var param = new
        {
            Schema = DataEntityExtension.GetSchema(tableName, m_dbSetting)?.AsUnquoted(m_dbSetting),
            TableName = DataEntityExtension.GetTableName(tableName, m_dbSetting).AsUnquoted(m_dbSetting)
        };

        // Iterate and extract
        using var reader = (DbDataReader)await connection.ExecuteReaderAsync(CommandText, param, transaction: transaction,
            cancellationToken: cancellationToken);

        var dbFields = new List<DbField>();

        // Iterate the list of the fields
        while (await reader.ReadAsync(cancellationToken))
        {
            dbFields.Add(await ReaderToDbFieldAsync(reader, cancellationToken));
        }

        // Return the list of fields
        return new(dbFields);
    }

    #endregion

    #region GetSchemaObjects
    private const string GetSchemaQuery = @"
        SELECT
            table_type AS Type,
            table_name AS Name,
            table_schema AS Schema
        FROM information_schema.tables
        WHERE table_schema NOT IN ('pg_catalog', 'information_schema')";

    /// <inheritdoc />
    public override IEnumerable<DbSchemaObject> GetSchemaObjects(IDbConnection connection, IDbTransaction? transaction = null)
    {
        return connection.ExecuteQuery<(string Type, string Name, string Schema)>(GetSchemaQuery, transaction)
                         .Select(MapSchemaQueryResult);
    }

    /// <inheritdoc />
    public override async ValueTask<IEnumerable<DbSchemaObject>> GetSchemaObjectsAsync(IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var results = await connection.ExecuteQueryAsync<(string Type, string Name, string Schema)>(GetSchemaQuery, transaction, cancellationToken: cancellationToken);
        return results.Select(MapSchemaQueryResult);
    }

    private static DbSchemaObject MapSchemaQueryResult((string Type, string Name, string Schema) r) =>
        new DbSchemaObject
        {
            Type = r.Type switch
            {
                "BASE TABLE" => DbSchemaType.Table,
                "VIEW" => DbSchemaType.View,
                _ => throw new NotSupportedException($"Unsupported schema object type: {r.Type}")
            },
            Name = r.Name,
            Schema = r.Schema
        };
    #endregion

    #region DynamicHandler

    /// <inheritdoc />
    public override Expression? GetParameterPostCreationExpression(ParameterExpression dbParameterExpression, ParameterExpression? propertyExpression, DbField dbField)
    {
        // Shortcut the DynamicHandler to allow inlining
        return Expression.Call(typeof(PostgreSqlDbHelper).GetMethod(nameof(HandleDbParameterPostCreation), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!,
                Expression.Convert(dbParameterExpression, typeof(NpgsqlParameter)));
    }

    #region Handlers

    private static void HandleDbParameterPostCreation(NpgsqlParameter parameter)
    {
        if (parameter.Value is Enum)
        {
            parameter.DbType = DbType.Object;
        }
        else if (parameter.Value is JsonNode jn)
        {
            parameter.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Json;
            parameter.Value = jn.ToJsonString(Converter.JsonSerializerOptions);
        }
        else if (parameter.Value is IDbJsonValue jv)
        {
            parameter.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Json;
            parameter.Value = jv.JsonNode?.ToJsonString(Converter.JsonSerializerOptions);
        }
    }

    /// <inheritdoc />
    public override object? ParameterValueToDb(object? value, IDbDataParameter parameter)
    {
        if (parameter is NpgsqlParameter np)
        {
            if (value is IDbJsonValue jv)
            {
                np.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Json;
                return jv.JsonNode?.ToJsonString(Converter.JsonSerializerOptions);
            }
            else if (value is JsonNode jn)
            {
                np.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Json;
                return jn.ToJsonString(Converter.JsonSerializerOptions);
            }
            else if (MaybeUpdateNpgsqlParameterCallback?.Invoke(ref value, np) == true)
                return value;
#if NET
            else if (value is Half h)
                return (float)h; // Npgsql doesn't support Half, so convert to float
#endif
            else if (value is IFormattable)
                return value; // Don't fall through for this case
        }

        return base.ParameterValueToDb(value, parameter);
    }

    #endregion

    #endregion

    #endregion

    /// <inheritdoc/>
    public override bool CanCreateTableParameter(IDbConnection connection, IDbTransaction? transaction, Type? fieldType, IEnumerable values)
    {
        var elementType = fieldType ?? values.GetElementType();

        return elementType.GetUnderlyingType() is { IsPrimitive: true } && ClientTypeToNpgsqlDbTypeResolver.Instance.Resolve(elementType) is { };
    }

    /// <inheritdoc/>
    public override DbParameter? CreateTableParameter(IDbConnection connection, IDbTransaction? transaction, Type? fieldType, IEnumerable values, string parameterName)
    {
        var elementType = (fieldType ?? values.GetElementType()).GetUnderlyingType();
        var set = values.AsTypedSet();
        var npgType = ClientTypeToNpgsqlDbTypeResolver.Instance.Resolve(elementType)!.Value | NpgsqlDbType.Array;

        Array items = Array.CreateInstance(elementType, set.Count);
        set.CopyTo(items, 0);

        return new NpgsqlParameter(parameterName, npgType)
        {
            Value = items
        };
    }

    private const string PostgresRuntimeInfoQuery = @"
        SHOW server_version;
        SELECT version();
";

    /// <inheritdoc />
    public override DbRuntimeSetting GetDbConnectionRuntimeInformation(IDbConnection connection, IDbTransaction? transaction)
    {
        using var rdr = (NpgsqlDataReader)connection.ExecuteReader(PostgresRuntimeInfoQuery, transaction: transaction);

        string? serverVersion = null;
        string? fullVersion = null;

        if (rdr.Read())
        {
            serverVersion = rdr.GetString(0);
        }

        if (rdr.NextResult() && rdr.Read())
        {
            fullVersion = rdr.GetString(0);
        }

        var versionMatch = Regex.Match(serverVersion ?? "", @"\d+(\.\d+)+");
        var parsedVersion = versionMatch.Success
            ? Version.Parse(versionMatch.Value)
            : new Version(0, 0);

        var engineName = "PostgreSQL";

        // Optional: try to detect a fork (e.g., Amazon Aurora, EDB) from full version string
        if (fullVersion is { } fv)
        {
            if (fv.Contains("Aurora", StringComparison.OrdinalIgnoreCase))
                engineName = "Aurora PostgreSQL";
            if (fv.Contains("EDB", StringComparison.OrdinalIgnoreCase))
                engineName = "EDB PostgreSQL";
        }

        return new()
        {
            EngineName = engineName,
            EngineVersion = parsedVersion,
            //CompatibilityVersion = null, // PostgreSQL has no compatibility levels
            ParameterTypeMap = null // No TVPs
        };
    }



    // For access by PostgreSql.Vectors

    internal delegate bool MaybeUpdateNpgsqlParameter(ref object? value, NpgsqlParameter parameter);
    internal static new ConcurrentDictionary<(Type fromType, Type toType), Func<Expression, Expression?>> ProviderSpecificTypeTransforms => BaseDbHelper.ProviderSpecificTypeTransforms;
    internal static MaybeUpdateNpgsqlParameter? MaybeUpdateNpgsqlParameterCallback;
}
