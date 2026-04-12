using RepoDb.Enumerations;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.QueryGroups;

/// <summary>
/// Tests for advanced QueryGroup folding optimizations.
/// Covers isNot parameter propagation, universal properties detection,
/// and pattern recognition across all constructor overloads.
/// </summary>
[TestClass]
public class QueryGroupFoldingTest
{
    private static readonly IDbSetting m_dbSetting = new CustomDbSetting();

    public required TestContext TestContext { get; init; }

    #region IsNot Folding - Single Parameter

    [TestMethod]
    public void TestFold_IsNot_QueryFieldWithIsNotTrue()
    {
        // Arrange
        var field = new QueryField("Id", 1);

        // Act
        var group = new QueryGroup(field, isNot: true);

        // Assert
        Assert.AreEqual("([Id] <> @Id)", group.GetString(m_dbSetting));
        Assert.AreEqual(Conjunction.And, group.Conjunction);
        Assert.HasCount(1, group.QueryFields);
    }

    [TestMethod]
    public void TestFold_IsNot_QueryFieldWithIsNotFalse()
    {
        // Arrange
        var field = new QueryField("Id", 1);

        // Act
        var group = new QueryGroup(field, isNot: false);

        // Assert
        Assert.AreEqual(false, group.IsNot);
    }

    #endregion

    #region IsNot Folding - Child Groups

    [TestMethod]
    public void TestFold_IsNot_QueryGroupWithIsNotTrue()
    {
        // Arrange
        var field = new QueryField("Id", 1);
        var child = new QueryGroup(field);

        // Act
        var parent = new QueryGroup(child, isNot: true);

        // Assert
        Assert.AreEqual("([Id] <> @Id)", parent.GetString(m_dbSetting));
    }

    #endregion

    #region SQL Generation with Advanced Folding

    [TestMethod]
    public void TestSql_IsNotFold_SingleField()
    {
        // Arrange
        var field = new QueryField("Id", 1);
        var group = new QueryGroup(field, isNot: true);

        // Act
        var sql = group.GetString(m_dbSetting);

        // Assert
        Assert.IsTrue(sql.Contains("<>"));
        Assert.IsTrue(sql.Contains("[Id]"));
    }


    [TestMethod]
    public void TestSql_IsNotFold_WithChild()
    {
        // Arrange
        var field = new QueryField("Id", 1);
        var child = new QueryGroup(field);
        var parent = new QueryGroup(child, isNot: true);

        // Act
        var sql = parent.GetString(m_dbSetting);

        // Assert
        Assert.AreEqual("([Id] <> @Id)", parent.GetString(m_dbSetting));
    }

    [TestMethod]
    public void TestSql_HomogeneousIsNot_AllTrue()
    {
        // Arrange
        var field1 = new QueryField("Status", "Active");
        var field2 = new QueryField("Type", "Active");
        var child1 = new QueryGroup(field1, isNot: true);
        var child2 = new QueryGroup(field2, isNot: true);
        var parent = new QueryGroup(new[] { child1, child2 }, Conjunction.And);

        // Act
        var sql = parent.GetString(m_dbSetting);
        Assert.AreEqual("([Status] <> @Status AND [Type] <> @Type)", sql);
    }

    #endregion

    #region Constructor Overload Coverage

    [TestMethod]
    public void TestConstructor_QueryFields_Conjunction_IsNot()
    {
        var fields = new[] { new QueryField("Id", 1) };
        var group = new QueryGroup(fields, Conjunction.Or, isNot: true);
        Assert.AreEqual(false, group.IsNot);
        Assert.AreEqual("([Id] <> @Id)", group.GetString(m_dbSetting));
    }

    #endregion

    #region Edge Cases

    [TestMethod]
    public void TestEdgeCase_IsNot_WithNullableIntValue()
    {
        // Arrange
        var field = new QueryField("Count", Operation.Equal, (int?)null);

        // Act
        var group = new QueryGroup(field, isNot: true);

        // Assert
        Assert.AreEqual("([Count] IS NOT NULL)", group.GetString(m_dbSetting));
    }

    [TestMethod]
    public void TestEdgeCase_UniversalOperation_SingleField()
    {
        // Arrange
        var field = new QueryField("Id", Operation.Equal, 1);

        // Act
        var group = new QueryGroup(field);

        // Assert
        // Single field - universal operation doesn't apply but shouldn't fail
        Assert.AreEqual(1, group.QueryFields?.Count);
    }

    #endregion

    [TestMethod]
    public void QueryGroup_Should_Flatten_Field_And_Group_With_Same_Conjunction()
    {
        // Arrange
        var inner = new QueryGroup(
            queryFields: [
            new QueryField("B", Operation.Equal, 1),
            new QueryField("C", Operation.Equal, 2)
            ],
            queryGroups: null,
            conjunction: Conjunction.And,
            isNot: false);

        var outer = new QueryGroup(
            queryFields: [
                new QueryField("A", Operation.Equal, 0)
            ],
            queryGroups: [ inner ],
            conjunction: Conjunction.And,
            isNot: false);


        TestContext.WriteLine(outer.GetString(m_dbSetting));
        // Act

        var fields = outer.QueryFields;
        var groups = outer.QueryGroups;


        Assert.AreEqual(3, fields?.Count, "Outer group still contains only A");
        Assert.AreEqual(null, groups?.Count, "Inner group was not flattened");

        Assert.AreEqual("([A] = @A AND [B] = @B AND [C] = @C)", outer.GetString(m_dbSetting));
    }
}
