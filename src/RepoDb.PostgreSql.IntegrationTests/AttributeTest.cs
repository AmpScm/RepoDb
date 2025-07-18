using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using NpgsqlTypes;
using RepoDb.Attributes.Parameter.Npgsql;
using RepoDb.Extensions;
using RepoDb.PostgreSql.IntegrationTests.Setup;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RepoDb.PostgreSql.IntegrationTests;

[TestClass]
public class AttributeTest
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

    #region Classes

    [Table("CompleteTable")]
    public class AttributeTable
    {
        public int Id { get; set; }

        [NpgsqlDbType(NpgsqlDbType.Bytea)]
        public byte[] ColumnByteA { get; set; }

        [NpgsqlDbType(NpgsqlDbType.Bigint)]
        public long ColumnBigInt { get; set; }

        [NpgsqlDbType(NpgsqlDbType.Date)]
        public DateTime ColumnDate { get; set; }

        [NpgsqlDbType(NpgsqlDbType.Text)]
        public string ColumnText { get; set; }
    }

    #endregion

    #region Helpers

    private IEnumerable<AttributeTable> CreateAttributeTables(int count = 10)
    {
        Random random = new Random();
        for (int i = 0; i < count; i++)
        {
            yield return new AttributeTable
            {
                Id = i,
                ColumnBigInt = Convert.ToInt64(random.Next(int.MaxValue)),
                ColumnByteA = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()),
                ColumnDate = DateTime.UtcNow.Date.AddDays(-random.Next(100)),
                ColumnText = $"ColumnNVarChar-{i}-{Guid.NewGuid()}"
            };
        }
    }

    #endregion

    #region Methods

    [TestMethod]
    public void TestNpgsqlConnectionForInsertForNpgsqlTypeMapAttribute()
    {
        // Setup
        AttributeTable table = CreateAttributeTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        connection.Insert<AttributeTable>(table);

        // Assert
        Assert.AreEqual(1, connection.CountAll<AttributeTable>());

        // Query
        AttributeTable queryResult = connection.QueryAll<AttributeTable>().First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public void TestNpgsqlConnectionForInsertAllForNpgsqlTypeMapAttribute()
    {
        // Setup
        List<AttributeTable> tables = CreateAttributeTables(10).AsList();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        connection.InsertAll<AttributeTable>(tables);

        // Assert
        Assert.AreEqual(tables.Count, connection.CountAll<AttributeTable>());

        // Query
        IEnumerable<AttributeTable> queryResult = connection.QueryAll<AttributeTable>();

        // Assert
        tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
    }

    [TestMethod]
    public void TestNpgsqlConnectionForQueryForNpgsqlTypeMapAttribute()
    {
        // Setup
        AttributeTable table = CreateAttributeTables(1).First();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        object id = connection.Insert<AttributeTable>(table);

        // Query
        AttributeTable queryResult = connection.Query<AttributeTable>(id).First();

        // Assert
        Helper.AssertPropertiesEquality(table, queryResult);
    }

    [TestMethod]
    public void TestNpgsqlConnectionForQueryAllForNpgsqlTypeMapAttribute()
    {
        // Setup
        List<AttributeTable> tables = CreateAttributeTables(10).AsList();

        using NpgsqlConnection connection = this.CreateTestConnection();
        // Act
        connection.InsertAll<AttributeTable>(tables);

        // Query
        IEnumerable<AttributeTable> queryResult = connection.QueryAll<AttributeTable>();

        // Assert
        Assert.AreEqual(tables.Count, queryResult.Count());
    }

    #endregion
}
