using OpenVision.Shared;

namespace OpenVision.Server.Core.Dtos;

/// <summary>
/// Represents the data required to create a new database.
/// </summary>
public class CreateDatabaseDto
{
    /// <summary>
    /// Gets or sets the name of the database.
    /// </summary>
    public required virtual string Name { get; set; }

    /// <summary>
    /// Gets or sets the type of the database.
    /// </summary>
    public required virtual DatabaseType Type { get; set; }
}