using Microsoft.Data.Sqlite;
using RepoDb.Extensions;

namespace RepoDb.Sqlite.Microsoft.UnitTests;

[TestClass]
public class QuotationTest
{
    [TestInitialize]
    public void Initialize()
    {
        GlobalConfiguration
            .Setup()
            .UseSqlite();
    }

    #region MDS

    #region AsQuoted

    [TestMethod]
    public void TestMsSqliteQuotationForQuotedAndTrimmed()
    {
        // Setup
        var setting = DbSettingMapper.Get<SqliteConnection>();

        // Act
        var result = " Field ".AsQuoted(true, setting);

        // Assert
        Assert.AreEqual("[Field]", result);
    }

    [TestMethod]
    public void TestMsSqliteQuotationForQuotedNonTrimmed()
    {
        // Setup
        var setting = DbSettingMapper.Get<SqliteConnection>();

        // Act
        var result = " Field ".AsQuoted(setting);

        // Assert
        Assert.AreEqual("[ Field ]", result);
    }

    [TestMethod]
    public void TestMsSqliteQuotationForQuotedForPreQuoted()
    {
        // Setup
        var setting = DbSettingMapper.Get<SqliteConnection>();

        // Act
        var result = "[Field]".AsQuoted(setting);

        // Assert
        Assert.AreEqual("[Field]", result);
    }

    [TestMethod]
    public void TestMsSqliteQuotationForQuotedForPreQuotedWithSpace()
    {
        // Setup
        var setting = DbSettingMapper.Get<SqliteConnection>();

        // Act
        var result = "[ Field ]".AsQuoted(setting);

        // Assert
        Assert.AreEqual("[ Field ]", result);
    }

    [TestMethod]
    public void TestMsSqliteQuotationForQuotedForPreQuotedWithSpaceAndTrimmed()
    {
        // Setup
        var setting = DbSettingMapper.Get<SqliteConnection>();

        // Act
        var result = " [ Field ] ".AsQuoted(true, setting);

        // Assert
        Assert.AreEqual("[ Field ]", result);
    }

    #endregion

    #region AsUnquoted

    [TestMethod]
    public void TestMsSqliteQuotationForUnquotedAndTrimmed()
    {
        // Setup
        var setting = DbSettingMapper.Get<SqliteConnection>();

        // Act
        var result = " [ Field ] ".AsUnquoted(true, setting);

        // Assert
        Assert.AreEqual("Field", result);
    }

    [TestMethod]
    public void TestMsSqliteQuotationForUnquotedNonTrimmed()
    {
        // Setup
        var setting = DbSettingMapper.Get<SqliteConnection>();

        // Act
        var result = "[ Field ]".AsUnquoted(setting);

        // Assert
        Assert.AreEqual(" Field ", result);
    }

    [TestMethod]
    public void TestMsSqliteQuotationForUnquotedForPlain()
    {
        // Setup
        var setting = DbSettingMapper.Get<SqliteConnection>();

        // Act
        var result = "Field".AsUnquoted(setting);

        // Assert
        Assert.AreEqual("Field", result);
    }

    [TestMethod]
    public void TestMsSqliteQuotationForUnquotedForPlainWithSpace()
    {
        // Setup
        var setting = DbSettingMapper.Get<SqliteConnection>();

        // Act
        var result = " Field ".AsUnquoted(setting);

        // Assert
        Assert.AreEqual(" Field ", result);
    }

    [TestMethod]
    public void TestMsSqliteQuotationForUnquotedAndTrimmedForPlainWithSpace()
    {
        // Setup
        var setting = DbSettingMapper.Get<SqliteConnection>();

        // Act
        var result = " Field ".AsUnquoted(true, setting);

        // Assert
        Assert.AreEqual("Field", result);
    }

    #endregion

    #endregion
}
