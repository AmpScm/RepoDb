namespace RepoDb.UnitTests;

public partial class QueryGroupTest
{
    [TestInitialize]
    public void Init()
    {
        GlobalConfiguration.Setup(new());
    }

    #region Name

    [TestMethod]
    public void TestQueryGroupParseExpressionWithNameAtLeft()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == 1);

        // Act
        var actual = parsed.QueryFields[0].Field.FieldName;
        var expected = "PropertyInt";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionWhereNameHasMapping()
    {
        // Setup
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.MappedPropertyString == "ABC");

        // Act
        var actual = parsed.QueryFields[0].Field.FieldName;
        var expected = "PropertyString";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ParseExpressionWithNameAtRight()
    {
        // Used to fail during parsing, but with better parser is now properly supported
        var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => 1 == e.PropertyInt);

        Assert.AreEqual("([PropertyInt] = @PropertyInt)", parsed.GetString(m_dbSetting));
    }

    #endregion

    #region Properties

    [TestMethod]
    public void TestQueryGroupParseExpressionWithDoubleSameFieldsForAnd()
    {
        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == 1 && e.PropertyInt == 2).GetString(m_dbSetting);
        var expected = "(([PropertyInt] = @PropertyInt) AND ([PropertyInt] = @PropertyInt_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DoNotParallelize]
    public void TestQueryGroupParseExpressionWithDoubleSameFieldsForAnd2()
    {
        GlobalConfiguration.Setup(new() { ExpressionNullSemantics = RepoDb.Enumerations.ExpressionNullSemantics.NullNotEqual });
        try
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == 1 && e.PropertyInt == 2).GetString(m_dbSetting);
            var expected = "(([PropertyInt] = @PropertyInt AND [PropertyInt] IS NOT NULL) AND ([PropertyInt] = @PropertyInt_2 AND [PropertyInt] IS NOT NULL))";

            // Assert
            Assert.AreEqual(expected, actual);
        }
        finally
        {
            GlobalConfiguration.Setup(new());
        }
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionWithDoubleSameFieldsForOr()
    {
        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == 1 || e.PropertyInt == 2).GetString(m_dbSetting);
        var expected = "(([PropertyInt] = @PropertyInt) OR ([PropertyInt] = @PropertyInt_1))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DoNotParallelize]
    public void TestQueryGroupParseExpressionWithDoubleSameFieldsForOr2()
    {
        GlobalConfiguration.Setup(new() { ExpressionNullSemantics = RepoDb.Enumerations.ExpressionNullSemantics.NullNotEqual });
        try
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == 1 || e.PropertyInt == 2).GetString(m_dbSetting);
            var expected = "(([PropertyInt] = @PropertyInt AND [PropertyInt] IS NOT NULL) OR ([PropertyInt] = @PropertyInt_2 AND [PropertyInt] IS NOT NULL))";

            // Assert
            Assert.AreEqual(expected, actual);
        }
        finally
        {
            GlobalConfiguration.Setup(new());
        }
    }


    #endregion

    #region Groupings

    [TestMethod]
    public void TestQueryGroupParseExpressionWithSingleGroupForAnd()
    {
        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == 1 && e.PropertyString == "A").GetString(m_dbSetting);
        var expected = "(([PropertyInt] = @PropertyInt) AND ([PropertyString] = @PropertyString))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionWithSingleGroupForOr()
    {
        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == 1 || e.PropertyString == "A").GetString(m_dbSetting);
        var expected = "(([PropertyInt] = @PropertyInt) OR ([PropertyString] = @PropertyString))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionWithSingleGroupForOrAndSingleFieldForAnd()
    {
        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (e.PropertyInt == 1 || e.PropertyDouble == 2) && e.PropertyString == "A").GetString(m_dbSetting);
        var expected = "((([PropertyInt] = @PropertyInt) OR ([PropertyDouble] = @PropertyDouble)) AND ([PropertyString] = @PropertyString))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionWithSingleGroupForAndAndSingleFieldForOr()
    {
        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (e.PropertyInt == 1 && e.PropertyDouble == 2) || e.PropertyString == "A").GetString(m_dbSetting);
        var expected = "((([PropertyInt] = @PropertyInt) AND ([PropertyDouble] = @PropertyDouble)) OR ([PropertyString] = @PropertyString))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionWithDoubleGroupForAnd()
    {
        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (e.PropertyInt == 1 && e.PropertyDouble == 2) && (e.PropertyString == "A" && e.PropertySingle == 1)).GetString(m_dbSetting);
        var expected = "((([PropertyInt] = @PropertyInt) AND ([PropertyDouble] = @PropertyDouble)) AND (([PropertyString] = @PropertyString) AND ([PropertySingle] = @PropertySingle)))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionWithDoubleGroupForOr()
    {
        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (e.PropertyInt == 1 || e.PropertyDouble == 2) || (e.PropertyString == "A" || e.PropertySingle == 1)).GetString(m_dbSetting);
        var expected = "((([PropertyInt] = @PropertyInt) OR ([PropertyDouble] = @PropertyDouble)) OR (([PropertyString] = @PropertyString) OR ([PropertySingle] = @PropertySingle)))";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    #endregion

    #region Mapped

    [TestMethod]
    public void TestQueryGroupParseExpressionForMappedProperty()
    {
        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.MappedPropertyString == "A").GetString(m_dbSetting);
        var expected = "([PropertyString] = @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    #endregion

    #region Quoted

    [TestMethod]
    public void TestQueryGroupParseExpressionForQuotedProperty()
    {
        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.QuotedPropertyString == "A").GetString(m_dbSetting);
        var expected = "([PropertyString] = @PropertyString)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    #endregion

    #region Unorganized

    [TestMethod]
    public void TestQueryGroupParseExpressionForUnorganizedProperty()
    {
        // Act
        var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.UnorganizedPropertyString == "A").GetString(m_dbSetting);
        var expected = "([Property / . String] = @Property_____String)";

        // Assert
        Assert.AreEqual(expected, actual);
    }

    #endregion
}
