using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using RepoDb.DbSettings;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Resolvers;

namespace RepoDb.DbHelpers;
public sealed class OracleDbHelper : BaseDbHelper
{
    public OracleDbHelper(IDbSetting dbSetting)
        : base(new OracleDbTypeToClientTypeResolver())
    {
        DbSetting = dbSetting ?? throw new ArgumentNullException(nameof(dbSetting));
    }

    public IDbSetting DbSetting { get; }

    public override void DynamicHandler<TEventInstance>(TEventInstance instance, string key)
    {
        if (key == "RepoDb.Internal.Compiler.Events[AfterCreateDbParameter]" && instance is OracleParameter op)
        {
            HandleDbParameterPostCreation(op);
        }

        void HandleDbParameterPostCreation(OracleParameter oracleParameter)
        {
            if (oracleParameter.Value is string)
            {
                oracleParameter.OracleDbType = OracleDbType.Varchar2;
            }
            else if (oracleParameter.Value is TimeSpan)
            {
                oracleParameter.OracleDbType = OracleDbType.IntervalDS;
            }
        }
    }

    const string GetFieldsQuery = @"
        SELECT 
            C.COLUMN_NAME,
            CASE WHEN PK.COLUMN_NAME IS NOT NULL THEN 1 ELSE 0 END AS IsPrimary,
            CASE WHEN C.IDENTITY_COLUMN = 'YES' THEN 1 ELSE 0 END AS IsIdentity,
            CASE WHEN C.NULLABLE = 'Y' THEN 1 ELSE 0 END AS IsNullable,
            C.DATA_TYPE AS DataType,
    
            -- Return character length for character types, otherwise use byte length
            CASE 
                WHEN C.CHAR_USED = 'C' THEN C.CHAR_LENGTH
                ELSE C.DATA_LENGTH
            END AS ""Size"",

            -- Expose precision and scale for NUMBER, FLOAT, etc.
            C.DATA_PRECISION AS Precision,
            C.DATA_SCALE AS Scale,

            CASE WHEN C.DATA_DEFAULT IS NOT NULL THEN 1 ELSE 0 END AS HasDefaultValue,
            CASE WHEN C.VIRTUAL_COLUMN = 'YES' THEN 1 ELSE 0 END AS IsComputed
        FROM ALL_TAB_COLS C
        LEFT JOIN (
            SELECT CC.OWNER, CC.TABLE_NAME, CC.COLUMN_NAME
            FROM ALL_CONSTRAINTS CONS
            JOIN ALL_CONS_COLUMNS CC 
              ON CONS.CONSTRAINT_NAME = CC.CONSTRAINT_NAME 
             AND CONS.OWNER = CC.OWNER
            WHERE CONS.CONSTRAINT_TYPE = 'P'
        ) PK ON PK.OWNER = C.OWNER 
            AND PK.TABLE_NAME = C.TABLE_NAME 
            AND PK.COLUMN_NAME = C.COLUMN_NAME
        WHERE C.TABLE_NAME = :TableName
            AND C.OWNER = :Schema
ORDER BY C.COLUMN_ID
    ";

    public override IEnumerable<DbField> GetFields(IDbConnection connection, string tableName, IDbTransaction? transaction = null)
    {
        var commandText = GetFieldsQuery;
        var param = new
        {
            Schema = DataEntityExtension.GetSchema(tableName, DbSetting)?.AsUnquoted(DbSetting).ToUpperInvariant(),
            TableName = DataEntityExtension.GetTableName(tableName, DbSetting).AsUnquoted(DbSetting)
        };
        var param2 = string.IsNullOrWhiteSpace(param.Schema) ? (object)new { param.TableName } : null;
        if (param2 is { })
        {
            commandText = commandText.Replace(":Schema", "USER");
        }

        // Iterate and extract
        using var reader = (DbDataReader)connection.ExecuteReader(commandText, param2 ?? param, transaction: transaction);

        var dbFields = new List<DbField>();

        // Iterate the list of the fields
        while (reader.Read())
        {
            dbFields.Add(ReaderToDbField(reader));
        }

        // Return the list of fields
        return dbFields;
    }

