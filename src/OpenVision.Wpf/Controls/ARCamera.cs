using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Emgu.CV;
using OpenVision.Core.DataTypes;
using OpenVision.Core.Reco;

namespace OpenVision.Wpf.Controls;

/// <summary>
/// Represents a custom control for AR camera functionality, including video capture and image processing.
/// </summary>
public class ARCamera : Control, IDisposable
{
    #region Fields/Consts

    private static readonly double SigmaX = 0;
    private static readonly int ImageLowResolution = 160;
    private static readonly System.Drawing.Size KSize = new(5, 5);

    private readonly ImageRequestBuilder _imageRequestBuilder;

    private VideoCapture? _capture;
    private IRecognition? _recognition;
    private bool _disposedValue;
    private Grid? _grid;
    private Image? _frameImage;
    private bool _trackWasFound = false;

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
        DependencyProperty.Register(nameof(IsTrackingEnabled), typeof(bool), typeof(ARCamera), new PropertyMetadata(true));

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

    static ARCamera()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(ARCamera), new FrameworkPropertyMetadata(typeof(ARCamera)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ARCamera"/> class.
    /// </summary>
    public ARCamera()
    {
        _imageRequestBuilder = new ImageRequestBuilder().WithGrayscale()
            .WithGaussianBlur(KSize, SigmaX)
            .WithLowResolution(ImageLowResolution);

        Loaded += CameraViewLayout_Loaded;
        Unloaded += CameraViewLayout_Unloaded;
    }

    #region Method Override

    /// <inheritdoc/>
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _frameImage = new()
        {
            Stretch = Stretch.Uniform
        };

        _grid?.Children.Clear();
        _grid = GetTemplateChild("PART_Root") as Grid;
        _grid?.Children.Add(_frameImage);
    }

    #endregion

    #region Methods

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
        if (DesignerProperties.GetIsInDesignMode(this))
        {
            return;
        }

        _capture = new(0, VideoCapture.API.DShow);
        _capture.Set(Emgu.CV.CvEnum.CapProp.Fps, 30);
        _capture.ImageGrabbed += Capture_ImageGrabbed;
        _capture.Start();
    }

    /// <summary>
    /// Stops capturing frames from the camera.
    /// </summary>
    private void StopCamera()
    {
        if (DesignerProperties.GetIsInDesignMode(this))
        {
            return;
        }

        if (_capture is null)
        {
            throw new ArgumentNullException(nameof(_capture));
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

        var isTrackingEnabled = Dispatcher.Invoke(() => IsTrackingEnabled);

        if (isTrackingEnabled)
        {
            var request = _imageRequestBuilder.Build(frame);
            var featureMatchingResult = _recognition?.Match(request);

            if (featureMatchingResult?.HasMatches == true)
            {
                Dispatcher.BeginInvoke(() => OnTrackFound(frame, featureMatchingResult.Matches));
            }
            else
            {
                Dispatcher.BeginInvoke(OnTrackLost);
            }
        }

        Dispatcher.BeginInvoke(() => UpdateView(frame));
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
    /// Updates the image control with the latest camera frame.
    /// </summary>
    /// <param name="frame">The captured camera frame.</param>
    private void UpdateView(Mat frame)
    {
        if (_frameImage is null)
        {
            return;
        }

        var imencode = CvInvoke.Imencode(".jpg", frame);
        var memoryStream = new MemoryStream(imencode);
        var imageSource = new BitmapImage();

        imageSource.BeginInit();
        imageSource.CacheOption = BitmapCacheOption.OnLoad;
        imageSource.StreamSource = memoryStream;
        imageSource.EndInit();

        _frameImage.Source = imageSource;
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