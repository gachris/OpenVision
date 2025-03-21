namespace OpenVision.Api.Core.Types;

/// <summary>
/// Represents a database API key.
/// </summary>
public class DatabaseApiKey
{
    /// <summary>
    /// Gets the key value.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseApiKey"/> class.
    /// </summary>
    /// <param name="key">The key value.</param>
    public DatabaseApiKey(string key)
    {
        Key = key;
    }
}