using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes.Parameter;
using RepoDb.Extensions;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Attributes.Parameter;

[TestClass]
public class NameAttributeTest
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

    private class NameAttributeTestClass
    {
        [Name("TableColumnName")]
        public object ColumnName { get; set; }
    }

    #endregion

    [TestMethod]
    public void TestNameAttributeViaEntityViaCreateParameters()
    {
        // Act
        using (var connection = new CustomDbConnection())
        {
            using (var command = connection.CreateCommand())
            {
                DbCommandExtension
                    .CreateParameters(command, new NameAttributeTestClass
                    {
                        ColumnName = "Test"
                    });

                // Assert
                Assert.HasCount(1, command.Parameters);

                // Assert
                var parameter = command.Parameters["@TableColumnName"];
                Assert.IsNotNull(parameter);
            }
        }
    }
}
