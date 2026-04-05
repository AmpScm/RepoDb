using System.Data;
using RepoDb.Extensions.QueryFields;

namespace RepoDb.DbSettings;

/// <inheritdoc />
public sealed record OracleDbSetting : BaseDbSetting
{
    /// <inheritdoc />
    public OracleDbSetting() : base()
    {
        AreTableHintsSupported = false;
        OpeningQuote = "\"";
        ClosingQuote = "\"";
        AverageableType = typeof(decimal);
        DefaultSchema = null;
        IsDirectionSupported = true;
        IsExecuteReaderDisposable = true;
        IsMultiStatementExecutable = true;
        IsPreparable = true;
        ParameterPrefix = ":p";
        MaxParameterCount = 999; //   Oracle.ManagedDataAccess.Client.OracleException: ORA-01795: maximum number of expressions in a list is 1000 https://docs.oracle.com/error-help/db/ora-01795/
        MaxQueriesInBatchCount = 1000;
        GenerateFinalSemiColon = false;
    }

    /// <inheritdoc />
    protected override string? CreateJsonExtract(string path, Parameter parameter)
    {
        return string.Concat(
            "JSON_VALUE({0}, '",
            JsonExtractQueryField.ToJsonPath(path).Replace("'", "''"),
            "'",
            parameter.DbType switch
            {
                DbType.Int32 or DbType.Int64 or DbType.Int16 or DbType.Byte or DbType.Decimal or DbType.Double or DbType.Single => " RETURNING NUMBER",
                DbType.DateTime or DbType.DateTime2 or DbType.Date or DbType.DateTimeOffset => " RETURNING DATE",
                _ => null
            },
            ")"
        );
    }

    /// <inheritdoc />
    protected override string TranslateFunctionalFormat(string format)
    {
        if (format.StartsWith("LEFT({0}, "))
            return "SUBSTR({0}, 1, " + format.Substring("LEFT({0}, ".Length);
        else if (format.StartsWith("RIGHT({0}, "))
            return "SUBSTR({0}, -" + format.Substring("RIGHT({0}, ".Length);

        return format switch
        {
            DateTimePartQueryField.DateFormat => "TRUNC({0})",
            DateTimePartQueryField.YearFormat => "EXTRACT(YEAR FROM {0})",
            DateTimePartQueryField.MonthFormat => "EXTRACT(MONTH FROM {0})",
            DateTimePartQueryField.DayFormat => "EXTRACT(DAY FROM {0})",
            DateTimePartQueryField.HourFormat => "EXTRACT(HOUR FROM {0})",
            DateTimePartQueryField.MinuteFormat => "EXTRACT(MINUTE FROM {0})",
            DateTimePartQueryField.SecondFormat => "TRUNC(EXTRACT(SECOND FROM {0}))",
            DateTimePartQueryField.MillisecondFormat => "MOD(EXTRACT(SECOND FROM {0}) * 1000, 1000)",

            _ => base.TranslateFunctionalFormat(format)
        };
    }
}
