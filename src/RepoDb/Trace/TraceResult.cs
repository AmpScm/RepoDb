using System.Data;
using System.Data.Common;

namespace RepoDb;

/// <summary>
///
/// </summary>
internal sealed class TraceResult
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="log"></param>
    public TraceResult(CancellableTraceLog log)
    {
        SessionId = log.SessionId;
        StartTime = log.StartTime;
        CancellableTraceLog = log;
    }

    #region Properties

    /// <summary>
    ///
    /// </summary>
    public long SessionId { get; }

    /// <summary>
    ///
    /// </summary>
    public DateTime StartTime { get; }

    /// <summary>
    ///
    /// </summary>
    public CancellableTraceLog CancellableTraceLog { get; }

    #endregion

    #region Methods

    private static long _nextSessionId = 1;

    /// <summary>
    ///
    /// </summary>
    /// <param name="key"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    public static TraceResult Create(string? key,
        DbCommand command) =>
        new TraceResult(
            new CancellableTraceLog(Interlocked.Increment(ref _nextSessionId),
                key, command.CommandText, GetParameters(command)));

    /// <summary>
    ///
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    private static List<IDbDataParameter> GetParameters(DbCommand command)
    {
        var list = new List<IDbDataParameter>();

        foreach (IDbDataParameter parameter in command.Parameters)
        {
            list.Add(parameter);
        }

        return list;
    }

    #endregion
}
