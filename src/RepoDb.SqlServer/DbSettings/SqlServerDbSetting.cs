using RepoDb.Extensions.QueryFields;

namespace RepoDb.DbSettings;

/// <summary>
/// A setting class used for SQL Server data provider.
/// </summary>
public sealed record SqlServerDbSetting : BaseDbSetting
{
    /// <summary>
    /// Creates a new instance of <see cref="SqlServerDbSetting"/> class.
    /// </summary>
    public SqlServerDbSetting()
    {
        AreTableHintsSupported = true;
        AverageableType = typeof(double);
        ClosingQuote = "]";
        DefaultSchema = "dbo";
        IsDirectionSupported = true;
        IsExecuteReaderDisposable = true;
        IsMultiStatementExecutable = true;
        IsPreparable = true;
        OpeningQuote = "[";
        ParameterPrefix = "@";

        /*
         * The supposed maximum parameters of 2100 is not working with Microsoft.Data.SqlClient.
         * I reported this issue to SqlClient repository at Github.
         * Link: https://github.com/dotnet/SqlClient/issues/531
         */
        MaxParameterCount = 2100 - 2;
        UseArrayParameterTreshold = 15;
        UseInValuesTreshold = 5;
    }

    /// <inheritdoc />
    protected override string TranslateFunctionalFormat(string format)
        => format switch
        {
            LengthQueryField.LengthFormat => "LEN({0})",
            JsonExtractQueryField.JsonExtractFormat => "JSON_VALUE({0}, @@path@@)",
            DateTimePartQueryField.YearFormat => "DATEPART(yyyy, {0})",
            DateTimePartQueryField.MonthFormat => "DATEPART(m, {0})",
            DateTimePartQueryField.DayFormat => "DATEPART(d, {0})",
            DateTimePartQueryField.HourFormat => "DATEPART(hh, {0})",
            DateTimePartQueryField.MinuteFormat => "DATEPART(mi, {0})",
            DateTimePartQueryField.SecondFormat => "DATEPART(ss, {0})",
            DateTimePartQueryField.MillisecondFormat => "DATEPART(ms, {0})",
            DateTimePartQueryField.DateFormat => "CAST({0} AS DATE)",
            _ => base.TranslateFunctionalFormat(format)

        };


    /// <inheritdoc/>
    public override string? CreateTableParameterText(string parameterName) => $"(SELECT * FROM {parameterName})";
}
