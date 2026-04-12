using System.Collections;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using RepoDb.Caches;
using RepoDb.DbSettings;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Oracle.Mappings;
using RepoDb.Resolvers;
using OracleINullable = Oracle.ManagedDataAccess.Types.INullable;

namespace RepoDb.DbHelpers;

/// <summary>
/// 
/// </summary>
public sealed class OracleDbHelper : BaseDbHelper
{
    /// <inheritdoc />
    public OracleDbHelper(IDbSetting dbSetting)
        : base(new OracleDbTypeToClientTypeResolver())
    {
        ArgumentNullException.ThrowIfNull(dbSetting);
        DbSetting = dbSetting;
    }

    /// <inheritdoc />
    private IDbSetting DbSetting { get; }

    /// <inheritdoc />
    public override Expression? GetParameterPostCreationExpression(ParameterExpression dbParameterExpression, ParameterExpression? propertyExpression, DbField dbField)
    {
        return Expression.Call(typeof(OracleDbHelper).GetMethod(nameof(HandleDbParameterPostCreation), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!,
                Expression.Convert(dbParameterExpression, typeof(OracleParameter)));
    }

    static void HandleDbParameterPostCreation(OracleParameter oracleParameter)
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

    private const string GetFieldsQuery = @"
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
        ORDER BY C.COLUMN_ID";

    /// <inheritdoc />
    public override DbFieldCollection GetFields(IDbConnection connection, string tableName, IDbTransaction? transaction = null)
    {
        connection.EnsureOpen();
        using var command = (DbCommand)connection.CreateCommand(GetFieldsQuery, transaction: transaction);

        command.Parameters.Add(command.CreateParameter(":TableName", DataEntityExtension.GetTableName(tableName, DbSetting), DbType.String));

        var schema = DataEntityExtension.GetSchema(tableName, DbSetting)?.AsUnquoted(DbSetting);

        if (string.IsNullOrWhiteSpace(schema))
            command.CommandText = command.CommandText.Replace(":Schema", "USER");
        else
            command.Parameters.Add(command.CreateParameter(":Schema", schema!.ToUpperInvariant(), DbType.String));

        // Iterate and extract
        using var reader = command.ExecuteReader();

        var dbFields = new List<DbField>();

        // Iterate the list of the fields
        while (reader.Read())
        {
            dbFields.Add(ReaderToDbField(reader));
        }

        // Return the list of fields
        return new(dbFields);
    }

    /// <inheritdoc />
    public override async ValueTask<DbFieldCollection> GetFieldsAsync(IDbConnection connection, string tableName, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        await connection.EnsureOpenAsync(cancellationToken).ConfigureAwait(false);
#if NET
        await
#endif
        using var command = (DbCommand)connection.CreateCommand(GetFieldsQuery, transaction: transaction);

        command.Parameters.Add(command.CreateParameter(":TableName", DataEntityExtension.GetTableName(tableName, DbSetting), DbType.String));

        var schema = DataEntityExtension.GetSchema(tableName, DbSetting)?.AsUnquoted(DbSetting);

        if (string.IsNullOrWhiteSpace(schema))
            command.CommandText = command.CommandText.Replace(":Schema", "USER");
        else
            command.Parameters.Add(command.CreateParameter(":Schema", schema!.ToUpperInvariant(), DbType.String));


        // Iterate and extract
#if NET
        await
#endif
        using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

        var dbFields = new List<DbField>();

        // Iterate the list of the fields
        while (await reader.ReadAsync(cancellationToken))
        {
            dbFields.Add(ReaderToDbField(reader));
        }

        // Return the list of fields
        return new(dbFields);
    }

    #region GetSchemaObjects
    private const string GetSchemaQuery = @"
        SELECT
            object_type ""Type"",
            object_name ""Name"",
            owner ""Schema"",
            CASE WHEN owner = USER THEN 1 ELSE 0 END AS ""IsCurrentSchema""
        FROM all_objects
        WHERE object_type IN ('TABLE', 'VIEW')
          AND owner NOT IN (
            'SYS', 'SYSTEM', 'CTXSYS', 'XDB', 'MDSYS', 'ORDSYS', 'WMSYS',
            'EXFSYS', 'DBSNMP', 'APPQOSSYS', 'OUTLN', 'AUDSYS', 'GSMADMIN_INTERNAL',
            'OJVMSYS', 'ANONYMOUS', 'DVSYS', 'DVF', 'REMOTE_SCHEDULER_AGENT',
            'MGMT_VIEW', 'SI_INFORMTN_SCHEMA', 'APEX_PUBLIC_USER', 'FLOWS_FILES',
            'APEX_040000', 'APEX_050000', 'XS$NULL', 'LBACSYS'
        ) AND INSTR(owner, '$') = 0
";

    private DbField ReaderToDbField(DbDataReader reader)
    {
        var dbType = reader.IsDBNull(4) ? "VARCHAR2" : reader.GetString(4);
        var type = DbTypeResolver.Resolve(dbType) ?? typeof(object);
        var size = reader.IsDBNull(5) ? null : (int?)reader.GetInt32(5);
        var precision = reader.IsDBNull(6) ? null : (byte?)reader.GetInt32(6);
        var scale = reader.IsDBNull(7) ? null : (byte?)reader.GetInt32(7);

        if (type == typeof(decimal))
        {
            type = precision switch
            {
                <= 9 when scale == 0 => typeof(int),
                <= 19 when scale == 0 => typeof(long),
                _ => type
            };
        }
        else if (type == typeof(string))
        {
            if (size == 1 && string.Equals("CHAR", dbType, StringComparison.OrdinalIgnoreCase))
                type = typeof(char);
        }

        return new DbField(
            reader.GetString(0),                                // COLUMN_NAME
            !reader.IsDBNull(1) && reader.GetInt32(1) == 1,    // IsPrimary
            !reader.IsDBNull(2) && reader.GetInt32(2) == 1,    // IsIdentity
            !reader.IsDBNull(3) && reader.GetInt32(3) == 1,    // IsNullable
            type,
            size,
            precision,
            scale,
            dbType,
            !reader.IsDBNull(8) && reader.GetInt32(8) == 1,    // HasDefaultValue
            !reader.IsDBNull(9) && reader.GetInt32(9) == 1,    // IsComputed
            "ORACLE"
        );
    }

    /// <inheritdoc />
    public override IEnumerable<DbSchemaObject> GetSchemaObjects(IDbConnection connection, IDbTransaction? transaction = null)
    {
        return connection.ExecuteQuery<(string Type, string Name, string Schema, bool IsCurrentSchema)>(GetSchemaQuery, transaction)
                         .SelectMany(MapSchemaQueryResult);
    }

    /// <inheritdoc />
    public override async ValueTask<IEnumerable<DbSchemaObject>> GetSchemaObjectsAsync(IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var results = await connection.ExecuteQueryAsync<(string Type, string Name, string Schema, bool IsCurrentSchema)>(GetSchemaQuery, transaction, cancellationToken: cancellationToken);
        return results.SelectMany(MapSchemaQueryResult);
    }

    private static IEnumerable<DbSchemaObject> MapSchemaQueryResult((string Type, string Name, string Schema, bool IsCurrentSchema) r)
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

        if (r.IsCurrentSchema)
            yield return rr with { Schema = null };
    }
    #endregion

