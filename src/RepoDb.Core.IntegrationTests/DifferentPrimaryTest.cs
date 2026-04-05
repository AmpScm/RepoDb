using Microsoft.Data.SqlClient;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;

namespace RepoDb.IntegrationTests;

[TestClass]
public class DifferentPrimaryTest : TestBase
{

    #region Insert

    [TestMethod]
    public void TestSqlConnectionInsertForIdentityTableWithDifferentPrimary()
    {
        // Setup
        var entity = Helper.CreateIdentityTableWithDifferentPrimary();

        using var connection = CreateConnection();
        // Act
        var insertResult = connection.Insert<IdentityTableWithDifferentPrimary, long>(entity);

        // Assert
        Assert.AreEqual(entity.Id, insertResult);
        Assert.IsGreaterThan(0, insertResult);
        Assert.IsGreaterThan(0, entity.Id);
        Assert.AreEqual(1, connection.CountAll<IdentityTableWithDifferentPrimary>());
    }

    #endregion

    #region InsertAll

    [TestMethod]
    public void TestSqlConnectionInsertAllForIdentityTableWithDifferentPrimary()
    {
        // Setup
        var entities = Helper.CreateIdentityTableWithDifferentPrimaries(10);

        using var connection = CreateConnection();
        // Act
        var insertAllResult = connection.InsertAll(entities);

        // Assert
        Assert.AreEqual(entities.Count, insertAllResult);
        Assert.AreEqual(entities.Count, connection.CountAll<IdentityTableWithDifferentPrimary>());

        // Act
        var queryResult = connection.QueryAll<IdentityTableWithDifferentPrimary>();

        // Assert
        Assert.AreEqual(entities.Count, queryResult.Count());
        foreach (var entity in entities)
            Helper.AssertPropertiesEquality(entity,
                queryResult.ElementAt(entities.IndexOf(entity)));
    }

    #endregion

    #region Delete

    [TestMethod]
    public void TestSqlConnectionDeleteForIdentityTableWithDifferentPrimaryViaDataEntity()
    {
        // Setup
        var entity = Helper.CreateIdentityTableWithDifferentPrimary();

        using var connection = CreateConnection();
        // Act
        connection.Insert(entity);

        // Act
        var deleteResult = connection.Delete(entity);

        // Assert
        Assert.IsGreaterThan(0, deleteResult);
        Assert.AreEqual(0, connection.CountAll<IdentityTableWithDifferentPrimary>());
    }

    [TestMethod]
    public void TestSqlConnectionDeleteForIdentityTableWithDifferentPrimaryViaPrimary()
    {
        // Setup
        var entity = Helper.CreateIdentityTableWithDifferentPrimary();

        using var connection = CreateConnection();
        // Act
        connection.Insert(entity);

        // Act
        var deleteResult = connection.Delete<IdentityTableWithDifferentPrimary>(entity.RowGuid);

        // Assert
        Assert.IsGreaterThan(0, deleteResult);
        Assert.AreEqual(0, connection.CountAll<IdentityTableWithDifferentPrimary>());
    }

    #endregion

    #region Query

    [TestMethod]
    public void TestSqlConnectionQueryForIdentityTableWithDifferentPrimary()
    {
        // Setup
        var entity = Helper.CreateIdentityTableWithDifferentPrimary();

        using var connection = CreateConnection();
        // Act
        connection.Insert<IdentityTableWithDifferentPrimary, long>(entity);

        // Act
        var queryResult = connection.Query<IdentityTableWithDifferentPrimary>(entity.RowGuid).FirstOrDefault();

        // Assert
        Assert.IsNotNull(queryResult);
        Helper.AssertPropertiesEquality(entity, queryResult);
    }

    #endregion

    #region Update

    [TestMethod]
    public void TestSqlConnectionUpdateForIdentityTableWithDifferentPrimaryViaDataEntity()
    {
        // Setup
        var entity = Helper.CreateIdentityTableWithDifferentPrimary();

        using var connection = CreateConnection();
        // Act
        connection.Insert<IdentityTableWithDifferentPrimary, long>(entity);

        // Setup
        entity.ColumnBit = false;
        entity.ColumnDateTime2 = DateTime.UtcNow;

        // Act
        var updateResult = connection.Update(entity);

        // Assert
        Assert.IsGreaterThan(0, updateResult);

        // Act
        var data = connection.Query<IdentityTableWithDifferentPrimary>(entity.RowGuid).FirstOrDefault();

        // Assert
        Assert.IsNotNull(data);
        Helper.AssertPropertiesEquality(entity, data);
    }

    [TestMethod]
    public void TestSqlConnectionUpdateForIdentityTableWithDifferentPrimaryViaPrimaryKey()
    {
        // Setup
        var entity = Helper.CreateIdentityTableWithDifferentPrimary();

        using var connection = CreateConnection();
        // Act
        connection.Insert<IdentityTableWithDifferentPrimary, long>(entity);

        // Setup
        entity.ColumnBit = false;
        entity.ColumnDateTime2 = DateTime.UtcNow;

        // Act
        var updateResult = connection.Update<IdentityTableWithDifferentPrimary>(entity, entity.RowGuid);

        // Assert
        Assert.IsGreaterThan(0, updateResult);

        // Act
        var data = connection.Query<IdentityTableWithDifferentPrimary>(entity.RowGuid).FirstOrDefault();

        // Assert
        Assert.IsNotNull(data);
        Helper.AssertPropertiesEquality(entity, data);
    }

    #endregion

    #region UpdateAll

    [TestMethod]
    public void TestSqlConnectionUpdateAllForIdentityTableWithDifferentPrimaries()
    {
        // Setup
        var entities = Helper.CreateIdentityTableWithDifferentPrimaries(10);

        using var connection = CreateConnection();
        // Act
        connection.InsertAll(entities);

        // Setup
        foreach (var entity in entities)
        {
            entity.ColumnBit = false;
            entity.ColumnDateTime2 = DateTime.UtcNow;
        }

        // Act
        var updateAllResult = connection.UpdateAll(entities);

        // Assert
        Assert.AreEqual(entities.Count, updateAllResult);

        // Act
        var queryResult = connection.QueryAll<IdentityTableWithDifferentPrimary>();

        // Assert
        Assert.AreEqual(entities.Count, queryResult.Count());
        foreach (var entity in entities)
            Helper.AssertPropertiesEquality(entity,
                queryResult.ElementAt(entities.IndexOf(entity)));
    }

    #endregion
}
