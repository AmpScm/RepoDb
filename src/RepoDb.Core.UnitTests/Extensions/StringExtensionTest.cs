using RepoDb.Extensions;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests;

[TestClass]
public class StringExtensionTest
{
    #region Join

    [TestMethod]
    public void TestJoinWithTrim()
    {
        // Arrange
        var strings = new[] { " one ", " two ", " three " };
        var separator = ", ";

        // Act
        var result = strings.Join(separator);

        // Assert
        Assert.AreEqual("one, two, three", result);
    }

    [TestMethod]
    public void TestJoinWithoutTrim()
    {
        // Arrange
        var strings = new[] { " one ", " two ", " three " };
        var separator = ", ";

        // Act
        var result = strings.Join(separator, trim: false);

        // Assert
        Assert.AreEqual(" one ,  two ,  three ", result);
    }

    [TestMethod]
    public void TestJoinWithoutWhitespace()
    {
        // Arrange
        var strings = new[] { "one", "two", "three" };
        var separator = "-";

        // Act
        var result = strings.Join(separator);

        // Assert
        Assert.AreEqual("one-two-three", result);
    }

    [TestMethod]
    public void TestJoinEmptyCollection()
    {
        // Arrange
        var strings = Enumerable.Empty<string>();
        var separator = ", ";

        // Act
        var result = strings.Join(separator);

        // Assert
        Assert.AreEqual("", result);
    }

    [TestMethod]
    public void TestJoinSingleItem()
    {
        // Arrange
        var strings = new[] { "only" };
        var separator = ", ";

        // Act
        var result = strings.Join(separator);

        // Assert
        Assert.AreEqual("only", result);
    }

    #endregion

    #region AsAlphaNumeric

    [TestMethod]
    public void TestAsAlphaNumericWithSpecialCharacters()
    {
        // Arrange
        var value = "hello-world!@#$%";

        // Act
        var result = value.AsAlphaNumeric();

        // Assert
        Assert.AreEqual("hello_world_____", result);
    }

    [TestMethod]
    public void TestAsAlphaNumericWithWhitespace()
    {
        // Arrange
        var value = "  hello world  ";

        // Act
        var result = value.AsAlphaNumeric();

        // Assert
        Assert.AreEqual("hello_world", result);
    }

    [TestMethod]
    public void TestAsAlphaNumericWithoutTrim()
    {
        // Arrange
        var value = "  hello-world  ";

        // Act
        var result = value.AsAlphaNumeric(trim: false);

        // Assert
        Assert.AreEqual("__hello_world__", result);
    }

    [TestMethod]
    public void TestAsAlphaNumericWithNumbers()
    {
        // Arrange
        var value = "test123";

        // Act
        var result = value.AsAlphaNumeric();

        // Assert
        Assert.AreEqual("test123", result);
    }

    [TestMethod]
    public void TestAsAlphaNumericEmpty()
    {
        // Arrange
        var value = "";

        // Act
        var result = value.AsAlphaNumeric();

        // Assert
        Assert.AreEqual("", result);
    }

    #endregion

    #region IsOpenQuoted

    [TestMethod]
    public void TestIsOpenQuotedWithQuotedString()
    {
        // Arrange
        var value = "[column]";
        var dbSetting = new CustomDbSetting();

        // Act
        var result = value.IsOpenQuoted(dbSetting);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestIsOpenQuotedWithUnquotedString()
    {
        // Arrange
        var value = "column";
        var dbSetting = new CustomDbSetting();

        // Act
        var result = value.IsOpenQuoted(dbSetting);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void TestIsOpenQuotedWithNullDbSetting()
    {
        // Arrange
        var value = "[column]";

        // Act
        var result = value.IsOpenQuoted(null);

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region IsCloseQuoted

    [TestMethod]
    public void TestIsCloseQuotedWithQuotedString()
    {
        // Arrange
        var value = "[column]";
        var dbSetting = new CustomDbSetting();

        // Act
        var result = value.IsCloseQuoted(dbSetting);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestIsCloseQuotedWithUnquotedString()
    {
        // Arrange
        var value = "column";
        var dbSetting = new CustomDbSetting();

        // Act
        var result = value.IsCloseQuoted(dbSetting);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void TestIsCloseQuotedWithNullDbSetting()
    {
        // Arrange
        var value = "[column]";

        // Act
        var result = value.IsCloseQuoted(null);

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region AsUnquoted

    [TestMethod]
    public void TestAsUnquotedWithQuotedValue()
    {
        // Arrange
        var value = "[ColumnName]";
        var dbSetting = new CustomDbSetting();

        // Act
        var result = value.AsUnquoted(dbSetting);

        // Assert
        Assert.AreEqual("ColumnName", result);
    }

    [TestMethod]
    public void TestAsUnquotedWithUnquotedValue()
    {
        // Arrange
        var value = "ColumnName";
        var dbSetting = new CustomDbSetting();

        // Act
        var result = value.AsUnquoted(dbSetting);

        // Assert
        Assert.AreEqual("ColumnName", result);
    }

    [TestMethod]
    public void TestAsUnquotedWithNullDbSetting()
    {
        // Arrange
        var value = "[ColumnName]";

        // Act
        var result = value.AsUnquoted(null);

        // Assert
        Assert.AreEqual("[ColumnName]", result);
    }

    [TestMethod]
    public void TestAsUnquotedWithEmptyString()
    {
        // Arrange
        var value = "";
        var dbSetting = new CustomDbSetting();

        // Act
        var result = value.AsUnquoted(dbSetting);

        // Assert
        Assert.AreEqual("", result);
    }

    [TestMethod]
    public void TestAsUnquotedWithSchemaQualifiedName()
    {
        // Arrange
        var value = "[dbo].[Table]";
        var dbSetting = new CustomDbSetting();

        // Act
        var result = value.AsUnquoted(dbSetting);

        // Assert
        Assert.AreEqual("dbo.Table", result);
    }

    [TestMethod]
    public void TestAsUnquotedWithTrim()
    {
        // Arrange
        var value = "  [ColumnName]  ";
        var dbSetting = new CustomDbSetting();

        // Act
        var result = value.AsUnquoted(trim: true, dbSetting);

        // Assert
        Assert.AreEqual("ColumnName", result);
    }

    [TestMethod]
    public void TestAsUnquotedWithoutTrim()
    {
        // Arrange
        var value = "  [ColumnName]  ";
        var dbSetting = new CustomDbSetting();

        // Act
        var result = value.AsUnquoted(trim: false, dbSetting);

        // Assert
        Assert.AreEqual("  ColumnName  ", result);
    }

    #endregion
}
