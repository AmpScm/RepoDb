using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Trace;

[TestClass]
public class CancellableTraceLogTest
{
    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        DbSettingMapper.Add<TraceDbConnection>(new CustomDbSetting(), true);
        DbHelperMapper.Add<TraceDbConnection>(new CustomDbHelper(), true);
        StatementBuilderMapper.Add<TraceDbConnection>(new CustomStatementBuilder(), true);
    }

    #region SubClasses

    private class TraceDbConnection : CustomDbConnection { }

    private class TraceEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    private class ErroneousCancellationTrace : ITrace
    {
        public void AfterExecution<TResult>(ResultTraceLog<TResult> log)
        {
        }

        public ValueTask AfterExecutionAsync<TResult>(ResultTraceLog<TResult> log,
            CancellationToken cancellationToken = default)
        {
            return new();
        }

        public void BeforeExecution(CancellableTraceLog log)
        {
            log.Cancel(true);
        }

        public ValueTask BeforeExecutionAsync(CancellableTraceLog log,
            CancellationToken cancellationToken = default)
        {
            log.Cancel(true);
            return new();
        }
    }

    private class SilentCancellationTrace : ITrace
    {
        public void AfterExecution<TResult>(ResultTraceLog<TResult> log)
        {
            AfterExecutionInvocationCount++;
        }

        public ValueTask AfterExecutionAsync<TResult>(ResultTraceLog<TResult> log,
            CancellationToken cancellationToken = default)
        {
            AfterExecutionInvocationCount++;
            return new();
        }

        public void BeforeExecution(CancellableTraceLog log)
        {
            log.Cancel(false);
            BeforeExecutionInvocationCount++;
        }

        public ValueTask BeforeExecutionAsync(CancellableTraceLog log,
            CancellationToken cancellationToken = default)
        {
            log.Cancel(false);
            BeforeExecutionInvocationCount++;
            return new();
        }

        #region Properties

        public int BeforeExecutionInvocationCount { get; private set; }

        public int AfterExecutionInvocationCount { get; private set; }

        #endregion
    }

    private class PropertyValidatorTrace : ITrace
    {
        public bool IsValid { get; private set; }

        public void AfterExecution<TResult>(ResultTraceLog<TResult> log)
        {
            ValidateResultTraceLog(log);
            IsValid = true;
        }

        public ValueTask AfterExecutionAsync<TResult>(ResultTraceLog<TResult> log,
            CancellationToken cancellationToken = default)
        {
            ValidateResultTraceLog(log);
            IsValid = true;
            return new();
        }

        public void BeforeExecution(CancellableTraceLog log)
        {
            ValidateCancellableTraceLog(log);
            IsValid = true;
        }

        public ValueTask BeforeExecutionAsync(CancellableTraceLog log,
            CancellationToken cancellationToken = default)
        {
            ValidateCancellableTraceLog(log);
            IsValid = true;
            return new();
        }

        private void ValidateResultTraceLog<TResult>(ResultTraceLog<TResult> log)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            if (string.IsNullOrEmpty(log.Key))
            {
                throw new InvalidOperationException(nameof(log.Key));
            }
            if (log.ExecutionTime == TimeSpan.MinValue)
            {
                throw new InvalidOperationException(nameof(log.ExecutionTime));
            }
            if (log.BeforeExecutionLog == null)
            {
                throw new InvalidOperationException(nameof(log.BeforeExecutionLog));
            }
            //if (log.Result == null)
            //{
            //    throw new InvalidOperationException(nameof(log.Result));
            //}
        }

        private void ValidateCancellableTraceLog(CancellableTraceLog log)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            if (log.SessionId == 0)
            {
                throw new InvalidOperationException(nameof(log.SessionId));
            }
            if (string.IsNullOrEmpty(log.Key))
            {
                throw new InvalidOperationException(nameof(log.Key));
            }
            if (log.Parameters == null)
            {
                throw new InvalidOperationException(nameof(log.Parameters));
            }
            if (log.StartTime == DateTime.MinValue)
            {
                throw new InvalidOperationException(nameof(log.StartTime));
            }
        }
    }

    #endregion

    #region Cancelled

    #region ExecuteNonQuery

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnExecuteNonQueryCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        connection
            .ExecuteNonQuery("", trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnExecuteNonQueryAsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        await connection
            .ExecuteNonQueryAsync("", trace: new ErroneousCancellationTrace())
            ;
    }

    #endregion

    #region ExecuteQuery

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnExecuteQueryCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        connection
            .ExecuteQuery("", trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnExecuteQueryAsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        await connection
            .ExecuteQueryAsync("", trace: new ErroneousCancellationTrace())
            ;
    }

    #endregion

    #region ExecuteScalar

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnExecuteScalarCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        connection
            .ExecuteScalar("", trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnExecuteScalarAsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        await connection
            .ExecuteScalarAsync("", trace: new ErroneousCancellationTrace())
            ;
    }

    #endregion

    #region ExecuteQueryMultiple

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnExecuteQueryMultipleCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        connection
            .ExecuteQueryMultiple("", trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionExecuteQueryMultipleAsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        await connection
            .ExecuteQueryMultipleAsync("", trace: new ErroneousCancellationTrace())
            ;
    }

    #endregion

    #region Average

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnAverageCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        connection
            .Average("", (Field)null, (object?)null, trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnAverageAsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        await connection
            .AverageAsync("", (Field)null, (object?)null, trace: new ErroneousCancellationTrace())
            ;
    }

    #endregion

    #region AverageAll

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnAverageAllCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        connection
            .AverageAll("", (Field)null, trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnAverageAllAsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        await connection
            .AverageAllAsync("", (Field)null, trace: new ErroneousCancellationTrace())
            ;
    }

    #endregion

    #region BatchQuery

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnBatchQueryCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        connection
            .BatchQuery("", 0, 100, null, (object?)null, trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnBatchQueryAsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        await connection
            .BatchQueryAsync("", 0, 100, null, (object?)null, trace: new ErroneousCancellationTrace())
            ;
    }

    #endregion

    #region Count

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnCountCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        connection
            .Count("", (Field)null, trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnCountAsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        await connection
            .CountAsync("", (Field)null, trace: new ErroneousCancellationTrace())
            ;
    }

    #endregion

    #region CountAll

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnCountAllCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        connection
            .CountAll("", trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnCountAllAsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        await connection
            .CountAllAsync("", trace: new ErroneousCancellationTrace())
            ;
    }

    #endregion

    #region Delete

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnDeleteCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        connection
            .Delete("", (Field)null, trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnDeleteAsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        await connection
            .DeleteAsync("", (Field)null, trace: new ErroneousCancellationTrace())
            ;
    }

    #endregion

    #region DeleteAll

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnDeleteAllCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        connection
            .DeleteAll("", trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnDeleteAllAsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        await connection
            .DeleteAllAsync("", trace: new ErroneousCancellationTrace())
            ;
    }

    #endregion

    #region Exists

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnExistsCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        connection
            .Exists("", (Field)null, trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnExistsAsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        await connection
            .ExistsAsync("", (Field)null, trace: new ErroneousCancellationTrace())
            ;
    }

    #endregion

    #region Insert

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnInsertCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        connection
            .Insert("", fields: default, trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnInsertAsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        await connection
            .InsertAsync("", fields: default, trace: new ErroneousCancellationTrace())
            ;
    }

    #endregion

    #region InsertAll

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnInsertAllCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();
        var entities = new[]
        {
            new { Id = 1 }
        };


        // Act
        connection
            .InsertAll("", entities, trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnInsertAllMultipleEntitiesCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();
        var entities = new[]
        {
            new { Id = 1 }
        };

        // Act
        connection
            .InsertAll("", entities, trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnInsertAllAsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();
        var entities = new[]
        {
            new { Id = 1 },
            new { Id = 2 },
            new { Id = 3 }
        };

        // Act
        await connection
            .InsertAllAsync("", entities, trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnInsertAllAsyncMultipleEntitiesCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();
        var entities = new[]
        {
            new { Id = 1 },
            new { Id = 2 },
            new { Id = 3 }
        };

        // Act
        await connection
            .InsertAllAsync("", entities, trace: new ErroneousCancellationTrace())
            ;
    }

    #endregion

    #region Max

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnMaxCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        connection
            .Max("", (Field)null, (object?)null, trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnMaxAsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        await connection
            .MaxAsync("", (Field)null, (object?)null, trace: new ErroneousCancellationTrace())
            ;
    }

    #endregion

    #region MaxAll

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnMaxAllCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        connection
            .MaxAll("", (Field)null, trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnMaxAllAsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        await connection
            .MaxAllAsync("", (Field)null, trace: new ErroneousCancellationTrace())
            ;
    }

    #endregion

    #region Merge

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnMergeCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        connection
            .Merge("", new { Id = 1 }, (Field)null, trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnMergeAsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        await connection
            .MergeAsync("", new { Id = 1 }, (Field)null, trace: new ErroneousCancellationTrace())
            ;
    }

    #endregion

    #region MergeAll

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnMergeAllCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();
        var entities = new[]
        {
            new { Id = 1, Name="None" }
        };

        // Act
        connection
            .MergeAll("", entities, trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnMergeAllAsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();
        var entities = new[]
        {
            new { Id = 1, Name= "One" }
        };

        // Act
        await connection
            .MergeAllAsync("", entities, trace: new ErroneousCancellationTrace())
            ;
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnMergeAllMultipleEntitiesCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();
        var entities = new[]
        {
            new { Id = 1, Name="Some" },
            new { Id = 2, Name="Some" },
            new { Id = 3, Name="Some" }
        };

        // Act
        connection
            .MergeAll("", entities, trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnMergeAllMultipleEntitiesAsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();
        var entities = new[]
        {
            new { Id = 1, Name="Other" },
            new { Id = 2, Name="Other" },
            new { Id = 3, Name="Other" }
        };

        // Act
        await connection
            .MergeAllAsync("", entities, trace: new ErroneousCancellationTrace())
            ;
    }

    #endregion

    #region Min

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnMinCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        connection
            .Min("", (Field)null, (object?)null, trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnMinAsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        await connection
            .MinAsync("", (Field)null, (object?)null, trace: new ErroneousCancellationTrace())
            ;
    }

    #endregion

    #region MaxAll

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnMinAllCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        connection
            .MinAll("", (Field)null, trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnMinAllAsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        await connection
            .MinAllAsync("", (Field)null, trace: new ErroneousCancellationTrace())
            ;
    }

    #endregion

    #region Query

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnQueryCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        connection
            .Query("", (QueryField)null, trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnQueryAsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        await connection
            .QueryAsync("", (QueryField)null, trace: new ErroneousCancellationTrace())
            ;
    }

    #endregion

    #region QueryAll

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnQueryAllCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        connection
            .QueryAll("", trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnQueryAllAsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        await connection
            .QueryAllAsync("", trace: new ErroneousCancellationTrace())
            ;
    }

    #endregion

    #region QueryMultiple

    #region T2

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnQueryMultipleForT2CancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        connection
            .QueryMultiple("", (QueryField)null,
                "", (QueryField)null,
                trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnQueryMultipleForT2AsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        await connection
            .QueryMultipleAsync("", (QueryField)null,
                "", (QueryField)null,
                trace: new ErroneousCancellationTrace())
            ;
    }

    #endregion

    #region T3

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnQueryMultipleForT3CancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        connection
            .QueryMultiple("", (QueryField)null,
                "", (QueryField)null,
                "", (QueryField)null,
                trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnQueryMultipleForT3AsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        await connection
            .QueryMultipleAsync("", (QueryField)null,
                "", (QueryField)null,
                "", (QueryField)null,
                trace: new ErroneousCancellationTrace())
            ;
    }

    #endregion

    #region T4

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnQueryMultipleForT4CancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        connection
            .QueryMultiple("", (QueryField)null,
                "", (QueryField)null,
                "", (QueryField)null,
                "", (QueryField)null,
                trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnQueryMultipleForT4AsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        await connection
            .QueryMultipleAsync("", (QueryField)null,
                "", (QueryField)null,
                "", (QueryField)null,
                "", (QueryField)null,
                trace: new ErroneousCancellationTrace())
            ;
    }

    #endregion

    #region T5

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnQueryMultipleForT5CancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        connection
            .QueryMultiple("", (QueryField)null,
                "", (QueryField)null,
                "", (QueryField)null,
                "", (QueryField)null,
                "", (QueryField)null,
                trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnQueryMultipleForT5AsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        await connection
            .QueryMultipleAsync("", (QueryField)null,
                "", (QueryField)null,
                "", (QueryField)null,
                "", (QueryField)null,
                "", (QueryField)null,
                trace: new ErroneousCancellationTrace())
            ;
    }

    #endregion

    #region T6

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnQueryMultipleForT6CancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        connection
            .QueryMultiple("", (QueryField)null,
                "", (QueryField)null,
                "", (QueryField)null,
                "", (QueryField)null,
                "", (QueryField)null,
                "", (QueryField)null,
                trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnQueryMultipleForT6AsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        await connection
            .QueryMultipleAsync("", (QueryField)null,
                "", (QueryField)null,
                "", (QueryField)null,
                "", (QueryField)null,
                "", (QueryField)null,
                "", (QueryField)null,
                trace: new ErroneousCancellationTrace())
            ;
    }

    #endregion

    #region T7

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnQueryMultipleForT7CancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        connection
            .QueryMultiple("", (QueryField)null,
                "", (QueryField)null,
                "", (QueryField)null,
                "", (QueryField)null,
                "", (QueryField)null,
                "", (QueryField)null,
                "", (QueryField)null,
                trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnQueryMultipleForT7AsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        await connection
            .QueryMultipleAsync("", (QueryField)null,
                "", (QueryField)null,
                "", (QueryField)null,
                "", (QueryField)null,
                "", (QueryField)null,
                "", (QueryField)null,
                "", (QueryField)null,
                trace: new ErroneousCancellationTrace())
            ;
    }

    #endregion

    #endregion

    #region Sum

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnSumCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        connection
            .Sum("", (Field)null, (object?)null, trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnSumAsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        await connection
            .SumAsync("", (Field)null, (object?)null, trace: new ErroneousCancellationTrace())
            ;
    }

    #endregion

    #region SumAll

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnSumAllCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        connection
            .SumAll("", (Field)null, trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnSumAllAsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        await connection
            .SumAllAsync("", (Field)null, trace: new ErroneousCancellationTrace())
            ;
    }

    #endregion

    #region Truncate

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnTruncateCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        connection
            .Truncate("", trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnTruncateAsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        await connection
            .TruncateAsync("", trace: new ErroneousCancellationTrace())
            ;
    }

    #endregion

    #region Update

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnUpdateCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        connection
            .Update("", new { Id = 1 }, (Field)null, trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnUpdateAsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();

        // Act
        await connection
            .UpdateAsync("", new { Id = 1 }, (Field)null, trace: new ErroneousCancellationTrace())
            ;
    }

    #endregion

    #region UpdateAll

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnUpdateAllCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();
        var entities = new[]
        {
            new { Id = 1 }
        };

        // Act
        connection
            .UpdateAll("", entities, trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnUpdateAllAsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();
        var entities = new[]
        {
            new { Id = 1 }
        };

        // Act
        await connection
            .UpdateAllAsync("", entities, trace: new ErroneousCancellationTrace())
            ;
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public void ThrowExceptionOnUpdateAllMultipleEntitiesCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();
        var entities = new[]
        {
            new { Id = 1 },
            new { Id = 2 },
            new { Id = 3 }
        };

        // Act
        connection
            .UpdateAll("", entities, trace: new ErroneousCancellationTrace());
    }

    [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
    public async ValueTask ThrowExceptionOnUpdateAllMultipleEntitiesAsyncCancelledOperation()
    {
        // Prepare
        var connection = new TraceDbConnection();
        var entities = new[]
        {
            new { Id = 1 },
            new { Id = 2 },
            new { Id = 3 }
        };

        // Act
        await connection
            .UpdateAllAsync("", entities, trace: new ErroneousCancellationTrace())
            ;
    }

    #endregion

    #endregion

    #region Silent Cancellation

    #region ExecuteNonQuery

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForExecuteNonQuery()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection
            .ExecuteNonQuery("", trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForExecuteNonQueryAsync()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection
            .ExecuteNonQueryAsync("", trace: trace)
            ;

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region ExecuteQuery

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForExecuteQuery()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection
            .ExecuteQuery("", trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForExecuteQueryAsync()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection
            .ExecuteQueryAsync("", trace: trace)
            ;

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region ExecuteScalar

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForExecuteScalar()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection
            .ExecuteScalar("", trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForExecuteScalarAsync()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection
            .ExecuteScalarAsync("", trace: trace)
            ;

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region ExecuteQueryMultiple

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForExecuteQueryMultiple()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection
            .ExecuteQueryMultiple("", trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForExecuteQueryMultipleAsync()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection
            .ExecuteQueryMultipleAsync("", trace: trace)
            ;

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region Average

    #region Average

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForAverage()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Average<TraceEntity>(trace: trace,
            field: e => e.Id,
            where: (object?)null);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }


    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForAverageViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Average(ClassMappedNameCache.Get<TraceEntity>(),
            field: new Field("Id"),
            where: (object?)null,
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region AverageAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForAverageAsync()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.AverageAsync<TraceEntity>(trace: trace,
            field: e => e.Id,
            where: (object?)null);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForAverageAsyncViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.AverageAsync(ClassMappedNameCache.Get<TraceEntity>(),
            field: new Field("Id"),
            where: (object?)null,
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #endregion

    #region AverageAll

    #region AverageAll

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForAverageAll()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.AverageAll<TraceEntity>(trace: trace,
            field: e => e.Id);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForAverageAllViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.AverageAll(ClassMappedNameCache.Get<TraceEntity>(),
            field: new Field("Id"),
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region AverageAllAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForAverageAllAsync()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.AverageAllAsync<TraceEntity>(trace: trace,
            field: e => e.Id);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForAverageAllAsyncViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.AverageAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
            field: new Field("Id"),
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #endregion

    #region BatchQuery

    #region BatchQuery

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForBatchQuery()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.BatchQuery<TraceEntity>(0,
            10,
            OrderField.Ascending<TraceEntity>(t => t.Id).AsEnumerable(),
            where: (QueryGroup)null,
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region BatchQueryAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForBatchQueryAsync()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.BatchQueryAsync<TraceEntity>(0,
            10,
            OrderField.Ascending<TraceEntity>(t => t.Id).AsEnumerable(),
            where: (QueryGroup)null,
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #endregion

    #region Count

    #region Count

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForCount()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Count<TraceEntity>(trace: trace,
            where: (object?)null);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForCountViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Count(ClassMappedNameCache.Get<TraceEntity>(),
            where: (object?)null,
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region CountAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForCountAsync()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.CountAsync<TraceEntity>(trace: trace,
            where: (object?)null);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForCountAsyncViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.CountAsync(ClassMappedNameCache.Get<TraceEntity>(),
            where: (object?)null,
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #endregion

    #region CountAll

    #region CountAll

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForCountAll()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.CountAll<TraceEntity>(trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForCountAllViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.CountAll(ClassMappedNameCache.Get<TraceEntity>(),
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region CountAllAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForCountAllAsync()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.CountAllAsync<TraceEntity>(trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForCountAllAsyncViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.CountAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #endregion

    #region Delete

    #region Delete

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForDelete()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Delete<TraceEntity>(0,
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForDeleteViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Delete(ClassMappedNameCache.Get<TraceEntity>(),
            new
            {
                Id = 1
            },
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region DeleteAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForDeleteAsync()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.DeleteAsync<TraceEntity>(0,
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForDeleteAsyncViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.DeleteAsync(ClassMappedNameCache.Get<TraceEntity>(),
            new
            {
                Id = 1
            },
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #endregion

    #region DeleteAll

    #region DeleteAll

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForDeleteAll()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.DeleteAll<TraceEntity>(trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForDeleteAllViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.DeleteAll(ClassMappedNameCache.Get<TraceEntity>(),
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region DeleteAllAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForDeleteAllAsync()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.DeleteAllAsync<TraceEntity>(trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForDeleteAllAsyncViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.DeleteAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #endregion

    #region Exists

    #region Exists

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForExists()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Exists<TraceEntity>(trace: trace,
            what: (object?)null);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForExistsViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Exists(ClassMappedNameCache.Get<TraceEntity>(),
            what: (object?)null,
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region ExistsAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForExistsAsync()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.ExistsAsync<TraceEntity>(trace: trace,
            what: (object?)null);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForExistsAsyncViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.ExistsAsync(ClassMappedNameCache.Get<TraceEntity>(),
            what: (object?)null,
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #endregion

    #region Insert

    #region Insert

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForInsert()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Insert<TraceEntity>(
            new TraceEntity { Name = "Name" },
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForInsertViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Insert(ClassMappedNameCache.Get<TraceEntity>(),
            new { Name = "Name" },
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region InsertAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForInsertAsync()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.InsertAsync<TraceEntity>(
            new TraceEntity { Name = "Name" },
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForInsertAsyncViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.InsertAsync(ClassMappedNameCache.Get<TraceEntity>(),
            new { Name = "Name" },
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #endregion

    #region InsertAll

    #region InsertAll

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForInsertAll()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.InsertAll<TraceEntity>(new[] { new TraceEntity { Name = "Name" } },
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForInsertAllViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.InsertAll(ClassMappedNameCache.Get<TraceEntity>(),
            new[] { new { Name = "Name" } },
            fields: Field.From("Name"),
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region InsertAllAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForInsertAllAsync()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.InsertAllAsync<TraceEntity>(new[] { new TraceEntity { Name = "Name" } },
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForInsertAllAsyncViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.InsertAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
            new[] { new { Name = "Name" } },
            fields: Field.From("Name"),
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #endregion

    #region Max

    #region Max

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForMax()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Max<TraceEntity>(trace: trace,
            field: e => e.Id,
            where: (object?)null);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForMaxViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Max(ClassMappedNameCache.Get<TraceEntity>(),
            field: new Field("Id"),
            where: (object?)null,
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region MaxAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForMaxAsync()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.MaxAsync<TraceEntity>(trace: trace,
            field: e => e.Id,
            where: (object?)null);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForMaxAsyncViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.MaxAsync(ClassMappedNameCache.Get<TraceEntity>(),
            field: new Field("Id"),
            where: (object?)null,
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #endregion

    #region MaxAll

    #region MaxAll

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForMaxAll()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.MaxAll<TraceEntity>(trace: trace,
            field: e => e.Id);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForMaxAllViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.MaxAll(ClassMappedNameCache.Get<TraceEntity>(),
            field: new Field("Id"),
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region MaxAllAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForMaxAllAsync()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.MaxAllAsync<TraceEntity>(trace: trace,
            field: e => e.Id);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForMaxAllAsyncViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.MaxAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
            field: new Field("Id"),
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #endregion

    #region Merge

    #region Merge

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForMerge()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Merge<TraceEntity>(
            new TraceEntity { Id = 1, Name = "Name" },
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForMergeViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Merge(ClassMappedNameCache.Get<TraceEntity>(),
            new { Id = 1, Name = "Name" },
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region MergeAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForMergeAsync()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.MergeAsync<TraceEntity>(
            new TraceEntity { Id = 1, Name = "Name" },
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForMergeAsyncViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.MergeAsync(ClassMappedNameCache.Get<TraceEntity>(),
            new { Id = 1, Name = "Name" },
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #endregion

    #region MergeAll

    #region MergeAll

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForMergeAll()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.MergeAll<TraceEntity>(
            new[] { new TraceEntity { Id = 1, Name = "Name" } },
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForMergeAllViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.MergeAll(ClassMappedNameCache.Get<TraceEntity>(),
            new[] { new TraceEntity { Id = 1, Name = "Name" } },
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region MergeAllAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForMergeAllAsync()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.MergeAllAsync<TraceEntity>(
            new[] { new TraceEntity { Id = 1, Name = "Name" } },
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForMergeAllAsyncViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.MergeAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
            new[] { new { Id = 1, Name = "Name" } },
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #endregion

    #region Min

    #region Min

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForMin()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Min<TraceEntity>(trace: trace,
            field: e => e.Id,
            where: (object?)null);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForMinViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Min(ClassMappedNameCache.Get<TraceEntity>(),
            field: new Field("Id"),
            where: (object?)null,
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region MinAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForMinAsync()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.MinAsync<TraceEntity>(trace: trace,
            field: e => e.Id,
            where: (object?)null);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForMinAsyncViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.MinAsync(ClassMappedNameCache.Get<TraceEntity>(),
            field: new Field("Id"),
            where: (object?)null,
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #endregion

    #region MinAll

    #region MinAll

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForMinAll()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.MinAll<TraceEntity>(trace: trace,
            field: e => e.Id);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForMinAllViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.MinAll(ClassMappedNameCache.Get<TraceEntity>(),
            field: new Field("Id"),
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region MinAllAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForMinAllAsync()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.MinAllAsync<TraceEntity>(trace: trace,
            field: e => e.Id);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForMinAllAsyncViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.MinAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
            field: new Field("Id"),
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #endregion

    #region Query

    #region Query

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForQuery()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Query<TraceEntity>(te => te.Id == 1,
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region QueryAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForQueryAsync()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.QueryAsync<TraceEntity>(te => te.Id == 1,
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #endregion

    #region QueryAll

    #region QueryAll

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForQueryAll()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.QueryAll<TraceEntity>(trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region QueryAllAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForQueryAllAsync()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.QueryAllAsync<TraceEntity>(trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #endregion

    #region QueryMultiple

    #region QueryMultiple

    #region T2

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForQueryMultipleForT2()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.QueryMultiple<TraceEntity, TraceEntity>(te => te.Id == 1,
            te => te.Id == 1,
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region T3

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForQueryMultipleForT3()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.QueryMultiple<TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region T4

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForQueryMultipleForT4()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.QueryMultiple<TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region T5

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForQueryMultipleForT5()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.QueryMultiple<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region T6

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForQueryMultipleForT6()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.QueryMultiple<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region T7

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForQueryMultipleForT7()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.QueryMultiple<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #endregion

    #region QueryMultipleAsync

    #region T2

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForQueryMultipleAsyncForT2()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.QueryMultipleAsync<TraceEntity, TraceEntity>(te => te.Id == 1,
            te => te.Id == 1,
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region T3

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForQueryMultipleAsyncForT3()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.QueryMultipleAsync<TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region T4

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForQueryMultipleAsyncForT4()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.QueryMultipleAsync<TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region T5

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForQueryMultipleAsyncForT5()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.QueryMultipleAsync<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region T6

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForQueryMultipleAsyncForT6()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.QueryMultipleAsync<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region T7

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForQueryMultipleAsyncForT7()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.QueryMultipleAsync<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #endregion

    #endregion

    #region Sum

    #region Sum

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForSum()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Sum<TraceEntity>(trace: trace,
            field: e => e.Id,
            where: (object?)null);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForSumViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Sum(ClassMappedNameCache.Get<TraceEntity>(),
            field: new Field("Id"),
            where: (object?)null,
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region SumAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForSumAsync()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.SumAsync<TraceEntity>(trace: trace,
            field: e => e.Id,
            where: (object?)null);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForSumAsyncViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.SumAsync(ClassMappedNameCache.Get<TraceEntity>(),
            field: new Field("Id"),
            where: (object?)null,
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #endregion

    #region SumAll

    #region SumAll

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForSumAll()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.SumAll<TraceEntity>(trace: trace,
            field: e => e.Id);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForSumAllViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.SumAll(ClassMappedNameCache.Get<TraceEntity>(),
            field: new Field("Id"),
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region SumAllAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForSumAllAsync()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.SumAllAsync<TraceEntity>(trace: trace,
            field: e => e.Id);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForSumAllAsyncViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.SumAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
            field: new Field("Id"),
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #endregion

    #region Truncate

    #region Truncate

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForTruncate()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Truncate<TraceEntity>(trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForTruncateViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Truncate(ClassMappedNameCache.Get<TraceEntity>(),
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region TruncateAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForTruncateAsync()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.TruncateAsync<TraceEntity>(trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForTruncateAsyncViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.TruncateAsync(ClassMappedNameCache.Get<TraceEntity>(),
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #endregion

    #region Update

    #region Update

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForUpdate()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Update<TraceEntity>(
            new TraceEntity
            {
                Id = 1,
                Name = "Name"
            },
            what: 1,
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForUpdateViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Update(ClassMappedNameCache.Get<TraceEntity>(),
            new
            {
                Name = "Name"
            },
            new
            {
                Id = 1
            },
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region UpdateAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForUpdateAsync()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.UpdateAsync<TraceEntity>(
            new TraceEntity
            {
                Id = 1,
                Name = "Name"
            },
            what: 1,
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForUpdateAsyncViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.UpdateAsync(ClassMappedNameCache.Get<TraceEntity>(),
            new
            {
                Name = "Name"
            },
            new
            {
                Id = 1
            },
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #endregion

    #region UpdateAll

    #region UpdateAll

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForUpdateAll()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.UpdateAll<TraceEntity>(
            new[] { new TraceEntity { Id = 1, Name = "Name" } },
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public void TestDbConnectionTraceSilentCancellationForUpdateAllViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.UpdateAll(ClassMappedNameCache.Get<TraceEntity>(),
            new[] { new { Id = 1, Name = "Name" } },
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #region UpdateAllAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForUpdateAllAsync()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.UpdateAllAsync<TraceEntity>(
            new[] { new TraceEntity { Id = 1, Name = "Name" } },
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTraceSilentCancellationForUpdateAllAsyncViaTableName()
    {
        // Prepare
        var trace = new SilentCancellationTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.UpdateAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
            new[] { new { Id = 1, Name = "Name" } },
            trace: trace);

        // Assert
        Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
        Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
    }

    #endregion

    #endregion

    #endregion

    #region Properties Validation

    #region ExecuteNonQuery

    [TestMethod]
    public void TestDbConnectionTracePropertiesForExecuteNonQuery()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection
            .ExecuteNonQuery("", trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForExecuteNonQueryAsync()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection
            .ExecuteNonQueryAsync("", trace: trace)
            ;

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region ExecuteQuery

    [TestMethod]
    public void TestDbConnectionTracePropertiesForExecuteQuery()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection
            .ExecuteQuery("", trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForExecuteQueryAsync()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection
            .ExecuteQueryAsync("", trace: trace)
            ;

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region ExecuteScalar

    [TestMethod]
    public void TestDbConnectionTracePropertiesForExecuteScalar()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection
            .ExecuteScalar("", trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForExecuteScalarAsync()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection
            .ExecuteScalarAsync("", trace: trace)
            ;

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region ExecuteQueryMultiple

    [TestMethod]
    public void TestDbConnectionTracePropertiesForExecuteQueryMultiple()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection
            .ExecuteQueryMultiple("", trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForExecuteQueryMultipleAsync()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection
            .ExecuteQueryMultipleAsync("", trace: trace)
            ;

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region Average

    #region Average

    [TestMethod]
    public void TestDbConnectionTracePropertiesForAverage()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Average<TraceEntity>(trace: trace,
            field: e => e.Id,
            where: (object?)null);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }


    [TestMethod]
    public void TestDbConnectionTracePropertiesForAverageViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Average(ClassMappedNameCache.Get<TraceEntity>(),
            field: new Field("Id"),
            where: (object?)null,
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region AverageAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForAverageAsync()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.AverageAsync<TraceEntity>(trace: trace,
            field: e => e.Id,
            where: (object?)null);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForAverageAsyncViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.AverageAsync(ClassMappedNameCache.Get<TraceEntity>(),
            field: new Field("Id"),
            where: (object?)null,
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #endregion

    #region AverageAll

    #region AverageAll

    [TestMethod]
    public void TestDbConnectionTracePropertiesForAverageAll()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.AverageAll<TraceEntity>(trace: trace,
            field: e => e.Id);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public void TestDbConnectionTracePropertiesForAverageAllViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.AverageAll(ClassMappedNameCache.Get<TraceEntity>(),
            field: new Field("Id"),
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region AverageAllAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForAverageAllAsync()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.AverageAllAsync<TraceEntity>(trace: trace,
            field: e => e.Id);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForAverageAllAsyncViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.AverageAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
            field: new Field("Id"),
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #endregion

    #region BatchQuery

    #region BatchQuery

    [TestMethod]
    public void TestDbConnectionTracePropertiesForBatchQuery()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.BatchQuery<TraceEntity>(0,
            10,
            OrderField.Ascending<TraceEntity>(t => t.Id).AsEnumerable(),
            where: (QueryGroup)null,
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region BatchQueryAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForBatchQueryAsync()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.BatchQueryAsync<TraceEntity>(0,
            10,
            OrderField.Ascending<TraceEntity>(t => t.Id).AsEnumerable(),
            where: (QueryGroup)null,
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #endregion

    #region Count

    #region Count

    [TestMethod]
    public void TestDbConnectionTracePropertiesForCount()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Count<TraceEntity>(trace: trace,
            where: (object?)null);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public void TestDbConnectionTracePropertiesForCountViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Count(ClassMappedNameCache.Get<TraceEntity>(),
            where: (object?)null,
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region CountAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForCountAsync()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.CountAsync<TraceEntity>(trace: trace,
            where: (object?)null);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForCountAsyncViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.CountAsync(ClassMappedNameCache.Get<TraceEntity>(),
            where: (object?)null,
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #endregion

    #region CountAll

    #region CountAll

    [TestMethod]
    public void TestDbConnectionTracePropertiesForCountAll()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.CountAll<TraceEntity>(trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public void TestDbConnectionTracePropertiesForCountAllViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.CountAll(ClassMappedNameCache.Get<TraceEntity>(),
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region CountAllAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForCountAllAsync()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.CountAllAsync<TraceEntity>(trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForCountAllAsyncViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.CountAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #endregion

    #region Delete

    #region Delete

    [TestMethod]
    public void TestDbConnectionTracePropertiesForDelete()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Delete<TraceEntity>(0,
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public void TestDbConnectionTracePropertiesForDeleteViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Delete(ClassMappedNameCache.Get<TraceEntity>(),
            new
            {
                Id = 1
            },
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region DeleteAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForDeleteAsync()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.DeleteAsync<TraceEntity>(0,
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForDeleteAsyncViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.DeleteAsync(ClassMappedNameCache.Get<TraceEntity>(),
            new
            {
                Id = 1
            },
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #endregion

    #region DeleteAll

    #region DeleteAll

    [TestMethod]
    public void TestDbConnectionTracePropertiesForDeleteAll()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.DeleteAll<TraceEntity>(trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public void TestDbConnectionTracePropertiesForDeleteAllViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.DeleteAll(ClassMappedNameCache.Get<TraceEntity>(),
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region DeleteAllAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForDeleteAllAsync()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.DeleteAllAsync<TraceEntity>(trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForDeleteAllAsyncViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.DeleteAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #endregion

    #region Exists

    #region Exists

    [TestMethod]
    public void TestDbConnectionTracePropertiesForExists()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Exists<TraceEntity>(trace: trace,
            what: (object?)null);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public void TestDbConnectionTracePropertiesForExistsViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Exists(ClassMappedNameCache.Get<TraceEntity>(),
            what: (object?)null,
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region ExistsAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForExistsAsync()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.ExistsAsync<TraceEntity>(trace: trace,
            what: (object?)null);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForExistsAsyncViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.ExistsAsync(ClassMappedNameCache.Get<TraceEntity>(),
            what: (object?)null,
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #endregion

    #region Insert

    #region Insert

    [TestMethod]
    public void TestDbConnectionTracePropertiesForInsert()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Insert<TraceEntity>(
            new TraceEntity { Name = "Name" },
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public void TestDbConnectionTracePropertiesForInsertViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Insert(ClassMappedNameCache.Get<TraceEntity>(),
            new { Name = "Name" },
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region InsertAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForInsertAsync()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.InsertAsync<TraceEntity>(
            new TraceEntity { Name = "Name" },
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForInsertAsyncViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.InsertAsync(ClassMappedNameCache.Get<TraceEntity>(),
            new { Name = "Name" },
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #endregion

    #region InsertAll

    #region InsertAll

    [TestMethod]
    public void TestDbConnectionTracePropertiesForInsertAll()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.InsertAll<TraceEntity>(new[] { new TraceEntity { Name = "Name" } },
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public void TestDbConnectionTracePropertiesForInsertAllViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.InsertAll(ClassMappedNameCache.Get<TraceEntity>(),
            new[] { new { Name = "Name" } },
            fields: Field.From("Name"),
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region InsertAllAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForInsertAllAsync()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.InsertAllAsync<TraceEntity>(new[] { new TraceEntity { Name = "Name" } },
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForInsertAllAsyncViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.InsertAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
            new[] { new { Name = "Name" } },
            fields: Field.From("Name"),
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #endregion

    #region Max

    #region Max

    [TestMethod]
    public void TestDbConnectionTracePropertiesForMax()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Max<TraceEntity>(trace: trace,
            field: e => e.Id,
            where: (object?)null);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public void TestDbConnectionTracePropertiesForMaxViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Max(ClassMappedNameCache.Get<TraceEntity>(),
            field: new Field("Id"),
            where: (object?)null,
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region MaxAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForMaxAsync()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.MaxAsync<TraceEntity>(trace: trace,
            field: e => e.Id,
            where: (object?)null);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForMaxAsyncViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.MaxAsync(ClassMappedNameCache.Get<TraceEntity>(),
            field: new Field("Id"),
            where: (object?)null,
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #endregion

    #region MaxAll

    #region MaxAll

    [TestMethod]
    public void TestDbConnectionTracePropertiesForMaxAll()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.MaxAll<TraceEntity>(trace: trace,
            field: e => e.Id);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public void TestDbConnectionTracePropertiesForMaxAllViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.MaxAll(ClassMappedNameCache.Get<TraceEntity>(),
            field: new Field("Id"),
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region MaxAllAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForMaxAllAsync()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.MaxAllAsync<TraceEntity>(trace: trace,
            field: e => e.Id);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForMaxAllAsyncViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.MaxAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
            field: new Field("Id"),
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #endregion

    #region Merge

    #region Merge

    [TestMethod]
    public void TestDbConnectionTracePropertiesForMerge()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Merge<TraceEntity>(
            new TraceEntity { Id = 1, Name = "Name" },
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public void TestDbConnectionTracePropertiesForMergeViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Merge(ClassMappedNameCache.Get<TraceEntity>(),
            new { Id = 1, Name = "Name" },
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region MergeAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForMergeAsync()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.MergeAsync<TraceEntity>(
            new TraceEntity { Id = 1, Name = "Name" },
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForMergeAsyncViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.MergeAsync(ClassMappedNameCache.Get<TraceEntity>(),
            new { Id = 1, Name = "Name" },
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #endregion

    #region MergeAll

    #region MergeAll

    [TestMethod]
    public void TestDbConnectionTracePropertiesForMergeAll()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.MergeAll<TraceEntity>(
            new[] { new TraceEntity { Id = 1, Name = "Name" } },
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public void TestDbConnectionTracePropertiesForMergeAllViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.MergeAll(ClassMappedNameCache.Get<TraceEntity>(),
            new[] { new TraceEntity { Id = 1, Name = "Name" } },
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region MergeAllAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForMergeAllAsync()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.MergeAllAsync<TraceEntity>(
            new[] { new TraceEntity { Id = 1, Name = "Name" } },
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForMergeAllAsyncViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.MergeAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
            new[] { new { Id = 1, Name = "Name" } },
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #endregion

    #region Min

    #region Min

    [TestMethod]
    public void TestDbConnectionTracePropertiesForMin()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Min<TraceEntity>(trace: trace,
            field: e => e.Id,
            where: (object?)null);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public void TestDbConnectionTracePropertiesForMinViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Min(ClassMappedNameCache.Get<TraceEntity>(),
            field: new Field("Id"),
            where: (object?)null,
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region MinAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForMinAsync()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.MinAsync<TraceEntity>(trace: trace,
            field: e => e.Id,
            where: (object?)null);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForMinAsyncViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.MinAsync(ClassMappedNameCache.Get<TraceEntity>(),
            field: new Field("Id"),
            where: (object?)null,
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #endregion

    #region MinAll

    #region MinAll

    [TestMethod]
    public void TestDbConnectionTracePropertiesForMinAll()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.MinAll<TraceEntity>(trace: trace,
            field: e => e.Id);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public void TestDbConnectionTracePropertiesForMinAllViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.MinAll(ClassMappedNameCache.Get<TraceEntity>(),
            field: new Field("Id"),
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region MinAllAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForMinAllAsync()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.MinAllAsync<TraceEntity>(trace: trace,
            field: e => e.Id);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForMinAllAsyncViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.MinAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
            field: new Field("Id"),
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #endregion

    #region Query

    #region Query

    [TestMethod]
    public void TestDbConnectionTracePropertiesForQuery()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Query<TraceEntity>(te => te.Id == 1,
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region QueryAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForQueryAsync()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.QueryAsync<TraceEntity>(te => te.Id == 1,
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #endregion

    #region QueryAll

    #region QueryAll

    [TestMethod]
    public void TestDbConnectionTracePropertiesForQueryAll()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.QueryAll<TraceEntity>(trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region QueryAllAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForQueryAllAsync()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.QueryAllAsync<TraceEntity>(trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #endregion

    #region QueryMultiple

    #region QueryMultiple

    #region T2

    [TestMethod]
    public void TestDbConnectionTracePropertiesForQueryMultipleForT2()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.QueryMultiple<TraceEntity, TraceEntity>(te => te.Id == 1,
            te => te.Id == 1,
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region T3

    [TestMethod]
    public void TestDbConnectionTracePropertiesForQueryMultipleForT3()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.QueryMultiple<TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region T4

    [TestMethod]
    public void TestDbConnectionTracePropertiesForQueryMultipleForT4()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.QueryMultiple<TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region T5

    [TestMethod]
    public void TestDbConnectionTracePropertiesForQueryMultipleForT5()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.QueryMultiple<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region T6

    [TestMethod]
    public void TestDbConnectionTracePropertiesForQueryMultipleForT6()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.QueryMultiple<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region T7

    [TestMethod]
    public void TestDbConnectionTracePropertiesForQueryMultipleForT7()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.QueryMultiple<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #endregion

    #region QueryMultipleAsync

    #region T2

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForQueryMultipleAsyncForT2()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.QueryMultipleAsync<TraceEntity, TraceEntity>(te => te.Id == 1,
            te => te.Id == 1,
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region T3

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForQueryMultipleAsyncForT3()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.QueryMultipleAsync<TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region T4

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForQueryMultipleAsyncForT4()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.QueryMultipleAsync<TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region T5

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForQueryMultipleAsyncForT5()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.QueryMultipleAsync<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region T6

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForQueryMultipleAsyncForT6()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.QueryMultipleAsync<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region T7

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForQueryMultipleAsyncForT7()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.QueryMultipleAsync<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            te => te.Id == 1,
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #endregion

    #endregion

    #region Sum

    #region Sum

    [TestMethod]
    public void TestDbConnectionTracePropertiesForSum()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Sum<TraceEntity>(trace: trace,
            field: e => e.Id,
            where: (object?)null);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public void TestDbConnectionTracePropertiesForSumViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Sum(ClassMappedNameCache.Get<TraceEntity>(),
            field: new Field("Id"),
            where: (object?)null,
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region SumAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForSumAsync()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.SumAsync<TraceEntity>(trace: trace,
            field: e => e.Id,
            where: (object?)null);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForSumAsyncViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.SumAsync(ClassMappedNameCache.Get<TraceEntity>(),
            field: new Field("Id"),
            where: (object?)null,
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #endregion

    #region SumAll

    #region SumAll

    [TestMethod]
    public void TestDbConnectionTracePropertiesForSumAll()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.SumAll<TraceEntity>(trace: trace,
            field: e => e.Id);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public void TestDbConnectionTracePropertiesForSumAllViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.SumAll(ClassMappedNameCache.Get<TraceEntity>(),
            field: new Field("Id"),
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region SumAllAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForSumAllAsync()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.SumAllAsync<TraceEntity>(trace: trace,
            field: e => e.Id);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForSumAllAsyncViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.SumAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
            field: new Field("Id"),
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #endregion

    #region Truncate

    #region Truncate

    [TestMethod]
    public void TestDbConnectionTracePropertiesForTruncate()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Truncate<TraceEntity>(trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public void TestDbConnectionTracePropertiesForTruncateViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Truncate(ClassMappedNameCache.Get<TraceEntity>(),
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region TruncateAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForTruncateAsync()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.TruncateAsync<TraceEntity>(trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForTruncateAsyncViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.TruncateAsync(ClassMappedNameCache.Get<TraceEntity>(),
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #endregion

    #region Update

    #region Update

    [TestMethod]
    public void TestDbConnectionTracePropertiesForUpdate()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Update<TraceEntity>(
            new TraceEntity
            {
                Id = 1,
                Name = "Name"
            },
            what: 1,
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public void TestDbConnectionTracePropertiesForUpdateViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.Update(ClassMappedNameCache.Get<TraceEntity>(),
            new
            {
                Name = "Name"
            },
            new
            {
                Id = 1
            },
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region UpdateAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForUpdateAsync()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.UpdateAsync<TraceEntity>(
            new TraceEntity
            {
                Id = 1,
                Name = "Name"
            },
            what: 1,
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForUpdateAsyncViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.UpdateAsync(ClassMappedNameCache.Get<TraceEntity>(),
            new
            {
                Name = "Name"
            },
            new
            {
                Id = 1
            },
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #endregion

    #region UpdateAll

    #region UpdateAll

    [TestMethod]
    public void TestDbConnectionTracePropertiesForUpdateAll()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.UpdateAll<TraceEntity>(
            new[] { new TraceEntity { Id = 1, Name = "Name" } },
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public void TestDbConnectionTracePropertiesForUpdateAllViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        connection.UpdateAll(ClassMappedNameCache.Get<TraceEntity>(),
            new[] { new { Id = 1, Name = "Name" } },
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #region UpdateAllAsync

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForUpdateAllAsync()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.UpdateAllAsync<TraceEntity>(
            new[] { new TraceEntity { Id = 1, Name = "Name" } },
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    [TestMethod]
    public async ValueTask TestDbConnectionTracePropertiesForUpdateAllAsyncViaTableName()
    {
        // Prepare
        var trace = new PropertyValidatorTrace();
        var connection = new TraceDbConnection();

        // Act
        await connection.UpdateAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
            new[] { new { Id = 1, Name = "Name" } },
            trace: trace);

        // Assert
        Assert.IsTrue(trace.IsValid);
    }

    #endregion

    #endregion

    #endregion
}
