using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;
using RepoDb.Attributes;
using RepoDb.Attributes.Parameter;
using RepoDb.Schema;
using RepoDb.Trace;

namespace RepoDb.TestCore;

public abstract class VectorTestsBase<TDbInstance> : DbTestBase<TDbInstance> where TDbInstance : DbInstance, new()
{

    public record class VectorDataClass
    {
        [Identity]
        public int Id { get; set; }

        [Size(3)]
        public ReadOnlyMemory<float> FloatVector { get; set; }
    }

    public virtual bool HaveVectorSupport()
    {
        return true;
    }

    [TestMethod]
    public async Task CreateUpdateAndFetchVectorAsMemory()
    {
        if (!HaveVectorSupport())
            return;

        using var sql = await CreateOpenConnectionAsync();

        if (!await sql.SchemaObjectExistsAsync<VectorDataClass>(cancellationToken: TestContext.CancellationToken))
        {
            await sql.CreateTableAsync<VectorDataClass>(trace: new DiagnosticsTracer());
        }
        else
        {
            await sql.TruncateAsync<VectorDataClass>(cancellationToken: TestContext.CancellationToken);
        }


        VectorDataClass[] items = [
            new() { FloatVector = new float[] { 1.0f, 2.0f, 3.0f } },
            new() { FloatVector = new float[] { 4.0f, 5.0f, 6.0f } },
        ];

        await sql.InsertAllAsync(items, trace: new DiagnosticsTracer());

        var data = await sql.QueryAllAsync<VectorDataClass>(cancellationToken: TestContext.CancellationToken);

        Assert.HasCount(2, data);
        Assert.HasCount(3, data.First().FloatVector.ToArray());

        if (sql.GetType().Name.Contains("Oracle") == false)
        {
            // Oracle doesn't support batch update for vector type
            await sql.UpdateAllAsync(items, trace: new DiagnosticsTracer());
        }
    }
}
