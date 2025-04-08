using Emgu.CV;
using OpenVision.Core.DataTypes;

namespace OpenVision.Server.Core.Requests;

/// <inheritdoc/>
public class CloudImageRequest : IImageRequest
{
    /// <inheritdoc/>
    public string Id { get; }

    /// <inheritdoc/>
    public Mat Mat { get; }

    /// <inheritdoc/>
    public int OriginalWidth { get; }

    /// <inheritdoc/>
    public int OriginalHeight { get; }

    /// <inheritdoc/>
    public bool IsGrayscale { get; }

    /// <inheritdoc/>
    public bool IsLowResolution { get; }

    /// <inheritdoc/>
    public bool HasRoi { get; }

    /// <inheritdoc/>
    public bool HasGaussianBlur { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CloudImageRequest"/> class.
    /// </summary>
    /// <param name="id">The ID of the image request.</param>
    /// <param name="mat">The Emgu.CV.Mat object representing the image data.</param>
    /// <param name="originalWidth">The original width of the image.</param>
    /// <param name="originalHeight">The original height of the image.</param>
    /// <param name="isGrayscale">Indicates if the image is in grayscale.</param>
    /// <param name="isLowResolution">Indicates if the image is low resolution.</param>
    /// <param name="hasRoi">Indicates if the image has a Region of Interest (ROI).</param>
    /// <param name="hasGaussianBlur">Indicates if the image has Gaussian blur applied.</param>
    public CloudImageRequest(
        string id,
        Mat mat,
        int originalWidth,
        int originalHeight,
        bool isGrayscale,
        bool isLowResolution,
        bool hasRoi,
        bool hasGaussianBlur)
    {
        Id = id;
        Mat = mat;
        OriginalWidth = originalWidth;
        OriginalHeight = originalHeight;
        IsGrayscale = isGrayscale;
        IsLowResolution = isLowResolution;
        HasRoi = hasRoi;
        HasGaussianBlur = hasGaussianBlur;
    }
}