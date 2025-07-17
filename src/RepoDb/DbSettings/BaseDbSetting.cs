using RepoDb.Interfaces;

namespace RepoDb.DbSettings;

/// <summary>
/// A base class to be used when implementing an <see cref="IDbSetting"/>-based object to support a specific RDBMS data provider.
/// </summary>
public abstract record BaseDbSetting : IDbSetting, IEquatable<BaseDbSetting>
{
    #region Privates

    private int? hashCode;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new instance of <see cref="BaseDbSetting"/> class.
    /// </summary>
    protected BaseDbSetting()
    {
        AreTableHintsSupported = true;
        AverageableType = StaticType.Double;
        ClosingQuote = "]";
        DefaultSchema = "dbo";
        IsDirectionSupported = true;
        IsExecuteReaderDisposable = true;
        IsMultiStatementExecutable = true;
        IsPreparable = true;
        IsUseUpsert = false;
        OpeningQuote = "[";
        ParameterPrefix = "@";
        GenerateFinalSemiColon = true;
    }

    #endregion

    #region Properties

    /// <inheritdoc />
    public bool AreTableHintsSupported { get; protected init; }

    /// <inheritdoc />
    public string ClosingQuote { get; protected init; }

    /// <inheritdoc />
    public Type AverageableType { get; protected init; }

    /// <inheritdoc />
    public string? DefaultSchema { get; protected init; }

    /// <inheritdoc />
    public bool IsDirectionSupported { get; protected init; }

    /// <inheritdoc />
    public bool IsExecuteReaderDisposable { get; protected init; }

    /// <inheritdoc />
    public bool IsMultiStatementExecutable { get; protected init; }

    /// <inheritdoc />
    public bool IsPreparable { get; protected init; }

    /// <inheritdoc />
    public bool IsUseUpsert { get; protected init; }

    /// <inheritdoc />
    public string OpeningQuote { get; protected init; }

    /// <inheritdoc />
    public string ParameterPrefix { get; protected init; }

    /// <inheritdoc />
    public int MaxParameterCount { get; protected init; } = 2100 - 2; // SqlServer supports 2100 parameters, but Microsoft.Data.SqlServer and System.Data.SqlClient reserve 2 for internal use

    /// <inheritdoc />
    public int MaxQueriesInBatchCount { get; protected init; } = 1000;

    /// <inheritdoc />
    public bool GenerateFinalSemiColon { get; protected init; }

    /// <inheritdoc />
    public bool QuoteParameterNames { get; protected init; }

    /// <inheritdoc />
    public int? UseArrayParameterTreshold { get; protected init; }

    /// <inheritdoc />
    public int? UseInValuesTreshold { get; protected init; }

    public int MaxArrayParameterValueCount { get; protected init; } = ushort.MaxValue;

    #endregion

    #region Equality and comparers

    /// <summary>
    /// Returns the hashcode for this <see cref="BaseDbSetting"/>.
    /// </summary>
    /// <returns>The hashcode value.</returns>
    public override int GetHashCode()
    {
        if (this.hashCode != null)
        {
            return this.hashCode.Value;
        }

        // Use the non nullable for perf purposes
        var hashCode = 0;

        // AreTableHintsSupported
        hashCode = HashCode.Combine(hashCode,
            AreTableHintsSupported,
            OpeningQuote,
            ClosingQuote,
            ParameterPrefix,
            AverageableType,
            DefaultSchema);

        // IsDirectionSupported
        hashCode = HashCode.Combine(hashCode,
            IsDirectionSupported,
            IsExecuteReaderDisposable,
            IsMultiStatementExecutable,
            IsPreparable,
            IsUseUpsert);

        hashCode = HashCode.Combine(hashCode,
            MaxParameterCount,
            GenerateFinalSemiColon,
            QuoteParameterNames,
            MaxQueriesInBatchCount,
            UseArrayParameterTreshold,
            UseInValuesTreshold);

        // Set and return the hashcode
        return this.hashCode ??= hashCode;
    }

    #endregion
}
