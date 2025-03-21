using Emgu.CV;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;

namespace OpenVision.WinUI.Controls;

/// <summary>
/// A control for displaying camera feed.
/// </summary>
public class Camera : Control, IDisposable
{
    #region Fields/Consts

    private Image? _frameImage;
    private Grid? _grid;
    private VideoCapture? _capture;
    private bool _disposedValue;

    /// <summary>
    /// Event raised when the camera frame changes.
    /// </summary>
    public event EventHandler<FrameChangedEventArgs>? FrameChanged;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="Camera"/> class.
    /// </summary>
    public Camera()
    {
        DefaultStyleKey = typeof(Camera);

        Loaded += Camera_Loaded;
        Unloaded += Camera_Unloaded;
    }

    #region Methods Overrides

    /// <inheritdoc/>
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

        DispatcherQueue.TryEnqueue(() =>
        {
            UpdateView(frame);
        });
    }

    /// <summary>
    /// Updates the image control with the latest camera frame.
    /// </summary>
    /// <param name="frame">The captured camera frame.</param>
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

        memoryStream.Dispose();

        OnFrameChanged(imencode);
    }

    /// <summary>
    /// Raises the FrameChanged event with the provided frame data.
    /// </summary>
    /// <param name="frame">The byte array containing the frame data.</param>
    private void OnFrameChanged(byte[] frame)
    {
        FrameChanged?.Invoke(this, new FrameChangedEventArgs(frame));
    }

    #endregion

    #region Events Subscription

    /// <summary>
    /// Event handler for when the control is loaded.
    /// </summary>
    private void Camera_Loaded(object sender, RoutedEventArgs e)
    {
        StartCamera();
    }

    /// <summary>
    /// Event handler for when the control is unloaded.
    /// </summary>
    private void Camera_Unloaded(object sender, RoutedEventArgs e)
    {
        StopCamera();
    }

    /// <summary>
    /// Event handler for when a new frame is grabbed from the camera.
    /// </summary>
    private void Capture_ImageGrabbed(object? sender, EventArgs e)
    {
        ImageGrabbed();
    }

    #endregion

    #region IDisposable Implementation

    /// <summary>
    /// Cleans up resources used by the Camera.
    /// </summary>
    /// <param name="disposing">True if called from Dispose method, false if called from finalizer.</param>
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
    /// Disposes the Camera and releases all resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
