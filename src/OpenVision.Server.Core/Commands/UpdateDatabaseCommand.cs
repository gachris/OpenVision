using MediatR;
using OpenVision.Server.Core.Dtos;

namespace OpenVision.Server.Core.Commands;

/// <summary>
/// Represents a command to update an existing database.
/// </summary>
/// <param name="DatabaseId">The unique identifier of the database to update.</param>
/// <param name="UpdateDatabaseDto">The data transfer object containing the updated database details.</param>
public record UpdateDatabaseCommand(Guid DatabaseId, UpdateDatabaseDto UpdateDatabaseDto) : IRequest<DatabaseDto>;