using System.Data;
using System.Data.Common;
using RepoDb.Interfaces;

namespace RepoDb;
public static partial class DbConnectionExtension
{
    #region ExecuteReader

    /// <summary>
    /// Executes a SQL statement from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
    /// returns the instance of the data reader.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="commandText">The command text to be used.</param>
    /// <param name="param">
    /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
    /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
    /// </param>
    /// <param name="commandType">The command type to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <return>The instance of the <see cref="IDataReader"/> object.</return>
    public static IDataReader ExecuteReader(this IDbConnection connection,
        string commandText,
        object? param = null,
        CommandType commandType = default,
        string? traceKey = TraceKeys.ExecuteReader,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ITrace? trace = null)
    {
        return ExecuteReaderInternal(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            traceKey: traceKey,
            commandTimeout: commandTimeout,
            transaction: transaction,
            trace: trace,
            entityType: null,
            dbFields: null,
            skipCommandArrayParametersCheck: false);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="commandText"></param>
    /// <param name="param"></param>
    /// <param name="commandType"></param>
    /// <param name="commandTimeout"></param>
    /// <param name="traceKey"></param>
    /// <param name="transaction"></param>
    /// <param name="trace"></param>
    /// <param name="entityType"></param>
    /// <param name="dbFields"></param>
    /// <param name="skipCommandArrayParametersCheck"></param>
    /// <param name="beforeExecutionCallback"></param>
    /// <returns></returns>
    internal static DbDataReader ExecuteReaderInternal(this IDbConnection connection,
        string commandText,
        object? param,
        CommandType commandType,
        int commandTimeout,
        string? traceKey,
        IDbTransaction? transaction,
        ITrace? trace,
        Type? entityType,
        DbFieldCollection? dbFields,
        bool skipCommandArrayParametersCheck,
        Func<DbCommand, TraceResult?>? beforeExecutionCallback = null)
    {
        // Variables
        var setting = DbSettingMapper.Get(connection);
        var command = CreateDbCommandForExecution(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            commandTimeout: commandTimeout,
            transaction: transaction,
            entityType: entityType,
            dbFields: dbFields,
            skipCommandArrayParametersCheck: skipCommandArrayParametersCheck);
        var hasError = false;

        // Ensure the DbCommand disposal
        try
        {
            // A hacky solution for other operations (i.e.: QueryMultiple)
            var traceResult = beforeExecutionCallback?.Invoke(command);

            // Before Execution
            traceResult ??= Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            // Silent cancellation
            if (traceResult?.CancellableTraceLog?.IsCancelled == true)
            {
                return null!;
            }

            // Execute
            var reader = command.ExecuteReader();

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, reader);

            // Set the output parameters
            SetOutputParameters(param);

            // Return
            return reader;
        }
        catch
        {
            hasError = true;
            throw;
        }
        finally
        {
            if (setting?.IsExecuteReaderDisposable == true || hasError)
            {
                command.Dispose();
            }
        }
    }

    #endregion

    #region ExecuteReaderAsync

    /// <summary>
    /// Executes a SQL statement from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
    /// returns the instance of the data reader.
    /// </summary>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="commandText">The command text to be used.</param>
    /// <param name="param">
    /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
    /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
    /// </param>
    /// <param name="commandType">The command type to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <return>The instance of the <see cref="IDataReader"/> object.</return>
    public static async Task<IDataReader> ExecuteReaderAsync(this IDbConnection connection,
        string commandText,
        object? param = null,
        CommandType commandType = default,
        string? traceKey = TraceKeys.ExecuteReader,
        int commandTimeout = 0,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteReaderAsyncInternal(connection: connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            traceKey: traceKey,
            commandTimeout: commandTimeout,
            transaction: transaction,
            trace: trace,
            cancellationToken: cancellationToken,
            entityType: null,
            dbFields: null,
            skipCommandArrayParametersCheck: false);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="commandText"></param>
    /// <param name="param"></param>
    /// <param name="commandType"></param>
    /// <param name="commandTimeout"></param>
    /// <param name="traceKey"></param>
    /// <param name="transaction"></param>
    /// <param name="trace"></param>
    /// <param name="entityType"></param>
    /// <param name="dbFields"></param>
    /// <param name="skipCommandArrayParametersCheck"></param>
    /// <param name="beforeExecutionCallbackAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal static async ValueTask<DbDataReader> ExecuteReaderAsyncInternal(this IDbConnection connection,
        string commandText,
        object? param,
        CommandType commandType,
        int commandTimeout,
        string? traceKey,
        IDbTransaction? transaction,
        ITrace? trace,
        Type? entityType,
        DbFieldCollection? dbFields,
        bool skipCommandArrayParametersCheck,
        Func<DbCommand, CancellationToken, ValueTask<TraceResult?>>? beforeExecutionCallbackAsync = null,
        CancellationToken cancellationToken = default)
    {
        // Variables
        var setting = connection.GetDbSetting();
        var command = await CreateDbCommandForExecutionAsync(connection: (DbConnection)connection,
            commandText: commandText,
            param: param,
            commandType: commandType,
            commandTimeout: commandTimeout,
            transaction: transaction,
            entityType: entityType,
            dbFields: dbFields,
            skipCommandArrayParametersCheck: skipCommandArrayParametersCheck,
            cancellationToken: cancellationToken).ConfigureAwait(false);
        var hasError = false;

        // Ensure the DbCommand disposal
        try
        {
            TraceResult? traceResult = null;

            // A hacky solution for other operations (i.e.: QueryMultipleAsync)
            if (beforeExecutionCallbackAsync != null)
            {
                traceResult = await beforeExecutionCallbackAsync(command, cancellationToken).ConfigureAwait(false);
            }

            // Before Execution
            traceResult ??= await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken).ConfigureAwait(false);

            // Silent cancellation
            if (traceResult?.CancellableTraceLog?.IsCancelled == true)
            {
                return null!;
            }

            // Execute
            var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, reader, cancellationToken).ConfigureAwait(false);

            // Set the output parameters
            SetOutputParameters(param);

            // Return
            return reader;
        }
        catch
        {
            hasError = true;
            throw;
        }
        finally
        {
            if (setting.IsExecuteReaderDisposable || hasError)
            {
                command.Dispose();
            }
        }
    }

    #endregion
}
