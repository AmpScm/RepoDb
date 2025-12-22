using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Resolvers;
using RepoDb.Schema;
using RepoDb.StatementBuilders;
using RepoDb.Trace;

namespace RepoDb.TestCore;

public abstract partial class NullTestsBase<TDbInstance> : DbTestBase<TDbInstance> where TDbInstance : DbInstance, new()
{
    public record CommonNullTestData
    {
        public int ID { get; set; }
        public string Txt { get; set; }
        public string? TxtNull { get; set; }

        public int Nr { get; set; }
        public int? NrNull { get; set; }
    }


    public enum NullTest
    {
        Value1 = 1,
        Value2 = 2,
        Value3 = 3
    }

    [Table(nameof(CommonNullTestData))]
    public record EnumNullTestData
    {
        public int ID { get; set; }
        public NullTest Txt { get; set; }
        public NullTest? TxtNull { get; set; }

        public NullTest Nr { get; set; }
        public NullTest? NrNull { get; set; }
    }


    [TestMethod]
    public async Task NullQueryTest()
    {
        using var sql = await CreateOpenConnectionAsync();

        if (!await sql.SchemaObjectExistsAsync<CommonNullTestData>(cancellationToken: TestContext.CancellationToken))
        {
            var sqlText = @$"CREATE TABLE [CommonNullTestData] (
                        [ID] int NOT NULL,
                        [Txt] {VarCharName}(128) NOT NULL,
                        [TxtNull] {VarCharName}(128) NULL,
                        [Nr] int NOT NULL,
                        [NrNull] int NULL
                )";

            var set = sql.GetDbSetting();

            if (set.OpeningQuote != "[")
                sqlText = sqlText.Replace("[", set.OpeningQuote);
            if (set.ClosingQuote != "]")
                sqlText = sqlText.Replace("]", set.ClosingQuote);

            await sql.ExecuteNonQueryAsync(sqlText, cancellationToken: TestContext.CancellationToken);
        }

        var t = await sql.BeginTransactionAsync(TestContext.CancellationToken);

        await sql.InsertAllAsync(
            [
                new CommonNullTestData(){ ID = 1, Txt = "t1", TxtNull = "t2", Nr = 10, NrNull = 11},
                new CommonNullTestData(){ ID = 2, Txt = "t5", TxtNull = null, Nr = 15, NrNull = null}
            ], transaction: t, cancellationToken: TestContext.CancellationToken);

        var all = sql.QueryAll<CommonNullTestData>(transaction: t);
        Assert.AreEqual(2, all.Count());


        Assert.AreEqual(1, await sql.CountAsync<CommonNullTestData>(where: x => x.Txt != "t1", transaction: t, trace: new DiagnosticsTracer(), cancellationToken: TestContext.CancellationToken));
        Assert.AreEqual(1, await sql.CountAsync<CommonNullTestData>(where: x => x.Nr != 10, transaction: t, cancellationToken: TestContext.CancellationToken));

        Assert.AreEqual(0, await sql.CountAsync<CommonNullTestData>(where: x => x.TxtNull != "t2", transaction: t, trace: new DiagnosticsTracer(), cancellationToken: TestContext.CancellationToken));
        Assert.AreEqual(0, await sql.CountAsync<CommonNullTestData>(where: x => x.NrNull != 11, transaction: t, cancellationToken: TestContext.CancellationToken));

        GlobalConfiguration.Setup(GlobalConfiguration.Options with { ExpressionNullSemantics = ExpressionNullSemantics.NullNotEqual });

        Assert.AreEqual(1, await sql.CountAsync<CommonNullTestData>(where: x => x.TxtNull != "t2", transaction: t, trace: new DiagnosticsTracer(), cancellationToken: TestContext.CancellationToken));
        Assert.AreEqual(1, await sql.CountAsync<CommonNullTestData>(where: x => x.NrNull != 11, transaction: t, trace: new DiagnosticsTracer(), cancellationToken: TestContext.CancellationToken));

        var storeQueries = new StoreQueryTracer();

        Assert.AreEqual(1, await sql.CountAsync<CommonNullTestData>(where: x => x.Txt != "t1", transaction: t, trace: storeQueries, cancellationToken: TestContext.CancellationToken));
        Assert.AreEqual(1, await sql.CountAsync<CommonNullTestData>(where: x => x.Nr != 10, transaction: t, trace: storeQueries, cancellationToken: TestContext.CancellationToken));


        Assert.IsFalse(storeQueries.Traces.Any(x => x.Contains("NULL")), string.Join(Environment.NewLine, storeQueries.Traces));


        Assert.AreEqual(1, await sql.CountAsync<CommonNullTestData>(where: x => x.ID == 1 && !(x.Txt == "t5" && x.Nr == 22), transaction: t, trace: new DiagnosticsTracer(), cancellationToken: TestContext.CancellationToken));


        sql.Insert(new EnumNullTestData(), trace: new DiagnosticsTracer(), transaction: t);


        var toDel = new int[] { 1, 2 };
        sql.Delete<CommonNullTestData>(x => toDel.Contains(x.NrNull.Value), transaction: t, trace: new DiagnosticsTracer());

