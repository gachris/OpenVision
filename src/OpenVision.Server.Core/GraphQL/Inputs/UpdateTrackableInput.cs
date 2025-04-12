using OpenVision.Shared.Types;

namespace OpenVision.Server.Core.GraphQL.Inputs;

/// <summary>
/// Represents the data used to update a target, with each field being optional.
/// </summary>
[GraphQLDescription("Represents the data used to update a target, with each field being optional.")]
public record UpdateTrackableInput
{
    /// <summary>
    /// Gets or sets the name of the trackable.
    /// </summary>
    [GraphQLDescription("The name of the trackable. Leave null if no change is desired.")]
    public virtual string? Name { get; init; }

    /// <summary>
    /// Gets or sets the width of the trackable in meters.
    /// </summary>
    [GraphQLDescription("The width of the trackable in meters. Leave null if no change is desired.")]
    public virtual float? Width { get; init; }

    /// <summary>
    /// Gets or sets the URL of the image to use for the trackable.
    /// </summary>
    [GraphQLDescription("The URL of the trackable image. Leave null if no change is desired.")]
    public virtual string? Image { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the trackable is active or inactive.
    /// </summary>
    [GraphQLDescription("A flag indicating whether the trackable is active or inactive. Leave null if no change is desired.")]
    public virtual ActiveFlag? ActiveFlag { get; init; }

    /// <summary>
    /// Gets or sets the metadata for the trackable.
    /// </summary>
    [GraphQLDescription("Optional metadata for the trackable. Leave null if no change is desired.")]
    public virtual string? Metadata { get; init; }
}