namespace OpenVision.Core.DataTypes;

/// <summary>
/// Represents a request for image processing operations to be performed on an input <see cref="Mat"/> object.
/// </summary>
public interface IImageRequest
{
    /// <summary>
    /// Gets the input <see cref="Mat"/> object for the image processing operations.
    /// </summary>
    public Mat Mat { get; }

    /// <summary>
    /// Gets the original width of the input image.
    /// </summary>
    int OriginalWidth { get; }

    /// <summary>
    /// Gets the original height of the input image.
    /// </summary>
    int OriginalHeight { get; }

    /// <summary>
    /// Gets a value indicating whether the image is grayscale.
    /// </summary>
    bool IsGrayscale { get; }

    /// <summary>
    /// Gets a value indicating whether the image is low resolution.
    /// </summary>
    bool IsLowResolution { get; }

    /// <summary>
    /// Gets a value indicating whether the image has a region of interest.
    /// </summary>
    bool HasRoi { get; }

    /// <summary>
    /// Gets a value indicating whether the image has a Gaussian blur.
    /// </summary>
    bool HasGaussianBlur { get; }
}