        await t.RollbackAsync(TestContext.CancellationToken);
    }

    public record GuidNullData
    {
        public int ID { get; set; }
        public Guid Txt { get; set; }
        public Guid? TxtNull { get; set; }

        public Guid Uuid { get; set; }
        public Guid? UuidNull { get; set; }

        public byte[] BlobData { get; set; }
        public byte[]? BlobDataNull { get; set; }
    }

    public virtual string UuidDbType => "[uniqueidentifier]";
    public virtual string BlobDbType => "varbinary(128)";
    public virtual string TextDbType => "TEXT";

    public virtual string IntDbType => "int";

    [TestMethod]
    public async Task GuidNullTest()
    {
        // Regression test. Failed on sqlite and sqlserver before this commit
        using var sql = await CreateOpenConnectionAsync();

        if (!await sql.SchemaObjectExistsAsync("GuidNullData", cancellationToken: TestContext.CancellationToken))
        {
            await PerformCreateTableAsync(sql, $@"CREATE TABLE [GuidNullData] (
                        [ID] int NOT NULL,
                        [Txt] {TextDbType} NOT NULL,
                        [TxtNull] {VarCharName}(128) NULL,
                        [Uuid] {UuidDbType} NOT NULL,
                        [UuidNull] {UuidDbType} NULL,
                        [BlobData] {BlobDbType} NOT NULL,
                        [BlobDataNull] {BlobDbType} NULL
                )");
        }

        var t = await sql.BeginTransactionAsync(TestContext.CancellationToken);

        await sql.InsertAllAsync(
            [
                new GuidNullData(){ ID = 1, Txt = Guid.NewGuid(), TxtNull = Guid.NewGuid(), Uuid = Guid.NewGuid(), UuidNull=Guid.NewGuid(), BlobData = " "u8.ToArray(), BlobDataNull = "A"u8.ToArray() },
                new GuidNullData(){ ID = 2, Txt = Guid.NewGuid(), Uuid = Guid.NewGuid(), BlobData = "a"u8.ToArray() },
            ], transaction: t, cancellationToken: TestContext.CancellationToken);

        await t.RollbackAsync(TestContext.CancellationToken);
    }

    [Table("CommonDateTimeNullTestData")]
    private class DateTestData
    {
        public int ID { get; set; }
        public DateTime? Txt { get; set; }
        public DateTime? Date { get; set; }
    }

    [Table("CommonDateTimeNullTestData")]
    private class DateOffsetTestData
    {
        public int ID { get; set; }
        public DateTimeOffset? Txt { get; set; }
        public DateTimeOffset? Date { get; set; }
    }

    public virtual string DateTimeDbType => "datetime";

    [TestMethod]
    public async Task DateTimeToDateTimeOffsetTests()
    {
        using var sql = await CreateOpenConnectionAsync();

        if (sql.GetType().Name is { } name && (name.Contains("Postgre") || name.Contains("Npgsql")))
            return;

        if (!await sql.SchemaObjectExistsAsync("CommonDateTimeNullTestData", cancellationToken: TestContext.CancellationToken))
        {
            await PerformCreateTableAsync(sql, $@"CREATE TABLE [CommonDateTimeNullTestData] (
                        [ID] int NOT NULL,
                        [Txt] {VarCharName}(128) NULL,
                        [Date] {DateTimeDbType} NULL
                )");
        }
        else
        {
            await sql.TruncateAsync(tableName: "CommonDateTimeNullTestData", cancellationToken: TestContext.CancellationToken);
        }

        var t = await sql.BeginTransactionAsync(TestContext.CancellationToken);

        await sql.InsertAllAsync(
            [
                new DateTestData(){ ID = 1, Txt = new DateTime(2001, 1,1,1,1,1, DateTimeKind.Utc), Date = new DateTime(2002, 1,2,2,2,2, DateTimeKind.Utc)},
                new DateTestData(){ ID = 2, Txt =null, Date = null }
            ],
            trace: new DiagnosticsTracer(),
            transaction: t, cancellationToken: TestContext.CancellationToken);
        await sql.InsertAllAsync(
            [
                new DateOffsetTestData(){ ID = 3, Txt = new DateTimeOffset(2003, 1,3,3,3,3, TimeSpan.Zero), Date = new DateTimeOffset(2004, 1,4,4,4,4, TimeSpan.Zero)},
                new DateOffsetTestData(){ ID = 4, Txt =null, Date = null }
            ], transaction: t, cancellationToken: TestContext.CancellationToken);

        var all = sql.QueryAll<DateTestData>(transaction: t);
        Assert.AreEqual(4, all.Count());

        var allOffset = sql.QueryAll<DateOffsetTestData>(transaction: t);
        Assert.AreEqual(4, all.Count());

        foreach (var v in all)
        {
            if (v.Txt is { } d2)
            {
                Assert.AreEqual(v.ID, d2.Day);
            }
            if (v.Date is { } d)
            {
                Assert.AreEqual(v.ID + 1, d.Day);
            }
        }

        var l = sql.Query<DateOffsetTestData>(where: x => x.Date < DateTimeOffset.Now, transaction: t);

        await t.RollbackAsync(TestContext.CancellationToken);
    }

    public virtual string DateTimeOffsetDbType => "datetimeoffset";

