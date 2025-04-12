using MediatR;
using OpenVision.Client.Core.Dtos;
using OpenVision.Shared.Requests;
using OpenVision.Shared.Responses;

namespace OpenVision.Client.Core.Mediator.Queries;

/// <summary>
/// Query to retrieve a paginated list of targets using filtering parameters.
/// </summary>
/// <param name="Query">The browser query specifying pagination and filtering options.</param>
public record GetTargetsQuery(TargetBrowserQuery Query) : IRequest<ResultDto<IPagedResponse<IEnumerable<TargetResponse>>>>;
