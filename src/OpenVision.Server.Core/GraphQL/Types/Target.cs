using OpenVision.Shared;

namespace OpenVision.Server.Core.GraphQL.Types;

/// <summary>
/// Represents a target entity exposed via the GraphQL API.
/// </summary>
[GraphQLDescription("Represents a target entity with associated database, image, and tracking details.")]
public record Target
{
    /// <summary>
    /// Gets or sets the unique identifier of the target.
    /// </summary>
    [ID]
    [GraphQLDescription("The unique identifier of the target.")]
    public required virtual Guid Id { get; init; }

    /// <summary>
    /// Gets or sets the unique identifier of the database that the target belongs to.
    /// </summary>
    [GraphQLDescription("The unique identifier of the database to which the target belongs.")]
    public required virtual Guid DatabaseId { get; init; }

    /// <summary>
    /// Gets or sets the preprocessed image of the target.
    /// </summary>
    [GraphQLDescription("The preprocessed image of the target.")]
    public required virtual byte[] PreprocessImage { get; init; }

    /// <summary>
    /// Gets or sets the image of the target after it has been processed.
    /// </summary>
    [GraphQLDescription("The image of the target after it has been processed.")]
    public required virtual byte[] AfterProcessImage { get; init; }

    /// <summary>
    /// Gets or sets the image of the target after it has been processed with keypoints.
    /// </summary>
    [GraphQLDescription("The image of the target after it has been processed with keypoints.")]
    public required virtual byte[] AfterProcessImageWithKeypoints { get; init; }

    /// <summary>
    /// Gets or sets the units in the X direction.
    /// </summary>
    [GraphQLDescription("The units in the X direction.")]
    public required virtual float XUnits { get; init; }

    /// <summary>
    /// Gets or sets the units in the Y direction.
    /// </summary>
    [GraphQLDescription("The units in the Y direction.")]
    public required virtual float YUnits { get; init; }

    /// <summary>
    /// Gets or sets the number of recognitions.
    /// </summary>
    [GraphQLDescription("The number of recognitions for the target.")]
    public required virtual int Recos { get; init; }

    /// <summary>
    /// Gets or sets the rating of the target.
    /// </summary>
    [GraphQLDescription("The rating of the target.")]
    public required virtual int Rating { get; init; }

    /// <summary>
    /// Gets or sets the name of the target.
    /// </summary>
    [GraphQLDescription("The name of the target.")]
    public required virtual string Name { get; init; }

    /// <summary>
    /// Gets or sets the type of the target.
    /// </summary>
    [GraphQLDescription("The type of the target.")]
    public required virtual TargetType Type { get; init; }

    /// <summary>
    /// Gets or sets the active flag of the target.
    /// </summary>
    [GraphQLDescription("The active flag indicating whether the target is active.")]
    public required virtual ActiveFlag ActiveFlag { get; init; }

    /// <summary>
    /// Gets or sets the metadata of the target.
    /// </summary>
    [GraphQLDescription("Additional metadata associated with the target.")]
    public virtual string? Metadata { get; init; }

    /// <summary>
    /// Gets or sets the creation date of the target object.
    /// </summary>
    [GraphQLDescription("The date and time when the target was created.")]
    public required virtual DateTimeOffset Created { get; init; }

    /// <summary>
    /// Gets or sets the last update date of the target object.
    /// </summary>
    [GraphQLDescription("The date and time when the target was last updated.")]
    public required virtual DateTimeOffset Updated { get; init; }
}
