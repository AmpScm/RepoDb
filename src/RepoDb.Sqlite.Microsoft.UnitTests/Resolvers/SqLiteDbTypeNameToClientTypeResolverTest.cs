using RepoDb.Resolvers;

namespace RepoDb.Sqlite.Microsoft.UnitTests.Resolvers;

[TestClass]
public class SqliteDbTypeNameToClientTypeResolverTest
{
    [TestInitialize]
    public void Initialize()
    {
        GlobalConfiguration
            .Setup(new())
            .UseSqlite();
    }

    [TestMethod]
    public void TestMsSqliteDbTypeNameToClientTypeResolverForBigInt()
    {
        // Setup
        var resolver = new SqliteDbTypeNameToClientTypeResolver();

        // Act
        var result = resolver.Resolve("BIGINT");

        // Assert
        Assert.AreEqual(typeof(long), result);
    }

    [TestMethod]
    public void TestMsSqliteDbTypeNameToClientTypeResolverForInteger()
    {
        // Setup
        var resolver = new SqliteDbTypeNameToClientTypeResolver();

        // Act
        var result = resolver.Resolve("INTEGER");

        // Assert
        Assert.AreEqual(typeof(long), result);
    }

    [TestMethod]
    public void TestMsSqliteDbTypeNameToClientTypeResolverForBlob()
    {
        // Setup
        var resolver = new SqliteDbTypeNameToClientTypeResolver();

        // Act
        var result = resolver.Resolve("BLOB");

        // Assert
        Assert.AreEqual(typeof(byte[]), result);
    }

    [TestMethod]
    public void TestMsSqliteDbTypeNameToClientTypeResolverForBoolean()
    {
        // Setup
        var resolver = new SqliteDbTypeNameToClientTypeResolver();

        // Act
        var result = resolver.Resolve("BOOLEAN");

        // Assert
        Assert.AreEqual(typeof(string), result);
    }

    [TestMethod]
    public void TestMsSqliteDbTypeNameToClientTypeResolverForChar()
    {
        // Setup
        var resolver = new SqliteDbTypeNameToClientTypeResolver();

        // Act
        var result = resolver.Resolve("CHAR");

        // Assert
        Assert.AreEqual(typeof(string), result);
    }

    [TestMethod]
    public void TestMsSqliteDbTypeNameToClientTypeResolverForString()
    {
        // Setup
        var resolver = new SqliteDbTypeNameToClientTypeResolver();

        // Act
        var result = resolver.Resolve("STRING");

        // Assert
        Assert.AreEqual(typeof(string), result);
    }

    [TestMethod]
    public void TestMsSqliteDbTypeNameToClientTypeResolverForText()
    {
        // Setup
        var resolver = new SqliteDbTypeNameToClientTypeResolver();

        // Act
        var result = resolver.Resolve("TEXT");

        // Assert
        Assert.AreEqual(typeof(string), result);
    }

    [TestMethod]
    public void TestMsSqliteDbTypeNameToClientTypeResolverForVarChar()
    {
        // Setup
        var resolver = new SqliteDbTypeNameToClientTypeResolver();

        // Act
        var result = resolver.Resolve("VARCHAR");

        // Assert
        Assert.AreEqual(typeof(string), result);
    }

    [TestMethod]
    public void TestMsSqliteDbTypeNameToClientTypeResolverForDate()
    {
        // Setup
        var resolver = new SqliteDbTypeNameToClientTypeResolver();

        // Act
        var result = resolver.Resolve("DATE");

        // Assert
        Assert.AreEqual(typeof(DateTime), result);
    }

    [TestMethod]
    public void TestMsSqliteDbTypeNameToClientTypeResolverForDateTime()
    {
        // Setup
        var resolver = new SqliteDbTypeNameToClientTypeResolver();

        // Act
        var result = resolver.Resolve("DATETIME");

        // Assert
        Assert.AreEqual(typeof(DateTime), result);
    }

    [TestMethod]
    public void TestMsSqliteDbTypeNameToClientTypeResolverForTime()
    {
        // Setup
        var resolver = new SqliteDbTypeNameToClientTypeResolver();

        // Act
        var result = resolver.Resolve("TIME");

        // Assert
        Assert.AreEqual(typeof(DateTime), result);
    }

    [TestMethod]
    public void TestMsSqliteDbTypeNameToClientTypeResolverForDecimal()
    {
        // Setup
        var resolver = new SqliteDbTypeNameToClientTypeResolver();

        // Act
        var result = resolver.Resolve("DECIMAL");

        // Assert
        Assert.AreEqual(typeof(decimal), result);
    }

    [TestMethod]
    public void TestMsSqliteDbTypeNameToClientTypeResolverForNumeric()
    {
        // Setup
        var resolver = new SqliteDbTypeNameToClientTypeResolver();

        // Act
        var result = resolver.Resolve("NUMERIC");

        // Assert
        Assert.AreEqual(typeof(decimal), result);
    }

    [TestMethod]
    public void TestMsSqliteDbTypeNameToClientTypeResolverForDouble()
    {
        // Setup
        var resolver = new SqliteDbTypeNameToClientTypeResolver();

        // Act
        var result = resolver.Resolve("DOUBLE");

        // Assert
        Assert.AreEqual(typeof(double), result);
    }

    [TestMethod]
    public void TestMsSqliteDbTypeNameToClientTypeResolverForReal()
    {
        // Setup
        var resolver = new SqliteDbTypeNameToClientTypeResolver();

        // Act
        var result = resolver.Resolve("REAL");

        // Assert
        Assert.AreEqual(typeof(double), result);
    }

    [TestMethod]
    public void TestMsSqliteDbTypeNameToClientTypeResolverForInt()
    {
        // Setup
        var resolver = new SqliteDbTypeNameToClientTypeResolver();

        // Act
        var result = resolver.Resolve("INT");

        // Assert
        Assert.AreEqual(typeof(long), result);
    }

    [TestMethod]
    public void TestMsSqliteDbTypeNameToClientTypeResolverForNone()
    {
        // Setup
        var resolver = new SqliteDbTypeNameToClientTypeResolver();

        // Act
        var result = resolver.Resolve("NONE");

        // Assert
        Assert.AreEqual(typeof(string), result);
    }

    [TestMethod]
    public void TestMsSqliteDbTypeNameToClientTypeResolverForOther()
    {
        // Setup
        var resolver = new SqliteDbTypeNameToClientTypeResolver();

        // Act
        var result = resolver.Resolve("WHATEVER");

        // Assert
        Assert.AreEqual(typeof(object), result);
    }
}
