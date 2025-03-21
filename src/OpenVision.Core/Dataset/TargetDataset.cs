namespace OpenVision.Core.Dataset;

/// <summary>
/// Represents a dataset of targets to be matched against images.
/// </summary>
public sealed class TargetDataset
{
    #region Properties

    /// <summary>
    /// Gets the read-only collection of targets in the dataset.
    /// </summary>
    public IReadOnlyCollection<Target> Targets { get; }

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="TargetDataset"/> class with the specified targets.
    /// </summary>
    /// <param name="targets">The targets to include in the dataset.</param>
    internal TargetDataset(IReadOnlyCollection<Target> targets)
    {
        Targets = targets;
    }

    #region Methods

    /// <summary>
    /// Loads a <see cref="TargetDataset"/> from a stream asynchronously.
    /// </summary>
    /// <param name="stream">The target dataset stream.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the loaded <see cref="TargetDataset"/>.</returns>
    public static async Task<TargetDataset> LoadAsync(Stream stream)
    {
        return await DatasetSerializer.DeserializeAsync(stream);
    }

    /// <summary>
    /// Loads a <see cref="TargetDataset"/> from a file asynchronously.
    /// </summary>
    /// <param name="filename">The path to the file to load.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the loaded <see cref="TargetDataset"/>.</returns>
    public static async Task<TargetDataset> LoadAsync(string filename)
    {
        return await DatasetSerializer.DeserializeAsync(filename);
    }

    #endregion
}
