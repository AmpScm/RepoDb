using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Exceptions;

namespace RepoDb.SqlServer.Tests.UnitTests;

[TestClass]
[DoNotParallelize]
public class StatementBuilderTest
{
    [TestInitialize]
    public void Initialize()
    {
        GlobalConfiguration
            .Setup()
            .UseSqlServer();
    }

    #region CreateBatchQuery

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateBatchQueryFirstBatch()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2"]);
        var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

        // Act
        var actual = statementBuilder.CreateBatchQuery(tableName: tableName,
            fields: fields,
            page: 0,
            rowsPerBatch: 10,
            orderBy: orderBy,
            where: null);
        var expected =
            "SELECT [Field1], [Field2] " +
            "FROM [Table] " +
            "ORDER BY [Field1] ASC " +
            "OFFSET 0 " +
            "ROWS FETCH NEXT 10 ROWS ONLY;";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateBatchQuerySecondBatch()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2"]);
        var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

        // Act
        var actual = statementBuilder.CreateBatchQuery(tableName: tableName,
            fields: fields,
            page: 1,
            rowsPerBatch: 10,
            orderBy: orderBy,
            where: null);
        var expected =
            "SELECT [Field1], [Field2] " +
            "FROM [Table] " +
            "ORDER BY [Field1] ASC " +
            "OFFSET 10 " +
            "ROWS FETCH NEXT 10 ROWS ONLY;";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateBatchQueryWithHints()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "[dbo].[Table]";
        var fields = Field.From(["Field1", "Field2"]);
        var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

        // Act
        var actual = statementBuilder.CreateBatchQuery(tableName: tableName,
            fields: fields,
            page: 0,
            rowsPerBatch: 10,
            orderBy: orderBy,
            where: null,
            hints: SqlServerTableHints.NoLock);
        var expected =
            "SELECT [Field1], [Field2] " +
            "FROM [dbo].[Table] WITH (NOLOCK) " +
            "ORDER BY [Field1] ASC " +
            "OFFSET 0 " +
            "ROWS FETCH NEXT 10 ROWS ONLY;";
        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateBatchQueryWithQuotedTableSchema()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "[dbo].[Table]";
        var fields = Field.From(["Field1", "Field2"]);
        var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

        // Act
        var actual = statementBuilder.CreateBatchQuery(tableName: tableName,
            fields: fields,
            page: 0,
            rowsPerBatch: 10,
            orderBy: orderBy,
            where: null);
        var expected =
            "SELECT [Field1], [Field2] " +
            "FROM [dbo].[Table] " +
            "ORDER BY [Field1] ASC " +
            "OFFSET 0 " +
            "ROWS FETCH NEXT 10 ROWS ONLY;";
        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateBatchQueryWithUnquotedTableSchema()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "dbo.Table";
        var fields = Field.From(["Field1", "Field2"]);
        var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

        // Act
        var actual = statementBuilder.CreateBatchQuery(tableName: tableName,
            fields: fields,
            page: 0,
            rowsPerBatch: 10,
            orderBy: orderBy,
            where: null);
        var expected =
            "SELECT [Field1], [Field2] " +
            "FROM [dbo].[Table] " +
            "ORDER BY [Field1] ASC " +
            "OFFSET 0 " +
            "ROWS FETCH NEXT 10 ROWS ONLY;";
        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateBatchQueryWithWhereExpression()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2"]);
        var where = new QueryGroup(new QueryField("Field1", Operation.NotEqual, 1));
        var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

        // Act
        var actual = statementBuilder.CreateBatchQuery(tableName: tableName,
            fields: fields,
            page: 1,
            rowsPerBatch: 10,
            orderBy: orderBy,
            where: where);
        var expected =
            "SELECT [Field1], [Field2] " +
            "FROM [Table] " +
            "WHERE ([Field1] <> @Field1) " +
            "ORDER BY [Field1] ASC " +
            "OFFSET 10 " +
            "ROWS FETCH NEXT 10 ROWS ONLY;";
        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateBatchQueryWithWhereExpressionUniqueField()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2"]);
        var where = new QueryGroup(new QueryField("Id", Operation.NotEqual, 1));
        var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

        // Act
        var actual = statementBuilder.CreateBatchQuery(tableName: tableName,
            fields: fields,
            page: 1,
            rowsPerBatch: 10,
            orderBy: orderBy,
            where: where);
        var expected =
            "SELECT [Field1], [Field2] " +
            "FROM [Table] " +
            "WHERE ([Id] <> @Id) " +
            "ORDER BY [Field1] ASC " +
            "OFFSET 10 " +
            "ROWS FETCH NEXT 10 ROWS ONLY;";
        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ThrowExceptionOnSqlServerStatementBuilderCreateBatchQueryIfTheTableIsNull()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        string? tableName = null;
        var fields = Field.From(["Field1", "Field2"]);

