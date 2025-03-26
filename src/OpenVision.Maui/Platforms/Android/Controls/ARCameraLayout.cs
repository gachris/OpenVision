using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using OpenCV.Android;
using OpenVision.Core.Configuration;
using OpenVision.Core.DataTypes;
using OpenVision.Core.Reco;
using static OpenCV.Android.CameraBridgeViewBase;

namespace OpenVision.Maui.Controls;

/// <summary>
/// Custom layout for displaying augmented reality camera view using OpenCV on Android.
/// Implements ILoaderCallbackInterface and ICvCameraViewListener2 for OpenCV initialization
/// and camera frame handling, respectively.
/// </summary>
public class ARCameraLayout : CoordinatorLayout, ICvCameraViewListener2
{
    #region Fields/Consts

    private readonly Context _context;
    private readonly ARCamera _camera;
    private readonly CameraView _javaCameraView;
    private bool _isTrackingEnabled;
    private IRecognition? _recognition;
    private bool _trackWasFound = false;

    #endregion

    /// <summary>
    /// Initializes a new instance of the ARCameraLayout class.
    /// </summary>
    /// <param name="context">The context in which the layout is being created.</param>
    /// <param name="camera">The ARCamera instance associated with this layout.</param>
    public ARCameraLayout(Context context, ARCamera camera) : base(context)
    {
        _context = context;
        _camera = camera;

        SetBackgroundColor(Android.Graphics.Color.Black);

        var relativeLayout = new FrameLayout(_context)
        {
            LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent)
            {
                Gravity = (int)GravityFlags.Center
            }
        };

        _javaCameraView = new CameraView(_context, CameraIdAny, Orientation.PORTRAIT)
        {
            LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent),
        };

        _javaCameraView.SetCvCameraViewListener2(this);
        _javaCameraView.SetCameraPermissionGranted();

        relativeLayout.AddView(_javaCameraView);
        AddView(relativeLayout);
    }

    #region ICvCameraViewListener2 Implementation

    /// <summary>
    /// Callback method when a new camera frame is available.
    /// </summary>
    /// <param name="cvCameraViewFrame">The frame containing camera data.</param>
    /// <returns>The processed camera frame.</returns>
    Mat? ICvCameraViewListener2.OnCameraFrame(ICvCameraViewFrame? cvCameraViewFrame)
    {
        if (cvCameraViewFrame is null)
        {
            return null;
        }

        var mRgba = cvCameraViewFrame.Rgba()!;

        if (!_isTrackingEnabled)
        {
            return mRgba;
        }

        var request = VisionSystemConfig.ImageRequestBuilder.Build(mRgba);
        var featureMatchingResult = _recognition?.Match(request);
        if (featureMatchingResult?.HasMatches == true)
        {
            var targetMatch = featureMatchingResult.Matches;
            OnTrackFound(mRgba, targetMatch);
        }
        else
        {
            OnTrackLost();
        }

        return mRgba;
    }

    /// <summary>
    /// Callback method when camera view is started.
    /// </summary>
    /// <param name="p0">Width of the camera view.</param>
    /// <param name="p1">Height of the camera view.</param>
    void ICvCameraViewListener2.OnCameraViewStarted(int p0, int p1)
    {
        // Not used in this implementation
    }

    /// <summary>
    /// Callback method when camera view is stopped.
    /// </summary>
    void ICvCameraViewListener2.OnCameraViewStopped()
    {
        // Not used in this implementation
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
    /// <param name="changedView">The view whose visibility has changed.</param>
    /// <param name="visibility">The new visibility state.</param>
    protected override void OnVisibilityChanged(Android.Views.View changedView, [GeneratedEnum] ViewStates visibility)
    {
        base.OnVisibilityChanged(changedView, visibility);

        if (visibility is ViewStates.Visible)
        {
            _javaCameraView.EnableView();
        }
        else
        {
            _javaCameraView?.DisableView();
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handles actions when a target match is found.
    /// </summary>
    /// <param name="frame">The frame in which the target match was found.</param>
    /// <param name="targetMatches">Details about the matched targets.</param>
    private void OnTrackFound(Mat frame, IReadOnlyCollection<TargetMatchResult> targetMatches)
    {
        _trackWasFound = true;

        var eventArgs = new TargetMatchingEventArgs(frame, targetMatches);
        _camera.OnTrackFound(eventArgs);
    }

    /// <summary>
    /// Handles actions when no target is found during feature matching.
    /// </summary>
    private void OnTrackLost()
    {
        if (!_trackWasFound)
        {
            return;
        }

        _camera.OnTrackLost(EventArgs.Empty);
        _trackWasFound = false;
    }

    /// <summary>
    /// Updates the tracking status based on the ARCamera instance.
    /// </summary>
    internal void UpdateIsTrackingEnabled()
    {
        _isTrackingEnabled = _camera.IsTrackingEnabled;
    }

    /// <summary>
    /// Updates the recognition service based on the ARCamera instance.
    /// </summary>
    internal void UpdateRecognition()
    {
        _recognition = _camera.Recognition;
    }

    #endregion
}