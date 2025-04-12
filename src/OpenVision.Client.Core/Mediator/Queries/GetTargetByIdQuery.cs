using MediatR;
using OpenVision.Client.Core.Dtos;
using OpenVision.Shared.Responses;

namespace OpenVision.Client.Core.Mediator.Queries;

/// <summary>
/// Query to retrieve the details of a target by its unique identifier.
/// </summary>
/// <param name="TargetId">The unique identifier of the target.</param>
public record GetTargetByIdQuery(Guid TargetId) : IRequest<ResultDto<TargetResponse>>;
