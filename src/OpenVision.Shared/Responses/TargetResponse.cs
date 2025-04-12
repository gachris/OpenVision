using OpenVision.Shared.Types;

namespace OpenVision.Shared.Responses;

/// <summary>
/// Represents the response payload returned by the API for a target record.
/// </summary>
public record TargetResponse
{
    /// <summary>
    /// Gets the unique identifier of the target.
    /// </summary>
    public virtual required Guid Id { get; init; }

    /// <summary>
    /// Gets the unique identifier of the database that the target belongs to.
    /// </summary>
    public virtual required Guid DatabaseId { get; init; }

    /// <summary>
    /// Gets the preprocessed image of the target.
    /// </summary>
    public virtual required byte[] PreprocessImage { get; init; }

    /// <summary>
    /// Gets the image of the target after it has been processed.
    /// </summary>
    public virtual required byte[] AfterProcessImage { get; init; }

    /// <summary>
    /// Gets the image of the target after it has been processed with keypoints.
    /// </summary>
    public virtual required byte[] AfterProcessImageWithKeypoints { get; init; }

    /// <summary>
    /// Gets the units in the x direction.
    /// </summary>
    public virtual required float XUnits { get; init; }

    /// <summary>
    /// Gets the units in the y direction.
    /// </summary>
    public virtual required float YUnits { get; init; }

    /// <summary>
    /// Gets the number of recognitions.
    /// </summary>
    public virtual required int Recos { get; init; }

    /// <summary>
    /// Gets the rating of the target.
    /// </summary>
    public virtual required int Rating { get; init; }

    /// <summary>
    /// Gets the name of the target.
    /// </summary>
    public virtual required string Name { get; init; }

    /// <summary>
    /// Gets the type of the target.
    /// </summary>
    public virtual required TargetType Type { get; init; }

    /// <summary>
    /// Gets the active flag of the target.
    /// </summary>
    public virtual required ActiveFlag ActiveFlag { get; init; }

    /// <summary>
    /// Gets the metadata of the target.
    /// </summary>
    public virtual string? Metadata { get; init; }

    /// <summary>
    /// Gets the creation date of the target object.
    /// </summary>
    public virtual required DateTimeOffset Created { get; init; }

    /// <summary>
    /// Gets the last update date of the target object.
    /// </summary>
    public virtual required DateTimeOffset Updated { get; init; }
}