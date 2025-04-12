using OpenVision.Shared.Types;

namespace OpenVision.Server.Core.GraphQL.Types;

/// <summary>
/// Represents a simplified target record with essential properties for lookup and display.
/// </summary>
[GraphQLDescription("Represents a simplified target record with key properties for lookup and display purposes.")]
public record TargetRecordModel
{
    /// <summary>
    /// Gets the target ID.
    /// </summary>
    [GraphQLDescription("The unique identifier of the target as a string.")]
    [ID]
    public required virtual Guid TargetId { get; init; }

    /// <summary>
    /// Gets the active flag.
    /// </summary>
    [GraphQLDescription("Indicates whether the target is active.")]
    public required virtual ActiveFlag ActiveFlag { get; init; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    [GraphQLDescription("The display name of the target.")]
    public required virtual string Name { get; init; }

    /// <summary>
    /// Gets the width.
    /// </summary>
    [GraphQLDescription("The width of the target.")]
    public required virtual float Width { get; init; }

    /// <summary>
    /// Gets the tracking rating.
    /// </summary>
    [GraphQLDescription("The tracking rating assigned to the target.")]
    public required virtual int TrackingRating { get; init; }
}