    public override async ValueTask<IEnumerable<DbField>> GetFieldsAsync(IDbConnection connection, string tableName, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var commandText = GetFieldsQuery;
        var param = new
        {
            Schema = DataEntityExtension.GetSchema(tableName, DbSetting)?.AsUnquoted(DbSetting).ToUpperInvariant(),
            TableName = DataEntityExtension.GetTableName(tableName, DbSetting).AsUnquoted(DbSetting)
        };
        var param2 = string.IsNullOrWhiteSpace(param.Schema) ? (object)new { param.TableName } : null;
        if (param2 is { })
        {
            commandText = commandText.Replace(":Schema", "USER");
        }

        // Iterate and extract
        using var reader = (DbDataReader)await connection.ExecuteReaderAsync(commandText, param2 ?? param, transaction: transaction,
            cancellationToken: cancellationToken);

        var dbFields = new List<DbField>();

        // Iterate the list of the fields
        while (await reader.ReadAsync(cancellationToken))
        {
            dbFields.Add(ReaderToDbField(reader));
        }

        // Return the list of fields
        return dbFields;
    }

    #region GetSchemaObjects
    const string GetSchemaQuery = @"
        SELECT
            object_type ""Type"",
            object_name ""Name"",
            owner ""Schema"",
            (owner = USER) AS ""IsCurrentUser""
        FROM all_objects
        WHERE object_type IN ('TABLE', 'VIEW')
          AND owner NOT IN (
            'SYS', 'SYSTEM', 'CTXSYS', 'XDB', 'MDSYS', 'ORDSYS', 'WMSYS',
            'EXFSYS', 'DBSNMP', 'APPQOSSYS', 'OUTLN', 'AUDSYS', 'GSMADMIN_INTERNAL',
            'OJVMSYS', 'ANONYMOUS', 'DVSYS', 'DVF', 'REMOTE_SCHEDULER_AGENT',
            'MGMT_VIEW', 'SI_INFORMTN_SCHEMA', 'APEX_PUBLIC_USER', 'FLOWS_FILES',
            'APEX_040000', 'APEX_050000', 'XS$NULL'
)
";

    private DbField ReaderToDbField(DbDataReader reader)
    {
        var dbType = reader.IsDBNull(4) ? "VARCHAR2" : reader.GetString(4);

        return new DbField(
            reader.GetString(0),                                // COLUMN_NAME
            !reader.IsDBNull(1) && reader.GetInt32(1) == 1,    // IsPrimary
            !reader.IsDBNull(2) && reader.GetInt32(2) == 1,    // IsIdentity
            !reader.IsDBNull(3) && reader.GetInt32(3) == 1,    // IsNullable
            DbTypeResolver.Resolve(dbType) ?? typeof(object),
            reader.IsDBNull(5) ? null : reader.GetInt32(5),    // Size
            reader.IsDBNull(6) ? null : (byte)reader.GetInt32(6), // Precision
            reader.IsDBNull(7) ? null : (byte)reader.GetInt32(7), // Scale
            dbType,
            !reader.IsDBNull(8) && reader.GetInt32(8) == 1,    // HasDefaultValue
            !reader.IsDBNull(9) && reader.GetInt32(9) == 1,    // IsComputed
            "ORACLE"
        );
    }

    public override IEnumerable<DbSchemaObject> GetSchemaObjects(IDbConnection connection, IDbTransaction? transaction = null)
    {
        return connection.ExecuteQuery<(string Type, string Name, string Schema, bool IsCurrentUser)>(GetSchemaQuery, transaction)
                         .SelectMany(MapSchemaQueryResult);
    }

    public override async ValueTask<IEnumerable<DbSchemaObject>> GetSchemaObjectsAsync(IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var results = await connection.ExecuteQueryAsync<(string Type, string Name, string Schema, bool IsCurrentUser)>(GetSchemaQuery, transaction, cancellationToken: cancellationToken);
        return results.SelectMany(MapSchemaQueryResult);
    }

    private static IEnumerable<DbSchemaObject> MapSchemaQueryResult((string Type, string Name, string Schema, bool IsCurrentUser) r)
    {
        var rr = new DbSchemaObject
        {
            Type = r.Type switch
            {
                "TABLE" => DbSchemaType.Table,
                "VIEW" => DbSchemaType.View,
                _ => throw new NotSupportedException($"Unsupported schema object type: {r.Type}")
            },
            Name = r.Name,
            Schema = r.Schema
        };

        yield return rr;

        if (r.IsCurrentUser)
            yield return rr with { Schema = null };
    }
    #endregion

