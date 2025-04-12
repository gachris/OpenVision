using OpenVision.Shared.Types;

namespace OpenVision.Server.Core.Dtos;

/// <summary>
/// Represents a data transfer object for a target.
/// </summary>
public record TargetDto
{
    /// <summary>
    /// Gets the unique identifier of the target.
    /// </summary>
    public required virtual Guid Id { get; init; }

    /// <summary>
    /// Gets the unique identifier of the database that the target belongs to.
    /// </summary>
    public required virtual Guid DatabaseId { get; init; }

    /// <summary>
    /// Gets the preprocessed image of the target.
    /// </summary>
    public required virtual byte[] PreprocessImage { get; init; }

    /// <summary>
    /// Gets the image of the target after it has been processed.
    /// </summary>
    public required virtual byte[] AfterProcessImage { get; init; }

    /// <summary>
    /// Gets the image of the target after it has been processed with keypoints.
    /// </summary>
    public required virtual byte[] AfterProcessImageWithKeypoints { get; init; }

    /// <summary>
    /// Gets the units in the x direction.
    /// </summary>
    public required virtual float XUnits { get; init; }

    /// <summary>
    /// Gets the units in the y direction.
    /// </summary>
    public required virtual float YUnits { get; init; }

    /// <summary>
    /// Gets the number of recognitions.
    /// </summary>
    public required virtual int Recos { get; init; }

    /// <summary>
    /// Gets the rating of the target.
    /// </summary>
    public required virtual int Rating { get; init; }

    /// <summary>
    /// Gets the name of the target.
    /// </summary>
    public required virtual string Name { get; init; }

    /// <summary>
    /// Gets the type of the target.
    /// </summary>
    public required virtual TargetType Type { get; init; }

    /// <summary>
    /// Gets the active flag of the target.
    /// </summary>
    public required virtual ActiveFlag ActiveFlag { get; init; }

    /// <summary>
    /// Gets the metadata of the target.
    /// </summary>
    public virtual string? Metadata { get; init; }

    /// <summary>
    /// Gets the creation date of the target object.
    /// </summary>
    public required virtual DateTimeOffset Created { get; init; }

    /// <summary>
    /// Gets the last update date of the target object.
    /// </summary>
    public required virtual DateTimeOffset Updated { get; init; }

    /// <summary>
    /// Gets the database to which the image target belongs.
    /// </summary>
    public virtual DatabaseDto? Database { get; init; }
}