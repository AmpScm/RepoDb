using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SqlServer.IntegrationTests.Setup;
using RepoDb.Trace;

namespace RepoDb.SqlServer.IntegrationTests;

#if NET
[TestClass]
public class AdditionalDbTypesTests
{
    [TestInitialize]
    public void Initialize()
    {
        GlobalConfiguration
            .Setup()
            .UseSqlServer();

        Database.Initialize();
        Cleanup();

        using var connection = new SqlConnection(Database.ConnectionString).EnsureOpen();


        /* Unmerged change from project 'RepoDb.SqlServer.IntegrationTests (net9.0)'
        Before:
                    connection.ExecuteNonQuery($@"
                        IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = '{nameof(DateOnlyTestData)}'))
        After:
                connection.ExecuteNonQuery($@"
                        IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = '{nameof(DateOnlyTestData)}'))
        */
        connection.ExecuteNonQuery($@"
            IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = '{nameof(DateOnlyTestData)}'))
            BEGIN
                CREATE TABLE [dbo].[{nameof(DateOnlyTestData)}] (
                    [Id] INT IDENTITY(1, 1),
                    [DateOnly]              DATE NOT NULL,
                    [DateOnlyNullable]      DATE NULL,
                    CONSTRAINT [{nameof(DateOnlyTestData)}_Id] PRIMARY KEY
                    (
                        [Id] ASC
                    )
                ) ON [PRIMARY]
            END");

        // Do this again as this is now overwritten
        GlobalConfiguration
            .Setup().UseSqlServer();
    }

    [TestCleanup]
    public void Cleanup()
    {
        Database.Cleanup();
    }

    [TestMethod]
    public async Task TestDateTimeOnlyInsertQuery()
    {
        await using var connection = new SqlConnection(Database.ConnectionString);
        await connection.OpenAsync();
        await using var t = await connection.BeginTransactionAsync();

        await connection.InsertAllAsync(
            new DateOnlyTestData[] {
                new()
                {
                    DateOnly = new DateOnly(2024,1,1),
                    DateOnlyNullable = null,
                },

/* Unmerged change from project 'RepoDb.SqlServer.IntegrationTests (net9.0)'
Before:
                transaction: t);
After:
                new ()
                {
                    DateOnly = new DateOnly(2025,1,1),
                    DateOnlyNullable = new DateOnly(2026,1,1),
                }
            },
            transaction: t);
*/

/* Unmerged change from project 'RepoDb.SqlServer.IntegrationTests (net9.0)'
Before:
            var all = await connection.QueryAllAsync<DateOnlyTestData>(transaction: t);

            Assert.IsTrue(all.Any(x => x.DateOnly == new DateOnly(2024, 1, 1)), "Found DateOnly");
            Assert.IsTrue(all.Any(x => x.DateOnlyNullable == new DateOnly(2026, 1, 1)), "Found nullable DateOnly");
            Assert.IsTrue(all.Any(x => x.DateOnlyNullable == null), "Found null DateOnly?");

            var cmp1 = new DateOnly(2024, 1, 1);
            Assert.AreEqual(1, (await connection.QueryAsync<DateOnlyTestData>(where: x => x.DateOnly == cmp1, transaction: t)).Count());
            Assert.IsTrue((await connection.QueryAsync<DateOnlyTestData>(where: x => x.DateOnlyNullable == new DateOnly(2026, 1, 1), transaction: t)).Count() == 1);
        }

        [TestMethod]
        public async Task CompareValuesTests()
        {
            await using var connection = new SqlConnection(Database.ConnectionString);
            await connection.OpenAsync();
            await using var t = await connection.BeginTransactionAsync();
After:
        var all = await connection.QueryAllAsync<DateOnlyTestData>(transaction: t);

        Assert.IsTrue(all.Any(x => x.DateOnly == new DateOnly(2024, 1, 1)), "Found DateOnly");
        Assert.IsTrue(all.Any(x => x.DateOnlyNullable == new DateOnly(2026, 1, 1)), "Found nullable DateOnly");
        Assert.IsTrue(all.Any(x => x.DateOnlyNullable == null), "Found null DateOnly?");
*/
                new ()
                {
                    DateOnly = new DateOnly(2025,1,1),
                    DateOnlyNullable = new DateOnly(2026,1,1),
                }
            },
            transaction: t);


        var all = await connection.QueryAllAsync<DateOnlyTestData>(transaction: t);

        Assert.IsTrue(all.Any(x => x.DateOnly == new DateOnly(2024, 1, 1)), "Found DateOnly");
        Assert.IsTrue(all.Any(x => x.DateOnlyNullable == new DateOnly(2026, 1, 1)), "Found nullable DateOnly");
        Assert.IsTrue(all.Any(x => x.DateOnlyNullable == null), "Found null DateOnly?");

        var cmp1 = new DateOnly(2024, 1, 1);
        Assert.AreEqual(1, (await connection.QueryAsync<DateOnlyTestData>(where: x => x.DateOnly == cmp1, transaction: t)).Count());
        Assert.AreEqual(1, (await connection.QueryAsync<DateOnlyTestData>(where: x => x.DateOnlyNullable == new DateOnly(2026, 1, 1), transaction: t)).Count());
    }

    [TestMethod]
    public async Task CompareValuesTests()
    {
        await using var connection = new SqlConnection(Database.ConnectionString);
        await connection.OpenAsync();
        await using var t = await connection.BeginTransactionAsync();

        await connection.InsertAllAsync(
            new DateOnlyTestData[] {
                new()
                {
                    DateOnly = new DateOnly(2024,1,1),
                    DateOnlyNullable = null,
                },
                new ()
                {
                    DateOnly = new DateOnly(2025,1,1),
                    DateOnlyNullable = new DateOnly(2026,1,1),
                }
            },
            transaction: t, trace: new DiagnosticsTracer());


        // This one used to fail with NotSupportedException as '!' was not interpreted correctly
        var notEqualValue = new DateOnly(2024, 1, 1);
        var n = await connection.UpdateAsync<DateOnlyTestData>(
            new()
            {
                DateOnly = new DateOnly(2027, 1, 1)
            },
            where: QueryGroup.Parse<DateOnlyTestData>(x => !(x.DateOnly == notEqualValue)),
            fields: Field.Parse<DateOnlyTestData>(x => x.DateOnly),
            transaction: t);

        Assert.AreEqual(1, n);

        // This one used to fail by silently ignoring the '!(x.DateOnlyNullable == notEqualValue2)' part
        var notEqualValue2 = new DateOnly(2029, 1, 1);
        var n2 = await connection.UpdateAsync<DateOnlyTestData>(
            new()
            {
                DateOnly = new DateOnly(2030, 1, 1)
            },
            where: QueryGroup.Parse<DateOnlyTestData>(x => x.DateOnlyNullable == null || !(x.DateOnlyNullable == notEqualValue2)),
            fields: Field.Parse<DateOnlyTestData>(x => x.DateOnly),
            transaction: t);
        Assert.AreEqual(2, n2);
    }



    public class DateOnlyTestData
    {
        public int ID { get; set; }
        public DateOnly DateOnly { get; set; }
        public DateOnly? DateOnlyNullable { get; set; }
    }
}
#endif
