namespace RepoDb.Options;

public record class SqlServerOptions
{
    /// <summary>
    /// Enable the support for the SQL Server's <c>IDENTITY_INSERT</c> feature. This allows the insertion of explicit values into identity columns.
    /// </summary>
    /// <remarks>In all other RepoDB implementations this is by default enabled, but</remarks>
    public bool UseIdentityInsert { get; init; }

    public static SqlServerOptions Current { get; internal set; } = new();
}
