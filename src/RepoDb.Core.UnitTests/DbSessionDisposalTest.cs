using System.Data.Common;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.Core.UnitTests;

[TestClass]
public class DbSessionDisposalTest
{
    #region Constructor - DbConnection

    [TestMethod]
    public void TestDbSessionConstructorWithConnectionOwnsTrue()
    {
        // Arrange
        var connection = new CustomDbConnection { ConnectionString = "test" };

        // Act
        var session = new DbSession(connection, ownsConnection: true);

        // Assert
        Assert.AreEqual(connection, session.Connection);
        Assert.IsNull(session.Transaction);
    }

    [TestMethod]
    public void TestDbSessionConstructorWithConnectionOwnsFalse()
    {
        // Arrange
        var connection = new CustomDbConnection { ConnectionString = "test" };

        // Act
        var session = new DbSession(connection, ownsConnection: false);

        // Assert
        Assert.AreEqual(connection, session.Connection);
        Assert.IsNull(session.Transaction);
    }

    [TestMethod]
    public void TestDbSessionConstructorWithNullConnection()
    {
        // Arrange & Act & Assert
        try
        {
            var session = new DbSession((DbConnection)null!, ownsConnection: false);
            Assert.Fail("Expected ArgumentNullException");
        }
        catch (ArgumentNullException)
        {
            // Expected
        }
    }

    #endregion

    #region Constructor - DbTransaction

    [TestMethod]
    public void TestDbSessionConstructorWithTransactionOwnsTrue()
    {
        // Arrange
        var connection = new CustomDbConnection { ConnectionString = "test" };
        var transaction = new CustomDbTransaction(connection);

        // Act
        var session = new DbSession(transaction, ownsTransaction: true);

        // Assert
        Assert.AreEqual(connection, session.Connection);
        Assert.AreEqual(transaction, session.Transaction);
    }

    [TestMethod]
    public void TestDbSessionConstructorWithTransactionOwnsFalse()
    {
        // Arrange
        var connection = new CustomDbConnection { ConnectionString = "test" };
        var transaction = new CustomDbTransaction(connection);

        // Act
        var session = new DbSession(transaction, ownsTransaction: false);

        // Assert
        Assert.AreEqual(connection, session.Connection);
        Assert.AreEqual(transaction, session.Transaction);
    }

    [TestMethod]
    public void TestDbSessionConstructorWithNullTransaction()
    {
        // Arrange & Act & Assert
        try
        {
            var session = new DbSession((DbTransaction)null!, ownsTransaction: false);
            Assert.Fail("Expected ArgumentNullException");
        }
        catch (ArgumentNullException)
        {
            // Expected
        }
    }

    #endregion

    #region Dispose - Connection

    [TestMethod]
    public void TestDbSessionDisposeConnectionWithOwnsTrue()
    {
        // Arrange
        var connection = new CustomDbConnection { ConnectionString = "test" };
        var session = new DbSession(connection, ownsConnection: true);

        // Act
        session.Dispose();

        // Assert
        Assert.IsTrue(connection.IsDisposed, "Connection should be disposed when owns=true");
    }

    [TestMethod]
    public void TestDbSessionDisposeConnectionWithOwnsFalse()
    {
        // Arrange
        var connection = new CustomDbConnection { ConnectionString = "test" };
        var session = new DbSession(connection, ownsConnection: false);

        // Act
        session.Dispose();

        // Assert
        Assert.IsFalse(connection.IsDisposed, "Connection should NOT be disposed when owns=false");
    }

    #endregion

    #region Dispose - Transaction

    [TestMethod]
    public void TestDbSessionDisposeTransactionWithOwnsTrue()
    {
        // Arrange
        var connection = new CustomDbConnection { ConnectionString = "test" };
        var transaction = new CustomDbTransaction(connection);
        var session = new DbSession(transaction, ownsTransaction: true);

        // Act
        session.Dispose();

        // Assert
        Assert.IsTrue(transaction.IsDisposed, "Transaction should be disposed when owns=true");
    }

    [TestMethod]
    public void TestDbSessionDisposeTransactionWithOwnsFalse()
    {
        // Arrange
        var connection = new CustomDbConnection { ConnectionString = "test" };
        var transaction = new CustomDbTransaction(connection);
        var session = new DbSession(transaction, ownsTransaction: false);

        // Act
        session.Dispose();

        // Assert
        Assert.IsFalse(transaction.IsDisposed, "Transaction should NOT be disposed when owns=false");
    }

    #endregion

    #region DisposeAsync - Connection

