using OpenVision.Shared.Types;

namespace OpenVision.Server.Core.Dtos;

/// <summary>
/// Represents the data required to create a new database.
/// </summary>
public record CreateDatabaseDto
{
    /// <summary>
    /// Gets the name of the database.
    /// </summary>
    public required virtual string Name { get; init; }

    /// <summary>
    /// Gets the type of the database.
    /// </summary>
    public required virtual DatabaseType Type { get; init; }
}