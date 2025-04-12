using MediatR;
using OpenVision.Client.Core.Dtos;
using OpenVision.Shared.Requests;
using OpenVision.Shared.Responses;

namespace OpenVision.Client.Core.Mediator.Commands;

/// <summary>
/// Command to update an existing target.
/// </summary>
/// <param name="TargetId">The unique identifier of the target.</param>
/// <param name="Request">The update details for the target.</param>
public record UpdateTargetCommand(Guid TargetId, UpdateTargetRequest Request)
    : IRequest<ResultDto<TargetResponse>>;
