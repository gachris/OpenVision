using Microsoft.Maui.Handlers;

namespace OpenVision.Maui.Controls;

/// <summary>
/// Custom handler for managing the platform-specific view of the ARCamera control.
/// </summary>
public partial class ARCameraHandler : ViewHandler<ARCamera, ARCameraLayout>
{
    /// <summary>
    /// Creates the platform-specific view for ARCamera.
    /// </summary>
    /// <returns>An instance of ARCameraLayout initialized with the VirtualView.</returns>
    protected override ARCameraLayout CreatePlatformView() => new(VirtualView);

    /// <summary>
    /// Connects the handler to the platform-specific view.
    /// </summary>
    /// <param name="platformView">The platform-specific view instance.</param>
    protected override void ConnectHandler(ARCameraLayout platformView)
    {
        base.ConnectHandler(platformView);
    }

    /// <summary>
    /// Disconnects the handler from the platform-specific view.
    /// </summary>
    /// <param name="platformView">The platform-specific view instance.</param>
    protected override void DisconnectHandler(ARCameraLayout platformView)
    {
        base.DisconnectHandler(platformView);
    }
}