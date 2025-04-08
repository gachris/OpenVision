using OpenVision.Shared;

namespace OpenVision.Server.Core.Dtos;

/// <summary>
/// Represents a data transfer object for a target.
/// </summary>
public class TargetDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the target.
    /// </summary>
    public required virtual Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the database that the target belongs to.
    /// </summary>
    public required virtual Guid DatabaseId { get; set; }

    /// <summary>
    /// Gets or sets the preprocessed image of the target.
    /// </summary>
    public required virtual byte[] PreprocessImage { get; set; }

    /// <summary>
    /// Gets or sets the image of the target after it has been processed.
    /// </summary>
    public required virtual byte[] AfterProcessImage { get; set; }

    /// <summary>
    /// Gets or sets the image of the target after it has been processed with keypoints.
    /// </summary>
    public required virtual byte[] AfterProcessImageWithKeypoints { get; set; }

    /// <summary>
    /// Gets or sets the units in the x direction.
    /// </summary>
    public required virtual float XUnits { get; set; }

    /// <summary>
    /// Gets or sets the units in the y direction.
    /// </summary>
    public required virtual float YUnits { get; set; }

    /// <summary>
    /// Gets or sets the number of recognitions.
    /// </summary>
    public required virtual int Recos { get; set; }

    /// <summary>
    /// Gets or sets the rating of the target.
    /// </summary>
    public required virtual int Rating { get; set; }

    /// <summary>
    /// Gets or sets the name of the target.
    /// </summary>
    public required virtual string Name { get; set; }

    /// <summary>
    /// Gets or sets the type of the target.
    /// </summary>
    public required virtual TargetType Type { get; set; }

    /// <summary>
    /// Gets or sets the active flag of the target.
    /// </summary>
    public required virtual ActiveFlag ActiveFlag { get; set; }

    /// <summary>
    /// Gets or sets the metadata of the target.
    /// </summary>
    public virtual string? Metadata { get; set; }

    /// <summary>
    /// Gets or sets the creation date of the target object.
    /// </summary>
    public required virtual DateTimeOffset Created { get; set; }

    /// <summary>
    /// Gets or sets the last update date of the target object.
    /// </summary>
    public required virtual DateTimeOffset Updated { get; set; }

    /// <summary>
    /// Gets or sets the database to which the image target belongs.
    /// </summary>
    public required virtual DatabaseDto Database { get; set; }
}