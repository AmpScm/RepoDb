namespace RepoDb;

public sealed record class DbRuntimeSetting
{
    public string EngineName { get; init; } = "";
    public Version EngineVersion { get; init; } = new Version(0, 0);
    public Version CompatibilityVersion { get; init; } = new Version(0, 0);

    public IReadOnlyDictionary<Type, DbDataParameterTypeMap>? ParameterTypeMap { get; set; }
}

public record struct DbDataParameterTypeMap(Type ParameterType, string SchemaObject, string? Schema, string ColumnName, bool NoNull, bool RequiresDistinct);
