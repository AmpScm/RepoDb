using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Data.SqlClient;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.IntegrationTests;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using RepoDb.Trace;

namespace RepoDb.IntegrationTests.Operations;

[TestClass]
public class CallThemAllTests : TestBase
{
    protected override void InitializeCore()
    {
        base.InitializeCore();
        Database.Cleanup();

        using var db = CreateOpenConnection();
        db.Insert(new IdentityTable());
    }
    public static IEnumerable<MethodInfo> AllDbOperations = typeof(DbConnectionExtension).GetMethods()
        .Where(x => x.IsPublic && x.IsStatic)
        .Where(x => x.GetParameters()[0].ParameterType.IsAssignableFrom(typeof(DbConnection)));


    public static IEnumerable<object[]> AllDbOperationsNonAsync =>
        AllDbOperations.Where(x => x.Name.EndsWith("Async") == false)
        .Select(x => new[] { x });

    public static IEnumerable<object[]> AllDbOperationsAsync =>
        AllDbOperations.Where(x => x.Name.EndsWith("Async") == true)
        .Select(x => new[] { x });

    public static string GetDisplayName(MethodInfo call, object[] info)
    {
        var mi = (MethodInfo)info[0];
        return $"{call.Name} {mi.Name}({string.Join(", ", mi.GetParameters().Select(x => x.ParameterType))})";
    }

    [TestMethod, DynamicData(nameof(AllDbOperationsNonAsync), DynamicDataDisplayName = nameof(GetDisplayName))]
    public void DbConnectionOperation(MethodInfo mi)
    {
        using var db = mi.Name.StartsWith("Truncate") ? CreateOpenConnection() : CreateOpenLimitedConnection();
        var parameters = mi.GetParameters();
        var args = new object?[parameters.Length];
        mi = SetupCall(mi, db, ref parameters, args);

        var r = mi.Invoke(null, args);

        if (r is System.Collections.IEnumerable enumerable)
        {
            foreach (var i in enumerable)
            {
                GC.KeepAlive(i);
            }
        }
    }


