using Microsoft.AspNetCore.WebUtilities;
using OpenVision.Web.Core.Filters;

namespace OpenVision.Server.Core.Services;

/// <summary>
/// Service for creating URIs for pagination and details endpoints.
/// </summary>
public class UriService : IUriService
{
    #region Fields/Consts

    private readonly string _baseUri;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="UriService"/> class.
    /// </summary>
    /// <param name="baseUri">The base URI for the service.</param>
    public UriService(string baseUri)
    {
        _baseUri = baseUri;
    }

    #region IUriService Implementation

    /// <inheritdoc/>
    public Uri GetPageUri(PaginationFilter filter, string route)
    {
        var enpointUri = new Uri(string.Concat(_baseUri, route));
        var modifiedUri = QueryHelpers.AddQueryString(enpointUri.ToString(), "page", filter.Page.ToString());
        modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "size", filter.Size.ToString());
        return new Uri(modifiedUri);
    }

    /// <inheritdoc/>
    public Uri GetDetailsUri(Guid id, string route)
    {
        return new Uri(string.Concat(_baseUri, route, id.ToString()));
    }

    #endregion
}