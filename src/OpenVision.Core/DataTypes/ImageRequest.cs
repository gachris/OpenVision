namespace OpenVision.Core.DataTypes;

/// <summary>
/// Represents a request for image processing operations to be performed on an input <see cref="Mat"/> object.
/// </summary>
public class ImageRequest : IImageRequest
{
    #region Properties

    /// <summary>
    /// Gets the input <see cref="Mat"/> object for the image processing operations.
    /// </summary>
    public Mat Mat { get; }

    /// <summary>
    /// Gets the original width of the input image.
    /// </summary>
    public int OriginalWidth { get; }

    /// <summary>
    /// Gets the original height of the input image.
    /// </summary>
    public int OriginalHeight { get; }

    /// <summary>
    /// Gets a value indicating whether the image is grayscale.
    /// </summary>
    public bool IsGrayscale { get; }

    /// <summary>
    /// Gets a value indicating whether the image is low resolution.
    /// </summary>
    public bool IsLowResolution { get; }

    /// <summary>
    /// Gets a value indicating whether the image has a region of interest.
    /// </summary>
    public bool HasRoi { get; }

    /// <summary>
    /// Gets a value indicating whether the image has a Gaussian blur.
    /// </summary>
    public bool HasGaussianBlur { get; }

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageRequest"/> class with the specified input <see cref="Mat"/> object and processing options.
    /// </summary>
    /// <param name="mat">The input <see cref="Mat"/> object for the image processing operations.</param>
    /// <param name="originalWidth">The original width of the input image.</param>
    /// <param name="originalHeight">The original height of the input image.</param>
    /// <param name="isGrayscale">A value indicating whether the image is grayscale.</param>
    /// <param name="isLowResolution">A value indicating whether the image is low resolution.</param>
    /// <param name="hasRoi">A value indicating whether the image has a region of interest.</param>
    /// <param name="hasGaussianBlur">A value indicating whether the image has a Gaussian blur.</param>
    internal ImageRequest(Mat mat,
                          int originalWidth,
                          int originalHeight,
                          bool isGrayscale,
                          bool isLowResolution,
                          bool hasRoi,
                          bool hasGaussianBlur)
    {
        Mat = mat;
        OriginalWidth = originalWidth;
        OriginalHeight = originalHeight;
        IsGrayscale = isGrayscale;
        IsLowResolution = isLowResolution;
        HasRoi = hasRoi;
        HasGaussianBlur = hasGaussianBlur;
    }
}