using RepoDb.Interfaces;
using SystemDiagnosticsTrace = System.Diagnostics.Trace;

namespace RepoDb.Trace;

/// <summary>
/// Creates a tracer that writes output to <see cref="SystemDiagnosticsTrace"/>
/// </summary>
public sealed class DiagnosticsTracer : ITrace
{
    private readonly Dictionary<long, DateTime> _timeMap = new();
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="log"></param>
    public void AfterExecution<TResult>(ResultTraceLog<TResult> log)
    {
        ArgumentNullException.ThrowIfNull(log);
        if (_timeMap.TryGetValue(log.SessionId, out var startTime))
        {
            _timeMap.Remove(log.SessionId);

            var executionTime = DateTime.UtcNow - startTime;

            SystemDiagnosticsTrace.WriteLine($"Session {log.SessionId} completed in {executionTime.TotalMilliseconds} ms");
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="log"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public ValueTask AfterExecutionAsync<TResult>(ResultTraceLog<TResult> log, CancellationToken cancellationToken = default)
    {
        if (_timeMap.TryGetValue(log.SessionId, out var startTime))
        {
            _timeMap.Remove(log.SessionId);

            var executionTime = DateTime.UtcNow - startTime;

            SystemDiagnosticsTrace.WriteLine($"Session {log.SessionId} completed in {executionTime.TotalMilliseconds} ms");
        }
        return new();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="log"></param>
    public void BeforeExecution(CancellableTraceLog log)
    {
        _timeMap[log.SessionId] = log.StartTime;
        SystemDiagnosticsTrace.Write("- ");
        SystemDiagnosticsTrace.WriteLine(log);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="log"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public ValueTask BeforeExecutionAsync(CancellableTraceLog log, CancellationToken cancellationToken = default)
    {
        _timeMap[log.SessionId] = log.StartTime;
        SystemDiagnosticsTrace.Write("- ");
        SystemDiagnosticsTrace.WriteLine(log);

        return new();
    }
}