    /// <inheritdoc />
    public override object? ParameterValueToDb(object? value, IDbDataParameter parameter)
    {
        switch (value)
        {
#if NET
            case DateOnly dateOnly:
                return dateOnly.ToDateTime(TimeOnly.MinValue);

            case TimeOnly to:
                return to.ToTimeSpan();
#endif
            case OracleVector vector:
                (parameter as OracleParameter)?.OracleDbType = vector.ProviderType;
                return value;
            case float[] floats:
                (parameter as OracleParameter)?.OracleDbType = OracleDbType.Vector_Float32;
                return new OracleVector(floats);
            case double[] doubles:
                (parameter as OracleParameter)?.OracleDbType = OracleDbType.Vector_Float64;
                return new OracleVector(doubles);
            default:
                return base.ParameterValueToDb(value, parameter);
        }
    }

    /// <inheritdoc />
    public override Func<object?> PrepareForIdentityOutput(DbCommand command, string tableName)
    {
        int collectionSize = 0;
        if (command is OracleCommand cmd)
        {
            cmd.BindByName = true;
            if (cmd.CommandText.StartsWith("/*FORALL*/", StringComparison.Ordinal))
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

            if (cmd.CommandText.EndsWith(":RepoDb_Result", StringComparison.Ordinal))
            {
                if ((cmd.CommandText.StartsWith("/*FORALL*/MERGE ", StringComparison.Ordinal)
                    || cmd.CommandText.StartsWith("MERGE ", StringComparison.Ordinal))
                    && DbRuntimeSettingCache.Get(command.Connection!, command.Transaction) is { } v && v.EngineVersion.Major < 23)
                {
                    // Oracle does not support returning values from a MERGE statement when using array binding prior to version 23.0, so we cannot proceed with this
                    // method. Lets do the next best thing and make the merge 'work'

                    int n = cmd.CommandText.LastIndexOf(" RETURNING ", StringComparison.Ordinal);

                    cmd.CommandText = cmd.CommandText.Substring(0, n);
                    return static () => null;
                }

                // Lets assume this value is already cached... As we already calculated the insert/merge
                var returnField = DbFieldCache.Get(command.Connection!, tableName, command.Transaction).GetReturnColumn();

                var p = new OracleParameter()
                {
                    ParameterName = ":RepoDb_Result",
                    Direction = ParameterDirection.Output,
                    OracleDbType = returnField?.Type switch
                    {
                        null => OracleDbType.Int64,
                        Type t when t == typeof(int) => OracleDbType.Int32,
                        Type t when t == typeof(long) => OracleDbType.Int64,
                        Type t when t == typeof(short) => OracleDbType.Int16,
                        Type t when t == typeof(decimal) => OracleDbType.Decimal,
                        _ => OracleDbType.Varchar2
                    },
                };
                if (p.OracleDbType == OracleDbType.Varchar2)
                    p.ArrayBindSize = Enumerable.Repeat(returnField?.Size ?? 32, cmd.ArrayBindCount).ToArray();

                command.Parameters.Add(p);

                return () =>
                {
                    var value = p.Value;

                    if (value is OracleINullable { IsNull: true })
                        return null;
                    else if (value is OracleDecimal od)
                    {
                        if (od.IsInt)
                            return od.ToInt64();
                        else
                            return od.Value;
                    }
                    else if (value is OracleDecimal[] oda)
                    {
                        return oda.Select(od => od.ToInt64()).ToArray();
                    }

                    return value;
                };
            }
        }
        return static () => null;
    }

