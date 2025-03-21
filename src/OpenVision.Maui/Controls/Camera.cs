namespace OpenVision.Maui.Controls;

/// <summary>
/// Represents a camera view with frame change events.
/// </summary>
public class Camera : View
{
    #region Fields/Consts

    /// <summary>
    /// Event raised when the camera frame changes.
    /// </summary>
    public event EventHandler<FrameChangedEventArgs>? FrameChanged;

    #endregion

    /// <summary>
    /// Constructs an instance of the Camera class.
    /// </summary>
    public Camera()
    {
    }

    #region Methods

    /// <summary>
    /// Raises the FrameChanged event with the provided frame data.
    /// </summary>
    /// <param name="frame">The byte array containing the frame data.</param>
    internal void OnFrameChanged(byte[] frame)
    {
        var e = new FrameChangedEventArgs(frame);

        FrameChanged?.Invoke(this, e);
        Handler?.Invoke(nameof(Camera.FrameChanged), e);
    }

    #endregion
}
