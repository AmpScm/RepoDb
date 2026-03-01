using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Identity.Client;

namespace RepoDb.UnitTests;

public partial class QueryGroupTest
{
    public record class ForJsonEntity
    {
        public int Id { get; set; }
        public DbJsonValue<JsonData> Data { get; set; }
        public DbJsonValue<JsonData>? DataNull { get; set; }
    }

    public record class JsonData
    {
        public int? Nr { get; set; }
        public string? Name { get; set; }
        public string[]? Values { get; set; }

        public JsonData? Inner { get; set; }
    }


    [TestMethod]
    public void TestQueryGroupParseExpressionJsonExtract()
    {
        var actual = QueryGroup.Parse<ForJsonEntity>(e => e.Data.ExtractValue(x => x.Name) == "a");
        Assert.AreEqual("(JSON_EXTRACT([Data], '$.Name') = @Data)", actual.GetString(m_dbSetting));
    }


    [TestMethod]
    public void TestQueryGroupParseExpressionJsonValue()
    {
        var actual = QueryGroup.Parse<ForJsonEntity>(e => e.Data.Value.Name == "a");
        Assert.AreEqual("(JSON_EXTRACT([Data], '$.Name') = @Data)", actual.GetString(m_dbSetting));
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionJsonExtract2()
    {
        var actual = QueryGroup.Parse<ForJsonEntity>(e => e.Data.ExtractValue(x => x.Name) == "a" && e.Data.ExtractValue(x => x.Nr) == 7);
        Assert.AreEqual("((JSON_EXTRACT([Data], '$.Name') = @Data) AND (JSON_EXTRACT([Data], '$.Nr') = @Data_1))", actual.GetString(m_dbSetting));
    }


    [TestMethod]
    public void TestQueryGroupParseExpressionJsonValue2()
    {
        var actual = QueryGroup.Parse<ForJsonEntity>(e => e.Data.Value.Name == "a" && e.Data.Value.Nr == 2);
        Assert.AreEqual("((JSON_EXTRACT([Data], '$.Name') = @Data) AND (JSON_EXTRACT([Data], '$.Nr') = @Data_1))", actual.GetString(m_dbSetting));
    }

    [TestMethod]
    public void TestQueryGroupParseExpressionJsonExtractDeep()
    {
        var actual = QueryGroup.Parse<ForJsonEntity>(e => e.Data.ExtractValue(x => x.Values[0]) == "a" && e.Data.ExtractValue(x => x.Inner.Inner.Nr) == 7);
        Assert.AreEqual("((JSON_EXTRACT([Data], '$.Values[0]') = @Data) AND (JSON_EXTRACT([Data], '$.Inner.Inner.Nr') = @Data_1))", actual.GetString(m_dbSetting));
    }


    [TestMethod]
    public void TestQueryGroupParseExpressionJsonValueDeep2()
    {
        var actual = QueryGroup.Parse<ForJsonEntity>(e => e.Data.Value.Values[0] == "a" && e.Data.Value.Inner.Inner.Nr == 2);
        Assert.AreEqual("((JSON_EXTRACT([Data], '$.Values[0]') = @Data) AND (JSON_EXTRACT([Data], '$.Inner.Inner.Nr') = @Data_1))", actual.GetString(m_dbSetting));
    }

}
