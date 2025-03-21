namespace OpenVision.Wpf.Controls;

/// <summary>
/// Provides data for the frame changed event.
/// </summary>
public class FrameChangedEventArgs
{
    /// <summary>
    /// Gets the frame data.
    /// </summary>
    public byte[] Frame { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FrameChangedEventArgs"/> class.
    /// </summary>
    /// <param name="frame">The frame data.</param>
    internal FrameChangedEventArgs(byte[] frame)
    {
        Frame = frame;
    }
}
