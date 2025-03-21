using MessagePack;

namespace OpenVision.Core.Dataset;

/// <summary>
/// Handles serialization and deserialization of Target objects using MessagePack.
/// </summary>
public class DatasetSerializer
{
    /// <summary>
    /// Serializes a collection of Target objects to a file asynchronously.
    /// </summary>
    /// <param name="filename">The path of the file to serialize the data to.</param>
    /// <param name="targets">The collection of Target objects to serialize.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task SerializeAsync(string filename, IEnumerable<Target> targets, CancellationToken cancellationToken = default)
    {
        var serializedData = MessagePackSerializer.Serialize(targets, cancellationToken: cancellationToken);
        await File.WriteAllBytesAsync(filename, serializedData, cancellationToken);
    }

    /// <summary>
    /// Serializes a collection of Target objects to a stream asynchronously.
    /// </summary>
    /// <param name="stream">The stream to serialize the data to.</param>
    /// <param name="targets">The collection of Target objects to serialize.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task SerializeAsync(Stream stream, IEnumerable<Target> targets, CancellationToken cancellationToken = default)
    {
        await MessagePackSerializer.SerializeAsync(stream, targets, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Deserializes a collection of Target objects from a file asynchronously.
    /// </summary>
    /// <param name="filename">The path of the file to deserialize the data from.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, containing the deserialized TargetDataset.</returns>
    public static async Task<TargetDataset> DeserializeAsync(string filename, CancellationToken cancellationToken = default)
    {
        var serializedData = await File.ReadAllBytesAsync(filename, cancellationToken);
        var targets = MessagePackSerializer.Deserialize<IReadOnlyCollection<Target>>(serializedData, cancellationToken: cancellationToken);
        return new TargetDataset(targets);
    }

    /// <summary>
    /// Deserializes a collection of Target objects from a stream asynchronously.
    /// </summary>
    /// <param name="stream">The stream to deserialize the data from.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, containing the deserialized TargetDataset.</returns>
    public static async Task<TargetDataset> DeserializeAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        var targets = await MessagePackSerializer.DeserializeAsync<IReadOnlyCollection<Target>>(stream, cancellationToken: cancellationToken);
        return new TargetDataset(targets);
    }
}