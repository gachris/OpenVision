using MediatR;
using OpenVision.Server.Core.Dtos;

namespace OpenVision.Server.Core.Queries;

/// <summary>
/// Represents a query to retrieve all databases.
/// </summary>
public record GetDatabasesQuery() : IRequest<IEnumerable<DatabaseDto>>;