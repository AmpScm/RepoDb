using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes.Parameter;
using RepoDb.Extensions;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Attributes.Parameter;

[TestClass]
[DoNotParallelize]
public class SizeAttributeTest
{
    [TestInitialize]
    public void Initialize()
    {
        DbSettingMapper.Add<CustomDbConnection>(new CustomDbSetting(), true);
        DbHelperMapper.Add<CustomDbConnection>(new CustomDbHelper(), true);
    }

    [TestCleanup]
    public void Cleanup()
    {
        DbSettingMapper.Clear();
        DbHelperMapper.Clear();
    }

    #region Classes

    private class SizeAttributeTestClass
    {
        [Size(1000)]
        public object ColumnName { get; set; }
    }

    #endregion

    [TestMethod]
    public void TestSizeAttributeViaEntityViaCreateParameters()
    {
        // Act
        using var connection = new CustomDbConnection();
        using var command = connection.CreateCommand();
        DbCommandExtension
            .CreateParameters(command, new SizeAttributeTestClass
            {
                ColumnName = "Test"
            });

        // Assert
        Assert.HasCount(1, command.Parameters);

        // Assert
        var parameter = command.Parameters["@ColumnName"];
        Assert.AreEqual(1000, ((CustomDbParameter)parameter).Size);
    }

    [TestMethod]
    public void TestSizeAttributeViaAnonymousViaCreateParameters()
    {
        // Act
        using var connection = new CustomDbConnection();
        using var command = connection.CreateCommand();
        DbCommandExtension
            .CreateParameters(command, new
            {
                ColumnName = "Test"
            },
            typeof(SizeAttributeTestClass));

        // Assert
        Assert.HasCount(1, command.Parameters);

        // Assert
        var parameter = command.Parameters["@ColumnName"];
        Assert.AreEqual(1000, ((CustomDbParameter)parameter).Size);
    }
}
