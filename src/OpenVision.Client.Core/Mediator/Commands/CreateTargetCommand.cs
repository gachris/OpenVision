using MediatR;
using OpenVision.Client.Core.Dtos;
using OpenVision.Shared.Requests;
using OpenVision.Shared.Responses;

namespace OpenVision.Client.Core.Mediator.Commands;

/// <summary>
/// Command to create a new target.
/// </summary>
/// <param name="Request">The details required to create the target.</param>
public record CreateTargetCommand(PostTargetRequest Request)
    : IRequest<ResultDto<TargetResponse>>;
