using Oracle.ManagedDataAccess.Types;
using Oracle.ManagedDataAccess.Client;

namespace RepoDb.Oracle.Mappings;

/// <summary>
/// 
/// </summary>
[OracleCustomTypeMapping(ODCINumberListFactory.ODCINUMBERLIST)]
public sealed class ODCINumberListFactory : IOracleArrayTypeFactory
{
    /// <summary>
    /// 
    /// </summary>
    public const string ODCINUMBERLIST = "SYS.ODCINUMBERLIST";
    /// <summary>
    /// 
    /// </summary>
    /// <param name="numElems"></param>
    /// <returns></returns>
    public Array CreateArray(int numElems)
        => new long[numElems];

    /// <summary>
    /// 
    /// </summary>
    /// <param name="numElems"></param>
    /// <returns></returns>
    public Array CreateStatusArray(int numElems)
        => new OracleUdtStatus[numElems];
}
