using Microsoft.Data.Sqlite;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Setup;
using RepoDb.Trace;

namespace RepoDb.Sqlite.Microsoft.IntegrationTests;

#if NET
[TestClass]
public class TypeTransferTests
{

    [TestInitialize]
    public void Initialize()
    {
        Database.Initialize();
        Cleanup();
        using var connection = new SqliteConnection(Database.ConnectionString);
        {
            AddTables(connection);
        }
    }


    /* Unmerged change from project 'RepoDb.Sqlite.Microsoft.IntegrationTests (net481)'
    Before:
            private static void AddTables(SqliteConnection connection)
            {
                connection.ExecuteNonQuery(
                        @"CREATE TABLE IF NOT EXISTS DateTypesData (
                            ID INTEGER PRIMARY KEY NOT NULL,
                            Bool                INT NOT NULL,
                            BoolNull            INT NULL,
                            Date                DATE NOT NULL,
                            DateNull            DATE NULL,
                            DateTime            DATE NOT NULL,
                            DateTimeNull        DATE NULL,
                            DateTimeOffset      DATE NOT NULL,
                            DateTimeOffsetNull  DATE NULL,
                            Decimal             Decimal NOT NULL,
                            DecimalNull         Decimal NULL
                        )");
    After:
        private static void AddTables(SqliteConnection connection)
        {
            connection.ExecuteNonQuery(
                    @"CREATE TABLE IF NOT EXISTS DateTypesData (
                        ID INTEGER PRIMARY KEY NOT NULL,
                        Bool                INT NOT NULL,
                        BoolNull            INT NULL,
                        Date                DATE NOT NULL,
                        DateNull            DATE NULL,
                        DateTime            DATE NOT NULL,
                        DateTimeNull        DATE NULL,
                        DateTimeOffset      DATE NOT NULL,
                        DateTimeOffsetNull  DATE NULL,
                        Decimal             Decimal NOT NULL,
                        DecimalNull         Decimal NULL
                    )");
    */
    private static void AddTables(SqliteConnection connection)
    {
        connection.ExecuteNonQuery(
                @"CREATE TABLE IF NOT EXISTS DateTypesData (
                        ID INTEGER PRIMARY KEY NOT NULL,
                        Bool                INT NOT NULL,
                        BoolNull            INT NULL,
                        Date                DATE NOT NULL,
                        DateNull            DATE NULL,
                        DateTime            DATE NOT NULL,
                        DateTimeNull        DATE NULL,
                        DateTimeOffset      DATE NOT NULL,
                        DateTimeOffsetNull  DATE NULL,
                        Decimal             Decimal NOT NULL,
                        DecimalNull         Decimal NULL
                    )");



        /* Unmerged change from project 'RepoDb.Sqlite.Microsoft.IntegrationTests (net481)'
        Before:
                    connection.ExecuteNonQuery(
                            @"CREATE TABLE IF NOT EXISTS EnumTypesData (
                                ID INTEGER PRIMARY KEY NOT NULL,
                                EnumInt             INT NOT NULL,
                                EnumIntNull         INT NULL,
                                EnumString          TEXT NOT NULL,
                                EnumStringNull      TEXT NULL,
                                EnumVarChar         VarChar NOT NULL,
                                EnumVarCharNull     VarChar NULL
                            )");
                }
        After:
                connection.ExecuteNonQuery(
                        @"CREATE TABLE IF NOT EXISTS EnumTypesData (
                            ID INTEGER PRIMARY KEY NOT NULL,
                            EnumInt             INT NOT NULL,
                            EnumIntNull         INT NULL,
                            EnumString          TEXT NOT NULL,
                            EnumStringNull      TEXT NULL,
                            EnumVarChar         VarChar NOT NULL,
                            EnumVarCharNull     VarChar NULL
                        )");
            }
        */
        connection.ExecuteNonQuery(
                @"CREATE TABLE IF NOT EXISTS EnumTypesData (
                        ID INTEGER PRIMARY KEY NOT NULL,
                        EnumInt             INT NOT NULL,
                        EnumIntNull         INT NULL,
                        EnumString          TEXT NOT NULL,
                        EnumStringNull      TEXT NULL,
                        EnumVarChar         VarChar NOT NULL,
                        EnumVarCharNull     VarChar NULL
                    )");


        connection.Truncate<DateTypesData>();
        connection.Truncate<EnumTypesData>();
    }

    public record DateTypesData
    {
        [Primary, Identity]
        public int ID { get; set; }

        public bool Bool { get; set; }
        public bool? BoolNull { get; set; }

