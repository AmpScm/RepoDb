namespace RepoDb.DbSettings;

/// <summary>
/// A setting class used for SQLite data provider.
/// </summary>
public sealed record SQLiteDbSetting : BaseDbSetting
{
    /// <summary>
    /// Creates a new instance of <see cref="SQLiteDbSetting"/> class.
    /// </summary>
    public SQLiteDbSetting()
        : this(true)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="SQLiteDbSetting"/> class.
    /// </summary>
    public SQLiteDbSetting(bool isExecuteReaderDisposable)
    {
        AreTableHintsSupported = false;
        AverageableType = typeof(double);
        ClosingQuote = "]";
        DefaultSchema = null;
        IsDirectionSupported = false;
        IsExecuteReaderDisposable = isExecuteReaderDisposable;
        IsMultiStatementExecutable = true;
        IsPreparable = true;
        IsUseUpsert = true; // Different than Microsoft.Data.SQLite
        OpeningQuote = "[";
        ParameterPrefix = "@";
        MaxQueriesInBatchCount = 10; // No need to optimize using higher value as there is no network latency in SQLite
    }
}
