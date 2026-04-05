using RepoDb.Oracle.IntegrationTests.Enumerations;

namespace RepoDb.Oracle.IntegrationTests.Models;

public class EnumTable
{
    public long Id { get; set; }
    public Hands? ColumnEnumHand { get; set; }
}
