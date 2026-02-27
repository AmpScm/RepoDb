using System.Data;
using System.Text;
using Npgsql;
using RepoDb.Extensions.QueryFields;

namespace RepoDb.DbSettings;

/// <summary>
/// A setting class used for <see cref="NpgsqlConnection"/> data provider.
/// </summary>
public sealed record PostgreSqlDbSetting : BaseDbSetting
{
    /// <summary>
    /// Creates a new instance of <see cref="PostgreSqlDbSetting"/> class.
    /// </summary>
    public PostgreSqlDbSetting()
    {
        AreTableHintsSupported = false;
        AverageableType = typeof(double);
        ClosingQuote = "\"";
        DefaultSchema = "public";
        IsDirectionSupported = true;
        IsExecuteReaderDisposable = true;
        IsMultiStatementExecutable = true;
        IsPreparable = true;
        OpeningQuote = "\"";
        ParameterPrefix = "@";
        MaxParameterCount = 8096; // PostgreSQL allows up to 32767 parameters, but we set it lower to avoid issues with large queries and code generated for that
    }

    protected override string? CreateJsonExtract(string path, Parameter parameter)
    {
        var segments = JsonExtractQueryField.SplitJsonPath(path).ToList();
        if (segments.Count == 0)
            return null;

        var sb = new StringBuilder("{0}");

        for (int ix = 0; ix < segments.Count; ix++)
        {
            var seg = segments[ix];
            var op = (ix == segments.Count - 1) ? " ->> " : " -> ";

            if (seg[0] == '[')
            {
                // array index
                var index = seg.Trim('[', ']');
                sb.Append(op).Append(index);
            }
            else
            {
                // property
                sb.Append(op).Append('\'').Append(seg.Replace("'", "''")).Append('\'');
            }
        }

        var expr = sb.ToString();

        // Type casts
        return parameter.DbType switch
        {
            DbType.Int16 or DbType.Int32 or DbType.Int64 or DbType.Byte => $"({expr})::int",
            DbType.Boolean => $"({expr})::boolean",
            DbType.Guid => $"({expr})::uuid",
            DbType.Date => $"({expr})::date",
            DbType.DateTime or DbType.DateTime2 or DbType.DateTimeOffset => $"({expr})::timestamptz",
            DbType.Decimal or DbType.Double or DbType.Single => $"({expr})::numeric",
            _ => expr
        };
    }

    protected override string TranslateFunctionalFormat(string format)
    {
        if (format == JsonExtractQueryField.JsonExtractFormat)
            return "{0}";

        return base.TranslateFunctionalFormat(format);
    }
}
