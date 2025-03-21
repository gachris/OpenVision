using OpenVision.Shared;

namespace OpenVision.Server.EntityFramework.Entities;

/// <summary>
/// Represents an image target in the system.
/// </summary>
public class ImageTarget
{
    /// <summary>
    /// Gets or sets the unique identifier of the image target.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the database to which the image target belongs.
    /// </summary>
    public Guid DatabaseId { get; set; }

    /// <summary>
    /// Gets or sets the name of the image target.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the preprocessed image data of the image target.
    /// </summary>
    public required byte[] PreprocessImage { get; set; }

    /// <summary>
    /// Gets or sets the postprocessed image data of the image target.
    /// </summary>
    public required byte[] AfterProcessImage { get; set; }

    /// <summary>
    /// Gets or sets the postprocessed image data of the image target with keypoints.
    /// </summary>
    public required byte[] AfterProcessImageWithKeypoints { get; set; }

    /// <summary>
    /// Gets or sets the keypoints of the image target.
    /// </summary>
    public required byte[] Keypoints { get; set; }

    /// <summary>
    /// Gets or sets the descriptors of the image target.
    /// </summary>
    public required byte[] Descriptors { get; set; }

    /// <summary>
    /// Gets or sets the number of rows in the descriptor matrix of the image target.
    /// </summary>
    public int DescriptorsRows { get; set; }

    /// <summary>
    /// Gets or sets the number of columns in the descriptor matrix of the image target.
    /// </summary>
    public int DescriptorsCols { get; set; }

    /// <summary>
    /// Gets or sets the width of the image target in X units.
    /// </summary>
    public float Width { get; set; }

    /// <summary>
    /// Gets or sets the height of the image target in Y units.
    /// </summary>
    public float Height { get; set; }

    /// <summary>
    /// Gets or sets the number of recognitions of the image target.
    /// </summary>
    public int Recos { get; set; }

    /// <summary>
    /// Gets or sets the rating of the image target.
    /// </summary>
    public int Rating { get; set; }

    /// <summary>
    /// Gets or sets the metadata associated with the image target.
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// Gets or sets the type of the image target.
    /// </summary>
    public TargetType Type { get; set; }

    /// <summary>
    /// Gets or sets the active flag of the image target.
    /// </summary>
    public ActiveFlag ActiveFlag { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the image target was created.
    /// </summary>
    public DateTimeOffset Created { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the image target was last updated.
    /// </summary>
    public DateTimeOffset Updated { get; set; }

    /// <summary>
    /// Gets or sets the database to which the image target belongs.
    /// </summary>
    public required virtual Database Database { get; set; }
}