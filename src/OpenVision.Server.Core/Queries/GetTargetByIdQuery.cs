using MediatR;
using OpenVision.Server.Core.Dtos;

namespace OpenVision.Server.Core.Queries;

/// <summary>
/// Represents a query to retrieve a specific target by its identifier.
/// </summary>
/// <param name="TargetId">The unique identifier of the target to retrieve.</param>
public record GetTargetByIdQuery(Guid TargetId) : IRequest<TargetDto?>;
