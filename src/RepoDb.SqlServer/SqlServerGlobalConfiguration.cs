using Microsoft.Data.SqlClient;
using RepoDb.Options;

namespace RepoDb;

/// <summary>
/// A class that is being used to initialize the necessary settings for the <see cref="SqlConnection"/> object.
/// </summary>
public static partial class SqlServerGlobalConfiguration
{
    /// <summary>
    /// Initializes all the necessary settings for SQL Server.
    /// </summary>
    /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
    /// <returns>The used global configuration instance itself.</returns>
    public static GlobalConfiguration UseSqlServer(this GlobalConfiguration globalConfiguration)
    {
        return UseSqlServer(globalConfiguration, null);
    }

    public static GlobalConfiguration UseSqlServer(this GlobalConfiguration globalConfiguration,
                                                    SqlServerOptions? options = null)
    {
        SqlServerBootstrap.InitializeInternal();
        if (options is { })
        {
            SqlServerOptions.Current = options;
        }
        return globalConfiguration;
    }
}
