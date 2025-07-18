using Npgsql;
using RepoDb.IntegrationTests.Setup;
using RepoDb.PostgreSql.BulkOperations.IntegrationTests.Enumerations;

namespace RepoDb.PostgreSql.BulkOperations.IntegrationTests;

internal static class LocalExtensions
{
    private static readonly Lazy<NpgsqlDataSource> setup = new(DoSetup, true);
    public static NpgsqlConnection CreateTestConnection(this object q)
    {
        return setup.Value.CreateConnection();
    }

    private static NpgsqlDataSource DoSetup()
    {
        var src = new NpgsqlDataSourceBuilder(Database.ConnectionStringForRepoDb);

        src.MapEnum<Hands>("hand");
        return src.Build();
    }
}
