using Microsoft.Data.SqlClient;
using RepoDb.IntegrationTests.Setup;

namespace RepoDb.IntegrationTests;

[TestClass]
public class BatchExecutionTest : TestBase
{
    [TestMethod]
    public async Task TestBatchExecutionForInsertAll()
    {
        using var connection = CreateConnection();
        for (var i = Constant.DefaultBatchOperationSize * 2; i > 0; i--)
        {
            var identityTables = Helper.CreateIdentityTables(i);
            connection.InsertAll(identityTables);
            await connection.InsertAllAsync(identityTables, cancellationToken: TestContext.CancellationToken);
            connection.UpdateAll(identityTables);
            await connection.UpdateAllAsync(identityTables, cancellationToken: TestContext.CancellationToken);
            connection.MergeAll(identityTables);
            await connection.MergeAllAsync(identityTables, cancellationToken: TestContext.CancellationToken);
        }
    }

    [TestMethod]
    public async Task TestBatchExecutionForUpdateAll()
    {
        using var connection = CreateConnection();
        for (var i = Constant.DefaultBatchOperationSize + 2; i > 0; i--)
        {
            var identityTables = Helper.CreateIdentityTables(i);
            connection.InsertAll(identityTables);
            connection.UpdateAll(identityTables);
            await connection.UpdateAllAsync(identityTables, cancellationToken: TestContext.CancellationToken);
        }
    }

    [TestMethod]
    public async Task TestBatchExecutionForMergeAllEmptyTable()
    {
        using var connection = CreateConnection();
        for (var i = Constant.DefaultBatchOperationSize * 2; i > 0; i--)
        {
            var identityTables = Helper.CreateIdentityTables(i);
            connection.MergeAll(identityTables);
            await connection.MergeAllAsync(identityTables, cancellationToken: TestContext.CancellationToken);
        }
    }

    [TestMethod]
    public async Task TestBatchExecutionForMergeAllNonEmptyTable()
    {
        using var connection = CreateConnection();
        for (var i = Constant.DefaultBatchOperationSize * 2; i > 0; i--)
        {
            var identityTables = Helper.CreateIdentityTables(i);
            connection.InsertAll(identityTables);
            connection.MergeAll(identityTables);
            await connection.MergeAllAsync(identityTables, cancellationToken: TestContext.CancellationToken);
        }
    }
}
