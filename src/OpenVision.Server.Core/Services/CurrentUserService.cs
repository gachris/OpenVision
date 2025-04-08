using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenVision.Server.Core.Contracts;

namespace OpenVision.Server.Core.Services;

/// <summary>
/// Default implementation of <see cref="ICurrentUserService"/>.
/// This implementation retrieves the current user ID from the current HTTP context.
/// If the user identifier is not found, an exception is thrown.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    #region Fields/Consts

    private readonly ILogger<CurrentUserService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    #endregion

    #region Properties

    /// <inheritdoc/>
    public string UserId => GetUserIdOrThrow();

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentUserService"/> class.
    /// </summary>
    /// <param name="logger">The logger instance used for logging information and warnings.</param>
    /// <param name="httpContextAccessor">The HTTP context accessor used to access the current HTTP context.</param>
    public CurrentUserService(ILogger<CurrentUserService> logger, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    #region Methods

    /// <summary>
    /// Retrieves the user identifier from the current HTTP context or throws an exception if not found.
    /// </summary>
    /// <returns>The user identifier.</returns>
    protected string GetUserIdOrThrow()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("User identifier is null or empty.");
            throw new ArgumentException("User identifier is null or empty.", nameof(userId));
        }
        return userId;
    }

    #endregion
}
