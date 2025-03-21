using Emgu.CV;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using OpenVision.Core.DataTypes;
using OpenVision.Core.Reco;

namespace OpenVision.WinUI.Controls;

/// <summary>
/// ARCamera for displaying AR camera feed and handling AR tracking.
/// </summary>
public class ARCamera : Control, IDisposable
{
    #region Fields/Consts

    private static readonly double SigmaX = 0;
    private static readonly int ImageLowResolution = 160;
    private static readonly System.Drawing.Size KSize = new(5, 5);

    private readonly ImageRequestBuilder _imageRequestBuilder;

    private Image? _frameImage;
    private Grid? _grid;
    private VideoCapture? _capture;
    private bool _disposedValue;
    private bool _trackWasFound = false;
    private IRecognition? _recognition;
    private bool _isTrackingEnabled = (bool)IsTrackingEnabledProperty.GetMetadata(typeof(ARCamera)).DefaultValue;

    /// <summary>
    /// Occurs when a track is found in the video feed.
    /// </summary>
    public event EventHandler<TargetMatchingEventArgs>? TrackFound;

    /// <summary>
    /// Occurs when a track is lost in the video feed.
    /// </summary>
    public event EventHandler? TrackLost;

    /// <summary>
    /// Identifies the <see cref="IsTrackingEnabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsTrackingEnabledProperty =
        DependencyProperty.Register(nameof(IsTrackingEnabled), typeof(bool), typeof(ARCamera), new PropertyMetadata(true, OnIsTrackingEnabledChanged));

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets a value indicating whether tracking is enabled.
    /// </summary>
    public bool IsTrackingEnabled
    {
        get => (bool)GetValue(IsTrackingEnabledProperty);
        set => SetValue(IsTrackingEnabledProperty, value);
    }

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="ARCamera"/> class.
    /// </summary>
    public ARCamera()
    {
        DefaultStyleKey = typeof(ARCamera);

        _imageRequestBuilder = new ImageRequestBuilder().WithGrayscale()
            .WithGaussianBlur(KSize, SigmaX)
            .WithLowResolution(ImageLowResolution);

        Loaded += CameraViewLayout_Loaded;
        Unloaded += CameraViewLayout_Unloaded;
    }

    #region Methods Overrides

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _frameImage = new Image
        {
            Stretch = Microsoft.UI.Xaml.Media.Stretch.Uniform
        };

        _grid?.Children.Clear();
        _grid = GetTemplateChild("PART_Root") as Grid;
        _grid?.Children.Add(_frameImage);
    }

    #endregion

    #region Methods

    private static void OnIsTrackingEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var arCamera = (ARCamera)d;
        arCamera.OnIsTrackingEnabledChanged((bool)e.OldValue, (bool)e.NewValue);
    }

    private void OnIsTrackingEnabledChanged(bool oldValue, bool newValue)
    {
        _isTrackingEnabled = newValue;
    }

    /// <summary>
    /// Sets the recognition service for the AR camera.
    /// </summary>
    /// <param name="vision">The recognition service to use.</param>
    public void SetRecoService(IRecognition vision)
    {
        _recognition = vision;
    }

    /// <summary>
    /// Starts capturing frames from the camera.
    /// </summary>
    private void StartCamera()
    {
        _capture = new VideoCapture(0, VideoCapture.API.DShow);
        _capture.Set(Emgu.CV.CvEnum.CapProp.Fps, 30);
        _capture.ImageGrabbed += Capture_ImageGrabbed;
        _capture.Start();
    }

    /// <summary>
    /// Stops capturing frames from the camera.
    /// </summary>
    private void StopCamera()
    {
        if (_capture is null)
        {
            return;
        }

        _capture.Stop();
        _capture.ImageGrabbed -= Capture_ImageGrabbed;
        _capture.Dispose();
        _capture = null;
    }

    /// <summary>
    /// Event handler for when a new frame is grabbed from the camera.
    /// </summary>
    private void ImageGrabbed()
    {
        if (_capture is null)
        {
            throw new ArgumentNullException(nameof(_capture));
        }

        var frame = new Mat();

        _capture.Read(frame);

        if (frame.IsEmpty)
        {
            return;
        }

        if (_isTrackingEnabled)
        {
            var imageRequest = _imageRequestBuilder.Build(frame);
            var featureMatchingResult = _recognition?.Match(imageRequest);

            DispatcherQueue.TryEnqueue(() =>
            {
                if (featureMatchingResult?.HasMatches == true)
                {
                    OnTrackFound(frame, featureMatchingResult.Matches);
                }
                else
                {
                    OnTrackLost();
                }

                UpdateView(frame);
            });
        }
        else
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                UpdateView(frame);
            });
        }
    }

    /// <summary>
    /// Handles actions when a target match is found.
    /// </summary>
    /// <param name="frame">The frame in which the target match was found.</param>
    /// <param name="targetMatches">Details about the matched targets.</param>
    private void OnTrackFound(Mat frame, IReadOnlyCollection<TargetMatchResult> targetMatches)
    {
        _trackWasFound = true;

        var eventArgs = new TargetMatchingEventArgs(frame, targetMatches);
        TrackFound?.Invoke(this, eventArgs);
    }

    /// <summary>
    /// Handles actions when a target match is lost.
    /// </summary>
    private void OnTrackLost()
    {
        if (!_trackWasFound)
        {
            return;
        }

        TrackLost?.Invoke(this, EventArgs.Empty);
        _trackWasFound = false;
    }

    /// <summary>
    /// Updates the displayed image frame in the UI.
    /// </summary>
    /// <param name="frame">The frame to display.</param>
    private async void UpdateView(Mat frame)
    {
        if (_frameImage is null)
        {
            return;
        }

        var imencode = CvInvoke.Imencode(".jpg", frame);
        var memoryStream = new MemoryStream(imencode);
        var randomAccessStream = memoryStream.AsRandomAccessStream();
        var bitmapImage = new BitmapImage();

        await bitmapImage.SetSourceAsync(randomAccessStream);

        _frameImage.Source = bitmapImage;
    }

    #endregion

    #region Events Subscription

    /// <summary>
    /// Event handler for when the ARCamera is loaded.
    /// Starts the camera capture.
    /// </summary>
    private void CameraViewLayout_Loaded(object sender, RoutedEventArgs e)
    {
        StartCamera();
    }

    /// <summary>
    /// Event handler for when the ARCamera is unloaded.
    /// Stops the camera capture.
    /// </summary>
    private void CameraViewLayout_Unloaded(object sender, RoutedEventArgs e)
    {
        StopCamera();
    }

    /// <summary>
    /// Event handler for when a new frame is grabbed from the camera.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">Event arguments.</param>
    private void Capture_ImageGrabbed(object? sender, EventArgs e)
    {
        ImageGrabbed();
    }

    #endregion

    #region IDisposable Implementation

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="ARCamera"/> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                StopCamera();
                _capture = null;
            }

            _disposedValue = true;
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}