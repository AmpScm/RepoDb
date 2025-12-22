using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes.Parameter;
using RepoDb.Extensions;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Attributes.Parameter;

[TestClass]
[DoNotParallelize]
public class DbTypeAttributeTest
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

    private class DbTypeAttributeTestClass
    {
        [DbType(DbType.AnsiStringFixedLength)]
        [Size(100)]
        public object ColumnName { get; set; }
    }

    #endregion

    [TestMethod]
    public void TestDbTypeAttributeViaEntityViaCreateParameters()
    {
        // Act
        using var connection = new CustomDbConnection();
        using var command = connection.CreateCommand();
        DbCommandExtension
            .CreateParameters(command, new DbTypeAttributeTestClass
            {
                ColumnName = "Test"
            });

        // Assert
        Assert.HasCount(1, command.Parameters);

        // Assert
        var parameter = command.Parameters["@ColumnName"];
        Assert.AreEqual(DbType.AnsiStringFixedLength, ((CustomDbParameter)parameter).DbType);
    }

    [TestMethod]
    public void TestDbTypeAttributeViaAnonymousViaCreateParameters()
    {
        // Act
        using var connection = new CustomDbConnection();
        using var command = connection.CreateCommand();
        DbCommandExtension
            .CreateParameters(command, new
            {
                ColumnName = "Test"
            },
            typeof(DbTypeAttributeTestClass));

        // Assert
        Assert.HasCount(1, command.Parameters);

        // Assert
        var parameter = command.Parameters["@ColumnName"];
        Assert.AreEqual(DbType.AnsiStringFixedLength, ((CustomDbParameter)parameter).DbType);
    }
}