        public DateOnly Date { get; set; }
        public DateOnly? DateNull { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime? DateTimeNull { get; set; }

        public DateTimeOffset DateTimeOffset { get; set; }
        public DateTimeOffset? DateTimeOffsetNull { get; set; }

        public decimal Decimal { get; set; }
        public decimal? DecimalNull { get; set; }
    }

    public enum Wind
    {
        North,
        East,
        South,
        West
    }

    public enum Current
    {
        AC,
        DC,
        SomeC
    }

    public enum VAT
    {
        None,
        High,
        Low,
        Exempt
    }

    public record EnumTypesData
    {
        [Primary, Identity]
        public int ID { get; set; }
        public Wind EnumInt { get; set; }
        public Wind? EnumIntNull { get; set; }
        public Current EnumString { get; set; }
        public Current? EnumStringNull { get; set; }

        public VAT EnumVarChar { get; set; }
        public VAT? EnumVarCharNull { get; set; }
    }

    [TestCleanup]
    public void Cleanup()
    {
        Database.Cleanup();
    }

    [TestMethod]
    public void BoolDateDecimalInsertRead()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        {
            AddTables(connection);

            var src = new DateTypesData
            {
                Bool = true,
                BoolNull = false,
                Date = DateOnly.FromDateTime(new DateTime(2000, 1, 1)),
                DateNull = DateOnly.FromDateTime(new DateTime(2001, 2, 2)),
                DateTime = new DateTime(2004, 3, 3),
                DateTimeNull = new DateTime(2004, 4, 4),
                DateTimeOffset = new DateTimeOffset(2010, 5, 5, 0, 0, 0, TimeSpan.Zero),
                DateTimeOffsetNull = new DateTimeOffset(2011, 6, 6, 0, 0, 0, TimeSpan.Zero),
                Decimal = 2345m,
                DecimalNull = 6789m
            };


            /* Unmerged change from project 'RepoDb.Sqlite.Microsoft.IntegrationTests (net481)'
            Before:
                            var src = new DateTypesData
                            {
                                Bool = true,
                                BoolNull = false,
                                Date = DateOnly.FromDateTime(new DateTime(2000, 1, 1)),
                                DateNull = DateOnly.FromDateTime(new DateTime(2001, 2, 2)),
                                DateTime = new DateTime(2004, 3, 3),
                                DateTimeNull = new DateTime(2004, 4, 4),
                                DateTimeOffset = new DateTimeOffset(2010, 5, 5, 0, 0, 0, TimeSpan.Zero),
                                DateTimeOffsetNull = new DateTimeOffset(2011, 6, 6, 0, 0, 0, TimeSpan.Zero),
                                Decimal = 2345m,
                                DecimalNull = 6789m
                            };

                            connection.Insert(src);
            After:
                        connection.Insert(src);
            */
            connection.Insert(src);


            var item = connection.QueryAll<DateTypesData>().Single();


            /* Unmerged change from project 'RepoDb.Sqlite.Microsoft.IntegrationTests (net481)'
            Before:
                            var item = connection.QueryAll<DateTypesData>().Single();

                            Assert.AreEqual(src, item);

                            src = src with
                            {
                                BoolNull = null,
                                DateNull = null,
                                DateTimeNull = null,
                                DateTimeOffsetNull = null,
                                DecimalNull = null,
                            };

                            connection.Update(src);
            After:
                        var item = connection.QueryAll<DateTypesData>().Single();

                        Assert.AreEqual(src, item);

                        src = src with
                        {
                            BoolNull = null,
                            DateNull = null,
                            DateTimeNull = null,
                            DateTimeOffsetNull = null,
                            DecimalNull = null,
                        };

                        connection.Update(src);
            */
            Assert.AreEqual(src, item);

            src = src with
            {
                BoolNull = null,
                DateNull = null,
                DateTimeNull = null,
                DateTimeOffsetNull = null,
                DecimalNull = null,
            };

            connection.Update(src);

            var item2 = connection.QueryAll<DateTypesData>().Single();

            Assert.AreEqual(src, item2);
        }
    }

