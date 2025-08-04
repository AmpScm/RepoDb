namespace RepoDb.UnitTests;

public partial class QueryGroupTest
{
    [TestMethod]
    public void TestQueryGroupParseDynamicValueForNullField()
    {
        // Setup
        var parsed = QueryGroup.Parse(new { Field1 = (object)null });

        // Act
        var actual = parsed.QueryFields[0].Parameter.Value;

        // Assert
        Assert.AreEqual((object?)null, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseDynamicValueForSingleField()
    {
        // Setup
        var parsed = QueryGroup.Parse(new { Field1 = 1 });

        // Act
        var actual = parsed.QueryFields[0].Parameter.Value;

        // Assert
        Assert.AreEqual(1, actual);
    }

    [TestMethod]
    public void TestQueryGroupParseDynamicValueForMultipleFields()
    {
        // Setup
        var parsed = QueryGroup.Parse(new { Field1 = 1, Field2 = 2 });

        // Act
        var actual1 = parsed.QueryFields[0].Parameter.Value;
        var actual2 = parsed.QueryFields[parsed.QueryFields.Count - 1].Parameter.Value;

        // Assert
        Assert.AreEqual(1, actual1);
        Assert.AreEqual(2, actual2);
    }

    [TestMethod]
    public void TestQueryGroupParseDynamicValueForEnums()
    {
        // Setup
        var parsed = QueryGroup.Parse(new { Field1 = Direction.West, Field2 = Direction.East });

        // Act
        var actual1 = parsed.QueryFields[0].Parameter.Value;
        var actual2 = parsed.QueryFields[parsed.QueryFields.Count - 1].Parameter.Value;

        // Assert
        Assert.AreEqual(Direction.West, actual1);
        Assert.AreEqual(Direction.East, actual2);
    }
}
