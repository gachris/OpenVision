using Microsoft.Maui.Handlers;

namespace OpenVision.Maui.Controls;

/// <summary>
/// Custom handler for managing the Camera view in a Maui application.
/// </summary>
public partial class CameraHandler : ViewHandler<Camera, CameraLayout>
{
    /// <summary>
    /// Creates the platform-specific CameraLayout view.
    /// </summary>
    /// <returns>The created CameraLayout instance.</returns>
    protected override CameraLayout CreatePlatformView() => new(VirtualView);

    /// <summary>
    /// Connects the handler with the platform-specific CameraLayout view.
    /// </summary>
    /// <param name="platformView">The platform-specific CameraLayout instance.</param>
    protected override void ConnectHandler(CameraLayout platformView)
    {
        base.ConnectHandler(platformView);
    }

    /// <summary>
    /// Disconnects the handler from the platform-specific CameraLayout view.
    /// </summary>
    /// <param name="platformView">The platform-specific CameraLayout instance.</param>
    protected override void DisconnectHandler(CameraLayout platformView)
    {
        base.DisconnectHandler(platformView);
    }
}