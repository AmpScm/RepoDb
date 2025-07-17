using System.Collections;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using RepoDb.DbSettings;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Resolvers;

namespace RepoDb.DbHelpers;

/// <summary>
/// A helper class for database specially for the direct access. This class is only meant for SQL Server.
/// </summary>
public sealed class SqlServerDbHelper : BaseDbHelper
{
    /// <summary>
    /// Creates a new instance of <see cref="SqlServerDbHelper"/> class.
    /// </summary>
    public SqlServerDbHelper()
        : this(new SqlServerDbTypeNameToClientTypeResolver())
    { }

    /// <summary>
    /// Creates a new instance of <see cref="SqlServerDbHelper"/> class.
    /// </summary>
    /// <param name="dbTypeResolver">The type resolver to be used.</param>
    public SqlServerDbHelper(IResolver<string, Type> dbTypeResolver)
        : base(dbTypeResolver)
    { }

    #region Helpers

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    private const string FieldInfoCommandText = @"
        SELECT C.COLUMN_NAME AS ColumnName
            , CONVERT(BIT, COALESCE(TC.is_primary, 0)) AS IsPrimary
            , CONVERT(BIT, COALESCE(TMP.is_identity, 1)) AS IsIdentity
            , CONVERT(BIT, COALESCE(TMP.is_nullable, 1)) AS IsNullable
            , C.DATA_TYPE AS DataType
            , COALESCE(C.CHARACTER_MAXIMUM_LENGTH, TMP.max_length)  AS Size
            , CONVERT(TINYINT, COALESCE(TMP.precision, 1)) AS Precision
            , CONVERT(TINYINT, COALESCE(TMP.scale, 1)) AS Scale
            , CONVERT(BIT, IIF(C.COLUMN_DEFAULT IS NOT NULL, 1, 0)) AS DefaultValue
            , CONVERT(BIT, COALESCE(TMP.is_computed, 0)) AS IsComputed
        FROM INFORMATION_SCHEMA.COLUMNS C
        OUTER APPLY
        (
            SELECT 1 AS is_primary
            FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU
            LEFT JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC
                ON TC.TABLE_SCHEMA = C.TABLE_SCHEMA
                AND TC.TABLE_NAME = C.TABLE_NAME
                AND TC.CONSTRAINT_NAME = KCU.CONSTRAINT_NAME
            WHERE KCU.TABLE_SCHEMA = C.TABLE_SCHEMA
                AND KCU.TABLE_NAME = C.TABLE_NAME
                AND KCU.COLUMN_NAME = C.COLUMN_NAME
                AND TC.CONSTRAINT_TYPE = 'PRIMARY KEY'
        ) TC
        OUTER APPLY
        (
            SELECT SC.name
                , SC.is_identity
                , SC.is_nullable
                ,  SC.max_length
                , SC.scale
                , SC.precision
                , SC.is_computed
            FROM [sys].[columns] SC
            INNER JOIN [sys].[tables] ST ON ST.object_id = SC.object_id
            INNER JOIN [sys].[schemas] S ON S.schema_id = ST.schema_id
            WHERE SC.name = C.COLUMN_NAME
                AND ST.name = C.TABLE_NAME
                AND S.name = C.TABLE_SCHEMA
        ) TMP
        WHERE
            C.TABLE_SCHEMA = @Schema
            AND C.TABLE_NAME = @TableName;";

