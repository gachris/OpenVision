using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OpenVision.Shared.Requests;

/// <summary>
/// Represents a request to create a new database.
/// </summary>
public class PostDatabaseRequest
{
    /// <summary>
    /// Gets or sets the name of the database.
    /// </summary>
    [JsonPropertyName("name")]
    [Required(ErrorMessage = "Database Name is required.")]
    public virtual string? Name { get; set; }

    /// <summary>
    /// Gets or sets the type of the database.
    /// </summary>
    [JsonPropertyName("type")]
    [Required(ErrorMessage = "Database Type is required.")]
    public virtual DatabaseType? Type { get; set; }
}