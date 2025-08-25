namespace RepoDb.DbSettings;

/// <summary>
/// A setting class used for SQLite data provider.
/// </summary>
public sealed record SqLiteDbSetting : BaseDbSetting
{
    /// <summary>
    /// Creates a new instance of <see cref="SqLiteDbSetting"/> class.
    /// </summary>
    public SqLiteDbSetting()
        : this(false)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="SqLiteDbSetting"/> class.
    /// </summary>
    public SqLiteDbSetting(bool isExecuteReaderDisposable)
    {
        AreTableHintsSupported = false;
        AverageableType = typeof(double);
        ClosingQuote = "]";
        DefaultSchema = null;
        IsDirectionSupported = false;
        IsExecuteReaderDisposable = isExecuteReaderDisposable;
        IsMultiStatementExecutable = true;
        IsPreparable = true;
        OpeningQuote = "[";
        ParameterPrefix = "@";
        MaxQueriesInBatchCount = 10; // No need to optimize using higher value as there is no network latency in SQLite
    }
}
