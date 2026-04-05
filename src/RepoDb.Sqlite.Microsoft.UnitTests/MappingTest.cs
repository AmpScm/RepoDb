using Microsoft.Data.Sqlite;

namespace RepoDb.Sqlite.Microsoft.UnitTests;

[TestClass]
public class MappingTest
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
    public void TestMsSqliteStatementBuilderMapper()
    {
        // Setup
        var builder = StatementBuilderMapper.Get<SqliteConnection>();

        // Assert
        Assert.IsNotNull(builder);
    }

    [TestMethod]
    public void TestMsSqliteDbHelperMapper()
    {
        // Setup
        var helper = DbHelperMapper.Get<SqliteConnection>();

        // Assert
        Assert.IsNotNull(helper);
    }

    [TestMethod]
    public void TestMsSqliteDbSettingMapper()
    {
        // Setup
        var setting = DbSettingMapper.Get<SqliteConnection>();

        // Assert
        Assert.IsNotNull(setting);
    }

    #endregion
}
