using Oracle.ManagedDataAccess.Client;
using RepoDb.Enumerations;
using RepoDb.Exceptions;

namespace RepoDb.Oracle.System.UnitTests;

[TestClass]
public class StatementBuilderTest
{
    [TestInitialize]
    public void Initialize()
    {
        GlobalConfiguration
            .Setup()
            .UseOracle();
    }

    #region CreateBatchQuery

    [TestMethod]
    public void TestOracleStatementBuilderCreateBatchQuery()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<OracleConnection>();

        // Act
        var query = builder.CreateBatchQuery("Table",
            Field.From("Id", "Name"),
            0,
            10,
            OrderField.Parse(new { Id = Order.Ascending }));
        var expected = "SELECT \"Id\", \"Name\" FROM \"Table\" ORDER BY \"Id\" ASC FETCH FIRST 10 ROWS ONLY";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestOracleStatementBuilderCreateBatchQueryWithPage()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<OracleConnection>();

        // Act
        var query = builder.CreateBatchQuery("Table",
            Field.From("Id", "Name"),
            3,
            10,
            OrderField.Parse(new { Id = Order.Ascending }));
        var expected = "SELECT \"Id\", \"Name\" FROM \"Table\" ORDER BY \"Id\" ASC OFFSET 30 ROWS FETCH NEXT 10 ROWS ONLY";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void ThrowExceptionOnOracleStatementBuilderCreateBatchQueryIfThereAreNoFields()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<OracleConnection>();

