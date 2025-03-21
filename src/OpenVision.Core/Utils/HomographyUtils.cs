using OpenVision.Core.DataTypes;

namespace OpenVision.Core.Utils;

/// <summary>
/// Provides utility methods for working with homography matrices.
/// </summary>
internal static class HomographyUtils
{
    /// <summary>
    /// Converts a <see cref="HomographyResult"/> to a <see cref="TargetMatchResult"/>.
    /// </summary>
    /// <param name="homographyResult">The <see cref="HomographyResult"/> to convert.</param>
    /// <param name="request">The <see cref="IImageRequest"/> associated with this match request.</param>
    /// <param name="queryInfo">The <see cref="TargetMatchQuery"/> associated with the query image.</param>
    /// <param name="trainInfo">The <see cref="TargetMatchQuery"/> associated with the training image.</param>
    /// <returns>A <see cref="TargetMatchResult"/> representing the result of the homography computation.</returns>
    public static TargetMatchResult ToTargetMatchResult(this HomographyResult homographyResult,
                                                        IImageRequest request,
                                                        TargetMatchQuery queryInfo,
                                                        TargetMatchQuery trainInfo)
    {
        if (!homographyResult.MatchFound)
        {
            throw new Exception("Homography is empty!");
        }
        var perspectiveTransform = PerspectiveTransform(homographyResult.Homography!, trainInfo.Mat);
        perspectiveTransform.region = Upscale(perspectiveTransform.region, request.OriginalWidth, request.OriginalHeight, queryInfo.Mat);

        MinAreaRect(perspectiveTransform.region, out var xCenter, out var yCenter, out var angle, out var rectSize);

#if ANDROID
        var projectedRegion = Array.ConvertAll(perspectiveTransform.region.ToArray(), p => new System.Drawing.PointF((float)p.X, (float)p.Y));
#else
        var projectedRegion = perspectiveTransform.region;
#endif
        return new TargetMatchResult(trainInfo.Id, projectedRegion, xCenter, yCenter, perspectiveTransform.angle, rectSize, perspectiveTransform.homographyArray);
    }

    /// <summary>
    /// Computes the perspective transform of a set of points using a homography matrix.
    /// </summary>
    /// <param name="homography">The homography matrix to use for the transform.</param>
    /// <param name="mat">The <see cref="Mat"/> containing the points to transform.</param>
    /// <returns>An array of transformed points.</returns>
#if ANDROID
    private static (float angle, Point[] region, Mat homographyArray) PerspectiveTransform(Mat homography, Mat mat)
    {
        var size = mat.Size()!;
        //draw a rectangle along the projected model
        var rect = new System.Drawing.Rectangle(System.Drawing.Point.Empty, new System.Drawing.Size((int)size.Width, (int)size.Height));
        var projectedRegion = new MatOfPoint2f(
        [
            new Point(rect.Left, rect.Bottom),
            new Point(rect.Right, rect.Bottom),
            new Point(rect.Right, rect.Top),
            new Point(rect.Left, rect.Top)
        ]);

        OpenCV.Core.Core.PerspectiveTransform(projectedRegion, projectedRegion, homography);

        return (0, projectedRegion.ToArray()!, homography);
    }
#else
    private static (float angle, System.Drawing.PointF[] region, Mat homographyArray) PerspectiveTransform(Mat homography, Mat mat)
    {
        var rect = new System.Drawing.Rectangle(System.Drawing.Point.Empty, mat.Size);
        var projectedRegion = new System.Drawing.PointF[]
        {
            new System.Drawing.PointF(rect.Left, rect.Bottom),
            new System.Drawing.PointF(rect.Right, rect.Bottom),
            new System.Drawing.PointF(rect.Right, rect.Top),
            new System.Drawing.PointF(rect.Left, rect.Top)
        };

        var y = homography.GetData().GetValue(1, 0) as double? ?? 0;
        var x = homography.GetData().GetValue(0, 0) as double? ?? 0;

        var angle = Math.Atan2(y, x) * 180 / Math.PI;

        return ((float)angle, CvInvoke.PerspectiveTransform(projectedRegion, homography), homography);
    }
#endif

    /// <summary>
    /// Computes the minimum area rectangle that bounds a set of points.
    /// </summary>
    /// <param name="projectedRegion">The set of points to compute the minimum area rectangle for.</param>
    /// <param name="xCenter">The x-coordinate of the center of the minimum area rectangle.</param>
    /// <param name="yCenter">The y-coordinate of the center of the minimum area rectangle.</param>
    /// <param name="angle">The rotation angle of the minimum area rectangle, in degrees.</param>
    /// <param name="size">The size of the minimum area rectangle.</param>
#if ANDROID
    private static void MinAreaRect(Point[] projectedRegion,
                                    out float xCenter,
                                    out float yCenter,
                                    out float angle,
                                    out System.Drawing.SizeF size)
    {
        var rotatedRect = Imgproc.MinAreaRect(new MatOfPoint2f(projectedRegion))!;
        angle = (float)rotatedRect.Angle;
        xCenter = (float)rotatedRect.Center!.X;
        yCenter = (float)rotatedRect.Center!.Y;
        size = new System.Drawing.SizeF((float)rotatedRect.Size!.Width, (float)rotatedRect.Size!.Height);
    }
#else
    private static void MinAreaRect(System.Drawing.PointF[] projectedRegion,
                                    out float xCenter,
                                    out float yCenter,
                                    out float angle,
                                    out System.Drawing.SizeF size)
    {
        var rotatedRect = CvInvoke.MinAreaRect(projectedRegion);
        angle = rotatedRect.Angle;
        xCenter = rotatedRect.Center.X;
        yCenter = rotatedRect.Center.Y;
        size = rotatedRect.Size;
    }
#endif

    /// <summary>
    /// Upscales a set of points based on the dimensions of the original image and the dimensions of the low-resolution image used for the homography computation.
    /// </summary>
    /// <param name="points">The set of points to upscale.</param>
    /// <param name="originalWidth">The width of the original image.</param>
    /// <param name="originalHeight">The height of the original image.</param>
    /// <param name="mat">The <see cref="Mat"/> containing the low-resolution image used for the homography computation.</param>
    /// <returns>An array of upscaled points.</returns>
#if ANDROID
    private static Point[] Upscale(Point[] points,
                                   int originalWidth,
                                   int originalHeight,
                                   Mat mat)
    {
        var lowResolutionWidth = mat.Width();
        var lowResolutionHeight = mat.Height();

        var scalingFactorX = (float)originalWidth / lowResolutionWidth;
        var scalingFactorY = (float)originalHeight / lowResolutionHeight;

        var scaledPoints = new Point[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            var x = points[i].X * scalingFactorX;
            var y = points[i].Y * scalingFactorY;
            scaledPoints[i] = new Point(x, y);
        }

        return scaledPoints;
    }
#else
    private static System.Drawing.PointF[] Upscale(System.Drawing.PointF[] points,
                                                   int originalWidth,
                                                   int originalHeight,
                                                   Mat mat)
    {
        var lowResolutionWidth = mat.Width;
        var lowResolutionHeight = mat.Height;

        var scalingFactorX = (float)originalWidth / lowResolutionWidth;
        var scalingFactorY = (float)originalHeight / lowResolutionHeight;

        var scaledPoints = new System.Drawing.PointF[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            var x = points[i].X * scalingFactorX;
            var y = points[i].Y * scalingFactorY;
            scaledPoints[i] = new System.Drawing.PointF(x, y);
        }

        return scaledPoints;
    }
#endif
}
