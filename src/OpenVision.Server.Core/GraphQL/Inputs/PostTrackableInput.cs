using OpenVision.Shared.Types;

namespace OpenVision.Server.Core.GraphQL.Inputs;

/// <summary>
/// Represents the data required to create a new target.
/// </summary>
[GraphQLDescription("Represents the data required to create a new target.")]
public record PostTrackableInput
{
    /// <summary>
    /// Gets or sets the name of the trackable.
    /// </summary>
    [GraphQLDescription("The name of the trackable. This field is required.")]
    public virtual required string Name { get; init; }

    /// <summary>
    /// Gets or sets the width of the trackable in meters.
    /// </summary>
    [GraphQLDescription("The width of the trackable, in meters. This field is required.")]
    public virtual required float Width { get; init; }

    /// <summary>
    /// Gets or sets the URL of the image to use for the trackable.
    /// </summary>
    [GraphQLDescription("The URL of the image to use for the trackable. This field is required.")]
    public virtual required string Image { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the trackable is active or inactive.
    /// </summary>
    [GraphQLDescription("A flag indicating whether the trackable is active or inactive.")]
    public virtual ActiveFlag? ActiveFlag { get; init; }

    /// <summary>
    /// Gets or sets the metadata for the trackable.
    /// </summary>
    [GraphQLDescription("Optional metadata providing additional information for the trackable.")]
    public virtual string? Metadata { get; init; }
}