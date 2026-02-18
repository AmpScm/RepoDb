using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.UnitTests;

public partial class QueryGroupTest
{
    #region Equality (==)

    [TestMethod]
    public void TestQueryGroupParseExpressionStringConstant()
    {
        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString == "A").GetString(m_dbSetting);
        var expected = "([PropertyString] = @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringVariable()
    {
        // Setup
        var value = "A";

        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString == value).GetString(m_dbSetting);
        var expected = "([PropertyString] = @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringClassProperty()
    {
        // Setup
        var value = new QueryGroupTestExpressionClass
        {
            PropertyString = "A"
        };

        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString == value.PropertyString).GetString(m_dbSetting);
        var expected = "([PropertyString] = @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringMethodCall()
    {
        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString == GetStringValueForParseExpression()).GetString(m_dbSetting);
        var expected = "([PropertyString] = @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringVariableMethodCall()
    {
        // Setup
        var value = GetStringValueForParseExpression();

        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString == value).GetString(m_dbSetting);
        var expected = "([PropertyString] = @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    #endregion

    #region NotEqual (!=)

    [TestMethod]
    public void TestQueryGroupParseExpressionStringConstantNot()
    {
        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString != "A").GetString(m_dbSetting);
        var expected = "([PropertyString] <> @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringVariableNot()
    {
        // Setup
        var value = "A";

        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString != value).GetString(m_dbSetting);
        var expected = "([PropertyString] <> @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringClassPropertyNot()
    {
        // Setup
        var value = new QueryGroupTestExpressionClass
        {
            PropertyString = "A"
        };

        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString != value.PropertyString).GetString(m_dbSetting);
        var expected = "([PropertyString] <> @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringMethodCallNot()
    {
        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString != GetStringValueForParseExpression()).GetString(m_dbSetting);
        var expected = "([PropertyString] <> @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringVariableMethodCallNot()
    {
        // Setup
        var value = GetStringValueForParseExpression();

        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString != value).GetString(m_dbSetting);
        var expected = "([PropertyString] <> @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    #endregion

    #region String.Equals

    [TestMethod]
    public void TestQueryGroupParseExpressionStringEqualsForConstant()
    {
        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Equals("A")).GetString(m_dbSetting);
        var expected = "([PropertyString] = @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringEqualsStaticForConstant()
    {
        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => string.Equals(e.PropertyString, "A")).GetString(m_dbSetting);
        var expected = "([PropertyString] = @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringEqualsForVariable()
    {
        // Setup
        var value = "A";

        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Equals(value)).GetString(m_dbSetting);
        var expected = "([PropertyString] = @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringEqualsStaticForVariable()
    {
        // Setup
        var value = "A";

        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => string.Equals(e.PropertyString, value)).GetString(m_dbSetting);
        var expected = "([PropertyString] = @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringEqualsForClassProperty()
    {
        // Setup
        var value = new QueryGroupTestExpressionClass
        {
            PropertyString = "A"
        };

        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Equals(value.PropertyString)).GetString(m_dbSetting);
        var expected = "([PropertyString] = @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringEqualsStaticForClassProperty()
    {
        // Setup
        var value = new QueryGroupTestExpressionClass
        {
            PropertyString = "A"
        };

        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => string.Equals(e.PropertyString, value.PropertyString)).GetString(m_dbSetting);
        var expected = "([PropertyString] = @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringEqualsForMethodCall()
    {
        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Equals(GetStringValueForParseExpression())).GetString(m_dbSetting);
        var expected = "([PropertyString] = @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringEqualsStaticForMethodCall()
    {
        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => string.Equals(e.PropertyString, GetStringValueForParseExpression())).GetString(m_dbSetting);
        var expected = "([PropertyString] = @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringEqualsForVariableMethodCall()
    {
        // Setup
        var value = GetStringValueForParseExpression();

        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Equals(value)).GetString(m_dbSetting);
        var expected = "([PropertyString] = @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringEqualsStaticForVariableMethodCall()
    {
        // Setup
        var value = GetStringValueForParseExpression();

        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => string.Equals(e.PropertyString, value)).GetString(m_dbSetting);
        var expected = "([PropertyString] = @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    #endregion

    #region Not String.Equals

    [TestMethod]
    public void TestQueryGroupParseExpressionStringEqualsForConstantNot()
    {
        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Equals("A")).GetString(m_dbSetting);
        var expected = "([PropertyString] <> @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringEqualsStaticForConstantNot()
    {
        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !string.Equals(e.PropertyString, "A")).GetString(m_dbSetting);
        var expected = "([PropertyString] <> @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringEqualsForVariableNot()
    {
        // Setup
        var value = "A";

        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Equals(value)).GetString(m_dbSetting);
        var expected = "([PropertyString] <> @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringEqualsStaticForVariableNot()
    {
        // Setup
        var value = "A";

        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !string.Equals(e.PropertyString, value)).GetString(m_dbSetting);
        var expected = "([PropertyString] <> @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringEqualsForClassPropertyNot()
    {
        // Setup
        var value = new QueryGroupTestExpressionClass
        {
            PropertyString = "A"
        };

        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Equals(value.PropertyString)).GetString(m_dbSetting);
        var expected = "([PropertyString] <> @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringEqualsStaticForClassPropertyNot()
    {
        // Setup
        var value = new QueryGroupTestExpressionClass
        {
            PropertyString = "A"
        };

        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !string.Equals(e.PropertyString, value.PropertyString)).GetString(m_dbSetting);
        var expected = "([PropertyString] <> @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringEqualsForMethodCallNot()
    {
        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Equals(GetStringValueForParseExpression())).GetString(m_dbSetting);
        var expected = "([PropertyString] <> @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringEqualsStaticForMethodCallNot()
    {
        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !string.Equals(e.PropertyString, GetStringValueForParseExpression())).GetString(m_dbSetting);
        var expected = "([PropertyString] <> @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringEqualsForVariableMethodCallNot()
    {
        // Setup
        var value = GetStringValueForParseExpression();

        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Equals(value)).GetString(m_dbSetting);
        var expected = "([PropertyString] <> @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringEqualsStaticForVariableMethodCallNot()
    {
        // Setup
        var value = GetStringValueForParseExpression();

        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !string.Equals(e.PropertyString, value)).GetString(m_dbSetting);
        var expected = "([PropertyString] <> @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    #endregion
}
