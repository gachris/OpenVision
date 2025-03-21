namespace OpenVision.Api.Core;

/// <summary>
/// Represents an exception thrown by an API service.
/// </summary>
public class ApiServiceException : Exception
{
    /// <summary>
    /// Gets the name of the service related to this exception.
    /// </summary>
    public string ServiceName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiServiceException"/> class with the specified service name and message.
    /// </summary>
    /// <param name="serviceName">The name of the service related to this exception.</param>
    /// <param name="message">The message that describes the error.</param>
    public ApiServiceException(string serviceName, string message) : this(serviceName, message, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiServiceException"/> class with the specified service name, message, and inner exception.
    /// </summary>
    /// <param name="serviceName">The name of the service related to this exception.</param>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="inner">The exception that is the cause of the current exception, or <c>null</c> if no inner exception is specified.</param>
    public ApiServiceException(string serviceName, string message, Exception inner)
        : base(message, inner)
    {
        ServiceName = serviceName;
    }

    /// <inheritdoc/>
    public override string ToString() => string.Format("The service {0} has thrown an exception: {1}", ServiceName, base.ToString());
}