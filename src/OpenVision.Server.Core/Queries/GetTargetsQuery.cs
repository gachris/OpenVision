using MediatR;
using OpenVision.Server.Core.Dtos;

namespace OpenVision.Server.Core.Queries;

/// <summary>
/// Represents a query to retrieve all image targets.
/// </summary>
public record GetTargetsQuery() : IRequest<IQueryable<TargetDto>>;
