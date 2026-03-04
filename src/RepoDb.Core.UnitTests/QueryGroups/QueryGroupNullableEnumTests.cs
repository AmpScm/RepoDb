using RepoDb.Enumerations;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests;

[TestClass]
public class QueryGroupNullableEnumTests
{
    private readonly IDbSetting _dbSetting = new CustomDbSetting();

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    [TestCleanup]
    public void QQ()
    {
        GlobalConfiguration.Setup(new());
    }

    public record Map(Direction Direction, Direction? AdditionalDirection, int N);

    [TestMethod]
    [DataRow(ExpressionNullSemantics.SqlNull)]
    [DataRow(ExpressionNullSemantics.NullNotEqual)]
    public void QueryGroupEnumBad(ExpressionNullSemantics setting)
    {
        GlobalConfiguration.Setup(GlobalConfiguration.Options with { ExpressionNullSemantics = setting });

        var dir = Direction.Up;
        Direction? dir2 = null;
        var v = QueryGroup.Parse<Map>(x => x.Direction == dir && x.AdditionalDirection == dir2);

        Assert.AreEqual(
            setting switch
            {
                ExpressionNullSemantics.SqlNull => "(([Direction] = @Direction) AND ([AdditionalDirection] IS NULL))",
                ExpressionNullSemantics.NullNotEqual => "(([Direction] = @Direction AND [Direction] IS NOT NULL) AND ([AdditionalDirection] IS NULL))",
                _ => null,
            }, v.GetString(_dbSetting));

        v = QueryGroup.Parse<Map>(x => x.N == 12 && !(x.Direction == dir && x.AdditionalDirection == dir2));

        Assert.AreEqual(
            setting switch
            {
                ExpressionNullSemantics.SqlNull => "(([N] = @N) AND NOT ((([Direction] = @Direction) AND ([AdditionalDirection] IS NULL))))",
                ExpressionNullSemantics.NullNotEqual => "(([N] = @N AND [N] IS NOT NULL) AND NOT ((([Direction] = @Direction AND [Direction] IS NOT NULL) AND ([AdditionalDirection] IS NULL))))",
                _ => null
            }, v.GetString(_dbSetting));

        v = QueryGroup.Parse<Map>(x => x.N == 12 && (x.Direction != dir || x.AdditionalDirection != dir2));

        Assert.AreEqual(
            setting switch
            {
                ExpressionNullSemantics.SqlNull => "(([N] = @N) AND (([Direction] <> @Direction) OR ([AdditionalDirection] IS NOT NULL)))",
                ExpressionNullSemantics.NullNotEqual => "(([N] = @N AND [N] IS NOT NULL) AND (([Direction] <> @Direction OR [Direction] IS NULL) OR ([AdditionalDirection] IS NOT NULL)))",
                _ => null
            }, v.GetString(_dbSetting));


        dir2 = Direction.Left;
        v = QueryGroup.Parse<Map>(x => x.N == 12 && !(x.Direction == dir && x.AdditionalDirection == dir2));
        Assert.AreEqual(
            setting switch
            {
                ExpressionNullSemantics.SqlNull => "(([N] = @N) AND NOT ((([Direction] = @Direction) AND ([AdditionalDirection] = @AdditionalDirection))))",
                ExpressionNullSemantics.NullNotEqual => "(([N] = @N AND [N] IS NOT NULL) AND NOT ((([Direction] = @Direction AND [Direction] IS NOT NULL) AND ([AdditionalDirection] = @AdditionalDirection AND [AdditionalDirection] IS NOT NULL))))",
                _ => null
            }, v.GetString(_dbSetting));

        v = QueryGroup.Parse<Map>(x => x.N == 12 && (x.Direction != dir || x.AdditionalDirection != dir2));

        Assert.AreEqual(
            setting switch
            {
                ExpressionNullSemantics.SqlNull => "(([N] = @N) AND (([Direction] <> @Direction) OR ([AdditionalDirection] <> @AdditionalDirection)))",
                ExpressionNullSemantics.NullNotEqual => "(([N] = @N AND [N] IS NOT NULL) AND (([Direction] <> @Direction OR [Direction] IS NULL) OR ([AdditionalDirection] <> @AdditionalDirection OR [AdditionalDirection] IS NULL)))",
                _ => null
            }, v.GetString(_dbSetting));
    }
}