#if NET
    public virtual string TimeOnlyDbType => "time";
    public virtual string DateOnlyDbType => "date";

    private record DateTimeOnlyTable
    {
        public TimeOnly TOnly { get; set; }
        public DateOnly DOnly { get; set; }

        public DateTimeOffset DOffset { get; set; }
    }

    [TestMethod]
    public async Task DateTimeOnlyTests()
    {
        using var sql = await CreateOpenConnectionAsync();

        if (!await sql.SchemaObjectExistsAsync<DateTimeOnlyTable>(cancellationToken: TestContext.CancellationToken))
        {
            await PerformCreateTableAsync(sql, $@"CREATE TABLE [{nameof(DateTimeOnlyTable)}] (
                        [TOnly] {TimeOnlyDbType} NOT NULL,
                        [DOnly] {DateOnlyDbType} NOT NULL,
                        [DOffset] {DateTimeOffsetDbType} NOT NULL
            )");
        }
        else
        {
            await sql.TruncateAsync<DateTimeOnlyTable>(cancellationToken: TestContext.CancellationToken);
        }

        await sql.InsertAsync(new DateTimeOnlyTable() { DOnly = new DateOnly(2021, 1, 1), TOnly = new TimeOnly(1, 2, 3), DOffset = new DateTimeOffset(2023, 1, 1, 1, 1, 1, TimeSpan.Zero) }, cancellationToken: TestContext.CancellationToken);

        Assert.IsNotEmpty(await sql.QueryAllAsync<DateTimeOnlyTable>(cancellationToken: TestContext.CancellationToken));

        Assert.IsNotEmpty(await sql.ExecuteQueryAsync<DateTimeOnlyTable>(ApplySqlRules(sql, "SELECT * FROM [DateTimeOnlyTable]"), cancellationToken: TestContext.CancellationToken));

        var today = DateOnly.FromDateTime(DateTime.Now);
        await sql.QueryAsync<DateTimeOnlyTable>(where: x => x.DOnly < today, trace: new DiagnosticsTracer(), cancellationToken: TestContext.CancellationToken);

        if (sql.GetType().Name is { } name && (name.Contains("Postgre") || name.Contains("Npgsql")))
            return; // Limitations of PostgreSQL DateTimeOffset

        await sql.QueryAsync<DateTimeOnlyTable>(where: x => x.DOffset < DateTime.Now, cancellationToken: TestContext.CancellationToken);
        await sql.QueryAsync<DateTimeOnlyTable>(where: x => x.DOffset < DateTimeOffset.Now, cancellationToken: TestContext.CancellationToken);
    }
