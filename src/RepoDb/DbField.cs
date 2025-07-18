using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace RepoDb;

/// <summary>
/// A class the holds the column definition of the table.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay, nq}")]
public sealed class DbField : Field, IEquatable<DbField>
{
    private int? HashCode { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="DbField"/> object.
    /// </summary>
    /// <param name="name">The name of the field.</param>
    /// <param name="isPrimary">The value that indicates whether the field is primary.</param>
    /// <param name="isIdentity">The value that indicates whether the field is identity.</param>
    /// <param name="isNullable">The value that indicates whether the field is nullable.</param>
    /// <param name="type">The equivalent .NET CLR type of the field.</param>
    /// <param name="size">The size of the field.</param>
    /// <param name="precision">The precision of the field.</param>
    /// <param name="scale">The scale of the field.</param>
    /// <param name="databaseType">The database type of the field.</param>
    /// <param name="hasDefaultValue">The value that defines whether the column has a default value..</param>
    /// <param name="provider">The database provider who created this instance.</param>
    public DbField(string name,
        bool isPrimary,
        bool isIdentity,
        bool isNullable,
        Type type,
        int? size,
        byte? precision,
        byte? scale,
        string? databaseType,
        bool hasDefaultValue = false,
        string? provider = null)
        : this(name, isPrimary, isIdentity, isNullable, type, size, precision, scale, databaseType, hasDefaultValue, false, provider)
    { }

    public DbField(string name,
        bool isPrimary,
        bool isIdentity,
        bool isNullable,
        Type type,
        int? size,
        byte? precision,
        byte? scale,
        string? databaseType,
        bool hasDefaultValue,
        bool isGenerated,
        string? provider = null)
        : base(name, type)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(name);

        // Set the properties
        IsPrimary = isPrimary;
        IsIdentity = isIdentity;
        IsNullable = isNullable;
        Size = size;
        if (type == StaticType.Double && precision > 38)
        {
            Precision = 38;
        }
        else
        {
            Precision = precision;
        }
        Scale = scale;
        DatabaseType = databaseType;
        HasDefaultValue = hasDefaultValue;
        IsGenerated = isGenerated;
        Provider = provider;
    }

    #region Properties

    /// <summary>
    /// Gets the value that indicates whether the column is a primary column.
    /// </summary>
    public bool IsPrimary { get; }

    /// <summary>
    /// Gets the value that indicates whether the column is an identify column.
    /// </summary>
    public bool IsIdentity { get; }

    /// <summary>
    /// Gets the value that indicates whether the column is nullable.
    /// </summary>
    public bool IsNullable { get; }

    /// <summary>
    /// Gets the .NET type of the column.
    /// </summary>
    public new Type Type => base.Type!;

    /// <summary>
    /// Gets the size of the column.
    /// </summary>
    public int? Size { get; }

    /// <summary>
    /// Gets the precision of the column.
    /// </summary>
    public byte? Precision { get; }

    /// <summary>
    /// Gets the scale of the column.
    /// </summary>
    public byte? Scale { get; }

    /// <summary>
    /// Gets the database type of the column.
    /// </summary>
    public string? DatabaseType { get; }

    /// <summary>
    /// Gets the value that defines whether the column has a default value.
    /// </summary>
    public bool HasDefaultValue { get; }

    /// <summary>
    /// Gets the value that defines whether the column is computed by the database server
    /// </summary>
    public bool IsGenerated { get; }

    /// <summary>
    /// Indicates if the member is read-only. It is read-only if it is either generated or an identity.
    /// </summary>
    public bool IsReadOnly => IsGenerated || IsIdentity;

    /// <summary>
    /// Gets the database provider who created this instance.
    /// </summary>
    public string? Provider { get; }

    /// <summary>
    /// Gets the type to map to, including nullable
    /// </summary>
    /// <returns></returns>
    public Type TypeNullable() => IsNullable && Type.IsValueType ? typeof(Nullable<>).MakeGenericType(Type) : Type;

