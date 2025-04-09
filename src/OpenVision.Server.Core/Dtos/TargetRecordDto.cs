using OpenVision.Shared;

namespace OpenVision.Server.Core.Dtos;

/// <summary>
/// Represents a target record model.
/// </summary>
public class TargetRecordDto
{
    /// <summary>
    /// Gets or sets the target ID.
    /// </summary>
    public required virtual string TargetId { get; set; }

    /// <summary>
    /// Gets or sets the active flag.
    /// </summary>
    public required virtual ActiveFlag ActiveFlag { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public required virtual string Name { get; set; }

    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    public required virtual float Width { get; set; }

    /// <summary>
    /// Gets or sets the tracking rating.
    /// </summary>
    public required virtual int TrackingRating { get; set; }
}