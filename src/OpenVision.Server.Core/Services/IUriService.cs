using OpenVision.Web.Core.Filters;

namespace OpenVision.Server.Core.Services;

/// <summary>
/// Interface for a service that provides URI generation for pagination and details endpoints.
/// </summary>
public interface IUriService
{
    /// <summary>
    /// Generates a URI for accessing details of a specific entity by ID.
    /// </summary>
    /// <param name="id">The ID of the entity.</param>
    /// <param name="route">The route to append to the base URI.</param>
    /// <returns>A URI for accessing the entity details.</returns>
    public Uri GetDetailsUri(Guid id, string route);

    /// <summary>
    /// Generates a URI for pagination based on the given filter and route.
    /// </summary>
    /// <param name="filter">The pagination filter containing page number and page size.</param>
    /// <param name="route">The route to append to the base URI.</param>
    /// <returns>A URI for the specified pagination parameters.</returns>
    public Uri GetPageUri(PaginationFilter filter, string route);
}
