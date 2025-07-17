namespace RepoDb.Exceptions;

/// <summary>
/// An exception that is being thrown if the target property is not found.
/// </summary>
public class PropertyNotFoundException : ArgumentException
{
    /// <summary>
    /// Creates a new instance of <see cref="PropertyNotFoundException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public PropertyNotFoundException(string? paramName, string? message)
        : base(message: message, paramName: paramName) { }
    public PropertyNotFoundException()
    {
    }
    public PropertyNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
    public PropertyNotFoundException(string message) : base(message)
    {
    }
}
