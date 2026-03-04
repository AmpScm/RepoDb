using System.Collections;
using Moq;
using RepoDb.Enumerations;

namespace RepoDb.UnitTests.Equalities;

[TestClass]
public class OrderFieldEqualityTest
{
    [TestMethod]
    public void TestOrderFieldHashCodeEquality()
    {
        // Prepare
        var objA = new OrderField("OrderFieldName", Order.Ascending);
        var objB = new OrderField("OrderFieldName", Order.Ascending);

        Assert.AreEqual(objA.GetHashCode(), objB.GetHashCode());
        Assert.AreEqual(objA, objB);
    }

    [TestMethod]
    public void TestOrderFieldHashCodeEqualityFromImproperString()
    {
        // Prepare
        var objA = new OrderField("OrderFieldName", Order.Ascending);

        // Act
        var equal = (objA.GetHashCode() == "fieldname".GetHashCode());

        // Assert
        Assert.IsFalse(equal);
    }

    [TestMethod]
    public void TestOrderFieldObjectEquality()
    {
        // Prepare
        var objA = new OrderField("OrderFieldName", Order.Ascending);
        var objB = new OrderField("OrderFieldName", Order.Ascending);

        // Act
        var equal = (objA == objB);

        // Assert
        Assert.IsTrue(equal);
    }

    [TestMethod]
    public void TestOrderFieldObjectEqualityFromEqualsMethod()
    {
        // Prepare
        var objA = new OrderField("OrderFieldName", Order.Ascending);
        var objB = new OrderField("OrderFieldName", Order.Ascending);

        // Act
        var equal = Equals(objA, objB);

        // Assert
        Assert.IsTrue(equal);
    }

    [TestMethod]
    public void TestOrderFieldArrayListContainability()
    {
        // Prepare
        var objA = new OrderField("OrderFieldName", Order.Ascending);
        var objB = new OrderField("OrderFieldName", Order.Ascending);
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
    public void TestOrderFieldGenericListContainability()
    {
        // Prepare
        var objA = new OrderField("OrderFieldName", Order.Ascending);
        var objB = new OrderField("OrderFieldName", Order.Ascending);
        var list = new List<OrderField>
        {
            // Act
            objA
        };
        var equal = list.Contains(objB);

        // Assert
        Assert.IsTrue(equal);
    }

    [TestMethod]
    public void TestOrderFieldIneqaulityByOrder()
    {
        // Prepare
        var objA = new OrderField("OrderFieldName", Order.Ascending);
        var objB = new OrderField("OrderFieldName", Order.Descending);

        // Act
        var equal = (objA == objB);

        // Assert
        Assert.IsFalse(equal);
    }

    [TestMethod]
    public void TestOrderFieldEqualityByOrderFromEqualsMethod()
    {
        // Prepare
        var objA = new OrderField("OrderFieldName", Order.Ascending);
        var objB = new OrderField("OrderFieldName", Order.Descending);

        // Act
        var equal = Equals(objA, objB);

        // Assert
        Assert.IsFalse(equal);
    }

    [TestMethod]
    public void TestOrderFieldArrayListInequalityByOrder()
    {
        // Prepare
        var objA = new OrderField("OrderFieldName", Order.Ascending);
        var objB = new OrderField("OrderFieldName", Order.Descending);
        var list = new ArrayList
        {
            // Act
            objA
        };
        var equal = list.Contains(objB);

        // Assert
        Assert.IsFalse(equal);
    }

    [TestMethod]
    public void TestOrderFieldGenericListInequalityByOrder()
    {
        // Prepare
        var objA = new OrderField("OrderFieldName", Order.Ascending);
        var objB = new OrderField("OrderFieldName", Order.Descending);
        var list = new List<OrderField>
        {
            // Act
            objA
        };
        var equal = list.Contains(objB);

        // Assert
        Assert.IsFalse(equal);
    }

    [TestMethod]
    public void TestOrderFieldGetHashCodeInvocationOnCheckNotNull()
    {
        // Prepare
        var mockOfFiled = new Mock<OrderField>("OrderFieldName", Order.Ascending);

        // Act
        if (mockOfFiled.Object != null) { }

        // Assert
        mockOfFiled.Verify(x => x.GetHashCode(), Times.Never);
    }

    [TestMethod]
    public void TestOrderFieldGetHashCodeInvocationOnCheckNull()
    {
        // Prepare
        var mockOfFiled = new Mock<OrderField>("OrderFieldName", Order.Ascending);

        // Act
        if (mockOfFiled.Object == null) { }

        // Assert
        mockOfFiled.Verify(x => x.GetHashCode(), Times.Never);
    }

    [TestMethod]
    public void TestOrderFieldGetHashCodeInvocationOnEqualsNull()
    {
        // Prepare
        var mockOfFiled = new Mock<OrderField>("OrderFieldName", Order.Ascending);

        // Act
        mockOfFiled.Object.Equals(null);

        // Assert
        mockOfFiled.Verify(x => x.GetHashCode(), Times.Never);
    }
}
