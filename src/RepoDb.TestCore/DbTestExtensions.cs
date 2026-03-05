using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json.Nodes;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Resolvers;
using RepoDb.StatementBuilders;

namespace RepoDb.TestCore;

public static class DbTestExtensions
{

    public static string ReplaceForTests(this DbConnection connection, string sqlText)
    {
        if (connection.GetDbSetting() is { } set)
        {
            if (set.OpeningQuote != "[")
                sqlText = sqlText.Replace("[", set.OpeningQuote);
            if (set.ClosingQuote != "]")
                sqlText = sqlText.Replace("]", set.ClosingQuote);
            if (set.ParameterPrefix != "@")
                sqlText = sqlText.Replace("@", set.ParameterPrefix);

        }
        return sqlText;
    }

    public static async Task CreateTableAsync<TEntity>(this DbConnection connection, ITrace? trace = null, CancellationToken cancellationToken=default) where TEntity : class
    {
        var tableName = ClassMappedNameCache.Get<TEntity>();

        await CreateTableAsync<TEntity>(connection, tableName, trace, cancellationToken);
    }

    public static async Task CreateTableAsync<TEntity>(this DbConnection sql, string tableName, ITrace? trace = null, CancellationToken cancellationToken=default) where TEntity : class
    {
        var dbSetting = sql.GetDbSetting();
        var stmt = (BaseStatementBuilder)sql.GetStatementBuilder();
        var toDbField = (stmt.ConvertFieldResolver as DbConvertFieldResolver)?.StringNameResolver ?? FindResolver(sql);
        var cp = PropertyCache.Get<TEntity>();
#if NET
        NullabilityInfoContext ctx = new NullabilityInfoContext();
#endif

        var qb = new QueryBuilder();

        qb.WriteText("CREATE").Table()
            .TableNameFrom(tableName, dbSetting)
            .OpenParen()
            .NewLine();

        var identityKey = IdentityCache.Get<TEntity>();
        var primaryKeys = PrimaryCache.GetPrimaryKeys<TEntity>();

        bool first = true;
        foreach (var prop in cp)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (first)
            {
                first = false;
            }
            else
            {
                qb.Comma()
                    .NewLine();
            }

            var field = prop.AsField();

            qb.WriteQuoted(prop.FieldName, dbSetting)
                .WriteText(" ");

            var type = (field.Type ?? prop.PropertyInfo.PropertyType).GetUnderlyingType();


            string? columnTypeName = null;
            if ((type == typeof(JsonNode) || type == typeof(JsonObject) || type == typeof(JsonArray))
                && sql.GetStatementBuilder() is BaseStatementBuilder bs
                && bs.JsonColumnType is { } jsonColumnType)
            {
                columnTypeName = jsonColumnType;
            }
            else
            {
                var dbType =
                    prop.DbType
                    ?? type.GetDbType()
                    ?? TypeMapCache.Get(type)
                    ?? TypeMapper.Get(type)
                    ?? ClientTypeToDbTypeResolver.Instance.Resolve(type)
                    ?? DbType.AnsiString;

                columnTypeName = toDbField?.Resolve(dbType) ?? "TEXT";
            }

            qb.WriteText(columnTypeName);

            if (type == typeof(string) && !columnTypeName.Contains('(') && !string.Equals(columnTypeName, "text", StringComparison.OrdinalIgnoreCase))
            {
                qb.OpenParen().WriteText("255").CloseParen();
            }

            bool isIdentity = (identityKey?.FieldName == prop.FieldName);

            if (isIdentity && (stmt.PrimaryBeforeIdentity == false))
            {
                qb.WriteText(stmt.IdentityDefinition ?? "IDENTITY");
            }

            if (primaryKeys.OneOrDefault() is { } primaryKey && primaryKey.FieldName == prop.FieldName)
            {
                qb.WriteText("PRIMARY KEY");
                primaryKeys = []; // Handled here. Don't add separate key
            }

            if (isIdentity && (stmt.PrimaryBeforeIdentity == true))
            {
                qb.WriteText(stmt.IdentityDefinition ?? "IDENTITY");
            }

            if (prop.PropertyInfo.PropertyType.IsNullable())
            {
                qb.WriteText("NULL");
            }
            else if (prop.PropertyInfo.PropertyType.IsValueType)
            {
                qb.WriteText("NOT NULL");
            }
#if NET
            else if (ctx.Create(prop.PropertyInfo) is { } nullability)
            {
                qb.WriteText(nullability.ReadState == NullabilityState.Nullable ? "NULL" : "NOT NULL");
            }
#endif
        }

        if (primaryKeys.Any())
        {
            qb.Comma().NewLine()
                .WriteText("CONSTRAINT ")
                .WriteQuoted($"PK_{tableName}", dbSetting)
                .WriteText(" PRIMARY KEY (");
            first = true;
            foreach (var pk in primaryKeys)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    qb.Comma();
                }
                qb.WriteQuoted(pk.FieldName, dbSetting);
            }
            qb.WriteText(")");
        }

        // TODO: Add foreign keys, indexes, etc. if needed

        qb.NewLine().CloseParen();

        Debug.WriteLine(qb.ToString());

        await sql.ExecuteNonQueryAsync(qb.ToString(), trace: trace, cancellationToken: cancellationToken);
    }

    static readonly ConcurrentDictionary<Type, IResolver<DbType, string?>> _resolverCache = new();
    private static IResolver<DbType, string?> FindResolver(DbConnection sql)
    {
        return _resolverCache.GetOrAdd(sql.GetType(), (_) =>
        {
            var asm = sql.GetDbHelper().GetType().Assembly;

            foreach (var t in asm.GetTypes())
            {
                if (typeof(IResolver<DbType, string?>).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                {
                    var inst = (IResolver<DbType, string?>)Activator.CreateInstance(t);
                    if (inst is not null)
                    {
                        return inst;
                    }
                }
            }

            return null;
        });


    }

    public static async Task DropTableAsync<TEntity>(this DbConnection connection, ITrace? trace = null, CancellationToken cancellationToken = default) where TEntity : class
    {
        var tableName = ClassMappedNameCache.Get<TEntity>();

        await DropTableAsync<TEntity>(connection, tableName, trace, cancellationToken: cancellationToken);
    }

    public static async Task DropTableAsync<TEntity>(this DbConnection sql, string tableName, ITrace? trace = null, CancellationToken cancellationToken = default) where TEntity : class
    {
        var dbSetting = sql.GetDbSetting();

        var qb = new QueryBuilder();

        qb.WriteText("DROP").Table()
            .TableNameFrom(tableName, dbSetting);

        await sql.ExecuteNonQueryAsync(qb.ToString(), trace: trace, cancellationToken: cancellationToken);
    }
}
