using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
using RepoDb.Attributes;
using RepoDb.Extensions;
using RepoDb.Schema;
using RepoDb.StatementBuilders;
using RepoDb.Trace;

namespace RepoDb.TestCore;

public abstract class JsonTestsBase<TDbInstance> : DbTestBase<TDbInstance> where TDbInstance : DbInstance, new()
{

    public virtual string? JsonBColumnType => null;

    public record class JsonTestClass
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

    public record class JsonBTestClass
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

    [Table(nameof(JsonTestClass))]
    public record class JsonTestClassValues
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DbJsonValue<JsonBTestClass> JsonNode { get; set; }
        public DbJsonValue<JsonBTestClass>? Object { get; set; }
        public DbJsonValue<string[]> Array { get; set; }
    }

    [TestMethod]
    public async Task CreateUpdateAndFetchJsonValue()
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

        await sql.InsertAsync(new JsonTestClassValues
        {
            Id = 1,
            Name = "Test",
            JsonNode = new JsonBTestClass { Id = 5 },
            Object = new JsonBTestClass { },
            Array = new string[] { "a", "b" }
        });

        var r = await sql.QueryAllAsync<JsonTestClassValues>(trace: new DiagnosticsTracer());
        Assert.HasCount(1, r);

        r = await sql.QueryAsync<JsonTestClassValues>(v => v.JsonNode.ExtractValue(x => x.Id) == 5, trace: new DiagnosticsTracer());
        Assert.HasCount(1, r);


        r = await sql.QueryAsync<JsonTestClassValues>(v => v.JsonNode.Value.Id == 5, trace: new DiagnosticsTracer());
        Assert.HasCount(1, r);
    }

    [TestMethod]
    public async Task RawExecOnJson()
    {
        using var sql = await CreateOpenConnectionAsync();

        //if (sql.GetType().Name.Contains("pgsql", StringComparison.OrdinalIgnoreCase))
        //    return; // Text insert in json column fails

        if (!await sql.SchemaObjectExistsAsync<JsonTestClass>(cancellationToken: TestContext.CancellationToken))
        {
            await sql.CreateTableAsync<JsonTestClass>(trace: new DiagnosticsTracer());
        }
        else
        {
            await sql.TruncateAsync<JsonTestClass>(cancellationToken: TestContext.CancellationToken);
        }

        DbJsonValue<JsonBTestClass> value;
        value = new JsonBTestClass { Id = 5 };

        await sql.ExecuteNonQueryAsync(sql.ReplaceForTests($@"UPDATE [{nameof(JsonTestClass)}] SET [JsonNode] = @Item"), new { Item = value }, trace: new DiagnosticsTracer(), cancellationToken: TestContext.CancellationToken);
    }

    [TestMethod]
    public async Task RawJsonCompare()
    {
        using var sql = await CreateOpenConnectionAsync();

        if (sql.GetType().Name.Contains("pgsql", StringComparison.OrdinalIgnoreCase))
            return; // Npgsql.PostgresException: 42883: operator does not exist: json = json
        else if (sql.GetType().Name.Contains("oracle", StringComparison.OrdinalIgnoreCase))
            return; // json = json returns false

        if (!await sql.SchemaObjectExistsAsync<JsonTestClass>(cancellationToken: TestContext.CancellationToken))
        {
            await sql.CreateTableAsync<JsonTestClass>(trace: new DiagnosticsTracer());
        }
        else
        {
            await sql.TruncateAsync<JsonTestClass>(cancellationToken: TestContext.CancellationToken);
        }

        DbJsonValue<JsonBTestClass> value;
        value = new JsonBTestClass { Id = 5 };

        await sql.InsertAsync(new JsonTestClassValues
        {
            Id = 1,
            Name = "Test",
            JsonNode = value,
            Object = value,
            Array = new string[] { "a", "b" }
        }, trace: new DiagnosticsTracer());

        Assert.AreEqual(1, await sql.CountAsync<JsonTestClassValues>(where: x => x.Object == value, cancellationToken: TestContext.CancellationToken, trace: new DiagnosticsTracer()));

    }

    class IdValue2Base
    {
        [Identity]
        public int ID { get; set; }
        public string Value { get; set; }
        public string? ValueNull { get; set; }
    }

    struct InnerValueString : IFormattable
#if NET
        , IParsable<InnerValueString>
