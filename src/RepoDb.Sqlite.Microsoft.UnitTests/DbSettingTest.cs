using Microsoft.Data.Sqlite;

namespace RepoDb.Sqlite.Microsoft.UnitTests;

[TestClass]
public class DbSettingTest
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
    public void TestMsSqliteDbSettingAreTableHintsSupportedProperty()
    {
        // Setup
        var setting = DbSettingMapper.Get<SqliteConnection>();

        // Assert
        Assert.IsFalse(setting.AreTableHintsSupported);
    }

    [TestMethod]
    public void TestMsSqliteDbSettingAverageableTypeProperty()
    {
        // Setup
        var setting = DbSettingMapper.Get<SqliteConnection>();

        // Assert
        Assert.AreEqual(typeof(double), setting.AverageableType);
    }

    [TestMethod]
    public void TestMsSqliteDbSettingClosingQuoteProperty()
    {
        // Setup
        var setting = DbSettingMapper.Get<SqliteConnection>();

        // Assert
        Assert.AreEqual("]", setting.ClosingQuote);
    }

    [TestMethod]
    public void TestMsSqliteDbSettingDefaultSchemaProperty()
    {
        // Setup
        var setting = DbSettingMapper.Get<SqliteConnection>();

        // Assert
        Assert.IsNull(setting.DefaultSchema);
    }

    [TestMethod]
    public void TestMsSqliteDbSettingIsDirectionSupportedSupportedProperty()
    {
        // Setup
        var setting = DbSettingMapper.Get<SqliteConnection>();

        // Assert
        Assert.IsFalse(setting.IsDirectionSupported);
    }

    [TestMethod]
    public void TestMsSqliteDbSettingIsExecuteReaderDisposableProperty()
    {
        // Setup
        var setting = DbSettingMapper.Get<SqliteConnection>();

        // Assert
        Assert.IsFalse(setting.IsExecuteReaderDisposable);
    }

    [TestMethod]
    public void TestMsSqliteDbSettingIsMultiStatementExecutableProperty()
    {
        // Setup
        var setting = DbSettingMapper.Get<SqliteConnection>();

        // Assert
        Assert.IsTrue(setting.IsMultiStatementExecutable);
    }

    [TestMethod]
    public void TestMsSqliteDbSettingOpeningQuoteProperty()
    {
        // Setup
        var setting = DbSettingMapper.Get<SqliteConnection>();

        // Assert
        Assert.AreEqual("[", setting.OpeningQuote);
    }

    [TestMethod]
    public void TestMsSqliteDbSettingParameterPrefixProperty()
    {
        // Setup
        var setting = DbSettingMapper.Get<SqliteConnection>();

        // Assert
        Assert.AreEqual("@", setting.ParameterPrefix);
    }

    #endregion
}
