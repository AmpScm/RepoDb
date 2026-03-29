using System.Data;
using RepoDb.Enumerations;

namespace RepoDb.Extensions.QueryFields;

/// <summary>
/// 
/// </summary>
public enum DateTimePartType
{
    /// <summary></summary>
    Date,
    /// <summary></summary>
    Year,
    /// <summary></summary>
    Month,
    /// <summary></summary>
    Day,
    /// <summary></summary>
    Hour,
    /// <summary></summary>
    Minute,
    /// <summary></summary>
    Second,
    /// <summary></summary>
    Millisecond
}

/// <summary>
/// 
/// </summary>
public class DateTimePartQueryField : FunctionalQueryField
{
    /// <summary></summary>
    public const string DateFormat = "DATE({0})";
    /// <summary></summary>
    public const string YearFormat = "YEAR({0})";
    /// <summary></summary>
    public const string MonthFormat = "MONTH({0})";
    /// <summary></summary>
    public const string DayFormat = "DAY({0})";
    /// <summary></summary>
    public const string HourFormat = "HOUR({0})";
    /// <summary></summary>
    public const string MinuteFormat = "MINUTE({0})";
    /// <summary></summary>
    public const string SecondFormat = "SECOND({0})";
    /// <summary></summary>
    public const string MillisecondFormat = "MILLISECOND({0})";

    #region Constructors

    /// <summary>
    /// Creates a new instance of <see cref="LengthQueryField"/> object.
    /// </summary>
    /// <param name="fieldName">The name of the field for the query expression.</param>
    /// <param name="value">The value to be used for the query expression.</param>
    /// <param name="dateTimePart"></param>
    public DateTimePartQueryField(string fieldName,
        object? value,
        DateTimePartType dateTimePart = DateTimePartType.Date)
        : this(fieldName, Operation.Equal, value, dbType: null, dateTimePart)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="LengthQueryField"/> object.
    /// </summary>
    /// <param name="fieldName">The name of the field for the query expression.</param>
    /// <param name="value">The value to be used for the query expression.</param>
    /// <param name="dbType">The database type to be used for the query expression.</param>
    /// <param name="dateTimePart"></param>
    public DateTimePartQueryField(string fieldName,
        object? value,
        DbType? dbType,
        DateTimePartType dateTimePart = DateTimePartType.Date)
        : this(fieldName, Operation.Equal, value, dbType, dateTimePart)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="LengthQueryField"/> object.
    /// </summary>
    /// <param name="fieldName">The name of the field for the query expression.</param>
    /// <param name="operation">The operation to be used for the query expression.</param>
    /// <param name="value">The value to be used for the query expression.</param>
    /// <param name="dateTimePart"></param>
    public DateTimePartQueryField(string fieldName,
        Operation operation,
        object? value,
        DateTimePartType dateTimePart = DateTimePartType.Date)
        : this(fieldName, operation, value, null, dateTimePart)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="LengthQueryField"/> object.
    /// </summary>
    /// <param name="fieldName">The name of the field for the query expression.</param>
    /// <param name="operation">The operation to be used for the query expression.</param>
    /// <param name="value">The value to be used for the query expression.</param>
    /// <param name="dbType">The database type to be used for the query expression.</param>
    /// <param name="dateTimePart"></param>
    public DateTimePartQueryField(string fieldName,
        Operation operation,
        object? value,
        DbType? dbType,
        DateTimePartType dateTimePart = DateTimePartType.Date)
        : base(fieldName, operation, value,
            dbType ?? dateTimePart switch
            {
                DateTimePartType.Date => DbType.Date,
                _ => DbType.Int32
            },
            dateTimePart switch
            {
                DateTimePartType.Date => DateFormat,
                DateTimePartType.Year => YearFormat,
                DateTimePartType.Month => MonthFormat,
                DateTimePartType.Day => DayFormat,
                DateTimePartType.Hour => HourFormat,
                DateTimePartType.Minute => MinuteFormat,
                DateTimePartType.Second => SecondFormat,
                DateTimePartType.Millisecond => MillisecondFormat,
                _ => throw new ArgumentOutOfRangeException(nameof(dateTimePart))
            })
    { }

    #endregion
}
