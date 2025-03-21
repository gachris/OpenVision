using OpenVision.Shared;
using OpenVision.Shared.Responses;

namespace OpenVision.Core.Reco.DataTypes.Responses;

/// <summary>
/// Represents the response message for a feature matching operation.
/// </summary>
public class FeatureMatchingResponse : ResponseMessage<IReadOnlyCollection<TargetMatchResponse>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureMatchingResponse"/> class with the specified values.
    /// </summary>
    /// <param name="response">The response document containing the result of the feature matching operation.</param>
    /// <param name="transactionId">The unique identifier of the transaction associated with the feature matching operation.</param>
    /// <param name="statusCode">The HTTP status code of the response.</param>
    /// <param name="errors">The collection of errors that occurred during the feature matching operation, if any.</param>
    public FeatureMatchingResponse(
        ResponseDoc<IReadOnlyCollection<TargetMatchResponse>> response,
        Guid transactionId,
        StatusCode statusCode,
        IReadOnlyCollection<Error> errors) : base(response, transactionId, statusCode, errors)
    {
    }
}