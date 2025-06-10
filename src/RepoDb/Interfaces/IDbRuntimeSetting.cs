namespace RepoDb.Interfaces;

internal interface IDbRuntimeSetting : IDbSetting
{
    DbRuntimeSetting RuntimeInfo { get; }
}
