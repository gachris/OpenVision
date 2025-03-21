namespace OpenVision.Core.Reco;

/// <summary>
/// Interface for cloud-based image recognition.
/// </summary>
public interface ICloudRecognition : IRecognition
{
    /// <summary>
    /// Initializes the cloud-based recognition service asynchronously with the specified API key.
    /// </summary>
    /// <param name="apikey">The API key required for authentication.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task InitAsync(string apikey);
}
