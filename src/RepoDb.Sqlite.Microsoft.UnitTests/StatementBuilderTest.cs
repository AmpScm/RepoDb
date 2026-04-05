using Microsoft.Data.Sqlite;
using RepoDb.Enumerations;
using RepoDb.Exceptions;

namespace RepoDb.Sqlite.Microsoft.UnitTests;

[TestClass]
public class StatementBuilderTest
{
    [TestInitialize]
    public void Initialize()
    {
        GlobalConfiguration
            .Setup()
            .UseSqlite();
    }

    #region CreateBatchQuery

    [TestMethod]
    public void TestMsSqliteStatementBuilderCreateBatchQuery()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<SqliteConnection>();

        // Act
        var query = builder.CreateBatchQuery("Table",
            Field.From("Id", "Name"),
            0,
            10,
            OrderField.Parse(new { Id = Order.Ascending }));
        var expected = "SELECT [Id], [Name] FROM [Table] ORDER BY [Id] ASC LIMIT 10;";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestMsSqliteStatementBuilderCreateBatchQueryWithPage()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<SqliteConnection>();

        // Act
        var query = builder.CreateBatchQuery("Table",
            Field.From("Id", "Name"),
            3,
            10,
            OrderField.Parse(new { Id = Order.Ascending }));
        var expected = "SELECT [Id], [Name] FROM [Table] ORDER BY [Id] ASC LIMIT 30, 10;";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void ThrowExceptionOnMsSqliteStatementBuilderCreateBatchQueryIfThereAreNoFields()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<SqliteConnection>();

