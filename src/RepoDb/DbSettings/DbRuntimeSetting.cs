namespace RepoDb.DbSettings;

/// <summary>
///
/// </summary>
public sealed record class DbRuntimeSetting
{
    /// <summary>
    /// The name of the database engine (e.g. SQL Server, MySQL, etc.).
    /// </summary>
    public string EngineName { get; init; } = "";

    /// <summary>
    /// The version of the database engine (e.g. 15.0 for SQL Server 2019, etc.).
    /// </summary>
    public Version EngineVersion { get; init; } = new Version(0, 0);

    /// <summary>
    /// The compatibility version of the database engine. This is used to determine the features that are supported by the database engine.
    /// For example, SQL Server 2019 has a compatibility level of 150, while SQL Server 2016 has a compatibility level of 130. This can be used to determine if certain features (e.g. JSON support) are available or not.
    /// </summary>
    public Version CompatibilityVersion { get; init; } = new Version(0, 0);

    /// <summary>
    /// The parameter type map for the database connection. This is used to determine the mapping between .NET types and database types for parameters. This can be used to optimize parameter creation
    /// and execution by providing specific mappings for different RDBMS data providers.
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
