using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Mappers;

[TestClass]
[DoNotParallelize]
public partial class StatementBuilderMapperTest
{
    [TestInitialize]
    public void Initialize()
    {
        Cleanup();
    }

    [TestCleanup]
    public void Cleanup()
    {
        StatementBuilderMapper.Clear();
    }

    #region Methods

    [TestMethod]
    public void TestStatementBuilderMapperMappingViaGeneric()
    {
        // Setup
        var statementBuilder = new CustomStatementBuilder();
        StatementBuilderMapper.Add<CustomDbConnection>(statementBuilder, true);

        // Act
        var actual = StatementBuilderMapper.Get<CustomDbConnection>();

        // Assert
        Assert.AreSame(statementBuilder, actual);
    }

    [TestMethod]
    public void TestStatementBuilderMapperMappingCanBeRemovedViaGeneric()
    {
        // Setup
        var statementBuilder = new CustomStatementBuilder();
        StatementBuilderMapper.Add<CustomDbConnection>(statementBuilder, true);

        // Act
        StatementBuilderMapper.Remove<CustomDbConnection>();

        // Assert
        Assert.ThrowsExactly<ArgumentException>(() => StatementBuilderMapper.Get<CustomDbConnection>());
    }

    #endregion
}
