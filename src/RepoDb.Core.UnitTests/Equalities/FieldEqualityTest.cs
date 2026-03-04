using System.Collections;
using Moq;

namespace RepoDb.UnitTests.Equalities;

[TestClass]
public class FieldEqualityTest
{
    [TestMethod]
    public void TestFieldEqualityFromString()
    {
        // Prepare
        var objA = new Field("FieldName");
        var objB = new Field("FieldName");

        Assert.AreEqual(objA.GetHashCode(), objB.GetHashCode());
    }

    [TestMethod]
    public void TestFieldNameCaseSensitivity()
    {
        // Prepare
        var objA = new Field("FieldName");

        // Act
        var equal = Equals(objA, "fieldname");

        // Assert
        Assert.IsFalse(equal);
    }

    [TestMethod]
    public void TestFieldHashCodeEquality()
    {
        // Prepare
        var objA = new Field("FieldName");
        var objB = new Field("FieldName");

        // Act
        var equal = (objA.GetHashCode() == objB.GetHashCode());

        // Assert
        Assert.IsTrue(equal);
    }

    [TestMethod]
    public void TestFieldHashCodeEqualityFromImproperString()
    {
        // Prepare
        var objA = new Field("FieldName");

        // Act
        var equal = (objA.GetHashCode() == "[fieldname]".GetHashCode());

        // Assert
        Assert.IsFalse(equal);
    }

    [TestMethod]
    public void TestFieldObjectEquality()
    {
        // Prepare
        var objA = new Field("FieldName");
        var objB = new Field("FieldName");

        // Act
        var equal = (objA == objB);

        // Assert
        Assert.IsTrue(equal);
    }

    [TestMethod]
    public void TestFieldObjectEqualityFromEqualsMethod()
    {
        // Prepare
        var objA = new Field("FieldName");
        var objB = new Field("FieldName");

        // Act
        var equal = Equals(objA, objB);

        // Assert
        Assert.IsTrue(equal);
    }

    [TestMethod]
    public void TestFieldFromArrayListContainability()
    {
        // Prepare
        var objA = new Field("FieldName");
        var objB = new Field("FieldName");
        var list = new ArrayList
        {
            // Act
            objA
        };
        var equal = list.Contains(objB);

        // Assert
        Assert.IsTrue(equal);
    }

    [TestMethod]
    public void TestFieldGenericListContainability()
    {
        // Prepare
        var objA = new Field("FieldName");
        var objB = new Field("FieldName");
        var list = new List<Field>() { objA };

        // Act
        var equal = list.Contains(objB);

        // Assert
        Assert.IsTrue(equal);
    }

    [TestMethod]
    public void TestFieldGetHashCodeInvocationOnCheckNotNull()
    {
        // Prepare
        var mockOfFiled = new Mock<Field>("FieldName");

        // Act
        if (mockOfFiled.Object != null) { }

        // Assert
        mockOfFiled.Verify(x => x.GetHashCode(), Times.Never);
    }

    [TestMethod]
    public void TestFieldGetHashCodeInvocationOnCheckNull()
    {
        // Prepare
        var mockOfFiled = new Mock<Field>("FieldName");

        // Act
        if (mockOfFiled.Object == null) { }

        // Assert
        mockOfFiled.Verify(x => x.GetHashCode(), Times.Never);
    }

    [TestMethod]
    public void TestFieldGetHashCodeInvocationOnEqualsNull()
    {
        // Prepare
        var mockOfFiled = new Mock<Field>("FieldName");

        // Act
        mockOfFiled.Object.Equals(null);

        // Assert
        mockOfFiled.Verify(x => x.GetHashCode(), Times.Never);
    }
}
