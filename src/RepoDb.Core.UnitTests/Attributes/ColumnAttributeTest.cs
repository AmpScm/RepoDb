using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Attributes;

[TestClass]
[DoNotParallelize]
public class ColumnAttributeTest
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

    private class TestColumnAttributeUnquotedNameClass
    {
        [Column("PrimaryId")]
        public int Id { get; set; }
    }

    private class TestColumnAttributeQuotedNameClass
    {
        [Column("[PrimaryId]")]
        public int Id { get; set; }
    }

    #endregion

    /*
     * Unquoted
     */

    [TestMethod]
    public void TestColumnAttributeViaExpression()
    {
        // Act
        var actual = PropertyMappedNameCache.Get<TestColumnAttributeUnquotedNameClass>(e => e.Id);
        var expected = "PrimaryId";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestColumnAttributeViaPropertyName()
    {
        // Act
        var actual = PropertyMappedNameCache.Get<TestColumnAttributeUnquotedNameClass>("Id");
        var expected = "PrimaryId";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestColumnAttributeViaField()
    {
        // Act
        var actual = PropertyMappedNameCache.Get<TestColumnAttributeUnquotedNameClass>(new Field("Id"));
        var expected = "PrimaryId";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestColumnAttributeUnquotedNameViaEntityViaCreateParameters()
    {
        // Act
        using var connection = new CustomDbConnection();
        var command = connection.CreateCommand();
        DbCommandExtension
            .CreateParameters(command, new TestColumnAttributeUnquotedNameClass
            {
                Id = 1
            });

        // Assert
        Assert.HasCount(1, command.Parameters);

        // Assert
        var parameter = command.Parameters["@PrimaryId"];
        Assert.IsNotNull(parameter);
    }

    [TestMethod]
    public void TestColumnAttributeUnquotedNameViaAnonymousViaCreateParameters()
    {
        // Act
        using var connection = new CustomDbConnection();
        var command = connection.CreateCommand();
        DbCommandExtension
            .CreateParameters(command, new
            {
                PrimaryId = 1
            },
            typeof(TestColumnAttributeUnquotedNameClass));

        // Assert
        Assert.HasCount(1, command.Parameters);

        // Assert
        var parameter = command.Parameters["@PrimaryId"];
        Assert.IsNotNull(parameter);
    }

    /*
     * Quoted
     */

    [TestMethod]
    public void TestColumnAttributeQuotedNameViaExpression()
    {
        // Act
        var actual = PropertyMappedNameCache.Get<TestColumnAttributeQuotedNameClass>(e => e.Id);
        var expected = "PrimaryId";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestColumnAttributeQuotedNameViaPropertyName()
    {
        // Act
        var actual = PropertyMappedNameCache.Get<TestColumnAttributeQuotedNameClass>("Id");
        var expected = "PrimaryId";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestColumnAttributeQuotedNameViaField()
    {
        // Act
        var actual = PropertyMappedNameCache.Get<TestColumnAttributeQuotedNameClass>(new Field("Id"));
        var expected = "PrimaryId";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestColumnAttributeQuotedNameViaEntityViaCreateParameters()
    {
        // Act
        using var connection = new CustomDbConnection();
        var command = connection.CreateCommand();
        DbCommandExtension
            .CreateParameters(command, new TestColumnAttributeQuotedNameClass
            {
                Id = 1
            });

        // Assert
        Assert.HasCount(1, command.Parameters);

        // Assert
        var parameter = command.Parameters["@PrimaryId"];
        Assert.IsNotNull(parameter);
    }

    [TestMethod]
    public void TestColumnAttributeQuotedNameViaAnonymousViaCreateParameters()
    {
        // Act
        using var connection = new CustomDbConnection();
        var command = connection.CreateCommand();
        DbCommandExtension
            .CreateParameters(command, new
            {
                PrimaryId = 1
            },
            typeof(TestColumnAttributeQuotedNameClass));

        // Assert
        Assert.HasCount(1, command.Parameters);

        // Assert
        var parameter = command.Parameters["@PrimaryId"];
        Assert.IsNotNull(parameter);
    }
}
