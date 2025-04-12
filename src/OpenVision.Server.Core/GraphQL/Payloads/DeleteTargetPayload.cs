namespace OpenVision.Server.Core.GraphQL.Payloads;

/// <summary>
/// Represents the payload returned when a target is deleted.
/// </summary>
[GraphQLDescription("Represents the payload returned when a target is deleted.")]
public record DeleteTargetPayload
{
    /// <summary>
    /// Gets or sets a value indicating whether the deletion was successful.
    /// </summary>
    [GraphQLDescription("A value indicating whether the deletion was successful.")]
    public virtual required bool Success { get; init; }
}