    public override object? ParameterValueToDb(object? value) => value switch
    {
#if NET
        DateOnly dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
        TimeOnly to => to.ToTimeSpan(),
#endif
        _ => value,
    };

    public override Func<object?> PrepareForIdentityOutput(DbCommand command)
    {
        int collectionSize = 0;
        if (command.CommandText.StartsWith("/*FORALL*/", StringComparison.Ordinal)
            && command is OracleCommand cmd)
        {
            // We tweak the parameters to support Oracle ForAll optimization
            var last = cmd.Parameters[cmd.Parameters.Count - 1];
            var name = last.ParameterName;
            var n = name.LastIndexOf('_');
            collectionSize = int.Parse(name.Substring(n + 1)) + 1;

            int nFields = cmd.Parameters.Count / collectionSize;

            if (name.StartsWith(":__RepoDb_OrderColumn"))
                nFields--; // Strip ordercolumn

            for (int i = 0; i < nFields; i++)
            {
                var p = (OracleParameter)cmd.Parameters[i];

                var values = Enumerable.Range(0, collectionSize).Select(n => cmd.Parameters[i + n * nFields].Value).ToArray();
                p.Value = values;
            }

            while (cmd.Parameters.Count > nFields)
                cmd.Parameters.RemoveAt(cmd.Parameters.Count - 1);

            cmd.ArrayBindCount = collectionSize;
        }

        if (command.CommandText.EndsWith(":RepoDb_Result", StringComparison.Ordinal))
        {
            var p = new OracleParameter()
            {
                ParameterName = ":RepoDb_Result",
                Direction = ParameterDirection.Output,
                OracleDbType = OracleDbType.Int32
            };

            command.Parameters.Add(p);

            return () =>
            {
                var value = p.Value;

                if (value is OracleDecimal od)
                {
                    return od.ToInt64();
                }
                else if (value is OracleDecimal[] oda)
                {
                    return oda.Select(od => od.ToInt64()).ToArray();
                }

                return value;
            };
        }
        else
        {
            return static () => null;
        }
    }

    private const string QueryVersion = @"SELECT banner FROM v$version";
    private const string QueryProduct = @"SELECT PRODUCT, VERSION FROM PRODUCT_COMPONENT_VERSION WHERE PRODUCT LIKE 'Oracle%'";

    public override DbRuntimeSetting GetDbConnectionRuntimeInformation(IDbConnection connection, IDbTransaction transaction)
    {
        string? banner = null;
        string? productName = null;
        string? productVersion = null;

        // Execute first query
        using (var command = connection.CreateCommand(QueryVersion, transaction: transaction))
        using (var reader = command.ExecuteReader())
        {
            if (reader.Read())
            {
                banner = reader.GetString(0);
            }
        }

        // Execute second query
        using (var command = connection.CreateCommand(QueryProduct, transaction: transaction))
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                var prod = reader.GetString(0); // PRODUCT
                if (prod.StartsWith("Oracle", StringComparison.OrdinalIgnoreCase))
                {
                    productName = prod;
                    productVersion = reader.GetString(1); // VERSION
                    break;
                }
            }
        }

        // Extract version number
        var versionMatch = Regex.Match(productVersion ?? banner ?? "", @"\d+(\.\d+){0,3}");
        var parsedVersion = versionMatch.Success ? Version.Parse(versionMatch.Value) : new Version(0, 0);

        // Determine edition or flavor
        var engineName = productName switch
        {
            var name when name?.Contains("Express", StringComparison.OrdinalIgnoreCase) == true => "Oracle XE",
            var name when name?.Contains("Free", StringComparison.OrdinalIgnoreCase) == true => "Oracle Free",
            var name when name?.Contains("Enterprise", StringComparison.OrdinalIgnoreCase) == true => "Oracle Enterprise",
            _ => "Oracle"
        };

        return new DbRuntimeSetting
        {
            EngineName = engineName,
            EngineVersion = parsedVersion,
            //CompatibilityVersion = null
        };
    }

}
