using MediatR;

namespace OpenVision.Server.Core.Commands;

/// <summary>
/// Represents a command to delete a target.
/// </summary>
/// <param name="TargetId">The unique identifier of the target to delete.</param>
/// <returns>A boolean value indicating whether the deletion was successful.</returns>
public record DeleteTargetCommand(Guid TargetId) : IRequest<bool>;