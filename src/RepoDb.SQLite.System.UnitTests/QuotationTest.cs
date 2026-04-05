using RepoDb.Extensions;
using System.Data.SQLite;

namespace RepoDb.SQLite.System.UnitTests;

[TestClass]
public class QuotationTest
{
    [TestInitialize]
    public void Initialize()
    {
        GlobalConfiguration
            .Setup()
            .UseSQLite();
    }

    #region SDS

    #region AsQuoted

    [TestMethod]
    public void TestSystemSqLiteQuotationForQuotedAndTrimmed()
    {
        // Setup
        var setting = DbSettingMapper.Get<SQLiteConnection>();

        // Act
        var result = " Field ".AsQuoted(true, setting);

        // Assert
        Assert.AreEqual("[Field]", result);
    }

    [TestMethod]
    public void TestSystemSqLiteQuotationForQuotedNonTrimmed()
    {
        // Setup
        var setting = DbSettingMapper.Get<SQLiteConnection>();

        // Act
        var result = " Field ".AsQuoted(setting);

        // Assert
        Assert.AreEqual("[ Field ]", result);
    }

    [TestMethod]
    public void TestSystemSqLiteQuotationForQuotedForPreQuoted()
    {
        // Setup
        var setting = DbSettingMapper.Get<SQLiteConnection>();

        // Act
        var result = "[Field]".AsQuoted(setting);

        // Assert
        Assert.AreEqual("[Field]", result);
    }

    [TestMethod]
    public void TestSystemSqLiteQuotationForQuotedForPreQuotedWithSpace()
    {
        // Setup
        var setting = DbSettingMapper.Get<SQLiteConnection>();

        // Act
        var result = "[ Field ]".AsQuoted(setting);

        // Assert
        Assert.AreEqual("[ Field ]", result);
    }

    [TestMethod]
    public void TestSystemSqLiteQuotationForQuotedForPreQuotedWithSpaceAndTrimmed()
    {
        // Setup
        var setting = DbSettingMapper.Get<SQLiteConnection>();

        // Act
        var result = " [ Field ] ".AsQuoted(true, setting);

        // Assert
        Assert.AreEqual("[ Field ]", result);
    }

    #endregion

    #region AsUnquoted

    [TestMethod]
    public void TestSystemSqLiteQuotationForUnquotedAndTrimmed()
    {
        // Setup
        var setting = DbSettingMapper.Get<SQLiteConnection>();

        // Act
        var result = " [ Field ] ".AsUnquoted(true, setting);

        // Assert
        Assert.AreEqual("Field", result);
    }

    [TestMethod]
    public void TestSystemSqLiteQuotationForUnquotedNonTrimmed()
    {
        // Setup
        var setting = DbSettingMapper.Get<SQLiteConnection>();

        // Act
        var result = "[ Field ]".AsUnquoted(setting);

        // Assert
        Assert.AreEqual(" Field ", result);
    }

    [TestMethod]
    public void TestSystemSqLiteQuotationForUnquotedForPlain()
    {
        // Setup
        var setting = DbSettingMapper.Get<SQLiteConnection>();

        // Act
        var result = "Field".AsUnquoted(setting);

        // Assert
        Assert.AreEqual("Field", result);
    }

    [TestMethod]
    public void TestSystemSqLiteQuotationForUnquotedForPlainWithSpace()
    {
        // Setup
        var setting = DbSettingMapper.Get<SQLiteConnection>();

        // Act
        var result = " Field ".AsUnquoted(setting);

        // Assert
        Assert.AreEqual(" Field ", result);
    }

    [TestMethod]
    public void TestSystemSqLiteQuotationForUnquotedAndTrimmedForPlainWithSpace()
    {
        // Setup
        var setting = DbSettingMapper.Get<SQLiteConnection>();

        // Act
        var result = " Field ".AsUnquoted(true, setting);

        // Assert
        Assert.AreEqual("Field", result);
    }

    #endregion

    #endregion
}
