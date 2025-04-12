using OpenVision.Server.Core.GraphQL.Types;

namespace OpenVision.Server.Core.GraphQL.Payloads;

/// <summary>
/// Represents the payload returned when a target is created.
/// </summary>
[GraphQLDescription("Represents the payload returned when a target is created.")]
public record CreateTargetPayload
{
    /// <summary>
    /// Gets or sets the created target.
    /// </summary>
    [GraphQLDescription("The created target.")]
    public virtual required TargetRecordModel Target { get; init; }
}