        // Act
        Assert.ThrowsExactly<ArgumentNullException>(() => builder.CreateBatchQuery("Table",
            null,
            0,
            10,
            OrderField.Parse(new { Id = Order.Ascending })));
    }

    [TestMethod]
    public void ThrowExceptionOnOracleStatementBuilderCreateBatchQueryIfThePageValueIsNullOrOutOfRange()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<OracleConnection>();

        // Act
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => builder.CreateBatchQuery("Table",
            Field.From("Id", "Name"),
            -1,
            10,
            OrderField.Parse(new { Id = Order.Ascending })));
    }

    [TestMethod]
    public void ThrowExceptionOnOracleStatementBuilderCreateBatchQueryIfTheRowsPerBatchValueIsNullOrOutOfRange()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<OracleConnection>();

        // Act
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => builder.CreateBatchQuery("Table",
            Field.From("Id", "Name"),
            0,
            -1,
            OrderField.Parse(new { Id = Order.Ascending })));
    }

    [TestMethod]
    public void ThrowExceptionOnOracleStatementBuilderCreateBatchQueryIfThereAreHints()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<OracleConnection>();

        // Act
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => builder.CreateBatchQuery("Table",
            Field.From("Id", "Name"),
            0,
            -1,
            OrderField.Parse(new { Id = Order.Ascending }),
            null,
            "WhatEver"));
    }

    #endregion

    #region CreateExists

    [TestMethod]
    public void TestOracleStatementBuilderCreateExists()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<OracleConnection>();

        // Act
        var query = builder.CreateExists("Table",
            QueryGroup.Parse(new { Id = 1 }));
        var expected = "SELECT 1 AS \"ExistsValue\" FROM \"Table\" WHERE (\"Id\" = :Id) FETCH FIRST 1 ROWS ONLY";

        // Assert
        Assert.AreEqual(expected, query);
    }

    #endregion

    #region CreateInsert

    [TestMethod]
    public void TestOracleStatementBuilderCreateInsert()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<OracleConnection>();

        // Act
        var query = builder.CreateInsert("Table",
            Field.From("Id", "Name", "Address"),
            primaryField: null,
            null);
        var expected = "INSERT INTO \"Table\" (\"Id\", \"Name\", \"Address\") VALUES (:Id, :Name, :Address)";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestOracleStatementBuilderCreateInsertWithPrimary()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<OracleConnection>();

        // Act
        var query = builder.CreateInsert("Table",
            Field.From("Id", "Name", "Address"),
            new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
            null);
        var expected = "INSERT INTO \"Table\" (\"Id\", \"Name\", \"Address\") VALUES (:Id, :Name, :Address) RETURNING \"Id\" INTO :RepoDb_Result";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestOracleStatementBuilderCreateInsertWithIdentity()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<OracleConnection>();

        // Act
        var query = builder.CreateInsert("Table",
            Field.From("Name", "Address"),
            null,
            new DbField("Id", false, true, false, typeof(int), null, null, null, null, false));
        var expected = "INSERT INTO \"Table\" (\"Name\", \"Address\") VALUES (:Name, :Address) "
            + "RETURNING \"Id\" INTO :RepoDb_Result";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void ThrowExceptionOnOracleStatementBuilderCreateInsertIfThereAreHints()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<OracleConnection>();

        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => builder.CreateInsert("Table",
            Field.From("Id", "Name", "Address"),
            null,
            new DbField("Id", false, true, false, typeof(int), null, null, null, null, false),
            "WhatEver"));
    }

    #endregion

    #region CreateInsertAll

    [TestMethod]
    public void TestOracleStatementBuilderCreateInsertAll()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<OracleConnection>();

        // Act
        var query = builder.CreateInsertAll("Table",
            Field.From("Id", "Name", "Address"),
            3,
            primaryField: null,
            null);

        // The "FORALL" is for RepoDB, not for Oracle. This prefix triggers a command update on execute
        var expected = "/*FORALL*/INSERT INTO \"Table\" (\"Id\", \"Name\", \"Address\") VALUES (:Id, :Name, :Address)";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestOracleStatementBuilderCreateInsertAllWithPrimary()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<OracleConnection>();

        // Act
        var query = builder.CreateInsertAll("Table",
            Field.From("Id", "Name", "Address"),
            3,
            new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
            null);
        // The "FORALL" is for RepoDB, not for Oracle. This prefix triggers a command update on execute
        var expected = "/*FORALL*/INSERT INTO \"Table\" (\"Id\", \"Name\", \"Address\") VALUES (:Id, :Name, :Address) RETURNING \"Id\" INTO :RepoDb_Result";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestOracleStatementBuilderCreateInsertAllWithIdentity()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<OracleConnection>();

        // Act
        var query = builder.CreateInsertAll("Table",
            Field.From("Name", "Address"),
            3,
            null,
            new DbField("Id", false, true, false, typeof(int), null, null, null, null, false));
        // The "FORALL" is for RepoDB, not for Oracle. This prefix triggers a command update on execute
        var expected = "/*FORALL*/INSERT INTO \"Table\" (\"Name\", \"Address\") VALUES (:Name, :Address) RETURNING \"Id\" INTO :RepoDb_Result";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void ThrowExceptionOnOracleStatementBuilderCreateInsertAllIfThereAreHints()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<OracleConnection>();

        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => builder.CreateInsertAll("Table",
            Field.From("Id", "Name", "Address"),
            3,
            null,
            new DbField("Id", false, true, false, typeof(int), null, null, null, null, false),
            "WhatEver"));
    }

    #endregion

    #region CreateMerge

    #endregion

    #region CreateMergeAll

    //[TestMethod]
    //public void TestOracleStatementBuilderCreateMergeAll()
    //{
    //    // Setup
    //    var builder = StatementBuilderMapper.Get<OracleConnection>();

    //    // Act
    //    var query = builder.CreateMergeAll(new QueryBuilder(),
    //        "Table",
    //        Field.From("Id", "Name", "Address"),
    //        null,
    //        3,
    //        new DbField("Id", true, false, false, typeof(int), null, null, null, null),
    //        null);
    //    var expected = "INSERT OR REPLACE INTO \"Table\" (\"Id\", \"Name\", \"Address\") VALUES (:Id, :Name, :Address ) ; SELECT CAST(:Id AS BIGINT) AS \"Result\" ; " +
    //        "INSERT OR REPLACE INTO \"Table\" (\"Id\", \"Name\", \"Address\") VALUES (:Id_1, :Name_1, :Address_1 ) ; SELECT CAST(:Id_1 AS BIGINT) AS \"Result\" ; " +
    //        "INSERT OR REPLACE INTO \"Table\" (\"Id\", \"Name\", \"Address\") VALUES (:Id_2, :Name_2, :Address_2 ) ; SELECT CAST(:Id_2 AS BIGINT) AS \"Result\"";

    //    // Assert
    //    Assert.AreEqual(expected, query);
    //}

    //[TestMethod]
    //public void TestOracleStatementBuilderCreateMergeAllWithPrimaryAsQualifier()
    //{
    //    // Setup
    //    var builder = StatementBuilderMapper.Get<OracleConnection>();

    //    // Act
    //    var query = builder.CreateMergeAll(new QueryBuilder(),
    //        "Table",
    //        Field.From("Id", "Name", "Address"),
    //        Field.From("Id"),
    //        3,
    //        new DbField("Id", true, false, false, typeof(int), null, null, null, null),
    //        null);
    //    var expected = "INSERT OR REPLACE INTO \"Table\" (\"Id\", \"Name\", \"Address\") VALUES (:Id, :Name, :Address ) ; SELECT CAST(:Id AS BIGINT) AS \"Result\" ; " +
    //        "INSERT OR REPLACE INTO \"Table\" (\"Id\", \"Name\", \"Address\") VALUES (:Id_1, :Name_1, :Address_1 ) ; SELECT CAST(:Id_1 AS BIGINT) AS \"Result\" ; " +
    //        "INSERT OR REPLACE INTO \"Table\" (\"Id\", \"Name\", \"Address\") VALUES (:Id_2, :Name_2, :Address_2 ) ; SELECT CAST(:Id_2 AS BIGINT) AS \"Result\"";

    //    // Assert
    //    Assert.AreEqual(expected, query);
    //}

    //[TestMethod]
    //public void TestOracleStatementBuilderCreateMergeAllWithIdentity()
    //{
    //    // Setup
    //    var builder = StatementBuilderMapper.Get<OracleConnection>();

    //    // Act
    //    var query = builder.CreateMergeAll(new QueryBuilder(),
    //        "Table",
    //        Field.From("Id", "Name", "Address"),
    //        null,
    //        3,
    //        new DbField("Id", true, false, false, typeof(int), null, null, null, null),
    //        new DbField("Id", false, true, false, typeof(int), null, null, null, null));
    //    var expected = "INSERT OR REPLACE INTO \"Table\" (\"Id\", \"Name\", \"Address\") VALUES (:Id, :Name, :Address ) ; SELECT CAST(COALESCE(last_insert_rowid(), :Id) AS INT) AS \"Result\" ; " +
    //        "INSERT OR REPLACE INTO \"Table\" (\"Id\", \"Name\", \"Address\") VALUES (:Id_1, :Name_1, :Address_1 ) ; SELECT CAST(COALESCE(last_insert_rowid(), :Id_1) AS INT) AS \"Result\" ; " +
    //        "INSERT OR REPLACE INTO \"Table\" (\"Id\", \"Name\", \"Address\") VALUES (:Id_2, :Name_2, :Address_2 ) ; SELECT CAST(COALESCE(last_insert_rowid(), :Id_2) AS INT) AS \"Result\"";

    //    // Assert
    //    Assert.AreEqual(expected, query);
    //}

    //[TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
    //public void ThrowExceptionOnOracleStatementBuilderCreateMergeAllIfThereIsNoPrimary()
    //{
    //    // Setup
    //    var builder = StatementBuilderMapper.Get<OracleConnection>();

    //    // Act
    //    builder.CreateMergeAll(new QueryBuilder(),
    //        "Table",
    //        Field.From("Id", "Name", "Address"),
    //        null,
    //        3,
    //        null,
    //        null);
    //}

    //[TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
    //public void ThrowExceptionOnOracleStatementBuilderCreateMergeAllIfThereAreNoFields()
    //{
    //    // Setup
    //    var builder = StatementBuilderMapper.Get<OracleConnection>();

    //    // Act
    //    builder.CreateMergeAll(new QueryBuilder(),
    //        "Table",
    //        Field.From("Id", "Name", "Address"),
    //        null,
    //        3,
    //        null,
    //        null);
    //}

    //[TestMethod, ExpectedException(typeof(InvalidQualifiersException))]
    //public void ThrowExceptionOnOracleStatementBuilderCreateMergeAllIfThereAreOtherFieldsAsQualifers()
    //{
    //    // Setup
    //    var builder = StatementBuilderMapper.Get<OracleConnection>();

    //    // Act
    //    builder.CreateMergeAll(new QueryBuilder(),
    //        "Table",
    //        Field.From("Id", "Name", "Address"),
    //        Field.From("Id", "Name"),
    //        3,
    //        new DbField("Id", true, false, false, typeof(int), null, null, null, null),
    //        null);
    //}

    //[TestMethod, ExpectedException(typeof(NotSupportedException))]
    //public void ThrowExceptionOnOracleStatementBuilderCreateMergeAllIfThereAreHints()
    //{
    //    // Setup
    //    var builder = StatementBuilderMapper.Get<OracleConnection>();

    //    // Act
    //    builder.CreateMergeAll(new QueryBuilder(),
    //        "Table",
    //        Field.From("Id", "Name", "Address"),
    //        Field.From("Id", "Name"),
    //        3,
    //        new DbField("Id", true, false, false, typeof(int), null, null, null, null),
    //        null,
    //        "WhatEver");
    //}

    #endregion

    #region CreateQuery

    [TestMethod]
    public void TestOracleStatementBuilderCreateQuery()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<OracleConnection>();

        // Act
        var query = builder.CreateQuery("Table",
            Field.From("Id", "Name", "Address"),
            null,
            null, 0);
        var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\"";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestOracleStatementBuilderCreateQueryWithExpression()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<OracleConnection>();

        // Act
        var query = builder.CreateQuery("Table",
            Field.From("Id", "Name", "Address"),
            QueryGroup.Parse(new { Id = 1, Name = "Michael" }));
        var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" WHERE (\"Id\" = :Id AND \"Name\" = :Name)";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestOracleStatementBuilderCreateQueryWithTop()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<OracleConnection>();

        // Act
        var query = builder.CreateQuery("Table",
            Field.From("Id", "Name", "Address"),
            null,
            null,
            0, 10,
            null);
        var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" FETCH FIRST 10 ROWS ONLY";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestOracleStatementBuilderCreateQueryOrderBy()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<OracleConnection>();

        // Act
        var query = builder.CreateQuery("Table",
            Field.From("Id", "Name", "Address"),
            null,
            OrderField.Parse(new { Id = Order.Ascending }), 0);
        var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" ORDER BY \"Id\" ASC";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestOracleStatementBuilderCreateQueryOrderByFields()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<OracleConnection>();

        // Act
        var query = builder.CreateQuery("Table",
            Field.From("Id", "Name", "Address"),
            null,
            OrderField.Parse(new { Id = Order.Ascending, Name = Order.Ascending }), 0);
        var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" ORDER BY \"Id\" ASC, \"Name\" ASC";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestOracleStatementBuilderCreateQueryOrderByDescending()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<OracleConnection>();

        // Act
        var query = builder.CreateQuery("Table",
            Field.From("Id", "Name", "Address"),
            null,
            OrderField.Parse(new { Id = Order.Descending }), 0);
        var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" ORDER BY \"Id\" DESC";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestOracleStatementBuilderCreateQueryOrderByFieldsDescending()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<OracleConnection>();

        // Act
        var query = builder.CreateQuery("Table",
            Field.From("Id", "Name", "Address"),
            null,
            OrderField.Parse(new { Id = Order.Descending, Name = Order.Descending }), 0);
        var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" ORDER BY \"Id\" DESC, \"Name\" DESC";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestOracleStatementBuilderCreateQueryOrderByFieldsMultiDirection()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<OracleConnection>();

        // Act
        var query = builder.CreateQuery("Table",
            Field.From("Id", "Name", "Address"),
            null,
            OrderField.Parse(new { Id = Order.Ascending, Name = Order.Descending }), 0);
        var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" ORDER BY \"Id\" ASC, \"Name\" DESC";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void ThrowExceptionOnOracleStatementBuilderCreateQueryIfThereAreHints()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<OracleConnection>();

        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => builder.CreateQuery("Table",
            Field.From("Id", "Name", "Address"),
            null,
            null,
            0, hints: "WhatEver"));
    }

    #endregion

    #region CreateSkipQuery

    [TestMethod]
    public void TestOracleStatementBuilderCreateSkipQuery()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<OracleConnection>();

        // Act
        var query = builder.CreateSkipQuery("Table",
            Field.From("Id", "Name"),
            0,
            10,
            OrderField.Parse(new { Id = Order.Ascending }));
        var expected = "SELECT \"Id\", \"Name\" FROM \"Table\" ORDER BY \"Id\" ASC FETCH FIRST 10 ROWS ONLY";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestOracleStatementBuilderCreateSkipQueryWithPage()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<OracleConnection>();

        // Act
        var query = builder.CreateSkipQuery("Table",
            Field.From("Id", "Name"),
            30,
            10,
            OrderField.Parse(new { Id = Order.Ascending }));
        var expected = "SELECT \"Id\", \"Name\" FROM \"Table\" ORDER BY \"Id\" ASC OFFSET 30 ROWS FETCH NEXT 10 ROWS ONLY";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void ThrowExceptionOnOracleStatementBuilderCreateSkipQueryIfThereAreNoFields()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<OracleConnection>();

        // Act
        Assert.ThrowsExactly<ArgumentNullException>(() => builder.CreateSkipQuery("Table",
            null,
            0,
            10,
            OrderField.Parse(new { Id = Order.Ascending })));
    }

    [TestMethod]
    public void ThrowExceptionOnOracleStatementBuilderCreateSkipQueryIfThePageValueIsNullOrOutOfRange()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<OracleConnection>();

        // Act
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => builder.CreateSkipQuery("Table",
            Field.From("Id", "Name"),
            -1,
            10,
            OrderField.Parse(new { Id = Order.Ascending })));
    }

    [TestMethod]
    public void ThrowExceptionOnOracleStatementBuilderCreateSkipQueryIfTheRowsPerBatchValueIsNullOrOutOfRange()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<OracleConnection>();

        // Act
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => builder.CreateSkipQuery("Table",
            Field.From("Id", "Name"),
            0,
            -1,
            OrderField.Parse(new { Id = Order.Ascending })));
    }

    [TestMethod]
    public void ThrowExceptionOnOracleStatementBuilderCreateSkipQueryIfThereAreHints()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<OracleConnection>();

        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => builder.CreateSkipQuery("Table",
            Field.From("Id", "Name"),
            0,
            0,
            OrderField.Parse(new { Id = Order.Ascending }),
            null,
            "WhatEver"));
    }

    #endregion

    #region CreateTruncate

    [TestMethod]
    public void TestOracleStatementBuilderCreateTruncate()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<OracleConnection>();

        // Act
        var query = builder.CreateTruncate("Table");
        var expected = "TRUNCATE TABLE \"Table\"";

        // Assert
        Assert.AreEqual(expected, query);
    }

    #endregion
}
