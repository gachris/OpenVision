using OpenVision.Shared.Types;

namespace OpenVision.Shared.Responses;

/// <summary>
/// Represents a target record model.
/// </summary>
public record TargetRecordModel
{
    /// <summary>
    /// Gets the target ID.
    /// </summary>
    public required virtual Guid TargetId { get; init; }

    /// <summary>
    /// Gets the active flag.
    /// </summary>
    public required virtual ActiveFlag ActiveFlag { get; init; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    public required virtual string Name { get; init; }

    /// <summary>
    /// Gets the width.
    /// </summary>
    public required virtual float Width { get; init; }

    /// <summary>
    /// Gets the tracking rating.
    /// </summary>
    public required virtual int TrackingRating { get; init; }
}