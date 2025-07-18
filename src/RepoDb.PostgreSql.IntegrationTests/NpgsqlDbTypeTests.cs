using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using NpgsqlTypes;
using RepoDb.Attributes;
using RepoDb.Attributes.Parameter.Npgsql;
using RepoDb.Extensions;
using RepoDb.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.PostgreSql.IntegrationTests;

[TestClass]
public class NpgsqlDbTypeTests
{
    [TestInitialize]
    public void Initialize()
    {
        Database.Initialize();
        Cleanup();
    }

    [TestCleanup]
    public void Cleanup()
    {
        Database.Cleanup();
    }

    #region SubClasses

    [Map("CompleteTable")]
    private class CompleteTableForJson
    {
        public System.Int64 Id { get; set; }
        [NpgsqlDbType(NpgsqlDbType.Json)]
        public System.String ColumnJson { get; set; }
    }

    [Map("CompleteTable")]
    private class CompleteTableForDateTime
    {
        public System.Int64 Id { get; set; }
        [NpgsqlDbType(NpgsqlDbType.TimestampTz)]
        public System.DateTimeOffset ColumnTimestampWithTimeZone { get; set; }
        [NpgsqlDbType(NpgsqlDbType.Timestamp)]
        public System.DateTime ColumnTimestampWithoutTimeZone { get; set; }
    }

    #endregion

    #region Helpers

    private IEnumerable<CompleteTableForJson> GetCompleteTableForJsons(int count = 0)
    {
        for (int i = 0; i < count; i++)
        {
            yield return new CompleteTableForJson
            {
                Id = 1,
                ColumnJson = $"{{\"Id\": {i}, \"Field1\": \"Field1Value\", \"Field2\": \"Field2Value\"}}"
            };
        }
    }

    private IEnumerable<CompleteTableForDateTime> GetCompleteTableForDateTimes(int count = 0)
    {
        Random random = new Random();
        for (int i = 0; i < count; i++)
        {
            yield return new CompleteTableForDateTime
            {
                Id = 1,
                ColumnTimestampWithTimeZone = DateTimeOffset.Now.Date.AddSeconds(random.Next(60)).ToUniversalTime(),
                ColumnTimestampWithoutTimeZone = DateTime.Now.Date.AddSeconds(random.Next(60))
            };
        }
    }

    #endregion

    #region JSON

    [TestMethod]
    public void TestInsertAndQueryForJson()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        CompleteTableForJson entity = GetCompleteTableForJsons(1).First();

        // Act
        connection.Insert(entity);

        // Act
        CompleteTableForJson queryResult = connection.Query<CompleteTableForJson>(entity.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(entity, queryResult);
    }

    [TestMethod]
    public void TestInsertAndQueryForJsons()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        List<CompleteTableForJson> entities = GetCompleteTableForJsons(10).AsList();

        // Act
        connection.InsertAll(entities);

        // Act
        IEnumerable<CompleteTableForJson> queryResult = connection.QueryAll<CompleteTableForJson>();

        // Assert
        entities.ForEach(e =>
            Helper.AssertPropertiesEquality(e, queryResult.First(item => item.Id == e.Id)));
    }

    #endregion

    #region DateTime

    [TestMethod]
    public void TestInsertAndQueryForDateTime()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        CompleteTableForDateTime entity = GetCompleteTableForDateTimes(1).First();

        // Act
        connection.Insert(entity);

        // Act
        CompleteTableForDateTime queryResult = connection.Query<CompleteTableForDateTime>(entity.Id).First();

        // Assert
        Helper.AssertPropertiesEquality(entity, queryResult);
    }

    [TestMethod]
    public void TestInsertAndQueryForDateTimes()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        List<CompleteTableForDateTime> entities = GetCompleteTableForDateTimes(10).AsList();

        // Act
        connection.InsertAll(entities);

        // Act
        IEnumerable<CompleteTableForDateTime> queryResult = connection.QueryAll<CompleteTableForDateTime>();

        // Assert
        entities.ForEach(e =>
            Helper.AssertPropertiesEquality(e, queryResult.First(item => item.Id == e.Id)));
    }

    [TestMethod]
    public void TestInsertAndQueryForDateTimeAsWhereExpression()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        CompleteTableForDateTime entity = GetCompleteTableForDateTimes(1).First();

        // Act
        connection.Insert(entity);

        // Setup
        DateTimeOffset startDate = DateTimeOffset.Now.Date.AddHours(-5).ToUniversalTime();
        DateTimeOffset endDate = DateTimeOffset.Now.Date.AddHours(5).ToUniversalTime();

        // Act
        CompleteTableForDateTime queryResult = connection.Query<CompleteTableForDateTime>(e =>
            e.ColumnTimestampWithTimeZone >= startDate && e.ColumnTimestampWithTimeZone <= endDate).FirstOrDefault();

        // Assert
        Helper.AssertPropertiesEquality(entity, queryResult);
    }

    [TestMethod]
    public void TestInsertAndQueryForDateTimeAsWhereExpressionFromVariable()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        CompleteTableForDateTime entity = GetCompleteTableForDateTimes(1).First();

        // Act
        connection.Insert(entity);

        // Setup
        DateTimeOffset startDate = DateTimeOffset.Now.Date.AddHours(-5).ToUniversalTime();
        DateTimeOffset endDate = DateTimeOffset.Now.Date.AddHours(5).ToUniversalTime();

        // Act
        CompleteTableForDateTime queryResult = connection.Query<CompleteTableForDateTime>(e =>
            e.ColumnTimestampWithTimeZone >= startDate && e.ColumnTimestampWithTimeZone <= endDate).FirstOrDefault();

        // Assert
        Helper.AssertPropertiesEquality(entity, queryResult);
    }

    [TestMethod]
    public void TestInsertAndQueryForDateTimeAsWhereExpressionWithAutomaticConversion()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        CompleteTableForDateTime entity = GetCompleteTableForDateTimes(1).First();

        // Act
        connection.Insert(entity);

        // Setup
        DateTimeOffset startDate = DateTimeOffset.Now.Date.AddHours(-5).ToUniversalTime();
        DateTimeOffset endDate = DateTimeOffset.Now.Date.AddHours(5).ToUniversalTime();

        // Act
        CompleteTableForDateTime queryResult = connection.Query<CompleteTableForDateTime>(e =>
            e.ColumnTimestampWithTimeZone >= startDate && e.ColumnTimestampWithTimeZone <= endDate).FirstOrDefault();

        // Assert
        Helper.AssertPropertiesEquality(entity, queryResult);
    }

    [TestMethod]
    public void TestInsertAndQueryForDateTimeAsWhereExpressionFromVariableWithAutomaticConversion()
    {
        using NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        CompleteTableForDateTime entity = GetCompleteTableForDateTimes(1).First();

        // Act
        connection.Insert(entity);

        // Setup
        DateTimeOffset startDate = DateTimeOffset.Now.Date.AddHours(-5).ToUniversalTime();
        DateTimeOffset endDate = DateTimeOffset.Now.Date.AddHours(5).ToUniversalTime();

        // Act
        CompleteTableForDateTime queryResult = connection.Query<CompleteTableForDateTime>(e =>
            e.ColumnTimestampWithTimeZone >= startDate && e.ColumnTimestampWithTimeZone <= endDate).FirstOrDefault();

        // Assert
        Helper.AssertPropertiesEquality(entity, queryResult);
    }

    #endregion
}
