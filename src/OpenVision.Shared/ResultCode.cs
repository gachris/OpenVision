namespace OpenVision.Shared;

/// <summary>
/// Represents the result code for an API operation.
/// </summary>
public enum ResultCode
{
    /// <summary>
    /// The operation was successful.
    /// </summary>
    Success,

    /// <summary>
    /// The request data was invalid.
    /// </summary>
    ValidationError,

    /// <summary>
    /// The request was invalid.
    /// </summary>
    InvalidRequest,

    /// <summary>
    /// The request was unauthorized.
    /// </summary>
    Unauthorized,

    /// <summary>
    /// The request was forbidden.
    /// </summary>
    Forbidden,

    /// <summary>
    /// The requested record was not found.
    /// </summary>
    RecordNotFound,

    /// <summary>
    /// An internal server error occurred.
    /// </summary>
    InternalServerError,

    /// <summary>
    /// An unknown error occurred.
    /// </summary>
    UnknownError
}