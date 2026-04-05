#nullable enable
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using RepoDb.Extensions;

namespace RepoDb.UnitTests;

public partial class QueryGroupTest
{
    #region String

    [TestMethod]
    public void TestQueryGroupParseExpressionStringContainsAtProperty()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains("A"));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] LIKE @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotStringContainsAtProperty()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains("A"));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] NOT LIKE @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringContainsEqualsTrueAtProperty()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains("A") == true);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] LIKE @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringContainsNotEqualsTrueAtProperty()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains("A") != true);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "NOT ([PropertyString] LIKE @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotStringContainsEqualsTrueAtProperty()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains("A") == true);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] NOT LIKE @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotStringContainsNotEqualsTrueAtProperty()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains("A") != true);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "NOT ([PropertyString] NOT LIKE @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringContainsEqualsFalseAtProperty()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains("A") == false);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "NOT ([PropertyString] LIKE @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringContainsNotEqualsFalseAtProperty()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains("A") != false);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] LIKE @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotStringContainsEqualsFalseAtProperty()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains("A") == false);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "NOT ([PropertyString] NOT LIKE @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotStringContainsNotEqualsFalseAtProperty()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains("A") != false);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] NOT LIKE @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringContainsFromClassProperty()
    {
        // Setup
        var @class = new QueryGroupTestExpressionClass
        {
            PropertyString = "A"
        };
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains(@class.PropertyString));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] LIKE @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotStringContainsFromClassProperty()
    {
        // Setup
        var @class = new QueryGroupTestExpressionClass
        {
            PropertyString = "A"
        };
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains(@class.PropertyString));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] NOT LIKE @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringContainsEqualsFalseFromClassProperty()
    {
        // Setup
        var @class = new QueryGroupTestExpressionClass
        {
            PropertyString = "A"
        };
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains(@class.PropertyString) == false);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "NOT ([PropertyString] LIKE @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotStringContainsEqualsFalseFromClassProperty()
    {
        // Setup
        var @class = new QueryGroupTestExpressionClass
        {
            PropertyString = "A"
        };
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains(@class.PropertyString) == false);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "NOT ([PropertyString] NOT LIKE @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringContainsEqualsTrueFromClassProperty()
    {
        // Setup
        var @class = new QueryGroupTestExpressionClass
        {
            PropertyString = "A"
        };
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains(@class.PropertyString) == true);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] LIKE @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotStringContainsEqualsTrueFromClassProperty()
    {
        // Setup
        var @class = new QueryGroupTestExpressionClass
        {
            PropertyString = "A"
        };
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains(@class.PropertyString) == true);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] NOT LIKE @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringContainsFromClassMethod()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains(GetStringValueForParseExpression()));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] LIKE @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotStringContainsFromClassMethod()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains(GetStringValueForParseExpression()));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] NOT LIKE @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringContainsEqualsFalseFromClassMethod()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains(GetStringValueForParseExpression()) == false);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "NOT ([PropertyString] LIKE @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotStringContainsEqualsFalseFromClassMethod()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains(GetStringValueForParseExpression()) == false);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "NOT ([PropertyString] LIKE @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringContainsEqualsTrueFromClassMethod()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains(GetStringValueForParseExpression()) == true);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] LIKE @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotStringContainsEqualsTrueFromClassMethod()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains(GetStringValueForParseExpression()) == true);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] NOT LIKE @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringContainsWithTwoConditionsForOr()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains("A") || e.PropertyString.Contains("B"));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] LIKE @PropertyString OR [PropertyString] LIKE @PropertyString_1)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringContainsWithTwoConditionsForAnd()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains("A") && e.PropertyString.Contains("B"));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] LIKE @PropertyString AND [PropertyString] LIKE @PropertyString_1)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringContainsWithTwoConditionsForOrEqualsFalse()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (e.PropertyString.Contains("A") || e.PropertyString.Contains("B")) == false);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "NOT ([PropertyString] LIKE @PropertyString OR [PropertyString] LIKE @PropertyString_1)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringContainsWithTwoConditionsForOrEqualsTrue()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (e.PropertyString.Contains("A") || e.PropertyString.Contains("B")) == true);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] LIKE @PropertyString OR [PropertyString] LIKE @PropertyString_1)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringContainsWithTwoConditionsForAndEqualsFalse()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (e.PropertyString.Contains("A") && e.PropertyString.Contains("B")) == false);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "NOT ([PropertyString] LIKE @PropertyString AND [PropertyString] LIKE @PropertyString_1)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringContainsWithTwoConditionsForAndEqualsTrue()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (e.PropertyString.Contains("A") && e.PropertyString.Contains("B")) == true);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] LIKE @PropertyString AND [PropertyString] LIKE @PropertyString_1)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotStringContainsAtLeftAndContainsAtRightForOr()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains("A") || e.PropertyString.Contains("B"));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] NOT LIKE @PropertyString OR [PropertyString] LIKE @PropertyString_1)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotStringContainsAtLeftAndContainsAtRightForAnd()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains("A") && e.PropertyString.Contains("B"));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] NOT LIKE @PropertyString AND [PropertyString] LIKE @PropertyString_1)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringContainsAtLeftAndNotContainsAtRightForOr()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains("A") || !e.PropertyString.Contains("B"));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] LIKE @PropertyString OR [PropertyString] NOT LIKE @PropertyString_1)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringContainsAtLeftAndNotContainsAtRightForAnd()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains("A") && !e.PropertyString.Contains("B"));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] LIKE @PropertyString AND [PropertyString] NOT LIKE @PropertyString_1)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotStringContainsAtLeftAndNotContainsAtRightForOr()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains("A") || !e.PropertyString.Contains("B"));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] NOT LIKE @PropertyString OR [PropertyString] NOT LIKE @PropertyString_1)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotStringContainsAtLeftAndNotContainsAtRightForAnd()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains("A") && !e.PropertyString.Contains("B"));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] NOT LIKE @PropertyString AND [PropertyString] NOT LIKE @PropertyString_1)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringPropertyContainsAndArrayAnyMethod()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains("A") && (new[] { "B", "C" }).Any(p => p != e.PropertyString));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "(([PropertyString] LIKE @PropertyString) AND ([PropertyString] <> @PropertyString_1 OR [PropertyString] <> @PropertyString_2))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringContainsAtMappedProperty()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.MappedPropertyString.Contains("A"));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] LIKE @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringContainsAtQuotedProperty()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.QuotedPropertyString.Contains("A"));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] LIKE @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionStringContainsAtUnorganizedProperty()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.UnorganizedPropertyString.Contains("A"));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([Property / . String] LIKE @Property_____String)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    #endregion

    #region Array

    [TestMethod]
    public void TestQueryGroupParseExpressionArrayContains()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new int[] { 1, 2 }).Contains(e.PropertyInt));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotArrayContains()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new int[] { 1, 2 }).Contains(e.PropertyInt));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyInt] NOT IN (@PropertyInt_In_0, @PropertyInt_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotArrayContainsEqualsTrue()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new int[] { 1, 2 }).Contains(e.PropertyInt) == true);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyInt] NOT IN (@PropertyInt_In_0, @PropertyInt_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionArrayContainsEqualsFalse()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new int[] { 1, 2 }).Contains(e.PropertyInt) == false);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "NOT ([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotArrayContainsEqualsFalse()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new int[] { 1, 2 }).Contains(e.PropertyInt) == false);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "NOT ([PropertyInt] NOT IN (@PropertyInt_In_0, @PropertyInt_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionArrayContainsFromVariable()
    {
        // Setup
        var list = new int[] { 1, 2 };
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.Contains(e.PropertyInt));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotArrayContainsFromVariable()
    {
        // Setup
        var list = new int[] { 1, 2 };
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !list.Contains(e.PropertyInt));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyInt] NOT IN (@PropertyInt_In_0, @PropertyInt_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionArrayContainsEqualsTrue()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new int[] { 1, 2 }).Contains(e.PropertyInt) == true);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionArrayContainsFromVariableEqualsTrue()
    {
        // Setup
        var list = new int[] { 1, 2 };
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.Contains(e.PropertyInt) == true);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotArrayContainsFromVariableEqualsTrue()
    {
        // Setup
        var list = new int[] { 1, 2 };
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !list.Contains(e.PropertyInt) == true);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyInt] NOT IN (@PropertyInt_In_0, @PropertyInt_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionArrayContainsFromVariableEqualsFalse()
    {
        // Setup
        var list = new int[] { 1, 2 };
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.Contains(e.PropertyInt) == false);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "NOT ([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotArrayContainsFromVariableEqualsFalse()
    {
        // Setup
        var list = new int[] { 1, 2 };
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !list.Contains(e.PropertyInt) == false);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "NOT ([PropertyInt] NOT IN (@PropertyInt_In_0, @PropertyInt_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionArrayContainsAtMappedProperty()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new string[] { "A", "B" }).Contains(e.MappedPropertyString));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] IN (@PropertyString_In_0, @PropertyString_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotArrayContainsAtMappedProperty()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new string[] { "A", "B" }).Contains(e.MappedPropertyString));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] NOT IN (@PropertyString_In_0, @PropertyString_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionArrayContainsAtMappedPropertyFromVariables()
    {
        // Setup
        var list = new string[] { "A", "B" };
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.Contains(e.MappedPropertyString));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] IN (@PropertyString_In_0, @PropertyString_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotArrayContainsAtMappedPropertyFromVariables()
    {
        // Setup
        var list = new string[] { "A", "B" };
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !list.Contains(e.MappedPropertyString));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] NOT IN (@PropertyString_In_0, @PropertyString_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionArrayContainsAtQuotedProperty()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new string[] { "A", "B" }).Contains(e.QuotedPropertyString));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] IN (@PropertyString_In_0, @PropertyString_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotArrayContainsAtQuotedProperty()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new string[] { "A", "B" }).Contains(e.QuotedPropertyString));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] NOT IN (@PropertyString_In_0, @PropertyString_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionArrayContainsAtQuotedPropertyFromVariables()
    {
        // Setup
        var list = new string[] { "A", "B" };
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.Contains(e.QuotedPropertyString));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] IN (@PropertyString_In_0, @PropertyString_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotArrayContainsAtQuotedPropertyFromVariables()
    {
        // Setup
        var list = new string[] { "A", "B" };
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.Contains(e.QuotedPropertyString));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] IN (@PropertyString_In_0, @PropertyString_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionArrayContainsAtUnorganizedProperty()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new string[] { "A", "B" }).Contains(e.UnorganizedPropertyString));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([Property / . String] IN (@Property_____String_In_0, @Property_____String_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotArrayContainsAtUnorganizedProperty()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new string[] { "A", "B" }).Contains(e.UnorganizedPropertyString));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([Property / . String] NOT IN (@Property_____String_In_0, @Property_____String_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionArrayContainsAtUnorganizedPropertyFromVariables()
    {
        // Setup
        var list = new string[] { "A", "B" };
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.Contains(e.UnorganizedPropertyString));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([Property / . String] IN (@Property_____String_In_0, @Property_____String_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotArrayContainsAtUnorganizedPropertyFromVariables()
    {
        // Setup
        var list = new string[] { "A", "B" };
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !list.Contains(e.UnorganizedPropertyString));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([Property / . String] NOT IN (@Property_____String_In_0, @Property_____String_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    #endregion

    #region List

    [TestMethod]
    public void TestQueryGroupParseExpressionListContains()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => new List<int>() { 1, 2 }.Contains(e.PropertyInt));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotListContains()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !new List<int>() { 1, 2 }.Contains(e.PropertyInt));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyInt] NOT IN (@PropertyInt_In_0, @PropertyInt_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotListContainsEqualsTrue()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !new List<int>() { 1, 2 }.Contains(e.PropertyInt) == true);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyInt] NOT IN (@PropertyInt_In_0, @PropertyInt_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotListContainsEqualsFalse()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !new List<int>() { 1, 2 }.Contains(e.PropertyInt) == false);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "NOT ([PropertyInt] NOT IN (@PropertyInt_In_0, @PropertyInt_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionListContainsFromVariable()
    {
        // Setup
        var list = new List<int>() { 1, 2 };
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.Contains(e.PropertyInt));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotListContainsFromVariable()
    {
        // Setup
        var list = new List<int>() { 1, 2 };
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !list.Contains(e.PropertyInt));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyInt] NOT IN (@PropertyInt_In_0, @PropertyInt_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionListContainsEqualsTrue()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => new List<int>() { 1, 2 }.Contains(e.PropertyInt) == true);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionListContainsEqualsFalse()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => new List<int>() { 1, 2 }.Contains(e.PropertyInt) == false);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "NOT ([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionListContainsFromVariableEqualsTrue()
    {
        // Setup
        var list = new List<int>() { 1, 2 };
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.Contains(e.PropertyInt) == true);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionListContainsFromVariableEqualsFalse()
    {
        // Setup
        var list = new List<int>() { 1, 2 };
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.Contains(e.PropertyInt) == false);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "NOT ([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionListContainsAtMappedProperty()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => new List<string>() { "A", "B" }.Contains(e.MappedPropertyString));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] IN (@PropertyString_In_0, @PropertyString_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionListContainsFromVariablesAtMappedProperty()
    {
        // Setup
        var list = new List<string>() { "A", "B" };
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.Contains(e.MappedPropertyString));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] IN (@PropertyString_In_0, @PropertyString_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionListContainsAtQuotedProperty()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => new List<string>() { "A", "B" }.Contains(e.QuotedPropertyString));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] IN (@PropertyString_In_0, @PropertyString_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionListContainsFromVariablesAtQuotedProperty()
    {
        // Setup
        var list = new List<string>() { "A", "B" };
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.Contains(e.QuotedPropertyString));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyString] IN (@PropertyString_In_0, @PropertyString_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionListContainsAtUnorganizedProperty()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => new List<string>() { "A", "B" }.Contains(e.UnorganizedPropertyString));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([Property / . String] IN (@Property_____String_In_0, @Property_____String_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionListContainsFromVariablesAtUnorganizedProperty()
    {
        // Setup
        var list = new List<string>() { "A", "B" };
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.Contains(e.UnorganizedPropertyString));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([Property / . String] IN (@Property_____String_In_0, @Property_____String_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    #endregion


    record OneR
    {
        public string? V { get; set; }
    }

    record Recursive
    {
        public string? Value { get; set; }

        public List<string> Tags { get; } = new();
        public Recursive? R { get; init; }

        public OneR One { get; } = new();
    }

    [TestMethod]
    public void TestQueryGroupParseRecursiveInit()
    {
        // This already used to Work
        QueryGroup.Parse<Recursive>(x => x.Value == new Recursive().Value);

        QueryGroup.Parse<Recursive>(x => x.Value == new Recursive() { Value = "A" }.Value);

        // But this situation didn't yet
        QueryGroup.Parse<Recursive>(x => x.Value == new Recursive() { Value = "A", Tags = { "a" } }.Value);


        QueryGroup.Parse<Recursive>(x => x.Value == new Recursive() { Value = "A", R = new() { Value = "B" } }.Value);


        QueryGroup.Parse<Recursive>(x => x.Value == new Recursive() { Value = "A", One = { V = "B" } }.Value);


        Expression<Func<Recursive, object?>> expr;
        object? ob;

        expr = x => new Recursive();
        Assert.IsTrue(expr.Body.TryGetValue(out ob));
        Assert.AreEqual(expr.Compile().Invoke(new())!.ToString(), ob?.ToString() ?? "null");

        expr = x => new Recursive() { Value = "A" };
        Assert.IsTrue(expr.Body.TryGetValue(out ob));
        Assert.AreEqual(expr.Compile().Invoke(new())!.ToString(), ob?.ToString() ?? "null");

        expr = x => new Recursive() { Value = "A", Tags = { "a" } };
        Assert.IsTrue(expr.Body.TryGetValue(out ob));
        Assert.AreEqual(expr.Compile().Invoke(new())!.ToString(), ob?.ToString() ?? "null");

        expr = x => new Recursive() { Value = "A", R = new() { Value = "B" } };
        Assert.IsTrue(expr.Body.TryGetValue(out ob));
        Assert.AreEqual(expr.Compile().Invoke(new())!.ToString(), ob?.ToString() ?? "null");

        expr = x => new Recursive() { Value = "A", One = { V = "B" } };
        Assert.IsTrue(expr.Body.TryGetValue(out ob));
        Assert.AreEqual(expr.Compile().Invoke(new())!.ToString(), ob?.ToString() ?? "null");


        expr = x => new List<string> { "A", "B", "C" };
        Assert.IsTrue(expr.Body.TryGetValue(out ob));
        Assert.AreEqual(expr.Compile().Invoke(new())!.ToString(), ob?.ToString() ?? "null");

        expr = x => new List<Recursive> { new(), new() };
        Assert.IsTrue(expr.Body.TryGetValue(out ob));
        Assert.AreEqual(expr.Compile().Invoke(new())!.ToString(), ob?.ToString() ?? "null");
    }

    record Rec(string A)
    {
        public string? B { get; init; }
    }

    static R ExprValue<R>(Expression<Func<R>> expr, [CallerArgumentExpression(nameof(expr))] string exprExpression = "")
    {
        Assert.IsTrue(expr.Body.TryGetValue(out var v), $"Expression {exprExpression}");
        return (R)v!;
    }

    static bool ExprResult<R>(Expression<Func<R>> expr, [CallerArgumentExpression(nameof(expr))] string exprExpression = "")
    {
        return expr.Body.TryGetValue(out var v);
    }

    static int OutFunc(out object? v) { v = "set"; return 1; }


    [TestMethod]
    public void ValueTests()
    {
        // Roslyn optimizes these values away if we put them in directly as constants. We don't see the full expression then
        bool True = true;
        bool False = false;

        bool? BoolNull = null;
        bool? TrueNull = true;

        Assert.AreEqual(true, ExprValue(() => !False));
        Assert.AreEqual(true, ExprValue(() => True || False));
        Assert.AreEqual(true, ExprValue(() => True || False || True));
        Assert.AreEqual(true, ExprValue(() => True == !False));
        Assert.AreEqual(true, ExprValue(() => True != False));
        Assert.AreEqual(true, ExprValue(() => True && !False));

        Assert.AreEqual(true, ExprValue(() => BoolNull ?? True));

        Assert.AreEqual(true, ExprValue(() => BoolNull == null));
        Assert.AreEqual(true, ExprValue(() => BoolNull.HasValue == false));
        Assert.AreEqual(true, ExprValue(() => TrueNull.Value != false));
        Assert.AreEqual(true, ExprValue(() => ((bool?)True ?? BoolNull)! == true));

        Assert.AreEqual(true, ExprValue(() => new { a = 12 }.a == 12));
        Assert.AreEqual(true, ExprValue(() => TrueNull.HasValue));
        Assert.AreEqual(false, ExprValue(() => True ^ True));
        Assert.AreEqual(true, ExprValue(() => True | True));
        Assert.AreEqual(true, ExprValue(() => True & True));

        var five = 5;
        var list = new List<int> { 1, 2, 3 };
        Assert.AreEqual(~5, ExprValue(() => ~five));
        Assert.AreEqual(2, ExprValue(() => list[1]));
        Assert.AreEqual(3, ExprValue(() => list.ToArray()[2]));
        Assert.AreEqual(5d, ExprValue(() => (double)five));

        Assert.AreEqual(true, ExprValue(() => (bool)True));
        Assert.AreEqual(true, ExprValue(() => (bool)(object)True));
        Assert.AreEqual(5, ExprValue(() => +five)); // Unary plus is a no-op, but we want to make sure it doesn't break the value retrieval
        Assert.AreEqual(-5m, ExprValue(() => -(decimal)five));

        // Casts, conversions, operators
        Assert.AreEqual(2, ExprValue(() => (five + five) / five));
        Assert.AreEqual("AB5", ExprValue(() => "A" + $"B{five}"));
        Assert.AreEqual("AB", ExprValue(() => "A" + 'B'));

        Assert.AreEqual("AA", ExprValue(() => new Rec("C") { B = "AA" }.B));

        // Strange init constructs
        Assert.IsNull(ExprValue(() => new Recursive().Value));
        Assert.AreEqual("A", ExprValue(() => new Recursive() { Value = "A" }.Value));
        Assert.AreEqual("A", ExprValue(() => new Recursive() { Value = "A", Tags = { "a" } }.Value));
        Assert.HasCount(1, ExprValue(() => new Recursive() { Value = "A", Tags = { "a" } }.Tags));
        Assert.AreEqual("A", ExprValue(() => new Recursive() { Value = "A", R = new() { Value = "B" } }.Value));
        Assert.AreEqual("A", ExprValue(() => new Recursive() { Value = "A", One = { V = "B" } }.Value));

        // Lambda argument (quote expression)
        Assert.AreEqual("word", ExprValue(() => ExprValue(() => "word")));

        // And the absolute worst case: output arguments
        object? qq = null;
        Assert.AreEqual(1, ExprValue(() => OutFunc(out qq)));
        Assert.AreEqual("set", qq);

        // Thus query works when casted
        Assert.AreEqual(true, ExprValue(() => ((IEnumerable<int>)new int[] { 1, 5 }).Contains(five) == true));
#if NET10_0_OR_GREATER
        // Handled as span, so we get byRef values, etc.
        Assert.IsFalse(ExprResult(() => (new int[] { 1, 5 }).Contains(five) == true));
#else
        // Handled as linq, so we get the full expression and can evaluate it
        Assert.IsFalse(ExprResult(() => (new int[] { 1, 5 }).Contains(five) == true));
#endif
    }


    [TestMethod]
    public void TestQueryGroupParseExpressionBooleans()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyBoolean);
        Assert.AreEqual("([PropertyBoolean] = @PropertyBoolean)", parsed.GetString(m_dbSetting));

        parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyBoolean);
        Assert.AreEqual("([PropertyBoolean] <> @PropertyBoolean)", parsed.GetString(m_dbSetting));

        // Not is in many cases handled as <>
        parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyBoolean && e.PropertyBoolean);
        Assert.AreEqual("([PropertyBoolean] <> @PropertyBoolean AND [PropertyBoolean] = @PropertyBoolean_1)", parsed.GetString(m_dbSetting));

        parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyBoolean && e.PropertyBoolean || e.PropertyBoolean);
        Assert.AreEqual("(([PropertyBoolean] <> @PropertyBoolean AND [PropertyBoolean] = @PropertyBoolean_1) OR ([PropertyBoolean] = @PropertyBoolean_2))", parsed.GetString(m_dbSetting));

        parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (!e.PropertyBoolean && e.PropertyBoolean) || e.PropertyBoolean);
        Assert.AreEqual("(([PropertyBoolean] <> @PropertyBoolean AND [PropertyBoolean] = @PropertyBoolean_1) OR ([PropertyBoolean] = @PropertyBoolean_2))", parsed.GetString(m_dbSetting));

        parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (!e.PropertyBoolean && e.PropertyBoolean) || !e.PropertyBoolean);
        Assert.AreEqual("(([PropertyBoolean] <> @PropertyBoolean AND [PropertyBoolean] = @PropertyBoolean_1) OR ([PropertyBoolean] <> @PropertyBoolean_2))", parsed.GetString(m_dbSetting));

        parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(e.PropertyInt == 12) || !e.PropertyBoolean);
        Assert.AreEqual("(NOT (([PropertyInt] = @PropertyInt)) OR ([PropertyBoolean] <> @PropertyBoolean))", parsed.GetString(m_dbSetting));
    }

    record ValueItem(int V);

    [TestMethod]
    public void TestOutOfOrder()
    {
        // With the newer TryGet helpers we can also parse queries that are not strictly db-left, value-right
        var parsed = QueryGroup.Parse<ValueItem>(e => e.V == 5 || 6 == e.V);
        Assert.AreEqual("([V] = @V OR [V] = @V_1)", parsed.GetString(m_dbSetting));

        parsed = QueryGroup.Parse<ValueItem>(e => e.V < 5 || 6 < e.V);
        Assert.AreEqual("([V] < @V OR [V] > @V_1)", parsed.GetString(m_dbSetting));

        parsed = QueryGroup.Parse<ValueItem>(e => e.V > 22 && 44 > e.V);
        Assert.AreEqual("([V] > @V AND [V] < @V_1)", parsed.GetString(m_dbSetting));

        parsed = QueryGroup.Parse<ValueItem>(e => !(e.V == 5 || 6 == e.V));
        Assert.AreEqual("NOT (([V] = @V OR [V] = @V_1))", parsed.GetString(m_dbSetting));

        parsed = QueryGroup.Parse<ValueItem>(e => !(e.V >= 5) || !(6 <= e.V));
        Assert.AreEqual("(NOT (([V] >= @V)) OR NOT (([V] >= @V_1)))", parsed.GetString(m_dbSetting));
    }

    record ValueJsonItem(JsonObject ob, DbJsonValue<ValueItem> vv);


    [TestMethod]
    public void TestJsonParseExpression()
    {
        var parsed = QueryGroup.Parse<ValueJsonItem>(e => e.vv.Value.V == 5);
        Assert.AreEqual("(JSON_EXTRACT([vv], '$.V') = @vv)", parsed.GetString(m_dbSetting));

        parsed = QueryGroup.Parse<ValueJsonItem>(e => e.vv.Value.V < 3);
        Assert.AreEqual("(JSON_EXTRACT([vv], '$.V') < @vv)", parsed.GetString(m_dbSetting));

        parsed = QueryGroup.Parse<ValueJsonItem>(e => e.ob.ExtractValue((ValueItem x) => x.V) < 3);
        Assert.AreEqual("(JSON_EXTRACT([ob], '$.V') < @ob)", parsed.GetString(m_dbSetting));

        parsed = QueryGroup.Parse<ValueJsonItem>(e => e.ob.ExtractValue<int>("V") < 3);
        Assert.AreEqual("(JSON_EXTRACT([ob], '$.V') < @ob)", parsed.GetString(m_dbSetting));

        //parsed = QueryGroup.Parse<ValueJsonItem>(e => e.vv.Json.ExtractValue((ValueItem x) => x.V) < 3);
        //Assert.AreEqual("(JSON_EXTRACT([vv], '$.V') < @vv)", parsed.GetString(m_dbSetting));


        ValueJsonItem item = new(new JsonObject() { ["V"] = 5 }, new DbJsonValue<ValueItem>(new ValueItem(5)));

        // And assert some value retrievals work as expected when not processed on server
        Assert.AreEqual(5, ExprValue(() => item.vv.Value.V));
        Assert.AreEqual(5, ExprValue(() => item.ob.ExtractValue((ValueItem x) => x.V)));
        Assert.AreEqual(5, ExprValue(() => item.ob.ExtractValue<int>("V")));
    }
}
