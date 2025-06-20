#nullable enable
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using RepoDb.DbSettings;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Resolvers;

namespace RepoDb.DbHelpers;

/// <summary>
/// A helper class for database specially for the direct access. This class is only meant for MySql.
/// </summary>
public sealed class MySqlDbHelper : BaseDbHelper
{
    private readonly IDbSetting m_dbSetting = DbSettingMapper.Get<MySqlConnection>()!;

    /// <summary>
    /// Creates a new instance of <see cref="MySqlDbHelper"/> class.
    /// </summary>
    public MySqlDbHelper()
        : this(new MySqlDbTypeNameToClientTypeResolver())
    { }

    /// <summary>
    /// Creates a new instance of <see cref="MySqlDbHelper"/> class.
    /// </summary>
    /// <param name="dbTypeResolver">The type resolver to be used.</param>
    public MySqlDbHelper(IResolver<string, Type> dbTypeResolver)
        : base(dbTypeResolver)
    {
    }

    #region Helpers

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private const string FieldCommandText = @"
        SELECT
            c.COLUMN_NAME AS ColumnName,
            CASE WHEN kcu.COLUMN_NAME IS NOT NULL THEN 1 ELSE 0 END AS IsPrimary,
            CASE 
                WHEN c.EXTRA = 'auto_increment' AND c.DATA_TYPE IN ('int', 'bigint', 'smallint', 'tinyint', 'mediumint') THEN 1 
                ELSE 0 
            END AS IsIdentity,
            CASE WHEN c.IS_NULLABLE = 'YES' THEN 1 ELSE 0 END AS IsNullable,
            c.DATA_TYPE AS ColumnType,
            c.CHARACTER_MAXIMUM_LENGTH AS Size,
            COALESCE(c.NUMERIC_PRECISION, c.DATETIME_PRECISION) AS `Precision`,
            c.NUMERIC_SCALE AS Scale,
            c.DATA_TYPE AS DatabaseType,
            CASE WHEN c.COLUMN_DEFAULT IS NOT NULL THEN 1 ELSE 0 END AS HasDefaultValue,
            CASE WHEN c.EXTRA LIKE '%VIRTUAL%' OR c.EXTRA LIKE '%STORED%' THEN 1 ELSE 0 END AS IsComputed
        FROM INFORMATION_SCHEMA.COLUMNS c
        LEFT JOIN INFORMATION_SCHEMA.TABLES t
            ON t.TABLE_SCHEMA = c.TABLE_SCHEMA AND t.TABLE_NAME = c.TABLE_NAME
        LEFT JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu
            ON kcu.TABLE_SCHEMA = c.TABLE_SCHEMA
            AND kcu.TABLE_NAME = c.TABLE_NAME
            AND kcu.COLUMN_NAME = c.COLUMN_NAME
            AND kcu.CONSTRAINT_NAME = 'PRIMARY'
        WHERE c.TABLE_SCHEMA = @TableSchema
            AND c.TABLE_NAME = @TableName
            AND t.TABLE_TYPE = 'BASE TABLE'
        ORDER BY c.ORDINAL_POSITION;";

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private HashSet<string> GetBlobTypes()
    {
        return
        [
            "blob",
            "blobasarray",
            "binary",
            "longtext",
            "mediumtext",
            "longblob",
            "mediumblob",
            "tinyblob",
            "varbinary"
        ];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    private DbField ReaderToDbField(DbDataReader reader)
    {
        var columnType = reader.GetString(4);
        var excluded = GetBlobTypes();
        int? size;
        if (excluded.Contains(columnType.ToLowerInvariant()))
        {
            size = null;
        }
        else
        {
            size = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5);
        }
        return new DbField(reader.GetString(0),
            reader.GetBoolean(1),
            reader.GetBoolean(2),
            reader.GetBoolean(3),
            DbTypeResolver.Resolve(columnType)!,
            size,
            reader.IsDBNull(6) ? null : byte.Parse(reader.GetInt32(6).ToString()),
            reader.IsDBNull(7) ? null : byte.Parse(reader.GetInt32(7).ToString()),
            reader.GetString(8),
            reader.GetBoolean(9),
            reader.GetBoolean(10),
            "MYSQL");
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
        var commandText = FieldCommandText;
        var param = new
        {
            TableSchema = connection.Database,
            TableName = DataEntityExtension.GetTableName(tableName, m_dbSetting).AsUnquoted(m_dbSetting)
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
        cancellationToken.ThrowIfCancellationRequested();

        // Variables
        var commandText = FieldCommandText;
        var param = new
        {
            TableSchema = connection.Database,
            TableName = DataEntityExtension.GetTableName(tableName, m_dbSetting).AsUnquoted(m_dbSetting)
        };

        // Iterate and extract
        using var reader = (DbDataReader)await connection.ExecuteReaderAsync(commandText, param, transaction: transaction,
            cancellationToken: cancellationToken);

        var dbFields = new List<DbField>();

        // Iterate the list of the fields
        while (await reader.ReadAsync(cancellationToken))
        {
            // The 'ReaderToDbFieldAsync' is having a bad behavior on different versions
            // of MySQL for this driver (from Oracle). Also, the 'CAST' and 'CONVERT' is
            // not working on our DEVENV.
            // dbFields.Add(await ReaderToDbFieldAsync(reader, cancellationToken));
            dbFields.Add(ReaderToDbField(reader));
        }

        // Return the list of fields
        return new(dbFields);
    }

    #endregion

    #region GetSchemaObjects
    const string GetSchemaQuery = @"
        SELECT
            table_type AS `Type`,
            table_name AS `Name`,
            table_schema AS `Schema`
        FROM information_schema.tables
        WHERE table_schema = DATABASE()";

    public override IEnumerable<DbSchemaObject> GetSchemaObjects(IDbConnection connection, IDbTransaction? transaction = null)
    {
        return connection.ExecuteQuery<(string Type, string Name, string Schema)>(GetSchemaQuery, transaction)
                         .Select(MapSchemaQueryResult);
    }

    public override async ValueTask<IEnumerable<DbSchemaObject>> GetSchemaObjectsAsync(IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var results = await connection.ExecuteQueryAsync<(string Type, string Name, string Schema)>(GetSchemaQuery, transaction);
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

    #endregion

    private const string MySqlRuntimeInfoQuery = @"
    SELECT VERSION() AS Version;
    SHOW VARIABLES LIKE 'version_comment';
";

    public override DbRuntimeSetting GetDbConnectionRuntimeInformation(IDbConnection connection, IDbTransaction? transaction)
    {
        using var rdr = (MySqlDataReader)connection.ExecuteReader(MySqlRuntimeInfoQuery, transaction: transaction);

        string? versionString = null;
        string? versionComment = null;

        if (rdr.Read())
        {
            versionString = rdr.GetString(0);
        }

        if (rdr.NextResult() && rdr.Read())
        {
            versionComment = rdr.GetString(1); // second column = 'Value' from SHOW VARIABLES LIKE ...
        }

        var engineName = versionComment?.Contains("MariaDB", StringComparison.OrdinalIgnoreCase) == true
            ? "MariaDB"
            : "MySQL";

        var versionMatch = Regex.Match(versionString ?? "", @"\d+(\.\d+)+");
        var parsedVersion = versionMatch.Success
            ? Version.Parse(versionMatch.Value)
            : new Version(0, 0);

        return new()
        {
            EngineName = engineName,
            EngineVersion = parsedVersion,
            //CompatibilityVersion = null, // Not really applicable for MySQL
            ParameterTypeMap = null // No TVPs
        };
    }
}
