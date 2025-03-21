using System.Text.Json.Serialization;

namespace OpenVision.Shared.Responses;

/// <summary>
/// Represents the base class for all API response messages.
/// </summary>
public class ResponseMessage : IResponseMessage
{
    /// <summary>
    /// Gets the unique identifier for the API transaction.
    /// </summary>
    [JsonPropertyName("transaction_id")]
    public virtual Guid TransactionId { get; }

    /// <summary>
    /// Gets the status code that indicates the success or failure of the API transaction.
    /// </summary>
    [JsonPropertyName("status_code")]
    public virtual StatusCode StatusCode { get; }

    /// <summary>
    /// Gets the list of errors associated with the response.
    /// </summary>
    [JsonPropertyName("errors")]
    public virtual IReadOnlyCollection<Error> Errors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponseMessage"/> class with the specified transaction ID and status code.
    /// </summary>
    /// <param name="transactionId">The unique identifier for the API transaction.</param>
    /// <param name="statusCode">The status code that indicates the success or failure of the API transaction.</param>
    /// <param name="errors">The list of errors associated with the response.</param>
    public ResponseMessage(Guid transactionId,
                           StatusCode statusCode,
                           IReadOnlyCollection<Error> errors)
    {
        TransactionId = transactionId;
        StatusCode = statusCode;
        Errors = errors;
    }
}

/// <summary>
/// Represents an API response message that contains a response payload of type <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TResult">The type of the response data.</typeparam>
public class ResponseMessage<TResult> : ResponseMessage, IResponseMessage, IResponseMessage<TResult>
{
    /// <summary>
    /// Gets the response payload of type <typeparamref name="TResult"/>.
    /// </summary>
    [JsonPropertyName("response")]
    public virtual ResponseDoc<TResult> Response { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponseMessage{TResult}"/> class with the specified response payload, transaction ID, and status code.
    /// </summary>
    /// <param name="response">The response payload of type <typeparamref name="TResult"/>.</param>
    /// <param name="transactionId">The unique identifier for the API transaction.</param>
    /// <param name="statusCode">The status code that indicates the success or failure of the API transaction.</param>
    /// <param name="errors">The list of errors associated with the response.</param>
    public ResponseMessage(ResponseDoc<TResult> response,
                           Guid transactionId,
                           StatusCode statusCode,
                           IReadOnlyCollection<Error> errors) : base(transactionId, statusCode, errors)
    {
        Response = response;
    }
}