    [TestMethod]
    public async Task TestDbSessionDisposeAsyncConnectionWithOwnsTrue()
    {
        // Arrange
        var connection = new CustomDbConnection { ConnectionString = "test" };
        var session = new DbSession(connection, ownsConnection: true);

        // Act
        await session.DisposeAsync();

        // Assert
        Assert.IsTrue(connection.IsDisposed, "Connection should be disposed when owns=true");
    }

    [TestMethod]
    public async Task TestDbSessionDisposeAsyncConnectionWithOwnsFalse()
    {
        // Arrange
        var connection = new CustomDbConnection { ConnectionString = "test" };
        var session = new DbSession(connection, ownsConnection: false);

        // Act
        await session.DisposeAsync();

        // Assert
        Assert.IsFalse(connection.IsDisposed, "Connection should NOT be disposed when owns=false");
    }

    #endregion

    #region DisposeAsync - Transaction

    [TestMethod]
    public async Task TestDbSessionDisposeAsyncTransactionWithOwnsTrue()
    {
        // Arrange
        var connection = new CustomDbConnection { ConnectionString = "test" };
        var transaction = new CustomDbTransaction(connection);
        var session = new DbSession(transaction, ownsTransaction: true);

        // Act
        await session.DisposeAsync();

        // Assert
        Assert.IsTrue(transaction.IsDisposed, "Transaction should be disposed when owns=true");
    }

    [TestMethod]
    public async Task TestDbSessionDisposeAsyncTransactionWithOwnsFalse()
    {
        // Arrange
        var connection = new CustomDbConnection { ConnectionString = "test" };
        var transaction = new CustomDbTransaction(connection);
        var session = new DbSession(transaction, ownsTransaction: false);

        // Act
        await session.DisposeAsync();

        // Assert
        Assert.IsFalse(transaction.IsDisposed, "Transaction should NOT be disposed when owns=false");
    }

    #endregion

    #region Equals

    [TestMethod]
    public void TestDbSessionEqualsWithSameTransaction()
    {
        // Arrange
        var connection = new CustomDbConnection { ConnectionString = "test" };
        var transaction = new CustomDbTransaction(connection);
        var session1 = new DbSession(transaction);
        var session2 = new DbSession(transaction);

        // Act
        var result = session1.Equals(session2);

        // Assert
        Assert.IsTrue(result, "Sessions with the same transaction should be equal");
    }

    [TestMethod]
    public void TestDbSessionEqualsWithDifferentTransactions()
    {
        // Arrange
        var connection = new CustomDbConnection { ConnectionString = "test" };
        var transaction1 = new CustomDbTransaction(connection);
        var transaction2 = new CustomDbTransaction(connection);
        var session1 = new DbSession(transaction1);
        var session2 = new DbSession(transaction2);

        // Act
        var result = session1.Equals(session2);

        // Assert
        Assert.IsFalse(result, "Sessions with different transactions should not be equal");
    }

    [TestMethod]
    public void TestDbSessionEqualsWithTransactionAndConnection()
    {
        // Arrange
        var connection = new CustomDbConnection { ConnectionString = "test" };
        var transaction = new CustomDbTransaction(connection);
        var session1 = new DbSession(transaction);
        var session2 = new DbSession(connection);

        // Act
        var result = session1.Equals(session2);

        // Assert
        Assert.IsFalse(result, "Session with transaction and session with connection should not be equal");
    }

    [TestMethod]
    public void TestDbSessionEqualsWithObject()
    {
        // Arrange
        var connection = new CustomDbConnection { ConnectionString = "test" };
        var session = new DbSession(connection);

        // Act
        var result = session.Equals((object)session);

        // Assert
        Assert.IsTrue(result, "Session should equal itself");
    }

    [TestMethod]
    public void TestDbSessionEqualsWithNonDbSessionObject()
    {
        // Arrange
        var connection = new CustomDbConnection { ConnectionString = "test" };
        var session = new DbSession(connection);

        // Act
        var result = session.Equals((object)"not a DbSession");

        // Assert
        Assert.IsFalse(result, "DbSession should not equal non-DbSession object");
    }

    #endregion

    #region GetHashCode

    [TestMethod]
    public void TestDbSessionGetHashCodeConsistency()
    {
        // Arrange
        var connection = new CustomDbConnection { ConnectionString = "test" };
        var session = new DbSession(connection);

        // Act
        var hashCode1 = session.GetHashCode();
        var hashCode2 = session.GetHashCode();

        // Assert
        Assert.AreEqual(hashCode1, hashCode2, "GetHashCode should return the same value on repeated calls");
    }

    #endregion
}
