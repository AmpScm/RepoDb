using System.Transactions;
using Npgsql;
using RepoDb.Enumerations;
using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.PostgreSql.IntegrationTests;

#if NET
[TestClass]
public class TransactionTests
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

    /*
     * Some tests here are only triggers (ie: BatchQuery, Count, CountAll, Query, QueryAll, Truncate)
     */

    #region DbTransaction

    #region BatchQuery

    #region BatchQuery

    [TestMethod]
    public void TestDbTransactionForBatchQuery()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction();
        // Act
        connection.BatchQuery<CompleteTable>(0, 10, OrderField.Parse(new { Id = Order.Ascending }), it => it.Id != 0, transaction: transaction);
    }

    #endregion

    #region BatchQueryAsync

    [TestMethod]
    public async Task TestDbTransactionForBatchQueryAsync()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction();
        // Act
        await connection.BatchQueryAsync<CompleteTable>(0, 10, OrderField.Parse(new { Id = Order.Ascending }), it => it.Id != 0, transaction: transaction, cancellationToken: TestContext.CancellationToken);
    }

    #endregion

    #endregion  

    #region Count

    #region Count

    [TestMethod]
    public void TestDbTransactionForCount()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction();
        // Act
        connection.Count<CompleteTable>(it => it.Id != 0, transaction: transaction);
    }

    #endregion

    #region CountAsync

    [TestMethod]
    public async Task TestDbTransactionForCountAsync()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction();
        // Act
        await connection.CountAsync<CompleteTable>(it => it.Id != 0, transaction: transaction, cancellationToken: TestContext.CancellationToken);
    }

    #endregion

    #endregion  

    #region CountAll

    #region CountAll

    [TestMethod]
    public void TestDbTransactionForCountAll()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction();
        // Act
        connection.CountAll<CompleteTable>(transaction: transaction);
    }

    #endregion

    #region CountAllAsync

    [TestMethod]
    public async Task TestDbTransactionForCountAllAsync()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction();
        // Act
        await connection.CountAllAsync<CompleteTable>(transaction: transaction, cancellationToken: TestContext.CancellationToken);
    }

    #endregion

    #endregion

    #region Delete

    #region Delete

    [TestMethod]
    public void TestDbTransactionForDeleteAsCommitted()
    {
        // Setup
        CompleteTable entity = Helper.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        connection.Insert<CompleteTable>(entity);

        // Prepare
        using (NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction())
        {
            // Act
            connection.Delete<CompleteTable>(entity, transaction: transaction);

            // Act
            transaction.Commit();
        }

        // Assert
        Assert.AreEqual(0, connection.CountAll<CompleteTable>());
    }

    [TestMethod]
    public void TestDbTransactionForDeleteAsRolledBack()
    {
        // Setup
        CompleteTable entity = Helper.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        connection.Insert<CompleteTable>(entity);

        // Prepare
        using (NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction())
        {
            // Act
            connection.Delete<CompleteTable>(entity, transaction: transaction);

            // Act
            transaction.Rollback();
        }

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
    }

    #endregion

    #region DeleteAsync

    [TestMethod]
    public async Task TestDbTransactionForDeleteAsyncAsCommitted()
    {
        // Setup
        CompleteTable entity = Helper.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        connection.Insert<CompleteTable>(entity);

        // Prepare
        using (NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction())
        {
            // Act
            await connection.DeleteAsync<CompleteTable>(entity, transaction: transaction, cancellationToken: TestContext.CancellationToken);

            // Act
            transaction.Commit();
        }

        // Assert
        Assert.AreEqual(0, connection.CountAll<CompleteTable>());
    }

    [TestMethod]
    public async Task TestDbTransactionForDeleteAsyncAsRolledBack()
    {
        // Setup
        CompleteTable entity = Helper.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        connection.Insert<CompleteTable>(entity);

        // Prepare
        using (NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction())
        {
            // Act
            await connection.DeleteAsync<CompleteTable>(entity, transaction: transaction, cancellationToken: TestContext.CancellationToken);

            // Act
            await transaction.RollbackAsync(TestContext.CancellationToken);
        }

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
    }

    #endregion

    #endregion

    #region DeleteAll

    #region DeleteAll

    [TestMethod]
    public void TestDbTransactionForDeleteAllAsCommitted()
    {
        // Setup
        List<CompleteTable> entities = Helper.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        connection.InsertAll<CompleteTable>(entities);

        // Prepare
        using (NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction())
        {
            // Act
            connection.DeleteAll<CompleteTable>(transaction: transaction);

            // Act
            transaction.Commit();
        }

        // Assert
        Assert.AreEqual(0, connection.CountAll<CompleteTable>());
    }

    [TestMethod]
    public void TestDbTransactionForDeleteAllAsRolledBack()
    {
        // Setup
        List<CompleteTable> entities = Helper.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        connection.InsertAll<CompleteTable>(entities);

        // Prepare
        using (NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction())
        {
            // Act
            connection.DeleteAll<CompleteTable>(transaction: transaction);

            // Act
            transaction.Rollback();
        }

        // Assert
        Assert.AreEqual(entities.Count, connection.CountAll<CompleteTable>());
    }

    #endregion

    #region DeleteAllAsync

    [TestMethod]
    public async Task TestDbTransactionForDeleteAllAsyncAsCommitted()
    {
        // Setup
        List<CompleteTable> entities = Helper.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        connection.InsertAll<CompleteTable>(entities);

        // Prepare
        using (NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction())
        {
            // Act
            await connection.DeleteAllAsync<CompleteTable>(transaction: transaction, cancellationToken: TestContext.CancellationToken);

            // Act
            transaction.Commit();
        }

        // Assert
        Assert.AreEqual(0, connection.CountAll<CompleteTable>());
    }

    [TestMethod]
    public async Task TestDbTransactionForDeleteAllAsyncAsRolledBack()
    {
        // Setup
        List<CompleteTable> entities = Helper.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        connection.InsertAll<CompleteTable>(entities);

        // Prepare
        using (NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction())
        {
            // Act
            await connection.DeleteAllAsync<CompleteTable>(transaction: transaction, cancellationToken: TestContext.CancellationToken);

            // Act
            await transaction.RollbackAsync(TestContext.CancellationToken);
        }

        // Assert
        Assert.AreEqual(entities.Count, connection.CountAll<CompleteTable>());
    }

    #endregion

    #endregion

    #region Insert

    #region Insert

    [TestMethod]
    public void TestDbTransactionForInsertAsCommitted()
    {
        // Setup
        CompleteTable entity = Helper.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using (NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction())
        {
            // Act
            connection.Insert<CompleteTable>(entity, transaction: transaction);

            // Act
            transaction.Commit();
        }

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
    }

    [TestMethod]
    public void TestDbTransactionForInsertAsRolledBack()
    {
        // Setup
        CompleteTable entity = Helper.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using (NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction())
        {
            // Act
            connection.Insert<CompleteTable>(entity, transaction: transaction);

            // Act
            transaction.Rollback();
        }

        // Assert
        Assert.AreEqual(0, connection.CountAll<CompleteTable>());
    }

    #endregion

    #region InsertAsync

    [TestMethod]
    public async Task TestDbTransactionForInsertAsyncAsCommitted()
    {
        // Setup
        CompleteTable entity = Helper.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using (NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction())
        {
            // Act
            await connection.InsertAsync<CompleteTable>(entity, transaction: transaction, cancellationToken: TestContext.CancellationToken);

            // Act
            transaction.Commit();
        }

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
    }

    [TestMethod]
    public async Task TestDbTransactionForInsertAsyncAsRolledBack()
    {
        // Setup
        CompleteTable entity = Helper.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using (NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction())
        {
            // Act
            await connection.InsertAsync<CompleteTable>(entity, transaction: transaction, cancellationToken: TestContext.CancellationToken);

            // Act
            await transaction.RollbackAsync(TestContext.CancellationToken);
        }

        // Assert
        Assert.AreEqual(0, connection.CountAll<CompleteTable>());
    }

    #endregion

    #endregion

    #region InsertAll

    #region InsertAll

    [TestMethod]
    public void TestDbTransactionForInsertAllAsCommitted()
    {
        // Setup
        List<CompleteTable> entities = Helper.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using (NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction())
        {
            // Act
            connection.InsertAll<CompleteTable>(entities, transaction: transaction);

            // Act
            transaction.Commit();
        }

        // Assert
        Assert.AreEqual(entities.Count, connection.CountAll<CompleteTable>());
    }

    [TestMethod]
    public void TestDbTransactionForInsertAllAsRolledBack()
    {
        // Setup
        List<CompleteTable> entities = Helper.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using (NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction())
        {
            // Act
            connection.InsertAll<CompleteTable>(entities, transaction: transaction);

            // Act
            transaction.Rollback();
        }

        // Assert
        Assert.AreEqual(0, connection.CountAll<CompleteTable>());
    }

    #endregion

    #region InsertAllAsync

    [TestMethod]
    public async Task TestDbTransactionForInsertAllAsyncAsCommitted()
    {
        // Setup
        List<CompleteTable> entities = Helper.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using (NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction())
        {
            // Act
            await connection.InsertAllAsync<CompleteTable>(entities, transaction: transaction, cancellationToken: TestContext.CancellationToken);

            // Act
            transaction.Commit();
        }

        // Assert
        Assert.AreEqual(entities.Count, connection.CountAll<CompleteTable>());
    }

    [TestMethod]
    public async Task TestDbTransactionForInsertAllAsyncAsRolledBack()
    {
        // Setup
        List<CompleteTable> entities = Helper.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using (NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction())
        {
            // Act
            await connection.InsertAllAsync<CompleteTable>(entities, transaction: transaction, cancellationToken: TestContext.CancellationToken);

            // Act
            await transaction.RollbackAsync(TestContext.CancellationToken);
        }

        // Assert
        Assert.AreEqual(0, connection.CountAll<CompleteTable>());
    }

    #endregion

    #endregion

    #region Merge

    #region Merge

    [TestMethod]
    public void TestDbTransactionForMergeAsCommitted()
    {
        // Setup
        CompleteTable entity = Helper.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using (NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction())
        {
            // Act
            connection.Merge<CompleteTable>(entity, transaction: transaction);

            // Act
            transaction.Commit();
        }

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
    }

    [TestMethod]
    public void TestDbTransactionForMergeAsRolledBack()
    {
        // Setup
        CompleteTable entity = Helper.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using (NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction())
        {
            // Act
            connection.Merge<CompleteTable>(entity, transaction: transaction);

            // Act
            transaction.Rollback();
        }

        // Assert
        Assert.AreEqual(0, connection.CountAll<CompleteTable>());
    }

    #endregion

    #region MergeAsync

    [TestMethod]
    public async Task TestDbTransactionForMergeAsyncAsCommitted()
    {
        // Setup
        CompleteTable entity = Helper.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction();

        // Act
        await connection.MergeAsync<CompleteTable>(entity, transaction: transaction, cancellationToken: TestContext.CancellationToken);

        // Act
        transaction.Commit();

        // Assert
        Assert.AreEqual(1, connection.CountAll<CompleteTable>());
    }

    [TestMethod]
    public async Task TestDbTransactionForMergeAsyncAsRolledBack()
    {
        // Setup
        CompleteTable entity = Helper.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction();

        // Act
        await connection.MergeAsync<CompleteTable>(entity, transaction: transaction, cancellationToken: TestContext.CancellationToken);

        // Act
        await transaction.RollbackAsync(TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(0, connection.CountAll<CompleteTable>());
    }

    #endregion

    #endregion

    #region MergeAll

    #region MergeAll

    [TestMethod]
    public void TestDbTransactionForMergeAllAsCommitted()
    {
        // Setup
        List<CompleteTable> entities = Helper.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using (NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction())
        {
            // Act
            connection.MergeAll<CompleteTable>(entities, transaction: transaction);

            // Act
            transaction.Commit();
        }

        // Assert
        Assert.AreEqual(entities.Count, connection.CountAll<CompleteTable>());
    }

    [TestMethod]
    public void TestDbTransactionForMergeAllAsRolledBack()
    {
        // Setup
        List<CompleteTable> entities = Helper.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using (NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction())
        {
            // Act
            connection.MergeAll<CompleteTable>(entities, transaction: transaction);

            // Act
            transaction.Rollback();
        }

        // Assert
        Assert.AreEqual(0, connection.CountAll<CompleteTable>());
    }

    #endregion

    #region MergeAllAsync

    [TestMethod]
    public async Task TestDbTransactionForMergeAllAsyncAsCommitted()
    {
        // Setup
        List<CompleteTable> entities = Helper.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using (NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction())
        {
            // Act
            await connection.MergeAllAsync<CompleteTable>(entities, transaction: transaction, cancellationToken: TestContext.CancellationToken);

            // Act
            transaction.Commit();
        }

        // Assert
        Assert.AreEqual(entities.Count, connection.CountAll<CompleteTable>());
    }

    [TestMethod]
    public async Task TestDbTransactionForMergeAllAsyncAsRolledBack()
    {
        // Setup
        List<CompleteTable> entities = Helper.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using (NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction())
        {
            // Act
            await connection.MergeAllAsync<CompleteTable>(entities, transaction: transaction, cancellationToken: TestContext.CancellationToken);

            // Act
            await transaction.RollbackAsync(TestContext.CancellationToken);
        }

        // Assert
        Assert.AreEqual(0, connection.CountAll<CompleteTable>());
    }

    #endregion

    #endregion

    #region Query

    #region Query

    [TestMethod]
    public void TestDbTransactionForQuery()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction();
        // Act
        connection.Query<CompleteTable>(it => it.Id != 0, transaction: transaction);
    }

    #endregion

    #region QueryAsync

    [TestMethod]
    public async Task TestDbTransactionForQueryAsync()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction();
        // Act
        await connection.QueryAsync<CompleteTable>(it => it.Id != 0, transaction: transaction, cancellationToken: TestContext.CancellationToken);
    }

    #endregion

    #endregion

    #region QueryAll

    #region QueryAll

    [TestMethod]
    public void TestDbTransactionForQueryAll()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction();
        // Act
        connection.QueryAll<CompleteTable>(transaction: transaction);
    }

    #endregion

    #region QueryAllAsync

    [TestMethod]
    public async Task TestDbTransactionForQueryAllAsync()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction();
        // Act
        await connection.QueryAllAsync<CompleteTable>(transaction: transaction, cancellationToken: TestContext.CancellationToken);
    }

    #endregion

    #endregion

    #region QueryMultiple

    #region QueryMultiple

    [TestMethod]
    public void TestDbTransactionForQueryMultipleT2()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction();
        // Act
        connection.QueryMultiple<CompleteTable, CompleteTable>(it => it.Id != 0,
            it => it.Id != 0,
            transaction: transaction);
    }

    [TestMethod]
    public void TestDbTransactionForQueryMultipleT3()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction();
        // Act
        connection.QueryMultiple<CompleteTable, CompleteTable, CompleteTable>(it => it.Id != 0,
            it => it.Id != 0,
            it => it.Id != 0,
            transaction: transaction);
    }

    [TestMethod]
    public void TestDbTransactionForQueryMultipleT4()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction();
        // Act
        connection.QueryMultiple<CompleteTable, CompleteTable, CompleteTable, CompleteTable>(it => it.Id != 0,
            it => it.Id != 0,
            it => it.Id != 0,
            it => it.Id != 0,
            transaction: transaction);
    }

    [TestMethod]
    public void TestDbTransactionForQueryMultipleT5()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction();
        // Act
        connection.QueryMultiple<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(it => it.Id != 0,
            it => it.Id != 0,
            it => it.Id != 0,
            it => it.Id != 0,
            it => it.Id != 0,
            transaction: transaction);
    }

    [TestMethod]
    public void TestDbTransactionForQueryMultipleT6()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction();
        // Act
        connection.QueryMultiple<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(it => it.Id != 0,
            it => it.Id != 0,
            it => it.Id != 0,
            it => it.Id != 0,
            it => it.Id != 0,
            it => it.Id != 0,
            transaction: transaction);
    }

    [TestMethod]
    public void TestDbTransactionForQueryMultipleT7()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction();
        // Act
        connection.QueryMultiple<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(it => it.Id != 0,
            it => it.Id != 0,
            it => it.Id != 0,
            it => it.Id != 0,
            it => it.Id != 0,
            it => it.Id != 0,
            it => it.Id != 0,
            transaction: transaction);
    }

    #endregion

    #region QueryMultipleAsync

    [TestMethod]
    public async Task TestDbTransactionForQueryMultipleAsyncT2()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction();
        // Act
        await connection.QueryMultipleAsync<CompleteTable, CompleteTable>(it => it.Id != 0,
            it => it.Id != 0,
            transaction: transaction, cancellationToken: TestContext.CancellationToken);
    }

    [TestMethod]
    public async Task TestDbTransactionForQueryMultipleAsyncT3()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction();
        // Act
        await connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable>(it => it.Id != 0,
            it => it.Id != 0,
            it => it.Id != 0,
            transaction: transaction, cancellationToken: TestContext.CancellationToken);
    }

    [TestMethod]
    public async Task TestDbTransactionForQueryMultipleAsyncT4()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction();
        // Act
        await connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable, CompleteTable>(it => it.Id != 0,
            it => it.Id != 0,
            it => it.Id != 0,
            it => it.Id != 0,
            transaction: transaction, cancellationToken: TestContext.CancellationToken);
    }

    [TestMethod]
    public async Task TestDbTransactionForQueryMultipleAsyncT5()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction();
        // Act
        await connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(it => it.Id != 0,
            it => it.Id != 0,
            it => it.Id != 0,
            it => it.Id != 0,
            it => it.Id != 0,
            transaction: transaction, cancellationToken: TestContext.CancellationToken);
    }

    [TestMethod]
    public async Task TestDbTransactionForQueryMultipleAsyncT6()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction();
        // Act
        await connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(it => it.Id != 0,
            it => it.Id != 0,
            it => it.Id != 0,
            it => it.Id != 0,
            it => it.Id != 0,
            it => it.Id != 0,
            transaction: transaction, cancellationToken: TestContext.CancellationToken);
    }

    [TestMethod]
    public async Task TestDbTransactionForQueryMultipleAsyncT7()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction();
        // Act
        await connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(it => it.Id != 0,
            it => it.Id != 0,
            it => it.Id != 0,
            it => it.Id != 0,
            it => it.Id != 0,
            it => it.Id != 0,
            it => it.Id != 0,
            transaction: transaction, cancellationToken: TestContext.CancellationToken);
    }

    #endregion

    #endregion

    #region Truncate

    #region Truncate

    [TestMethod]
    public void TestDbTransactionForTruncate()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction();
        // Act
        connection.Truncate<CompleteTable>(transaction: transaction);
    }

    #endregion

    #region TruncateAsync

    [TestMethod]
    public async Task TestDbTransactionForTruncateAsync()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Prepare
        using NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction();
        // Act
        await connection.TruncateAsync<CompleteTable>(transaction: transaction, cancellationToken: TestContext.CancellationToken);
    }

    #endregion

    #endregion

    #region Update

    #region Update

    [TestMethod]
    public void TestDbTransactionForUpdateAsCommitted()
    {
        // Setup
        CompleteTable entity = Helper.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        connection.Insert<CompleteTable>(entity);

        // Prepare
        using (NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction())
        {
            entity.ColumnBoolean = false;

            // Act
            connection.Update<CompleteTable>(entity, transaction: transaction);

            // Act
            transaction.Commit();
        }

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(entity.Id);

        // Assert
        Assert.IsFalse(queryResult.First().ColumnBoolean);
    }

    [TestMethod]
    public void TestDbTransactionForUpdateAsRolledBack()
    {
        // Setup
        CompleteTable entity = Helper.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        connection.Insert<CompleteTable>(entity);

        // Prepare
        using (NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction())
        {
            entity.ColumnBoolean = false;

            // Act
            connection.Update<CompleteTable>(entity, transaction: transaction);

            // Act
            transaction.Rollback();
        }

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(entity.Id);

        // Assert
        Assert.IsTrue(queryResult.First().ColumnBoolean);
    }

    #endregion

    #region UpdateAsync

    [TestMethod]
    public async Task TestDbTransactionForUpdateAsyncAsCommitted()
    {
        // Setup
        CompleteTable entity = Helper.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        connection.Insert<CompleteTable>(entity);

        // Prepare
        using (NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction())
        {
            entity.ColumnBoolean = false;

            // Act
            await connection.UpdateAsync<CompleteTable>(entity, transaction: transaction, cancellationToken: TestContext.CancellationToken);

            // Act
            transaction.Commit();
        }

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(entity.Id);

        // Assert
        Assert.IsFalse(queryResult.First().ColumnBoolean);
    }

    [TestMethod]
    public async Task TestDbTransactionForUpdateAsyncAsRolledBack()
    {
        // Setup
        CompleteTable entity = Helper.CreateCompleteTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        connection.Insert<CompleteTable>(entity);

        // Prepare
        using (NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction())
        {
            entity.ColumnBoolean = false;

            // Act
            await connection.UpdateAsync<CompleteTable>(entity, transaction: transaction, cancellationToken: TestContext.CancellationToken);

            // Act
            await transaction.RollbackAsync(TestContext.CancellationToken);
        }

        // Act
        IEnumerable<CompleteTable> queryResult = connection.Query<CompleteTable>(entity.Id);

        // Assert
        Assert.IsTrue(queryResult.First().ColumnBoolean);
    }

    #endregion

    #endregion

    #region UpdateAll

    #region UpdateAll

    [TestMethod]
    public void TestDbTransactionForUpdateAllAsCommitted()
    {
        // Setup
        List<CompleteTable> entities = Helper.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        connection.InsertAll<CompleteTable>(entities);

        // Prepare
        using (NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction())
        {
            entities.ForEach(entity => entity.ColumnBoolean = false);

            // Act
            connection.UpdateAll<CompleteTable>(entities, transaction: transaction);

            // Act
            transaction.Commit();
        }

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        entities.ForEach(entity => Assert.IsFalse(queryResult.First(item => item.Id == entity.Id).ColumnBoolean));
    }

    [TestMethod]
    public void TestDbTransactionForUpdateAllAsRolledBack()
    {
        // Setup
        List<CompleteTable> entities = Helper.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        connection.InsertAll<CompleteTable>(entities);

        // Prepare
        using (NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction())
        {
            entities.ForEach(entity => entity.ColumnBoolean = false);

            // Act
            connection.UpdateAll<CompleteTable>(entities, transaction: transaction);

            // Act
            transaction.Rollback();
        }

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        entities.ForEach(entity => Assert.IsTrue(queryResult.First(item => item.Id == entity.Id).ColumnBoolean));
    }

    #endregion

    #region UpdateAllAsync

    [TestMethod]
    public async Task TestDbTransactionForUpdateAllAsyncAsCommitted()
    {
        // Setup
        List<CompleteTable> entities = Helper.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        connection.InsertAll<CompleteTable>(entities);

        // Prepare
        using (NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction())
        {
            entities.ForEach(entity => entity.ColumnBoolean = false);

            // Act
            await connection.UpdateAllAsync<CompleteTable>(entities, transaction: transaction, cancellationToken: TestContext.CancellationToken);

            // Act
            transaction.Commit();
        }

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        entities.ForEach(entity => Assert.IsFalse(queryResult.First(item => item.Id == entity.Id).ColumnBoolean));
    }

    [TestMethod]
    public async Task TestDbTransactionForUpdateAllAsyncAsRolledBack()
    {
        // Setup
        List<CompleteTable> entities = Helper.CreateCompleteTables(10);

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        connection.InsertAll<CompleteTable>(entities);

        // Prepare
        using (NpgsqlTransaction transaction = connection.EnsureOpen().BeginTransaction())
        {
            entities.ForEach(entity => entity.ColumnBoolean = false);

            // Act
            await connection.UpdateAllAsync<CompleteTable>(entities, transaction: transaction, cancellationToken: TestContext.CancellationToken);

            // Act
            await transaction.RollbackAsync(TestContext.CancellationToken);
        }

        // Act
        IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

        // Assert
        entities.ForEach(entity => Assert.IsTrue(queryResult.First(item => item.Id == entity.Id).ColumnBoolean));
    }

    #endregion

    #endregion

    #endregion

    #region TransactionScope

    #region TransactionScope

    #region InsertAll

    [TestMethod]
    public void TestTransactionForInsertAll()
    {
        // Setup
        List<CompleteTable> entities = Helper.CreateCompleteTables(10);

        using var transaction = new TransactionScope();
        using (NpgsqlConnection connection = this.CreateTestConnection())
        {
            // Act
            connection.InsertAll<CompleteTable>(entities);

            // Assert
            Assert.AreEqual(entities.Count, connection.CountAll<CompleteTable>());
        }

        // Complete
        transaction.Complete();
    }

    [TestMethod]
    public async Task TestTransactionForInsertAllAsync()
    {
        // Setup
        List<CompleteTable> entities = Helper.CreateCompleteTables(10);

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        using (NpgsqlConnection connection = this.CreateTestConnection())
        {
            // Act
            await connection.InsertAllAsync<CompleteTable>(entities, cancellationToken: TestContext.CancellationToken);

            // Assert
            Assert.AreEqual(entities.Count, connection.CountAll<CompleteTable>());
        }

        // Complete
        transaction.Complete();
    }

    #endregion

    #region MergeAll

    [TestMethod]
    public void TestTransactionScopeForMergeAll()
    {
        // Setup
        List<CompleteTable> entities = Helper.CreateCompleteTables(10);

        using var transaction = new TransactionScope();
        using (NpgsqlConnection connection = this.CreateTestConnection())
        {
            // Act
            connection.MergeAll<CompleteTable>(entities);

            // Assert
            Assert.AreEqual(entities.Count, connection.CountAll<CompleteTable>());
        }

        // Complete
        transaction.Complete();
    }

    [TestMethod]
    public async Task TestTransactionScopeForMergeAllAsync()
    {
        // Setup
        List<CompleteTable> entities = Helper.CreateCompleteTables(10);

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        using (NpgsqlConnection connection = this.CreateTestConnection())
        {
            // Act
            await connection.MergeAllAsync<CompleteTable>(entities, cancellationToken: TestContext.CancellationToken);

            // Assert
            Assert.AreEqual(entities.Count, connection.CountAll<CompleteTable>());
        }

        // Complete
        transaction.Complete();
    }

    #endregion

    #region UpdateAll

    [TestMethod]
    public void TestTransactionScopeForUpdateAll()
    {
        // Setup
        List<CompleteTable> entities = Helper.CreateCompleteTables(10);

        using var transaction = new TransactionScope();
        using (NpgsqlConnection connection = this.CreateTestConnection())
        {
            // Act
            connection.InsertAll<CompleteTable>(entities);

            // Prepare
            entities.ForEach(entity => entity.ColumnBoolean = false);

            // Act
            connection.UpdateAll<CompleteTable>(entities);

            // Act
            IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

            // Assert
            entities.ForEach(entity => Assert.IsFalse(queryResult.First(item => item.Id == entity.Id).ColumnBoolean));
        }

        // Complete
        transaction.Complete();
    }

    [TestMethod]
    public async Task TestTransactionScopeForUpdateAllAsync()
    {
        // Setup
        List<CompleteTable> entities = Helper.CreateCompleteTables(10);

        using TransactionScope transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        using (NpgsqlConnection connection = this.CreateTestConnection())
        {
            // Act
            connection.InsertAll<CompleteTable>(entities);

            // Prepare
            entities.ForEach(entity => entity.ColumnBoolean = false);

            // Act
            await connection.UpdateAllAsync<CompleteTable>(entities, cancellationToken: TestContext.CancellationToken);

            // Act
            IEnumerable<CompleteTable> queryResult = connection.QueryAll<CompleteTable>();

            // Assert
            entities.ForEach(entity => Assert.IsFalse(queryResult.First(item => item.Id == entity.Id).ColumnBoolean));
        }

        // Complete
        transaction.Complete();
    }

    public TestContext TestContext { get; set; }

    #endregion

    #endregion

    #endregion
}
#endif
