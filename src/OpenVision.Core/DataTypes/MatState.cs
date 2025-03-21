using OpenVision.Core.Utils;

namespace OpenVision.Core.DataTypes;

/// <summary>
/// A helper class for modifying the state of an image represented by the <see cref="Mat"/> class.
/// </summary>
internal class MatState
{
    #region Fields/Consts

    private bool _isGray;
    private bool _isLowResolution;
    private bool _hasRoi;
    private bool _hasGaussianBlur;
    private System.Drawing.Size _kSize;
    private double _sigmaX;
    private int _roiX;
    private int _roiY;
    private int _roiWidth;
    private int _roiHeight;
    private int _resolution;

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether the image is grayscale.
    /// </summary>
    public bool IsGray => _isGray;

    /// <summary>
    /// Gets a value indicating whether the image is low resolution.
    /// </summary>
    public bool IsLowResolution => _isLowResolution;

    /// <summary>
    /// Gets a value indicating whether the image has a region of interest.
    /// </summary>
    public bool HasRoi => _hasRoi;

    /// <summary>
    /// Gets a value indicating whether the image has a Gaussian blur.
    /// </summary>
    public bool HasGaussianBlur => _hasGaussianBlur;

    #endregion

    #region Methods

    /// <summary>
    /// Sets the image to grayscale.
    /// </summary>
    public void ToGray()
    {
        _isGray = true;
    }

    /// <summary>
    /// Sets the image to low resolution.
    /// </summary>
    /// <param name="resolution">The resolution to set.</param>
    public void LowResolution(int resolution)
    {
        _isLowResolution = true;
        _resolution = resolution;
    }

    /// <summary>
    /// Sets the image to have a Gaussian blur.
    /// </summary>
    /// <param name="kSize">The size of the Gaussian kernel.</param>
    /// <param name="sigmaX">The standard deviation in X direction.</param>
    public void GaussianBlur(System.Drawing.Size kSize, double sigmaX)
    {
        _hasGaussianBlur = true;
        _kSize = kSize;
        _sigmaX = sigmaX;
    }

    /// <summary>
    /// Sets the image to have a region of interest.
    /// </summary>
    /// <param name="roiX">The X coordinate of the top-left corner of the ROI.</param>
    /// <param name="roiY">The Y coordinate of the top-left corner of the ROI.</param>
    /// <param name="roiWidth">The width of the ROI.</param>
    /// <param name="roiHeight">The height of the ROI.</param>
    public void Roi(int roiX, int roiY, int roiWidth, int roiHeight)
    {
        _hasRoi = true;
        _roiX = roiX;
        _roiY = roiY;
        _roiWidth = roiWidth;
        _roiHeight = roiHeight;
    }

    /// <summary>
    /// Applies the changes to the specified image.
    /// </summary>
    /// <param name="mat">The image to apply the changes to.</param>
    /// <returns>The modified image.</returns>
    public Mat ApplyChanges(Mat mat)
    {
        var result = mat.Clone()!;

        if (_isGray)
            result.ToGray();

        if (_isLowResolution)
            result.LowResolution(_resolution);

        if (_hasGaussianBlur)
            result.GaussianBlur(_kSize, _sigmaX);

        if (_hasRoi)
            result.Roi(_roiX, _roiY, _roiWidth, _roiHeight);

        return result;
    }

    #endregion
}