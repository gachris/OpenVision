using OpenVision.Shared;

namespace OpenVision.EntityFramework.Entities;

/// <summary>
/// Represents an image target in the system.
/// </summary>
public partial class ImageTarget
{
    /// <summary>
    /// Gets or sets the unique identifier of the image target.
    /// </summary>
    public virtual required Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the database to which the image target belongs.
    /// </summary>
    public virtual required Guid DatabaseId { get; set; }

    /// <summary>
    /// Gets or sets the name of the image target.
    /// </summary>
    public virtual required string Name { get; set; }

    /// <summary>
    /// Gets or sets the preprocessed image data of the image target.
    /// </summary>
    public virtual required byte[] PreprocessImage { get; set; }

    /// <summary>
    /// Gets or sets the postprocessed image data of the image target.
    /// </summary>
    public virtual required byte[] AfterProcessImage { get; set; }

    /// <summary>
    /// Gets or sets the postprocessed image data of the image target with keypoints.
    /// </summary>
    public virtual required byte[] AfterProcessImageWithKeypoints { get; set; }

    /// <summary>
    /// Gets or sets the keypoints of the image target.
    /// </summary>
    public virtual required byte[] Keypoints { get; set; }

    /// <summary>
    /// Gets or sets the descriptors of the image target.
    /// </summary>
    public virtual required byte[] Descriptors { get; set; }

    /// <summary>
    /// Gets or sets the number of rows in the descriptor matrix of the image target.
    /// </summary>
    public virtual required int DescriptorsRows { get; set; }

    /// <summary>
    /// Gets or sets the number of columns in the descriptor matrix of the image target.
    /// </summary>
    public virtual required int DescriptorsCols { get; set; }

    /// <summary>
    /// Gets or sets the width of the image target in X units.
    /// </summary>
    public virtual required float Width { get; set; }

    /// <summary>
    /// Gets or sets the height of the image target in Y units.
    /// </summary>
    public virtual required float Height { get; set; }

    /// <summary>
    /// Gets or sets the number of recognitions of the image target.
    /// </summary>
    public virtual required int Recos { get; set; }

    /// <summary>
    /// Gets or sets the rating of the image target.
    /// </summary>
    public virtual required int Rating { get; set; }

    /// <summary>
    /// Gets or sets the metadata associated with the image target.
    /// </summary>
    public virtual string? Metadata { get; set; }

    /// <summary>
    /// Gets or sets the type of the image target.
    /// </summary>
    public virtual required TargetType Type { get; set; }

    /// <summary>
    /// Gets or sets the active flag of the image target.
    /// </summary>
    public virtual required ActiveFlag ActiveFlag { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the image target was created.
    /// </summary>
    public virtual required DateTimeOffset Created { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the image target was last updated.
    /// </summary>
    public virtual required DateTimeOffset Updated { get; set; }

    /// <summary>
    /// Gets or sets the database to which the image target belongs.
    /// </summary>
    public virtual Database Database { get; set; } = null!;
}