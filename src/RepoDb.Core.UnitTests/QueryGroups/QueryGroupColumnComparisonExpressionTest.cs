using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.QueryGroups
{
    [TestClass]
    public class QueryGroupColumnComparisonExpressionTest
    {
        private readonly IDbSetting m_dbSetting = new CustomDbSetting();
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
            var sql = group.GetString(0, m_dbSetting);
            Assert.AreEqual("([ColA] = [ColB])", sql);
        }

        [TestMethod]
        public void Test_ColumnToColumn_NotEqual_GeneratesSql()
        {
            var group = QueryGroup.Parse<TestEntity>(x => x.ColA != x.ColB);
            var sql = group.GetString(0, m_dbSetting);
            Assert.AreEqual("([ColA] <> [ColB])", sql);
        }

        [TestMethod]
        public void Test_ColumnToColumn_LessThan_GeneratesSql()
        {
            var group = QueryGroup.Parse<TestEntity>(x => x.ColA < x.ColB);
            var sql = group.GetString(0, m_dbSetting);
            Assert.AreEqual("([ColA] < [ColB])", sql);
        }

        [TestMethod]
        public void Test_ColumnToColumn_GreaterThan_GeneratesSql()
        {
            var group = QueryGroup.Parse<TestEntity>(x => x.ColA > x.ColB);
            var sql = group.GetString(0, m_dbSetting);
            Assert.AreEqual("([ColA] > [ColB])", sql);
        }

        [TestMethod]
        public void Test_ColumnToColumn_LessThanOrEqual_GeneratesSql()
        {
            var group = QueryGroup.Parse<TestEntity>(x => x.ColA <= x.ColB);
            var sql = group.GetString(0, m_dbSetting);
            Assert.AreEqual("([ColA] <= [ColB])", sql);
        }

        [TestMethod]
        public void Test_ColumnToColumn_GreaterThanOrEqual_GeneratesSql()
        {
            var group = QueryGroup.Parse<TestEntity>(x => x.ColA >= x.ColB);
            var sql = group.GetString(0, m_dbSetting);
            Assert.AreEqual("([ColA] >= [ColB])", sql);
        }
    }
}
