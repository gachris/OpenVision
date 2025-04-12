using MediatR;
using OpenVision.Client.Core.Dtos;
using OpenVision.Shared.Requests;
using OpenVision.Shared.Responses;

namespace OpenVision.Client.Core.Mediator.Queries;

/// <summary>
/// Query to retrieve a paged list of databases.
/// </summary>
/// <param name="Query">The browser query specifying page, size, and filter options.</param>
public record GetDatabasesQuery(DatabaseBrowserQuery Query) : IRequest<ResultDto<IPagedResponse<IEnumerable<DatabaseResponse>>>>;
