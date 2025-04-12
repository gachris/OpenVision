using MediatR;
using OpenVision.Client.Core.Dtos;

namespace OpenVision.Client.Core.Mediator.Commands;

/// <summary>
/// Command to delete a database.
/// </summary>
/// <param name="DatabaseId">The unique identifier of the database to delete.</param>
public record DeleteDatabaseCommand(Guid DatabaseId) : IRequest<ResultDto<bool>>;
