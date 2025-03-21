namespace OpenVision.Maui.Controls;

/// <summary>
/// Partial class for handling property and command mapping of ARCamera control.
/// </summary>
public partial class ARCameraHandler
{
    /// <summary>
    /// Property mapper for ARCamera properties.
    /// </summary>
    public static readonly PropertyMapper<ARCamera, ARCameraHandler> PropertyMapper = new PropertyMapper<ARCamera, ARCameraHandler>(ViewMapper)
    {
        [nameof(ARCamera.IsTrackingEnabled)] = MapIsTrackingEnabled,
        [nameof(ARCamera.Recognition)] = MapRecognition
    };

    /// <summary>
    /// Command mapper for ARCamera commands.
    /// </summary>
    public static readonly CommandMapper<ARCamera, ARCameraHandler> CommandMapper = new CommandMapper<ARCamera, ARCameraHandler>(ViewCommandMapper)
    {
        // Currently empty, can be populated with command mappings if needed.
    };

    /// <summary>
    /// Constructs an instance of ARCameraHandler.
    /// </summary>
    public ARCameraHandler() : base(PropertyMapper, CommandMapper)
    {
    }

    /// <summary>
    /// Maps the IsTrackingEnabled property of ARCamera to the platform-specific handler.
    /// </summary>
    /// <param name="handler">The ARCameraHandler instance.</param>
    /// <param name="video">The ARCamera instance.</param>
    public static void MapIsTrackingEnabled(ARCameraHandler handler, ARCamera video)
    {
        handler.PlatformView?.UpdateIsTrackingEnabled();
    }

    /// <summary>
    /// Maps the IsTrackingEnabled property of ARCamera to the platform-specific handler.
    /// </summary>
    /// <param name="handler">The ARCameraHandler instance.</param>
    /// <param name="video">The ARCamera instance.</param>
    public static void MapRecognition(ARCameraHandler handler, ARCamera video)
    {
        handler.PlatformView?.UpdateRecognition();
    }
}