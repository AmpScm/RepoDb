using Microsoft.Data.Sqlite;
using RepoDb.Resolvers;

namespace RepoDb.Sqlite.Microsoft.UnitTests.Resolvers;

[TestClass]
public class SqliteConvertFieldResolverTest
{
    [TestInitialize]
    public void Initialize()
    {
        GlobalConfiguration
            .Setup()
            .UseSqlite();
    }

    #region MDS

    [TestMethod]
    public void TestMsSqliteConvertFieldResolverForInt32()
    {
        // Setup
        var setting = DbSettingMapper.Get<SqliteConnection>();
        var resolver = new SqliteConvertFieldResolver();
        var field = new Field("Field", typeof(int));

        // Act
        var result = resolver.Resolve(field, setting);

        // Assert
        Assert.AreEqual("CAST([Field] AS [INTEGER])", result);
    }

    [TestMethod]
    public void TestMsSqliteConvertFieldResolverForInt64()
    {
        // Setup
        var setting = DbSettingMapper.Get<SqliteConnection>();
        var resolver = new SqliteConvertFieldResolver();
        var field = new Field("Field", typeof(long));

        // Act
        var result = resolver.Resolve(field, setting);

        // Assert
        Assert.AreEqual("CAST([Field] AS [INTEGER])", result);
    }

    [TestMethod]
    public void TestMsSqliteConvertFieldResolverForInt16()
    {
        // Setup
        var setting = DbSettingMapper.Get<SqliteConnection>();
        var resolver = new SqliteConvertFieldResolver();
        var field = new Field("Field", typeof(short));

        // Act
        var result = resolver.Resolve(field, setting);

        // Assert
        Assert.AreEqual("CAST([Field] AS [INTEGER])", result);
    }

    [TestMethod]
    public void TestMsSqliteConvertFieldResolverForDateTime()
    {
        // Setup
        var setting = DbSettingMapper.Get<SqliteConnection>();
        var resolver = new SqliteConvertFieldResolver();
        var field = new Field("Field", typeof(DateTime));

        // Act
        var result = resolver.Resolve(field, setting);

        // Assert
        Assert.AreEqual("CAST([Field] AS [DATETIME])", result);
    }

    [TestMethod]
    public void TestMsSqliteConvertFieldResolverForString()
    {
        // Setup
        var setting = DbSettingMapper.Get<SqliteConnection>();
        var resolver = new SqliteConvertFieldResolver();
        var field = new Field("Field", typeof(string));

        // Act
        var result = resolver.Resolve(field, setting);

        // Assert
        Assert.AreEqual("CAST([Field] AS [TEXT])", result);
    }

    [TestMethod]
    public void TestMsSqliteConvertFieldResolverForByte()
    {
        // Setup
        var setting = DbSettingMapper.Get<SqliteConnection>();
        var resolver = new SqliteConvertFieldResolver();
        var field = new Field("Field", typeof(byte));

        // Act
        var result = resolver.Resolve(field, setting);

        // Assert
        Assert.AreEqual("CAST([Field] AS [INTEGER])", result);
    }

    [TestMethod]
    public void TestMsSqliteConvertFieldResolverForDecimal()
    {
        // Setup
        var setting = DbSettingMapper.Get<SqliteConnection>();
        var resolver = new SqliteConvertFieldResolver();
        var field = new Field("Field", typeof(decimal));

        // Act
        var result = resolver.Resolve(field, setting);

        // Assert
        Assert.AreEqual("CAST([Field] AS [DECIMAL])", result);
    }

    [TestMethod]
    public void TestMsSqliteConvertFieldResolverForFloat()
    {
        // Setup
        var setting = DbSettingMapper.Get<SqliteConnection>();
        var resolver = new SqliteConvertFieldResolver();
        var field = new Field("Field", typeof(float));

        // Act
        var result = resolver.Resolve(field, setting);

        // Assert
        Assert.AreEqual("CAST([Field] AS [REAL])", result);
    }

    [TestMethod]
    public void TestMsSqliteConvertFieldResolverForTimeSpan()
    {
        // Setup
        var setting = DbSettingMapper.Get<SqliteConnection>();
        var resolver = new SqliteConvertFieldResolver();
        var field = new Field("Field", typeof(TimeSpan));

        // Act
        var result = resolver.Resolve(field, setting);

        // Assert
        Assert.AreEqual("CAST([Field] AS [TIME])", result);
    }

    #endregion
}
