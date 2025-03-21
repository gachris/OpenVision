using System.Text.Json.Serialization;

namespace OpenVision.Shared.Responses;

/// <summary>
/// Represents a database response.
/// </summary>
public class DatabaseResponse
{
    /// <summary>
    /// Gets the unique identifier of the database.
    /// </summary>
    [JsonPropertyName("id")]
    public virtual Guid Id { get; }

    /// <summary>
    /// Gets the name of the database.
    /// </summary>
    [JsonPropertyName("name")]
    public virtual string Name { get; }

    /// <summary>
    /// Gets the type of the database (device or cloud).
    /// </summary>
    [JsonPropertyName("type")]
    public virtual DatabaseType Type { get; }

    /// <summary>
    /// Gets the collection of API keys associated with the database.
    /// </summary>
    [JsonPropertyName("api_keys")]
    public virtual IEnumerable<ApiKeyResponse> ApiKeys { get; }

    /// <summary>
    /// Gets the collection of targets associated with the database.
    /// </summary>
    [JsonPropertyName("targets")]
    public virtual IEnumerable<TargetResponse> Targets { get; }

    /// <summary>
    /// Gets the date and time that the database was created.
    /// </summary>
    [JsonPropertyName("created")]
    public virtual DateTimeOffset Created { get; }

    /// <summary>
    /// Gets the date and time that the database was last updated.
    /// </summary>
    [JsonPropertyName("updated")]
    public virtual DateTimeOffset Updated { get; }

    /// <summary>
    /// Initializes a new instance of the DatabaseResponse class with the specified ID, name, type, API keys, targets, created, and updated.
    /// </summary>
    /// <param name="id">The unique identifier of the database.</param>
    /// <param name="name">The name of the database.</param>
    /// <param name="type">The type of the database (device or cloud).</param>
    /// <param name="apiKeys">The collection of API keys associated with the database.</param>
    /// <param name="targets">The collection of targets associated with the database.</param>
    /// <param name="created">The date and time that the database was created.</param>
    /// <param name="updated">The date and time that the database was last updated.</param>
    public DatabaseResponse(Guid id,
                            string name,
                            DatabaseType type,
                            IEnumerable<ApiKeyResponse> apiKeys,
                            IEnumerable<TargetResponse> targets,
                            DateTimeOffset created,
                            DateTimeOffset updated)
    {
        Id = id;
        Name = name;
        Type = type;
        ApiKeys = apiKeys;
        Targets = targets;
        Created = created;
        Updated = updated;
    }
}