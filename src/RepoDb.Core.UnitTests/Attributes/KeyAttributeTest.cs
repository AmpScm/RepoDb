using System.ComponentModel.DataAnnotations;
using RepoDb.Attributes;

namespace RepoDb.UnitTests.Attributes;

[TestClass]
public class KeyAttributeTest
{
    #region SubClasses

    private class KeyAttributeTestClass
    {
        [Key]
        public int WhateverId { get; set; }
        public string Name { get; set; }
    }

    private class KeyAttributeCollisionTestClass
    {
        [Key]
        public int KeyId { get; set; }
        [Primary]
        public int PrimaryId { get; set; }
    }

    #endregion

    [TestMethod]
    public void TestKeyAttribute()
    {
        // Act
#pragma warning disable CS0618 // Type or member is obsolete
        var actual = PrimaryCache.Get<KeyAttributeTestClass>();
#pragma warning restore CS0618 // Type or member is obsolete
        var expected = "WhateverId";

        // Assert
        Assert.AreEqual(expected, actual.PropertyInfo.Name);
    }

    [TestMethod]
    public void TestKeyAndPrimaryAttributeCollision()
    {
        // Act
#pragma warning disable CS0618 // Type or member is obsolete
        var actual = PrimaryCache.Get<KeyAttributeCollisionTestClass>();
#pragma warning restore CS0618 // Type or member is obsolete
        var expected = "KeyId";

        // Assert
        Assert.AreEqual(expected, actual.PropertyInfo.Name);
    }
}
