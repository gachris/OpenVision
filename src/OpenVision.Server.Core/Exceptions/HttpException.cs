using OpenVision.Shared.Responses;

namespace OpenVision.Server.Core.Exceptions;

/// <summary>
/// Represents an HTTP exception that includes an error response message.
/// </summary>
public class HttpException : Exception
{
    /// <summary>
    /// Gets the error response message associated with the exception.
    /// </summary>
    public ResponseMessage ErrorResponseMessage { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpException"/> class with the specified error response message.
    /// </summary>
    /// <param name="message">The error response message to associate with the exception.</param>
    public HttpException(ResponseMessage message)
    {
        ErrorResponseMessage = message;
    }
}