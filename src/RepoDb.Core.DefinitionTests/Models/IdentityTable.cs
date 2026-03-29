using RepoDb.Attributes;

namespace RepoDb.IntegrationTests.Models;

[Map("[dbo].[IdentityTable]")]
public class IdentityTable
{
    public long Id { get; set; }
    public Guid RowGuid { get; set; }
    public bool? ColumnBit { get; set; }
    public DateTime? ColumnDateTime { get; set; }
    public DateTime? ColumnDateTime2 { get; set; }
    public decimal? ColumnDecimal { get; set; }
    public double? ColumnFloat { get; set; }
    public int? ColumnInt { get; set; }
    public string ColumnNVarChar { get; set; }
}
