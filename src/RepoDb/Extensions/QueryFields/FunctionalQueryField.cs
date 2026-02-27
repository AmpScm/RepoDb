using System.Data;
using RepoDb.DbSettings;
using RepoDb.Enumerations;
using RepoDb.Interfaces;

namespace RepoDb.Extensions.QueryFields;

/// <summary>
/// A dynamic functional-based <see cref="QueryField"/> object. This requires a properly constructed
/// formatted string (for a specific database function) in order to work properly.
/// </summary>
/// <example>
/// See sample code below that uses a TRIM function.
/// <code>
///     var where = new FunctionalQueryField("ColumnName", "Value", "TRIM({0})");
///     var result = connection.Query&lt;Entity&gt;(where);
/// </code>
/// </example>
public class FunctionalQueryField : QueryField
{
    #region Constructors

    /// <summary>
    /// Creates a new instance of <see cref="FunctionalQueryField"/> object.
    /// </summary>
    /// <param name="fieldName">The name of the field for the query expression.</param>
    /// <param name="operation">The operation to be used for the query expression.</param>
    /// <param name="value">The value to be used for the query expression.</param>
    /// <param name="dbType">The database type to be used for the query expression.</param>
    /// <param name="format">The properly constructed format of the target function to be used.</param>
    public FunctionalQueryField(string fieldName,
        Operation operation,
        object? value,
        DbType? dbType,
        string? format = null)
        : base(fieldName, operation, value, dbType)
    {
        Format = format;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the properly constructed format of the target function.
    /// </summary>
    public string? Format { get; }

    #endregion

    #region Methods

    /// <summary>
    /// Gets the string representations (column-value pairs) of the current <see cref="QueryField"/> object with the formatted-function transformations.
    /// </summary>
    /// <param name="index">The target index.</param>
    /// <param name="dbSetting">The database setting currently in used.</param>
    /// <returns>The string representations of the current <see cref="QueryField"/> object using the LOWER function.</returns>
    public override string GetString(int index,
        IDbSetting? dbSetting) =>
        GetString(index, (Format is { } && dbSetting is BaseDbSetting db) ? db.TranslateFunctionalFormat(Format) : Format, dbSetting);

    #endregion

    #region Equality and comparers

    /// <summary>
    /// Returns the hashcode for this <see cref="FunctionalQueryField"/>.
    /// </summary>
    /// <returns>The hashcode value.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Format);
    }

    public override bool Equals(QueryField? other)
    {
        return other is FunctionalQueryField fqf
            && base.Equals(fqf)
            && fqf.Format == Format;
    }

    #endregion
}
