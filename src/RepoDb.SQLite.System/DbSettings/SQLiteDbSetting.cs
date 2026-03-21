using RepoDb.Extensions.QueryFields;

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
        OpeningQuote = "[";
        ParameterPrefix = "@";
        MaxQueriesInBatchCount = 10; // No need to optimize using higher value as there is no network latency in SQLite
    }

    /// <inheritdoc />
    protected override string TranslateFunctionalFormat(string format)
    {
        if (format.StartsWith("LEFT({0}, ", StringComparison.Ordinal))
#pragma warning disable CA1845 // Use span-based 'string.Concat'
            return "SUBSTR({0}, 1, " + format.Substring("LEFT({0}, ".Length);
        else if (format.StartsWith("RIGHT({0}, ", StringComparison.Ordinal))
            return "SUBSTR({0}, -" + format.Substring("RIGHT({0}, ".Length);
#pragma warning restore CA1845 // Use span-based 'string.Concat'

        return format switch
        {
            DateTimePartQueryField.YearFormat => "CAST(STRFTIME('%Y', {0}) AS INTEGER)",
            DateTimePartQueryField.MonthFormat => "CAST(STRFTIME('%m', {0}) AS INTEGER)",
            DateTimePartQueryField.DayFormat => "CAST(STRFTIME('%d', {0}) AS INTEGER)",
            DateTimePartQueryField.HourFormat => "CAST(STRFTIME('%H', {0}) AS INTEGER)",
            DateTimePartQueryField.MinuteFormat => "CAST(STRFTIME('%M', {0}) AS INTEGER)",
            DateTimePartQueryField.SecondFormat => "CAST(STRFTIME('%S', {0}) AS INTEGER)",
            DateTimePartQueryField.MillisecondFormat => "CAST(strftime('%f', {0}) * 1000 AS INTEGER) % 1000",
            DateTimePartQueryField.DateFormat => "STRFTIME('%Y-%m-%dT00:00:00.0000000', {0})", // Standard DotNet date has msec added with 7 digits precision in this format
            _ => base.TranslateFunctionalFormat(format)
        };
    }
}
