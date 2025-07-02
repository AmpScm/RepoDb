using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Schema;
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

        if (!await sql.SchemaObjectExistsAsync("CommonNullTestData"))
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

            await sql.ExecuteNonQueryAsync(sqlText);
        }

        var t = await sql.BeginTransactionAsync();

        await sql.InsertAllAsync(
            new CommonNullTestData[]
            {
                new CommonNullTestData(){ ID = 1, Txt = "t1", TxtNull = "t2", Nr = 10, NrNull = 11},
                new CommonNullTestData(){ ID = 2, Txt = "t5", TxtNull = null, Nr = 15, NrNull = null}
            }, transaction: t);

        var all = sql.QueryAll<CommonNullTestData>(transaction: t);
        Assert.AreEqual(2, all.Count());


        Assert.AreEqual(1, await sql.CountAsync<CommonNullTestData>(where: x => x.Txt != "t1", transaction: t, trace: new DiagnosticsTracer()));
        Assert.AreEqual(1, await sql.CountAsync<CommonNullTestData>(where: x => x.Nr != 10, transaction: t));

        Assert.AreEqual(0, await sql.CountAsync<CommonNullTestData>(where: x => x.TxtNull != "t2", transaction: t, trace: new DiagnosticsTracer()));
        Assert.AreEqual(0, await sql.CountAsync<CommonNullTestData>(where: x => x.NrNull != 11, transaction: t));

        GlobalConfiguration.Setup(GlobalConfiguration.Options with { ExpressionNullSemantics = ExpressionNullSemantics.NullNotEqual });

        Assert.AreEqual(1, await sql.CountAsync<CommonNullTestData>(where: x => x.TxtNull != "t2", transaction: t, trace: new DiagnosticsTracer()));
        Assert.AreEqual(1, await sql.CountAsync<CommonNullTestData>(where: x => x.NrNull != 11, transaction: t, trace: new DiagnosticsTracer()));

        var storeQueries = new StoreQueryTracer();

        Assert.AreEqual(1, await sql.CountAsync<CommonNullTestData>(where: x => x.Txt != "t1", transaction: t, trace: storeQueries));
        Assert.AreEqual(1, await sql.CountAsync<CommonNullTestData>(where: x => x.Nr != 10, transaction: t, trace: storeQueries));


        Assert.IsFalse(storeQueries.Traces.Any(x => x.Contains("NULL")), string.Join(Environment.NewLine, storeQueries.Traces));


        Assert.AreEqual(1, await sql.CountAsync<CommonNullTestData>(where: x => x.ID == 1 && !(x.Txt == "t5" && x.Nr == 22), transaction: t, trace: new DiagnosticsTracer()));


        sql.Insert(new EnumNullTestData(), trace: new DiagnosticsTracer(), transaction: t);


        var toDel = new int[] { 1, 2 };
        sql.Delete<CommonNullTestData>(x => toDel.Contains(x.NrNull.Value), transaction: t, trace: new DiagnosticsTracer());

        await t.RollbackAsync();
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

        if (!await sql.SchemaObjectExistsAsync("GuidNullData"))
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

        var t = await sql.BeginTransactionAsync();

        await sql.InsertAllAsync(
            new GuidNullData[]
            {
                new GuidNullData(){ ID = 1, Txt = Guid.NewGuid(), TxtNull = Guid.NewGuid(), Uuid = Guid.NewGuid(), UuidNull=Guid.NewGuid(), BlobData = new byte[]{32}, BlobDataNull = new byte[]{65 } },
                new GuidNullData(){ ID = 2, Txt = Guid.NewGuid(), Uuid = Guid.NewGuid(), BlobData = new byte[]{ 97 } },
            }, transaction: t);

        await t.RollbackAsync();
    }

    [Table("CommonDateTimeNullTestData")]
    class DateTestData
    {
        public int ID { get; set; }
        public DateTime? Txt { get; set; }
        public DateTime? Date { get; set; }
    }

    [Table("CommonDateTimeNullTestData")]
    class DateOffsetTestData
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

        if (!await sql.SchemaObjectExistsAsync("CommonDateTimeNullTestData"))
        {
            await PerformCreateTableAsync(sql, $@"CREATE TABLE [CommonDateTimeNullTestData] (
                        [ID] int NOT NULL,
                        [Txt] {VarCharName}(128) NULL,
                        [Date] {DateTimeDbType} NULL
                )");
        }
        else
        {
            await sql.TruncateAsync(tableName: "CommonDateTimeNullTestData");
        }

        var t = await sql.BeginTransactionAsync();

        await sql.InsertAllAsync(
            new[]
            {
                new DateTestData(){ ID = 1, Txt = new DateTime(2001, 1,1,1,1,1, DateTimeKind.Utc), Date = new DateTime(2002, 1,2,2,2,2, DateTimeKind.Utc)},
                new DateTestData(){ ID = 2, Txt =null, Date = null }
            },
            trace: new DiagnosticsTracer(),
            transaction: t);
        await sql.InsertAllAsync(
            new[]
            {
                new DateOffsetTestData(){ ID = 3, Txt = new DateTimeOffset(2003, 1,3,3,3,3, TimeSpan.Zero), Date = new DateTimeOffset(2004, 1,4,4,4,4, TimeSpan.Zero)},
                new DateOffsetTestData(){ ID = 4, Txt =null, Date = null }
            }, transaction: t);

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

        await t.RollbackAsync();
    }

    public virtual string DateTimeOffsetDbType => "datetimeoffset";

