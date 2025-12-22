using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes.Parameter;
using RepoDb.Extensions;
using System.Data;

namespace RepoDb.UnitTests.Extensions;

[TestClass]
[DoNotParallelize]
public class PropertyExtensionTest
{
    [TestCleanup]
    public void Cleanup()
    {
        PropertyValueAttributeMapper.Clear();
    }

    #region SubClasses

    private class PropertyValueAttributeClass
    {
        [
            Name("ColumnString"),
            DbType(DbType.AnsiStringFixedLength),
            Direction(ParameterDirection.InputOutput),
            IsNullable(true),
            Precision(100),
            Scale(2),
            Size(256)
        ]
        public string PropertyString { get; set; }
    }

    #endregion

    [TestMethod]
    public void TestPropertyInfoGetPropertyValueAttributesMethod()
    {
        // Prepare
        var property = PropertyCache.Get<PropertyValueAttributeClass>(e => e.PropertyString);

        // Act
        var attributes = property.PropertyInfo.GetPropertyValueAttributes();

        // Assert
        Assert.AreEqual(7, attributes.Count());
    }

    [TestMethod]
    public void TestPropertyInfoGetPropertyValueAttributeMethod()
    {
        // Prepare
        var property = PropertyCache.Get<PropertyValueAttributeClass>(e => e.PropertyString);

        // Act
        var attribute = property.PropertyInfo.GetPropertyValueAttribute<NameAttribute>();

        // Assert
        Assert.IsNotNull(attribute);
        Assert.AreEqual("ColumnString", attribute.Name);
    }
}
