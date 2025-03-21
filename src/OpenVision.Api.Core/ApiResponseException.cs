using System.Net;
using OpenVision.Api.Core.Extensions;
using OpenVision.Shared.Responses;

namespace OpenVision.Api.Core;

/// <summary>
/// Represents an exception thrown by an API service.
/// </summary>
public class ApiResponseException : Exception
{
    /// <summary>
    /// Gets the name of the service related to this exception.
    /// </summary>
    public string ServiceName { get; }

    /// <summary>
    /// Gets the error message returned from the server, or <c>null</c> if unavailable.
    /// </summary>
    public ResponseMessage Error { get; }

    /// <summary>
    /// Gets the HTTP status code returned along with this error, or 0 if unavailable.
    /// </summary>
    public HttpStatusCode HttpStatusCode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiResponseException"/> class with the specified service name, error message, and HTTP status code.
    /// </summary>
    /// <param name="serviceName">The name of the service related to this exception.</param>
    /// <param name="error">The error message returned from the server.</param>
    /// <param name="httpStatusCode">The HTTP status code returned along with this error.</param>
    public ApiResponseException(string serviceName, ResponseMessage error, HttpStatusCode httpStatusCode)
        : this(serviceName, error, httpStatusCode, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiResponseException"/> class with the specified service name, error message, HTTP status code, and message.
    /// </summary>
    /// <param name="serviceName">The name of the service related to this exception.</param>
    /// <param name="error">The error message returned from the server.</param>
    /// <param name="httpStatusCode">The HTTP status code returned along with this error.</param>
    /// <param name="message">The message that describes the error.</param>
    public ApiResponseException(string serviceName, ResponseMessage error, HttpStatusCode httpStatusCode, string message)
        : this(serviceName, error, httpStatusCode, message, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiResponseException"/> class with the specified service name, error message, HTTP status code, message, and inner exception.
    /// </summary>
    /// <param name="serviceName">The name of the service related to this exception.</param>
    /// <param name="error">The error message returned from the server.</param>
    /// <param name="httpStatusCode">The HTTP status code returned along with this error.</param>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="inner">The exception that is the cause of the current exception.</param>
    public ApiResponseException(string serviceName, ResponseMessage error, HttpStatusCode httpStatusCode, string message, Exception inner)
        : base(message, inner)
    {
        serviceName.ThrowIfNullOrEmpty(nameof(serviceName));
        ServiceName = serviceName;
        Error = error;
        HttpStatusCode = httpStatusCode;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return string.Format("The service {0} has thrown an exception: {1}", ServiceName, base.ToString());
    }
}