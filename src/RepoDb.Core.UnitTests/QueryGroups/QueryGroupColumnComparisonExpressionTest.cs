using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb;
using RepoDb.Enumerations;
using RepoDb.QueryFields;

namespace RepoDb.UnitTests.QueryGroups
{
    [TestClass]
    public class QueryGroupColumnComparisonExpressionTest
    {
        private class TestEntity
        {
            public int ColA { get; set; }
            public int ColB { get; set; }
            public string StrA { get; set; }
            public string StrB { get; set; }
        }

        [TestMethod]
        public void Test_ColumnToColumn_Equal_GeneratesSql()
        {
            var group = QueryGroup.Parse<TestEntity>(x => x.ColA == x.ColB);
            var sql = group.GetString(0, TestDbSetting.Instance);
            Assert.AreEqual("([ColA] = [ColB])", sql);
        }

        [TestMethod]
        public void Test_ColumnToColumn_NotEqual_GeneratesSql()
        {
            var group = QueryGroup.Parse<TestEntity>(x => x.ColA != x.ColB);
            var sql = group.GetString(0, TestDbSetting.Instance);
            Assert.AreEqual("([ColA] <> [ColB])", sql);
        }

        [TestMethod]
        public void Test_ColumnToColumn_LessThan_GeneratesSql()
        {
            var group = QueryGroup.Parse<TestEntity>(x => x.ColA < x.ColB);
            var sql = group.GetString(0, TestDbSetting.Instance);
            Assert.AreEqual("([ColA] < [ColB])", sql);
        }

        [TestMethod]
        public void Test_ColumnToColumn_GreaterThan_GeneratesSql()
        {
            var group = QueryGroup.Parse<TestEntity>(x => x.ColA > x.ColB);
            var sql = group.GetString(0, TestDbSetting.Instance);
            Assert.AreEqual("([ColA] > [ColB])", sql);
        }

        [TestMethod]
        public void Test_ColumnToColumn_LessThanOrEqual_GeneratesSql()
        {
            var group = QueryGroup.Parse<TestEntity>(x => x.ColA <= x.ColB);
            var sql = group.GetString(0, TestDbSetting.Instance);
            Assert.AreEqual("([ColA] <= [ColB])", sql);
        }

        [TestMethod]
        public void Test_ColumnToColumn_GreaterThanOrEqual_GeneratesSql()
        {
            var group = QueryGroup.Parse<TestEntity>(x => x.ColA >= x.ColB);
            var sql = group.GetString(0, TestDbSetting.Instance);
            Assert.AreEqual("([ColA] >= [ColB])", sql);
        }
    }
}