        // Act
        Assert.ThrowsExactly<ArgumentNullException>(() => builder.CreateBatchQuery("Table",
            null,
            0,
            10,
            OrderField.Parse(new { Id = Order.Ascending })));
    }

    [TestMethod]
    public void ThrowExceptionOnMsSqliteStatementBuilderCreateBatchQueryIfThePageValueIsNullOrOutOfRange()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<SqliteConnection>();

        // Act
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => builder.CreateBatchQuery("Table",
            Field.From("Id", "Name"),
            -1,
            10,
            OrderField.Parse(new { Id = Order.Ascending })));
    }

    [TestMethod]
    public void ThrowExceptionOnMsSqliteStatementBuilderCreateBatchQueryIfTheRowsPerBatchValueIsNullOrOutOfRange()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<SqliteConnection>();

        // Act
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => builder.CreateBatchQuery("Table",
            Field.From("Id", "Name"),
            0,
            -1,
            OrderField.Parse(new { Id = Order.Ascending })));
    }

    [TestMethod]
    public void ThrowExceptionOnMsSqliteStatementBuilderCreateBatchQueryIfThereAreHints()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<SqliteConnection>();

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
    public void TestMsSqliteStatementBuilderCreateExists()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<SqliteConnection>();

        // Act
        var query = builder.CreateExists("Table",
            QueryGroup.Parse(new { Id = 1 }));
        var expected = "SELECT 1 AS [ExistsValue] FROM [Table] WHERE ([Id] = @Id) LIMIT 1;";

        // Assert
        Assert.AreEqual(expected, query);
    }

    #endregion

    #region CreateInsert

    [TestMethod]
    public void TestMsSqliteStatementBuilderCreateInsert()
    {
        //GlobalConfiguration.Setup(GlobalConfiguration.Options with { ConversionType = conversionType });
        // Setup
        var builder = StatementBuilderMapper.Get<SqliteConnection>();

        // Act
        var query = builder.CreateInsert("Table",
            Field.From("Id", "Name", "Address"),
            primaryField: null,
            null);
        var expected = "INSERT INTO [Table] ([Id], [Name], [Address]) VALUES (@Id, @Name, @Address);";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestMsSqliteStatementBuilderCreateInsertWithPrimary()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<SqliteConnection>();

        // Act
        var query = builder.CreateInsert("Table",
            Field.From("Id", "Name", "Address"),
            new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
            null);
        var expected = "INSERT INTO [Table] ([Id], [Name], [Address]) VALUES (@Id, @Name, @Address) "
            + "RETURNING [Id];";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestMsSqliteStatementBuilderCreateInsertWithIdentity()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<SqliteConnection>();

        // Act
        var query = builder.CreateInsert("Table",
            Field.From("Id", "Name", "Address"),
            null,
            new DbField("Id", false, true, false, typeof(int), null, null, null, null, false));
        var expected = "INSERT INTO [Table] ([Name], [Address]) VALUES (@Name, @Address) "
            + "RETURNING [Id];";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void ThrowExceptionOnMsSqliteStatementBuilderCreateInsertIfThereAreHints()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<SqliteConnection>();

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
    public void TestMsSqliteStatementBuilderCreateInsertAll()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<SqliteConnection>();

        // Act
        var query = builder.CreateInsertAll("Table",
            Field.From("Id", "Name", "Address"),
            3,
            primaryField: null,
            null);
        var expected = "INSERT INTO [Table] ([Id], [Name], [Address]) VALUES (@Id, @Name, @Address), (@Id_1, @Name_1, @Address_1), (@Id_2, @Name_2, @Address_2);";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestMsSqliteStatementBuilderCreateInsertAllWithPrimary()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<SqliteConnection>();

        // Act
        var query = builder.CreateInsertAll("Table",
            Field.From("Id", "Name", "Address"),
            3,
            new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
            null);
        var expected = "INSERT INTO [Table] ([Id], [Name], [Address]) VALUES (@Id, @Name, @Address)," +
            " (@Id_1, @Name_1, @Address_1), (@Id_2, @Name_2, @Address_2) " +
            "RETURNING [Id];";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestMsSqliteStatementBuilderCreateInsertAllWithIdentity()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<SqliteConnection>();

        // Act
        var query = builder.CreateInsertAll("Table",
            Field.From("Id", "Name", "Address"),
            3,
            null,
            new DbField("Id", false, true, false, typeof(int), null, null, null, null, false));
        var expected = "INSERT INTO [Table] ([Name], [Address]) VALUES (@Name, @Address), (@Name_1, @Address_1), (@Name_2, @Address_2) "
            + "RETURNING [Id];";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void ThrowExceptionOnMsSqliteStatementBuilderCreateInsertAllIfThereAreHints()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<SqliteConnection>();

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
    //public void TestMsSqliteStatementBuilderCreateMergeAll()
    //{
    //    // Setup
    //    var builder = StatementBuilderMapper.Get<SqliteConnection>();

    //    // Act
    //    var query = builder.CreateMergeAll(new QueryBuilder(),
    //        "Table",
    //        Field.From("Id", "Name", "Address"),
    //        null,
    //        3,
    //        new DbField("Id", true, false, false, typeof(int), null, null, null, null),
    //        null);
    //    var expected = "INSERT OR REPLACE INTO [Table] ([Id], [Name], [Address]) VALUES (@Id, @Name, @Address ) ; SELECT CAST(@Id AS BIGINT) AS [Result] ; " +
    //        "INSERT OR REPLACE INTO [Table] ([Id], [Name], [Address]) VALUES (@Id_1, @Name_1, @Address_1 ) ; SELECT CAST(@Id_1 AS BIGINT) AS [Result] ; " +
    //        "INSERT OR REPLACE INTO [Table] ([Id], [Name], [Address]) VALUES (@Id_2, @Name_2, @Address_2 ) ; SELECT CAST(@Id_2 AS BIGINT) AS [Result];";

    //    // Assert
    //    Assert.AreEqual(expected, query);
    //}

    //[TestMethod]
    //public void TestMsSqliteStatementBuilderCreateMergeAllWithPrimaryAsQualifier()
    //{
    //    // Setup
    //    var builder = StatementBuilderMapper.Get<SqliteConnection>();

    //    // Act
    //    var query = builder.CreateMergeAll(new QueryBuilder(),
    //        "Table",
    //        Field.From("Id", "Name", "Address"),
    //        Field.From("Id"),
    //        3,
    //        new DbField("Id", true, false, false, typeof(int), null, null, null, null),
    //        null);
    //    var expected = "INSERT OR REPLACE INTO [Table] ([Id], [Name], [Address]) VALUES (@Id, @Name, @Address ) ; SELECT CAST(@Id AS BIGINT) AS [Result] ; " +
    //        "INSERT OR REPLACE INTO [Table] ([Id], [Name], [Address]) VALUES (@Id_1, @Name_1, @Address_1 ) ; SELECT CAST(@Id_1 AS BIGINT) AS [Result] ; " +
    //        "INSERT OR REPLACE INTO [Table] ([Id], [Name], [Address]) VALUES (@Id_2, @Name_2, @Address_2 ) ; SELECT CAST(@Id_2 AS BIGINT) AS [Result];";

    //    // Assert
    //    Assert.AreEqual(expected, query);
    //}

    //[TestMethod]
    //public void TestMsSqliteStatementBuilderCreateMergeAllWithIdentity()
    //{
    //    // Setup
    //    var builder = StatementBuilderMapper.Get<SqliteConnection>();

    //    // Act
    //    var query = builder.CreateMergeAll(new QueryBuilder(),
    //        "Table",
    //        Field.From("Id", "Name", "Address"),
    //        null,
    //        3,
    //        new DbField("Id", true, false, false, typeof(int), null, null, null, null),
    //        new DbField("Id", false, true, false, typeof(int), null, null, null, null));
    //    var expected = "INSERT OR REPLACE INTO [Table] ([Id], [Name], [Address]) VALUES (@Id, @Name, @Address ) ; SELECT CAST(COALESCE(last_insert_rowid(), @Id) AS INT) AS [Result] ; " +
    //        "INSERT OR REPLACE INTO [Table] ([Id], [Name], [Address]) VALUES (@Id_1, @Name_1, @Address_1 ) ; SELECT CAST(COALESCE(last_insert_rowid(), @Id_1) AS INT) AS [Result] ; " +
    //        "INSERT OR REPLACE INTO [Table] ([Id], [Name], [Address]) VALUES (@Id_2, @Name_2, @Address_2 ) ; SELECT CAST(COALESCE(last_insert_rowid(), @Id_2) AS INT) AS [Result];";

    //    // Assert
    //    Assert.AreEqual(expected, query);
    //}

    //[TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
    //public void ThrowExceptionOnMsSqliteStatementBuilderCreateMergeAllIfThereIsNoPrimary()
    //{
    //    // Setup
    //    var builder = StatementBuilderMapper.Get<SqliteConnection>();

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
    //public void ThrowExceptionOnMsSqliteStatementBuilderCreateMergeAllIfThereAreNoFields()
    //{
    //    // Setup
    //    var builder = StatementBuilderMapper.Get<SqliteConnection>();

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
    //public void ThrowExceptionOnMsSqliteStatementBuilderCreateMergeAllIfThereAreOtherFieldsAsQualifers()
    //{
    //    // Setup
    //    var builder = StatementBuilderMapper.Get<SqliteConnection>();

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
    //public void ThrowExceptionOnMsSqliteStatementBuilderCreateMergeAllIfThereAreHints()
    //{
    //    // Setup
    //    var builder = StatementBuilderMapper.Get<SqliteConnection>();

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
    public void TestMsSqliteStatementBuilderCreateQuery()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<SqliteConnection>();

        // Act
        var query = builder.CreateQuery("Table",
            Field.From("Id", "Name", "Address"),
            null,
            null, 0);
        var expected = "SELECT [Id], [Name], [Address] FROM [Table];";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestMsSqliteStatementBuilderCreateQueryWithExpression()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<SqliteConnection>();

        // Act
        var query = builder.CreateQuery("Table",
            Field.From("Id", "Name", "Address"),
            QueryGroup.Parse(new { Id = 1, Name = "Michael" }),
            null, 0);
        var expected = "SELECT [Id], [Name], [Address] FROM [Table] WHERE ([Id] = @Id AND [Name] = @Name);";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestMsSqliteStatementBuilderCreateQueryWithTop()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<SqliteConnection>();

        // Act
        var query = builder.CreateQuery("Table",
            Field.From("Id", "Name", "Address"),
            null,
            null,
            0, 10,
            null);
        var expected = "SELECT [Id], [Name], [Address] FROM [Table] LIMIT 10;";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestMsSqliteStatementBuilderCreateQueryOrderBy()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<SqliteConnection>();

        // Act
        var query = builder.CreateQuery("Table",
            Field.From("Id", "Name", "Address"),
            null,
            OrderField.Parse(new { Id = Order.Ascending }), 0);
        var expected = "SELECT [Id], [Name], [Address] FROM [Table] ORDER BY [Id] ASC;";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestMsSqliteStatementBuilderCreateQueryOrderByFields()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<SqliteConnection>();

        // Act
        var query = builder.CreateQuery("Table",
            Field.From("Id", "Name", "Address"),
            null,
            OrderField.Parse(new { Id = Order.Ascending, Name = Order.Ascending }), 0);
        var expected = "SELECT [Id], [Name], [Address] FROM [Table] ORDER BY [Id] ASC, [Name] ASC;";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestMsSqliteStatementBuilderCreateQueryOrderByDescending()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<SqliteConnection>();

        // Act
        var query = builder.CreateQuery("Table",
            Field.From("Id", "Name", "Address"),
            null,
            OrderField.Parse(new { Id = Order.Descending }), 0);
        var expected = "SELECT [Id], [Name], [Address] FROM [Table] ORDER BY [Id] DESC;";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestMsSqliteStatementBuilderCreateQueryOrderByFieldsDescending()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<SqliteConnection>();

        // Act
        var query = builder.CreateQuery("Table",
            Field.From("Id", "Name", "Address"),
            null,
            OrderField.Parse(new { Id = Order.Descending, Name = Order.Descending }), 0);
        var expected = "SELECT [Id], [Name], [Address] FROM [Table] ORDER BY [Id] DESC, [Name] DESC;";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestMsSqliteStatementBuilderCreateQueryOrderByFieldsMultiDirection()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<SqliteConnection>();

        // Act
        var query = builder.CreateQuery("Table",
            Field.From("Id", "Name", "Address"),
            null,
            OrderField.Parse(new { Id = Order.Ascending, Name = Order.Descending }), 0);
        var expected = "SELECT [Id], [Name], [Address] FROM [Table] ORDER BY [Id] ASC, [Name] DESC;";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void ThrowExceptionOnMsSqliteStatementBuilderCreateQueryIfThereAreHints()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<SqliteConnection>();

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
    public void TestMsSqliteStatementBuilderCreateSkipQuery()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<SqliteConnection>();

        // Act
        var query = builder.CreateSkipQuery("Table",
            Field.From("Id", "Name"),
            0,
            10,
            OrderField.Parse(new { Id = Order.Ascending }));
        var expected = "SELECT [Id], [Name] FROM [Table] ORDER BY [Id] ASC LIMIT 10;";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestMsSqliteStatementBuilderCreateSkipQueryWithSkip()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<SqliteConnection>();

        // Act
        var query = builder.CreateSkipQuery("Table",
            Field.From("Id", "Name"),
            30,
            10,
            OrderField.Parse(new { Id = Order.Ascending }));
        var expected = "SELECT [Id], [Name] FROM [Table] ORDER BY [Id] ASC LIMIT 30, 10;";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void ThrowExceptionOnMsSqliteStatementBuilderSkipBatchQueryIfThereAreNoFields()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<SqliteConnection>();

        // Act
        Assert.ThrowsExactly<ArgumentNullException>(() => builder.CreateSkipQuery("Table",
            null,
            0,
            10,
            OrderField.Parse(new { Id = Order.Ascending })));
    }

    [TestMethod]
    public void ThrowExceptionOnMsSqliteStatementBuilderCreateSkipQueryIfThePageValueIsNullOrOutOfRange()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<SqliteConnection>();

        // Act
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => builder.CreateSkipQuery("Table",
            Field.From("Id", "Name"),
            -1,
            10,
            OrderField.Parse(new { Id = Order.Ascending })));
    }

    [TestMethod]
    public void ThrowExceptionOnMsSqliteStatementBuilderCreateSkipQueryIfTheRowsPerBatchValueIsNullOrOutOfRange()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<SqliteConnection>();

        // Act
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => builder.CreateSkipQuery("Table",
            Field.From("Id", "Name"),
            0,
            -1,
            OrderField.Parse(new { Id = Order.Ascending })));
    }

    [TestMethod]
    public void ThrowExceptionOnMsSqliteStatementBuilderCreateSkipQueryIfThereAreHints()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<SqliteConnection>();

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
    public void TestMsSqliteStatementBuilderCreateTruncate()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<SqliteConnection>();

        // Act
        var query = builder.CreateTruncate("Table");
        var expected = "DELETE FROM [Table];";

        // Assert
        Assert.AreEqual(expected, query);
    }

    #endregion
}
