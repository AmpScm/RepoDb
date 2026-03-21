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

    private class MultiPrimaryAttributeTestClass
    {
        [Primary]
        public int WhateverId { get; set; }
        [Primary]
        public int OtherId { get; set; }
        public string Name { get; set; }
    }

    [TestMethod]
    public void TestPrimaryAttribute()
    {
        // Act
#pragma warning disable CS0618 // Type or member is obsolete
        var actual = PrimaryCache.Get<PrimaryAttributeTestClass>();
#pragma warning restore CS0618 // Type or member is obsolete

        // Assert
        Assert.AreEqual(nameof(PrimaryAttributeTestClass.WhateverId), actual.PropertyInfo.Name);
    }

    [TestMethod]
    public void TestPrimaryAttributeMulti()
    {
        // Act
        var actual = PrimaryCache.GetPrimaryKeys<MultiPrimaryAttributeTestClass>().OrderBy(x => x.FieldName);

        // Assert
        Assert.AreEqual($"{nameof(MultiPrimaryAttributeTestClass.OtherId)},{nameof(MultiPrimaryAttributeTestClass.WhateverId)}", string.Join(",", actual.Select(x => x.FieldName)));
    }
}