#endif
    {
        public string Value { get; set; }

        public static InnerValueString Parse(string s, IFormatProvider? provider)
        {
            return new() { Value = s };
        }

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out InnerValueString result)
        {
            try
            {
                result = Parse(s, provider);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return Value ?? "";
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is InnerValueString other && Value == other.Value;
        }

        public override int GetHashCode() => 1;

        public static bool operator ==(InnerValueString left, InnerValueString right) => left.Equals(right);
        public static bool operator !=(InnerValueString left, InnerValueString right) => !left.Equals(right);
    }

    struct InnerClassString : IFormattable
#if NET
        , IParsable<InnerClassString>
#endif
    {
        public string Value { get; set; }

        public static InnerClassString Parse(string s, IFormatProvider? provider)
        {
            return new() { Value = s };
        }

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out InnerClassString result)
        {
            try
            {
                result = Parse(s, provider);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return Value ?? "";
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is InnerValueString other && Value == other.Value;
        }

        public override int GetHashCode() => 1;

        public static bool operator ==(InnerClassString left, InnerClassString right) => left.Equals(right);
        public static bool operator !=(InnerClassString left, InnerClassString right) => !left.Equals(right);
    }


    [Table(nameof(IdValue2Base))]
    record class ValueStructClass
    {
        public int ID { get; set; }
        public InnerValueString Value { get; set; }
        public InnerValueString? ValueNull { get; set; }
    }

    [TestMethod]
    public async Task ValueStructUsage()
    {
        using var sql = await CreateOpenConnectionAsync();

        if (!await sql.SchemaObjectExistsAsync<IdValue2Base>(cancellationToken: TestContext.CancellationToken))
        {
            await sql.CreateTableAsync<IdValue2Base>(trace: new DiagnosticsTracer());
        }
        else
        {
            await sql.TruncateAsync<IdValue2Base>(cancellationToken: TestContext.CancellationToken);
        }


        await sql.InsertAsync(new ValueStructClass
        {
            Value = new() { Value = "Test" },
            ValueNull = new() { Value = "TestNull" }
        });

        await sql.InsertAsync(new ValueStructClass
        {
            Value = new() { Value = "Test2" },
            ValueNull = null
        });

        var r = (await sql.QueryAllAsync<ValueStructClass>(orderBy: [OrderField.Parse<ValueStructClass>(x => x.ID, Enumerations.Order.Ascending)])).AsList();

        Assert.HasCount(2, r);
        Assert.AreEqual("Test", r[0].Value.Value);
        Assert.AreEqual("TestNull", r[0].ValueNull?.Value);
        Assert.AreEqual("Test2", r[1].Value.Value);
        Assert.AreEqual(null, r[1].ValueNull?.Value);

        var r2 = await sql.QueryAsync<ValueStructClass>(x => x.Value == new InnerValueString { Value = "Test" });
    }

    [Table(nameof(IdValue2Base))]
    record class ValueClassClass
    {
        public int ID { get; set; }
        public InnerClassString Value { get; set; }
        public InnerClassString? ValueNull { get; set; }
    }

    [TestMethod]
    public async Task ValueClassUsage()
    {
        using var sql = await CreateOpenConnectionAsync();

        if (!await sql.SchemaObjectExistsAsync<IdValue2Base>(cancellationToken: TestContext.CancellationToken))
        {
            await sql.CreateTableAsync<IdValue2Base>(trace: new DiagnosticsTracer());
        }
        else
        {
            await sql.TruncateAsync<IdValue2Base>(cancellationToken: TestContext.CancellationToken);
        }


        await sql.InsertAsync(new ValueClassClass
        {
            Value = new() { Value = "Test" },
            ValueNull = new() { Value = "TestNull" }
        });

        await sql.InsertAsync(new ValueClassClass
        {
            Value = new() { Value = "Test2" },
            ValueNull = null
        });

        var r = (await sql.QueryAllAsync<ValueClassClass>(orderBy: [OrderField.Parse<ValueClassClass>(x => x.ID, Enumerations.Order.Ascending)])).AsList();

        Assert.HasCount(2, r);
        Assert.AreEqual("Test", r[0].Value.Value);
        Assert.AreEqual("TestNull", r[0].ValueNull?.Value);
        Assert.AreEqual("Test2", r[1].Value.Value);
        Assert.AreEqual(null, r[1].ValueNull?.Value);

        var r2 = await sql.QueryAsync<ValueClassClass>(x => x.Value == new InnerClassString { Value = "Test" }, trace: new DiagnosticsTracer());
    }
}
