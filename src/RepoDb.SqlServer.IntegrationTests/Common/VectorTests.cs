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
public class VectorTests : VectorTestsBase<SqlServerDbInstance>
{

    protected override void InitializeCore() => Database.Initialize();

    public override bool HaveVectorSupport()
    {
        using var sql = CreateConnection();

        var info = sql.GetDbRuntimeSetting();

        return info?.EngineVersion.Major >= 17; // Vector support was added with SqlServer 2025
    }

    class Vectors
    {
        public int Id { get; set; }
        public SqlVector<float> VectorData { get; set; }
    }

    [TestMethod]
    public void RunVectorTest()
    {
        if (!HaveVectorSupport())
            return;

        using var connection = (SqlConnection)CreateConnection();

        connection.EnsureOpen();

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

    [TestMethod]
    public void VectorMetaData()
    {
        if (!HaveVectorSupport())
            return;

        //class Vectors
        //{
        //    public int Id { get; set; }
        //    public SqlVector<float> VectorData { get; set; }
        //}

        using var connection = (SqlConnection)CreateConnection();

        connection.EnsureOpen();

        var vectorDimensionCount = 3;

        using (var command = connection.CreateCommand())
        {
            command.CommandText = $@" IF OBJECT_ID('{nameof(Vectors)}', 'U') IS NOT NULL DROP TABLE {nameof(Vectors)};";
            command.ExecuteNonQuery();
        }

        using (var command = connection.CreateCommand())
        {
            command.CommandText = $@"
                CREATE TABLE {nameof(Vectors)} (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    VectorData VECTOR({vectorDimensionCount})
                );";
            command.ExecuteNonQuery();
        }

        // Raw insert, like Microsoft sample code
        using (var command = connection.CreateCommand())
        {
            command.CommandText = $@"INSERT INTO {nameof(Vectors)} (VectorData) VALUES (@VectorData)";
            var param = command.Parameters.Add("@VectorData", SqlDbTypeExtensions.Vector);

            // Insert non-null vector
            param.Value = new SqlVector<float>(new float[] { 3.14159f, 1.61803f, 1.41421f });
            command.ExecuteNonQuery();
        }

        using (var command = connection.CreateCommand())
        {
            command.CommandText = $@"SELECT Id, VectorData FROM {nameof(Vectors)}";
            using var reader = command.ExecuteReader();

            Assert.AreEqual(typeof(int), reader.GetFieldType(0));
            Assert.AreEqual(typeof(byte[]), reader.GetFieldType(1)); // This should return SqlVector<float>, but currently returns byte[] due to a bug in SqlClient

            while (reader.Read())
            {
                var value = reader.GetValue(1);

                Assert.IsInstanceOfType<SqlVector<float>>(value); // This succeeds, so the library is correctly identifying the type as SqlVector<float> and returning it as such, but the GetFieldType method is incorrectly reporting it as byte[] due to a bug in SqlClient

                var id = reader.GetFieldValue<int>(0);
                var dt = reader.GetFieldValue<SqlVector<float>>(1);

                Assert.ThrowsExactly<InvalidCastException>(() => reader.GetFieldValue<byte[]>(1));


                var dt2 = reader.GetSqlVector<float>(1);
            }
        }
    }
}
