using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenVision.Client.Core.Contracts;

namespace OpenVision.Client.Core.Services;

/// <summary>
/// Retrieves the current access token from the HTTP context.
/// </summary>
public class AccessTokenProvider : IAccessTokenProvider
{
    #region Fields/Consts

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AccessTokenProvider> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="AccessTokenProvider"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor used for retrieving the token.</param>
    /// <param name="logger">The logger instance.</param>
    public AccessTokenProvider(IHttpContextAccessor httpContextAccessor, ILogger<AccessTokenProvider> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    #region Methods

    /// <inheritdoc/>
    public async Task<string?> GetAccessTokenAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            _logger.LogWarning("HttpContext is null. Unable to retrieve access token.");
            return null;
        }

        // Retrieve the access token from the HTTP context.
        var token = await httpContext.GetTokenAsync("access_token");
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Access token was not found in the current HTTP context.");
        }
        else
        {
            _logger.LogDebug("Access token successfully retrieved.");
        }

        return token;
    }

    #endregion
}
