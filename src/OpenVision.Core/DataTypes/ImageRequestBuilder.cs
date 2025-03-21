namespace OpenVision.Core.DataTypes;

/// <summary>
/// A builder class for creating image requests with various modifications applied to the image.
/// </summary>
public sealed class ImageRequestBuilder
{
    private readonly MatState _state;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageRequestBuilder"/> class.
    /// </summary>
    public ImageRequestBuilder()
    {
        _state = new MatState();
    }

    /// <summary>
    /// Sets the image to grayscale.
    /// </summary>
    /// <returns>The current <see cref="ImageRequestBuilder"/> instance.</returns>
    public ImageRequestBuilder WithGrayscale()
    {
        _state.ToGray();
        return this;
    }

    /// <summary>
    /// Sets the image to low resolution.
    /// </summary>
    /// <param name="resolution">The resolution to set.</param>
    /// <returns>The current <see cref="ImageRequestBuilder"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown if the resolution is less than or equal to zero.</exception>
    public ImageRequestBuilder WithLowResolution(int resolution)
    {
        if (resolution <= 0)
            throw new ArgumentException("Resolution must be greater than zero.", nameof(resolution));

        _state.LowResolution(resolution);
        return this;
    }

    /// <summary>
    /// Sets the image to have a Gaussian blur.
    /// </summary>
    /// <param name="kSize">The size of the Gaussian kernel.</param>
    /// <param name="sigmaX">The standard deviation in X direction.</param>
    /// <returns>The current <see cref="ImageRequestBuilder"/> instance.</returns>
    public ImageRequestBuilder WithGaussianBlur(System.Drawing.Size kSize, double sigmaX)
    {
        if (sigmaX < 0)
            throw new ArgumentException("SigmaX must be greater than zero.", nameof(sigmaX));

        _state.GaussianBlur(kSize, sigmaX);
        return this;
    }

    /// <summary>
    /// Sets the image to have a region of interest.
    /// </summary>
    /// <param name="roiX">The X coordinate of the top-left corner of the ROI.</param>
    /// <param name="roiY">The Y coordinate of the top-left corner of the ROI.</param>
    /// <param name="roiWidth">The width of the ROI.</param>
    /// <param name="roiHeight">The height of the ROI.</param>
    /// <returns>The current <see cref="ImageRequestBuilder"/> instance.</returns>
    public ImageRequestBuilder WithRoi(int roiX, int roiY, int roiWidth, int roiHeight)
    {
        _state.Roi(roiX, roiY, roiWidth, roiHeight);
        return this;
    }

    /// <summary>
    /// Builds an <see cref="IImageRequest"/> instance with the specified modifications applied to the input <see cref="Mat"/> object.
    /// </summary>
    /// <param name="mat">The input <see cref="Mat"/> object.</param>
    /// <returns>An <see cref="IImageRequest"/> instance.</returns>
    public IImageRequest Build(Mat mat)
    {
#if ANDROID
        var originalWidth = mat.Width();
        var originalHeight = mat.Height();
#else
        var originalWidth = mat.Width;
        var originalHeight = mat.Height;
#endif

        var modifiedMat = _state.ApplyChanges(mat.Clone()!);

        return new ImageRequest(modifiedMat,
                                originalWidth,
                                originalHeight,
                                isGrayscale: _state.IsGray,
                                isLowResolution: _state.IsLowResolution,
                                hasRoi: _state.HasRoi,
                                hasGaussianBlur: _state.HasGaussianBlur);
    }
}
