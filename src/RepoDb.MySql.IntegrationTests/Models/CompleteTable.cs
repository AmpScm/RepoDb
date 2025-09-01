namespace RepoDb.MySql.IntegrationTests.Models;

public class CompleteTable
{
    public long Id { get; set; }
    public string ColumnVarchar { get; set; }
    public int? ColumnInt { get; set; }
    public decimal? ColumnDecimal2 { get; set; }
    public DateTime? ColumnDateTime { get; set; }
    public byte[] ColumnBlob { get; set; }
    public byte[] ColumnBlobAsArray { get; set; }
    public byte[] ColumnBinary { get; set; }
    public byte[] ColumnLongBlob { get; set; }
    public byte[] ColumnMediumBlob { get; set; }
    public byte[] ColumnTinyBlob { get; set; }
    public byte[] ColumnVarBinary { get; set; }
    public DateTime? ColumnDate { get; set; }
    public DateTime? ColumnDateTime2 { get; set; }
    public TimeSpan? ColumnTime { get; set; }
    public DateTime? ColumnTimeStamp { get; set; }
    public short? ColumnYear { get; set; }
    public byte[] ColumnGeometry { get; set; }
    public byte[] ColumnLineString { get; set; }
    public byte[] ColumnMultiLineString { get; set; }
    public byte[] ColumnMultiPoint { get; set; }
    public byte[] ColumnMultiPolygon { get; set; }
    public byte[] ColumnPoint { get; set; }
    public byte[] ColumnPolygon { get; set; }
    public long? ColumnBigint { get; set; }
    public decimal? ColumnDecimal { get; set; }
    public double? ColumnDouble { get; set; }
    public float? ColumnFloat { get; set; }
    public int? ColumnInt2 { get; set; }
    public int? ColumnMediumInt { get; set; }
    public double? ColumnReal { get; set; }
    public short? ColumnSmallInt { get; set; }
    public sbyte? ColumnTinyInt { get; set; }
    public string ColumnChar { get; set; }
    public string ColumnJson { get; set; }
    public string ColumnNChar { get; set; }
    public string ColumnNVarChar { get; set; }
    public string ColumnLongText { get; set; }
    public string ColumnMediumText { get; set; }
    public string ColumnText { get; set; }
    public string ColumnTinyText { get; set; }
    public ulong? ColumnBit { get; set; }
}
