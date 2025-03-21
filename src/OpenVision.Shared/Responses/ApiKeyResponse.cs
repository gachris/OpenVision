using System.Text.Json.Serialization;

namespace OpenVision.Shared.Responses;

/// <summary>
/// Represents an API key response.
/// </summary>
public class ApiKeyResponse
{
    /// <summary>
    /// Gets the unique identifier of the API key.
    /// </summary>
    [JsonPropertyName("id")]
    public virtual Guid Id { get; }

    /// <summary>
    /// Gets the API key string.
    /// </summary>
    [JsonPropertyName("key")]
    public virtual string Key { get; }

    /// <summary>
    /// Gets the type of the API key (client or server).
    /// </summary>
    [JsonPropertyName("type")]
    public virtual ApiKeyType Type { get; }

    /// <summary>
    /// Gets the date and time that the API key was last updated.
    /// </summary>
    [JsonPropertyName("update")]
    public virtual DateTimeOffset Updated { get; }

    /// <summary>
    /// Gets the date and time that the API key was created.
    /// </summary>
    [JsonPropertyName("created")]
    public virtual DateTimeOffset Created { get; }

    /// <summary>
    /// Initializes a new instance of the ApiKeyResponse class with the specified ID, key, type, updated, and created.
    /// </summary>
    /// <param name="id">The unique identifier of the API key.</param>
    /// <param name="key">The API key string.</param>
    /// <param name="type">The type of the API key (client or server).</param>
    /// <param name="updated">The date and time that the API key was last updated.</param>
    /// <param name="created">The date and time that the API key was created.</param>
    public ApiKeyResponse(Guid id,
                          string key,
                          ApiKeyType type,
                          DateTimeOffset updated,
                          DateTimeOffset created)
    {
        Id = id;
        Key = key;
        Type = type;
        Updated = updated;
        Created = created;
    }
}
