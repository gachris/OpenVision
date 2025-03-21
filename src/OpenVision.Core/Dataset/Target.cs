using MessagePack;

namespace OpenVision.Core.Dataset;

/// <summary>
/// Represents a target in a dataset.
/// </summary>
[MessagePackObject]
public class Target
{
    #region Properties

    /// <summary>
    /// Gets the ID of the target.
    /// </summary>
    [Key(0)]
    public string Id { get; }

    /// <summary>
    /// Gets the image data of the target.
    /// </summary>
    [Key(1)]
    public byte[] Image { get; }

    /// <summary>
    /// Gets the keypoints of the target.
    /// </summary>
    [Key(2)]
    public byte[] Keypoints { get; }

    /// <summary>
    /// Gets the descriptors of the target.
    /// </summary>
    [Key(3)]
    public byte[] Descriptors { get; }

    /// <summary>
    /// Gets the number of rows in the descriptors.
    /// </summary>
    [Key(4)]
    public int DescriptorsRows { get; }

    /// <summary>
    /// Gets the number of columns in the descriptors.
    /// </summary>
    [Key(5)]
    public int DescriptorsCols { get; }

    /// <summary>
    /// Gets the spatial units along the X-axis.
    /// </summary>
    [Key(6)]
    public float UnitsX { get; }

    /// <summary>
    /// Gets the spatial units along the Y-axis.
    /// </summary>
    [Key(7)]
    public float UnitsY { get; }

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="Target"/> class.
    /// </summary>
    /// <param name="id">The ID of the target.</param>
    /// <param name="image">The image data of the target.</param>
    /// <param name="keypoints">The keypoints of the target.</param>
    /// <param name="descriptors">The descriptors of the target.</param>
    /// <param name="descriptorsRows">The number of rows in the descriptors.</param>
    /// <param name="descriptorsCols">The number of columns in the descriptors.</param>
    /// <param name="unitsX">The spatial units along the X-axis.</param>
    /// <param name="unitsY">The spatial units along the Y-axis.</param>
    public Target(
        string id,
        byte[] image,
        byte[] keypoints,
        byte[] descriptors,
        int descriptorsRows,
        int descriptorsCols,
        float unitsX,
        float unitsY)
    {
        Id = id;
        Image = image;
        Keypoints = keypoints;
        Descriptors = descriptors;
        DescriptorsRows = descriptorsRows;
        DescriptorsCols = descriptorsCols;
        UnitsX = unitsX;
        UnitsY = unitsY;
    }
}
