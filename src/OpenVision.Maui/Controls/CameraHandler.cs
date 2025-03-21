namespace OpenVision.Maui.Controls;

/// <summary>
/// Partial class responsible for handling property and command mappings for the Camera view.
/// </summary>
public partial class CameraHandler
{
    /// <summary>
    /// Static PropertyMapper for mapping properties of Camera to this handler.
    /// </summary>
    public static readonly PropertyMapper<Camera, CameraHandler> PropertyMapper = new(ViewMapper)
    {
    };

    /// <summary>
    /// Static CommandMapper for mapping commands of Camera to this handler.
    /// </summary>
    public static readonly CommandMapper<Camera, CameraHandler> CommandMapper = new(ViewCommandMapper)
    {
    };

    /// <summary>
    /// Constructs an instance of CameraHandler, initializing with PropertyMapper and CommandMapper.
    /// </summary>
    public CameraHandler() : base(PropertyMapper, CommandMapper)
    {
    }
}