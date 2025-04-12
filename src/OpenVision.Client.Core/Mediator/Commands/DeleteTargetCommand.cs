using MediatR;
using OpenVision.Client.Core.Dtos;

namespace OpenVision.Client.Core.Mediator.Commands;

/// <summary>
/// Command to delete a target.
/// </summary>
/// <param name="TargetId">The unique identifier of the target to delete.</param>
public record DeleteTargetCommand(Guid TargetId)
    : IRequest<ResultDto<bool>>;
