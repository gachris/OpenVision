namespace OpenVision.Server.Core.Dtos;

/// <summary>
/// Represents the data used to update a database, with each field being optional.
/// </summary>
public class UpdateDatabaseDto
{
    /// <summary>
    /// Gets or sets the name for the database.
    /// </summary>
    public virtual string? Name { get; set; }
}