#if NET
    public virtual string TimeOnlyDbType => "time";
    public virtual string DateOnlyDbType => "date";

    record DateTimeOnlyTable
    {
        public TimeOnly TOnly { get; set; }
        public DateOnly DOnly { get; set; }

        public DateTimeOffset DOffset { get; set; }
    }

    [TestMethod]
    public async Task DateTimeOnlyTests()
    {
        using var sql = await CreateOpenConnectionAsync();

        if (!await sql.SchemaObjectExistsAsync(nameof(DateTimeOnlyTable)))
        {
            await PerformCreateTableAsync(sql, $@"CREATE TABLE [{nameof(DateTimeOnlyTable)}] (
                        [TOnly] {TimeOnlyDbType} NOT NULL,
                        [DOnly] {DateOnlyDbType} NOT NULL,
                        [DOffset] {DateTimeOffsetDbType} NOT NULL
            )");
        }
        else
        {
            await sql.TruncateAsync<DateTimeOnlyTable>();
        }

        await sql.InsertAsync(new DateTimeOnlyTable() { DOnly = new DateOnly(2021, 1, 1), TOnly = new TimeOnly(1, 2, 3), DOffset = new DateTimeOffset(2023, 1, 1, 1, 1, 1, TimeSpan.Zero) });

        Assert.IsNotEmpty(await sql.QueryAllAsync<DateTimeOnlyTable>());

        Assert.IsNotEmpty(await sql.ExecuteQueryAsync<DateTimeOnlyTable>(ApplySqlRules(sql, "SELECT * FROM [DateTimeOnlyTable]")));

        var today = DateOnly.FromDateTime(DateTime.Now);
        await sql.QueryAsync<DateTimeOnlyTable>(where: x => x.DOnly < today, trace: new DiagnosticsTracer());

        if (sql.GetType().Name is { } name && (name.Contains("Postgre") || name.Contains("Npgsql")))
            return; // Limitations of PostgreSQL DateTimeOffset

        await sql.QueryAsync<DateTimeOnlyTable>(where: x => x.DOffset < DateTime.Now);
        await sql.QueryAsync<DateTimeOnlyTable>(where: x => x.DOffset < DateTimeOffset.Now);
    }
