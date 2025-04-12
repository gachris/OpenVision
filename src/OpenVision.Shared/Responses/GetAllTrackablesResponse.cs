using OpenVision.Shared.Types;

namespace OpenVision.Shared.Responses;

/// <summary>
/// Represents a response message containing a collection of <see cref="TargetRecordModel"/> objects returned by the API.
/// </summary>
public record GetAllTrackablesResponse : ResponseMessage<IReadOnlyCollection<TargetRecordModel>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllTrackablesResponse"/> class with the specified response payload, transaction ID, and status code.
    /// </summary>
    /// <param name="response">The response payload of type <see cref="IReadOnlyCollection{T}"/> of <see cref="TargetRecordModel"/> objects.</param>
    /// <param name="transactionId">The unique identifier for the API transaction.</param>
    /// <param name="statusCode">The status code that indicates the success or failure of the API transaction.</param>
    /// <param name="errors">The list of errors associated with the response.</param>
    public GetAllTrackablesResponse(ResponseDoc<IReadOnlyCollection<TargetRecordModel>> response,
                                    Guid transactionId,
                                    StatusCode statusCode,
                                    IReadOnlyCollection<Error> errors) : base(response, transactionId, statusCode, errors)
    {
    }
}