using Microsoft.Maui.Handlers;

namespace OpenVision.Maui.Controls;

/// <summary>
/// Handler for the ARCamera view, responsible for connecting the virtual view (ARCamera) 
/// to the platform-specific view (ARCameraLayout).
/// </summary>
public partial class ARCameraHandler : ViewHandler<ARCamera, ARCameraLayout>
{
    /// <summary>
    /// Creates an instance of the platform-specific view (ARCameraLayout).
    /// </summary>
    /// <returns>An instance of ARCameraLayout.</returns>
    protected override ARCameraLayout CreatePlatformView() => new(Context, VirtualView);

    /// <summary>
    /// Connects the platform-specific view to its handler.
    /// </summary>
    /// <param name="platformView">The platform-specific view instance.</param>
    protected override void ConnectHandler(ARCameraLayout platformView)
    {
        base.ConnectHandler(platformView);
    }

    /// <summary>
    /// Disconnects the platform-specific view from its handler.
    /// </summary>
    /// <param name="platformView">The platform-specific view instance.</param>
    protected override void DisconnectHandler(ARCameraLayout platformView)
    {
        base.DisconnectHandler(platformView);
    }
}
