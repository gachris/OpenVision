using MediatR;
using OpenVision.Server.Core.Dtos;

namespace OpenVision.Server.Core.Commands;

/// <summary>
/// Represents a command to create a new database.
/// </summary>
/// <param name="CreateDatabaseDto">The data transfer object containing the details needed to create the database.</param>
public record CreateDatabaseCommand(CreateDatabaseDto CreateDatabaseDto) : IRequest<DatabaseDto>;