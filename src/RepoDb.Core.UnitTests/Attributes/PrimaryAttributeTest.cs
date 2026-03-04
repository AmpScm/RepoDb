using RepoDb.Attributes;

namespace RepoDb.UnitTests.Attributes;

[TestClass]
public class PrimaryAttributeTest
{
    private class PrimaryAttributeTestClass
    {
        [Primary]
        public int WhateverId { get; set; }
        public string Name { get; set; }
    }

    [TestMethod]
    public void TestPrimaryAttribute()
    {
        // Act
#pragma warning disable CS0618 // Type or member is obsolete
        var actual = PrimaryCache.Get<PrimaryAttributeTestClass>();
#pragma warning restore CS0618 // Type or member is obsolete
        var expected = "WhateverId";

        // Assert
        Assert.AreEqual(expected, actual.PropertyInfo.Name);
    }
}
