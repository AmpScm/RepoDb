namespace RepoDb;

/// <summary>
/// 
/// </summary>
public sealed record class DbRuntimeSetting
{
    /// <summary>
    /// 
    /// </summary>
    public string EngineName { get; init; } = "";
    /// <summary>
    /// 
    /// </summary>
    public Version EngineVersion { get; init; } = new Version(0, 0);
    /// <summary>
    /// 
    /// </summary>
    public Version CompatibilityVersion { get; init; } = new Version(0, 0);

    /// <summary>
    /// 
    /// </summary>
    public IReadOnlyDictionary<Type, DbDataParameterTypeMap>? ParameterTypeMap { get; set; }
}

/// <summary>
/// 
/// </summary>
/// <param name="ParameterType"></param>
/// <param name="SchemaObject"></param>
/// <param name="Schema"></param>
/// <param name="ColumnName"></param>
/// <param name="NoNull"></param>
/// <param name="RequiresDistinct"></param>
public record struct DbDataParameterTypeMap(Type ParameterType, string SchemaObject, string? Schema, string ColumnName, bool NoNull, bool RequiresDistinct);
