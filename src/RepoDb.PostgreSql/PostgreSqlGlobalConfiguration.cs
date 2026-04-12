using Npgsql;

namespace RepoDb;

/// <summary>
/// A class that is being used to initialize the necessary settings for the <see cref="NpgsqlConnection"/> object.
/// </summary>
public static partial class PostgreSqlGlobalConfiguration
{
    /// <summary>
    /// Initializes all the necessary settings for PostgreSql.
    /// </summary>
    /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
    /// <returns>The used global configuration instance itself.</returns>
    public static GlobalConfiguration UsePostgreSql(this GlobalConfiguration globalConfiguration)
    {
        PostgreSqlBootstrap.InitializeInternal();
        return globalConfiguration;
    }

    /// <summary>
    /// Registers all NodaTime types as passthrough so that RepoDb does not
    /// attempt to convert them. Npgsql handles these natively when the
    /// data source is configured with UseNodaTime().
    /// Call after UsePostgreSql():
    /// <code>GlobalConfiguration.Setup().UsePostgreSql().UseNodaTime();</code>
    /// </summary>
    /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
    /// <returns>The used global configuration instance itself.</returns>
    public static GlobalConfiguration UseNodaTime(this GlobalConfiguration globalConfiguration)
    {
        // All NodaTime types that Npgsql maps via UseNodaTime():
        // https://www.npgsql.org/doc/types/nodatime.html
        // Resolved by name to avoid a hard dependency on the NodaTime package.
        var nodaTimeTypes = new[]
        {
            "NodaTime.Instant",
            "NodaTime.LocalDate",
            "NodaTime.LocalTime",
            "NodaTime.LocalDateTime",
            "NodaTime.ZonedDateTime",
            "NodaTime.OffsetDateTime",
            "NodaTime.OffsetTime",
            "NodaTime.Period",
            "NodaTime.Duration",
            "NodaTime.DateInterval",
            "NodaTime.Interval",
        };

        foreach (var typeName in nodaTimeTypes)
        {
            var type = Type.GetType($"{typeName}, NodaTime");
            if (type is not null)
            {
                TypeMapper.AddPassthrough(type);
            }
        }

        return globalConfiguration;
    }
}
