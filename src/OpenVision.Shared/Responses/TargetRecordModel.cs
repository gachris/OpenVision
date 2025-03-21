using System.Text.Json.Serialization;

namespace OpenVision.Shared.Responses;

/// <summary>
/// Represents a target record model.
/// </summary>
public class TargetRecordModel
{
    /// <summary>
    /// Gets the target ID.
    /// </summary>
    [JsonPropertyName("target_id")]
    public virtual string TargetId { get; }

    /// <summary>
    /// Gets the active flag.
    /// </summary>
    [JsonPropertyName("active_flag")]
    public virtual ActiveFlag ActiveFlag { get; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    [JsonPropertyName("name")]
    public virtual string Name { get; }

    /// <summary>
    /// Gets the width.
    /// </summary>
    [JsonPropertyName("width")]
    public virtual float Width { get; }

    /// <summary>
    /// Gets the tracking rating.
    /// </summary>
    [JsonPropertyName("tracking_rating")]
    public virtual int TrackingRating { get; }

    /// <summary>
    /// Initializes a new instance of the TargetRecordModel class with the specified target ID, active flag, name, width, tracking rating, and recognition rating.
    /// </summary>
    /// <param name="targetId">The target ID.</param>
    /// <param name="activeFlag">The active flag.</param>
    /// <param name="name">The name of the target.</param>
    /// <param name="width">The width of the target.</param>
    /// <param name="trackingRating">The tracking rating of the target.</param>
    public TargetRecordModel(string targetId, ActiveFlag activeFlag, string name, float width, int trackingRating)
    {
        TargetId = targetId;
        ActiveFlag = activeFlag;
        Name = name;
        Width = width;
        TrackingRating = trackingRating;
    }
}