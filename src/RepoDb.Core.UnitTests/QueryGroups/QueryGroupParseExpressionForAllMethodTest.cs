using System.Linq.Expressions;
using RepoDb.DbSettings;
using RepoDb.Enumerations;
using RepoDb.Interfaces;

namespace RepoDb.UnitTests;

public partial class QueryGroupTest
{
    // All

    [TestMethod]
    public void TestQueryGroupParseExpressionAll()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { 1, 2 }).All(p => p == e.PropertyInt));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotAll()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { 1, 2 }).All(p => p != e.PropertyInt));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyInt] NOT IN (@PropertyInt_In_0, @PropertyInt_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionAllFromVariable()
    {
        // Setup
        var list = new int[] { 1, 2 };
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.All(p => p == e.PropertyInt));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotAllFromVariable()
    {
        // Setup
        var list = new int[] { 1, 2 };
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.All(p => p != e.PropertyInt));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyInt] NOT IN (@PropertyInt_In_0, @PropertyInt_In_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionAllFromClassProperty()
    {
        // Setup
        var @class = new QueryGroupTestExpressionClass
        {
            PropertyInt = 500
        };
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { @class.PropertyInt, @class.PropertyInt }).All(p => p == e.PropertyInt));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotAllFromClassProperty()
    {
        // Setup
        var @class = new QueryGroupTestExpressionClass
        {
            PropertyInt = 500
        };
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new[] { @class.PropertyInt, @class.PropertyInt }).All(p => p == e.PropertyInt));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyInt] <> @PropertyInt OR [PropertyInt] <> @PropertyInt_1)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionAllEqualsFalseFromClassProperty()
    {
        // Setup
        var @class = new QueryGroupTestExpressionClass
        {
            PropertyInt = 500
        };
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { @class.PropertyInt, @class.PropertyInt }).All(p => p == e.PropertyInt) == false);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyInt] <> @PropertyInt OR [PropertyInt] <> @PropertyInt_1)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionAllEqualsTrueFromClassProperty()
    {
        // Setup
        var @class = new QueryGroupTestExpressionClass
        {
            PropertyInt = 500
        };
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { @class.PropertyInt, @class.PropertyInt }).All(p => p == e.PropertyInt) == true);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionAllFromClassMethod()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { GetIntValueForParseExpression(), GetIntValueForParseExpression() }).All(p => p == e.PropertyInt));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotAllFromClassMethod()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new[] { GetIntValueForParseExpression(), GetIntValueForParseExpression() }).All(p => p == e.PropertyInt));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyInt] <> @PropertyInt OR [PropertyInt] <> @PropertyInt_1)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionAllEqualsFalseFromClassMethod()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { GetIntValueForParseExpression(), GetIntValueForParseExpression() }).All(p => p == e.PropertyInt) == false);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyInt] <> @PropertyInt OR [PropertyInt] <> @PropertyInt_1)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionAllEqualsTrueFromClassMethod()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { GetIntValueForParseExpression(), GetIntValueForParseExpression() }).All(p => p == e.PropertyInt));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotAllEqualsFalseFromClassMethod()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new[] { GetIntValueForParseExpression(), GetIntValueForParseExpression() }).All(p => p == e.PropertyInt) == false);

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionNotAllEqualsTrueFromClassMethod()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new[] { GetIntValueForParseExpression(), GetIntValueForParseExpression() }).All(p => p == e.PropertyInt));

        // Act
        var actual = parsed.GetString(m_dbSetting);
        var expected = "([PropertyInt] <> @PropertyInt OR [PropertyInt] <> @PropertyInt_1)";

        // Assert
        Assert.AreEqual(expected, actual);
    }


    static Expression<Func<QueryGroupTestExpressionClass, bool>> Parse(Expression<Func<QueryGroupTestExpressionClass, bool>> expression) => expression;
    public static IEnumerable<object[]> Cases =>
    [
        // NOT-NOT
        [
            Parse(e => !( !(e.PropertyInt == 1) )),
            "([PropertyInt] = @PropertyInt)"
        ],

        // De Morgan: NOT(A OR B) → NOT A AND NOT B
        [
            Parse(e => !(e.PropertyInt != 1 || e.PropertyInt != 2)),
            "([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)"
        ],

        // Single field
        [
            Parse(e => (e.PropertyInt == 1)),
            "([PropertyInt] = @PropertyInt)"
        ],

        // AND flattening
        [
            Parse(e => (e.PropertyInt == 1 && e.PropertyInt == 2) && e.PropertyInt == 3),
            "([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1 AND [PropertyInt] = @PropertyInt_2)"
        ],

        // OR flattening
        [
            Parse(e => (e.PropertyInt == 1 || e.PropertyInt == 2) || e.PropertyInt == 3),
            "([PropertyInt] = @PropertyInt OR [PropertyInt] = @PropertyInt_1 OR [PropertyInt] = @PropertyInt_2)"
        ],

        // NOT field
        [
            Parse(e => !(e.PropertyInt == 1)),
            "([PropertyInt] <> @PropertyInt)"
        ],

        // Mixed NOT
        [
            Parse(e => !( (e.PropertyInt == 1) && !(e.PropertyInt == 2) )),
            "([PropertyInt] <> @PropertyInt OR [PropertyInt] = @PropertyInt_1)"
        ],

        // All-equivalent → AND
        [
            Parse(e => (e.PropertyInt == 1 && e.PropertyInt == 2)),
            "([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)"
        ],

        // Any-equivalent → OR
        [
            Parse(e => (e.PropertyInt == 1 || e.PropertyInt == 2)),
            "([PropertyInt] = @PropertyInt OR [PropertyInt] = @PropertyInt_1)"
        ],

        // !(All-equivalent) == false → All-equivalent
        [
            Parse(e => !( (e.PropertyInt == 1 && e.PropertyInt == 2) ) == false),
            "([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)"
        ]
    ];


    [TestMethod]
    [DynamicData(nameof(Cases))]
    [DoNotParallelize]
    public void TestNormalization(Expression<Func<QueryGroupTestExpressionClass, bool>> expression, string expectedSql)
    {
        GlobalConfiguration.Setup(GlobalConfiguration.Options with { QueryGroupOptimization = QueryGroupOptimization.None });

        var noOptimizeParsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(expression).GetString(m_dbSetting);

        GlobalConfiguration.Setup(GlobalConfiguration.Options with { QueryGroupOptimization = QueryGroupOptimization.Full });

        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(expression);
        var parsedString = parsed.GetString(m_dbSetting);
        //Assert.AreNotEqual(noOptimizeParsed, parsed);
        Assert.AreEqual(expectedSql, parsedString);
    }
}
