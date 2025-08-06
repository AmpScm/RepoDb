using RepoDb.Enumerations;
using RepoDb.Extensions.QueryFields;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.QueryGroups
{
    [TestClass]
    public class FieldComparisonQueryFieldTest
    {
        private readonly IDbSetting _dbSetting = new CustomDbSetting();

        [TestMethod]
        public void Test_FieldComparisonQueryField_Equals()
        {
            var left = new Field("ColumnA");
            var right = new Field("ColumnB");
            var qf = new FieldComparisonQueryField(left, Operation.Equal, right);
            var sql = qf.GetString(_dbSetting);
            Assert.AreEqual("[ColumnA] = [ColumnB]", sql);
        }

        [TestMethod]
        public void Test_FieldComparisonQueryField_NotEquals()
        {
            var left = new Field("ColumnA");
            var right = new Field("ColumnB");
            var qf = new FieldComparisonQueryField(left, Operation.NotEqual, right);
            var sql = qf.GetString(_dbSetting);
            Assert.AreEqual("[ColumnA] <> [ColumnB]", sql);
        }

        [TestMethod]
        public void Test_FieldComparisonQueryField_LessThan()
        {
            var left = new Field("ColumnA");
            var right = new Field("ColumnB");
            var qf = new FieldComparisonQueryField(left, Operation.LessThan, right);
            var sql = qf.GetString(_dbSetting);
            Assert.AreEqual("[ColumnA] < [ColumnB]", sql);
        }

        [TestMethod]
        public void Test_FieldComparisonQueryField_GreaterThan()
        {
            var left = new Field("ColumnA");
            var right = new Field("ColumnB");
            var qf = new FieldComparisonQueryField(left, Operation.GreaterThan, right);
            var sql = qf.GetString(_dbSetting);
            Assert.AreEqual("[ColumnA] > [ColumnB]", sql);
        }

        [TestMethod]
        public void Test_FieldComparisonQueryField_LessThanOrEqual()
        {
            var left = new Field("ColumnA");
            var right = new Field("ColumnB");
            var qf = new FieldComparisonQueryField(left, Operation.LessThanOrEqual, right);
            var sql = qf.GetString(_dbSetting);
            Assert.AreEqual("[ColumnA] <= [ColumnB]", sql);
        }

        [TestMethod]
        public void Test_FieldComparisonQueryField_GreaterThanOrEqual()
        {
            var left = new Field("ColumnA");
            var right = new Field("ColumnB");
            var qf = new FieldComparisonQueryField(left, Operation.GreaterThanOrEqual, right);
            var sql = qf.GetString(_dbSetting);
            Assert.AreEqual("[ColumnA] >= [ColumnB]", sql);
        }
    }
}
