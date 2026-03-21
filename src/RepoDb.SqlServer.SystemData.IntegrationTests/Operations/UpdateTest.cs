using System.Data.SqlClient;
using RepoDb.SqlServer.SystemData.IntegrationTests.Models;
using RepoDb.SqlServer.SystemData.IntegrationTests.Setup;
using RepoDb.Trace;

namespace RepoDb.SqlServer.SystemData.IntegrationTests.Operations;

[TestClass]
public class UpdateTest : TestBase
{

    [TestMethod]
    public void TestSqlServerConnectionUpdateViaDataEntity()
    {
        // Setup
        var table = Database.CreateCompleteTables(1).First();

        using var connection = new SqlConnection(Database.ConnectionString);

        RoundTimes(table);

        // Setup
        Helper.UpdateCompleteTableProperties(table);

        RoundTimes(table);

        // Act
        var result = connection.Update<CompleteTable>(table);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    private void RoundTimes(CompleteTable table)
    {
        table.ColumnTime -= TimeSpan.FromMicroseconds(table.ColumnTime.Microseconds) + TimeSpan.FromMilliseconds(table.ColumnTime.Milliseconds);
        table.ColumnDateTime2 -= TimeSpan.FromMicroseconds(table.ColumnDateTime2.TimeOfDay.Microseconds) + TimeSpan.FromMilliseconds(table.ColumnDateTime2.TimeOfDay.Milliseconds);
    }

    [TestMethod]
    public void TestSqlServerConnectionUpdateViaExpression()
    {
        // Setup
        var table = Database.CreateCompleteTables(1).First();

        using var connection = new SqlConnection(Database.ConnectionString);

        RoundTimes(table);
        // Setup
        Helper.UpdateCompleteTableProperties(table);

        RoundTimes(table);
        // Act
        var result = connection.Update<CompleteTable>(table, e => e.Id == table.Id);

        // Assert
        Assert.AreEqual(1, result);

        // Act
        var queryResult = connection.Query<CompleteTable>(table.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }
}
