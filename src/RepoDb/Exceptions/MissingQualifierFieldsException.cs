namespace RepoDb.Exceptions;

/// <summary>
/// An exception that is being thrown if the qualifier fields are not found from the request.
/// </summary>
public class MissingQualifierFieldsException : ArgumentException
{
    /// <summary>
    /// Creates a new instance of <see cref="MissingQualifierFieldsException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public MissingQualifierFieldsException(string message)
        : base(message) { }

    /// <inheritdoc />
    public MissingQualifierFieldsException()
    {
    }

    /// <inheritdoc />
    public MissingQualifierFieldsException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
