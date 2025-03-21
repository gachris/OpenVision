using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Emgu.CV;

namespace OpenVision.Wpf.Controls;

/// <summary>
/// Represents a control that displays camera feed.
/// </summary>
public class Camera : Control, IDisposable
{
    #region Fields/Consts

    private readonly Image _frameImage;
    private VideoCapture? _capture;
    private bool _disposedValue;
    private Grid? _grid;

    /// <summary>
    /// Occurs when the camera frame has changed.
    /// </summary>
    public event EventHandler<FrameChangedEventArgs>? FrameChanged;

    #endregion

    static Camera()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Camera), new FrameworkPropertyMetadata(typeof(Camera)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Camera"/> class.
    /// </summary>
    public Camera()
    {
        _frameImage = new Image()
        {
            Stretch = Stretch.Uniform
        };

        Loaded += CameraViewLayout_Loaded;
        Unloaded += CameraViewLayout_Unloaded;
    }

    #region Method Override

    /// <summary>
    /// Builds the visual tree for the control when a new template is applied.
    /// </summary>
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _grid?.Children.Clear();
        _grid = GetTemplateChild("PART_Root") as Grid;
        _grid?.Children.Add(_frameImage);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Starts the camera capture.
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
    /// Stops the camera capture.
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
    }

    /// <summary>
    /// Handles the image grabbed event from the camera.
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

        UpdateView(frame);
    }

    /// <summary>
    /// Updates the view with the latest frame from the camera.
    /// </summary>
    /// <param name="frame">The latest frame.</param>
    private void UpdateView(Mat frame)
    {
        Dispatcher.BeginInvoke(() =>
        {
            var imencode = CvInvoke.Imencode(".jpg", frame);
            var memoryStream = new MemoryStream(imencode);
            var imageSource = new BitmapImage();

            imageSource.BeginInit();
            imageSource.CacheOption = BitmapCacheOption.OnLoad;
            imageSource.StreamSource = memoryStream;
            imageSource.EndInit();

            _frameImage.Source = imageSource;

            FrameChanged?.Invoke(this, new FrameChangedEventArgs(imencode));
        });
    }

    #endregion

    #region Events Subscription

    private void CameraViewLayout_Loaded(object sender, RoutedEventArgs e)
    {
        StartCamera();
    }

    private void CameraViewLayout_Unloaded(object sender, RoutedEventArgs e)
    {
        StopCamera();
    }

    private void Capture_ImageGrabbed(object? sender, EventArgs e)
    {
        ImageGrabbed();
    }

    #endregion

    #region IDisposable Implementation

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="Camera"/> and optionally releases the managed resources.
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
    /// Disposes the resources used by the <see cref="Camera"/>.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}