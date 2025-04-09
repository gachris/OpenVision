namespace OpenVision.Shared.Responses;

/// <summary>
/// Represents the response returned from the API when a new trackable is created.
/// </summary>
public class PostTrackableResponse : ResponseMessage<string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PostTrackableResponse"/> class with the specified target ID, transaction ID, and status code.
    /// </summary>
    /// <param name="response">The unique identifier assigned to the newly created trackable.</param>
    /// <param name="transactionId">The unique identifier for the API transaction.</param>
    /// <param name="statusCode">The status code that indicates the success or failure of the API transaction.</param>
    /// <param name="errors">The list of errors associated with the response.</param>
    public PostTrackableResponse(
        ResponseDoc<string> response,
        Guid transactionId,
        StatusCode statusCode,
        IReadOnlyCollection<Error> errors) : base(response, transactionId, statusCode, errors)
    {
    }
}