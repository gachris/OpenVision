using System.Drawing;
using Emgu.CV;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace OpenVision.Server.Core.Helpers;

/// <summary>
/// Provides utility methods for working with 2D features.
/// </summary>
internal static class Features2dHelper
{
    /// <summary>
    /// Draws keypoints on an input matrix.
    /// </summary>
    /// <param name="inputMat">The input matrix containing the image.</param>
    /// <param name="keypoints">The vector of keypoints to be drawn.</param>
    /// <param name="color">The color to be used for drawing the keypoints.</param>
    /// <returns>A new matrix with the keypoints drawn on the input image.</returns>
    public static Mat DrawKeypoints(Mat inputMat, VectorOfKeyPoint keypoints, Color color)
    {
        var outMat = new Mat();
        Features2DToolbox.DrawKeypoints(inputMat, keypoints, outMat, new Bgr(color.B, color.G, color.R));
        return outMat;
    }
}