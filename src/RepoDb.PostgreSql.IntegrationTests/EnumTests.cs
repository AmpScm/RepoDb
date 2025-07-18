using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Extensions;
using RepoDb.PostgreSql.IntegrationTests.Enumerations;
using RepoDb.PostgreSql.IntegrationTests.Setup;
using RepoDb.Trace;

namespace RepoDb.PostgreSql.IntegrationTests;

[TestClass]
public class EnumTests
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

    #region Enumerations

    #endregion

    #region SubClasses

    [Map("CompleteTable")]
    public class PersonWithText
    {
        public System.Int64 Id { get; set; }
        //[NpgsqlDbType(NpgsqlDbType.Varchar)]

        public Hands? ColumnText { get; set; }
    }

    [Map("CompleteTable")]
    public class PersonWithInteger
    {
        public System.Int64 Id { get; set; }
        public Hands? ColumnInteger { get; set; }
    }

    [Map("CompleteTable")]
    public class PersonWithTextAsInteger
    {
        public System.Int64 Id { get; set; }
        [TypeMap(System.Data.DbType.Int32)]
        public Hands? ColumnText { get; set; }
    }

    [Map("EnumTable")]
    public class PersonWithEnum
    {
        public System.Int64 Id { get; set; }
        public Hands ColumnEnumHand { get; set; }
    }

    [Map("EnumTable")]
    public class PersonWithNullableEnum
    {
        public System.Int64 Id { get; set; }
        public Hands? ColumnEnumHand { get; set; }
    }

    #endregion

    #region Helpers

    public IEnumerable<PersonWithText> GetPersonWithText(int count)
    {
        Random random = new Random();
        for (int i = 0; i < count; i++)
        {
            Hands hand = random.Next(100) > 50 ? Hands.Right : Hands.Left;
            yield return new PersonWithText
            {
                Id = i,
                ColumnText = hand
            };
        }
    }

    public IEnumerable<PersonWithInteger> GetPersonWithInteger(int count)
    {
        Random random = new Random();
        for (int i = 0; i < count; i++)
        {
            Hands hand = random.Next(100) > 50 ? Hands.Right : Hands.Left;
            yield return new PersonWithInteger
            {
                Id = i,
                ColumnInteger = hand
            };
        }
    }

    public IEnumerable<PersonWithTextAsInteger> GetPersonWithTextAsInteger(int count)
    {
        Random random = new Random();
        for (int i = 0; i < count; i++)
        {
            Hands hand = random.Next(100) > 50 ? Hands.Right : Hands.Left;
            yield return new PersonWithTextAsInteger
            {
                Id = i,
                ColumnText = hand
            };
        }
    }

    public IEnumerable<PersonWithEnum> GetPersonWithEnum(int count)
    {
        Random random = new Random();
        for (int i = 0; i < count; i++)
        {
            Hands hand = random.Next(100) > 50 ? Hands.Right : Hands.Left;
            yield return new PersonWithEnum
            {
                Id = i,
                ColumnEnumHand = hand
            };
        }
    }

    public IEnumerable<PersonWithNullableEnum> GetPersonWithNullableEnum(int count)
    {
        Random random = new Random();
        for (int i = 0; i < count; i++)
        {
            Hands hand = random.Next(100) > 50 ? Hands.Right : Hands.Left;
            yield return new PersonWithNullableEnum
            {
                Id = i,
                ColumnEnumHand = hand
            };
        }
    }

    #endregion

    [TestMethod]
    public void TestInsertAndQueryEnumAsTextAsNull()
    {
        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        PersonWithText person = GetPersonWithText(1).First();
        person.ColumnText = null;

        // Act
        connection.Insert(person);

        // Query
        PersonWithText queryResult = connection.Query<PersonWithText>(person.Id).First();

        // Assert
        Assert.IsNull(queryResult.ColumnText);
    }

    [TestMethod]
    public void TestInsertAndQueryEnumAsText()
    {
        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        PersonWithText person = GetPersonWithText(1).First();

        // Act
        connection.Insert(person);

        // Query
        PersonWithText queryResult = connection.Query<PersonWithText>(person.Id).First();

        // Assert
        Assert.AreEqual(person.ColumnText, queryResult.ColumnText);
    }

    [TestMethod]
    public void TestInsertAndQueryEnumAsTextByBatch()
    {
        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        List<PersonWithText> people = GetPersonWithText(10).AsList();

        // Act
        connection.InsertAll(people);

        // Query
        List<PersonWithText> queryResult = connection.QueryAll<PersonWithText>().AsList();

        // Assert
        people.ForEach(p =>
        {
            PersonWithText item = queryResult.First(e => e.Id == p.Id);
            Assert.AreEqual(p.ColumnText, item.ColumnText);
        });
    }

    [TestMethod]
    public void TestInsertAndQueryEnumAsIntegerAsNull()
    {
        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        PersonWithInteger person = GetPersonWithInteger(1).First();
        person.ColumnInteger = null;

        // Act
        connection.Insert(person);

        // Query
        PersonWithInteger queryResult = connection.Query<PersonWithInteger>(person.Id).First();

        // Assert
        Assert.IsNull(queryResult.ColumnInteger);
    }

    [TestMethod]
    public void TestInsertAndQueryEnumAsInteger()
    {
        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        PersonWithInteger person = GetPersonWithInteger(1).First();

        // Act
        connection.Insert(person);

        // Query
        PersonWithInteger queryResult = connection.Query<PersonWithInteger>(person.Id).First();

        // Assert
        Assert.AreEqual(person.ColumnInteger, queryResult.ColumnInteger);
    }

    [TestMethod]
    public void TestInsertAndQueryEnumAsIntegerAsBatch()
    {
        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        List<PersonWithInteger> people = GetPersonWithInteger(10).AsList();

        // Act
        connection.InsertAll(people);

        // Query
        List<PersonWithInteger> queryResult = connection.QueryAll<PersonWithInteger>().AsList();

        // Assert
        people.ForEach(p =>
        {
            PersonWithInteger item = queryResult.First(e => e.Id == p.Id);
            Assert.AreEqual(p.ColumnInteger, item.ColumnInteger);
        });
    }

    [TestMethod]
    public void TestInsertAndQueryEnumAsTextAsInt()
    {
        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        PersonWithTextAsInteger person = GetPersonWithTextAsInteger(1).First();

        // Act
        connection.Insert(person, trace: new DiagnosticsTracer());

        // Query
        PersonWithTextAsInteger queryResult = connection.Query<PersonWithTextAsInteger>(person.Id).First();

        // Assert
        Assert.AreEqual(person.ColumnText, queryResult.ColumnText);
    }

    [TestMethod]
    public void TestInsertAndQueryEnumAsTextAsIntAsBatch()
    {
        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        List<PersonWithTextAsInteger> people = GetPersonWithTextAsInteger(10).AsList();

        // Act
        connection.InsertAll(people);

        // Query
        List<PersonWithTextAsInteger> queryResult = connection.QueryAll<PersonWithTextAsInteger>().AsList();

        // Assert
        people.ForEach(p =>
        {
            PersonWithTextAsInteger item = queryResult.First(e => e.Id == p.Id);
            Assert.AreEqual(p.ColumnText, item.ColumnText);
        });
    }

    [TestMethod]
    public void TestInsertAndQueryEnumAsEnum()
    {
        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        PersonWithEnum person = GetPersonWithEnum(1).First();

        // Act
        connection.Insert(person, trace: new DiagnosticsTracer());

        // Query
        connection.ReloadTypes();
        PersonWithEnum queryResult = connection.Query<PersonWithEnum>(person.Id).First();

        // Assert
        Assert.AreEqual(person.ColumnEnumHand, queryResult.ColumnEnumHand);
    }

    [TestMethod]
    public void TestInsertAndQueryEnumAsEnumAsBatch()
    {
        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        List<PersonWithEnum> people = GetPersonWithEnum(10).AsList();

        // Act
        connection.InsertAll(people);

        // Query
        connection.ReloadTypes();
        List<PersonWithEnum> queryResult = connection.QueryAll<PersonWithEnum>().AsList();

        // Assert
        people.ForEach(p =>
        {
            PersonWithEnum item = queryResult.First(e => e.Id == p.Id);
            Assert.AreEqual(p.ColumnEnumHand, item.ColumnEnumHand);
        });
    }

    [TestMethod]
    public void TestInsertAndQueryEnumAsEnumViaEnum()
    {
        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        PersonWithEnum person = GetPersonWithEnum(1).First();

        // Act
        connection.Insert(person);

        // Query
        connection.ReloadTypes();
        PersonWithEnum queryResult = connection.Query<PersonWithEnum>(where: p => p.ColumnEnumHand == person.ColumnEnumHand).First();

        // Assert
        Assert.AreEqual(person.ColumnEnumHand, queryResult.ColumnEnumHand);
    }

    [TestMethod]
    public void TestInsertAndQueryEnumAsEnumViaDynamicEnum()
    {
        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        PersonWithEnum person = GetPersonWithEnum(1).First();

        // Act
        connection.Insert(person);

        // Query
        connection.ReloadTypes();
        PersonWithEnum queryResult = connection.Query<PersonWithEnum>(new { ColumnEnumHand = person.ColumnEnumHand }).First();

        // Assert
        Assert.AreEqual(person.ColumnEnumHand, queryResult.ColumnEnumHand);
    }

    [TestMethod]
    public void TestInsertAndQueryEnumAsNullableEnumAsNull()
    {
        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        PersonWithNullableEnum person = GetPersonWithNullableEnum(1).First();
        person.ColumnEnumHand = null;

        // Act
        connection.Insert(person);

        // Query
        connection.ReloadTypes();
        PersonWithNullableEnum queryResult = connection.Query<PersonWithNullableEnum>(person.Id).First();

        // Assert
        Assert.IsNull(queryResult.ColumnEnumHand);
    }

    [TestMethod]
    public void TestInsertAndQueryEnumAsNullableEnum()
    {
        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        PersonWithNullableEnum person = GetPersonWithNullableEnum(1).First();

        // Act
        connection.Insert(person);

        // Query
        connection.ReloadTypes();
        PersonWithNullableEnum queryResult = connection.Query<PersonWithNullableEnum>(person.Id).First();

        // Assert
        Assert.AreEqual(person.ColumnEnumHand, queryResult.ColumnEnumHand);
    }

    [TestMethod]
    public void TestInsertAndQueryEnumAsNullableEnumAsBatch()
    {
        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        List<PersonWithNullableEnum> people = GetPersonWithNullableEnum(10).AsList();

        // Act
        connection.InsertAll(people);

        // Query
        connection.ReloadTypes();
        List<PersonWithNullableEnum> queryResult = connection.QueryAll<PersonWithNullableEnum>().AsList();

        // Assert
        people.ForEach(p =>
        {
            PersonWithNullableEnum item = queryResult.First(e => e.Id == p.Id);
            Assert.AreEqual(p.ColumnEnumHand, item.ColumnEnumHand);
        });
    }

    [TestMethod]
    public void TestInsertAndQueryEnumAsNullableEnumByEnum()
    {
        using Npgsql.NpgsqlConnection connection = this.CreateTestConnection();
        // Setup
        PersonWithNullableEnum person = GetPersonWithNullableEnum(1).First();

        // Act
        connection.Insert(person);

        // Query
        connection.ReloadTypes();
        PersonWithNullableEnum queryResult = connection.Query<PersonWithNullableEnum>(where: p => p.ColumnEnumHand == person.ColumnEnumHand).First();

        // Assert
        Assert.AreEqual(person.ColumnEnumHand, queryResult.ColumnEnumHand);
    }
}
