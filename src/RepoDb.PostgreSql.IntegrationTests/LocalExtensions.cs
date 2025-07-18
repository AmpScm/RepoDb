using Npgsql;
using RepoDb.PostgreSql.IntegrationTests.Enumerations;
using RepoDb.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.PostgreSql.IntegrationTests;
internal static class LocalExtensions
{
    static readonly Lazy<NpgsqlDataSource> setup = new(DoSetup, true);
    public static NpgsqlConnection CreateTestConnection(this object q)
    {
        return setup.Value.CreateConnection();
    }

    static NpgsqlDataSource DoSetup()
    {
        NpgsqlDataSourceBuilder src = new NpgsqlDataSourceBuilder(Database.ConnectionString);

        src.MapEnum<Hands>("hand");
        return src.Build();
    }
}
