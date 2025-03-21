using System.Text.Json.Serialization;

namespace OpenVision.Shared.Requests;

/// <summary>
/// Represents a request to update an existing database.
/// </summary>
public class UpdateDatabaseRequest
{
    /// <summary>
    /// Gets or sets the new name for the database.
    /// </summary>
    [JsonPropertyName("name")]
    public virtual string? Name { get; set; }
}