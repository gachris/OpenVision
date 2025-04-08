using MediatR;
using OpenVision.Server.Core.Dtos;

namespace OpenVision.Server.Core.Commands;

/// <summary>
/// Represents a command to retrieve a file download package associated with a specific database.
/// </summary>
/// <param name="DatabaseId">
/// The unique identifier of the database from which the file will be downloaded.
/// </param>
public record GetDatabaseFileCommand(Guid DatabaseId) : IRequest<DatabaseFileDto>;
