using MediatR;
using OpenVision.Server.Core.Dtos;

namespace OpenVision.Server.Core.Commands;

/// <summary>
/// Represents a command to update an existing target.
/// </summary>
/// <param name="TargetId">The unique identifier of the target to update.</param>
/// <param name="UpdateTargetDto">The DTO containing the updated details of the target.</param>
public record UpdateTargetCommand(Guid TargetId, UpdateTargetDto UpdateTargetDto) : IRequest<TargetDto>;