using MediatR;
using OpenVision.Server.Core.Dtos;

namespace OpenVision.Server.Core.Commands;

/// <summary>
/// Represents a command to create a new target.
/// </summary>
/// <param name="CreateTargetDto">The DTO containing the details required to create the target.</param>
public record CreateTargetCommand(CreateTargetDto CreateTargetDto) : IRequest<TargetDto>;