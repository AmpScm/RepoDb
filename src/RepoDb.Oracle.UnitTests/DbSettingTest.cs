
using Oracle.ManagedDataAccess.Client;

namespace RepoDb.Oracle.System.UnitTests;

[TestClass]
public class DbSettingTest
{
    [TestInitialize]
    public void Initialize()
    {
        GlobalConfiguration
            .Setup()
            .UseOracle();
    }

    #region Oracle

    [TestMethod]
    public void TestOracleSettingAreTableHintsSupportedProperty()
    {
        // Setup
        var setting = DbSettingMapper.Get<OracleConnection>();

        // Assert
        Assert.IsFalse(setting.AreTableHintsSupported);
    }

    [TestMethod]
    public void TestOracleSettingAverageableTypeProperty()
    {
        // Setup
        var setting = DbSettingMapper.Get<OracleConnection>();

        // Assert
        Assert.AreEqual(typeof(decimal), setting.AverageableType);
    }

    [TestMethod]
    public void TestOracleSettingClosingQuoteProperty()
    {
        // Setup
        var setting = DbSettingMapper.Get<OracleConnection>();

        // Assert
        Assert.AreEqual("\"", setting.ClosingQuote);
    }

    [TestMethod]
    public void TestOracleSettingDefaultSchemaProperty()
    {
        // Setup
        var setting = DbSettingMapper.Get<OracleConnection>();

        // Assert
        Assert.IsNull(setting.DefaultSchema);
    }

    [TestMethod]
    public void TestOracleSettingIsDirectionSupportedSupportedProperty()
    {
        // Setup
        var setting = DbSettingMapper.Get<OracleConnection>();

        // Assert
        Assert.IsTrue(setting.IsDirectionSupported);
    }

    [TestMethod]
    public void TestOracleSettingIsExecuteReaderDisposableProperty()
    {
        // Setup
        var setting = DbSettingMapper.Get<OracleConnection>();

        // Assert
        Assert.IsTrue(setting.IsExecuteReaderDisposable);
    }

    [TestMethod]
    public void TestOracleSettingIsMultiStatementExecutableProperty()
    {
        // Setup
        var setting = DbSettingMapper.Get<OracleConnection>();

        // Assert
        Assert.IsTrue(setting.IsMultiStatementExecutable);
    }

    [TestMethod]
    public void TestOracleSettingOpeningQuoteProperty()
    {
        // Setup
        var setting = DbSettingMapper.Get<OracleConnection>();

        // Assert
        Assert.AreEqual("\"", setting.OpeningQuote);
    }

    [TestMethod]
    public void TestOracleSettingParameterPrefixProperty()
    {
        // Setup
        var setting = DbSettingMapper.Get<OracleConnection>();

        // Assert
        Assert.AreEqual(":", setting.ParameterPrefix);
    }

    #endregion
}