    public Field AsField() => this;

    #endregion

    #region Methods

    /// <summary>
    /// Gets the string that represents the instance of this <see cref="DbField"/> object.
    /// </summary>
    /// <returns>The string that represents the instance of this <see cref="DbField"/> object.</returns>
    public override string ToString() =>
        string.Concat(FieldName, ", ", IsPrimary.ToString(), " (", GetHashCode().ToString(CultureInfo.InvariantCulture), ")");

    private string DebuggerDisplay
        => String.Join(" ",
            new string?[] {
                $@"""{FieldName}""",
                IsPrimary ? "primary" : null,
                IsIdentity ? "identity" : null,
                Type?.Name is { } name ? $"type={name}" : null,
                DatabaseType is { } dbType ? $"dbtype={dbType}" : null
            }.Where(x => !string.IsNullOrWhiteSpace(x)));

    internal static new IEqualityComparer<DbField> CompareByName { get; } = new DbFieldNameEqualityComparer();


    #endregion

    #region Equality and comparers

    /// <summary>
    /// Returns the hashcode for this <see cref="DbField"/>.
    /// </summary>
    /// <returns>The hashcode value.</returns>
    public override int GetHashCode()
    {
        if (HashCode is not { } hashCode)
        {
            HashCode = hashCode = System.HashCode.Combine(
                FieldName,
                IsPrimary,
                IsIdentity,
                IsNullable,
                Type,
                Size,
                System.HashCode.Combine(
                    Precision,
                    Scale,
                    HasDefaultValue,
                    IsGenerated,
                    DatabaseType,
                    Provider));
        }

        return hashCode;
    }

    /// <summary>
    /// Compares the <see cref="DbField"/> object equality against the given target object.
    /// </summary>
    /// <param name="obj">The object to be compared to the current object.</param>
    /// <returns>True if the instances are equals.</returns>
    public override bool Equals(object? obj)
    {
        return Equals(obj as DbField);
    }

    /// <summary>
    /// Compares the <see cref="DbField"/> object equality against the given target object.
    /// </summary>
    /// <param name="other">The object to be compared to the current object.</param>
    /// <returns>True if the instances are equal.</returns>
    public bool Equals(DbField? other)
    {
        return other is not null
            && other.FieldName == FieldName
            && other.IsPrimary == IsPrimary
            && other.IsIdentity == IsIdentity
            && other.IsNullable == IsNullable
            && other.Type == Type
            && other.Size == Size
            && other.Precision == Precision
            && other.Scale == Scale
            && other.HasDefaultValue == HasDefaultValue
            && other.IsGenerated == IsGenerated
            && other.DatabaseType == DatabaseType
            && other.Provider == Provider;
    }

    /// <summary>
    /// Compares the equality of the two <see cref="DbField"/> objects.
    /// </summary>
    /// <param name="objA">The first <see cref="DbField"/> object.</param>
    /// <param name="objB">The second <see cref="DbField"/> object.</param>
    /// <returns>True if the instances are equal.</returns>
    public static bool operator ==(DbField? objA,
        DbField? objB)
    {
        if (objA is null)
        {
            return objB is null;
        }
        return objA.Equals(objB);
    }

    /// <summary>
    /// Compares the inequality of the two <see cref="DbField"/> objects.
    /// </summary>
    /// <param name="objA">The first <see cref="DbField"/> object.</param>
    /// <param name="objB">The second <see cref="DbField"/> object.</param>
    /// <returns>True if the instances are not equal.</returns>
    public static bool operator !=(DbField? objA,
        DbField? objB) =>
        !(objA == objB);

    #endregion

    private sealed class DbFieldNameEqualityComparer : IEqualityComparer<DbField>
    {
        public bool Equals(DbField? x, DbField? y)
        {
            return StringComparer.OrdinalIgnoreCase.Equals(x?.FieldName, y?.FieldName);
        }

        public int GetHashCode([DisallowNull] DbField obj)
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(obj.FieldName);
        }
    }
}
