using MediatR;
using OpenVision.Server.Core.Dtos;

namespace OpenVision.Server.Core.Queries;

/// <summary>
/// Represents a query to retrieve all targets.
/// </summary>
public record GetQueryableDatabaseQuery() : IRequest<IQueryable<DatabaseDto>>;