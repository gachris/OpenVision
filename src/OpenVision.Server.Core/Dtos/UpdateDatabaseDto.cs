namespace OpenVision.Server.Core.Dtos;

/// <summary>
/// Represents the data used to update a database, with each field being optional.
/// </summary>
public record UpdateDatabaseDto
{
    /// <summary>
    /// Gets the name for the database.
    /// </summary>
    public virtual string? Name { get; init; }
}