    /// <summary>
    ///
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    private DbField ReaderToDbField(DbDataReader reader)
    {
        return new DbField(reader.GetString(0),
            !reader.IsDBNull(1) && reader.GetBoolean(1),
            !reader.IsDBNull(2) && reader.GetBoolean(2),
            !reader.IsDBNull(3) && reader.GetBoolean(3),
            reader.IsDBNull(4) ? DbTypeResolver.Resolve("text") : DbTypeResolver.Resolve(reader.GetString(4)),
            reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
            reader.IsDBNull(6) ? (byte?)0 : reader.GetByte(6),
            reader.IsDBNull(7) ? (byte?)0 : reader.GetByte(7),
            reader.IsDBNull(7) ? "text" : reader.GetString(4),
            !reader.IsDBNull(8) && reader.GetBoolean(8),
            !reader.IsDBNull(9) && reader.GetBoolean(9),
            "MSSQL");
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<DbField> ReaderToDbFieldAsync(DbDataReader reader,
        CancellationToken cancellationToken = default)
    {
        return new DbField(await reader.GetFieldValueAsync<string>(0, cancellationToken),
            !await reader.IsDBNullAsync(1, cancellationToken) && await reader.GetFieldValueAsync<bool>(1, cancellationToken),
            !await reader.IsDBNullAsync(2, cancellationToken) && await reader.GetFieldValueAsync<bool>(2, cancellationToken),
            !await reader.IsDBNullAsync(3, cancellationToken) && await reader.GetFieldValueAsync<bool>(3, cancellationToken),
            await reader.IsDBNullAsync(4, cancellationToken) ? DbTypeResolver.Resolve("text") : DbTypeResolver.Resolve(await reader.GetFieldValueAsync<string>(4, cancellationToken)),
            await reader.IsDBNullAsync(5, cancellationToken) ? 0 : await reader.GetFieldValueAsync<int>(5, cancellationToken),
            await reader.IsDBNullAsync(6, cancellationToken) ? (byte?)0 : await reader.GetFieldValueAsync<byte>(6, cancellationToken),
            await reader.IsDBNullAsync(7, cancellationToken) ? (byte?)0 : await reader.GetFieldValueAsync<byte>(7, cancellationToken),
            await reader.IsDBNullAsync(7, cancellationToken) ? "text" : await reader.GetFieldValueAsync<string>(4, cancellationToken),
            !await reader.IsDBNullAsync(8, cancellationToken) && await reader.GetFieldValueAsync<bool>(8, cancellationToken),
            !await reader.IsDBNullAsync(9, cancellationToken) && await reader.GetFieldValueAsync<bool>(9, cancellationToken),
            "MSSQL");
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
    public override DbFieldCollection GetFields(IDbConnection connection,
        string tableName,
        IDbTransaction? transaction = null)
    {
        // Variables
        var commandText = FieldInfoCommandText;
        var setting = connection.GetDbSetting();
        var param = new
        {
            Schema = DataEntityExtension.GetSchema(tableName, setting),
            TableName = DataEntityExtension.GetTableName(tableName, setting)
        };

        // Iterate and extract
        using var reader = (DbDataReader)connection.ExecuteReader(commandText, param, transaction: transaction);

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
    public override async ValueTask<DbFieldCollection> GetFieldsAsync(IDbConnection connection,
        string tableName,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default)
    {
        // Variables
        var commandText = FieldInfoCommandText;
        var setting = connection.GetDbSetting();
        var param = new
        {
            Schema = DataEntityExtension.GetSchema(tableName, setting),
            TableName = DataEntityExtension.GetTableName(tableName, setting)
        };

        // Iterate and extract
        using var reader = (DbDataReader)await connection.ExecuteReaderAsync(commandText, param,
            transaction: transaction, cancellationToken: cancellationToken);

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
    const string GetSchemaQuery = @"
        SELECT
            o.type AS [Type],
            o.name AS [Name],
            s.name AS [Schema]
        FROM sys.objects o
        JOIN sys.schemas s ON o.schema_id = s.schema_id
        WHERE o.type IN ('U', 'V') AND is_ms_shipped = 0";

    public override IEnumerable<DbSchemaObject> GetSchemaObjects(IDbConnection connection, IDbTransaction? transaction = null)
    {
        return connection.ExecuteQuery<(string Type, string Name, string Schema)>(GetSchemaQuery, transaction)
                         .Select(MapSchemaQueryResult);
    }

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
                "U" or "U " => DbSchemaType.Table,
                "V" or "V " => DbSchemaType.View,
                _ => throw new NotSupportedException($"Unsupported schema object type: {r.Type}")
            },
            Name = r.Name,
            Schema = r.Schema
        };
    #endregion

    #endregion

    private const string DbRuntimeInfoQuery = @"
    SELECT
        SERVERPROPERTY('ProductVersion') AS SqlServerVersion,
        compatibility_level AS CompatibilityLevel
    FROM
        sys.databases
    WHERE
        name = DB_NAME();

    WITH TypeCandidates AS (
        SELECT
            tt.name AS TVPName,
            s.name AS SchemaName,
            t.name AS ColumnType,
            c.name AS ColumnName,
            c.max_length,
            c.is_nullable,
            COALESCE(i.is_primary_key, 0) AS IsPrimaryKey,
            COALESCE(i.ignore_dup_key, 0) AS IgnoreDupKey,
            ROW_NUMBER() OVER (
                PARTITION BY t.name, c.is_nullable
                ORDER BY
                    -- Prefer TVPs with a PK
                    CASE WHEN i.is_primary_key = 1 THEN 0 ELSE 1 END,
                    -- Then prefer varchar with longer length
                    CASE
                        WHEN t.name = 'varchar' THEN c.max_length
                        ELSE 0
                    END DESC
            ) AS rn
        FROM
            sys.table_types tt
        JOIN
            sys.schemas s ON tt.schema_id = s.schema_id
        JOIN
            sys.columns c ON c.object_id = tt.type_table_object_id
        JOIN
            sys.types t ON c.user_type_id = t.user_type_id
        LEFT JOIN
            sys.index_columns ic ON ic.object_id = tt.type_table_object_id AND ic.column_id = c.column_id
        LEFT JOIN
            sys.indexes i ON i.object_id = tt.type_table_object_id AND i.index_id = ic.index_id AND i.is_primary_key = 1
        WHERE
            (SELECT COUNT(*)
             FROM sys.columns c2
             WHERE c2.object_id = tt.type_table_object_id) = 1
            AND t.name IN ('int', 'bigint', 'tinyint', 'varchar', 'uniqueidentifier', 'bit')
    )
    SELECT
        ColumnType,
        TVPName,
        SchemaName,
        ColumnName,
        is_nullable,
        IsPrimaryKey,
        IgnoreDupKey
    FROM
        TypeCandidates
    WHERE
        rn = 1
    ORDER BY
        ColumnType, is_nullable DESC;";

    public override DbRuntimeSetting GetDbConnectionRuntimeInformation(IDbConnection connection, IDbTransaction transaction)
    {
        using var rdr = (SqlDataReader)connection.ExecuteReader(DbRuntimeInfoQuery, transaction: transaction);

        var ver = rdr.Read() ? new
        {
            serverVersion = rdr.GetString(0),
            compatibilityLevel = rdr.GetByte(1),
        } : null;

        Dictionary<Type, DbDataParameterTypeMap>? typeMap = new();
        if (rdr.NextResult())
        {
            while (rdr.Read())
            {
                var info = new
                {
                    ColumnType = rdr.GetString(0),
                    TVPName = rdr.GetString(1),
                    SchemaName = rdr.GetString(2),
                    ColumnName = rdr.GetString(3),
                    IsNullable = rdr.GetBoolean(4),
                    RequiresDistinct = rdr.GetInt32(5) != 0 && !(rdr.GetInt32(6) != 0),
                };

                var type = DbTypeResolver.Resolve(info.ColumnType);

                if (type.IsValueType && info.IsNullable)
                {
                    var nullableType = typeof(Nullable<>).MakeGenericType(type);

                    typeMap[nullableType] = new(nullableType, info.TVPName, info.SchemaName, info.ColumnName, !info.IsNullable, info.RequiresDistinct);

                    if (!typeMap.ContainsKey(type))
                    {
                        typeMap[type] = new(nullableType, info.TVPName, info.SchemaName, info.ColumnName, !info.IsNullable, info.RequiresDistinct);
                    }
                }
                else
                {
                    typeMap[type] = new(type, info.TVPName, info.SchemaName, info.ColumnName, !info.IsNullable, info.RequiresDistinct);
                }
            }
        }

        return new()
        {
            EngineName = "MSSQL",
            EngineVersion = Version.Parse(Regex.Replace(ver.serverVersion, "^.*?([0-9]+(\\.[0-9]+)*).*?$", "$1") ?? "0.0"),
            CompatibilityVersion = ver.compatibilityLevel is { } c ? new(c / 10, c % 10) : default,
            ParameterTypeMap = typeMap
#if NET
            .AsReadOnly()
#endif
        };
    }

    public override DbParameter? CreateTableParameter(IDbConnection connection, IDbTransaction? transaction, Type? fieldType, IEnumerable values, string parameterName)
    {
        var info = DbRuntimeSettingCache.Get(connection, transaction);

        if (info?.ParameterTypeMap is { } pm
            && (fieldType ?? values.GetElementType()) is { } elementType
            && pm.TryGetValue(elementType, out var mapping))
        {
            var dt = new DataTable();
            dt.Columns.Add(mapping.ColumnName, elementType.GetUnderlyingType());

            foreach (var v in values.AsTypedSet(mapping.RequiresDistinct))
            {
                if (v is null && mapping.NoNull)
                    continue;

                dt.Rows.Add(v);
            }

            var p = new SqlParameter(parameterName, SqlDbType.Structured)
            {
                Value = dt,
                TypeName = $"{mapping.Schema}.{mapping.SchemaObject}"
            };

            return p;
        }

        return null;
    }

    public override bool CanCreateTableParameter(IDbConnection connection, IDbTransaction? transaction, Type? fieldType, IEnumerable values)
    {
        var info = DbRuntimeSettingCache.Get(connection, transaction);

        return info?.ParameterTypeMap is { } pm
            && (fieldType ?? values.GetElementType()) is { } elementType
            && pm.TryGetValue(elementType, out _);
    }

    public override string? CreateTableParameterText(IDbConnection connection, IDbTransaction? transaction, Type? fieldType, string parameterName, IEnumerable values)
    {
        var info = DbRuntimeSettingCache.Get(connection, transaction);

        if (info?.ParameterTypeMap is { } pm
            && (fieldType ?? values.GetElementType()) is { } elementType
            && pm.TryGetValue(elementType, out _))
        {
            return $"SELECT * FROM {parameterName}";
        }

        return null;
    }
}
