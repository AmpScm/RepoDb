using System.Data;
using RepoDb;
using RepoDb.Enumerations;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.Core.UnitTests;

[TestClass]
public class DbRepositoryDisposalTest
{
    #region CreateConnection

    [TestMethod]
    public void TestCreateConnectionWithInstancePersistency()
    {
        // Arrange
        var connectionString = "Data Source=localhost;";
        var repository = new DbRepository<CustomDbConnection>(
            connectionString,
            connectionPersistency: ConnectionPersistency.Instance);

        try
        {
            // Act
            var connection1 = repository.CreateConnection();
            var connection2 = repository.CreateConnection();

            // Assert
            Assert.IsNotNull(connection1);
            Assert.IsNotNull(connection2);
            Assert.AreEqual(connection1, connection2, "Instance persistency should return the same connection");
        }
        finally
        {
            repository.Dispose();
        }
    }

    [TestMethod]
    public void TestCreateConnectionWithForceTrue()
    {
        // Arrange
        var connectionString = "Data Source=localhost;";
        var repository = new DbRepository<CustomDbConnection>(
            connectionString,
            connectionPersistency: ConnectionPersistency.Instance);

        try
        {
            // Act
            var connection1 = repository.CreateConnection(force: false);
            var connection2 = repository.CreateConnection(force: true);

            // Assert
            Assert.IsNotNull(connection1);
            Assert.IsNotNull(connection2);
            Assert.AreNotEqual(connection1, connection2, "force=true should create a new connection");
        }
        finally
        {
            repository.Dispose();
        }
    }

    [TestMethod]
    public void TestCreateConnectionWithPerCallPersistency()
    {
        // Arrange
        var connectionString = "Data Source=localhost;";
        var repository = new DbRepository<CustomDbConnection>(
            connectionString,
            connectionPersistency: ConnectionPersistency.PerCall);

        // Act
        var connection1 = repository.CreateConnection();
        var connection2 = repository.CreateConnection();

        try
        {
            // Assert
            Assert.IsNotNull(connection1);
            Assert.IsNotNull(connection2);
            Assert.AreNotEqual(connection1, connection2, "PerCall persistency should return different connections");
        }
        finally
        {
            connection1.Dispose();
            connection2.Dispose();
        }
    }

    #endregion

    #region Dispose

    [TestMethod]
    public void TestDisposeWithInstancePersistency()
    {
        // Arrange
        var connectionString = "Data Source=localhost;";
        var repository = new DbRepository<CustomDbConnection>(
            connectionString,
            connectionPersistency: ConnectionPersistency.Instance);

        var connection = repository.CreateConnection();

        // Act
        repository.Dispose();

        // Assert
        Assert.IsTrue(connection.IsDisposed, "Connection should be disposed when repository is disposed with Instance persistency");
    }

    [TestMethod]
    public void TestDisposeWithPerCallPersistency()
    {
        // Arrange
        var connectionString = "Data Source=localhost;";
        var repository = new DbRepository<CustomDbConnection>(
            connectionString,
            connectionPersistency: ConnectionPersistency.PerCall);

        var connection = repository.CreateConnection();

        // Act - Should not throw
        repository.Dispose();

        // Assert - The connection should not be disposed by the repository
        Assert.IsFalse(connection.IsDisposed, "Connection should not be disposed by repository with PerCall persistency");
        connection.Dispose();
    }

    [TestMethod]
    public void TestDisposeTwice()
    {
        // Arrange
        var connectionString = "Data Source=localhost;";
        var repository = new DbRepository<CustomDbConnection>(
            connectionString,
            connectionPersistency: ConnectionPersistency.Instance);

        repository.CreateConnection();

        // Act & Assert - Should not throw on second dispose
        repository.Dispose();
        repository.Dispose(); // Should not throw
    }

    #endregion

    #region DisposeConnectionForPerCall

    [TestMethod]
    public void TestDisposeConnectionForPerCallWithoutTransaction()
    {
        // Arrange
        var connectionString = "Data Source=localhost;";
        var repository = new DbRepository<CustomDbConnection>(
            connectionString,
            connectionPersistency: ConnectionPersistency.PerCall);

        var connection = repository.CreateConnection();
        var initialState = connection.IsDisposed;

        // Act
        repository.DisposeConnectionForPerCall(connection, transaction: null);

        // Assert
        Assert.IsFalse(initialState, "Connection should not be disposed before calling DisposeConnectionForPerCall");
        Assert.IsTrue(connection.IsDisposed, "Connection should be disposed when transaction is null and persistency is PerCall");
    }

    [TestMethod]
    public void TestDisposeConnectionForPerCallWithTransaction()
    {
        // Arrange
        var connectionString = "Data Source=localhost;";
        var repository = new DbRepository<CustomDbConnection>(
            connectionString,
            connectionPersistency: ConnectionPersistency.PerCall);

        var connection = repository.CreateConnection();
        var mockTransaction = new CustomDbTransaction(connection);

        // Act
        repository.DisposeConnectionForPerCall(connection, transaction: mockTransaction);

        // Assert
        Assert.IsFalse(connection.IsDisposed, "Connection should not be disposed when transaction is not null");
        connection.Dispose();
        mockTransaction.Dispose();
    }

    [TestMethod]
    public void TestDisposeConnectionForPerCallWithInstancePersistency()
    {
        // Arrange
        var connectionString = "Data Source=localhost;";
        var repository = new DbRepository<CustomDbConnection>(
            connectionString,
            connectionPersistency: ConnectionPersistency.Instance);

        var connection = repository.CreateConnection();

        // Act
        repository.DisposeConnectionForPerCall(connection, transaction: null);

        // Assert - With Instance persistency, connection should NOT be disposed
        Assert.IsFalse(connection.IsDisposed, "Connection should not be disposed with Instance persistency");
        repository.Dispose();
    }

    #endregion
}
