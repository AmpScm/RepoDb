using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes.Parameter;
using RepoDb.Extensions;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Attributes.Parameter;

[TestClass]
[DoNotParallelize]
public class DirectionAttributeTest
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

    private class DirectionAttributeTestClass
    {
        [Direction(ParameterDirection.Output)]
        public object ColumnName { get; set; }
    }

    #endregion

    [TestMethod]
    public void TestDirectionAttributeViaEntityViaCreateParameters()
    {
        // Act
        using (var connection = new CustomDbConnection())
        {
            using (var command = connection.CreateCommand())
            {
                DbCommandExtension
                    .CreateParameters(command, new DirectionAttributeTestClass
                    {
                        ColumnName = "Test"
                    });

                // Assert
                Assert.HasCount(1, command.Parameters);

                // Assert
                var parameter = command.Parameters["@ColumnName"];
                Assert.AreEqual(ParameterDirection.Output, ((CustomDbParameter)parameter).Direction);
            }
        }
    }

    [TestMethod]
    public void TestDirectionAttributeViaAnonymousViaCreateParameters()
    {
        // Act
        using (var connection = new CustomDbConnection())
        {
            using (var command = connection.CreateCommand())
            {
                DbCommandExtension
                    .CreateParameters(command, new
                    {
                        ColumnName = "Test"
                    },
                    typeof(DirectionAttributeTestClass));

                // Assert
                Assert.HasCount(1, command.Parameters);

                // Assert
                var parameter = command.Parameters["@ColumnName"];
                Assert.AreEqual(ParameterDirection.Output, ((CustomDbParameter)parameter).Direction);
            }
        }
    }
}