        // Act/Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => statementBuilder.CreateBatchQuery(tableName: tableName,
            fields: fields,
            page: 0,
            rowsPerBatch: 10,
            orderBy: null,
            where: null));
    }

    [TestMethod]
    public void ThrowExceptionOnSqlServerStatementBuilderCreateBatchQueryIfTheTableIsEmpty()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "";
        var fields = Field.From(["Field1", "Field2"]);

        // Act/Assert
        Assert.Throws<ArgumentException>(
        () => statementBuilder.CreateBatchQuery(tableName: tableName,
            fields: fields,
            page: 0,
            rowsPerBatch: 10,
            orderBy: null,
            where: null));
    }

    [TestMethod]
    public void ThrowExceptionOnSqlServerStatementBuilderCreateBatchQueryIfTheTableIsWhitespace()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = " ";
        var fields = Field.From(["Field1", "Field2"]);

        // Act/Assert
        Assert.Throws<ArgumentException>(
        () => statementBuilder.CreateBatchQuery(tableName: tableName,
            fields: fields,
            page: 0,
            rowsPerBatch: 10,
            orderBy: null,
            where: null));
    }

    [TestMethod]
    public void ThrowExceptionOnSqlServerStatementBuilderCreateBatchQueryIfTheFieldsAreNull()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

        // Act/Assert
        Assert.ThrowsExactly<MissingFieldsException>(() => statementBuilder.CreateBatchQuery(tableName: tableName,
            fields: null,
            page: 0,
            rowsPerBatch: 10,
            orderBy: orderBy,
            where: null));
    }

    [TestMethod]
    public void ThrowExceptionOnSqlServerStatementBuilderCreateBatchQueryIfThereAreNoOrderFields()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2"]);

        // Act/Assert
        Assert.ThrowsExactly<EmptyException>(() => statementBuilder.CreateBatchQuery(tableName: tableName,
            fields: fields,
            page: 0,
            rowsPerBatch: 10,
            orderBy: null,
            where: null));
    }

    [TestMethod]
    public void ThrowExceptionOnSqlServerStatementBuilderCreateBatchQueryIfThePageIsLessThanZero()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2"]);
        var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

        // Act/Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => statementBuilder.CreateBatchQuery(tableName: tableName,
            fields: fields,
            page: -1,
            rowsPerBatch: 10,
            orderBy: orderBy,
            where: null));
    }

    [TestMethod]
    public void ThrowExceptionOnSqlServerStatementBuilderCreateBatchQueryIfTheRowsPerBatchIsLessThanOne()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2"]);
        var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

        // Act/Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => statementBuilder.CreateBatchQuery(tableName: tableName,
            fields: fields,
            page: 0,
            rowsPerBatch: 0,
            orderBy: orderBy,
            where: null));
    }
    #endregion

    #region CreateCountAll

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateCountAll()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";

        // Act
        var actual = statementBuilder.CreateCountAll(tableName: tableName,
            hints: null);
        var expected = "SELECT COUNT_BIG (*) AS [CountValue] FROM [Table];";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateCountAllWithHints()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var hints = "WITH (NOLOCK)";

        // Act
        var actual = statementBuilder.CreateCountAll(tableName: tableName,
            hints: hints);
        var expected = "SELECT COUNT_BIG (*) AS [CountValue] FROM [Table] WITH (NOLOCK);";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateCountAllWithQuotedTableSchema()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "[dbo].[Table]";

        // Act
        var actual = statementBuilder.CreateCountAll(tableName: tableName,
            hints: null);
        var expected = "SELECT COUNT_BIG (*) AS [CountValue] FROM [dbo].[Table];";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateCountAllWithUnquotedTableSchema()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "dbo.Table";

        // Act
        var actual = statementBuilder.CreateCountAll(tableName: tableName,
            hints: null);
        var expected = "SELECT COUNT_BIG (*) AS [CountValue] FROM [dbo].[Table];";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    #endregion

    #region CreateCount

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateCount()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";

        // Act
        var actual = statementBuilder.CreateCount(tableName: tableName,
            hints: null);
        var expected = "SELECT COUNT_BIG (*) AS [CountValue] FROM [Table];";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateCountWithWhereExpression()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var where = new QueryGroup(new QueryField("Id", 1));

        // Act
        var actual = statementBuilder.CreateCount(tableName: tableName,
            where: where);
        var expected =
            "SELECT COUNT_BIG (*) AS [CountValue] " +
            "FROM [Table] " +
            "WHERE ([Id] = @Id);";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateCountWithHints()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var hints = "WITH (NOLOCK)";

        // Act
        var actual = statementBuilder.CreateCount(tableName: tableName,
            hints: hints);
        var expected = "SELECT COUNT_BIG (*) AS [CountValue] FROM [Table] WITH (NOLOCK);";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateCountWithWhereExpressionAndWithHints()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var where = new QueryGroup(new QueryField("Id", 1));
        var hints = "WITH (NOLOCK)";

        // Act
        var actual = statementBuilder.CreateCount(tableName: tableName,
            where: where,
            hints: hints);
        var expected =
            "SELECT COUNT_BIG (*) AS [CountValue] " +
            "FROM [Table] WITH (NOLOCK) " +
            "WHERE ([Id] = @Id);";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateCountWithQuotedTableSchema()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "[dbo].[Table]";

        // Act
        var actual = statementBuilder.CreateCount(tableName: tableName,
            hints: null);
        var expected = "SELECT COUNT_BIG (*) AS [CountValue] FROM [dbo].[Table];";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateCountWithUnquotedTableSchema()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "dbo.Table";

        // Act
        var actual = statementBuilder.CreateCount(tableName: tableName,
            hints: null);
        var expected = "SELECT COUNT_BIG (*) AS [CountValue] FROM [dbo].[Table];";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    #endregion

    #region CreateInsertAll

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateInsertAllWithIdentity()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var identityField = new DbField("Field1", false, true, false, typeof(int), null, null, null, null);

        // Act
        var actual = statementBuilder.CreateInsertAll(tableName: tableName,
            fields: fields,
            batchSize: 1,
            primaryField: null,
            identityField: identityField);
        var expected =
            "INSERT INTO [Table] ([Field2], [Field3]) " +
            "OUTPUT INSERTED.[Field1] " +
            "VALUES " +
            "(@Field2, @Field3);";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateInsertAllWithPrimaryAndIdentity()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var identityField = new DbField("Field2", false, true, false, typeof(int), null, null, null, null);

        // Act
        var actual = statementBuilder.CreateInsertAll(tableName: tableName,
            fields: fields,
            batchSize: 1,
            primaryField: null,
            identityField: identityField);
        var expected =
            "INSERT INTO [Table] ([Field1], [Field3]) " +
            "OUTPUT INSERTED.[Field2] " +
            "VALUES " +
            "(@Field1, @Field3);";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateInsertAllWithPrimaryAndIdentityAsBigInt()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var identityField = new DbField("Field2", false, true, false, typeof(long), null, null, null, null);

        // Act
        var actual = statementBuilder.CreateInsertAll(tableName: tableName,
            fields: fields,
            batchSize: 1,
            primaryField: null,
            identityField: identityField);
        var expected =
            "INSERT INTO [Table] ([Field1], [Field3]) " +
            "OUTPUT INSERTED.[Field2] " +
            "VALUES " +
            "(@Field1, @Field3);";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateInsertAllWithIdentityFor3Batches()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var identityField = new DbField("Field1", false, true, false, typeof(int), null, null, null, null);

        // Act
        var actual = statementBuilder.CreateInsertAll(tableName: tableName,
            fields: fields,
            batchSize: 3,
            primaryField: null,
            identityField: identityField);
        var expected =
            "INSERT INTO [Table] ([Field2], [Field3]) " +
            "OUTPUT INSERTED.[Field1] " +
            "VALUES " +
            "(@Field2, @Field3), " +
            "(@Field2_1, @Field3_1), " +
            "(@Field2_2, @Field3_2);";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateInsertAllWithIdentityFor5Batches()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var identityField = new DbField("Field1", false, true, false, typeof(int), null, null, null, null);

        // Act
        var actual = statementBuilder.CreateInsertAll(tableName: tableName,
            fields: fields,
            batchSize: 5,
            primaryField: null,
            identityField: identityField);
        var expected =
            "MERGE INTO [Table] AS T USING (VALUES " +
            "(@Field2, @Field3, 0), " +
            "(@Field2_1, @Field3_1, 1), " +
            "(@Field2_2, @Field3_2, 2), " +
            "(@Field2_3, @Field3_3, 3), " +
            "(@Field2_4, @Field3_4, 4)" +
            ") AS S ([Field2], [Field3], [__RepoDb_OrderColumn]) " +
            "ON 1=0 WHEN NOT MATCHED THEN INSERT ([Field2], [Field3]) VALUES (S.[Field2], S.[Field3]) " +
            "OUTPUT INSERTED.[Field1], S.[__RepoDb_OrderColumn];";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateInsertAllWithIdentityWithHints()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var identityField = new DbField("Field1", false, true, false, typeof(int), null, null, null, null);

        // Act
        var actual = statementBuilder.CreateInsertAll(tableName: tableName,
            fields: fields,
            batchSize: 1,
            primaryField: null,
            identityField: identityField,
            hints: SqlServerTableHints.TabLock);
        var expected =
            "INSERT INTO [Table] WITH (TABLOCK) ([Field2], [Field3]) " +
            "OUTPUT INSERTED.[Field1] " +
            "VALUES " +
            "(@Field2, @Field3);";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateInsertAllWithIdentityFor3BatchesWithHints()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var identityField = new DbField("Field1", false, true, false, typeof(int), null, null, null, null);

        // Act
        var actual = statementBuilder.CreateInsertAll(tableName: tableName,
            fields: fields,
            batchSize: 3,
            primaryField: null,
            identityField: identityField,
            hints: SqlServerTableHints.TabLock);
        var expected =
            "INSERT INTO [Table] WITH (TABLOCK) " +
            "([Field2], [Field3]) " +
            "OUTPUT INSERTED.[Field1] " +
            "VALUES (@Field2, @Field3), (@Field2_1, @Field3_1), (@Field2_2, @Field3_2);";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateInsertAllWithIdentityFor5BatchesWithHints()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var identityField = new DbField("Field1", false, true, false, typeof(int), null, null, null, null);

        // Act
        var actual = statementBuilder.CreateInsertAll(tableName: tableName,
            fields: fields,
            batchSize: 5,
            primaryField: null,
            identityField: identityField,
            hints: SqlServerTableHints.TabLock);
        var expected =
            "MERGE INTO [Table] AS T " +
            "USING (VALUES (@Field2, @Field3, 0), " +
            "(@Field2_1, @Field3_1, 1), " +
            "(@Field2_2, @Field3_2, 2), " +
            "(@Field2_3, @Field3_3, 3), " +
            "(@Field2_4, @Field3_4, 4)) " +
            "AS S ([Field2], [Field3], [__RepoDb_OrderColumn]) " +
            "ON 1=0 WHEN NOT MATCHED THEN INSERT ([Field2], [Field3]) VALUES (S.[Field2], S.[Field3]) " +
            "OUTPUT INSERTED.[Field1], S.[__RepoDb_OrderColumn];";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    #endregion

    #region CreateInsert

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateInsert()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);

        // Act
        var actual = statementBuilder.CreateInsert(tableName: tableName,
            fields: fields,
            primaryField: null,
            identityField: null);
        var expected =
            "INSERT INTO [Table] " +
            "([Field1], [Field2], [Field3]) " +
            "VALUES " +
            "(@Field1, @Field2, @Field3);";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateInsertWithQuotedTableSchema()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "[dbo].[Table]";
        var fields = Field.From(["Field1", "Field2", "Field3"]);

        // Act
        var actual = statementBuilder.CreateInsert(tableName: tableName,
            fields: fields,
            primaryField: null,
            identityField: null);
        var expected =
            "INSERT INTO [dbo].[Table] " +
            "([Field1], [Field2], [Field3]) " +
            "VALUES " +
            "(@Field1, @Field2, @Field3);";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateInsertWithUnquotedTableSchema()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "dbo.Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);

        // Act
        var actual = statementBuilder.CreateInsert(tableName: tableName,
            fields: fields,
            primaryField: null,
            identityField: null);
        var expected =
            "INSERT INTO [dbo].[Table] " +
            "([Field1], [Field2], [Field3]) " +
            "VALUES " +
            "(@Field1, @Field2, @Field3);";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateInsertWithPrimary()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var primaryField = new DbField("Field1", true, false, false, typeof(int), null, null, null, null);

        // Act
        var actual = statementBuilder.CreateInsert(tableName: tableName,
            fields: fields,
            primaryField: primaryField,
            identityField: null);
        var expected =
            "INSERT INTO [Table] " +
            "([Field1], [Field2], [Field3]) " +
            "OUTPUT INSERTED.[Field1] " +
            "VALUES " +
            "(@Field1, @Field2, @Field3);";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateInsertWithIdentity()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var identityField = new DbField("Field1", false, true, false, typeof(int), null, null, null, null);

        // Act
        var actual = statementBuilder.CreateInsert(tableName: tableName,
            fields: fields,
            primaryField: null,
            identityField: identityField);
        var expected =
            "INSERT INTO [Table] " +
            "([Field2], [Field3]) " +
            "OUTPUT INSERTED.[Field1] " +
            "VALUES " +
            "(@Field2, @Field3);";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateInsertWithIdentityAsBigInt()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var identityField = new DbField("Field1", false, true, false, typeof(long), null, null, null, null);

        // Act
        var actual = statementBuilder.CreateInsert(tableName: tableName,
            fields: fields,
            primaryField: null,
            identityField: identityField);
        var expected =
            "INSERT INTO [Table] " +
            "([Field2], [Field3]) " +
            "OUTPUT INSERTED.[Field1] " +
            "VALUES " +
            "(@Field2, @Field3);";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateInsertWithPrimaryAndIdentity()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var identityField = new DbField("Field2", false, true, false, typeof(int), null, null, null, null);

        // Act
        var actual = statementBuilder.CreateInsert(tableName: tableName,
            fields: fields,
            primaryField: null,
            identityField: identityField);
        var expected =
            "INSERT INTO [Table] " +
            "([Field1], [Field3]) " +
            "OUTPUT INSERTED.[Field2] " +
            "VALUES " +
            "(@Field1, @Field3);";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateInsertWithPrimaryAndIdentityAsBigInt()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var identityField = new DbField("Field2", false, true, false, typeof(long), null, null, null, null);

        // Act
        var actual = statementBuilder.CreateInsert(tableName: tableName,
            fields: fields,
            primaryField: null,
            identityField: identityField);
        var expected =
            "INSERT INTO [Table] " +
            "([Field1], [Field3]) " +
            "OUTPUT INSERTED.[Field2] " +
            "VALUES " +
            "(@Field1, @Field3);";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateInsertWithHints()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);

        // Act
        var actual = statementBuilder.CreateInsert(tableName: tableName,
            fields: fields,
            primaryField: null,
            identityField: null,
            hints: SqlServerTableHints.TabLock);
        var expected =
            "INSERT INTO [Table] WITH (TABLOCK) " +
            "([Field1], [Field2], [Field3]) " +
            "VALUES " +
            "(@Field1, @Field2, @Field3);";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    #endregion

    #region CreateMergeAll

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateMergeAll()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var qualifiers = Field.From("Field1");

        // Act
        var actual = statementBuilder.CreateMergeAll(tableName: tableName,
            fields: fields,
            qualifiers: qualifiers,
            batchSize: 1,
            primaryField: null,
            identityField: null);
        var expected =
            "MERGE [Table] AS T " +
            "USING (VALUES (@Field1, @Field2, @Field3)) " +
            "AS S ([Field1], [Field2], [Field3]) " +
            "ON ((S.[Field1] = T.[Field1] OR (S.[Field1] IS NULL AND T.[Field1] IS NULL))) " +
            "WHEN NOT MATCHED THEN " +
            "INSERT ([Field1], [Field2], [Field3]) " +
            "VALUES (S.[Field1], S.[Field2], S.[Field3]) " +
            "WHEN MATCHED THEN " +
            "UPDATE SET T.[Field2] = S.[Field2], T.[Field3] = S.[Field3];";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateMergeAllWithQuotedTableSchema()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "[dbo].[Table]";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var qualifiers = Field.From("Field1");

        // Act
        var actual = statementBuilder.CreateMergeAll(tableName: tableName,
            fields: fields,
            qualifiers: qualifiers,
            batchSize: 1,
            primaryField: null,
            identityField: null);
        var expected =
            "MERGE [dbo].[Table] AS T " +
            "USING (VALUES (@Field1, @Field2, @Field3)) " +
            "AS S ([Field1], [Field2], [Field3]) " +
            "ON ((S.[Field1] = T.[Field1] OR (S.[Field1] IS NULL AND T.[Field1] IS NULL))) " +
            "WHEN NOT MATCHED THEN " +
            "INSERT ([Field1], [Field2], [Field3]) " +
            "VALUES (S.[Field1], S.[Field2], S.[Field3]) " +
            "WHEN MATCHED THEN " +
            "UPDATE SET T.[Field2] = S.[Field2], T.[Field3] = S.[Field3];";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateMergeAllWithUnquotedTableSchema()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "dbo.Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var qualifiers = Field.From("Field1");

        // Act
        var actual = statementBuilder.CreateMergeAll(tableName: tableName,
            fields: fields,
            qualifiers: qualifiers,
            batchSize: 1,
            primaryField: null,
            identityField: null);
        var expected =
            "MERGE [dbo].[Table] AS T " +
            "USING (VALUES (@Field1, @Field2, @Field3)) " +
            "AS S ([Field1], [Field2], [Field3]) " +
            "ON ((S.[Field1] = T.[Field1] OR (S.[Field1] IS NULL AND T.[Field1] IS NULL))) " +
            "WHEN NOT MATCHED THEN " +
            "INSERT ([Field1], [Field2], [Field3]) " +
            "VALUES (S.[Field1], S.[Field2], S.[Field3]) " +
            "WHEN MATCHED THEN " +
            "UPDATE SET T.[Field2] = S.[Field2], T.[Field3] = S.[Field3];";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateMergeAllWithCoveredPrimary()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var qualifiers = Field.From("Field1");
        var primaryField = new DbField("Field1", true, false, false, typeof(int), null, null, null, null);

        // Act
        var actual = statementBuilder.CreateMergeAll(tableName: tableName,
            fields: fields,
            qualifiers: qualifiers,
            batchSize: 1,
            primaryField: primaryField,
            identityField: null);
        var expected =
            "MERGE [Table] AS T " +
            "USING (VALUES (@Field1, @Field2, @Field3)) " +
            "AS S ([Field1], [Field2], [Field3]) " +
            "ON ((S.[Field1] = T.[Field1] OR (S.[Field1] IS NULL AND T.[Field1] IS NULL))) " +
            "WHEN NOT MATCHED THEN " +
            "INSERT ([Field1], [Field2], [Field3]) " +
            "VALUES (S.[Field1], S.[Field2], S.[Field3]) " +
            "WHEN MATCHED THEN " +
            "UPDATE SET T.[Field2] = S.[Field2], T.[Field3] = S.[Field3] " +
            "OUTPUT INSERTED.[Field1];";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateMergeAllWithCoveredPrimaryAsIdentity()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var qualifiers = Field.From("Field1");
        var primaryField = new DbField("Field1", true, true, false, typeof(int), null, null, null, null);

        // Act
        var actual = statementBuilder.CreateMergeAll(tableName: tableName,
            fields: fields,
            qualifiers: qualifiers,
            batchSize: 1,
            primaryField: primaryField,
            identityField: primaryField);
        var expected =
            "MERGE [Table] AS T " +
            "USING (VALUES (@Field1, @Field2, @Field3)) " +
            "AS S ([Field1], [Field2], [Field3]) " +
            "ON ((S.[Field1] = T.[Field1] OR (S.[Field1] IS NULL AND T.[Field1] IS NULL))) " +
            "WHEN NOT MATCHED THEN " +
            "INSERT ([Field2], [Field3]) " +
            "VALUES (S.[Field2], S.[Field3]) " +
            "WHEN MATCHED THEN " +
            "UPDATE SET T.[Field2] = S.[Field2], T.[Field3] = S.[Field3] " +
            "OUTPUT INSERTED.[Field1];";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateMergeAllWithUncoveredPrimary()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var qualifiers = Field.From("Field1");
        var primaryField = new DbField("Id", true, true, false, typeof(int), null, null, null, null);

        // Act
        var actual = statementBuilder.CreateMergeAll(tableName: tableName,
            fields: fields,
            qualifiers: qualifiers,
            batchSize: 1,
            primaryField: primaryField,
            identityField: null);
        var expected =
            "MERGE [Table] AS T " +
            "USING (VALUES (@Field1, @Field2, @Field3)) " +
            "AS S ([Field1], [Field2], [Field3]) " +
            "ON ((S.[Field1] = T.[Field1] OR (S.[Field1] IS NULL AND T.[Field1] IS NULL))) " +
            "WHEN NOT MATCHED THEN " +
            "INSERT ([Field1], [Field2], [Field3]) " +
            "VALUES (S.[Field1], S.[Field2], S.[Field3]) " +
            "WHEN MATCHED THEN " +
            "UPDATE SET T.[Field2] = S.[Field2], T.[Field3] = S.[Field3] " +
            "OUTPUT INSERTED.[Id];";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateMergeAllWithCoveredIdentity()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var qualifiers = Field.From("Field1");
        var identityField = new DbField("Field1", false, true, false, typeof(int), null, null, null, null);
        GlobalConfiguration.Setup(GlobalConfiguration.Options with { SqlServerIdentityInsert = false });

        // Act
        var actual = statementBuilder.CreateMergeAll(tableName: tableName,
            fields: fields,
            qualifiers: qualifiers,
            batchSize: 1,
            primaryField: null,
            identityField: identityField);
        var expected =
            "MERGE [Table] AS T " +
            "USING (VALUES (@Field1, @Field2, @Field3)) " +
            "AS S ([Field1], [Field2], [Field3]) " +
            "ON ((S.[Field1] = T.[Field1] OR (S.[Field1] IS NULL AND T.[Field1] IS NULL))) " +
            "WHEN NOT MATCHED THEN " +
            "INSERT ([Field2], [Field3]) " +
            "VALUES (S.[Field2], S.[Field3]) " +
            "WHEN MATCHED THEN " +
            "UPDATE SET T.[Field2] = S.[Field2], T.[Field3] = S.[Field3] " +
            "OUTPUT INSERTED.[Field1];";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateMergeAllWithCoveredIdentity2()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3a"]);
        var qualifiers = Field.From("Field1");
        var identityField = new DbField("Field1", false, true, false, typeof(int), null, null, null, null);
        GlobalConfiguration.Setup(GlobalConfiguration.Options with { SqlServerIdentityInsert = true });

        // Act
        var actual = statementBuilder.CreateMergeAll(tableName: tableName,
            fields: fields,
            qualifiers: qualifiers,
            batchSize: 1,
            primaryField: null,
            identityField: identityField);

        GlobalConfiguration.Setup(GlobalConfiguration.Options with { SqlServerIdentityInsert = false });

        var expected =
            "BEGIN TRY SET IDENTITY_INSERT [Table] ON; END TRY BEGIN CATCH END CATCH " +
            "MERGE [Table] AS T " +
            "USING (VALUES (@Field1, @Field2, @Field3a)) " +
            "AS S ([Field1], [Field2], [Field3a]) " +
            "ON ((S.[Field1] = T.[Field1] OR (S.[Field1] IS NULL AND T.[Field1] IS NULL))) " +
            "WHEN NOT MATCHED THEN " +
            "INSERT ([Field1], [Field2], [Field3a]) " +
            "VALUES (S.[Field1], S.[Field2], S.[Field3a]) " +
            "WHEN MATCHED THEN " +
            "UPDATE SET T.[Field2] = S.[Field2], T.[Field3a] = S.[Field3a] " +
            "OUTPUT INSERTED.[Field1]; " +
            "BEGIN TRY SET IDENTITY_INSERT [Table] OFF; END TRY BEGIN CATCH END CATCH";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateMergeAllWithUncoveredIdentity()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var qualifiers = Field.From("Field1");
        var identityField = new DbField("Id", false, true, false, typeof(int), null, null, null, null);

        // Act
        var actual = statementBuilder.CreateMergeAll(tableName: tableName,
            fields: fields,
            qualifiers: qualifiers,
            batchSize: 1,
            primaryField: null,
            identityField: identityField);
        var expected =
            "MERGE [Table] AS T " +
            "USING (VALUES (@Field1, @Field2, @Field3)) " +
            "AS S ([Field1], [Field2], [Field3]) " +
            "ON ((S.[Field1] = T.[Field1] OR (S.[Field1] IS NULL AND T.[Field1] IS NULL))) " +
            "WHEN NOT MATCHED THEN " +
            "INSERT ([Field1], [Field2], [Field3]) " +
            "VALUES (S.[Field1], S.[Field2], S.[Field3]) " +
            "WHEN MATCHED THEN " +
            "UPDATE SET T.[Field2] = S.[Field2], T.[Field3] = S.[Field3] " +
            "OUTPUT INSERTED.[Id];";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateMergeAllWithCoveredPrimaryButWithoutQualifiers()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var primaryField = new DbField("Field1", true, isIdentity: false, false, typeof(int), null, null, null, null);

        // Act
        var actual = statementBuilder.CreateMergeAll(tableName: tableName,
            fields: fields,
            qualifiers: null,
            batchSize: 1,
            primaryField: primaryField,
            identityField: null);
        var expected =
            "MERGE [Table] AS T " +
            "USING (VALUES (@Field1, @Field2, @Field3)) " +
            "AS S ([Field1], [Field2], [Field3]) " +
            "ON ((S.[Field1] = T.[Field1] OR (S.[Field1] IS NULL AND T.[Field1] IS NULL))) " +
            "WHEN NOT MATCHED THEN " +
            "INSERT ([Field1], [Field2], [Field3]) " +
            "VALUES (S.[Field1], S.[Field2], S.[Field3]) " +
            "WHEN MATCHED THEN " +
            "UPDATE SET T.[Field2] = S.[Field2], T.[Field3] = S.[Field3] " +
            "OUTPUT INSERTED.[Field1];";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateMergeAllWithCoveredPrimaryAndWithCoveredIdentityButWithoutQualifiers()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var primaryField = new DbField("Field1", true, false, false, typeof(int), null, null, null, null);
        var identityField = new DbField("Field2", false, true, false, typeof(int), null, null, null, null);

        // Act
        var actual = statementBuilder.CreateMergeAll(tableName: tableName,
            fields: fields,
            qualifiers: null,
            batchSize: 1,
            primaryField: primaryField,
            identityField: identityField);
        var expected =
            "MERGE [Table] AS T " +
            "USING (VALUES (@Field1, @Field2, @Field3)) " +
            "AS S ([Field1], [Field2], [Field3]) " +
            "ON ((S.[Field2] = T.[Field2] OR (S.[Field2] IS NULL AND T.[Field2] IS NULL)) " +
                "AND (S.[Field1] = T.[Field1] OR (S.[Field1] IS NULL AND T.[Field1] IS NULL))) " +
            "WHEN NOT MATCHED THEN " +
            "INSERT ([Field1], [Field3]) " +
            "VALUES (S.[Field1], S.[Field3]) " +
            "WHEN MATCHED THEN " +
            "UPDATE SET T.[Field3] = S.[Field3] " +
            "OUTPUT INSERTED.[Field2], INSERTED.[Field1];";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateMergeAllWithIdentityForThreeBatches()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var qualifiers = Field.From("Field1");
        var identityField = new DbField("Field1", false, true, false, typeof(int), null, null, null, null);

        // Act
        var actual = statementBuilder.CreateMergeAll(tableName: tableName,
            fields: fields,
            qualifiers: qualifiers,
            batchSize: 3,
            primaryField: null,
            identityField: identityField);
        var expected =
            "MERGE [Table] AS T " +
             "USING (VALUES (@Field1, @Field2, @Field3, 0), " +
             "(@Field1_1, @Field2_1, @Field3_1, 1), " +
             "(@Field1_2, @Field2_2, @Field3_2, 2)) " +
            "AS S ([Field1], [Field2], [Field3], [__RepoDb_OrderColumn]) " +
            "ON ((S.[Field1] = T.[Field1] OR (S.[Field1] IS NULL AND T.[Field1] IS NULL))) " +
            "WHEN NOT MATCHED THEN " +
            "INSERT ([Field2], [Field3]) " +
            "VALUES (S.[Field2], S.[Field3]) " +
            "WHEN MATCHED THEN " +
            "UPDATE SET T.[Field2] = S.[Field2], T.[Field3] = S.[Field3] " +
            "OUTPUT INSERTED.[Field1], S.[__RepoDb_OrderColumn];";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateMergeAllWithHints()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var qualifiers = Field.From("Field1");

        // Act
        var actual = statementBuilder.CreateMergeAll(tableName: tableName,
            fields: fields,
            qualifiers: qualifiers,
            batchSize: 1,
            primaryField: null,
            identityField: null,
            hints: SqlServerTableHints.TabLock);
        var expected =
            "MERGE [Table] WITH (TABLOCK) AS T " +
            "USING (VALUES (@Field1, @Field2, @Field3)) " +
            "AS S ([Field1], [Field2], [Field3]) " +
            "ON ((S.[Field1] = T.[Field1] OR (S.[Field1] IS NULL AND T.[Field1] IS NULL))) " +
            "WHEN NOT MATCHED THEN " +
            "INSERT ([Field1], [Field2], [Field3]) " +
            "VALUES (S.[Field1], S.[Field2], S.[Field3]) " +
            "WHEN MATCHED THEN " +
            "UPDATE SET T.[Field2] = S.[Field2], T.[Field3] = S.[Field3];";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateMergeAllWithIdentityForThreeBatchesWithHints()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var qualifiers = Field.From("Field1");
        var identityField = new DbField("Field1", false, true, false, typeof(int), null, null, null, null);

        // Act
        var actual = statementBuilder.CreateMergeAll(tableName: tableName,
            fields: fields,
            qualifiers: qualifiers,
            batchSize: 3,
            primaryField: null,
            identityField: identityField,
            hints: SqlServerTableHints.TabLock);
        var expected =
            "MERGE [Table] WITH (TABLOCK) AS T " +
            "USING (VALUES (@Field1, @Field2, @Field3, 0), " +
             "(@Field1_1, @Field2_1, @Field3_1, 1), " +
             "(@Field1_2, @Field2_2, @Field3_2, 2)) " +
            "AS S ([Field1], [Field2], [Field3], [__RepoDb_OrderColumn]) " +
            "ON ((S.[Field1] = T.[Field1] OR (S.[Field1] IS NULL AND T.[Field1] IS NULL))) " +
            "WHEN NOT MATCHED THEN " +
            "INSERT ([Field2], [Field3]) " +
            "VALUES (S.[Field2], S.[Field3]) " +
            "WHEN MATCHED THEN " +
            "UPDATE SET T.[Field2] = S.[Field2], T.[Field3] = S.[Field3] " +
            "OUTPUT INSERTED.[Field1], S.[__RepoDb_OrderColumn];";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ThrowExceptionOnSqlServerStatementBuilderCreateMergeAllIfThereAreNoFields()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var qualifiers = Field.From("Id");

        // Act
        Assert.ThrowsExactly<MissingFieldsException>(() => statementBuilder.CreateMergeAll(tableName: tableName,
            fields: null,
            qualifiers: qualifiers,
            batchSize: 1,
            primaryField: null,
            identityField: null));
    }

    [TestMethod]
    public void ThrowExceptionOnSqlServerStatementBuilderCreateMergeAllIfThereAreNoPrimaryAndNoQualifiers()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);

        // Act
        Assert.ThrowsExactly<MissingQualifierFieldsException>(() => statementBuilder.CreateMergeAll(tableName: tableName,
            fields: fields,
            qualifiers: null,
            batchSize: 1,
            primaryField: null,
            identityField: null));
    }

    [TestMethod]
    public void ThrowExceptionOnSqlServerStatementBuilderCreateMergeAllIfTheQualifiersAreNotPresentAtTheGivenFields()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var qualifiers = Field.From("Id");

        // Act
        Assert.ThrowsExactly<InvalidQualifiersException>(() => statementBuilder.CreateMergeAll(tableName: tableName,
            fields: fields,
            qualifiers: qualifiers,
            batchSize: 1,
            primaryField: null,
            identityField: null));
    }

    [TestMethod]
    public void ThrowExceptionOnSqlServerStatementBuilderCreateMergeAllIfThePrimaryAsQualifierIsNotPresentAtTheGivenFields()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var primaryField = new DbField("Id", true, false, false, typeof(int), null, null, null, null);

        // Act
        Assert.ThrowsExactly<InvalidQualifiersException>(() => statementBuilder.CreateMergeAll(tableName: tableName,
            fields: fields,
            qualifiers: null,
            batchSize: 1,
            primaryField: primaryField,
            identityField: null));
    }

    [TestMethod]
    public void ThrowExceptionOnSqlServerStatementBuilderCreateMergeAllIfTheTableIsNull()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        string? tableName = null;
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var qualifiers = Field.From("Field1");

        // Act
        Assert.ThrowsExactly<ArgumentNullException>(() => statementBuilder.CreateMergeAll(tableName: tableName,
            fields: fields,
            qualifiers: qualifiers,
            batchSize: 1,
            primaryField: null,
            identityField: null));
    }

    [TestMethod]
    public void ThrowExceptionOnSqlServerStatementBuilderCreateMergeAllIfTheTableIsEmpty()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var qualifiers = Field.From("Field1");

        // Act
        Assert.Throws<ArgumentException>(
        () => statementBuilder.CreateMergeAll(tableName: tableName,
            fields: fields,
            qualifiers: qualifiers,
            batchSize: 1,
            primaryField: null,
            identityField: null));
    }

    [TestMethod]
    public void ThrowExceptionOnSqlServerStatementBuilderCreateMergeAllIfTheTableIsWhitespace()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = " ";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var qualifiers = Field.From("Field1");

        // Act
        Assert.Throws<ArgumentException>(
        () => statementBuilder.CreateMergeAll(tableName: tableName,
            fields: fields,
            qualifiers: qualifiers,
            batchSize: 1,
            primaryField: null,
            identityField: null));
    }

    [TestMethod]
    public void ThrowExceptionOnSqlServerStatementBuilderCreateMergeAllIfThePrimaryIsNotReallyAPrimary()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var primaryField = new DbField("Field1", false, false, false, typeof(int), null, null, null, null);

        // Act
        Assert.ThrowsExactly<InvalidOperationException>(() => statementBuilder.CreateMergeAll(tableName: tableName,
            fields: fields,
            qualifiers: null,
            batchSize: 1,
            primaryField: primaryField,
            identityField: null));
    }

    [TestMethod]
    public void ThrowExceptionOnSqlServerStatementBuilderCreateMergeAllIfTheIdentityIsNotReallyAnIdentity()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var qualifiers = Field.From("Field1");
        var identifyField = new DbField("Field2", false, false, false, typeof(int), null, null, null, null);

        // Act
        Assert.ThrowsExactly<InvalidOperationException>(() => statementBuilder.CreateMergeAll(tableName: tableName,
            fields: fields,
            qualifiers: null,
            batchSize: 1,
            primaryField: null,
            identityField: identifyField));
    }

    #endregion

    #region CreateMerge

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateMerge()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var qualifiers = Field.From("Field1");

        // Act
        var actual = statementBuilder.CreateMerge(tableName: tableName,
            fields: fields,
            qualifiers: qualifiers,
            primaryField: null,
            identityField: null);
        var expected =
            "MERGE [Table] AS T " +
            "USING (SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3]) " +
            "AS S ON ((S.[Field1] = T.[Field1] OR (S.[Field1] IS NULL AND T.[Field1] IS NULL))) " +
            "WHEN NOT MATCHED THEN " +
            "INSERT ([Field1], [Field2], [Field3]) " +
            "VALUES (S.[Field1], S.[Field2], S.[Field3]) " +
            "WHEN MATCHED THEN " +
            "UPDATE SET T.[Field2] = S.[Field2], T.[Field3] = S.[Field3];";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateMergeWithQuotedTableSchema()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "[dbo].[Table]";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var qualifiers = Field.From("Field1");

        // Act
        var actual = statementBuilder.CreateMerge(tableName: tableName,
            fields: fields,
            qualifiers: qualifiers,
            primaryField: null,
            identityField: null);
        var expected =
            "MERGE [dbo].[Table] AS T " +
            "USING (SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3]) " +
            "AS S ON ((S.[Field1] = T.[Field1] OR (S.[Field1] IS NULL AND T.[Field1] IS NULL))) " +
            "WHEN NOT MATCHED THEN " +
            "INSERT ([Field1], [Field2], [Field3]) " +
            "VALUES (S.[Field1], S.[Field2], S.[Field3]) " +
            "WHEN MATCHED THEN " +
            "UPDATE SET T.[Field2] = S.[Field2], T.[Field3] = S.[Field3];";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateMergeWithUnquotedTableSchema()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "dbo.Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var qualifiers = Field.From("Field1");

        // Act
        var actual = statementBuilder.CreateMerge(tableName: tableName,
            fields: fields,
            qualifiers: qualifiers,
            primaryField: null,
            identityField: null);
        var expected =
            "MERGE [dbo].[Table] AS T " +
            "USING (SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3]) " +
            "AS S ON ((S.[Field1] = T.[Field1] OR (S.[Field1] IS NULL AND T.[Field1] IS NULL))) " +
            "WHEN NOT MATCHED THEN " +
            "INSERT ([Field1], [Field2], [Field3]) " +
            "VALUES (S.[Field1], S.[Field2], S.[Field3]) " +
            "WHEN MATCHED THEN " +
            "UPDATE SET T.[Field2] = S.[Field2], T.[Field3] = S.[Field3];";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateMergeWithCoveredPrimary()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var qualifiers = Field.From("Field1");
        var primaryField = new DbField("Field1", true, false, false, typeof(int), null, null, null, null);

        // Act
        var actual = statementBuilder.CreateMerge(tableName: tableName,
            fields: fields,
            qualifiers: qualifiers,
            primaryField: primaryField,
            identityField: null);
        var expected =
            "MERGE [Table] AS T " +
            "USING (SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3]) " +
            "AS S ON ((S.[Field1] = T.[Field1] OR (S.[Field1] IS NULL AND T.[Field1] IS NULL))) " +
            "WHEN NOT MATCHED THEN " +
            "INSERT ([Field1], [Field2], [Field3]) " +
            "VALUES (S.[Field1], S.[Field2], S.[Field3]) " +
            "WHEN MATCHED THEN " +
            "UPDATE SET T.[Field2] = S.[Field2], T.[Field3] = S.[Field3] " +
            "OUTPUT INSERTED.[Field1];";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateMergeWithCoveredPrimaryAsIdentity()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var qualifiers = Field.From("Field1");
        var primaryField = new DbField("Field1", true, true, false, typeof(int), null, null, null, null);

        GlobalConfiguration.Setup(GlobalConfiguration.Options with { SqlServerIdentityInsert = false });

        // Act
        var actual = statementBuilder.CreateMerge(tableName: tableName,
            fields: fields,
            qualifiers: qualifiers,
            primaryField: primaryField,
            identityField: primaryField);
        var expected =
            "MERGE [Table] AS T " +
            "USING (SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3]) " +
            "AS S ON ((S.[Field1] = T.[Field1] OR (S.[Field1] IS NULL AND T.[Field1] IS NULL))) " +
            "WHEN NOT MATCHED THEN " +
            "INSERT ([Field2], [Field3]) " +
            "VALUES (S.[Field2], S.[Field3]) " +
            "WHEN MATCHED THEN " +
            "UPDATE SET T.[Field2] = S.[Field2], T.[Field3] = S.[Field3] " +
            "OUTPUT INSERTED.[Field1];";

        // Assert
        Assert.AreEqual(expected, actual);
    }
    [TestMethod]
    public void TestSqlServerStatementBuilderCreateMergeWithCoveredPrimaryAsIdentity2()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3b"]);
        var qualifiers = Field.From("Field1");
        var primaryField = new DbField("Field1", true, true, false, typeof(int), null, null, null, null);

        GlobalConfiguration.Setup(GlobalConfiguration.Options with { SqlServerIdentityInsert = true });

        // Act
        var actual = statementBuilder.CreateMerge(tableName: tableName,
            fields: fields,
            qualifiers: qualifiers,
            primaryField: primaryField,
            identityField: primaryField);
        GlobalConfiguration.Setup(GlobalConfiguration.Options with { SqlServerIdentityInsert = false });
        var expected =
            "BEGIN TRY SET IDENTITY_INSERT [Table] ON; END TRY BEGIN CATCH END CATCH " +
            "MERGE [Table] AS T " +
            "USING (SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3b AS [Field3b]) " +
            "AS S ON ((S.[Field1] = T.[Field1] OR (S.[Field1] IS NULL AND T.[Field1] IS NULL))) " +
            "WHEN NOT MATCHED THEN " +
            "INSERT ([Field1], [Field2], [Field3b]) " +
            "VALUES (S.[Field1], S.[Field2], S.[Field3b]) " +
            "WHEN MATCHED THEN " +
            "UPDATE SET T.[Field2] = S.[Field2], T.[Field3b] = S.[Field3b] " +
            "OUTPUT INSERTED.[Field1]; " +
            "BEGIN TRY SET IDENTITY_INSERT [Table] OFF; END TRY BEGIN CATCH END CATCH";

        // Assert
        Assert.AreEqual(expected, actual);
    }


    [TestMethod]
    public void TestSqlServerStatementBuilderCreateMergeWithUncoveredPrimary()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var qualifiers = Field.From("Field1");
        var primaryField = new DbField("Id", true, true, false, typeof(int), null, null, null, null);

        // Act
        var actual = statementBuilder.CreateMerge(tableName: tableName,
            fields: fields,
            qualifiers: qualifiers,
            primaryField: primaryField,
            identityField: null);
        var expected =
            "MERGE [Table] AS T " +
            "USING (SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3]) " +
            "AS S ON ((S.[Field1] = T.[Field1] OR (S.[Field1] IS NULL AND T.[Field1] IS NULL))) " +
            "WHEN NOT MATCHED THEN " +
            "INSERT ([Field1], [Field2], [Field3]) " +
            "VALUES (S.[Field1], S.[Field2], S.[Field3]) " +
            "WHEN MATCHED THEN " +
            "UPDATE SET T.[Field2] = S.[Field2], T.[Field3] = S.[Field3] " +
            "OUTPUT INSERTED.[Id];";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateMergeWithCoveredIdentity()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var qualifiers = Field.From("Field1");
        var identityField = new DbField("Field1", false, true, false, typeof(int), null, null, null, null);

        // Act
        var actual = statementBuilder.CreateMerge(tableName: tableName,
            fields: fields,
            qualifiers: qualifiers,
            primaryField: null,
            identityField: identityField);
        var expected =
            "MERGE [Table] AS T " +
            "USING (SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3]) " +
            "AS S ON ((S.[Field1] = T.[Field1] OR (S.[Field1] IS NULL AND T.[Field1] IS NULL))) " +
            "WHEN NOT MATCHED THEN " +
            "INSERT ([Field2], [Field3]) " +
            "VALUES (S.[Field2], S.[Field3]) " +
            "WHEN MATCHED THEN " +
            "UPDATE SET T.[Field2] = S.[Field2], T.[Field3] = S.[Field3] " +
            "OUTPUT INSERTED.[Field1];";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateMergeWithUncoveredIdentity()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var qualifiers = Field.From("Field1");
        var identityField = new DbField("Id", false, true, false, typeof(int), null, null, null, null);

        // Act
        var actual = statementBuilder.CreateMerge(tableName: tableName,
            fields: fields,
            qualifiers: qualifiers,
            primaryField: null,
            identityField: identityField);
        var expected =
            "MERGE [Table] AS T " +
            "USING (SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3]) " +
            "AS S ON ((S.[Field1] = T.[Field1] OR (S.[Field1] IS NULL AND T.[Field1] IS NULL))) " +
            "WHEN NOT MATCHED THEN " +
            "INSERT ([Field1], [Field2], [Field3]) " +
            "VALUES (S.[Field1], S.[Field2], S.[Field3]) " +
            "WHEN MATCHED THEN " +
            "UPDATE SET T.[Field2] = S.[Field2], T.[Field3] = S.[Field3] " +
            "OUTPUT INSERTED.[Id];";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateMergeWithCoveredPrimaryButWithoutQualifiers()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var primaryField = new DbField("Field1", true, isIdentity: false, false, typeof(int), null, null, null, null);

        GlobalConfiguration.Setup(GlobalConfiguration.Options with { SqlServerIdentityInsert = false });

        // Act
        var actual = statementBuilder.CreateMerge(tableName: tableName,
            fields: fields,
            qualifiers: null,
            primaryField: primaryField,
            identityField: null);
        var expected =
            "MERGE [Table] AS T " +
            "USING (SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3]) " +
            "AS S ON ((S.[Field1] = T.[Field1] OR (S.[Field1] IS NULL AND T.[Field1] IS NULL))) " +
            "WHEN NOT MATCHED THEN " +
            "INSERT ([Field1], [Field2], [Field3]) " +
            "VALUES (S.[Field1], S.[Field2], S.[Field3]) " +
            "WHEN MATCHED THEN " +
            "UPDATE SET T.[Field2] = S.[Field2], T.[Field3] = S.[Field3] " +
            "OUTPUT INSERTED.[Field1];";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateMergeWithCoveredPrimaryAndWithCoveredIdentityButWithoutQualifiers()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var primaryField = new DbField("Field1", true, false, false, typeof(int), null, null, null, null);
        var identityField = new DbField("Field2", false, true, false, typeof(int), null, null, null, null);

        GlobalConfiguration.Setup(GlobalConfiguration.Options with { SqlServerIdentityInsert = false });

        // Act
        var actual = statementBuilder.CreateMerge(tableName: tableName,
            fields: fields,
            qualifiers: null,
            primaryField: primaryField,
            identityField: identityField);
        var expected =
            "MERGE [Table] AS T " +
            "USING (SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3]) " +
            "AS S ON ((S.[Field1] = T.[Field1] OR (S.[Field1] IS NULL AND T.[Field1] IS NULL))) " +
            "WHEN NOT MATCHED THEN " +
            "INSERT ([Field1], [Field3]) " +
            "VALUES (S.[Field1], S.[Field3]) " +
            "WHEN MATCHED THEN " +
            "UPDATE SET T.[Field3] = S.[Field3] " +
            "OUTPUT INSERTED.[Field2], INSERTED.[Field1];";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateMergeWithHints()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var qualifiers = Field.From("Field1");

        // Act
        var actual = statementBuilder.CreateMerge(tableName: tableName,
            fields: fields,
            qualifiers: qualifiers,
            primaryField: null,
            identityField: null,
            hints: SqlServerTableHints.TabLock);
        var expected =
            "MERGE [Table] WITH (TABLOCK) AS T " +
            "USING (SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3]) " +
            "AS S ON ((S.[Field1] = T.[Field1] OR (S.[Field1] IS NULL AND T.[Field1] IS NULL))) " +
            "WHEN NOT MATCHED THEN " +
            "INSERT ([Field1], [Field2], [Field3]) " +
            "VALUES (S.[Field1], S.[Field2], S.[Field3]) " +
            "WHEN MATCHED THEN " +
            "UPDATE SET T.[Field2] = S.[Field2], T.[Field3] = S.[Field3];";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ThrowExceptionOnSqlServerStatementBuilderCreateMergeIfThereAreNoFields()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var qualifiers = Field.From("Id");

        // Act
        Assert.ThrowsExactly<MissingFieldsException>(() => statementBuilder.CreateMerge(tableName: tableName,
            fields: null,
            qualifiers: qualifiers,
            primaryField: null,
            identityField: null));
    }

    [TestMethod]
    public void ThrowExceptionOnSqlServerStatementBuilderCreateMergeIfThereAreNoPrimaryAndNoQualifiers()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);

        // Act
        Assert.ThrowsExactly<MissingQualifierFieldsException>(() => statementBuilder.CreateMerge(tableName: tableName,
            fields: fields,
            qualifiers: null,
            primaryField: null,
            identityField: null));
    }

    [TestMethod]
    public void ThrowExceptionOnSqlServerStatementBuilderCreateMergeIfTheQualifiersAreNotPresentAtTheGivenFields()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var qualifiers = Field.From("Id");

        // Act
        Assert.ThrowsExactly<InvalidQualifiersException>(() => statementBuilder.CreateMerge(tableName: tableName,
            fields: fields,
            qualifiers: qualifiers,
            primaryField: null,
            identityField: null));
    }

    [TestMethod]
    public void ThrowExceptionOnSqlServerStatementBuilderCreateMergeIfThePrimaryAsQualifierIsNotPresentAtTheGivenFields()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var primaryField = new DbField("Id", true, false, false, typeof(int), null, null, null, null);

        // Act
        Assert.ThrowsExactly<InvalidQualifiersException>(() => statementBuilder.CreateMerge(tableName: tableName,
            fields: fields,
            qualifiers: null,
            primaryField: primaryField,
            identityField: null));
    }

    [TestMethod]
    public void ThrowExceptionOnSqlServerStatementBuilderCreateMergeIfTheTableIsNull()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        string? tableName = null;
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var qualifiers = Field.From("Field1");

        // Act
        Assert.ThrowsExactly<ArgumentNullException>(() => statementBuilder.CreateMerge(tableName: tableName,
            fields: fields,
            qualifiers: qualifiers,
            primaryField: null,
            identityField: null));
    }

    [TestMethod]
    public void ThrowExceptionOnSqlServerStatementBuilderCreateMergeIfTheTableIsEmpty()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var qualifiers = Field.From("Field1");

        // Act
        Assert.Throws<ArgumentException>(
        () => statementBuilder.CreateMerge(tableName: tableName,
            fields: fields,
            qualifiers: qualifiers,
            primaryField: null,
            identityField: null));
    }

    [TestMethod]
    public void ThrowExceptionOnSqlServerStatementBuilderCreateMergeIfTheTableIsWhitespace()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = " ";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var qualifiers = Field.From("Field1");

        // Act
        Assert.Throws<ArgumentException>(
        () => statementBuilder.CreateMerge(tableName: tableName,
            fields: fields,
            qualifiers: qualifiers,
            primaryField: null,
            identityField: null));
    }

    [TestMethod]
    public void ThrowExceptionOnSqlServerStatementBuilderCreateMergeIfThePrimaryIsNotReallyAPrimary()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var primaryField = new DbField("Field1", false, false, false, typeof(int), null, null, null, null);

        // Act
        Assert.ThrowsExactly<InvalidOperationException>(() => statementBuilder.CreateMerge(tableName: tableName,
            fields: fields,
            qualifiers: null,
            primaryField: primaryField,
            identityField: null));
    }

    [TestMethod]
    public void ThrowExceptionOnSqlServerStatementBuilderCreateMergeIfTheIdentityIsNotReallyAnIdentity()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2", "Field3"]);
        var qualifiers = Field.From("Field1");
        var identifyField = new DbField("Field2", false, false, false, typeof(int), null, null, null, null);

        // Act
        Assert.ThrowsExactly<InvalidOperationException>(() => statementBuilder.CreateMerge(tableName: tableName,
            fields: fields,
            qualifiers: null,
            primaryField: null,
            identityField: identifyField));
    }

    #endregion

    #region CreateSkipQuery

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateSkipQueryFirstBatch()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2"]);
        var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

        // Act
        var actual = statementBuilder.CreateSkipQuery(tableName: tableName,
            fields: fields,
            skip: 0,
            take: 10,
            orderBy: orderBy,
            where: null);
        var expected =
            "WITH CTE AS " +
            "(" +
            "SELECT TOP (10) ROW_NUMBER() OVER (ORDER BY [Field1] ASC) AS [RowNumber], [Field1], [Field2] " +
            "FROM [Table] " +
            "ORDER BY [Field1] ASC) " +
            "SELECT [Field1], [Field2] " +
            "FROM CTE " +
            "WHERE ([RowNumber] BETWEEN 1 AND 10);";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateSkipQuerySecondBatch()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2"]);
        var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

        // Act
        var actual = statementBuilder.CreateSkipQuery(tableName: tableName,
            fields: fields,
            skip: 10,
            take: 10,
            orderBy: orderBy,
            where: null);
        var expected =
            "WITH CTE AS " +
            "(" +
            "SELECT TOP (20) ROW_NUMBER() OVER (ORDER BY [Field1] ASC) AS [RowNumber], [Field1], [Field2] " +
            "FROM [Table] " +
            "ORDER BY [Field1] ASC) " +
            "SELECT [Field1], [Field2] " +
            "FROM CTE " +
            "WHERE ([RowNumber] BETWEEN 11 AND 20);";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateSkipQueryWithHints()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "[dbo].[Table]";
        var fields = Field.From(["Field1", "Field2"]);
        var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

        // Act
        var actual = statementBuilder.CreateSkipQuery(tableName: tableName,
            fields: fields,
            skip: 0,
            take: 10,
            orderBy: orderBy,
            where: null,
            hints: SqlServerTableHints.NoLock);
        var expected =
            "WITH CTE AS " +
            "(" +
            "SELECT TOP (10) ROW_NUMBER() OVER (ORDER BY [Field1] ASC) AS [RowNumber], [Field1], [Field2] " +
            "FROM [dbo].[Table] WITH (NOLOCK) " +
            "ORDER BY [Field1] ASC) " +
            "SELECT [Field1], [Field2] " +
            "FROM CTE " +
            "WHERE ([RowNumber] BETWEEN 1 AND 10);";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateSkipQueryWithQuotedTableSchema()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "[dbo].[Table]";
        var fields = Field.From(["Field1", "Field2"]);
        var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

        // Act
        var actual = statementBuilder.CreateSkipQuery(tableName: tableName,
            fields: fields,
            skip: 0,
            take: 10,
            orderBy: orderBy,
            where: null);
        var expected =
            "WITH CTE AS " +
            "(" +
            "SELECT TOP (10) ROW_NUMBER() OVER (ORDER BY [Field1] ASC) AS [RowNumber], [Field1], [Field2] " +
            "FROM [dbo].[Table] " +
            "ORDER BY [Field1] ASC) " +
            "SELECT [Field1], [Field2] " +
            "FROM CTE " +
            "WHERE ([RowNumber] BETWEEN 1 AND 10);";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateSkipQueryWithUnquotedTableSchema()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "dbo.Table";
        var fields = Field.From(["Field1", "Field2"]);
        var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

        // Act
        var actual = statementBuilder.CreateSkipQuery(tableName: tableName,
            fields: fields,
            skip: 0,
            take: 10,
            orderBy: orderBy,
            where: null);
        var expected =
            "WITH CTE AS " +
            "(" +
            "SELECT TOP (10) ROW_NUMBER() OVER (ORDER BY [Field1] ASC) AS [RowNumber], [Field1], [Field2] " +
            "FROM [dbo].[Table] " +
            "ORDER BY [Field1] ASC) " +
            "SELECT [Field1], [Field2] " +
            "FROM CTE " +
            "WHERE ([RowNumber] BETWEEN 1 AND 10);";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateSkipQueryWithWhereExpression()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2"]);
        var where = new QueryGroup(new QueryField("Field1", Operation.NotEqual, 1));
        var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

        // Act
        var actual = statementBuilder.CreateSkipQuery(tableName: tableName,
            fields: fields,
            skip: 10,
            take: 10,
            orderBy: orderBy,
            where: where);
        var expected =
            "WITH CTE AS " +
            "(" +
            "SELECT TOP (20) ROW_NUMBER() OVER (ORDER BY [Field1] ASC) AS [RowNumber], [Field1], [Field2] " +
            "FROM [Table] " +
            "WHERE ([Field1] <> @Field1) " +
            "ORDER BY [Field1] ASC) " +
            "SELECT [Field1], [Field2] " +
            "FROM CTE " +
            "WHERE ([RowNumber] BETWEEN 11 AND 20);";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateSkipQueryWithWhereExpressionUniqueField()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2"]);
        var where = new QueryGroup(new QueryField("Id", Operation.NotEqual, 1));
        var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

        // Act
        var actual = statementBuilder.CreateSkipQuery(tableName: tableName,
            fields: fields,
            skip: 10,
            take: 10,
            orderBy: orderBy,
            where: where);
        var expected =
            "WITH CTE AS " +
            "(" +
            "SELECT TOP (20) ROW_NUMBER() OVER (ORDER BY [Field1] ASC) AS [RowNumber], [Field1], [Field2] " +
            "FROM [Table] " +
            "WHERE ([Id] <> @Id) " +
            "ORDER BY [Field1] ASC) " +
            "SELECT [Field1], [Field2] " +
            "FROM CTE " +
            "WHERE ([RowNumber] BETWEEN 11 AND 20);";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ThrowExceptionOnSqlServerStatementBuilderCreateSkipQueryIfTheTableIsNull()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        string? tableName = null;
        var fields = Field.From(["Field1", "Field2"]);

        // Act/Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => statementBuilder.CreateSkipQuery(tableName: tableName,
            fields: fields,
            skip: 0,
            take: 10,
            orderBy: null,
            where: null));
    }

    [TestMethod]
    public void ThrowExceptionOnSqlServerStatementBuilderCreateSkipQueryIfTheTableIsEmpty()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "";
        var fields = Field.From(["Field1", "Field2"]);

        // Act/Assert
        Assert.Throws<ArgumentException>(
        () => statementBuilder.CreateSkipQuery(tableName: tableName,
            fields: fields,
            skip: 0,
            take: 10,
            orderBy: null,
            where: null));
    }

    [TestMethod]
    public void ThrowExceptionOnSqlServerStatementBuilderCreateSkipQueryIfTheTableIsWhitespace()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = " ";
        var fields = Field.From(["Field1", "Field2"]);

        // Act/Assert
        Assert.Throws<ArgumentException>(
        () => statementBuilder.CreateSkipQuery(tableName: tableName,
            fields: fields,
            skip: 0,
            take: 10,
            orderBy: null,
            where: null));
    }

    [TestMethod]
    public void ThrowExceptionOnSqlServerStatementBuilderCreateSkipQueryIfTheFieldsAreNull()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

        // Act/Assert
        Assert.ThrowsExactly<MissingFieldsException>(() => statementBuilder.CreateSkipQuery(tableName: tableName,
            fields: null,
            skip: 0,
            take: 10,
            orderBy: orderBy,
            where: null));
    }

    [TestMethod]
    public void ThrowExceptionOnSqlServerStatementBuilderCreateSkipQueryIfThereAreNoOrderFields()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2"]);

        // Act/Assert
        Assert.ThrowsExactly<EmptyException>(() => statementBuilder.CreateSkipQuery(tableName: tableName,
            fields: fields,
            skip: 0,
            take: 10,
            orderBy: null,
            where: null));
    }

    [TestMethod]
    public void ThrowExceptionOnSqlServerStatementBuilderCreateSkipQueryIfThePageIsLessThanZero()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2"]);
        var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

        // Act/Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => statementBuilder.CreateSkipQuery(tableName: tableName,
            fields: fields,
            skip: -1,
            take: 10,
            orderBy: orderBy,
            where: null));
    }

    [TestMethod]
    public void ThrowExceptionOnSqlServerStatementBuilderCreateSkipQueryIfTheRowsPerBatchIsLessThanOne()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2"]);
        var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

        // Act/Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => statementBuilder.CreateSkipQuery(tableName: tableName,
            fields: fields,
            skip: 0,
            take: 0,
            orderBy: orderBy,
            where: null));
    }
    #endregion

    #region CreateUpdate
    [TestMethod]
    public void TestSqlServerStatementBuilderCreateUpdate()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2"]);
        var primaryField = new DbField("Field1", true, false, false, typeof(int), null, null, null, null);

        // Act
        var actual = statementBuilder.CreateUpdate(tableName: tableName,
            fields: fields,
            where: null,
            primaryField,
            identityField: null,
            hints: null);
        var expected =
            "UPDATE [Table] SET [Field2] = @Field2;";

        // Assert
        Assert.AreEqual(expected, actual);
    }
    #endregion

    #region CreateUpdateAll
    [TestMethod]
    public void TestSqlServerStatementBuilderCreateUpdateAll()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2"]);
        var primaryField = new DbField("Field1", true, false, false, typeof(int), null, null, null, null);
        var qualifiers = Field.From("Field1");

        // Act
        var actual = statementBuilder.CreateUpdateAll(tableName: tableName,
            fields: fields,
            qualifiers: qualifiers,
            primaryField: primaryField,
            batchSize: 1,
            hints: null);
        var expected =
            "UPDATE [Table] SET [Field2] = @Field2 WHERE ([Field1] = @Field1);";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSqlServerStatementBuilderCreateUpdateAllForThreeBatches()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<SqlConnection>();
        var tableName = "Table";
        var fields = Field.From(["Field1", "Field2"]);
        var primaryField = new DbField("Field1", true, false, false, typeof(int), null, null, null, null);
        var qualifiers = Field.From("Field1");

        // Act
        var actual = statementBuilder.CreateUpdateAll(tableName: tableName,
            fields: fields,
            qualifiers: qualifiers,
            primaryField: primaryField,
            batchSize: 3,
            hints: null);
        var expected =
            "MERGE INTO [Table] AS T USING (" +
            "VALUES " +
            "(@Field1, @Field2), " +
            "(@Field1_1, @Field2_1), " +
            "(@Field1_2, @Field2_2)" +
            ") " +
            "AS S ([Field1], [Field2]) ON " +
            "T.[Field1] = S.[Field1] " +
            "WHEN MATCHED THEN " +
            "UPDATE SET T.[Field2] = S.[Field2];";

        // Assert
        Assert.AreEqual(expected, actual);
    }
    #endregion
}
