using MediatR;
using OpenVision.Client.Core.Dtos;
using OpenVision.Shared.Requests;
using OpenVision.Shared.Responses;

namespace OpenVision.Client.Core.Mediator.Commands;

/// <summary>
/// Command to update an existing database.
/// </summary>
/// <param name="DatabaseId">The unique identifier of the database.</param>
/// <param name="Request">The update details for the database.</param>
public record UpdateDatabaseCommand(Guid DatabaseId, UpdateDatabaseRequest Request) : IRequest<ResultDto<DatabaseResponse>>;
