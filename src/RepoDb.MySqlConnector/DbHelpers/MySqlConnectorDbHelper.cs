#nullable enable
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
#if MYSQLPLAIN
using MySql.Data.MySqlClient;
#else
using MySqlConnector;
#endif
using RepoDb.DbSettings;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Resolvers;

namespace RepoDb.DbHelpers;

/// <summary>
/// A helper class for database specially for the direct access. This class is only meant for MySql.
/// </summary>
#if MYSQLPLAIN
public sealed class MySqlDbHelper : BaseDbHelper
#else
public sealed class MySqlConnectorDbHelper : BaseDbHelper
#endif
{
    private readonly IDbSetting m_dbSetting = DbSettingMapper.Get<MySqlConnection>()!;

#if MYSQLPLAIN
    /// <summary>
    /// Creates a new instance of <see cref="MySqlDbHelper"/> class.
    /// </summary>
    public MySqlDbHelper()
        : this(new MySqlDbTypeNameToClientTypeResolver())
    { }
#else
   /// <summary>
    /// Creates a new instance of <see cref="MySqlDbHelper"/> class.
    /// </summary>
    public MySqlConnectorDbHelper()
        : this(new MySqlConnectorDbTypeNameToClientTypeResolver())
    { }
#endif

#if MYSQLPLAIN
    /// <summary>
    /// Creates a new instance of <see cref="MySqlDbHelper"/> class.
    /// </summary>
    /// <param name="dbTypeResolver">The type resolver to be used.</param>
    public MySqlDbHelper(IResolver<string, Type> dbTypeResolver)
        : base(dbTypeResolver)
    {
    }
#else
 /// <summary>
    /// Creates a new instance of <see cref="MySqlDbHelper"/> class.
    /// </summary>
    /// <param name="dbTypeResolver">The type resolver to be used.</param>
    public MySqlConnectorDbHelper(IResolver<string, Type> dbTypeResolver)
        : base(dbTypeResolver)
    {
    }
#endif

    #region Helpers

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private const string FieldCommandText = @"
        SELECT
            COLUMN_NAME AS ColumnName,
            CASE WHEN COLUMN_KEY = 'PRI' THEN 1 ELSE 0 END AS IsPrimary,
            CASE WHEN EXTRA LIKE '%auto_increment%' THEN 1 ELSE 0 END AS IsIdentity,
            CASE WHEN IS_NULLABLE = 'YES' THEN 1 ELSE 0 END AS IsNullable,
            DATA_TYPE AS ColumnType,
            CHARACTER_MAXIMUM_LENGTH AS Size,
            COALESCE(NUMERIC_PRECISION, DATETIME_PRECISION) AS `Precision`,
            NUMERIC_SCALE AS Scale,
            DATA_TYPE AS DatabaseType,
            CASE WHEN COLUMN_DEFAULT IS NOT NULL THEN 1 ELSE 0 END AS HasDefaultValue,
            CASE
                WHEN EXTRA LIKE '%VIRTUAL%' THEN 1
                WHEN EXTRA LIKE '%STORED%' THEN 1
                WHEN EXTRA LIKE '%ON UPDATE%' THEN 1
                ELSE 0
            END AS IsComputed
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_SCHEMA = @TableSchema
            AND TABLE_NAME = @TableName
        ORDER BY ORDINAL_POSITION";

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private static readonly HashSet<string> BlobTypes = new([
        "blob",
        "blobasarray",
        "binary",
        "longtext",
        "mediumtext",
        "longblob",
        "mediumblob",
        "tinyblob",
        "varbinary"
    ], StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    private DbField ReaderToDbField(DbDataReader reader)
    {
        var columnType = reader.GetString(4);
        int? size = BlobTypes.Contains(columnType) ? null : reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5);
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
#if MYSQLPLAIN
            "MYSQL"
#else
            "MYSQLC"
#endif
            );
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
            TableName = DataEntityExtension.GetTableName(tableName, m_dbSetting)
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
    private const string GetSchemaQuery = @"
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

    public override Expression? GetParameterPostCreationExpression(ParameterExpression dbParameterExpression, ParameterExpression? propertyExpression, DbField dbField)
    {
        Expression? expr = null;
        if (string.Equals(dbField.DatabaseType, "TIMESTAMP", StringComparison.OrdinalIgnoreCase))
        {
            // MySQL's TIMESTAMP used to have the special behavior where it defaults to the current timestamp if not provided or when provided NULL, but this is not the
            // case anymore in newer versions of MySQL. This means that if the user tries to insert a DateTime.MinValue or DateTimeOffset.MinValue, it will actually try
            // to insert that value and cause an hard error
            //
            // In the past we could handle this edgecase by just inserting null, to trigger the expected default but in
            // newer versions of MySQL, things are closer to the SQL standard and inserting null will not trigger the default value,
            // but instead will actually try to insert null.
            // To handle this, we need to check if the value is DateTime.MinValue or DateTimeOffset.MinValue and if so, replace it with the current timestamp
            // on the client side. If the clock is properly synchronized this is exactly expected.
            var valueExpression = Expression.Property(dbParameterExpression, "Value");


            // .UtcNow? Probably not because MySQL has magic handling of timestamp
            expr = Expression.IfThenElse(
                Expression.AndAlso(
                    Expression.TypeIs(valueExpression, typeof(DateTime)),
                    Expression.Equal(Expression.Convert(valueExpression, typeof(DateTime)), Expression.Constant(DateTime.MinValue))
                ),
                Expression.Assign(valueExpression, Expression.Convert(Expression.Property(null, typeof(DateTime).GetProperty(nameof(DateTime.Now), BindingFlags.Static | BindingFlags.Public)!), typeof(object))),

                Expression.IfThen(
                    Expression.AndAlso(
                        Expression.TypeIs(valueExpression, typeof(DateTimeOffset)),
                        Expression.Equal(Expression.Convert(valueExpression, typeof(DateTimeOffset)), Expression.Constant(DateTimeOffset.MinValue))
                    ),
                    Expression.Assign(valueExpression, Expression.Convert(Expression.Property(null, typeof(DateTimeOffset).GetProperty(nameof(DateTimeOffset.Now), BindingFlags.Static | BindingFlags.Public)!), typeof(object)))
                )
            );
        }
        return expr;
    }

}
