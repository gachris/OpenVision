using MediatR;
using OpenVision.Client.Core.Dtos;
using OpenVision.Shared.Responses;

namespace OpenVision.Client.Core.Mediator.Commands;

/// <summary>
/// Command to deactivate a target.
/// </summary>
/// <param name="TargetId">The unique identifier of the target to deactivate.</param>
public record DeactivateTargetCommand(Guid TargetId)
    : IRequest<ResultDto<TargetResponse>>;
