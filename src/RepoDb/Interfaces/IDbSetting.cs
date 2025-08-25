using System.Data.Common;

namespace RepoDb.Interfaces;

/// <summary>
/// An interface that is used to mark a class to be a database setting object.
/// </summary>
public interface IDbSetting
{
    /// <summary>
    /// Gets the value that indicates whether the table hints are supported.
    /// </summary>
    bool AreTableHintsSupported { get; }

    /// <summary>
    /// Gets the default averageable .NET CLR types for the database.
    /// </summary>
    Type AverageableType { get; }

    /// <summary>
    /// Gets the character used for closing quote.
    /// </summary>
    string ClosingQuote { get; }

    /// <summary>
    /// Gets the default schema of the database.
    /// </summary>
    string? DefaultSchema { get; }

    /// <summary>
    /// Gets a value that indicates whether setting the value of <see cref="DbParameter.Direction"/> object is supported.
    /// </summary>
    bool IsDirectionSupported { get; }

    /// <summary>
    /// Gets a value that indicates whether the <see cref="DbCommand"/> object must be disposed after calling the <see cref="DbCommand.ExecuteReader()"/> method.
    /// </summary>
    bool IsExecuteReaderDisposable { get; }

    /// <summary>
    /// Gets a value whether the multiple statement execution is supported.
    /// </summary>
    bool IsMultiStatementExecutable { get; }

    /// <summary>
    /// Gets a value that indicates whether the current DB Provider supports the <see cref="DbCommand.Prepare()"/> calls.
    /// </summary>
    bool IsPreparable { get; }

    /// <summary>
    /// Gets the character used for opening quote.
    /// </summary>
    string OpeningQuote { get; }

    /// <summary>
    /// Gets the character used for the database command parameter prefixing.
    /// </summary>
    string ParameterPrefix { get; }

    /// <summary>
    /// This database provider supports a maximum of this number of parameters
    /// </summary>
    /// <remarks>SqlServer supports 2100-2 = 2098 parameters</remarks>
    int MaxParameterCount { get; }

    /// <summary>
    /// This database provider supports a maximum of this number of item in a batch operation
    /// </summary>
    int MaxQueriesInBatchCount { get; }

    /// <summary>
    /// This database provider allows a final semicolon in the command text
    /// </summary>
    bool GenerateFinalSemiColon { get; }

    /// <summary>
    /// Quote parameter names
    /// </summary>
    bool QuoteParameterNames { get; }

    int? UseInValuesTreshold { get; }

    /// <summary>
    /// Gets or initializes the threshold value that determines when table parameters should be used.
    /// </summary>
    int? UseArrayParameterTreshold { get; }

    int MaxArrayParameterValueCount { get; }
}
