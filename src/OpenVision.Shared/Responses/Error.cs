using OpenVision.Shared.Types;

namespace OpenVision.Shared.Responses;

/// <summary>
/// Represents an error that occurred during the processing of a request.
/// </summary>
public record Error
{
    /// <summary>
    /// Gets the result code associated with the error.
    /// </summary>
    public virtual ResultCode ResultCode { get; }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    public virtual string Message { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> class.
    /// </summary>
    /// <param name="resultCode">The result code associated with the error.</param>
    /// <param name="message">The error message.</param>
    public Error(ResultCode resultCode, string message)
    {
        ResultCode = resultCode;
        Message = message;
    }
}