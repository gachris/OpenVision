using MediatR;
using OpenVision.Client.Core.Dtos;
using OpenVision.Shared.Responses;

namespace OpenVision.Client.Core.Mediator.Commands;

/// <summary>
/// Command to activate a target.
/// </summary>
/// <param name="TargetId">The unique identifier of the target to activate.</param>
public record ActivateTargetCommand(Guid TargetId)
    : IRequest<ResultDto<TargetResponse>>;
