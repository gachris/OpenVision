using OpenVision.Server.Core.GraphQL.Types;

namespace OpenVision.Server.Core.GraphQL.Payloads;

/// <summary>
/// Represents the payload returned when a target is updated.
/// </summary>
[GraphQLDescription("Represents the payload returned when a target is updated.")]
public record UpdateTargetPayload
{
    /// <summary>
    /// Gets or sets the updated target.
    /// </summary>
    [GraphQLDescription("The updated target.")]
    public virtual required TargetRecordModel Target { get; init; }
}
