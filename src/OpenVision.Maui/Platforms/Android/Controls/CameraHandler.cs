using Microsoft.Maui.Handlers;

namespace OpenVision.Maui.Controls;

/// <summary>
/// Custom handler for managing the native implementation of the Camera control in a Maui app.
/// </summary>
public partial class CameraHandler : ViewHandler<Camera, CameraLayout>
{
    /// <summary>
    /// Creates a new instance of CameraLayout as the native view representation.
    /// </summary>
    /// <returns>A new CameraLayout instance.</returns>
    protected override CameraLayout CreatePlatformView() => new(Context, VirtualView);

    /// <summary>
    /// Connects the native CameraLayout view to its corresponding Maui Camera view.
    /// </summary>
    /// <param name="platformView">The native CameraLayout view instance.</param>
    protected override void ConnectHandler(CameraLayout platformView)
    {
        base.ConnectHandler(platformView);
    }

    /// <summary>
    /// Disconnects the native CameraLayout view from its corresponding Maui Camera view.
    /// </summary>
    /// <param name="platformView">The native CameraLayout view instance.</param>
    protected override void DisconnectHandler(CameraLayout platformView)
    {
        base.DisconnectHandler(platformView);
    }
}
