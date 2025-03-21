using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using OpenCV.Android;
using OpenCV.ImgCodecs;
using static OpenCV.Android.CameraBridgeViewBase;

namespace OpenVision.Maui.Controls;

/// <summary>
/// Custom layout class for displaying a camera view with OpenCV integration in a Maui Android app.
/// Implements ILoaderCallbackInterface and ICvCameraViewListener2 for OpenCV initialization
/// and camera frame handling, respectively.
/// </summary>
public class CameraLayout : CoordinatorLayout, ICvCameraViewListener2
{
    #region Fields/Consts

    private readonly Context _context;
    private readonly Camera _camera;
    private readonly CameraView _javaCameraView;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="CameraLayout"/> class.
    /// </summary>
    /// <param name="context">The Android context.</param>
    /// <param name="camera">The Maui Camera instance.</param>
    public CameraLayout(Context context, Camera camera) : base(context)
    {
        _context = context;
        _camera = camera;

        SetBackgroundColor(Android.Graphics.Color.Black);

        var relativeLayout = new RelativeLayout(_context)
        {
            LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent)
            {
                Gravity = (int)GravityFlags.Center
            }
        };

        _javaCameraView = new CameraView(_context, CameraIdAny, Orientation.PORTRAIT)
        {
            LayoutParameters = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent),
        };

        _javaCameraView.SetCvCameraViewListener2(this);
        _javaCameraView.SetCameraPermissionGranted();

        relativeLayout.AddView(_javaCameraView);
        AddView(relativeLayout);
    }

    #region ICvCameraViewListener Implementation

    /// <summary>
    /// Callback when a new camera frame is available.
    /// </summary>
    /// <param name="cvCameraViewFrame">The OpenCV camera frame.</param>
    /// <returns>The modified camera frame.</returns>
    Mat? ICvCameraViewListener2.OnCameraFrame(ICvCameraViewFrame? cvCameraViewFrame)
    {
        if (cvCameraViewFrame is null)
        {
            return null;
        }

        var mRgba = cvCameraViewFrame.Rgba();
        var matOfByte = new MatOfByte();
        var imencode = Imgcodecs.Imencode(".jpg", mRgba, matOfByte);

        if (imencode)
        {
            var bytes = matOfByte.ToArray();

            if (bytes != null)
            {
                OnFrameChanged(bytes);
            }
        }

        return mRgba;
    }

    /// <summary>
    /// Callback when the camera view is started.
    /// </summary>
    /// <param name="p0">Camera width.</param>
    /// <param name="p1">Camera height.</param>
    void ICvCameraViewListener2.OnCameraViewStarted(int p0, int p1)
    {
    }

    /// <summary>
    /// Callback when the camera view is stopped.
    /// </summary>
    void ICvCameraViewListener2.OnCameraViewStopped()
    {
    }

    #endregion

    #region Methods Override

    /// <summary>
    /// Called when the view is attached to the window.
    /// </summary>
    public override void OnAttachedToWindow()
    {
        base.OnAttachedToWindow();

        OpenCVLoader.InitLocal();
    }

    /// <summary>
    /// Called when the view is detached from the window.
    /// </summary>
    public override void OnDetachedFromWindow()
    {
        base.OnDetachedFromWindow();

        _javaCameraView.DisableView();
    }

    /// <summary>
    /// Called when the visibility of the view changes.
    /// </summary>
    /// <param name="changedView">The view whose visibility changed.</param>
    /// <param name="visibility">The new visibility state.</param>
    protected override void OnVisibilityChanged(Android.Views.View changedView, [GeneratedEnum] ViewStates visibility)
    {
        base.OnVisibilityChanged(changedView, visibility);

        if (visibility is ViewStates.Visible)
        {
            _javaCameraView?.EnableView();
        }
        else
        {
            _javaCameraView?.DisableView();
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Notifies the Maui Camera instance about a new camera frame.
    /// </summary>
    /// <param name="frame">The captured camera frame data.</param>
    private void OnFrameChanged(byte[] frame)
    {
        _camera.OnFrameChanged(frame);
    }

    #endregion
}
