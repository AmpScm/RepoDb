using RepoDb.Resolvers;
using System.Data;

namespace RepoDb.Sqlite.Microsoft.UnitTests.Resolvers;

[TestClass]
public class DbTypeToSqliteStringNameResolverTest
{
    [TestMethod]
    public void TestDbTypeToSqliteStringNameResolverInt64()
    {
        // Setup
        var resolver = new DbTypeToSqliteStringNameResolver();

        // Assert
        Assert.AreEqual("INTEGER", resolver.Resolve(DbType.Int64));
    }

    [TestMethod]
    public void TestDbTypeToSqliteStringNameResolverByte()
    {
        // Setup
        var resolver = new DbTypeToSqliteStringNameResolver();

        // Assert
        Assert.AreEqual("INTEGER", resolver.Resolve(DbType.Byte));
    }

    [TestMethod]
    public void TestDbTypeToSqliteStringNameResolverBinary()
    {
        // Setup
        var resolver = new DbTypeToSqliteStringNameResolver();

        // Assert
        Assert.AreEqual("BLOB", resolver.Resolve(DbType.Binary));
    }

    [TestMethod]
    public void TestDbTypeToSqliteStringNameResolverBoolean()
    {
        // Setup
        var resolver = new DbTypeToSqliteStringNameResolver();

        // Assert
        Assert.AreEqual("BOOLEAN", resolver.Resolve(DbType.Boolean));
    }

    [TestMethod]
    public void TestDbTypeToSqliteStringNameResolverString()
    {
        // Setup
        var resolver = new DbTypeToSqliteStringNameResolver();

        // Assert
        Assert.AreEqual("TEXT", resolver.Resolve(DbType.String));
    }

    [TestMethod]
    public void TestDbTypeToSqliteStringNameResolverAnsiString()
    {
        // Setup
        var resolver = new DbTypeToSqliteStringNameResolver();

        // Assert
        Assert.AreEqual("TEXT", resolver.Resolve(DbType.AnsiString));
    }

    [TestMethod]
    public void TestDbTypeToSqliteStringNameResolverAnsiStringFixedLength()
    {
        // Setup
        var resolver = new DbTypeToSqliteStringNameResolver();

        // Assert
        Assert.AreEqual("TEXT", resolver.Resolve(DbType.AnsiStringFixedLength));
    }

    [TestMethod]
    public void TestDbTypeToSqliteStringNameResolverStringFixedLength()
    {
        // Setup
        var resolver = new DbTypeToSqliteStringNameResolver();

        // Assert
        Assert.AreEqual("TEXT", resolver.Resolve(DbType.StringFixedLength));
    }

    [TestMethod]
    public void TestDbTypeToSqliteStringNameResolverDate()
    {
        // Setup
        var resolver = new DbTypeToSqliteStringNameResolver();

        // Assert
        Assert.AreEqual("DATE", resolver.Resolve(DbType.Date));
    }

    [TestMethod]
    public void TestDbTypeToSqliteStringNameResolverDateTime()
    {
        // Setup
        var resolver = new DbTypeToSqliteStringNameResolver();

        // Assert
        Assert.AreEqual("DATETIME", resolver.Resolve(DbType.DateTime));
    }

    [TestMethod]
    public void TestDbTypeToSqliteStringNameResolverDateTime2()
    {
        // Setup
        var resolver = new DbTypeToSqliteStringNameResolver();

        // Assert
        Assert.AreEqual("DATETIME", resolver.Resolve(DbType.DateTime2));
    }

    [TestMethod]
    public void TestDbTypeToSqliteStringNameResolverDateTimeOffset()
    {
        // Setup
        var resolver = new DbTypeToSqliteStringNameResolver();

        // Assert
        Assert.AreEqual("DATETIME", resolver.Resolve(DbType.DateTimeOffset));
    }

    [TestMethod]
    public void TestDbTypeToSqliteStringNameResolverDecimal()
    {
        // Setup
        var resolver = new DbTypeToSqliteStringNameResolver();

        // Assert
        Assert.AreEqual("DECIMAL", resolver.Resolve(DbType.Decimal));
    }

    [TestMethod]
    public void TestDbTypeToSqliteStringNameResolverSingle()
    {
        // Setup
        var resolver = new DbTypeToSqliteStringNameResolver();

        // Assert
        Assert.AreEqual("REAL", resolver.Resolve(DbType.Single));
    }

    [TestMethod]
    public void TestDbTypeToSqliteStringNameResolverDouble()
    {
        // Setup
        var resolver = new DbTypeToSqliteStringNameResolver();

        // Assert
        Assert.AreEqual("REAL", resolver.Resolve(DbType.Double));
    }

    [TestMethod]
    public void TestDbTypeToSqliteStringNameResolverInt32()
    {
        // Setup
        var resolver = new DbTypeToSqliteStringNameResolver();

        // Assert
        Assert.AreEqual("INTEGER", resolver.Resolve(DbType.Int32));
    }

    [TestMethod]
    public void TestDbTypeToSqliteStringNameResolverInt16()
    {
        // Setup
        var resolver = new DbTypeToSqliteStringNameResolver();

        // Assert
        Assert.AreEqual("INTEGER", resolver.Resolve(DbType.Int16));
    }

    [TestMethod]
    public void TestDbTypeToSqliteStringNameResolverTime()
    {
        // Setup
        var resolver = new DbTypeToSqliteStringNameResolver();

        // Assert
        Assert.AreEqual("TIME", resolver.Resolve(DbType.Time));
    }
}
