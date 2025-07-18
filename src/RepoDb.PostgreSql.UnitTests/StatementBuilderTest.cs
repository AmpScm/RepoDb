using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Enumerations;
using RepoDb.Exceptions;

namespace RepoDb.PostgreSql.UnitTests;

[TestClass]
public class StatementBuilderTest
{
    [TestInitialize]
    public void Initialize()
    {
        GlobalConfiguration
            .Setup()
            .UsePostgreSql();
    }

    #region CreateBatchQuery

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateBatchQuery()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateBatchQuery("Table",
            Field.From("Id", "Name"),
            0,
            10,
            OrderField.Parse(new { Id = Order.Ascending }));
        var expected = "SELECT \"Id\", \"Name\" FROM \"Table\" ORDER BY \"Id\" ASC LIMIT 10;";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateBatchQueryWithPage()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateBatchQuery("Table",
            Field.From("Id", "Name"),
            3,
            10,
            OrderField.Parse(new { Id = Order.Ascending }));
        var expected = "SELECT \"Id\", \"Name\" FROM \"Table\" ORDER BY \"Id\" ASC LIMIT 10 OFFSET 30;";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlStatementBuilderCreateBatchQueryIfThereAreNoFields()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        Assert.ThrowsExactly<EmptyException>(() => builder.CreateBatchQuery("Table",
            null,
            0,
            10,
            OrderField.Parse(new { Id = Order.Ascending })));
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlStatementBuilderCreateBatchQueryIfThereAreNoOrderFields()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        Assert.ThrowsExactly<EmptyException>(() => builder.CreateBatchQuery("Table",
            Field.From("Id", "Name"),
            0,
            10,
            null));
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlStatementBuilderCreateBatchQueryIfThePageValueIsNullOrOutOfRange()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => builder.CreateBatchQuery("Table",
            Field.From("Id", "Name"),
            -1,
            10,
            OrderField.Parse(new { Id = Order.Ascending })));
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlStatementBuilderCreateBatchQueryIfTheRowsPerBatchValueIsNullOrOutOfRange()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => builder.CreateBatchQuery("Table",
            Field.From("Id", "Name"),
            0,
            -1,
            OrderField.Parse(new { Id = Order.Ascending })));
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlStatementBuilderCreateBatchQueryIfThereAreHints()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => builder.CreateBatchQuery("Table",
            Field.From("Id", "Name"),
            0,
            -1,
            OrderField.Parse(new { Id = Order.Ascending }),
            null,
            "WhatEver"));
    }

    #endregion

    #region CreateCount

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateCount()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateCount("Table",
            null,
            null);
        var expected = "SELECT COUNT (*) AS \"CountValue\" FROM \"Table\";";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateCountWithExpression()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateCount("Table",
            QueryGroup.Parse(new { Id = 1 }),
            null);
        var expected = "SELECT COUNT (*) AS \"CountValue\" FROM \"Table\" WHERE (\"Id\" = @Id);";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlStatementBuilderCreateCountIfThereAreHints()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => builder.CreateCount("Table",
            QueryGroup.Parse(new { Id = 1 }),
            "WhatEver"));
    }

    #endregion

    #region CreateCountAll

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateCountAll()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateCountAll("Table",
            null);
        var expected = "SELECT COUNT (*) AS \"CountValue\" FROM \"Table\";";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlStatementBuilderCreateCountAllIfThereAreHints()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => builder.CreateCountAll("Table",
            "WhatEver"));
    }

    #endregion

    #region CreateExists

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateExists()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateExists("Table",
            QueryGroup.Parse(new { Id = 1 }));
        var expected = "SELECT 1 AS \"ExistsValue\" FROM \"Table\" WHERE (\"Id\" = @Id) LIMIT 1;";

        // Assert
        Assert.AreEqual(expected, query);
    }

    #endregion

    #region CreateInsert

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateInsert()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateInsert("Table",
            Field.From("Id", "Name", "Address"),
            primaryField: null,
            null);
        var expected = "INSERT INTO \"Table\" (\"Id\", \"Name\", \"Address\") VALUES (@Id, @Name, @Address);";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateInsertWithPrimary()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateInsert("Table",
            Field.From("Id", "Name", "Address"),
            new DbField("Id", true, false, false, typeof(int), null, null, null, null),
            null);
        var expected = "INSERT INTO \"Table\" (\"Id\", \"Name\", \"Address\") VALUES (@Id, @Name, @Address) RETURNING \"Id\";";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateInsertWithIdentity()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateInsert("Table",
            Field.From("Id", "Name", "Address"),
            null,
            new DbField("Id", false, true, false, typeof(int), null, null, null, null));
        var expected = "INSERT INTO \"Table\" (\"Name\", \"Address\") VALUES (@Name, @Address) RETURNING \"Id\";";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlStatementBuilderCreateInsertIfThereAreHints()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => builder.CreateInsert("Table",
            Field.From("Id", "Name", "Address"),
            null,
            new DbField("Id", false, true, false, typeof(int), null, null, null, null),
            "WhatEver"));
    }

    #endregion

    #region CreateInsertAll

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateInsertAll()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateInsertAll("Table",
            Field.From("Id", "Name", "Address"),
            3,
            primaryField: null,
            null);
        var expected = "INSERT INTO \"Table\" (\"Id\", \"Name\", \"Address\") " +
            "VALUES " +
            "(@Id, @Name, @Address), " +
            "(@Id_1, @Name_1, @Address_1), " +
            "(@Id_2, @Name_2, @Address_2);";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateInsertAllWithPrimary()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateInsertAll("Table",
            Field.From("Id", "Name", "Address"),
            3,
            new DbField("Id", true, false, false, typeof(int), null, null, null, null),
            null);
        var expected = "INSERT INTO \"Table\" (\"Id\", \"Name\", \"Address\") " +
            "VALUES " +
            "(@Id, @Name, @Address), " +
            "(@Id_1, @Name_1, @Address_1), " +
            "(@Id_2, @Name_2, @Address_2) " +
            "RETURNING \"Id\";";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateInsertAllWithIdentity()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateInsertAll("Table",
            Field.From("Id", "Name", "Address"),
            3,
            null,
            new DbField("Id", false, true, false, typeof(int), null, null, null, null));
        var expected = "INSERT INTO \"Table\" (\"Name\", \"Address\") " +
            "VALUES " +
            "(@Name, @Address), " +
            "(@Name_1, @Address_1), " +
            "(@Name_2, @Address_2) " +
            "RETURNING \"Id\";";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlStatementBuilderCreateInsertAllIfThereAreHints()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => builder.CreateInsertAll("Table",
            Field.From("Id", "Name", "Address"),
            3,
            null,
            new DbField("Id", false, true, false, typeof(int), null, null, null, null),
            "WhatEver"));
    }

    #endregion

    #region CreateMax

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateMax()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateMax("Table",
            new Field("Field", typeof(int)),
            null,
            null);
        var expected = "SELECT MAX (\"Field\") AS \"MaxValue\" FROM \"Table\";";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateMaxWithExpression()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateMax("Table",
            new Field("Field", typeof(int)),
            QueryGroup.Parse(new { Id = 1 }),
            null);
        var expected = "SELECT MAX (\"Field\") AS \"MaxValue\" FROM \"Table\" WHERE (\"Id\" = @Id);";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlStatementBuilderCreateMaxIfThereAreHints()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => builder.CreateMax("Table",
            new Field("Field", typeof(int)),
            QueryGroup.Parse(new { Id = 1 }),
            "WhatEver"));
    }

    #endregion

    #region CreateMaxAll

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateMaxAll()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateMaxAll("Table",
            new Field("Field", typeof(int)),
            null);
        var expected = "SELECT MAX (\"Field\") AS \"MaxValue\" FROM \"Table\";";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlStatementBuilderCreateMaxAllIfThereAreHints()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => builder.CreateMaxAll("Table",
            new Field("Field", typeof(int)),
            "WhatEver"));
    }

    #endregion

    #region CreateMin

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateMin()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateMin("Table",
            new Field("Field", typeof(int)),
            null,
            null);
        var expected = "SELECT MIN (\"Field\") AS \"MinValue\" FROM \"Table\";";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateMinWithExpression()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateMin("Table",
            new Field("Field", typeof(int)),
            QueryGroup.Parse(new { Id = 1 }),
            null);
        var expected = "SELECT MIN (\"Field\") AS \"MinValue\" FROM \"Table\" WHERE (\"Id\" = @Id);";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlStatementBuilderCreateMinIfThereAreHints()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => builder.CreateMin("Table",
            new Field("Field", typeof(int)),
            QueryGroup.Parse(new { Id = 1 }),
            "WhatEver"));
    }

    #endregion

    #region CreateMinAll

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateMinAll()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateMinAll("Table",
            new Field("Field", typeof(int)),
            null);
        var expected = "SELECT MIN (\"Field\") AS \"MinValue\" FROM \"Table\";";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlStatementBuilderCreateMinAllIfThereAreHints()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => builder.CreateMinAll("Table",
            new Field("Field", typeof(int)),
            "WhatEver"));
    }

    #endregion

    #region CreateMerge

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateMerge()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateMerge("Table",
            Field.From("Id", "Name", "Address"),
            null,
            new DbField("Id", true, false, false, typeof(int), null, null, null, null),
            null);
        var expected = "INSERT INTO \"Table\" (\"Id\", \"Name\", \"Address\") VALUES (@Id, @Name, @Address) " +
            "ON CONFLICT (\"Id\") DO " +
            "UPDATE SET \"Name\" = @Name, \"Address\" = @Address " +
            "RETURNING \"Id\";";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateMergeWithPrimaryAsQualifier()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateMerge("Table",
            Field.From("Id", "Name", "Address"),
            Field.From("Id"),
            new DbField("Id", true, false, false, typeof(int), null, null, null, null),
            null);
        var expected = "INSERT INTO \"Table\" (\"Id\", \"Name\", \"Address\") " +
            "VALUES (@Id, @Name, @Address) ON CONFLICT (\"Id\") DO UPDATE SET \"Name\" = @Name, \"Address\" = @Address " +
            "RETURNING \"Id\";";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateMergeWithIdentity()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateMerge("Table",
            Field.From("Id", "Name", "Address"),
            null,
            new DbField("Id", true, true, false, typeof(int), null, null, null, null),
            new DbField("Id", true, true, false, typeof(int), null, null, null, null));
        var expected = "INSERT INTO \"Table\" (\"Id\", \"Name\", \"Address\") " +
            "OVERRIDING SYSTEM VALUE VALUES (@Id, @Name, @Address) ON CONFLICT (\"Id\") DO UPDATE SET \"Name\" = @Name, \"Address\" = @Address " +
            "RETURNING \"Id\";";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlStatementBuilderCreateMergeIfThereIsNoPrimary()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        Assert.ThrowsExactly<PrimaryFieldNotFoundException>(() => builder.CreateMerge("Table",
            Field.From("Id", "Name", "Address"),
            null,
            primaryField: null,
            null));
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlStatementBuilderCreateMergeIfThereAreNoFields()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        Assert.ThrowsExactly<PrimaryFieldNotFoundException>(() => builder.CreateMerge("Table",
            Field.From("Id", "Name", "Address"),
            null,
            primaryField: null,
            null));
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlStatementBuilderCreateMergeIfThereAreHints()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => builder.CreateMerge("Table",
            Field.From("Id", "Name", "Address"),
            Field.From("Id", "Name"),
            new DbField("Id", true, false, false, typeof(int), null, null, null, null),
            null,
            "WhatEver"));
    }

    #endregion

    #region CreateMergeAll

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateMergeAll()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateMergeAll("Table",
            Field.From("Id", "Name", "Address"),
            null,
            3,
            new DbField("Id", true, false, false, typeof(int), null, null, null, null),
            null);
        var expected =
            "INSERT INTO \"Table\" (\"Id\", \"Name\", \"Address\") VALUES (@Id, @Name, @Address) ON CONFLICT (\"Id\") DO UPDATE SET \"Name\" = @Name, \"Address\" = @Address RETURNING \"Id\"; " +
            "INSERT INTO \"Table\" (\"Id\", \"Name\", \"Address\") VALUES (@Id_1, @Name_1, @Address_1) ON CONFLICT (\"Id\") DO UPDATE SET \"Name\" = @Name_1, \"Address\" = @Address_1 RETURNING \"Id\"; " +
            "INSERT INTO \"Table\" (\"Id\", \"Name\", \"Address\") VALUES (@Id_2, @Name_2, @Address_2) ON CONFLICT (\"Id\") DO UPDATE SET \"Name\" = @Name_2, \"Address\" = @Address_2 RETURNING \"Id\";";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateMergeAllWithPrimaryAsQualifier()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateMergeAll("Table",
            Field.From("Id", "Name", "Address"),
            Field.From("Id"),
            3,
            new DbField("Id", true, false, false, typeof(int), null, null, null, null),
            null);
        var expected =
            "INSERT INTO \"Table\" (\"Id\", \"Name\", \"Address\") VALUES (@Id, @Name, @Address) ON CONFLICT (\"Id\") DO UPDATE SET \"Name\" = @Name, \"Address\" = @Address RETURNING \"Id\"; " +
            "INSERT INTO \"Table\" (\"Id\", \"Name\", \"Address\") VALUES (@Id_1, @Name_1, @Address_1) ON CONFLICT (\"Id\") DO UPDATE SET \"Name\" = @Name_1, \"Address\" = @Address_1 RETURNING \"Id\"; " +
            "INSERT INTO \"Table\" (\"Id\", \"Name\", \"Address\") VALUES (@Id_2, @Name_2, @Address_2) ON CONFLICT (\"Id\") DO UPDATE SET \"Name\" = @Name_2, \"Address\" = @Address_2 RETURNING \"Id\";";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateMergeAllWithIdentity()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateMergeAll("Table",
            Field.From("Id", "Name", "Address"),
            null,
            3,
            new DbField("Id", true, true, false, typeof(int), null, null, null, null),
            new DbField("Id", true, true, false, typeof(int), null, null, null, null));
        var expected =
            "INSERT INTO \"Table\" (\"Id\", \"Name\", \"Address\") OVERRIDING SYSTEM VALUE VALUES (@Id, @Name, @Address) ON CONFLICT (\"Id\") DO UPDATE SET \"Name\" = @Name, \"Address\" = @Address RETURNING \"Id\"; " +
            "INSERT INTO \"Table\" (\"Id\", \"Name\", \"Address\") OVERRIDING SYSTEM VALUE VALUES (@Id_1, @Name_1, @Address_1) ON CONFLICT (\"Id\") DO UPDATE SET \"Name\" = @Name_1, \"Address\" = @Address_1 RETURNING \"Id\"; " +
            "INSERT INTO \"Table\" (\"Id\", \"Name\", \"Address\") OVERRIDING SYSTEM VALUE VALUES (@Id_2, @Name_2, @Address_2) ON CONFLICT (\"Id\") DO UPDATE SET \"Name\" = @Name_2, \"Address\" = @Address_2 RETURNING \"Id\";"; ;

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlStatementBuilderCreateMergeAllIfThereIsNoPrimary()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        Assert.ThrowsExactly<PrimaryFieldNotFoundException>(() => builder.CreateMergeAll("Table",
            Field.From("Id", "Name", "Address"),
            null,
            3,
            primaryField: null,
            null));
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlStatementBuilderCreateMergeAllIfThereAreNoFields()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        Assert.ThrowsExactly<PrimaryFieldNotFoundException>(() => builder.CreateMergeAll("Table",
            Field.From("Id", "Name", "Address"),
            null,
            3,
            primaryField: null,
            null));
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlStatementBuilderCreateMergeAllIfThereAreHints()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => builder.CreateMergeAll("Table",
            Field.From("Id", "Name", "Address"),
            Field.From("Id", "Name"),
            3,
            new DbField("Id", true, false, false, typeof(int), null, null, null, null),
            null,
            "WhatEver"));
    }

    #endregion

    #region CreateQuery

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateQuery()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateQuery("Table",
            Field.From("Id", "Name", "Address"),
            null,
            null);
        var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\";";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateQueryWithExpression()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateQuery("Table",
            Field.From("Id", "Name", "Address"),
            QueryGroup.Parse(new { Id = 1, Name = "Michael" }));
        var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" WHERE (\"Id\" = @Id AND \"Name\" = @Name);";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateQueryWithTop()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateQuery("Table",
            Field.From("Id", "Name", "Address"),
            null,
            null,
            10,
            null);
        var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" LIMIT 10;";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateQueryOrderBy()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateQuery("Table",
            Field.From("Id", "Name", "Address"),
            null,
            OrderField.Parse(new { Id = Order.Ascending }));
        var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" ORDER BY \"Id\" ASC;";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateQueryOrderByFields()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateQuery("Table",
            Field.From("Id", "Name", "Address"),
            null,
            OrderField.Parse(new { Id = Order.Ascending, Name = Order.Ascending }));
        var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" ORDER BY \"Id\" ASC, \"Name\" ASC;";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateQueryOrderByDescending()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateQuery("Table",
            Field.From("Id", "Name", "Address"),
            null,
            OrderField.Parse(new { Id = Order.Descending }));
        var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" ORDER BY \"Id\" DESC;";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateQueryOrderByFieldsDescending()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateQuery("Table",
            Field.From("Id", "Name", "Address"),
            null,
            OrderField.Parse(new { Id = Order.Descending, Name = Order.Descending }));
        var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" ORDER BY \"Id\" DESC, \"Name\" DESC;";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateQueryOrderByFieldsMultiDirection()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateQuery("Table",
            Field.From("Id", "Name", "Address"),
            null,
            OrderField.Parse(new { Id = Order.Ascending, Name = Order.Descending }));
        var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" ORDER BY \"Id\" ASC, \"Name\" DESC;";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlStatementBuilderCreateQueryIfThereAreHints()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => builder.CreateQuery("Table",
            Field.From("Id", "Name", "Address"),
            null,
            null,
            hints: "WhatEver"));
    }

    #endregion

    #region CreateSkipQuery

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateSkipQuery()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateSkipQuery("Table",
            Field.From("Id", "Name"),
            0,
            10,
            OrderField.Parse(new { Id = Order.Ascending }));
        var expected = "SELECT \"Id\", \"Name\" FROM \"Table\" ORDER BY \"Id\" ASC LIMIT 10;";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateSkipQueryWithSkip()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateSkipQuery("Table",
            Field.From("Id", "Name"),
            30,
            10,
            OrderField.Parse(new { Id = Order.Ascending }));
        var expected = "SELECT \"Id\", \"Name\" FROM \"Table\" ORDER BY \"Id\" ASC LIMIT 10 OFFSET 30;";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlStatementBuilderCreateSkipQueryIfThereAreNoFields()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        Assert.ThrowsExactly<EmptyException>(() => builder.CreateSkipQuery("Table",
            null,
            0,
            10,
            OrderField.Parse(new { Id = Order.Ascending })));
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlStatementBuilderCreateSkipQueryIfThereAreNoOrderFields()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        Assert.ThrowsExactly<EmptyException>(() => builder.CreateSkipQuery("Table",
            Field.From("Id", "Name"),
            0,
            10,
            null));
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlStatementBuilderCreateSkipQueryIfThePageValueIsNullOrOutOfRange()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => builder.CreateSkipQuery("Table",
            Field.From("Id", "Name"),
            -1,
            10,
            OrderField.Parse(new { Id = Order.Ascending })));
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlStatementBuilderCreateSkipQueryIfTheRowsPerBatchValueIsNullOrOutOfRange()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => builder.CreateSkipQuery("Table",
            Field.From("Id", "Name"),
            0,
            -1,
            OrderField.Parse(new { Id = Order.Ascending })));
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlStatementBuilderCreateSkipQueryIfThereAreHints()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => builder.CreateSkipQuery("Table",
            Field.From("Id", "Name"),
            0,
            -1,
            OrderField.Parse(new { Id = Order.Ascending }),
            null,
            "WhatEver"));
    }

    #endregion

    #region CreateSum

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateSum()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateSum("Table",
            new Field("Field", typeof(int)),
            null,
            null);
        var expected = "SELECT SUM (\"Field\") AS \"SumValue\" FROM \"Table\";";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateSumWithExpression()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateSum("Table",
            new Field("Field", typeof(int)),
            QueryGroup.Parse(new { Id = 1 }),
            null);
        var expected = "SELECT SUM (\"Field\") AS \"SumValue\" FROM \"Table\" WHERE (\"Id\" = @Id);";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlStatementBuilderCreateSumIfThereAreHints()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => builder.CreateSum("Table",
            new Field("Field", typeof(int)),
            QueryGroup.Parse(new { Id = 1 }),
            "WhatEver"));
    }

    #endregion

    #region CreateSumAll

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateSumAll()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        var query = builder.CreateSumAll("Table",
            new Field("Field", typeof(int)),
            null);
        var expected = "SELECT SUM (\"Field\") AS \"SumValue\" FROM \"Table\";";

        // Assert
        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    public void ThrowExceptionOnPostgreSqlStatementBuilderCreateSumAllIfThereAreHints()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

        // Act
        Assert.ThrowsExactly<NotSupportedException>(() => builder.CreateSumAll("Table",
            new Field("Field", typeof(int)),
            "WhatEver"));
    }

    #endregion

    #region CreateUpdate
    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateUpdate()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<NpgsqlConnection>();
        var tableName = "Table";
        var fields = Field.From(new[] { "Field1", "Field2" });
        var primaryField = new DbField("Field1", true, false, false, typeof(int), null, null, null, null);

        // Act
        var actual = statementBuilder.CreateUpdate(tableName: tableName,
            fields: fields,
            where: null,
            primaryField,
            identityField: null,
            hints: null);
        var expected =
            "UPDATE \"Table\" SET \"Field2\" = @Field2;";

        // Assert
        Assert.AreEqual(expected, actual);
    }
    #endregion

    #region CreateUpdateAll
    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateUpdateAll()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<NpgsqlConnection>();
        var tableName = "Table";
        var fields = Field.From(new[] { "Field1", "Field2" });
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
            "UPDATE \"Table\" SET \"Field2\" = @Field2 WHERE (\"Field1\" = @Field1);";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestPostgreSqlStatementBuilderCreateUpdateAllForThreeBatches()
    {
        // Setup
        var statementBuilder = StatementBuilderMapper.Get<NpgsqlConnection>();
        var tableName = "Table";
        var fields = Field.From(new[] { "Field1", "Field2" });
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
            "UPDATE \"Table\" AS T " +
            "SET \"Field2\" = S.\"Field2\" FROM (" +
            "VALUES (@Field1, @Field2), (@Field1_1, @Field2_1), (@Field1_2, @Field2_2)" + "" +
            ") AS S (\"Field1\", \"Field2\") " +
            "WHERE T.\"Field1\" = S.\"Field1\";";

        // Assert
        Assert.AreEqual(expected, actual);
    }
    #endregion
}
