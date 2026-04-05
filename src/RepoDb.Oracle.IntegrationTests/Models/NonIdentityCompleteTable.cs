namespace RepoDb.Oracle.IntegrationTests.Models;

public class NonIdentityCompleteTable
{
    public System.Int64 Id { get; set; }
    public long? ColumnBigInt { get; set; }
    public byte[]? ColumnBinary { get; set; }
    public bool? ColumnBit { get; set; }
    public string? ColumnChar { get; set; }
    public DateTime? ColumnDate { get; set; }
    public DateTime? ColumnDateTime { get; set; }
    public DateTime? ColumnDateTime2 { get; set; }
    public DateTimeOffset? ColumnDateTimeOffset { get; set; }

    public decimal? ColumnDecimal { get; set; }
    public double? ColumnFloat { get; set; }

    public byte[]? ColumnImage { get; set; }

    public int? ColumnInt { get; set; }
    public decimal? ColumnMoney { get; set; }

    public string? ColumnNChar { get; set; }
    public string? ColumnNText { get; set; }

    public decimal? ColumnNumeric { get; set; }

    public string? ColumnNVarChar { get; set; }

    public float? ColumnReal { get; set; }

    public DateTime? ColumnSmallDateTime { get; set; }

    public short? ColumnSmallInt { get; set; }

    public decimal? ColumnSmallMoney { get; set; }

    public string? ColumnText { get; set; }

    //public TimeSpan? ColumnTime { get; set; }
    public TimeSpan? ColumnTimestamp { get; set; }


    public byte? ColumnTinyInt { get; set; }
    public Guid? ColumnUniqueIdentifier { get; set; }
    public byte[]? ColumnVarBinary { get; set; }
    public string? ColumnVarChar { get; set; }
    public string? ColumnXml { get; set; }
}
