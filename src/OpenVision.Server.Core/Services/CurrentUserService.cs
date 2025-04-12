using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using OpenVision.Server.Core.Auth;
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

    private readonly IHttpContextAccessor _httpContextAccessor;

    #endregion

    #region Properties

    /// <inheritdoc/>
    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    /// <inheritdoc/>
    public string? ApiKey => _httpContextAccessor.HttpContext?.User?.FindFirst(ApiKeyDefaults.X_API_KEY)?.Value;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentUserService"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor used to access the current HTTP context.</param>
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
}