    [TestMethod]
    public void EnumInsertRead()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        {
            AddTables(connection);

            var src = new EnumTypesData
            {
                EnumInt = Wind.South,
                EnumIntNull = Wind.East,
                EnumString = Current.AC,
                EnumStringNull = Current.DC,
                EnumVarChar = VAT.High,
                EnumVarCharNull = VAT.Exempt
            };

            connection.Insert(src, trace: new DiagnosticsTracer());


            var item = connection.QueryAll<EnumTypesData>().Single();

            Assert.AreEqual(src, item);

            src = src with
            {
                EnumIntNull = null,
                EnumStringNull = null,
                EnumVarCharNull = null
            };

            connection.Update(src);

            var item2 = connection.QueryAll<EnumTypesData>().Single();

            Assert.AreEqual(src, item2);
        }
    }

    [TestMethod]
    public void EnumCompareValue()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        {
            AddTables(connection);

            var src = new EnumTypesData
            {
                EnumInt = Wind.South,
                EnumIntNull = Wind.East,
                EnumString = Current.AC,
                EnumStringNull = Current.DC,
                EnumVarChar = VAT.High,
                EnumVarCharNull = VAT.Exempt
            };

            connection.Insert(src);

            var item = connection.QueryAll<EnumTypesData>().Single();

            Assert.IsNotNull(connection.Query<EnumTypesData>(x => x.EnumInt == Wind.South).Single());
            Assert.IsNotNull(connection.Query<EnumTypesData>(x => x.EnumIntNull == Wind.East).Single());
            var r = connection.Query<EnumTypesData>(x => x.EnumString == Current.AC);
            Assert.IsNotNull(connection.Query<EnumTypesData>(x => x.EnumString == Current.AC, trace: new DiagnosticsTracer()).Single());
            Assert.IsNotNull(connection.Query<EnumTypesData>(x => x.EnumStringNull == Current.DC).Single());
            Assert.IsNotNull(connection.Query<EnumTypesData>(x => x.EnumVarChar == VAT.High).Single());
            Assert.IsNotNull(connection.Query<EnumTypesData>(x => x.EnumVarCharNull == VAT.Exempt).Single());
        }
    }

    [TestMethod]
    public void EnumCompareNull()
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        {
            AddTables(connection);

            var src = new EnumTypesData
            {
                EnumInt = Wind.South,
                EnumString = Current.AC,
                EnumVarChar = VAT.High,
            };

            connection.Insert(src, trace: new DiagnosticsTracer());


            var r = connection.Query<EnumTypesData>(where: x => x.EnumVarCharNull == null, trace: new DiagnosticsTracer());
            Assert.AreEqual(1, r.Count());

            r = connection.Query<EnumTypesData>(where: x => x.EnumVarCharNull != null, trace: new DiagnosticsTracer());
            Assert.AreEqual(0, r.Count());

            r = connection.Query<EnumTypesData>(where: x => (x.EnumVarCharNull ?? 0) != VAT.High, trace: new DiagnosticsTracer());
            Assert.AreEqual(1, r.Count());

            r = connection.Query<EnumTypesData>(where: x => (x.EnumVarCharNull ?? VAT.None) == VAT.None, trace: new DiagnosticsTracer());
            Assert.AreEqual(1, r.Count());


            // Now test strict boolean implementation
            r = connection.Query<EnumTypesData>(where: x => x.EnumVarCharNull != VAT.None, trace: new DiagnosticsTracer());
            Assert.AreEqual(0, r.Count());

            r = connection.Query<EnumTypesData>(where: x => x.EnumVarCharNull != VAT.None || x.EnumIntNull != Wind.North, trace: new DiagnosticsTracer());
            Assert.AreEqual(0, r.Count());

            r = connection.Query<EnumTypesData>(where: x => !(x.EnumVarCharNull == VAT.None), trace: new DiagnosticsTracer());
            Assert.AreEqual(0, r.Count());

            r = connection.Query<EnumTypesData>(where: x => !(x.EnumVarCharNull == VAT.None && x.EnumIntNull == Wind.North), trace: new DiagnosticsTracer());
            Assert.AreEqual(0, r.Count());

            GlobalConfiguration.Setup(GlobalConfiguration.Options with { ExpressionNullSemantics = ExpressionNullSemantics.NullNotEqual });

            r = connection.Query<EnumTypesData>(where: x => x.EnumVarCharNull != VAT.None, trace: new DiagnosticsTracer());
            Assert.AreEqual(1, r.Count());

            r = connection.Query<EnumTypesData>(where: x => x.EnumVarCharNull != VAT.None || x.EnumIntNull != Wind.North, trace: new DiagnosticsTracer());
            Assert.AreEqual(1, r.Count());

            r = connection.Query<EnumTypesData>(where: x => !(x.EnumVarCharNull == VAT.None), trace: new DiagnosticsTracer());
            Assert.AreEqual(1, r.Count());

            r = connection.Query<EnumTypesData>(where: x => !(x.EnumVarCharNull == VAT.None && x.EnumIntNull == Wind.North), trace: new DiagnosticsTracer());
            Assert.AreEqual(1, r.Count());
        }

    }
}
#endif
