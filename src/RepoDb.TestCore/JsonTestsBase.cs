using System.Text.Json;
using System.Text.Json.Nodes;
using RepoDb.Schema;
using RepoDb.Trace;

namespace RepoDb.TestCore;

public abstract class JsonTestsBase<TDbInstance> : DbTestBase<TDbInstance> where TDbInstance : DbInstance, new()
{
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
}
