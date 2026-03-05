using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Text.Json.Nodes;
using Microsoft.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlTypes;
using RepoDb.SqlServer.IntegrationTests.Setup;
using RepoDb.TestCore;

namespace RepoDb.SqlServer.IntegrationTests.Common;

[TestClass]
public class VectorTests : DbTestBase<SqlServerDbInstance>
{

    protected override void InitializeCore() => Database.Initialize();

    public override DbConnection CreateConnection() => new SqlConnection(Database.ConnectionString);

    class Vectors
    {
        public int Id { get; set; }
        public SqlVector<float> VectorData { get; set; }
    }

    [TestMethod]
    public void RunVectorTest()
    {
        using var connection = (SqlConnection)CreateConnection();

        if (connection.GetDbHelper().GetDbConnectionRuntimeInformation(connection, null) is { } rti
            && rti.EngineVersion.Major < 17)
        {
            return; // Vector support was added with SqlServer 2025
        }

        string tableName = nameof(Vectors);
        var vectorDimensionCount = 3;

        using (var command = connection.CreateCommand($@"
                IF OBJECT_ID('{tableName}', 'U') IS NOT NULL DROP TABLE {tableName};
                IF OBJECT_ID('{tableName}Copy', 'U') IS NOT NULL DROP TABLE {tableName}Copy;"))
        {
            command.ExecuteNonQuery();
        }

        using (var command = connection.CreateCommand($@"
                CREATE TABLE {tableName} (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    VectorData VECTOR({vectorDimensionCount})
                );

                CREATE TABLE {tableName}Copy (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    VectorData VECTOR({vectorDimensionCount})
                );"))
        {
            command.ExecuteNonQuery();
        }

        // Raw insert, like Microsoft sample code
        using (var command = (SqlCommand)connection.CreateCommand($"INSERT INTO {tableName} (VectorData) VALUES (@VectorData)"))
        {
            var param = command.Parameters.Add("@VectorData", SqlDbTypeExtensions.Vector);

            // Insert null using DBNull.Value
            param.Value = DBNull.Value;
            command.ExecuteNonQuery();

            // Insert non-null vector
            param.Value = new SqlVector<float>(new float[] { 3.14159f, 1.61803f, 1.41421f });
            command.ExecuteNonQuery();

            // Insert typed null vector
            param.Value = SqlVector<float>.CreateNull(vectorDimensionCount);
            command.ExecuteNonQuery();

            // Prepare once and reuse for loop
            command.Prepare();
            for (int i = 0; i < 10; i++)
            {
                param.Value = new SqlVector<float>(new float[]
                {
                    i + 0.1f,
                    i + 0.2f,
                    i + 0.3f
                });
                command.ExecuteNonQuery();
            }
        }

        // And do this the RepoDb way
        connection.Insert(new Vectors
        {
            VectorData = new SqlVector<float>(new float[] { 0.1f, 0.2f, 0.3f })
        });

        foreach (var c in connection.QueryAll<Vectors>())
        {
            if (!c.VectorData.IsNull)
            {
                float[] values = c.VectorData.Memory.ToArray();
                Console.WriteLine("VectorData: " + string.Join(", ", values));
            }
            else
            {
                Console.WriteLine("VectorData: NULL");
            }
        }

        foreach (var c in connection.ExecuteQuery<double?>($"SELECT VECTOR_DISTANCE(@how, {nameof(Vectors.VectorData)}, @qv) FROM {nameof(Vectors)}",
            new
            {
                qv = new SqlVector<float>(new float[] { 1, 2, 3 }),
                how = "euclidean"
            })
            )
        {
            Console.WriteLine(c);
        }
    }
}
