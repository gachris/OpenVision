using MediatR;
using OpenVision.Server.Core.Dtos;

namespace OpenVision.Server.Core.Queries;

/// <summary>
/// Represents a query to retrieve a specific database by its identifier.
/// </summary>
/// <param name="DatabaseId">The unique identifier of the database to retrieve.</param>
public record GetDatabaseByIdQuery(Guid DatabaseId) : IRequest<DatabaseDto?>;