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
}