#endif

    private record WithComputed
    {
        public int ID { get; set; }
        public string Writable { get; set; }
        public string Computed { get; set; }
    }

    public virtual string GeneratedColumnDefinition(string expression, string type) => $"GENERATED ALWAYS AS ({expression})";

    [TestMethod]
    public async Task ComputedColumnTest()
    {
        using var sql = await CreateOpenConnectionAsync();

        if (!await sql.SchemaObjectExistsAsync<WithComputed>(cancellationToken: TestContext.CancellationToken))
        {
            await PerformCreateTableAsync(sql, $@"CREATE TABLE [WithComputed] (
                        [ID] int NOT NULL,
                        [Writable] {VarCharName}(128) NOT NULL,
                        [Computed] {GeneratedColumnDefinition("CONCAT('--', [Writable])", $"{VarCharName}(130)")}
            )");
        }
        else
        {
            await sql.TruncateAsync<WithComputed>(trace: new DiagnosticsTracer(), cancellationToken: TestContext.CancellationToken);
        }

        var fields = await DbFieldCache.GetAsync(sql, nameof(WithComputed), transaction: null, TestContext.CancellationToken);
        Assert.IsTrue(fields.First(x => x.FieldName == "Computed").IsGenerated);

        await sql.InsertAsync(new WithComputed() { ID = 1, Writable = "a" }, cancellationToken: TestContext.CancellationToken);

        var r = (await sql.QueryAllAsync<WithComputed>(cancellationToken: TestContext.CancellationToken)).FirstOrDefault();

        Assert.AreEqual(1, r.ID);
        Assert.AreEqual("a", r.Writable);
        Assert.AreEqual("--a", r.Computed);

        await sql.QueryAllAsync<WithComputed>(orderBy: [OrderField.Parse<WithComputed>(x => x.Computed, Order.Ascending)], cancellationToken: TestContext.CancellationToken);

        await sql.UpdateAsync<WithComputed>(
            new()
            {
                Writable = "b"
            },
            where: x => x.Computed == "--a",
            trace: new DiagnosticsTracer(), cancellationToken: TestContext.CancellationToken);
    }

    private record NullIdentityTest
    {
        public long? ID { get; set; } // NULL identity, while database has NOT NULL
        public string Key { get; set; }
        public string? Value { get; set; }
    }

    [TestMethod]
    public async Task InsertMergeNullTest()
    {
        using var sql = await CreateOpenConnectionAsync();

        if (sql.GetType().Name.Contains("Oracle"))
            return;

        if (!await sql.SchemaObjectExistsAsync<NullIdentityTest>(cancellationToken: TestContext.CancellationToken))
        {
            await PerformCreateTableAsync(sql, $@"CREATE TABLE [{nameof(NullIdentityTest)}] (
                        [ID] {IdentityDefinition} {(IdentityDefinition.Contains("PRIMARY") ? "" : " PRIMARY KEY")},
                        [Key] {VarCharName}(128) NOT NULL,
                        [Value] {VarCharName}(128) NULL
            )");

            await PerformCreateTableAsync(sql, $@"CREATE UNIQUE INDEX [IX_{nameof(NullIdentityTest)}] ON [{nameof(NullIdentityTest)}] ([Key]);");
        }
        else
        {
            await sql.TruncateAsync<NullIdentityTest>(cancellationToken: TestContext.CancellationToken);
        }


        var r = new NullIdentityTest() { ID = 77, Key = "k1", Value = null };
        var v1 = await sql.InsertAsync<NullIdentityTest, int>(r, cancellationToken: TestContext.CancellationToken);
        Assert.AreEqual(1, v1);
        var v2 = await sql.MergeAsync<NullIdentityTest, int>(r, cancellationToken: TestContext.CancellationToken);
        Assert.AreEqual(1, v2);

        var v3 = await sql.MergeAsync<NullIdentityTest, int>(new() { ID = null, Key = "k1", Value = null }, qualifiers: Field.Parse<NullIdentityTest>(x => x.Key), cancellationToken: TestContext.CancellationToken);
        Assert.AreEqual(1, v2);
    }

    [TestMethod]
    public void TestReadTuple()
    {
        var sql = CreateConnection().EnsureOpen();

        var t = sql.ExecuteQuery<Tuple<int, string>>("SELECT 1, 'a'").FirstOrDefault();
        Assert.AreEqual(1, t.Item1);
        Assert.AreEqual("a", t.Item2);
    }

    [TestMethod]
    public void TestReadValueTuple()
    {
        var sql = CreateConnection().EnsureOpen();

        var (v1, c2) = sql.ExecuteQuery<(int v1, string c2)>("SELECT 1, 'a'").FirstOrDefault();

        Assert.AreEqual(1, v1);
        Assert.AreEqual("a", c2);
    }

    private record WithGroupByItems
    {
        public int ID { get; set; }
        public string Txt { get; set; }
    }

    [TestMethod]
    public async Task VarPrefix()
    {
        using var sql = await CreateOpenConnectionAsync();

        if (!await sql.SchemaObjectExistsAsync<WithGroupByItems>(cancellationToken: TestContext.CancellationToken))
        {
            await PerformCreateTableAsync(sql, $@"CREATE TABLE [WithGroupByItems] (
                        [ID] int NOT NULL,
                        [Txt] {VarCharName}(128) NOT NULL,
                        [Nr] int NOT NULL
            )");
        }

        // This used to trigger an escaping issue between @a and @aa (which starts with '@a')
        var s = await sql.ExecuteQueryAsync<WithGroupByItems>(
            ApplySqlRules(sql, "SELECT [Txt] from [WithGroupByItems] WHERE [Txt] IN (@a) GROUP BY [Txt] HAVING COUNT(1) = @aa"),
            new { a = new string[] { "a" }, aa = 1 },
            cancellationToken: TestContext.CancellationToken);
    }

    private class Id2record
    {

        public int ID { get; set; }
        public int ID2 { get; set; }
    }
    private class Id2recordNullable
    {
        public int ID { get; set; }
        public int? ID2 { get; set; }
    }

    private const string IntNotNullable = nameof(IntNotNullable);
    [TestMethod]
    public async Task NullableIntError()
    {
        using var sql = await CreateOpenConnectionAsync();

        if (!await sql.SchemaObjectExistsAsync(nameof(IntNotNullable), cancellationToken: TestContext.CancellationToken))
        {
            await PerformCreateTableAsync(sql, $@"CREATE TABLE [{IntNotNullable}] (
                        [ID] int NOT NULL,
                        [ID2] int NOT NULL
            )");
        }

        await sql.InsertAsync(tableName: IntNotNullable, new Id2record { ID = 1, ID2 = 2 }, cancellationToken: TestContext.CancellationToken);

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await sql.InsertAsync(tableName: IntNotNullable, new Id2recordNullable { ID = 3, ID2 = null }, cancellationToken: TestContext.CancellationToken),
            "Required Nullable<Int32> property ID2 evaluated to null.");
    }


    private class FieldLengthTable
    {
        public string ID { get; set; }
        public string ID2 { get; set; }
        public string? VAL3 { get; set; }
    }

    public virtual string AltVarChar => "varchar";
    public virtual string VarCharName => "varchar";

    [TestMethod]
    public async Task FieldLengthTest()
    {
        using var sql = await CreateOpenConnectionAsync();

        if (!await sql.SchemaObjectExistsAsync<FieldLengthTable>(cancellationToken: TestContext.CancellationToken))
        {
            await PerformCreateTableAsync(sql, $@"CREATE TABLE [{nameof(FieldLengthTable)}] (
                    [ID] {VarCharName}(36) NOT NULL,
                    [ID2] {AltVarChar}(37) NOT NULL,
                    [VAL3] {AltVarChar}(38) NULL,
                    CONSTRAINT [PK_{nameof(FieldLengthTable)}] PRIMARY KEY
                    (
                        [ID], [ID2]
                    )
            )");
        }
        else
        {
            await sql.TruncateAsync<FieldLengthTable>(cancellationToken: TestContext.CancellationToken);
        }

        var fd = await sql.GetDbHelper().GetFieldsAsync(sql, nameof(FieldLengthTable), cancellationToken: TestContext.CancellationToken);

        var id1 = fd.First(x => x.FieldName == "ID");
        var id2 = fd.First(x => x.FieldName == "ID2");
        var val3 = fd.First(x => x.FieldName == "VAL3");
        Assert.AreEqual("ID", id1?.FieldName);
        Assert.AreEqual("ID2", id2?.FieldName);
        Assert.AreEqual("VAL3", val3?.FieldName);
        Assert.AreEqual(typeof(string), id1?.Type);
        Assert.AreEqual(typeof(string), id2?.Type);
        Assert.AreEqual(typeof(string), val3?.Type);

        Assert.AreEqual(VarCharName, id1?.DatabaseType);
        Assert.AreEqual(AltVarChar == "varchar" ? VarCharName : AltVarChar, id2?.DatabaseType);
        Assert.AreEqual(AltVarChar == "varchar" ? VarCharName : AltVarChar, val3?.DatabaseType);

        Assert.AreEqual(36, id1?.Size);
        Assert.AreEqual(37, id2?.Size);
        Assert.AreEqual(38, val3?.Size);

        Assert.IsTrue(id1?.IsPrimary);
        Assert.IsTrue(id2?.IsPrimary);
        Assert.IsFalse(val3?.IsPrimary);

        Assert.IsFalse(id1?.IsIdentity);
        Assert.IsFalse(id2?.IsIdentity);
        Assert.IsFalse(val3?.IsIdentity);

        Assert.IsFalse(id1?.IsNullable);
        Assert.IsFalse(id2?.IsNullable);
        Assert.IsTrue(val3?.IsNullable);

        var ftf = new FieldLengthTable[]
        {
            new FieldLengthTable { ID = "a12345678901234567890123456789012345", ID2 = "b12345678901234567890123456789012345", VAL3 = "c" },
            new FieldLengthTable { ID = "d12345678901234567890123456789012345", ID2 = "e12345678901234567890123456789012345", VAL3 = null }
        };

        Assert.AreEqual(2, await sql.InsertAllAsync(ftf, trace: new DiagnosticsTracer(), cancellationToken: TestContext.CancellationToken));

        var data = (await sql.QueryAllAsync<FieldLengthTable>(cancellationToken: TestContext.CancellationToken)).ToArray();

        Assert.HasCount(2, data);
        Assert.AreEqual(ftf[0].ID, data[0].ID);
    }

    private class MorePrimaryKeyTable
    {
        public string ID { get; set; }
        public int ID2 { get; set; }
        public string? Value { get; set; }
    }

    protected virtual string IdentityDefinition => IntDbType + " GENERATED ALWAYS AS IDENTITY";



    [TestMethod]
    public async Task MultiKeyReturnIdentity()
    {
        using var sql = await CreateOpenConnectionAsync();

        if (sql.GetType().Name.Contains("iteConnection"))
            return;

        if (!await sql.SchemaObjectExistsAsync<MorePrimaryKeyTable>(cancellationToken: TestContext.CancellationToken))
        {
            await PerformCreateTableAsync(sql, $@"CREATE TABLE [{nameof(MorePrimaryKeyTable)}] (
                    [ID] {VarCharName}(20) NOT NULL,
                    [ID2] {IdentityDefinition},
                    [Value] {AltVarChar}(38) NULL,
                    CONSTRAINT [PK_{nameof(MorePrimaryKeyTable)}] PRIMARY KEY
                    (
                        [ID2], [ID]
                    )
            )");
        }
        else
        {
            await sql.TruncateAsync<MorePrimaryKeyTable>(cancellationToken: TestContext.CancellationToken);
        }

        var fd = await sql.GetDbHelper().GetFieldsAsync(sql, nameof(MorePrimaryKeyTable), cancellationToken: TestContext.CancellationToken);

        var id1 = fd.First(x => x.FieldName == "ID");
        var id2 = fd.First(x => x.FieldName == "ID2");
        var val3 = fd.First(x => x.FieldName == "Value");
        Assert.AreEqual("ID", id1?.FieldName);
        Assert.AreEqual("ID2", id2?.FieldName);
        Assert.AreEqual("Value", val3?.FieldName);

        Assert.AreEqual(VarCharName, id1?.DatabaseType);
        //Assert.AreEqual("INT", id2?.DatabaseType); // Or 'int' or 'integer', or ...
        Assert.AreEqual(AltVarChar == "varchar" ? VarCharName : AltVarChar, val3?.DatabaseType);

        Assert.AreEqual(typeof(string), id1?.Type);
        Assert.IsTrue(id2?.Type == typeof(int) || id2?.Type == typeof(decimal));
        Assert.AreEqual(typeof(string), val3?.Type);

        Assert.IsTrue(id1?.IsPrimary);
        Assert.IsTrue(id2?.IsPrimary);
        Assert.IsFalse(val3?.IsPrimary);

        Assert.IsFalse(id1?.IsIdentity);
        Assert.IsTrue(id2?.IsIdentity);
        Assert.IsFalse(val3?.IsIdentity);

        Assert.IsFalse(id1?.IsNullable);
        Assert.IsFalse(id2?.IsNullable);
        Assert.IsTrue(val3?.IsNullable);

        var ftf = new MorePrimaryKeyTable[]
        {
            new MorePrimaryKeyTable { ID = "A", ID2 = 0, Value = "c" },
            new MorePrimaryKeyTable { ID = "B", ID2 = 0, Value = null }
        };

        Assert.AreEqual(2, await sql.InsertAllAsync(ftf, trace: new DiagnosticsTracer(), cancellationToken: TestContext.CancellationToken));

        var data = (await sql.QueryAllAsync<MorePrimaryKeyTable>(cancellationToken: TestContext.CancellationToken)).ToArray();

        Assert.HasCount(2, data);
        Assert.AreEqual(ftf[0].ID, data[0].ID);
        Assert.AreEqual(ftf[1].ID, data[1].ID);
        Assert.AreEqual(ftf[0].ID2, data[0].ID2);
        Assert.AreEqual(ftf[1].ID2, data[1].ID2);
        Assert.AreNotEqual(ftf[0].ID2, ftf[1].ID2);
        Assert.AreEqual(ftf[0].Value, data[0].Value);
        Assert.AreEqual(ftf[1].Value, data[1].Value);

        // More smoke tests // TODO: Create separate tests
        Assert.IsTrue(sql.Exists<MorePrimaryKeyTable>(where: x => x.ID == "A" && x.ID2 == data[0].ID2));

        await sql.MergeAsync(ftf[0], cancellationToken: TestContext.CancellationToken);
    }

    [TestMethod]
    public async Task GetRuntimeInfo()
    {
        using var sql = await CreateOpenConnectionAsync();

        var info = await DbRuntimeSettingCache.GetAsync(sql, null, TestContext.CancellationToken);

        Assert.IsNotNull(info);
    }

    [TestMethod]
    public async Task BulkQuery()
    {
        using var sql = await CreateOpenConnectionAsync();


    }

    class RelatedTable
    {
        [Primary]
        public int ID { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
        public int Ordered { get; set; }
        public int Canceled { get; set; }
        public int Delivered { get; set; }
    }

    [TestMethod]
    public async Task RelatedFields()
    {
        using var sql = await CreateOpenConnectionAsync();

        if (!await sql.SchemaObjectExistsAsync<RelatedTable>(cancellationToken: TestContext.CancellationToken))
        {
            await sql.CreateTableAsync<RelatedTable>();
        }
        else
        {
            await sql.TruncateAsync<RelatedTable>(cancellationToken: TestContext.CancellationToken);
        }

        await sql.InsertAllAsync<RelatedTable>([
            new RelatedTable { ID = 1, Name = "a", Ordered = 10, Canceled = 1, Delivered = 5 },
            new RelatedTable { ID = 2, Name = "b", Ordered = 20, Canceled = 2, Delivered = 10 },
            new RelatedTable { ID = 3, Name = "c", Ordered = 30, Canceled =0, Delivered =0 },
            new RelatedTable { ID = 4, Name = "d", Ordered = 40, Canceled = 40, Delivered = 0 }
        ], cancellationToken: TestContext.CancellationToken);

        Assert.AreEqual(3,
            await sql.UpdateAsync<RelatedTable>(
                new()
                {
                    Status = 1
                },
                where: x => x.Status == 0 && x.Ordered > x.Canceled,
                Field.Parse<RelatedTable>(x => x.Status),
                trace: new DiagnosticsTracer(), cancellationToken: TestContext.CancellationToken));

        // Not supported (yet). Was error
        await Assert.ThrowsExactlyAsync<NotSupportedException>(async () =>
            await sql.UpdateAsync<RelatedTable>(
                new()
                {
                    Status = 1
                },
                where: x => x.Status == 0 && x.Ordered - x.Canceled > 0,
                Field.Parse<RelatedTable>(x => x.Status),
                trace: new DiagnosticsTracer(), cancellationToken: TestContext.CancellationToken)
            );
    }


    private record MergeEdgeTable
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string? Value { get; set; }
    }

    [TestMethod]
    public async Task MergeEdgeCasesTestAsync()
    {
        GlobalConfiguration.Setup(GlobalConfiguration.Options with { SqlServerIdentityInsert = true });
        try
        {
            using var sql = await CreateOpenConnectionAsync();

            if (sql.GetType().Name.Contains("Oracle"))
                return;

            bool noIdentityInPrimaryKey = sql.GetType().Name.Contains("SQLite", StringComparison.OrdinalIgnoreCase) || sql.GetType().Name.Contains("Oracle");

            if (!await sql.SchemaObjectExistsAsync(nameof(MergeEdgeTable), cancellationToken: TestContext.CancellationToken))
            {
                await PerformCreateTableAsync(sql, $@"CREATE TABLE [{nameof(MergeEdgeTable)}] (
                    [ID] {IdentityDefinition},
                    [Name] {VarCharName}(20) NOT NULL,
                    [Value] {VarCharName}(38) NULL,
                    CONSTRAINT [PK_{nameof(MergeEdgeTable)}] PRIMARY KEY
                    (
                        [ID], [Name]
                    )
            )".Replace(IdentityDefinition, noIdentityInPrimaryKey ? IntDbType + " NOT NULL" : IdentityDefinition));

                await PerformCreateTableAsync(sql, $@"CREATE UNIQUE INDEX [IX_{nameof(MergeEdgeTable)}] ON [{nameof(MergeEdgeTable)}] ([Name]);");
            }
            else
            {
                await sql.TruncateAsync<MergeEdgeTable>(cancellationToken: TestContext.CancellationToken);
            }

            var r = new MergeEdgeTable()
            {
                ID = 1,
                Name = "a",
                Value = "b"
            };

            var v1 = await sql.MergeAsync<MergeEdgeTable, int>(r, trace: new DiagnosticsTracer(), cancellationToken: TestContext.CancellationToken);
            var v2 = await sql.MergeAsync<MergeEdgeTable, int>(r, cancellationToken: TestContext.CancellationToken);

            Assert.IsGreaterThan(0, v1);
            Assert.AreEqual(v1, v2);

            var all = await sql.QueryAllAsync<MergeEdgeTable>(cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(1, all.Count());

            // Still doing the same thing
            Assert.AreEqual(v2, await sql.MergeAsync<MergeEdgeTable, int>(r, qualifiers: Field.Parse<MergeEdgeTable>(x => new { x.ID, x.Name }), trace: new DiagnosticsTracer(), cancellationToken: TestContext.CancellationToken));

            // Now just update the first two fields. No-op
            Assert.AreEqual(v2, await sql.MergeAsync<MergeEdgeTable, int>(r, fields: Field.Parse<MergeEdgeTable>(x => new { x.ID, x.Name }), trace: new DiagnosticsTracer(), cancellationToken: TestContext.CancellationToken));

            var r2 = new MergeEdgeTable()
            {
                ID = 2,
                Name = "b",
                Value = "c"
            };

            await sql.MergeAsync(r2, cancellationToken: TestContext.CancellationToken);
            await sql.MergeAsync(r2, cancellationToken: TestContext.CancellationToken);

            all = await sql.QueryAllAsync<MergeEdgeTable>(cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(2, all.Count());

            await sql.DeleteAsync(r, cancellationToken: TestContext.CancellationToken);
            await sql.MergeAsync(r, trace: new DiagnosticsTracer(), cancellationToken: TestContext.CancellationToken);

            all = await sql.QueryAllAsync<MergeEdgeTable>(cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(2, all.Count());

            Assert.IsTrue(all.Any(x => x.ID == 1 || x.ID == 2));
        }
        finally
        {
            GlobalConfiguration.Setup(GlobalConfiguration.Options with { SqlServerIdentityInsert = false });
        }
    }

    [TestMethod]
    public void MergeEdgeCasesTest()
    {
        GlobalConfiguration.Setup(GlobalConfiguration.Options with { SqlServerIdentityInsert = true });
        try
        {
            using var sql = CreateOpenConnection();

            if (sql.GetType().Name.Contains("Oracle"))
                return;

            bool noIdentityInPrimaryKey = sql.GetType().Name.Contains("iteConnection") || sql.GetType().Name.Contains("Oracle");

            if (!sql.SchemaObjectExists(nameof(MergeEdgeTable)))
            {
                PerformCreateTable(sql, $@"CREATE TABLE [{nameof(MergeEdgeTable)}] (
                    [ID] {IdentityDefinition},
                    [Name] {VarCharName}(20) NOT NULL,
                    [Value] {VarCharName}(38) NULL,
                    CONSTRAINT [PK_{nameof(MergeEdgeTable)}] PRIMARY KEY
                    (
                        [ID], [Name]
                    )
            )".Replace(IdentityDefinition, noIdentityInPrimaryKey ? IntDbType + " NOT NULL" : IdentityDefinition));

                PerformCreateTable(sql, $@"CREATE UNIQUE INDEX [IX_{nameof(MergeEdgeTable)}] ON [{nameof(MergeEdgeTable)}] ([Name]);");
            }
            else
            {
                sql.Truncate<MergeEdgeTable>();
            }

            var r = new MergeEdgeTable()
            {
                ID = 1,
                Name = "a",
                Value = "b"
            };
            {
                var v1 = sql.Merge(r, trace: new DiagnosticsTracer());
                var v2 = sql.Merge<MergeEdgeTable, int>(r);

                Assert.IsTrue(Converter.ToType<int>(v1) is int i1 && i1 > 0);
                Assert.IsGreaterThan(0, v2);
            }

            var all = sql.QueryAll<MergeEdgeTable>();
            Assert.AreEqual(1, all.Count());

            // Still doing the same thing
            sql.Merge(r, qualifiers: Field.Parse<MergeEdgeTable>(x => new { x.ID, x.Name }), trace: new DiagnosticsTracer());

            // Now just update the first two fields. No-op
            sql.Merge(r, fields: Field.Parse<MergeEdgeTable>(x => new { x.ID, x.Name }), trace: new DiagnosticsTracer());

            var r2 = new MergeEdgeTable()
            {
                ID = 2,
                Name = "b",
                Value = "c"
            };

            sql.Merge(r2);
            sql.Merge(r2);

            all = sql.QueryAll<MergeEdgeTable>();
            Assert.AreEqual(2, all.Count());

            sql.Delete(r);
            sql.Merge(r, trace: new DiagnosticsTracer());

            all = sql.QueryAll<MergeEdgeTable>();
            Assert.AreEqual(2, all.Count());

            Assert.IsTrue(all.Any(x => x.ID == 1 || x.ID == 2));
        }
        finally
        {
            GlobalConfiguration.Setup(GlobalConfiguration.Options with { SqlServerIdentityInsert = false });
        }
    }

    private record MergeEdgeTable2
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string? Value { get; set; }
    }

    [TestMethod]
    public async Task MergeQualifierEdgeCasesTestAsync()
    {
        GlobalConfiguration.Setup(GlobalConfiguration.Options with { SqlServerIdentityInsert = true });
        try
        {
            using var sql = await CreateOpenConnectionAsync();

            if (sql.GetType().Name.Contains("Oracle"))
                return;

            if (!await sql.SchemaObjectExistsAsync(nameof(MergeEdgeTable2), cancellationToken: TestContext.CancellationToken))
            {
                await PerformCreateTableAsync(sql, $@"CREATE TABLE [{nameof(MergeEdgeTable2)}] (
                    [ID] {IdentityDefinition}{(IdentityDefinition.Contains("PRIMARY") ? "" : " PRIMARY KEY")},
                    [Name] {VarCharName}(20) NOT NULL,
                    [Value] {VarCharName}(38) NULL
            )");

                await PerformCreateTableAsync(sql, $@"CREATE UNIQUE INDEX [IX_{nameof(MergeEdgeTable2)}] ON [{nameof(MergeEdgeTable2)}] ([Name]);
        ");
            }
            else
            {
                await sql.TruncateAsync<MergeEdgeTable2>(cancellationToken: TestContext.CancellationToken);
            }

            var r = await sql.MergeAsync<MergeEdgeTable2, int>(new MergeEdgeTable2
            {
                Name = "a",
                Value = "c"
            }, qualifiers: Field.Parse<MergeEdgeTable2>(x => x.Name), trace: new DiagnosticsTracer(), cancellationToken: TestContext.CancellationToken);

            Assert.AreNotEqual(0, r);

            var v2 = new MergeEdgeTable2
            {
                Name = "d",
                Value = "e"
            };

            await sql.MergeAllAsync([v2], qualifiers: Field.Parse<MergeEdgeTable2>(x => x.Name), trace: new DiagnosticsTracer(), cancellationToken: TestContext.CancellationToken);
            Assert.AreNotEqual(0, v2.ID);

            v2.ID = 0;

            await sql.MergeAllAsync([v2], qualifiers: Field.Parse<MergeEdgeTable2>(x => x.Name), trace: new DiagnosticsTracer(), cancellationToken: TestContext.CancellationToken);
            Assert.AreNotEqual(0, v2.ID);
        }
        finally
        {
            GlobalConfiguration.Setup(GlobalConfiguration.Options with { SqlServerIdentityInsert = false });
        }
    }
}

public static class DbTestExtensions
{

    public static async Task CreateTableAsync<TEntity>(this DbConnection connection, ITrace? trace = null) where TEntity : class
    {
        var tableName = ClassMappedNameCache.Get<TEntity>();

        await CreateTableAsync<TEntity>(connection, tableName, trace);
    }

    public static async Task CreateTableAsync<TEntity>(this DbConnection sql, string tableName, ITrace? trace = null) where TEntity : class
    {
        var dbSetting = sql.GetDbSetting();
        var stmt = (BaseStatementBuilder)sql.GetStatementBuilder();
        var toDbField = (stmt.ConvertFieldResolver as DbConvertFieldResolver)?.StringNameResolver ?? FindResolver(sql);
        var cp = PropertyCache.Get<TEntity>();

        var qb = new QueryBuilder();

        qb.WriteText("CREATE").Table()
            .TableNameFrom(tableName, dbSetting)
            .OpenParen()
            .NewLine();

        bool first = true;
        foreach (var prop in cp)
        {
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

            var dbType =
                prop.DbType
                ?? type.GetDbType()
                ?? TypeMapCache.Get(type)
                ?? TypeMapper.Get(type)
                ?? ClientTypeToDbTypeResolver.Instance.Resolve(type)
                ?? System.Data.DbType.AnsiString;

            string name = toDbField?.Resolve(dbType) ?? "TEXT";
            qb.WriteText(name);

            if (type == typeof(string) && !name.Contains('(') && !string.Equals(name, "text", StringComparison.OrdinalIgnoreCase))
            {
                qb.OpenParen().WriteText("255").CloseParen();
            }
        }

        var primaryKeys = PrimaryCache.GetPrimaryKeys<TEntity>();

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

        await sql.ExecuteNonQueryAsync(qb.ToString(), trace: trace);
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
                    if (inst != null)
                    {
                        return inst;
                    }
                }
            }

            return null;
        });


    }

    public static async Task DropTableAsync<TEntity>(this DbConnection connection, ITrace? trace = null) where TEntity : class
    {
        var tableName = ClassMappedNameCache.Get<TEntity>();

        await DropTableAsync<TEntity>(connection, tableName, trace);
    }

    public static async Task DropTableAsync<TEntity>(this DbConnection sql, string tableName, ITrace? trace = null) where TEntity : class
    {
        var dbSetting = sql.GetDbSetting();

        var qb = new QueryBuilder();

        qb.WriteText("DROP").Table()
            .TableNameFrom(tableName, dbSetting);

        await sql.ExecuteNonQueryAsync(qb.ToString(), trace: trace);
    }
}
