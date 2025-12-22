using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.SqlClient;
using RepoDb.SqlServer.IntegrationTests.Models;
using RepoDb.SqlServer.IntegrationTests.Setup;

namespace RepoDb.SqlServer.IntegrationTests.Operations;

[TestClass]
public class AverageAllTest
{
    [TestInitialize]
    public void Initialize()
    {
        Database.Initialize();
        Cleanup();
    }

    [TestCleanup]
    public void Cleanup()
    {
        Database.Cleanup();
    }

    #region DataEntity

    #region Sync

    [TestMethod]
    public void TestSqlServerConnectionAverageAll()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.AverageAll<CompleteTable>(e => e.ColumnInt);

        // Assert
        Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
    }

    [TestMethod]
    public void TestSqlServerConnectionAverageAllWithHints()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.AverageAll<CompleteTable>(e => e.ColumnInt,
            SqlServerTableHints.TabLock);

        // Assert
        Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqlServerConnectionAverageAllAsync()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = await connection.AverageAllAsync<CompleteTable>(e => e.ColumnInt, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
    }

    [TestMethod]
    public async Task TestSqlServerConnectionAverageAllAsyncWithHints()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = await connection.AverageAllAsync<CompleteTable>(e => e.ColumnInt,
            SqlServerTableHints.TabLock, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
    }

    #endregion

    #endregion

    #region TableName

    #region Sync

    [TestMethod]
    public void TestSqlServerConnectionAverageAllViaTableName()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.AverageAll(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInt).First());

        // Assert
        Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
    }

    [TestMethod]
    public void TestSqlServerConnectionAverageAllViaTableNameWithHints()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = connection.AverageAll(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
            SqlServerTableHints.TabLock);

        // Assert
        Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task TestSqlServerConnectionAverageAllAsyncViaTableName()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = await connection.AverageAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInt).First(), cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
    }

    [TestMethod]
    public async Task TestSqlServerConnectionAverageAllAsyncViaTableNameWithHints()
    {
        // Setup
        var tables = Database.CreateCompleteTables(10);

        using var connection = new SqlConnection(Database.ConnectionString);
        // Act
        var result = await connection.AverageAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
            Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
            SqlServerTableHints.TabLock, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
    }

    public TestContext TestContext { get; set; }

    #endregion

    #endregion
}
