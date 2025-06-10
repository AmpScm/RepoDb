using RepoDb.Interfaces;

namespace RepoDb;

public sealed record class DbRuntimeSetting : IDbRuntimeSetting
{
    public string EngineName { get; init; } = "";
    public Version EngineVersion { get; init; } = new Version(0, 0);
    public Version CompatibilityVersion { get; init; } = new Version(0, 0);

    public IReadOnlyDictionary<Type, DbDataParameterTypeMap>? ParameterTypeMap { get; set; }

    public required IDbSetting DbSetting { get; init; }

    DbRuntimeSetting IDbRuntimeSetting.RuntimeInfo => this;

    bool IDbSetting.AreTableHintsSupported => DbSetting.AreTableHintsSupported;

    Type IDbSetting.AverageableType => DbSetting.AverageableType;
    string IDbSetting.ClosingQuote => DbSetting.ClosingQuote;
    string? IDbSetting.DefaultSchema => DbSetting.DefaultSchema;
    bool IDbSetting.IsDirectionSupported => DbSetting.IsDirectionSupported;
    bool IDbSetting.IsExecuteReaderDisposable => DbSetting.IsExecuteReaderDisposable;
    bool IDbSetting.IsMultiStatementExecutable => DbSetting.IsMultiStatementExecutable;
    bool IDbSetting.IsPreparable => DbSetting.IsPreparable;
    bool IDbSetting.IsUseUpsert => DbSetting.IsUseUpsert;
    string IDbSetting.OpeningQuote => DbSetting.OpeningQuote;
    string IDbSetting.ParameterPrefix => DbSetting.ParameterPrefix;
    bool IDbSetting.ForceAutomaticConversions => DbSetting.ForceAutomaticConversions;
    int IDbSetting.MaxParameterCount => DbSetting.MaxParameterCount;
    int IDbSetting.MaxQueriesInBatchCount => DbSetting.MaxQueriesInBatchCount;
    bool IDbSetting.GenerateFinalSemiColon => DbSetting.GenerateFinalSemiColon;
    bool IDbSetting.QuoteParameterNames => DbSetting.QuoteParameterNames;
    int? IDbSetting.UseArrayParameterTreshold => DbSetting.UseArrayParameterTreshold;
    int? IDbSetting.UseInValuesTreshold => DbSetting.UseInValuesTreshold;
}

public record struct DbDataParameterTypeMap(Type ParameterType, string SchemaObject, string? Schema, string ColumnName, bool NoNull, bool RequiresDistinct);
