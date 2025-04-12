using System.ComponentModel.DataAnnotations;
using OpenVision.Shared.Types;

namespace OpenVision.Shared.Requests;

/// <summary>
/// Represents a request to create a new database.
/// </summary>
public record PostDatabaseRequest
{
    /// <summary>
    /// Gets or sets the name of the database.
    /// </summary>
    [Required(ErrorMessage = "Database Name is required.")]
    public virtual required string Name { get; init; }

    /// <summary>
    /// Gets or sets the type of the database.
    /// </summary>
    [Required(ErrorMessage = "Database Type is required.")]
    public virtual required DatabaseType Type { get; init; }
}