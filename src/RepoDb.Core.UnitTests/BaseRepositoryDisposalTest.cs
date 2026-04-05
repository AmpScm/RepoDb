using RepoDb.Enumerations;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.Core.UnitTests;

[TestClass]
public class BaseRepositoryDisposalTest
{
    public class SimpleEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class TestRepository : BaseRepository<SimpleEntity, CustomDbConnection>
    {
        public TestRepository(string connectionString)
            : base(connectionString)
        {
        }

        public TestRepository(string connectionString, ConnectionPersistency connectionPersistency)
            : base(connectionString, null, connectionPersistency, null, Constant.DefaultCacheItemExpirationInMinutes, null, null)
        {
        }
    }

    #region Dispose

    [TestMethod]
    public void TestBaseRepositoryDisposeWithPerCallPersistency()
    {
        // Arrange
        var connectionString = "Data Source=localhost;";
        var repository = new TestRepository(connectionString, ConnectionPersistency.PerCall);

        // Act
        repository.Dispose();

        // Assert - Should not throw
    }

    [TestMethod]
    public void TestBaseRepositoryDisposeWithInstancePersistency()
    {
        // Arrange
        var connectionString = "Data Source=localhost;";
        var repository = new TestRepository(connectionString, ConnectionPersistency.Instance);

        // Act
        repository.Dispose();

        // Assert - Should not throw
    }

    [TestMethod]
    public void TestBaseRepositoryDisposeTwice()
    {
        // Arrange
        var connectionString = "Data Source=localhost;";
        var repository = new TestRepository(connectionString);

        // Act & Assert - Should not throw on second dispose
        repository.Dispose();
        repository.Dispose(); // Should not throw
    }

    #endregion
}
