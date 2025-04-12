using MediatR;
using OpenVision.Client.Core.Dtos;
using OpenVision.Shared.Responses;

namespace OpenVision.Client.Core.Mediator.Queries;

/// <summary>
/// Query to retrieve a single database by its ID.
/// </summary>
/// <param name="DatabaseId">The unique identifier of the database.</param>
public record GetDatabaseByIdQuery(Guid DatabaseId) : IRequest<ResultDto<DatabaseResponse>>;