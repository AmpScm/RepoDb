using System.Data;
using RepoDb.Extensions;

namespace RepoDb;

/// <summary>
/// A class that holds the value of the field parameter.
/// </summary>
public sealed class Parameter : IEquatable<Parameter>
{
    private int? hashCode;

    #region Constructors

    /// <summary>
    /// Creates a new instance of <see cref="Parameter"/> object.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    public Parameter(string name,
        object? value)
        : this(name, value, null, false)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="Parameter"/> object.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    /// <param name="dbType">The database type of the parameter.</param>
    public Parameter(string name,
        object? value,
        DbType? dbType)
        : this(name, value, dbType, false)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="Parameter"/> object.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    /// <param name="dbType">The database type of the parameter.</param>
    /// <param name="prependUnderscore">The value to identify whether the underscope prefix will be prepended.</param>
    internal Parameter(string name,
        object? value,
        DbType? dbType,
        bool prependUnderscore)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(name);

        // Set the properties
        OriginalName = name.AsAlphaNumeric();
        Name = OriginalName;
        OriginalValue = value;
        Value = value;
        DbType = dbType;
        if (prependUnderscore)
        {
            PrependAnUnderscoreAtParameter();
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the name of the parameter.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the original name of the parameter.
    /// </summary>
    private string OriginalName { get; }

    /// <summary>
    /// Gets the value of the parameter.
    /// </summary>
    public object? Value { get; private set; }

    /// <summary>
    /// Gets the original value of the parameter.
    /// </summary>
    private object? OriginalValue { get; }

    /// <summary>
    /// Gets the database type of the parameter.
    /// </summary>
    public DbType? DbType { get; private set; }

    #endregion

    #region Methods

    /// <summary>
    /// Prepend an underscore on the current parameter object.
    /// </summary>
    internal void PrependAnUnderscoreAtParameter()
    {
        if (!Name.StartsWith('_'))
        {
            Name = '_' + Name;
        }
    }

    /// <summary>
    /// Set the name of the parameter.
    /// </summary>
    /// <param name="name">The new name.</param>
    internal void SetName(string name) =>
        Name = name;

    /// <summary>
    /// Set the value of the parameter.
    /// </summary>
    /// <param name="value">The new value.</param>
    internal void SetValue(object? value) =>
        Value = value;

    /// <summary>
    /// Resets the <see cref="Parameter"/> object back to its default state (as is newly instantiated).
    /// </summary>
    public void Reset()
    {
        Name = OriginalName;
        Value = OriginalValue;
        hashCode = null;
    }

    /// <summary>
    /// Stringify the current object. Will return the format of <b>Name (Value)</b> text.
    /// </summary>
    /// <returns></returns>
    public override string ToString() =>
        string.Concat(Name, " (", Value?.ToString(), ")");

    #endregion

    #region Equality and comparers

    /// <summary>
    /// Returns the hashcode for this <see cref="Parameter"/>.
    /// </summary>
    /// <returns>The hashcode value.</returns>
    public override int GetHashCode()
    {
        if (this.hashCode != null)
        {
            return this.hashCode.Value;
        }

        // OriginalName
        var hashCode = OriginalName.GetHashCode();

        // DbType
        if (DbType.HasValue)
        {
            hashCode = HashCode.Combine(hashCode, DbType.Value);
        }

        // Set and return the hashcode
        return this.hashCode ??= hashCode;
    }

    /// <summary>
    /// Compares the <see cref="Parameter"/> object equality against the given target object.
    /// </summary>
    /// <param name="obj">The object to be compared to the current object.</param>
    /// <returns>True if the instances are equals.</returns>
    public override bool Equals(object? obj)
    {
        return Equals(obj as Parameter);
    }

    /// <summary>
    /// Compares the <see cref="Parameter"/> object equality against the given target object.
    /// </summary>
    /// <param name="other">The object to be compared to the current object.</param>
    /// <returns>True if the instances are equal.</returns>
    public bool Equals(Parameter? other)
    {
        return other is not null
            && other.OriginalName == OriginalName
            && other.DbType == DbType;
    }

    /// <summary>
    /// Compares the equality of the two <see cref="Parameter"/> objects.
    /// </summary>
    /// <param name="objA">The first <see cref="Parameter"/> object.</param>
    /// <param name="objB">The second <see cref="Parameter"/> object.</param>
    /// <returns>True if the instances are equal.</returns>
    public static bool operator ==(Parameter objA,
        Parameter objB)
    {
        if (objA is null)
        {
            return objB is null;
        }
        return objA.Equals(objB);
    }

    /// <summary>
    /// Compares the inequality of the two <see cref="Parameter"/> objects.
    /// </summary>
    /// <param name="objA">The first <see cref="Parameter"/> object.</param>
    /// <param name="objB">The second <see cref="Parameter"/> object.</param>
    /// <returns>True if the instances are not equal.</returns>
    public static bool operator !=(Parameter objA,
        Parameter objB) =>
        (objA == objB) == false;

    #endregion
}
