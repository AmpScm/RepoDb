using System.ComponentModel.DataAnnotations.Schema;
using NodaTime;
using Npgsql;
using RepoDb.Attributes;
using RepoDb.Enumerations;

namespace RepoDb.PostgreSql.DependencyIntegrationTests;

[TestClass]
public class NodaTimeTests : DbTestBase
{
    [TestMethod]
    public void NodaTimeParameterTransfer()
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(CreateConnection().ConnectionString);
        dataSourceBuilder.UseNodaTime();
        var dataSource = dataSourceBuilder.Build();

        GlobalConfiguration.Setup(GlobalConfiguration.Options with
        {
            KeyColumnReturnBehavior = KeyColumnReturnBehavior.Identity
        }).UsePostgreSql();

        using var conn = dataSource.CreateConnection().EnsureOpen();

        conn.ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS public.repodb_nodatime_test (
                id SERIAL PRIMARY KEY,
                totedate DATE NOT NULL,
                created TIMESTAMPTZ NOT NULL,
                name TEXT NOT NULL
            )");


        // This INSERT fails on AmpScm.RepoDb 1.2026.304.420+ but worked on 1.2026.302.405
        var entity = new TestEntity
        {
            ToteDate = new LocalDate(2026, 3, 16),
            Created = SystemClock.Instance.GetCurrentInstant(),
            Name = "test"
        };
        var id = conn.Insert<TestEntity, int>(entity);

        var results = conn.Query<TestEntity>(x => x.ToteDate == new LocalDate(2026, 3, 16));

        var qf = new QueryField("totedate", Operation.Equal, new LocalDate(2026, 3, 16));
        results = conn.Query<TestEntity>(new QueryGroup(qf));
    }

    [Table("repodb_nodatime_test")]
    public class TestEntity
    {
        [Identity, Column("id")]
        public int Id { get; set; }

        [Column("totedate")]
        public LocalDate ToteDate { get; set; }

        [Column("created")]
        public Instant Created { get; set; }

        [Column("name")]
        public string Name { get; set; } = "";
    }
}
