using OpenVision.Shared;

namespace OpenVision.Shared.Responses;

/// <summary>
/// Represents a response message returned by the API.
/// </summary>
public interface IResponseMessage
{
    /// <summary>
    /// Gets the unique identifier for the transaction.
    /// </summary>
    Guid TransactionId { get; }

    /// <summary>
    /// Gets the status code of the response.
    /// </summary>
    StatusCode StatusCode { get; }

    /// <summary>
    /// Gets the list of errors associated with the response.
    /// </summary>
    IReadOnlyCollection<Error> Errors { get; }
}

/// <summary>
/// Represents a response message with data returned by the API.
/// </summary>
/// <typeparam name="TResult">The type of the response data.</typeparam>
public interface IResponseMessage<TResult> : IResponseMessage
{
    /// <summary>
    /// Gets the response data.
    /// </summary>
    ResponseDoc<TResult> Response { get; }
}