    /// <inheritdoc />
    public override void PrepareForBatchOperation(DbCommand command, int count)
    {
        OracleCommand cmd = (OracleCommand)command;
        var commandText = cmd.CommandText;
        cmd.BindByName = true;

        if (commandText is not { })
            return;

        if (count <= 1 || !commandText.StartsWith("/*FORALL*/", StringComparison.Ordinal))
        {
            cmd.ArrayBindCount = 0;
        }
        else
        {
            cmd.ArrayBindCount = count;
            int nFields = cmd.Parameters.Count / count;

            for (int i = 0; i < nFields; i++)
            {
                var p = (OracleParameter)cmd.Parameters[i];

                var values = Enumerable.Range(0, count).Select(n => cmd.Parameters[i + n * nFields].Value).ToArray();
                p.Value = values;
            }

            while (cmd.Parameters.Count > nFields)
                cmd.Parameters.RemoveAt(cmd.Parameters.Count - 1);
        }
    }

    /// <inheritdoc />
    public override void PrepareForExecuteReader(DbCommand command)
    {
        OracleCommand cmd = (OracleCommand)command;
        var commandText = cmd.CommandText;

        if (commandText is not { })
            return;

        if (commandText.StartsWith("/*ASCURSOR:", StringComparison.Ordinal) && commandText.IndexOf("*/", 11, StringComparison.Ordinal) is { } nEnd
            && !cmd.Parameters.Cast<OracleParameter>().Any(x => x.OracleDbType == OracleDbType.RefCursor)
            && int.TryParse(commandText.Substring(11, nEnd - 11), out int nItems))
        {
            cmd.BindByName = true; // Arguments will certainly be out-of-order

            for (int i = 0; i < nItems; i++)
            {
                cmd.Parameters.Add($":c_cur{i}", OracleDbType.RefCursor, ParameterDirection.Output);
            }
        }
    }

