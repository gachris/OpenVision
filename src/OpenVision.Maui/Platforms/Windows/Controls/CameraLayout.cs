using Microsoft.UI.Xaml.Controls;

namespace OpenVision.Maui.Controls;

/// <summary>
/// UserControl for displaying camera feed.
/// </summary>
public class CameraLayout : UserControl, IDisposable
{
    #region Fields/Consts

    private readonly Camera _camera;
    private OpenVision.WinUI.Controls.Camera? _winUICamera;
    private bool _disposedValue;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="CameraLayout"/> class.
    /// </summary>
    /// <param name="camera">The camera instance associated with this layout.</param>
    public CameraLayout(Camera camera)
    {
        _camera = camera;
        _winUICamera = new WinUI.Controls.Camera()
        {
            HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch,
            VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Stretch
        };

        Content = _winUICamera;
    }

    #region IDisposable Implementation

    /// <summary>
    /// Cleans up resources used by the CameraLayout.
    /// </summary>
    /// <param name="disposing">True if called from Dispose method, false if called from finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _winUICamera = null;
            }

            _disposedValue = true;
        }
    }

    /// <summary>
    /// Disposes the CameraLayout and releases all resources.
    /// </summary>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}