#endif

    record WithComputed
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

        if (!await sql.SchemaObjectExistsAsync("WithComputed"))
        {
            await PerformCreateTableAsync(sql, $@"CREATE TABLE [WithComputed] (
                        [ID] int NOT NULL,
                        [Writable] {VarCharName}(128) NOT NULL,
                        [Computed] {GeneratedColumnDefinition("CONCAT('--', [Writable])", $"{VarCharName}(130)")}
            )");
        }
        else
        {
            await sql.TruncateAsync<WithComputed>(trace: new DiagnosticsTracer());
        }

        var fields = await DbFieldCache.GetAsync(sql, nameof(WithComputed), transaction: null);
        Assert.AreEqual(true, fields.First(x => x.FieldName == "Computed").IsGenerated);

        await sql.InsertAsync(new WithComputed() { ID = 1, Writable = "a" });

        var r = (await sql.QueryAllAsync<WithComputed>()).FirstOrDefault();

        Assert.AreEqual(1, r.ID);
        Assert.AreEqual("a", r.Writable);
        Assert.AreEqual("--a", r.Computed);

        await sql.QueryAllAsync<WithComputed>(orderBy: [OrderField.Parse<WithComputed>(x => x.Computed, Order.Ascending)]);

        await sql.UpdateAsync<WithComputed>(
            new()
            {
                Writable = "b"
            },
            where: x => x.Computed == "--a",
            trace: new DiagnosticsTracer());
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

        var t = sql.ExecuteQuery<(int v1, string c2)>("SELECT 1, 'a'").FirstOrDefault();

        Assert.AreEqual(1, t.Item1);
        Assert.AreEqual("a", t.Item2);
    }

    record WithGroupByItems
    {
        public int ID { get; set; }
        public string Txt { get; set; }
    }

    [TestMethod]
    public async Task VarPrefix()
    {
        using var sql = await CreateOpenConnectionAsync();

        if (!await sql.SchemaObjectExistsAsync("WithGroupByItems"))
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
            new { a = new string[] { "a" }, aa = 1 }
            );
    }

    class id2record
    {

        public int ID { get; set; }
        public int ID2 { get; set; }
    }
    class id2recordNullable
    {
        public int ID { get; set; }
        public int? ID2 { get; set; }
    }

    const string IntNotNullable = nameof(IntNotNullable);
    [TestMethod]
    public async Task NullableIntError()
    {
        using var sql = await CreateOpenConnectionAsync();

        if (!await sql.SchemaObjectExistsAsync(nameof(IntNotNullable)))
        {
            await PerformCreateTableAsync(sql, $@"CREATE TABLE [{IntNotNullable}] (
                        [ID] int NOT NULL,
                        [ID2] int NOT NULL
            )");
        }

        await sql.InsertAsync(tableName: IntNotNullable, new id2record { ID = 1, ID2 = 2 });

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await sql.InsertAsync(tableName: IntNotNullable, new id2recordNullable { ID = 3, ID2 = null }),
            "Required Nullable<Int32> property ID2 evaluated to null.");
    }


    class FieldLengthTable
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

        if (!await sql.SchemaObjectExistsAsync(nameof(FieldLengthTable)))
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
            await sql.TruncateAsync<FieldLengthTable>();
        }

        var fd = await sql.GetDbHelper().GetFieldsAsync(sql, nameof(FieldLengthTable));

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

        Assert.AreEqual(true, id1?.IsPrimary);
        Assert.AreEqual(true, id2?.IsPrimary);
        Assert.AreEqual(false, val3?.IsPrimary);

        Assert.AreEqual(false, id1?.IsIdentity);
        Assert.AreEqual(false, id2?.IsIdentity);
        Assert.AreEqual(false, val3?.IsIdentity);

        Assert.AreEqual(false, id1?.IsNullable);
        Assert.AreEqual(false, id2?.IsNullable);
        Assert.AreEqual(true, val3?.IsNullable);

        var ftf = new FieldLengthTable[]
        {
            new FieldLengthTable { ID = "a12345678901234567890123456789012345", ID2 = "b12345678901234567890123456789012345", VAL3 = "c" },
            new FieldLengthTable { ID = "d12345678901234567890123456789012345", ID2 = "e12345678901234567890123456789012345", VAL3 = null }
        };

        Assert.AreEqual(2, await sql.InsertAllAsync(ftf, trace: new DiagnosticsTracer()));

        var data = (await sql.QueryAllAsync<FieldLengthTable>()).ToArray();

        Assert.AreEqual(2, data.Count());
        Assert.AreEqual(ftf[0].ID, data[0].ID);
    }

    class MorePrimaryKeyTable
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

        if (!await sql.SchemaObjectExistsAsync(nameof(MorePrimaryKeyTable)))
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
            await sql.TruncateAsync<MorePrimaryKeyTable>();
        }

        var fd = await sql.GetDbHelper().GetFieldsAsync(sql, nameof(MorePrimaryKeyTable));

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

        Assert.AreEqual(true, id1?.IsPrimary);
        Assert.AreEqual(true, id2?.IsPrimary);
        Assert.AreEqual(false, val3?.IsPrimary);

        Assert.AreEqual(false, id1?.IsIdentity);
        Assert.AreEqual(true, id2?.IsIdentity);
        Assert.AreEqual(false, val3?.IsIdentity);

        Assert.AreEqual(false, id1?.IsNullable);
        Assert.AreEqual(false, id2?.IsNullable);
        Assert.AreEqual(true, val3?.IsNullable);

        var ftf = new MorePrimaryKeyTable[]
        {
            new MorePrimaryKeyTable { ID = "A", ID2 = 0, Value = "c" },
            new MorePrimaryKeyTable { ID = "B", ID2 = 0, Value = null }
        };

        Assert.AreEqual(2, await sql.InsertAllAsync(ftf, trace: new DiagnosticsTracer()));

        var data = (await sql.QueryAllAsync<MorePrimaryKeyTable>()).ToArray();

        Assert.AreEqual(2, data.Count());
        Assert.AreEqual(ftf[0].ID, data[0].ID);
        Assert.AreEqual(ftf[1].ID, data[1].ID);
        Assert.AreEqual(ftf[0].ID2, data[0].ID2);
        Assert.AreEqual(ftf[1].ID2, data[1].ID2);
        Assert.AreNotEqual(ftf[0].ID2, ftf[1].ID2);
        Assert.AreEqual(ftf[0].Value, data[0].Value);
        Assert.AreEqual(ftf[1].Value, data[1].Value);

        // More smoke tests // TODO: Create separate tests
        Assert.IsTrue(sql.Exists<MorePrimaryKeyTable>(where: x => x.ID == "A" && x.ID2 == data[0].ID2));

        await sql.MergeAsync(ftf[0]);
    }

    [TestMethod]
    public async Task GetRuntimeInfo()
    {
        using var sql = await CreateOpenConnectionAsync();

        var info = await DbRuntimeSettingCache.GetAsync(sql, null);

        Assert.IsNotNull(info);
    }

    [TestMethod]
    public async Task BulkQuery()
    {
        using var sql = await CreateOpenConnectionAsync();


    }

    record MergeEdgeTable
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

            bool noIdentityInPrimaryKey = sql.GetType().Name.Contains("iteConnection") || sql.GetType().Name.Contains("Oracle");

            if (!await sql.SchemaObjectExistsAsync(nameof(MergeEdgeTable)))
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

                await PerformCreateTableAsync(sql, $@"CREATE UNIQUE INDEX [IX_{nameof(MergeEdgeTable)}] ON [{nameof(MergeEdgeTable)}] ([Name]);
        ");
            }
            else
            {
                await sql.TruncateAsync<MergeEdgeTable>();
            }

            var r = new MergeEdgeTable()
            {
                ID = 1,
                Name = "a",
                Value = "b"
            };

            var v1 = await sql.MergeAsync<MergeEdgeTable, int>(r, trace: new DiagnosticsTracer());
            var v2 = await sql.MergeAsync<MergeEdgeTable, int>(r);

            Assert.IsTrue(v1 > 0);
            Assert.AreEqual(v1, v2);

            var all = await sql.QueryAllAsync<MergeEdgeTable>();
            Assert.AreEqual(1, all.Count());

            // Still doing the same thing
            Assert.AreEqual(v2, await sql.MergeAsync<MergeEdgeTable, int>(r, qualifiers: Field.Parse<MergeEdgeTable>(x => new { x.ID, x.Name }), trace: new DiagnosticsTracer()));

            // Now just update the first two fields. No-op
            Assert.AreEqual(v2, await sql.MergeAsync<MergeEdgeTable, int>(r, fields: Field.Parse<MergeEdgeTable>(x => new { x.ID, x.Name }), trace: new DiagnosticsTracer()));

            var r2 = new MergeEdgeTable()
            {
                ID = 2,
                Name = "b",
                Value = "c"
            };

            await sql.MergeAsync(r2);
            await sql.MergeAsync(r2);

            all = await sql.QueryAllAsync<MergeEdgeTable>();
            Assert.AreEqual(2, all.Count());

            await sql.DeleteAsync(r);
            await sql.MergeAsync(r, trace: new DiagnosticsTracer());

            all = await sql.QueryAllAsync<MergeEdgeTable>();
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
                Assert.IsTrue(v2 > 0);
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
}
