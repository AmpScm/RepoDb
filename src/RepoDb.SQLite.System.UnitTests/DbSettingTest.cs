using System.Data.SQLite;

namespace RepoDb.SQLite.System.UnitTests;

[TestClass]
public class DbSettingTest
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
    public void TestSystemSqLiteDbSettingAreTableHintsSupportedProperty()
    {
        // Setup
        var setting = DbSettingMapper.Get<SQLiteConnection>();

        // Assert
        Assert.IsFalse(setting.AreTableHintsSupported);
    }

    [TestMethod]
    public void TestSystemSqLiteDbSettingAverageableTypeProperty()
    {
        // Setup
        var setting = DbSettingMapper.Get<SQLiteConnection>();

        // Assert
        Assert.AreEqual(typeof(double), setting.AverageableType);
    }

    [TestMethod]
    public void TestSystemSqLiteDbSettingClosingQuoteProperty()
    {
        // Setup
        var setting = DbSettingMapper.Get<SQLiteConnection>();

        // Assert
        Assert.AreEqual("]", setting.ClosingQuote);
    }

    [TestMethod]
    public void TestSystemSqLiteDbSettingDefaultSchemaProperty()
    {
        // Setup
        var setting = DbSettingMapper.Get<SQLiteConnection>();

        // Assert
        Assert.IsNull(setting.DefaultSchema);
    }

    [TestMethod]
    public void TestSystemSqLiteDbSettingIsDirectionSupportedSupportedProperty()
    {
        // Setup
        var setting = DbSettingMapper.Get<SQLiteConnection>();

        // Assert
        Assert.IsFalse(setting.IsDirectionSupported);
    }

    [TestMethod]
    public void TestSystemSqLiteDbSettingIsExecuteReaderDisposableProperty()
    {
        // Setup
        var setting = DbSettingMapper.Get<SQLiteConnection>();

        // Assert
        Assert.IsTrue(setting.IsExecuteReaderDisposable);
    }

    [TestMethod]
    public void TestSystemSqLiteDbSettingIsMultiStatementExecutableProperty()
    {
        // Setup
        var setting = DbSettingMapper.Get<SQLiteConnection>();

        // Assert
        Assert.IsTrue(setting.IsMultiStatementExecutable);
    }

    [TestMethod]
    public void TestSystemSqLiteDbSettingOpeningQuoteProperty()
    {
        // Setup
        var setting = DbSettingMapper.Get<SQLiteConnection>();

        // Assert
        Assert.AreEqual("[", setting.OpeningQuote);
    }

    [TestMethod]
    public void TestSystemSqLiteDbSettingParameterPrefixProperty()
    {
        // Setup
        var setting = DbSettingMapper.Get<SQLiteConnection>();

        // Assert
        Assert.AreEqual("@", setting.ParameterPrefix);
    }

    #endregion
}
