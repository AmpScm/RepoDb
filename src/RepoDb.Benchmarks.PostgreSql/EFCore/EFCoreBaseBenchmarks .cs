using System.ComponentModel;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using RepoDb.Benchmarks.PostgreSql.Configurations;
using RepoDb.Benchmarks.PostgreSql.Models;
using RepoDb.Benchmarks.PostgreSql.Setup;

namespace RepoDb.Benchmarks.PostgreSql.EFCore;

[Description(OrmNameConstants.EFCore)]
public class EFCoreBaseBenchmarks : BaseBenchmark
{
    [GlobalSetup]
    public void Setup() => BaseSetup();

    protected override void Bootstrap()
    {
        using var context = new EFCoreContext(DatabaseHelper.ConnectionString);

        GC.KeepAlive(context.Persons.FirstOrDefault());
        GC.KeepAlive(context.Persons.AsNoTracking().FirstOrDefault());
        GC.KeepAlive(context.Persons.FromSqlRaw(@"select * from ""Person""").FirstOrDefault());
    }
}