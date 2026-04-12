using RepoDb.Extensions;

namespace RepoDb.UnitTests;

[TestClass]
public class ListExtensionTest
{
    #region AddIfNotNull

    [TestMethod]
    public void TestAddIfNotNullWithNonNullItem()
    {
        // Arrange
        var list = new List<string>();
        var item = "test";

        // Act
        list.AddIfNotNull(item);

        // Assert
        Assert.AreEqual(1, list.Count);
        Assert.AreEqual(item, list[0]);
    }

    [TestMethod]
    public void TestAddIfNotNullWithNullItem()
    {
        // Arrange
        var list = new List<string?>();
        string? item = null;

        // Act
        list.AddIfNotNull(item);

        // Assert
        Assert.AreEqual(0, list.Count);
    }

    [TestMethod]
    public void TestAddIfNotNullMultipleCalls()
    {
        // Arrange
        var list = new List<string?>();

        // Act
        list.AddIfNotNull("item1");
        list.AddIfNotNull(null);
        list.AddIfNotNull("item2");
        list.AddIfNotNull(null);

        // Assert
        Assert.AreEqual(2, list.Count);
        Assert.AreEqual("item1", list[0]);
        Assert.AreEqual("item2", list[1]);
    }

    [TestMethod]
    public void TestAddIfNotNullWithNullList()
    {
        // Arrange & Act & Assert
        try
        {
            List<string>? list = null;
            list!.AddIfNotNull("item");
            Assert.Fail("Expected ArgumentNullException");
        }
        catch (ArgumentNullException)
        {
            // Expected
        }
    }

    #endregion

    #region AddRangeIfNotNullOrNotEmpty

    [TestMethod]
    public void TestAddRangeIfNotNullOrNotEmptyWithItems()
    {
        // Arrange
        var list = new List<int>();
        var items = new[] { 1, 2, 3 };

        // Act
        list.AddRangeIfNotNullOrNotEmpty(items);

        // Assert
        Assert.AreEqual(3, list.Count);
        Assert.AreEqual(1, list[0]);
        Assert.AreEqual(2, list[1]);
        Assert.AreEqual(3, list[2]);
    }

    [TestMethod]
    public void TestAddRangeIfNotNullOrNotEmptyWithNullItems()
    {
        // Arrange
        var list = new List<int>();
        IEnumerable<int>? items = null;

        // Act
        list.AddRangeIfNotNullOrNotEmpty(items);

        // Assert
        Assert.AreEqual(0, list.Count);
    }

    [TestMethod]
    public void TestAddRangeIfNotNullOrNotEmptyWithEmptyItems()
    {
        // Arrange
        var list = new List<int>();
        var items = Enumerable.Empty<int>();

        // Act
        list.AddRangeIfNotNullOrNotEmpty(items);

        // Assert
        Assert.AreEqual(0, list.Count);
    }

    [TestMethod]
    public void TestAddRangeIfNotNullOrNotEmptyMultipleCalls()
    {
        // Arrange
        var list = new List<int>();

        // Act
        list.AddRangeIfNotNullOrNotEmpty(new[] { 1, 2 });
        list.AddRangeIfNotNullOrNotEmpty(null);
        list.AddRangeIfNotNullOrNotEmpty(new[] { 3, 4 });
        list.AddRangeIfNotNullOrNotEmpty(Enumerable.Empty<int>());

        // Assert
        Assert.AreEqual(4, list.Count);
        Assert.AreEqual(1, list[0]);
        Assert.AreEqual(2, list[1]);
        Assert.AreEqual(3, list[2]);
        Assert.AreEqual(4, list[3]);
    }

    [TestMethod]
    public void TestAddRangeIfNotNullOrNotEmptyWithNullList()
    {
        // Arrange & Act & Assert
        try
        {
            List<int>? list = null;
            list!.AddRangeIfNotNullOrNotEmpty(new[] { 1, 2, 3 });
            Assert.Fail("Expected ArgumentNullException");
        }
        catch (ArgumentNullException)
        {
            // Expected
        }
    }

    #endregion
}
