using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Nodes;
using RepoDb.DbSettings;
using RepoDb.Schema;
using RepoDb.StatementBuilders;
using RepoDb.Trace;

namespace RepoDb.TestCore;

public abstract class JsonTestsBase<TDbInstance> : DbTestBase<TDbInstance> where TDbInstance : DbInstance, new()
{

    public virtual string? JsonBColumnType => null;

    public class JsonTestClass
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public JsonNode? JsonNode { get; set; }
        public JsonObject? Object { get; set; }
        public JsonArray? Array { get; set; }
    }

    [TestMethod]
    public async Task CreateUpdateAndFetchJson()
    {
        using var sql = await CreateOpenConnectionAsync();

        if (!await sql.SchemaObjectExistsAsync<JsonTestClass>(cancellationToken: TestContext.CancellationToken))
        {
            await sql.CreateTableAsync<JsonTestClass>(trace: new DiagnosticsTracer());
        }
        else
        {
            await sql.TruncateAsync<JsonTestClass>(cancellationToken: TestContext.CancellationToken);
        }


        await sql.InsertAsync(new JsonTestClass
        {
            Id = 1,
            Name = "Test",
            JsonNode = new JsonObject
            {
                ["Key"] = "Value"
            },
            Object = new JsonObject
            {
                ["Key"] = "Value"
            },
            Array = new JsonArray
            {
                "Value1",
                "Value2"
            }
        }, cancellationToken: TestContext.CancellationToken);

        await sql.UpdateAsync(new JsonTestClass
        {
            Id = 1,
            Name = "Test Updated",
            JsonNode = new JsonObject
            {
                ["Key"] = "NewValue"
            },
            Object = new JsonObject
            {
                ["Key"] = "NewValue"
            },
            Array = new JsonArray
            {
                "NewValue1",
                "NewValue2"
            }
        }, trace: new DiagnosticsTracer(), cancellationToken: TestContext.CancellationToken);


        var result = await sql.QueryAllAsync<JsonTestClass>(cancellationToken: TestContext.CancellationToken);


        Assert.HasCount(1, result);
        Assert.IsNotNull(result.First().JsonNode);
        Assert.AreEqual(@"{""Key"":""NewValue""}", result.First().JsonNode.ToJsonString(Converter.JsonSerializerOptions));
    }


    class JsonPerson
    {
        public string name { get; set; }
        public int age { get; set; }
    }

    [TestMethod]
    public async Task QueryJson()
    {
        using var sql = await CreateOpenConnectionAsync();

        if (!await sql.SchemaObjectExistsAsync<JsonTestClass>(cancellationToken: TestContext.CancellationToken))
        {
            await sql.CreateTableAsync<JsonTestClass>(trace: new DiagnosticsTracer());
        }
        else
        {
            await sql.TruncateAsync<JsonTestClass>(cancellationToken: TestContext.CancellationToken);
        }


        await sql.InsertAsync(new JsonTestClass
        {
            Name = "Test",
            JsonNode = new JsonObject
            {
                ["name"] = "Jaap",
                ["age"] = 48
            },
            Object = new JsonObject
            {
                ["Key"] = "Value"
            },
            Array = new JsonArray
            {
                "Value1",
                "Value2"
            }
        }, cancellationToken: TestContext.CancellationToken);


        var r = await sql.QueryAsync<JsonTestClass>(where: x => x.JsonNode.ExtractValue<string>("name") == "Jaap", trace: new DiagnosticsTracer());
        Assert.HasCount(1, r);


        r = await sql.QueryAsync<JsonTestClass>(where: x => x.JsonNode.ExtractValue<int>("age") >= 45, trace: new DiagnosticsTracer());
        Assert.HasCount(1, r);

        r = await sql.QueryAsync<JsonTestClass>(where: x => x.Array.ExtractValue<string>("[0]") == "Value1", trace: new DiagnosticsTracer());
        Assert.HasCount(1, r);

        r = await sql.QueryAsync<JsonTestClass>(where: x => x.JsonNode.ExtractValue((JsonPerson p) => p.age) >= 45, trace: new DiagnosticsTracer());
        Assert.HasCount(1, r);

        r = await sql.QueryAsync<JsonTestClass>(where: x => x.Array.ExtractValue((string[] v) => v[0]) == "Value1", trace: new DiagnosticsTracer());
        Assert.HasCount(1, r);
    }

    class JsonBTestClass
    {
        public int Id { get; set; }
        public JsonNode? JsonNode { get; set; }
    }

    [TestMethod]
    public async Task JsonBinaryTests()
    {
        using var sql = await CreateOpenConnectionAsync();

        if (!await sql.SchemaObjectExistsAsync<JsonBTestClass>(cancellationToken: TestContext.CancellationToken))
        {
            await PerformCreateTableAsync(sql, $@"CREATE TABLE [{nameof(JsonBTestClass)}] (
                [Id] int NOT NULL,
                [JsonNode] {JsonBColumnType ?? ((BaseStatementBuilder)sql.GetStatementBuilder()).JsonColumnType ?? "TEXT"}
            )");
        }
        else
        {
            await sql.TruncateAsync<JsonBTestClass>(cancellationToken: TestContext.CancellationToken);
        }

        await sql.InsertAsync(new JsonBTestClass
        {
            Id = 1,
            JsonNode = new JsonObject
            {
                ["Key"] = "Value"
            }
        }, cancellationToken: TestContext.CancellationToken);


        var r = await sql.QueryAsync<JsonBTestClass>(where: x => x.JsonNode.ExtractValue<string>("Key") == "Value", trace: new DiagnosticsTracer());
        Assert.HasCount(1, r);
        Assert.AreEqual("{\"Key\":\"Value\"}", r.First().JsonNode.ToJsonString(Converter.JsonSerializerOptions));
    }
}
