using Microsoft.UI.Xaml.Controls;

namespace OpenVision.Maui.Controls;

/// <summary>
/// UserControl for displaying AR camera feed and handling AR tracking.
/// </summary>
public class ARCameraLayout : UserControl, IDisposable
{
    #region Fields/Consts

    private readonly ARCamera _arCamera;
    private OpenVision.WinUI.Controls.ARCamera? _winUIARCamera;
    private bool _disposedValue;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="ARCameraLayout"/> class.
    /// </summary>
    /// <param name="camera">Instance of ARCamera to use for AR functionalities.</param>
    public ARCameraLayout(ARCamera camera)
    {
        _arCamera = camera;

        _winUIARCamera = new WinUI.Controls.ARCamera()
        {
            HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch,
            VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Stretch
        };

        _winUIARCamera.TrackFound += (sender, args) =>
        {
            var eventArgs = new TargetMatchingEventArgs(args.Frame, args.TargetMatchResults);
            _arCamera.OnTrackFound(eventArgs);
        };

        _winUIARCamera.TrackLost += (sender, args) =>
        {
            _arCamera.OnTrackLost(EventArgs.Empty);
        };

        Content = _winUIARCamera;
    }

    #region Methods

    /// <summary>
    /// Updates the tracking status based on the ARCamera instance.
    /// </summary>
    internal void UpdateIsTrackingEnabled()
    {
        if (_winUIARCamera is null)
        {
            return;
        }

        _winUIARCamera.IsTrackingEnabled = _arCamera.IsTrackingEnabled;
    }

    /// <summary>
    /// Updates the recognition service based on the ARCamera instance.
    /// </summary>
    internal void UpdateRecognition()
    {
        _winUIARCamera?.SetRecoService(_arCamera.Recognition);
    }

    #endregion

    #region IDisposable Implementation

    /// <summary>
    /// Disposes of managed and unmanaged resources.
    /// </summary>
    /// <param name="disposing">True if disposing of managed resources, false otherwise.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _winUIARCamera = null;
            }

            _disposedValue = true;
        }
    }

    /// <summary>
    /// Disposes of the object.
    /// </summary>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}