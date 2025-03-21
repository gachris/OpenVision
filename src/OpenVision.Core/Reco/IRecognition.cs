using OpenVision.Core.DataTypes;

namespace OpenVision.Core.Reco;

/// <summary>
/// Interface for recognition services.
/// </summary>
public interface IRecognition
{
    /// <summary>
    /// Gets a value indicating whether the recognition service is ready.
    /// </summary>
    bool IsReady { get; }

    /// <summary>
    /// Matches features in the provided image request against predefined targets.
    /// </summary>
    /// <param name="request">The image request containing the image to match.</param>
    /// <returns>A result containing information about matched targets.</returns>
    FeatureMatchingResult Match(IImageRequest request);
}