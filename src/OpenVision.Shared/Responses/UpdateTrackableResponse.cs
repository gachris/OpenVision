using OpenVision.Shared.Types;

namespace OpenVision.Shared.Responses;

/// <summary>
/// Represents a response message indicating the successful update of a trackable.
/// </summary>
public record UpdateTrackableResponse : ResponseMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateTrackableResponse"/> class with the specified transaction ID and status code.
    /// </summary>
    /// <param name="transactionId">The unique identifier for the API transaction.</param>
    /// <param name="statusCode">The status code that indicates the success or failure of the API transaction.</param>
    /// <param name="errors">The list of errors associated with the response.</param>
    public UpdateTrackableResponse(Guid transactionId,
                                   StatusCode statusCode,
                                   IReadOnlyCollection<Error> errors) : base(transactionId, statusCode, errors)
    {
    }
}