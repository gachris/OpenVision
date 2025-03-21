namespace OpenVision.Shared.Responses;

/// <summary>
/// Represents the response returned by the API when retrieving a target record from the database.
/// </summary>
public class TrackableRetrieveResponse : ResponseMessage<TargetRecordModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TrackableRetrieveResponse"/> class with the specified target record, status, transaction ID, and status code.
    /// </summary>
    /// <param name="response">The target record retrieved from the database.</param>
    /// <param name="transactionId">The unique identifier for the API transaction.</param>
    /// <param name="statusCode">The status code that indicates the success or failure of the API transaction.</param>
    /// <param name="errors">The list of errors associated with the response.</param>
    public TrackableRetrieveResponse(ResponseDoc<TargetRecordModel> response,
                                     Guid transactionId,
                                     StatusCode statusCode,
                                     IReadOnlyCollection<Error> errors) : base(response, transactionId, statusCode, errors)
    {
    }
}