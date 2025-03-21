using OpenVision.Core.Dataset;
using OpenVision.Core.Reco.DataTypes;

namespace OpenVision.Core.Reco;

/// <summary>
/// Interface for image recognition services, providing initialization methods for different data sources.
/// </summary>
public interface IImageRecognition : IRecognition
{
    /// <summary>
    /// Asynchronously initializes the image recognition service using a collection of images.
    /// </summary>
    /// <param name="images">The collection of images to be used for initialization.</param>
    /// <returns>A task representing the asynchronous initialization operation.</returns>
    Task InitAsync(IEnumerable<ImageData> images);

    /// <summary>
    /// Asynchronously initializes the image recognition service using a target dataset.
    /// </summary>
    /// <param name="dataset">The target dataset containing images and associated metadata.</param>
    /// <returns>A task representing the asynchronous initialization operation.</returns>
    Task InitAsync(TargetDataset dataset);

    /// <summary>
    /// Asynchronously initializes the image recognition service using a collection of target match queries.
    /// </summary>
    /// <param name="targets">The collection of target match queries to initialize the service with.</param>
    /// <returns>A task representing the asynchronous initialization operation.</returns>
    Task InitAsync(IEnumerable<Target> targets);
}