    [TestMethod, DynamicData(nameof(AllDbOperationsAsync), DynamicDataDisplayName = nameof(GetDisplayName))]
    public async Task DbConnectionOperationAsync(MethodInfo mi)
    {
        using var db = mi.Name.StartsWith("Truncate") ? await CreateOpenConnectionAsync() : await CreateOpenLimitedConnectionAsync();

        var parameters = mi.GetParameters();
        var args = new object?[parameters.Length];
        mi = SetupCall(mi, db, ref parameters, args);

        var r = mi.Invoke(null, args);

        if (r is not Task tsk)
        {
            var type = r.GetType();
            if (type.GetInterfaces().FirstOrDefault(x=>x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>)) is { } enumType)
            {
                tsk = (Task)typeof(CallThemAllTests).GetMethod(nameof(Walk)).MakeGenericMethod(enumType.GetGenericArguments()).Invoke(this, [r]);
            }
            else
                tsk = (Task)type.GetMethod("AsTask").Invoke(r, []);
        }
        await tsk;
    }

    public async Task Walk<T>(IAsyncEnumerable<T> v)
    {
        await foreach(var i in v)
        {
            GC.KeepAlive(i);
        }
    }


    public static IEnumerable<MethodInfo> AllRepoOperations = typeof(DbRepository<SqlConnection>).GetMethods()
        .Where(x=>x.IsPublic && !x.IsStatic);

    public static IEnumerable<object[]> AllRepoOperationsNonAync =>
        AllRepoOperations.Where(x => x.Name.EndsWith("Async") == false)
        .Select(x => new[] { x });

    public static IEnumerable<object[]> AllRepoOperationsAsync =>
        AllRepoOperations.Where(x => x.Name.EndsWith("Async") == true)
        .Select(x => new[] { x });


    [TestMethod, DynamicData(nameof(AllRepoOperationsNonAync), DynamicDataDisplayName = nameof(GetDisplayName))]
    public void RepoConnectionOperation(MethodInfo mi)
    {
        using var repo = new DbRepository<SqlConnection>(mi.Name.StartsWith("Truncate") ? DbInstance.ConnectionString : DbInstance.LimitedConnectionString);
        var parameters = mi.GetParameters();
        var args = new object?[parameters.Length];
        mi = SetupCall(mi, null, ref parameters, args);

        mi.Invoke(repo, args);
    }


    [TestMethod, DynamicData(nameof(AllRepoOperationsAsync), DynamicDataDisplayName = nameof(GetDisplayName))]
    public async Task RepoConnectionOperationAsync(MethodInfo mi)
    {
        using var repo = new DbRepository<SqlConnection>(mi.Name.StartsWith("Truncate") ? DbInstance.ConnectionString : DbInstance.LimitedConnectionString);
        var parameters = mi.GetParameters();
        var args = new object?[parameters.Length];
        mi = SetupCall(mi, null, ref parameters, args);


        var r = mi.Invoke(repo, args);

        if (r is not Task tsk)
        {
            var type = r.GetType();
            if (type.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>)) is { } enumType)
            {
                tsk = (Task)typeof(CallThemAllTests).GetMethod(nameof(Walk)).MakeGenericMethod(enumType.GetGenericArguments()).Invoke(this, [r]);
            }
            else
                tsk = (Task)type.GetMethod("AsTask").Invoke(r, []);
        }
        await tsk;
    }

    public static IEnumerable<MethodInfo> AllBaseRepoOperations = typeof(BaseRepository<IdentityTable, SqlConnection>).GetMethods()
        .Where(x => x.IsPublic && !x.IsStatic);

    public static IEnumerable<object[]> AllBaseRepoOperationsNonAync =>
        AllBaseRepoOperations.Where(x => x.Name.EndsWith("Async") == false)
        .Select(x => new[] { x });

    public static IEnumerable<object[]> AllBaseRepoOperationsAsync =>
        AllBaseRepoOperations.Where(x => x.Name.EndsWith("Async") == true)
        .Select(x => new[] { x });


    class MyRepository<TEntity, TConnection> : BaseRepository<IdentityTable, SqlConnection>
        where TEntity : class
        where TConnection : DbConnection, new()
    {
        public MyRepository(string connectionString, ConnectionPersistency connectionPersistency)
            : base(connectionString, null, connectionPersistency, null, Constant.DefaultCacheItemExpirationInMinutes, null, null)
        {
        }
    }

    [TestMethod, DynamicData(nameof(AllBaseRepoOperationsNonAync), DynamicDataDisplayName = nameof(GetDisplayName))]
    public void BaseRepoConnectionOperation(MethodInfo mi)
    {
        var BaseRepo = new MyRepository<IdentityTable, SqlConnection>(mi.Name.StartsWith("Truncate") ? DbInstance.ConnectionString : DbInstance.LimitedConnectionString, ConnectionPersistency.PerCall);
        var parameters = mi.GetParameters();
        var args = new object?[parameters.Length];
        mi = SetupCall(mi, null, ref parameters, args);

        mi.Invoke(BaseRepo, args);
    }


    [TestMethod, DynamicData(nameof(AllBaseRepoOperationsAsync), DynamicDataDisplayName = nameof(GetDisplayName))]
    public async Task BaseRepoConnectionOperationAsync(MethodInfo mi)
    {
        var BaseRepo = new MyRepository<IdentityTable, SqlConnection>(mi.Name.StartsWith("Truncate") ? DbInstance.ConnectionString : DbInstance.LimitedConnectionString, ConnectionPersistency.PerCall);
        var parameters = mi.GetParameters();
        var args = new object?[parameters.Length];
        mi = SetupCall(mi, null, ref parameters, args);

        var r = mi.Invoke(BaseRepo, args);

        if (r is not Task tsk)
        {
            var type = r.GetType();
            if (type.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>)) is { } enumType)
            {
                tsk = (Task)typeof(CallThemAllTests).GetMethod(nameof(Walk)).MakeGenericMethod(enumType.GetGenericArguments()).Invoke(this, [r]);
            }
            else
                tsk = (Task)type.GetMethod("AsTask").Invoke(r, []);
        }
        await tsk;
    }

    private static MethodInfo SetupCall(MethodInfo mi, DbConnection? db, ref ParameterInfo[] parameters, object?[] args)
    {
        if (mi.IsGenericMethod)
        {
            var ga = mi.GetGenericArguments();

            if (ga.Length == 1 && ga[0].Name is "TEntity" or "TWhat")
                mi = mi.MakeGenericMethod(typeof(IdentityTable));
            else if (ga.Length == 1 && ga[0].Name is "TResult" or "TKey")
                mi = mi.MakeGenericMethod(typeof(int?));
            else if (ga.Length == 2 && ga[0].Name == "TEntity" && ga[1].Name is "TResult" or "TKey")
                mi = mi.MakeGenericMethod(typeof(IdentityTable), typeof(int?));
            else if (ga.Length == 2 && ga[0].Name == "TEntity" && ga[1].Name == "TWhat")
                mi = mi.MakeGenericMethod(typeof(IdentityTable), typeof(IdentityTable));
            else if (ga.All(x => x.Name.StartsWith("T") && x.Name.Substring(1).All(char.IsDigit)))
            {
                mi = mi.MakeGenericMethod(ga.Select(x => typeof(IdentityTable)).ToArray());
            }
            parameters = mi.GetParameters();
        }
        if (db is { })
        {
            args[0] = db;
        }

        for (int i = 0; i < parameters.Length; i++)
        {
            var qp = parameters[i];

            object? value = Convert.IsDBNull(qp.DefaultValue) ? null : qp.DefaultValue;

            var an = qp.Name.ToLowerInvariant();

            while (an.Length > 0 && char.IsDigit(an[an.Length - 1]))
                an = an.Substring(0, an.Length - 1);

            switch (an)
            {
                case "tablename":
                    value = nameof(IdentityTable);
                    break;
                case "commandtext":
                    value = "SELECT 1 AS Id;SELECT 1 AS Id;SELECT 1 AS Id;SELECT 1 AS Id;SELECT 1 AS Id;SELECT 1 AS Id;SELECT 1 AS Id;SELECT 1 AS Id;";
                    break;
                case "connection":
                    value = db;
                    break;
                case "entities":
                    value = new[]
                    {
                        new IdentityTable(),
                        new IdentityTable()
                    };
                    break;
                case "keys" when qp.ParameterType.IsAssignableFrom(typeof(int?[])):
                    value = new int?[] { 1, 2, 3 };
                    break;
                case "keys" when qp.ParameterType.IsAssignableFrom(typeof(object?[])):
                    value = new object?[] { 1, 2, 3 };
                    break;
                case "entity":
                    value = new IdentityTable();
                    break;
                case "qualifier" when !qp.HasDefaultValue:
                    value = new Field(nameof(IdentityTable.Id));
                    break;
                case "qualifiers" when !qp.HasDefaultValue && qp.ParameterType.IsAssignableFrom(typeof(FieldSet)):
                    value = new Field(nameof(IdentityTable.Id)).AsEnumerable();
                    break;
                case "qualifiers" when !qp.HasDefaultValue && qp.ParameterType.IsAssignableFrom(typeof(Expression<Func<IdentityTable, object?>>)):
                    value = (Expression<Func<IdentityTable, object?>>)(x => x.Id);
                    break;
                case "field" when !qp.HasDefaultValue && qp.ParameterType.IsAssignableFrom(typeof(Field)):
                    value = new Field(nameof(IdentityTable.ColumnInt));
                    break;
                case "field" when !qp.HasDefaultValue && qp.ParameterType.IsAssignableFrom(typeof(Expression<Func<IdentityTable, object?>>)):
                    value = (Expression<Func<IdentityTable, object?>>)(x => x.ColumnInt);
                    break;
                case "field" when !qp.HasDefaultValue && qp.ParameterType.IsAssignableFrom(typeof(Expression<Func<IdentityTable, int?>>)):
                    value = (Expression<Func<IdentityTable, int?>>)(x => x.ColumnInt);
                    break;
            }

            args[i] = value;
        }

        return mi;
    }
}
