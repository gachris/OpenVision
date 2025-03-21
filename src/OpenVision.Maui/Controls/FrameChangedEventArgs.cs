namespace OpenVision.Maui.Controls;

/// <summary>
/// Event arguments containing a frame represented as a byte array.
/// </summary>
public class FrameChangedEventArgs
{
    /// <summary>
    /// Gets the byte array representing the frame data.
    /// </summary>
    public byte[] Frame { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FrameChangedEventArgs"/> class.
    /// </summary>
    /// <param name="frame">The byte array representing the frame data.</param>
    internal FrameChangedEventArgs(byte[] frame)
    {
        Frame = frame;
    }
}
