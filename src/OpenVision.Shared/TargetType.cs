namespace OpenVision.Shared;

/// <summary>
/// Represents the type of a target.
/// </summary>
public enum TargetType
{
    /// <summary>
    /// The target is a cloud.
    /// </summary>
    Cloud,

    /// <summary>
    /// The target is an image.
    /// </summary>
    Image,

    /// <summary>
    /// The target is a multi-target.
    /// </summary>
    Multi,

    /// <summary>
    /// The target is a cylinder.
    /// </summary>
    Cylinder,

    /// <summary>
    /// The target is an object.
    /// </summary>
    Object
}