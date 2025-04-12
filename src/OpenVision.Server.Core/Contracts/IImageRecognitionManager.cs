using OpenVision.Core.Reco.DataTypes.Requests;
using OpenVision.Core.Reco.DataTypes.Responses;

namespace OpenVision.Server.Core.Contracts;

public interface IImageRecognitionManager
{
    Task InitializeAsync();

    FeatureMatchingResponse MatchFeatures(WSMatchRequest matchRequest);
}