    /// <inheritdoc />
    public override bool CanCreateTableParameter(IDbConnection connection, IDbTransaction? transaction, Type? fieldType, IEnumerable values)
    {
        var elementType = fieldType ?? values.GetElementType();

        if (elementType == typeof(int) || elementType == typeof(long) || elementType == typeof(short) || elementType == typeof(uint) || elementType == typeof(ushort) || elementType == typeof(byte) || elementType == typeof(sbyte))
            return true;

        return false;
    }

    /// <inheritdoc />
    public override DbParameter? CreateTableParameter(IDbConnection connection, IDbTransaction? transaction, Type? fieldType, IEnumerable values, string parameterName)
    {
        var elementType = fieldType ?? values.GetElementType();
        var p = new OracleParameter();
        p.OracleDbType = OracleDbType.Object;
        p.UdtTypeName = ODCINumberListFactory.ODCINUMBERLIST;

        var set = values.AsTypedSet();

        var items = new long[set.Count];
        int i = 0;
        foreach (var o in set)
        {
            items[i++] = Converter.ToType<long>(o);
        }

        p.Value = items;

        return p;
    }

    /// <inheritdoc />
    public override string? GetJsonColumnType(DbConnection sql, DbTransaction? transaction)
    {
        if (sql.GetDbRuntimeSetting(transaction) is { } info)
        {
            if (info.EngineVersion.Major >= 21)
                return "JSON";
        }

        return "CLOB";
    }

    private const string QueryVersion = @"SELECT banner FROM v$version";
    private const string QueryProduct = @"SELECT PRODUCT, VERSION FROM PRODUCT_COMPONENT_VERSION WHERE PRODUCT LIKE 'Oracle%'";

    /// <inheritdoc />
    public override DbRuntimeSetting GetDbConnectionRuntimeInformation(IDbConnection connection, IDbTransaction? transaction)
    {
        string? banner = null;
        string? productName = null;
        string? productVersion = null;

        connection.EnsureOpen();

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

    static OracleDbHelper()
    {
        ProviderSpecificTypeTransforms.TryAdd((typeof(ReadOnlyMemory<float>), typeof(OracleVector)),
            (fromExpr) => Expression.New(typeof(OracleVector).GetConstructor(new[] { typeof(float[]) })!, [Expression.Convert(Expression.Call(fromExpr, "ToArray", null, []), typeof(float[]))])
        );
        ProviderSpecificTypeTransforms.TryAdd((typeof(OracleVector), typeof(ReadOnlyMemory<float>)),
            (fromExpr) => Expression.New(typeof(ReadOnlyMemory<float>).GetConstructor(new[] { typeof(float[]) })!, [Expression.Call(fromExpr, typeof(OracleVector).GetMethod(nameof(OracleVector.ToFloatArray))!, [])])
        );
        ProviderSpecificTypeTransforms.TryAdd((typeof(float[]), typeof(ReadOnlyMemory<float>)),
            (fromExpr) => Expression.New(typeof(ReadOnlyMemory<float>).GetConstructor(new[] { typeof(float[]) })!, [fromExpr])
        );
    }
}
