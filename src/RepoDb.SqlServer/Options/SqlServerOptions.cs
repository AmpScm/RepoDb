namespace RepoDb.Options;

/// <summary>
/// Represents configuration options for SQL Server-specific features used by RepoDB. See <see cref="SqlServerGlobalConfiguration.UseSqlServer(RepoDb.GlobalConfiguration, RepoDb.Options.SqlServerOptions?)"/>
/// </summary>
/// <remarks>Use this type to configure behaviors that are unique to SQL Server when working with RepoDB using <see cref="SqlServerGlobalConfiguration.UseSqlServer(RepoDb.GlobalConfiguration, RepoDb.Options.SqlServerOptions?)"/>. These
/// options influence how RepoDB interacts with SQL Server databases, such as enabling support for identity insert
/// operations.</remarks>
public record class SqlServerOptions
{
    /// <summary>
    /// Enable the support for the SQL Server's <c>IDENTITY_INSERT</c> feature. This allows the insertion of explicit values into identity columns.
    /// </summary>
    /// <remarks>In all other RepoDB implementations this is by default enabled, but</remarks>
    public bool UseIdentityInsert { get; init; }


    /// <summary>
    /// Enables support for the legacy System.Data.SqlClient
    /// </summary>
    public bool InitializeSystemDataSqlClient { get; init; }

    /// <summary>
    /// Gets the current SQL Server settings for RepoDB. This is used internally by RepoDB to determine the behavior of certain operations when interacting with SQL Server databases.
    /// </summary>
    public static SqlServerOptions Current { get; internal set; } = new();
}
