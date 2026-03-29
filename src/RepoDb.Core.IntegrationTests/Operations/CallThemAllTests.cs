using System.Reflection;

namespace RepoDb.IntegrationTests.Operations;

[TestClass]
public class CallThemAllTests : TestBase
{
    public static IEnumerable<MethodInfo> AllDbOperations = typeof(DbConnectionExtension).GetMethods()
        .Where(x => x.IsPublic && x.IsStatic);


    public static IEnumerable<object[]> AllDbOperationsNonAsync =>
        AllDbOperations.Where(x => x.Name.EndsWith("Async") == false)
        .Select(x=> new[] { x });

    public static IEnumerable<object[]> AllDbOperationsAsync =>
        AllDbOperations.Where(x => x.Name.EndsWith("Async") == true)
        .Select(x => new[] { x });

    public static string GetDisplayName(MethodInfo call, object[] info)
    {
        var mi = (MethodInfo)info[0];
        return $"{call.Name} {mi.Name}({string.Join(", ", mi.GetParameters().Select(x => x.ParameterType))})";
    }

    [TestMethod, DynamicData(nameof(AllDbOperationsNonAsync), DynamicDataDisplayName = nameof(GetDisplayName))]
    public void AllDbConnectionOperation(MethodInfo mi)
    {
    }


    [TestMethod, DynamicData(nameof(AllDbOperationsAsync))]
    public async Task AllDbConnectionOperationAsync(MethodInfo mi)
    {
    }
}
