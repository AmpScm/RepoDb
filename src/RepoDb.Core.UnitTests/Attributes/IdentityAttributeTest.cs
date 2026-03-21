using RepoDb.Attributes;
using RepoDb.Resolvers;

namespace RepoDb.UnitTests.Attributes;

[TestClass]
public class IdentityAttributeTest
{
    private class IdentityAttributeTestClass
    {
        [Identity]
        public int WhateverId { get; set; }
        public string Name { get; set; }
    }

    [TestMethod]
    public void TestIdentityAttribute()
    {
        // Act
        var result = IdentityResolver.Instance.Resolve(typeof(IdentityAttributeTestClass));
        var actual = IdentityCache.Get<IdentityAttributeTestClass>();
        
        // Assert
        Assert.AreEqual(nameof(IdentityAttributeTestClass.WhateverId), result?.PropertyName);
        Assert.AreEqual(nameof(IdentityAttributeTestClass.WhateverId), actual.PropertyInfo.Name);
    }
}
