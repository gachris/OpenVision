namespace OpenVision.Core.Utils;

/// <summary>
/// Helper class for unit conversions and calculations.
/// </summary>
public class UnitsHelper
{
    /// <summary>
    /// Calculates the height in units based on the given width in units and the aspect ratio of an image.
    /// </summary>
    /// <param name="xUnits">The width in units.</param>
    /// <param name="imageWidth">The width of the image.</param>
    /// <param name="imageHeight">The height of the image.</param>
    /// <returns>The calculated height in units.</returns>
    public static float CalculateYUnits(float xUnits, float imageWidth, float imageHeight)
    {
        // Calculate the aspect ratio of the image
        var aspectRatio = imageWidth / imageHeight;

        // Calculate the height in units based on the width and aspect ratio
        var yUnits = xUnits / aspectRatio;

        return yUnits;
    }
}