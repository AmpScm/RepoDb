using RepoDb.DbSettings;

namespace RepoDb.UnitTests.CustomObjects;

public sealed record CustomDbSetting : BaseDbSetting
{
    public CustomDbSetting()
    {
        AreTableHintsSupported = true;
        AverageableType = typeof(double);
        ClosingQuote = "]";
        DefaultSchema = "dbo";
        IsDirectionSupported = false;
        IsExecuteReaderDisposable = true;
        IsMultiStatementExecutable = true;
        IsPreparable = true;
        OpeningQuote = "[";
        ParameterPrefix = "@";
    }
}

public sealed record CustomNonHintsSupportingDbSetting : BaseDbSetting
{
    public CustomNonHintsSupportingDbSetting()
    {
        AreTableHintsSupported = false;
        AverageableType = typeof(double);
        ClosingQuote = "]";
        DefaultSchema = "dbo";
        IsDirectionSupported = false;
        IsExecuteReaderDisposable = true;
        IsMultiStatementExecutable = true;
        IsPreparable = true;
        OpeningQuote = "[";
        ParameterPrefix = "@";

    }
}

public sealed record CustomSingleStatementSupportDbSetting : BaseDbSetting
{
    public CustomSingleStatementSupportDbSetting()
    {
        AreTableHintsSupported = true;
        AverageableType = typeof(double);
        ClosingQuote = "]";
        DefaultSchema = "dbo";
        IsDirectionSupported = false;
        IsExecuteReaderDisposable = true;
        IsMultiStatementExecutable = false;
        IsPreparable = true;
        OpeningQuote = "[";
        ParameterPrefix = "@";
    }
}
