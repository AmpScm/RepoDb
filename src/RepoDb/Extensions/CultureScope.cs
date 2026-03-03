using System.Globalization;

namespace RepoDb.Extensions;

/// <summary>
/// store current culture and set CultureInfo.DefaultThreadCurrentCulture for unit test case.
/// restore original culture when dispose.
/// </summary>
public struct CultureScope : IDisposable
{
    private readonly CultureInfo originalCulture;

    public CultureScope(CultureInfo setCulture)
    {
        originalCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = setCulture ?? CultureInfo.InvariantCulture;
    }

    public void Dispose()
    {
        CultureInfo.CurrentCulture = originalCulture;
    }
}
