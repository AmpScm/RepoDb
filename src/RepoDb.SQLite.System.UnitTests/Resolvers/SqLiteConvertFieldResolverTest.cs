using RepoDb.Resolvers;
using System.Data.SQLite;

namespace RepoDb.SQLite.System.UnitTests.Resolvers;

[TestClass]
public class SqLiteConvertFieldResolverTest
{
    [TestInitialize]
    public void Initialize()
    {
        GlobalConfiguration
            .Setup()
            .UseSQLite();
    }

    #region SDS

    [TestMethod]
    public void TestSystemSqLiteConvertFieldResolverForInt32()
    {
        // Setup
        var setting = DbSettingMapper.Get<SQLiteConnection>();
        var resolver = new SQLiteConvertFieldResolver();
        var field = new Field("Field", typeof(int));

        // Act
        var result = resolver.Resolve(field, setting);

        // Assert
        Assert.AreEqual("CAST([Field] AS [INTEGER])", result);
    }

    [TestMethod]
    public void TestSystemSqLiteConvertFieldResolverForInt64()
    {
        // Setup
        var setting = DbSettingMapper.Get<SQLiteConnection>();
        var resolver = new SQLiteConvertFieldResolver();
        var field = new Field("Field", typeof(long));

        // Act
        var result = resolver.Resolve(field, setting);

        // Assert
        Assert.AreEqual("CAST([Field] AS [INTEGER])", result);
    }

    [TestMethod]
    public void TestSystemSqLiteConvertFieldResolverForInt16()
    {
        // Setup
        var setting = DbSettingMapper.Get<SQLiteConnection>();
        var resolver = new SQLiteConvertFieldResolver();
        var field = new Field("Field", typeof(short));

        // Act
        var result = resolver.Resolve(field, setting);

        // Assert
        Assert.AreEqual("CAST([Field] AS [INTEGER])", result);
    }

    [TestMethod]
    public void TestSystemSqLiteConvertFieldResolverForDateTime()
    {
        // Setup
        var setting = DbSettingMapper.Get<SQLiteConnection>();
        var resolver = new SQLiteConvertFieldResolver();
        var field = new Field("Field", typeof(DateTime));

        // Act
        var result = resolver.Resolve(field, setting);

        // Assert
        Assert.AreEqual("CAST([Field] AS [DATETIME])", result);
    }

    [TestMethod]
    public void TestSystemSqLiteConvertFieldResolverForString()
    {
        // Setup
        var setting = DbSettingMapper.Get<SQLiteConnection>();
        var resolver = new SQLiteConvertFieldResolver();
        var field = new Field("Field", typeof(string));

        // Act
        var result = resolver.Resolve(field, setting);

        // Assert
        Assert.AreEqual("CAST([Field] AS [TEXT])", result);
    }

    [TestMethod]
    public void TestSystemSqLiteConvertFieldResolverForByte()
    {
        // Setup
        var setting = DbSettingMapper.Get<SQLiteConnection>();
        var resolver = new SQLiteConvertFieldResolver();
        var field = new Field("Field", typeof(byte));

        // Act
        var result = resolver.Resolve(field, setting);

        // Assert
        Assert.AreEqual("CAST([Field] AS [INTEGER])", result);
    }

    [TestMethod]
    public void TestSystemSqLiteConvertFieldResolverForDecimal()
    {
        // Setup
        var setting = DbSettingMapper.Get<SQLiteConnection>();
        var resolver = new SQLiteConvertFieldResolver();
        var field = new Field("Field", typeof(decimal));

        // Act
        var result = resolver.Resolve(field, setting);

        // Assert
        Assert.AreEqual("CAST([Field] AS [DECIMAL])", result);
    }

    [TestMethod]
    public void TestSystemSqLiteConvertFieldResolverForFloat()
    {
        // Setup
        var setting = DbSettingMapper.Get<SQLiteConnection>();
        var resolver = new SQLiteConvertFieldResolver();
        var field = new Field("Field", typeof(float));

        // Act
        var result = resolver.Resolve(field, setting);

        // Assert
        Assert.AreEqual("CAST([Field] AS [REAL])", result);
    }

    [TestMethod]
    public void TestSystemSqLiteConvertFieldResolverForTimeSpan()
    {
        // Setup
        var setting = DbSettingMapper.Get<SQLiteConnection>();
        var resolver = new SQLiteConvertFieldResolver();
        var field = new Field("Field", typeof(TimeSpan));

        // Act
        var result = resolver.Resolve(field, setting);

        // Assert
        Assert.AreEqual("CAST([Field] AS [TIME])", result);
    }

    #endregion
}
