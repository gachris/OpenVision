namespace OpenVision.Shared.Requests;

/// <summary>
/// Represents a request to update an existing database.
/// </summary>
public record UpdateDatabaseRequest
{
    /// <summary>
    /// Gets or sets the new name for the database.
    /// </summary>
    public virtual string? Name { get; init; }
}