using Android.Content;
using Android.Graphics;
using Java.Lang;
using OpenCV.Android;
using OpenCV.ImgProc;

namespace OpenVision.Maui.Controls;

internal enum Orientation
{
    LANDSCAPE_LEFT,
    PORTRAIT,
    LANDSCAPE_RIGHT
}

internal class CameraView : JavaCameraView
{
    private class CvCameraViewFrameImplHack : Java.Lang.Object, ICvCameraViewFrame
    {
        private readonly Mat _rgbMat;

        public CvCameraViewFrameImplHack(Mat rgbMat)
        {
            _rgbMat = rgbMat;
        }

        public Mat Rgba()
        {
            return _rgbMat;
        }

        public Mat Gray()
        {
            var grayMat = new Mat();
            Imgproc.CvtColor(_rgbMat, grayMat, Imgproc.ColorBgr2gray);
            return grayMat;
        }
    }

    public Orientation Orientation { get; private set; }

    public CameraView(Context context, int cameraId, Orientation orientation) : base(context, cameraId)
    {
        Orientation = orientation;
    }

    protected override void AllocateCache()
    {
        if (IsLandscape(Orientation))
        {
            base.AllocateCache();
            return;
        }

        try
        {
            var privateField = Class.FromType(typeof(CameraBridgeViewBase)).GetDeclaredField("mCacheBitmap");
            privateField.Accessible = true;

            var bitmap = Bitmap.CreateBitmap(FrameHeight, FrameWidth, Bitmap.Config.Argb8888!);

            privateField.Set(this, bitmap);
        }
        catch (NoSuchFieldException e)
        {
            throw new RuntimeException(e);
        }
        catch (IllegalAccessException e)
        {
            throw new RuntimeException(e);
        }
    }

    private static Mat RotateToPortrait(Mat mat)
    {
        var transposed = mat.T()!;
        var flipped = new Mat();
        OpenCV.Core.Core.Flip(transposed, flipped, 1);
        transposed.Release();
        return flipped;
    }

    private static Mat RotateToLandscapeRight(Mat mat)
    {
        var flipped = new Mat();
        OpenCV.Core.Core.Flip(mat, flipped, -1);
        return flipped;
    }

    protected override void DeliverAndDrawFrame(ICvCameraViewFrame? frame)
    {
        if (frame is null)
        {
            base.DeliverAndDrawFrame(frame);
            return;
        }

        Mat? rotated;
        var frameMat = frame.Rgba()!;

        if (IsLandscape(Orientation))
        {
            rotated = IsLandscapeRight(Orientation) ? RotateToLandscapeRight(frameMat) : frameMat;
            Scale = Width / frameMat.Width();
        }
        else
        {
            rotated = RotateToPortrait(frameMat);
            Scale = Height / rotated.Height();
        }

        var hackFrame = new CvCameraViewFrameImplHack(rotated);

        base.DeliverAndDrawFrame(hackFrame);
    }

    private static bool IsLandscape(Orientation orientation)
    {
        return orientation is Orientation.LANDSCAPE_LEFT or Orientation.LANDSCAPE_RIGHT;
    }

    private static bool IsLandscapeRight(Orientation orientation)
    {
        return orientation == Orientation.LANDSCAPE_RIGHT;
    }
}