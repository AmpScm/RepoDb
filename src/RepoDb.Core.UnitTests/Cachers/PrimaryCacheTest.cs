using RepoDb.Attributes;

namespace RepoDb.UnitTests.Caches;

[TestClass]
public class PrimaryCacheTest
{
    #region SubClasses

    public class BaseClass
    {
        public int PrimaryId { get; set; }
        public string Property1 { get; set; }
    }

    public class BaseClassWithPrimary
    {
        [Primary]
        public int PrimaryId { get; set; }
        public string Property1 { get; set; }
    }

    public class DerivedClass : BaseClass
    {
        public string Property2 { get; set; }
        public string Property3 { get; set; }
    }

    public class DerivedClassWithPrimaryAtBase : BaseClassWithPrimary
    {
        public string Property2 { get; set; }
        public string Property3 { get; set; }
    }

    private class PrimaryClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    private class PrimaryClassWithPrimary
    {
        [Primary]
        public int PrimaryId { get; set; }
        public string Name { get; set; }
    }

    private class PrimaryClassWithUniformPrimary
    {
        public int PrimaryClassWithUniformPrimaryId { get; set; }
        public string Name { get; set; }
    }

    [Map("Primary")]
    private class PrimaryClassWithUniformPrimaryFromTheMapping
    {
        public int PrimaryId { get; set; }
        public string Name { get; set; }
    }

    #endregion

    #region BaseClass

    [TestMethod]
    public void TestPrimaryCacheForBaseClass()
    {
        // Act
#pragma warning disable CS0618 // Type or member is obsolete
        var primary = PrimaryCache.Get<BaseClassWithPrimary>();
#pragma warning restore CS0618 // Type or member is obsolete

        // Assert
        Assert.IsNotNull(primary);
    }

    [TestMethod]
    public void TestPrimaryCacheForTypeOfBaseClass()
    {
        // Act
#pragma warning disable CS0618 // Type or member is obsolete
        var primary = PrimaryCache.Get(typeof(BaseClassWithPrimary));
#pragma warning restore CS0618 // Type or member is obsolete

        // Assert
        Assert.IsNotNull(primary);
    }

    [TestMethod]
    public void TestPrimaryCacheForBaseClassWithoutPrimary()
    {
        // Act
#pragma warning disable CS0618 // Type or member is obsolete
        var primary = PrimaryCache.Get<BaseClass>();
#pragma warning restore CS0618 // Type or member is obsolete

        // Assert
        Assert.IsNull(primary);
    }

    [TestMethod]
    public void TestPrimaryCacheForTypeOfBaseClassWithoutPrimary()
    {
        // Act
#pragma warning disable CS0618 // Type or member is obsolete
        var primary = PrimaryCache.Get(typeof(BaseClass));
#pragma warning restore CS0618 // Type or member is obsolete

        // Assert
        Assert.IsNull(primary);
    }

    #endregion

    #region DerivedClass

    [TestMethod]
    public void TestPrimaryCacheForDerivedClass()
    {
        // Act
#pragma warning disable CS0618 // Type or member is obsolete
        var primary = PrimaryCache.Get<DerivedClassWithPrimaryAtBase>();
#pragma warning restore CS0618 // Type or member is obsolete

        // Assert
        Assert.IsNotNull(primary);
    }

    [TestMethod]
    public void TestPrimaryCacheForTypeOfDerivedClass()
    {
        // Act
#pragma warning disable CS0618 // Type or member is obsolete
        var primary = PrimaryCache.Get(typeof(DerivedClassWithPrimaryAtBase));
#pragma warning restore CS0618 // Type or member is obsolete

        // Assert
        Assert.IsNotNull(primary);
    }

    [TestMethod]
    public void TestPrimaryCacheForDerivedClassWithoutPrimary()
    {
        // Act
#pragma warning disable CS0618 // Type or member is obsolete
        var primary = PrimaryCache.Get<DerivedClass>();
#pragma warning restore CS0618 // Type or member is obsolete

        // Assert
        Assert.IsNull(primary);
    }

    [TestMethod]
    public void TestPrimaryCacheForTypeOfDerivedClassWithoutPrimary()
    {
        // Act
#pragma warning disable CS0618 // Type or member is obsolete
        var primary = PrimaryCache.Get(typeof(DerivedClass));
#pragma warning restore CS0618 // Type or member is obsolete

        // Assert
        Assert.IsNull(primary);
    }

    #endregion

    #region Names

    [TestMethod]
    public void TestPrimaryClass()
    {
        // Act
#pragma warning disable CS0618 // Type or member is obsolete
        var primary = PrimaryCache.Get<PrimaryClass>();
#pragma warning restore CS0618 // Type or member is obsolete

        // Assert
        Assert.IsNotNull(primary);
    }

    [TestMethod]
    public void TestPrimaryClassWithPrimary()
    {
        // Act
#pragma warning disable CS0618 // Type or member is obsolete
        var primary = PrimaryCache.Get<PrimaryClassWithPrimary>();
#pragma warning restore CS0618 // Type or member is obsolete

        // Assert
        Assert.IsNotNull(primary);
    }

    [TestMethod]
    public void TestPrimaryClassWithUniformPrimary()
    {
        // Act
#pragma warning disable CS0618 // Type or member is obsolete
        var primary = PrimaryCache.Get<PrimaryClassWithUniformPrimary>();
#pragma warning restore CS0618 // Type or member is obsolete

        // Assert
        Assert.IsNotNull(primary);
    }

    [TestMethod]
    public void TestPrimaryClassWithUniformPrimaryFromTheMapping()
    {
        // Act
#pragma warning disable CS0618 // Type or member is obsolete
        var primary = PrimaryCache.Get<PrimaryClassWithUniformPrimaryFromTheMapping>();
#pragma warning restore CS0618 // Type or member is obsolete

        // Assert
        Assert.IsNotNull(primary);
    }

    #endregion
}
