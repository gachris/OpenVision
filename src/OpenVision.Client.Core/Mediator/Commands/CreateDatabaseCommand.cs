using MediatR;
using OpenVision.Client.Core.Dtos;
using OpenVision.Shared.Requests;
using OpenVision.Shared.Responses;

namespace OpenVision.Client.Core.Mediator.Commands;

/// <summary>
/// Command to create a new database.
/// </summary>
/// <param name="Request">The details required to create the database.</param>
public record CreateDatabaseCommand(PostDatabaseRequest Request) : IRequest<ResultDto<DatabaseResponse>>;